using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Serialized Chapter Map composition using whole sprites and the accepted Home HUD.</summary>
    public static class ProductionChapterMapRuntime
    {
        public const string ContentName = "CHAPTER_MAP_ALIGNED_CONTENT";
        public const string ArtRoot = "ProductionUI/ChapterMapAligned/";
        public static readonly string[] ArtNames = { "environment", "banner", "node" };
        // Ten authored levels follow the newly painted village path, bottom to top.
        public const int NodePoolSize=14;
        public static readonly Vector2[] NodeCenters = {
            new Vector2(435,1355), new Vector2(560,1270), new Vector2(435,1185), new Vector2(565,1100),
            new Vector2(365,1015), new Vector2(520,930), new Vector2(385,845), new Vector2(560,760),
            new Vector2(435,675), new Vector2(645,590)
        };

        public static void EnsureNodePool(ProductionChapterMapView view)
        {
            if(view.Nodes.Length>=NodePoolSize)return;
            int previous=view.Nodes.Length;
            Array.Resize(ref view.Nodes,NodePoolSize);
            for(int i=previous;i<view.Nodes.Length;i++)view.Nodes[i]=CreateNode(view,i);
        }
        static ChapterMapNode CreateNode(ProductionChapterMapView view,int i)
        {
                RectTransform node = Rect(view.MapArea,"Level_"+(i+1),NodeCenters[i%NodeCenters.Length].x-63,NodeCenters[i%NodeCenters.Length].y-450,126,132);
                var hit = node.gameObject.AddComponent<Image>(); hit.color = Color.clear;
                Button button = Button(hit); button.transition = Selectable.Transition.None;
                button.GetComponent<ProductionHomeButtonFeedback>().Role = MetaUiControlRole.MapNode;
                var halo = Graphic(node,"SelectedHalo",WorldMapGraphicKind.Halo,3,-10,120,120);
                var face = Art(node,"LevelMedallion",Load("node"),8,-5,110,110);
                Text number = Label(node,"Number",(i+1).ToString(),22,12,82,72,43,view.Typography);
                number.gameObject.AddComponent<ProductionHomeGoldText>().BeveledFace = true;
                number.material = Resources.Load<Material>(ProductionHomeRuntime.ArtRoot+"home_play_gold");
                var stars = new ProductionWorldMapGraphic[3];
                for (int j=0;j<3;j++) stars[j]=Graphic(node,"Star_"+j,WorldMapGraphicKind.Star,15+j*31,98,31,32);
                var locked=Graphic(node,"Lock",WorldMapGraphicKind.Lock,96,74,24,28);
                return new ChapterMapNode{Root=node,Button=button,Face=face,Halo=halo,Number=number,Stars=stars,Lock=locked};
        }

        public static void BuildInto(RectTransform root)
        {
            var existing = root.GetComponent<ProductionChapterMapView>();
            if (existing != null && existing.HasRequiredBindings) { ProductionChapterMapSourceControls.Apply(existing); return; }
            if(existing!=null && existing.Content!=null && !Application.isPlaying)
            { ProductionChapterMapSourceControls.Apply(existing);return; }
            if (root.Find(ContentName) != null)
                throw new InvalidOperationException("Chapter Map bindings need repair outside Play Mode.");
            var homePrefab = Resources.Load<GameObject>("ProductionUI/Pages/Home");
            var home = homePrefab != null ? homePrefab.GetComponent<ProductionHomeView>() : null;
            if (home == null || !home.HasRequiredBindings) throw new InvalidOperationException("Chapter Map requires the accepted Home HUD.");
            var view = existing != null ? existing : root.gameObject.AddComponent<ProductionChapterMapView>();
            view.Typography = home.Typography;
            RectTransform content = Rect(root,ContentName,0,0,941,1672);
            view.Content = content;
            view.Environment = Art(content,"Environment",Load("environment"),0,0,941,1672);
            view.Environment.color = ProductionHomeRuntime.EnvironmentTone;
            var shade = Rect(view.Environment.rectTransform,"LowerSceneTone",0,0,941,1672).gameObject.AddComponent<ProductionHomeBackdropShade>();
            shade.color = new Color32(4,8,15,255); shade.raycastTarget = false; shade.rectTransform.anchorMin = Vector2.zero; shade.rectTransform.anchorMax = Vector2.one;
            shade.rectTransform.offsetMin = shade.rectTransform.offsetMax = Vector2.zero;
            view.MapArea = Rect(content,"LevelPath",0,400,941,1040);
            view.Nodes = new ChapterMapNode[NodePoolSize];
            for(int i=0;i<view.Nodes.Length;i++)view.Nodes[i]=CreateNode(view,i);
            view.HudBacking = Clone(home.GetComponent<ProductionHomeLayout>().HudBacking,content,"HudOpaqueNavyBacking");
            view.Hud = Clone(home.GetComponent<ProductionHomeLayout>().Hud,content,"ChapterMapHud");
            // Instantiate remaps the shared header's serialized references to this clone.
            // Home's current capsules no longer live at the historical GoldResource paths.
            var header = view.Hud.GetComponent<ProductionPlayerHeader>();
            if (header == null || !header.HasBindings)
                throw new InvalidOperationException("Cloned Home HUD requires complete shared header bindings.");
            view.ProfileButton = header.Profile;
            view.SettingsButton = header.Settings;
            view.HeroName = header.PlayerName;
            view.LevelText = header.Level;
            view.GoldText = header.Balances[0];
            view.GemsText = header.Balances[1];
            view.EnergyText = header.Balances[2];
            view.PlusButtons = (Button[])header.Plus.Clone();
            view.TitleArea=Rect(content,"TitleArea",0,130,941,280);
            view.TitlePlaque=Art(view.TitleArea,"ChapterBanner",Load("banner"),55,0,831,277);
            view.ChapterText=Label(view.TitleArea,"Chapter","ГЛАВА 1",240,68,461,47,32,view.Typography);
            view.TitleText=Label(view.TitleArea,"Title","РОДНАЯ ДЕРЕВНЯ",178,110,585,72,48,view.Typography);
            foreach(Text title in new[]{view.TitleText,view.ChapterText})
            {
                title.gameObject.AddComponent<ProductionHomeGoldText>().BeveledFace=true;
                title.material=Resources.Load<Material>(ProductionHomeRuntime.ArtRoot+"home_play_gold");
            }
            view.FooterOrnament=Art(content,"FooterOrnament",home.FooterBase.sprite,-2,1540,945,323);
            view.Footer=Rect(content,"Navigation",0,1440,941,232);
            var backHit=Art(view.Footer,"Back",null,28,30,144,144);backHit.color=Color.clear;
            view.BackButton=Button(backHit);Graphic(backHit.rectTransform,"BackArrow",WorldMapGraphicKind.Back,3,3,138,138);
            var play=Clone((RectTransform)home.PlayButton.transform,view.Footer,"PlayButton");
            play.anchoredPosition=new Vector2(230,0);play.localScale=Vector3.one*.93f;
            view.PlayButton=play.GetComponent<Button>();view.PlayText=play.Find("PlayLabel").GetComponent<Text>();
            var layout = root.GetComponent<ProductionChapterMapLayout>() ?? root.gameObject.AddComponent<ProductionChapterMapLayout>();
            layout.View = view;
            ProductionChapterMapSourceControls.Apply(view);
            view.Revision = ProductionChapterMapView.CurrentRevision;
            view.RefreshContent(); layout.ApplyNow();
            if (!view.HasRequiredBindings) throw new InvalidOperationException("Chapter Map composition bindings are incomplete.");
            AlignPresentation(view);
        }

        public static void AlignPresentation(ProductionChapterMapView view)
        {
            if(view.SourceControls)return;
            for(int i=0;i<view.Nodes.Length;i++)
            {
                var node=view.Nodes[i];Vector2 center=NodeCenters[i];
                node.Root.anchoredPosition=new Vector2(center.x-63,-(center.y-450));
                node.Number.GetComponent<SplashTitleTextGeometry>().SnapToScreenPixels=false;
                node.Number.fontStyle=FontStyle.Bold;
                foreach(var star in node.Stars)
                    if(star.GetComponent<ProductionChapterMapDecoration>()==null)star.gameObject.AddComponent<ProductionChapterMapDecoration>();
            }
            foreach(var title in new[]{view.TitleText,view.ChapterText})
            {
                title.fontStyle=FontStyle.Bold;
                var shadow=title.GetComponent<Shadow>()??title.gameObject.AddComponent<Shadow>();
                shadow.effectColor=new Color32(34,15,2,230);shadow.effectDistance=new Vector2(0,-2);
            }
            var arrow=view.BackButton.transform.Find("BackArrow").GetComponent<ProductionWorldMapGraphic>();
            var decoration=arrow.GetComponent<ProductionChapterMapDecoration>()??arrow.gameObject.AddComponent<ProductionChapterMapDecoration>();
            decoration.BackArrow=true;
            if(view.BackButton.transform.Find("BackMedallion")==null)
                Art(view.BackButton.transform,"BackMedallion",Load("node"),-5,-5,154,154).transform.SetAsFirstSibling();
            view.BackButton.GetComponent<ProductionHomeButtonFeedback>().Role=MetaUiControlRole.Back;
            ProductionNavigationStyle.Apply(view);

        }

        static Sprite Load(string name) => Resources.Load<Sprite>(ArtRoot+name) ?? throw new InvalidOperationException("Missing Chapter Map artwork: "+name);
        static RectTransform Clone(RectTransform source, Transform parent, string name)
        {
            var copy = UnityEngine.Object.Instantiate(source.gameObject,parent,false); copy.name=name; return (RectTransform)copy.transform;
        }
        public static RectTransform Rect(Transform parent,string name,float x,float y,float w,float h)
        {
            var go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);
            rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w,h);return rect;
        }
        static Image Art(Transform parent,string name,Sprite sprite,float x,float y,float w,float h)
        {
            var image=Rect(parent,name,x,y,w,h).gameObject.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=false;return image;
        }
        static ProductionWorldMapGraphic Graphic(Transform parent,string name,WorldMapGraphicKind kind,float x,float y,float w,float h)
        {
            var image=Rect(parent,name,x,y,w,h).gameObject.AddComponent<ProductionWorldMapGraphic>();image.Kind=kind;image.raycastTarget=false;return image;
        }
        static Button Button(Graphic image)
        {
            image.raycastTarget=true;var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;
            button.navigation=new Navigation{mode=Navigation.Mode.None};
            var colors=button.colors;colors.pressedColor=new Color(.8f,.8f,.8f,1);colors.selectedColor=Color.white;colors.fadeDuration=.06f;button.colors=colors;
            button.gameObject.AddComponent<ProductionHomeButtonFeedback>();return button;
        }
        static Text Label(Transform parent,string name,string value,float x,float y,float w,float h,int size,SplashTitleTypography typography)
        {
            var text=Rect(parent,name,x,y,w*2,h*2).gameObject.AddComponent<Text>();text.rectTransform.localScale=Vector3.one*.5f;
            text.text=value;text.font=typography.RegularFont;text.fontSize=size*2;text.color=new Color32(243,213,156,255);
            text.alignment=TextAnchor.MiddleCenter;text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;text.raycastTarget=false;
            text.gameObject.AddComponent<SplashTitleTextGeometry>();return text;
        }
    }
}
