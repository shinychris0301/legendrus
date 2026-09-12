using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Subtle scale feedback on the real Button; no Animator or invisible screenshot hotspot.</summary>
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public sealed class SplashTitleButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Range(.9f, 1f)] public float PressedScale = .975f;
        Button button;
        Vector3 restScale;
        bool pressed;
        int pointer = int.MinValue;
        void Awake() { button = GetComponent<Button>(); restScale = transform.localScale; }
        void OnEnable() { if (restScale == Vector3.zero) restScale = transform.localScale; }
        void OnDisable() { pressed = false; pointer = int.MinValue; transform.localScale = restScale; }
        public void OnPointerDown(PointerEventData e)
        {
            if (button == null || !button.IsInteractable() || e.button != PointerEventData.InputButton.Left) return;
            if (pointer != int.MinValue) return;
            pointer = e.pointerId; pressed = true;
        }
        public void OnPointerUp(PointerEventData e)
        { if (pointer == e.pointerId) { pressed = false; pointer = int.MinValue; } }
        public void OnPointerExit(PointerEventData e)
        { if (pointer == e.pointerId) { pressed = false; pointer = int.MinValue; } }
        void Update()
        {
            if (button == null) return;
            Vector3 target = restScale * (pressed && button.IsInteractable() ? PressedScale : 1f);
            transform.localScale = Vector3.MoveTowards(transform.localScale, target, Time.unscaledDeltaTime * 1.5f);
        }
    }
}
