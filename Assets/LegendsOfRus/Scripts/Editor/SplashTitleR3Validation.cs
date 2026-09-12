#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    public static class SplashTitleR3Validation
    {
        public const string OutputRoot="ProjectDocs/SplashTitle_R3_FidelityFix02/RuntimeEvidence";
        [MenuItem("Legends of Rus/Production UI/Splash R3/Validate Serialized Wiring")]
        public static void ValidateMenu()
        {
            List<string> issues=ValidateWiring();
            if(issues.Count>0) {Debug.LogError("Splash R3 serialized wiring FAIL:\n"+string.Join("\n",issues));return;}
            Debug.Log("Splash R3 serialized wiring PASS. This is not a compilation, Play Mode, font-match, or visual-fidelity pass.");
            var type=AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(Step20SplashTitleR3PrefabRebuilder.TypographyPath);
            if(type==null||!type.HasVerifiedBindings)Debug.LogWarning("Splash R3 exact-font gate OPEN: original source font bindings/evidence are absent.");
        }
        public static List<string> ValidateWiring()
        {
            GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Step20SplashTitleR3PrefabRebuilder.PrefabPath);
            return ValidateWiring(prefab);
        }
        public static List<string> ValidateWiring(GameObject prefab)
        {
            var issues=new List<string>();
            if(prefab==null){issues.Add("Canonical prefab could not be imported.");return issues;}
            SplashTitleView view=prefab.GetComponent<SplashTitleView>();
            if(view==null)issues.Add("Canonical prefab has no root SplashTitleView.");
            else if(!view.HasRequiredBindings)issues.Add(view.GetValidationFailure());
            if(prefab.GetComponentsInChildren<Button>(true).Length!=5)issues.Add("Expected exactly five real Button components.");
            if(prefab.GetComponentsInChildren<Text>(true).Length!=8)issues.Add("Expected eight independent live Text components.");
            if(prefab.GetComponentsInChildren<Canvas>(true).Length!=0)issues.Add("Nested Canvas unexpectedly added.");
            if(prefab.GetComponentsInChildren<AnimatedHotspot>(true).Length!=0)issues.Add("Legacy AnimatedHotspot present.");
            foreach(Image image in prefab.GetComponentsInChildren<Image>(true))
            {
                if(image.sprite==null)issues.Add("Missing sprite: "+image.name);
                if(!image.preserveAspect)issues.Add("Sprite aspect ratio is not preserved: "+image.name);
                // Imported prefab ancestors are inactive in the hierarchy; button-owned artwork still receives input.
                if(image.raycastTarget && image.GetComponentInParent<Button>(true)==null)issues.Add("Decorative graphic intercepts input: "+image.name);
            }
            foreach(string path in AssetDatabase.GetDependencies(Step20SplashTitleR3PrefabRebuilder.PrefabPath,true))
                if(path.Contains("SplashTitleFinal/")||path.Contains("Art/ReferenceOnly/")||path.Contains("Resources/DesignScreens/"))
                    issues.Add("Forbidden legacy/reference dependency: "+path);
            if(!ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.SplashTitle))issues.Add("Splash cutover is disabled.");
            return issues;
        }
        [MenuItem("Legends of Rus/Production UI/Splash R3/Validate PrefabFix07")]
        public static void ValidatePrefabFix07()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            { Debug.LogError("Stop Play Mode and wait for imports/compilation before validating the saved prefab."); return; }
            try
            {
                List<string> issues = ValidateWiring();
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20SplashTitleR3PrefabRebuilder.PrefabPath);
                if (prefab != null) ValidateLayout(prefab, issues);
                if (issues.Count != 0)
                { Debug.LogError("Splash PrefabFix07 FAIL:\n" + string.Join("\n", issues)); return; }
                Debug.Log("Splash PrefabFix07 PASS: revision " + SplashTitleView.CurrentRevision +
                    "; bindings and serialized layout match. This is not a Play Mode, visual-fidelity, or exact-font approval.");
            }
            catch (Exception ex) { Debug.LogError("Splash PrefabFix07 validation failed: " + ex.Message); }
        }

        static void ValidateLayout(GameObject prefab, List<string> issues)
        {
            string path = Step20SplashTitleR3PrefabRebuilder.LayoutPath;
            if (!File.Exists(path)) { issues.Add("Missing authoritative layout: " + path); return; }
            var doc = JsonUtility.FromJson<Step20SplashTitleR3PrefabRebuilder.LayoutDoc>(File.ReadAllText(path));
            if (doc == null || doc.schemaVersion != 4 || doc.art == null || doc.buttons == null || doc.texts == null)
            { issues.Add("Unsupported or incomplete layout JSON."); return; }
            Rect whole = new Rect(0, 0, doc.referenceWidth, doc.referenceHeight);
            foreach (var spec in doc.art)
                CheckRect(prefab.transform.Find(spec.name) as RectTransform, spec.rect, whole, 1, spec.name, issues);
            var buttonRects = new Dictionary<string, Rect>();
            foreach (var spec in doc.buttons)
            {
                RectTransform button = prefab.transform.Find(spec.name) as RectTransform;
                Rect bounds = new Rect(spec.rect[0], spec.rect[1], spec.rect[2], spec.rect[3]);
                buttonRects[spec.name] = bounds;
                CheckRect(button, spec.rect, whole, 1, spec.name, issues);
                CheckRect(button != null ? button.Find("Artwork") as RectTransform : null,
                    spec.artRect, bounds, 1, spec.name + "/Artwork", issues);
            }
            foreach (var spec in doc.texts)
            {
                string textPath = string.IsNullOrEmpty(spec.parent) ? spec.name : spec.parent + "/" + spec.name;
                Rect parent = string.IsNullOrEmpty(spec.parent) ? whole : buttonRects[spec.parent];
                CheckRect(prefab.transform.Find(textPath) as RectTransform, spec.rect, parent,
                    Mathf.Clamp(spec.rasterScale, 1, 3), textPath, issues);
            }
        }

        static void CheckRect(RectTransform rect, float[] expected, Rect parent, int density,
            string label, List<string> issues)
        {
            if (rect == null) { issues.Add("Missing RectTransform: " + label); return; }
            if (expected == null || expected.Length != 4) { issues.Add("Invalid layout rectangle: " + label); return; }
            Vector2 position = new Vector2(expected[0] - parent.x, -(expected[1] - parent.y));
            Vector2 size = new Vector2(expected[2], expected[3]) * density;
            const float tolerance = .02f;
            if (Vector2.Distance(rect.anchoredPosition, position) > tolerance ||
                Vector2.Distance(rect.sizeDelta, size) > tolerance ||
                Mathf.Abs(rect.localScale.x - 1f / density) > .0001f ||
                Mathf.Abs(rect.localScale.y - 1f / density) > .0001f ||
                Vector2.Distance(rect.anchorMin, new Vector2(0, 1)) > .0001f ||
                Vector2.Distance(rect.anchorMax, new Vector2(0, 1)) > .0001f ||
                Vector2.Distance(rect.pivot, new Vector2(0, 1)) > .0001f)
                issues.Add("Saved rectangle differs from authoritative layout: " + label);
        }

        [MenuItem("Legends of Rus/Production UI/Splash R3/Capture Current Runtime Screen")]
        public static void CaptureCurrent()
        {
            ScreenRouter router=ScreenRouter.Instance;
            if(!EditorApplication.isPlaying||router==null||router.CurrentScreen!=ScreenId.SplashTitle)
            {Debug.LogError("Start the real Bootstrap scene and wait for SplashTitle before capturing.");return;}
            if(router.IsScreenTransitioning||router.ActiveModal.HasValue)
            {Debug.LogWarning("Wait until the page transition or modal has finished.");return;}
            SplashTitleView view=router.GetComponentInChildren<SplashTitleView>(true);
            if(view==null||!view.ActionsBound){Debug.LogError("The active route has no bound SplashTitleView.");return;}
            QACaptureService capture=router.GetComponent<QACaptureService>();
            if(capture==null){Debug.LogError("Existing QACaptureService not found on the runtime root.");return;}
            Directory.CreateDirectory(OutputRoot);
            string id="SplashTitle_"+Screen.width+"x"+Screen.height+"_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
            var request=new FidelityCaptureRequest
            {
                captureId=id,targetId="splash-r3-fidelityfix02",targetLabel="Actual Splash R3 FidelityFix02 runtime",
                outputDirectory=Path.GetFullPath(OutputRoot),requestedWidth=Screen.width,requestedHeight=Screen.height,showToast=false
            };
            if(!capture.TryRequestCapture(request)){Debug.LogWarning("A QA capture is already pending.");return;}
            var info=new RuntimeInfo
            {
                captureId=id,unityVersion=Application.unityVersion,applicationVersion=Application.version,
                route=router.CurrentScreen.ToString(),routeGeneration=router.RouteGeneration,
                prefabResource=ProductionSplashTitleRuntime.PrefabResource,font=view.ActiveFontSource,
                fontIdentityVerified=view.TypographyVerified,buttonCount=view.Buttons().Length,
                nativeActionsBound=view.ActionsBound,visualMatchVerified=false,
                note="Requested real end-of-frame capture. Check its separate capture-result metadata; this context JSON alone does not prove capture success."
            };
            File.WriteAllText(Path.Combine(OutputRoot,id+"_context.json"),JsonUtility.ToJson(info,true));
        }
        [Serializable] sealed class RuntimeInfo
        {public string captureId,unityVersion,applicationVersion,route,prefabResource,font,note;public int routeGeneration,buttonCount;public bool fontIdentityVerified,nativeActionsBound,visualMatchVerified;}
    }
    /// <summary>Development testing remains possible; release cannot silently use an OS font substitute.</summary>
    public sealed class SplashTitleR3BuildGuard : IPreprocessBuildWithReport
    {
        public int callbackOrder=>50;
        public void OnPreprocessBuild(BuildReport report)
        {
            if(!ProductionMetaPageCutoverRegistry.IsCutover(ScreenId.SplashTitle))return;
            List<string> wiring=SplashTitleR3Validation.ValidateWiring();
            if(wiring.Count>0)throw new BuildFailedException("Splash R3 wiring: "+string.Join("; ",wiring));
            var type=AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(Step20SplashTitleR3PrefabRebuilder.TypographyPath);
            if(type!=null&&type.HasVerifiedBindings)return;
            if((report.summary.options&BuildOptions.Development)!=0)
            {Debug.LogWarning("Development build uses unverified Splash typography; not approved for release.");return;}
            throw new BuildFailedException("SplashTitle exact source fonts are not bound/verified. " +
                "Import and explicitly bind the original licensed fonts locally, or enable Development Build for testing. " +
                "A lookalike font must not be marked source-verified.");
        }
    }
}
#endif
