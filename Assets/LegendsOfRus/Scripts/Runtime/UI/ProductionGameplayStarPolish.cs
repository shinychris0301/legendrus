using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Gameplay-only unearned outline; shared World Map rendering and earned facets are untouched.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionGameplayStarPolish : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            var star=graphic as ProductionWorldMapGraphic;
            if(!IsActive()||star==null||star.Kind!=WorldMapGraphicKind.Star||star.ActiveStarArt!=null||!star.Dimmed)return;
            vh.Clear();Rect r=star.GetPixelAdjustedRect();Vector2 center=r.center;float radius=Mathf.Min(r.width,r.height)*.5f;
            Color dark=new Color32(4,11,17,255),gold=new Color32(179,122,45,255),light=new Color32(250,209,121,255);
            for(int i=0;i<10;i++)
            {
                Vector2 a=Point(i,radius),b=Point((i+1)%10,radius);
                Color ca=Color.Lerp(gold,light,Mathf.Clamp01(.5f+(-a.x+a.y)/Mathf.Max(1,radius)*.35f));
                Color cb=Color.Lerp(gold,light,Mathf.Clamp01(.5f+(-b.x+b.y)/Mathf.Max(1,radius)*.35f));
                Tri(vh,center,center+a*.70f,center+b*.70f,dark,dark,dark);
                Tri(vh,center+a,center+b,center+b*.70f,ca,cb,gold);Tri(vh,center+b*.70f,center+a*.70f,center+a,gold,gold,ca);
            }
        }
        static Vector2 Point(int i,float radius)
        {
            float angle=Mathf.PI*.5f+i*Mathf.PI/5;return new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius*(i%2==0?1:.44f);
        }
        static void Tri(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Color ca,Color cb,Color cc)
        {
            int n=vh.currentVertCount;vh.AddVert(a,ca,Vector2.zero);vh.AddVert(b,cb,Vector2.zero);vh.AddVert(c,cc,Vector2.zero);vh.AddTriangle(n,n+1,n+2);
        }
    }
}
