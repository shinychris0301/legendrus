using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionTeamHubLayout:MonoBehaviour
    {
        public ProductionTeamHubView View;readonly Vector3[] corners=new Vector3[4];bool applying;
        void OnEnable(){Canvas.preWillRenderCanvases+=ApplyNow;ApplyNow();}
        void OnDisable(){Canvas.preWillRenderCanvases-=ApplyNow;}
        void OnRectTransformDimensionsChange(){ApplyNow();}
        public void ApplyNow()
        {
            if(applying||View==null||!View.HasRequiredBindings)return;var canvas=GetComponentInParent<Canvas>();if(canvas==null)return;
            var whole=(RectTransform)canvas.rootCanvas.transform;var fitter=GetComponentInParent<SafeAreaFitter>();var safe=fitter!=null?(RectTransform)fitter.transform:whole;applying=true;
            try{
                var content=View.Content;safe.GetWorldCorners(corners);var low=content.InverseTransformPoint(corners[0]);var high=content.InverseTransformPoint(corners[2]);
                float scale=Mathf.Max(.1f,Mathf.Min((high.x-low.x)/941,(high.y-low.y)/1672));View.Body.localScale=Vector3.one*scale;
                View.Body.anchoredPosition=new Vector2((low.x+high.x-941*scale)*.5f-content.rect.xMin,high.y-content.rect.yMax);
                safe.GetWorldCorners(corners);var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);
                View.Hud.anchoredPosition=new Vector2(0,safeHigh.y-View.Body.rect.yMax);
                float bottom=safeLow.y-View.Body.rect.yMin;
                ((RectTransform)View.BackButton.transform).anchoredPosition=new Vector2(22,-1548+bottom);
                ((RectTransform)View.FriendsButton.transform).anchoredPosition=new Vector2(755,-1576+bottom);
                ((RectTransform)View.Body.Find("MottoFrame")).anchoredPosition=new Vector2(200,-1545+bottom);
                View.Motto.rectTransform.anchoredPosition=new Vector2(244,-1564+bottom);
                ((RectTransform)View.Body.Find("FooterCrest")).anchoredPosition=new Vector2(129.6f,-1537.8f+bottom);
                ((RectTransform)View.GuildInfoButton.transform).anchoredPosition=new Vector2(686,-1564+bottom);
                ((RectTransform)View.Body.Find("CarvedFooter")).anchoredPosition=new Vector2(0,-1537+bottom);
                ((RectTransform)View.InfoPanel.transform).anchoredPosition=new Vector2(0,bottom*.5f);
                var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");
                shield.anchoredPosition=new Vector2(0,-bottom*.5f);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
