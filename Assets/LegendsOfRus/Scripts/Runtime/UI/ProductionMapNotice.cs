using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>One reusable, nonblocking map notice using the approved navy panel and gold frame.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionMapNotice : MonoBehaviour
    {
        public CanvasGroup Group;
        public Text Heading, Message;
        float remaining;
        public bool HasBindings=>Group && Heading && Message;
        public void Show(string message)
        {
            Heading.text=DemoFeatureState.LanguageCode=="en"?"NOT AVAILABLE YET":"ПОКА НЕДОСТУПНО";
            Message.text=message;remaining=3.5f;Group.alpha=1;transform.SetAsLastSibling();
        }
        public void Hide(){remaining=0;if(Group)Group.alpha=0;}
        void OnDisable(){Hide();}
        void Update()
        {
            if(!Group || remaining<=0)return;
            remaining=Mathf.Max(0,remaining-Time.unscaledDeltaTime);
            Group.alpha=Mathf.Clamp01(remaining/.2f);
        }
        public static ProductionMapNotice Install(RectTransform content)
        {
            var t=content.Find("MapNotice") as RectTransform??ProductionChapterMapRuntime.Rect(content,"MapNotice",0,0,758,230);
            t.anchorMin=t.anchorMax=t.pivot=new Vector2(.5f,.5f);t.anchoredPosition=Vector2.zero;t.sizeDelta=new Vector2(758,230);
            var notice=Ensure<ProductionMapNotice>(t);
            notice.Group=Ensure<CanvasGroup>(t);
            notice.Group.interactable=false;notice.Group.blocksRaycasts=false;
            Art(t,"NavyPanel","card_surface",12,10,734,208);
            Art(t,"GoldFrame","card_frame",0,0,758,230);
            notice.Heading=Label(t,"Heading",54,30,650,52,34);
            notice.Message=Label(t,"Message",60,89,638,104,30);
            notice.Hide();return notice;
        }
        static T Ensure<T>(Component owner) where T:Component
        {
            var component=owner.GetComponent<T>();return component?component:owner.gameObject.AddComponent<T>();
        }
        static void Art(RectTransform parent,string name,string key,float x,float y,float w,float h)
        {
            var rt=parent.Find(name) as RectTransform??ProductionChapterMapRuntime.Rect(parent,name,x,y,w,h);
            var image=Ensure<Image>(rt);
            image.sprite=ProductionWorldMapSourceComposition.Load(key);image.raycastTarget=false;
        }
        static Text Label(RectTransform parent,string name,float x,float y,float w,float h,int size)
        {
            var rt=parent.Find(name) as RectTransform??ProductionChapterMapRuntime.Rect(parent,name,x,y,w*2,h*2);
            rt.localScale=new Vector3(.5f,.5f,1);
            var text=Ensure<Text>(rt);
            text.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");text.fontSize=size*2;
            text.color=new Color32(245,214,153,255);text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
            text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Truncate;return text;
        }
    }
}
