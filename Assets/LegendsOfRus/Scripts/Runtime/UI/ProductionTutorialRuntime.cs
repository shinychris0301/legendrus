using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionTutorialRuntime
    {
        public static Sprite Load(string key)=>Resources.Load<Sprite>("ProductionUI/TutorialAligned/"+key);
        public static void BuildInto(RectTransform root)
        {
            var old=root.GetComponent<ProductionTutorialView>();
            if(old&&old.HasRequiredBindings){old.RefreshContent();return;}
            if(old){
                if(Application.isPlaying)throw new InvalidOperationException("Rebuild Tutorial outside Play Mode.");
                if(old.Content)UnityEngine.Object.DestroyImmediate(old.Content.gameObject);
                var stale=root.GetComponent<ProductionTutorialLayout>();if(stale)UnityEngine.Object.DestroyImmediate(stale);
            }
            var v=old?old:root.gameObject.AddComponent<ProductionTutorialView>();v.Revision=ProductionTutorialView.CurrentRevision;
            v.BodyFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");v.TitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            v.Content=Rect(root,"TUTORIAL_ALIGNED_CONTENT",0,0,941,1672);
            v.Environment=Art(v.Content,"CleanEnvironment",ProductionSettingsRuntime.Shared("background"),0,0,941,1672);v.Environment.color=new Color(.28f,.28f,.32f,1);
            v.Body=Rect(v.Content,"Composition",0,0,941,1672);
            Art(v.Body,"SourcePanel",Load("panel_r3"),0,0,941,1647);
            v.Heading=Label(v,v.Body,"Heading",137,163,668,95,67,true);
            var headingShadow=v.Heading.gameObject.AddComponent<Shadow>();headingShadow.effectColor=new Color(0,0,0,.8f);headingShadow.effectDistance=new Vector2(2,-3);
            v.Subtitle=Label(v,v.Body,"Subtitle",100,371,741,64,46);
            v.Close=Button(Art(v.Body,"Close",Load("close"),806,88,112,116));
            v.Tabs=new Button[4];v.TabLabels=new Text[4];
            for(int i=0;i<4;i++){v.Tabs[i]=Button(Panel(v.Body,"Topic_"+i,ProductionSettingsRuntime.Shared("tab"),88+i*193,270,190,84));Solid(v.Tabs[i].transform,"SelectedFace",13,13,164,58,new Color(.015f,.19f,.38f,.7f));v.TabLabels[i]=Label(v,v.Tabs[i].transform,"Label",7,14,176,53,25);}
            v.MatchDiagram=Rect(v.Body,"MatchDiagram",0,0,941,950).gameObject;
            v.BeforeLabel=Label(v,v.MatchDiagram.transform,"Before",105,436,315,52,32);
            v.AfterLabel=Label(v,v.MatchDiagram.transform,"After",521,436,315,52,32);
            BuildMatchGrid(v.MatchDiagram.transform,"BeforeGrid",105,498,false);
            BuildMatchGrid(v.MatchDiagram.transform,"AfterGrid",521,498,true);
            Arrow(v.MatchDiagram.transform,440,646,57,false);
            v.MatchInstruction=Label(v,v.MatchDiagram.transform,"Instruction",85,850,771,106,35);
            v.Rows=new GameObject[4];v.RowIcons=new Image[4];v.RowTitles=new Text[4];v.RowDescriptions=new Text[4];
            for(int i=0;i<4;i++){
                var row=Rect(v.Body,"GuideRow_"+i,107,455+i*202,728,192);v.Rows[i]=row.gameObject;
                Panel(row,"Card",ProductionSettingsRuntime.Shared("reward_panel"),0,0,728,192);
                v.RowIcons[i]=Art(row,"Illustration",ProductionGameplayArt.Gem(i),20,30,134,136);
                var moves=Label(v,row,"MovesExample",52,29,72,103,23);moves.gameObject.SetActive(false);
                v.RowTitles[i]=Label(v,row,"Title",178,12,526,54,29);v.RowTitles[i].alignment=TextAnchor.MiddleLeft;
                v.RowDescriptions[i]=Label(v,row,"Description",178,66,526,114,28);v.RowDescriptions[i].alignment=TextAnchor.UpperLeft;
                v.RowDescriptions[i].color=new Color32(241,224,186,255);
            }
            v.Hint=Label(v,v.Body,"Hint",105,1283,731,73,25);
            v.Previous=Button(Panel(v.Body,"Previous",ProductionSettingsRuntime.Shared("rules"),107,1451,285,79));v.PreviousLabel=Label(v,v.Previous.transform,"Label",17,10,251,54,29);
            v.Next=Button(Art(v.Body,"Next",Load("next"),245,1420,450,145));v.NextLabel=Label(v,v.Next.transform,"Label",64,17,319,69,42,true);
            v.Counter=Label(v,v.Body,"Counter",391,1360,159,51,36);
            var layout=root.gameObject.AddComponent<ProductionTutorialLayout>();layout.View=v;v.RefreshContent();layout.ApplyNow();
        }
        // Deterministic illustration of one legal swap; it never touches the gameplay board.
        public static int[] MatchExample(bool after)=>after?new[]{0,1,2,0,1,3,0,2,3}:new[]{0,1,2,1,0,3,0,2,3};
        static void BuildMatchGrid(Transform parent,string name,float x,float y,bool after)
        {
            var grid=Rect(parent,name,x,y,315,315);
            Art(grid,"Wood",ProductionGameplayArt.Load("board_wood_r7"),0,0,315,315).preserveAspect=false;
            int[] gems=MatchExample(after);
            for(int i=0;i<9;i++)Art(grid,"Gem_"+i,ProductionGameplayArt.Gem(gems[i]),(i%3)*105+12,(i/3)*105+10,81,85);
            Color gold=new Color32(142,100,40,255);
            for(int i=0;i<=3;i++){
                Solid(grid,"Horizontal_"+i,0,i*105,315,2,gold);
                Solid(grid,"Vertical_"+i,i*105,0,2,315,gold);
                for(int j=0;j<=3;j++){var knot=Solid(grid,"Joint_"+i+"_"+j,i*105-3,j*105-3,6,6,gold);knot.localEulerAngles=new Vector3(0,0,45);}
            }
            if(after){Color highlight=new Color32(235,190,83,255);Solid(grid,"MatchLeft",2,2,3,311,highlight);Solid(grid,"MatchRight",100,2,3,311,highlight);Solid(grid,"MatchTop",2,2,101,3,highlight);Solid(grid,"MatchBottom",2,311,101,3,highlight);}
            else Arrow(grid,62,157,80,true);
        }
        static RectTransform Solid(Transform p,string n,float x,float y,float w,float h,Color color){var r=Rect(p,n,x,y,w,h);var a=r.gameObject.AddComponent<Image>();a.color=color;a.raycastTarget=false;return r;}
        static void Arrow(Transform p,float x,float y,float width,bool both)
        {
            Color c=new Color32(238,188,79,255);Solid(p,"ArrowStem",x,y,width,5,c);
            var head=Solid(p,"ArrowHead",x+width-12,y-9,18,5,c);head.localEulerAngles=new Vector3(0,0,-40);
            head=Solid(p,"ArrowHead",x+width-8,y+12,18,5,c);head.localEulerAngles=new Vector3(0,0,40);
            if(both){head=Solid(p,"ArrowTail",x,y+3,18,5,c);head.localEulerAngles=new Vector3(0,0,40);head=Solid(p,"ArrowTail",x+3,y+2,18,5,c);head.localEulerAngles=new Vector3(0,0,-40);}
        }
        static RectTransform Rect(Transform p,string n,float x,float y,float w,float h){var go=new GameObject(n,typeof(RectTransform));go.layer=5;go.transform.SetParent(p,false);var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return r;}
        static Image Art(Transform p,string n,Sprite s,float x,float y,float w,float h){if(!s)throw new InvalidOperationException("Missing Tutorial art "+n);var a=Rect(p,n,x,y,w,h).gameObject.AddComponent<Image>();a.sprite=s;a.preserveAspect=true;a.raycastTarget=false;return a;}
        static Image Panel(Transform p,string n,Sprite s,float x,float y,float w,float h){var a=Art(p,n,s,x,y,w,h);a.type=Image.Type.Sliced;a.preserveAspect=false;a.pixelsPerUnitMultiplier=2;return a;}
        static Button Button(Image a){a.raycastTarget=true;var b=a.gameObject.AddComponent<Button>();b.targetGraphic=a;b.navigation=new Navigation{mode=Navigation.Mode.None};return b;}
        static Text Label(ProductionTutorialView v,Transform p,string n,float x,float y,float w,float h,int size,bool title=false){var t=Rect(p,n,x,y,w,h).gameObject.AddComponent<Text>();t.font=title?v.TitleFont:v.BodyFont;t.fontSize=size;t.color=new Color32(241,204,128,255);t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.resizeTextForBestFit=true;t.resizeTextMinSize=size-5;t.resizeTextMaxSize=size;return t;}
    }
}
