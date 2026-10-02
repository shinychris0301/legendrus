using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class CoreLoopMotionFeedbackPlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject stateObject;
        GameObject feedbackObject;
        GameObject eventSystemObject;
        GameObject canvasObject;
        GameObject boardObject;
        Match3Board board;
        CoreLoopFeedbackService feedback;
        bool ownsStateObject;
        bool ownsFeedbackObject;
        bool ownsEventSystemObject;
        GameState stateUnderTest;
        bool[] hadPersistedSaves;
        string[] persistedSaveJson;
        string runtimeSaveJson;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            persistedSaveJson = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                persistedSaveJson[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();
            yield return null;

            GameState state = GameState.Instance;
            if (state == null)
            {
                stateObject = new GameObject("MotionTestGameState");
                state = stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }

            stateUnderTest = state;
            runtimeSaveJson = JsonUtility.ToJson(state.Save);
            state.Save = new PlayerSaveData
            {
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks
            };
            state.Save.sfx = true;
            state.Save.vibration = true;
            state.Save.music = true;

            feedback = CoreLoopFeedbackService.Instance;
            if (feedback == null)
            {
                feedbackObject = new GameObject("MotionTestFeedback");
                feedback = feedbackObject.AddComponent<CoreLoopFeedbackService>();
                ownsFeedbackObject = true;
            }
            feedback.ResetSinks();

            if (EventSystem.current == null)
            {
                eventSystemObject = new GameObject(
                    "MotionTestEventSystem",
                    typeof(EventSystem)
                );
                ownsEventSystemObject = true;
            }

            canvasObject = new GameObject(
                "MotionTestCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            boardObject = new GameObject("MotionTestBoard", typeof(RectTransform));
            boardObject.transform.SetParent(canvasObject.transform, false);
            RectTransform boardRect = boardObject.GetComponent<RectTransform>();
            boardRect.anchorMin = new Vector2(0.5f, 0.5f);
            boardRect.anchorMax = new Vector2(0.5f, 0.5f);
            boardRect.pivot = new Vector2(0.5f, 0.5f);
            boardRect.sizeDelta = new Vector2(786f, 942f);

            board = boardObject.AddComponent<Match3Board>();
            board.Initialize(null, null);
            Canvas.ForceUpdateCanvases();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (feedback != null)
                feedback.ResetSinks();
            if (boardObject != null) UnityEngine.Object.Destroy(boardObject);
            if (canvasObject != null) UnityEngine.Object.Destroy(canvasObject);
            if (ownsEventSystemObject && eventSystemObject != null)
                UnityEngine.Object.Destroy(eventSystemObject);
            if (ownsFeedbackObject && feedbackObject != null)
                UnityEngine.Object.Destroy(feedbackObject);
            RestoreRuntimeSave();
            if (ownsStateObject && stateObject != null)
                UnityEngine.Object.Destroy(stateObject);
            yield return null;
            RestoreRuntimeSave();
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadPersistedSaves[i])
                    PlayerPrefs.SetString(key, persistedSaveJson[i]);
            }
            PlayerPrefs.Save();
        }

        [Test]
        public void MotionProfile_HasPositiveBoundedPresentationTimings()
        {
            Assert.That(CoreLoopMotionProfile.SwapSeconds, Is.GreaterThan(0f));
            Assert.That(CoreLoopMotionProfile.RejectedSwapSeconds, Is.GreaterThan(0f));
            Assert.That(CoreLoopMotionProfile.FallSeconds, Is.GreaterThan(0f));
            Assert.That(CoreLoopMotionProfile.SpecialCreationSeconds, Is.GreaterThan(0f));
            Assert.That(CoreLoopMotionProfile.SwapSeconds, Is.LessThan(0.5f));
            Assert.That(CoreLoopMotionProfile.EaseOutCubic(0f), Is.EqualTo(0f));
            Assert.That(CoreLoopMotionProfile.EaseOutCubic(1f), Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator BlockedSwap_EmitsOneRejectionAndSettlesWithoutMutation()
        {
            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            string beforeHash = board.BoardStateHash;
            int beforeMoves = board.Moves;
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            Assert.That(board.SwipeCell(0, 0, Vector2Int.right), Is.True);
            yield return WaitUntilInteractive();

            Assert.That(motions.Count, Is.EqualTo(1));
            Assert.That(motions[0].Kind, Is.EqualTo(GameplayMotionKind.SwapRejected));
            Assert.That(board.BoardStateHash, Is.EqualTo(beforeHash));
            Assert.That(board.Moves, Is.EqualTo(beforeMoves));
            Assert.That(board.HasActiveCommand, Is.False);
            AssertVisualsSettled();
        }

        [UnityTest]
        public IEnumerator InvalidSwap_EmitsForwardAndReturnTimeline()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            var kinds = new List<GameplayMotionKind>();
            board.MotionEventPublished += motion => kinds.Add(motion.Kind);

            Assert.That(board.SwipeCell(3, 4, Vector2Int.right), Is.True);
            yield return WaitUntilInteractive();

            CollectionAssert.AreEqual(
                new[]
                {
                    GameplayMotionKind.SwapStarted,
                    GameplayMotionKind.SwapCommitted,
                    GameplayMotionKind.SwapReturnStarted,
                    GameplayMotionKind.SwapReturned
                },
                kinds
            );
            AssertVisualsSettled();
        }

        [UnityTest]
        public IEnumerator SpecialCombination_EmitsOrderedActivationWaveAndFallHooks()
        {
            Assert.That(board.LoadQAPreset("special.row-column"), Is.True);
            BoardQAPresetFactory.TryGet("special.row-column", out BoardQAPreset preset);
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            Assert.That(
                board.SwipeCell(
                    preset.Primary.X,
                    preset.Primary.Y,
                    Delta(preset.Primary, preset.Secondary)
                ),
                Is.True
            );
            yield return WaitUntilInteractive();

            Assert.That(motions.Count, Is.GreaterThan(5));
            for (int i = 0; i < motions.Count; i++)
                Assert.That(motions[i].Sequence, Is.EqualTo(i + 1));

            int wave = IndexOf(motions, GameplayMotionKind.MatchWaveStarted);
            int activation = IndexOf(motions, GameplayMotionKind.SpecialActivationStarted);
            int fallStarted = IndexOf(motions, GameplayMotionKind.FallStarted);
            int fallCompleted = IndexOf(motions, GameplayMotionKind.FallCompleted);
            int complete = IndexOf(motions, GameplayMotionKind.MatchWaveCompleted);
            Assert.That(wave, Is.GreaterThanOrEqualTo(0));
            Assert.That(activation, Is.GreaterThan(wave));
            Assert.That(fallStarted, Is.GreaterThan(activation));
            Assert.That(fallCompleted, Is.GreaterThan(fallStarted));
            Assert.That(complete, Is.GreaterThan(fallCompleted));
            Assert.That(board.BoardStateHash, Is.EqualTo(board.LastPlanFinalStateHash));
            Assert.That(board.LastWaveVerificationPassed, Is.True);
            AssertVisualsSettled();
        }

        [UnityTest]
        public IEnumerator SamePreset_ProducesSameSemanticMotionTrace()
        {
            string first = null;
            string second = null;

            yield return CaptureTrace("special.row-column", value => first = value);
            yield return CaptureTrace("special.row-column", value => second = value);

            Assert.That(first, Is.EqualTo(second));
        }

        [UnityTest]
        public IEnumerator FourGemMatch_EmitsSpecialCreationHook()
        {
            BoardModel model = CreateSpecialCreationBoard();
            Assert.That(model.HasAnyMatches(), Is.False);
            Assert.That(board.LoadSnapshotForTesting(model.CreateSnapshot()), Is.True);
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            board.SwipeCell(3, 4, new Vector2Int(0, -1));
            yield return WaitUntilInteractive();

            int creation = IndexOf(motions, GameplayMotionKind.SpecialCreationStarted);
            int complete = IndexOf(motions, GameplayMotionKind.MatchWaveCompleted);
            Assert.That(creation, Is.GreaterThanOrEqualTo(0));
            Assert.That(complete, Is.GreaterThan(creation));
            Assert.That(motions[creation].SpecialKind, Is.Not.EqualTo(SpecialGemKind.None));
        }

        [UnityTest]
        public IEnumerator MultiWavePlan_EmitsCascadeHookWithIncreasingDepth()
        {
            BoardResolutionPlan plan = FindCascadePlan();
            Assert.That(plan, Is.Not.Null, "No deterministic cascade fixture was found.");
            Assert.That(board.LoadSnapshotForTesting(plan.BeforeSnapshot), Is.True);
            var motions = new List<GameplayMotionEvent>();
            board.MotionEventPublished += motions.Add;

            board.SwipeCell(
                plan.PrimaryPosition.X,
                plan.PrimaryPosition.Y,
                Delta(plan.PrimaryPosition, plan.SecondaryPosition)
            );
            yield return WaitUntilInteractive();

            var cascadeDepths = new List<int>();
            for (int i = 0; i < motions.Count; i++)
            {
                if (motions[i].Kind == GameplayMotionKind.CascadeStarted)
                    cascadeDepths.Add(motions[i].ComboIndex);
            }

            Assert.That(cascadeDepths.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(cascadeDepths[0], Is.EqualTo(2));
            for (int i = 1; i < cascadeDepths.Count; i++)
                Assert.That(cascadeDepths[i], Is.EqualTo(cascadeDepths[i - 1] + 1));
        }

        [UnityTest]
        public IEnumerator InputRemainsLockedForEveryResolvedMotionHook()
        {
            Assert.That(board.LoadQAPreset("special.row-column"), Is.True);
            BoardQAPresetFactory.TryGet("special.row-column", out BoardQAPreset preset);
            bool allLocked = true;
            int observed = 0;
            board.MotionEventPublished += motion =>
            {
                observed++;
                allLocked &= !board.CanAcceptPointerInput;
            };

            board.SwipeCell(
                preset.Primary.X,
                preset.Primary.Y,
                Delta(preset.Primary, preset.Secondary)
            );
            yield return WaitUntilInteractive();

            Assert.That(observed, Is.GreaterThan(0));
            Assert.That(allLocked, Is.True);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
        }

        [UnityTest]
        public IEnumerator CellHitRoots_CoverFormerHorizontalVerticalAndJunctionSeams()
        {
            Canvas.ForceUpdateCanvases();
            AssertRaycastHitsCell(SeamPoint(2, 3, 3, 3));
            AssertRaycastHitsCell(SeamPoint(4, 4, 4, 5));

            RectTransform a = board.GetCellRectForTesting(2, 3);
            RectTransform b = board.GetCellRectForTesting(3, 4);
            Vector3 aBottomRight = a.TransformPoint(
                new Vector2(a.rect.xMax, a.rect.yMin)
            );
            Vector3 bTopLeft = b.TransformPoint(
                new Vector2(b.rect.xMin, b.rect.yMax)
            );
            AssertRaycastHitsCell((aBottomRight + bTopLeft) * 0.5f);
            yield return null;
        }

        [UnityTest]
        public IEnumerator GestureStartedDuringResolution_CanCommitAfterBoardReturnsIdle()
        {
            Assert.That(board.LoadQAPreset("special.row-column"), Is.True);
            BoardQAPresetFactory.TryGet("special.row-column", out BoardQAPreset preset);
            board.SwipeCell(
                preset.Primary.X,
                preset.Primary.Y,
                Delta(preset.Primary, preset.Secondary)
            );
            Assert.That(board.CanAcceptPointerInput, Is.False);

            Match3CellInput input = board.GetCellInputForTesting(0, 0);
            PointerEventData pointer = CreatePointer(17, 0, 0);
            input.OnPointerDown(pointer);
            int before = board.RequestedCommandCount;

            yield return WaitUntilInteractive();
            MovePointer(pointer, 0, 0, new Vector2(0.45f, 0.04f));
            input.OnPointerUp(pointer);
            yield return WaitUntilInteractive();

            Assert.That(board.RequestedCommandCount, Is.EqualTo(before + 1));
        }

        [UnityTest]
        public IEnumerator ForeignPointer_CannotStealBoardGestureOwnership()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Match3CellInput first = board.GetCellInputForTesting(2, 2);
            Match3CellInput second = board.GetCellInputForTesting(4, 4);
            PointerEventData firstPointer = CreatePointer(31, 2, 2);
            PointerEventData secondPointer = CreatePointer(32, 4, 4);

            first.OnPointerDown(firstPointer);
            second.OnPointerDown(secondPointer);
            MovePointer(secondPointer, 4, 4, new Vector2(0.45f, 0f));
            second.OnPointerUp(secondPointer);
            Assert.That(board.RequestedCommandCount, Is.EqualTo(0));

            MovePointer(firstPointer, 2, 2, new Vector2(0.45f, 0f));
            first.OnDrag(firstPointer);
            first.OnPointerUp(firstPointer);
            yield return WaitUntilInteractive();

            Assert.That(board.RequestedCommandCount, Is.EqualTo(1));
            Assert.That(board.LastRequestedPrimary, Is.EqualTo(new BoardPosition(2, 2)));
        }

        [UnityTest]
        public IEnumerator AmbiguousDrag_WaitsForDominantAxisBeforeCommit()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Match3CellInput input = board.GetCellInputForTesting(3, 4);
            PointerEventData pointer = CreatePointer(42, 3, 4);
            input.OnPointerDown(pointer);

            MovePointer(pointer, 3, 4, new Vector2(0.22f, 0.20f));
            input.OnDrag(pointer);
            Assert.That(board.RequestedCommandCount, Is.EqualTo(0));

            MovePointer(pointer, 3, 4, new Vector2(0.45f, 0.06f));
            input.OnDrag(pointer);
            input.OnPointerUp(pointer);
            yield return WaitUntilInteractive();

            Assert.That(board.RequestedCommandCount, Is.EqualTo(1));
            Assert.That(board.LastRequestedSecondary, Is.EqualTo(new BoardPosition(4, 4)));
        }

        [UnityTest]
        public IEnumerator AmbiguousRelease_CancelsWithoutSwapOrTapSelection()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Match3CellInput input = board.GetCellInputForTesting(3, 4);
            PointerEventData pointer = CreatePointer(43, 3, 4);
            input.OnPointerDown(pointer);
            MovePointer(pointer, 3, 4, new Vector2(0.24f, 0.23f));
            input.OnDrag(pointer);
            input.OnPointerUp(pointer);
            yield return null;

            Assert.That(board.RequestedCommandCount, Is.EqualTo(0));
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
        }

        [UnityTest]
        public IEnumerator CancelledPointer_ReleasesOwnershipForNextGesture()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Match3CellInput input = board.GetCellInputForTesting(3, 4);
            PointerEventData cancelled = CreatePointer(51, 3, 4);
            input.OnPointerDown(cancelled);
            input.OnCancel(new BaseEventData(EventSystem.current));

            PointerEventData replacement = CreatePointer(52, 3, 4);
            input.OnPointerDown(replacement);
            MovePointer(replacement, 3, 4, new Vector2(0.45f, 0f));
            input.OnDrag(replacement);
            input.OnPointerUp(replacement);
            yield return WaitUntilInteractive();

            Assert.That(board.RequestedCommandCount, Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator ApplicationPause_ReleasesOwnershipForNextGesture()
        {
            Assert.That(board.LoadQAPreset("booster.firebird"), Is.True);
            Match3CellInput input = board.GetCellInputForTesting(3, 4);
            PointerEventData suspended = CreatePointer(53, 3, 4);
            input.OnPointerDown(suspended);
            input.SendMessage("OnApplicationPause", true);

            PointerEventData replacement = CreatePointer(54, 3, 4);
            input.OnPointerDown(replacement);
            MovePointer(replacement, 3, 4, new Vector2(0.45f, 0f));
            input.OnDrag(replacement);
            input.OnPointerUp(replacement);
            yield return WaitUntilInteractive();

            Assert.That(board.RequestedCommandCount, Is.EqualTo(1));
        }

        [Test]
        public void FeedbackSettings_GateSfxAndHapticsIndependently()
        {
            var sfx = new RecordingSfxSink();
            var haptic = new RecordingHapticSink();
            feedback.ConfigureSinks(sfx, haptic);
            GameplayMotionEvent rejected = Motion(GameplayMotionKind.SwapRejected);

            GameState.Instance.Save.sfx = false;
            GameState.Instance.Save.vibration = true;
            CoreLoopFeedbackService.Publish(rejected);
            Assert.That(sfx.Count, Is.EqualTo(0));
            Assert.That(haptic.Count, Is.EqualTo(1));

            GameState.Instance.Save.sfx = true;
            GameState.Instance.Save.vibration = false;
            GameState.Instance.Save.music = false;
            CoreLoopFeedbackService.Publish(rejected);
            Assert.That(sfx.Count, Is.EqualTo(1));
            Assert.That(haptic.Count, Is.EqualTo(1));
        }

        [Test]
        public void CascadeWave_UsesOneWaveLevelFeedbackCue()
        {
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.CascadeStarted, 2, 7),
                    out CoreLoopFeedbackSignal cascade,
                    out bool cascadeHaptic
                ),
                Is.True
            );
            Assert.That(cascade.Cue, Is.EqualTo(CoreLoopFeedbackCue.CascadeResolved));
            Assert.That(cascadeHaptic, Is.True);
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.MatchWaveStarted, 2, 7),
                    out _,
                    out _
                ),
                Is.False
            );
        }

        [Test]
        public void ThrowingSfxSink_IsolatedWhileHapticStillRuns()
        {
            var haptic = new RecordingHapticSink();
            feedback.ConfigureSinks(new ThrowingSfxSink(), haptic);
            LogAssert.Expect(
                LogType.Warning,
                new Regex("Core-loop SFX sink failed and was isolated")
            );

            Assert.DoesNotThrow(() =>
                CoreLoopFeedbackService.Publish(Motion(GameplayMotionKind.SwapRejected))
            );
            Assert.That(haptic.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThrowingHapticSink_IsolatedWhileSfxStillRuns()
        {
            var sfx = new RecordingSfxSink();
            feedback.ConfigureSinks(sfx, new ThrowingHapticSink());
            LogAssert.Expect(
                LogType.Warning,
                new Regex("Core-loop haptic sink failed and was isolated")
            );

            Assert.DoesNotThrow(() =>
                CoreLoopFeedbackService.Publish(Motion(GameplayMotionKind.SwapRejected))
            );
            Assert.That(sfx.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThrowingFeedbackObserver_CannotBlockConfiguredSinks()
        {
            var sfx = new RecordingSfxSink();
            var haptic = new RecordingHapticSink();
            feedback.ConfigureSinks(sfx, haptic);
            Action<CoreLoopFeedbackSignal> throwingObserver = signal =>
                throw new InvalidOperationException("feedback observer fault");
            feedback.SignalPublished += throwingObserver;
            LogAssert.Expect(
                LogType.Warning,
                new Regex("Core-loop feedback observer failed and was isolated")
            );

            try
            {
                Assert.DoesNotThrow(() =>
                    CoreLoopFeedbackService.Publish(Motion(GameplayMotionKind.SwapRejected))
                );
            }
            finally
            {
                feedback.SignalPublished -= throwingObserver;
            }
            Assert.That(sfx.Count, Is.EqualTo(1));
            Assert.That(haptic.Count, Is.EqualTo(1));
        }

        [Test]
        public void DefaultFeedbackSinks_AreSafeNoOps()
        {
            feedback.ResetSinks();
            int before = feedback.SignalCount;

            Assert.DoesNotThrow(() =>
                CoreLoopFeedbackService.Publish(Motion(GameplayMotionKind.SwapRejected))
            );
            Assert.That(feedback.SignalCount, Is.EqualTo(before + 1));
        }

        [UnityTest]
        public IEnumerator ThrowingMotionObserver_CannotStallRejectedSwap()
        {
            Assert.That(board.LoadQAPreset("booster.invalid-empty"), Is.True);
            board.MotionEventPublished += motion =>
                throw new InvalidOperationException("observer fault");
            LogAssert.Expect(
                LogType.Warning,
                new Regex("Gameplay motion observer failed and was isolated")
            );

            board.SwipeCell(0, 0, Vector2Int.right);
            yield return WaitUntilInteractive();

            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(board.HasActiveCommand, Is.False);
        }

        [UnityTest]
        public IEnumerator ThrowingLegacyBoardObserver_CannotStallResolvedSwap()
        {
            Assert.That(board.LoadQAPreset("special.row-column"), Is.True);
            BoardQAPresetFactory.TryGet("special.row-column", out BoardQAPreset preset);
            Action<int> throwingMoves = value =>
                throw new InvalidOperationException("moves observer fault");
            board.MovesChanged += throwingMoves;
            LogAssert.Expect(
                LogType.Warning,
                new Regex("MovesChanged observer failed and was isolated")
            );

            try
            {
                board.SwipeCell(
                    preset.Primary.X,
                    preset.Primary.Y,
                    Delta(preset.Primary, preset.Secondary)
                );
                yield return WaitUntilInteractive();
            }
            finally
            {
                board.MovesChanged -= throwingMoves;
            }

            Assert.That(board.HasActiveCommand, Is.False);
            Assert.That(board.LastWaveVerificationPassed, Is.True);
        }

        IEnumerator CaptureTrace(string presetId, Action<string> completed)
        {
            Assert.That(board.LoadQAPreset(presetId), Is.True);
            BoardQAPresetFactory.TryGet(presetId, out BoardQAPreset preset);
            var trace = new List<string>();
            Action<GameplayMotionEvent> handler = motion => trace.Add(
                $"{motion.Kind}:{motion.ComboIndex}:{motion.AffectedCount}:" +
                $"{motion.SpecialKind}:{motion.Primary}:{motion.Secondary}"
            );
            board.MotionEventPublished += handler;
            board.SwipeCell(
                preset.Primary.X,
                preset.Primary.Y,
                Delta(preset.Primary, preset.Secondary)
            );
            yield return WaitUntilInteractive();
            board.MotionEventPublished -= handler;
            completed(string.Join("|", trace));
        }

        void RestoreRuntimeSave()
        {
            if (
                stateUnderTest == null ||
                GameState.Instance != stateUnderTest ||
                string.IsNullOrEmpty(runtimeSaveJson)
            )
            {
                return;
            }

            PlayerSaveData restored = JsonUtility.FromJson<PlayerSaveData>(
                runtimeSaveJson
            );
            if (restored != null)
                stateUnderTest.Save = restored;
        }

        IEnumerator WaitUntilInteractive(float timeout = 8f)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (
                Time.realtimeSinceStartup < end &&
                (board.HasActiveCommand || !board.CanAcceptPointerInput)
            )
            {
                yield return null;
            }

            Assert.That(board.HasActiveCommand, Is.False, board.ActiveCommandLabel);
            Assert.That(board.CanAcceptPointerInput, Is.True, board.Phase.ToString());
        }

        void AssertVisualsSettled()
        {
            for (int y = 0; y < board.RowCount; y++)
            for (int x = 0; x < board.ColumnCount; x++)
            {
                RectTransform visual = board.GetCellVisualRectForTesting(x, y);
                Assert.That(visual.anchoredPosition, Is.EqualTo(Vector2.zero));
                Assert.That(visual.localScale, Is.EqualTo(Vector3.one));
                Assert.That(board.GetCellVisualAlphaForTesting(x, y), Is.EqualTo(1f));
            }
        }

        Vector2 SeamPoint(int firstX, int firstY, int secondX, int secondY)
        {
            RectTransform first = board.GetCellRectForTesting(firstX, firstY);
            RectTransform second = board.GetCellRectForTesting(secondX, secondY);
            Vector3 firstCenter = first.TransformPoint(first.rect.center);
            Vector3 secondCenter = second.TransformPoint(second.rect.center);
            return (firstCenter + secondCenter) * 0.5f;
        }

        void AssertRaycastHitsCell(Vector2 screenPoint)
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                position = screenPoint
            };
            var results = new List<RaycastResult>();
            canvasObject.GetComponent<GraphicRaycaster>().Raycast(pointer, results);
            bool hitCell = false;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.GetComponent<Match3CellInput>() != null)
                {
                    hitCell = true;
                    break;
                }
            }
            Assert.That(hitCell, Is.True, $"No cell hit at {screenPoint}.");
        }

        PointerEventData CreatePointer(int pointerId, int x, int y)
        {
            RectTransform rect = board.GetCellRectForTesting(x, y);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(
                null,
                rect.TransformPoint(rect.rect.center)
            );
            return new PointerEventData(EventSystem.current)
            {
                pointerId = pointerId,
                position = screen,
                pressPosition = screen,
                button = PointerEventData.InputButton.Left
            };
        }

        void MovePointer(
            PointerEventData pointer,
            int x,
            int y,
            Vector2 normalizedDelta
        )
        {
            RectTransform rect = board.GetCellRectForTesting(x, y);
            pointer.position = pointer.pressPosition + new Vector2(
                normalizedDelta.x * rect.rect.width,
                normalizedDelta.y * rect.rect.height
            );
        }

        static Vector2Int Delta(BoardPosition first, BoardPosition second)
        {
            return new Vector2Int(second.X - first.X, second.Y - first.Y);
        }

        static int IndexOf(
            IReadOnlyList<GameplayMotionEvent> events,
            GameplayMotionKind kind
        )
        {
            for (int i = 0; i < events.Count; i++)
                if (events[i].Kind == kind) return i;
            return -1;
        }

        static GameplayMotionEvent Motion(
            GameplayMotionKind kind,
            int comboIndex = 1,
            int affectedCount = 3
        )
        {
            return new GameplayMotionEvent(
                1,
                kind,
                new BoardPosition(0, 0),
                new BoardPosition(1, 0),
                comboIndex,
                affectedCount,
                SpecialGemKind.None,
                "test-plan"
            );
        }

        static BoardModel CreateSpecialCreationBoard()
        {
            var model = new BoardModel(
                BoardQAPresetFactory.Width,
                BoardQAPresetFactory.Height,
                BoardQAPresetFactory.TileTypes,
                new SeededBoardRandom(141401)
            );
            var values = new int[
                BoardQAPresetFactory.Width * BoardQAPresetFactory.Height
            ];
            for (int y = 0; y < BoardQAPresetFactory.Height; y++)
            for (int x = 0; x < BoardQAPresetFactory.Width; x++)
                values[y * BoardQAPresetFactory.Width + x] =
                    (x + 2 * y) % BoardQAPresetFactory.TileTypes;
            model.LoadRowMajor(values);

            model.SetCell(new BoardPosition(1, 4), new BoardCellData(1));
            model.SetCell(new BoardPosition(2, 4), new BoardCellData(1));
            model.SetCell(new BoardPosition(3, 4), new BoardCellData(2));
            model.SetCell(new BoardPosition(4, 4), new BoardCellData(1));
            model.SetCell(new BoardPosition(5, 4), new BoardCellData(3));
            model.SetCell(new BoardPosition(3, 3), new BoardCellData(1));
            return model;
        }

        static BoardResolutionPlan FindCascadePlan()
        {
            for (int seed = 1; seed <= 96; seed++)
            {
                var model = new BoardModel(
                    BoardQAPresetFactory.Width,
                    BoardQAPresetFactory.Height,
                    BoardQAPresetFactory.TileTypes,
                    new SeededBoardRandom(141400 + seed)
                );
                model.GeneratePlayableBoard();
                var planner = new BoardResolutionPlanner(model);

                for (int y = 0; y < model.Height; y++)
                for (int x = 0; x < model.Width; x++)
                {
                    var first = new BoardPosition(x, y);
                    if (x + 1 < model.Width)
                    {
                        BoardResolutionPlan horizontal = planner.PlanSwap(
                            first,
                            new BoardPosition(x + 1, y)
                        );
                        if (horizontal.IsResolved && horizontal.Waves.Count >= 2)
                            return horizontal;
                    }
                    if (y + 1 < model.Height)
                    {
                        BoardResolutionPlan vertical = planner.PlanSwap(
                            first,
                            new BoardPosition(x, y + 1)
                        );
                        if (vertical.IsResolved && vertical.Waves.Count >= 2)
                            return vertical;
                    }
                }
            }

            return null;
        }

        sealed class RecordingSfxSink : ICoreLoopSfxSink
        {
            public int Count { get; private set; }
            public void Play(CoreLoopFeedbackSignal signal) => Count++;
        }

        sealed class RecordingHapticSink : ICoreLoopHapticSink
        {
            public int Count { get; private set; }
            public void Pulse(CoreLoopFeedbackSignal signal) => Count++;
        }

        sealed class ThrowingSfxSink : ICoreLoopSfxSink
        {
            public void Play(CoreLoopFeedbackSignal signal)
            {
                throw new InvalidOperationException("sfx fault");
            }
        }

        sealed class ThrowingHapticSink : ICoreLoopHapticSink
        {
            public void Pulse(CoreLoopFeedbackSignal signal)
            {
                throw new InvalidOperationException("haptic fault");
            }
        }
    }
}
