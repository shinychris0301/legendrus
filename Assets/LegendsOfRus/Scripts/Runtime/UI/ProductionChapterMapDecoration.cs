using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Chapter-only empty star rims and a dimensional Back arrow, over the shared native renderer.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionChapterMapDecoration : BaseMeshEffect
    {
        public bool BackArrow;
        static readonly Color Gold=new Color32(201,145,56,255),Light=new Color32(255,232,166,255),Dark=new Color32(58,31,8,255);
        public override void ModifyMesh(VertexHelper mesh)
        {
            if(!IsActive())return;
            var source=graphic as ProductionWorldMapGraphic;
            if(source==null||(!BackArrow&&!source.Dimmed))return;
            mesh.Clear();Rect r=graphic.rectTransform.rect;
            if(BackArrow)
            {
                // Two convex faces meet along an upper-left highlight; the dark offset gives depth.
                Vector2[] points={new Vector2(-.31f,0),new Vector2(-.02f,.26f),new Vector2(-.02f,.105f),
                    new Vector2(.30f,.105f),new Vector2(.30f,-.105f),new Vector2(-.02f,-.105f),new Vector2(-.02f,-.26f)};
                for(int i=0;i<points.Length;i++)points[i]=r.center+Vector2.Scale(points[i],r.size);
                ArrowFace(mesh,points,new Vector2(2,-4),Dark,Dark);
                ArrowFace(mesh,points,Vector2.zero,Light,Gold);
                return;
            }
            float radius=Mathf.Min(r.width,r.height)*.5f;
            for(int i=0;i<10;i++)
            {
                Vector2 a=Point(i,radius),b=Point(i+1,radius),innerA=a*.78f,innerB=b*.78f;
                Color interior=new Color32(5,10,17,235);
                Tri(mesh,r.center,r.center+innerA,r.center+innerB,interior,interior,interior);
                Color ca=i<5?Gold:Light,cb=i<5?Dark:Gold;
                Tri(mesh,r.center+a,r.center+b,r.center+innerB,ca,ca,cb);
                Tri(mesh,r.center+innerB,r.center+innerA,r.center+a,cb,cb,ca);
            }
        }
        static Vector2 Point(int i,float radius)
        {
            float angle=Mathf.PI*.5f+i*Mathf.PI/5;
            return new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius*(i%2==0?1:.44f);
        }
        static void ArrowFace(VertexHelper mesh,Vector2[] p,Vector2 offset,Color light,Color gold)
        {
            Tri(mesh,p[0]+offset,p[1]+offset,p[6]+offset,light,light,gold);
            Tri(mesh,p[2]+offset,p[3]+offset,p[4]+offset,light,light,gold);
            Tri(mesh,p[4]+offset,p[5]+offset,p[2]+offset,gold,gold,light);
        }
        static void Tri(VertexHelper mesh,Vector2 a,Vector2 b,Vector2 c,Color ca,Color cb,Color cc)
        {
            int start=mesh.currentVertCount;mesh.AddVert(a,ca,Vector2.zero);mesh.AddVert(b,cb,Vector2.zero);
            mesh.AddVert(c,cc,Vector2.zero);mesh.AddTriangle(start,start+1,start+2);
        }
    }
}
