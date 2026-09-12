using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Native ornamental divider. The generated delivery contained no separate divider PNG.
    /// Reconstructed geometry, not an extracted screenshot region; visual signoff still required.
    /// </summary>
    public sealed class SplashTitleDivider : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            float cy = r.center.y, cx = r.center.x;
            float k = r.width / 760f;
            Line(vh, new Vector2(r.xMin,cy), new Vector2(cx-66*k,cy), 1.15f*k, color);
            Line(vh, new Vector2(cx+66*k,cy), new Vector2(r.xMax,cy), 1.15f*k, color);
            Diamond(vh, new Vector2(r.xMin+106*k,cy), 8*k, 12*k, k);
            Diamond(vh, new Vector2(r.xMax-111*k,cy), 8*k, 12*k, k);
            Diamond(vh, new Vector2(cx,cy), 6*k, 17*k, 1.5f*k);
            for (int s = -1; s <= 1; s += 2)
            {
                Curve(vh,new Vector2(cx+s*8*k,cy),new Vector2(cx+s*12*k,cy+23*k),new Vector2(cx+s*30*k,cy+14*k),new Vector2(cx+s*8*k,cy),1.7f*k);
                Curve(vh,new Vector2(cx+s*7*k,cy),new Vector2(cx+s*18*k,cy-14*k),new Vector2(cx+s*26*k,cy-5*k),new Vector2(cx+s*32*k,cy),1.6f*k);
                Curve(vh,new Vector2(cx+s*20*k,cy),new Vector2(cx+s*42*k,cy+18*k),new Vector2(cx+s*56*k,cy-12*k),new Vector2(cx+s*65*k,cy),1.6f*k);
                Diamond(vh,new Vector2(cx+s*60*k,cy),4*k,4*k,k);
            }
        }
        void Diamond(VertexHelper vh,Vector2 p,float w,float h,float thick)
        {
            Vector2 a=p+Vector2.up*h,b=p+Vector2.right*w,c=p-Vector2.up*h,d=p-Vector2.right*w;
            Line(vh,a,b,thick,color);Line(vh,b,c,thick,color);Line(vh,c,d,thick,color);Line(vh,d,a,thick,color);
        }
        void Curve(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,float thick)
        {
            Vector2 prev=a;
            for(int i=1;i<=20;i++)
            {float t=i/20f,u=1f-t;Vector2 next=u*u*u*a+3*u*u*t*b+3*u*t*t*c+t*t*t*d;Line(vh,prev,next,thick,color);prev=next;}
        }
        static void Line(VertexHelper vh,Vector2 a,Vector2 b,float thick,Color c)
        {
            Vector2 delta=b-a;if(delta.sqrMagnitude<.0001f)return;
            Vector2 n=new Vector2(-delta.y,delta.x).normalized*thick*.5f;
            int i=vh.currentVertCount;
            vh.AddVert(a-n,c,Vector2.zero);vh.AddVert(a+n,c,Vector2.zero);
            vh.AddVert(b+n,c,Vector2.zero);vh.AddVert(b-n,c,Vector2.zero);
            vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);
        }
    }
}
