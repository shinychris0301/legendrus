using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public sealed class ProductionLoginView : MonoBehaviour
    {
        public const int CurrentRevision=1;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body;
        public Image Environment,Panel;
        public Font BodyFont,TitleFont;
        public Text Heading,LoginLabel,GuestLabel,LinkLabel,Divider,Availability,TermsLabel,PrivacyLabel,InfoHeading,InfoDescription,InfoCloseLabel;
        public InputField Identity,Password;
        public Button Login,Guest,Link,Eye,Back,Terms,Privacy,InfoClose;
        public Button[] Providers;
        public GameObject InfoPanel;
        public bool ActionsBound {get;private set;}
        public bool PasswordVisible {get;private set;}
        public bool OnlineAuthenticationAvailable => false;
        readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();
        string language;int infoKind;bool committed;
        public bool HasRequiredBindings => Revision==CurrentRevision&&Content&&Body&&Environment&&Panel&&BodyFont&&TitleFont&&Heading&&LoginLabel&&GuestLabel&&LinkLabel&&Divider&&Availability&&TermsLabel&&PrivacyLabel&&Identity&&Password&&Login&&Guest&&Link&&Eye&&Back&&Terms&&Privacy&&InfoClose&&InfoPanel&&InfoHeading&&InfoDescription&&InfoCloseLabel&&Providers!=null&&Providers.Length==4&&Array.TrueForAll(Providers,b=>b);
        void OnEnable(){RefreshContent();}
        void OnDisable(){Unbind();if(Identity)Identity.SetTextWithoutNotify("");if(Password)Password.SetTextWithoutNotify("");PasswordVisible=false;if(Password){Password.contentType=InputField.ContentType.Password;Password.ForceLabelUpdate();}}
        void Update(){if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        bool Ready()=>ActionsBound&&!committed&&isActiveAndEnabled&&(ScreenRouter.Instance==null||(!ScreenRouter.Instance.IsScreenTransitioning&&!ScreenRouter.Instance.HasActiveModalBlocker));
        void Listen(Button button,Action action,bool overlay=false){UnityAction call=()=>{if(!Ready()||!button.IsInteractable()||(!overlay&&InfoPanel.activeSelf))return;action();};button.onClick.AddListener(call);buttons.Add(button);listeners.Add(call);}
        public void Bind(Action guest,Action back)
        {
            Unbind();committed=false;
            Listen(Guest,()=>{committed=true;Identity.SetTextWithoutNotify("");Password.SetTextWithoutNotify("");guest();});
            Listen(Back,()=>{committed=true;back();});Listen(Login,()=>ShowInfo(0));Listen(Link,()=>ShowInfo(1));
            for(int i=0;i<4;i++){int j=i;Listen(Providers[i],()=>ShowInfo(j+2));}
            Listen(Terms,()=>ShowInfo(6));Listen(Privacy,()=>ShowInfo(7));Listen(InfoClose,()=>TryDismiss(),true);
            Listen(Eye,()=>{PasswordVisible=!PasswordVisible;Password.contentType=PasswordVisible?InputField.ContentType.Standard:InputField.ContentType.Password;Password.ForceLabelUpdate();});
            ActionsBound=true;RefreshContent();
        }
        void ShowInfo(int kind){infoKind=kind;Identity.DeactivateInputField();Password.DeactivateInputField();InfoPanel.SetActive(true);RefreshInfo();}
        public bool TryDismiss(){if(!InfoPanel||!InfoPanel.activeSelf)return false;InfoPanel.SetActive(false);return true;}
        void RefreshInfo()
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            string[] titles=en?new[]{"SIGN IN","LINK ACCOUNT","VK","OK","MAIL.RU","GOOGLE","TERMS OF USE","PRIVACY"}:new[]{"ВХОД","ПРИВЯЗКА АККАУНТА","VK","OK","MAIL.RU","GOOGLE","УСЛОВИЯ ИСПОЛЬЗОВАНИЯ","КОНФИДЕНЦИАЛЬНОСТЬ"};
            InfoHeading.text=titles[infoKind];
            InfoDescription.text=infoKind>=6?(en?"The published document is not connected yet. No legal acceptance is recorded on this page.":"Опубликованный документ пока не подключён. На этой странице согласие не сохраняется."):(en?"Online sign-in and account linking are not connected yet. Nothing has been sent. Continue as a guest to play with progress saved on this device.":"Онлайн-вход и привязка аккаунта пока не подключены. Данные не отправлены. Продолжите как гость: прогресс сохраняется на этом устройстве.");
            InfoCloseLabel.text=en?"CLOSE":"ЗАКРЫТЬ";
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            Panel.sprite=ProductionLoginRuntime.Load(en?"panel_blank":"panel");Heading.enabled=en;Heading.text="SIGN IN TO YOUR ACCOUNT";
            LoginLabel.text=en?"SIGN IN":"ВОЙТИ";GuestLabel.text=en?"CONTINUE AS GUEST":"ПРОДОЛЖИТЬ КАК ГОСТЬ";LinkLabel.text=en?"LINK ACCOUNT":"ПРИВЯЗАТЬ АККАУНТ";
            Divider.text=en?"or continue with":"или продолжите с помощью";Availability.text=en?"Online sign-in is not available yet":"Онлайн-вход пока недоступен";
            ((Text)Identity.placeholder).text=en?"Email or phone":"Email или телефон";((Text)Password.placeholder).text=en?"Password":"Пароль";
            TermsLabel.text=en?"Terms of use":"Условия использования";PrivacyLabel.text=en?"Privacy policy":"Политика конфиденциальности";
            RefreshInfo();
        }
    }
}
