using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionEventDetailLayout:MonoBehaviour
    {
        public ProductionEventDetailView View;readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var whole=(RectTransform)canvas.rootCanvas.transform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?(RectTransform)fitter.transform:whole;applying=true;
            try{
                var content=View.Content;safe.GetWorldCorners(corners);var low=content.InverseTransformPoint(corners[0]);var high=content.InverseTransformPoint(corners[2]);
                float scale=Mathf.Max(.1f,Mathf.Min((high.x-low.x)/941,(high.y-low.y)/1672));View.Body.localScale=Vector3.one*scale;
                View.Body.anchoredPosition=new Vector2((low.x+high.x-941*scale)*.5f-content.rect.xMin,high.y-content.rect.yMax);
                safe.GetWorldCorners(corners);
                var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);
                View.Hud.anchoredPosition=new Vector2(0,safeHigh.y-View.Body.rect.yMax);
                float bottom=safeLow.y-View.Body.rect.yMin;
                ((RectTransform)View.BackButton.transform).anchoredPosition=new Vector2(40,-1503+bottom);
                ((RectTransform)View.ActionButton.transform).anchoredPosition=new Vector2(211,-1495+bottom);
                View.Footer.rectTransform.anchoredPosition=new Vector2(292,-1530+bottom);
                ((RectTransform)View.Body.Find("CarvedFooter")).anchoredPosition=new Vector2(0,-1498+bottom);
                ((RectTransform)View.Body.Find("RewardRim")).anchoredPosition=new Vector2(737,-1500+bottom);
                ((RectTransform)View.Body.Find("RewardPreview")).anchoredPosition=new Vector2(758,-1495+bottom);
                ((RectTransform)View.Body.Find("RewardPreviewLabel")).anchoredPosition=new Vector2(795,-1517+bottom);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
