using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Gameplay metal joints and recessed surfaces; input stays on the full cell root.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionGameplayCell : Image
    {
        public bool FrameOnly, InsetPanel, BronzeSurface;
        static readonly Color Light=new Color32(239,207,140,255),Gold=new Color32(186,136,65,255),Dark=new Color32(62,39,20,255);
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect r=GetPixelAdjustedRect();if(r.width<=0||r.height<=0)return;
            if(FrameOnly){BoardRim(vh,r);return;}
            // Continuous independent lacquer lies under the translucent cell faces.
            Quad(vh,r,BronzeSurface?new Color32(28,23,16,255):new Color32(12,24,31,115),
                BronzeSurface?new Color32(12,12,10,255):new Color32(3,10,17,175));
            Border(vh,r,1.15f,Dark);r=Inset(r,1.15f);Border(vh,r,.65f,Gold);
            r=Inset(r,.65f);Border(vh,r,.55f,new Color32(32,23,15,255));
            float radius=InsetPanel?5.1f:2.65f;
            Joint(vh,new Vector2(r.xMin,r.yMin),radius);Joint(vh,new Vector2(r.xMax,r.yMin),radius);
            Joint(vh,new Vector2(r.xMin,r.yMax),radius);Joint(vh,new Vector2(r.xMax,r.yMax),radius);
            if(InsetPanel)
            {
                Curl(vh,new Vector2(r.xMin+8,r.yMax-3),new Vector2(1,-1),5);
                Curl(vh,new Vector2(r.xMax-8,r.yMax-3),new Vector2(-1,-1),5);
                Curl(vh,new Vector2(r.xMin+8,r.yMin+3),new Vector2(1,1),5);
                Curl(vh,new Vector2(r.xMax-8,r.yMin+3),new Vector2(-1,1),5);
                Line(vh,new Vector2(r.xMin+13,r.yMax-1.5f),new Vector2(r.xMax-13,r.yMax-1.5f),1.1f,new Color32(0,0,0,160));
            }
        }
        void BoardRim(VertexHelper vh,Rect r)
        {
            Chamfer(vh,Inset(r,1),8,3.1f,new Color32(25,19,13,255));
            Chamfer(vh,Inset(r,2.2f),7.3f,1.7f,Gold);Chamfer(vh,Inset(r,3.6f),6.7f,.6f,Light);Chamfer(vh,Inset(r,5),5.5f,1,Dark);
            Joint(vh,new Vector2(r.xMin+7,r.yMax-7),4);Joint(vh,new Vector2(r.xMax-7,r.yMax-7),4);
            Joint(vh,new Vector2(r.xMin+7,r.yMin+7),4);Joint(vh,new Vector2(r.xMax-7,r.yMin+7),4);
            Vector2 top=new Vector2(r.center.x,r.yMax-4);Jewel(vh,top,7,10);
            Curl(vh,top+new Vector2(-10,0),new Vector2(-1,-1),7);Curl(vh,top+new Vector2(10,0),new Vector2(1,-1),7);
            Jewel(vh,new Vector2(r.xMin+2.2f,r.center.y),3.3f,5);Jewel(vh,new Vector2(r.xMax-2.2f,r.center.y),3.3f,5);
        }
        static Rect Inset(Rect r,float p)=>Rect.MinMaxRect(r.xMin+p,r.yMin+p,r.xMax-p,r.yMax-p);
        void Chamfer(VertexHelper vh,Rect r,float cut,float width,Color c)
        {
            Vector2[] p={new Vector2(r.xMin+cut,r.yMax),new Vector2(r.xMax-cut,r.yMax),new Vector2(r.xMax,r.yMax-cut),new Vector2(r.xMax,r.yMin+cut),
                new Vector2(r.xMax-cut,r.yMin),new Vector2(r.xMin+cut,r.yMin),new Vector2(r.xMin,r.yMin+cut),new Vector2(r.xMin,r.yMax-cut)};
            for(int i=0;i<p.Length;i++)Line(vh,p[i],p[(i+1)%p.Length],width,Color.Lerp(c,new Color(c.r*.52f,c.g*.52f,c.b*.52f,c.a),i>=2&&i<=5?.45f:0));
        }
        void Border(VertexHelper vh,Rect r,float w,Color c)
        {
            Color low=new Color(c.r*.6f,c.g*.6f,c.b*.6f,c.a);
            Quad(vh,new Rect(r.xMin,r.yMin,r.width,w),low,low);Quad(vh,new Rect(r.xMin,r.yMax-w,r.width,w),c,c);
            Quad(vh,new Rect(r.xMin,r.yMin,w,r.height),c,low);Quad(vh,new Rect(r.xMax-w,r.yMin,w,r.height),low,low);
        }
        void Quad(VertexHelper vh,Rect r,Color top,Color bottom)
        {
            Tri(vh,new Vector2(r.xMin,r.yMin),new Vector2(r.xMin,r.yMax),new Vector2(r.xMax,r.yMax),bottom,top,top);
            Tri(vh,new Vector2(r.xMax,r.yMax),new Vector2(r.xMax,r.yMin),new Vector2(r.xMin,r.yMin),top,bottom,bottom);
        }
        void Tri(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Color ca,Color cb,Color cc)
        {
            int n=vh.currentVertCount;vh.AddVert(a,ca*color,Vector2.zero);vh.AddVert(b,cb*color,Vector2.zero);vh.AddVert(c,cc*color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);
        }
        void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color c)
        {
            Vector2 d=(b-a).normalized,n=new Vector2(-d.y,d.x)*width*.5f;
            Tri(vh,a+n,b+n,b-n,c,c,c);Tri(vh,b-n,a-n,a+n,c,c,c);
        }
        void Joint(VertexHelper vh,Vector2 center,float radius)
        {
            Vector2 previous=Vector2.zero;const int count=16;
            for(int i=0;i<=count;i++)
            {
                float angle=i*Mathf.PI*2/count,r=radius*(.78f+.22f*Mathf.Cos(angle*4));
                Vector2 p=center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*r;
                if(i>0)Line(vh,previous,p,.65f,i>count/2?Gold:Light);previous=p;
            }
            Jewel(vh,center,radius*.36f,radius*.46f,false);
        }
        void Curl(VertexHelper vh,Vector2 origin,Vector2 direction,float size)
        {
            Vector2 previous=origin;
            for(int i=1;i<=18;i++)
            {
                float t=i/18f,angle=t*Mathf.PI*2.2f,radius=size*(1-t)*.45f;
                Vector2 p=origin+Vector2.Scale(new Vector2(size*t+Mathf.Sin(angle)*radius,(1-Mathf.Cos(angle))*radius),direction);
                Line(vh,previous,p,.7f,i<9?Light:Gold);previous=p;
            }
        }
        void Jewel(VertexHelper vh,Vector2 c,float x,float y,bool sapphire=true)
        {
            Vector2 up=c+Vector2.up*y,right=c+Vector2.right*x,down=c+Vector2.down*y,left=c+Vector2.left*x;
            Tri(vh,up,right,down,Light,Gold,Dark);Tri(vh,down,left,up,Dark,Gold,Light);
            Color blue=sapphire?new Color32(12,86,141,255):Gold,lit=sapphire?new Color32(108,185,218,255):Light,shade=sapphire?new Color32(3,20,49,255):Dark;
            const float inner=.64f;
            Tri(vh,c,Vector2.Lerp(c,up,inner),Vector2.Lerp(c,right,inner),blue,lit,shade);
            Tri(vh,c,Vector2.Lerp(c,right,inner),Vector2.Lerp(c,down,inner),blue,shade,shade);
            Tri(vh,c,Vector2.Lerp(c,down,inner),Vector2.Lerp(c,left,inner),blue,shade,blue);
            Tri(vh,c,Vector2.Lerp(c,left,inner),Vector2.Lerp(c,up,inner),blue,lit,lit);
        }
    }
}
