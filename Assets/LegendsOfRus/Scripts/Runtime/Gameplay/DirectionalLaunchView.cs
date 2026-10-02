using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Two reusable Canvas batches: approved crystal tips and independently stretched exhaust.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class DirectionalLaunchView : MaskableGraphic
    {
        struct Launch {public BoardPosition Cell;public bool Vertical;public float Born;}
        const int Capacity=32;
        readonly List<Launch> launches=new List<Launch>(Capacity);
        Match3Board board;Sprite body;bool trails;
        public int ActiveCount=>launches.Count;
        public bool TrailLayer=>trails;
        public override Texture mainTexture=>trails?CreationAccentsGraphic.Atlas:body?body.texture:base.mainTexture;
        public static float DistanceInCells(float age,int span)=>Mathf.Max(0,age-BoardPresentationTimeline.Charge)/BoardPresentationTimeline.Sweep*span;
        public static void Attach(Match3Board board)
        {
            var root=new GameObject("DirectionalLaunchClip",typeof(RectTransform),typeof(RectMask2D));root.transform.SetParent(board.transform,false);Stretch((RectTransform)root.transform);
            for(int i=0;i<2;i++){var go=new GameObject(i==0?"DirectionalExhaust":"DirectionalHeads",typeof(RectTransform),typeof(DirectionalLaunchView));go.transform.SetParent(root.transform,false);Stretch((RectTransform)go.transform);var v=go.GetComponent<DirectionalLaunchView>();v.board=board;v.body=PrototypeBoardArt.Load("line");v.trails=i==0;v.raycastTarget=false;board.MotionEventPublished+=v.Motion;}
        }
        static void Stretch(RectTransform r){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;}
        void Motion(GameplayMotionEvent e)
        {
            if(e.Kind==GameplayMotionKind.CommandRecovered){Clear();return;}if(!isActiveAndEnabled)return;
            bool vertical=e.SpecialKind==SpecialGemKind.Column;
            float born=Time.unscaledTime;
            if(e.Kind==GameplayMotionKind.BoosterActivated&&(e.BoosterKind==GameplayBoosterKind.BogatyrMaceRow||e.BoosterKind==GameplayBoosterKind.BogatyrMaceColumn)){vertical=e.BoosterKind==GameplayBoosterKind.BogatyrMaceColumn;born+=.18f-BoardPresentationTimeline.Charge;}
            else if((e.Kind!=GameplayMotionKind.SpecialActivationStarted&&e.Kind!=GameplayMotionKind.SpecialPayloadImpact)||(e.SpecialKind!=SpecialGemKind.Row&&e.SpecialKind!=SpecialGemKind.Column))return;
            if(launches.Count==Capacity)launches.RemoveAt(0);
            launches.Add(new Launch{Cell=e.Primary,Vertical=vertical,Born=born});SetVerticesDirty();
        }
        void Clear(){launches.Clear();SetVerticesDirty();}
        void Update(){if(launches.RemoveAll(e=>Time.unscaledTime-e.Born>.62f)>0||launches.Count>0)SetVerticesDirty();}
        protected override void OnDisable(){Clear();base.OnDisable();}
        protected override void OnDestroy(){if(board)board.MotionEventPublished-=Motion;base.OnDestroy();}
        protected override void OnPopulateMesh(VertexHelper h)
        {
            h.Clear();if(!board||!body||!CreationAccentsGraphic.Atlas)return;
            Rect r=rectTransform.rect;float cw=r.width/board.ColumnCount,ch=r.height/board.RowCount,u=Mathf.Min(cw,ch);int span=Mathf.Max(board.ColumnCount,board.RowCount);
            var bodyUV=DataUtility.GetOuterUV(body);
            var tipUV=new Vector4(Mathf.Lerp(bodyUV.x,bodyUV.z,.60f),bodyUV.y,bodyUV.z,bodyUV.w);
            var trailUV=new Vector4(0,1-630f/1254f,670f/1254f,1);
            foreach(var e in launches)
            {
                float age=Time.unscaledTime-e.Born;if(age<0)continue;
                Vector2 origin=new Vector2(r.xMin+(e.Cell.X+.5f)*cw,r.yMax-(e.Cell.Y+.5f)*ch);
                float cells=DistanceInCells(age,span),distance=cells*(e.Vertical?ch:cw);
                for(int sign=-1;sign<=1;sign+=2)
                {
                    Vector2 axis=(e.Vertical?Vector2.up:Vector2.right)*sign;
                    float angle=Mathf.Atan2(axis.y,axis.x);
                    Vector2 head=origin+axis*distance;
                    float edge=e.Vertical?(sign>0?r.yMax-origin.y:origin.y-r.yMin):(sign>0?r.xMax-origin.x:origin.x-r.xMin);
                    float fade=1-Mathf.Clamp01((distance-edge-u*.25f)/(u*1.5f));
                    if(fade<=0)continue;
                    if(!trails)
                    {
                        float charge=Mathf.Clamp01(age/BoardPresentationTimeline.Charge);
                        float length=u*(age<BoardPresentationTimeline.Charge?.33f+.12f*charge:.52f);
                        Quad(h,tipUV,head-axis*length*.5f,new Vector2(length,u*.64f),angle,fade);
                    }
                    else if(age>=BoardPresentationTimeline.Charge)
                    {
                        float length=Mathf.Min(distance+u*.3f,u*2.6f);
                        Quad(h,trailUV,head-axis*length*.55f,new Vector2(length,u*.62f),angle,fade);
                        Quad(h,trailUV,head-axis*length*.45f,new Vector2(length*.85f,u*.27f),angle,fade*.85f);
                        // Short textured sparks shed from the moving front; no particle GameObjects.
                        Vector2 normal=new Vector2(-axis.y,axis.x);
                        for(int j=0;j<6;j++)
                        {
                            float phase=Mathf.Repeat(age*3.5f+j*.167f,1),side=j%2==0?-1:1;
                            Vector2 q=head-axis*(u*(.3f+phase*1.8f))+normal*side*u*phase*(.2f+(j%3)*.1f);
                            Quad(h,trailUV,q,new Vector2(u*.30f,u*.09f),angle+side*.25f,fade*(1-phase)*.8f);
                        }
                    }
                }
            }
        }
        static void Quad(VertexHelper h,Vector4 uv,Vector2 center,Vector2 size,float angle,float alpha)
        {
            float c=Mathf.Cos(angle),s=Mathf.Sin(angle);Vector2 x=new Vector2(c,s)*size.x*.5f,y=new Vector2(-s,c)*size.y*.5f;int n=h.currentVertCount;var tint=new Color(1,1,1,Mathf.Clamp01(alpha));
            h.AddVert(center-x-y,tint,new Vector2(uv.x,uv.y));h.AddVert(center-x+y,tint,new Vector2(uv.x,uv.w));h.AddVert(center+x+y,tint,new Vector2(uv.z,uv.w));h.AddVert(center+x-y,tint,new Vector2(uv.z,uv.y));h.AddTriangle(n,n+1,n+2);h.AddTriangle(n,n+2,n+3);
        }
    }
}
