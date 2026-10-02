using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    // Presentation driven by the immutable resolution plan. One bounded UI mesh,
    // no full-screen flash, camera transform changes, or gameplay random calls.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed partial class PrototypeImpactView : MaskableGraphic
    {
        enum Shape { Shards, Blast, Row, Column, Charge, Target, Link, Renewal, Jewel, Phoenix, Radiance, Flight, Contact }
        struct Effect { public Vector2 Cell, End; public Color Color; public float Born, Radius, Life; public Shape Shape; }
        const int Capacity=192;
        readonly List<Effect> effects=new List<Effect>(Capacity);
        Match3Board board;
        Material energyMaterial;
        public bool UsesEnergyMaterial=>energyMaterial&&material==energyMaterial;
        readonly List<BoardPosition> colorOrigins=new List<BoardPosition>(8);
        public int ActiveCount=>effects.Count;
        public const float Anticipation=.12f;
        public const float ActivationSeconds=.32f;
        static readonly Color Gold=new Color(1,.69f,.19f);
        static readonly Color[] Colors={new Color(1,.18f,.10f),new Color(.18f,.7f,1),new Color(.4f,.9f,.15f),new Color(1,.75f,.15f),new Color(.75f,.25f,1),new Color(1,.4f,.08f)};
        public static PrototypeImpactView Attach(Match3Board board)
        {
            Transform parent=board.transform;
            if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4)
            {
                var clip=new GameObject("PrototypeEnergyClip",typeof(RectTransform),typeof(RectMask2D));clip.transform.SetParent(parent,false);
                var cr=(RectTransform)clip.transform;cr.anchorMin=Vector2.zero;cr.anchorMax=Vector2.one;cr.offsetMin=cr.offsetMax=Vector2.zero;parent=clip.transform;
            }
            var go=new GameObject("PrototypeMaterialImpacts",typeof(RectTransform));go.transform.SetParent(parent,false);
            var r=(RectTransform)go.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;
            var g=go.AddComponent<PrototypeImpactView>();g.raycastTarget=false;g.board=board;
            if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4){var shader=Resources.Load<Shader>("ProductionUI/MechanicsEffectsR16/Energy");if(shader){g.energyMaterial=new Material(shader){hideFlags=HideFlags.HideAndDontSave};g.material=g.energyMaterial;}}
            board.MotionEventPublished+=g.Motion;board.ObstacleInteracted+=g.Obstacle;return g;
        }
        protected override void OnDestroy(){if(energyMaterial)Destroy(energyMaterial);if(board){board.MotionEventPublished-=Motion;board.ObstacleInteracted-=Obstacle;}base.OnDestroy();}
        protected override void OnDisable(){effects.Clear();colorOrigins.Clear();SetVerticesDirty();base.OnDisable();}
        void Update(){if(effects.RemoveAll(e=>Time.unscaledTime-e.Born>e.Life)>0||effects.Count>0)SetVerticesDirty();}
        void Add(BoardPosition p,Color c,Shape shape,float radius=1,float delay=0,float life=.5f)
        {if(effects.Count>=Capacity)effects.RemoveAt(0);effects.Add(new Effect{Cell=new Vector2(p.X,p.Y),Color=c,Born=Time.unscaledTime+delay,Shape=shape,Radius=radius,Life=life});SetVerticesDirty();}
        public void PresentLink(BoardPosition from, BoardPosition to, float duration,float delay=0)
        {
            Add(from,new Color(.4f,.75f,1),Shape.Link,1,delay,Mathf.Max(.02f,duration));
            int last=effects.Count-1;var e=effects[last];e.End=new Vector2(to.X,to.Y);effects[last]=e;
        }
        void Motion(GameplayMotionEvent m)
        {
            if(m.Kind==GameplayMotionKind.CommandRecovered){effects.Clear();colorOrigins.Clear();SetVerticesDirty();return;}
            if(!isActiveAndEnabled)return;
            if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4 && m.Kind==GameplayMotionKind.CellsImpacted)
                foreach(var cell in m.ClearedCells)if(cell.Cell.Special==SpecialGemKind.None)
                    Add(cell.Position,Colors[Mathf.Clamp(cell.Cell.TileType,0,5)],Shape.Jewel,1,0,GemShatterView.ChipLifetime(3));
            if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4 && m.Kind==GameplayMotionKind.BoosterActivated)
            {
                PresentRichBooster(m);return;
            }
            if(m.Kind==GameplayMotionKind.MatchWaveStarted && board.RuleVersion<GameplayRuleVersion.PrototypeR4)
            {
                foreach(var cell in m.ClearedCells)
                {
                    Color color=Colors[Mathf.Clamp(cell.Cell.TileType,0,5)];
                    foreach(var origin in colorOrigins)
                    {
                        Add(origin,color,Shape.Link,1,0,.34f);
                        int last=effects.Count-1;var link=effects[last];link.End=new Vector2(cell.Position.X,cell.Position.Y);effects[last]=link;
                    }
                }
                colorOrigins.Clear();
            }
            if(m.Kind==GameplayMotionKind.SpecialActivationStarted || m.Kind==GameplayMotionKind.SpecialPayloadImpact)
            {
                if(m.SpecialKind==SpecialGemKind.Spark)
                {
                    Add(m.Primary,Gold,Shape.Charge,.65f,0,.16f);
                    if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4){Add(m.Primary,Gold,Shape.Flight,1,0,SparkFlightView.Duration);int index=effects.Count-1;var flight=effects[index];flight.End=new Vector2(m.Secondary.X,m.Secondary.Y);effects[index]=flight;}
                    Add(m.Secondary,Gold,Shape.Target,.5f,0,SparkFlightView.Duration);
                    if(m.Payload==SpecialGemKind.None)Add(m.Secondary,Gold,Shape.Blast,.65f,SparkFlightView.Duration,.38f);
                }
                else
                {
                    if(board.RuleVersion<GameplayRuleVersion.PrototypeR4&&m.SpecialKind==SpecialGemKind.Color&&colorOrigins.Count<8)colorOrigins.Add(m.Primary);
                    Add(m.Primary,Gold,Shape.Charge,.8f,0,Anticipation);
                    var shape=m.SpecialKind==SpecialGemKind.Row?Shape.Row:m.SpecialKind==SpecialGemKind.Column?Shape.Column:Shape.Blast;
                    Add(m.Primary,m.SpecialKind==SpecialGemKind.Color?new Color(.35f,.7f,1):Gold,shape,m.SpecialKind==SpecialGemKind.Burst?(board.RuleVersion>=GameplayRuleVersion.PrototypeR4?(m.CombinationKind==SpecialCombinationKind.BurstWithBurst?4:2):2.3f):1.2f,Anticipation,.48f);
                }
            }
            // Formation already has an exact-art renderer. Do not layer a second charge/halo over it.
            if(board.RuleVersion<GameplayRuleVersion.PrototypeR4 && (m.Kind==GameplayMotionKind.SpecialCreationStarted || m.Kind==GameplayMotionKind.SpecialGatherStarted || m.Kind==GameplayMotionKind.SpecialConversionStarted))Add(m.Primary,Gold,Shape.Charge,.9f,0,.3f);
            if(m.Kind==GameplayMotionKind.BoosterActivated)
            {
                if(m.BoosterKind==GameplayBoosterKind.SeaPearl)
                {
                    Add(new BoardPosition(board.ColumnCount/2,board.RowCount/2),new Color(.25f,.85f,1),Shape.Renewal,1,0,.6f);
                    return;
                }
                if(m.BoosterKind==GameplayBoosterKind.LivingLight&&colorOrigins.Count<8)colorOrigins.Add(m.Primary);
                var shape=m.BoosterKind==GameplayBoosterKind.BogatyrMaceRow?Shape.Row:m.BoosterKind==GameplayBoosterKind.BogatyrMaceColumn?Shape.Column:Shape.Blast;
                Color tint=m.BoosterKind==GameplayBoosterKind.LivingLight?new Color(.35f,.7f,1):Gold;
                Add(m.Primary,tint,Shape.Charge,1,0,Anticipation);
                Add(m.Primary,tint,shape,m.BoosterKind==GameplayBoosterKind.LivingLight?.8f:2,Anticipation,.5f);
            }
        }
        void Obstacle(BoardObstacleInteractionInstruction o)
        {
            if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4)
            {
                if(isActiveAndEnabled&&o.LayersAfter<o.LayersBefore)Add(o.Position,o.Kind==BoardObstacleKind.Ice||o.Kind==BoardObstacleKind.RunicSeal?new Color(.2f,.65f,1):Gold,Shape.Contact,o.LayersAfter==0?1.25f:.65f,0,.16f);
                return; // MaterialBreakView owns physical material fragments.
            }
            Color c=o.Kind==BoardObstacleKind.RunicSeal?new Color(.22f,.62f,1):o.Kind==BoardObstacleKind.Ice?new Color(.65f,.9f,1):o.Kind==BoardObstacleKind.Chain?new Color(.8f,.75f,.58f):new Color(.7f,.4f,.17f);
            Add(o.Position,c,Shape.Blast,o.LayersAfter==0?.9f:.55f,0,.3f);
            Add(o.Position,c,Shape.Shards,o.LayersAfter==0?1.4f:.7f,0,.5f);
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();if(!board)return;var r=rectTransform.rect;float cw=r.width/board.ColumnCount,ch=r.height/board.RowCount,unit=Mathf.Min(cw,ch);
            foreach(var e in effects)
            {
                if(vh.currentVertCount>58000)break; // Stay below the UI 16-bit vertex budget.
                if(Time.unscaledTime<e.Born)continue;
                float t=Mathf.Clamp01((Time.unscaledTime-e.Born)/e.Life),ease=1-Mathf.Pow(1-t,3);
                Color c=e.Color;c.a=(1-t)*(1-t);
                Vector2 center=new Vector2(r.xMin+(e.Cell.x+.5f)*cw,r.yMax-(e.Cell.y+.5f)*ch);
                if(board.RuleVersion>=GameplayRuleVersion.PrototypeR4 && RichEffect(vh,r,e,center,unit,t))continue;
                switch(e.Shape)
                {
                    case Shape.Charge:
                        Ring(vh,r,center,unit*e.Radius*(1-.65f*t),unit*.04f,Alpha(e.Color,Mathf.Sin(t*Mathf.PI)*.9f));
                        Glow(vh,r,center,unit*.5f,Alpha(e.Color,t*.7f));break;
                    case Shape.Target:
                        Ring(vh,r,center,unit*(.32f+.04f*Mathf.Sin(t*12)),unit*.025f,Alpha(e.Color,.65f));break;
                    case Shape.Link:
                        Vector2 target=new Vector2(r.xMin+(e.End.x+.5f)*cw,r.yMax-(e.End.y+.5f)*ch);
                        Vector2 delta=(target-center)*ease;
                        Beam(vh,r,center+delta*.5f,delta.normalized,delta.magnitude*.5f,unit*.026f*(1-t),Alpha(e.Color,.75f*(1-t)));
                        Glow(vh,r,center+delta,unit*.16f,Alpha(Color.white,1-t));break;
                    case Shape.Renewal:
                        Ring(vh,r,center+Vector2.up*unit*t*.7f,unit*(.4f+t*.7f),unit*.025f,c);
                        for(int mote=0;mote<7;mote++)
                        {
                            var p=center+new Vector2(Mathf.Sin(mote*2.4f+t*4)*unit*.6f,unit*(t*2-(mote%3)*.2f));
                            Glow(vh,r,p,unit*.13f,Alpha(e.Color,.8f*(1-t)));
                        }
                        break;
                    case Shape.Row:case Shape.Column:
                        Vector2 axis=e.Shape==Shape.Row?Vector2.right:Vector2.up;
                        float travel=board.RuleVersion>=GameplayRuleVersion.PrototypeR4?Mathf.Clamp01((Time.unscaledTime-e.Born)/BoardPresentationTimeline.Sweep):ease;
                        float length=(board.RuleVersion>=GameplayRuleVersion.PrototypeR4?Mathf.Max(r.width,r.height):(e.Shape==Shape.Row?r.width:r.height))*travel;
                        Beam(vh,r,center,axis,length,unit*.30f*(1-t),Alpha(e.Color,.55f*(1-t)));
                        Beam(vh,r,center,axis,length,unit*.075f*(1-t),Alpha(new Color(1,.97f,.77f),1-t));
                        Glow(vh,r,center,unit*.7f,Alpha(e.Color,(1-t)*.6f));
                        float maxTravel=e.Shape==Shape.Row?r.width:r.height;
                        float headTravel=board.RuleVersion>=GameplayRuleVersion.PrototypeR4?length:maxTravel*t*1.8f;
                        for(int sign=-1;sign<=1;sign+=2)
                        {
                            Vector2 head=center+axis*headTravel*sign;
                            if(r.Contains(head))
                            {
                                Glow(vh,r,head,unit*.42f,Alpha(e.Color,(1-t)*.8f));
                                Vector2 side=new Vector2(-axis.y,axis.x)*unit*.12f;
                                Quad(vh,r,head+axis*unit*.24f,head+side,head-axis*unit*.24f,head-side,Alpha(new Color(1,.97f,.8f),1-t));
                            }
                        }
                        break;
                    case Shape.Blast:
                        float blastTravel=board.RuleVersion>=GameplayRuleVersion.PrototypeR4?Mathf.Clamp01((Time.unscaledTime-e.Born)/BoardPresentationTimeline.Sweep):ease;
                        float radius=unit*(.15f+blastTravel*e.Radius);
                        Glow(vh,r,center,radius,Alpha(e.Color,.42f*(1-t)));
                        Glow(vh,r,center,unit*.65f*(1-t),Alpha(new Color(1,.97f,.75f),.9f*(1-t)));
                        Ring(vh,r,center,radius,unit*.085f*(1-t),c);
                        Ring(vh,r,center,radius*.72f,unit*.025f*(1-t),Alpha(new Color(1,.92f,.65f),c.a*.6f));
                        Shards(vh,r,center,unit,t,e.Radius,e.Color,10);break;
                    default:Shards(vh,r,center,unit,t,e.Radius,e.Color,7);break;
                }
            }
        }
        static Color Alpha(Color c,float a){c.a=a;return c;}
        void Shards(VertexHelper h,Rect r,Vector2 center,float unit,float t,float scale,Color color,int count)
        {
            for(int i=0;i<count;i++)
            {
                float a=i*Mathf.PI*2/count+.37f;
                Vector2 direction=new Vector2(Mathf.Cos(a),Mathf.Sin(a));
                Vector2 p=center+direction*(Mathf.Sin(t*Mathf.PI*.5f)*(.45f+(i%3)*.12f)*scale*unit)+Vector2.down*(t*t*.48f*unit);
                float size=unit*(.09f+(i%2)*.025f)*(1-t);Vector2 tip=new Vector2(Mathf.Cos(a+t*4),Mathf.Sin(a+t*4))*size,side=new Vector2(-tip.y,tip.x)*.5f;
                Color c=Alpha(color,1-t);Quad(h,r,p+tip,p+side,p-tip,p-side,c);
                Triangle(h,r,p+tip,p,p-side,Alpha(Color.Lerp(color,Color.white,.7f),1-t));
            }
        }
        void Beam(VertexHelper h,Rect r,Vector2 p,Vector2 axis,float length,float width,Color c)
        {var side=new Vector2(-axis.y,axis.x)*width;Quad(h,r,p-axis*length-side,p-axis*length+side,p+axis*length+side,p+axis*length-side,c);}
        void Glow(VertexHelper h,Rect r,Vector2 p,float radius,Color c)
        {
            for(int i=0;i<24;i++){float a=i*Mathf.PI/12,b=(i+1)*Mathf.PI/12;int n=h.currentVertCount;
                Vertex(h,r,p,c);Vertex(h,r,p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,Alpha(c,0));Vertex(h,r,p+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*radius,Alpha(c,0));h.AddTriangle(n,n+1,n+2);}
        }
        void Ring(VertexHelper h,Rect r,Vector2 p,float radius,float width,Color c)
        {for(int i=0;i<40;i++){float a=i*Mathf.PI/20,b=(i+1)*Mathf.PI/20;var x=new Vector2(Mathf.Cos(a),Mathf.Sin(a));var y=new Vector2(Mathf.Cos(b),Mathf.Sin(b));Quad(h,r,p+x*radius,p+y*radius,p+y*(radius+width),p+x*(radius+width),c);}}
        void Vertex(VertexHelper h,Rect r,Vector2 p,Color c)=>h.AddVert(board&&board.RuleVersion>=GameplayRuleVersion.PrototypeR4?(Vector3)p:new Vector3(Mathf.Clamp(p.x,r.xMin,r.xMax),Mathf.Clamp(p.y,r.yMin,r.yMax)),c,Vector2.zero);
        void Triangle(VertexHelper h,Rect r,Vector2 a,Vector2 b,Vector2 c,Color color){int n=h.currentVertCount;Vertex(h,r,a,color);Vertex(h,r,b,color);Vertex(h,r,c,color);h.AddTriangle(n,n+1,n+2);}
        void Quad(VertexHelper h,Rect r,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color color){int n=h.currentVertCount;Vertex(h,r,a,color);Vertex(h,r,b,color);Vertex(h,r,c,color);Vertex(h,r,d,color);h.AddTriangle(n,n+1,n+2);h.AddTriangle(n,n+2,n+3);}
    }
}
