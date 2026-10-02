using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionRewardReceivedLayout:MonoBehaviour
    {
        public ProductionRewardReceivedView View;
        readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;
            var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var whole=(RectTransform)canvas.rootCanvas.transform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?(RectTransform)fitter.transform:whole;
            applying=true;
            try
            {
                var content=View.Content;safe.GetWorldCorners(corners);var low=content.InverseTransformPoint(corners[0]);var high=content.InverseTransformPoint(corners[2]);
                float scale=Mathf.Min(1,(high.x-low.x-24)/795,(high.y-low.y-48)/982);scale=Mathf.Max(.1f,scale);
                View.Body.localScale=Vector3.one*scale;
                float lowerOffset=Mathf.Min(62f*scale,Mathf.Max(0,(high.y-low.y-982*scale)*.5f-24));
                View.Body.anchoredPosition=new Vector2((low.x+high.x-795*scale)*.5f-content.rect.xMin,(low.y+high.y+982*scale)*.5f-content.rect.yMax-lowerOffset);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);
                View.Shade.rectTransform.anchoredPosition=new Vector2(low.x-content.rect.xMin,high.y-content.rect.yMax);
                View.Shade.rectTransform.sizeDelta=new Vector2(high.x-low.x,high.y-low.y);
            }
            finally{applying=false;}
        }
    }
}
