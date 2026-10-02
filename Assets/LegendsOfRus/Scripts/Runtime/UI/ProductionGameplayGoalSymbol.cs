using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Dedicated score star and survival hourglass in the existing navy/gold goal medallion.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionGameplayGoalSymbol : MaskableGraphic
    {
        public bool Survival;
        static readonly Color Light = new Color32(255, 239, 185, 255);
        static readonly Color Gold = new Color32(216, 162, 68, 255);
        static readonly Color Shade = new Color32(108, 64, 19, 255);
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); Rect r = GetPixelAdjustedRect(); Vector2 c = r.center;
            float radius = Mathf.Min(r.width, r.height) * .5f;
            if (!Survival)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 a = Star(i, radius), b = Star((i + 1) % 10, radius);
                    Tri(vh, c, c+a, c+b, Gold, Light, Shade);
                    Tri(vh, c, c+a*.83f, c+b*.83f, i%2==0?Light:Gold, Gold, i%2==0?Gold:Shade);
                }
                return;
            }
            float x = radius*.61f, y = radius*.78f;
            // Gold supports enclose sapphire glass and two readable sand chambers.
            Tri(vh, c+new Vector2(-x,y), c+new Vector2(x,y), c, Light, Gold, new Color32(39,108,155,255));
            Tri(vh, c+new Vector2(x,-y), c+new Vector2(-x,-y), c, Shade, Gold, new Color32(24,67,97,255));
            Line(vh,c+new Vector2(-x,y),c+new Vector2(x,-y),radius*.10f,Light);
            Line(vh,c+new Vector2(x,y),c+new Vector2(-x,-y),radius*.10f,Gold);
            Tri(vh,c+new Vector2(-x*.56f,y*.45f),c+new Vector2(x*.56f,y*.45f),c,Light,Gold,Gold);
            Tri(vh,c+new Vector2(-x*.78f,-y*.87f),c+new Vector2(x*.78f,-y*.87f),c+Vector2.down*y*.30f,Gold,Shade,Light);
            Line(vh,c+new Vector2(-x*1.15f,y),c+new Vector2(x*1.15f,y),radius*.18f,Light);
            Line(vh,c+new Vector2(-x*1.15f,-y),c+new Vector2(x*1.15f,-y),radius*.18f,Gold);
        }
        static Vector2 Star(int i,float r){float a=Mathf.PI*.5f+i*Mathf.PI/5;return new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r*(i%2==0?1:.44f);}
        static void Tri(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Color ca,Color cb,Color cc)
        {int n=vh.currentVertCount;vh.AddVert(a,ca,Vector2.zero);vh.AddVert(b,cb,Vector2.zero);vh.AddVert(c,cc,Vector2.zero);vh.AddTriangle(n,n+1,n+2);}
        static void Line(VertexHelper vh,Vector2 a,Vector2 b,float w,Color c)
        {Vector2 d=(b-a).normalized,n=new Vector2(-d.y,d.x)*w*.5f;Tri(vh,a+n,b+n,b-n,c,c,c);Tri(vh,b-n,a-n,a+n,c,c,c);}
    }
}
