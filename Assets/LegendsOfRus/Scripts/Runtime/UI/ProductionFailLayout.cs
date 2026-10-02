using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionFailLayout:MonoBehaviour
    {
        public ProductionFailView View;
        readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;
            var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var canvasRect=canvas.rootCanvas.transform as RectTransform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?fitter.transform as RectTransform:canvasRect;
            applying=true;
            try
            {
                var content=View.Content;safe.GetWorldCorners(corners);Vector3 low=content.InverseTransformPoint(corners[0]),high=content.InverseTransformPoint(corners[2]);
                float top=high.y-content.rect.yMax;
                View.Hud.anchoredPosition=new Vector2(0,top);View.HudBacking.anchoredPosition=new Vector2(0,top);
                float available=Mathf.Max(1,high.y-low.y-150),scale=Mathf.Min(1,available/1515);
                View.Body.localScale=Vector3.one*scale;View.Body.anchoredPosition=new Vector2(941*(1-scale)*.5f,top-150-Mathf.Max(0,available-1515)*.5f);
                var sceneWindow=View.Body.Find("SceneWindow") as RectTransform;
                if(sceneWindow!=null)
                {
                    float gap=Mathf.Max(0,available-1515)*.5f/scale;
                    ProductionVictoryView.Place(sceneWindow,0,-22-gap,941,555+gap);
                    var scene=sceneWindow.Find("Scene") as RectTransform;
                    if(scene!=null)scene.anchoredPosition=new Vector2(0,-ProductionFailRuntime.SceneTop-gap);
                }
                canvasRect.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);
                Vector2 native=View.Environment.sprite.rect.size;float cover=Mathf.Max((high.x-low.x)/native.x,(high.y-low.y)/native.y);Vector2 size=native*cover;
                View.Environment.rectTransform.anchoredPosition=new Vector2((low.x+high.x-size.x)*.5f-content.rect.xMin,(low.y+high.y+size.y)*.5f-content.rect.yMax);View.Environment.rectTransform.sizeDelta=size;
            }
            finally{applying=false;}
        }
    }
}
