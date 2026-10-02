using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Extends only the score plaque's plain rails; carved ends and center jewels keep their aspect.</summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Image))]
    public sealed class ProductionGameplayScoreFrame : BaseMeshEffect
    {
        // Boundaries between complete ornaments and plain surfaces in score_panel_r3.png (1501 x 489).
        static readonly float[] SourceX = { 0, 340, 590, 911, 1161, 1501 };

        public override void ModifyMesh(VertexHelper vh)
        {
            var image = graphic as Image;
            if (!IsActive() || image == null || image.sprite == null) return;

            Rect rect = image.GetPixelAdjustedRect();
            if (rect.width <= 0 || rect.height <= 0) return;
            // Fit ornaments by height. The two remaining strips contain straight rails and blank wells.
            float scale = Mathf.Min(rect.height / 489f, rect.width / 1501f);
            float end = 340 * scale, center = 321 * scale;
            float rail = (rect.width - end * 2 - center) * .5f;
            float height = 489 * scale;
            float bottom = rect.center.y - height * .5f;
            Vector4 uv = DataUtility.GetOuterUV(image.sprite);
            vh.Clear();
            float left = rect.xMin;
            for (int strip = 0; strip < 5; strip++)
            {
                float width = strip == 0 || strip == 4 ? end : strip == 2 ? center : rail;
                float u0 = Mathf.Lerp(uv.x, uv.z, SourceX[strip] / 1501f);
                float u1 = Mathf.Lerp(uv.x, uv.z, SourceX[strip + 1] / 1501f);
                int vertex = vh.currentVertCount;
                vh.AddVert(new Vector3(left, bottom, 0), image.color, new Vector2(u0, uv.y));
                vh.AddVert(new Vector3(left, bottom + height, 0), image.color, new Vector2(u0, uv.w));
                vh.AddVert(new Vector3(left + width, bottom + height, 0), image.color, new Vector2(u1, uv.w));
                vh.AddVert(new Vector3(left + width, bottom, 0), image.color, new Vector2(u1, uv.y));
                vh.AddTriangle(vertex, vertex + 1, vertex + 2);
                vh.AddTriangle(vertex + 2, vertex + 3, vertex);
                left += width;
            }
        }
    }
}
