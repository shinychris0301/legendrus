using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Distinct final-layer fissures over the retained material; no puzzle mutation.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class PrototypeObstacleDamage : MaskableGraphic
    {
        public bool Ice, EdgeOnly;
        protected override void OnPopulateMesh(VertexHelper h)
        {
            h.Clear();var r=rectTransform.rect;float s=Mathf.Min(r.width,r.height);Vector2 P(float x,float y)=>r.center+new Vector2(x,y)*s;
            if(EdgeOnly)
            {
                Line(h,P(-.1f,-.30f),P(.08f,-.37f),s*.035f,new Color(.02f,.03f,.04f,.95f));
                Line(h,P(.08f,-.37f),P(.12f,-.28f),s*.025f,new Color(.8f,.6f,.25f,.9f));return;
            }
            Vector2[] crack={P(-.18f,.37f),P(-.08f,.2f),P(-.15f,.09f),P(.07f,-.03f),P(.02f,-.2f),P(.18f,-.37f)};
            for(int i=0;i<crack.Length-1;i++)
            {
                Line(h,crack[i],crack[i+1],s*.035f,new Color(.015f,.035f,.06f,.95f));
                Line(h,crack[i]+Vector2.right*s*.018f,crack[i+1]+Vector2.right*s*.018f,s*.012f,Ice?new Color(.75f,.94f,1):new Color(.25f,.72f,1,.8f));
            }
            Line(h,crack[2],P(-.32f,-.06f),s*.025f,new Color(.015f,.035f,.06f,.95f));
        }
        static void Line(VertexHelper h,Vector2 a,Vector2 b,float width,Color c)
        {var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*width*.5f;int i=h.currentVertCount;h.AddVert(a-n,c,Vector2.zero);h.AddVert(a+n,c,Vector2.zero);h.AddVert(b+n,c,Vector2.zero);h.AddVert(b-n,c,Vector2.zero);h.AddTriangle(i,i+1,i+2);h.AddTriangle(i,i+2,i+3);}
    }
}
