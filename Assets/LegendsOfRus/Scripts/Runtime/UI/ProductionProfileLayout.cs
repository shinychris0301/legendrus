using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    [ExecuteAlways,DisallowMultipleComponent]
    public sealed class ProductionProfileLayout:MonoBehaviour
    {
        public ProductionProfileView View;readonly Vector3[] corners=new Vector3[4];bool applying;
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
                ((RectTransform)View.Body.Find("ContentBacking")).sizeDelta=new Vector2(941,1301-bottom);
                // Measure after the canvas has applied its current phone scale; build-time glyph metrics differ.
                for(int i=0;i<6;i++){
                    var child=View.DynamicRoot.Find("Relic_"+i+"/Name");if(!child)continue;
                    var caption=child.GetComponent<Text>();caption.fontSize=18;
                    while(caption.fontSize>15&&(caption.preferredWidth>caption.rectTransform.rect.width-8||caption.preferredHeight>caption.rectTransform.rect.height-2))caption.fontSize--;
                }
                Move(View.BackButton.transform,22,1527,bottom);
                for(int i=0;i<3;i++)Move(View.FooterButtons[i].transform,i==0?178:i==1?480:654,1543,bottom);
                Move(View.Body.Find("FooterStrip"),158,1528,bottom);
                Move(View.Body.Find("CarvedFooter"),0,1518,bottom);
                var collection=View.DynamicRoot.Find("RelicCollectionFrame") as RectTransform;if(collection)collection.sizeDelta=new Vector2(927,(View.SelectedTab==ProfileTab.Relics?845:529)-bottom);
                var frame=View.DynamicRoot.Find("RelicDetailFrame") as RectTransform;if(frame)frame.sizeDelta=new Vector2(839,253-bottom);
                var action=View.DynamicRoot.Find("CollectionInfo");if(action){
                    float detailOffset=bottom*.5f;
                    Move(action,585,1452,detailOffset);
                    Move(View.DynamicRoot.Find("SelectedRelic"),56,1270,detailOffset);
                    Move(View.DynamicRoot.Find("RelicTitle"),276,1282,detailOffset);
                    Move(View.DynamicRoot.Find("RelicPreviewBadge"),620,1283,detailOffset);
                    Move(View.DynamicRoot.Find("RelicPreviewLabel"),639,1290,detailOffset);
                    Move(View.DynamicRoot.Find("RelicDescription"),276,1331,detailOffset);
                    Move(View.DynamicRoot.Find("RelicSelectionHint"),592,1348,detailOffset);
                }
                Move(View.InfoPanel.transform,0,0,bottom*.5f);var shield=(RectTransform)View.InfoPanel.transform.Find("InputShield");shield.anchoredPosition=new Vector2(0,-bottom*.5f);shield.sizeDelta=new Vector2(941,safeHigh.y-safeLow.y);
                whole.GetWorldCorners(corners);low=content.InverseTransformPoint(corners[0]);high=content.InverseTransformPoint(corners[2]);Vector2 dimensions=View.Environment.sprite.rect.size;
                float cover=Mathf.Max((high.x-low.x)/dimensions.x,(high.y-low.y)/dimensions.y);var background=View.Environment.rectTransform;
                background.sizeDelta=dimensions*cover;background.anchoredPosition=new Vector2((low.x+high.x-background.sizeDelta.x)*.5f-content.rect.xMin,(low.y+high.y+background.sizeDelta.y)*.5f-content.rect.yMax);
            }finally{applying=false;}
        }
    }
}
