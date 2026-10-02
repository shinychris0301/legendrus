using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Fades the independently painted scene into the cover plate at the
    /// focal window boundaries. No screenshot-region artwork or opaque seam.</summary>
    [ExecuteAlways, RequireComponent(typeof(Image)), DisallowMultipleComponent]
    public sealed class ProductionFailSceneBlend : BaseMeshEffect
    {
        Rect previousWindow;
        Vector3 previousPosition;
        void LateUpdate()
        {
            var window=transform.parent as RectTransform;
            if(window==null||graphic==null)return;
            if(window.rect==previousWindow&&transform.localPosition==previousPosition)return;
            previousWindow=window.rect;previousPosition=transform.localPosition;graphic.SetVerticesDirty();
        }
        public override void ModifyMesh(VertexHelper vertices)
        {
            var image=graphic as Image;var window=transform.parent as RectTransform;
            if(!IsActive()||image==null||image.sprite==null||window==null)return;
            const int rows=80;Rect rect=image.GetPixelAdjustedRect();Rect bounds=window.rect;
            Vector4 uv=DataUtility.GetOuterUV(image.sprite);vertices.Clear();
            for(int row=0;row<=rows;row++)
            {
                float t=row/(float)rows;float y=Mathf.Lerp(rect.yMin,rect.yMax,t);
                float parentY=y+transform.localPosition.y;
                float fade=Mathf.Clamp01(Mathf.Min((parentY-bounds.yMin)/90f,(bounds.yMax-parentY)/70f));
                fade=fade*fade*(3-2*fade);
                for(int side=0;side<2;side++)
                {
                    var v=UIVertex.simpleVert;v.position=new Vector3(side==0?rect.xMin:rect.xMax,y);
                    Color color=image.color;color.a*=fade;v.color=color;
                    v.uv0=new Vector2(side==0?uv.x:uv.z,Mathf.Lerp(uv.y,uv.w,t));vertices.AddVert(v);
                }
            }
            for(int row=0;row<rows;row++){int i=row*2;vertices.AddTriangle(i,i+2,i+3);vertices.AddTriangle(i,i+3,i+1);}
        }
    }
}
