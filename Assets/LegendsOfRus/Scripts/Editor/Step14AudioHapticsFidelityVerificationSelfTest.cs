#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step14AudioHapticsFidelityVerificationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.14 Step 4 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check(
                "meta UI feedback accepts only transition start phases",
                MetaStartPhaseFilteringIsDeterministic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "meta UI gameplay controls stay on the core-loop boundary",
                () => !MetaUiFeedbackService.TryCreateSignal(
                    new MetaUiMotionTrace(
                        1,
                        MetaUiMotionCue.ButtonActivated,
                        MetaUiTransitionPhase.Entering,
                        MetaUiMotionPreset.ScreenStandard,
                        "gameplay"
                    ),
                    MetaUiControlRole.Gameplay,
                    out _,
                    out _
                ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "meta UI feedback exposes replaceable sinks and diagnostics",
                () =>
                    HasPublicMethod(
                        typeof(MetaUiFeedbackService),
                        "ConfigureSinks",
                        typeof(IMetaUiSfxSink),
                        typeof(IMetaUiHapticSink)
                    ) &&
                    HasPublicMethod(typeof(MetaUiFeedbackService), "ResetSinks") &&
                    HasPublicProperty(typeof(MetaUiFeedbackService), "SignalCount") &&
                    HasPublicProperty(typeof(MetaUiFeedbackService), "LastSignal") &&
                    typeof(MetaUiFeedbackService).GetEvent("SignalPublished") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "core-loop feedback preserves motion metadata",
                CoreMetadataIsPreserved,
                ref passed,
                ref total,
                failures
            );
            Check(
                "core-loop feedback filters duplicate presentation beats",
                CoreDuplicateFilteringIsDeterministic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic throttle is exactly sixty milliseconds",
                () => Math.Abs(
                    PlatformHapticProvider.MinimumIntervalSeconds - 0.06d
                ) <= 0.0000001d,
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic provider implements both semantic sink contracts",
                () =>
                    typeof(ICoreLoopHapticSink).IsAssignableFrom(
                        typeof(PlatformHapticProvider)
                    ) &&
                    typeof(IMetaUiHapticSink).IsAssignableFrom(
                        typeof(PlatformHapticProvider)
                    ) &&
                    typeof(IDisposable).IsAssignableFrom(
                        typeof(PlatformHapticProvider)
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic provider exposes injection and lifecycle seams",
                () =>
                    typeof(PlatformHapticProvider).GetConstructor(new[]
                    {
                        typeof(IHapticClock),
                        typeof(IPlatformHapticBridge),
                    }) != null &&
                    HasPublicMethod(
                        typeof(PlatformHapticProvider),
                        "TryPulse",
                        typeof(HapticFeedbackIntent)
                    ) &&
                    HasPublicMethod(typeof(PlatformHapticProvider), "Suspend") &&
                    HasPublicMethod(typeof(PlatformHapticProvider), "Resume") &&
                    HasPublicMethod(typeof(PlatformHapticProvider), "Dispose"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic diagnostics expose support throttle and disposal state",
                () =>
                    HasPublicProperty(typeof(PlatformHapticProvider), "IsSupported") &&
                    HasPublicProperty(typeof(PlatformHapticProvider), "IsSuspended") &&
                    HasPublicProperty(typeof(PlatformHapticProvider), "IsDisposed") &&
                    HasPublicProperty(
                        typeof(PlatformHapticProvider),
                        "AcceptedPulseCount"
                    ) &&
                    HasPublicProperty(
                        typeof(PlatformHapticProvider),
                        "ThrottledPulseCount"
                    ) &&
                    HasPublicProperty(
                        typeof(PlatformHapticProvider),
                        "LastAcceptedIntent"
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "core-loop haptic mapping is complete",
                CoreHapticMappingIsComplete,
                ref passed,
                ref total,
                failures
            );
            Check(
                "meta UI haptic mapping preserves role semantics",
                MetaHapticMappingIsComplete,
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic priority guard and exact boundary are deterministic",
                HapticThrottleIsDeterministic,
                ref passed,
                ref total,
                failures
            );
            Check(
                "haptic suspend and dispose are idempotent",
                HapticLifecycleIsIdempotent,
                ref passed,
                ref total,
                failures
            );
            Check(
                "production provider implements both audio sink contracts",
                () =>
                    typeof(ICoreLoopSfxSink).IsAssignableFrom(
                        typeof(ProductionFeedbackProvider)
                    ) &&
                    typeof(IMetaUiSfxSink).IsAssignableFrom(
                        typeof(ProductionFeedbackProvider)
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "production provider has a fixed four-voice pool",
                () => ProductionFeedbackProvider.SfxVoiceCapacity == 4,
                ref passed,
                ref total,
                failures
            );
            Check(
                "production provider exposes bounded output diagnostics",
                () =>
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "IsInitialized"
                    ) &&
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "PoolCapacity"
                    ) &&
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "PlayedSfxCount"
                    ) &&
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "MissingClipCount"
                    ) &&
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "MissingClipRequestCount"
                    ) &&
                    HasPublicProperty(
                        typeof(ProductionFeedbackProvider),
                        "VoiceStealCount"
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "production hierarchy and resources use once-only guards",
                ProductionProviderHasOnceOnlyGuards,
                ref passed,
                ref total,
                failures
            );
            Check(
                "fidelity matrix contains exactly four targets",
                () => FidelityVerification.TargetCount == 4,
                ref passed,
                ref total,
                failures
            );
            Check(
                "fidelity targets have exact identities and dimensions",
                ExactFidelityTargetsArePresent,
                ref passed,
                ref total,
                failures
            );
            Check(
                "locked design reference remains 941 by 1672",
                () =>
                    Approximately(FidelityVerification.DesignWidth, 941f) &&
                    Approximately(FidelityVerification.DesignHeight, 1672f) &&
                    Approximately(
                        FidelityVerification.DesignAspect,
                        941f / 1672f
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "all target viewports match the deterministic fit plan",
                ExactTargetViewportsAreStable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "invalid safe areas fall back to the full screen",
                InvalidSafeAreaFallsBackToScreen,
                ref passed,
                ref total,
                failures
            );
            Check(
                "safe areas are clamped to physical screen bounds",
                SafeAreaClampingIsStable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "fidelity evaluator distinguishes exact layout from drift",
                FidelityEvaluatorDetectsDrift,
                ref passed,
                ref total,
                failures
            );
            Check(
                "safe-area fitter exposes an injected apply seam",
                () =>
                    HasPublicMethod(
                        typeof(SafeAreaFitter),
                        "Apply",
                        typeof(Rect),
                        typeof(Vector2Int)
                    ) &&
                    HasPublicProperty(typeof(SafeAreaFitter), "LastAppliedSafeArea") &&
                    HasPublicProperty(
                        typeof(SafeAreaFitter),
                        "LastAppliedScreenSize"
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "adaptive scaler exposes injected and pure resolution seams",
                () =>
                    HasPublicMethod(
                        typeof(AdaptiveCanvasScaler),
                        "Apply",
                        typeof(Vector2Int)
                    ) &&
                    HasPublicMethod(
                        typeof(AdaptiveCanvasScaler),
                        "Apply",
                        typeof(Rect),
                        typeof(Vector2Int)
                    ) &&
                    HasPublicStaticMethod(
                        typeof(AdaptiveCanvasScaler),
                        "ResolveMatch",
                        typeof(Vector2Int),
                        typeof(float),
                        typeof(float)
                    ) &&
                    HasPublicStaticMethod(
                        typeof(AdaptiveCanvasScaler),
                        "ResolveScale",
                        typeof(Vector2Int),
                        typeof(float),
                        typeof(float)
                    ) &&
                    HasPublicProperty(
                        typeof(AdaptiveCanvasScaler),
                        "LastAppliedMatch"
                    ) &&
                    HasPublicProperty(
                        typeof(AdaptiveCanvasScaler),
                        "LastAppliedScale"
                    ) &&
                    HasPublicProperty(
                        typeof(AdaptiveCanvasScaler),
                        "LastAppliedSafeArea"
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "all phone targets remain width-limited",
                AllPhoneTargetsUseWidthMatch,
                ref passed,
                ref total,
                failures
            );
            Check(
                "QA capture exposes one-request latch state",
                () =>
                    HasPublicMethod(
                        typeof(QACaptureService),
                        "TryRequestCapture",
                        typeof(FidelityCaptureRequest)
                    ) &&
                    HasPublicMethod(
                        typeof(QACaptureService),
                        "CancelPendingCapture"
                    ) &&
                    HasPublicProperty(typeof(QACaptureService), "IsCapturePending") &&
                    typeof(QACaptureService).GetEvent("CaptureCompleted") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "capture metadata carries layout and route evidence",
                CaptureMetadataContractIsComplete,
                ref passed,
                ref total,
                failures
            );
            Check(
                "runtime performance policy locks the sixty fps gate",
                RuntimePerformancePolicyIsStable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "runtime accumulator exposes fixed-capacity pure seams",
                () =>
                    HasPublicMethod(
                        typeof(RuntimePerformanceAccumulator),
                        "Begin",
                        typeof(RuntimePerformanceScenario),
                        typeof(int),
                        typeof(int),
                        typeof(int),
                        typeof(bool),
                        typeof(bool)
                    ) &&
                    HasPublicMethod(
                        typeof(RuntimePerformanceAccumulator),
                        "TryPushSample",
                        typeof(ulong),
                        typeof(double),
                        typeof(double),
                        typeof(double),
                        typeof(double),
                        typeof(double),
                        typeof(long)
                    ) &&
                    HasPublicMethod(
                        typeof(RuntimePerformanceAccumulator),
                        "TryBuildReport",
                        typeof(RuntimePerformanceReport).MakeByRefType()
                    ) &&
                    HasPublicProperty(
                        typeof(RuntimePerformanceAccumulator),
                        "Capacity"
                    ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "runtime accumulator uses nearest-rank percentiles",
                AccumulatorPercentilesAreStable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "runtime accumulator passes supported in-budget samples",
                AccumulatorPassingGateIsStable,
                ref passed,
                ref total,
                failures
            );
            Check(
                "unsupported runtime metrics remain incomplete",
                UnsupportedPerformanceMetricsStayIncomplete,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.14 Step 4 audio, haptics, and fidelity verification " +
                    $"self-tests passed: {passed}/{total}."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.14 Step 4 self-tests passed: {passed}/{total}",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.14 Step 4 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
            if (Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "v1.14 Step 4 self-tests failed."
                );
            }
        }

        static bool MetaStartPhaseFilteringIsDeterministic()
        {
            bool entering = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    1,
                    MetaUiMotionCue.ModalEnter,
                    MetaUiTransitionPhase.Entering,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Standard,
                out MetaUiFeedbackSignal signal,
                out bool haptic
            );
            bool entered = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    1,
                    MetaUiMotionCue.ModalEnter,
                    MetaUiTransitionPhase.Entered,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Standard,
                out _,
                out _
            );
            bool enterOnExitPhase = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    1,
                    MetaUiMotionCue.ModalEnter,
                    MetaUiTransitionPhase.Exiting,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Standard,
                out _,
                out _
            );
            bool exiting = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    2,
                    MetaUiMotionCue.ModalExit,
                    MetaUiTransitionPhase.Exiting,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Back,
                out _,
                out _
            );
            bool exited = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    2,
                    MetaUiMotionCue.ModalExit,
                    MetaUiTransitionPhase.Exited,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Back,
                out _,
                out _
            );
            bool exitOnEnterPhase = MetaUiFeedbackService.TryCreateSignal(
                new MetaUiMotionTrace(
                    2,
                    MetaUiMotionCue.ModalExit,
                    MetaUiTransitionPhase.Entering,
                    MetaUiMotionPreset.ModalStandard,
                    "modal"
                ),
                MetaUiControlRole.Back,
                out _,
                out _
            );
            return entering && !entered && exiting && !exited &&
                !enterOnExitPhase && !exitOnEnterPhase &&
                signal.Cue == MetaUiMotionCue.ModalEnter && haptic;
        }

        static bool CoreMetadataIsPreserved()
        {
            var motion = new GameplayMotionEvent(
                23,
                GameplayMotionKind.BoosterActivated,
                new BoardPosition(1, 2),
                new BoardPosition(3, 4),
                5,
                17,
                SpecialGemKind.Color,
                "STEP04",
                SpecialCombinationKind.ColorWithColor,
                GameplayBoosterKind.LivingLight
            );
            if (!CoreLoopFeedbackService.TryCreateSignal(
                motion,
                out CoreLoopFeedbackSignal signal,
                out bool haptic
            )) return false;

            return signal.Cue == CoreLoopFeedbackCue.BoosterActivated &&
                signal.Magnitude == 17 &&
                signal.CascadeDepth == 5 &&
                signal.PlanHash == "STEP04" &&
                signal.BoosterKind == GameplayBoosterKind.LivingLight &&
                signal.Sequence == 23 &&
                signal.SpecialKind == SpecialGemKind.Color &&
                signal.CombinationKind == SpecialCombinationKind.ColorWithColor &&
                haptic;
        }

        static bool CoreDuplicateFilteringIsDeterministic()
        {
            return
                !TryCore(GameplayMotionKind.MatchWaveStarted, 2, 5) &&
                !TryCore(GameplayMotionKind.SpecialActivationStarted, 1, 0) &&
                !TryCore(GameplayMotionKind.SwapCommitted, 1, 0) &&
                TryCore(GameplayMotionKind.MatchWaveStarted, 1, 3);
        }

        static bool TryCore(GameplayMotionKind kind, int combo, int affected)
        {
            return CoreLoopFeedbackService.TryCreateSignal(
                new GameplayMotionEvent(
                    1,
                    kind,
                    new BoardPosition(0, 0),
                    new BoardPosition(1, 0),
                    combo,
                    affected,
                    SpecialGemKind.None,
                    "self-test"
                ),
                out _,
                out _
            );
        }

        static bool CoreHapticMappingIsComplete()
        {
            foreach (CoreLoopFeedbackCue cue in Enum.GetValues(
                typeof(CoreLoopFeedbackCue)
            ))
            {
                HapticFeedbackIntent actual = PlatformHapticProvider.ResolveIntent(
                    new CoreLoopFeedbackSignal(cue, 3, 1, "plan")
                );
                HapticFeedbackIntent expected;
                switch (cue)
                {
                    case CoreLoopFeedbackCue.SwapRejected:
                    case CoreLoopFeedbackCue.InvalidSwap:
                    case CoreLoopFeedbackCue.CommandRecovered:
                    case CoreLoopFeedbackCue.BoosterRejected:
                        expected = HapticFeedbackIntent.Reject;
                        break;
                    case CoreLoopFeedbackCue.MatchResolved:
                    case CoreLoopFeedbackCue.CascadeResolved:
                        expected = HapticFeedbackIntent.Confirm;
                        break;
                    case CoreLoopFeedbackCue.SpecialCreated:
                    case CoreLoopFeedbackCue.SpecialActivated:
                    case CoreLoopFeedbackCue.Reshuffled:
                    case CoreLoopFeedbackCue.BoosterActivated:
                        expected = HapticFeedbackIntent.Emphasis;
                        break;
                    default:
                        expected = HapticFeedbackIntent.None;
                        break;
                }
                if (actual != expected) return false;
            }
            return PlatformHapticProvider.ResolveIntent(
                (CoreLoopFeedbackSignal)null
            ) == HapticFeedbackIntent.None;
        }

        static bool MetaHapticMappingIsComplete()
        {
            return
                ResolveMetaIntent(
                    MetaUiMotionCue.ButtonActivated,
                    MetaUiControlRole.Standard
                ) == HapticFeedbackIntent.Selection &&
                ResolveMetaIntent(
                    MetaUiMotionCue.ButtonActivated,
                    MetaUiControlRole.Primary
                ) == HapticFeedbackIntent.Confirm &&
                ResolveMetaIntent(
                    MetaUiMotionCue.ButtonActivated,
                    MetaUiControlRole.MapNode
                ) == HapticFeedbackIntent.Confirm &&
                ResolveMetaIntent(
                    MetaUiMotionCue.ButtonActivated,
                    MetaUiControlRole.Reward
                ) == HapticFeedbackIntent.Emphasis &&
                ResolveMetaIntent(
                    MetaUiMotionCue.ButtonActivated,
                    MetaUiControlRole.Gameplay
                ) == HapticFeedbackIntent.None &&
                ResolveMetaIntent(
                    MetaUiMotionCue.FailureReveal,
                    MetaUiControlRole.Standard
                ) == HapticFeedbackIntent.Reject &&
                ResolveMetaIntent(
                    MetaUiMotionCue.ModalEnter,
                    MetaUiControlRole.Standard
                ) == HapticFeedbackIntent.Selection;
        }

        static HapticFeedbackIntent ResolveMetaIntent(
            MetaUiMotionCue cue,
            MetaUiControlRole role
        )
        {
            return PlatformHapticProvider.ResolveIntent(
                new MetaUiFeedbackSignal(
                    cue,
                    role,
                    MetaUiMotionPreset.ScreenStandard,
                    "target"
                )
            );
        }

        static bool HapticThrottleIsDeterministic()
        {
            var clock = new TestHapticClock { Now = 1d };
            var bridge = new TestHapticBridge();
            using (var provider = new PlatformHapticProvider(clock, bridge))
            {
                if (!provider.TryPulse(HapticFeedbackIntent.Selection)) return false;
                clock.Now = 1.01d;
                if (!provider.TryPulse(HapticFeedbackIntent.Confirm)) return false;
                clock.Now = 1.02d;
                if (provider.TryPulse(HapticFeedbackIntent.Selection)) return false;
                clock.Now = 1.07d;
                if (!provider.TryPulse(HapticFeedbackIntent.Selection)) return false;
                return provider.AcceptedPulseCount == 3 &&
                    provider.ThrottledPulseCount == 1 &&
                    bridge.PerformedCount == 3;
            }
        }

        static bool HapticLifecycleIsIdempotent()
        {
            var clock = new TestHapticClock { Now = 1d };
            var bridge = new TestHapticBridge();
            var provider = new PlatformHapticProvider(clock, bridge);
            if (!provider.TryPulse(HapticFeedbackIntent.Confirm)) return false;
            provider.Suspend();
            if (!provider.IsSuspended || provider.IsSupported) return false;
            if (provider.TryPulse(HapticFeedbackIntent.Selection)) return false;
            provider.Resume();
            if (provider.IsSuspended || !provider.IsSupported) return false;
            if (!provider.TryPulse(HapticFeedbackIntent.Selection)) return false;
            provider.Dispose();
            provider.Dispose();
            return provider.IsDisposed &&
                !provider.IsSupported &&
                !provider.TryPulse(HapticFeedbackIntent.Reject) &&
                bridge.SuspendCount == 2 &&
                bridge.DisposeCount == 1;
        }

        static bool ProductionProviderHasOnceOnlyGuards()
        {
            Type type = typeof(ProductionFeedbackProvider);
            return type.GetField(
                "hierarchyPrewarmed",
                BindingFlags.Instance | BindingFlags.NonPublic
            ) != null &&
                type.GetField(
                    "resourcesLoaded",
                    BindingFlags.Instance | BindingFlags.NonPublic
                ) != null &&
                type.GetMethod(
                    "PrewarmHierarchy",
                    BindingFlags.Instance | BindingFlags.NonPublic
                ) != null &&
                type.GetMethod(
                    "LoadResourcesOnce",
                    BindingFlags.Instance | BindingFlags.NonPublic
                ) != null &&
                HasPublicMethod(
                    type,
                    "Initialize",
                    typeof(CoreLoopFeedbackService),
                    typeof(MetaUiFeedbackService),
                    typeof(GameState)
                );
        }

        static bool ExactFidelityTargetsArePresent()
        {
            string[] ids = { "16x9", "18x9", "19_5x9", "20x9" };
            string[] labels = { "16:9", "18:9", "19.5:9", "20:9" };
            int[] heights = { 1920, 2160, 2340, 2400 };
            for (int i = 0; i < ids.Length; i++)
            {
                FidelityTarget target = FidelityVerification.GetTarget(i);
                if (
                    target.id != ids[i] ||
                    target.label != labels[i] ||
                    target.width != 1080 ||
                    target.height != heights[i]
                ) return false;
            }
            return true;
        }

        static bool ExactTargetViewportsAreStable()
        {
            float[] expectedY = { 0.5101f, 120.5101f, 210.5101f, 240.5101f };
            for (int i = 0; i < FidelityVerification.TargetCount; i++)
            {
                FidelityTarget target = FidelityVerification.GetTarget(i);
                Rect viewport = FidelityVerification.CalculateExpectedViewport(
                    new Rect(0f, 0f, target.width, target.height),
                    target.Size
                );
                if (
                    !Approximately(viewport.x, 0f, 0.002f) ||
                    !Approximately(viewport.y, expectedY[i], 0.002f) ||
                    !Approximately(viewport.width, 1080f, 0.002f) ||
                    !Approximately(viewport.height, 1918.9799f, 0.002f)
                ) return false;
            }
            return true;
        }

        static bool InvalidSafeAreaFallsBackToScreen()
        {
            Rect safe = FidelityVerification.NormalizeSafeArea(
                new Rect(float.NaN, 0f, 1080f, 2400f),
                new Vector2Int(1080, 2400)
            );
            return RectApproximately(
                safe,
                new Rect(0f, 0f, 1080f, 2400f)
            );
        }

        static bool SafeAreaClampingIsStable()
        {
            Rect safe = FidelityVerification.NormalizeSafeArea(
                new Rect(-20f, 80f, 1120f, 2400f),
                new Vector2Int(1080, 2400)
            );
            return RectApproximately(
                safe,
                new Rect(0f, 80f, 1080f, 2320f)
            );
        }

        static bool FidelityEvaluatorDetectsDrift()
        {
            var size = new Vector2Int(1080, 2400);
            var safe = new Rect(0f, 80f, 1080f, 2240f);
            Rect expected = FidelityVerification.CalculateExpectedViewport(safe, size);
            FidelityLayoutEvaluation pass = FidelityVerification.Evaluate(
                safe,
                size,
                expected
            );
            expected.x += 3f;
            FidelityLayoutEvaluation fail = FidelityVerification.Evaluate(
                safe,
                size,
                expected
            );
            return pass.passed && string.IsNullOrEmpty(pass.failure) &&
                !fail.passed && !string.IsNullOrEmpty(fail.failure);
        }

        static bool AllPhoneTargetsUseWidthMatch()
        {
            for (int i = 0; i < FidelityVerification.TargetCount; i++)
            {
                Vector2Int size = FidelityVerification.GetTarget(i).Size;
                float fidelity = FidelityVerification.ResolveCanvasMatch(size);
                float scaler = AdaptiveCanvasScaler.ResolveMatch(
                    size,
                    FidelityVerification.DesignWidth,
                    FidelityVerification.DesignHeight
                );
                if (!Approximately(fidelity, 0f) ||
                    !Approximately(fidelity, scaler)) return false;
            }
            return
                Approximately(
                    AdaptiveCanvasScaler.ResolveMatch(
                        new Vector2Int(1080, 1800),
                        FidelityVerification.DesignWidth,
                        FidelityVerification.DesignHeight
                    ),
                    1f
                ) &&
                Approximately(
                    AdaptiveCanvasScaler.ResolveScale(
                        new Vector2Int(1080, 1800),
                        FidelityVerification.DesignWidth,
                        FidelityVerification.DesignHeight
                    ),
                    1800f / 1672f
                );
        }

        static bool CaptureMetadataContractIsComplete()
        {
            string[] fields =
            {
                "targetId",
                "screenId",
                "modalId",
                "actualWidth",
                "actualHeight",
                "safeArea",
                "viewportRect",
                "canvasScaleFactor",
                "canvasMatchWidthOrHeight",
                "routeGeneration",
                "screenTransitioning",
                "modalTransitioning",
                "layout",
            };
            Type type = typeof(FidelityCaptureMetadata);
            for (int i = 0; i < fields.Length; i++)
                if (type.GetField(fields[i]) == null) return false;
            return true;
        }

        static bool RuntimePerformancePolicyIsStable()
        {
            return RuntimePerformanceProfile.TargetFramesPerSecond == 60 &&
                Math.Abs(
                    RuntimePerformanceProfile.FrameBudgetMilliseconds -
                    1000d / 60d
                ) <= 0.000001d &&
                RuntimePerformanceProfile.P95BudgetMilliseconds == 18d &&
                RuntimePerformanceProfile.P99BudgetMilliseconds == 25d &&
                RuntimePerformanceProfile.MinimumPassingSamples == 600 &&
                RuntimePerformanceProfile.MaximumSamples == 3600 &&
                RuntimePerformanceProfile.RequiresZeroManagedAllocation(
                    RuntimePerformanceScenario.GameplayIdle
                ) &&
                RuntimePerformanceProfile.RequiresZeroManagedAllocation(
                    RuntimePerformanceScenario.MetaIdle
                ) &&
                !RuntimePerformanceProfile.RequiresZeroManagedAllocation(
                    RuntimePerformanceScenario.GameplayActive
                );
        }

        static bool AccumulatorPercentilesAreStable()
        {
            var accumulator = new RuntimePerformanceAccumulator(5);
            accumulator.Begin(
                RuntimePerformanceScenario.GameplayActive,
                0,
                5,
                60,
                true,
                true
            );
            double[] samples = { 5d, 1d, 3d, 2d, 4d };
            for (int i = 0; i < samples.Length; i++)
            {
                if (!accumulator.TryPushSample(
                    (ulong)(i + 1),
                    samples[i],
                    1d,
                    1d,
                    0d,
                    2d,
                    0L
                )) return false;
            }
            if (!accumulator.TryBuildReport(out RuntimePerformanceReport report))
                return false;
            RuntimePerformanceMetricSummary metric = report.CpuTotalMilliseconds;
            return report.Verdict == RuntimePerformanceVerdict.Incomplete &&
                metric.Count == 5 &&
                metric.Minimum == 1d &&
                metric.Mean == 3d &&
                metric.P50 == 3d &&
                metric.P95 == 5d &&
                metric.P99 == 5d &&
                metric.Maximum == 5d;
        }

        static bool AccumulatorPassingGateIsStable()
        {
            var accumulator = new RuntimePerformanceAccumulator(600);
            accumulator.Begin(
                RuntimePerformanceScenario.GameplayIdle,
                0,
                600,
                60,
                true,
                true
            );
            for (int i = 0; i < 600; i++)
            {
                if (!accumulator.TryPushSample(
                    (ulong)(i + 1),
                    10d,
                    2d,
                    1d,
                    0d,
                    8d,
                    0L
                )) return false;
            }
            if (!accumulator.TryBuildReport(out RuntimePerformanceReport report) ||
                report.Verdict != RuntimePerformanceVerdict.Pass ||
                report.CpuTotalMilliseconds.P95 != 10d ||
                report.ManagedAllocationBytes.TotalBytes != 0L)
                return false;

            var gpuBound = new RuntimePerformanceAccumulator(600);
            gpuBound.Begin(
                RuntimePerformanceScenario.GameplayActive,
                0,
                600,
                60,
                true,
                true
            );
            for (int i = 0; i < 600; i++)
            {
                if (!gpuBound.TryPushSample(
                    (ulong)(i + 1),
                    10d,
                    2d,
                    1d,
                    0d,
                    50d,
                    0L
                )) return false;
            }
            return gpuBound.TryBuildReport(
                    out RuntimePerformanceReport gpuReport
                ) &&
                gpuReport.Verdict == RuntimePerformanceVerdict.Fail &&
                gpuReport.GpuMilliseconds.P95 == 50d &&
                gpuReport.SevereFrameRatio == 1d;
        }

        static bool UnsupportedPerformanceMetricsStayIncomplete()
        {
            var accumulator = new RuntimePerformanceAccumulator(600);
            accumulator.Begin(
                RuntimePerformanceScenario.GameplayIdle,
                0,
                600,
                60,
                false,
                false
            );
            return !accumulator.IsSampling &&
                accumulator.TryBuildReport(out RuntimePerformanceReport report) &&
                report.Verdict == RuntimePerformanceVerdict.Incomplete &&
                !report.FrameTimingSupported &&
                !report.GcCounterSupported &&
                !report.GpuTimingSupported;
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

        static bool HasPublicStaticMethod(
            Type type,
            string name,
            params Type[] arguments
        )
        {
            return type.GetMethod(
                name,
                BindingFlags.Static | BindingFlags.Public,
                null,
                arguments,
                null
            ) != null;
        }

        static bool Approximately(float first, float second, float tolerance = 0.0001f)
        {
            return Mathf.Abs(first - second) <= tolerance;
        }

        static bool RectApproximately(Rect first, Rect second)
        {
            return Approximately(first.x, second.x) &&
                Approximately(first.y, second.y) &&
                Approximately(first.width, second.width) &&
                Approximately(first.height, second.height);
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

        sealed class TestHapticClock : IHapticClock
        {
            public double Now;
            public double RealtimeSeconds => Now;
        }

        sealed class TestHapticBridge : IPlatformHapticBridge
        {
            public int PerformedCount;
            public int SuspendCount;
            public int DisposeCount;

            public bool IsSupported => true;

            public bool TryPerform(HapticFeedbackIntent intent)
            {
                PerformedCount++;
                return true;
            }

            public void Suspend() => SuspendCount++;
            public void Dispose() => DisposeCount++;
        }
    }
}
#endif
