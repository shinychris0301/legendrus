using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Source-based lesson overview; all gems are the live Gameplay resources.</summary>
    public sealed class ProductionTutorialOverview : MonoBehaviour
    {
        public RectTransform Root,Footer,Viewport,ScrollContent;
        public ScrollRect Scroller;
        public ProductionPlayerHeader Header;
        public Image Title,DoneArt,FirstArt,Panel;
        public Text TitleText,DoneText,FirstText,RewardsText,MovesText,ExampleText;
        public Text[] Headings,Descriptions,SpecialCaptions,GoalCaptions;
        public Button Done,First,Back;
        public Button[] Lessons;
        public Image[] DiagramGems,GoalGems;
        public bool HasBindings=>Root&&Footer&&Viewport&&ScrollContent&&Scroller&&Header&&Header.HasBindings&&Title&&Done&&First&&Back&&Lessons!=null&&Lessons.Length==3&&DiagramGems!=null&&DiagramGems.Length==24;
        public static Sprite Load(string name)=>Resources.Load<Sprite>("ProductionUI/TutorialOriginal/"+name);
        public static ProductionTutorialOverview Build(ProductionTutorialView owner)
        {
            var root=Rect(owner.Content,"OriginalOverview",0,0,941,1672);var v=root.gameObject.AddComponent<ProductionTutorialOverview>();v.Root=root;
            var shop=Resources.Load<GameObject>("ProductionUI/Pages/Shop").GetComponent<ProductionShopView>();
            v.Header=Instantiate(shop.Header,root,false);v.Header.name="SharedPlayerHeader";
            foreach(var button in v.Header.GetComponentsInChildren<Button>(true))Feedback(button);
            // Keep source-sized plus artwork while providing a usable target on narrow phones.
            foreach(var button in v.Header.Plus)button.image.raycastPadding=new Vector4(-16,-14.5f,-16,-14.5f);
            // Energy sits beside Settings; expand left only to keep those actions separate.
            v.Header.Plus[2].image.raycastPadding=new Vector4(-32,-14.5f,0,-14.5f);
            v.Title=Art(root,"Title",Load("title_ru"),172,107,612,186);v.TitleText=Text(owner,root,"LocalizedTitle",264,174,420,78,57,true);
            v.Viewport=Rect(root,"LessonViewport",0,304,941,1121);var hit=v.Viewport.gameObject.AddComponent<Image>();hit.color=Color.clear;hit.raycastTarget=true;v.Viewport.gameObject.AddComponent<RectMask2D>();
            v.ScrollContent=Rect(v.Viewport,"Lessons",0,0,941,1121);v.Scroller=v.Viewport.gameObject.AddComponent<ScrollRect>();v.Scroller.viewport=v.Viewport;v.Scroller.content=v.ScrollContent;v.Scroller.horizontal=false;v.Scroller.vertical=true;v.Scroller.movementType=ScrollRect.MovementType.Clamped;v.Scroller.scrollSensitivity=45;
            var p=v.ScrollContent;v.Panel=Art(p,"ParchmentFrame",Load("panel_ru_r4"),76,0,788,1121);
            v.Headings=new Text[3];v.Descriptions=new Text[3];v.Lessons=new Button[3];float[] ys={17,383,687};
            for(int i=0;i<3;i++){var a=Rect(p,"LessonHeading_"+i,150,ys[i],634,76).gameObject.AddComponent<Image>();a.color=Color.clear;v.Lessons[i]=Button(a);v.Headings[i]=Text(owner,a.transform,"Label",64,15,510,44,33,true);}
            var grid=Rect(p,"TeachingBoard",126,95,369,252);Art(grid,"Wood",ProductionGameplaySourceControls.Load("wood"),0,0,369,252).preserveAspect=false;
            int[] gems={0,2,3,4,1,0,0,1,1,1,3,4,2,2,0,0,4,1,3,4,2,1,0,0};v.DiagramGems=new Image[24];
            for(int i=0;i<24;i++){
                var cell=Rect(grid,"Cell_"+i,(i%6)*61.5f,(i/6)*63,61.5f,63).gameObject.AddComponent<ProductionGameplayCell>();cell.raycastTarget=false;
                v.DiagramGems[i]=Art(grid,"GameplayGem_"+i,ProductionGameplayArt.Gem(gems[i]),(i%6)*61.5f+5,(i/6)*63+5,51.5f,53);
            }
            var rails=Rect(grid,"SourceRails",0,0,369,252).gameObject.AddComponent<ProductionGameplayCell>();rails.GridOnly=true;rails.Columns=6;rails.Rows=4;rails.sprite=ProductionGameplaySourceControls.Load("grid");rails.raycastTarget=false;
            // A restrained line marks the three blue gems, without substituting different gem artwork.
            var line=Rect(grid,"MatchedLine",69,95,170,3).gameObject.AddComponent<Image>();line.color=new Color32(250,194,70,230);line.raycastTarget=false;
            v.Descriptions[0]=Ink(owner,p,"MatchDescription",540,147,267,170,28);
            v.SpecialCaptions=new Text[3];Sprite[] specials={ProductionGameplayArt.LineSpecial(1),ProductionGameplayArt.Load("color"),ProductionGameplayArt.Load("burst")};
            for(int i=0;i<3;i++){float x=116+i*156;Art(p,"GameplaySpecial_"+i,specials[i],x+16,i==0?502:477,112,106);v.SpecialCaptions[i]=Ink(owner,p,"SpecialCaption_"+i,x-3,606,152,64,21);v.SpecialCaptions[i].alignment=TextAnchor.UpperCenter;}
            v.Descriptions[1]=Ink(owner,p,"SpecialDescription",598,475,211,169,26);
            v.GoalCaptions=new Text[3];v.GoalGems=new Image[2];
            for(int i=0;i<3;i++){float x=116+i*118;var icon=Art(p,"GoalIcon_"+i,i<2?ProductionGameplayArt.Gem(i):ProductionGameplayArt.Load("crate"),x+12,777,88,92);if(i<2)v.GoalGems[i]=icon;v.GoalCaptions[i]=Ink(owner,p,"GoalCaption_"+i,x-2,889,118,62,22);v.GoalCaptions[i].alignment=TextAnchor.UpperCenter;}
            v.MovesText=Text(owner,p,"MovesExample",519,812,60,24,18,true);v.ExampleText=Ink(owner,p,"ExampleLabel",486,925,127,25,16);v.ExampleText.alignment=TextAnchor.MiddleCenter;
            v.Descriptions[2]=Ink(owner,p,"GoalsDescription",621,784,189,169,25);
            v.RewardsText=Text(owner,p,"RewardsLabel",397,960,145,29,25,true);
            Art(p,"ApprovedHomeGift",Load("gift_medallion_r5"),250,1000,130,86);
            Art(p,"ApprovedCoins",Resources.Load<Sprite>("ProductionUI/MailAligned/coin"),432,998,110,80);
            Art(p,"GameplayRewardGem",ProductionGameplayArt.Gem(1),610,993,91,91);
            v.Footer=Rect(root,"Footer",0,1425,941,247);v.DoneArt=Art(v.Footer,"Done",Load("done_ru_r5"),202,-10,536,142);v.Done=Button(v.DoneArt);v.DoneText=Text(owner,v.Done.transform,"Label",98,27,346,77,55,true);
            v.FirstArt=Art(v.Footer,"FirstLevel",Load("first_ru_r5"),248,126,452,83);v.First=Button(v.FirstArt);v.FirstText=Text(owner,v.First.transform,"Label",62,20,330,42,30,true);
            v.Back=Button(Art(v.Footer,"Back",ProductionSettingsRuntime.Load("back"),24,65,126,125));return v;
        }
        public void Refresh(bool returnToRound)
        {
            bool en=DemoFeatureState.LanguageCode=="en";Panel.sprite=Load(en?"panel_r4":"panel_ru_r4");Header.RefreshValues();Title.sprite=Load(en?"title":"title_ru");TitleText.enabled=en;TitleText.text="TUTORIAL";
            DoneArt.sprite=Load(en?"done_r5":"done_ru_r5");DoneText.enabled=en;DoneText.text="GOT IT";
            FirstArt.sprite=Load(en||returnToRound?"first_r5":"first_ru_r5");FirstText.enabled=en||returnToRound;FirstText.text=returnToRound?(en?"RETURN TO ROUND":"ВЕРНУТЬСЯ В РАУНД"):"TO THE FIRST LEVEL";
            string[] headings=en?new[]{"1. MATCH GEMS","2. CREATE SPECIALS","3. COMPLETE GOALS"}:new[]{"1. СОБИРАЙТЕ САМОЦВЕТЫ","2. СОЗДАВАЙТЕ БУСТЕРЫ","3. ВЫПОЛНЯЙТЕ ЦЕЛИ"};
            string[] descriptions=en?new[]{"Swap neighboring gems to match three or more in a row.","Matches of four, five or intersecting lines create special pieces. Match or swap them to activate.","Each level has goals. Complete them before you run out of moves to win rewards."}:new[]{"Меняйте местами соседние самоцветы, чтобы собрать 3 или больше в ряд.","Ряды из 4, 5 и пересечения создают бустеры. Активируйте их совпадением или обменом.","На каждом уровне есть цели. Выполните их до окончания ходов, чтобы получить награды."};
            string[] specials=en?new[]{"4 in a row\nLine clear","5 in a row\nColor special","T or L\nArea blast"}:new[]{"4 в ряд\nЛинейный взрыв","5 в ряд\nЦветовой бустер","Т или Г\nВзрыв области"};
            string[] goals=en?new[]{"Collect\nred gems","Collect\nblue gems","Clear\ncrates"}:new[]{"Соберите\nкрасные","Соберите\nсиние","Уберите\nящики"};
            for(int i=0;i<3;i++){Headings[i].text=headings[i];Headings[i].enabled=en;Descriptions[i].text=descriptions[i];Descriptions[i].enabled=en||i==1;SpecialCaptions[i].text=specials[i];GoalCaptions[i].text=goals[i];GoalCaptions[i].enabled=en||i==2;}
            // The source medallion keeps its original illustrative 28; only its caption localizes.
            MovesText.enabled=en;MovesText.text="MOVES";ExampleText.text=en?"Examples":"Примеры";
            RewardsText.enabled=en;RewardsText.text="REWARDS";
            foreach(var b in Header.GetComponentsInChildren<Button>(true))b.interactable=!returnToRound;
        }
        public void ApplyLayout(float safeWidth,float safeHeight,Vector2 topLeft)
        {
            float scale=Mathf.Max(.1f,safeWidth/941);float height=safeHeight/scale;Root.localScale=Vector3.one*scale;Root.sizeDelta=new Vector2(941,height);Root.anchoredPosition=topLeft;
            Footer.anchoredPosition=new Vector2(0,-(height-247));Viewport.sizeDelta=new Vector2(941,Mathf.Max(120,height-304-247));
        }
        static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h){var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Tutorial original art: "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Text Text(ProductionTutorialView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(244,211,145,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.resizeTextForBestFit=true;t.resizeTextMinSize=Mathf.Max(14,size-7);t.resizeTextMaxSize=size;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
        static Text Ink(ProductionTutorialView v,Transform p,string n,float x,float y,float w,float h,int size){var t=Text(v,p,n,x,y,w,h,size);t.color=new Color32(42,25,10,255);t.alignment=TextAnchor.MiddleLeft;return t;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};Feedback(b);return b;}
        static void Feedback(Button b){b.transition=Selectable.Transition.None;if(!b.GetComponent<ProductionMapButtonFeedback>())b.gameObject.AddComponent<ProductionMapButtonFeedback>();}
    }
}
