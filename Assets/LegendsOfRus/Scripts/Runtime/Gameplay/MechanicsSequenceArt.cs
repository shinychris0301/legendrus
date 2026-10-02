using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Original R18 artwork. Pixel coordinates are measured transparent atlas regions, not equal cells.
    public static class MechanicsSequenceArt
    {
        static Texture2D effects,materials;
        public static Texture2D Effects=>effects?effects:(effects=Resources.Load<Texture2D>("ProductionUI/MechanicsEffectsR19/EffectsAtlas"));
        public static Texture2D Materials=>materials?materials:(materials=Resources.Load<Texture2D>("ProductionUI/MechanicsEffectsR19/MaterialsAndFormation"));
        public static bool Available=>Effects&&Materials;
        static readonly int[] FxRows={0,330,660,890,1254},MaterialRows={0,314,618,960,1254};
        static readonly int[] GemRegions={1,0,2,3,4,5};
        public static int GemRegion(int color)=>GemRegions[Mathf.Clamp(color,0,5)];
        public static Rect Region(bool material,int index)
        {
            index=Mathf.Clamp(index,0,15);var rows=material?MaterialRows:FxRows;int col=index%4,row=index/4;
            int x=col*1254/4,right=(col+1)*1254/4;
            return new Rect(x,rows[row],right-x,rows[row+1]-rows[row]);
        }
        public static void Quad(VertexHelper h,bool material,int index,Vector2 center,float width,float angle,float alpha,bool flip=false)
        {
            var r=Region(material,index);float height=width*r.height/r.width,c=Mathf.Cos(angle),s=Mathf.Sin(angle);
            float x=width*.5f,y=height*.5f;Vector2 a=new Vector2(-x,-y),b=new Vector2(-x,y),d=new Vector2(x,-y),e=new Vector2(x,y);
            int n=h.currentVertCount;var tint=new Color(1,1,1,Mathf.Clamp01(alpha));float u0=r.xMin/1254f,u1=r.xMax/1254f,v0=1-r.yMax/1254f,v1=1-r.yMin/1254f;
            if(flip){float swap=u0;u0=u1;u1=swap;}
            h.AddVert(center+Rotate(a,c,s),tint,new Vector2(u0,v0));h.AddVert(center+Rotate(b,c,s),tint,new Vector2(u0,v1));
            h.AddVert(center+Rotate(e,c,s),tint,new Vector2(u1,v1));h.AddVert(center+Rotate(d,c,s),tint,new Vector2(u1,v0));h.AddTriangle(n,n+1,n+2);h.AddTriangle(n,n+2,n+3);
        }
        static Vector2 Rotate(Vector2 v,float c,float s)=>new Vector2(v.x*c-v.y*s,v.x*s+v.y*c);
        public static void Blend(VertexHelper h,bool material,int first,int count,float t,Vector2 p,float w,float angle,float alpha)
        {
            float f=Mathf.Clamp01(t)*(count-1);int i=Mathf.FloorToInt(f);float mix=f-i;
            Quad(h,material,first+i,p,w,angle,alpha*(1-mix));if(mix>.001f)Quad(h,material,first+Mathf.Min(count-1,i+1),p,w,angle,alpha*mix);
        }
    }
}
