using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Routes native Home Button feedback through the established audio/haptic service.</summary>
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public sealed class ProductionHomeButtonFeedback : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public MetaUiControlRole Role = MetaUiControlRole.Standard;
        public MetaUiMotionTrace LastTrace { get; private set; }

        const int NoPointer = int.MinValue;
        Button button;
        int pointerId = NoPointer;
        int traceSequence;

        void Awake() { button = GetComponent<Button>(); }

        bool CanInteract()
        {
            if (button == null) button = GetComponent<Button>();
            return isActiveAndEnabled && button != null &&
                button.isActiveAndEnabled && button.IsInteractable();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanInteract() || eventData == null ||
                eventData.button != PointerEventData.InputButton.Left || pointerId != NoPointer) return;
            pointerId = eventData.pointerId;
            Publish(MetaUiMotionCue.ButtonPress, MetaUiTransitionPhase.Entering);
        }

        public void OnPointerUp(PointerEventData eventData) { ReleasePointer(eventData); }
        public void OnPointerExit(PointerEventData eventData) { ReleasePointer(eventData); }

        void ReleasePointer(PointerEventData eventData)
        {
            if (eventData == null || pointerId == NoPointer || eventData.pointerId != pointerId) return;
            pointerId = NoPointer;
            Publish(MetaUiMotionCue.ButtonRelease, MetaUiTransitionPhase.Entered);
        }

        /// <summary>
        /// Called only after ProductionHomeView accepts its once-only navigation action.
        /// The adapter has no click listener, action callback or delayed dispatch of its own.
        /// </summary>
        public void NotifyActivated()
        {
            if (!CanInteract()) return;
            pointerId = NoPointer;
            Publish(MetaUiMotionCue.ButtonActivated, MetaUiTransitionPhase.Entering);
            // The service filters completion traces, emitting one physical cue for this action.
            Publish(MetaUiMotionCue.ButtonActivated, MetaUiTransitionPhase.Entered);
        }

        void Publish(MetaUiMotionCue cue, MetaUiTransitionPhase phase)
        {
            LastTrace = new MetaUiMotionTrace(++traceSequence, cue, phase,
                MetaUiMotionPreset.ScreenStandard, gameObject.name);
            MetaUiFeedbackService.Publish(LastTrace, Role);
        }

        void OnDisable() { pointerId = NoPointer; }
    }
}
