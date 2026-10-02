using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public enum WorldMapGraphicKind { Node, Star, Halo, Lock, Path, Back }

    /// <summary>Native gold bevels and path lamps, with optional source-backed stars.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionWorldMapGraphic : MaskableGraphic
    {
        public WorldMapGraphicKind Kind;
        public bool Dimmed;
        // Optional source stars, assigned only by Gameplay; other pages retain native geometry.
        public Sprite StarFilledArt,StarEmptyArt;
        public Sprite ActiveStarArt=>Kind==WorldMapGraphicKind.Star?(Dimmed?StarEmptyArt:StarFilledArt):null;
        public override Texture mainTexture=>ActiveStarArt!=null?ActiveStarArt.texture:base.mainTexture;
        static readonly Color Gold=new Color32(235,176,71,255),Light=new Color32(255,236,171,255),Dark=new Color32(63,38,14,255);
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect r=GetPixelAdjustedRect();Vector2 c=r.center;float radius=Mathf.Min(r.width,r.height)*.5f;
            if(ActiveStarArt!=null){SourceStar(vh,r,ActiveStarArt);return;}
            if(Kind==WorldMapGraphicKind.Path){Path(vh,r);return;}
            if(Kind==WorldMapGraphicKind.Star){Star(vh,c,radius);return;}
            if(Kind==WorldMapGraphicKind.Halo){Glow(vh,c,radius,new Color(1,.61f,.15f,.35f));return;}
            if(Kind==WorldMapGraphicKind.Lock){Lock(vh,r);return;}
            Node(vh,c,radius);
            if(Kind==WorldMapGraphicKind.Back)
            {
                Vector2 a=c+new Vector2(radius*.25f,radius*.45f),b=c-new Vector2(radius*.25f,0),d=c+new Vector2(radius*.25f,-radius*.45f);
                Line(vh,a,b,4,Light);Line(vh,b,d,4,Gold);
            }
        }
        void SourceStar(VertexHelper vh,Rect r,Sprite art)
        {
            float fit=Mathf.Min(r.width/art.rect.width,r.height/art.rect.height);
            Vector2 size=art.rect.size*fit;Rect q=new Rect(r.center-size*.5f,size);
            Vector4 uv=UnityEngine.Sprites.DataUtility.GetOuterUV(art);
            vh.AddVert(new Vector3(q.xMin,q.yMin),color,new Vector2(uv.x,uv.y));
            vh.AddVert(new Vector3(q.xMin,q.yMax),color,new Vector2(uv.x,uv.w));
            vh.AddVert(new Vector3(q.xMax,q.yMax),color,new Vector2(uv.z,uv.w));
            vh.AddVert(new Vector3(q.xMax,q.yMin),color,new Vector2(uv.z,uv.y));
            vh.AddTriangle(0,1,2);vh.AddTriangle(2,3,0);
        }
        Color Tone(Color input)
        {
            if(!Dimmed)return input;
            float gray=input.grayscale;return new Color(gray*.62f,gray*.59f,gray*.51f,input.a);
        }
        void Node(VertexHelper vh,Vector2 c,float radius)
        {
            Disc(vh,c+new Vector2(1,-3),radius+2,new Color(0,0,0,.75f),new Color(0,0,0,.2f));
            Band(vh,c,radius,radius*.96f,Tone(Dark),Tone(Gold));
            Band(vh,c,radius*.96f,radius*.91f,Tone(Light),Tone(Dark));
            Band(vh,c,radius*.91f,radius*.86f,Tone(new Color32(167,104,32,255)),Tone(Light));
            Disc(vh,c,radius*.86f,Tone(new Color32(9,21,31,255)),Tone(new Color32(15,39,56,255)));
            Band(vh,c,radius*.84f,radius*.80f,Tone(Gold),Tone(new Color32(96,62,25,255)));
        }
        static Vector2 Circle(float angle)=>new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
        static void Tri(VertexHelper vh,Vector2 a,Vector2 b,Vector2 c,Color ca,Color cb,Color cc)
        {
            int n=vh.currentVertCount;vh.AddVert(a,ca,Vector2.zero);vh.AddVert(b,cb,Vector2.zero);vh.AddVert(c,cc,Vector2.zero);vh.AddTriangle(n,n+1,n+2);
        }
        static Color Direction(Color light,Color dark,Vector2 p)=>Color.Lerp(dark,light,Mathf.Clamp01(.5f+(-p.x+p.y)*.36f));
        static void Band(VertexHelper vh,Vector2 c,float outer,float inner,Color light,Color dark)
        {
            const int count=64;
            for(int i=0;i<count;i++)
            {
                Vector2 a=Circle(i*Mathf.PI*2/count),b=Circle((i+1)*Mathf.PI*2/count);
                Color ca=Direction(light,dark,a),cb=Direction(light,dark,b);
                Tri(vh,c+a*outer,c+b*outer,c+b*inner,ca,cb,cb);Tri(vh,c+b*inner,c+a*inner,c+a*outer,cb,ca,ca);
            }
        }
        static void Disc(VertexHelper vh,Vector2 c,float radius,Color center,Color edge)
        {
            for(int i=0;i<48;i++)Tri(vh,c,c+Circle(i*Mathf.PI/24)*radius,c+Circle((i+1)*Mathf.PI/24)*radius,center,edge,edge);
        }
        static void Glow(VertexHelper vh,Vector2 c,float radius,Color color)
        {
            Disc(vh,c,radius,color,new Color(color.r,color.g,color.b,0));
        }
        void Star(VertexHelper vh,Vector2 c,float radius)
        {
            for(int i=0;i<10;i++)
            {
                Vector2 a=Circle(Mathf.PI*.5f+i*Mathf.PI/5)*(i%2==0?radius:radius*.44f);
                Vector2 b=Circle(Mathf.PI*.5f+(i+1)*Mathf.PI/5)*(i%2==0?radius*.44f:radius);
                Tri(vh,c,c+a,c+b,Tone(Gold),Tone(Dark),Tone(Dark));
                Color facet=i%2==0?Light:Gold;
                Tri(vh,c,c+a*.82f,c+b*.82f,Tone(facet),Tone(Gold),Tone(i%2==0?Gold:Dark));
            }
        }
        static void Line(VertexHelper vh,Vector2 a,Vector2 b,float width,Color col)
        {
            Vector2 d=(b-a).normalized;Vector2 n=new Vector2(-d.y,d.x)*width*.5f;
            Tri(vh,a+n,b+n,b-n,col,col,col);Tri(vh,b-n,a-n,a+n,col,col,col);
        }
        static void Lock(VertexHelper vh,Rect r)
        {
            Vector2 c=r.center;float w=r.width*.65f;
            Vector2 a=c+new Vector2(-w*.5f,-r.height*.38f),b=c+new Vector2(w*.5f,r.height*.1f);
            Tri(vh,a,new Vector2(a.x,b.y),b,Dark,Light,Gold);Tri(vh,b,new Vector2(b.x,a.y),a,Gold,Dark,Dark);
            Vector2 center=c+new Vector2(0,r.height*.1f);
            for(int i=0;i<16;i++)Line(vh,center+Circle(i*Mathf.PI/16)*w*.36f,center+Circle((i+1)*Mathf.PI/16)*w*.36f,2.5f,Gold);
            Disc(vh,c-new Vector2(0,r.height*.08f),2,Dark,Dark);
        }
        static Vector2 Point(int i,Rect r)
        {
            Vector2 p=ProductionWorldMapRuntime.NodeCenters[Mathf.Clamp(i,0,14)];
            return new Vector2(r.xMin+p.x/941*r.width,r.yMax-(p.y-300)/1030*r.height);
        }
        static void Path(VertexHelper vh,Rect r)
        {
            for(int segment=0;segment<14;segment++)
            {
                Vector2 a=Point(segment,r),b=Point(segment+1,r);
                float distance=Vector2.Distance(a,b);int lamps=Mathf.Max(3,Mathf.CeilToInt(distance/18));
                for(int j=1;j<lamps;j++)
                {
                    float t=(float)j/lamps;
                    // Ease at turns without overshoot; all lamps are independent live geometry.
                    Vector2 p=Vector2.Lerp(a,b,t);p.y+=Mathf.Sin(t*Mathf.PI)*Mathf.Sign(b.x-a.x)*14;
                    if(Vector2.Distance(p,a)<39||Vector2.Distance(p,b)<39)continue;
                    Glow(vh,p,10,new Color(1,.55f,.1f,.34f));
                    Disc(vh,p,4.7f,new Color32(255,238,179,255),new Color32(158,87,24,255));
                    Disc(vh,p+new Vector2(-.7f,1),1.8f,Light,Gold);
                }
            }
        }
    }
}
