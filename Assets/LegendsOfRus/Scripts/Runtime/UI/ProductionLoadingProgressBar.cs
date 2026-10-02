using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Native rounded lacquer/gold track with a sapphire fill; no baked progress artwork.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionLoadingProgressBar : MaskableGraphic
    {
        [SerializeField, Range(0f, 1f)] float progress;
        [SerializeField] bool indeterminate = true;
        public Sprite SourceFill;
        public override Texture mainTexture => SourceFill != null ? SourceFill.texture : base.mainTexture;

        public float Progress => progress;
        public bool IsIndeterminate => indeterminate;

        public void SetProgress(float value, bool unknownDuration)
        {
            progress = Mathf.Clamp01(value);
            indeterminate = unknownDuration;
            SetVerticesDirty();
        }

        void Update()
        {
            // Movement communicates unknown duration only. It never sets measured progress.
            if (indeterminate && Application.isPlaying) SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r = GetPixelAdjustedRect();
            if (SourceFill != null)
            {
                float width = r.width * (indeterminate ? .28f : progress);
                float start = indeterminate ? (r.width-width) * (Application.isPlaying ? .5f+.5f*Mathf.Sin(Time.unscaledTime*2f) : .5f) : 0;
                if (width <= .001f) return;
                Vector4 uv = UnityEngine.Sprites.DataUtility.GetOuterUV(SourceFill);
                vh.AddVert(new Vector3(r.xMin+start,r.yMin),Color.white,new Vector2(uv.x,uv.y));
                vh.AddVert(new Vector3(r.xMin+start,r.yMax),Color.white,new Vector2(uv.x,uv.w));
                vh.AddVert(new Vector3(r.xMin+start+width,r.yMax),Color.white,new Vector2(Mathf.Lerp(uv.x,uv.z,width/r.width),uv.w));
                vh.AddVert(new Vector3(r.xMin+start+width,r.yMin),Color.white,new Vector2(Mathf.Lerp(uv.x,uv.z,width/r.width),uv.y));
                vh.AddTriangle(0,1,2);vh.AddTriangle(2,3,0);
                return;
            }
            DrawCapsule(vh, r, new Color32(155, 115, 58, 255), new Color32(180, 140, 70, 255));
            Rect track = Inset(r, 2f);
            DrawCapsule(vh, track, new Color32(23, 41, 61, 255), new Color32(8, 15, 26, 255));
            Rect fill = Inset(r, 6f);
            if (indeterminate)
            {
                float segmentWidth = fill.width * .36f;
                float phase = Application.isPlaying ? .5f + .5f * Mathf.Sin(Time.unscaledTime * 2f) : .5f;
                fill.x += (fill.width - segmentWidth) * phase;
                fill.width = segmentWidth;
            }
            else fill.width *= progress;
            if (fill.width > .01f)
                DrawCapsule(vh, fill, new Color32(26, 69, 108, 255), new Color32(72, 137, 181, 255));
        }

        static Rect Inset(Rect r, float amount)
        {
            return new Rect(r.x + amount, r.y + amount,
                Mathf.Max(0f, r.width - amount * 2f), Mathf.Max(0f, r.height - amount * 2f));
        }

        static void DrawCapsule(VertexHelper vh, Rect r, Color bottom, Color top)
        {
            if (r.width <= 0f || r.height <= 0f) return;
            const int segments = 12;
            int first = vh.currentVertCount;
            float radius = Mathf.Min(r.height, r.width) * .5f;
            vh.AddVert(r.center, Color.Lerp(bottom, top, .5f), Vector2.zero);
            for (int corner = 0; corner < 4; corner++)
            {
                Vector2 center = new Vector2(corner < 2 ? r.xMax - radius : r.xMin + radius,
                    corner == 0 || corner == 3 ? r.yMax - radius : r.yMin + radius);
                for (int i = 0; i <= segments; i++)
                {
                    float angle = (90f - corner * 90f - i * 90f / segments) * Mathf.Deg2Rad;
                    Vector2 p = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                    vh.AddVert(p, Color.Lerp(bottom, top, (p.y - r.yMin) / r.height), Vector2.zero);
                }
            }
            int edgeCount = 4 * (segments + 1);
            for (int i = 0; i < edgeCount; i++)
                vh.AddTriangle(first, first + 1 + i, first + 1 + (i + 1) % edgeCount);
        }
    }
}
