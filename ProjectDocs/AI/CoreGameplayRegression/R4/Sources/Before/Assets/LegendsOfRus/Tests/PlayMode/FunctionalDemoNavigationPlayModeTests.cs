using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.EventSystems;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class FunctionalDemoNavigationPlayModeTests
    {
        GameObject ownedRouterObject;
        ScreenRouter router;
        bool reusedRouter;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated))
                Assert.Ignore("Run navigation checks with isolated XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT.");
            isolated = Path.GetFullPath(isolated).TrimEnd(Path.DirectorySeparatorChar);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(
                isolated + Path.DirectorySeparatorChar, StringComparison.Ordinal), Is.True);
            router = ScreenRouter.Instance;
            if (router == null)
            {
                ownedRouterObject = new GameObject(
                    "Step20FunctionalQaRouter",
                    typeof(ScreenRouter)
                );
                router = ownedRouterObject.GetComponent<ScreenRouter>();
                router.Initialize();
            }
            else
            {
                reusedRouter = true;
            }

            DemoFeatureState.ResetSession();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DemoFeatureState.ResetSession();

            if (ownedRouterObject != null)
                UnityEngine.Object.Destroy(ownedRouterObject);
            else if (reusedRouter && router != null)
                router.Show(ScreenId.Home);

            router = null;
            reusedRouter = false;
            yield return null;
        }

        [UnityTest]
        public IEnumerator MapBackButtons_FollowChapterMapWorldMapHomeHierarchy()
        {
            router.Show(ScreenId.ChapterMap);
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap));

            GameObject chapterMap = GameObject.Find("Screen_ChapterMap");
            Assert.That(chapterMap, Is.Not.Null);
            var alignedChapter=chapterMap.GetComponentInChildren<ProductionChapterMapView>();
            Assert.That(alignedChapter,Is.Not.Null);
            UnityEngine.UI.Button chapterBack=alignedChapter.BackButton;
            Assert.That(chapterBack,Is.Not.Null);
            Assert.That(chapterBack.GetComponent<RectTransform>().rect.width,Is.GreaterThanOrEqualTo(100));
            chapterBack.onClick.Invoke();
            TickRouter(8);
            Assert.That(
                router.CurrentScreen,
                Is.EqualTo(ScreenId.WorldMap),
                "Visible Chapter Map Back must return to World Map."
            );

            GameObject worldMap = GameObject.Find("Screen_WorldMap");
            Assert.That(worldMap, Is.Not.Null);
            ProductionWorldMapView alignedMap = worldMap.GetComponentInChildren<ProductionWorldMapView>();
            Assert.That(alignedMap, Is.Not.Null);
            UnityEngine.UI.Button worldBack = alignedMap.BackButton;
            Assert.That(worldBack, Is.Not.Null);

            Canvas.ForceUpdateCanvases();
            Rect worldBackRect = worldBack.GetComponent<RectTransform>().rect;
            Assert.That(worldBackRect.width, Is.GreaterThanOrEqualTo(100f));
            // The extracted Back face is 91 design units high. Check its phone-size
            // target instead of the obsolete square-hotspot dimensions.
            Assert.That(worldBackRect.height * 576f / 941f, Is.GreaterThanOrEqualTo(44f));

            worldBack.onClick.Invoke();
            TickRouter(8);
            Assert.That(
                router.CurrentScreen,
                Is.EqualTo(ScreenId.Home),
                "Visible World Map Back must return to Home."
            );

            yield return null;
        }

        [UnityTest]
        public IEnumerator PreLevelVisibleBackButton_ReturnsToChapterMap()
        {
            router.Show(ScreenId.PreLevel);
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.PreLevel));

            GameObject preLevel = GameObject.Find("Screen_PreLevel");
            Assert.That(preLevel, Is.Not.Null);
            var view=preLevel.GetComponentInChildren<ProductionPreLevelView>();
            Assert.That(view,Is.Not.Null);
            Assert.That(view.BackButton.GetComponent<ProductionHomeButtonFeedback>().Role,Is.EqualTo(MetaUiControlRole.Back));
            view.BackButton.onClick.Invoke();
            TickRouter(8);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ExtendedDemoRoutes_RenderWithAtLeastOneInteraction()
        {
            ScreenId[] routes =
            {
                ScreenId.SplashTitle,
                ScreenId.EventDetail,
                ScreenId.IndividualRanking,
                ScreenId.TeamHub,
                ScreenId.TeamRanking,
                ScreenId.TeamChat,
                ScreenId.FriendsList,
                ScreenId.Inventory,
                ScreenId.SpecialOffer,
                ScreenId.LanguageSelect,
                ScreenId.Login,
                ScreenId.TutorialOverlay,
                ScreenId.OfflineState,
                ScreenId.MaintenanceState
            };

            for (int i = 0; i < routes.Length; i++)
            {
                ScreenId id = routes[i];
                router.Show(id);
                TickRouter(12);

                Assert.That(router.CurrentScreen, Is.EqualTo(id), "route " + id);
                GameObject root = GameObject.Find("Screen_" + id);
                Assert.That(root, Is.Not.Null, "root " + id);

                CanvasGroup group = root.GetComponent<CanvasGroup>();
                Assert.That(group, Is.Not.Null, "canvas group " + id);
                Assert.That(group.alpha, Is.EqualTo(1f).Within(0.001f), "visible " + id);
                Assert.That(group.blocksRaycasts, Is.True, "raycasts " + id);

                Canvas.ForceUpdateCanvases();
                Button[] buttons = root.GetComponentsInChildren<Button>();
                Assert.That(buttons.Any(button => button.isActiveAndEnabled && button.IsInteractable()
                    && button.targetGraphic != null && button.targetGraphic.raycastTarget
                    && button.GetComponent<RectTransform>().rect.width > 0
                    && button.GetComponent<RectTransform>().rect.height > 0), Is.True,
                    "A visible native button must provide interaction on " + id);
                Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true), Is.Empty,
                    "Rebuilt pages must not fall back to screenshot hotspots: " + id);
                yield return null;
            }

            // Error is a modal over the current page, not a replacement page route.
            ScreenId underlying = router.CurrentScreen;
            router.Show(ScreenId.EmptyErrorState);
            TickRouter(12);
            yield return null;
            Assert.That(router.CurrentScreen, Is.EqualTo(underlying));
            Assert.That(router.ActiveModal, Is.EqualTo(ScreenId.EmptyErrorState));
            Assert.That(router.HasActiveModalBlocker, Is.True);
            var error = router.Viewport.GetComponentInChildren<ProductionErrorDialogView>();
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Close.IsInteractable(), Is.True);
            error.Close.onClick.Invoke();
            TickRouter(12);
            yield return null;
            Assert.That(router.ActiveModal, Is.Null);
            Assert.That(router.CurrentScreen, Is.EqualTo(underlying));
        }

        [UnityTest]
        public IEnumerator NavigateBack_FriendsListReturnsToTeamHub()
        {
            router.Show(ScreenId.TeamHub);
            TickRouter(12);
            var team = router.Viewport.GetComponentInChildren<ProductionTeamHubView>();
            Assert.That(team, Is.Not.Null);
            team.FriendsButton.onClick.Invoke();
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.FriendsList));

            router.NavigateBack();
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.TeamHub));
            yield return null;
        }

        [UnityTest] public IEnumerator NativeJourneyWinsAndReturnsHome() { yield return Journey(true); }
        [UnityTest] public IEnumerator NativeJourneyFailsRetriesAndExitsHome() { yield return Journey(false); }
        T View<T>() where T:Component => router.Viewport.GetComponentInChildren<T>();
        string journey; int captureIndex;
        [UnityTest]
        public IEnumerator TextClarityRepresentativePages()
        {
            GameState.Instance.ResetProgress();
            foreach (string language in new[] { "ru", "en" })
            {
                DemoFeatureState.SetLanguage(language);
                journey = "TextClarity_" + language; captureIndex = 0;
                foreach (ScreenId page in new[] { ScreenId.Settings, ScreenId.Profile, ScreenId.FriendsList,
                    ScreenId.Mail, ScreenId.Inventory, ScreenId.Events, ScreenId.TeamHub, ScreenId.Characters })
                {
                    router.Show(page);
                    yield return Step(page.ToString());
                    Assert.That(router.CurrentScreen, Is.EqualTo(page));
                    Assert.That(router.GameCanvas.GetComponentsInChildren<Text>().Length, Is.GreaterThan(0));
                }
            }
        }

        // These two phases must run in separate Editor processes sharing only isolated storage.
        [UnityTest]
        public IEnumerator PersistenceWriterCompletesAndSaves()
        {
            string checkpoint = PersistenceCheckpoint("write");
            yield return Journey(true);
            var state = GameState.Instance;
            state.Save = SaveService.Load();
            Assert.That(state.GetLevelProgress(1, 1).completions, Is.EqualTo(1));
            journey = "PersistWrite"; captureIndex = 0;
            router.Show(ScreenId.Settings);
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                yield return Step("Setting" + i, () => View<ProductionSettingsView>().SwitchButtons[index]);
                Assert.That(View<ProductionSettingsView>().Preference(index), Is.False);
            }
            DemoFeatureState.SelectLanguage("en");
            Assert.That(state.TryUseBooster(BoosterType.BogatyrMace), Is.True);
            Assert.That(state.SpendGems(17), Is.True);
            Assert.That(state.SelectLevel(1, 2), Is.True);
            Assert.That(state.LastWriteResult.Success, Is.True);
            router.Show(ScreenId.Home); yield return Step("SavedHome");
            File.WriteAllText(checkpoint, JsonUtility.ToJson(state.Save, true));
            File.WriteAllText(checkpoint + ".pid", System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
        }

        [UnityTest]
        public IEnumerator PersistenceReaderRestoresWithoutDuplicateReward()
        {
            string checkpoint = PersistenceCheckpoint("read");
            Assert.That(File.Exists(checkpoint), Is.True, "Run writer in a separate process first");
            Assert.That(File.ReadAllText(checkpoint + ".pid"), Is.Not.EqualTo(System.Diagnostics.Process.GetCurrentProcess().Id.ToString()));
            var expected = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(checkpoint));
            var state = GameState.Instance;
            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)), "Durable journal payload");
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)), "Fresh GameState initialization");
            Assert.That(state.CanPersist, Is.True);
            Assert.That(DemoFeatureState.LanguageCode, Is.EqualTo("en"));
            Assert.That(state.LastCompletionResult, Is.Null, "Transient result is not a saved board/session");
            Assert.That(state.HasCompletedStoryScene("chapter01.firebird_found"), Is.True);
            journey = "PersistRead"; captureIndex = 0;
            router.Show(ScreenId.Home); yield return Step("RestoredHome");
            router.Show(ScreenId.Settings); yield return Step("RestoredSettings");
            for (int i = 0; i < 4; i++) Assert.That(View<ProductionSettingsView>().Preference(i), Is.False);
            router.Show(ScreenId.ChapterMap); yield return Step("RestoredMap");
            int gold = state.Save.gold, gems = state.Save.gems, mace = state.Save.boosterMace;
            var repeat = state.CompleteLevel(ChapterLevelCatalog.Find(1, 1), 7000);
            Assert.That(repeat.FirstClearRewardGranted, Is.False);
            Assert.That(state.Save.gold, Is.EqualTo(gold));
            Assert.That(state.Save.gems, Is.EqualTo(gems));
            Assert.That(state.Save.boosterMace, Is.EqualTo(mace));
            Assert.That(state.GetLevelProgress(1, 1).completions, Is.EqualTo(2));
            File.WriteAllText(checkpoint + ".read.json", "{\"separateProcess\":true,\"payloadRestored\":true,\"replayRewardDuplicated\":false}");
        }

        static string PersistenceCheckpoint(string phase)
        {
            if (Environment.GetEnvironmentVariable("LOR_PERSISTENCE_PHASE") != phase)
                Assert.Ignore("Run the isolated persistence audit writer/reader phases separately.");
            string checkpoint = Environment.GetEnvironmentVariable("LOR_PERSISTENCE_CHECKPOINT");
            Assert.That(checkpoint, Is.Not.Null.And.Not.Empty);
            return checkpoint;
        }

        [UnityTest]
        public IEnumerator LifecycleCallbacksPreserveBoardAndReleaseInterruptedTouch()
        {
            var state = GameState.Instance;
            state.ResetProgress(); StoryService.ResetRuntime();
            foreach (var level in ChapterLevelCatalog.LoadChapter1().levels)
                foreach (var step in level.tutorialSteps) state.CompleteTutorial(step.id);
            journey = "Lifecycle"; captureIndex = 0;
            router.Show(ScreenId.PreLevel);
            yield return Step("PreLevel", () => View<ProductionPreLevelView>().PlayButton);
            var board = View<ProductionGameplayView>().Board;
            int energy = state.Save.energy, moves = board.Moves, score = board.Score;
            string hash = board.BoardStateHash;
            yield return Step("BeforeInterruption");
            foreach (bool focusLoss in new[] { false, true })
            {
                var cell = board.GetCellInputForTesting(0, 0);
                var rect = board.GetCellRectForTesting(0, 0);
                var point = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center));
                var pointer = new PointerEventData(EventSystem.current) { pointerId = 731, position = point, pressPosition = point, button = PointerEventData.InputButton.Left };
                cell.OnPointerDown(pointer);
                Assert.That(board.OwnsPointer(pointer.pointerId), Is.True);
                SendLifecycle(focusLoss ? "OnApplicationFocus" : "OnApplicationPause", !focusLoss);
                Assert.That(board.OwnsPointer(pointer.pointerId), Is.False);
                yield return null;
                SendLifecycle(focusLoss ? "OnApplicationFocus" : "OnApplicationPause", focusLoss);
                pointer.position += new Vector2(rect.rect.width, 0);
                cell.OnPointerUp(pointer);
                Assert.That(board.RequestedCommandCount, Is.Zero, "Stale release must not swap");
                Assert.That(board.BoardStateHash, Is.EqualTo(hash));
                Assert.That(board.Moves, Is.EqualTo(moves));
                Assert.That(board.Score, Is.EqualTo(score));
                Assert.That(state.Save.energy, Is.EqualTo(energy));
            }
            yield return Step("AfterCallbacks", () => View<ProductionGameplayView>().PauseButton);
            SendLifecycle("OnApplicationPause", true); yield return null; SendLifecycle("OnApplicationPause", false);
            Assert.That(router.ActiveModal, Is.EqualTo(ScreenId.Pause));
            Assert.That(board.CanAcceptPointerInput, Is.False);
            yield return Step("PausedAfterResume", () => View<ProductionPauseView>().Actions[0]);
            Assert.That(board.BoardStateHash, Is.EqualTo(hash));
            var plan = ChooseMove(board, true);
            Assert.That(plan, Is.Not.Null);
            Assert.That(board.SwipeCell(plan.PrimaryPosition.X, plan.PrimaryPosition.Y, new Vector2Int(plan.SecondaryPosition.X - plan.PrimaryPosition.X, plan.SecondaryPosition.Y - plan.PrimaryPosition.Y)), Is.True);
            SendLifecycle("OnApplicationPause", true);
            yield return null;
            SendLifecycle("OnApplicationPause", false);
            float deadline = Time.realtimeSinceStartup + 15;
            while (!board.CanAcceptPointerInput && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(board.CanAcceptPointerInput, Is.True);
            Assert.That(board.RequestedCommandCount, Is.EqualTo(1));
            Assert.That(board.Moves, Is.EqualTo(moves - 1));
            Assert.That(state.Save.energy, Is.EqualTo(energy));
            Assert.That(View<ProductionGameplayView>().Board, Is.SameAs(board));
            yield return Step("ResolvedAfterResume");
        }

        static void SendLifecycle(string method, bool value)
        {
            // Explicit simulation of Unity messages, not an OS suspension or a process kill.
            foreach (var component in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                var callback = component.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (callback != null) callback.Invoke(component, new object[] { value });
            }
        }

        [Test]
        public void EnergyClockCatchupIsCappedAndNotRepeated()
        {
            var state = GameState.Instance;
            state.ResetProgress(); Assert.That(state.SpendEnergy(4), Is.True);
            state.Save.lastEnergyUtcTicks = DateTime.UtcNow.AddMinutes(-61).Ticks;
            state.RefreshEnergyFromClock(); Assert.That(state.Save.energy, Is.EqualTo(3));
            long after = state.Save.lastEnergyUtcTicks;
            state.RefreshEnergyFromClock(); Assert.That(state.Save.energy, Is.EqualTo(3));
            Assert.That(state.Save.lastEnergyUtcTicks, Is.EqualTo(after));
            Assert.That(SaveService.Load().energy, Is.EqualTo(3));
            state.Save.lastEnergyUtcTicks = DateTime.UtcNow.AddHours(-6).Ticks;
            state.RefreshEnergyFromClock(); Assert.That(state.Save.energy, Is.EqualTo(state.Save.energyMax));
            Assert.That(state.SpendEnergy(1), Is.True);
            state.Save.lastEnergyUtcTicks = DateTime.UtcNow.AddHours(1).Ticks;
            state.RefreshEnergyFromClock(); Assert.That(state.Save.energy, Is.EqualTo(4), "Clock rollback must not add energy");
        }

        IEnumerator SettleJourney(){TickRouter(24);Canvas.ForceUpdateCanvases();yield return null;yield return null;}
        IEnumerator Step(string name,Func<Button> control=null){
            yield return SettleJourney();CaptureJourney(name,control==null?null:control());yield return SettleJourney();
        }
        IEnumerator Journey(bool win){
            var state=GameState.Instance;string previous=JsonUtility.ToJson(state.Save),language=DemoFeatureState.LanguageCode;
            journey=win?"Win_RU":"Fail_EN";captureIndex=0;
            try{
                state.ResetProgress();StoryService.ResetRuntime();DemoFeatureState.SetLanguage(win?"ru":"en");
                // This journey exercises the core route; dedicated tutorial tests cover its teaching overlays.
                foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)foreach(var step in level.tutorialSteps)state.CompleteTutorial(step.id);
                router.Show(ScreenId.Home);yield return Step("Home",()=>View<ProductionHomeView>().WorldMapButton);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));
                yield return Step("WorldMap",()=>View<ProductionWorldMapView>().Cards[0].Button);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
                yield return Step("ChapterMap",()=>Array.Find(View<ProductionChapterMapView>().Nodes,n=>n.LevelId==1).Button);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Dialogue));
                for(int line=0;router.CurrentScreen==ScreenId.Dialogue;line++){Assert.That(line,Is.LessThan(4));yield return Step("Intro"+line,()=>View<ProductionDialogueView>().Continue);}
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(state.Save.selectedLevel,Is.EqualTo(1));
                if(!win){
                    state.SpendEnergy(state.Save.energy);yield return Step("EnergyGate",()=>View<ProductionPreLevelView>().PlayButton);
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));
                    yield return Step("OutOfEnergy",()=>View<ProductionOutOfEnergyView>().Close);Assert.That(router.ActiveModal,Is.Null);state.AddEnergy(2);
                }
                int energy=state.Save.energy,gold=state.Save.gold;
                yield return Step("PreLevel",()=>View<ProductionPreLevelView>().PlayButton);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(state.Save.energy,Is.EqualTo(energy-1));
                var board=View<ProductionGameplayView>().Board;string hash=board.BoardStateHash;int moves=board.Moves;
                yield return Step("Gameplay",()=>View<ProductionGameplayView>().PauseButton);
                Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));Assert.That(board.CanAcceptPointerInput,Is.False);
                yield return Step("Pause",()=>View<ProductionPauseView>().Actions[2]);Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Settings));
                yield return Step("Settings",()=>View<ProductionSettingsView>().BackButton);Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.Pause));
                yield return Step("Resume",()=>View<ProductionPauseView>().Actions[0]);Assert.That(router.ActiveModal,Is.Null);
                Assert.That(View<ProductionGameplayView>().Board,Is.SameAs(board));Assert.That(board.BoardStateHash,Is.EqualTo(hash));Assert.That(board.Moves,Is.EqualTo(moves));
                int commands=0;
                while(router.CurrentScreen==ScreenId.Gameplay && board && board.CanAcceptPointerInput){
                    Assert.That(++commands,Is.LessThanOrEqualTo(28));var plan=ChooseMove(board,win);Assert.That(plan,Is.Not.Null,"A legal move must exist");
                    Assert.That(board.SwipeCell(plan.PrimaryPosition.X,plan.PrimaryPosition.Y,new Vector2Int(plan.SecondaryPosition.X-plan.PrimaryPosition.X,plan.SecondaryPosition.Y-plan.PrimaryPosition.Y)),Is.True);
                    float deadline=Time.realtimeSinceStartup+15;
                    while(router.CurrentScreen==ScreenId.Gameplay&&board&&(board.HasActiveCommand||!board.CanAcceptPointerInput)&&Time.realtimeSinceStartup<deadline)yield return null;
                    Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline),"Board resolution timeout");yield return SettleJourney();
                }
                Assert.That(router.CurrentScreen,Is.EqualTo(win?ScreenId.Victory:ScreenId.Fail));Assert.That(state.Save.energy,Is.EqualTo(energy-1));
                if(win){
                    Assert.That(state.GetLevelProgress(1,1).completions,Is.EqualTo(1));Assert.That(state.Save.gold,Is.EqualTo(gold+ChapterLevelCatalog.Find(1,1).rewards.gold));
                    yield return Step("Victory",()=>View<ProductionVictoryView>().ContinueButton);
                    for(int line=0;router.CurrentScreen==ScreenId.Dialogue;line++){Assert.That(line,Is.LessThan(3));yield return Step("PostVictory"+line,()=>View<ProductionDialogueView>().Continue);}
                    Assert.That(state.HasCompletedStoryScene("chapter01.firebird_found"),Is.True);
                }else{
                    Assert.That(state.Save.gold,Is.EqualTo(gold));yield return Step("Fail",()=>View<ProductionFailView>().Retry);
                    Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(View<ProductionGameplayView>().Board,Is.Not.SameAs(board));
                    Assert.That(View<ProductionGameplayView>().Board.Moves,Is.EqualTo(28));Assert.That(state.Save.energy,Is.EqualTo(energy-1),"Existing retry restarts without an additional energy charge");
                    yield return Step("Retry",()=>View<ProductionGameplayView>().PauseButton);yield return Step("ExitRound",()=>View<ProductionPauseView>().Actions[3]);
                }
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
                yield return Step("ReturnChapter",()=>View<ProductionChapterMapView>().BackButton);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.WorldMap));yield return Step("ReturnWorld",()=>View<ProductionWorldMapView>().BackButton);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));yield return Step("ReturnHome");
                Assert.That(router.ActiveModal,Is.Null);Assert.That(state.Save.energy,Is.EqualTo(energy-1));
                File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR"),journey+"_Result.json"),JsonUtility.ToJson(new JourneyResult{language=DemoFeatureState.LanguageCode,outcome=win?"Victory":"Fail",legalMoves=commands,energyBefore=energy,energyAfter=state.Save.energy,goldBefore=gold,goldAfter=state.Save.gold},true));
            }finally{router.Show(ScreenId.Home);TickRouter(24);StoryService.ResetRuntime();state.Save=JsonUtility.FromJson<PlayerSaveData>(previous);DemoFeatureState.SetLanguage(language);}
        }
        [Serializable] sealed class JourneyResult{public string language,outcome;public int legalMoves,energyBefore,energyAfter,goldBefore,goldAfter;}
        static BoardResolutionPlan ChooseMove(Match3Board board,bool win){
            // Read-only access: the planner evaluates a copy, and the real board executes the selected legal swipe.
            var model=(BoardModel)typeof(Match3Board).GetField("model",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(board);string hash=board.BoardStateHash;
            var planner=new BoardResolutionPlanner(model,null,board.RuleVersion);BoardResolutionPlan best=null;float bestValue=float.NegativeInfinity;
            for(int y=0;y<model.Height;y++)for(int x=0;x<model.Width;x++)foreach(var direction in new[]{new BoardPosition(1,0),new BoardPosition(0,1)}){
                var a=new BoardPosition(x,y);var b=new BoardPosition(x+direction.X,y+direction.Y);if(!model.IsInside(b))continue;var plan=planner.PlanSwap(a,b);if(!plan.IsResolved||!plan.ConsumesMove)continue;
                int red=0,blue=0;foreach(var wave in plan.Waves){red+=wave.ClearedByTileType[0];blue+=wave.ClearedByTileType[1];}
                float value=win?1000*(Math.Min(red,board.RedRemaining)+Math.Min(blue,board.BlueRemaining))+plan.TotalScoreDelta:-10000*blue-plan.TotalScoreDelta;
                if(value>bestValue){bestValue=value;best=plan;}
            }
            Assert.That(board.BoardStateHash,Is.EqualTo(hash),"Move planning must not mutate runtime board");return best;
        }
        void CaptureJourney(string stage,Button button){
            string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");Assert.That(dir,Is.Not.Null.And.Not.Empty);Directory.CreateDirectory(dir);
            var size=new Vector2Int(576,1280);var safeRect=new Rect(0,48,576,1172);var canvas=router.GameCanvas;var scaler=canvas.GetComponent<CanvasScaler>();var adaptive=canvas.GetComponent<AdaptiveCanvasScaler>();var safe=router.SafeArea.GetComponent<SafeAreaFitter>();
            var mode=canvas.renderMode;var oldCamera=canvas.worldCamera;float distance=canvas.planeDistance,scale=canvas.scaleFactor;bool se=scaler.enabled,ae=adaptive.enabled,fe=safe.enabled;var active=RenderTexture.active;
            var target=new RenderTexture(size.x,size.y,24);var go=new GameObject("JourneyCaptureCamera",typeof(Camera));var texture=new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try{
                target.Create();scaler.enabled=adaptive.enabled=safe.enabled=false;var camera=go.GetComponent<Camera>();camera.enabled=false;camera.transform.position=new Vector3(0,0,-100);camera.orthographic=true;camera.nearClipPlane=.1f;camera.farClipPlane=1000;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;camera.targetTexture=target;
                canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=100;adaptive.Apply(safeRect,size);canvas.scaleFactor=AdaptiveCanvasScaler.ResolveScale(new Vector2Int(576,1172),941,1672);safe.Apply(safeRect,size);
                // RenderTexture captures change scale synchronously, before the usual frame update.
                // Refresh every live label at the final scale; otherwise old atlas glyphs are enlarged.
                foreach (Text label in canvas.GetComponentsInChildren<Text>(true)) label.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Responsive layouts can settle during that pass. Rebuild glyph snapping once more.
                foreach (Text label in canvas.GetComponentsInChildren<Text>(true)) label.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                camera.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0);texture.Apply();
                string name=journey+"_"+(captureIndex++).ToString("D2")+"_"+stage;File.WriteAllBytes(Path.Combine(dir,name+".png"),texture.EncodeToPNG());
                if(button){Assert.That(button.IsInteractable(),Is.True,stage);var r=(RectTransform)button.transform;Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,r.TransformPoint(r.rect.center));Assert.That(safeRect.Contains(point),Is.True,stage+" visible control");var data=new PointerEventData(EventSystem.current){position=point,button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),stage+" native hit");ExecuteEvents.Execute(button.gameObject,data,ExecuteEvents.pointerClickHandler);}
            }finally{RenderTexture.active=active;canvas.renderMode=mode;canvas.worldCamera=oldCamera;canvas.planeDistance=distance;canvas.scaleFactor=scale;scaler.enabled=se;adaptive.enabled=ae;safe.enabled=fe;adaptive.ApplyNow();safe.ApplyNow();UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(go);target.Release();UnityEngine.Object.Destroy(target);}
        }

        void TickRouter(int count)
        {
            Assert.That(router, Is.Not.Null);
            Assert.That(router.MotionPresenter, Is.Not.Null);
            for (int i = 0; i < count; i++)
                router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
        }

    }
}
