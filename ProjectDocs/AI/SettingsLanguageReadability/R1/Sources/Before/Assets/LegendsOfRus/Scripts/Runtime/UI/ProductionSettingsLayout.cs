using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionSettingsLayout:MonoBehaviour
    {
        public ProductionSettingsView View;readonly Vector3[] corners=new Vector3[4];bool applying;
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
                safe.GetWorldCorners(corners);var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);
                // Keep the central carved composition at its original aspect; navigation follows safe edges.
                float top=safeHigh.y-View.Body.rect.yMax,bottom=safeLow.y-View.Body.rect.yMin;
                View.Hud.anchoredPosition=new Vector2(0,top);
                ((RectTransform)View.Body.Find("CarvedFrame")).sizeDelta=new Vector2(822,1363-bottom);
                ((RectTransform)View.BackButton.transform).anchoredPosition=new Vector2(380,-1477+bottom);
                View.VersionText.rectTransform.anchoredPosition=new Vector2(636,-1573+bottom);
                var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");shield.anchoredPosition=new Vector2(0,top);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
