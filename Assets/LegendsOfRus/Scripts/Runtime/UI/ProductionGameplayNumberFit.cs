using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Contains numeric glyph overhang and its shadow after normal font best-fit.</summary>
    [RequireComponent(typeof(Text)),DisallowMultipleComponent]
    public sealed class ProductionGameplayNumberFit : BaseMeshEffect
    {
        public override void ModifyMesh(VertexHelper vh)
        {
            if(!IsActive()||vh.currentVertCount==0)return;
            Rect area=graphic.rectTransform.rect;
            area=Rect.MinMaxRect(area.xMin+4,area.yMin+2,area.xMax-4,area.yMax-2);
            if(area.width<=0||area.height<=0)return;
            Vector2 low=new Vector2(float.PositiveInfinity,float.PositiveInfinity),high=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
            UIVertex vertex=default;
            for(int i=0;i<vh.currentVertCount;i++){vh.PopulateUIVertex(ref vertex,i);low=Vector2.Min(low,vertex.position);high=Vector2.Max(high,vertex.position);}
            if(low.x>=area.xMin&&low.y>=area.yMin&&high.x<=area.xMax&&high.y<=area.yMax)return;
            Vector2 size=high-low,center=(low+high)*.5f;
            float scale=Mathf.Min(1,Mathf.Min(area.width/Mathf.Max(.001f,size.x),area.height/Mathf.Max(.001f,size.y)));
            Vector2 extent=size*scale*.5f;
            Vector2 target=new Vector2(Mathf.Clamp(center.x,area.xMin+extent.x,area.xMax-extent.x),Mathf.Clamp(center.y,area.yMin+extent.y,area.yMax-extent.y));
            for(int i=0;i<vh.currentVertCount;i++)
            {
                vh.PopulateUIVertex(ref vertex,i);Vector2 position=target+((Vector2)vertex.position-center)*scale;
                vertex.position=new Vector3(position.x,position.y,vertex.position.z);vh.SetUIVertex(vertex,i);
            }
        }
    }
}
