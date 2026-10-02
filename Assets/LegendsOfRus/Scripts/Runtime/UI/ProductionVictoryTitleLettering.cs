using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Victory's single-line, live gold title. Runs after SplashTitleTextGeometry
    /// on the original Text quads; disable Shadow and ProductionHomeGoldText on
    /// this label because this effect supplies its own depth and bevel data.
    /// </summary>
    [RequireComponent(typeof(Text)), DisallowMultipleComponent]
    public sealed class ProductionVictoryTitleLettering : BaseMeshEffect
    {
        [Tooltip("Center rise in the parent's reference units; compensates for Text's raster-density scale.")]
        [Min(0)] public float ArcHeight = 32f;
        [Tooltip("Bevel width in reference units. The existing HomePlayGold material uses this mesh's UV0.zw and UV1.")]
        [Min(0)] public float BevelWidth = 1.75f;
        public Color32 BottomColor = new Color32(191, 104, 10, 255);
        public Color32 BodyColor = new Color32(247, 166, 30, 255);
        public Color32 HighlightColor = new Color32(255, 211, 87, 255);
        public Color32 TopColor = new Color32(255, 185, 49, 255);
        public Color32 ExtrusionColor = new Color32(123, 53, 5, 255);
        public Color32 ShadowColor = new Color32(43, 13, 2, 225);
        public Vector2 ExtrusionDistance = new Vector2(1.1f, -1.4f);
        public Vector2 ShadowDistance = new Vector2(2.6f, -4f);

        const int FaceBands = 4;
        readonly List<Glyph> glyphs = new List<Glyph>(16);
        readonly UIVertex[] quad = new UIVertex[4];

        struct Glyph
        {
            public UIVertex A, B, C, D;
            public float CenterX, Baseline, Sine, Cosine, Rise;
            public Vector4 AtlasBounds;
            public Vector2 BevelStep;
        }

        /// <summary>Call after changing public style fields at runtime; no per-frame rebuild is needed.</summary>
        public void Refresh() { if (graphic != null) graphic.SetVerticesDirty(); }

        public override void ModifyMesh(VertexHelper vertices)
        {
            Text label = graphic as Text;
            if (!IsActive() || label == null || string.IsNullOrEmpty(label.text) ||
                label.text.IndexOf('\n') >= 0 || label.text.IndexOf('\r') >= 0 ||
                vertices.currentVertCount < 4 || vertices.currentVertCount % 4 != 0 ||
                vertices.currentIndexCount != vertices.currentVertCount / 4 * 6)
                return;

            // A single title should never approach uGUI's 65k vertex limit.
            // Leave an unexpectedly long localized value intact instead of overflowing it.
            if (vertices.currentVertCount > 65000 / (FaceBands + 2)) return;
            glyphs.Clear();
            Vector2 low = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 high = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            for (int start = 0; start < vertices.currentVertCount; start += 4)
            {
                Vector2 glyphLow = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                Vector2 glyphHigh = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
                for (int i = 0; i < 4; i++)
                {
                    vertices.PopulateUIVertex(ref quad[i], start + i);
                    glyphLow = Vector2.Min(glyphLow, quad[i].position);
                    glyphHigh = Vector2.Max(glyphHigh, quad[i].position);
                }
                if (glyphHigh.x - glyphLow.x < .001f || glyphHigh.y - glyphLow.y < .001f)
                    continue; // Whitespace must not change the visible arc span.
                glyphs.Add(new Glyph { A = quad[0], B = quad[1], C = quad[2], D = quad[3],
                    CenterX = (glyphLow.x + glyphHigh.x) * .5f });
                low = Vector2.Min(low, glyphLow);
                high = Vector2.Max(high, glyphHigh);
            }
            if (glyphs.Count == 0) return;

            // The existing labels render at twice their reference size and scale
            // down by .5. Canvas/layout scaling remains entirely with the owner.
            Vector3 scale = label.rectTransform.localScale;
            Vector2 toLocal = new Vector2(1f / Mathf.Max(.001f, Mathf.Abs(scale.x)),
                1f / Mathf.Max(.001f, Mathf.Abs(scale.y)));
            float rise = Mathf.Max(0, ArcHeight) * toLocal.y;
            float center = (low.x + high.x) * .5f;
            float halfWidth = (high.x - low.x) * .5f;
            Vector2 bevel = toLocal * Mathf.Max(0, BevelWidth);
            for (int i = 0; i < glyphs.Count; i++)
            {
                Glyph glyph = glyphs[i];
                float x = (glyph.CenterX - center) / halfWidth;
                float angle = Mathf.Atan(-2f * rise * x / halfWidth);
                glyph.Baseline = low.y;
                glyph.Rise = rise * (1f - x * x);
                glyph.Sine = Mathf.Sin(angle);
                glyph.Cosine = Mathf.Cos(angle);
                Vector2 uvLow = Vector2.Min(Vector2.Min(glyph.A.uv0, glyph.B.uv0),
                    Vector2.Min(glyph.C.uv0, glyph.D.uv0));
                Vector2 uvHigh = Vector2.Max(Vector2.Max(glyph.A.uv0, glyph.B.uv0),
                    Vector2.Max(glyph.C.uv0, glyph.D.uv0));
                glyph.AtlasBounds = new Vector4(uvLow.x, uvLow.y, uvHigh.x, uvHigh.y);
                glyph.BevelStep = BevelCoordinates(glyph, bevel);
                glyphs[i] = glyph;
            }

            if (label.canvas != null &&
                (label.canvas.additionalShaderChannels & AdditionalCanvasShaderChannels.TexCoord1) == 0)
                label.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;

            vertices.Clear();
            // Depth is mesh from the same live glyphs, with a zero bevel step;
            // neither layer can sample or light neighboring font-atlas entries.
            AddDepth(vertices, ShadowColor, Vector2.Scale(ShadowDistance, toLocal));
            AddDepth(vertices, ExtrusionColor, Vector2.Scale(ExtrusionDistance, toLocal));
            foreach (Glyph glyph in glyphs)
            {
                for (int band = 0; band < FaceBands; band++)
                {
                    float first = (float)band / FaceBands, second = (float)(band + 1) / FaceBands;
                    quad[0] = Interpolate(glyph.A, glyph.D, first);
                    quad[1] = Interpolate(glyph.B, glyph.C, first);
                    quad[2] = Interpolate(glyph.B, glyph.C, second);
                    quad[3] = Interpolate(glyph.A, glyph.D, second);
                    for (int i = 0; i < 4; i++)
                    {
                        UIVertex vertex = quad[i];
                        Color32 color = FaceColor(Mathf.InverseLerp(low.y, high.y, vertex.position.y));
                        color.a = (byte)(color.a * vertex.color.a / 255);
                        vertex.color = color;
                        vertex.position = Curve(vertex.position, glyph);
                        vertex.uv0.z = glyph.BevelStep.x;
                        vertex.uv0.w = glyph.BevelStep.y;
                        vertex.uv1 = glyph.AtlasBounds;
                        quad[i] = vertex;
                    }
                    vertices.AddUIVertexQuad(quad);
                }
            }
        }

        void AddDepth(VertexHelper vertices, Color32 color, Vector2 offset)
        {
            foreach (Glyph glyph in glyphs)
            {
                quad[0] = glyph.A; quad[1] = glyph.B; quad[2] = glyph.C; quad[3] = glyph.D;
                for (int i = 0; i < 4; i++)
                {
                    UIVertex vertex = quad[i];
                    Color32 tint = color;
                    tint.a = (byte)(tint.a * vertex.color.a / 255);
                    vertex.color = tint;
                    vertex.position = Curve(vertex.position, glyph) + (Vector3)offset;
                    vertex.uv0.z = vertex.uv0.w = 0;
                    vertex.uv1 = glyph.AtlasBounds;
                    quad[i] = vertex;
                }
                vertices.AddUIVertexQuad(quad);
            }
        }

        Color32 FaceColor(float t)
        {
            if (t < .25f) return Color32.Lerp(BottomColor, BodyColor, t * 4f);
            if (t < .75f) return Color32.Lerp(BodyColor, HighlightColor, (t - .25f) * 2f);
            return Color32.Lerp(HighlightColor, TopColor, (t - .75f) * 4f);
        }

        static Vector3 Curve(Vector3 point, Glyph glyph)
        {
            // Rotate the entire glyph about its baseline, then raise that pivot
            // onto the parabola. Every glyph stays rigid, including its UVs.
            float x = point.x - glyph.CenterX, y = point.y - glyph.Baseline;
            return new Vector3(glyph.CenterX + x * glyph.Cosine - y * glyph.Sine,
                glyph.Baseline + glyph.Rise + x * glyph.Sine + y * glyph.Cosine, point.z);
        }

        static Vector2 BevelCoordinates(Glyph glyph, Vector2 bevel)
        {
            Vector2 a = Curve(glyph.A.position, glyph);
            Vector2 p = (Vector2)Curve(glyph.B.position, glyph) - a;
            Vector2 q = (Vector2)Curve(glyph.C.position, glyph) - a;
            Vector2 u = glyph.B.uv0 - glyph.A.uv0, v = glyph.C.uv0 - glyph.A.uv0;
            float determinant = p.x * q.y - p.y * q.x;
            if (Mathf.Abs(determinant) < .001f) return Vector2.zero;
            // Compute the fixed upper-left light direction AFTER rotation. UV
            // bounds cover the whole glyph, not each face strip, and remain safe
            // when Unity repacks or rotates entries in the dynamic font atlas.
            Vector2 dx = (u * q.y - v * p.y) / determinant;
            Vector2 dy = (v * p.x - u * q.x) / determinant;
            return -bevel.x * dx + bevel.y * dy;
        }

        static UIVertex Interpolate(UIVertex a, UIVertex b, float t)
        {
            UIVertex result = a;
            result.position = Vector3.LerpUnclamped(a.position, b.position, t);
            result.uv0 = Vector4.LerpUnclamped(a.uv0, b.uv0, t);
            result.color = Color32.Lerp(a.color, b.color, t);
            return result;
        }
    }
}
