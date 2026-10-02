using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class GameplayAlignmentPlayModeTests
    {
        ScreenRouter router;
        string previousSave, previousLanguage;
        readonly string[] keys = { "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging" };
        string[] values;
        bool[] existed;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated)) Assert.Ignore("Requires isolated test storage.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated) + Path.DirectorySeparatorChar), Is.True);
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(ScreenRouter.Instance, Is.SameAs(router));
            previousSave = JsonUtility.ToJson(GameState.Instance.Save);
            previousLanguage = DemoFeatureState.LanguageCode;
            values = new string[keys.Length]; existed = new bool[keys.Length];
            for (int i = 0; i < keys.Length; i++) { existed[i] = PlayerPrefs.HasKey(keys[i]); values[i] = PlayerPrefs.GetString(keys[i]); }
            GameState.Instance.ResetProgress(); StoryService.ResetRuntime(); DemoFeatureState.SetLanguage("ru");
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach(var step in level.tutorialSteps)GameState.Instance.CompleteTutorial(step.id);
            router.Show(ScreenId.Gameplay); Settle();
            yield return null; yield return null;
            yield return WaitForNormalButtonColors();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (router != null) { router.Show(ScreenId.SplashTitle); Settle(); }
            StoryService.ResetRuntime();
            if (previousSave != null) GameState.Instance.Save = JsonUtility.FromJson<PlayerSaveData>(previousSave);
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (values != null)
            {
                for (int i = 0; i < keys.Length; i++) { PlayerPrefs.DeleteKey(keys[i]); if (existed[i]) PlayerPrefs.SetString(keys[i], values[i]); }
                PlayerPrefs.Save();
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator CaptureReferenceAndPhonePortraits()
        {
            Capture("Gameplay_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Gameplay_576x1280_safearea_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("Gameplay_1080x2400_safearea_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Capture("Gameplay_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");yield return null;yield return null;
            Capture("Gameplay_941x1672_RU_roundtrip",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }

        [UnityTest]
        public IEnumerator SmallPortraitPausePaddingAndFourDigitStockPreserveTheRound()
        {
            var v=Root().GetComponentInChildren<ProductionGameplayView>();string board=v.Board.BoardStateHash;
            var save=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));save.lastEnergyUtcTicks=0;
            GameObject tap=null;
            foreach(var lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);yield return null;yield return null;
                foreach(var count in v.BoosterCounts)count.text="9 999";
                Capture("Gameplay_TouchAndStock_360_"+lang,new Vector2Int(360,640),new Rect(0,20,360,600),camera=>
                {
                    tap=CoreOutcomeTouchAssertions.Check(v.PauseButton,camera,new Rect(0,20,360,600),"Gameplay","360");
                    foreach(var count in v.BoosterCounts)
                    {
                        Assert.That(count.cachedTextGenerator.lineCount,Is.EqualTo(1));Assert.That(count.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(count.text.Length));
                        var mesh=count.canvasRenderer.GetMesh();foreach(var vertex in mesh.vertices){Assert.That(vertex.x,Is.InRange(count.rectTransform.rect.xMin-.1f,count.rectTransform.rect.xMax+.1f));Assert.That(vertex.y,Is.InRange(count.rectTransform.rect.yMin-.1f,count.rectTransform.rect.yMax+.1f));}
                    }
                });
                Capture("Gameplay_TouchAndStock_1080_"+lang,new Vector2Int(1080,2400),new Rect(45,100,990,2160),camera=>CoreOutcomeTouchAssertions.Check(v.PauseButton,camera,new Rect(45,100,990,2160),"Gameplay","1080"));
            }
            CoreOutcomeTouchAssertions.Click(tap);Settle();yield return null;yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(v.Board.Phase,Is.EqualTo(GameplayPhase.Paused));Assert.That(v.Board.BoardStateHash,Is.EqualTo(board));
            var after=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));after.lastEnergyUtcTicks=0;Assert.That(JsonUtility.ToJson(after),Is.EqualTo(JsonUtility.ToJson(save)));
        }

        [UnityTest]
        public IEnumerator PortraitLowerBandDoesNotDarkenExposedScenery()
        {
            var header=Root().GetComponentInChildren<ProductionGameplayView>().Hud.Find("JoinedHeaderBacking").GetComponent<Image>();
            string directory=Environment.GetEnvironmentVariable("LOR_GAMEPLAY_CAPTURE_DIR");
            Capture("Gameplay_PortraitBand_Normal",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            try
            {
                header.enabled=false;
                Capture("Gameplay_PortraitBand_HeaderHidden_Control",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            }
            finally { header.enabled=true; }
            var normal=new Texture2D(2,2);var control=new Texture2D(2,2);
            try
            {
                normal.LoadImage(File.ReadAllBytes(Path.Combine(directory,"Gameplay_PortraitBand_Normal.png")));
                control.LoadImage(File.ReadAllBytes(Path.Combine(directory,"Gameplay_PortraitBand_HeaderHidden_Control.png")));
                for(int y=233;y<241;y++)for(int x=146;x<223;x++)
                    Assert.That(normal.GetPixel(x,1671-y),Is.EqualTo(control.GetPixel(x,1671-y)),"No header matte or detached rail may darken the scenery below the portrait at "+x+","+y);
            }
            finally { UnityEngine.Object.Destroy(normal);UnityEngine.Object.Destroy(control); }
            // Check the whole former nameplate area against a HUD-free render.
            // This distinguishes an actual translucent UI band from a defect baked
            // into the environment; scenery continuity also requires visual review.
            var hud=header.transform.parent.gameObject.AddComponent<CanvasGroup>();
            try
            {
                hud.alpha=0;
                Capture("Gameplay_PortraitBand_HudHidden_Control",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            }
            finally { hud.alpha=1;UnityEngine.Object.Destroy(hud); }
            normal=new Texture2D(2,2);control=new Texture2D(2,2);
            try
            {
                normal.LoadImage(File.ReadAllBytes(Path.Combine(directory,"Gameplay_PortraitBand_Normal.png")));
                control.LoadImage(File.ReadAllBytes(Path.Combine(directory,"Gameplay_PortraitBand_HudHidden_Control.png")));
                for(int y=246;y<282;y++)for(int x=24;x<258;x++)
                    Assert.That(normal.GetPixel(x,1671-y),Is.EqualTo(control.GetPixel(x,1671-y)),"No UI layer may cover the trees below the portrait at "+x+","+y);
            }
            finally { UnityEngine.Object.Destroy(normal);UnityEngine.Object.Destroy(control); }
            yield return null;
        }

        [UnityTest]
        public IEnumerator LargeHudValuesAndThreeMissionsStayReadableInBothLanguages()
        {
            GameState.Instance.Save.highestUnlockedLevel=10;
            GameState.Instance.SelectLevel(1,8);router.Show(ScreenId.Gameplay);Settle();
            yield return null;yield return null;yield return WaitForNormalButtonColors();
            foreach(string language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);yield return null;yield return null;
                var view=Root().GetComponentInChildren<ProductionGameplayView>();
                view.MovesValue.text="999";view.ScoreValue.text="9 999 999";
                view.LevelText.text=language=="ru"?"УРОВЕНЬ 500":"LEVEL 500";
                view.Goals[0].Count.text="12 345";view.Goals[1].Count.text="✓";view.Goals[2].Count.text="999";
                Capture("Gameplay_ReadabilityFixture_438_"+language,new Vector2Int(438,780),new Rect(0,0,438,780));
                Capture("Gameplay_ThreeMissions_576_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172));
            }
        }

        [UnityTest]
        public IEnumerator GameplayStatusReplacesLocalizesAndLeavesTheBoardAndControlsClear()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var status=view.Status;
            string before=view.Board.BoardStateHash;int stock=GameState.Instance.BoosterCount(BoosterType.Firebird);
            int lightStock=GameState.Instance.BoosterCount(BoosterType.LivingLight);
            view.BoosterButtons[1].onClick.Invoke();yield return null;
            Assert.That(status.IsVisible,Is.True);Assert.That(status.Label.text,Does.StartWith("Перо Жар-Птицы:"));
            view.Board.CancelArmedBooster(showToast:false);yield return null;
            Assert.That(status.IsVisible,Is.False,"Silent cancellation must clear a persistent targeting instruction.");
            view.BoosterButtons[1].onClick.Invoke();yield return null;
            view.BoosterButtons[2].onClick.Invoke();yield return null;
            Assert.That(status.Label.text,Does.StartWith("Живой Свет:"));
            Assert.That(view.Board.ArmedBooster,Is.EqualTo(BoosterType.LivingLight));
            Assert.That(status.Panel.GetComponentsInChildren<Text>().Length,Is.EqualTo(1));
            foreach(Transform child in router.Viewport)Assert.That(child.name,Is.Not.EqualTo("Toast"));
            Capture("Gameplay_Status_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Gameplay_Status_576x1280_safearea_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Assert.That(status.Label.text,Does.StartWith("Living Light:"));
            Capture("Gameplay_Status_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Gameplay_Status_1080x2400_safearea_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            Assert.That(view.Board.BoardStateHash,Is.EqualTo(before));Assert.That(GameState.Instance.BoosterCount(BoosterType.Firebird),Is.EqualTo(stock));
            Assert.That(GameState.Instance.BoosterCount(BoosterType.LivingLight),Is.EqualTo(lightStock));
            view.BoosterButtons[2].onClick.Invoke();yield return null;
            Assert.That(status.Label.text,Is.EqualTo("Booster canceled."));
            float deadline=Time.realtimeSinceStartup+3f;
            while(status.IsVisible&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(status.IsVisible,Is.False,"Transient feedback must expire.");
            router.Toast("Жемчужина Морского Царя: +5 ходов.");Assert.That(status.IsVisible,Is.True);
            router.Show(ScreenId.Home);Settle();yield return null;yield return null;
            router.Toast("HUD regression: legacy Home feedback");yield return null;
            bool legacyToast=false;foreach(Transform child in router.Viewport)if(child.name=="Toast")legacyToast=true;
            Assert.That(legacyToast,Is.True,"The Gameplay message route must preserve feedback on other pages.");
            yield return new WaitForSecondsRealtime(2.3f);
            router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;
            Assert.That(Root().GetComponentInChildren<ProductionGameplayView>().Status.IsVisible,Is.False,"Messages cannot leak into a new screen instance.");
        }

        [UnityTest]
        public IEnumerator LiveInventoryAndMovesUseTheExistingBoosterTransaction()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;var state=GameState.Instance;
            Assert.That(view.MovesValue.text,Is.EqualTo(board.Moves.ToString()));Assert.That(view.Goals[0].Count.text,Is.EqualTo(board.GoalSlot1Remaining.ToString()));
            for(int i=0;i<4;i++){state.AddBooster(ProductionPreLevelRuntime.Boosters[i],2);Assert.That(view.BoosterCounts[i].text,Is.EqualTo(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));}
            int inventory=state.BoosterCount(BoosterType.Firebird);string before=board.BoardStateHash;
            view.BoosterButtons[1].onClick.Invoke();yield return null;
            Assert.That(board.ArmedBooster,Is.EqualTo(BoosterType.Firebird));Assert.That(view.ArmedMarkers[1].gameObject.activeSelf,Is.True);
            var card=view.Footer.Find("BoosterCard_1").GetComponent<ProductionGameplayBoosterCard>();
            ClickCardEdge(card);yield return null;
            Assert.That(board.IsBoosterArmed,Is.False,"The wood-card surface must reach the existing button action.");
            ClickCardEdge(card);yield return null;
            Assert.That(board.IsBoosterArmed,Is.True);
            view.BoosterButtons[1].onClick.Invoke();yield return null;Assert.That(board.IsBoosterArmed,Is.False);
            Assert.That(state.BoosterCount(BoosterType.Firebird),Is.EqualTo(inventory));Assert.That(board.BoardStateHash,Is.EqualTo(before));
            int moves=board.Moves;inventory=state.BoosterCount(BoosterType.SeaPearl);
            view.BoosterButtons[3].onClick.Invoke();view.BoosterButtons[3].onClick.Invoke();
            float deadline=Time.realtimeSinceStartup+8;
            while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)yield return null;
            Assert.That(board.HasActiveCommand,Is.False);Assert.That(board.Moves,Is.EqualTo(moves+5));Assert.That(view.MovesValue.text,Is.EqualTo(board.Moves.ToString()));
            Assert.That(state.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(inventory-1));Assert.That(view.BoosterCounts[3].text,Is.EqualTo((inventory-1).ToString()));
        }

        [UnityTest]
        public IEnumerator NativePauseKeepsTheSameBoardAndBlocksBoosterInput()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;string hash=board.BoardStateHash;
            view.PauseButton.onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Paused));
            int stock=GameState.Instance.BoosterCount(BoosterType.SeaPearl);view.BoosterButtons[3].onClick.Invoke();Assert.That(GameState.Instance.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(stock));
            var pause=router.GameCanvas.GetComponentInChildren<ProductionPauseView>();
            Assert.That(pause,Is.Not.Null);pause.Actions[0].onClick.Invoke();
            Settle();yield return null;yield return null;
            Assert.That(board.Phase,Is.EqualTo(GameplayPhase.Idle));Assert.That(view.Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(view.ActionsBound,Is.True);
        }

        [UnityTest,Timeout(600000)]
        public IEnumerator AuthoredGoalsObstaclesAndFeathersUseTheirActualState()
        {
            GameState.Instance.Save.highestUnlockedLevel=ChapterLevelCatalog.LoadChapter1().levels.Length;
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
            {
                GameState.Instance.SelectLevel(1,level.levelId);Assert.That(GameState.Instance.Save.selectedLevel,Is.EqualTo(level.levelId));router.Show(ScreenId.Gameplay);Settle();yield return null;yield return null;yield return WaitForNormalButtonColors();
                var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
                Assert.That(Array.TrueForAll(view.Stars,star=>star.Dimmed),Is.True,"No score stars are lit before earning points.");
                Assert.That(board.Score,Is.Zero,"Every authored round starts at zero earned points.");
                Assert.That(view.ScoreValue.text.Replace(" ",""),Is.EqualTo("0"));
                Assert.That(view.HeroName.gameObject.activeSelf,Is.False);
                Assert.That(board.ColumnCount,Is.EqualTo(8));Assert.That(board.RowCount,Is.EqualTo(7));
                Assert.That(board.GoalCount,Is.EqualTo(level.goals.Length));
                Assert.That(board.CurrentLevel.levelId,Is.EqualTo(level.levelId));Assert.That(view.LevelText.text,Is.EqualTo("УРОВЕНЬ "+level.levelId));
                for(int i=0;i<level.goals.Length;i++)
                {
                    var goal=board.GetGoalSlot(i);
                    Assert.That(goal,Is.Not.Null);
                    Assert.That(view.Goals[i].Root.gameObject.activeSelf,Is.True);
                    Assert.That(view.Goals[i].Count.text.Replace(" ",""),Is.EqualTo(goal.Remaining.ToString()));
                    bool symbol=goal.Type==LevelGoalTypes.Score||goal.Type==LevelGoalTypes.Survival;
                    Assert.That(view.Goals[i].Symbol.enabled,Is.False,"Painted objective icons replace the retained geometric symbol.");
                    if(symbol)Assert.That(view.Goals[i].Icon.sprite,Is.SameAs(ProductionGameplayArt.Load(goal.Type==LevelGoalTypes.Survival?"goal_survival_r5":"goal_score_r5")));
                }
                var model=(BoardModel)typeof(Match3Board).GetField("model",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(board);
                for(int y=0;y<board.RowCount;y++)for(int x=0;x<board.ColumnCount;x++)
                {
                    var pos=new BoardPosition(x,y);var cell=board.GetCellInputForTesting(x,y);var obstacle=model.GetObstacle(pos);
                    var graphic=cell.transform.Find("CellContent/Obstacle").GetComponent<Image>();
                    Assert.That(graphic.enabled,Is.EqualTo(!obstacle.IsEmpty));if(!obstacle.IsEmpty)Assert.That(graphic.sprite,Is.SameAs(PrototypeObstacleArt.Load(obstacle.Kind,obstacle.Layers)));
                    Assert.That(cell.transform.Find("CellContent/ObstacleLayers"),Is.Null,"Approved obstacle stages use damaged artwork, not numbers.");
                    if(obstacle.Layers>1)Assert.That(PrototypeObstacleArt.Load(obstacle.Kind,1),Is.Not.SameAs(graphic.sprite),"Final-stage art must differ from intact art.");
                    var gem=cell.transform.Find("CellContent/GemVisual/Gem").GetComponent<Image>();
                    Assert.That(cell.transform.Find("CellContent/GemVisual/SpecialMatchColor"),Is.Null);
                    if(model.GetObjectiveItem(pos)!=BoardObjectiveItemKind.None){Assert.That(gem.enabled,Is.True);Assert.That(gem.sprite,Is.SameAs(ProductionGameplayArt.Load("feather")));}
                    else if(model.GetSpecial(pos)==SpecialGemKind.None&&model.GetTile(pos)>=0)Assert.That(gem.sprite,Is.SameAs(ProductionGameplayArt.Gem(model.GetTile(pos))));
                }
                if(level.levelId==4||level.levelId==6||level.levelId==8||level.levelId==9||level.levelId==10)Capture("Gameplay_Level"+level.levelId.ToString("00")+"_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            }
            var finalView=Root().GetComponentInChildren<ProductionGameplayView>();DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Capture("Gameplay_LastInstalledLevel_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }

        [UnityTest]
        public IEnumerator SpecialColorsUseWholeOrientedObjectsWithoutCompanionFragments()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
            var model=new BoardModel(8,7,6,new SeededBoardRandom(101));model.GeneratePlayableBoard();
            for(int x=0;x<6;x++)
            {
                model.SetCell(new BoardPosition(x,1),new BoardCellData(x,SpecialGemKind.Row));
                model.SetCell(new BoardPosition(x,2),new BoardCellData(x,SpecialGemKind.Column));
            }
            model.SetCell(new BoardPosition(6,3),new BoardCellData(0,SpecialGemKind.Burst));
            model.SetCell(new BoardPosition(7,3),new BoardCellData(2,SpecialGemKind.Color));
            Assert.That(board.LoadSnapshotForTesting(model.CreateSnapshot()),Is.True);yield return null;
            for(int y=1;y<=2;y++)for(int x=0;x<6;x++)
            {
                var root=board.GetCellInputForTesting(x,y).transform.Find("CellContent/GemVisual");
                Assert.That(root.Find("SpecialMatchColor"),Is.Null);
                var special=root.Find("Special").GetComponent<Image>();
                Assert.That(special.enabled,Is.True);Assert.That(special.sprite,Is.SameAs(PrototypeBoardArt.Load("line")));
                Assert.That(Mathf.DeltaAngle(special.rectTransform.localEulerAngles.z,y==1?0:90),Is.EqualTo(0).Within(.001));
                Assert.That(root.Find("Gem").GetComponent<Image>().enabled,Is.False);
            }
            var grid=board.transform.Find("SharedGrid").GetComponent<ProductionGameplayCell>();
            Assert.That(grid.GridOnly,Is.True);Assert.That(grid.GridJointCount,Is.EqualTo(72));Assert.That(grid.raycastTarget,Is.False);
            Assert.That(grid.sprite,Is.SameAs(ProductionGameplaySourceControls.Load("grid")),"The native shared grid must use the isolated source metal atlas.");
            Assert.That(grid.mainTexture,Is.SameAs(grid.sprite.texture));
            Capture("Gameplay_Specials_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("Gameplay_Specials_438x780_RU",new Vector2Int(438,780),new Rect(0,0,438,780),camera=>
            {
                for(int y=1;y<=2;y++)for(int x=0;x<6;x++)
                {
                    var special=(RectTransform)board.GetCellInputForTesting(x,y).transform.Find("CellContent/GemVisual/Special");
                    var corners=new Vector3[4];special.GetWorldCorners(corners);
                    float shortSide=Vector2.Distance(RectTransformUtility.WorldToScreenPoint(camera,corners[0]),RectTransformUtility.WorldToScreenPoint(camera,corners[1]));
                    float other=Vector2.Distance(RectTransformUtility.WorldToScreenPoint(camera,corners[1]),RectTransformUtility.WorldToScreenPoint(camera,corners[2]));
                    Assert.That(Mathf.Min(shortSide,other),Is.GreaterThan(22),"The colored body must remain readable at the reported screenshot width.");
                }
            });
        }

        [UnityTest]
        public IEnumerator GridRemainsVisibleAfterTwelveResolvedMovesAt438Pixels()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
            var model=(BoardModel)typeof(Match3Board).GetField("model",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(board);
            Assert.That(board.LoadSnapshotForTesting(model.CreateSnapshot()),Is.True);
            for(int move=0;move<12;move++)
            {
                bool found=false;BoardPosition a=default,b=default;
                for(int y=0;y<7&&!found;y++)for(int x=0;x<8&&!found;x++)
                foreach(var step in new[]{new BoardPosition(1,0),new BoardPosition(0,1)})
                {
                    var source=new BoardPosition(x,y);var target=new BoardPosition(x+step.X,y+step.Y);
                    if(model.IsInside(target)&&model.SwapCreatesMatch(source,target)){a=source;b=target;found=true;break;}
                }
                Assert.That(found,Is.True);Assert.That(board.TapCell(a.X,a.Y),Is.True);Assert.That(board.TapCell(b.X,b.Y),Is.True);
                float deadline=Time.realtimeSinceStartup+12;
                while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)yield return null;
                Assert.That(board.HasActiveCommand,Is.False);
                var grid=board.transform.Find("SharedGrid").GetComponent<ProductionGameplayCell>();
                Assert.That(grid.isActiveAndEnabled,Is.True);Assert.That(grid.GridOnly,Is.True);
                var vfx=board.transform.Find("CoreLoopVfxLayer");
                if(vfx!=null)Assert.That(grid.transform.GetSiblingIndex(),Is.GreaterThan(vfx.GetSiblingIndex()),"Persistent junctions must render above match washes.");
            }
            yield return WaitForNormalButtonColors();
            Capture("Gameplay_After12Moves_438x780_RU",new Vector2Int(438,780),new Rect(0,0,438,780));
            var presenter=board.GetComponent<CoreLoopVfxPresenter>();
            float settleDeadline=Time.realtimeSinceStartup+4;
            while(presenter!=null&&presenter.ActiveEffectCount>0&&Time.realtimeSinceStartup<settleDeadline)yield return null;
            Assert.That(presenter==null||presenter.ActiveEffectCount==0,Is.True);
            Capture("Gameplay_After12Moves_Settled_438x780_RU",new Vector2Int(438,780),new Rect(0,0,438,780));
        }

        [UnityTest]
        public IEnumerator AutomaticReshuffleMovesGemsLocksInputAndPreservesPlannedResult()
        {
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var board=view.Board;
            BoardSnapshot source=null;BoardResolutionPlan expected=null;
            for(int seed=1;seed<=500;seed++)
            {
                var candidate=new BoardModel(8,7,6,new SeededBoardRandom(seed));
                for(int y=0;y<7;y++)for(int x=0;x<8;x++)candidate.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%6));
                for(int y=0;y<7;y++)for(int x=0;x<8;x++)if(x>=3||y>=3)candidate.SetObstacle(new BoardPosition(x,y),new BoardObstacleData(BoardObstacleKind.Crate,2));
                candidate.SetObstacle(new BoardPosition(7,6),new BoardObstacleData(BoardObstacleKind.Crate,2));
                var plan=new BoardResolutionPlanner(candidate,BoardBalanceProfile.CommercialDefault).PlanBooster(new BoardBoosterRequest(BoardBoosterCommandKind.MaceRow,new BoardPosition(0,0)));
                if(plan.Reshuffle.Attempts>0){source=candidate.CreateSnapshot();expected=plan;break;}
            }
            Assert.That(source,Is.Not.Null,"A deterministic no-move fixture must exercise the real planner reshuffle.");
            Assert.That(board.LoadSnapshotForTesting(source),Is.True);
            bool started=false,completed=false,moved=false;int observedFrames=0;
            board.MotionEventPublished+=e=>{if(e.Kind==GameplayMotionKind.ReshuffleStarted)started=true;if(e.Kind==GameplayMotionKind.ReshuffleCompleted)completed=true;};
            GameState.Instance.AddBooster(BoosterType.BogatyrMace,1);
            board.UseBooster(BoosterType.BogatyrMace);Assert.That(board.TapCell(0,0),Is.True);
            float deadline=Time.realtimeSinceStartup+15;
            while(board.HasActiveCommand&&Time.realtimeSinceStartup<deadline)
            {
                if(started&&!completed)
                {
                    Assert.That(board.CanAcceptPointerInput,Is.False);Assert.That(board.TapCell(1,1),Is.False);
                    var rt=(RectTransform)board.GetCellInputForTesting(0,2).transform.Find("CellContent/GemVisual");
                    if(rt.anchoredPosition.sqrMagnitude>4)moved=true;
                    observedFrames++;
                    var crate=(RectTransform)board.GetCellInputForTesting(7,6).transform.Find("CellContent/Obstacle");
                    Assert.That(crate.anchoredPosition,Is.EqualTo(new Vector2(0,0)));
                }
                yield return null;
            }
            Assert.That(board.HasActiveCommand,Is.False);Assert.That(started&&completed&&moved,Is.True);Assert.That(observedFrames,Is.GreaterThan(3));
            Assert.That(board.Moves,Is.EqualTo(50),"Automatic reshuffle costs no extra turn.");
            var actual=(BoardModel)typeof(Match3Board).GetField("model",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(board);
            Assert.That(actual.HasLegalMove(),Is.True);Assert.That(actual.HasAnyMatches(),Is.False);
            Assert.That(actual.GetObstacle(new BoardPosition(7,6)).Layers,Is.EqualTo(2));
            Assert.That(board.BoardStateHash,Is.EqualTo(board.LastPlanFinalStateHash));
            for(int y=0;y<7;y++)for(int x=0;x<8;x++)
            {
                var rt=(RectTransform)board.GetCellInputForTesting(x,y).transform.Find("CellContent/GemVisual");
                Assert.That(rt.anchoredPosition,Is.EqualTo(Vector2.zero));Assert.That(rt.localScale,Is.EqualTo(Vector3.one));
                Assert.That(rt.GetComponent<CanvasGroup>().alpha,Is.EqualTo(1));
            }
        }

        void CloseVisibleModal()
        {
            AnimatedHotspot close=null;
            foreach(var hotspot in router.GameCanvas.GetComponentsInChildren<AnimatedHotspot>())
                if(hotspot.name=="Hotspot_Close"&&hotspot.isActiveAndEnabled)close=hotspot;
            Assert.That(close,Is.Not.Null);
            var pointer=new PointerEventData(EventSystem.current){pointerId=704,position=new Vector2(100,100)};
            close.OnPointerDown(pointer);close.OnPointerUp(pointer);close.OnPointerClick(pointer);
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_Gameplay" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active Pre-Level."); return null;
        }

        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            Canvas.ForceUpdateCanvases();
        }

        IEnumerator WaitForNormalButtonColors()
        {
            float deadline=Time.realtimeSinceStartup+2f;
            int stable=0;
            do
            {
                yield return null;
                bool ready=true;
                foreach(Button button in Root().GetComponentsInChildren<Button>())
                {
                    Color expected=button.colors.normalColor*button.colors.colorMultiplier;
                    Color actual=button.targetGraphic.canvasRenderer.GetColor();
                    if(Mathf.Max(Mathf.Abs(actual.r-expected.r),Mathf.Abs(actual.g-expected.g),
                        Mathf.Abs(actual.b-expected.b),Mathf.Abs(actual.a-expected.a))>.0001f)ready=false;
                }
                stable=ready?stable+1:0;
            } while(stable<2&&Time.realtimeSinceStartup<deadline);
            Assert.That(stable,Is.EqualTo(2),"Capture only after native disabled-to-normal button fades finish.");
        }

        [Serializable] sealed class CaptureInfo
        {
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_GAMEPLAY_CAPTURE_DIR");
            Assert.That(directory, Is.Not.Null.And.Not.Empty);
            Directory.CreateDirectory(directory);
            Canvas canvas = router.GameCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            AdaptiveCanvasScaler adaptive = canvas.GetComponent<AdaptiveCanvasScaler>();
            SafeAreaFitter safe = router.SafeArea.GetComponent<SafeAreaFitter>();
            RenderMode mode = canvas.renderMode; Camera oldCamera = canvas.worldCamera;
            float distance = canvas.planeDistance, scale = canvas.scaleFactor;
            bool se = scaler.enabled, ae = adaptive.enabled, fe = safe.enabled;
            RenderTexture active = RenderTexture.active;
            var target = new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);
            var cameraGo = new GameObject("GameplayCaptureCamera",typeof(Camera));
            var texture = new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try
            {
                target.Create(); scaler.enabled = false; adaptive.enabled = false; safe.enabled = false;
                Camera camera = cameraGo.GetComponent<Camera>(); camera.enabled = false;
                camera.transform.position = new Vector3(0,0,-100); camera.orthographic = true;
                camera.nearClipPlane = .1f; camera.farClipPlane = 1000;
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color32(4,13,24,255);
                camera.targetTexture = target; canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera; canvas.planeDistance = 100;
                adaptive.Apply(safeArea,size);
                canvas.scaleFactor = AdaptiveCanvasScaler.ResolveScale(new Vector2Int(Mathf.RoundToInt(safeArea.width),Mathf.RoundToInt(safeArea.height)),941,1672);
                safe.Apply(safeArea,size);
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Rebuild glyph snapping after the capture camera's responsive layout has settled.
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();

                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionGameplayView>(),camera,size,safeArea);
                inspect?.Invoke(camera);
                camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0); texture.Apply();
                if(size.x==438)
                {
                    var grid=Root().GetComponentInChildren<ProductionGameplayView>().Board.transform.Find("SharedGrid").GetComponent<RectTransform>();
                    for(int y=1;y<7;y++)for(int x=1;x<8;x++)
                    {
                        Vector3 world=grid.TransformPoint(new Vector3(grid.rect.xMin+grid.rect.width*x/8f,grid.rect.yMin+grid.rect.height*y/7f));
                        Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,world);int gold=0;
                        for(int dy=-2;dy<=2;dy++)for(int dx=-2;dx<=2;dx++)
                        {
                            Color c=texture.GetPixel(Mathf.RoundToInt(point.x)+dx,Mathf.RoundToInt(point.y)+dy);
                            if(c.r>.30f&&c.g>.18f&&c.b<c.r*.72f)gold++;
                        }
                        Assert.That(gold,Is.GreaterThanOrEqualTo(4),"Rendered junction "+x+","+y+" must remain visible at438px.");
                    }
                }
                string output = Path.Combine(directory,name);
                Assert.That(File.Exists(output + ".png"), Is.False, "Keep earlier evidence.");
                File.WriteAllBytes(output + ".png",texture.EncodeToPNG());
                File.WriteAllText(output + ".json",JsonUtility.ToJson(new CaptureInfo
                {
                    unityVersion = Application.unityVersion, graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                    language = DemoFeatureState.LanguageCode, width = size.x, height = size.y, safeArea = safeArea,
                    buttonCount = Root().GetComponentsInChildren<Button>(true).Length
                },true) + "\n");
            }
            finally
            {
                RenderTexture.active = active; canvas.renderMode = mode; canvas.worldCamera = oldCamera;
                canvas.planeDistance = distance; canvas.scaleFactor = scale;
                scaler.enabled = se; adaptive.enabled = ae; safe.enabled = fe;
                adaptive.ApplyNow(); safe.ApplyNow();
                UnityEngine.Object.Destroy(texture); UnityEngine.Object.Destroy(cameraGo);
                target.Release(); UnityEngine.Object.Destroy(target);
            }
        }

        static Rect Bounds(RectTransform rect,Camera camera)
        {
            var corners=new Vector3[4];rect.GetWorldCorners(corners);
            Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        static void AssertInside(Rect inner,Rect outer,string message)
        {
            Assert.That(inner.xMin,Is.GreaterThanOrEqualTo(outer.xMin-1),message);
            Assert.That(inner.yMin,Is.GreaterThanOrEqualTo(outer.yMin-1),message);
            Assert.That(inner.xMax,Is.LessThanOrEqualTo(outer.xMax+1),message);
            Assert.That(inner.yMax,Is.LessThanOrEqualTo(outer.yMax+1),message);
        }
        static GameObject RaycastCard(ProductionGameplayBoosterCard card,Vector2 point)
        {
            var hits=new List<RaycastResult>();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
            Assert.That(hits,Is.Not.Empty,card.name);
            Assert.That(hits[0].gameObject.GetComponentInParent<ProductionGameplayBoosterCard>(),Is.SameAs(card),card.name+" edge must reach its existing forwarding control.");
            Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.Null,"This point must exercise the card surface, outside the retained image button.");
            return hits[0].gameObject;
        }
        static void ClickCardEdge(ProductionGameplayBoosterCard card)
        {
            Canvas.ForceUpdateCanvases();
            var canvas=card.GetComponentInParent<Canvas>().rootCanvas;
            var camera=canvas.renderMode==RenderMode.ScreenSpaceOverlay?null:canvas.worldCamera;
            Rect rect=Bounds(card.GetComponent<RectTransform>(),camera);
            Vector2 point=new Vector2(rect.center.x,Mathf.Lerp(rect.yMin,rect.yMax,.03f));
            var target=RaycastCard(card,point);
            var tap=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};
            ExecuteEvents.ExecuteHierarchy(target,tap,ExecuteEvents.pointerDownHandler);
            ExecuteEvents.ExecuteHierarchy(target,tap,ExecuteEvents.pointerUpHandler);
            ExecuteEvents.ExecuteHierarchy(target,tap,ExecuteEvents.pointerClickHandler);
        }
        [UnityTest] public IEnumerator LargerBoosterNamesPreserveTheBoardAndForwardCaptionInput()
        {
            GameState.Instance.SelectLevel(1,8);router.Show(ScreenId.Gameplay);Settle();yield return null;
            var view=Root().GetComponentInChildren<ProductionGameplayView>();var originalButtons=(Button[])view.BoosterButtons.Clone();
            string board=view.Board.BoardStateHash;var save=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));save.lastEnergyUtcTicks=0;string before=JsonUtility.ToJson(save);GameObject target=null;
            foreach(var language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);view.RefreshContent();yield return null;yield return null;
                foreach(var size in new[]{new Vector2Int(360,640),new Vector2Int(576,1280),new Vector2Int(941,1672),new Vector2Int(1080,2400)})
                {
                    Rect safe=size.x==360?new Rect(0,20,360,600):size.x==576?new Rect(0,48,576,1172):size.x==941?new Rect(0,0,941,1672):new Rect(45,100,990,2160);
                    Capture("CaptionNames_"+language+"_"+size.x,size,safe,camera=>
                    {
                        var rows=new List<CoreCaptionAssertions.Row>();
                        for(int i=0;i<4;i++)rows.Add(CoreCaptionAssertions.Check(view.Footer.Find("BoosterCard_"+i+"/BoosterLabel").GetComponent<Text>(),camera,8.1f));
                        var caption=view.Footer.Find("BoosterCard_2/CaptionBacking").GetComponent<ProductionHomePanel>();
                        Rect rect=CoreCaptionAssertions.Bounds(caption.rectTransform,camera);
                        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                        Assert.That(hits,Is.Not.Empty);Assert.That(hits[0].gameObject.GetComponentInParent<ProductionGameplayBoosterCard>().Target,Is.SameAs(originalButtons[2]));target=hits[0].gameObject;
                        CoreCaptionAssertions.Save("Gameplay_"+language+"_"+size.x,rows);
                    });
                }
            }
            CoreOutcomeTouchAssertions.Click(target);yield return null;
            Assert.That(view.Board.ArmedBooster,Is.EqualTo(BoosterType.LivingLight));view.Board.CancelArmedBooster(showToast:false);
            Assert.That(view.BoosterButtons,Is.EqualTo(originalButtons));Assert.That(view.Board.BoardStateHash,Is.EqualTo(board));
            save=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));save.lastEnergyUtcTicks=0;Assert.That(JsonUtility.ToJson(save),Is.EqualTo(before));
        }
        static void VerifyBoosterCards(ProductionGameplayView view,Camera camera,Rect safe)
        {
            for(int i=0;i<view.BoosterButtons.Length;i++)
            {
                var card=view.Footer.Find("BoosterCard_"+i).GetComponent<ProductionGameplayBoosterCard>();
                Assert.That(card.Target,Is.SameAs(view.BoosterButtons[i]));
                Rect cardBounds=Bounds(card.GetComponent<RectTransform>(),camera);
                AssertInside(cardBounds,safe,card.name+" must fit the safe area.");
                var label=card.transform.Find("BoosterLabel").GetComponent<Text>();
                Rect labelBounds=Bounds(label.rectTransform,camera);
                var caption=card.transform.Find("CaptionBacking").GetComponent<ProductionHomePanel>();
                Rect captionBounds=Bounds(caption.rectTransform,camera);
                AssertInside(captionBounds,safe,label.text+" caption must fit the safe area.");
                AssertInside(labelBounds,captionBounds,label.text+" label must remain inside its native caption face.");
                Assert.That(captionBounds.Overlaps(cardBounds),Is.False,"Names occupy the existing gap above the unchanged cards.");
                Assert.That(captionBounds.Overlaps(Bounds(view.BoardArea.Find("BoardFrame").GetComponent<RectTransform>(),camera)),Is.False,"Names must clear the board frame.");
                Assert.That(label.text,Is.Not.Empty);
                Assert.That(label.cachedTextGenerator.lineCount,Is.InRange(1,3),label.text);
                Assert.That(label.cachedTextGenerator.characterCountVisible,Is.GreaterThanOrEqualTo(label.text.Replace("\n","").Length),"All name characters must survive wrapping.");
                Assert.That(label.preferredHeight,Is.LessThanOrEqualTo(label.rectTransform.rect.height+1),label.text+" must fit without vertical truncation.");
                Assert.That(label.GetComponent<ProductionGameplayCaptionFit>().LastScale,Is.GreaterThanOrEqualTo(.98f),"Larger names must not silently shrink back into the old well.");
                var textMesh=label.canvasRenderer.GetMesh();
                Assert.That(textMesh.vertexCount,Is.GreaterThan(0),label.text);
                var glyphBounds=textMesh.bounds;
                var a=RectTransformUtility.WorldToScreenPoint(camera,label.rectTransform.TransformPoint(glyphBounds.min));
                var b=RectTransformUtility.WorldToScreenPoint(camera,label.rectTransform.TransformPoint(glyphBounds.max));
                AssertInside(Rect.MinMaxRect(a.x,a.y,b.x,b.y),labelBounds,label.text+" rendered lettering must stay inside its label well.");
                AssertInside(Rect.MinMaxRect(a.x,a.y,b.x,b.y),captionBounds,"Actual name ink must remain on its lacquer face.");
                Rect badge=Bounds(view.BoosterCounts[i].transform.parent.GetComponent<RectTransform>(),camera);
                AssertInside(badge,cardBounds,card.name+" stock badge must remain inside its card.");
                Assert.That(badge.Overlaps(labelBounds),Is.False,card.name+" stock badge must not cover the live label.");
                foreach(var point in new[]{new Vector2(Mathf.Lerp(cardBounds.xMin,cardBounds.xMax,.02f),cardBounds.center.y),
                    new Vector2(Mathf.Lerp(cardBounds.xMin,cardBounds.xMax,.98f),cardBounds.center.y),
                    new Vector2(cardBounds.center.x,Mathf.Lerp(cardBounds.yMin,cardBounds.yMax,.02f)),
                    new Vector2(cardBounds.center.x,Mathf.Lerp(cardBounds.yMin,cardBounds.yMax,.98f))})RaycastCard(card,point);
                var marker=view.ArmedMarkers[i];
                if(marker.gameObject.activeInHierarchy)
                {
                    Assert.That(marker.enabled,Is.True);Assert.That(marker.canvasRenderer.cull,Is.False);
                    Assert.That(marker.raycastTarget,Is.False);
                    Assert.That(marker.transform.parent,Is.SameAs(card.transform));
                    Assert.That(marker.transform.GetSiblingIndex(),Is.LessThan(view.BoosterButtons[i].transform.GetSiblingIndex()),"Armed feedback must render behind the isolated object.");
                    var mesh=marker.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                    Assert.That(Array.Exists(mesh.colors32,c=>c.a>0),Is.True,"Armed feedback must have visible geometry.");
                }
            }
        }
        static void VerifyHudReadability(ProductionGameplayView view,Camera camera)
        {
            var texts=new List<Text>{view.MovesHeading,view.MovesValue,view.GoalsHeading,view.ScoreHeading,view.ScoreValue,view.LevelText};
            Rect parchment=Bounds(view.Hud.Find("MissionParchment").GetComponent<RectTransform>(),camera);
            foreach(var goal in view.Goals)
            {
                if(!goal.Root.gameObject.activeSelf)continue;
                texts.Add(goal.Count);Rect icon=Bounds(goal.Icon.rectTransform,camera),count=Bounds(goal.Count.rectTransform,camera);
                AssertInside(icon,parchment,"Mission icon must stay inside parchment.");
                AssertInside(count,parchment,"Mission count must stay inside parchment.");
                Assert.That(icon.Overlaps(count),Is.False,"Mission icon and count need separate space.");
                Assert.That(Mathf.Abs(icon.center.x-count.center.x),Is.LessThan(1),"Mission count must align with its icon.");
            }
            foreach(var text in texts)
            {
                var mesh=text.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0),text.name);
                Vector2 low=RectTransformUtility.WorldToScreenPoint(camera,text.rectTransform.TransformPoint(mesh.bounds.min));
                Vector2 high=RectTransformUtility.WorldToScreenPoint(camera,text.rectTransform.TransformPoint(mesh.bounds.max));
                AssertInside(Rect.MinMaxRect(low.x,low.y,high.x,high.y),Bounds(text.rectTransform,camera),text.name+" glyphs must fit their live text area.");
            }
        }
        void VerifyLayoutAndInput(ProductionGameplayView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.ActionsBound,Is.True);
            foreach(var star in view.Stars)
            {
                Assert.That(star.GetComponent<ProductionGameplayStarPolish>(),Is.Not.Null);
                var mesh=star.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                var expected=star.Dimmed?star.StarEmptyArt:star.StarFilledArt;
                Assert.That(expected,Is.Not.Null,"Gameplay stars must use the extracted reference state.");
                Assert.That(star.ActiveStarArt,Is.SameAs(expected));
                Assert.That(star.mainTexture,Is.SameAs(expected.texture));
                Assert.That(mesh.vertexCount,Is.EqualTo(4),"Source star should keep its textured quad.");

            }
            Assert.That(view.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(5));
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            Rect environment=Bounds(view.Environment.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Rect board=Bounds(view.BoardMount,camera);
            Rect movesFrame=Bounds(view.Hud.Find("MovesMedallion").GetComponent<RectTransform>(),camera);
            Rect movesText=Bounds(view.MovesValue.rectTransform,camera);
            Assert.That(Mathf.Abs(movesFrame.center.x-movesText.center.x),Is.LessThan(2.5f),"The moves number must remain centered in its medallion.");
            Assert.That(view.Board.ColumnCount,Is.EqualTo(8));Assert.That(view.Board.RowCount,Is.EqualTo(7));
            Assert.That(board.width/board.height,Is.EqualTo(808f/748).Within(.001f),"The 8×7 board uses slightly taller cells to match the reference proportions.");
            Assert.That(board.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1));Assert.That(board.xMax,Is.LessThanOrEqualTo(safe.xMax+1));
            Assert.That(board.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1));Assert.That(board.yMax,Is.LessThanOrEqualTo(safe.yMax+1));
            Assert.That(view.GoalProgress.sprite,Is.Null);Assert.That(view.StarProgress.sprite,Is.Null);
            Assert.That(view.GoalProgress.gameObject.activeSelf,Is.False);Assert.That(view.StarProgress.gameObject.activeSelf,Is.False);
            float expectedGoalProgress=0;
            for(int i=0;i<view.Board.GoalCount;i++)
            {
                var goal=view.Board.GetGoalSlot(i);
                expectedGoalProgress+=1-Mathf.Clamp01(goal.Remaining/(float)goal.Target);
            }
            Assert.That(view.GoalProgress.fillAmount,Is.EqualTo(expectedGoalProgress/view.Board.GoalCount).Within(.0001f));
            var targetScore=view.Board.CurrentLevel.starScoreThresholds[2];
            Assert.That(view.StarProgress.fillAmount,Is.EqualTo(Mathf.Clamp01(view.Board.Score/(float)targetScore)).Within(.0001f));
            Assert.That(view.ScoreMeter.Fraction,Is.EqualTo(view.StarProgress.fillAmount).Within(.0001f));
            Assert.That(view.ScoreMeter.canvasRenderer.GetMesh().vertexCount,Is.GreaterThanOrEqualTo(18));
            VerifyBoosterCards(view,camera,safe);
            VerifyHudReadability(view,camera);
            if(view.Status.IsVisible)
            {
                Rect statusRect=Bounds(view.Status.Panel,camera);
                Assert.That(statusRect.Overlaps(board),Is.False,"Messages must not cover board cells.");
                Assert.That(statusRect.yMax,Is.LessThanOrEqualTo(safe.yMax+1));
                Assert.That(statusRect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1));
                foreach(var graphic in view.Status.Panel.GetComponentsInChildren<Graphic>())Assert.That(graphic.raycastTarget,Is.False);
                Assert.That(view.Status.Label.cachedTextGenerator.lineCount,Is.LessThanOrEqualTo(2));
                Assert.That(view.Status.Label.preferredHeight,Is.LessThanOrEqualTo(view.Status.Label.rectTransform.rect.height+1));
                foreach(var button in view.BoosterButtons)Assert.That(statusRect.Overlaps(Bounds(button.GetComponent<RectTransform>(),camera)),Is.False);
            }
            foreach(Button button in view.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                Assert.That(rect.Overlaps(board),Is.False,button.name+" overlaps playable board");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits,Is.Not.Empty,button.name);Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name);
            }
            var cellInputs=new HashSet<Match3CellInput>();
            for(int y=0;y<view.Board.RowCount;y++)for(int x=0;x<view.Board.ColumnCount;x++)
            {
                var cell=view.Board.GetCellInputForTesting(x,y);Rect rect=Bounds((RectTransform)cell.transform,camera);
                Assert.That(cellInputs.Add(cell),Is.True,"Each logical position must own a distinct input cell.");
                float width=board.width/8,height=board.height/7;
                Assert.That(rect.xMin,Is.EqualTo(board.xMin+x*width).Within(.2f));
                Assert.That(rect.yMax,Is.EqualTo(board.yMax-y*height).Within(.2f));
                Assert.That(rect.width,Is.EqualTo(width).Within(.2f));Assert.That(rect.height,Is.EqualTo(height).Within(.2f));
                // Artwork padding must never shrink the full-cell touch target, including the four corners.
                foreach(var point in new[]{rect.center,
                    new Vector2(rect.xMin+width*.02f,rect.yMin+height*.02f),
                    new Vector2(rect.xMin+width*.02f,rect.yMax-height*.02f),
                    new Vector2(rect.xMax-width*.02f,rect.yMin+height*.02f),
                    new Vector2(rect.xMax-width*.02f,rect.yMax-height*.02f)})
                {
                    var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                    Assert.That(hits,Is.Not.Empty,cell.name);Assert.That(hits[0].gameObject.GetComponent<Match3CellInput>(),Is.SameAs(cell),cell.name);
                }
                var graphic=cell.GetComponentInChildren<ProductionGameplayCell>();Assert.That(graphic.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(0));
                var gem=cell.transform.Find("CellContent/GemVisual/Gem").GetComponent<Image>();
                if(gem.enabled&&gem.sprite!=null&&gem.sprite.name.StartsWith("gem_",StringComparison.Ordinal))
                {
                    Assert.That(gem.preserveAspect,Is.True,"Rectangular board cells must not stretch the gem artwork.");
                    var mesh=gem.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                    var rendered=mesh.bounds;
                    Assert.That(rendered.size.x/rendered.size.y,Is.EqualTo(gem.sprite.rect.width/gem.sprite.rect.height).Within(.002f),gem.sprite.name);
                    var min=RectTransformUtility.WorldToScreenPoint(camera,gem.rectTransform.TransformPoint(rendered.min));
                    var max=RectTransformUtility.WorldToScreenPoint(camera,gem.rectTransform.TransformPoint(rendered.max));
                    Assert.That(max.x-min.x,Is.LessThanOrEqualTo(rect.width*.90f+.001f),"Normal gems need horizontal breathing room.");
                    Assert.That(max.y-min.y,Is.LessThanOrEqualTo(rect.height*.90f+.001f),"Normal gems need vertical breathing room.");
                    Assert.That(min.x,Is.GreaterThan(rect.xMin));Assert.That(max.x,Is.LessThan(rect.xMax));
                    Assert.That(min.y,Is.GreaterThan(rect.yMin));Assert.That(max.y,Is.LessThan(rect.yMax));
                }
            }
            Assert.That(cellInputs.Count,Is.EqualTo(56));
            foreach(var star in view.Stars)Assert.That(star.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(0));
            foreach(var badge in view.GetComponentsInChildren<ProductionPreLevelBadge>())Assert.That(badge.canvasRenderer.GetMesh().vertexCount,Is.GreaterThan(100));
            var layout=view.GetComponent<ProductionGameplayLayout>();Vector2 old=view.BoardArea.anchoredPosition;layout.ApplyNow();Canvas.ForceUpdateCanvases();Assert.That(Vector2.Distance(old,view.BoardArea.anchoredPosition),Is.LessThan(.01f));
        }
    }
}
