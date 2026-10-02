using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionFriendsListLayout:MonoBehaviour
    {
        public ProductionFriendsListView View;readonly Vector3[] corners=new Vector3[4];bool applying;float footerPixelsPerUnit=-1;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        static void Move(Transform t,float x,float y,float bottom){((RectTransform)t).anchoredPosition=new Vector2(x,-y+bottom);}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var whole=(RectTransform)canvas.rootCanvas.transform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?(RectTransform)fitter.transform:whole;applying=true;
            try{
                var content=View.Content;safe.GetWorldCorners(corners);var low=content.InverseTransformPoint(corners[0]);var high=content.InverseTransformPoint(corners[2]);
                float scale=Mathf.Max(.1f,Mathf.Min((high.x-low.x)/941,(high.y-low.y)/1672));View.Body.localScale=Vector3.one*scale;
                View.Body.anchoredPosition=new Vector2((low.x+high.x-941*scale)*.5f-content.rect.xMin,high.y-content.rect.yMax);
                safe.GetWorldCorners(corners);var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);float bottom=safeLow.y-View.Body.rect.yMin;
                for(int i=0;i<5;i++){var nav=(RectTransform)View.NavButtons[i].transform;Move(nav,(i+.5f)*188.2f-nav.rect.width*.5f,1430,bottom);}
                if(!Mathf.Approximately(footerPixelsPerUnit,View.NavLabels[0].pixelsPerUnit)){
                    footerPixelsPerUnit=View.NavLabels[0].pixelsPerUnit;
                    foreach(var label in View.NavLabels)ProductionFriendsListRuntime.FitFooterCaption(label);
                }
                Move(View.SortButton.transform,86,1362,bottom);Move(View.Counter.transform,328,1362,bottom);Move(View.RemoveButton.transform,598,1368,bottom);Move(View.Body.Find("CarvedFooter"),0,1431,bottom);
                ((RectTransform)View.Body.Find("TableFrame")).sizeDelta=new Vector2(860,952-bottom);
                View.RequestsFrame.rectTransform.sizeDelta=new Vector2(927,884-bottom);
                ((RectTransform)View.InfoPanel.transform).anchoredPosition=new Vector2(0,bottom*.5f);
                var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");shield.anchoredPosition=new Vector2(0,-bottom*.5f);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
