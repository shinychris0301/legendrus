using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Transient, pooled match facets, impact rings and directional energy trails.</summary>
    public sealed class CoreLoopEffectGraphic : Image
    {
        public CoreLoopVfxCue Cue { get; private set; }
        public float Progress { get; private set; }
        public void SetFrame(CoreLoopVfxCue cue,float progress){Cue=cue;Progress=Mathf.Clamp01(progress);SetVerticesDirty();}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect r=GetPixelAdjustedRect();Vector2 c=r.center;
            bool line=Cue==CoreLoopVfxCue.SpecialRowSweep||Cue==CoreLoopVfxCue.SpecialColumnSweep||Cue==CoreLoopVfxCue.BoosterMaceRow||Cue==CoreLoopVfxCue.BoosterMaceColumn||Cue==CoreLoopVfxCue.SwapTrail;
            if(line)
            {
                bool vertical=r.height>r.width;Vector2 along=vertical?Vector2.up:Vector2.right,across=vertical?Vector2.right:Vector2.up;
                float length=(vertical?r.height:r.width)*.48f,thickness=(vertical?r.width:r.height)*.1f;
                // Bright narrow core and translucent shoulders; never a filled row-sized rectangle.
                Quad(vh,c-along*length-across*thickness,c+along*length-across*thickness,c+along*length+across*thickness,c-along*length+across*thickness,color);
                Color soft=color;soft.a*=.22f;
                Quad(vh,c-along*length-across*thickness*2.5f,c+along*length-across*thickness*2.5f,c+along*length+across*thickness*2.5f,c-along*length+across*thickness*2.5f,soft);
                Diamond(vh,c+along*Mathf.Lerp(-length,length,Progress),Mathf.Max(3,thickness*2),color);
                return;
            }
            bool impact=Cue==CoreLoopVfxCue.SpecialBurstPulse||Cue==CoreLoopVfxCue.BoosterFirebird||Cue==CoreLoopVfxCue.BoosterFirebirdCompleted;
            bool shards=Cue==CoreLoopVfxCue.MatchBurst||Cue==CoreLoopVfxCue.MatchResolveGlint||Cue==CoreLoopVfxCue.FallSettleSpark;
            float size=Mathf.Min(r.width,r.height);
            if(shards||impact)
            {
                int count=impact?10:6;float spread=size*Mathf.Lerp(.10f,.43f,Progress);
                for(int i=0;i<count;i++)
                {
                    float angle=(i*360f/count+13)*Mathf.Deg2Rad;Vector2 dir=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle));
                    Vector2 point=c+dir*spread+Vector2.down*Progress*Progress*size*.08f;
                    float radius=size*(impact?.035f:.055f)*(1-Progress*.6f);
                    Diamond(vh,point,Mathf.Max(.5f,radius),color);
                }
                if(shards)return;
            }
            Color ring=color;ring.a*=impact?1f:.35f;
            Ring(vh,c,r.width*Mathf.Lerp(.16f,.46f,Progress),r.height*Mathf.Lerp(.16f,.46f,Progress),Mathf.Max(1,size*(impact?.020f:.013f)),ring);
            if(impact)
            {
                Color inner=Color.Lerp(color,new Color(1f,.78f,.22f,color.a),.4f);inner.a*=.55f;
                Ring(vh,c,r.width*Mathf.Lerp(.08f,.32f,Progress),r.height*Mathf.Lerp(.08f,.32f,Progress),Mathf.Max(1,size*.022f),inner);
            }
        }
        static void Diamond(VertexHelper vh,Vector2 center,float radius,Color tint)
        {
            Vector2 top=center+Vector2.up*radius*1.5f,left=center+Vector2.left*radius,right=center+Vector2.right*radius,bottom=center+Vector2.down*radius;
            Color light=Color.Lerp(tint,new Color(1,.95f,.78f,tint.a),.45f);light.a=tint.a;
            Triangle(vh,top,left,bottom,tint);Triangle(vh,top,bottom,right,light);
        }
        static void Ring(VertexHelper vh,Vector2 c,float rx,float ry,float thickness,Color tint)
        {
            for(int i=0;i<40;i++)
            {
                float a=i*Mathf.PI*2/40,b=(i+1)*Mathf.PI*2/40;
                Quad(vh,c+new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry),c+new Vector2(Mathf.Cos(b)*rx,Mathf.Sin(b)*ry),c+new Vector2(Mathf.Cos(b)*Mathf.Max(0,rx-thickness),Mathf.Sin(b)*Mathf.Max(0,ry-thickness)),c+new Vector2(Mathf.Cos(a)*Mathf.Max(0,rx-thickness),Mathf.Sin(a)*Mathf.Max(0,ry-thickness)),tint);
            }
        }
        static void Quad(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color tint)
        {int i=v.currentVertCount;v.AddVert(a,tint,Vector2.zero);v.AddVert(b,tint,Vector2.zero);v.AddVert(c,tint,Vector2.zero);v.AddVert(d,tint,Vector2.zero);v.AddTriangle(i,i+1,i+2);v.AddTriangle(i,i+2,i+3);}
        static void Triangle(VertexHelper v,Vector2 a,Vector2 b,Vector2 c,Color tint)
        {int i=v.currentVertCount;v.AddVert(a,tint,Vector2.zero);v.AddVert(b,tint,Vector2.zero);v.AddVert(c,tint,Vector2.zero);v.AddTriangle(i,i+1,i+2);}
    }
}
