using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus {
 public enum InventoryTab { Artifacts, Boosters, Materials, Quest }
 [DisallowMultipleComponent] public sealed class ProductionInventoryView:MonoBehaviour {
  public const int CurrentRevision=1;public int Revision=CurrentRevision;
  public RectTransform Content,Body,Hud,DynamicRoot;
  public Image Environment,Title,DetailArt;
  public Font BodyFont,NamesFont,TitleFont;
  public Text Heading,HeroName,LevelText,GoldText,GemsText,EnergyText,PreviewLabel,QuantityLabel,QuantityText,ItemName,Description,UseLabel,DetailsLabel,InfoHeading,InfoDescription,CloseInfoLabel;
  public Button ProfileButton,SettingsButton,BackButton,UseButton,DetailsButton,CloseInfoButton;
  public Button[] PlusButtons,Tabs,NavButtons;public Text[] TabLabels,NavLabels;public GameObject InfoPanel;
  public InventoryTab SelectedTab{get;private set;}public int SelectedItem{get;private set;}public bool ActionsBound{get;private set;}public bool HasCollectibleInventory=>false;public bool CanUseSelectedItem=>false;
  public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Hud&&DynamicRoot&&Environment&&Environment.sprite&&Title&&Title.sprite&&DetailArt&&BodyFont&&NamesFont&&TitleFont&&Heading&&HeroName&&LevelText&&GoldText&&GemsText&&EnergyText&&PreviewLabel&&QuantityLabel&&QuantityText&&ItemName&&Description&&UseLabel&&DetailsLabel&&InfoHeading&&InfoDescription&&CloseInfoLabel&&ProfileButton&&SettingsButton&&BackButton&&UseButton&&DetailsButton&&CloseInfoButton&&InfoPanel&&Valid(PlusButtons,3)&&Valid(Tabs,4)&&Valid(TabLabels,4)&&Valid(NavButtons,5)&&Valid(NavLabels,5);
  static bool Valid<T>(T[] a,int n)where T:UnityEngine.Object=>a!=null&&a.Length==n&&Array.TrueForAll(a,x=>x!=null);
  readonly List<Button> buttons=new List<Button>();readonly List<UnityAction> listeners=new List<UnityAction>();GameState observed;string language;bool committed,vipInfo;
  void OnEnable(){Observe();RefreshContent();}void OnDisable(){if(observed!=null)observed.Changed-=RefreshState;observed=null;Unbind();}
  void Update(){if(observed!=GameState.Instance){Observe();RefreshState();}if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
  void Observe(){if(observed!=null)observed.Changed-=RefreshState;observed=GameState.Instance;if(isActiveAndEnabled&&observed!=null)observed.Changed+=RefreshState;}
  void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
  public bool CanAct()=>ActionsBound&&!committed&&isActiveAndEnabled&&!InfoPanel.activeSelf&&(ScreenRouter.Instance==null||!ScreenRouter.Instance.HasActiveModalBlocker&&!ScreenRouter.Instance.IsScreenTransitioning);
  void Listen(Button b,Action action,bool route=false){UnityAction call=()=>{if(!CanAct()||!b.IsInteractable())return;if(route)committed=true;action?.Invoke();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
  public void Bind(Action<ScreenId> show,Action back,Action<string> shop){Unbind();committed=false;Listen(ProfileButton,()=>show(ScreenId.Profile),true);Listen(SettingsButton,()=>show(ScreenId.Settings),true);Listen(BackButton,back,true);Listen(DetailsButton,ShowDetails);
   for(int i=0;i<3;i++){int j=i;Listen(PlusButtons[i],()=>{if(j==2)show(ScreenId.OutOfEnergy);else shop(j==0?"coins":"gems");},true);}
   for(int i=0;i<4;i++){int j=i;Listen(Tabs[i],()=>SelectTab((InventoryTab)j));}
   Listen(NavButtons[0],()=>show(ScreenId.SpecialOffer),true);Listen(NavButtons[1],()=>shop("gems"),true);Listen(NavButtons[2],()=>shop("bundles"),true);Listen(NavButtons[3],()=>SelectTab(InventoryTab.Boosters));Listen(NavButtons[4],ShowVip);
   UnityAction close=()=>{if(ActionsBound&&isActiveAndEnabled&&InfoPanel.activeSelf)CloseInfo();};CloseInfoButton.onClick.AddListener(close);buttons.Add(CloseInfoButton);listeners.Add(close);ActionsBound=true;RefreshContent();
  }
  public static int[] VisibleIds(InventoryTab tab){switch(tab){case InventoryTab.Artifacts:return new[]{0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19};case InventoryTab.Boosters:return new[]{20,21,22,23};case InventoryTab.Materials:return new[]{3,6,7};case InventoryTab.Quest:return new[]{2,8,18,19};default:throw new ArgumentOutOfRangeException(nameof(tab));}}
  public void SelectTab(InventoryTab tab){var ids=VisibleIds(tab);if(!CanAct())return;SelectedTab=tab;SelectedItem=ids[0];RefreshContent();}
  public void SelectItem(int id){if(!CanAct())return;if(Array.IndexOf(VisibleIds(SelectedTab),id)<0)throw new ArgumentOutOfRangeException(nameof(id));SelectedItem=id;RefreshContent();}
  public int BoosterQuantity(int i){var s=GameState.Instance!=null?GameState.Instance.Save:null;if(i<0||i>3)throw new ArgumentOutOfRangeException(nameof(i));return s==null?0:Mathf.Max(0,new[]{s.boosterMace,s.boosterFirebird,s.boosterLivingLight,s.boosterSeaPearl}[i]);}
  public bool TryDismiss(){if(!InfoPanel.activeSelf)return false;CloseInfo();return true;}public void CloseInfo(){if(InfoPanel)InfoPanel.SetActive(false);}
  public void ShowDetails(){if(!CanAct())return;vipInfo=false;RefreshInfo();InfoPanel.SetActive(true);}public void ShowVip(){if(!CanAct())return;vipInfo=true;RefreshInfo();InfoPanel.SetActive(true);}
  static readonly string[] Ru={"Перо Жар-Птицы","Золотой амулет","Запечатанный свиток","Синие кристаллы","Синее зелье","Солнечный щит","Пучок трав","Золотые монеты","Золотой ключ","Сундук","Деревянный идол","Подзорная труба","Сапфировое кольцо","Золотая чаша","Книга мудрости","Рубиновый амулет","Гусли","Золотое яйцо","Знамя","Древняя карта","Богатырская булава","Перо Жар-Птицы","Живой свет","Жемчужина Морского царя"};
  static readonly string[] En={"Firebird Feather","Golden Amulet","Sealed Scroll","Blue Crystals","Blue Potion","Sun Shield","Herb Bundle","Gold Coins","Golden Key","Chest","Wooden Idol","Spyglass","Sapphire Ring","Golden Chalice","Book of Wisdom","Ruby Amulet","Gusli","Golden Egg","Banner","Ancient Map","Bogatyr Mace","Firebird Feather","Living Light","Sea King's Pearl"};
  public static string ItemTitle(int id){if(id<0||id>=Ru.Length)throw new ArgumentOutOfRangeException(nameof(id));return (DemoFeatureState.LanguageCode=="en"?En:Ru)[id];}
  void RefreshInfo(){bool en=DemoFeatureState.LanguageCode=="en";InfoHeading.text=vipInfo?"VIP":ItemTitle(SelectedItem);InfoDescription.text=vipInfo?(en?"VIP is not available yet. No subscription or purchase is started.":"VIP пока недоступен. Подписка или покупка не оформляется."):SelectedItem>=20?(en?"This quantity comes from your saved booster inventory. Boosters are used during gameplay. Viewing this item does not consume it or change your progress.":"Количество взято из сохранённого запаса усилителей. Они применяются во время игры. Просмотр предмета не расходует запас и не меняет прогресс."):(en?"This is a collectible design preview, not an owned item. Quantities, recipes, quest requirements and item use have not been implemented. Category groupings are examples. The coin illustration is not your wallet balance.":"Это пример коллекционного предмета, а не вещь в вашем владении. Количество, рецепты, условия заданий и применение пока не реализованы. Категории показаны для примера. Рисунок монет не отражает баланс кошелька.");CloseInfoLabel.text=en?"CLOSE":"ЗАКРЫТЬ";}
  public void RefreshContent(){if(!HasRequiredBindings)return;language=DemoFeatureState.LanguageCode;bool en=language=="en";foreach(var t in GetComponentsInChildren<Text>(true)){t.font=BodyFont;t.fontStyle=FontStyle.Normal;}Heading.font=TitleFont;HeroName.font=NamesFont;ItemName.font=TitleFont;
   Title.sprite=ProductionInventoryRuntime.Load(en?"title_blank":"title_ru");Heading.enabled=en;Heading.text=en?"INVENTORY":"ИНВЕНТАРЬ";HeroName.text=en?"Ivan":"Иван";
   string[] tabs=en?new[]{"ARTIFACTS","BOOSTERS","MATERIALS","QUEST"}:new[]{"АРТЕФАКТЫ","БУСТЕРЫ","МАТЕРИАЛЫ","КВЕСТ"};for(int i=0;i<4;i++){TabLabels[i].text=tabs[i];Tabs[i].image.sprite=ProductionInventoryRuntime.Shared(i==(int)SelectedTab?"tab_active":"tab");}
   string[] nav=en?new[]{"OFFERS","GEMS","BUNDLES","BOOSTERS","VIP"}:new[]{"ПРЕДЛОЖЕНИЯ","САМОЦВЕТЫ","НАБОРЫ","БУСТЕРЫ","VIP"};for(int i=0;i<5;i++)NavLabels[i].text=nav[i];
   PreviewLabel.text=SelectedTab==InventoryTab.Boosters?(en?"OWNED BOOSTERS · SAVED QUANTITIES":"УСИЛИТЕЛИ · КОЛИЧЕСТВО ИЗ СОХРАНЕНИЯ"):(en?"COLLECTIBLE PREVIEWS · NOT OWNED ITEMS":"ПРИМЕРЫ КОЛЛЕКЦИИ · НЕ ПРЕДМЕТЫ ВО ВЛАДЕНИИ");
   DetailsLabel.text=en?"DETAILS":"ПОДРОБНЕЕ";UseButton.interactable=false;UseLabel.text=SelectedItem>=20?(en?"IN GAMEPLAY":"В ИГРЕ"):(en?"UNAVAILABLE":"НЕДОСТУПНО");ItemName.text=ItemTitle(SelectedItem);
   Description.text=SelectedItem>=20?(en?"Owned booster. Use it during gameplay.\n\nViewing the inventory does not spend boosters.":"Усилитель из вашего запаса. Применяется во время игры.\n\nПросмотр инвентаря не расходует усилители."):(en?"Collectible design preview.\n\nOwnership, crafting and item use are not available yet.":"Пример коллекционного предмета.\n\nВладение, создание и применение предметов пока недоступны.");
   DetailArt.sprite=SelectedItem==0?ProductionInventoryRuntime.Load("feather_detail"):ProductionInventoryRuntime.ItemArt(SelectedItem);QuantityLabel.text=SelectedItem>=20?(en?"OWNED":"В НАЛИЧИИ"):(en?"PREVIEW":"ПРИМЕР");RefreshInfo();RefreshState();
  }
  public void RefreshState(){if(!HasRequiredBindings)return;var s=GameState.Instance!=null?GameState.Instance.Save:null;bool en=DemoFeatureState.LanguageCode=="en";LevelText.text=(en?"Level ":"Уровень ")+(s==null?"—":s.level.ToString());GoldText.text=s==null?"—":Number(s.gold);GemsText.text=s==null?"—":Number(s.gems);EnergyText.text=s==null?"—":s.energy+"/"+s.energyMax;QuantityText.text=SelectedItem>=20?Number(BoosterQuantity(SelectedItem-20)):"—";ProductionInventoryRuntime.RebuildContent(this);}
  public static string Number(int n)=>Mathf.Max(0,n).ToString("N0",CultureInfo.InvariantCulture).Replace(","," ");
 }
}
