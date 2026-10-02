using UnityEngine;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    // Code-native prototype emblem: navy stone, antique gold bands and sapphire rune.
    // It reads state only; damage is owned by the resolver.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class RunicSealGraphic : MaskableGraphic
    {
        public int Layers { get; private set; } = 2;
        static readonly Color Gold = new Color32(176,130,53,255);
        static readonly Color Blue = new Color32(86,174,230,255);
        public static void Refresh(Image host, BoardObstacleKind kind, int layers = 2)
        {
            var existing=host.transform.Find("RunicSealGlyph");
            if(kind!=BoardObstacleKind.RunicSeal){if(existing)existing.gameObject.SetActive(false);host.enabled=true;return;}
            RunicSealGraphic glyph;
            if(existing)glyph=existing.GetComponent<RunicSealGraphic>();
            else
            {
                var go=new GameObject("RunicSealGlyph",typeof(RectTransform),typeof(RunicSealGraphic));go.transform.SetParent(host.transform,false);
                var r=(RectTransform)go.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
                glyph=go.GetComponent<RunicSealGraphic>();glyph.raycastTarget=false;
            }
            host.enabled=false;glyph.gameObject.SetActive(true);glyph.Layers=layers;glyph.SetVerticesDirty();
        }
        protected override void OnPopulateMesh(VertexHelper h)
        {
            h.Clear();var rect=rectTransform.rect;float s=Mathf.Min(rect.width,rect.height);Vector2 c=rect.center;
            Vector2 P(float x,float y)=>c+new Vector2(x,y)*s;
            var dark=new Color32(12,24,39,255);
            // Eight beveled faces, deliberately opaque within the panel boundary.
            for(int i=0;i<8;i++)
            {
                float a=(i+.5f)*Mathf.PI/4,b=(i+1.5f)*Mathf.PI/4;
                var v1=P(Mathf.Cos(a)*.47f,Mathf.Sin(a)*.47f);var v2=P(Mathf.Cos(b)*.47f,Mathf.Sin(b)*.47f);
                Triangle(h,c,v1,v2,dark);
                Line(h,v1,v2,s*.027f,Gold);
                if(Layers==2 || i%3!=0)Line(h,c+(v1-c)*.79f,c+(v2-c)*.79f,s*.012f,Layers==2?Gold:new Color32(89,73,48,255));
            }
            // Branching protective rune; two intact bands become one cracked ring.
            Line(h,P(0,-.28f),P(0,.27f),s*.044f,Gold);
            Line(h,P(0,-.25f),P(0,.25f),s*.019f,Blue);
            Line(h,P(0,.02f),P(-.18f,.18f),s*.029f,Blue);
            Line(h,P(0,.02f),P(.18f,.18f),s*.029f,Blue);
            if(Layers==2){Line(h,P(-.19f,-.15f),P(.19f,-.15f),s*.021f,Gold);Line(h,P(-.19f,.3f),P(.19f,.3f),s*.021f,Gold);}
            else {Line(h,P(.25f,.36f),P(.12f,.15f),s*.028f,Color.black);Line(h,P(.12f,.15f),P(.26f,-.05f),s*.024f,Color.black);Line(h,P(.26f,-.05f),P(.12f,-.34f),s*.018f,Color.black);}
            Triangle(h,P(0,.35f),P(-.045f,.3f),P(0,.25f),Blue);Triangle(h,P(0,.35f),P(0,.25f),P(.045f,.3f),new Color32(25,87,170,255));
        }
        static void Triangle(VertexHelper h,Vector2 a,Vector2 b,Vector2 c,Color color)
        {int n=h.currentVertCount;h.AddVert(a,color,Vector2.zero);h.AddVert(b,color,Vector2.zero);h.AddVert(c,color,Vector2.zero);h.AddTriangle(n,n+1,n+2);}
        static void Line(VertexHelper h,Vector2 a,Vector2 b,float width,Color color)
        {var d=(b-a).normalized;var p=new Vector2(-d.y,d.x)*width*.5f;Triangle(h,a-p,a+p,b+p,color);Triangle(h,a-p,b+p,b-p,color);}
    }
}
