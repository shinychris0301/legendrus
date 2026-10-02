using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Visible depression without moving the button's hit area or source-art geometry.</summary>
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public sealed class ProductionMapButtonFeedback : MonoBehaviour, IPointerDownHandler,
        IPointerUpHandler, IPointerExitHandler
    {
        const int None = int.MinValue;
        Button button;
        Graphic[] graphics;
        Color[] colors;
        int pointer = None;
        float strength;
        static readonly Color Pressed = new Color(.62f,.69f,.78f,1);
        public bool IsPressed => pointer != None;

        public static void Install(Transform root)
        {
            foreach(var b in root.GetComponentsInChildren<Button>(true))
            {
                b.transition=Selectable.Transition.None;
                if(!b.GetComponent<ProductionHomeButtonFeedback>())b.gameObject.AddComponent<ProductionHomeButtonFeedback>();
                if(!b.GetComponent<ProductionMapButtonFeedback>())b.gameObject.AddComponent<ProductionMapButtonFeedback>();
            }
        }
        public void OnPointerDown(PointerEventData e)
        {
            if(!button)button=GetComponent<Button>();
            if(e==null || e.button!=PointerEventData.InputButton.Left || pointer!=None ||
                !isActiveAndEnabled || !button.IsActive() || !button.IsInteractable())return;
            Restore();
            graphics=GetComponentsInChildren<Graphic>(true);colors=new Color[graphics.Length];
            for(int i=0;i<graphics.Length;i++)colors[i]=graphics[i].canvasRenderer.GetColor();
            pointer=e.pointerId;strength=1;Paint();
        }
        public void OnPointerUp(PointerEventData e){Release(e);}
        public void OnPointerExit(PointerEventData e){Release(e);}
        void Release(PointerEventData e){if(e!=null && e.pointerId==pointer)pointer=None;}
        void LateUpdate()
        {
            if(graphics==null)return;
            var sc=GetComponentInParent<ProductionChapterMapScroller>();
            if(!button.IsInteractable() || (sc && sc.IsDragging))pointer=None;
            if(pointer==None)strength=Mathf.MoveTowards(strength,0,Time.unscaledDeltaTime/0.12f);
            Paint();if(strength<=0)Restore();
        }
        void Paint()
        {
            Color tint=Color.Lerp(Color.white,Pressed,strength);
            for(int i=0;i<graphics.Length;i++)if(graphics[i])graphics[i].canvasRenderer.SetColor(colors[i]*tint);
        }
        void Restore()
        {
            if(graphics!=null)for(int i=0;i<graphics.Length;i++)if(graphics[i])graphics[i].canvasRenderer.SetColor(colors[i]);
            graphics=null;colors=null;strength=0;pointer=None;
        }
        void OnDisable(){Restore();}
    }
}
