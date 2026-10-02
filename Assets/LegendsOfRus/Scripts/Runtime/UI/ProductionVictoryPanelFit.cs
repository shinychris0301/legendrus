using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Fits Victory's existing isolated wood surfaces without stretching
    /// their corner carvings or central jewels. Only plain connecting wood varies.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class ProductionVictoryPanelFit : BaseMeshEffect
    {
        public bool RewardCard;

        public override void ModifyMesh(VertexHelper vh)
        {
            var image = graphic as Image;
            if (!IsActive() || image == null || image.sprite == null) return;
            Rect rect = image.GetPixelAdjustedRect();
            float sourceW = RewardCard ? 1050 : 1942;
            float sourceH = RewardCard ? 1498 : 809;
            float scale = Mathf.Min(rect.width / sourceW, rect.height / sourceH);
            if (scale <= 0) return;
            // Five columns preserve the middle crest as well as both corners.
            float edge = RewardCard ? 240 : 290, middle = RewardCard ? 130 : 300;
            float[] sx = {0, edge, sourceW / 2 - middle, sourceW / 2 + middle, sourceW - edge, sourceW};
            float[] dx = {0, edge * scale, rect.width / 2 - middle * scale,
                rect.width / 2 + middle * scale, rect.width - edge * scale, rect.width};
            float[] sy, dy;
            if (RewardCard)
            {
                sy = new float[] {0, 350, 1238, 1498};
                dy = new float[] {0, 350 * scale, rect.height - 260 * scale, rect.height};
            }
            else
            {
                // Also preserve the circular ornaments halfway up the side rails.
                sy = new float[] {0, 225, 370, 445, 540, 809};
                dy = new float[] {0, 225 * scale, rect.height / 2 - 37.5f * scale,
                    rect.height / 2 + 37.5f * scale, rect.height - 269 * scale, rect.height};
            }
            Vector4 uv = DataUtility.GetOuterUV(image.sprite);
            vh.Clear();
            for (int y = 0; y < sy.Length; y++)
                for (int x = 0; x < sx.Length; x++)
                {
                    var vertex = UIVertex.simpleVert;
                    vertex.position = new Vector3(rect.xMin + dx[x], rect.yMin + dy[y]);
                    vertex.uv0 = new Vector2(Mathf.Lerp(uv.x, uv.z, sx[x] / sourceW),
                        Mathf.Lerp(uv.y, uv.w, sy[y] / sourceH));
                    vertex.color = image.color;
                    vh.AddVert(vertex);
                }
            for (int y = 0; y < sy.Length - 1; y++)
                for (int x = 0; x < sx.Length - 1; x++)
                {
                    int i = y * sx.Length + x;
                    vh.AddTriangle(i, i + sx.Length, i + sx.Length + 1);
                    vh.AddTriangle(i, i + sx.Length + 1, i + 1);
                }
        }
    }
}
