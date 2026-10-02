using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Serialized World Map composition using whole sprites and the accepted Home HUD.</summary>
    public static class ProductionWorldMapRuntime
    {
        public const string ContentName = "WORLD_MAP_ALIGNED_CONTENT";
        public const string ArtRoot = "ProductionUI/WorldMapAligned/";
        public static readonly string[] ArtNames = { "environment", "title", "map", "tasks", "guild" };
        // Chapter positions from the reference, in its 941 x 1672 portrait coordinate system.
        public static readonly Vector2[] NodeCenters = {
            new Vector2(493,1238), new Vector2(606,1160), new Vector2(458,1132), new Vector2(290,1038),
            new Vector2(458,1019), new Vector2(634,957), new Vector2(482,929), new Vector2(300,873),
            new Vector2(482,801), new Vector2(660,744), new Vector2(444,686), new Vector2(333,620),
            new Vector2(625,566), new Vector2(466,518), new Vector2(525,426)
        };

        public static void BuildInto(RectTransform root)
        {
            // The source composition retains its original Home HUD as an inactive child.
            var legacyProgress = root.Find(ContentName + "/WorldMapHud/ProfileButton/ProfileProgressFrame");
            if (legacyProgress != null) legacyProgress.GetComponent<Image>().sprite = null;
            var existing = root.GetComponent<ProductionWorldMapView>();
            if (existing != null && existing.HasRequiredBindings) { ClearDormantFooterArtwork(existing); existing.RefreshContent(); AlignPresentation(existing); return; }
            if(existing != null && existing.Content != null && !Application.isPlaying)
            {
                ProductionWorldMapSourceComposition.Apply(existing);
                existing.GetComponent<ProductionWorldMapLayout>().View=existing;
                existing.RefreshContent();AlignPresentation(existing);return;
            }
            if (root.Find(ContentName) != null)
                throw new InvalidOperationException("World Map bindings need repair outside Play Mode.");
            var homePrefab = Resources.Load<GameObject>("ProductionUI/Pages/Home");
            var home = homePrefab != null ? homePrefab.GetComponent<ProductionHomeView>() : null;
            if (home == null || !home.HasRequiredBindings) throw new InvalidOperationException("World Map requires the accepted Home HUD.");
            var view = existing != null ? existing : root.gameObject.AddComponent<ProductionWorldMapView>();
            view.Typography = home.Typography;
            RectTransform content = Rect(root,ContentName,0,0,941,1672);
            view.Content = content;
            view.Environment = Art(content,"Environment",Load("environment"),0,0,941,1672);
            view.Environment.color = ProductionHomeRuntime.EnvironmentTone;
            var shade = Rect(view.Environment.rectTransform,"LowerSceneTone",0,0,941,1672).gameObject.AddComponent<ProductionHomeBackdropShade>();
            shade.color = new Color32(4,8,15,255); shade.raycastTarget = false; shade.rectTransform.anchorMin = Vector2.zero; shade.rectTransform.anchorMax = Vector2.one;
            shade.rectTransform.offsetMin = shade.rectTransform.offsetMax = Vector2.zero;
            view.MapArea = Rect(content,"ChapterPath",0,300,941,1030);
            view.Path = Rect(view.MapArea,"PathLight",0,0,941,1030).gameObject.AddComponent<ProductionWorldMapGraphic>();
            view.Path.Kind = WorldMapGraphicKind.Path; view.Path.raycastTarget = false;
            view.Nodes = new WorldMapNode[NodeCenters.Length];
            for (int i = 0; i < view.Nodes.Length; i++)
            {
                RectTransform node = Rect(view.MapArea,"Chapter_"+(i+1),NodeCenters[i].x-50,NodeCenters[i].y-350,100,116);
                var hit = node.gameObject.AddComponent<Image>(); hit.color = Color.clear;
                Button button = Button(hit); button.transition = Selectable.Transition.None;
                var halo = Graphic(node,"SelectedHalo",WorldMapGraphicKind.Halo,7,7,86,86);
                var face = Graphic(node,"ChapterDisc",WorldMapGraphicKind.Node,17,17,66,66);
                Text number = Label(node,"Number",(i+1).ToString(),17,17,66,66,36,view.Typography);
                var stars = new ProductionWorldMapGraphic[3];
                for (int j = 0; j < 3; j++) stars[j] = Graphic(node,"Star_"+j,WorldMapGraphicKind.Star,15+j*24,77,24,27);
                var locked = Graphic(node,"Lock",WorldMapGraphicKind.Lock,68,57,23,27);
                view.Nodes[i] = new WorldMapNode { Root=node, Button=button, Face=face, Halo=halo, Number=number, Stars=stars, Lock=locked };
            }
            view.HudBacking = Clone(home.GetComponent<ProductionHomeLayout>().HudBacking,content,"HudOpaqueNavyBacking");
            view.Hud = Clone(home.GetComponent<ProductionHomeLayout>().Hud,content,"WorldMapHud");
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
            view.TitleArea = Rect(content,"TitleArea",0,130,941,170);
            view.TitlePlaque = Art(view.TitleArea,"TitlePlaque",Load("title"),176,-4,620,176);
            view.TitleText = Label(view.TitleArea,"Title","КАРТА МИРА",242,34,496,82,53,view.Typography);
            view.TitleText.fontStyle = FontStyle.Bold;
            var gold = view.TitleText.gameObject.AddComponent<ProductionHomeGoldText>(); gold.BeveledFace = true;
            view.TitleText.material = Resources.Load<Material>(ProductionHomeRuntime.ArtRoot+"home_play_gold");
            var backHit = Art(view.TitleArea,"Back",null,10,146,104,104); backHit.color = Color.clear;
            view.BackButton = Button(backHit); Graphic(backHit.rectTransform,"BackArrow",WorldMapGraphicKind.Back,18,18,68,68);
            view.Footer = Rect(content,"Navigation",0,1352,941,320);
            view.FooterOrnament = Art(content,"FooterOrnament",null,-2,1540,945,323);
            view.FooterOrnament.transform.SetSiblingIndex(view.Footer.GetSiblingIndex());
            view.NavButtons = new Button[3]; view.NavLabels = new Text[3];
            string[] names = {"map","tasks","guild"}; string[] labels = {"КАРТА","ЗАДАНИЯ","ГИЛЬДИЯ"};
            for (int i=0;i<3;i++)
            {
                Image image = Art(view.Footer,names[i],Load(names[i]),24+i*298,-8,298,298);
                view.NavButtons[i] = Button(image);
                view.NavLabels[i] = Label(image.rectTransform,"Label",labels[i],38,228,222,48,32,view.Typography);
            }
            // A local chapter task summary uses real progress, without inventing a quest service.
            view.TaskPanel = Rect(content,"ChapterTasks",140,570,661,480).gameObject;
            var panel = view.TaskPanel.AddComponent<ProductionHomePanel>(); panel.CornerJewels = true; panel.Radius = 22;
            view.TaskHeading = Label(panel.rectTransform,"Heading","ЗАДАНИЯ",40,30,581,70,42,view.Typography);
            view.TaskCopy = Label(panel.rectTransform,"Progress","",40,116,581,220,31,view.Typography);
            view.TaskCopy.horizontalOverflow = HorizontalWrapMode.Wrap;
            var close = Rect(panel.rectTransform,"Close",160,363,341,76).gameObject.AddComponent<ProductionHomePanel>();
            view.TaskClose = Button(close); view.TaskCloseText = Label(close.rectTransform,"Label","ЗАКРЫТЬ",0,0,341,76,30,view.Typography);
            view.TaskPanel.SetActive(false);
            var layout = root.GetComponent<ProductionWorldMapLayout>() ?? root.gameObject.AddComponent<ProductionWorldMapLayout>();
            layout.View = view;
            ProductionWorldMapSourceComposition.Apply(view);
            view.Revision = ProductionWorldMapView.CurrentRevision;
            view.RefreshContent(); layout.ApplyNow();
            if (!view.HasRequiredBindings) throw new InvalidOperationException("World Map composition bindings are incomplete.");
            AlignPresentation(view);
        }

        // The original footer object survives alongside the current source footer.
        internal static void ClearDormantFooterArtwork(ProductionWorldMapView view)
        {
            var old = view.transform.Find(ContentName + "/FooterOrnament");
            var image = old != null ? old.GetComponent<Image>() : null;
            if (image != null && image != view.FooterOrnament) image.sprite = null;
        }

        public static void AlignPresentation(ProductionWorldMapView view)
        {
            if(view.SourceComposition)return;
            foreach (var graphic in view.GetComponentsInChildren<ProductionWorldMapGraphic>(true))
                if (graphic.GetComponent<CanvasRenderer>() == null) graphic.gameObject.AddComponent<CanvasRenderer>();
            // Node positions stretch with the safe-area height. Avoid a one-pixel glyph jump
            // when tiny transform-rounding differences cross a pixel-snap boundary.
            foreach (var node in view.Nodes)
                node.Number.GetComponent<SplashTitleTextGeometry>().SnapToScreenPixels = false;
            view.TitleText.rectTransform.anchoredPosition = new Vector2(242,-42);
            foreach (var label in view.NavLabels) label.rectTransform.anchoredPosition = new Vector2(38,-218);
            ProductionNavigationStyle.Apply(view);
        }

        static Sprite Load(string name) => Resources.Load<Sprite>(ArtRoot+name) ?? throw new InvalidOperationException("Missing World Map artwork: "+name);
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
