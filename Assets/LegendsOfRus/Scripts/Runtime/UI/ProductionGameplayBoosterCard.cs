using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Extends the retained booster button's hit area to its full wooden card.</summary>
    public sealed class ProductionGameplayBoosterCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public Button Target;
        public void OnPointerDown(PointerEventData e) { if(Target!=null)Target.OnPointerDown(e); }
        public void OnPointerUp(PointerEventData e) { if(Target!=null)Target.OnPointerUp(e); }
        public void OnPointerClick(PointerEventData e) { if(Target!=null)Target.OnPointerClick(e); }
    }
}
