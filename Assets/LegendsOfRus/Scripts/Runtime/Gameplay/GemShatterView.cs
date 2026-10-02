using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace LegendsOfRus
{
    // Six persistent texture batches. Pieces retain the approved gem's actual facets.
    // No spawned objects, physics bodies or gameplay RNG during a clear.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class GemShatterView : MaskableGraphic
    {
        struct Break { public BoardPosition Cell; public float Born; }
        const int Capacity = 16;
        float Lifetime => UsesArtwork ? .62f : .46f;
        readonly List<Break> breaks = new List<Break>(Capacity);
        static readonly Vector2[] Edge = {
            new Vector2(-.5f,-.5f), new Vector2(0,-.5f), new Vector2(.5f,-.5f),
            new Vector2(.5f,0), new Vector2(.5f,.5f), new Vector2(0,.5f),
            new Vector2(-.5f,.5f), new Vector2(-.5f,0)
        };
        static readonly Vector2[] FacetEdge = {
            new Vector2(-.5f,-.5f),new Vector2(-.12f,-.5f),new Vector2(.5f,-.5f),new Vector2(.5f,-.08f),
            new Vector2(.5f,.5f),new Vector2(.16f,.5f),new Vector2(-.5f,.5f),new Vector2(-.5f,.21f)
        };
        Match3Board board;
        Sprite sprite;
        int tileType;
        public bool UsesArtwork => board && board.RuleVersion>=GameplayRuleVersion.PrototypeR4 && MechanicsSequenceArt.Available;
        public int FragmentRegion => MechanicsSequenceArt.GemRegion(tileType);
        public int ActivePieces => breaks.Count * (UsesArtwork?24:8);
        public override Texture mainTexture => sprite ? sprite.texture : base.mainTexture;

        public static void Attach(Match3Board board)
        {
            var root = new GameObject("GemFragmentBatches", typeof(RectTransform), typeof(RectMask2D));
            root.transform.SetParent(board.transform, false);
            Stretch((RectTransform)root.transform);
            for (int i = 0; i < 6; i++)
            {
                var go = new GameObject("GemFragments" + i, typeof(RectTransform));
                go.transform.SetParent(root.transform, false);
                Stretch((RectTransform)go.transform);
                var view = go.AddComponent<GemShatterView>();
                view.board = board; view.tileType = i; view.sprite = ProductionGameplayArt.Gem(i);
                view.raycastTarget = false;
                board.MotionEventPublished += view.Motion;
            }
        }
        static void Stretch(RectTransform r)
        { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        void Motion(GameplayMotionEvent e)
        {
            if (e.Kind == GameplayMotionKind.CommandRecovered) { breaks.Clear(); SetVerticesDirty(); return; }
            if (!isActiveAndEnabled) return;
            if (e.Kind != (board.RuleVersion>=GameplayRuleVersion.PrototypeR4?GameplayMotionKind.CellsImpacted:GameplayMotionKind.MatchWaveStarted)) return;
            foreach (var cell in e.ClearedCells)
            {
                if (cell.Cell.TileType != tileType || (board.RuleVersion>=GameplayRuleVersion.PrototypeR4&&cell.Cell.Special!=SpecialGemKind.None)) continue;
                if (breaks.Count == Capacity) breaks.RemoveAt(0);
                breaks.Add(new Break { Cell = cell.Position, Born = Time.unscaledTime });
            }
            SetVerticesDirty();
        }
        void Update()
        {
            if (breaks.RemoveAll(b => Time.unscaledTime - b.Born >= Lifetime) > 0 || breaks.Count > 0)
                SetVerticesDirty();
        }
        protected override void OnDisable() { breaks.Clear(); SetVerticesDirty(); base.OnDisable(); }
        protected override void OnDestroy()
        { if (board) board.MotionEventPublished -= Motion; base.OnDestroy(); }
        protected override void OnPopulateMesh(VertexHelper h)
        {
            h.Clear(); if (!board || !sprite) return;
            Rect r = rectTransform.rect;
            float w = r.width / board.ColumnCount, height = r.height / board.RowCount;
            float unit = Mathf.Min(w, height), aspect = sprite.rect.width / sprite.rect.height;
            bool redesigned=board.RuleVersion>=GameplayRuleVersion.PrototypeR4;
            Vector2 size = new Vector2(Mathf.Min(1, aspect), Mathf.Min(1, 1 / aspect)) * unit * (redesigned?.98f:.88f);
            Vector4 uv = DataUtility.GetOuterUV(sprite);
            foreach (var b in breaks)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - b.Born) / Lifetime);
                Vector2 center = new Vector2(r.xMin + (b.Cell.X + .5f) * w, r.yMax - (b.Cell.Y + .5f) * height);
                if(UsesArtwork)
                {
                    // Eight irregular wedges retain the source facets; chips add a faster, finer scatter.
                    float alpha=1-Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.34f)/.66f));
                    float seed=b.Cell.X*.71f+b.Cell.Y*1.13f;
                    for(int i=0;i<8;i++)
                    {
                        Vector2 a=FacetEdge[i],c=FacetEdge[(i+1)%8],centroid=(a+c)/3;
                        float angle=Mathf.Atan2(centroid.y,centroid.x)+.32f*Mathf.Sin(seed+i*2.4f);
                        Vector2 travel=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*unit*(.85f+(i%3)*.29f)*(1-Mathf.Pow(1-t,3));
                        travel+=Vector2.up*unit*.3f*Mathf.Sin(t*Mathf.PI)+Vector2.down*unit*1.1f*t*t;
                        float spin=(i%2==0?1:-1)*t*(2.2f+i*.25f);int n=h.currentVertCount;
                        var chunkTint=Color.Lerp(Color.white,new Color(.70f,.79f,.92f),.18f*(i%3));chunkTint.a=alpha;
                        Add(h,center+travel,Vector2.zero,centroid,size,spin,uv,chunkTint,t*.18f,true);
                        Add(h,center+travel,a,centroid,size,spin,uv,chunkTint,t*.18f,true);
                        Add(h,center+travel,c,centroid,size,spin,uv,chunkTint,t*.18f,true);
                        h.AddTriangle(n,n+1,n+2);
                    }
                    float age=Time.unscaledTime-b.Born;
                    for(int i=0;i<16;i++)
                    {
                        float life=ChipLifetime(i),progress=age/life;if(progress>=1)continue;
                        float chipAlpha=1-Mathf.SmoothStep(0,1,Mathf.Clamp01((progress-.25f)/.75f));
                        Vector2 q=center+ChipOffset(b.Cell,i,unit,age);
                        float radius=unit*(.028f+(i%4)*.008f)*(1-.4f*progress),angle=seed+i*2.4f+age*(i%2==0?9:-7);
                        Vector2 x=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius,y=new Vector2(-x.y,x.x)*1.45f;
                        // Sample the opaque inner facets, avoiding transparent sprite corners.
                        float ux=.40f+(i%3)*.10f,uy=.40f+(i%2)*.14f;
                        Vector2 tex=new Vector2(Mathf.Lerp(uv.x,uv.z,ux),Mathf.Lerp(uv.y,uv.w,uy));
                        Vector2 du=new Vector2((uv.z-uv.x)*.08f,0),dv=new Vector2(0,(uv.w-uv.y)*.08f);
                        var chipTint=new Color(1,1,1,chipAlpha);int n=h.currentVertCount;
                        h.AddVert(q-x,chipTint,tex-du);h.AddVert(q+y,chipTint,tex+dv);h.AddVert(q+x,chipTint,tex+du);h.AddVert(q-y,chipTint,tex-dv);
                        h.AddTriangle(n,n+1,n+2);h.AddTriangle(n,n+2,n+3);
                    }
                    continue;
                }
                Color tint = Color.Lerp(new Color(1, 1, .8f), Color.white, Mathf.Clamp01(t * 5));
                tint.a = 1 - Mathf.SmoothStep(0, 1, Mathf.Clamp01((t - .18f) / .82f));
                bool faceted=board.RuleVersion>=GameplayRuleVersion.PrototypeR4;
                var edges=faceted?FacetEdge:Edge;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 a = edges[i], c = edges[(i + 1) % 8], centroid = (a + c) / 3;
                    Vector2 travel = centroid.normalized * unit * (.48f + (i % 3) * .10f) * (1 - Mathf.Pow(1 - t, 2));
                    travel += Vector2.down * unit * .65f * t * t;
                    float angle = t * (faceted?(i%2==0?2.5f+i*.15f:-2.8f-i*.12f):(i % 2 == 0 ? 2.4f : -2.4f));
                    if(faceted)travel=centroid.normalized*unit*(1.12f+(i%3)*.34f)*(1-Mathf.Pow(1-t,3))+Vector2.up*unit*.40f*Mathf.Sin(t*Mathf.PI)+Vector2.down*unit*1.2f*t*t;
                    var facetTint=faceted?Color.Lerp(tint,new Color(.72f,.8f,.9f,tint.a),(i%3)*.065f):tint;
                    int n = h.currentVertCount;
                    Add(h, center + travel, Vector2.zero, centroid, size, angle, uv, facetTint, faceted?t*.25f:t,faceted);
                    Add(h, center + travel, a, centroid, size, angle, uv, facetTint, faceted?t*.25f:t,faceted);
                    Add(h, center + travel, c, centroid, size, angle, uv, facetTint, faceted?t*.25f:t,faceted);
                    h.AddTriangle(n, n + 1, n + 2);
                }
            }
        }
        // Shared by textured chips and the additive glints so the light follows material.
        public static float ChipLifetime(int index) => .38f+(index%4)*.055f;
        public static Vector2 ChipOffset(BoardPosition cell,int index,float unit,float age)
        {
            float t=Mathf.Clamp01(age/ChipLifetime(index));
            float a=index*2.39996f+cell.X*.71f+cell.Y*1.13f;
            float reach=unit*(.62f+(index%5)*.24f)*(1-Mathf.Pow(1-t,3));
            return new Vector2(Mathf.Cos(a),Mathf.Sin(a))*reach+Vector2.down*unit*.72f*t*t;
        }
        static void Add(VertexHelper h, Vector2 center, Vector2 point, Vector2 centroid,
            Vector2 size, float angle, Vector4 uv, Color tint, float t,bool tumble)
        {
            Vector2 local = Vector2.Scale(point - centroid, size) * (1 - .5f * t);
            float cos = Mathf.Cos(angle), sin = Mathf.Sin(angle);
            if(tumble)local.x *= Mathf.Lerp(1,.40f,Mathf.Abs(Mathf.Sin(angle*1.7f)));
            local = new Vector2(local.x * cos - local.y * sin, local.x * sin + local.y * cos);
            Vector2 position = center + Vector2.Scale(centroid, size) + local;
            h.AddVert(position, tint, new Vector2(Mathf.Lerp(uv.x, uv.z, point.x + .5f), Mathf.Lerp(uv.y, uv.w, point.y + .5f)));
        }
    }
}
