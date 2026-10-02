using UnityEngine;

namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionPreLevelLayout : MonoBehaviour
    {
        public ProductionPreLevelView View;
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
                safeRect.GetWorldCorners(corners);
                Vector3 low=content.InverseTransformPoint(corners[0]),high=content.InverseTransformPoint(corners[2]);
                float top=high.y-content.rect.yMax,bottom=low.y-content.rect.yMin;
                if(View.SourceControls)
                {
                    Position(View.Hud,0,top);Position(View.TitleArea,0,top-116);Position(View.Footer,0,bottom-1440);
                    Position(View.FooterOrnament.rectTransform,0,bottom-1590);Size(View.FooterOrnament.rectTransform,new Vector2(941,82));
                    float room=Mathf.Max(1,high.y-low.y-594),bodyScale=Mathf.Min(1,room/1078),spare=Mathf.Max(0,room-1078);
                    Position(View.Body,(941-941*bodyScale)*.5f,top-382);View.Body.localScale=Vector3.one*bodyScale;
                    Position(View.GoalsArea,208,0);Position(View.MovesArea,365,-(293+spare*.28f));Position(View.BoostersArea,120,-(437+spare*.5f));Position(View.RewardsArea,122,-(758+spare));
                    canvasRect.GetWorldCorners(corners);Vector3 a=content.InverseTransformPoint(corners[0]),b=content.InverseTransformPoint(corners[2]);
                    Vector2 n=View.Environment.sprite.rect.size;float cover=Mathf.Max((b.x-a.x)/n.x,(b.y-a.y)/n.y);Vector2 sz=n*cover;
                    Position(View.Environment.rectTransform,(a.x+b.x-sz.x)*.5f-content.rect.xMin,(a.y+b.y+sz.y)*.5f-content.rect.yMax);Size(View.Environment.rectTransform,sz);
                    return;
                }
                Position(View.Hud,0,top);Position(View.HudBacking,5,top-5);
                Position(View.TitleArea,0,top-130);Position(View.Footer,0,bottom-1440);
                float available=Mathf.Max(1,high.y-low.y-632);
                float scaleBody=Mathf.Min(1,available/1040);
                float extra=Mathf.Max(0,available-1040);
                Position(View.Body,(941-941*scaleBody)*.5f,top-400);
                View.Body.localScale=Vector3.one*scaleBody;
                Position(View.GoalsArea,185,0);
                Position(View.MovesArea,370,-(300+extra*.32f));
                Position(View.BoostersArea,105,-(455+extra*.62f));
                Position(View.RewardsArea,105,-(755+extra));
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
