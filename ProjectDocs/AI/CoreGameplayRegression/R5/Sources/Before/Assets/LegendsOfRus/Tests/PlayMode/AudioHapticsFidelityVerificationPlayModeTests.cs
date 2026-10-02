using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class AudioHapticsFidelityVerificationPlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        readonly List<GameObject> ownedObjects = new List<GameObject>();

        bool[] hadPersistedSaves;
        string[] persistedSaveJson;
        string runtimeSaveJson;
        GameState state;
        CoreLoopFeedbackService coreFeedback;
        MetaUiFeedbackService metaFeedback;
        ProductionFeedbackProvider productionProvider;

        [SetUp]
        public void SetUp()
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

            state = GameState.Instance;
            if (state == null)
                state = CreateComponent<GameState>("Step04_TestGameState");

            runtimeSaveJson = JsonUtility.ToJson(state.Save);
            state.Save = new PlayerSaveData
            {
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks,
                music = true,
                sfx = true,
                vibration = true,
            };

            coreFeedback = CoreLoopFeedbackService.Instance;
            if (coreFeedback == null)
                coreFeedback = CreateComponent<CoreLoopFeedbackService>(
                    "Step04_TestCoreFeedback"
                );

            metaFeedback = MetaUiFeedbackService.Instance;
            if (metaFeedback == null)
                metaFeedback = CreateComponent<MetaUiFeedbackService>(
                    "Step04_TestMetaFeedback"
                );

            productionProvider = ProductionFeedbackProvider.Instance;
            if (productionProvider == null)
                productionProvider = CreateComponent<ProductionFeedbackProvider>(
                    "Step04_TestProductionFeedback"
                );

            coreFeedback.ResetSinks();
            metaFeedback.ResetSinks();
        }

        [TearDown]
        public void TearDown()
        {
            if (coreFeedback != null)
                coreFeedback.ResetSinks();
            if (metaFeedback != null)
                metaFeedback.ResetSinks();

            if (state != null && !string.IsNullOrEmpty(runtimeSaveJson))
            {
                PlayerSaveData restored =
                    JsonUtility.FromJson<PlayerSaveData>(runtimeSaveJson);
                state.Save = restored ?? new PlayerSaveData();
            }

            if (
                productionProvider != null &&
                coreFeedback != null &&
                metaFeedback != null &&
                state != null
            )
            {
                productionProvider.Initialize(coreFeedback, metaFeedback, state);
            }

            for (int i = ownedObjects.Count - 1; i >= 0; i--)
            {
                if (ownedObjects[i] != null)
                    UnityEngine.Object.DestroyImmediate(ownedObjects[i]);
            }
            ownedObjects.Clear();

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
        public void MetaUiFeedback_StartPhasesEmitOnceAndCompletionPhasesAreFiltered()
        {
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalEnter,
                        MetaUiTransitionPhase.Entering,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Standard,
                    out MetaUiFeedbackSignal modalEnter,
                    out bool modalEnterHaptic
                ),
                Is.True
            );
            Assert.That(modalEnter.Cue, Is.EqualTo(MetaUiMotionCue.ModalEnter));
            Assert.That(modalEnterHaptic, Is.True);

            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalEnter,
                        MetaUiTransitionPhase.Entered,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Standard,
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalEnter,
                        MetaUiTransitionPhase.Exiting,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Standard,
                    out _,
                    out _
                ),
                Is.False,
                "ModalEnter must not emit from the opposite start phase."
            );
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalExit,
                        MetaUiTransitionPhase.Exiting,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Back,
                    out MetaUiFeedbackSignal modalExit,
                    out bool modalExitHaptic
                ),
                Is.True
            );
            Assert.That(modalExit.Cue, Is.EqualTo(MetaUiMotionCue.ModalExit));
            Assert.That(modalExitHaptic, Is.False);
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalExit,
                        MetaUiTransitionPhase.Exited,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Back,
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    Trace(
                        MetaUiMotionCue.ModalExit,
                        MetaUiTransitionPhase.Entering,
                        MetaUiMotionPreset.ModalStandard
                    ),
                    MetaUiControlRole.Back,
                    out _,
                    out _
                ),
                Is.False,
                "ModalExit must not emit from the opposite start phase."
            );
        }

        [Test]
        public void MetaUiFeedback_AcceptedActionsPreserveIdentityAndRequestRoleHaptics()
        {
            MetaUiMotionTrace accepted = new MetaUiMotionTrace(
                17,
                MetaUiMotionCue.ButtonActivated,
                MetaUiTransitionPhase.Entering,
                MetaUiMotionPreset.ScreenStandard,
                "Home/Play"
            );

            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    accepted,
                    MetaUiControlRole.Primary,
                    out MetaUiFeedbackSignal signal,
                    out bool requestHaptic
                ),
                Is.True
            );
            Assert.That(signal.Cue, Is.EqualTo(MetaUiMotionCue.ButtonActivated));
            Assert.That(signal.Role, Is.EqualTo(MetaUiControlRole.Primary));
            Assert.That(signal.Preset, Is.EqualTo(MetaUiMotionPreset.ScreenStandard));
            Assert.That(signal.Target, Is.EqualTo("Home/Play"));
            Assert.That(requestHaptic, Is.True);

            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    accepted,
                    MetaUiControlRole.Standard,
                    out _,
                    out bool standardHaptic
                ),
                Is.True
            );
            Assert.That(standardHaptic, Is.False);
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    accepted,
                    MetaUiControlRole.Gameplay,
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                MetaUiFeedbackService.TryCreateSignal(
                    new MetaUiMotionTrace(
                        17,
                        MetaUiMotionCue.ButtonActivated,
                        MetaUiTransitionPhase.Entered,
                        MetaUiMotionPreset.ScreenStandard,
                        "Home/Play"
                    ),
                    MetaUiControlRole.Primary,
                    out _,
                    out _
                ),
                Is.False
            );
        }

        [Test]
        public void MetaUiFeedback_SettingsAreReadForEverySignal()
        {
            var sfx = new MetaSfxProbe();
            var haptic = new MetaHapticProbe();
            metaFeedback.ConfigureSinks(sfx, haptic);
            int beforeSignals = metaFeedback.SignalCount;
            MetaUiMotionTrace trace = AcceptedButtonTrace();

            state.Save.sfx = false;
            state.Save.vibration = false;
            MetaUiFeedbackService.Publish(trace, MetaUiControlRole.Primary);
            Assert.That(sfx.Calls, Is.Zero);
            Assert.That(haptic.Calls, Is.Zero);

            state.Save.sfx = true;
            MetaUiFeedbackService.Publish(trace, MetaUiControlRole.Primary);
            Assert.That(sfx.Calls, Is.EqualTo(1));
            Assert.That(haptic.Calls, Is.Zero);

            state.Save.vibration = true;
            MetaUiFeedbackService.Publish(trace, MetaUiControlRole.Primary);
            Assert.That(sfx.Calls, Is.EqualTo(2));
            Assert.That(haptic.Calls, Is.EqualTo(1));
            Assert.That(metaFeedback.SignalCount - beforeSignals, Is.EqualTo(3));
        }

        [Test]
        public void MetaUiFeedback_FaultyObserversAndSinksAreIsolated()
        {
            var sfx = new ThrowingMetaSfxSink();
            var haptic = new ThrowingMetaHapticSink();
            int healthyObserverCalls = 0;
            Action<MetaUiFeedbackSignal> faultyObserver = _ =>
                throw new InvalidOperationException("observer");
            Action<MetaUiFeedbackSignal> healthyObserver = _ =>
                healthyObserverCalls++;
            metaFeedback.SignalPublished += faultyObserver;
            metaFeedback.SignalPublished += healthyObserver;
            metaFeedback.ConfigureSinks(sfx, haptic);

            LogAssert.Expect(
                LogType.Warning,
                "Meta UI feedback observer failed and was isolated: observer"
            );
            LogAssert.Expect(
                LogType.Warning,
                "Meta UI SFX sink failed and was isolated: sfx"
            );
            LogAssert.Expect(
                LogType.Warning,
                "Meta UI haptic sink failed and was isolated: haptic"
            );

            try
            {
                Assert.DoesNotThrow(() => MetaUiFeedbackService.Publish(
                    AcceptedButtonTrace(),
                    MetaUiControlRole.Primary
                ));
                Assert.DoesNotThrow(() => MetaUiFeedbackService.Publish(
                    AcceptedButtonTrace(),
                    MetaUiControlRole.Primary
                ));
            }
            finally
            {
                metaFeedback.SignalPublished -= faultyObserver;
                metaFeedback.SignalPublished -= healthyObserver;
            }

            Assert.That(healthyObserverCalls, Is.EqualTo(2));
            Assert.That(sfx.Calls, Is.EqualTo(2));
            Assert.That(haptic.Calls, Is.EqualTo(2));
        }

        [Test]
        public void CoreLoopFeedback_PreservesEveryMotionMetadataField()
        {
            var motion = new GameplayMotionEvent(
                19,
                GameplayMotionKind.BoosterActivated,
                new BoardPosition(2, 3),
                new BoardPosition(4, 5),
                7,
                12,
                SpecialGemKind.Color,
                "A1B2C3D4",
                SpecialCombinationKind.ColorWithBurst,
                GameplayBoosterKind.Firebird
            );

            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    motion,
                    out CoreLoopFeedbackSignal signal,
                    out bool requestHaptic
                ),
                Is.True
            );
            Assert.That(signal.Cue, Is.EqualTo(CoreLoopFeedbackCue.BoosterActivated));
            Assert.That(signal.Magnitude, Is.EqualTo(12));
            Assert.That(signal.CascadeDepth, Is.EqualTo(7));
            Assert.That(signal.PlanHash, Is.EqualTo("A1B2C3D4"));
            Assert.That(signal.BoosterKind, Is.EqualTo(GameplayBoosterKind.Firebird));
            Assert.That(signal.Sequence, Is.EqualTo(19));
            Assert.That(signal.SpecialKind, Is.EqualTo(SpecialGemKind.Color));
            Assert.That(
                signal.CombinationKind,
                Is.EqualTo(SpecialCombinationKind.ColorWithBurst)
            );
            Assert.That(requestHaptic, Is.True);
        }

        [Test]
        public void CoreLoopFeedback_DeduplicatesLaterCascadeAndActivationBeats()
        {
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.MatchWaveStarted, 2, 5),
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SpecialActivationStarted, 1, 0),
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.SwapCommitted, 1, 0),
                    out _,
                    out _
                ),
                Is.False
            );
            Assert.That(
                CoreLoopFeedbackService.TryCreateSignal(
                    Motion(GameplayMotionKind.MatchWaveStarted, 1, 3),
                    out CoreLoopFeedbackSignal firstWave,
                    out bool firstWaveHaptic
                ),
                Is.True
            );
            Assert.That(firstWave.Cue, Is.EqualTo(CoreLoopFeedbackCue.MatchResolved));
            Assert.That(firstWaveHaptic, Is.True);
        }

        [Test]
        public void PlatformHaptics_MapsEveryCoreLoopCueToSemanticIntent()
        {
            var expected = new Dictionary<CoreLoopFeedbackCue, HapticFeedbackIntent>
            {
                { CoreLoopFeedbackCue.SwapAccepted, HapticFeedbackIntent.None },
                { CoreLoopFeedbackCue.SwapRejected, HapticFeedbackIntent.Reject },
                { CoreLoopFeedbackCue.InvalidSwap, HapticFeedbackIntent.Reject },
                { CoreLoopFeedbackCue.MatchResolved, HapticFeedbackIntent.Confirm },
                { CoreLoopFeedbackCue.CascadeResolved, HapticFeedbackIntent.Confirm },
                { CoreLoopFeedbackCue.FallSettled, HapticFeedbackIntent.None },
                { CoreLoopFeedbackCue.SpecialCreated, HapticFeedbackIntent.Emphasis },
                { CoreLoopFeedbackCue.SpecialActivated, HapticFeedbackIntent.Emphasis },
                { CoreLoopFeedbackCue.Reshuffled, HapticFeedbackIntent.Emphasis },
                { CoreLoopFeedbackCue.CommandRecovered, HapticFeedbackIntent.Reject },
                { CoreLoopFeedbackCue.BoosterActivated, HapticFeedbackIntent.Emphasis },
                { CoreLoopFeedbackCue.BoosterCompleted, HapticFeedbackIntent.None },
                { CoreLoopFeedbackCue.BoosterRejected, HapticFeedbackIntent.Reject },
            };

            foreach (KeyValuePair<CoreLoopFeedbackCue, HapticFeedbackIntent> pair in expected)
            {
                var signal = new CoreLoopFeedbackSignal(pair.Key, 3, 1, "plan");
                Assert.That(
                    PlatformHapticProvider.ResolveIntent(signal),
                    Is.EqualTo(pair.Value),
                    pair.Key.ToString()
                );
            }
            Assert.That(
                PlatformHapticProvider.ResolveIntent((CoreLoopFeedbackSignal)null),
                Is.EqualTo(HapticFeedbackIntent.None)
            );
        }

        [Test]
        public void PlatformHaptics_MapsMetaRolesAndOutcomeCuesToSemanticIntent()
        {
            AssertMetaIntent(
                MetaUiMotionCue.ButtonActivated,
                MetaUiControlRole.Standard,
                HapticFeedbackIntent.Selection
            );
            AssertMetaIntent(
                MetaUiMotionCue.ButtonActivated,
                MetaUiControlRole.Primary,
                HapticFeedbackIntent.Confirm
            );
            AssertMetaIntent(
                MetaUiMotionCue.ButtonActivated,
                MetaUiControlRole.MapNode,
                HapticFeedbackIntent.Confirm
            );
            AssertMetaIntent(
                MetaUiMotionCue.ButtonActivated,
                MetaUiControlRole.Reward,
                HapticFeedbackIntent.Emphasis
            );
            AssertMetaIntent(
                MetaUiMotionCue.ButtonActivated,
                MetaUiControlRole.Gameplay,
                HapticFeedbackIntent.None
            );
            AssertMetaIntent(
                MetaUiMotionCue.VictoryReveal,
                MetaUiControlRole.Standard,
                HapticFeedbackIntent.Emphasis
            );
            AssertMetaIntent(
                MetaUiMotionCue.FailureReveal,
                MetaUiControlRole.Standard,
                HapticFeedbackIntent.Reject
            );
            AssertMetaIntent(
                MetaUiMotionCue.ModalEnter,
                MetaUiControlRole.Standard,
                HapticFeedbackIntent.Selection
            );
        }

        [Test]
        public void PlatformHaptics_UsesExactSixtyMillisecondBoundary()
        {
            var clock = new HapticClockProbe { Now = 1d };
            var bridge = new HapticBridgeProbe();
            using (var provider = new PlatformHapticProvider(clock, bridge))
            {
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.True);
                clock.Now = 1.059999d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.False);
                clock.Now = 1.06d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.True);
                Assert.That(provider.AcceptedPulseCount, Is.EqualTo(2));
                Assert.That(provider.ThrottledPulseCount, Is.EqualTo(1));
                Assert.That(bridge.Performed.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void PlatformHaptics_StrongerIntentBypassesGuardButLowerOrEqualDoesNot()
        {
            var clock = new HapticClockProbe { Now = 3d };
            var bridge = new HapticBridgeProbe();
            using (var provider = new PlatformHapticProvider(clock, bridge))
            {
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.True);
                clock.Now = 3.01d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Confirm), Is.True);
                clock.Now = 3.02d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.False);
                clock.Now = 3.03d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Confirm), Is.False);
                clock.Now = 3.04d;
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Reject), Is.True);

                Assert.That(
                    bridge.Performed,
                    Is.EqualTo(new[]
                    {
                        HapticFeedbackIntent.Selection,
                        HapticFeedbackIntent.Confirm,
                        HapticFeedbackIntent.Reject,
                    })
                );
                Assert.That(provider.AcceptedPulseCount, Is.EqualTo(3));
                Assert.That(provider.ThrottledPulseCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void PlatformHaptics_UnsupportedAndFailedBridgesAreSafeNoOps()
        {
            var clock = new HapticClockProbe { Now = 4d };
            var unsupported = new HapticBridgeProbe { Supported = false };
            using (var provider = new PlatformHapticProvider(clock, unsupported))
            {
                Assert.That(provider.IsSupported, Is.False);
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Reject), Is.False);
                Assert.That(provider.TryPulse(HapticFeedbackIntent.None), Is.False);
                Assert.That(provider.AcceptedPulseCount, Is.Zero);
                Assert.That(unsupported.Performed, Is.Empty);
            }

            var failed = new HapticBridgeProbe { PerformResult = false };
            using (var provider = new PlatformHapticProvider(clock, failed))
            {
                Assert.That(provider.TryPulse(HapticFeedbackIntent.Confirm), Is.False);
                Assert.That(provider.AcceptedPulseCount, Is.Zero);
                Assert.That(provider.ThrottledPulseCount, Is.Zero);
                Assert.That(failed.Performed.Count, Is.EqualTo(1));
            }

            Assert.DoesNotThrow(() =>
            {
                using (var provider = new PlatformHapticProvider(null, null))
                    Assert.That(provider.TryPulse(HapticFeedbackIntent.Confirm), Is.False);
            });
        }

        [Test]
        public void PlatformHaptics_SuspendResetsThrottleAndDisposeIsIdempotent()
        {
            var clock = new HapticClockProbe { Now = 5d };
            var bridge = new HapticBridgeProbe();
            var provider = new PlatformHapticProvider(clock, bridge);

            Assert.That(provider.TryPulse(HapticFeedbackIntent.Confirm), Is.True);
            provider.Suspend();
            Assert.That(provider.LastAcceptedIntent, Is.EqualTo(HapticFeedbackIntent.None));
            Assert.That(provider.IsSuspended, Is.True);
            Assert.That(provider.IsSupported, Is.False);
            Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.False);
            provider.Resume();
            Assert.That(provider.IsSuspended, Is.False);
            Assert.That(provider.TryPulse(HapticFeedbackIntent.Selection), Is.True);

            provider.Dispose();
            provider.Dispose();
            Assert.That(provider.IsDisposed, Is.True);
            Assert.That(provider.IsSupported, Is.False);
            Assert.That(provider.TryPulse(HapticFeedbackIntent.Reject), Is.False);
            Assert.That(bridge.SuspendCalls, Is.EqualTo(2));
            Assert.That(bridge.DisposeCalls, Is.EqualTo(1));
        }

        [Test]
        public void FidelityVerification_ExposesTheFourExactPortraitTargets()
        {
            Assert.That(FidelityVerification.TargetCount, Is.EqualTo(4));
            AssertTarget(0, "16x9", "16:9", 1080, 1920);
            AssertTarget(1, "18x9", "18:9", 1080, 2160);
            AssertTarget(2, "19_5x9", "19.5:9", 1080, 2340);
            AssertTarget(3, "20x9", "20:9", 1080, 2400);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => FidelityVerification.GetTarget(-1)
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => FidelityVerification.GetTarget(4)
            );
        }

        [Test]
        public void FidelityVerification_CalculatesExpectedViewportForEveryTarget()
        {
            float[] expectedY = { 0.5101f, 120.5101f, 210.5101f, 240.5101f };
            const float expectedHeight = 1918.9799f;

            for (int i = 0; i < FidelityVerification.TargetCount; i++)
            {
                FidelityTarget target = FidelityVerification.GetTarget(i);
                Rect full = new Rect(0f, 0f, target.width, target.height);
                Rect viewport = FidelityVerification.CalculateExpectedViewport(
                    full,
                    target.Size
                );
                Assert.That(viewport.x, Is.EqualTo(0f).Within(0.001f), target.id);
                Assert.That(
                    viewport.y,
                    Is.EqualTo(expectedY[i]).Within(0.001f),
                    target.id
                );
                Assert.That(
                    viewport.width,
                    Is.EqualTo(1080f).Within(0.001f),
                    target.id
                );
                Assert.That(
                    viewport.height,
                    Is.EqualTo(expectedHeight).Within(0.001f),
                    target.id
                );
                Assert.That(
                    FidelityVerification.ResolveCanvasMatch(target.Size),
                    Is.EqualTo(0f),
                    target.id
                );
            }
        }

        [Test]
        public void FidelityVerification_NormalizesInvalidAndClampedSafeAreas()
        {
            var size = new Vector2Int(1080, 2400);
            Rect full = FidelityVerification.NormalizeSafeArea(
                new Rect(float.NaN, 0f, 1080f, 2400f),
                size
            );
            Assert.That(full, Is.EqualTo(new Rect(0f, 0f, 1080f, 2400f)));

            Rect clamped = FidelityVerification.NormalizeSafeArea(
                new Rect(-30f, 80f, 1140f, 2400f),
                size
            );
            Assert.That(clamped, Is.EqualTo(new Rect(0f, 80f, 1080f, 2320f)));

            Rect collapsed = FidelityVerification.NormalizeSafeArea(
                new Rect(500f, 500f, 0.2f, 0.2f),
                size
            );
            Assert.That(collapsed, Is.EqualTo(new Rect(0f, 0f, 1080f, 2400f)));
        }

        [Test]
        public void FidelityVerification_EvaluationPassesExactPlanAndRejectsDrift()
        {
            var size = new Vector2Int(1080, 2400);
            var safe = new Rect(0f, 80f, 1080f, 2240f);
            Rect expected = FidelityVerification.CalculateExpectedViewport(safe, size);

            FidelityLayoutEvaluation pass = FidelityVerification.Evaluate(
                safe,
                size,
                expected
            );
            Assert.That(pass.passed, Is.True);
            Assert.That(pass.failure, Is.Empty);
            Assert.That(
                pass.maximumEdgeErrorPixels,
                Is.EqualTo(0f).Within(0.0001f)
            );

            Rect drifted = expected;
            drifted.x += 3f;
            FidelityLayoutEvaluation fail = FidelityVerification.Evaluate(
                safe,
                size,
                drifted,
                1f
            );
            Assert.That(fail.passed, Is.False);
            Assert.That(fail.maximumEdgeErrorPixels, Is.GreaterThan(1f));
            Assert.That(fail.failure, Is.Not.Empty);

            FidelityLayoutEvaluation invalid = FidelityVerification.Evaluate(
                safe,
                size,
                new Rect(0f, 0f, 0f, 0f)
            );
            Assert.That(invalid.passed, Is.False);
            Assert.That(invalid.failure, Is.EqualTo("Viewport bounds are invalid."));
        }

        [Test]
        public void SafeAreaFitter_ApplyUsesInjectedScreenAndSafeArea()
        {
            GameObject host = CreateObject(
                "Step04_SafeAreaFitter",
                typeof(RectTransform),
                typeof(SafeAreaFitter)
            );
            var fitter = host.GetComponent<SafeAreaFitter>();
            var rect = host.GetComponent<RectTransform>();
            var size = new Vector2Int(1080, 2400);
            var safe = new Rect(0f, 96f, 1080f, 2208f);

            fitter.Apply(safe, size);

            Assert.That(rect.anchorMin.x, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(rect.anchorMin.y, Is.EqualTo(0.04f).Within(0.0001f));
            Assert.That(rect.anchorMax.x, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(rect.anchorMax.y, Is.EqualTo(0.96f).Within(0.0001f));
            Assert.That(rect.offsetMin, Is.EqualTo(Vector2.zero));
            Assert.That(rect.offsetMax, Is.EqualTo(Vector2.zero));
            Assert.That(fitter.LastAppliedSafeArea, Is.EqualTo(safe));
            Assert.That(fitter.LastAppliedScreenSize, Is.EqualTo(size));
        }

        [Test]
        public void AdaptiveCanvasScaler_ApplyUsesSafeAreaFitScaleAndCrossover()
        {
            GameObject host = CreateObject(
                "Step04_AdaptiveScaler",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(AdaptiveCanvasScaler)
            );
            var adaptive = host.GetComponent<AdaptiveCanvasScaler>();
            var scaler = host.GetComponent<CanvasScaler>();
            adaptive.DesignWidth = FidelityVerification.DesignWidth;
            adaptive.DesignHeight = FidelityVerification.DesignHeight;

            adaptive.Apply(new Vector2Int(1080, 2400));
            Assert.That(adaptive.LastAppliedMatch, Is.EqualTo(0f));
            Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(0f));
            Assert.That(
                scaler.uiScaleMode,
                Is.EqualTo(CanvasScaler.ScaleMode.ConstantPixelSize)
            );
            Assert.That(
                adaptive.LastAppliedScale,
                Is.EqualTo(1080f / 941f).Within(0.0001f)
            );
            Assert.That(
                adaptive.LastAppliedScreenSize,
                Is.EqualTo(new Vector2Int(1080, 2400))
            );

            var screenSize = new Vector2Int(1080, 1920);
            var insetSafeArea = new Rect(0f, 60f, 1080f, 1800f);
            adaptive.Apply(insetSafeArea, screenSize);
            Assert.That(adaptive.LastAppliedMatch, Is.EqualTo(1f));
            Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(1f));
            Assert.That(
                adaptive.LastAppliedScale,
                Is.EqualTo(1800f / 1672f).Within(0.0001f)
            );
            Assert.That(scaler.scaleFactor, Is.EqualTo(1800f / 1672f).Within(0.0001f));
            Assert.That(adaptive.LastAppliedSafeArea, Is.EqualTo(insetSafeArea));
            Assert.That(adaptive.LastAppliedScreenSize, Is.EqualTo(screenSize));
            Assert.That(
                AdaptiveCanvasScaler.ResolveScale(
                    new Vector2Int(1080, 1800),
                    941f,
                    1672f
                ),
                Is.EqualTo(1800f / 1672f).Within(0.0001f)
            );
        }

        [Test]
        public void AdaptiveCanvasScaler_RefreshesFixedTextOnlyWhenActualScaleChanges()
        {
            GameObject host = CreateObject("TextScaleCanvas", typeof(RectTransform),
                typeof(Canvas), typeof(CanvasScaler), typeof(AdaptiveCanvasScaler));
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var adaptive = host.GetComponent<AdaptiveCanvasScaler>();
            GameObject child = CreateObject("FixedSizeLabel", typeof(RectTransform), typeof(Text));
            child.transform.SetParent(host.transform, false);
            var label = child.GetComponent<Text>();
            label.font = Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");
            label.text = "Настройки / Settings";
            label.fontSize = 36;
            label.rectTransform.sizeDelta = new Vector2(400, 60);
            canvas.scaleFactor = .3f;
            adaptive.SendMessage("LateUpdate");
            Canvas.ForceUpdateCanvases();
            int refreshes = 0;
            label.RegisterDirtyVerticesCallback(() => refreshes++);

            // Same rect size, different raster density. Do not rely on a layout resize.
            canvas.scaleFactor = .8f;
            refreshes = 0;
            adaptive.SendMessage("LateUpdate");
            Assert.That(refreshes, Is.EqualTo(1));
            Assert.That(label.GetGenerationSettings(label.rectTransform.rect.size).scaleFactor,
                Is.EqualTo(.8f).Within(.0001f));
            Assert.That(label.rectTransform.sizeDelta, Is.EqualTo(new Vector2(400, 60)));
            adaptive.SendMessage("LateUpdate");
            Assert.That(refreshes, Is.EqualTo(1), "Stable frames must not rebuild all text");

            canvas.scaleFactor = .3f;
            refreshes = 0;
            adaptive.SendMessage("LateUpdate");
            Assert.That(refreshes, Is.EqualTo(1), "Shrinking must also refresh the glyph mesh");
        }

        [Test]
        public void QACaptureService_AllowsOnlyOnePendingRequestWithoutCapturing()
        {
            GameObject host = CreateObject(
                "Step04_QACaptureLatch",
                typeof(QACaptureService)
            );
            var capture = host.GetComponent<QACaptureService>();
            var request = new FidelityCaptureRequest
            {
                captureId = "latch-only",
                targetId = "20x9",
                targetLabel = "20:9",
                requestedWidth = 1080,
                requestedHeight = 2400,
                showToast = false,
            };

            Assert.That(capture.TryRequestCapture(request), Is.True);
            Assert.That(capture.IsCapturePending, Is.True);
            Assert.That(capture.TryRequestCapture(request), Is.False);
            Assert.That(capture.TryRequestCapture(null), Is.False);

            // Cancel before WaitForEndOfFrame resumes, so this contract test
            // never reads pixels or writes a QA artifact.
            Assert.That(capture.CancelPendingCapture(), Is.True);
            Assert.That(capture.IsCapturePending, Is.False);
            Assert.That(capture.CancelPendingCapture(), Is.False);
        }

        [Test]
        public void RuntimePerformanceAccumulator_UsesNearestRankPercentiles()
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
                Assert.That(
                    accumulator.TryPushSample(
                        (ulong)(i + 1),
                        samples[i],
                        1d,
                        1d,
                        0d,
                        2d,
                        0L
                    ),
                    Is.True
                );
            }

            Assert.That(accumulator.TryBuildReport(out RuntimePerformanceReport report), Is.True);
            Assert.That(report.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Incomplete));
            Assert.That(report.CpuTotalMilliseconds.Minimum, Is.EqualTo(1d));
            Assert.That(report.CpuTotalMilliseconds.Mean, Is.EqualTo(3d));
            Assert.That(report.CpuTotalMilliseconds.P50, Is.EqualTo(3d));
            Assert.That(report.CpuTotalMilliseconds.P95, Is.EqualTo(5d));
            Assert.That(report.CpuTotalMilliseconds.P99, Is.EqualTo(5d));
            Assert.That(report.CpuTotalMilliseconds.Maximum, Is.EqualTo(5d));
        }

        [Test]
        public void RuntimePerformanceAccumulator_AppliesFrameAndAllocationGates()
        {
            RuntimePerformanceReport passing = BuildPerformanceReport(
                RuntimePerformanceScenario.GameplayIdle,
                i => 10d,
                i => 0L
            );
            Assert.That(passing.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Pass));
            Assert.That(passing.CpuTotalMilliseconds.P95, Is.EqualTo(10d));
            Assert.That(passing.ManagedAllocationBytes.TotalBytes, Is.Zero);

            RuntimePerformanceReport slow = BuildPerformanceReport(
                RuntimePerformanceScenario.GameplayActive,
                i => i < 31 ? 20d : 10d,
                i => 32L
            );
            Assert.That(slow.CpuTotalMilliseconds.P95, Is.EqualTo(20d));
            Assert.That(slow.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Fail));

            RuntimePerformanceReport allocatingIdle = BuildPerformanceReport(
                RuntimePerformanceScenario.MetaIdle,
                i => 10d,
                i => i == 0 ? 1L : 0L
            );
            Assert.That(
                allocatingIdle.Verdict,
                Is.EqualTo(RuntimePerformanceVerdict.Fail)
            );

            RuntimePerformanceReport allocatingActive = BuildPerformanceReport(
                RuntimePerformanceScenario.MetaTransition,
                i => 10d,
                i => 64L
            );
            Assert.That(
                allocatingActive.Verdict,
                Is.EqualTo(RuntimePerformanceVerdict.Pass)
            );

            RuntimePerformanceReport gpuBound = BuildPerformanceReport(
                RuntimePerformanceScenario.GameplayActive,
                i => 10d,
                i => 0L,
                i => 50d
            );
            Assert.That(gpuBound.GpuMilliseconds.P95, Is.EqualTo(50d));
            Assert.That(gpuBound.SevereFrameRatio, Is.EqualTo(1d));
            Assert.That(gpuBound.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Fail));
        }

        [Test]
        public void RuntimePerformanceAccumulator_UnsupportedMetricsStayIncomplete()
        {
            var noTiming = new RuntimePerformanceAccumulator(600);
            noTiming.Begin(
                RuntimePerformanceScenario.GameplayIdle,
                0,
                600,
                60,
                false,
                true
            );
            Assert.That(noTiming.IsSampling, Is.False);
            Assert.That(noTiming.TryBuildReport(out RuntimePerformanceReport noTimingReport), Is.True);
            Assert.That(
                noTimingReport.Verdict,
                Is.EqualTo(RuntimePerformanceVerdict.Incomplete)
            );
            Assert.That(noTimingReport.FrameTimingSupported, Is.False);

            var noGc = new RuntimePerformanceAccumulator(600);
            noGc.Begin(
                RuntimePerformanceScenario.GameplayIdle,
                0,
                600,
                60,
                true,
                false
            );
            PushUniformSamples(noGc, 600, 10d, 8d, -1L);
            Assert.That(noGc.TryBuildReport(out RuntimePerformanceReport noGcReport), Is.True);
            Assert.That(noGcReport.GcCounterSupported, Is.False);
            Assert.That(noGcReport.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Incomplete));

            var noGpu = new RuntimePerformanceAccumulator(600);
            noGpu.Begin(
                RuntimePerformanceScenario.GameplayIdle,
                0,
                600,
                60,
                true,
                true
            );
            PushUniformSamples(noGpu, 600, 10d, 0d, 0L);
            Assert.That(noGpu.TryBuildReport(out RuntimePerformanceReport noGpuReport), Is.True);
            Assert.That(noGpuReport.GpuTimingSupported, Is.False);
            Assert.That(noGpuReport.Verdict, Is.EqualTo(RuntimePerformanceVerdict.Incomplete));
        }

        [Test]
        public void ProductionFeedbackProvider_ConfiguresVariantsAndFallsBackWhenClipsAreMissing()
        {
            productionProvider.Initialize(coreFeedback, metaFeedback, state);

            object configuredMatchFour = PrivateRecipe(
                productionProvider,
                "matchFourRecipe"
            );
            object configuredMatchFive = PrivateRecipe(
                productionProvider,
                "matchFiveRecipe"
            );
            object configuredReward = PrivateRecipe(
                productionProvider,
                "rewardButtonRecipe"
            );
            object configuredMap = PrivateRecipe(
                productionProvider,
                "mapButtonRecipe"
            );
            Assert.That(
                RecipeFloat(configuredMatchFour, "Volume"),
                Is.EqualTo(0.86f).Within(0.001f)
            );
            Assert.That(
                RecipeFloat(configuredMatchFive, "Volume"),
                Is.EqualTo(0.90f).Within(0.001f)
            );
            Assert.That(
                RecipeFloat(configuredReward, "Volume"),
                Is.EqualTo(0.82f).Within(0.001f)
            );
            Assert.That(
                RecipeFloat(configuredMap, "Volume"),
                Is.EqualTo(0.78f).Within(0.001f)
            );

            object matchThree = ResolveCoreRecipe(
                productionProvider,
                new CoreLoopFeedbackSignal(
                    CoreLoopFeedbackCue.MatchResolved,
                    3,
                    1,
                    "plan"
                )
            );
            object matchFour = ResolveCoreRecipe(
                productionProvider,
                new CoreLoopFeedbackSignal(
                    CoreLoopFeedbackCue.MatchResolved,
                    4,
                    1,
                    "plan"
                )
            );
            object matchFive = ResolveCoreRecipe(
                productionProvider,
                new CoreLoopFeedbackSignal(
                    CoreLoopFeedbackCue.MatchResolved,
                    5,
                    1,
                    "plan"
                )
            );
            Assert.That(RecipeFloat(matchThree, "Volume"), Is.EqualTo(0.82f).Within(0.001f));
            Assert.That(RecipeFloat(matchFour, "Volume"), Is.EqualTo(0.82f).Within(0.001f));
            Assert.That(RecipeFloat(matchFive, "Volume"), Is.EqualTo(0.82f).Within(0.001f));

            object standard = ResolveMetaRecipe(
                productionProvider,
                MetaUiControlRole.Standard
            );
            object reward = ResolveMetaRecipe(
                productionProvider,
                MetaUiControlRole.Reward
            );
            object map = ResolveMetaRecipe(
                productionProvider,
                MetaUiControlRole.MapNode
            );
            Assert.That(RecipeFloat(standard, "Volume"), Is.EqualTo(0.72f).Within(0.001f));
            Assert.That(RecipeFloat(reward, "Volume"), Is.EqualTo(0.72f).Within(0.001f));
            Assert.That(RecipeFloat(map, "Volume"), Is.EqualTo(0.72f).Within(0.001f));
        }

        [Test]
        public void ProductionFeedbackProvider_PrewarmsHierarchyOnceAndMissingClipsNoOp()
        {
            productionProvider.Initialize(coreFeedback, metaFeedback, state);
            int firstSourceCount =
                productionProvider.GetComponents<AudioSource>().Length;
            Assert.That(firstSourceCount, Is.EqualTo(ProductionFeedbackProvider.SfxVoiceCapacity + 1));
            Assert.That(productionProvider.PoolCapacity, Is.EqualTo(4));
            Assert.That(productionProvider.HasAudioListener, Is.True);

            productionProvider.Initialize(coreFeedback, metaFeedback, state);
            Assert.That(
                productionProvider.GetComponents<AudioSource>().Length,
                Is.EqualTo(firstSourceCount)
            );

            int beforeMissing = productionProvider.MissingClipRequestCount;
            int beforePlayed = productionProvider.PlayedSfxCount;
            bool previousPaused = PrivateFieldValue<bool>(
                productionProvider,
                "applicationPaused"
            );
            bool previousFocused = PrivateFieldValue<bool>(
                productionProvider,
                "applicationFocused"
            );
            try
            {
                SetPrivateField(productionProvider, "applicationPaused", false);
                SetPrivateField(productionProvider, "applicationFocused", true);
                productionProvider.Play(new MetaUiFeedbackSignal(
                    MetaUiMotionCue.ScreenEnter,
                    MetaUiControlRole.Standard,
                    MetaUiMotionPreset.ScreenStandard,
                    "missing-recipe"
                ));
                Assert.That(
                    productionProvider.MissingClipRequestCount,
                    Is.EqualTo(beforeMissing + 1)
                );
                Assert.That(
                    productionProvider.PlayedSfxCount,
                    Is.EqualTo(beforePlayed)
                );
                Assert.DoesNotThrow(() => productionProvider.Play(
                    (CoreLoopFeedbackSignal)null
                ));
            }
            finally
            {
                SetPrivateField(
                    productionProvider,
                    "applicationPaused",
                    previousPaused
                );
                SetPrivateField(
                    productionProvider,
                    "applicationFocused",
                    previousFocused
                );
            }
        }

        T CreateComponent<T>(string name) where T : Component
        {
            GameObject host = CreateObject(name);
            return host.AddComponent<T>();
        }

        GameObject CreateObject(string name, params Type[] components)
        {
            var host = new GameObject(name, components);
            ownedObjects.Add(host);
            return host;
        }

        static MetaUiMotionTrace Trace(
            MetaUiMotionCue cue,
            MetaUiTransitionPhase phase,
            MetaUiMotionPreset preset
        )
        {
            return new MetaUiMotionTrace(1, cue, phase, preset, "Step04/Test");
        }

        static MetaUiMotionTrace AcceptedButtonTrace()
        {
            return Trace(
                MetaUiMotionCue.ButtonActivated,
                MetaUiTransitionPhase.Entering,
                MetaUiMotionPreset.ScreenStandard
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
                SpecialGemKind.None,
                "step04-plan"
            );
        }

        static void AssertMetaIntent(
            MetaUiMotionCue cue,
            MetaUiControlRole role,
            HapticFeedbackIntent expected
        )
        {
            var signal = new MetaUiFeedbackSignal(
                cue,
                role,
                MetaUiMotionPreset.ScreenStandard,
                "target"
            );
            Assert.That(
                PlatformHapticProvider.ResolveIntent(signal),
                Is.EqualTo(expected),
                cue + "/" + role
            );
        }

        static void AssertTarget(
            int index,
            string id,
            string label,
            int width,
            int height
        )
        {
            FidelityTarget target = FidelityVerification.GetTarget(index);
            Assert.That(target.id, Is.EqualTo(id));
            Assert.That(target.label, Is.EqualTo(label));
            Assert.That(target.width, Is.EqualTo(width));
            Assert.That(target.height, Is.EqualTo(height));
            Assert.That(target.Size, Is.EqualTo(new Vector2Int(width, height)));
        }

        static RuntimePerformanceReport BuildPerformanceReport(
            RuntimePerformanceScenario scenario,
            Func<int, double> cpuMilliseconds,
            Func<int, long> allocationBytes,
            Func<int, double> gpuMilliseconds = null
        )
        {
            var accumulator = new RuntimePerformanceAccumulator(600);
            accumulator.Begin(scenario, 0, 600, 60, true, true);
            for (int index = 0; index < 600; index++)
            {
                Assert.That(
                    accumulator.TryPushSample(
                        (ulong)(index + 1),
                        cpuMilliseconds(index),
                        2d,
                        1d,
                        0d,
                        gpuMilliseconds != null ? gpuMilliseconds(index) : 8d,
                        allocationBytes(index)
                    ),
                    Is.True
                );
            }
            Assert.That(
                accumulator.TryBuildReport(out RuntimePerformanceReport report),
                Is.True
            );
            return report;
        }

        static void PushUniformSamples(
            RuntimePerformanceAccumulator accumulator,
            int count,
            double cpuMilliseconds,
            double gpuMilliseconds,
            long allocationBytes
        )
        {
            for (int index = 0; index < count; index++)
            {
                Assert.That(
                    accumulator.TryPushSample(
                        (ulong)(index + 1),
                        cpuMilliseconds,
                        2d,
                        1d,
                        0d,
                        gpuMilliseconds,
                        allocationBytes
                    ),
                    Is.True
                );
            }
        }

        static object ResolveCoreRecipe(
            ProductionFeedbackProvider provider,
            CoreLoopFeedbackSignal signal
        )
        {
            MethodInfo method = typeof(ProductionFeedbackProvider).GetMethod(
                "ResolveCoreRecipe",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.That(method, Is.Not.Null);
            return method.Invoke(provider, new object[] { signal, (int)signal.Cue });
        }

        static object ResolveMetaRecipe(
            ProductionFeedbackProvider provider,
            MetaUiControlRole role
        )
        {
            var signal = new MetaUiFeedbackSignal(
                MetaUiMotionCue.ButtonActivated,
                role,
                MetaUiMotionPreset.ScreenStandard,
                "mapping"
            );
            MethodInfo method = typeof(ProductionFeedbackProvider).GetMethod(
                "ResolveMetaRecipe",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.That(method, Is.Not.Null);
            return method.Invoke(
                provider,
                new object[] { signal, (int)signal.Cue }
            );
        }

        static float RecipeFloat(object recipe, string propertyName)
        {
            Assert.That(recipe, Is.Not.Null);
            PropertyInfo property = recipe.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public
            );
            Assert.That(property, Is.Not.Null);
            return (float)property.GetValue(recipe);
        }

        static object PrivateRecipe(
            ProductionFeedbackProvider provider,
            string fieldName
        )
        {
            FieldInfo field = typeof(ProductionFeedbackProvider).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.That(field, Is.Not.Null);
            return field.GetValue(provider);
        }

        static void SetPrivateField(
            ProductionFeedbackProvider provider,
            string fieldName,
            object value
        )
        {
            FieldInfo field = typeof(ProductionFeedbackProvider).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.That(field, Is.Not.Null);
            field.SetValue(provider, value);
        }

        static T PrivateFieldValue<T>(
            ProductionFeedbackProvider provider,
            string fieldName
        )
        {
            FieldInfo field = typeof(ProductionFeedbackProvider).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.That(field, Is.Not.Null);
            return (T)field.GetValue(provider);
        }

        sealed class MetaSfxProbe : IMetaUiSfxSink
        {
            public int Calls { get; private set; }
            public void Play(MetaUiFeedbackSignal signal) => Calls++;
        }

        sealed class MetaHapticProbe : IMetaUiHapticSink
        {
            public int Calls { get; private set; }
            public void Pulse(MetaUiFeedbackSignal signal) => Calls++;
        }

        sealed class ThrowingMetaSfxSink : IMetaUiSfxSink
        {
            public int Calls { get; private set; }

            public void Play(MetaUiFeedbackSignal signal)
            {
                Calls++;
                throw new InvalidOperationException("sfx");
            }
        }

        sealed class ThrowingMetaHapticSink : IMetaUiHapticSink
        {
            public int Calls { get; private set; }

            public void Pulse(MetaUiFeedbackSignal signal)
            {
                Calls++;
                throw new InvalidOperationException("haptic");
            }
        }

        sealed class HapticClockProbe : IHapticClock
        {
            public double Now;
            public double RealtimeSeconds => Now;
        }

        sealed class HapticBridgeProbe : IPlatformHapticBridge
        {
            public readonly List<HapticFeedbackIntent> Performed =
                new List<HapticFeedbackIntent>();

            public bool Supported = true;
            public bool PerformResult = true;
            public int SuspendCalls;
            public int DisposeCalls;

            public bool IsSupported => Supported;

            public bool TryPerform(HapticFeedbackIntent intent)
            {
                Performed.Add(intent);
                return PerformResult;
            }

            public void Suspend() => SuspendCalls++;
            public void Dispose() => DisposeCalls++;
        }
    }
}
