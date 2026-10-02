using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus
{
    // Articulates the approved sprite itself; the final pose is exactly the idle Image.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SpecialCreationGraphic : MaskableGraphic
    {
        Sprite art; RectTransform target; bool settling; float born,settleStart;
        CreationAccentsGraphic accents;
        public const float GatherSeconds=.20f,SettleSeconds=.24f;
        public bool Active {get;private set;}
        public bool Settling=>Active&&settling;
        public bool FullyAssembled=>Settling&&Time.unscaledTime-born>=.16f;
        public Sprite Artwork=>art;
        public SpecialGemKind Kind {get;private set;}
        public RectTransform Anchor=>target;
        public float SettleEntryProgress=>settleStart;
        public float Age=>Time.unscaledTime-born;
        public float Progress=>settling?Mathf.Lerp(settleStart,1,Mathf.SmoothStep(0,1,Age/.16f)):GatherProgress(Age);
        public override Texture mainTexture=>art?art.texture:base.mainTexture;
        public void Begin(Sprite sprite,RectTransform anchor,SpecialGemKind kind)
        {
            art=sprite;target=anchor;Kind=kind;settling=false;born=Time.unscaledTime;Active=true;raycastTarget=false;
            if(!accents)
            {
                var go=new GameObject("CreationAccents",typeof(RectTransform),typeof(CreationAccentsGraphic));
                go.transform.SetParent(transform.parent,false);var rt=(RectTransform)go.transform;
                rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=rt.offsetMax=Vector2.zero;
                go.transform.SetSiblingIndex(transform.GetSiblingIndex());accents=go.GetComponent<CreationAccentsGraphic>();accents.Bind(this);
            }
            SetMaterialDirty();SetVerticesDirty();
        }
        static float GatherProgress(float age)=>age<GatherSeconds?Mathf.SmoothStep(0,1,age/GatherSeconds)*.65f:.65f+.19f*(1-Mathf.Exp(-(age-GatherSeconds)*9));
        public void Settle(){settleStart=Progress;settling=true;born=Time.unscaledTime;SetVerticesDirty();}
        public void Finish(){Active=false;SetVerticesDirty();if(accents)accents.Clear();}
        void Update(){if(Active){if(Age>8)Finish();else SetVerticesDirty();}}
        protected override void OnDisable(){Finish();base.OnDisable();}
        protected override void OnDestroy(){if(accents)Destroy(accents.gameObject);base.OnDestroy();}
        protected override void OnPopulateMesh(VertexHelper h)
        {
            h.Clear();if(!Active||!art||!target)return;
            Vector2 size=target.rect.size*1.08f;float aspect=art.rect.width/art.rect.height;
            if(size.x/size.y>aspect)size.x=size.y*aspect;else size.y=size.x/aspect;
            var uv=DataUtility.GetOuterUV(art);float progress=Progress;
            // Whole-art hold: no residual sliced edges or scale discontinuity at handoff.
            if(FullyAssembled){Quad(h,-.5f,-.5f,.5f,.5f,size,uv,progress);return;}
            Quad(h,-.5f,-.5f,.5f,.5f,size,uv,progress);
        }
        void Quad(VertexHelper h,float x0,float y0,float x1,float y1,Vector2 size,Vector4 uv,float progress,int part=0)
        {
            int n=h.currentVertCount;Add(h,new Vector2(x0,y0),size,uv,progress,part);Add(h,new Vector2(x0,y1),size,uv,progress,part);Add(h,new Vector2(x1,y1),size,uv,progress,part);Add(h,new Vector2(x1,y0),size,uv,progress,part);h.AddTriangle(n,n+1,n+2);h.AddTriangle(n,n+2,n+3);
        }
        void Add(VertexHelper h,Vector2 source,Vector2 size,Vector4 uv,float progress,int part)
        {
            Vector2 p=source;float r=1-progress;
            float pulse=settling?Mathf.Sin(Mathf.Clamp01(Age/.16f)*Mathf.PI):0;
            if(!FullyAssembled)
            {
                float reveal=Mathf.SmoothStep(0,1,Mathf.Clamp01((progress-.22f)/.65f));
                float scale=settling?1+.22f*Mathf.Sin(Mathf.Clamp01(Age/.16f)*Mathf.PI):Mathf.Lerp(.18f,1,reveal);
                p*=scale;
                float tilt=Kind==SpecialGemKind.Spark?-.16f:Kind==SpecialGemKind.Burst?.10f:.06f;
                float angle=tilt*(1-reveal),c=Mathf.Cos(angle),s=Mathf.Sin(angle);
                p=new Vector2(p.x*c-p.y*s,p.x*s+p.y*c);
            }
            p=Vector2.Scale(p,size);if(Kind==SpecialGemKind.Column)p=new Vector2(-p.y,p.x);
            var local=rectTransform.InverseTransformPoint(target.TransformPoint(p));
            h.AddVert(local,new Color(1,1,1,Mathf.SmoothStep(0,1,progress/.30f)),new Vector2(Mathf.Lerp(uv.x,uv.z,source.x+.5f),Mathf.Lerp(uv.y,uv.w,source.y+.5f)));
        }
    }
}
