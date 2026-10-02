using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionDailyRewardLayout:MonoBehaviour
    {
        public ProductionDailyRewardView View;readonly Vector3[] corners=new Vector3[4];bool applying;
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
                View.Body.anchoredPosition=new Vector2((low.x+high.x-941*scale)*.5f-content.rect.xMin,(low.y+high.y+1672*scale)*.5f-content.rect.yMax);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);float ratio=View.Environment.sprite.rect.width/View.Environment.sprite.rect.height;
                float height=Mathf.Max(high.y-low.y,(high.x-low.x)/ratio),width=height*ratio;
                View.Environment.rectTransform.anchoredPosition=new Vector2((low.x+high.x-width)*.5f-content.rect.xMin,(low.y+high.y+height)*.5f-content.rect.yMax);View.Environment.rectTransform.sizeDelta=new Vector2(width,height);
            }finally{applying=false;}
        }
    }
}
