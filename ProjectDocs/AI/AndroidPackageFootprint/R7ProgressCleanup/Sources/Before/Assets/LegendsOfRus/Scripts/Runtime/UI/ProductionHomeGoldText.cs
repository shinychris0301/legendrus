using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Home's live gold lettering; retains the font mesh and existing shadow.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionHomeGoldText : BaseMeshEffect
    {
        public Color32 TopColor = new Color32(255, 231, 164, 255);
        public Color32 BottomColor = new Color32(176, 105, 26, 255);
        // Only Play opts into the matching material. UV0.zw carries an inward
        // bevel sample direction; badges retain their accepted default mesh.
        public bool BeveledFace;
        readonly List<UIVertex> bevelVertices = new List<UIVertex>();

        public override void ModifyMesh(VertexHelper vertices)
        {
            if (!IsActive() || vertices.currentVertCount == 0) return;
            Color32 foreground = graphic.color;
            if (BeveledFace) AddBevelCoordinates(vertices, foreground);
            float low = float.PositiveInfinity, high = float.NegativeInfinity;
            UIVertex vertex = default;
            for (int i = 0; i < vertices.currentVertCount; i++)
            {
                vertices.PopulateUIVertex(ref vertex, i);
                if (!IsForeground(vertex.color, foreground)) continue;
                low = Mathf.Min(low, vertex.position.y);
                high = Mathf.Max(high, vertex.position.y);
            }
            if (high - low < .001f || float.IsInfinity(low)) return;

            for (int i = 0; i < vertices.currentVertCount; i++)
            {
                vertices.PopulateUIVertex(ref vertex, i);
                // Shadow may already have expanded the mesh into a triangle stream.
                // Its dark copies retain their authored color and alpha, in either order.
                if (!IsForeground(vertex.color, foreground)) continue;
                Color32 gold = Color32.Lerp(BottomColor, TopColor,
                    Mathf.InverseLerp(low, high, vertex.position.y));
                gold.a = vertex.color.a;
                vertex.color = gold;
                vertices.SetUIVertex(vertex, i);
            }
        }

        void AddBevelCoordinates(VertexHelper vertices, Color32 foreground)
        {
            // Bounds travel in UV1 so a wider bevel cannot sample a neighboring
            // character after Unity repacks the dynamic font atlas.
            if (graphic.canvas != null &&
                (graphic.canvas.additionalShaderChannels & AdditionalCanvasShaderChannels.TexCoord1) == 0)
                graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            bevelVertices.Clear();
            vertices.GetUIVertexStream(bevelVertices);
            for (int start = 0; start + 2 < bevelVertices.Count; start += 3)
            {
                UIVertex a = bevelVertices[start], b = bevelVertices[start + 1], c = bevelVertices[start + 2];
                Vector2 p = b.position - a.position, q = c.position - a.position;
                Vector2 u = b.uv0 - a.uv0, v = c.uv0 - a.uv0;
                float determinant = p.x * q.y - p.y * q.x;
                Vector2 sample = Vector2.zero;
                Vector2 lowUv = Vector2.Min(a.uv0, Vector2.Min(b.uv0, c.uv0));
                Vector2 highUv = Vector2.Max(a.uv0, Vector2.Max(b.uv0, c.uv0));
                if (IsForeground(a.color, foreground) && Mathf.Abs(determinant) > .001f)
                {
                    // Map a 3.5-local-pixel upper-left step through each actual
                    // glyph triangle. This also handles rotated dynamic atlas glyphs.
                    Vector2 dx = (u * q.y - v * p.y) / determinant;
                    Vector2 dy = (v * p.x - u * q.x) / determinant;
                    sample = -3.5f * dx + 3.5f * dy;
                }
                for (int i = start; i < start + 3; i++)
                {
                    UIVertex vertex = bevelVertices[i];
                    vertex.uv0.z = sample.x; vertex.uv0.w = sample.y;
                    vertex.uv1 = new Vector4(lowUv.x, lowUv.y, highUv.x, highUv.y);
                    bevelVertices[i] = vertex;
                }
            }
            vertices.Clear();
            vertices.AddUIVertexTriangleStream(bevelVertices);
        }

        static bool IsForeground(Color32 color, Color32 foreground) =>
            color.a > 0 && color.r == foreground.r && color.g == foreground.g && color.b == foreground.b;
    }
}
