using UnityEngine;

namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionWorldMapLayout : MonoBehaviour
    {
        public ProductionWorldMapView View;
        readonly Vector3[] corners=new Vector3[4];
        bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;
            Canvas canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var canvasRect=canvas.rootCanvas.transform as RectTransform;
            var safe=GetComponentInParent<SafeAreaFitter>();var safeRect=safe!=null?safe.transform as RectTransform:canvasRect;
            applying=true;
            try
            {
                RectTransform content=View.Content;
                if(View.SourceComposition)
                {
                    // All foreground layers retain the approved safe 941x1672 registration.
                    canvasRect.GetWorldCorners(corners);
                    Vector3 lo=content.InverseTransformPoint(corners[0]),hi=content.InverseTransformPoint(corners[2]);
                    Vector2 sourceNative=View.Environment.sprite.rect.size;
                    float cover=Mathf.Max((hi.x-lo.x)/sourceNative.x,(hi.y-lo.y)/sourceNative.y);
                    Vector2 dimensions=sourceNative*cover;
                    Position(View.Environment.rectTransform,(lo.x+hi.x-dimensions.x)*.5f-content.rect.xMin,
                        (lo.y+hi.y+dimensions.y)*.5f-content.rect.yMax);
                    Size(View.Environment.rectTransform,dimensions);
                    return;
                }
                safeRect.GetWorldCorners(corners);
                Vector3 low=content.InverseTransformPoint(corners[0]),high=content.InverseTransformPoint(corners[2]);
                float top=high.y-content.rect.yMax,bottom=low.y-content.rect.yMin;
                Position(View.Hud,0,top);Position(View.HudBacking,5,top-5);
                Position(View.TitleArea,0,top-130);Position(View.Footer,0,bottom-1352);
                float mapHeight=Mathf.Max(800,high.y-low.y-642);
                Position(View.MapArea,0,top-300);Size(View.MapArea,new Vector2(941,mapHeight));
                Size(View.Path.rectTransform,new Vector2(941,mapHeight));
                for(int i=0;i<View.Nodes.Length;i++)
                {
                    Vector2 p=ProductionWorldMapRuntime.NodeCenters[i];
                    Position(View.Nodes[i].Root,p.x-50,-((p.y-300)/1030*mapHeight-50));
                }
                float middle=(high.y+low.y)*.5f;
                Position((RectTransform)View.TaskPanel.transform,140,middle+240-content.rect.yMax);
                canvasRect.GetWorldCorners(corners);
                low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);
                Vector2 native=View.Environment.sprite.rect.size;
                float scale=Mathf.Max((high.x-low.x)/native.x,(high.y-low.y)/native.y);
                Vector2 size=native*scale;
                Position(View.Environment.rectTransform,(low.x+high.x-size.x)*.5f-content.rect.xMin,(low.y+high.y+size.y)*.5f-content.rect.yMax);
                Size(View.Environment.rectTransform,size);
                float width=high.x-low.x,ornamentScale=width/2136;
                Position(View.FooterOrnament.rectTransform,low.x-content.rect.xMin-4*ornamentScale,low.y+708*ornamentScale-content.rect.yMax);
                Size(View.FooterOrnament.rectTransform,new Vector2(2143*ornamentScale,734*ornamentScale));
            }
            finally{applying=false;}
        }
        static void Position(RectTransform r,float x,float y){Vector2 v=new Vector2(x,y);if((r.anchoredPosition-v).sqrMagnitude>.0001f)r.anchoredPosition=v;}
        static void Size(RectTransform r,Vector2 v){if((r.sizeDelta-v).sqrMagnitude>.0001f)r.sizeDelta=v;}
    }
}
