from pathlib import Path
import re,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
p=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text()
s=s.replace('levels.levels.Length,Is.EqualTo(200)','levels.levels.Length,Is.EqualTo(500)').replace('Chapter01EditorialR42','Chapter01CompleteR44').replace('Chapter01Sector04R32/levels.json','Chapter01EditorialR42/levels.json').replace('i<190','i<200')
s=s.replace('levels.levels.Skip(190).Sum(l=>l.rewards.gold),Is.EqualTo(1840)','levels.levels.Skip(200).Sum(l=>l.rewards.gold),Is.EqualTo(55200)').replace('levels,false).Errors','levels,true).Errors').replace('completion?.finalLevelId??0,Is.Zero','completion?.finalLevelId??0,Is.EqualTo(500)').replace('ChapterLevelCatalog.Next(Level(200)),Is.Null','ChapterLevelCatalog.Next(Level(500)),Is.Null')
a=s.index('        [TestCase(1)]');b=s.index('        public void ThreeAttemptSeeds',a)
s=s[:a]+'        static IEnumerable<int> AuthoredRounds => Enumerable.Range(1,500);\n        [TestCaseSource(nameof(AuthoredRounds))]\n'+s[b:]
s=s.replace('        IEnumerator SaveMigration(int completed)','        [UnityTest] public IEnumerator ExistingTwoHundredSaveUnlocks201WithoutReplayRewards() { yield return SaveMigration(200); }\n        [UnityTest] public IEnumerator Existing490SaveReaches500WithoutEarlyFinale() { yield return SaveMigration(490); }\n        IEnumerator SaveMigration(int completed)')
a=s.index('                for(int n=completed+1;');b=s.index('            } finally',a)
part=s[a:b].replace('200','end').replace('201','end+1')
part=part.replace('StoryService.ShouldShowChapterComplete(),Is.False','StoryService.ShouldShowChapterComplete(),Is.False')
# At nonterminal endpoints the next round is unlocked; the chapter 2 boundary remains closed.
part=part.replace('state.Save.highestUnlockedLevel,Is.EqualTo(end)','state.Save.highestUnlockedLevel,Is.EqualTo(Math.Min(500,end+1))').replace('state.SelectLevel(1,end+1),Is.False','state.SelectLevel(1,end+1),Is.EqualTo(end<500)')
s=s[:a]+'                int end=Math.Min(500,completed+10);\n'+part+s[b:]
s=s.replace('NormalRouterPlaysOrchardFieldRecoveryTripleDeliveriesSurvivalAndSectorCheckpointWithNativeRewardsAndStory','NormalRouterPlaysNewSectorsAndFinaleWithNativeRewardsAndStory').replace('new[]{190,191,195,198,200}','new[]{200,201,245,298,350,400,450,495,500}').replace('Timeout(300000)','Timeout(600000)')
a=s.index('                    state.Save.completedStoryScenes=');b=s.index('\n',a)
s=s[:a]+'                    var catalog=StoryCatalog.LoadChapter1();\n                    state.Save.completedStoryScenes=new[]{catalog.introSceneId}.Concat(catalog.milestones.Where(m=>m.afterLevelId<n).Select(m=>m.sceneId)).ToList();'+s[b:]
s=s.replace('map.LevelAvailable(201),Is.False','map.LevelAvailable(501),Is.False')
a=s.index('                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));if(n==40)');b=s.index('\n                }',a)
s=s[:a]+'''                    if(n==500) {
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterComplete));
                        Assert.That(StoryService.ShouldShowChapterComplete(),Is.True);
                        var complete=router.GameCanvas.GetComponentInChildren<ProductionChapterCompleteView>();
                        Assert.That(complete.Summary.text,Does.Contain("500/500"));
                        var received=ProductionChapterCompleteView.ReceivedRewards(state);
                        Assert.That(received[0],Is.EqualTo(ChapterLevelCatalog.LoadChapter1().levels.Sum(l=>l.rewards.gold)));
                        string capture=Environment.GetEnvironmentVariable("LOR_CHAPTER_COMPLETE_CAPTURE_DIR");
                        if(!string.IsNullOrEmpty(capture)) {
                            var texture=new Texture2D(Screen.width,Screen.height,TextureFormat.RGB24,false);
                            yield return new WaitForEndOfFrame();
                            texture.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);texture.Apply();
                            Directory.CreateDirectory(capture);File.WriteAllBytes(Path.Combine(capture,"Chapter1_500_Complete.png"),texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);
                        }
                        complete.Actions[0].onClick.Invoke();yield return Settle(router);
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
                        Assert.That(state.HasAcknowledgedChapterCompletion(1),Is.True);
                        Assert.That(state.SelectLevel(2,1),Is.False);
                    } else {
                        Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
                        var milestone=catalog.milestones.SingleOrDefault(m=>m.afterLevelId==n);
                        if(milestone!=null)Assert.That(state.HasCompletedStoryScene(milestone.sceneId),Is.True);
                        Assert.That(StoryService.ShouldShowChapterComplete(),Is.False);
                    }
                    Assert.That(state.Save.gold,Is.EqualTo(Level(n).rewards.gold),"Finale cannot grant a second reward");
                    router.Show(ScreenId.Home);yield return Settle(router);'''+s[b:]
