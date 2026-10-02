using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public sealed class ProductionTutorialView : MonoBehaviour
    {
        public const int CurrentRevision=7;
        public int Revision=CurrentRevision;
        public RectTransform Content,Body;
        public ProductionTutorialOverview Overview;
        public bool DetailsVisible {get;private set;}
        public bool ReturnToRound {get;private set;}
        public GameObject MatchDiagram;
        public Text BeforeLabel,AfterLabel,MatchInstruction;
        public Image Environment;
        public Font BodyFont,TitleFont;
        public Text Heading,Subtitle,Hint,Counter,PreviousLabel,NextLabel;
        public Button Close,Previous,Next;
        public Button[] Tabs;
        public Text[] TabLabels,RowTitles,RowDescriptions;
        public Image[] RowIcons;
        public GameObject[] Rows;
        public int Section {get;private set;}
        public bool ActionsBound {get;private set;}
        readonly List<Button> buttons=new List<Button>();
        readonly List<UnityAction> listeners=new List<UnityAction>();
        string language;bool committed;Action exit;
        public bool HasRequiredBindings=>Revision==CurrentRevision&&Content&&Body&&Overview&&Overview.HasBindings&&MatchDiagram&&BeforeLabel&&AfterLabel&&MatchInstruction&&Environment&&BodyFont&&TitleFont&&Heading&&Subtitle&&Hint&&Counter&&PreviousLabel&&NextLabel&&Close&&Previous&&Next&&Tabs!=null&&Tabs.Length==4&&TabLabels!=null&&TabLabels.Length==4&&Rows!=null&&Rows.Length==4&&RowIcons!=null&&RowIcons.Length==4&&RowTitles!=null&&RowTitles.Length==4&&RowDescriptions!=null&&RowDescriptions.Length==4;
        void OnEnable(){RefreshContent();}
        void OnDisable(){Unbind();}
        void Update(){if(language!=DemoFeatureState.LanguageCode)RefreshContent();}
        void Unbind(){for(int i=0;i<buttons.Count;i++)if(buttons[i])buttons[i].onClick.RemoveListener(listeners[i]);buttons.Clear();listeners.Clear();ActionsBound=false;}
        bool Ready()=>ActionsBound&&!committed&&isActiveAndEnabled&&(ScreenRouter.Instance==null||(!ScreenRouter.Instance.IsScreenTransitioning&&(ReturnToRound?ScreenRouter.Instance.ActiveModal==ScreenId.TutorialOverlay&&!ScreenRouter.Instance.IsModalTransitioning:!ScreenRouter.Instance.HasActiveModalBlocker)));
        void Listen(Button b,Action a){UnityAction call=()=>{if(Ready()&&b.IsInteractable())a();};b.onClick.AddListener(call);buttons.Add(b);listeners.Add(call);}
        public void Bind(Action back,Action firstLevel=null,Action<ScreenId> navigate=null,bool returnToRound=false,Action<string> openShop=null)
        {
            Unbind();exit=back;committed=false;ReturnToRound=returnToRound;
            Listen(Close,()=>{if(DetailsVisible)ShowOverview();else Leave();});
            Listen(Previous,()=>SelectSection(Section-1));Listen(Next,()=>{if(Section==3)ShowOverview();else SelectSection(Section+1);});
            for(int i=0;i<4;i++){int j=i;Listen(Tabs[i],()=>SelectSection(j));}
            Listen(Overview.Done,Leave);Listen(Overview.Back,Leave);
            Overview.First.interactable=firstLevel!=null;
            Listen(Overview.First,()=>{committed=true;firstLevel?.Invoke();});
            for(int i=0;i<3;i++){int section=new[]{0,1,3}[i];Listen(Overview.Lessons[i],()=>ShowDetails(section));}
            if(navigate!=null){Listen(Overview.Header.Profile,()=>Route(navigate,ScreenId.Profile));Listen(Overview.Header.Settings,()=>Route(navigate,ScreenId.Settings));for(int i=0;i<3;i++){int j=i;Listen(Overview.Header.Plus[i],()=>{if(j<2&&openShop!=null){committed=true;openShop(j==0?"coins":"gems");}else Route(navigate,j==2?ScreenId.OutOfEnergy:ScreenId.Shop);});}}
            ActionsBound=true;RefreshContent();
        }
        void Route(Action<ScreenId> navigate,ScreenId page){if(ReturnToRound)return;committed=true;navigate(page);}
        public void ShowDetails(int section=0){DetailsVisible=true;SelectSection(section);}
        public void ShowOverview(){DetailsVisible=false;RefreshContent();}
        public bool TryDismissDetails(){if(!DetailsVisible)return false;ShowOverview();return true;}
        void Leave(){committed=true;exit?.Invoke();}
        public void SelectSection(int section){Section=Mathf.Clamp(section,0,3);RefreshContent();}
        public void RefreshContent()
        {
            if(!HasRequiredBindings)return;
            language=DemoFeatureState.LanguageCode;bool en=language=="en";Body.gameObject.SetActive(DetailsVisible);Overview.gameObject.SetActive(!DetailsVisible);Overview.Refresh(ReturnToRound);
            string[] topics=en?new[]{"BASICS","SPECIALS","BOOSTERS","OBSTACLES"}:new[]{"ОСНОВЫ","КОМБИНАЦИИ","БУСТЕРЫ","ПРЕГРАДЫ"};
            Heading.text=en?"HOW TO PLAY":"КАК ИГРАТЬ";Subtitle.text=Section==0?(en?"Match three in a row":"Соберите три в ряд"):topics[Section];Counter.text=(Section+1)+" / 4";PreviousLabel.text=en?"PREVIOUS":"НАЗАД";NextLabel.text=Section==3?(en?"DONE":"ПОНЯТНО"):(en?"NEXT":"ДАЛЕЕ");Previous.interactable=Section>0;Previous.gameObject.SetActive(Section>0);
            MatchDiagram.SetActive(Section==0);
            BeforeLabel.text=en?"Swap":"Обмен";AfterLabel.text=en?"Match":"Совпадение";
            MatchInstruction.text=en?"Swap neighboring gems.\nMake lines of three or more.":"Меняйте соседние самоцветы местами.\nСобирайте линии из трёх и более.";
            Next.GetComponent<RectTransform>().anchoredPosition=new Vector2(Section==0?245:467,Section==0?-1420:-1451);
            Next.GetComponent<RectTransform>().sizeDelta=new Vector2(Section==0?450:360,145);
            NextLabel.rectTransform.anchoredPosition=new Vector2(Section==0?64:51,-17);NextLabel.rectTransform.sizeDelta=new Vector2(Section==0?319:255,Section==0?60:48);
            for(int i=0;i<4;i++){TabLabels[i].text=topics[i];Tabs[i].transform.Find("SelectedFace").gameObject.SetActive(i==Section);Tabs[i].GetComponent<Image>().color=i==Section?new Color(.55f,.8f,1,1):new Color(.65f,.65f,.65f,1);}
            string[] titles,details;Sprite[] icons;
            if(Section==0){
                titles=en?new[]{"Complete the goals","Watch your moves"}:new[]{"Выполняйте цели","Берегите ходы"};
                details=en?new[]{"Collect the required items.","Complete the goals within the available moves."}:new[]{"Соберите нужные предметы.","Достигните целей за отведённые ходы."};
                icons=new[]{ProductionGameplayArt.Gem(2),ProductionGameplayArt.Load("hud_moves_r10")};
                Hint.text="";
            }else if(Section==1){
                titles=en?new[]{"FOUR IN A LINE","T OR L MATCH","FIVE IN A LINE"}:new[]{"ЧЕТЫРЕ В РЯД","СОВПАДЕНИЕ Т ИЛИ Г","ПЯТЬ В РЯД"};
                details=en?new[]{"Creates a line special. Match it to clear a whole row or column; its orientation shows the direction.","Intersecting lines create an area special. Match it to clear the surrounding 3×3 cells.","Creates a color special. Swap it with a normal gem to clear gems of that color."}:new[]{"Создаёт линейный бустер. Соберите с ним совпадение, чтобы очистить ряд или столбец. Направление видно по его положению.","Пересечение рядов создаёт взрывной бустер. Совпадение с ним очищает область 3×3 вокруг него.","Создаёт цветовой бустер. Поменяйте его с обычным самоцветом, чтобы убрать самоцветы этого цвета."};
                icons=new[]{ProductionGameplayArt.LineSpecial(1),ProductionGameplayArt.Load("burst"),ProductionGameplayArt.Load("color")};
                Hint.text=en?"Match-created specials do not use your inventory.":"Бустеры от совпадений не расходуют ваш запас.";
            }else if(Section==2){
                titles=en?new[]{"BOGATYR MACE","FIREBIRD FEATHER","LIVING LIGHT","SEA KING’S PEARL"}:new[]{"БОГАТЫРСКАЯ БУЛАВА","ПЕРО ЖАР-ПТИЦЫ","ЖИВОЙ СВЕТ","ЖЕМЧУЖИНА МОРСКОГО ЦАРЯ"};
                details=en?new[]{"Choose the booster, then tap a cell to clear its row. Swipe up or down to clear its column.","Choose a cell to clear a 3×3 area around it.","Choose a gem to clear all gems of that color.","Adds five moves to the current round."}:new[]{"Выберите бустер и коснитесь клетки: очистится ряд. Проведите вверх или вниз: столбец.","Выберите клетку, чтобы очистить область 3×3 вокруг неё.","Выберите самоцвет, чтобы убрать все самоцветы этого цвета.","Добавляет пять ходов в текущем раунде."};
                icons=new[]{ProductionBoosterArt.Icon(0,true),ProductionBoosterArt.Icon(1,true),ProductionBoosterArt.Icon(2,true),ProductionBoosterArt.Icon(3,true)};
                Hint.text=en?"Inventory boosters are used during a round, not in this guide.":"Бустеры из запаса применяются в раунде, а не в справке.";
            }else{
                titles=en?new[]{"WOODEN CRATES","ICE","CHAINS"}:new[]{"ДЕРЕВЯННЫЕ ЯЩИКИ","ЛЁД","ЦЕПИ"};
                details=en?new[]{"Make matches beside a crate or hit it with a booster. Each hit removes one layer: a normal crate takes one hit, a reinforced crate takes two.","Match the gem on an icy cell or hit that cell with a booster to remove one ice layer.","A chained gem cannot be swapped. Include it in a match or hit it with a booster to remove a chain layer."}:new[]{"Собирайте совпадения рядом или бейте бустером. Удар убирает один слой: обычный ящик — один удар, усиленный — два.","Соберите совпадение с самоцветом на льду или ударьте по клетке бустером, чтобы убрать один слой льда.","Самоцвет в цепях нельзя менять местами. Включите его в совпадение или ударьте бустером, чтобы убрать слой цепи."};
                icons=new[]{ProductionGameplayArt.Load("crate"),ProductionGameplayArt.Load("ice"),ProductionGameplayArt.Load("chain")};
                Hint.text=en?"No possible moves? The board reshuffles automatically.":"Нет возможных ходов? Поле перемешается автоматически.";
            }
            for(int i=0;i<4;i++){Rows[i].SetActive(i<titles.Length);if(i>=titles.Length)continue;RowTitles[i].text=titles[i];RowDescriptions[i].text=details[i];RowIcons[i].sprite=icons[i];RowIcons[i].preserveAspect=true;
                var row=(RectTransform)Rows[i].transform;
                row.anchoredPosition=new Vector2(107,-(Section==0?995+i*174:455+i*202));
                row.sizeDelta=new Vector2(728,Section==0?160:192);
                var face=(RectTransform)row.Find("Card");face.sizeDelta=row.sizeDelta;
                RowTitles[i].rectTransform.anchoredPosition=new Vector2(178,Section==0?-16:-12);
                RowDescriptions[i].rectTransform.anchoredPosition=new Vector2(178,Section==0?-73:-66);
                RowDescriptions[i].rectTransform.sizeDelta=new Vector2(526,Section==0?74:114);
                RowTitles[i].fontSize=Section==0?37:29;RowTitles[i].resizeTextMaxSize=RowTitles[i].fontSize;RowTitles[i].resizeTextMinSize=Section==0?32:25;
                RowDescriptions[i].fontSize=Section==0?32:28;RowDescriptions[i].resizeTextMaxSize=RowDescriptions[i].fontSize;
                RowIcons[i].rectTransform.anchoredPosition=new Vector2(20,Section==0?-21:-30);
                RowIcons[i].rectTransform.sizeDelta=new Vector2(134,Section==0?118:136);
                var moves=row.Find("MovesExample");moves.gameObject.SetActive(Section==0&&i==1);
                if(Section==0&&i==1){RowIcons[i].rectTransform.anchoredPosition=new Vector2(52,-12);RowIcons[i].rectTransform.sizeDelta=new Vector2(72,136);moves.GetComponent<Text>().text=en?"<size=18>MOVES</size>\n<size=35>20</size>":"<size=18>ХОДЫ</size>\n<size=35>20</size>";}
            }
        }
    }
}
