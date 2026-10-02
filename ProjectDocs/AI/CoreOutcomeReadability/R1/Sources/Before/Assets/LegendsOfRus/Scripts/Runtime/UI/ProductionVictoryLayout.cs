using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionVictoryLayout : MonoBehaviour
    {
        public ProductionVictoryView View;
        readonly Vector3[] corners=new Vector3[4];
        bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;
            var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var canvasRect=canvas.rootCanvas.transform as RectTransform;
            var safe=GetComponentInParent<SafeAreaFitter>();var safeRect=safe!=null?safe.transform as RectTransform:canvasRect;
            applying=true;
            try
            {
                var content=View.Content;safeRect.GetWorldCorners(corners);
                Vector3 low=content.InverseTransformPoint(corners[0]),high=content.InverseTransformPoint(corners[2]);
                float top=high.y-content.rect.yMax;
                View.Hud.anchoredPosition=new Vector2(0,top);
                float safeBottom=low.y-content.rect.yMax;
                ProductionVictoryReferenceStyle.Layout(View,top,safeBottom);
                canvasRect.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);
                Vector2 native=View.Environment.sprite.rect.size;float cover=Mathf.Max((high.x-low.x)/native.x,(high.y-low.y)/native.y);Vector2 size=native*cover;
                View.Environment.rectTransform.anchoredPosition=new Vector2((low.x+high.x-size.x)*.5f-content.rect.xMin,(low.y+high.y+size.y)*.5f-content.rect.yMax);View.Environment.rectTransform.sizeDelta=size;
            }
            finally{applying=false;}
        }
    }
}
