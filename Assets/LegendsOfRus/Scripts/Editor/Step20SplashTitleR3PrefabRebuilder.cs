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
            if (!File.Exists(LayoutPath)) throw new FileNotFoundException("Missing authoritative layout",LayoutPath);
            LayoutDoc doc = JsonUtility.FromJson<LayoutDoc>(File.ReadAllText(LayoutPath));
            if (doc == null || doc.schemaVersion != 4) throw new InvalidOperationException("Unsupported Splash layout schema.");
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
            string path=ArtRoot+file;Sprite sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if(sprite==null)throw new InvalidOperationException("Missing Sprite import: "+path);
            return sprite;
        }
        [Serializable] public class LayoutDoc
        {public int schemaVersion;public float referenceWidth;public float referenceHeight;public ArtSpec[] art;public ButtonSpec[] buttons;public TextSpec[] texts;public ShadeSpec footerShade;}
        [Serializable] public class ArtSpec {public string name;public string file;public float[] rect;public float fadeBottom;}
        [Serializable] public class ButtonSpec {public string name;public string file;public float[] rect;public float[] artRect;}
        [Serializable] public class ShadeSpec {public float[] rect;public float[] color;}
        [Serializable] public class TextSpec
        {
            public string name,parent,text;public float[] rect,color,opticalOffset;
            public int size,alignment,fontStyle,rasterScale;
            public bool startFont,centerHorizontally,centerVertically,pixelSnap,shadow;
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
                    path.StartsWith(Step20SplashTitleR3PrefabRebuilder.ArtRoot, StringComparison.Ordinal))
                    return true;
            return false;
        }
    }

}
#endif
