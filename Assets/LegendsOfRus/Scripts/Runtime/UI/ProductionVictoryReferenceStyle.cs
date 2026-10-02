using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Source-derived Victory composition; existing completion services own all values.</summary>
    public static class ProductionVictoryReferenceStyle
    {
        public const string Root="ProductionUI/VictoryReferenceR1/";
        public static readonly string[] ArtNames={"environment","title","firebird","star","star_empty","panel","card","resource_card","coin","gem","continue","video","footer","title_ru","continue_ru","panel_ru","video_ru","firebird_glow"};
        public static Sprite Load(string name)=>Resources.Load<Sprite>(Root+name);
        public static void Apply(ProductionVictoryView v)
        {
            v.Hud.gameObject.SetActive(false); // This reference has a celebration title, not a wallet header.
            v.Environment.sprite=Load("environment");v.Firebird.sprite=Load("firebird");
            var banner=v.Body.Find("VictoryBanner").GetComponent<Image>();banner.sprite=Load("title");
            Place(banner.rectTransform,158,21,632,222);Place(v.Firebird.rectTransform,12,70,917,643);
            var glowTransform=v.Body.Find("FirebirdGlow");
            if(glowTransform==null)
            {
                var go=new GameObject("FirebirdGlow",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image));
                go.transform.SetParent(v.Body,false);glowTransform=go.transform;
                var rect=(RectTransform)glowTransform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
            }
            var birdGlow=glowTransform.GetComponent<Image>();birdGlow.sprite=Load("firebird_glow");birdGlow.raycastTarget=false;
            Place(birdGlow.rectTransform,0,0,941,828);
            glowTransform.SetAsFirstSibling();banner.transform.SetSiblingIndex(1);v.Title.transform.SetSiblingIndex(2);v.Firebird.transform.SetSiblingIndex(3);
            Text(v.Title,246,114,452,94,94);v.Title.fontStyle=FontStyle.Normal;
            var lettering=v.Title.GetComponent<ProductionVictoryTitleLettering>();lettering.ArcHeight=0;lettering.BevelWidth=1.15f;
            lettering.BottomColor=new Color32(153,88,19,255);lettering.BodyColor=new Color32(223,161,64,255);
            lettering.HighlightColor=new Color32(255,230,158,255);lettering.TopColor=new Color32(242,203,113,255);lettering.Refresh();
            v.Body.sizeDelta=new Vector2(941,1360);
            for(int i=0;i<3;i++)
            {
                float size=i==1?212:190,x=i==0?180:i==1?369:579,y=i==1?598:641;
                var star=v.Stars[i];star.StarFilledArt=Load("star");star.StarEmptyArt=Load("star_empty");star.color=Color.white;
                foreach(var fx in star.GetComponents<BaseMeshEffect>())fx.enabled=false;
                Place(star.rectTransform,x,y,size,size);Place(v.StarGlows[i].rectTransform,x+size*.5f-225,y+size*.5f-225,450,450);
                star.SetVerticesDirty();star.SetMaterialDirty();
            }
            Place(v.RewardsArea,0,828,941,532);Place(v.RewardsPanel.rectTransform,90,0,763,532);
            v.RewardsPanel.sprite=Load("panel");v.RewardsPanel.GetComponent<ProductionVictoryPanelFit>().enabled=false;
            v.RewardsPanel.type=Image.Type.Simple;
            Text(v.Score,235,47,471,80,82);GoldScore(v);Text(v.RewardsHeading,300,142,341,35,28);
            Text(v.RewardStatus,141,285,660,20,14);
            var scoreHeading=Label(v,"ScoreHeading");Text(scoreHeading,310,22,321,32,27);
            var boosterHeading=Label(v,"BoosterHeading");Text(boosterHeading,270,306,401,28,26);
            var empty=Label(v,"BoosterEmpty");Text(empty,190,370,561,82,23);empty.horizontalOverflow=HorizontalWrapMode.Wrap;
            foreach(var card in v.RewardCards)
            {card.GetComponent<ProductionVictoryPanelFit>().enabled=false;card.GetComponent<Image>().type=Image.Type.Sliced;}
            Place(v.MessageInset.rectTransform,132,285,677,21);v.MessageInset.color=Color.clear;
            Text(v.Reaction,142,286,657,19,14);Text(v.Status,142,286,657,19,14);
            v.Footer.sizeDelta=new Vector2(941,312);
            var button=v.ContinueButton.GetComponent<Image>();button.sprite=Load("continue");button.preserveAspect=true;
            Place(button.rectTransform,157,0,629,121);Text(v.ContinueText,55,28,519,66,54);
            v.ContinueText.fontStyle=FontStyle.Normal;
            var video=v.DoubleButton.GetComponent<Image>();video.sprite=Load("video");video.preserveAspect=true;
            Place(video.rectTransform,94,121,752,142);Text(v.DoubleText,208,25,460,92,32);v.DoubleText.color=new Color32(255,226,166,255);
            v.FooterOrnament.sprite=Load("footer");Place(v.FooterOrnament.rectTransform,317,252,308,60);
        }
        static void GoldScore(ProductionVictoryView v)
        {
            var effect=v.Score.GetComponent<ProductionHomeGoldText>();
            if(effect==null)effect=v.Score.gameObject.AddComponent<ProductionHomeGoldText>();
            effect.enabled=true;effect.BeveledFace=true;
            effect.TopColor=new Color32(255,234,167,255);effect.BottomColor=new Color32(180,113,32,255);
            v.Score.material=v.Title.material;v.Score.color=Color.white;v.Score.fontStyle=FontStyle.Normal;
        }
        static Text Label(ProductionVictoryView v,string name)
        {
            var found=v.RewardsArea.Find(name);if(found!=null)return found.GetComponent<Text>();
            var go=Object.Instantiate(v.RewardsHeading.gameObject,v.RewardsArea);go.name=name;return go.GetComponent<Text>();
        }
        public static void Refresh(ProductionVictoryView v,int count,bool current)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            v.Body.Find("VictoryBanner").GetComponent<Image>().sprite=Load(en?"title":"title_ru");
            v.ContinueButton.GetComponent<Image>().sprite=Load(en?"continue":"continue_ru");
            v.RewardsPanel.sprite=Load(en?"panel":"panel_ru");
            v.Title.enabled=en;v.ContinueText.enabled=en;v.RewardsHeading.enabled=en;
            v.RewardsArea.Find("ScoreHeading").GetComponent<Text>().enabled=en;
            v.RewardsArea.Find("BoosterHeading").GetComponent<Text>().enabled=en;
            bool sourceVideo=!en&&v.DoubleButton.interactable;
            v.DoubleButton.GetComponent<Image>().sprite=Load(sourceVideo?"video_ru":"video");
            v.DoubleText.enabled=!sourceVideo;
            Text(v.DoubleText,208,25,460,92,32);
            var result=GameState.Instance!=null?GameState.Instance.LastCompletionResult:null;
            v.Score.text=current?Mathf.Max(0,result.Score).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," "):"—";
            v.RewardsHeading.text=en?"REWARDS":"НАГРАДЫ";
            v.RewardsArea.Find("ScoreHeading").GetComponent<Text>().text=en?"SCORE":"СЧЁТ";
            v.RewardsArea.Find("BoosterHeading").GetComponent<Text>().text=en?"BOOSTERS":"БУСТЕРЫ";
            var empty=v.RewardsArea.Find("BoosterEmpty").GetComponent<Text>();empty.gameObject.SetActive(count==2);
            empty.text=en?"No boosters awarded this round":"В этом раунде бустеры не начислены";
            v.RewardIcons[0].sprite=Load("coin");v.RewardIcons[1].sprite=Load("gem");
            // Share the narrow feedback line; never place the story over lower gold carving.
            bool explainBonus=v.DoubleButton.interactable&&string.IsNullOrEmpty(v.Status.text);
            v.RewardStatus.gameObject.SetActive(explainBonus||!v.MessageInset.enabled);
            if(explainBonus)
            {
                v.RewardStatus.text=en?"Video bonus doubles coins only":"Видеобонус удваивает только монеты";
                v.Reaction.gameObject.SetActive(false);
            }
            int boosters=count-2;
            for(int i=0;i<count;i++)
            {
                var card=v.RewardCards[i];var image=card.GetComponent<Image>();v.RewardLabels[i].gameObject.SetActive(false);
                if(i<2)
                {
                    image.sprite=Load("resource_card");Place(card,235+i*240,180,231,104);
                    var s=v.RewardIcons[i].sprite;float fit=Mathf.Min(78/s.rect.width,78/s.rect.height);
                    Place(v.RewardIcons[i].rectTransform,18+(78-s.rect.width*fit)*.5f,13+(78-s.rect.height*fit)*.5f,s.rect.width*fit,s.rect.height*fit);
                    Text(v.RewardValues[i],101,26,115,51,35);
                }
                else
                {
                    const float width=131;float start=(941-(boosters*width+(boosters-1)*25))*.5f;
                    image.sprite=Load("card");Place(card,start+(i-2)*(width+25),336,width,171);
                    var s=v.RewardIcons[i].sprite;float fit=Mathf.Min(105/s.rect.width,109/s.rect.height);
                    Place(v.RewardIcons[i].rectTransform,(width-s.rect.width*fit)*.5f,8+(109-s.rect.height*fit)*.5f,s.rect.width*fit,s.rect.height*fit);
                    Text(v.RewardValues[i],10,132,width-20,25,29);
                }
            }
            foreach(var star in v.Stars)star.SetMaterialDirty();
        }
        public static void Layout(ProductionVictoryView v,float top,float bottom)
        {
            float scale=Mathf.Min(1,(top-bottom)/1672f);
            float x=941*(1-scale)*.5f;
            v.Body.localScale=v.Footer.localScale=Vector3.one*scale;
            float extra=Mathf.Max(0,top-bottom-1672*scale);
            float groupTop=top-extra*.5f;
            v.Body.anchoredPosition=new Vector2(x,groupTop);
            v.Footer.anchoredPosition=new Vector2(x,groupTop-1360*scale);
            v.FooterOrnament.rectTransform.anchoredPosition=new Vector2(317,(bottom-v.Footer.anchoredPosition.y)/scale+60);
        }
        static void Place(RectTransform r,float x,float y,float w,float h)=>ProductionVictoryView.Place(r,x,y,w,h);
        static void Text(Text t,float x,float y,float w,float h,int size)=>ProductionVictoryView.PlaceText(t,x,y,w,h,size);
    }
}
