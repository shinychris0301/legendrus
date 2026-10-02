using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public sealed class ProductionMaintenanceView : MonoBehaviour
    {
        public const int CurrentRevision=3;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body;
        public Image Environment,Logo,Chest,Panel,EstimatePanel;
        public Font BodyFont,TitleFont;
        public Text Heading,Description,EstimateHeading,EstimateValue,CompensationHeading,CompensationDescription,PreviewLabel,ContinueLabel;
        public Button Continue;
        public bool ActionsBound {get;private set;}
        UnityAction listener;bool leaving;string language;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Environment&&Logo&&Chest&&Panel&&EstimatePanel&&BodyFont&&TitleFont&&Heading&&Description&&EstimateHeading&&EstimateValue&&CompensationHeading&&CompensationDescription&&PreviewLabel&&ContinueLabel&&Continue;
        void OnEnable(){Refresh();}
        void OnDisable(){Unbind();}
        void Update(){if(language!=DemoFeatureState.LanguageCode)Refresh();}
        public void Bind(Action onContinue)
        {
            Unbind();leaving=false;
            listener=()=>{if(!ActionsBound||leaving||!isActiveAndEnabled||!Continue.IsInteractable()||onContinue==null)return;if(ScreenRouter.Instance!=null&&(ScreenRouter.Instance.IsScreenTransitioning||ScreenRouter.Instance.HasActiveModalBlocker))return;leaving=true;onContinue();};
            Continue.onClick.AddListener(listener);ActionsBound=true;Refresh();
        }
        void Unbind(){if(Continue&&listener!=null)Continue.onClick.RemoveListener(listener);listener=null;ActionsBound=false;}
        public void Refresh()
        {
            if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";
            Panel.sprite=ProductionMaintenanceRuntime.Load(en?"panel_en_surface_r3":"panel_ru_surface_r3");
            EstimatePanel.sprite=ProductionMaintenanceRuntime.Load(en?"timer":"timer_ru_r2");
            ((Image)Continue.targetGraphic).sprite=ProductionMaintenanceRuntime.Load(en?"button":"button_ru_r2");
            Heading.enabled=EstimateHeading.enabled=ContinueLabel.enabled=en;
            Heading.text=en?"SERVICE\nMAINTENANCE":"ТЕХНИЧЕСКИЕ\nРАБОТЫ";
            Description.text=en?"During maintenance, online services\nmay be unavailable. Your local\nprogress remains saved.":"Во время технических работ\nонлайн-сервисы могут быть недоступны.\nВаш локальный прогресс сохранён.";
            EstimateHeading.text=en?"ESTIMATED COMPLETION":"ОРИЕНТИРОВОЧНОЕ ЗАВЕРШЕНИЕ";
            EstimateValue.text=en?"Not announced":"Время не объявлено";
            CompensationHeading.text=en?"COMPENSATION":"КОМПЕНСАЦИЯ";
            CompensationDescription.text=en?"Rewards have not\nyet been confirmed.":"Информация о награде\nпока не подтверждена.";
            PreviewLabel.text=en?"GIFT PREVIEW":"ПРЕДПРОСМОТР ПОДАРКА";
            ContinueLabel.text=en?"UNDERSTOOD":"ПОНЯТНО";
        }
    }
}
