using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Draws an opaque navy inset inside the World Map metalwork; isolated emblems render separately.</summary>
    [AddComponentMenu(""), RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionFooterSymbolGraphic : MaskableGraphic
    {
        public Sprite Source;
        // Source pixel coordinates measured from the top left, excluding its old frame.
        public Rect SourceTopRect;
        public override Texture mainTexture => Source ? Source.texture : Texture2D.whiteTexture;

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (!Source) return;
            var rect = GetPixelAdjustedRect();
            var uv = SourceTopRect;
            var texture = Source.texture;
            const int segments = 96;
            AddVertex(mesh, rect, uv, texture, new Vector2(.5f, .5f));
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;
                // Circular opening continues behind the caption; clip at the caption rail.
                float x = .5f + .5f * Mathf.Cos(angle);
                float y = Mathf.Clamp01((72 + 72 * Mathf.Sin(angle)) / 130f);
                AddVertex(mesh, rect, uv, texture, new Vector2(x, y));
                if (i > 0) mesh.AddTriangle(0, i, i + 1);
            }
        }

        void AddVertex(VertexHelper mesh, Rect rect, Rect source, Texture texture, Vector2 point)
        {
            mesh.AddVert(new Vector3(rect.xMin + rect.width * point.x, rect.yMax - rect.height * point.y), color,
                new Vector2((Source.rect.x + source.x + source.width * point.x) / texture.width,
                    (Source.rect.yMax - source.y - source.height * point.y) / texture.height));
        }
    }
}
