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
    /// <summary>Uses the production bootstrap/router with isolated save storage; never builds prefabs.</summary>
    public sealed class LoadingAlignmentPlayModeTests
    {
        ScreenRouter router;
        string previousLanguage;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated))
                Assert.Ignore("Launch with matching XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT test directories.");
            isolated = Path.GetFullPath(isolated).TrimEnd(Path.DirectorySeparatorChar);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(
                isolated + Path.DirectorySeparatorChar, StringComparison.Ordinal), Is.True,
                "The actual bootstrap must not access the user's normal save directory.");

            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null, "RuntimeBootstrap must create the production router.");
            ScreenRouter bootstrapRouter = router;
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo("Assets/Scenes/Bootstrap.unity"));
            Assert.That(ScreenRouter.Instance, Is.SameAs(bootstrapRouter));
            Assert.That(GameState.Instance, Is.Not.Null);
            Assert.That(GameState.Instance.Save, Is.Not.Null);
            AssertSingleInfrastructure();
            previousLanguage = DemoFeatureState.LanguageCode;
            DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.SplashTitle);
            SettleMotion();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (router != null)
            {
                router.Show(ScreenId.SplashTitle);
                SettleMotion();
            }
            previousLanguage = null;
            router = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator SplashStart_UsesMeasuredLoadingAndEntersHomeExactlyOnce()
        {
            SplashTitleView splash = ActiveRoot(ScreenId.SplashTitle).GetComponentInChildren<SplashTitleView>();
            Assert.That(splash, Is.Not.Null);
            Assert.That(splash.HasRequiredBindings, Is.True);
            Assert.That(splash.StartButton.interactable, Is.True);
            int beforeStart = router.RouteGeneration;
            splash.StartButton.onClick.Invoke();
            SettleMotion();
            ProductionLoadingView view = LoadingView();
            Assert.That(router.RouteGeneration, Is.EqualTo(beforeStart + 1));
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(view.Logo.sprite, Is.SameAs(Resources.Load<Sprite>(ProductionLoadingRuntime.LogoResource)));
            Assert.That(view.Typography, Is.SameAs(splash.Typography));

            var observed = new List<float>();
            bool completeFrameVisible = false;
            float deadline = Time.realtimeSinceStartup + 30f;
            while (router.CurrentScreen == ScreenId.Loading && Time.realtimeSinceStartup < deadline)
            {
                if (!view.ProgressBar.IsIndeterminate)
                {
                    float progress = view.ProgressBar.Progress;
                    if (observed.Count > 0)
                        Assert.That(progress, Is.GreaterThanOrEqualTo(observed[observed.Count - 1]));
                    Assert.That(progress, Is.InRange(0f, 1f));
                    Assert.That(view.PercentText.text, Is.EqualTo(Mathf.FloorToInt(progress * 100f) + "%"));
                    observed.Add(progress);
                    completeFrameVisible |= progress == 1f && view.PercentText.gameObject.activeInHierarchy;
                }
                Assert.That(view.RetryButton.gameObject.activeSelf, Is.False);
                yield return null;
            }
            SettleMotion();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Home));
            Assert.That(completeFrameVisible, Is.True, "Validated completion must render before leaving Loading.");
            Assert.That(observed.Count, Is.GreaterThan(0));
            Assert.That(router.RouteGeneration, Is.EqualTo(beforeStart + 2));
            int homeGeneration = router.RouteGeneration;
            for (int i = 0; i < 6; i++) yield return null;
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Home));
            Assert.That(router.RouteGeneration, Is.EqualTo(homeGeneration), "No delayed duplicate Home transition.");
            AssertSingleInfrastructure();
            WriteEvidence("ActualLoadingFlow.json", new FlowEvidence
            {
                unityVersion = Application.unityVersion,
                route = "Actual RuntimeBootstrap -> Splash Start Button -> Loading -> Home",
                progressMeaning = "Observed equal-weight Home resource-operation progress; no injected requests or timer",
                observedProgress = observed.ToArray(),
                completionFrameVisible = completeFrameVisible,
                routeChanges = router.RouteGeneration - beforeStart
            });
        }

        [UnityTest]
        public IEnumerator LoadingRetry_UsesLiveErrorStateAndRejectsRepeatedTap()
        {
            router.Show(ScreenId.Loading);
            StopWorkForLayoutFixture();
            SettleMotion();
            ProductionLoadingView view = LoadingView();
            int attempts = 0;
            view.BindRetry(() => attempts++);
            view.SetProgress(.9999f);
            Assert.That(view.PercentText.text, Is.EqualTo("99%"));
            view.ShowError();
            Assert.That(view.RetryButton.interactable, Is.True);
            Assert.That(view.PercentText.gameObject.activeSelf, Is.False);
            Assert.That(view.StatusText.text, Is.EqualTo("Не удалось загрузить"));
            view.RetryButton.onClick.Invoke();
            view.RetryButton.onClick.Invoke();
            Assert.That(attempts, Is.EqualTo(1));
            Assert.That(view.ProgressBar.IsIndeterminate, Is.True);
            Assert.That(view.RetryButton.gameObject.activeSelf, Is.False);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Loading));
            yield return null;
        }

        [UnityTest]
        public IEnumerator LoadingError_RouterRetryCompletesRealWorkAndNavigatesOnce()
        {
            router.Show(ScreenId.Loading);
            StopWorkForLayoutFixture();
            SettleMotion();
            ProductionLoadingView view = LoadingView();
            int loadingGeneration = router.RouteGeneration;
            FieldInfo preparationField = typeof(ScreenRouter).GetField("homePreparation", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(preparationField, Is.Not.Null);
            (preparationField.GetValue(router) as HomeLoadingPreparation)?.Dispose();
            var missing = new HomeLoadingPreparation((key, type) => new MissingResourceRequest());
            preparationField.SetValue(router, missing);
            MethodInfo prepareMethod = typeof(ScreenRouter).GetMethod("PrepareHome", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(prepareMethod, Is.Not.Null);
            Coroutine failure = router.StartCoroutine((IEnumerator)prepareMethod.Invoke(router, new object[] { loadingGeneration, missing }));
            typeof(ScreenRouter).GetField("loadingRoutine", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(router, failure);
            float deadline = Time.realtimeSinceStartup + 5f;
            while (!view.RetryButton.gameObject.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(view.RetryButton.gameObject.activeSelf, Is.True, "A failed resource must expose the router's retry control.");
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Loading));
            Assert.That(router.RouteGeneration, Is.EqualTo(loadingGeneration));
            Assert.That(view.StatusText.text, Is.EqualTo("Не удалось загрузить"));
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_LOADING_CAPTURE_DIR")) &&
                SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null)
                Capture(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Loading_error_941x1672", true);
            view.RetryButton.onClick.Invoke();
            view.RetryButton.onClick.Invoke();
            Assert.That(view.ProgressBar.IsIndeterminate, Is.True);
            Assert.That(view.RetryButton.gameObject.activeSelf, Is.False);
            deadline = Time.realtimeSinceStartup + 30f;
            while (router.CurrentScreen == ScreenId.Loading && Time.realtimeSinceStartup < deadline) yield return null;
            SettleMotion();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Home));
            Assert.That(router.RouteGeneration, Is.EqualTo(loadingGeneration + 1));
            for (int i = 0; i < 4; i++) yield return null;
            Assert.That(router.RouteGeneration, Is.EqualTo(loadingGeneration + 1));
        }

        [UnityTest]
        public IEnumerator LoadingPortraits_RenderActualPageWithExplicitLayoutFixture()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_LOADING_CAPTURE_DIR")))
                Assert.Ignore("Set LOR_LOADING_CAPTURE_DIR to retain actual Unity layout captures.");
            Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null),
                "A real graphics device is required; do not run this capture with -nographics.");
            router.Show(ScreenId.Loading);
            StopWorkForLayoutFixture();
            SettleMotion();
            ProductionLoadingView view = LoadingView();
            view.SetProgress(.42f);
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(view.ProgressBar.GetComponent<CanvasRenderer>(), Is.Not.Null,
                "Custom progress graphics need a renderer as well as valid layout bounds.");
            Assert.That(view.transform.Find(ProductionLoadingRuntime.ContentName + "/LowerContrast")
                .GetComponent<CanvasRenderer>(), Is.Not.Null, "The lower contrast graphic must render.");
            AssertSingleInfrastructure();
            Capture(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Loading_941x1672");
            Capture(view, new Vector2Int(1080, 2340), new Rect(0, 0, 1080, 2340), "Loading_1080x2340");
            Capture(view, new Vector2Int(1080, 2400), new Rect(0, 0, 1080, 2400), "Loading_1080x2400");
            Capture(view, new Vector2Int(1080, 2400), new Rect(45, 100, 990, 2160), "Loading_1080x2400_safearea");
            AssertSingleInfrastructure();
            yield return null;
        }

        void StopWorkForLayoutFixture()
        {
            // The separate flow test runs actual resource work. A visual fixture stops only
            // this attempt's coroutine before requests begin; production navigation stays intact.
            FieldInfo field = typeof(ScreenRouter).GetField("loadingRoutine", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(field, Is.Not.Null);
            Coroutine coroutine = field.GetValue(router) as Coroutine;
            Assert.That(coroutine, Is.Not.Null);
            router.StopCoroutine(coroutine);
            field.SetValue(router, null);
        }

        void Capture(ProductionLoadingView view, Vector2Int size, Rect safeRect, string name, bool errorState = false)
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
                adaptive.enabled = false;
                scaler.enabled = false;
                safe.enabled = false;
                target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
                target.Create();
                cameraObject = new GameObject("LoadingTestCaptureCamera", typeof(Camera));
                Camera camera = cameraObject.GetComponent<Camera>();
                camera.enabled = false;
                camera.transform.position = new Vector3(0, 0, -100);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color32(4, 13, 24, 255);
                camera.orthographic = true;
                camera.nearClipPlane = .1f;
                camera.farClipPlane = 1000f;
                camera.targetTexture = target;
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
                canvas.planeDistance = 100f;
                adaptive.Apply(safeRect, size);
                canvas.scaleFactor = AdaptiveCanvasScaler.ResolveScale(
                    new Vector2Int(Mathf.RoundToInt(safeRect.width), Mathf.RoundToInt(safeRect.height)), 941, 1672);
                safe.Apply(safeRect, size);
                for (int i = 0; i < 3; i++)
                {
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(router.SafeArea);
                    view.GetComponent<SplashTitleResponsiveLayout>().ApplyNow();
                }
                Assert.That(router.Viewport.rect.width, Is.EqualTo(941f).Within(1f));
                Assert.That(router.Viewport.rect.height, Is.EqualTo(1672f).Within(1f));
                Rect barBounds = ScreenBounds(view.ProgressBar.rectTransform, camera);
                Rect percentBounds = ScreenBounds(view.PercentText.rectTransform, camera);
                Rect bodyBounds = ScreenBounds(view.TipBody.rectTransform, camera);
                AssertInside(barBounds, safeRect, "progress bar");
                AssertInside(percentBounds, safeRect, "percentage");
                AssertInside(bodyBounds, safeRect, "tip text");
                Assert.That(view.TipBody.preferredHeight, Is.LessThanOrEqualTo(view.TipBody.rectTransform.rect.height + 1));
                Rect environmentBounds = ScreenBounds(view.Environment.rectTransform, camera);
                Assert.That(environmentBounds.xMin, Is.LessThanOrEqualTo(1f));
                Assert.That(environmentBounds.yMin, Is.LessThanOrEqualTo(1f));
                Assert.That(environmentBounds.xMax, Is.GreaterThanOrEqualTo(size.x - 1f));
                Assert.That(environmentBounds.yMax, Is.GreaterThanOrEqualTo(size.y - 1f));
                camera.Render();
                RenderTexture.active = target;
                texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
                texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
                texture.Apply();
                Color empty = texture.GetPixel(Mathf.RoundToInt(barBounds.xMin + barBounds.width * .75f),
                    Mathf.RoundToInt(barBounds.center.y));
                Assert.That(empty.b, Is.GreaterThan(empty.r), "The visible empty track must retain its navy hue.");
                Assert.That(empty.r, Is.LessThan(.3f), "The opaque track must cover the bright snowy environment.");
                if (!errorState)
                {
                    Color filled = texture.GetPixel(Mathf.RoundToInt(barBounds.xMin + barBounds.width * .15f),
                        Mathf.RoundToInt(barBounds.center.y));
                    Assert.That(filled.b, Is.GreaterThan(filled.r), "The visible fill must retain its sapphire hue.");
                    Assert.That(filled.g, Is.GreaterThan(empty.g + .03f), "Filled and remaining work must be visibly distinct.");
                }
                string output = OutputPath(name + ".png");
                Assert.That(File.Exists(output), Is.False, "Do not overwrite earlier capture evidence.");
                File.WriteAllBytes(output, texture.EncodeToPNG());
                WriteEvidence(name + ".json", new CaptureEvidence
                {
                    unityVersion = Application.unityVersion,
                    capture = "Actual Unity Play Mode rendering of active Loading prefab on existing router Canvas",
                    fixture = errorState ? "Injected missing Home resource; actual router failure and retry presentation"
                        : "Explicit 42% visual layout fixture; this image does not measure loading duration or progress",
                    width = size.x, height = size.y, safeArea = safeRect,
                    progressBar = barBounds, percent = percentBounds, tipText = bodyBounds,
                    environment = environmentBounds,
                    graphicsDevice = SystemInfo.graphicsDeviceType.ToString()
                });
            }
            finally
            {
                RenderTexture.active = oldActive;
                canvas.renderMode = oldMode;
                canvas.worldCamera = oldCamera;
                canvas.planeDistance = oldDistance;
                canvas.scaleFactor = oldScale;
                scaler.enabled = oldScalerEnabled;
                adaptive.enabled = oldAdaptiveEnabled;
                safe.enabled = oldSafeEnabled;
                adaptive.ApplyNow();
                safe.ApplyNow();
                if (texture != null) UnityEngine.Object.Destroy(texture);
                if (cameraObject != null) UnityEngine.Object.Destroy(cameraObject);
                if (target != null) { target.Release(); UnityEngine.Object.Destroy(target); }
            }
        }

        static Rect ScreenBounds(RectTransform rect, Camera camera)
        {
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector2 min = camera.WorldToScreenPoint(corners[0]);
            Vector2 max = min;
            foreach (Vector3 corner in corners)
            {
                Vector2 point = camera.WorldToScreenPoint(corner);
                min = Vector2.Min(min, point); max = Vector2.Max(max, point);
            }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        static void AssertInside(Rect actual, Rect safe, string label)
        {
            Assert.That(actual.xMin, Is.GreaterThanOrEqualTo(safe.xMin - 1f), label);
            Assert.That(actual.yMin, Is.GreaterThanOrEqualTo(safe.yMin - 1f), label);
            Assert.That(actual.xMax, Is.LessThanOrEqualTo(safe.xMax + 1f), label);
            Assert.That(actual.yMax, Is.LessThanOrEqualTo(safe.yMax + 1f), label);
        }

        GameObject ActiveRoot(ScreenId id)
        {
            Assert.That(router.CurrentScreen, Is.EqualTo(id));
            foreach (Transform child in router.Viewport)
                if (child.name == "Screen_" + id && child.gameObject.activeInHierarchy) return child.gameObject;
            Assert.Fail("Missing active production root for " + id);
            return null;
        }

        ProductionLoadingView LoadingView()
        {
            ProductionLoadingView view = ActiveRoot(ScreenId.Loading).GetComponentInChildren<ProductionLoadingView>();
            Assert.That(view, Is.Not.Null);
            return view;
        }

        void SettleMotion()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
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
            string directory = Environment.GetEnvironmentVariable("LOR_LOADING_CAPTURE_DIR");
            Assert.That(Path.IsPathRooted(directory), Is.True);
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, filename);
        }

        static void WriteEvidence(string filename, object evidence)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_LOADING_CAPTURE_DIR"))) return;
            File.WriteAllText(OutputPath(filename), JsonUtility.ToJson(evidence, true) + "\n");
        }

        [Serializable] sealed class FlowEvidence
        {
            public string unityVersion, route, progressMeaning;
            public float[] observedProgress;
            public bool completionFrameVisible;
            public int routeChanges;
        }

        sealed class MissingResourceRequest : IHomeLoadingRequest
        {
            public bool IsDone => true;
            public float Progress => 1f;
            public UnityEngine.Object Asset => null;
        }

        [Serializable] sealed class CaptureEvidence
        {
            public string unityVersion, capture, fixture, graphicsDevice;
            public int width, height;
            public Rect safeArea, progressBar, percent, tipText, environment;
        }
    }
}
