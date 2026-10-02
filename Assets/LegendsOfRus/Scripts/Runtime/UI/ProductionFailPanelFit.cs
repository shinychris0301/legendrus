using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Page-only fitting of the complete isolated navy panel. Keeps corner
    /// carvings and central sapphires at uniform scale while plain spans resize.</summary>
    [RequireComponent(typeof(Image)),DisallowMultipleComponent]
    public sealed class ProductionFailPanelFit:BaseMeshEffect
    {
        [Range(0,.45f)]public float TopProtectedFraction=.25f;
        [Range(0,.45f)]public float BottomProtectedFraction=.25f;
        [Range(.25f,1)]public float ProtectedDetailScale=1;
        public override void ModifyMesh(VertexHelper vh)
        {
            var image=graphic as Image;if(!IsActive()||image==null||image.sprite==null)return;
            Rect r=image.GetPixelAdjustedRect();Vector2 native=image.sprite.rect.size;
            float s=Mathf.Min(r.width/native.x,r.height/native.y);
            s*=Mathf.Clamp(ProtectedDetailScale,.25f,1);
            if(s<=0)return;
            float ex=native.x*.15f,cx=native.x*.09f;
            float bottom=Mathf.Clamp(BottomProtectedFraction,0,.45f),top=Mathf.Clamp(TopProtectedFraction,0,.45f);
            float[] ux={0,.15f,.41f,.59f,.85f,1},uy={0,bottom,1-top,1};
            float[] x={0,ex*s,r.width*.5f-cx*s,r.width*.5f+cx*s,r.width-ex*s,r.width};
            float[] y={0,native.y*bottom*s,r.height-native.y*top*s,r.height};
            Vector4 uv=DataUtility.GetOuterUV(image.sprite);vh.Clear();
            for(int j=0;j<4;j++)for(int i=0;i<6;i++)
            {var v=UIVertex.simpleVert;v.position=new Vector3(r.xMin+x[i],r.yMin+y[j]);v.color=image.color;v.uv0=new Vector2(Mathf.Lerp(uv.x,uv.z,ux[i]),Mathf.Lerp(uv.y,uv.w,uy[j]));vh.AddVert(v);}
            for(int j=0;j<3;j++)for(int i=0;i<5;i++){int k=j*6+i;vh.AddTriangle(k,k+6,k+7);vh.AddTriangle(k,k+7,k+1);}
        }
    }
}
