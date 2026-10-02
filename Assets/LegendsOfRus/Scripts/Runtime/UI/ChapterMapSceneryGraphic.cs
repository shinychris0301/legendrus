using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Soft overlap hides seams between reused scenery strips; no new bitmap or cropped UI.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ChapterMapSceneryGraphic : RawImage
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect r=GetPixelAdjustedRect();const int bands=20;
            for(int i=0;i<bands;i++)
            {
                float a=i/(float)bands,b=(i+1)/(float)bands;
                Color ca=color,cb=color;ca.a*=Mathf.Clamp01(Mathf.Min(a,1-a)*12);cb.a*=Mathf.Clamp01(Mathf.Min(b,1-b)*12);
                int start=vh.currentVertCount;
                vh.AddVert(new Vector3(r.xMin,Mathf.Lerp(r.yMin,r.yMax,a)),ca,new Vector2(uvRect.xMin,Mathf.Lerp(uvRect.yMin,uvRect.yMax,a)));
                vh.AddVert(new Vector3(r.xMax,Mathf.Lerp(r.yMin,r.yMax,a)),ca,new Vector2(uvRect.xMax,Mathf.Lerp(uvRect.yMin,uvRect.yMax,a)));
                vh.AddVert(new Vector3(r.xMax,Mathf.Lerp(r.yMin,r.yMax,b)),cb,new Vector2(uvRect.xMax,Mathf.Lerp(uvRect.yMin,uvRect.yMax,b)));
                vh.AddVert(new Vector3(r.xMin,Mathf.Lerp(r.yMin,r.yMax,b)),cb,new Vector2(uvRect.xMin,Mathf.Lerp(uvRect.yMin,uvRect.yMax,b)));
                vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);
            }
        }
    }
}
