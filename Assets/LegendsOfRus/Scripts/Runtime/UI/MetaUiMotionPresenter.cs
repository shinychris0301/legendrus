using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public interface IMetaUiMotionTickable
    {
        bool IsMotionTickEnabled { get; }
        void TickMotion(float deltaTime);
        void CancelMotion();
    }

    public interface IMetaUiTransitionObserver
    {
        void OnMetaUiTransitionCompleted(
            MetaUiTransition transition,
            MetaUiTransitionPhase phase
        );
    }

    /// <summary>
    /// One bounded, allocation-free runtime clock for all meta UI motion.
    /// Registrations use fixed slots so callbacks may safely disable old roots
    /// and register a replacement while a tick is in progress.
    /// </summary>
    public sealed class MetaUiMotionPresenter : MonoBehaviour
    {
        readonly IMetaUiMotionTickable[] tickables =
            new IMetaUiMotionTickable[MetaUiMotionProfile.MaximumRegisteredMotions];
        readonly int[] eligibleTick =
            new int[MetaUiMotionProfile.MaximumRegisteredMotions];

        int registeredCount;
        int peakRegisteredCount;
        int droppedRegistrationCount;
        int tickCount;
        bool isTicking;

        public int RegisteredCount => registeredCount;
        public int PeakRegisteredCount => peakRegisteredCount;
        public int DroppedRegistrationCount => droppedRegistrationCount;
        public int TickCount => tickCount;

        public MetaUiMotionDiagnostics Diagnostics => new MetaUiMotionDiagnostics(
            registeredCount,
            peakRegisteredCount,
            droppedRegistrationCount,
            tickCount
        );

        public bool Register(IMetaUiMotionTickable tickable)
        {
            if (tickable == null) return false;

            for (int i = 0; i < tickables.Length; i++)
            {
                if (ReferenceEquals(tickables[i], tickable))
                    return true;
            }

            for (int i = 0; i < tickables.Length; i++)
            {
                if (!IsVacant(i)) continue;

                tickables[i] = tickable;
                eligibleTick[i] = tickCount + (isTicking ? 1 : 0);
                registeredCount++;
                peakRegisteredCount = Mathf.Max(peakRegisteredCount, registeredCount);
                return true;
            }

            droppedRegistrationCount++;
            Debug.LogWarning(
                "Meta UI motion registration was dropped because the fixed pool is full."
            );
            return false;
        }

        public void Unregister(IMetaUiMotionTickable tickable)
        {
            if (tickable == null) return;

            for (int i = 0; i < tickables.Length; i++)
            {
                if (!ReferenceEquals(tickables[i], tickable)) continue;
                ClearSlot(i);
                return;
            }
        }

        void Update()
        {
            TickForTesting(Time.unscaledDeltaTime);
        }

        public void TickForTesting(float deltaTime)
        {
            float clampedDelta = MetaUiMotionProfile.ClampTick(deltaTime);
            tickCount++;
            isTicking = true;

            try
            {
                for (int i = 0; i < tickables.Length; i++)
                {
                    IMetaUiMotionTickable tickable = tickables[i];
                    if (tickable == null) continue;

                    UnityEngine.Object unityObject = tickable as UnityEngine.Object;
                    if (unityObject == null)
                    {
                        ClearSlot(i);
                        continue;
                    }

                    if (eligibleTick[i] > tickCount || !tickable.IsMotionTickEnabled)
                        continue;

                    try
                    {
                        tickable.TickMotion(clampedDelta);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception, unityObject);
                        bool cleanupSucceeded = false;
                        try
                        {
                            tickable.CancelMotion();
                            cleanupSucceeded = true;
                        }
                        catch (Exception cleanupException)
                        {
                            Debug.LogException(cleanupException, unityObject);
                        }
                        if (!cleanupSucceeded)
                            ClearSlot(i);
                    }
                }
            }
            finally
            {
                isTicking = false;
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < tickables.Length; i++)
            {
                IMetaUiMotionTickable tickable = tickables[i];
                if (tickable != null)
                {
                    UnityEngine.Object unityObject = tickable as UnityEngine.Object;
                    if (unityObject != null)
                    {
                        try
                        {
                            tickable.CancelMotion();
                        }
                        catch (Exception exception)
                        {
                            Debug.LogException(exception, unityObject);
                        }
                    }
                }
                tickables[i] = null;
                eligibleTick[i] = 0;
            }
            registeredCount = 0;
        }

        void OnDisable()
        {
            ClearAll();
        }

        bool IsVacant(int index)
        {
            IMetaUiMotionTickable existing = tickables[index];
            if (existing == null) return true;

            UnityEngine.Object unityObject = existing as UnityEngine.Object;
            if (unityObject != null) return false;

            ClearSlot(index);
            return true;
        }

        void ClearSlot(int index)
        {
            if (tickables[index] != null)
                registeredCount = Mathf.Max(0, registeredCount - 1);
            tickables[index] = null;
            eligibleTick[index] = 0;
        }
    }

    /// <summary>
    /// A per-root reservation prevents delayed sibling taps from dispatching
    /// conflicting navigation, reward, or economy actions.
    /// </summary>
    public sealed class MetaUiActionGate : MonoBehaviour
    {
        AnimatedHotspot owner;
        int acceptedReservations;
        int rejectedReservations;

        public bool IsBusy => owner != null;
        public int AcceptedReservations => acceptedReservations;
        public int RejectedReservations => rejectedReservations;

        public bool TryAcquire(AnimatedHotspot candidate)
        {
            if (candidate == null) return false;
            if (owner != null && owner != candidate)
            {
                rejectedReservations++;
                return false;
            }

            owner = candidate;
            acceptedReservations++;
            return true;
        }

        public void Release(AnimatedHotspot candidate)
        {
            if (owner == candidate)
                owner = null;
        }

        void OnDisable()
        {
            owner = null;
        }
    }

    /// <summary>
    /// Deterministic CanvasGroup/RectTransform transition. Full-screen screen
    /// and modal presets are alpha-only so opaque approved artwork never exposes
    /// its edges or drifts away from the locked reference geometry.
    /// </summary>
    public sealed class MetaUiTransition : MonoBehaviour, IMetaUiMotionTickable
    {
        RectTransform rectTransform;
        CanvasGroup canvasGroup;
        MetaUiMotionPresenter coordinator;
        IMetaUiTransitionObserver observer;
        MetaUiTransitionStyle style;
        MetaUiMotionPreset preset;
        MetaUiTransitionPhase phase;
        Vector2 basePosition;
        Vector3 baseScale;
        float elapsed;
        float exitStartAlpha;
        Vector2 exitStartPosition;
        Vector3 exitStartScale;
        int traceSequence;
        bool initialized;
        bool clockRegistrationRejected;

        public event Action<MetaUiMotionTrace> MotionTracePublished;

        public MetaUiTransitionPhase Phase => phase;
        public MetaUiMotionPreset Preset => preset;
        public CanvasGroup Group => canvasGroup;
        public MetaUiMotionTrace LastTrace { get; private set; }
        public bool IsMotionTickEnabled =>
            phase == MetaUiTransitionPhase.Entering ||
            phase == MetaUiTransitionPhase.Exiting;

        public void Initialize(
            MetaUiMotionPresenter motionCoordinator,
            MetaUiMotionPreset motionPreset,
            IMetaUiTransitionObserver transitionObserver = null,
            bool playEnter = true
        )
        {
            if (!initialized)
            {
                rectTransform = GetComponent<RectTransform>();
                if (rectTransform == null)
                    throw new InvalidOperationException(
                        "MetaUiTransition requires a RectTransform."
                    );

                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();

                basePosition = rectTransform.anchoredPosition;
                baseScale = rectTransform.localScale;
                initialized = true;
            }

            if (coordinator != motionCoordinator)
            {
                coordinator?.Unregister(this);
                coordinator = motionCoordinator;
            }
            clockRegistrationRejected = coordinator == null ||
                !coordinator.Register(this);

            observer = transitionObserver;
            preset = motionPreset;
            style = MetaUiMotionProfile.ResolveTransition(preset);

            if (clockRegistrationRejected)
                CompleteEnter();
            else if (playEnter)
                PlayEnter();
            else
                ApplyEnteredState();
        }

        public void PlayEnter()
        {
            if (!initialized) return;

            elapsed = 0f;
            phase = MetaUiTransitionPhase.Entering;
            canvasGroup.alpha = style.StartAlpha;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            ApplyScaleAndOffset(style.StartScale, style.StartOffset);
            Publish(ResolveCue(true), phase);
        }

        public void PlayExit()
        {
            if (!initialized || phase == MetaUiTransitionPhase.Exiting ||
                phase == MetaUiTransitionPhase.Exited)
                return;

            elapsed = 0f;
            exitStartAlpha = canvasGroup.alpha;
            exitStartPosition = rectTransform.anchoredPosition;
            exitStartScale = rectTransform.localScale;
            phase = MetaUiTransitionPhase.Exiting;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Publish(ResolveCue(false), phase);

            if (clockRegistrationRejected)
                CompleteExit();
        }

        public void TickMotion(float deltaTime)
        {
            if (!IsMotionTickEnabled) return;

            float dt = MetaUiMotionProfile.ClampTick(deltaTime);
            elapsed += dt;

            if (phase == MetaUiTransitionPhase.Entering)
                TickEnter();
            else if (phase == MetaUiTransitionPhase.Exiting)
                TickExit();
        }

        public void TickForTesting(float deltaTime)
        {
            TickMotion(deltaTime);
        }

        public void CancelMotion()
        {
            if (!initialized) return;

            phase = MetaUiTransitionPhase.Cancelled;
            elapsed = 0f;
            rectTransform.anchoredPosition = basePosition;
            rectTransform.localScale = baseScale;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Publish(MetaUiMotionCue.None, phase);
        }

        void TickEnter()
        {
            if (elapsed < style.DelaySeconds) return;

            float duration = Mathf.Max(0.01f, style.EnterDurationSeconds);
            float progress = Mathf.Clamp01((elapsed - style.DelaySeconds) / duration);
            float eased = MetaUiMotionProfile.EaseOutCubic(progress);
            float scaleProgress = style.OvershootScale
                ? MetaUiMotionProfile.EaseOutBack(progress)
                : eased;

            canvasGroup.alpha = Mathf.Lerp(style.StartAlpha, 1f, eased);
            float scale = Mathf.LerpUnclamped(style.StartScale, 1f, scaleProgress);
            Vector2 offset = Vector2.LerpUnclamped(style.StartOffset, Vector2.zero, eased);
            ApplyScaleAndOffset(scale, offset);

            if (progress >= 1f)
                CompleteEnter();
        }

        void TickExit()
        {
            float duration = Mathf.Max(0.01f, style.ExitDurationSeconds);
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = MetaUiMotionProfile.EaseInCubic(progress);
            canvasGroup.alpha = Mathf.Lerp(exitStartAlpha, 0f, eased);

            Vector3 targetScale = ScaleVector(baseScale, style.ExitScale);
            rectTransform.localScale = Vector3.LerpUnclamped(
                exitStartScale,
                targetScale,
                eased
            );
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(
                exitStartPosition,
                basePosition + style.ExitOffset,
                eased
            );

            if (progress >= 1f)
                CompleteExit();
        }

        void CompleteEnter()
        {
            ApplyEnteredState();
            Publish(ResolveCue(true), phase);
            NotifyObserver();
        }

        void CompleteExit()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            phase = MetaUiTransitionPhase.Exited;
            Publish(ResolveCue(false), phase);
            NotifyObserver();
        }

        void ApplyEnteredState()
        {
            elapsed = 0f;
            rectTransform.anchoredPosition = basePosition;
            rectTransform.localScale = baseScale;
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = style.EnablesInteraction;
            canvasGroup.blocksRaycasts = style.EnablesInteraction;
            phase = MetaUiTransitionPhase.Entered;
        }

        void ApplyScaleAndOffset(float scale, Vector2 offset)
        {
            rectTransform.localScale = ScaleVector(baseScale, scale);
            rectTransform.anchoredPosition = basePosition + offset;
        }

        static Vector3 ScaleVector(Vector3 source, float scale)
        {
            return new Vector3(source.x * scale, source.y * scale, source.z);
        }

        MetaUiMotionCue ResolveCue(bool entering)
        {
            switch (preset)
            {
                case MetaUiMotionPreset.ModalStandard:
                    return entering ? MetaUiMotionCue.ModalEnter : MetaUiMotionCue.ModalExit;
                case MetaUiMotionPreset.ModalReward:
                case MetaUiMotionPreset.OutcomeReward:
                    return entering ? MetaUiMotionCue.RewardReveal : MetaUiMotionCue.ModalExit;
                case MetaUiMotionPreset.ScreenVictory:
                case MetaUiMotionPreset.OutcomeVictory:
                    return MetaUiMotionCue.VictoryReveal;
                case MetaUiMotionPreset.ScreenFailure:
                case MetaUiMotionPreset.OutcomeFailure:
                    return MetaUiMotionCue.FailureReveal;
                case MetaUiMotionPreset.ScreenChapterComplete:
                case MetaUiMotionPreset.ChapterComplete:
                    return MetaUiMotionCue.ChapterCompleteReveal;
                default:
                    return entering
                        ? MetaUiMotionCue.ScreenEnter
                        : MetaUiMotionCue.ScreenExit;
            }
        }

        void Publish(MetaUiMotionCue cue, MetaUiTransitionPhase tracePhase)
        {
            LastTrace = new MetaUiMotionTrace(
                ++traceSequence,
                cue,
                tracePhase,
                preset,
                gameObject.name
            );

            MetaUiFeedbackService.Publish(
                LastTrace,
                MetaUiControlRole.Standard
            );

            Action<MetaUiMotionTrace> observers = MotionTracePublished;
            if (observers == null) return;

            Delegate[] listeners = observers.GetInvocationList();
            for (int i = 0; i < listeners.Length; i++)
            {
                try
                {
                    ((Action<MetaUiMotionTrace>)listeners[i]).Invoke(LastTrace);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }

        void NotifyObserver()
        {
            if (observer == null) return;
            try
            {
                observer.OnMetaUiTransitionCompleted(this, phase);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        void OnDisable()
        {
            CancelMotion();
            coordinator?.Unregister(this);
        }

        void OnEnable()
        {
            if (!initialized) return;
            clockRegistrationRejected = coordinator == null ||
                !coordinator.Register(this);
        }
    }

    /// <summary>
    /// One prewarmed, non-raycast pulse attached only to existing map hotspots.
    /// It follows the user's tap and never resolves or mutates map progression.
    /// </summary>
    public sealed class MetaUiMapNodeFeedback : MonoBehaviour, IMetaUiMotionTickable
    {
        const float PulseDuration = 0.16f;
        const float PulseSize = 112f;

        MetaUiMotionPresenter coordinator;
        RectTransform hostRect;
        RectTransform pulseRect;
        CanvasGroup pulseGroup;
        float elapsed;
        bool active;
        int pulseCount;
        bool clockRegistrationRejected;

        public int PulseCount => pulseCount;
        public bool IsRaycastSafe { get; private set; }
        public bool IsMotionTickEnabled => active;
        public MetaUiMotionTrace LastTrace { get; private set; }

        public void Initialize(MetaUiMotionPresenter motionCoordinator)
        {
            coordinator = motionCoordinator;
            hostRect = GetComponent<RectTransform>();

            var pulse = new GameObject(
                "MapNodeTapPulse",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Outline),
                typeof(CanvasGroup)
            );
            pulse.transform.SetParent(transform, false);
            pulseRect = pulse.GetComponent<RectTransform>();
            pulseRect.anchorMin = new Vector2(0.5f, 0.5f);
            pulseRect.anchorMax = new Vector2(0.5f, 0.5f);
            pulseRect.pivot = new Vector2(0.5f, 0.5f);
            pulseRect.sizeDelta = new Vector2(PulseSize, PulseSize);
            pulseRect.localRotation = Quaternion.Euler(0f, 0f, 45f);

            Image image = pulse.GetComponent<Image>();
            image.color = new Color(1f, 0.76f, 0.22f, 0.58f);
            image.raycastTarget = false;

            Outline outline = pulse.GetComponent<Outline>();
            outline.effectColor = new Color(1f, 0.91f, 0.55f, 0.75f);
            outline.effectDistance = new Vector2(3f, -3f);
            outline.useGraphicAlpha = true;

            pulseGroup = pulse.GetComponent<CanvasGroup>();
            pulseGroup.alpha = 0f;
            pulseGroup.interactable = false;
            pulseGroup.blocksRaycasts = false;
            pulseRect.localScale = Vector3.one * 0.45f;

            IsRaycastSafe = !image.raycastTarget &&
                !pulseGroup.interactable && !pulseGroup.blocksRaycasts;
            clockRegistrationRejected = coordinator == null ||
                !coordinator.Register(this);
        }

        public void Pulse(PointerEventData eventData)
        {
            if (pulseRect == null || hostRect == null) return;

            Vector2 localPosition = Vector2.zero;
            if (eventData != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    hostRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out localPosition
                );
            }

            pulseRect.anchoredPosition = localPosition;
            if (clockRegistrationRejected)
            {
                pulseCount++;
                LastTrace = new MetaUiMotionTrace(
                    pulseCount,
                    MetaUiMotionCue.MapNodePulse,
                    MetaUiTransitionPhase.Entered,
                    MetaUiMotionPreset.ScreenStandard,
                    gameObject.name
                );
                return;
            }

            pulseRect.localScale = Vector3.one * 0.45f;
            pulseGroup.alpha = 1f;
            elapsed = 0f;
            active = true;
            pulseCount++;
            LastTrace = new MetaUiMotionTrace(
                pulseCount,
                MetaUiMotionCue.MapNodePulse,
                MetaUiTransitionPhase.Entering,
                MetaUiMotionPreset.ScreenStandard,
                gameObject.name
            );
        }

        public void TickMotion(float deltaTime)
        {
            if (!active || pulseRect == null || pulseGroup == null) return;

            elapsed += MetaUiMotionProfile.ClampTick(deltaTime);
            float progress = Mathf.Clamp01(elapsed / PulseDuration);
            float eased = MetaUiMotionProfile.EaseOutCubic(progress);
            pulseRect.localScale = Vector3.one * Mathf.Lerp(0.45f, 1.35f, eased);
            pulseGroup.alpha = 1f - MetaUiMotionProfile.EaseInCubic(progress);

            if (progress >= 1f)
            {
                active = false;
                pulseGroup.alpha = 0f;
                LastTrace = new MetaUiMotionTrace(
                    pulseCount,
                    MetaUiMotionCue.MapNodePulse,
                    MetaUiTransitionPhase.Entered,
                    MetaUiMotionPreset.ScreenStandard,
                    gameObject.name
                );
            }
        }

        public void TickForTesting(float deltaTime)
        {
            TickMotion(deltaTime);
        }

        public void CancelMotion()
        {
            active = false;
            elapsed = 0f;
            if (pulseGroup != null) pulseGroup.alpha = 0f;
            if (pulseRect != null) pulseRect.localScale = Vector3.one * 0.45f;
        }

        void OnDisable()
        {
            CancelMotion();
            coordinator?.Unregister(this);
        }

        void OnEnable()
        {
            if (pulseRect == null) return;
            clockRegistrationRejected = coordinator == null ||
                !coordinator.Register(this);
        }
    }
}
