using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class ActiveCampaignIntegrationTests
    {
        static LevelData Level(int n)=>ChapterLevelCatalog.Find(1,n);
        static BoardModel Start(LevelData l,int attempt)=>LevelBoardFactory.Create(l,BoardSeedUtility.CreateRuntimeSeed(1,l.levelId,l.boardSeed+attempt),rules:Match3Board.CurrentGameplayRules);
        [Test] public void RuntimeResourcesMatchCandidateAndBothLanguagesValidate()
        {
            var c=ActiveCampaignContent.Load(true);var levels=ChapterLevelCatalog.LoadChapter1(true);
            Assert.That(levels.levels.Length,Is.EqualTo(200));
            foreach(var name in new[]{"levels","story","story_ru","story_en"})
                Assert.That(Resources.Load<TextAsset>(ActiveCampaignContent.ResourceRoot+name).text,Is.EqualTo(File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01EditorialR42",name+".json"))));
            var previous=JsonUtility.FromJson<ChapterLevelCatalogData>(File.ReadAllText(Path.Combine(Application.dataPath,"LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R32/levels.json")));
            for(int i=0;i<190;i++)Assert.That(JsonUtility.ToJson(levels.levels[i]),Is.EqualTo(JsonUtility.ToJson(previous.levels[i])));
            Assert.That(ChapterLevelCatalog.Next(Level(190)).levelId,Is.EqualTo(191));
            Assert.That(c.LoadStory(1).milestones.Single(m=>m.afterLevelId==200).sceneId,Is.EqualTo("campaign.chapter01.orchard_field_paths"));
            Assert.That(levels.levels.Skip(190).Sum(l=>l.rewards.gold),Is.EqualTo(1840));
            foreach(var language in new[]{"ru","en"})Assert.That(StoryCatalog.ValidateCampaign(c.LoadStory(1),c.LoadLocalization(1,language),levels,false).Errors,Is.Empty);
            Assert.That(StoryCatalog.LoadChapter1(true).completion?.finalLevelId??0,Is.Zero);
            Assert.That(ChapterLevelCatalog.Next(Level(10)).levelId,Is.EqualTo(11));
            Assert.That(ChapterLevelCatalog.Next(Level(200)),Is.Null);Assert.That(ChapterLevelCatalog.Find(2,1),Is.Null);
            foreach(var l in levels.levels)for(int a=1;a<=3;a++){var m=Start(l,a);Assert.That(m.HasAnyMatches(),Is.False,l.id);Assert.That(m.HasLegalMove(),Is.True,l.id);Assert.That(m.ComputeStateHash(),Is.EqualTo(Start(l,a).ComputeStateHash()));}
        }
        sealed class Node {public BoardSnapshot Board;public LevelGoalRuntimeSnapshot Goals;public int Score;public float Cost;public List<MechanicsReplayCommand> Path=new List<MechanicsReplayCommand>();}
        [Serializable] sealed class Witness {public int round,attempt,moveLimit,moves,expanded;public string initialHash,finalHash;public MechanicsReplayCommand[] commands;}
        static BoardModel Restore(BoardSnapshot s){var m=new BoardModel(s.Width,s.Height,s.TileTypeCount,new SeededBoardRandom(1)){SquareMatches=true,StandaloneSpecials=true};m.RestoreSnapshot(s);return m;}
        static IEnumerable<MechanicsReplayCommand> Actions(BoardSnapshot s)
        {
            var m=Restore(s);var token=s.ToReplayToken();
            for(int y=0;y<m.Height;y++)for(int x=0;x<m.Width;x++){
                var p=new BoardPosition(x,y);
                if(m.GetSpecial(p)!=SpecialGemKind.None)yield return new MechanicsReplayCommand{Rules=Match3Board.CurrentGameplayRules,BeforeToken=token,Tap=true,X=x,Y=y};
                foreach(var q in new[]{new BoardPosition(x+1,y),new BoardPosition(x,y+1)})
                    if(m.IsInside(q)&&m.CanSwap(p,q)&&(m.GetSpecial(p)!=SpecialGemKind.None||m.GetSpecial(q)!=SpecialGemKind.None||m.SwapCreatesMatch(p,q)))
                        yield return new MechanicsReplayCommand{Rules=Match3Board.CurrentGameplayRules,BeforeToken=token,X=x,Y=y,ToX=q.X,ToY=q.Y};
            }
        }
        static Witness Solve(LevelData l,int attempt)
        {
            var start=Start(l,attempt).CreateSnapshot();var nodes=new List<Node>{new Node{Board=start,Goals=new LevelGoalRuntime(l,0).CaptureSnapshot()}};int expanded=0;
            for(int depth=0;depth<l.moveLimit;depth++){
                var next=new List<Node>();var seen=new HashSet<string>();
                foreach(var n in nodes)foreach(var action in Actions(n.Board)){
                    Assert.That(++expanded,Is.LessThan(14000),"Search cap "+l.id);var p=action.Resolve();if(!p.IsResolved||!p.ConsumesMove)continue;
                    var g=new LevelGoalRuntime(l,0);g.RestoreSnapshot(n.Goals);int score=n.Score;foreach(var w in p.Waves){score+=w.ScoreDelta;g.ApplyWave(w,score);}g.RecordConsumedMove();
                    action.ExpectedPlanHash=p.PlanHash;var path=new List<MechanicsReplayCommand>(n.Path){action};
                    if(g.AllComplete)return new Witness{round=l.levelId,attempt=attempt,moveLimit=l.moveLimit,moves=path.Count,expanded=expanded,initialHash=start.StateHash,finalHash=p.FinalSnapshot.StateHash,commands=path.ToArray()};
                    var key=p.FinalSnapshot.StateHash+":"+string.Join(",",Enumerable.Range(0,g.SlotCount).Select(i=>g.RemainingAt(i)));if(!seen.Add(key))continue;
                    float cost=Enumerable.Range(0,g.SlotCount).Sum(i=>1000f*g.RemainingAt(i)/l.goals[i].target)+p.FinalSnapshot.ObstacleLayers.Sum(x=>(int)x)*5-p.FinalSnapshot.Specials.Count(x=>x!=SpecialGemKind.None)*5;
                    next.Add(new Node{Board=p.FinalSnapshot,Goals=g.CaptureSnapshot(),Score=score,Path=path,Cost=cost});
                }
                nodes=next.OrderBy(n=>n.Cost).ThenBy(n=>n.Board.StateHash,StringComparer.Ordinal).Take(8).ToList();
            }
            Assert.Fail("No inventory-free witness: "+l.id+" attempt "+attempt);return null;
        }
        static string Output {get{string p=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR")??Path.Combine(Application.temporaryCachePath,"CampaignSectorCandidate");Directory.CreateDirectory(p);return p;}}
        [TestCase(1)][TestCase(2)][TestCase(3)][TestCase(4)][TestCase(5)][TestCase(6)][TestCase(7)][TestCase(8)][TestCase(9)][TestCase(10)][TestCase(11)][TestCase(12)][TestCase(13)][TestCase(14)][TestCase(15)][TestCase(16)][TestCase(17)][TestCase(18)][TestCase(19)][TestCase(20)][TestCase(21)][TestCase(22)][TestCase(23)][TestCase(24)][TestCase(25)][TestCase(26)][TestCase(27)][TestCase(28)][TestCase(29)][TestCase(30)][TestCase(31)][TestCase(32)][TestCase(33)][TestCase(34)][TestCase(35)][TestCase(36)][TestCase(37)][TestCase(38)][TestCase(39)][TestCase(40)][TestCase(41)][TestCase(42)][TestCase(43)][TestCase(44)][TestCase(45)][TestCase(46)][TestCase(47)][TestCase(48)][TestCase(49)][TestCase(50)][TestCase(51)][TestCase(52)][TestCase(53)][TestCase(54)][TestCase(55)][TestCase(56)][TestCase(57)][TestCase(58)][TestCase(59)][TestCase(60)][TestCase(61)][TestCase(62)][TestCase(63)][TestCase(64)][TestCase(65)][TestCase(66)][TestCase(67)][TestCase(68)][TestCase(69)][TestCase(70)][TestCase(71)][TestCase(72)][TestCase(73)][TestCase(74)][TestCase(75)][TestCase(76)][TestCase(77)][TestCase(78)][TestCase(79)][TestCase(80)][TestCase(81)][TestCase(82)][TestCase(83)][TestCase(84)][TestCase(85)][TestCase(86)][TestCase(87)][TestCase(88)][TestCase(89)][TestCase(90)][TestCase(91)][TestCase(92)][TestCase(93)][TestCase(94)][TestCase(95)][TestCase(96)][TestCase(97)][TestCase(98)][TestCase(99)][TestCase(100)][TestCase(101)][TestCase(102)][TestCase(103)][TestCase(104)][TestCase(105)][TestCase(106)][TestCase(107)][TestCase(108)][TestCase(109)][TestCase(110)][TestCase(111)][TestCase(112)][TestCase(113)][TestCase(114)][TestCase(115)][TestCase(116)][TestCase(117)][TestCase(118)][TestCase(119)][TestCase(120)][TestCase(121)][TestCase(122)][TestCase(123)][TestCase(124)][TestCase(125)][TestCase(126)][TestCase(127)][TestCase(128)][TestCase(129)][TestCase(130)][TestCase(131)][TestCase(132)][TestCase(133)][TestCase(134)][TestCase(135)][TestCase(136)][TestCase(137)][TestCase(138)][TestCase(139)][TestCase(140)][TestCase(141)][TestCase(142)][TestCase(143)][TestCase(144)][TestCase(145)][TestCase(146)][TestCase(147)][TestCase(148)][TestCase(149)][TestCase(150)][TestCase(151)][TestCase(152)][TestCase(153)][TestCase(154)][TestCase(155)][TestCase(156)][TestCase(157)][TestCase(158)][TestCase(159)][TestCase(160)][TestCase(161)][TestCase(162)][TestCase(163)][TestCase(164)][TestCase(165)][TestCase(166)][TestCase(167)][TestCase(168)][TestCase(169)][TestCase(170)][TestCase(171)][TestCase(172)][TestCase(173)][TestCase(174)][TestCase(175)][TestCase(176)][TestCase(177)][TestCase(178)][TestCase(179)][TestCase(180)][TestCase(181)][TestCase(182)][TestCase(183)][TestCase(184)][TestCase(185)][TestCase(186)][TestCase(187)][TestCase(188)][TestCase(189)][TestCase(190)][TestCase(191)][TestCase(192)][TestCase(193)][TestCase(194)][TestCase(195)][TestCase(196)][TestCase(197)][TestCase(198)][TestCase(199)][TestCase(200)]
        public void ThreeAttemptSeedsHaveInventoryFreeReplayWitnesses(int round)
        {
            var l=Level(round);for(int attempt=1;attempt<=3;attempt++){
                var w=Solve(l,attempt);var m=Start(l,attempt);var goals=new LevelGoalRuntime(l,0);int score=0;
                foreach(var a in w.commands){Assert.That(a.BeforeToken,Is.EqualTo(m.CreateSnapshot().ToReplayToken()));var p=a.Resolve();Assert.That(p.IsResolved&&p.ConsumesMove,Is.True);foreach(var wave in p.Waves){score+=wave.ScoreDelta;goals.ApplyWave(wave,score);}goals.RecordConsumedMove();m.RestoreSnapshot(p.FinalSnapshot);}
                Assert.That(goals.AllComplete,Is.True);Assert.That(m.ComputeStateHash(),Is.EqualTo(w.finalHash));Assert.That(w.moves,Is.LessThanOrEqualTo(l.moveLimit));
                File.WriteAllText(Path.Combine(Output,$"Round{round}_Attempt{attempt}_Witness.json"),JsonUtility.ToJson(w,true));
            }
        }

        static void Isolation()
        {
            var root=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(root,Is.Not.Null.And.Not.Empty);
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(root)+Path.DirectorySeparatorChar,StringComparison.Ordinal),Is.True);
        }
        [UnityTest] public IEnumerator ExistingTenRoundSaveKeepsBalancesAndUnlocksEleven() { yield return SaveMigration(10); }
        [UnityTest] public IEnumerator ExistingFortyRoundSaveKeepsBalancesAndUnlocksFortyOne() { yield return SaveMigration(40); }
        [UnityTest] public IEnumerator ExistingFiftyRoundSaveKeepsBalancesAndUnlocksFiftyOne() { yield return SaveMigration(50); }
        [UnityTest] public IEnumerator ExistingSixtyRoundSaveKeepsBalancesAndUnlocksSixtyOne() { yield return SaveMigration(60); }
        [UnityTest] public IEnumerator ExistingSeventyRoundSaveKeepsBalancesAndUnlocksSeventyOne() { yield return SaveMigration(70); }
        [UnityTest] public IEnumerator ExistingEightyRoundSaveKeepsBalancesAndUnlocksEightyOne() { yield return SaveMigration(80); }
        [UnityTest] public IEnumerator ExistingNinetyRoundSaveKeepsBalancesAndUnlocksNinetyOne() { yield return SaveMigration(90); }
        [UnityTest] public IEnumerator ExistingHundredRoundSaveKeepsBalancesAndUnlocksHundredOne() { yield return SaveMigration(100); }
        [UnityTest] public IEnumerator ExistingHundredTenRoundSaveKeepsBalancesAndUnlocksHundredEleven() { yield return SaveMigration(110); }
        [UnityTest] public IEnumerator ExistingHundredTwentyRoundSaveKeepsBalancesAndUnlocksHundredTwentyOne() { yield return SaveMigration(120); }
        [UnityTest] public IEnumerator ExistingHundredThirtyRoundSaveKeepsBalancesAndUnlocksHundredThirtyOne() { yield return SaveMigration(130); }
        [UnityTest] public IEnumerator ExistingHundredFortyRoundSaveKeepsBalancesAndUnlocksHundredFortyOne() { yield return SaveMigration(140); }
        [UnityTest] public IEnumerator ExistingHundredFiftyRoundSaveKeepsBalancesAndUnlocksHundredFiftyOne() { yield return SaveMigration(150); }
        [UnityTest] public IEnumerator ExistingHundredSixtyRoundSaveKeepsBalancesAndUnlocksHundredSixtyOne() { yield return SaveMigration(160); }
        [UnityTest] public IEnumerator ExistingHundredSeventyRoundSaveKeepsBalancesAndUnlocksHundredSeventyOne() { yield return SaveMigration(170); }
        [UnityTest] public IEnumerator ExistingHundredEightyRoundSaveKeepsBalancesAndUnlocksHundredEightyOne() { yield return SaveMigration(180); }
        [UnityTest] public IEnumerator ExistingHundredNinetyRoundSaveKeepsBalancesAndUnlocksHundredNinetyOne() { yield return SaveMigration(190); }
        IEnumerator SaveMigration(int completed)
        {
            Isolation();var obj=GameState.Instance==null?new GameObject("ActiveCampaignMigrationTest",typeof(GameState)):null;var state=GameState.Instance;var previous=JsonUtility.ToJson(state.Save);
            try {
                state.Save=new PlayerSaveData{gold=777,gems=19,energy=7,energyMax=30,highestUnlockedLevel=completed,selectedLevel=completed,lastEnergyUtcTicks=DateTime.UtcNow.Ticks};
                state.Save.levelProgress=Enumerable.Range(1,completed).Select(n=>new LevelProgressData{chapterId=1,levelId=n,completions=1,rewardClaimed=true,bestStars=2,attempts=1}).ToList();
                state.Persist();var saved=SaveService.LoadWithResult();Assert.That(saved.CanPersist,Is.True);state.Save=saved.Data;
                typeof(GameState).GetMethod("MigrateSave",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(state,null);
                Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(completed+1));Assert.That(state.Save.selectedLevel,Is.EqualTo(completed));
                Assert.That(state.Save.gold,Is.EqualTo(777));Assert.That(state.Save.gems,Is.EqualTo(19));Assert.That(state.Save.energy,Is.EqualTo(7));Assert.That(state.Save.levelProgress.Count,Is.EqualTo(completed));
                Assert.That(state.SelectLevel(1,completed+1),Is.True);Assert.That(state.SelectLevel(1,completed+2),Is.False);
                var replay=state.CompleteLevel(Level(completed),3000);Assert.That(state.Save.gold,Is.EqualTo(777));
                int gold=state.Save.gold,gems=state.Save.gems,mace=state.Save.boosterMace,pearl=state.Save.boosterSeaPearl;int firebird=state.Save.boosterFirebird,light=state.Save.boosterLivingLight;
                for(int n=completed+1;n<=200;n++){Assert.That(state.SelectLevel(1,n),Is.True);state.CompleteLevel(Level(n),3000);}
                Assert.That(state.Save.gold-gold,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.gold)));
                Assert.That(state.Save.gems-gems,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.gems)));
                Assert.That(state.Save.boosterMace-mace,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.boosterMace)));
                Assert.That(state.Save.boosterSeaPearl-pearl,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.boosterSeaPearl)));
                Assert.That(state.Save.boosterFirebird-firebird,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.boosterFirebird)));
                Assert.That(state.Save.boosterLivingLight-light,Is.EqualTo(Enumerable.Range(completed+1,200-completed).Sum(n=>Level(n).rewards.boosterLivingLight)));
                light=state.Save.boosterLivingLight;
                mace=state.Save.boosterMace;pearl=state.Save.boosterSeaPearl;firebird=state.Save.boosterFirebird;
                Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(200));Assert.That(state.SelectLevel(1,201),Is.False);Assert.That(state.SelectLevel(2,1),Is.False);Assert.That(StoryService.ShouldShowChapterComplete(),Is.False);
                gold=state.Save.gold;gems=state.Save.gems;state.CompleteLevel(Level(200),4000);Assert.That(state.Save.gold,Is.EqualTo(gold));Assert.That(state.Save.gems,Is.EqualTo(gems));
                Assert.That(state.Save.boosterMace,Is.EqualTo(mace));Assert.That(state.Save.boosterSeaPearl,Is.EqualTo(pearl));Assert.That(state.Save.boosterFirebird,Is.EqualTo(firebird));Assert.That(state.Save.boosterLivingLight,Is.EqualTo(light));
                state.Persist();state.Save=SaveService.LoadWithResult().Data;Assert.That(state.Save.highestUnlockedLevel,Is.EqualTo(200));Assert.That(state.GetLevelProgress(1,200).completions,Is.EqualTo(2));
            } finally { state.Save=JsonUtility.FromJson<PlayerSaveData>(previous);if(obj)UnityEngine.Object.Destroy(obj);StoryService.ResetRuntime(); }
            yield return null;
        }
        [UnityTest,Timeout(300000)] public IEnumerator NormalRouterPlaysOrchardFieldRecoveryTripleDeliveriesSurvivalAndSectorCheckpointWithNativeRewardsAndStory()
        {
            Isolation();var stateObject=GameState.Instance==null?new GameObject("ActiveCampaignRuntimeState",typeof(GameState)):null;var obj=ScreenRouter.Instance==null?new GameObject("ActiveCampaignRouterTest",typeof(ScreenRouter)):null;var router=obj?obj.GetComponent<ScreenRouter>():ScreenRouter.Instance;router.Initialize();yield return null;
            try {
                foreach(int n in new[]{190,191,195,198,200}){
                    var state=GameState.Instance;Assert.That(state,Is.Not.Null);
                    state.Save=new PlayerSaveData{highestUnlockedLevel=n,selectedLevel=n,energy=30,energyMax=30,lastEnergyUtcTicks=DateTime.UtcNow.Ticks,gold=0,gems=0};
                    state.Save.levelProgress=Enumerable.Range(1,n-1).Select(i=>new LevelProgressData{chapterId=1,levelId=i,completions=1,rewardClaimed=true}).ToList();
                    state.Save.completedStoryScenes=StoryCatalog.LoadChapter1().scenes.Where(s=>s.id!=(n==200?"campaign.chapter01.orchard_field_paths":n==190?"campaign.chapter01.field_home_paths":n==180?"campaign.chapter01.field_approach":n==170?"campaign.chapter01.orchard_paths":n==160?"campaign.chapter01.orchard_entrance":n==150?"campaign.chapter01.mill_routes_secured":n==140?"campaign.chapter01.provision_paths":n==130?"campaign.chapter01.storehouse_approach":n==120?"campaign.chapter01.mill_yard":n==110?"campaign.chapter01.mill_approach":n==100?"campaign.chapter01.well_and_hearths":n==90?"campaign.chapter01.hearth_routes":n==80?"campaign.chapter01.courtyard_paths":n==70?"campaign.chapter01.far_street":n==60?"campaign.chapter01.well_path":n==50?"campaign.chapter01.beacon_lit":n==40?"campaign.chapter01.river_bank":string.Empty)).Select(s=>s.id).ToList();
                    StoryService.ResetRuntime();router.Show(ScreenId.ChapterMap);yield return Settle(router);
                    var map=router.GameCanvas.GetComponentInChildren<ProductionChapterMapView>();Assert.That(map.LevelAvailable(n),Is.True);Assert.That(map.LevelAvailable(201),Is.False);
                    if(n>=100){
                        map.Scroller.JumpToCurrent();yield return Settle(router);
                        var node=map.Nodes.Single(x=>x.LevelId==n);Assert.That(node.Root.gameObject.activeInHierarchy,Is.True);
                        Assert.That(node.Number.text,Is.EqualTo(n.ToString()));Assert.That(node.Button.IsInteractable(),Is.True);
                        node.Button.onClick.Invoke();
                    }else map.PlayButton.onClick.Invoke();
                    yield return Settle(router);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
                    router.GameCanvas.GetComponentInChildren<ProductionPreLevelView>().PlayButton.onClick.Invoke();yield return Settle(router);
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));var board=router.GameCanvas.GetComponentInChildren<ProductionGameplayView>().Board;
                    Assert.That(board.CurrentLevel.levelId,Is.EqualTo(n));Assert.That(board.Moves,Is.EqualTo(Level(n).moveLimit));Assert.That(board.RuleVersion,Is.EqualTo(GameplayRuleVersion.PrototypeR4));
                    var witness=Solve(Level(n),1);Assert.That(board.BoardStateHash,Is.EqualTo(witness.initialHash));
                    foreach(var command in witness.commands){
                        Assert.That(board.BoardReplayToken,Is.EqualTo(command.BeforeToken));
                        bool accepted=command.Tap?board.TapCell(command.X,command.Y):board.SwipeCell(command.X,command.Y,new Vector2Int(command.ToX-command.X,command.ToY-command.Y));Assert.That(accepted,Is.True);
                        float deadline=Time.realtimeSinceStartup+20;
                        while(router.CurrentScreen==ScreenId.Gameplay&&board&&(board.HasActiveCommand||!board.CanAcceptPointerInput)&&Time.realtimeSinceStartup<deadline)yield return null;
                        Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline));yield return Settle(router);
                    }
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Victory));Assert.That(state.Save.energy,Is.EqualTo(29));Assert.That(state.Save.gold,Is.EqualTo(Level(n).rewards.gold));Assert.That(state.GetLevelProgress(1,n).completions,Is.EqualTo(1));
                    router.GameCanvas.GetComponentInChildren<ProductionVictoryView>().ContinueButton.onClick.Invoke();yield return Settle(router);
                    int lines=0;while(router.CurrentScreen==ScreenId.Dialogue){Assert.That(++lines,Is.LessThan(10));router.GameCanvas.GetComponentInChildren<ProductionDialogueView>().Continue.onClick.Invoke();yield return Settle(router);}
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));if(n==40)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.river_bank"),Is.True);
                    if(n==50)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.beacon_lit"),Is.True);
                    if(n==60)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.well_path"),Is.True);
                    if(n==70)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.far_street"),Is.True);
                    if(n==80)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.courtyard_paths"),Is.True);
                    if(n==90)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.hearth_routes"),Is.True);
                    if(n==100)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.well_and_hearths"),Is.True);
                    if(n==110)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.mill_approach"),Is.True);
                    if(n==120)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.mill_yard"),Is.True);
                    if(n==130)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.storehouse_approach"),Is.True);
                    if(n==140)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.provision_paths"),Is.True);
                    if(n==150)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.mill_routes_secured"),Is.True);
                    if(n==160)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_entrance"),Is.True);
                    if(n==170)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_paths"),Is.True);
                    if(n==180)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.field_approach"),Is.True);
                    if(n==190)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.field_home_paths"),Is.True);
                    if(n==200)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_field_paths"),Is.True);
                    Assert.That(StoryService.ShouldShowChapterComplete(),Is.False);router.Show(ScreenId.Home);yield return Settle(router);
                }
            } finally {router.Show(ScreenId.Home);if(obj)UnityEngine.Object.Destroy(obj);if(stateObject)UnityEngine.Object.Destroy(stateObject);StoryService.ResetRuntime();}
            yield return null;
        }
        static IEnumerator Settle(ScreenRouter r){for(int i=0;i<24;i++)r.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);yield return null;yield return null;}
    }
}
