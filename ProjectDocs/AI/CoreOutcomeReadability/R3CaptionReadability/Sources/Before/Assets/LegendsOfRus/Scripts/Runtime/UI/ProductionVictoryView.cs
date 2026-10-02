using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    [DisallowMultipleComponent]
    public sealed class ProductionVictoryView : MonoBehaviour
    {
        public Func<CampaignPlayerSave> CampaignSaveProvider;
        public const int CurrentRevision=11;
        public int Revision=CurrentRevision;
        public SplashTitleTypography Typography;
        public RectTransform Content,Hud,Body,Footer,RewardsArea;
        public Image Environment,Firebird;
        public Image RewardsPanel,FooterOrnament,MessageInset;
        public Image ResourceHeader;
        public Text EnergyTimer;
        public Text Title,Score,Reaction,RewardsHeading,RewardStatus,ContinueText,DoubleText,Status;
        public Text[] ResourceValues,RewardValues,RewardLabels;
        public RectTransform[] RewardCards;
        public Image[] RewardIcons;
        public ProductionWorldMapGraphic[] Stars,StarGlows;
        public Button ContinueButton,DoubleButton;
        public Button[] ResourceButtons;
        public bool ActionsBound{get;private set;}
        public bool HasRequiredBindings=>Revision==CurrentRevision&&RewardsPanel!=null&&RewardsPanel.GetComponent<ProductionVictoryPanelFit>()!=null&&MessageInset!=null&&ResourceHeader!=null&&ResourceHeader.sprite!=null&&EnergyTimer!=null&&RewardsPanel!=null&&RewardsPanel.sprite!=null&&FooterOrnament!=null&&HasBaseBindings&&Title.GetComponent<ProductionVictoryTitleLettering>()!=null&&Array.TrueForAll(StarGlows,g=>g.GetComponent<ProductionVictoryStarHalo>()!=null);
        public bool HasBaseBindings=>Typography!=null&&Content!=null&&Hud!=null&&Body!=null&&Footer!=null&&RewardsArea!=null&&Environment!=null&&Environment.sprite!=null&&Firebird!=null&&Firebird.sprite!=null&&Title!=null&&Score!=null&&Reaction!=null&&RewardsHeading!=null&&RewardStatus!=null&&ContinueText!=null&&DoubleText!=null&&Status!=null&&ContinueButton!=null&&DoubleButton!=null&&Valid(ResourceButtons,3)&&Valid(ResourceValues,3)&&Valid(RewardCards,6)&&Valid(RewardIcons,6)&&Valid(RewardValues,6)&&Valid(RewardLabels,6)&&Valid(Stars,3)&&Valid(StarGlows,3);
        static bool Valid<T>(T[] items,int n)where T:UnityEngine.Object=>items!=null&&items.Length==n&&Array.TrueForAll(items,x=>x!=null);
        GameState subscribed;
        string language;
        bool committed;
        float elapsed;
        float nextEnergyIndicatorRefresh;
        readonly List<Button> buttons=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        void OnEnable(){committed=false;elapsed=0;BindState();RefreshContent();}
        void OnDisable(){Unbind();if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=null;}
        void Update()
        {
            if(CampaignSaveProvider==null&&subscribed!=GameState.Instance){BindState();RefreshContent();}
            if(language!=DemoFeatureState.LanguageCode)RefreshContent();
            elapsed+=Time.unscaledDeltaTime;
            if(HasRequiredBindings&&Time.unscaledTime>=nextEnergyIndicatorRefresh)RefreshEnergyIndicator();
            // One soft reveal, no flashing or perpetual particle loop.
            if(HasRequiredBindings)
                for(int i=0;i<3;i++)
                {
                    float t=Mathf.SmoothStep(0,1,Mathf.Clamp01((elapsed-i*.12f)/.55f));
                    Stars[i].rectTransform.localScale=Vector3.one*Mathf.Lerp(.88f,1,t);
                    Color glow=StarGlows[i].color;glow.a=Stars[i].Dimmed?0:.64f*t;StarGlows[i].color=glow;
                }
        }
        void BindState(){if(subscribed!=null)subscribed.Changed-=RefreshState;subscribed=CampaignSaveProvider==null?GameState.Instance:null;if(subscribed!=null)subscribed.Changed+=RefreshState;}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";string source;
            Font font=Application.isPlaying?Typography.Resolve(false,out source):Typography.RegularFont;
            foreach(Text text in GetComponentsInChildren<Text>(true)){text.font=font;text.raycastTarget=false;}
            if(Application.isPlaying){Title.font=Typography.Resolve(true,out source);ContinueText.font=Title.font;}
            Title.text=en?"VICTORY!":"ПОБЕДА!";RewardsHeading.text=en?"YOUR REWARDS":"ВАШИ НАГРАДЫ";
            ContinueText.text=en?"CONTINUE":"ПРОДОЛЖИТЬ";Status.text="";
            RefreshState();
        }
        public void RefreshState()
        {
            if(!HasRequiredBindings)return;
            var campaign=CampaignSaveProvider?.Invoke();var state=CampaignSaveProvider==null?GameState.Instance:null;var save=campaign!=null?campaign.player:state!=null?state.Save:null;bool en=DemoFeatureState.LanguageCode=="en";
            ResourceValues[0].text=save!=null?Number(save.gold):"—";ResourceValues[1].text=save!=null?Number(save.gems):"—";ResourceValues[2].text=save!=null?save.energy+"/"+save.energyMax:"—";
            RefreshEnergyIndicator();
            var result=campaign!=null&&campaign.run.state==CampaignRunState.Won?new LevelCompletionResult(JsonUtility.FromJson<LevelData>(campaign.run.levelJson),campaign.run.stars,campaign.run.granted.gold,campaign.run.granted.gems,!CampaignRoundLifecycle.ZeroReward(campaign.run.granted),campaign.run.score):state!=null?state.LastCompletionResult:null;
            bool current=result!=null&&save!=null&&result.Level.chapterId==save.lastCompletedChapter&&result.Level.levelId==save.lastCompletedLevel;
            int stars=current?result.Stars:0;
            for(int i=0;i<3;i++){Stars[i].Dimmed=i>=stars;Stars[i].SetVerticesDirty();StarGlows[i].gameObject.SetActive(i<stars);}
            Score.text=current?(en?"LEVEL ":"УРОВЕНЬ ")+result.Level.levelId+"  •  "+(en?"SCORE ":"СЧЁТ ")+Number(result.Score):(en?"No completed round":"Нет завершённого раунда");
            var reaction=current&&campaign==null?StoryService.GetLocalizedReaction(result.Level,StoryReactionOutcomes.Victory,DemoFeatureState.LanguageCode):null;
            Reaction.text=reaction!=null?reaction.Speaker+": "+reaction.Text:"";
            Reaction.gameObject.SetActive(string.IsNullOrEmpty(Status.text));
            MessageInset.enabled=!string.IsNullOrEmpty(Status.text)||!string.IsNullOrEmpty(Reaction.text);
            bool granted=current&&result.FirstClearRewardGranted;
            RewardStatus.text=!current?"":granted?(en?"Rewards added":"Награды начислены"):(en?"First-clear rewards already received":"Награда за первое прохождение уже получена");
            int gold=current?result.GoldReward:0,gems=current?result.GemReward:0;
            if(current&&save.lastRewardDoubled)gold+=result.GoldReward;
            FillReward(0,ProductionVictoryRuntime.Home("coin_icon"),gold,en?"COINS":"МОНЕТЫ");
            FillReward(1,ProductionVictoryRuntime.Home("gems_icon"),gems,en?"GEMS":"САМОЦВЕТЫ");
            int count=2;
            if(granted)
                for(int i=0;i<4;i++)
                {
                    int amount=ProductionPreLevelView.BoosterReward(result.Level.rewards,i);
                    if(amount>0)FillReward(count++,ProductionGameplayArt.Booster(i),amount,en?"BOOSTER":"БУСТЕР");
                }
            float width=Mathf.Min(240,(720-(count-1)*14)/count);
            float start=(941-(width*count+14*(count-1)))*.5f;
            for(int i=0;i<6;i++)
            {
                RewardCards[i].gameObject.SetActive(i<count);if(i>=count)continue;
                RewardCards[i].anchoredPosition=new Vector2(start+i*(width+14),-167);RewardCards[i].sizeDelta=new Vector2(width,218);
                // A top-left Image pivot letterboxes against the left edge. Fit the
                // rectangle itself to the source ratio, then center that rectangle.
                var sprite=RewardIcons[i].sprite;
                float fit=Mathf.Min(Mathf.Min(185,width-22)/sprite.rect.width,129/sprite.rect.height);
                float iconWidth=sprite.rect.width*fit,iconHeight=sprite.rect.height*fit;
                Place(RewardIcons[i].rectTransform,(width-iconWidth)*.5f,36+(129-iconHeight)*.5f,iconWidth,iconHeight);
                PlaceText(RewardValues[i],10,178,width-20,32,32);
                PlaceText(RewardLabels[i],8,14,width-16,22,count>3?17:20);
            }
            bool eligible=current&&save.lastRewardEligibleForDouble&&!save.lastRewardDoubled;
            DoubleButton.interactable=eligible;
            DoubleText.text=current&&save.lastRewardDoubled?(en?"GOLD DOUBLED":"МОНЕТЫ УДВОЕНЫ"):eligible?(en?"×2 COINS\nWATCH VIDEO":"×2 МОНЕТЫ\nСМОТРЕТЬ ВИДЕО"):(en?"NO BONUS AVAILABLE":"БОНУС НЕДОСТУПЕН");
            ProductionVictoryReferenceStyle.Refresh(this,count,current);
        }
        void RefreshEnergyIndicator()
        {
            nextEnergyIndicatorRefresh=Time.unscaledTime+1f;
            var campaign=CampaignSaveProvider?.Invoke();var state=CampaignSaveProvider==null?GameState.Instance:null;var save=campaign!=null?campaign.player:state!=null?state.Save:null;
            if(save==null){EnergyTimer.text="—";return;}
            if(save.energy>=save.energyMax){EnergyTimer.text=DemoFeatureState.LanguageCode=="en"?"FULL":"ПОЛНАЯ";return;}
            // Read the existing service clock; rendering never grants or spends energy.
            int seconds=Mathf.Max(0,(int)Math.Ceiling((campaign==null?state.EnergyRemaining().TotalSeconds:Math.Max(0,TimeSpan.FromMinutes(GameState.EnergyRefillMinutes).TotalSeconds-Math.Max(0,(DateTime.UtcNow.Ticks-save.lastEnergyUtcTicks)/(double)TimeSpan.TicksPerSecond)%TimeSpan.FromMinutes(GameState.EnergyRefillMinutes).TotalSeconds))));
            EnergyTimer.text=(seconds/60).ToString("00",CultureInfo.InvariantCulture)+":"+(seconds%60).ToString("00",CultureInfo.InvariantCulture);
        }
        void FillReward(int index,Sprite sprite,int count,string label){RewardIcons[index].sprite=sprite;RewardValues[index].text=Number(count);RewardLabels[index].text=label;}
        public void SetStatus(string message)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            if(message.StartsWith("Видео с наградой"))message=en?"Reward video unavailable. Your normal rewards are kept.":"Видео недоступно. Обычная награда сохранена.";
            else if(message=="Проверенная награда успешно начислена.")message=en?"Verified bonus added.":"Проверенный бонус начислен.";
            Status.text=message;RefreshState();
        }
        public void BindNavigation(Action next,Action reward,Action<ScreenId> show)
        {
            Unbind();committed=false;
            Bind(ContinueButton,next,true);Bind(DoubleButton,reward,false);
            for(int i=0;i<3;i++){int index=i;Bind(ResourceButtons[i],()=>show(index==2?ScreenId.OutOfEnergy:ScreenId.Shop),true);}
            ActionsBound=true;
        }
        void Bind(Button button,Action action,bool route)
        {
            UnityAction listener=()=>{if(committed||!isActiveAndEnabled||!button.IsInteractable()||ScreenRouter.Instance!=null&&ScreenRouter.Instance.HasActiveModalBlocker)return;if(route)committed=true;action();};
            buttons.Add(button);listeners.Add(listener);button.onClick.AddListener(listener);
        }
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i]!=null)buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        static string Number(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
        internal static void Place(RectTransform r,float x,float y,float w,float h){r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        internal static void PlaceText(Text t,float x,float y,float w,float h,int size){Place(t.rectTransform,x,y,w*2,h*2);t.fontSize=size*2;}
    }
}
