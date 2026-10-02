#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>The sole owner of the canonical Splash prefab. No scene loading or Play Mode writes.</summary>
    [InitializeOnLoad]
    public static class Step20SplashTitleR3PrefabRebuilder
    {
        public const string PrefabPath = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab";
        public const string ArtRoot = "Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/";
        public const string ConfigRoot = "Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/";
        public const string TypographyPath = ConfigRoot + "SplashTitleTypography.asset";
        public const string LayoutPath = ConfigRoot + "SplashTitleLayout.json";
        static bool ensureQueued;
        static bool buildInProgress;

        static Step20SplashTitleR3PrefabRebuilder()
        {
            // No AssetDatabase reads in the static constructor: assets may still be importing.
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            QueueEnsure();
        }

        internal static void QueueEnsure()
        {
            if (ensureQueued) return;
            ensureQueued = true;
            EditorApplication.delayCall += EnsureOnLoad;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode) QueueEnsure();
        }

        static void EnsureOnLoad()
        {
            ensureQueued = false;
            // Do not write a prefab in Play Mode. EnteredEditMode will request another check.
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
            { QueueEnsure(); return; }
            // A valid, already-shipped prefab is a no-op; no repeated rebuild on each import.
            try { EnsureBuilt(false); }
            catch (Exception ex)
            {
                Debug.LogError("Splash PrefabFix07 repair did not complete: " + ex.Message +
                    " Stop Play Mode, finish compilation/imports, then run Build or Repair.");
            }
        }

        [MenuItem("Legends of Rus/Production UI/Splash R3/Build or Repair")]
        public static void BuildMenu() { EnsureBuilt(true); }

        [MenuItem("Legends of Rus/Production UI/Home Style/Align Splash and Loading")]
        public static void BuildHomeAlignedFrontEnd()
        {
            EnsureBuilt(true);
            Step20ProductionUiFix01SerializedPrefabBuilder.BuildLoadingAligned();
        }

        [MenuItem("Legends of Rus/Production UI/Splash R3/Repair Responsive Layout")]
        public static void RepairResponsiveLayout() { EnsureBuilt(false); }

        [MenuItem("Legends of Rus/Production UI/Splash R3/Select Typography Settings")]
        public static void SelectTypography()
        { Selection.activeObject = AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(TypographyPath); }

        public static void EnsureBuilt(bool force)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Stop Play Mode before rebuilding SplashTitle.");
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException("Wait for compilation, importing, and Player builds to finish before rebuilding SplashTitle.");
            if (buildInProgress) return;
            buildInProgress = true;
            try { BuildCanonical(force); }
            finally { buildInProgress = false; }
        }

        static void BuildCanonical(bool force)
        {
            GameObject old = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            SplashTitleView existing = old != null ? old.GetComponent<SplashTitleView>() : null;
            if (!force && existing != null && existing.HasRequiredBindings) return;
            // Upgrade the accepted prefab in place; do not reconstruct its artwork or serialized objects.
            if (existing != null && existing.HasCompleteBindings &&
                existing.Revision >= 5 && existing.Revision <= SplashTitleView.CurrentRevision)
            {
                GameObject contents = PrefabUtility.LoadPrefabContents(PrefabPath);
                try
                {
                    ApplyReferenceStyle(contents);
                    BindResponsiveLayout(contents);
                    SplashTitleView view = contents.GetComponent<SplashTitleView>();
                    view.Revision = SplashTitleView.CurrentRevision;
                    if (!view.HasRequiredBindings) throw new InvalidOperationException(view.GetValidationFailure());
                    bool saved;
                    PrefabUtility.SaveAsPrefabAsset(contents, PrefabPath, out saved);
                    if (!saved) throw new IOException("Unity did not save the Splash responsive repair.");
                }
                finally { PrefabUtility.UnloadPrefabContents(contents); }
                AssetDatabase.ImportAsset(PrefabPath, ImportAssetOptions.ForceSynchronousImport);
                SplashTitleView repaired = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath).GetComponent<SplashTitleView>();
                if (!repaired.HasRequiredBindings) throw new InvalidOperationException(repaired.GetValidationFailure());
                Debug.Log("Splash original-reference alignment saved and read back at revision " + repaired.Revision +
                    "; existing object identities preserved; source composition applied.");
                return;
            }
            if (!File.Exists(LayoutPath)) throw new FileNotFoundException("Missing authoritative layout",LayoutPath);
            LayoutDoc doc = JsonUtility.FromJson<LayoutDoc>(File.ReadAllText(LayoutPath));
            if (doc == null || doc.schemaVersion != 4) throw new InvalidOperationException("Unsupported Splash layout schema.");
            AssetDatabase.Refresh();
            foreach (ArtSpec item in doc.art) RequireSprite(item.file);
            foreach (ButtonSpec item in doc.buttons) RequireSprite(item.file);

            SplashTitleTypography typography = AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(TypographyPath);
            if (typography == null)
            {
                typography = ScriptableObject.CreateInstance<SplashTitleTypography>();
                AssetDatabase.CreateAsset(typography, TypographyPath);
            }
            var root = new GameObject("SplashTitle", typeof(RectTransform), typeof(SplashTitleView),
                typeof(SplashTitleResponsiveLayout), typeof(ProductionCoreLoopPageMarker));
            try
            {
                root.layer = 5;
                RectTransform rt = root.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f);
                rt.sizeDelta = new Vector2(doc.referenceWidth,doc.referenceHeight);
                SplashTitleView view = root.GetComponent<SplashTitleView>();
                view.Typography = typography; view.Revision = SplashTitleView.CurrentRevision;
                root.GetComponent<ProductionCoreLoopPageMarker>().Configure(ScreenId.SplashTitle);
                Rect whole = new Rect(0,0,doc.referenceWidth,doc.referenceHeight);
                var objects = new Dictionary<string,RectTransform>();
                var rects = new Dictionary<string,Rect>();
                objects[""]=rt; rects[""]=whole;
                foreach (ArtSpec item in doc.art)
                {
                    Image image=MakeImage(rt,item.name,RequireSprite(item.file),RectOf(item.rect),whole,false);
                    if(item.fadeBottom>0)image.gameObject.AddComponent<SplashTitleCharacterFade>().BottomFraction=item.fadeBottom;
                    if(item.name=="BackgroundEnvironment")root.GetComponent<SplashTitleResponsiveLayout>().Background=image;
                }
                if(doc.footerShade==null)throw new InvalidOperationException("Missing footer contrast specification.");
                RectTransform shade=MakeRect(rt,"FooterContrast",RectOf(doc.footerShade.rect),whole);
                SplashTitleFooterShade shadeGraphic=shade.gameObject.AddComponent<SplashTitleFooterShade>();
                float[] shadeColor=doc.footerShade.color;
                shadeGraphic.color=new Color(shadeColor[0],shadeColor[1],shadeColor[2],shadeColor[3]);
                shadeGraphic.raycastTarget=false;
                foreach(ButtonSpec item in doc.buttons)
                {
                    Rect bounds=RectOf(item.rect);
                    RectTransform br=MakeRect(rt,item.name,bounds,whole);
                    Image image=MakeImage(br,"Artwork",RequireSprite(item.file),RectOf(item.artRect),bounds,true);
                    Button button=br.gameObject.AddComponent<Button>();button.targetGraphic=image;
                    button.transition=Selectable.Transition.ColorTint;
                    ColorBlock colors=button.colors;
                    colors.normalColor=Color.white;colors.highlightedColor=new Color(.98f,.98f,.96f,1);
                    colors.selectedColor=Color.white;colors.pressedColor=new Color(.76f,.80f,.86f,1);
                    colors.disabledColor=new Color(.46f,.46f,.46f,.7f);colors.colorMultiplier=1;colors.fadeDuration=.08f;
                    button.colors=colors;
                    br.gameObject.AddComponent<SplashTitleButtonFeedback>();
                    objects[item.name]=br;rects[item.name]=bounds;
                    switch(item.name)
                    {case "StartButton":view.StartButton=button;break;case "LanguageSelector":view.LanguageButton=button;break;
                     case "SettingsButton":view.SettingsButton=button;break;case "AccountButton":view.AccountButton=button;break;
                     case "HelpButton":view.HelpButton=button;break;}
                }
                RectTransform divider=MakeRect(rt,"TaglineDivider",new Rect(91,1511,760,40),whole);
                var dividerGraphic=divider.gameObject.AddComponent<SplashTitleDivider>();
                dividerGraphic.color=new Color(203/255f,159/255f,79/255f,1);dividerGraphic.raycastTarget=false;
                foreach(TextSpec item in doc.texts)
                {
                    RectTransform tr=MakeRect(objects[item.parent],item.name,RectOf(item.rect),rects[item.parent]);
                    // Deliberate 2x glyph raster density, not accidental Canvas scaling.
                    // Double only the label's local rectangle and font size, then halve
                    // that label's local scale: the authored layout/hit area stays fixed.
                    int density=Mathf.Clamp(item.rasterScale,1,3);
                    tr.sizeDelta*=density;
                    tr.localScale=new Vector3(1f/density,1f/density,1);
                    Text text=tr.gameObject.AddComponent<Text>();text.text=item.text;text.fontSize=item.size*density;
                    text.font=item.startFont?(typography.StartFont!=null?typography.StartFont:typography.RegularFont):typography.RegularFont;
                    // Null approved bindings stay null in the asset. Only runtime development may fall back.
                    text.fontStyle=(FontStyle)item.fontStyle;text.alignment=(TextAnchor)item.alignment;
                    text.alignByGeometry=true;
                    text.color=new Color(item.color[0],item.color[1],item.color[2],item.color[3]);
                    text.raycastTarget=false;text.supportRichText=false;text.resizeTextForBestFit=false;
                    text.horizontalOverflow=HorizontalWrapMode.Overflow;text.verticalOverflow=VerticalWrapMode.Overflow;
                    SplashTitleTextGeometry geometry=tr.gameObject.AddComponent<SplashTitleTextGeometry>();
                    geometry.CenterHorizontally=item.centerHorizontally;
                    geometry.CenterVertically=item.centerVertically;
                    geometry.SnapToScreenPixels=item.pixelSnap;
                    geometry.OpticalOffset=new Vector2(item.opticalOffset[0],item.opticalOffset[1])*density;
                    // Small labels have no doubled dark shadow. The primary label
                    // retains one restrained emboss shadow, AFTER geometry alignment.
                    if(item.shadow)
                    {
                        Shadow shadow=tr.gameObject.AddComponent<Shadow>();
                        shadow.effectColor=new Color(0,0,0,.55f);
                        shadow.effectDistance=new Vector2(0,-density);shadow.useGraphicAlpha=true;
                    }
                    switch(item.name)
                    {case "VersionText":view.VersionText=text;break;case "LanguageText":view.LanguageText=text;break;
                     case "StartText":view.StartText=text;break;case "SettingsText":view.SettingsText=text;break;
                     case "AccountText":view.AccountText=text;break;case "HelpText":view.HelpText=text;break;
                     case "TaglineText":view.TaglineText=text;break;case "LegalText":view.LegalText=text;break;}
                }
                ApplyReferenceStyle(root);
                BindResponsiveLayout(root);
                if(!view.HasRequiredBindings)throw new InvalidOperationException("New Splash prefab has incomplete bindings.");
                bool savedSuccessfully;
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath, out savedSuccessfully);
                if (!savedSuccessfully) throw new IOException("Unity did not save the canonical SplashTitle prefab.");
                AssetDatabase.ImportAsset(PrefabPath, ImportAssetOptions.ForceSynchronousImport);
                GameObject saved = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                SplashTitleView savedView = saved != null ? saved.GetComponent<SplashTitleView>() : null;
                if (savedView == null || !savedView.HasRequiredBindings)
                    throw new InvalidOperationException("Saved SplashTitle prefab failed its read-back check: " +
                        (savedView == null ? "SplashTitleView unavailable after import" : savedView.GetValidationFailure()));
                Debug.Log("Splash PrefabFix07: canonical layered prefab saved and read back at revision " +
                    savedView.Revision + ". Existing PNGs and authoritative layout were used. " +
                    "Exact font and runtime visual acceptance remain separate checks.");
            }
            finally { UnityEngine.Object.DestroyImmediate(root); }
        }
        static void ApplyReferenceStyle(GameObject root)
        {
            const string referenceArt="Assets/LegendsOfRus/Resources/ProductionUI/SplashReferenceAligned/";
            foreach(string file in Directory.GetFiles(referenceArt,"*.png")) {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;
                importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;importer.spriteBorder=Vector4.zero;importer.SaveAndReimport();
            }
            LayoutDoc doc=JsonUtility.FromJson<LayoutDoc>(File.ReadAllText(LayoutPath));
            if(doc==null||doc.schemaVersion!=4)throw new InvalidOperationException("Unsupported Splash layout schema.");
            Rect whole=new Rect(0,0,doc.referenceWidth,doc.referenceHeight);
            var rootRect=(RectTransform)root.transform;
            foreach(ArtSpec item in doc.art) {
                var child=root.transform.Find(item.name);
                Image image=child?child.GetComponent<Image>():MakeImage(rootRect,item.name,RequireSprite(item.file),RectOf(item.rect),whole,false);
                image.sprite=RequireSprite(item.file);ApplyRect(image.rectTransform,RectOf(item.rect),whole,1);image.preserveAspect=true;
                image.color=item.color!=null&&item.color.Length==4?new Color(item.color[0],item.color[1],item.color[2],item.color[3]):Color.white;
                image.raycastTarget=false;image.gameObject.SetActive(!item.hidden);
                var fade=image.GetComponent<SplashTitleCharacterFade>();if(fade){fade.BottomFraction=item.fadeBottom;fade.enabled=item.fadeBottom>0;}
            }
            var view=root.GetComponent<SplashTitleView>();
            foreach(ButtonSpec item in doc.buttons) {
                var tr=(RectTransform)root.transform.Find(item.name);Rect bounds=RectOf(item.rect);ApplyRect(tr,bounds,whole,1);
                var image=tr.Find("Artwork").GetComponent<Image>();image.sprite=RequireSprite(item.file);
                ApplyRect(image.rectTransform,RectOf(item.artRect),bounds,1);image.preserveAspect=true;tr.gameObject.SetActive(!item.hidden);
            }
            foreach(TextSpec item in doc.texts) {
                var parent=string.IsNullOrEmpty(item.parent)?root.transform:root.transform.Find(item.parent);
                var text=parent.Find(item.name).GetComponent<Text>();Rect parentBounds=whole;
                foreach(ButtonSpec button in doc.buttons)if(button.name==item.parent)parentBounds=RectOf(button.rect);
                int density=Mathf.Clamp(item.rasterScale,1,3);ApplyRect(text.rectTransform,RectOf(item.rect),parentBounds,density);
                text.fontSize=item.size*density;text.fontStyle=(FontStyle)item.fontStyle;text.alignment=(TextAnchor)item.alignment;
                text.text=item.text;text.raycastTarget=false;text.gameObject.SetActive(!item.hidden);
                var geometry=text.GetComponent<SplashTitleTextGeometry>();geometry.CenterHorizontally=item.centerHorizontally;
                geometry.CenterVertically=item.centerVertically;geometry.OpticalOffset=new Vector2(item.opticalOffset[0],item.opticalOffset[1])*density;
            }
            view.ReferenceVersionFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            view.ReferenceTitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            view.ReferenceStartRussian=RequireSprite("ProductionUI/SplashReferenceAligned/source_start_lettered_ru.png");
            view.ReferenceStartBlank=RequireSprite("ProductionUI/SplashReferenceAligned/source_start_blank.png");
            view.StartText.enabled=false;
            // These are the existing selected fonts, not an assertion of exact source-font identity.
            if(!view.ReferenceVersionFont||!view.ReferenceTitleFont)throw new InvalidOperationException("Missing embedded Cyrillic fonts.");

            view.StartText.color=new Color32(244,199,100,255);
            view.StartText.material=AssetDatabase.LoadAssetAtPath<Material>("Assets/LegendsOfRus/Resources/"+ProductionHomeRuntime.PlayGoldMaterialResource+".mat");
            var gold=view.StartText.GetComponent<ProductionHomeGoldText>()??view.StartText.gameObject.AddComponent<ProductionHomeGoldText>();gold.enabled=true;gold.BeveledFace=true;
            var shadow=view.StartText.GetComponent<Shadow>()??view.StartText.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(34,15,2,230);shadow.effectDistance=new Vector2(0,-2);
            root.transform.Find("FooterContrast").GetComponent<SplashTitleFooterShade>().color=Color.clear;
            root.transform.Find("TaglineDivider").gameObject.SetActive(false);
            root.transform.Find("SourceOuterFrame").SetAsLastSibling();view.StartButton.transform.SetAsLastSibling();view.VersionText.transform.SetAsLastSibling();
        }
        static void ApplyRect(RectTransform tr,Rect r,Rect parent,int density)
        {
            tr.anchorMin=tr.anchorMax=tr.pivot=new Vector2(0,1);tr.anchoredPosition=new Vector2(r.x-parent.x,-(r.y-parent.y));
            tr.sizeDelta=r.size*density;tr.localScale=new Vector3(1f/density,1f/density,1);
        }

        static void BindResponsiveLayout(GameObject root)
        {
            SplashTitleForegroundLayout layout = root.GetComponent<SplashTitleForegroundLayout>();
            if (layout == null) layout = root.AddComponent<SplashTitleForegroundLayout>();
            layout.ReferenceComposition = true;
            layout.Logo = root.transform.Find("Logo") as RectTransform;
            layout.Version = root.transform.Find("VersionText") as RectTransform;
            layout.Language = root.transform.Find("LanguageSelector") as RectTransform;
            Transform shade = root.transform.Find("FooterContrast");
            layout.FooterShade = shade != null ? shade.GetComponent<SplashTitleFooterShade>() : null;
            layout.CaptureAuthoredLayout();
            if (!layout.HasRequiredBindings) throw new InvalidOperationException("Splash responsive layout has incomplete bindings.");
        }
        static Rect RectOf(float[] v)
        {if(v==null||v.Length!=4)throw new ArgumentException("Expected x/y/w/h rectangle.");return new Rect(v[0],v[1],v[2],v[3]);}
        static RectTransform MakeRect(RectTransform parent,string name,Rect r,Rect parentRect)
        {
            GameObject go=new GameObject(name,typeof(RectTransform));go.layer=5;go.transform.SetParent(parent,false);
            RectTransform rt=go.GetComponent<RectTransform>();rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(0,1);
            rt.anchoredPosition=new Vector2(r.x-parentRect.x,-(r.y-parentRect.y));rt.sizeDelta=r.size;return rt;
        }
        static Image MakeImage(RectTransform parent,string name,Sprite sprite,Rect r,Rect parentRect,bool interactive)
        {
            RectTransform rt=MakeRect(parent,name,r,parentRect);Image image=rt.gameObject.AddComponent<Image>();
            image.sprite=sprite;image.type=Image.Type.Simple;image.preserveAspect=true;image.raycastTarget=interactive;return image;
        }
        static Sprite RequireSprite(string file)
        {
            string path=file.StartsWith("ProductionUI/", StringComparison.Ordinal)
                ? "Assets/LegendsOfRus/Resources/" + file : ArtRoot+file;
            Sprite sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if(sprite==null)throw new InvalidOperationException("Missing Sprite import: "+path);
            return sprite;
        }
        [Serializable] public class LayoutDoc
        {public int schemaVersion;public float referenceWidth;public float referenceHeight;public ArtSpec[] art;public ButtonSpec[] buttons;public TextSpec[] texts;public ShadeSpec footerShade;}
        [Serializable] public class ArtSpec {public string name;public string file;public float[] rect,color;public float fadeBottom;public bool hidden;}
        [Serializable] public class ButtonSpec {public string name;public string file;public float[] rect;public float[] artRect;public bool hidden;}
        [Serializable] public class ShadeSpec {public float[] rect;public float[] color;}
        [Serializable] public class TextSpec
        {
            public string name,parent,text;public float[] rect,color,opticalOffset;
            public int size,alignment,fontStyle,rasterScale;
            public bool hidden;public bool startFont,centerHorizontally,centerVertically,pixelSnap,shadow;
        }
    }
    /// <summary>Asset imports and domain reloads can finish after InitializeOnLoad.</summary>
    internal sealed class SplashTitlePrefabImportWatch : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (didDomainReload || TouchesSplash(importedAssets) || TouchesSplash(deletedAssets) ||
                TouchesSplash(movedAssets) || TouchesSplash(movedFromAssetPaths))
                Step20SplashTitleR3PrefabRebuilder.QueueEnsure();
        }

        static bool TouchesSplash(string[] paths)
        {
            foreach (string path in paths)
                if (path == Step20SplashTitleR3PrefabRebuilder.PrefabPath ||
                    path.StartsWith(Step20SplashTitleR3PrefabRebuilder.ConfigRoot, StringComparison.Ordinal) ||
                    path.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/FrontEndHomeAligned/", StringComparison.Ordinal) ||
                    path.StartsWith(Step20SplashTitleR3PrefabRebuilder.ArtRoot, StringComparison.Ordinal))
                    return true;
            return false;
        }
    }

}
#endif
