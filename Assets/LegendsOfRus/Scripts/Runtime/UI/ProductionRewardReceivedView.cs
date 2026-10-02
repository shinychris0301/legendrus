using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionRewardReceivedView : MonoBehaviour
    {
        public const int CurrentRevision=2;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Body;
        public Image Shade,Panel,Chest,HeadingLettering,ChestGlow;
        public Text Title,Message,ConfirmText;
        public Image[] Cards,Icons;
        public Text[] Amounts,Names;
        public Button Confirm;
        public bool ActionsBound {get;private set;}
        public bool HasReceipt => receipt.HasValue;
        public int VisibleRewardCount {get;private set;}
        public bool HasRequiredBindings => Revision==CurrentRevision && Typography && Content && Body && Shade && Panel && Panel.sprite && Chest && Chest.sprite && HeadingLettering && ChestGlow && Title && Message && ConfirmText && Confirm && Valid(Cards) && Valid(Icons) && Valid(Amounts) && Valid(Names);
        static bool Valid<T>(T[] items) where T:UnityEngine.Object => items!=null && items.Length==7 && Array.TrueForAll(items,item=>item!=null);
        DailyRewardGrant? receipt;int day;Action close;bool confirming;string language;
        void OnEnable(){RefreshContent();}
        void OnDisable(){Confirm?.onClick.RemoveListener(ConfirmOnce);close=null;ActionsBound=false;receipt=null;}
        void Update(){if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        public void Bind(DailyRewardGrant? granted,int rewardDay,Action confirm)
        {
            Confirm.onClick.RemoveListener(ConfirmOnce);receipt=granted;day=rewardDay;close=confirm;confirming=false;ActionsBound=true;
            Confirm.onClick.AddListener(ConfirmOnce);RefreshContent();
        }
        void ConfirmOnce()
        {
            if(!ActionsBound||confirming||!isActiveAndEnabled||!Confirm.IsInteractable())return;
            confirming=true;Confirm.interactable=false;close?.Invoke();
        }
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            var font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(var text in GetComponentsInChildren<Text>(true))text.font=font;
            Title.text=HasReceipt?(en?"REWARD RECEIVED":"ПОЛУЧЕНА НАГРАДА"):(en?"REWARDS":"НАГРАДЫ");
            ConfirmText.text=HasReceipt?(en?"COLLECT":"ЗАБРАТЬ"):(en?"CLOSE":"ЗАКРЫТЬ");
            Message.text=HasReceipt?(en?"Great job!\nThese rewards have been added to your account:":"Отличная работа!\nЭти награды уже добавлены на ваш счёт:"):(en?"No new reward to display.":"Нет новой награды для отображения.");
            Chest.gameObject.SetActive(HasReceipt);
            var grant=receipt.GetValueOrDefault();int[] counts={grant.Gold,grant.Gems,grant.Energy,grant.Mace,grant.Firebird,grant.LivingLight,grant.SeaPearl};
            string[] labels=en?new[]{"Coins","Gems","Energy","Mace","Firebird","Living Light","Sea Pearl"}:new[]{"Монеты","Самоцветы","Энергия","Булава","Жар-птица","Живой свет","Жемчужина"};
            string[] keys={"NavigationAligned/coin","NavigationAligned/gem","NavigationAligned/energy","GameplayAligned/booster_mace_r9","GameplayAligned/booster_firebird_r9","GameplayAligned/booster_livinglight_r9","GameplayAligned/booster_seapearl_r9"};
            var slots=new List<int>();for(int i=0;i<7;i++)if(HasReceipt&&(counts[i]>0||day==3&&i==2))slots.Add(i);
            VisibleRewardCount=slots.Count;
            for(int i=0;i<7;i++)
            {
                Cards[i].gameObject.SetActive(i<slots.Count);if(i>=slots.Count)continue;int kind=slots[i];
                int row=i/5,first=row*5,inRow=Math.Min(5,slots.Count-first);
                Cards[i].rectTransform.localScale=Vector3.one;
                Cards[i].rectTransform.anchoredPosition=new Vector2((795-inRow*134)*.5f+(i-first)*134+6,-(slots.Count<=5?628:554+row*144));
                Icons[i].sprite=kind<3?ProductionRewardReceivedRuntime.Load(new[]{"coin_r2","gem_r2","energy_r2"}[kind]):ProductionBoosterArt.Icon(kind-3);
                float ratio=Icons[i].sprite.rect.width/Icons[i].sprite.rect.height;
                float iconHeight=Mathf.Min(66,76/ratio),iconWidth=iconHeight*ratio;
                Icons[i].rectTransform.sizeDelta=new Vector2(iconWidth,iconHeight);
                Icons[i].rectTransform.anchoredPosition=new Vector2((122-iconWidth)/2,-(8+(66-iconHeight)/2));
                Amounts[i].text=counts[kind].ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ");
                Amounts[i].resizeTextForBestFit=true;Amounts[i].resizeTextMinSize=15;Amounts[i].resizeTextMaxSize=27;Names[i].text=labels[kind];
            }
            if(HasReceipt&&day==3&&grant.Energy==0)Message.text=en?"Daily reward recorded.\nYour energy is already at capacity.":"Ежедневная награда отмечена полученной.\nЗапас энергии уже полон.";
            ProductionRewardReceivedRuntime.ApplySourceState(this,en);
            Confirm.interactable=ActionsBound&&!confirming;
        }
    }
}
