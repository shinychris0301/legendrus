using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionGameplayLayout : MonoBehaviour
    {
        public ProductionGameplayView View;
        readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;
            Canvas canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var canvasRect=canvas.rootCanvas.transform as RectTransform;var safe=GetComponentInParent<SafeAreaFitter>();var safeRect=safe!=null?safe.transform as RectTransform:canvasRect;
            applying=true;
            try
            {
                var content=View.Content;safeRect.GetWorldCorners(corners);
                Vector3 low=content.InverseTransformPoint(corners[0]),high=content.InverseTransformPoint(corners[2]);
                float top=high.y-content.rect.yMax,bottom=low.y-content.rect.yMin,height=high.y-low.y;
                Position(View.Hud,0,top);Position(View.Footer,0,bottom-1335);
                float boardScale=Mathf.Min(1,Mathf.Max(.1f,(height-890)/748));
                float extra=Mathf.Max(0,height-1672);
                Position(View.BoardArea,68+(808-808*boardScale)*.5f,top-486-extra*.5f);
                View.BoardArea.localScale=Vector3.one*boardScale;
                canvasRect.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);
                Vector2 native=View.Environment.sprite.rect.size;float scale=Mathf.Max((high.x-low.x)/native.x,(high.y-low.y)/native.y);Vector2 size=native*scale;
                Position(View.Environment.rectTransform,(low.x+high.x-size.x)*.5f-content.rect.xMin,(low.y+high.y+size.y)*.5f-content.rect.yMax);
                if((View.Environment.rectTransform.sizeDelta-size).sqrMagnitude>.0001f)View.Environment.rectTransform.sizeDelta=size;
            }
            finally{applying=false;}
        }
        static void Position(RectTransform r,float x,float y){var p=new Vector2(x,y);if((r.anchoredPosition-p).sqrMagnitude>.0001f)r.anchoredPosition=p;}
    }
}
