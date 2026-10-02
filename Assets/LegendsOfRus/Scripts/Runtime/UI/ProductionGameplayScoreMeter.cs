using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Readable native score fill; consumes the same live score fraction as the HUD.</summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProductionGameplayScoreMeter : MaskableGraphic
    {
        [SerializeField, Range(0,1)] float fraction;
        public float Fraction => fraction;
        public void SetFraction(float value)
        {
            value=Mathf.Clamp01(value);
            if(Mathf.Approximately(value,fraction))return;
            fraction=value;SetVerticesDirty();
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();Rect r=GetPixelAdjustedRect();
            Face(vh,r,new Color32(228,181,69,255),new Color32(96,52,12,255));
            r=Rect.MinMaxRect(r.xMin+2,r.yMin+2,r.xMax-2,r.yMax-2);
            Face(vh,r,new Color32(8,13,8,255),new Color32(32,26,12,255));
            if(fraction<=0)return;
            r=Rect.MinMaxRect(r.xMin+2,r.yMin+2,r.xMin+2+(r.width-4)*fraction,r.yMax-2);
            Face(vh,r,new Color32(177,221,66,255),new Color32(29,106,24,255));
        }
        static void Face(VertexHelper vh,Rect r,Color top,Color bottom)
        {
            // Smooth rounded end caps, including a narrow fill at low scores.
            float radius=Mathf.Min(r.height*.5f,r.width*.5f);
            var p=new System.Collections.Generic.List<Vector2>(36);
            Vector2[] centers={new Vector2(r.xMax-radius,r.yMax-radius),new Vector2(r.xMax-radius,r.yMin+radius),new Vector2(r.xMin+radius,r.yMin+radius),new Vector2(r.xMin+radius,r.yMax-radius)};
            for(int corner=0;corner<4;corner++)for(int step=0;step<=8;step++)
            {
                float angle=(90-corner*90-step*90f/8)*Mathf.Deg2Rad;
                p.Add(centers[corner]+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius);
            }
            int start=vh.currentVertCount;vh.AddVert(r.center,Color.Lerp(bottom,top,.5f),Vector2.zero);
            foreach(var point in p)vh.AddVert(point,Color.Lerp(bottom,top,Mathf.InverseLerp(r.yMin,r.yMax,point.y)),Vector2.zero);
            for(int i=0;i<p.Count;i++)vh.AddTriangle(start,start+1+i,start+1+(i+1)%p.Count);
        }
    }
}
