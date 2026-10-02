using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class MetaUiTestTickable : MonoBehaviour, IMetaUiMotionTickable
    {
        public bool TickEnabled = true;
        public int TickCalls;
        public int CancelCalls;
        public float LastDelta;
        public Action TickAction;

        public bool IsMotionTickEnabled => TickEnabled;

        public void TickMotion(float deltaTime)
        {
            TickCalls++;
            LastDelta = deltaTime;
            TickAction?.Invoke();
        }

        public void CancelMotion()
        {
            CancelCalls++;
            TickEnabled = false;
        }
    }

    public sealed class MetaUiTransitionObserverProbe : IMetaUiTransitionObserver
    {
        public readonly List<MetaUiTransitionPhase> Phases =
            new List<MetaUiTransitionPhase>();

        public void OnMetaUiTransitionCompleted(
            MetaUiTransition transition,
            MetaUiTransitionPhase phase
        )
        {
            Phases.Add(phase);
        }
    }

    public sealed class MetaUiMotionTransitionPolishPlayModeTests
    {
        const string PersistedSaveKey = "LegendsOfRus_Save_v1";
        static readonly string[] PersistedSaveKeys =
        {
            PersistedSaveKey,
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject testRoot;
        GameObject stateObject;
        GameObject eventSystemObject;
        GameObject ownedRouterObject;
        MetaUiMotionPresenter coordinator;
        GameState state;
        bool ownsStateObject;
        bool ownsEventSystemObject;
        bool usedExistingRouter;
        bool[] hadPersistedSaves;
        string[] persistedSaveJson;
        string runtimeSaveJson;
        float originalTimeScale;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            originalTimeScale = Time.timeScale;
            Time.timeScale = 1f;

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
            {
                stateObject = new GameObject("Step14MetaUiTestGameState");
                state = stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }
            runtimeSaveJson = JsonUtility.ToJson(state.Save);

            if (EventSystem.current == null)
            {
                eventSystemObject = new GameObject(
                    "Step14MetaUiTestEventSystem",
                    typeof(EventSystem)
                );
                ownsEventSystemObject = true;
            }

            testRoot = new GameObject(
                "Step14MetaUiTestRoot",
                typeof(RectTransform),
                typeof(MetaUiMotionPresenter)
            );
            coordinator = testRoot.GetComponent<MetaUiMotionPresenter>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = originalTimeScale;

            if (ownedRouterObject != null)
            {
                UnityEngine.Object.Destroy(ownedRouterObject);
            }
            else if (usedExistingRouter && ScreenRouter.Instance != null)
            {
                ScreenRouter.Instance.Show(ScreenId.Home);
                MetaUiMotionPresenter routerClock =
                    ScreenRouter.Instance.MotionPresenter;
                Tick(routerClock, 12);
            }

            if (testRoot != null)
                UnityEngine.Object.Destroy(testRoot);

            RestoreRuntimeSave();
            if (ownsStateObject && stateObject != null)
                UnityEngine.Object.Destroy(stateObject);
            if (ownsEventSystemObject && eventSystemObject != null)
                UnityEngine.Object.Destroy(eventSystemObject);

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
        public void Profile_ResolvesBoundedDeterministicStyles()
        {
            foreach (MetaUiMotionPreset preset in Enum.GetValues(typeof(MetaUiMotionPreset)))
            {
                MetaUiTransitionStyle style = MetaUiMotionProfile.ResolveTransition(preset);
                Assert.That(style.DelaySeconds, Is.InRange(0f, 0.45f), preset.ToString());
                Assert.That(
                    style.EnterDurationSeconds,
                    Is.InRange(0.01f, MetaUiMotionProfile.MaximumTransitionSeconds),
                    preset.ToString()
                );
                Assert.That(
                    style.ExitDurationSeconds,
                    Is.InRange(0.01f, MetaUiMotionProfile.MaximumTransitionSeconds),
                    preset.ToString()
                );
                Assert.That(style.StartAlpha, Is.InRange(0f, 1f), preset.ToString());
                Assert.That(style.StartScale, Is.InRange(0.85f, 1.15f), preset.ToString());
                Assert.That(style.ExitScale, Is.InRange(0.85f, 1.15f), preset.ToString());
                Assert.That(
                    style.StartOffset.magnitude,
                    Is.LessThanOrEqualTo(MetaUiMotionProfile.MaximumOffsetPixels),
                    preset.ToString()
                );
                Assert.That(
                    style.ExitOffset.magnitude,
                    Is.LessThanOrEqualTo(MetaUiMotionProfile.MaximumOffsetPixels),
                    preset.ToString()
                );
            }

            foreach (MetaUiControlRole role in Enum.GetValues(typeof(MetaUiControlRole)))
            {
                MetaUiPressStyle style = MetaUiMotionProfile.ResolvePress(role);
                Assert.That(style.InvokeDelaySeconds, Is.InRange(0f, 0.20f), role.ToString());
                Assert.That(style.ReleaseDurationSeconds, Is.InRange(0.01f, 0.20f), role.ToString());
                Assert.That(style.PressedScale, Is.InRange(0.90f, 1.10f), role.ToString());
                Assert.That(style.ActivatedScale, Is.InRange(0.90f, 1.10f), role.ToString());
            }

            Assert.That(
                MetaUiMotionProfile.ResolveScreenPreset(ScreenId.Victory),
                Is.EqualTo(MetaUiMotionPreset.ScreenVictory)
            );
            Assert.That(
                MetaUiMotionProfile.ResolveScreenPreset(ScreenId.Fail),
                Is.EqualTo(MetaUiMotionPreset.ScreenFailure)
            );
            Assert.That(
                MetaUiMotionProfile.ResolveModalPreset(ScreenId.RewardClaim),
                Is.EqualTo(MetaUiMotionPreset.ModalReward)
            );
        }

        [Test]
        public void Profile_ClampsClockAndEasingEndpoints()
        {
            Assert.That(MetaUiMotionProfile.ClampTick(-5f), Is.EqualTo(0f));
            Assert.That(
                MetaUiMotionProfile.ClampTick(10f),
                Is.EqualTo(MetaUiMotionProfile.MaximumTickSeconds)
            );
            Assert.That(MetaUiMotionProfile.EaseOutCubic(0f), Is.EqualTo(0f));
            Assert.That(MetaUiMotionProfile.EaseOutCubic(1f), Is.EqualTo(1f));
            Assert.That(MetaUiMotionProfile.EaseInCubic(0f), Is.EqualTo(0f));
            Assert.That(MetaUiMotionProfile.EaseInCubic(1f), Is.EqualTo(1f));
            Assert.That(MetaUiMotionProfile.EaseOutBack(0f), Is.EqualTo(0f).Within(0.0001f));
            Assert.That(MetaUiMotionProfile.EaseOutBack(1f), Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void Coordinator_DuplicateRegistrationIsIdempotentAndDeltaIsClamped()
        {
            MetaUiTestTickable tickable = CreateTickable("DuplicateRegistration");

            Assert.That(coordinator.Register(tickable), Is.True);
            Assert.That(coordinator.Register(tickable), Is.True);
            Assert.That(coordinator.RegisteredCount, Is.EqualTo(1));

            coordinator.TickForTesting(10f);

            Assert.That(tickable.TickCalls, Is.EqualTo(1));
            Assert.That(
                tickable.LastDelta,
                Is.EqualTo(MetaUiMotionProfile.MaximumTickSeconds)
            );
            Assert.That(coordinator.PeakRegisteredCount, Is.EqualTo(1));
            Assert.That(coordinator.DroppedRegistrationCount, Is.Zero);

            coordinator.Unregister(tickable);
            Assert.That(coordinator.RegisteredCount, Is.Zero);
        }

        [Test]
        public void Coordinator_RegistrationDuringTickWaitsUntilNextTick()
        {
            MetaUiTestTickable first = CreateTickable("FirstTickable");
            MetaUiTestTickable late = CreateTickable("LateTickable");
            first.TickAction = () => coordinator.Register(late);
            Assert.That(coordinator.Register(first), Is.True);

            coordinator.TickForTesting(0.02f);
            Assert.That(first.TickCalls, Is.EqualTo(1));
            Assert.That(late.TickCalls, Is.Zero);
            Assert.That(coordinator.RegisteredCount, Is.EqualTo(2));

            coordinator.TickForTesting(0.02f);
            Assert.That(first.TickCalls, Is.EqualTo(2));
            Assert.That(late.TickCalls, Is.EqualTo(1));
        }

        [Test]
        public void Coordinator_ClearAllCancelsEveryRegistration()
        {
            MetaUiTestTickable first = CreateTickable("ClearFirst");
            MetaUiTestTickable second = CreateTickable("ClearSecond");
            coordinator.Register(first);
            coordinator.Register(second);

            coordinator.ClearAll();

            Assert.That(coordinator.RegisteredCount, Is.Zero);
            Assert.That(first.CancelCalls, Is.EqualTo(1));
            Assert.That(second.CancelCalls, Is.EqualTo(1));
            coordinator.TickForTesting(0.05f);
            Assert.That(first.TickCalls, Is.Zero);
            Assert.That(second.TickCalls, Is.Zero);
        }

        [Test]
        public void Coordinator_FixedCapacityDropsOverflowWithoutGrowing()
        {
            for (int i = 0; i < MetaUiMotionProfile.MaximumRegisteredMotions; i++)
            {
                MetaUiTestTickable tickable = CreateTickable("Capacity_" + i);
                Assert.That(coordinator.Register(tickable), Is.True, i.ToString());
            }

            LogAssert.Expect(
                LogType.Warning,
                "Meta UI motion registration was dropped because the fixed pool is full."
            );
            MetaUiTestTickable overflow = CreateTickable("Capacity_Overflow");
            Assert.That(coordinator.Register(overflow), Is.False);
            Assert.That(
                coordinator.RegisteredCount,
                Is.EqualTo(MetaUiMotionProfile.MaximumRegisteredMotions)
            );
            Assert.That(
                coordinator.PeakRegisteredCount,
                Is.EqualTo(MetaUiMotionProfile.MaximumRegisteredMotions)
            );
            Assert.That(coordinator.DroppedRegistrationCount, Is.EqualTo(1));
        }

        [Test]
        public void Transition_EnterExitIsDeterministicAndPreservesReferenceGeometry()
        {
            Time.timeScale = 0f;
            var observer = new MetaUiTransitionObserverProbe();
            MetaUiTransition transition = CreateTransition(
                "ModalTransition",
                MetaUiMotionPreset.ModalStandard,
                observer,
                new Vector2(17f, -23f),
                new Vector3(1.1f, 0.9f, 1f)
            );
            RectTransform rect = transition.GetComponent<RectTransform>();

            Assert.That(transition.Phase, Is.EqualTo(MetaUiTransitionPhase.Entering));
            Assert.That(transition.Group.alpha, Is.EqualTo(0f));
            Assert.That(transition.Group.blocksRaycasts, Is.False);

            Tick(coordinator, 12);

            Assert.That(transition.Phase, Is.EqualTo(MetaUiTransitionPhase.Entered));
            Assert.That(transition.Group.alpha, Is.EqualTo(1f));
            Assert.That(transition.Group.interactable, Is.True);
            Assert.That(transition.Group.blocksRaycasts, Is.True);
            Assert.That(rect.anchoredPosition, Is.EqualTo(new Vector2(17f, -23f)));
            Assert.That(rect.localScale, Is.EqualTo(new Vector3(1.1f, 0.9f, 1f)));
            Assert.That(observer.Phases, Is.EqualTo(new[] { MetaUiTransitionPhase.Entered }));

            transition.PlayExit();
            Assert.That(transition.Group.interactable, Is.False);
            Assert.That(transition.Group.blocksRaycasts, Is.False);
            Tick(coordinator, 12);

            Assert.That(transition.Phase, Is.EqualTo(MetaUiTransitionPhase.Exited));
            Assert.That(transition.Group.alpha, Is.EqualTo(0f));
            Assert.That(
                observer.Phases,
                Is.EqualTo(new[]
                {
                    MetaUiTransitionPhase.Entered,
                    MetaUiTransitionPhase.Exited
                })
            );
        }

        [Test]
        public void Transition_OutcomePresetsAreDistinctBoundedAndNonInteractive()
        {
            MetaUiTransition victory = CreateTransition(
                "VictoryReaction",
                MetaUiMotionPreset.OutcomeVictory
            );
            MetaUiTransition failure = CreateTransition(
                "FailureReaction",
                MetaUiMotionPreset.OutcomeFailure
            );
            MetaUiTransition reward = CreateTransition(
                "RewardReaction",
                MetaUiMotionPreset.OutcomeReward
            );
            MetaUiTransition chapter = CreateTransition(
                "ChapterReaction",
                MetaUiMotionPreset.ChapterComplete
            );

            Tick(coordinator, 14);

            AssertOutcome(victory, MetaUiMotionCue.VictoryReveal);
            AssertOutcome(failure, MetaUiMotionCue.FailureReveal);
            AssertOutcome(reward, MetaUiMotionCue.RewardReveal);
            AssertOutcome(chapter, MetaUiMotionCue.ChapterCompleteReveal);
        }

        [Test]
        public void Transition_CancelRestoresGeometryAndDisablesInteraction()
        {
            Vector2 basePosition = new Vector2(-11f, 29f);
            Vector3 baseScale = new Vector3(0.93f, 1.07f, 1f);
            MetaUiTransition transition = CreateTransition(
                "CancelledReward",
                MetaUiMotionPreset.OutcomeReward,
                null,
                basePosition,
                baseScale
            );
            RectTransform rect = transition.GetComponent<RectTransform>();
            transition.TickForTesting(0.15f);

            transition.CancelMotion();

            Assert.That(transition.Phase, Is.EqualTo(MetaUiTransitionPhase.Cancelled));
            Assert.That(transition.Group.alpha, Is.EqualTo(1f));
            Assert.That(transition.Group.interactable, Is.False);
            Assert.That(transition.Group.blocksRaycasts, Is.False);
            Assert.That(rect.anchoredPosition, Is.EqualTo(basePosition));
            Assert.That(rect.localScale, Is.EqualTo(baseScale));
            Assert.That(transition.LastTrace.Cue, Is.EqualTo(MetaUiMotionCue.None));
        }

        [Test]
        public void TransitionTrace_IsDeterministicForIdenticalSequence()
        {
            string first = CaptureTransitionTrace("TraceTarget");
            string second = CaptureTransitionTrace("TraceTarget");

            Assert.That(second, Is.EqualTo(first));
            Assert.That(
                first,
                Does.Contain("ModalEnter").And.Contain("ModalExit")
            );
        }

        [Test]
        public void Hotspot_PressExitRestoresBaselineWithoutInvoking()
        {
            int invocations = 0;
            MetaUiActionGate gate = CreateInteractionRoot("PressExitRoot");
            AnimatedHotspot hotspot = CreateHotspot(
                gate,
                "PressExit",
                () => invocations++,
                MetaUiControlRole.Primary
            );
            PointerEventData pointer = Pointer(17);

            hotspot.OnPointerDown(pointer);
            Assert.That(hotspot.IsPressed, Is.True);
            Assert.That(hotspot.LastTrace.Cue, Is.EqualTo(MetaUiMotionCue.ButtonPress));

            hotspot.OnPointerExit(pointer);
            Tick(coordinator, 6);

            Assert.That(invocations, Is.Zero);
            Assert.That(hotspot.IsPressed, Is.False);
            Assert.That(hotspot.IsInvokingAction, Is.False);
            Assert.That(hotspot.IsReady, Is.True);
            Assert.That(hotspot.IsHighlightRaycastSafe, Is.True);
            Assert.That(hotspot.LastTrace.Cue, Is.EqualTo(MetaUiMotionCue.ButtonRelease));
        }

        [Test]
        public void Hotspot_ClickInvokesExactlyOnceAndCanBeReused()
        {
            int invocations = 0;
            MetaUiActionGate gate = CreateInteractionRoot("ReusableRoot");
            AnimatedHotspot hotspot = CreateHotspot(
                gate,
                "Reusable",
                () => invocations++,
                MetaUiControlRole.Primary
            );
            PointerEventData pointer = Pointer(21);

            Activate(hotspot, pointer);
            Tick(coordinator, 6);

            Assert.That(invocations, Is.EqualTo(1));
            Assert.That(hotspot.ActivationCount, Is.EqualTo(1));
            Assert.That(gate.IsBusy, Is.False);
            Assert.That(hotspot.IsReady, Is.True);

            Activate(hotspot, pointer);
            Tick(coordinator, 6);

            Assert.That(invocations, Is.EqualTo(2));
            Assert.That(hotspot.ActivationCount, Is.EqualTo(2));
            Assert.That(gate.AcceptedReservations, Is.EqualTo(2));
        }

        [Test]
        public void OneShotReward_DispatchesAtMostOnceAcrossRepeatedClicks()
        {
            int invocations = 0;
            MetaUiActionGate gate = CreateInteractionRoot("OneShotRewardRoot");
            AnimatedHotspot hotspot = CreateHotspot(
                gate,
                "OneShotReward",
                () => invocations++,
                MetaUiControlRole.OneShotReward
            );
            PointerEventData pointer = Pointer(23);

            Activate(hotspot, pointer);
            Tick(coordinator, 8);
            Assert.That(invocations, Is.EqualTo(1));
            Assert.That(hotspot.ActivationCount, Is.EqualTo(1));
            Assert.That(hotspot.IsReady, Is.False);

            Activate(hotspot, pointer);
            Tick(coordinator, 8);
            Assert.That(invocations, Is.EqualTo(1));
            Assert.That(hotspot.ActivationCount, Is.EqualTo(1));
            Assert.That(gate.AcceptedReservations, Is.EqualTo(1));
        }

        [Test]
        public void ActionGate_RejectsSiblingUntilAcceptedActionDispatches()
        {
            int firstInvocations = 0;
            int secondInvocations = 0;
            MetaUiActionGate gate = CreateInteractionRoot("SiblingGateRoot");
            AnimatedHotspot first = CreateHotspot(
                gate,
                "FirstSibling",
                () => firstInvocations++,
                MetaUiControlRole.Standard
            );
            AnimatedHotspot second = CreateHotspot(
                gate,
                "SecondSibling",
                () => secondInvocations++,
                MetaUiControlRole.Standard
            );
            PointerEventData pointer = Pointer(25);

            Activate(first, pointer);
            Activate(second, pointer);

            Assert.That(gate.IsBusy, Is.True);
            Assert.That(gate.AcceptedReservations, Is.EqualTo(1));
            Assert.That(gate.RejectedReservations, Is.EqualTo(1));
            Assert.That(second.IsInvokingAction, Is.False);

            Tick(coordinator, 2);
            Assert.That(firstInvocations, Is.EqualTo(1));
            Assert.That(secondInvocations, Is.Zero);
            Assert.That(gate.IsBusy, Is.False);

            Activate(second, pointer);
            Tick(coordinator, 2);
            Assert.That(secondInvocations, Is.EqualTo(1));
        }

        [Test]
        public void Hotspot_DisableCancelsPendingActionAndReleasesGate()
        {
            int invocations = 0;
            MetaUiActionGate gate = CreateInteractionRoot("DisableRoot");
            AnimatedHotspot hotspot = CreateHotspot(
                gate,
                "DisablePending",
                () => invocations++,
                MetaUiControlRole.Reward
            );

            Activate(hotspot, Pointer(31));
            Assert.That(gate.IsBusy, Is.True);
            Assert.That(hotspot.IsInvokingAction, Is.True);

            hotspot.gameObject.SetActive(false);
            Tick(coordinator, 8);

            Assert.That(invocations, Is.Zero);
            Assert.That(gate.IsBusy, Is.False);
            Assert.That(coordinator.RegisteredCount, Is.Zero);
        }

        [Test]
        public void MapNodePulse_IsPrewarmedNonRaycastAndDeterministic()
        {
            string saveBefore = JsonUtility.ToJson(state.Save);
            int selectedChapter = state.Save.selectedChapter;
            int selectedLevel = state.Save.selectedLevel;
            int highestUnlockedLevel = state.Save.highestUnlockedLevel;
            GameObject host = new GameObject(
                "MapNodeHost",
                typeof(RectTransform),
                typeof(Image),
                typeof(MetaUiMapNodeFeedback)
            );
            host.transform.SetParent(testRoot.transform, false);
            MetaUiMapNodeFeedback feedback = host.GetComponent<MetaUiMapNodeFeedback>();
            feedback.Initialize(coordinator);
            int childrenAfterInitialization = host.transform.childCount;

            feedback.Pulse(null);
            Assert.That(feedback.PulseCount, Is.EqualTo(1));
            Assert.That(feedback.IsRaycastSafe, Is.True);
            Assert.That(feedback.LastTrace.Cue, Is.EqualTo(MetaUiMotionCue.MapNodePulse));
            Assert.That(
                feedback.LastTrace.Phase,
                Is.EqualTo(MetaUiTransitionPhase.Entering)
            );

            Tick(coordinator, 7);
            Assert.That(feedback.IsMotionTickEnabled, Is.False);
            Assert.That(
                feedback.LastTrace.Phase,
                Is.EqualTo(MetaUiTransitionPhase.Entered)
            );
            Assert.That(host.transform.childCount, Is.EqualTo(childrenAfterInitialization));

            feedback.Pulse(null);
            Tick(coordinator, 7);
            Assert.That(feedback.PulseCount, Is.EqualTo(2));
            Assert.That(host.transform.childCount, Is.EqualTo(childrenAfterInitialization));
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(saveBefore));
            Assert.That(state.Save.selectedChapter, Is.EqualTo(selectedChapter));
            Assert.That(state.Save.selectedLevel, Is.EqualTo(selectedLevel));
            Assert.That(state.Save.highestUnlockedLevel, Is.EqualTo(highestUnlockedLevel));
        }

        [Test]
        public void PresentationOnlySequence_DoesNotMutateSaveOrPublishStateChange()
        {
            string before = JsonUtility.ToJson(state.Save);
            string activeStoryBefore = StoryService.ActiveSceneId;
            int changed = 0;
            Action onChanged = () => changed++;
            state.Changed += onChanged;

            try
            {
                MetaUiTransition transition = CreateTransition(
                    "IsolationReward",
                    MetaUiMotionPreset.OutcomeReward
                );
                MetaUiActionGate gate = CreateInteractionRoot("IsolationGate");
                AnimatedHotspot hotspot = CreateHotspot(
                    gate,
                    "IsolationHotspot",
                    () => { },
                    MetaUiControlRole.Reward
                );
                GameObject mapHost = new GameObject(
                    "IsolationMapNode",
                    typeof(RectTransform),
                    typeof(Image),
                    typeof(MetaUiMapNodeFeedback)
                );
                mapHost.transform.SetParent(testRoot.transform, false);
                MetaUiMapNodeFeedback map = mapHost.GetComponent<MetaUiMapNodeFeedback>();
                map.Initialize(coordinator);

                Activate(hotspot, Pointer(37));
                map.Pulse(null);
                Tick(coordinator, 14);
                transition.PlayExit();
                Tick(coordinator, 10);
            }
            finally
            {
                state.Changed -= onChanged;
            }

            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(activeStoryBefore));
            Assert.That(changed, Is.Zero);
        }

        [UnityTest]
        public IEnumerator PublicRouterFlow_WiresMapMotionAndAtomicModalBlocking()
        {
            state.Save = new PlayerSaveData
            {
                lastEnergyUtcTicks = DateTime.UtcNow.Ticks
            };
            string saveBefore = JsonUtility.ToJson(state.Save);
            ScreenRouter router = ScreenRouter.Instance;
            if (router == null)
            {
                ownedRouterObject = new GameObject(
                    "Step14MetaUiTestRouter",
                    typeof(ScreenRouter)
                );
                router = ownedRouterObject.GetComponent<ScreenRouter>();
                router.Initialize();
            }
            else
            {
                usedExistingRouter = true;
            }

            MetaUiMotionPresenter routerClock = router.MotionPresenter;
            Assert.That(routerClock, Is.Not.Null);

            router.Show(ScreenId.WorldMap);
            Tick(routerClock, 12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.WorldMap));

            GameObject worldMap = GameObject.Find("Screen_WorldMap");
            Assert.That(worldMap, Is.Not.Null);
            AnimatedHotspot chapter = FindHotspot(
                worldMap,
                "Hotspot_Chapter1"
            );
            Assert.That(chapter, Is.Not.Null);
            Assert.That(chapter.Role, Is.EqualTo(MetaUiControlRole.MapNode));
            MetaUiMapNodeFeedback mapFeedback =
                chapter.GetComponent<MetaUiMapNodeFeedback>();
            Assert.That(mapFeedback, Is.Not.Null);

            Activate(chapter, Pointer(39));
            Assert.That(mapFeedback.PulseCount, Is.EqualTo(1));
            Assert.That(mapFeedback.IsRaycastSafe, Is.True);
            Tick(routerClock, 2);
            Assert.That(
                router.CurrentScreen,
                Is.EqualTo(ScreenId.WorldMap),
                "The map pulse must become visible before navigation dispatch."
            );
            Tick(routerClock, 2);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap));

            router.Show(ScreenId.PreLevel);
            Tick(routerClock, 12);

            GameObject preLevel = GameObject.Find("Screen_PreLevel");
            Assert.That(preLevel, Is.Not.Null);
            CanvasGroup screenGroup = preLevel.GetComponent<CanvasGroup>();
            Assert.That(screenGroup.alpha, Is.EqualTo(1f));
            Assert.That(screenGroup.blocksRaycasts, Is.True);

            AnimatedHotspot boosters = FindHotspot(preLevel, "Hotspot_Boosters");
            Assert.That(boosters, Is.Not.Null);
            Activate(boosters, Pointer(41));
            boosters.TickForTesting(0.05f);

            GameObject modal = GameObject.Find("Modal_BoosterSelect");
            Assert.That(modal, Is.Not.Null);
            Assert.That(router.ActiveModal, Is.EqualTo(ScreenId.BoosterSelect));
            Assert.That(router.IsModalTransitioning, Is.True);
            Assert.That(router.HasActiveModalBlocker, Is.True);
            Image blocker = modal.GetComponent<Image>();
            Assert.That(blocker, Is.Not.Null);
            Assert.That(blocker.raycastTarget, Is.True);

            Transform visual = modal.transform.Find("ModalVisual_BoosterSelect");
            Assert.That(visual, Is.Not.Null);
            CanvasGroup visualGroup = visual.GetComponent<CanvasGroup>();
            Tick(routerClock, 12);
            Assert.That(visualGroup.alpha, Is.EqualTo(1f));
            Assert.That(visualGroup.blocksRaycasts, Is.True);
            Assert.That(router.IsModalTransitioning, Is.False);

            AnimatedHotspot close = FindHotspot(
                visual.gameObject,
                "Hotspot_Close"
            );
            Assert.That(close, Is.Not.Null);
            Activate(close, Pointer(43));
            close.TickForTesting(0.05f);

            Assert.That(modal.activeSelf, Is.True);
            Assert.That(blocker.raycastTarget, Is.True);
            Assert.That(visualGroup.blocksRaycasts, Is.False);
            Assert.That(router.IsModalTransitioning, Is.True);
            Assert.That(router.HasActiveModalBlocker, Is.True);

            Tick(routerClock, 8);
            Assert.That(modal.activeSelf, Is.False);
            Assert.That(router.ActiveModal, Is.Null);
            Assert.That(router.HasActiveModalBlocker, Is.False);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(saveBefore));

            yield return null;
        }

        MetaUiTestTickable CreateTickable(string name)
        {
            GameObject go = new GameObject(name, typeof(MetaUiTestTickable));
            go.transform.SetParent(testRoot.transform, false);
            return go.GetComponent<MetaUiTestTickable>();
        }

        MetaUiTransition CreateTransition(
            string name,
            MetaUiMotionPreset preset,
            IMetaUiTransitionObserver observer = null,
            Vector2? position = null,
            Vector3? scale = null
        )
        {
            GameObject go = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(MetaUiTransition)
            );
            go.transform.SetParent(testRoot.transform, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = position ?? Vector2.zero;
            rect.localScale = scale ?? Vector3.one;
            MetaUiTransition transition = go.GetComponent<MetaUiTransition>();
            transition.Initialize(coordinator, preset, observer);
            return transition;
        }

        MetaUiActionGate CreateInteractionRoot(string name)
        {
            GameObject go = new GameObject(
                name,
                typeof(RectTransform),
                typeof(MetaUiActionGate)
            );
            go.transform.SetParent(testRoot.transform, false);
            return go.GetComponent<MetaUiActionGate>();
        }

        AnimatedHotspot CreateHotspot(
            MetaUiActionGate gate,
            string name,
            Action callback,
            MetaUiControlRole role
        )
        {
            GameObject go = new GameObject(
                name,
                typeof(RectTransform),
                typeof(Image),
                typeof(AnimatedHotspot)
            );
            go.transform.SetParent(gate.transform, false);
            AnimatedHotspot hotspot = go.GetComponent<AnimatedHotspot>();
            hotspot.Initialize(callback, role, coordinator, gate);
            return hotspot;
        }

        PointerEventData Pointer(int pointerId)
        {
            return new PointerEventData(EventSystem.current)
            {
                pointerId = pointerId,
                position = new Vector2(120f, 240f)
            };
        }

        static void Activate(AnimatedHotspot hotspot, PointerEventData pointer)
        {
            hotspot.OnPointerDown(pointer);
            hotspot.OnPointerUp(pointer);
            hotspot.OnPointerClick(pointer);
        }

        static void Tick(MetaUiMotionPresenter clock, int count)
        {
            Assert.That(clock, Is.Not.Null);
            for (int i = 0; i < count; i++)
                clock.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
        }

        static void AssertOutcome(
            MetaUiTransition transition,
            MetaUiMotionCue expectedCue
        )
        {
            Assert.That(transition.Phase, Is.EqualTo(MetaUiTransitionPhase.Entered));
            Assert.That(transition.Group.alpha, Is.EqualTo(1f));
            Assert.That(transition.Group.interactable, Is.False);
            Assert.That(transition.Group.blocksRaycasts, Is.False);
            Assert.That(transition.LastTrace.Cue, Is.EqualTo(expectedCue));
            Assert.That(
                transition.LastTrace.Phase,
                Is.EqualTo(MetaUiTransitionPhase.Entered)
            );
        }

        string CaptureTransitionTrace(string targetName)
        {
            var traces = new List<string>();
            GameObject go = new GameObject(
                targetName,
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(MetaUiTransition)
            );
            go.transform.SetParent(testRoot.transform, false);
            MetaUiTransition transition = go.GetComponent<MetaUiTransition>();
            transition.MotionTracePublished += trace => traces.Add(trace.StableKey);
            transition.Initialize(coordinator, MetaUiMotionPreset.ModalStandard);
            for (int i = 0; i < 12; i++)
                transition.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            transition.PlayExit();
            for (int i = 0; i < 12; i++)
                transition.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);

            go.SetActive(false);
            UnityEngine.Object.Destroy(go);
            return string.Join("|", traces);
        }

        static AnimatedHotspot FindHotspot(GameObject root, string name)
        {
            AnimatedHotspot[] hotspots =
                root.GetComponentsInChildren<AnimatedHotspot>(true);
            for (int i = 0; i < hotspots.Length; i++)
                if (hotspots[i].gameObject.name == name) return hotspots[i];
            return null;
        }

        void RestoreRuntimeSave()
        {
            if (state == null || string.IsNullOrEmpty(runtimeSaveJson)) return;
            state.Save = JsonUtility.FromJson<PlayerSaveData>(runtimeSaveJson);
        }
    }
}
