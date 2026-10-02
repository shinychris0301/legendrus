using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    /// <summary>Actual production Splash captures; isolated saves, no prefab or scene edits.</summary>
    public sealed class SplashResponsivePlayModeTests
    {
        ScreenRouter router;
        string previousLanguage;
        bool Baseline => Environment.GetEnvironmentVariable("LOR_SPLASH_BASELINE") == "1";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated))
                Assert.Ignore("Use an isolated matching XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT.");
            isolated = Path.GetFullPath(isolated).TrimEnd(Path.DirectorySeparatorChar);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(
                isolated + Path.DirectorySeparatorChar, StringComparison.Ordinal), Is.True,
                "The production bootstrap must not access the normal user save directory.");
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo("Assets/Scenes/Bootstrap.unity"));
            Assert.That(ScreenRouter.Instance, Is.SameAs(router));
            Assert.That(GameState.Instance.Save, Is.Not.Null);
            previousLanguage = DemoFeatureState.LanguageCode;
            DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.SplashTitle);
            Settle();
            yield return WaitForNormalButtonColors();
            AssertSingleInfrastructure();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (router != null) { router.Show(ScreenId.SplashTitle); Settle(); }
            previousLanguage = null;
            router = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator SplashPortraits_PreserveReferenceCompositionAndSafeControls()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_SPLASH_CAPTURE_DIR")))
                Assert.Ignore("Set LOR_SPLASH_CAPTURE_DIR to retain actual Unity screenshots and geometry evidence.");
            Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null));
            SplashTitleView view = SplashView();
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(view.VersionText.text, Is.EqualTo("v" + Application.version));
            Assert.That(view.StartText.enabled, Is.False, "Russian source lettering must not be doubled by live text.");
            Assert.That(((Image)view.StartButton.targetGraphic).sprite, Is.SameAs(view.ReferenceStartRussian));
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Splash_941x1672");
            WithDisplay(view, new Vector2Int(576, 1280), new Rect(0, 0, 576, 1280), "Splash_576x1280");
            WithDisplay(view, new Vector2Int(576, 1280), new Rect(0, 0, 576, 1230), "Splash_576x1280_inferredsafe");
            WithDisplay(view, new Vector2Int(1080, 2340), new Rect(0, 0, 1080, 2340), "Splash_1080x2340");
            WithDisplay(view, new Vector2Int(1080, 2400), new Rect(45, 100, 990, 2160), "Splash_1080x2400_safearea");
            // A wide Editor view and a subsequent portrait must not accumulate offsets.
            WithDisplay(view, new Vector2Int(1672, 941), new Rect(0, 0, 1672, 941), null);
            DemoFeatureState.SetLanguage("en"); view.RefreshContent();
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Splash_941x1672_EN");
            Assert.That(view.StartText.text, Is.EqualTo("TAP TO START"));
            Assert.That(view.StartText.enabled, Is.True);
            Assert.That(((Image)view.StartButton.targetGraphic).sprite, Is.SameAs(view.ReferenceStartBlank));
            DemoFeatureState.SetLanguage("ru"); view.RefreshContent();
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Splash_941x1672_roundtrip");
            Assert.That(view.StartText.enabled, Is.False);
            Assert.That(((Image)view.StartButton.targetGraphic).sprite, Is.SameAs(view.ReferenceStartRussian));
            AssertCapturePixelsEqual(OutputPath("Splash_941x1672.png"), OutputPath("Splash_941x1672_roundtrip.png"));
            AssertSingleInfrastructure();
            yield return null;
        }

        [UnityTest]
        public IEnumerator SplashStart_VisibleSourceButtonReceivesPointerAndNavigatesOnce()
        {
            SplashTitleView view=SplashView();int before=router.RouteGeneration;
            WithDisplay(view,new Vector2Int(576,1280),new Rect(0,0,576,1230),null,camera=>{
                Rect bounds=ScreenBounds(view.StartButton.GetComponent<RectTransform>(),camera);
                var pointer=new PointerEventData(EventSystem.current){position=bounds.center,button=PointerEventData.InputButton.Left};
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);GameObject hit=null;
                foreach(var result in hits)if(result.gameObject.GetComponentInParent<Button>()==view.StartButton){hit=result.gameObject;break;}
                Assert.That(hit,Is.Not.Null,"Source Start must be reachable through GraphicRaycaster");
                ExecuteEvents.ExecuteHierarchy(hit,pointer,ExecuteEvents.pointerClickHandler);
                ExecuteEvents.ExecuteHierarchy(hit,pointer,ExecuteEvents.pointerClickHandler);
                Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Loading));Assert.That(router.RouteGeneration,Is.EqualTo(before+1));
            });
            AssertSingleInfrastructure();yield return null;
        }

        internal static void AssertCapturePixelsEqual(string before, string after)
        {
            var a = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            var b = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                Assert.That(a.LoadImage(File.ReadAllBytes(before)), Is.True);
                Assert.That(b.LoadImage(File.ReadAllBytes(after)), Is.True);
                Assert.That(new Vector2Int(a.width, a.height), Is.EqualTo(new Vector2Int(b.width, b.height)));
                Color32[] first = a.GetPixels32(), second = b.GetPixels32();
                int changed = 0, maximumDelta = 0;
                for (int i = 0; i < first.Length; i++) {
                    if (!first[i].Equals(second[i])) changed++;
                    maximumDelta = Mathf.Max(maximumDelta, Mathf.Abs(first[i].r-second[i].r), Mathf.Abs(first[i].g-second[i].g), Mathf.Abs(first[i].b-second[i].b), Mathf.Abs(first[i].a-second[i].a));
                }
                // Dynamic font atlas repacking produced nine one-byte rounding differences
                // in the gold shader. Permit only this subvisible variance, never movement.
                Assert.That(maximumDelta, Is.LessThanOrEqualTo(1), "Round-trip must preserve colors to one byte.");
                Assert.That(changed, Is.LessThanOrEqualTo(16), "Round-trip must preserve the composition and live lettering.");
            }
            finally { UnityEngine.Object.Destroy(a); UnityEngine.Object.Destroy(b); }
        }

        IEnumerator WaitForNormalButtonColors()
        {
            float deadline = Time.realtimeSinceStartup + 2;
            int stableFrames = 0;
            do
            {
                yield return null;
                bool stable = true;
                foreach (Button button in SplashView().Buttons())
                {
                    if(!button.gameObject.activeInHierarchy)continue;
                    Color expected = button.colors.normalColor * button.colors.colorMultiplier;
                    Color actual = button.targetGraphic.canvasRenderer.GetColor();
                    stable &= button.IsInteractable() && Mathf.Abs(actual.r - expected.r) < .0001f &&
                        Mathf.Abs(actual.g - expected.g) < .0001f && Mathf.Abs(actual.b - expected.b) < .0001f &&
                        Mathf.Abs(actual.a - expected.a) < .0001f;
                }
                stableFrames = stable ? stableFrames + 1 : 0;
            } while (stableFrames < 2 && Time.realtimeSinceStartup < deadline);
            Assert.That(stableFrames, Is.EqualTo(2), "Capture only after native button fades have completed.");
        }

        void WithDisplay(SplashTitleView view, Vector2Int size, Rect safeRect, string name, Action<Camera> action = null)
        {
            Canvas canvas = router.GameCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            AdaptiveCanvasScaler adaptive = canvas.GetComponent<AdaptiveCanvasScaler>();
            SafeAreaFitter safe = router.SafeArea.GetComponent<SafeAreaFitter>();
            RenderMode oldMode = canvas.renderMode;
            Camera oldCamera = canvas.worldCamera;
            float oldDistance = canvas.planeDistance, oldScale = canvas.scaleFactor;
            bool oldScalerEnabled = scaler.enabled, oldAdaptiveEnabled = adaptive.enabled, oldSafeEnabled = safe.enabled;
            GameObject cameraObject = null;
            RenderTexture target = null;
            Texture2D texture = null;
            RenderTexture oldActive = RenderTexture.active;
            try
            {
                adaptive.enabled = false; scaler.enabled = false; safe.enabled = false;
                target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
                target.Create();
                cameraObject = new GameObject("SplashTestCaptureCamera", typeof(Camera));
                Camera camera = cameraObject.GetComponent<Camera>();
                camera.enabled = false;
                camera.transform.position = new Vector3(0, 0, -100);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(4, 13, 24, 255);
                camera.orthographic = true;
                camera.nearClipPlane = .1f; camera.farClipPlane = 1000f;
                camera.targetTexture = target;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 100f;
                adaptive.Apply(safeRect, size);
                canvas.scaleFactor = AdaptiveCanvasScaler.ResolveScale(
                    new Vector2Int(Mathf.RoundToInt(safeRect.width), Mathf.RoundToInt(safeRect.height)), 941, 1672);
                safe.Apply(safeRect, size);
                // Match Home's synchronous display fixture: refresh glyph geometry at the new Canvas scale.
                foreach (Text label in view.GetComponentsInChildren<Text>(true)) label.SetVerticesDirty();
                if (!Baseline)
                {
                    // Exercise the production callbacks after one normal Canvas pass.
                    // Do not call the foreground component or force layout rebuilds first.
                    Canvas.ForceUpdateCanvases();
                    AssertAutomaticFirstPass(view, camera, size, safeRect, canvas.scaleFactor);
                }
                for (int i = 0; i < 3; i++) ApplyLayouts(view);
                RectTransform logo = view.transform.Find("Logo") as RectTransform;
                SplashTitleFooterShade shade = view.transform.Find("FooterContrast").GetComponent<SplashTitleFooterShade>();
                Assert.That(logo, Is.Not.Null);
                Assert.That(router.Viewport.rect.width, Is.EqualTo(941f).Within(1f));
                Assert.That(router.Viewport.rect.height, Is.EqualTo(1672f).Within(1f));
                AssertAuthoredRect(view,"Logo",new Rect(66,88,806,601));
                AssertAuthoredRect(view,"Ivan",new Rect(16,644,910,1010));
                AssertAuthoredRect(view,"StartButton",new Rect(159,1356,629,198));
                AssertAuthoredRect(view,"SourceOuterFrame",new Rect(0,0,941,1672));
                Assert.That(view.transform.Find("Masha").gameObject.activeSelf,Is.False);
                Assert.That(view.transform.Find("Firebird").gameObject.activeSelf,Is.False);
                Rect logoBounds = ScreenBounds(logo, camera);
                Rect versionBounds = ScreenBounds(view.VersionText.rectTransform, camera);
                Rect languageBounds = ScreenBounds(view.LanguageButton.GetComponent<RectTransform>(), camera);
                Rect shadeBounds = ScreenBounds(shade.rectTransform, camera);
                Rect envBounds = ScreenBounds(view.GetComponent<SplashTitleResponsiveLayout>().Background.rectTransform, camera);
                Assert.That(envBounds.xMin, Is.LessThanOrEqualTo(1f));
                Assert.That(envBounds.yMin, Is.LessThanOrEqualTo(1f));
                Assert.That(envBounds.xMax, Is.GreaterThanOrEqualTo(size.x - 1f));
                Assert.That(envBounds.yMax, Is.GreaterThanOrEqualTo(size.y - 1f));
                if (!Baseline)
                {
                    foreach(var bounds in new[]{logoBounds,versionBounds,ScreenBounds(view.StartButton.GetComponent<RectTransform>(),camera)}) {
                        Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(safeRect.xMin-1));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(safeRect.xMax+1));
                        Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(safeRect.yMin-1));Assert.That(bounds.yMax,Is.LessThanOrEqualTo(safeRect.yMax+1));
                    }
                }
                for (int i = 0; i < 12; i++) ApplyLayouts(view);
                AssertRect(ScreenBounds(logo, camera), logoBounds, .01f, "Repeated ApplyNow must not drift the logo.");
                AssertRect(ScreenBounds(view.VersionText.rectTransform, camera), versionBounds, .01f, "Version drift");
                AssertRect(ScreenBounds(view.LanguageButton.GetComponent<RectTransform>(), camera), languageBounds, .01f, "Language drift");
                AssertRect(ScreenBounds(shade.rectTransform, camera), shadeBounds, .01f, "Shade drift");
                camera.Render();
                if (!Baseline) Assert.That(shade.color.a,Is.Zero,"Reference has no extra bottom contrast band");
                if (name != null)
                {
                    RenderTexture.active = target;
                    texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
                    texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); texture.Apply();
                    string output = OutputPath(name + ".png");
                    Assert.That(File.Exists(output), Is.False, "Preserve prior capture evidence.");
                    File.WriteAllBytes(output, texture.EncodeToPNG());
                    WriteEvidence(name + ".json", new CaptureEvidence
                    {
                        unityVersion = Application.unityVersion,
                        capture = "Actual Unity Play Mode production Splash on the existing router Canvas",
                        mode = Baseline ? "BEFORE correction, capture-only baseline" : "AFTER correction, validated geometry",
                        fixture = DemoFeatureState.LanguageCode + " language, live Application.version; motion settled; glyph meshes refreshed for synchronous simulated display/safe area, not physical-device evidence",
                        safeAreaNote = name.Contains("inferredsafe") ? "Top inset 50px is inferred from the user image; actual phone safe area is unknown" : "Explicit layout test rectangle",
                        width = size.x, height = size.y, safeArea = safeRect,
                        logo = logoBounds, version = versionBounds, language = languageBounds,
                        footerShade = shadeBounds, environment = envBounds,
                        viewport = ScreenBounds(router.Viewport, camera),
                        scale = canvas.scaleFactor,
                        automaticLayoutFirstCanvasPass = !Baseline,
                        graphicsDevice = SystemInfo.graphicsDeviceType.ToString()
                    });
                }
                action?.Invoke(camera);
            }
            finally
            {
                RenderTexture.active = oldActive;
                canvas.renderMode = oldMode; canvas.worldCamera = oldCamera;
                canvas.planeDistance = oldDistance; canvas.scaleFactor = oldScale;
                scaler.enabled = oldScalerEnabled; adaptive.enabled = oldAdaptiveEnabled; safe.enabled = oldSafeEnabled;
                adaptive.ApplyNow(); safe.ApplyNow();
                if (view != null) ApplyLayouts(view);
                if (texture != null) UnityEngine.Object.Destroy(texture);
                if (cameraObject != null) UnityEngine.Object.Destroy(cameraObject);
                if (target != null) { target.Release(); UnityEngine.Object.Destroy(target); }
            }
        }

        static void AssertAutomaticFirstPass(SplashTitleView view, Camera camera, Vector2Int size, Rect safeRect, float scale)
        {
            Assert.That(view.GetComponent("SplashTitleForegroundLayout"), Is.Not.Null,
                "The actual serialized Splash must own automatic foreground layout.");
            Assert.That(view.GetComponent<SplashTitleForegroundLayout>().ReferenceComposition,Is.True);
            AssertAuthoredRect(view,"Logo",new Rect(66,88,806,601));
            Assert.That(view.VersionText.rectTransform.anchoredPosition,Is.EqualTo(new Vector2(414,-1570)));
            Assert.That(view.StartButton.gameObject.activeSelf,Is.True);
            foreach(var hidden in new[]{view.LanguageButton,view.SettingsButton,view.AccountButton,view.HelpButton})Assert.That(hidden.gameObject.activeSelf,Is.False);
        }

        void ApplyLayouts(SplashTitleView view)
        {
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(router.SafeArea);
            view.GetComponent<SplashTitleResponsiveLayout>().ApplyNow();
            Component foreground = view.GetComponent("SplashTitleForegroundLayout");
            if (!Baseline) Assert.That(foreground, Is.Not.Null, "The active serialized Splash must own its foreground layout.");
            if (foreground != null)
            {
                MethodInfo apply = foreground.GetType().GetMethod("ApplyNow", BindingFlags.Public | BindingFlags.Instance);
                Assert.That(apply, Is.Not.Null);
                apply.Invoke(foreground, null);
            }
            Canvas.ForceUpdateCanvases();
        }

        static void AssertShadeContinuity(SplashTitleFooterShade shade)
        {
            // Unity 6000.5 exposes the renderer-owned mesh; do not destroy it.
            Mesh mesh = shade.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null, "The footer contrast graphic must own a rendered mesh.");
            Assert.That(mesh.vertexCount, Is.GreaterThanOrEqualTo(4), "The footer contrast graphic must render an actual mesh.");
            Rect rect = shade.GetPixelAdjustedRect();
            float x = rect.center.x;
            Assert.That(AlphaAt(mesh, new Vector2(x, rect.yMax - 183f)),
                Is.EqualTo(shade.color.a * .5f).Within(.004f), "The authored 366-unit fade must retain its midpoint opacity.");
            if (rect.height > 370f)
            {
                Assert.That(AlphaAt(mesh, new Vector2(x, rect.yMax - 367f)),
                    Is.EqualTo(shade.color.a).Within(.004f), "Shade must remain continuous below the original viewport edge.");
                Assert.That(AlphaAt(mesh, new Vector2(x, rect.yMin + 2f)),
                    Is.EqualTo(shade.color.a).Within(.004f), "The additional lower display must retain constant footer opacity.");
            }
        }

        static float AlphaAt(Mesh mesh, Vector2 point)
        {
            Vector3[] vertices = mesh.vertices;
            Color32[] colors = mesh.colors32;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int ai = triangles[i], bi = triangles[i + 1], ci = triangles[i + 2];
                Vector2 a = vertices[ai], b = vertices[bi], c = vertices[ci];
                float denominator = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
                if (Mathf.Abs(denominator) < .0001f) continue;
                float wa = ((b.y - c.y) * (point.x - c.x) + (c.x - b.x) * (point.y - c.y)) / denominator;
                float wb = ((c.y - a.y) * (point.x - c.x) + (a.x - c.x) * (point.y - c.y)) / denominator;
                float wc = 1f - wa - wb;
                if (wa < -.0001f || wb < -.0001f || wc < -.0001f) continue;
                return (wa * colors[ai].a + wb * colors[bi].a + wc * colors[ci].a) / 255f;
            }
            Assert.Fail("No rendered footer triangle covers " + point); return 0f;
        }

        static void AssertTop(Rect bounds, Rect safe, float authoredTop, float scale, string label)
        {
            Assert.That(safe.yMax - bounds.yMax, Is.EqualTo(authoredTop * scale).Within(1f), label + " must anchor to the safe top.");
        }

        static void AssertAuthoredRect(SplashTitleView view, string name, Rect expected)
        {
            RectTransform root = (RectTransform)view.transform;
            RectTransform child = view.transform.Find(name) as RectTransform;
            Assert.That(child, Is.Not.Null, name);
            Vector3[] corners = new Vector3[4]; child.GetWorldCorners(corners);
            Vector3 lo = root.InverseTransformPoint(corners[0]), hi = root.InverseTransformPoint(corners[2]);
            Rect authored = new Rect(lo.x - root.rect.xMin, root.rect.yMax - hi.y, hi.x - lo.x, hi.y - lo.y);
            AssertRect(authored, expected, .03f, name + " authored geometry must be preserved.");
        }

        static void AssertRect(Rect actual, Rect expected, float tolerance, string label)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(tolerance), label);
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(tolerance), label);
            Assert.That(actual.width, Is.EqualTo(expected.width).Within(tolerance), label);
            Assert.That(actual.height, Is.EqualTo(expected.height).Within(tolerance), label);
        }

        static Rect ScreenBounds(RectTransform rect, Camera camera)
        {
            Vector3[] corners = new Vector3[4]; rect.GetWorldCorners(corners);
            Vector2 min = camera.WorldToScreenPoint(corners[0]), max = min;
            foreach (Vector3 corner in corners)
            { Vector2 point = camera.WorldToScreenPoint(corner); min = Vector2.Min(min, point); max = Vector2.Max(max, point); }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        SplashTitleView SplashView()
        {
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.SplashTitle));
            foreach (Transform child in router.Viewport)
                if (child.name == "Screen_SplashTitle" && child.gameObject.activeInHierarchy)
                    return child.GetComponentInChildren<SplashTitleView>();
            Assert.Fail("Missing active production Splash root."); return null;
        }

        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            foreach (SplashTitleLivingMotion motion in router.GetComponentsInChildren<SplashTitleLivingMotion>(true)) motion.enabled = false;
            Canvas.ForceUpdateCanvases();
        }

        void AssertSingleInfrastructure()
        {
            Assert.That(UnityEngine.Object.FindObjectsByType<ScreenRouter>().Length, Is.EqualTo(1));
            Assert.That(router.GetComponentsInChildren<Canvas>(true).Length, Is.EqualTo(1));
            Assert.That(UnityEngine.Object.FindObjectsByType<EventSystem>().Length, Is.EqualTo(1));
        }

        static string OutputPath(string filename)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_SPLASH_CAPTURE_DIR");
            Assert.That(Path.IsPathRooted(directory), Is.True); Directory.CreateDirectory(directory);
            return Path.Combine(directory, filename);
        }

        static void WriteEvidence(string filename, object evidence)
        { File.WriteAllText(OutputPath(filename), JsonUtility.ToJson(evidence, true) + "\n"); }

        [Serializable] sealed class CaptureEvidence
        {
            public string unityVersion, capture, mode, fixture, safeAreaNote, graphicsDevice;
            public int width, height;
            public Rect safeArea, logo, version, language, footerShade, environment, viewport;
            public float scale;
            public bool automaticLayoutFirstCanvasPass;
        }
    }
}
