using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public sealed class ProductionErrorDialogView:MonoBehaviour
    {
        public const int CurrentRevision=1;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body;
        public Image Shade,Emblem;
        public Font BodyFont,TitleFont;
        public Text Heading,Description,RetryLabel,CloseLabel,SupportLabel;
        public Button Retry,Close,Support;
        public bool ActionsBound{get;private set;}
        public bool CanRetry{get;private set;}
        UnityAction retryListener,closeListener;bool leaving;string language;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Shade&&Emblem&&BodyFont&&TitleFont&&Heading&&Description&&RetryLabel&&CloseLabel&&SupportLabel&&Retry&&Close&&Support;
        void OnEnable(){Refresh();}
        void OnDisable(){Unbind();}
        void Update(){if(language!=DemoFeatureState.LanguageCode)Refresh();}
        bool Ready()=>ActionsBound&&!leaving&&isActiveAndEnabled&&(ScreenRouter.Instance==null||(!ScreenRouter.Instance.IsModalTransitioning&&!ScreenRouter.Instance.IsScreenTransitioning));
        public void Bind(Action close,Action retry)
        {
            Unbind();leaving=false;CanRetry=retry!=null;
            closeListener=()=>{if(!Ready()||!Close.IsInteractable()||close==null)return;leaving=true;close();};
            retryListener=()=>{if(!Ready()||!Retry.IsInteractable()||retry==null)return;leaving=true;retry();};
            Close.onClick.AddListener(closeListener);Retry.onClick.AddListener(retryListener);ActionsBound=true;Refresh();
        }
        void Unbind(){if(Close&&closeListener!=null)Close.onClick.RemoveListener(closeListener);if(Retry&&retryListener!=null)Retry.onClick.RemoveListener(retryListener);closeListener=null;retryListener=null;ActionsBound=false;}
        public void Refresh()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            Heading.text=en?"AN ERROR OCCURRED":"ПРОИЗОШЛА ОШИБКА";
            Description.text=CanRetry?(en?"The action could not be completed.\nPlease try again.":"Не удалось выполнить действие.\nПожалуйста, попробуйте ещё раз."):(en?"Action could not be completed.\nClose this message and try again.":"Действие не выполнено.\nЗакройте окно и попробуйте снова.");
            RetryLabel.text=en?"TRY AGAIN":"ПОВТОРИТЬ";CloseLabel.text=en?"CLOSE":"ЗАКРЫТЬ";SupportLabel.text=en?"SUPPORT UNAVAILABLE":"ПОДДЕРЖКА НЕДОСТУПНА";
            Retry.interactable=CanRetry;Support.interactable=false;
        }
    }
}
