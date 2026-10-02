using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public sealed class ProductionOfflineView : MonoBehaviour
    {
        public const int CurrentRevision=2;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body,Hud;
        public Image Environment;
        public Font BodyFont,TitleFont;
        public Text Heading,Description,LocalDescription,OnlineDescription,RetryLabel,BackLabel,CheckNote,SupportLabel;
        public Text HeroName,LevelText,GoldText,GemsText,EnergyText;
        public Button Retry,Back,ProfileButton,SettingsButton;
        public Button[] PlusButtons;
        public Image Emblem;
        public bool ActionsBound {get;private set;}
        public bool NetworkAvailable {get;private set;}
        public int CheckCount {get;private set;}
        Func<NetworkReachability> reachability;
        readonly List<Button> buttons=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        string language;bool leaving;GameState observed;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Hud&&Environment&&BodyFont&&TitleFont&&Heading&&Description&&LocalDescription&&OnlineDescription&&RetryLabel&&BackLabel&&CheckNote&&SupportLabel&&Retry&&Back&&Emblem&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&ProfileButton&&SettingsButton&&PlusButtons!=null&&PlusButtons.Length==3&&Array.TrueForAll(PlusButtons,b=>b!=null);
        void OnEnable(){Observe();Refresh();}
        void OnDisable(){if(observed!=null)observed.Changed-=RefreshBalances;observed=null;Unbind();}
        void Update(){if(observed!=GameState.Instance){Observe();RefreshBalances();}if(language!=DemoFeatureState.LanguageCode)Refresh();}
        void Observe(){if(observed!=null)observed.Changed-=RefreshBalances;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshBalances;}
        bool Ready()=>ActionsBound&&!leaving&&isActiveAndEnabled&&(ScreenRouter.Instance==null||(!ScreenRouter.Instance.IsScreenTransitioning&&!ScreenRouter.Instance.HasActiveModalBlocker));
        void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{if(!Ready()||!b.IsInteractable()||action==null)return;if(route)leaving=true;action();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action onBack,Func<NetworkReachability> networkProbe=null,Action<ScreenId> navigate=null,Action<string> shop=null)
        {
            Unbind();reachability=networkProbe??(()=>Application.internetReachability);leaving=false;CheckCount=0;
            Listen(Retry,CheckNetwork);Listen(Back,onBack,true);
            Listen(ProfileButton,navigate==null?(Action)null:()=>navigate(ScreenId.Profile),true);
            Listen(SettingsButton,navigate==null?(Action)null:()=>navigate(ScreenId.Settings),true);
            Listen(PlusButtons[0],shop==null?(Action)null:()=>shop("coins"),true);
            Listen(PlusButtons[1],shop==null?(Action)null:()=>shop("gems"),true);
            Listen(PlusButtons[2],navigate==null?(Action)null:()=>navigate(ScreenId.OutOfEnergy),true);
            ActionsBound=true;ReadNetwork();Refresh();
        }
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        void ReadNetwork(){NetworkAvailable=(reachability??(()=>Application.internetReachability))()!=NetworkReachability.NotReachable;}
        void CheckNetwork(){ReadNetwork();CheckCount++;Refresh();}
        void OnApplicationFocus(bool focused){if(focused&&ActionsBound){ReadNetwork();Refresh();}}
        public void RefreshBalances(){if(!HasRequiredBindings)return;var save=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";HeroName.text=en?"Ivan":"Иван";LevelText.text=(en?"Level ":"Уровень ")+(save==null?"—":save.level.ToString());GoldText.text=save==null?"—":ProductionInventoryView.Number(save.gold);GemsText.text=save==null?"—":ProductionInventoryView.Number(save.gems);EnergyText.text=save==null?"—":save.energy+"/"+save.energyMax;}
        public void Refresh()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";
            Heading.text=NetworkAvailable?(en?"CHECK CONNECTION":"ПРОВЕРЬТЕ СЕТЬ"):(en?"NO CONNECTION":"НЕТ СОЕДИНЕНИЯ");
            Description.text=NetworkAvailable?(en?"Your device has a network connection.\nAccess to game services is not confirmed.":"На устройстве есть сетевое подключение.\nДоступ к игровым сервисам не подтверждён."):(en?"Check your internet connection\nand try again.":"Проверьте подключение к интернету\nи попробуйте снова.");
            LocalDescription.text=en?"Offline levels and local saving remain available.":"Офлайн-уровни и локальное сохранение доступны.";
            SupportLabel.text=en?"SUPPORT: UNAVAILABLE":"ПОДДЕРЖКА: НЕДОСТУПНА";
            OnlineDescription.text=en?"SERVER STATUS: UNKNOWN":"СТАТУС СЕРВЕРОВ: НЕИЗВЕСТЕН";
            RetryLabel.text=en?"TRY AGAIN":"ПОВТОРИТЬ";BackLabel.text=en?"BACK TO HOME":"НА ГЛАВНУЮ";
            CheckNote.text=CheckCount==0?"":(en?"Device network availability checked.":"Доступность сети на устройстве проверена.");
            RefreshBalances();
        }
    }
}
