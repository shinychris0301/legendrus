#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Editor
{
    public static class Step14CoreLoopMotionFeedbackSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.14 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("swap timing is bounded", () =>
                CoreLoopMotionProfile.SwapSeconds > 0f &&
                CoreLoopMotionProfile.SwapSeconds < 0.5f,
                ref passed, ref total, failures);
            Check("rejection timing is bounded", () =>
                CoreLoopMotionProfile.RejectedSwapSeconds > 0f &&
                CoreLoopMotionProfile.RejectedSwapSeconds < 0.5f,
                ref passed, ref total, failures);
            Check("fall timing is bounded", () =>
                CoreLoopMotionProfile.FallSeconds > 0f &&
                CoreLoopMotionProfile.FallSeconds < 0.5f,
                ref passed, ref total, failures);
            Check("ease endpoints are exact", () =>
                Mathf.Approximately(CoreLoopMotionProfile.EaseOutCubic(0f), 0f) &&
                Mathf.Approximately(CoreLoopMotionProfile.EaseOutCubic(1f), 1f),
                ref passed, ref total, failures);
            Check("swipe dominance exceeds one", () =>
                CoreLoopMotionProfile.SwipeAxisDominance > 1f,
                ref passed, ref total, failures);

            GameplayMotionEvent rejected = Motion(
                GameplayMotionKind.SwapRejected,
                0,
                0
            );
            Check("rejected swap maps to feedback", () =>
                CoreLoopFeedbackService.TryCreateSignal(
                    rejected,
                    out CoreLoopFeedbackSignal signal,
                    out bool haptic
                ) &&
                signal.Cue == CoreLoopFeedbackCue.SwapRejected &&
                haptic,
                ref passed, ref total, failures);

            GameplayMotionEvent cascade = Motion(
                GameplayMotionKind.CascadeStarted,
                2,
                7
            );
            Check("cascade preserves depth and magnitude", () =>
                CoreLoopFeedbackService.TryCreateSignal(
                    cascade,
                    out CoreLoopFeedbackSignal signal,
                    out bool haptic
                ) &&
                signal.Cue == CoreLoopFeedbackCue.CascadeResolved &&
                signal.CascadeDepth == 2 &&
                signal.Magnitude == 7 &&
                haptic,
                ref passed, ref total, failures);

            Check("fall start is hook-only", () =>
                !CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.FallStarted, 1, 5),
                    out _,
                    out _
                ),
                ref passed, ref total, failures);
            Check("fall completion maps to settled feedback", () =>
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.FallCompleted, 1, 5),
                    out CoreLoopFeedbackSignal signal,
                    out _
                ) && signal.Cue == CoreLoopFeedbackCue.FallSettled,
                ref passed, ref total, failures);
            Check("special creation maps to feedback", () =>
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SpecialCreationStarted, 1, 1),
                    out CoreLoopFeedbackSignal signal,
                    out bool haptic
                ) && signal.Cue == CoreLoopFeedbackCue.SpecialCreated && haptic,
                ref passed, ref total, failures);
            Check("activation batch maps once", () =>
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SpecialActivationStarted, 1, 4),
                    out CoreLoopFeedbackSignal signal,
                    out _
                ) && signal.Magnitude == 4,
                ref passed, ref total, failures);
            Check("secondary activation hook is silent", () =>
                !CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SpecialActivationStarted, 1, 0),
                    out _,
                    out _
                ),
                ref passed, ref total, failures);
            Check("swap committed is hook-only", () =>
                !CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SwapCommitted, 0, 2),
                    out _,
                    out _
                ),
                ref passed, ref total, failures);

            var phase = new GameplayStateMachine();
            Check("rejection feedback keeps legal phase route", () =>
                phase.TryTransition(GameplayPhase.Swapping) &&
                phase.TryTransition(GameplayPhase.Idle) &&
                phase.CanAcceptPointerInput,
                ref passed, ref total, failures);

            Check("swipe reports board acceptance", () =>
            {
                MethodInfo swipe = typeof(Match3Board).GetMethod(
                    "SwipeCell",
                    new[] { typeof(int), typeof(int), typeof(Vector2Int) }
                );
                return swipe != null && swipe.ReturnType == typeof(bool);
            },
                ref passed, ref total, failures);
            Check("motion hook is public", () =>
                typeof(Match3Board).GetEvent("MotionEventPublished") != null,
                ref passed, ref total, failures);
            Check("pointer cancellation is implemented", () =>
                typeof(ICancelHandler).IsAssignableFrom(typeof(Match3CellInput)),
                ref passed, ref total, failures);
            Check("SFX boundary is replaceable", () =>
                typeof(ICoreLoopSfxSink).IsInterface,
                ref passed, ref total, failures);
            Check("haptic boundary is replaceable", () =>
                typeof(ICoreLoopHapticSink).IsInterface,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.14 Step 1 core-loop motion and feedback self-tests " +
                    $"passed: {passed}/{total}."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.14 Step 1 self-tests passed: {passed}/{total}",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.14 Step 1 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static GameplayMotionEvent Motion(
            GameplayMotionKind kind,
            int combo,
            int affected
        )
        {
            return new GameplayMotionEvent(
                1,
                kind,
                new BoardPosition(0, 0),
                new BoardPosition(1, 0),
                combo,
                affected,
                SpecialGemKind.Row,
                "self-test"
            );
        }

        static void Check(
            string name,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test()) passed++;
                else failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
