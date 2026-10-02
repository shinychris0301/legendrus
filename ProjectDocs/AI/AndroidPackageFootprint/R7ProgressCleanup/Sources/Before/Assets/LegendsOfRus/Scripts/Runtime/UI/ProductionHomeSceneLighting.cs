using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Home-only light around an unchanged sprite, with its original mesh/fade above it.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class ProductionHomeSceneLighting : BaseMeshEffect
    {
        [Range(0f, 1f)] public float Strength = 1f;
        [Range(0f, .1f)] public float Padding = .03f;
        readonly List<UIVertex> surface = new List<UIVertex>();
        readonly List<UIVertex> combined = new List<UIVertex>();

        public override void ModifyMesh(VertexHelper vertices)
        {
            if (!IsActive() || vertices.currentVertCount == 0 || Strength <= 0) return;
            if (graphic.canvas != null &&
                (graphic.canvas.additionalShaderChannels & AdditionalCanvasShaderChannels.TexCoord1) == 0)
                graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            surface.Clear(); vertices.GetUIVertexStream(surface);
            Vector2 low = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 high = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            Vector2 lowUv = Vector2.one, highUv = Vector2.zero;
            foreach (UIVertex vertex in surface)
            {
                low = Vector2.Min(low, vertex.position); high = Vector2.Max(high, vertex.position);
                lowUv = Vector2.Min(lowUv, vertex.uv0); highUv = Vector2.Max(highUv, vertex.uv0);
            }
            Vector2 padding = (high - low) * Padding, uvPadding = (highUv - lowUv) * Padding;
            combined.Clear();
            UIVertex bl = GlowVertex(low - padding, lowUv - uvPadding);
            UIVertex tl = GlowVertex(new Vector2(low.x - padding.x, high.y + padding.y),
                new Vector2(lowUv.x - uvPadding.x, highUv.y + uvPadding.y));
            UIVertex tr = GlowVertex(high + padding, highUv + uvPadding);
            UIVertex br = GlowVertex(new Vector2(high.x + padding.x, low.y - padding.y),
                new Vector2(highUv.x + uvPadding.x, lowUv.y - uvPadding.y));
            // Render light first, behind the original sprite. No extra Image, hitbox or layout rectangle.
            combined.Add(bl); combined.Add(tl); combined.Add(tr);
            combined.Add(tr); combined.Add(br); combined.Add(bl);
            foreach (UIVertex input in surface)
            {
                UIVertex vertex = input;
                vertex.uv1 = new Vector4(Strength, 0, 0, 0);
                combined.Add(vertex);
            }
            vertices.Clear(); vertices.AddUIVertexTriangleStream(combined);
        }

        UIVertex GlowVertex(Vector2 position, Vector2 uv)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.position = new Vector3(position.x, position.y, 0);
            vertex.uv0 = uv;
            vertex.uv1 = new Vector4(Strength, 0, 1, 0);
            vertex.color = graphic.color;
            return vertex;
        }
    }
}
