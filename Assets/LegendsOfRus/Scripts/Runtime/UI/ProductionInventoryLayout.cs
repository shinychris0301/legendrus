using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionInventoryLayout:MonoBehaviour
    {
        public ProductionInventoryView View;readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        static void Move(Transform t,float x,float y,float offset){((RectTransform)t).anchoredPosition=new Vector2(x,-y+offset);}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var whole=(RectTransform)canvas.rootCanvas.transform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?(RectTransform)fitter.transform:whole;applying=true;
            try{
                var content=View.Content;safe.GetWorldCorners(corners);var low=content.InverseTransformPoint(corners[0]);var high=content.InverseTransformPoint(corners[2]);
                float scale=Mathf.Max(.1f,Mathf.Min((high.x-low.x)/941,(high.y-low.y)/1672));View.Body.localScale=Vector3.one*scale;
                View.Body.anchoredPosition=new Vector2((low.x+high.x-941*scale)*.5f-content.rect.xMin,high.y-content.rect.yMax);
                safe.GetWorldCorners(corners);var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);float bottom=safeLow.y-View.Body.rect.yMin;
                for(int i=0;i<5;i++)Move(View.NavButtons[i].transform,14+i*185,1495,bottom);
                Move(View.Body.Find("CarvedFooter"),0,1503,bottom);
                float detail=bottom*.5f;
                Move(View.DetailArt.transform,12,1093,detail);Move(View.Body.Find("DetailPaper"),299,1102,detail);
                Move(View.Body.Find("QuantityPanel"),35,1412,detail);Move(View.QuantityLabel.transform,57,1419,detail);Move(View.QuantityText.transform,54,1445,detail);
                Move(View.ItemName.transform,335,1125,detail);Move(View.Description.transform,349,1183,detail);
                Move(View.DetailDivider.transform,349,1295,detail);Move(View.UsageNote.transform,349,1320,detail);
                Move(View.UseButton.transform,321,1402,detail);Move(View.DetailsButton.transform,611,1402,detail);
                Move(View.InfoPanel.transform,0,0,bottom*.5f);var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");shield.anchoredPosition=new Vector2(0,-bottom*.5f);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
