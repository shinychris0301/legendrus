using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    /// <summary>Upgrades the existing serialized HUD; all controls and board mounts retain identity.</summary>
    public static class ProductionGameplayHudStyle
    {
        public static void Apply(ProductionGameplayView v)
        {
            // Keep hidden Image components: RefreshState still uses their fillAmount values.
            v.GoalProgress.sprite = null; v.StarProgress.sprite = null;
            v.Hud.Find("GoalProgressFrame").GetComponent<Image>().sprite = null;
            v.Hud.Find("StarProgressFrame").GetComponent<Image>().sprite = null;
            // Retain old objects and IDs; the new composition uses independent panel sprites.
            v.Hud.Find("HudPanel").gameObject.SetActive(false);
            Image oldScore=v.Footer.Find("ScorePanel").GetComponent<Image>();oldScore.enabled=false;
            v.ScoreFrame=oldScore.GetComponent<ProductionGameplayScoreFrame>()??oldScore.gameObject.AddComponent<ProductionGameplayScoreFrame>();
            v.ScoreFrame.enabled=false;
            v.Footer.Find("ScoreInset").GetComponent<ProductionGameplayCell>().enabled=false;
            v.Hud.Find("GoalPanel").GetComponent<ProductionGameplayCell>().enabled=false;
            v.Hud.Find("GoalLacquer").gameObject.SetActive(false);
            v.Hud.Find("GoalProgressFrame").gameObject.SetActive(false);v.GoalProgress.gameObject.SetActive(false);
            var portraitFrame=v.Hud.Find("IvanFrame").GetComponent<Image>();
            portraitFrame.sprite=ProductionGameplayArt.Load("hud_portrait_r10");
            Place(portraitFrame.rectTransform,0,0,232,232);
            var clip=v.Hud.Find("PortraitClip").GetComponent<ProductionHomePanel>();Place(clip.rectTransform,28,24,178,178);clip.Radius=89;
            Place(clip.transform.Find("IvanPortrait").GetComponent<RectTransform>(),0,0,178,178);
            Panel(v.Hud,"HeroNamePlate","panel_score_r6",10,228,236,48);
            HeaderText(v.HeroName,25,237,204,29,25,false);
            v.HeroName.gameObject.SetActive(false);
            v.Hud.Find("HeroNamePlate").gameObject.SetActive(false);
            Panel(v.Hud,"RoundPlate","panel_score_r6",409,238,204,40);
            HeaderText(v.LevelText,420,244,182,28,22,true);v.LevelText.transform.SetAsLastSibling();
            var moves=v.Hud.Find("MovesMedallion").GetComponent<Image>();moves.sprite=ProductionGameplayArt.Load("hud_moves_r10");moves.preserveAspect=false;
            Place(moves.rectTransform,232,60,142,174);
            HeaderText(v.MovesHeading,244,79,118,31,26,true);
            HeaderText(v.MovesValue,246,108,114,91,82,true);
            Panel(v.Hud,"MissionParchment","hud_goals_r10",380,60,338,174).transform.SetAsFirstSibling();
            HeaderText(v.GoalsHeading,409,78,280,31,27,true);Ink(v.GoalsHeading);
            Panel(v.Hud,"TopScorePanel","hud_score_r10",724,60,205,174).transform.SetAsFirstSibling();
            v.ScoreHeading.transform.SetParent(v.Hud,false);v.ScoreValue.transform.SetParent(v.Hud,false);
            HeaderText(v.ScoreHeading,744,77,165,31,27,true);
            HeaderText(v.ScoreValue,739,115,176,43,35,true);
            Place(v.Hud.Find("StarProgressFrame").GetComponent<RectTransform>(),745,168,161,22);
            Place(v.StarProgress.rectTransform,751,174,149,10);
            v.StarProgress.gameObject.SetActive(false);v.Hud.Find("StarProgressFrame").gameObject.SetActive(false);
            var meterNode=v.Hud.Find("ScoreMeter");
            v.ScoreMeter=meterNode!=null?meterNode.GetComponent<ProductionGameplayScoreMeter>():ProductionGameplayRuntime.Rect(v.Hud,"ScoreMeter",744,169,164,26).gameObject.AddComponent<ProductionGameplayScoreMeter>();
            v.ScoreMeter.raycastTarget=false;
            for(int i=0;i<3;i++)Place(v.Stars[i].rectTransform,743+i*56,200,46,46);
            for(int i=0;i<v.Goals.Length;i++)
            {
                var g=v.Goals[i];
                g.Icon.rectTransform.pivot=new Vector2(.5f,.5f);Place(g.Icon.rectTransform,49,32,60,64);
                HeaderText(g.Count,0,65,98,33,28,true);Ink(g.Count);
                g.Description.enabled=false;
                var found=g.Root.Find("GoalSymbol");
                g.Symbol=found!=null?found.GetComponent<ProductionGameplayGoalSymbol>():ProductionGameplayRuntime.Rect(g.Root,"GoalSymbol",22,19,50,58).gameObject.AddComponent<ProductionGameplayGoalSymbol>();
                g.Symbol.raycastTarget=false;g.Symbol.enabled=false;
            }
            LayoutGoals(v,2);
            for(int i=0;i<4;i++)
            {
                Image card=Panel(v.Footer,"BoosterCard_"+i,"card_wood_r9",82+i*192,0,180,252);
                card.type=Image.Type.Sliced;card.pixelsPerUnitMultiplier=4.5f;card.fillCenter=true;
                var hit=card.GetComponent<ProductionGameplayBoosterCard>()??card.gameObject.AddComponent<ProductionGameplayBoosterCard>();hit.Target=v.BoosterButtons[i];card.raycastTarget=true;
                var face=v.BoosterButtons[i].GetComponent<Image>();face.transform.SetParent(card.transform,false);Place(face.rectTransform,14,22,152,156);
                var badge=v.BoosterCounts[i].transform.parent.GetComponent<RectTransform>();badge.SetParent(card.transform,false);Place(badge,125,134,45,45);
                HeaderText(v.BoosterCounts[i],0,0,45,45,30,true);
                v.BoosterCounts[i].resizeTextForBestFit=true;v.BoosterCounts[i].resizeTextMinSize=30;v.BoosterCounts[i].resizeTextMaxSize=64;
                var found=card.transform.Find("BoosterLabel");
                Text label=found!=null?found.GetComponent<Text>():ProductionGameplayRuntime.Rect(card.transform,"BoosterLabel",0,0,1,1).gameObject.AddComponent<Text>();
                HeaderText(label,13,187,154,46,18,true);label.font=v.Typography.RegularFont;label.horizontalOverflow=HorizontalWrapMode.Wrap;label.verticalOverflow=VerticalWrapMode.Truncate;
                v.ArmedMarkers[i].transform.SetParent(card.transform,false);Place(v.ArmedMarkers[i].rectTransform,7,8,166,176);
                v.ArmedMarkers[i].transform.SetAsFirstSibling();
            }
            v.PauseButton.GetComponent<Image>().sprite=ProductionGameplayArt.Load("pause_gear_r3");
            Place(v.PauseButton.GetComponent<RectTransform>(),838,240,91,96);
            Place(v.BoardArea,68,486,808,748);Place(v.BoardMount,0,0,808,748);
            var wood=v.BoardArea.Find("BoardLacquer").GetComponent<Image>();
            wood.sprite=ProductionGameplayArt.Load("board_wood_r7");wood.preserveAspect=false;wood.raycastTarget=false;
            Place(wood.rectTransform,0,0,808,748);
            var boardFrame=v.BoardArea.Find("BoardFrame").GetComponent<RectTransform>();
            Place(boardFrame,-14,-10,836,768);
            // Retain the serialized frame and its component identity while using the isolated carved artwork.
            boardFrame.GetComponent<ProductionGameplayCell>().enabled=false;
            var carving=Panel(boardFrame,"CarvedFrame","board_frame_r7",0,0,836,768);carving.preserveAspect=false;
            carving.type=Image.Type.Sliced;carving.pixelsPerUnitMultiplier=4f;carving.fillCenter=false;
            boardFrame.SetAsLastSibling();
            EnsureStatus(v);
            ProductionGameplaySourceControls.Apply(v);
            v.Revision=ProductionGameplayView.CurrentRevision;
        }
        public static void LayoutGoals(ProductionGameplayView v,int count)
        {
            count=Mathf.Clamp(count,1,3);float stride=count==3?100:130;
            float start=548.5f-(count-1)*stride*.5f-49;
            for(int i=0;i<v.Goals.Length;i++)Place(v.Goals[i].Root,start+i*stride,112,98,106);
        }
        static void Ink(Text t)
        {
            t.color=new Color32(54,29,9,255);t.GetComponent<Shadow>().enabled=false;
            t.GetComponent<ProductionHomeGoldText>().enabled=false;
        }
        static Image Panel(Transform parent,string name,string resource,float x,float y,float w,float h)
        {
            var found=parent.Find(name);var image=found!=null?found.GetComponent<Image>():ProductionGameplayRuntime.Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();
            image.sprite=ProductionGameplayArt.Load(resource);image.raycastTarget=false;Place(image.rectTransform,x,y,w,h);return image;
        }
        static void EnsureStatus(ProductionGameplayView v)
        {
            var status=v.GetComponent<ProductionGameplayStatus>()??v.gameObject.AddComponent<ProductionGameplayStatus>();
            var found=v.Hud.Find("GameplayStatus");
            RectTransform panel=found!=null?found.GetComponent<RectTransform>():ProductionGameplayRuntime.Rect(v.Hud,"GameplayStatus",91,278,760,62);
            if(panel.Find("Lacquer")==null)
            {
                var image=ProductionGameplayRuntime.Rect(panel,"Lacquer",0,0,760,62).gameObject.AddComponent<Image>();
                image.sprite=ProductionGameplayArt.Load("cell_lacquer");image.raycastTarget=false;
                var border=ProductionGameplayRuntime.Rect(panel,"Border",0,0,760,62).gameObject.AddComponent<ProductionGameplayCell>();border.InsetPanel=true;border.raycastTarget=false;
            }
            var label=panel.Find("Message");
            var t=label!=null?label.GetComponent<Text>():ProductionGameplayRuntime.Rect(panel,"Message",14,3,732,56).gameObject.AddComponent<Text>();
            Text(t,14,3,732,56,23,false);t.font=v.Typography.RegularFont;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;
            status.View=v;status.Panel=panel;status.Label=t;status.Clear();v.Status=status;
        }
        static void Place(RectTransform r,float x,float y,float w,float h)
        {r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
        static void HeaderText(Text t,float x,float y,float w,float h,int size,bool bold,TextAnchor alignment=TextAnchor.MiddleCenter)
        {
            Text(t,x,y,w,h,size,bold,alignment);
            // Retain the local font and double-resolution sampling; shade only the live glyph face.
            var gold=t.GetComponent<ProductionHomeGoldText>();gold.enabled=true;
            gold.TopColor=new Color32(255,244,207,255);gold.BottomColor=new Color32(212,173,94,255);
            t.color=new Color32(255,242,204,255);
            var shadow=t.GetComponent<Shadow>();shadow.effectColor=new Color32(10,7,3,230);shadow.effectDistance=new Vector2(.8f,-1.2f);
        }
        static void Text(Text t,float x,float y,float w,float h,int size,bool bold,TextAnchor alignment=TextAnchor.MiddleCenter)
        {
            Place(t.rectTransform,x,y,w*2,h*2);t.rectTransform.localScale=Vector3.one*.5f;
            t.fontSize=size*2;t.fontStyle=bold?FontStyle.Bold:FontStyle.Normal;t.alignment=alignment;
            t.color=new Color32(255,240,198,255);t.material=null;t.raycastTarget=false;
            var gold=t.GetComponent<ProductionHomeGoldText>()??t.gameObject.AddComponent<ProductionHomeGoldText>();
            gold.BeveledFace=false;gold.TopColor=new Color32(255,245,211,255);gold.BottomColor=new Color32(221,182,107,255);
            var shadow=t.GetComponent<Shadow>()??t.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(48,25,7,245);shadow.effectDistance=new Vector2(1.6f,-2.8f);shadow.useGraphicAlpha=true;
        }
    }
}
