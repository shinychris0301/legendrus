using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Allocation-free press/tap feedback for interaction rectangles placed over
    /// approved baked artwork. Meta controls animate a prewarmed non-raycast
    /// highlight; gameplay controls retain their established invisible overlay.
    /// </summary>
    public sealed class AnimatedHotspot :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler,
        IPointerClickHandler,
        IMetaUiMotionTickable
    {
        enum HotspotPhase
        {
            Idle,
            Pressed,
            Invoking,
            Releasing
        }

        static readonly Color NormalColor = new Color(1f, 0.82f, 0.35f, 0.001f);
        const int NoPointer = int.MinValue;

        RectTransform rectTransform;
        Image hitImage;
        RectTransform highlightRect;
        Image highlightImage;
        CanvasGroup highlightGroup;
        MetaUiMapNodeFeedback mapFeedback;
        MetaUiMotionPresenter coordinator;
        MetaUiActionGate actionGate;
        MetaUiPressStyle style;
        MetaUiControlRole role;
        Action action;
        HotspotPhase phase;
        float elapsed;
        float releaseStartScale;
        float releaseStartAlpha;
        Color releaseStartColor;
        int pointerId = NoPointer;
        int clickEligiblePointerId = NoPointer;
        int activationCount;
        int traceSequence;
        bool initialized;
        bool clockRegistrationRejected;

        public event Action<MetaUiMotionTrace> MotionTracePublished;

        public bool IsInvokingAction => phase == HotspotPhase.Invoking;
        public bool IsPressed => phase == HotspotPhase.Pressed;
        public bool IsReady => initialized && action != null && !IsInvokingAction &&
            (role != MetaUiControlRole.OneShotReward || activationCount == 0);
        public bool IsMotionTickEnabled =>
            phase == HotspotPhase.Invoking || phase == HotspotPhase.Releasing;
        public int ActivationCount => activationCount;
        public MetaUiControlRole Role => role;
        public MetaUiMotionTrace LastTrace { get; private set; }
        public bool IsHighlightRaycastSafe =>
            highlightImage == null ||
            (!highlightImage.raycastTarget &&
             highlightGroup != null &&
             !highlightGroup.interactable &&
             !highlightGroup.blocksRaycasts);

        public void Initialize(Action callback)
        {
            Initialize(
                callback,
                MetaUiControlRole.Standard,
                GetComponentInParent<MetaUiMotionPresenter>(),
                GetComponentInParent<MetaUiActionGate>()
            );
        }

        public void Initialize(
            Action callback,
            MetaUiControlRole controlRole,
            MetaUiMotionPresenter motionCoordinator,
            MetaUiActionGate gate
        )
        {
            action = callback;
            role = controlRole;
            style = MetaUiMotionProfile.ResolvePress(role);
            coordinator = motionCoordinator;
            actionGate = gate;
            rectTransform = GetComponent<RectTransform>();
            hitImage = GetComponent<Image>();

            if (rectTransform == null || hitImage == null)
                throw new InvalidOperationException(
                    "AnimatedHotspot requires RectTransform and Image components."
                );

            hitImage.raycastTarget = true;
            hitImage.color = NormalColor;

            if (style.ShowsHighlight && highlightRect == null)
                BuildHighlight();

            mapFeedback = GetComponent<MetaUiMapNodeFeedback>();
            initialized = true;
            RestoreImmediate();
            clockRegistrationRejected = coordinator == null ||
                !coordinator.Register(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsReady || pointerId != NoPointer ||
                clickEligiblePointerId != NoPointer)
                return;

            pointerId = eventData != null ? eventData.pointerId : 0;
            phase = HotspotPhase.Pressed;
            elapsed = 0f;
            ApplyPressedVisual(style.PressedScale, style.PressedColor);
            Publish(MetaUiMotionCue.ButtonPress, MetaUiTransitionPhase.Entering);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!OwnsPointer(eventData)) return;
            clickEligiblePointerId = pointerId;
            pointerId = NoPointer;
            if (phase == HotspotPhase.Pressed)
                BeginRelease();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!OwnsPointer(eventData)) return;
            pointerId = NoPointer;
            clickEligiblePointerId = NoPointer;
            if (phase == HotspotPhase.Pressed)
                BeginRelease();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsReady) return;
            int clickPointerId = eventData != null
                ? eventData.pointerId
                : clickEligiblePointerId;
            if (clickEligiblePointerId == NoPointer ||
                clickPointerId != clickEligiblePointerId)
                return;
            if (actionGate != null && !actionGate.TryAcquire(this))
            {
                clickEligiblePointerId = NoPointer;
                return;
            }

            pointerId = NoPointer;
            clickEligiblePointerId = NoPointer;
            phase = HotspotPhase.Invoking;
            elapsed = 0f;
            ApplyPressedVisual(style.ActivatedScale, style.ActivatedColor);
            mapFeedback?.Pulse(eventData);
            Publish(MetaUiMotionCue.ButtonActivated, MetaUiTransitionPhase.Entering);

            if (clockRegistrationRejected)
            {
                DispatchExactlyOnce();
                if (this != null && isActiveAndEnabled)
                {
                    phase = HotspotPhase.Idle;
                    RestoreImmediate();
                }
            }
        }

        public void TickMotion(float deltaTime)
        {
            float dt = MetaUiMotionProfile.ClampTick(deltaTime);
            if (phase == HotspotPhase.Invoking)
            {
                elapsed += dt;
                if (elapsed >= style.InvokeDelaySeconds)
                    DispatchExactlyOnce();
                return;
            }

            if (phase != HotspotPhase.Releasing) return;

            elapsed += dt;
            float duration = Mathf.Max(0.01f, style.ReleaseDurationSeconds);
            float progress = Mathf.Clamp01(elapsed / duration);
            float eased = MetaUiMotionProfile.EaseOutCubic(progress);

            if (highlightRect != null)
            {
                float scale = Mathf.Lerp(releaseStartScale, 1f, eased);
                highlightRect.localScale = new Vector3(scale, scale, 1f);
                highlightGroup.alpha = Mathf.Lerp(releaseStartAlpha, 0f, eased);
            }
            else if (role == MetaUiControlRole.Gameplay && rectTransform != null)
            {
                float scale = Mathf.Lerp(releaseStartScale, 1f, eased);
                rectTransform.localScale = new Vector3(scale, scale, 1f);
                hitImage.color = Color.Lerp(releaseStartColor, NormalColor, eased);
            }
            else if (hitImage != null)
            {
                hitImage.color = Color.Lerp(releaseStartColor, NormalColor, eased);
            }

            if (progress >= 1f)
            {
                phase = HotspotPhase.Idle;
                clickEligiblePointerId = NoPointer;
                RestoreImmediate();
                Publish(MetaUiMotionCue.ButtonRelease, MetaUiTransitionPhase.Entered);
            }
        }

        public void TickForTesting(float deltaTime)
        {
            TickMotion(deltaTime);
        }

        public void CancelMotion()
        {
            actionGate?.Release(this);
            pointerId = NoPointer;
            clickEligiblePointerId = NoPointer;
            phase = HotspotPhase.Idle;
            elapsed = 0f;
            RestoreImmediate();
        }

        void DispatchExactlyOnce()
        {
            Action callback = action;
            activationCount++;

            // Clear the latch and root reservation before user code runs. This
            // fixes the inherited one-use control defect and remains safe when
            // the callback retires this root or throws an exception.
            actionGate?.Release(this);
            pointerId = NoPointer;
            BeginRelease();
            Publish(MetaUiMotionCue.ButtonActivated, MetaUiTransitionPhase.Entered);
            if (callback == null) return;
            try
            {
                callback.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        void BeginRelease()
        {
            releaseStartScale = highlightRect != null
                ? highlightRect.localScale.x
                : rectTransform != null ? rectTransform.localScale.x : 1f;
            releaseStartAlpha = highlightGroup != null ? highlightGroup.alpha : 0f;
            releaseStartColor = hitImage != null ? hitImage.color : NormalColor;
            elapsed = 0f;
            phase = HotspotPhase.Releasing;
        }

        void ApplyPressedVisual(float scale, Color color)
        {
            if (highlightRect != null)
            {
                highlightRect.localScale = new Vector3(scale, scale, 1f);
                highlightImage.color = new Color(color.r, color.g, color.b, 1f);
                highlightGroup.alpha = color.a;
            }
            else if (role == MetaUiControlRole.Gameplay && rectTransform != null)
            {
                rectTransform.localScale = new Vector3(scale, scale, 1f);
                hitImage.color = color;
            }
            else if (role == MetaUiControlRole.MapNode)
            {
                hitImage.color = NormalColor;
            }
            else if (hitImage != null)
            {
                hitImage.color = color;
            }
        }

        void RestoreImmediate()
        {
            if (rectTransform != null)
                rectTransform.localScale = Vector3.one;
            if (hitImage != null)
                hitImage.color = NormalColor;
            if (highlightRect != null)
                highlightRect.localScale = Vector3.one;
            if (highlightGroup != null)
                highlightGroup.alpha = 0f;
        }

        bool OwnsPointer(PointerEventData eventData)
        {
            if (pointerId == NoPointer) return false;
            return eventData == null || eventData.pointerId == pointerId;
        }

        void BuildHighlight()
        {
            var highlight = new GameObject(
                "PressHighlight",
                typeof(RectTransform),
                typeof(Image),
                typeof(CanvasGroup)
            );
            highlight.transform.SetParent(transform, false);
            highlightRect = highlight.GetComponent<RectTransform>();
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;
            highlightRect.offsetMin = new Vector2(5f, 5f);
            highlightRect.offsetMax = new Vector2(-5f, -5f);
            highlightRect.pivot = new Vector2(0.5f, 0.5f);

            highlightImage = highlight.GetComponent<Image>();
            highlightImage.color = Color.white;
            highlightImage.raycastTarget = false;

            highlightGroup = highlight.GetComponent<CanvasGroup>();
            highlightGroup.alpha = 0f;
            highlightGroup.interactable = false;
            highlightGroup.blocksRaycasts = false;
        }

        void Publish(MetaUiMotionCue cue, MetaUiTransitionPhase tracePhase)
        {
            LastTrace = new MetaUiMotionTrace(
                ++traceSequence,
                cue,
                tracePhase,
                MetaUiMotionPreset.ScreenStandard,
                gameObject.name
            );

            MetaUiFeedbackService.Publish(LastTrace, role);

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
}
