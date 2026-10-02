using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Original isolated controls; the existing level path and save-driven actions remain native.</summary>
    public static class ProductionChapterMapSourceControls
    {
        public const string ArtRoot="ProductionUI/ChapterMapReferenceAligned/";
        public static readonly string[] ArtNames={"environment","environment_worldmap01_landmarks","banner","banner_blank","play","play_blank","back","node_blank","star_full","star_empty","footer"};
        public static void Apply(ProductionChapterMapView view)
        {
            foreach(var graphic in view.HudBacking.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            ProductionPlayerHeader.Attach(view);
            Art(view.Environment,"environment_worldmap01_landmarks",0,0,941,1672);view.Environment.preserveAspect=false;
            foreach(var shade in view.Environment.GetComponentsInChildren<ProductionHomeBackdropShade>(true))shade.enabled=false;
            view.EnvironmentMargins=Need(view.Content,"EnvironmentMargins");
            Art(view.EnvironmentMargins,"environment_worldmap01_landmarks",0,0,941,1672);view.EnvironmentMargins.preserveAspect=false;
            view.EnvironmentMargins.color=new Color(.7f,.7f,.7f,1);view.EnvironmentMargins.transform.SetAsFirstSibling();
            Place(view.TitleArea,0,116,941,273);
            Art(view.TitlePlaque,"banner",77,0,798,273);
            Label(view.ChapterText,250,75,455,38,28,"Title");
            Label(view.TitleText,194,112,557,65,42,"Title");
            Place(view.MapArea,0,400,941,1040);
            ProductionChapterMapRuntime.EnsureNodePool(view);
            for(int i=0;i<view.Nodes.Length;i++)
            {
                var node=view.Nodes[i];var center=ProductionChapterMapRuntime.NodeCenters[i%ProductionChapterMapRuntime.NodeCenters.Length];
                Place(node.Root,center.x-63,center.y-450,126,132);
                Art(node.Face,"node_blank",8,-5,110,110);
                Label(node.Number,22,12,82,72,43,"RankNumbers");
                node.SourceStars=new Image[3];
                for(int j=0;j<3;j++)
                {
                    node.Stars[j].gameObject.SetActive(false);
                    node.SourceStars[j]=Need(node.Root,"SourceStar"+j);
                    Art(node.SourceStars[j],"star_empty",15+j*31,98+(j==1?6:0),31,32);
                }
            }
            Art(view.FooterOrnament,"footer",0,1590,941,82);
            Place(view.Footer,0,1440,941,232);
            view.SourceBack=Control(view.BackButton,"back",8,32,174,156);
            view.SourcePlay=Control(view.PlayButton,"play",232,20,481,203);
            ProductionHomeRuntime.ClearDormantPlayArtwork(view.PlayButton);
            Label(view.PlayText,85,43,310,85,48,"Title");
            ProductionChapterMapScroller.Install(view);
            view.Notice=ProductionMapNotice.Install(view.Content);
            ProductionMapButtonFeedback.Install(view.transform);
            view.Revision=ProductionChapterMapView.CurrentRevision;view.SourceControls=true;
            view.RefreshContent();view.GetComponent<ProductionChapterMapLayout>()?.ApplyNow();
        }
        public static void RefreshLanguage(ProductionChapterMapView view)
        {
            bool en=DemoFeatureState.LanguageCode=="en";
            view.TitlePlaque.sprite=Load(en?"banner_blank":"banner");
            view.SourcePlay.sprite=Load(en?"play_blank":"play");
            view.ChapterText.text=en?"CHAPTER 1":"ГЛАВА 1";view.TitleText.text=en?"NATIVE VILLAGE":"РОДНАЯ ДЕРЕВНЯ";view.PlayText.text=en?"PLAY":"ИГРАТЬ";
            view.ChapterText.enabled=view.TitleText.enabled=view.PlayText.enabled=en;
        }
        public static void RefreshStars(ProductionChapterMapView view)
        {
            foreach(var node in view.Nodes)for(int j=0;j<3;j++)node.SourceStars[j].sprite=Load(node.Stars[j].Dimmed?"star_empty":"star_full");
        }
        static Image Control(Button button,string key,float x,float y,float w,float h)
        {
            foreach(var graphic in button.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
            foreach(var effect in button.GetComponentsInChildren<BaseMeshEffect>(true))effect.enabled=false;
            Place((RectTransform)button.transform,x,y,w,h);
            var image=Need(button.transform,"OriginalSourceArtwork");Art(image,key,0,0,w,h);
            image.raycastTarget=true;button.targetGraphic=image;button.transition=Selectable.Transition.None;
            return image;
        }
        static Image Need(Transform parent,string name)
        {
            var child=parent.Find(name);if(child==null)child=ProductionChapterMapRuntime.Rect(parent,name,0,0,1,1);
            return child.GetComponent<Image>()??child.gameObject.AddComponent<Image>();
        }
        static Sprite Load(string name)=>Resources.Load<Sprite>(ArtRoot+name)??throw new InvalidOperationException("Missing source Chapter Map control: "+name);
        static void Art(Image image,string key,float x,float y,float w,float h)
        {
            Place(image.rectTransform,x,y,w,h);image.sprite=Load(key);image.color=Color.white;image.type=Image.Type.Simple;image.preserveAspect=true;image.enabled=true;image.material=null;image.raycastTarget=false;
        }
        static void Place(RectTransform rect,float x,float y,float w,float h)
        {
            rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.localScale=Vector3.one;rect.localRotation=Quaternion.identity;rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);
        }
        static void Label(Text text,float x,float y,float w,float h,int size,string font)
        {
            foreach(var effect in text.GetComponents<BaseMeshEffect>())effect.enabled=false;
            text.transform.SetAsLastSibling();Place(text.rectTransform,x,y,w*2,h*2);text.rectTransform.localScale=new Vector3(.5f,.5f,1);
            text.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/"+font)??text.font;text.fontSize=size*2;text.fontStyle=FontStyle.Normal;
            text.material=null;text.color=new Color32(245,214,153,255);text.alignment=TextAnchor.MiddleCenter;text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;text.raycastTarget=false;text.enabled=true;
        }
    }
}
