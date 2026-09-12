#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Editor
{
    public static class Step14MetaUiMotionTransitionPolishSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.14 Step 3 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check(
                "motion registry has a fixed practical bound",
                () => MetaUiMotionProfile.MaximumRegisteredMotions >= 16 &&
                    MetaUiMotionProfile.MaximumRegisteredMotions <= 256,
                ref passed,
                ref total,
                failures
            );
            Check(
                "unscaled clock delta is clamped",
                () => MetaUiMotionProfile.MaximumTickSeconds > 0f &&
                    MetaUiMotionProfile.MaximumTickSeconds <= 0.05f &&
                    Approximately(
                        MetaUiMotionProfile.ClampTick(10f),
                        MetaUiMotionProfile.MaximumTickSeconds
                    ) &&
                    Approximately(MetaUiMotionProfile.ClampTick(-1f), 0f),
                ref passed,
                ref total,
                failures
            );
            Check(
                "transition duration is restrained",
                () => MetaUiMotionProfile.MaximumTransitionSeconds >= 0.20f &&
                    MetaUiMotionProfile.MaximumTransitionSeconds <= 0.50f,
                ref passed,
                ref total,
                failures
            );
            Check(
                "transition offsets stay inside the polish budget",
                () => MetaUiMotionProfile.MaximumOffsetPixels > 0f &&
                    MetaUiMotionProfile.MaximumOffsetPixels <= 32f,
                ref passed,
                ref total,
                failures
            );
            Check(
                "screen presets preserve outcome identity",
                () =>
                    MetaUiMotionProfile.ResolveScreenPreset(ScreenId.Home) ==
                        MetaUiMotionPreset.ScreenStandard &&
                    MetaUiMotionProfile.ResolveScreenPreset(ScreenId.Victory) ==
                        MetaUiMotionPreset.ScreenVictory &&
                    MetaUiMotionProfile.ResolveScreenPreset(ScreenId.Fail) ==
                        MetaUiMotionPreset.ScreenFailure &&
                    MetaUiMotionProfile.ResolveScreenPreset(ScreenId.ChapterComplete) ==
                        MetaUiMotionPreset.ScreenChapterComplete,
                ref passed,
                ref total,
                failures
            );
            Check(
                "reward modal has a distinct preset",
                () =>
                    MetaUiMotionProfile.ResolveModalPreset(ScreenId.RewardClaim) ==
                        MetaUiMotionPreset.ModalReward &&
                    MetaUiMotionProfile.ResolveModalPreset(ScreenId.Pause) ==
                        MetaUiMotionPreset.ModalStandard,
                ref passed,
                ref total,
                failures
            );
            Check(
                "all transition styles are bounded",
                AllTransitionStylesAreBounded,
                ref passed,
                ref total,
                failures
            );
            Check(
                "all press styles are bounded",
                AllPressStylesAreBounded,
                ref passed,
                ref total,
                failures
            );
            Check(
                "full-screen and modal artwork use alpha-only motion",
                FullScreenStylesAreAlphaOnly,
                ref passed,
                ref total,
                failures
            );
            Check(
                "gameplay hotspots retain the established invisible response",
                () => !MetaUiMotionProfile.ResolvePress(
                    MetaUiControlRole.Gameplay
                ).ShowsHighlight,
                ref passed,
                ref total,
                failures
            );
            Check(
                "easing endpoints are stable",
                () =>
                    Approximately(MetaUiMotionProfile.EaseOutCubic(0f), 0f) &&
                    Approximately(MetaUiMotionProfile.EaseOutCubic(1f), 1f) &&
                    Approximately(MetaUiMotionProfile.EaseInCubic(0f), 0f) &&
                    Approximately(MetaUiMotionProfile.EaseInCubic(1f), 1f) &&
                    Approximately(MetaUiMotionProfile.EaseOutBack(0f), 0f) &&
                    Approximately(MetaUiMotionProfile.EaseOutBack(1f), 1f),
                ref passed,
                ref total,
                failures
            );
            Check(
                "all required semantic cues are present",
                RequiredCuesArePresent,
                ref passed,
                ref total,
                failures
            );
            Check(
                "motion payloads are value types",
                () =>
                    typeof(MetaUiTransitionStyle).IsValueType &&
                    typeof(MetaUiPressStyle).IsValueType &&
                    typeof(MetaUiMotionTrace).IsValueType &&
                    typeof(MetaUiMotionDiagnostics).IsValueType,
                ref passed,
                ref total,
                failures
            );
            Check(
                "coordinator exposes deterministic clock controls",
                () =>
                    HasPublicMethod(
                        typeof(MetaUiMotionPresenter),
                        "TickForTesting",
                        typeof(float)
                    ) &&
                    HasPublicMethod(typeof(MetaUiMotionPresenter), "ClearAll"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "coordinator exposes bounded diagnostics",
                () =>
                    HasPublicProperty(typeof(MetaUiMotionPresenter), "RegisteredCount") &&
                    HasPublicProperty(typeof(MetaUiMotionPresenter), "PeakRegisteredCount") &&
                    HasPublicProperty(typeof(MetaUiMotionPresenter), "DroppedRegistrationCount") &&
                    HasPublicProperty(typeof(MetaUiMotionPresenter), "TickCount") &&
                    HasPublicProperty(typeof(MetaUiMotionPresenter), "Diagnostics"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "transition exposes enter exit cancel and test seams",
                () =>
                    HasPublicMethod(typeof(MetaUiTransition), "PlayEnter") &&
                    HasPublicMethod(typeof(MetaUiTransition), "PlayExit") &&
                    HasPublicMethod(typeof(MetaUiTransition), "CancelMotion") &&
                    HasPublicMethod(
                        typeof(MetaUiTransition),
                        "TickForTesting",
                        typeof(float)
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "transition exposes phase group and semantic trace",
                () =>
                    HasPublicProperty(typeof(MetaUiTransition), "Phase") &&
                    HasPublicProperty(typeof(MetaUiTransition), "Preset") &&
                    HasPublicProperty(typeof(MetaUiTransition), "Group") &&
                    HasPublicProperty(typeof(MetaUiTransition), "LastTrace") &&
                    typeof(MetaUiTransition).GetEvent("MotionTracePublished") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "hotspot implements complete pointer lifecycle",
                () =>
                    typeof(IPointerDownHandler).IsAssignableFrom(typeof(AnimatedHotspot)) &&
                    typeof(IPointerUpHandler).IsAssignableFrom(typeof(AnimatedHotspot)) &&
                    typeof(IPointerExitHandler).IsAssignableFrom(typeof(AnimatedHotspot)) &&
                    typeof(IPointerClickHandler).IsAssignableFrom(typeof(AnimatedHotspot)) &&
                    typeof(IMetaUiMotionTickable).IsAssignableFrom(typeof(AnimatedHotspot)),
                ref passed,
                ref total,
                failures
            );
            Check(
                "hotspot exposes deterministic activation and cleanup",
                () =>
                    HasPublicProperty(typeof(AnimatedHotspot), "ActivationCount") &&
                    HasPublicProperty(typeof(AnimatedHotspot), "IsReady") &&
                    HasPublicProperty(typeof(AnimatedHotspot), "LastTrace") &&
                    HasPublicMethod(
                        typeof(AnimatedHotspot),
                        "TickForTesting",
                        typeof(float)
                    ) &&
                    HasPublicMethod(typeof(AnimatedHotspot), "CancelMotion"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "hotspot exposes non-raycast highlight diagnostics",
                () => HasPublicProperty(
                    typeof(AnimatedHotspot),
                    "IsHighlightRaycastSafe"
                ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "root action gate exposes exactly-once reservation state",
                () =>
                    HasPublicMethod(
                        typeof(MetaUiActionGate),
                        "TryAcquire",
                        typeof(AnimatedHotspot)
                    ) &&
                    HasPublicMethod(
                        typeof(MetaUiActionGate),
                        "Release",
                        typeof(AnimatedHotspot)
                    ) &&
                    HasPublicProperty(typeof(MetaUiActionGate), "IsBusy") &&
                    HasPublicProperty(typeof(MetaUiActionGate), "RejectedReservations"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "map feedback exposes a bounded non-raycast test seam",
                () =>
                    HasPublicMethod(
                        typeof(MetaUiMapNodeFeedback),
                        "TickForTesting",
                        typeof(float)
                    ) &&
                    HasPublicMethod(typeof(MetaUiMapNodeFeedback), "CancelMotion") &&
                    HasPublicProperty(typeof(MetaUiMapNodeFeedback), "PulseCount") &&
                    HasPublicProperty(typeof(MetaUiMapNodeFeedback), "IsRaycastSafe") &&
                    HasPublicProperty(typeof(MetaUiMapNodeFeedback), "LastTrace"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "screen router observes transition completion without exposing mutation",
                () =>
                    typeof(IMetaUiTransitionObserver).IsAssignableFrom(
                        typeof(ScreenRouter)
                    ) &&
                    HasPublicProperty(typeof(ScreenRouter), "MotionPresenter"),
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.14 Step 3 meta UI motion and transition polish " +
                    $"self-tests passed: {passed}/{total}."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.14 Step 3 self-tests passed: {passed}/{total}",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.14 Step 3 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
            if (Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "v1.14 Step 3 self-tests failed."
                );
            }
        }

        static bool AllTransitionStylesAreBounded()
        {
            foreach (MetaUiMotionPreset preset in Enum.GetValues(typeof(MetaUiMotionPreset)))
            {
                MetaUiTransitionStyle style = MetaUiMotionProfile.ResolveTransition(preset);
                if (
                    style.DelaySeconds < 0f ||
                    style.DelaySeconds > MetaUiMotionProfile.MaximumTransitionSeconds ||
                    style.EnterDurationSeconds < 0.01f ||
                    style.EnterDurationSeconds > MetaUiMotionProfile.MaximumTransitionSeconds ||
                    style.ExitDurationSeconds < 0.01f ||
                    style.ExitDurationSeconds > MetaUiMotionProfile.MaximumTransitionSeconds ||
                    style.StartAlpha < 0f ||
                    style.StartAlpha > 1f ||
                    style.StartScale < 0.85f ||
                    style.StartScale > 1.15f ||
                    style.ExitScale < 0.85f ||
                    style.ExitScale > 1.15f ||
                    style.StartOffset.magnitude > MetaUiMotionProfile.MaximumOffsetPixels ||
                    style.ExitOffset.magnitude > MetaUiMotionProfile.MaximumOffsetPixels
                ) return false;
            }
            return true;
        }

        static bool AllPressStylesAreBounded()
        {
            foreach (MetaUiControlRole role in Enum.GetValues(typeof(MetaUiControlRole)))
            {
                MetaUiPressStyle style = MetaUiMotionProfile.ResolvePress(role);
                if (
                    style.InvokeDelaySeconds < 0f ||
                    style.InvokeDelaySeconds > 0.20f ||
                    style.ReleaseDurationSeconds < 0.01f ||
                    style.ReleaseDurationSeconds > 0.20f ||
                    style.PressedScale < 0.90f ||
                    style.PressedScale > 1.10f ||
                    style.ActivatedScale < 0.90f ||
                    style.ActivatedScale > 1.10f ||
                    style.PressedColor.a < 0f ||
                    style.PressedColor.a > 1f ||
                    style.ActivatedColor.a < 0f ||
                    style.ActivatedColor.a > 1f
                ) return false;
            }
            return true;
        }

        static bool FullScreenStylesAreAlphaOnly()
        {
            MetaUiMotionPreset[] presets =
            {
                MetaUiMotionPreset.ScreenStandard,
                MetaUiMotionPreset.ScreenVictory,
                MetaUiMotionPreset.ScreenFailure,
                MetaUiMotionPreset.ScreenChapterComplete,
                MetaUiMotionPreset.ModalStandard,
                MetaUiMotionPreset.ModalReward
            };

            for (int i = 0; i < presets.Length; i++)
            {
                MetaUiTransitionStyle style =
                    MetaUiMotionProfile.ResolveTransition(presets[i]);
                if (
                    !Approximately(style.StartScale, 1f) ||
                    !Approximately(style.ExitScale, 1f) ||
                    style.StartOffset != Vector2.zero ||
                    style.ExitOffset != Vector2.zero
                ) return false;
            }
            return true;
        }

        static bool RequiredCuesArePresent()
        {
            MetaUiMotionCue[] cues =
            {
                MetaUiMotionCue.ScreenEnter,
                MetaUiMotionCue.ModalEnter,
                MetaUiMotionCue.ModalExit,
                MetaUiMotionCue.ButtonPress,
                MetaUiMotionCue.ButtonRelease,
                MetaUiMotionCue.ButtonActivated,
                MetaUiMotionCue.MapNodePulse,
                MetaUiMotionCue.VictoryReveal,
                MetaUiMotionCue.FailureReveal,
                MetaUiMotionCue.RewardReveal,
                MetaUiMotionCue.ChapterCompleteReveal
            };
            for (int i = 0; i < cues.Length; i++)
                if (!Enum.IsDefined(typeof(MetaUiMotionCue), cues[i])) return false;
            return true;
        }

        static bool HasPublicProperty(Type type, string name)
        {
            return type.GetProperty(
                name,
                BindingFlags.Instance | BindingFlags.Public
            ) != null;
        }

        static bool HasPublicMethod(Type type, string name, params Type[] arguments)
        {
            return type.GetMethod(
                name,
                BindingFlags.Instance | BindingFlags.Public,
                null,
                arguments,
                null
            ) != null;
        }

        static bool Approximately(float first, float second)
        {
            return Mathf.Abs(first - second) <= 0.0001f;
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