p.write_text(s)
for path in ['CampaignPlayMode/CampaignBoardIntegrationTests.cs','PlayMode/LevelSystemRuntimePlayModeTests.cs']:
 p=root/'Assets/LegendsOfRus/Tests'/path;s=p.read_text().replace('EqualTo(200)','EqualTo(500)').replace('TwoHundredValidated','FiveHundredValidated');p.write_text(s)
p=root/'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CampaignActivePlaytestSetup.cs';shutil.copy2(p,base/(p.stem+'.before.cs'));s=p.read_text().replace('ChapterId = 1, ContentRevision = definition.ContentRevision,','ChapterId = 1, ContentRevision = definition.ContentRevision,\n                FinaleReady = JsonUtility.FromJson<ChapterStoryCatalogData>(storyJson)?.completion?.finalLevelId == 500,');p.write_text(s)
p=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignActivePlaytestTests.cs';shutil.copy2(p,base/(p.stem+'.before.cs'));s=p.read_text().replace('[TestCase(201)] public void Unavailable','[TestCase(501)] public void Unavailable');a=s.index('        [TestCase(1)]');b=s.index('        public void EachInstalledBlock',a);s=s[:a]+'        static System.Collections.Generic.IEnumerable<int> Blocks => Enumerable.Range(0,50).Select(n=>1+10*n);\n        [TestCaseSource(nameof(Blocks))]\n'+s[b:];p.write_text(s)
p=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(200,','Math.Min(500,');s=s.replace('var milestones = StoryCatalog.LoadChapter1().milestones;','var milestones = StoryCatalog.LoadChapter1().milestones.Where(m=>m.afterLevelId<=200).ToArray();')
s=s.replace('State.Save.highestUnlockedLevel, Is.EqualTo(200)','State.Save.highestUnlockedLevel, Is.EqualTo(201)')
s=s.replace('UnavailableRoundAndChapterCannotChangeCompletedTwoHundredSave','UnavailableRoundAndChapterCannotChangeCompletedFiveHundredSave');a=s.index('        [Test] public void UnavailableRoundAndChapter');sub=s[a:].replace('PrepareCompletedFixture(200','PrepareCompletedFixture(500').replace('Find(1, 201)','Find(1, 501)').replace('Find(1, 200)','Find(1, 500)').replace('SelectLevel(1, 201)','SelectLevel(1, 501)').replace('Is.EqualTo(201)','Is.EqualTo(500)');s=s[:a]+sub
# Existing early-content catch-up stays an explicit 200-round fixture; add new sector cases.
s=s.replace('[TestCase(200)]\n        public void UnreadCheckpoint','[TestCase(200)][TestCase(250)][TestCase(300)][TestCase(350)][TestCase(400)][TestCase(450)][TestCase(490)]\n        public void UnreadCheckpoint').replace('[TestCase(200)]\n        public void FailedCheckpoint','[TestCase(200)][TestCase(250)][TestCase(300)][TestCase(350)][TestCase(400)][TestCase(450)][TestCase(500)]\n        public void FailedCheckpoint')
p.write_text(s)
print('Updated active loaders and current-content integration expectations.')
