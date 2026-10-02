using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Optically centers live booster lettering, including its shadow, inside the wood well.</summary>
    [RequireComponent(typeof(Text)),DisallowMultipleComponent]
    public sealed class ProductionGameplayCaptionFit : BaseMeshEffect
    {
        public bool ExpandedCaption;
        public float LastScale { get; private set; } = 1;
        public override void ModifyMesh(VertexHelper vh)
        {
            LastScale = 1;
            if(!IsActive()||vh.currentVertCount==0)return;
            Rect area=graphic.rectTransform.rect;
            area=ExpandedCaption
                ? Rect.MinMaxRect(area.xMin+8,area.yMin+4,area.xMax-8,area.yMax-4)
                : Rect.MinMaxRect(area.xMin+46,area.yMin+10,area.xMax-54,area.yMax-8);
            if(area.width<=0||area.height<=0)return;
            Vector2 low=new Vector2(float.PositiveInfinity,float.PositiveInfinity),high=new Vector2(float.NegativeInfinity,float.NegativeInfinity);
            UIVertex vertex=default;
            for(int i=0;i<vh.currentVertCount;i++)
            {
                vh.PopulateUIVertex(ref vertex,i);
                low=Vector2.Min(low,vertex.position);high=Vector2.Max(high,vertex.position);
            }
            Vector2 size=high-low,center=(low+high)*.5f;
            float scale=Mathf.Min(1,Mathf.Min(area.width/Mathf.Max(.001f,size.x),area.height/Mathf.Max(.001f,size.y)));
            LastScale = scale;
            for(int i=0;i<vh.currentVertCount;i++)
            {
                vh.PopulateUIVertex(ref vertex,i);
                Vector2 position=area.center+((Vector2)vertex.position-center)*scale;
                vertex.position=new Vector3(position.x,position.y,vertex.position.z);vh.SetUIVertex(vertex,i);
            }
        }
    }
}
