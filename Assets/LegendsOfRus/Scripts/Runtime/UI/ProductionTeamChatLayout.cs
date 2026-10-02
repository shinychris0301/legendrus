using UnityEngine;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionTeamChatLayout:MonoBehaviour
    {
        public ProductionTeamChatView View;readonly Vector3[] corners=new Vector3[4];bool applying;
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
                safe.GetWorldCorners(corners);var safeLow=View.Body.InverseTransformPoint(corners[0]);var safeHigh=View.Body.InverseTransformPoint(corners[2]);
                float bottom=safeLow.y-View.Body.rect.yMin;
                Move(View.Composer.transform,123,1404,bottom);Move(View.SendButton.transform,679,1406,bottom);Move(View.BackButton.transform,15,1398,bottom);
                Move(View.LocalStatus.transform,20,1645,bottom);Move(View.Body.Find("CarvedFooter"),0,1493,bottom);Move(View.Body.Find("FooterSapphire"),300,1615,bottom);
                float[] xs={85,361,638},ys={1492,1484,1493};for(int i=0;i<3;i++)Move(View.FooterButtons[i].transform,xs[i],ys[i],bottom);
                View.ChatViewport.sizeDelta=new Vector2(855,985-bottom);
                ((RectTransform)View.Body.Find("ChatFrame")).sizeDelta=new Vector2(888,1128-bottom);
                float rowsHeight=0;foreach(RectTransform row in View.Messages)rowsHeight+=row.rect.height;
                View.Messages.sizeDelta=new Vector2(855,Mathf.Max(View.ChatViewport.rect.height,rowsHeight));
                ((RectTransform)View.InfoPanel.transform).anchoredPosition=new Vector2(0,bottom*.5f);
                var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");shield.anchoredPosition=new Vector2(0,-bottom*.5f);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                ProductionTeamChatRuntime.RefitInlineMessages(View);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
