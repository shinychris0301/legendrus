using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.PlayMode
{
    /// <summary>Exercises the active Home prefab, real services and router; no prefab rebuilding.</summary>
    public sealed class HomeAlignmentPlayModeTests
    {
        static readonly string[] SaveKeys =
        {
            "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging"
        };
        ScreenRouter router;
        string previousLanguage, previousSave;
        bool[] hadSaveKeys;
        string[] saveValues;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated))
                Assert.Ignore("Use matching isolated XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT directories.");
            isolated = Path.GetFullPath(isolated).TrimEnd(Path.DirectorySeparatorChar);
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(
                isolated + Path.DirectorySeparatorChar, StringComparison.Ordinal), Is.True,
                "The actual bootstrap must not access the normal user save directory.");
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null, "Use the existing production RuntimeBootstrap.");
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo("Assets/Scenes/Bootstrap.unity"));
            Assert.That(ScreenRouter.Instance, Is.SameAs(router));
            Assert.That(GameState.Instance.Save, Is.Not.Null);
            previousLanguage = DemoFeatureState.LanguageCode;
            previousSave = JsonUtility.ToJson(GameState.Instance.Save);
            hadSaveKeys = new bool[SaveKeys.Length];
            saveValues = new string[SaveKeys.Length];
            for (int i = 0; i < SaveKeys.Length; i++)
            {
                hadSaveKeys[i] = PlayerPrefs.HasKey(SaveKeys[i]);
                saveValues[i] = PlayerPrefs.GetString(SaveKeys[i], string.Empty);
            }
            GameState.Instance.ResetProgress();
            StoryService.ResetRuntime();
            DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.Home);
            Settle();
            yield return WaitForNormalHomeButtonColors(HomeView());
            AssertSingleInfrastructure();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (router != null) { router.Show(ScreenId.SplashTitle); Settle(); }
            StoryService.ResetRuntime();
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (previousSave != null && GameState.Instance != null)
                GameState.Instance.Save = JsonUtility.FromJson<PlayerSaveData>(previousSave);
            if (hadSaveKeys != null)
            {
                for (int i = 0; i < SaveKeys.Length; i++)
                {
                    PlayerPrefs.DeleteKey(SaveKeys[i]);
                    if (hadSaveKeys[i]) PlayerPrefs.SetString(SaveKeys[i], saveValues[i]);
                }
                PlayerPrefs.Save();
            }
            router = null; previousLanguage = null; previousSave = null;
            hadSaveKeys = null; saveValues = null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator PreparedHome_IsSerializedAndUsesCurrentResourceContract()
        {
            string prefabKey = ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Home);
            Assert.That(prefabKey, Is.EqualTo("ProductionUI/Pages/Home"));
            GameObject prefab = Resources.Load<GameObject>(prefabKey);
            Assert.That(prefab, Is.Not.Null);
            ProductionHomeView serialized = prefab.GetComponentInChildren<ProductionHomeView>(true);
            Assert.That(serialized, Is.Not.Null, "The active prefab must contain the authored Home, not a childless bootstrap.");
            Assert.That(serialized.HasRequiredBindings, Is.True, "Serialized ownership must agree with runtime bindings/revision.");
            Assert.That(serialized.Revision, Is.EqualTo(ProductionHomeView.CurrentRevision));
            Assert.That(serialized.transform.Find(ProductionHomeRuntime.ContentName), Is.Not.Null);
            IReadOnlyList<string> keys = ProductionUiFix01Runtime.GetHomeTextureResourceKeys();
            CollectionAssert.AreEquivalent(ProductionHomeRuntime.GetTextureResourceKeys(), keys);
            Assert.That(keys.Count, Is.EqualTo(ProductionHomeSourceComposition.Names.Length), "Prepare every original sprite and localization variant.");
            Assert.That(new HashSet<string>(keys).Count, Is.EqualTo(keys.Count), "Do not count duplicate resource work.");
            var textures = new HashSet<Texture>();
            using (var preparation = new HomeLoadingPreparation())
            {
                yield return preparation.Run(null);
                Assert.That(preparation.HasFailed, Is.False, preparation.FailureReason);
                Assert.That(preparation.IsComplete, Is.True);
                Assert.That(preparation.TotalResourceCount, Is.EqualTo(keys.Count + 1));
                Assert.That(preparation.CompletedResourceCount, Is.EqualTo(keys.Count + 1));
                Assert.That(preparation.TryBuildHome(() =>
                {
                    Assert.That(HomeLoadingPreparation.LoadPreparedPrefab(prefabKey), Is.SameAs(prefab));
                    foreach (string key in keys)
                    {
                        Assert.That(key, Does.Not.Contain("DesignScreens"));
                        Assert.That(key, Does.Not.Contain("ReferenceMatch"));
                        Assert.That(key, Does.Not.Contain("Fix01/Pages/Home"));
                        Texture2D held = HomeLoadingPreparation.LoadPreparedTexture(key);
                        Assert.That(held, Is.Not.Null, key);
                        Assert.That(held, Is.SameAs(Resources.Load<Texture2D>(key)), key);
                        textures.Add(held);
                    }
                    router.Show(ScreenId.Home);
                }), Is.True);
            }
            Settle();
            ProductionHomeView view = HomeView();
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(ProductionHomeSourceComposition.Valid(view), Is.True);
            Assert.That(view.SourcePlay.sprite.name, Is.EqualTo("source_play_ru"));
            Assert.That(view.Typography, Is.SameAs(Resources.Load<SplashTitleTypography>(ProductionLoadingRuntime.TypographyResource)));
            foreach (Image image in view.GetComponentsInChildren<Image>(true))
                if (image.isActiveAndEnabled && image.sprite != null)
                    Assert.That(textures.Contains(image.sprite.texture), Is.True,
                        image.name + " uses a texture missing from Loading's actual Home resource contract.");
            Assert.That(ActiveRoot(ScreenId.Home).GetComponentsInChildren<Button>(true).Length, Is.EqualTo(12),
                "The nine established controls and three visible resource plus controls must receive real input.");
            AssertSingleInfrastructure();
        }

        [UnityTest]
        public IEnumerator LiveHud_RefreshesFromServiceChangesAndReentry()
        {
            GameState state = GameState.Instance;
            ProductionHomeView view = HomeView();
            AssertHud(view, state.Save);
            state.AddGold(73);
            state.AddGems(11);
            Assert.That(state.SpendEnergy(2), Is.True);
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            Assert.That(first, Is.Not.Null);
            state.CompleteLevel(first, first.starScoreThresholds[0]);
            yield return null;
            Assert.That(state.Save.level, Is.EqualTo(2));
            AssertHud(view, state.Save);
            state.Save.lastEnergyUtcTicks = DateTime.UtcNow.AddMinutes(-GameState.EnergyRefillMinutes - 1).Ticks;
            state.RefreshEnergyFromClock();
            yield return null;
            Assert.That(state.Save.energy, Is.EqualTo(4), "The existing energy clock must remain the source of regeneration.");
            AssertHud(view, state.Save);
            router.Show(ScreenId.Settings);
            Settle();
            state.AddGold(29);
            state.AddGems(3);
            state.AddEnergy(1);
            router.Show(ScreenId.Home);
            Settle();
            yield return WaitForNormalHomeButtonColors(HomeView());
            AssertHud(HomeView(), state.Save);
            state.AddGold(int.MaxValue - state.Save.gold);
            state.AddGems(int.MaxValue - state.Save.gems);
            yield return null;
            view = HomeView();
            AssertHud(view, state.Save);
            string balanceCapture = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_HOME_CAPTURE_DIR"))
                ? null : "Home_576x1280_maxbalances";
            WithDisplay(view, new Vector2Int(576, 1280), new Rect(0, 24, 576, 1206), balanceCapture, camera =>
            {
                AssertCompleteBalanceGlyphs(view.GoldText);
                AssertCompleteBalanceGlyphs(view.GemsText);
            });
            AssertSingleInfrastructure();
        }

        [UnityTest]
        public IEnumerator OriginalLettering_SwitchesToLiveEnglishAndBack()
        {
            var v=HomeView();Assert.That(v.SourcePlay.sprite,Is.SameAs(v.SourceRussianPlay));Assert.That(v.PlayText.enabled,Is.False);
            DemoFeatureState.SetLanguage("en");yield return null;yield return null;
            Assert.That(v.SourcePlay.sprite,Is.SameAs(v.SourceBlankPlay));Assert.That(v.PlayText.enabled,Is.True);
            for(int i=0;i<4;i++)Assert.That(v.SourceNavigation[i].sprite,Is.SameAs(v.SourceBlankCards[i]));
            DemoFeatureState.SetLanguage("ru");yield return null;yield return null;
            Assert.That(v.SourcePlay.sprite,Is.SameAs(v.SourceRussianPlay));Assert.That(v.PlayText.enabled,Is.False);
        }

        static Color32[] ReadCameraPixels(Camera camera)
        {
            RenderTexture previous = RenderTexture.active;
            Texture2D pixels = null;
            try
            {
                camera.Render(); RenderTexture.active = camera.targetTexture;
                pixels = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGBA32, false);
                pixels.ReadPixels(new Rect(0, 0, pixels.width, pixels.height), 0, 0); pixels.Apply();
                return pixels.GetPixels32();
            }
            finally
            {
                RenderTexture.active = previous;
                if (pixels != null) UnityEngine.Object.Destroy(pixels);
            }
        }

        [UnityTest]
        public IEnumerator OriginalForeground_UsesUntintedSourceAndCleanEnvironment()
        {
            var v=HomeView();Assert.That(v.Environment.sprite.name,Is.EqualTo("clean_environment"));Assert.That(v.Environment.color,Is.EqualTo(Color.white));
            Assert.That(v.Ivan.sprite.name,Is.EqualTo("source_hero"));Assert.That(v.Ivan.color,Is.EqualTo(Color.white));
            Assert.That(v.BackgroundShade.enabled,Is.False);Assert.That(v.MapFoliage.enabled,Is.False);yield return null;
        }

        static int RgbDifference(Color32 a, Color32 b) => a.r + a.g + a.b - b.r - b.g - b.b;

        static Rect Expanded(Rect rect, float padding)
        {
            float x = rect.width * padding + 1, y = rect.height * padding + 1;
            return Rect.MinMaxRect(rect.xMin - x, rect.yMin - y, rect.xMax + x, rect.yMax + y);
        }

        [UnityTest]
        public IEnumerator NativeOriginalControls_HaveIndependentRaycastGraphics()
        {
            foreach(var b in HomeView().Buttons()){Assert.That(b.targetGraphic.enabled,Is.True,b.name);Assert.That(b.targetGraphic.raycastTarget,Is.True,b.name);Assert.That(b.targetGraphic.transform.IsChildOf(b.transform),Is.True,b.name);}yield return null;
        }

        void CaptureLightingBackgroundDiagnostics(ProductionHomeView view, Camera camera)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_HOME_CAPTURE_DIR");
            if (string.IsNullOrEmpty(directory)) return;
            var disabled = new List<GameObject>();
            Color previous = camera.backgroundColor;
            Transform navigation = view.GetComponent<ProductionHomeLayout>().Navigation;
            try
            {
                // The router's full-canvas letterbox otherwise covers the camera clear color.
                GameObject letterbox = router.GameCanvas.transform.Find("LetterboxBackground").gameObject;
                if (letterbox.activeSelf)
                { disabled.Add(letterbox); letterbox.SetActive(false); }
                foreach (Transform child in view.transform.Find(ProductionHomeRuntime.ContentName))
                    if (child != view.Ivan.transform && child != navigation && child.gameObject.activeSelf)
                    { disabled.Add(child.gameObject); child.gameObject.SetActive(false); }
                foreach (Transform child in navigation)
                    if (child != view.MapFoliage.transform && child.gameObject.activeSelf)
                    { disabled.Add(child.gameObject); child.gameObject.SetActive(false); }
                string[] names = { "Black", "White", "Magenta" };
                Color[] backgrounds = { Color.black, Color.white, new Color(1, 0, .85f, 1) };
                for (int i = 0; i < names.Length; i++)
                {
                    camera.backgroundColor = backgrounds[i]; Canvas.ForceUpdateCanvases();
                    Color32[] pixels = ReadCameraPixels(camera);
                    Color32 expected = backgrounds[i], emptyCorner = pixels[941 * 1600 + 900];
                    Assert.That(Mathf.Abs(emptyCorner.r - expected.r), Is.LessThanOrEqualTo(1), names[i] + " background red");
                    Assert.That(Mathf.Abs(emptyCorner.g - expected.g), Is.LessThanOrEqualTo(1), names[i] + " background green");
                    Assert.That(Mathf.Abs(emptyCorner.b - expected.b), Is.LessThanOrEqualTo(1), names[i] + " background blue");
                    var texture = new Texture2D(941, 1672, TextureFormat.RGBA32, false);
                    try
                    {
                        texture.SetPixels32(pixels); texture.Apply();
                        string path = Path.Combine(directory, "LightingBackground_" + names[i] + ".png");
                        Assert.That(File.Exists(path), Is.False);
                        File.WriteAllBytes(path, texture.EncodeToPNG());
                    }
                    finally { UnityEngine.Object.Destroy(texture); }
                }
            }
            finally
            {
                camera.backgroundColor = previous;
                foreach (GameObject child in disabled) child.SetActive(true);
                Canvas.ForceUpdateCanvases();
            }
        }

        [UnityTest]
        public IEnumerator OriginalSubjects_AreNotRedrawnByLegacyLightingEffects()
        {
            var v=HomeView();Assert.That(v.Ivan.GetComponent<ProductionHomeSceneLighting>().enabled,Is.False);Assert.That(v.Ivan.GetComponent<SplashTitleCharacterFade>().enabled,Is.False);Assert.That(v.Firebird.gameObject.activeSelf,Is.False);yield return null;
        }

        static void AssertProtectedReferenceRegionsUnchanged(Color32[] actual, Color32[] baseline, string treatment)
        {
            // Small fully opaque regions measured in the accepted941x1672 capture;
            // transparent feathers and ornamental antialiasing intentionally reveal the plate behind them.
            string[] names = { "Ivan face", "Ivan tunic", "Firebird tail", "Play face and lettering",
                "Map face", "Event face", "Event badge", "Gift badge" };
            RectInt[] regions =
            {
                new RectInt(390, 560, 50, 65), new RectInt(375, 810, 90, 115), new RectInt(795, 940, 20, 70),
                new RectInt(280, 1080, 380, 90), new RectInt(90, 1290, 120, 90), new RectInt(520, 1290, 120, 90),
                new RectInt(636, 1240, 14, 14), new RectInt(905, 1475, 12, 12)
            };
            AssertReferenceRegionsUnchanged(actual, baseline, treatment, names, regions);
        }

        static void AssertReferenceRegionsUnchanged(Color32[] actual, Color32[] baseline, string treatment,
            string[] names, RectInt[] regions)
        {
            for (int region = 0; region < regions.Length; region++)
            {
                int worst = 0;
                RectInt r = regions[region];
                for (int y = r.yMin; y < r.yMax; y++)
                    for (int x = r.xMin; x < r.xMax; x++)
                    {
                        int i = (1671 - y) * 941 + x;
                        Color32 a = actual[i], b = baseline[i];
                        worst = Mathf.Max(worst, Mathf.Abs(a.r - b.r), Mathf.Abs(a.g - b.g), Mathf.Abs(a.b - b.b));
                    }
                Assert.That(worst, Is.LessThanOrEqualTo(1), treatment + " must preserve the accepted opaque " + names[region] + ".");
            }
        }

        [UnityTest]
        public IEnumerator QuestUnavailableAndDailyRewardBadgeReflectRealPersistedState()
        {
            ProductionHomeView view = HomeView();
            Assert.That(view.HasQuestMeasurement, Is.False);
            Assert.That(view.ProfileProgressText.text, Is.EqualTo("—"));
            Assert.That(view.QuestProgressFill.gameObject.activeSelf, Is.False);
            Assert.That(view.QuestProgressFill.fillAmount, Is.Zero);
            Assert.That(view.QuestProgressText.text, Is.EqualTo("— / —"));
            Assert.That(view.QuestRewardText.text, Is.EqualTo("—"));
            Assert.That(view.DailyRewardBadge.gameObject.activeSelf, Is.True);
            Assert.That(view.DailyRewardBadgeText.text, Is.EqualTo("1"));
            GameState state = GameState.Instance;
            Assert.That(state.ClaimDailyReward(), Is.True);
            yield return null;
            Assert.That(view.DailyRewardBadge.gameObject.activeSelf, Is.False);
            Assert.That(view.DailyRewardBadgeText.text, Is.Empty);
            Assert.That(state.ClaimDailyReward(), Is.False);
            // No event service exists yet: explicitly hide this reference decoration for the visual fixture.
            Assert.That(view.EventsBadge.transform.Find("SourceBadge").GetComponent<Image>().sprite.name,
                Is.EqualTo("source_events_badge"));
            view.EventsBadge.gameObject.SetActive(false);
            ProductionHomeSourceComposition.Refresh(view);
            Assert.That(view.EventsBadge.gameObject.activeSelf, Is.False, "Language refresh must preserve badge visibility.");
            WithDisplay(view, new Vector2Int(941,1672), new Rect(0,0,941,1672), "Home_NoBadges_RU");
            DemoFeatureState.SetLanguage("en");view.RefreshContent();
            Assert.That(view.EventsBadge.gameObject.activeSelf, Is.False);
            WithDisplay(view, new Vector2Int(941,1672), new Rect(0,0,941,1672), "Home_NoBadges_EN");
            WithDisplay(view, new Vector2Int(576,1280), new Rect(0,24,576,1206), "Home_NoBadges_TallSafe_EN");
            DemoFeatureState.SetLanguage("ru");view.RefreshContent();
            view.EventsBadge.gameObject.SetActive(true);
            state.Save.lastDailyRewardUtc = DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");
            state.Persist();
            yield return null;
            Assert.That(view.DailyRewardBadge.gameObject.activeSelf, Is.True);
            Assert.That(view.DailyRewardBadgeText.text, Is.EqualTo("1"));
            AssertHud(view, state.Save);
        }

        [UnityTest]
        public IEnumerator Language_RefreshesLiveLabelsWithoutChangingSaveOrArt()
        {
            ProductionHomeView view = HomeView();
            string originalSave = SaveWithoutClockTicks();
            Sprite originalLogo = view.Logo.sprite;
            Sprite originalIvan = view.Ivan.sprite;
            string russianQuest = view.QuestText.text;
            Assert.That(russianQuest, Is.EqualTo("Соберите синие самоцветы"));
            Assert.That(view.HeroName.text, Is.EqualTo("Иван"));
            Assert.That(view.LevelText.text, Does.Contain("Уровень"));
            DemoFeatureState.SetLanguage("en");
            yield return null;
            yield return null;
            Assert.That(view.HeroName.text, Is.EqualTo("Ivan"));
            Assert.That(view.LevelText.text, Does.Contain("Level"));
            Assert.That(view.PlayText.text, Is.EqualTo("PLAY"));
            Assert.That(view.WorldMapText.text, Is.EqualTo("WORLD MAP"));
            Assert.That(view.ShopText.text, Is.EqualTo("SHOP"));
            Assert.That(view.EventsText.text, Is.EqualTo("EVENTS"));
            Assert.That(view.CharactersText.text, Is.EqualTo("CHARACTERS"));
            Assert.That(view.QuestText.text, Is.EqualTo("Collect blue gems"));
            Assert.That(view.Logo.sprite, Is.SameAs(originalLogo));
            Assert.That(view.Ivan.sprite, Is.SameAs(originalIvan));
            Assert.That(SaveWithoutClockTicks(), Is.EqualTo(originalSave));
            DemoFeatureState.SetLanguage("ru");
            yield return null;
            yield return null;
            Assert.That(view.HeroName.text, Is.EqualTo("Иван"));
            Assert.That(view.PlayText.text, Is.EqualTo("ИГРАТЬ"));
            Assert.That(view.QuestText.text, Is.EqualTo(russianQuest));
            AssertHud(view, GameState.Instance.Save);
        }

        [UnityTest]
        public IEnumerator VisibleControls_RouteNineActionsAndThreeResourceButtonsOnceFromTallSafeArea()
        {
            var expected = new[]
            {
                ScreenId.Profile, ScreenId.Settings, ScreenId.ChapterMap, ScreenId.WorldMap,
                ScreenId.Shop, ScreenId.Events, ScreenId.Characters, ScreenId.Mail, ScreenId.DailyReward,
                ScreenId.Shop, ScreenId.Shop, ScreenId.Shop
            };
            for (int index = 0; index < expected.Length; index++)
            {
                router.Show(ScreenId.Home);
                // A real user can click only after the new Canvas has rendered. In the
                // same Show frame, newly enabled masked graphics may still have depth -1.
                yield return null;
                Settle();
                ProductionHomeView view = HomeView();
                Button button = Buttons(view)[index];
                int generation = router.RouteGeneration;
                int energy = GameState.Instance.Save.energy;
                MetaUiFeedbackService feedback = MetaUiFeedbackService.Instance;
                Assert.That(feedback, Is.Not.Null, "Home must use the existing feedback service.");
                ProductionHomeButtonFeedback adapter = button.GetComponent<ProductionHomeButtonFeedback>();
                Assert.That(adapter, Is.Not.Null, button.name);
                MetaUiControlRole role = index == 2 ? MetaUiControlRole.Primary : MetaUiControlRole.Standard;
                Assert.That(adapter.Role, Is.EqualTo(role), button.name);
                string target = button.name;
                var acceptedCues = new List<MetaUiFeedbackSignal>();
                Action<MetaUiFeedbackSignal> observe = signal =>
                {
                    if (signal.Cue == MetaUiMotionCue.ButtonActivated && signal.Target == target)
                        acceptedCues.Add(signal);
                };
                feedback.SignalPublished += observe;
                try
                {
                    WithDisplay(view, new Vector2Int(576, 1280), new Rect(0, 24, 576, 1206), null,
                        camera => PointerClick(button, camera, true), "Action " + index + " -> " + expected[index]);
                    Settle();
                    Assert.That(acceptedCues.Count, Is.EqualTo(1), "Repeated tap must emit one accepted cue for " + target);
                    Assert.That(acceptedCues[0].Role, Is.EqualTo(role), target);
                }
                finally { feedback.SignalPublished -= observe; }
                Assert.That(router.CurrentScreen, Is.EqualTo(expected[index]), button.name);
                Assert.That(router.RouteGeneration, Is.EqualTo(generation + 1), "Repeated tap must not double-route " + button.name);
                Assert.That(GameState.Instance.Save.energy, Is.EqualTo(energy), "Home navigation must not spend level energy.");
                yield return null;
            }
            AssertSingleInfrastructure();
        }

        [UnityTest]
        public IEnumerator Play_PreservesPendingStoryAndChapterCompletionGates()
        {
            GameState state = GameState.Instance;
            state.Save.energy = 0;
            state.Save.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
            state.Persist();
            Assert.That(StoryService.ShouldShowChapterIntro(), Is.True);
            HomeView().PlayButton.onClick.Invoke();
            Settle();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap),
                "Fresh players must keep the existing chapter-introduction map entry.");
            Assert.That(state.Save.energy, Is.Zero, "Opening the map must not demand/spend energy.");
            ChapterStoryCatalogData story = StoryCatalog.LoadChapter1();
            state.CompleteStoryScene(story.introSceneId);
            LevelData first = ChapterLevelCatalog.Find(1, 1);
            state.CompleteLevel(first, first.starScoreThresholds[0]);
            StoryService.ResetRuntime();
            router.Show(ScreenId.Home); Settle();
            HomeView().PlayButton.onClick.Invoke(); Settle();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Dialogue),
                "Play must honor pending post-victory dialogue rather than bypass it.");
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo(StoryCatalog.FindMilestoneAfterLevel(1).sceneId));
            StoryService.ResetRuntime();
            int finalLevel = story.completion.finalLevelId;
            for (int level = 2; level <= finalLevel; level++)
            {
                LevelData data = ChapterLevelCatalog.Find(1, level);
                state.CompleteLevel(data, data.starScoreThresholds[0]);
            }
            foreach (StoryMilestoneData milestone in story.milestones)
                state.CompleteStoryScene(milestone.sceneId);
            Assert.That(StoryService.ShouldShowChapterComplete(), Is.True);
            router.Show(ScreenId.Home); Settle();
            HomeView().PlayButton.onClick.Invoke(); Settle();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterComplete),
                "The final chapter presentation must remain in the existing Play route.");
            Assert.That(state.Save.energy, Is.Zero);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SplashLoadingHome_EndsAtAlignedBoundPageWithoutDuplicateRoutes()
        {
            router.Show(ScreenId.SplashTitle); Settle();
            SplashTitleView splash = ActiveRoot(ScreenId.SplashTitle).GetComponentInChildren<SplashTitleView>();
            Assert.That(splash, Is.Not.Null);
            int generation = router.RouteGeneration;
            splash.StartButton.onClick.Invoke();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Loading));
            float deadline = Time.realtimeSinceStartup + 30f;
            while (router.CurrentScreen == ScreenId.Loading && Time.realtimeSinceStartup < deadline)
                yield return null;
            Settle();
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.Home));
            Assert.That(router.RouteGeneration, Is.EqualTo(generation + 2));
            Assert.That(HomeView().HasRequiredBindings, Is.True);
            Assert.That(HomeView().ActionsBound, Is.True);
            AssertHud(HomeView(), GameState.Instance.Save);
            for (int i = 0; i < 6; i++) yield return null;
            Assert.That(router.RouteGeneration, Is.EqualTo(generation + 2));
            AssertSingleInfrastructure();
        }

        [UnityTest]
        public IEnumerator HomePortraits_RenderLivePageAndKeepControlsInsideSafeArea()
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_HOME_CAPTURE_DIR")))
                Assert.Ignore("Set LOR_HOME_CAPTURE_DIR to retain actual Unity screenshots and geometry.");
            Assert.That(SystemInfo.graphicsDeviceType, Is.Not.EqualTo(GraphicsDeviceType.Null),
                "A real graphics device is required; do not run capture with -nographics.");
            ProductionHomeView view = HomeView();
            Color32[] firstRussian = null;
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Home_941x1672_RU",
                camera => firstRussian = ReadCameraPixels(camera));
            WithDisplay(view, new Vector2Int(576, 1280), new Rect(0, 24, 576, 1206), "Home_576x1280_safearea_RU");
            WithDisplay(view, new Vector2Int(1080, 2340), new Rect(0, 0, 1080, 2340), "Home_1080x2340_RU");
            WithDisplay(view, new Vector2Int(1080, 2400), new Rect(45, 100, 990, 2160), "Home_1080x2400_safearea_RU");
            DemoFeatureState.SetLanguage("en");
            yield return null; yield return null;
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Home_941x1672_EN");
            DemoFeatureState.SetLanguage("ru");
            yield return null; yield return null;
            WithDisplay(view, new Vector2Int(941, 1672), new Rect(0, 0, 941, 1672), "Home_941x1672_RU_roundtrip", camera =>
            {
                Color32[] finalRussian = ReadCameraPixels(camera);
                int worst = 0;
                for (int i = 0; i < finalRussian.Length; i++)
                {
                    worst = Mathf.Max(worst, Mathf.Abs(finalRussian[i].r - firstRussian[i].r));
                    worst = Mathf.Max(worst, Mathf.Abs(finalRussian[i].g - firstRussian[i].g));
                    worst = Mathf.Max(worst, Mathf.Abs(finalRussian[i].b - firstRussian[i].b));
                }
                Assert.That(worst, Is.LessThanOrEqualTo(2),
                    "Changing display scales and language must preserve the Play bevel beyond capture rounding.");
            });
            AssertSingleInfrastructure();
        }

        void WithDisplay(ProductionHomeView view, Vector2Int size, Rect safeRect, string name, Action<Camera> action = null,
            string context = null)
        {
            Canvas canvas = router.GameCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            AdaptiveCanvasScaler adaptive = canvas.GetComponent<AdaptiveCanvasScaler>();
            SafeAreaFitter safe = router.SafeArea.GetComponent<SafeAreaFitter>();
            RenderMode oldMode = canvas.renderMode;
            Camera oldCamera = canvas.worldCamera;
            float oldDistance = canvas.planeDistance, oldScale = canvas.scaleFactor;
            bool oldScalerEnabled = scaler.enabled, oldAdaptiveEnabled = adaptive.enabled, oldSafeEnabled = safe.enabled;
            RenderTexture oldActive = RenderTexture.active;
            GameObject cameraObject = null;
            RenderTexture target = null;
            Texture2D texture = null;
            try
            {
                adaptive.enabled = false; scaler.enabled = false; safe.enabled = false;
                target = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
                target.Create();
                cameraObject = new GameObject("HomeTestCaptureCamera", typeof(Camera));
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
                // This synchronous fixture swaps Canvas scale without a display frame.
                // Dynamic Text otherwise retains glyph geometry from the previous Game View scale.
                foreach (Text label in Labels(view)) label.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                ProductionHomeLayout layout = view.GetComponent<ProductionHomeLayout>();
                Assert.That(layout, Is.Not.Null, "The actual Home prefab must own its responsive layout.");
                if (name != null) WritePreAssertionDiagnostic(camera, target, size, name);
                Rect environment = ScreenBounds(view.Environment.rectTransform,camera);
                Rect shade = ScreenBounds(view.LowerShade.rectTransform,camera);
                Button[] buttons=Buttons(view);var bounds=new Rect[buttons.Length];var names=new string[buttons.Length];
                Assert.That(view.HasRequiredBindings,Is.True);
                AssertCoversDisplay(environment,size,"Clean environment");
                for(int i=0;i<buttons.Length;i++){
                    names[i]=buttons[i].name;bounds[i]=ScreenBounds(buttons[i].transform as RectTransform,camera);
                    AssertInside(bounds[i],safeRect,names[i]);
                    Assert.That(PointerTarget(buttons[i],camera),Is.Not.Null,context+": "+names[i]+" must receive its visible-center raycast. "+PointerDiagnostics(buttons[i],camera));
                }
                foreach(var label in Labels(view))if(label.isActiveAndEnabled){
                    AssertInside(ScreenBounds(label.rectTransform,camera),safeRect,label.name);
                    if(!label.resizeTextForBestFit)Assert.That(label.preferredHeight,Is.LessThanOrEqualTo(label.rectTransform.rect.height+1),label.name+" vertical overflow");
                }
                for(int i=0;i<10;i++){layout.ApplyNow();Canvas.ForceUpdateCanvases();}
                for(int i=0;i<buttons.Length;i++)AssertRect(ScreenBounds(buttons[i].transform as RectTransform,camera),bounds[i],names[i]+" must not drift");
                camera.Render();
                if (name != null)
                {
                    RenderTexture.active = target;
                    texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
                    texture.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); texture.Apply();
                    string output = OutputPath(name + ".png");
                    Assert.That(File.Exists(output), Is.False, "Preserve prior capture evidence.");
                    File.WriteAllBytes(output, texture.EncodeToPNG());
                    File.WriteAllText(OutputPath(name + ".json"), JsonUtility.ToJson(new CaptureEvidence
                    {
                        unityVersion = Application.unityVersion,
                        capture = "Actual Unity Play Mode rendering of the active Home prefab on the existing router Canvas",
                        fixture = "Live GameState in isolated test storage; displayed values recorded below; explicit simulated display/safe area with text geometry rebuilt at capture scale, not physical-device evidence",
                        language = DemoFeatureState.LanguageCode,
                        graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                        width = size.x, height = size.y, safeArea = safeRect,
                        environment = environment, lowerShade = shade,
                        logo = ScreenBounds(view.Logo.rectTransform, camera),
                        ivan = ScreenBounds(view.Ivan.rectTransform, camera),
                        firebird = ScreenBounds(view.Firebird.rectTransform, camera),
                        footerBase = ScreenBounds(view.FooterBase.rectTransform, camera),
                        controls = names, controlBounds = bounds,
                        gold = view.GoldText.text, gems = view.GemsText.text, energy = view.EnergyText.text, level = view.LevelText.text,
                        automaticFirstCanvasPass = true, allVisibleControlCentersRaycast = true, repeatedLayoutStable = true
                    }, true) + "\n");
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
                if (view != null && view.isActiveAndEnabled)
                {
                    view.GetComponent<ProductionHomeLayout>().ApplyNow();
                    foreach (Text label in Labels(view)) label.SetVerticesDirty();
                }
                if (texture != null) UnityEngine.Object.Destroy(texture);
                if (cameraObject != null) UnityEngine.Object.Destroy(cameraObject);
                if (target != null) { target.Release(); UnityEngine.Object.Destroy(target); }
            }
        }

        void WritePreAssertionDiagnostic(Camera camera, RenderTexture target, Vector2Int size, string name)
        {
            string output = OutputPath("Diagnostic_" + name + ".png");
            Assert.That(File.Exists(output), Is.False, "Preserve earlier diagnostic capture evidence.");
            RectTransform[] firstPassRects = HomeView().GetComponentsInChildren<RectTransform>(true);
            var firstPassPositions = new Vector2[firstPassRects.Length];
            var firstPassSizes = new Vector2[firstPassRects.Length];
            for (int i = 0; i < firstPassRects.Length; i++)
            {
                firstPassPositions[i] = firstPassRects[i].anchoredPosition;
                firstPassSizes[i] = firstPassRects[i].sizeDelta;
            }
            RenderTexture previous = RenderTexture.active;
            Texture2D diagnostic = null;
            try
            {
                camera.Render();
                RenderTexture.active = target;
                diagnostic = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
                diagnostic.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0); diagnostic.Apply();
                File.WriteAllBytes(output, diagnostic.EncodeToPNG());
                File.WriteAllText(OutputPath("Diagnostic_" + name + ".json"),
                    "{\n  \"capture\": \"Actual Unity rendering before geometry assertions; diagnostic evidence, not a passing result or visual approval\",\n  \"unityVersion\": \"" + Application.unityVersion + "\"\n}\n");
            }
            finally
            {
                RenderTexture.active = previous;
                if (diagnostic != null) UnityEngine.Object.Destroy(diagnostic);
            }
            for (int i = 0; i < firstPassRects.Length; i++)
            {
                Assert.That(Vector2.Distance(firstPassRects[i].anchoredPosition, firstPassPositions[i]), Is.LessThan(.01f),
                    firstPassRects[i].name + " must already be positioned before diagnostic rendering");
                Assert.That(Vector2.Distance(firstPassRects[i].sizeDelta, firstPassSizes[i]), Is.LessThan(.01f),
                    firstPassRects[i].name + " must already be sized before diagnostic rendering");
            }
        }

        static void AssertHudComposition(ProductionHomeView view, ProductionHomeLayout layout, Camera camera)
        {
            Assert.That(layout.HasRequiredBindings, Is.True, "The HUD backing must participate in responsive layout.");
            Rect hud = ScreenBounds(layout.Hud, camera);
            Rect backing = ScreenBounds(layout.HudBacking, camera);
            float scale = hud.width / 941f;
            Rect expected = new Rect(hud.xMin + 5 * scale, hud.yMin + 8 * scale, 931 * scale, 117 * scale);
            AssertRect(backing, expected, "Opaque HUD backing must follow its visible frame on the first Canvas pass and every resize");
            AssertInside(backing, hud, "Opaque HUD backing must never escape below its frame");
            Assert.That(layout.HudBacking.parent, Is.SameAs(layout.Hud.parent));
            Assert.That(layout.HudBacking.GetSiblingIndex(), Is.LessThan(layout.Hud.GetSiblingIndex()),
                "Opaque backing must draw behind the HUD frame.");
            Rect logo = ScreenBounds(view.Logo.rectTransform, camera);
            Assert.That(logo.yMax, Is.LessThanOrEqualTo(hud.yMin + 21 * scale),
                "Only the reference crest upper ornament may overlap the lower HUD edge.");
            Assert.That(view.Logo.transform.GetSiblingIndex(), Is.GreaterThan(layout.Hud.GetSiblingIndex()));
        }

        static void AssertForegroundAndFooterComposition(ProductionHomeView view, ProductionHomeLayout layout,
            Camera camera, Rect safeRect, Vector2Int size)
        {
            AssertBackgroundToneMesh(view);
            AssertHeroLowerFadeMesh(view);
            Rect ivan = ScreenBounds(view.Ivan.rectTransform, camera);
            float scale = ivan.width / 967f;
            float extra = Mathf.Max(0, safeRect.height / scale - 1672);
            Assert.That((safeRect.yMax - ivan.yMax) / scale, Is.LessThanOrEqualTo(474 + extra * .12f + 1),
                "Tall phones must keep the complete reference hero close to the crest instead of opening a large sky gap.");
            Assert.That(view.Firebird.gameObject.activeSelf, Is.False, "Do not draw the retired R5 Firebird over the new combined reference pose.");
            Assert.That(ivan.yMin, Is.LessThanOrEqualTo(ScreenBounds(layout.Play, camera).yMax + 1),
                "The complete hero must still reach behind Start at the reviewed portrait aspect ratios.");
            Rect footer = ScreenBounds(layout.FooterBacking, camera);
            Assert.That(footer.xMin, Is.LessThanOrEqualTo(1));
            Assert.That(footer.xMax, Is.GreaterThanOrEqualTo(size.x - 1));
            Assert.That(footer.yMin, Is.EqualTo(0).Within(1),
                "The continuous footer backing must reach physical display edges under safe-area insets.");
            Rect carved = ScreenBounds(view.FooterBase.rectTransform, camera);
            Assert.That(footer.yMax, Is.EqualTo(carved.yMax - 346f / 734 * carved.height).Within(1),
                "Solid footer backing starts at the visible carved base, leaving scenery behind the medallion tops.");
            Assert.That(carved.yMax - 708f / 734 * carved.height, Is.EqualTo(0).Within(1),
                "The measured visible lower edge of the whole carved source must finish the display.");
            Assert.That(carved.width / carved.height, Is.EqualTo(2143f / 734).Within(.001f));
            Assert.That(view.FooterBase.raycastTarget, Is.False);
            Assert.That(view.FooterBase.transform.GetSiblingIndex(), Is.EqualTo(1));
            Assert.That(layout.FooterBacking.GetSiblingIndex(), Is.Zero);
            Assert.That(view.EventsBadge.raycastTarget, Is.False);
            Assert.That(view.EventsBadgeText.text, Is.EqualTo("!"));
            Assert.That(view.EventsBadgeText.canvasRenderer.GetMesh().vertexCount, Is.GreaterThan(0));
            AssertRaisedBadge(view.EventsBadge, view.EventsBadgeText);
            AssertRaisedBadge(view.DailyRewardBadge, view.DailyRewardBadgeText);
        }

        static void AssertBackgroundToneMesh(ProductionHomeView view)
        {
            ProductionHomeBackdropShade shade = view.BackgroundShade;
            Assert.That(shade, Is.Not.Null);
            Assert.That(shade.raycastTarget, Is.False);
            Assert.That(shade.transform.parent, Is.SameAs(view.Environment.transform));
            Assert.That(view.LowerShade.color.a, Is.Zero, "The R9 gradient must stay under the hero; the historical wash stays disabled.");
            Mesh mesh = shade.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null);
            Vector3[] vertices = mesh.vertices;
            Color32[] colors = mesh.colors32;
            Assert.That(vertices.Length, Is.GreaterThan(4), "The treatment needs a smooth lower/edge gradient.");
            Assert.That(colors.Length, Is.EqualTo(vertices.Length));
            Rect bounds = shade.rectTransform.rect;
            var alphaLevels = new HashSet<byte>();
            int transparentUpperVertices = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                Assert.That(float.IsNaN(vertex.x) || float.IsNaN(vertex.y) || float.IsNaN(vertex.z) ||
                    float.IsInfinity(vertex.x) || float.IsInfinity(vertex.y) || float.IsInfinity(vertex.z), Is.False);
                Assert.That(vertex.x, Is.InRange(bounds.xMin - 1, bounds.xMax + 1));
                Assert.That(vertex.y, Is.InRange(bounds.yMin - 1, bounds.yMax + 1));
                Assert.That(colors[i].a, Is.LessThanOrEqualTo(59), "Maximum23% opacity keeps the dusk plate restrained.");
                alphaLevels.Add(colors[i].a);
                float fromTop = (bounds.yMax - vertex.y) / bounds.height;
                if (fromTop <= .4601f)
                {
                    Assert.That(colors[i].a, Is.Zero, "The upper46% must have no background wash.");
                    transparentUpperVertices++;
                }
            }
            Assert.That(transparentUpperVertices, Is.GreaterThan(4));
            Assert.That(alphaLevels.Count, Is.GreaterThan(8), "The visible lower tone must not collapse to a flat overlay.");
        }

        static void AssertRaisedBadge(ProductionHomePanel badge, Text label)
        {
            Assert.That(badge.RaisedBadge, Is.True);
            Assert.That(badge.raycastTarget || label.raycastTarget, Is.False,
                "Raised notification artwork must not intercept the underlying control.");
            if (!badge.isActiveAndEnabled) return;
            Mesh mesh = badge.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null);
            var colors = new HashSet<Color32>(mesh.colors32);
            Assert.That(colors.Count, Is.GreaterThan(32), "Dome and metal need directional shading, not flat fill bands.");
            float radius = badge.rectTransform.rect.width * .5f;
            Vector2 center = badge.rectTransform.rect.center;
            bool partialShadowOutsideFace = false;
            Vector3[] vertices = mesh.vertices;
            Color32[] paintedColors = mesh.colors32;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 p = vertices[i];
                Assert.That(float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsInfinity(p.x) || float.IsInfinity(p.y), Is.False);
                float distance = Vector2.Distance(p, center);
                Assert.That(distance, Is.LessThanOrEqualTo(radius * 1.35f), "Badge shadow must stay close to its unchanged footprint.");
                Color32 c = paintedColors[i];
                if (distance > radius && c.a > 0 && c.a < 255 && c.r < 20) partialShadowOutsideFace = true;
            }
            Assert.That(partialShadowOutsideFace, Is.True, "A translucent contact shadow must separate the badge from its icon.");
            Mesh lettering = label.canvasRenderer.GetMesh();
            Assert.That(lettering, Is.Not.Null);
            Assert.That(lettering.triangles.Length, Is.EqualTo(label.text.Length * 12), "Live badge glyphs and one native emboss shadow stay complete.");
            Assert.That(label.GetComponent<ProductionHomeGoldText>().enabled, Is.True);
        }

        static void AssertHeroLowerFadeMesh(ProductionHomeView view)
        {
            SplashTitleCharacterFade fade = view.Ivan.GetComponent<SplashTitleCharacterFade>();
            Assert.That(fade != null && fade.enabled, Is.True);
            Assert.That(fade.BottomFraction, Is.EqualTo(.08f));
            Mesh mesh = view.Ivan.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null);
            Vector3[] vertices = mesh.vertices;
            Color32[] colors = mesh.colors32;
            Vector2[] uv = mesh.uv;
            var lighting = new List<Vector4>(); mesh.GetUVs(1, lighting);
            Assert.That(vertices.Length, Is.GreaterThan(4), "The lower fade needs a subdivided native image surface.");
            float low = float.PositiveInfinity, high = float.NegativeInfinity;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (lighting.Count > i && lighting[i].z > .5f) continue; // Separate padded glow quad.
                low = Mathf.Min(low, vertices[i].y); high = Mathf.Max(high, vertices[i].y);
            }
            Assert.That(high - low, Is.EqualTo(view.Ivan.rectTransform.rect.height).Within(.01f));
            bool foundTransparentEdge = false, foundPartialFade = false, foundOpaqueUpper = false;
            Color32 inputColor = view.Ivan.color;
            float minV = 1, maxV = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (lighting.Count > i && lighting[i].z > .5f) continue;
                float fraction = (vertices[i].y - low) / (high - low);
                Assert.That(colors[i].r == inputColor.r && colors[i].g == inputColor.g && colors[i].b == inputColor.b, Is.True,
                    "The shared hero fade must preserve the Image's requested RGB while changing alpha only.");
                if (fraction <= .0001f) { Assert.That(colors[i].a, Is.Zero); foundTransparentEdge = true; }
                if (fraction >= .08f) { Assert.That(colors[i].a, Is.EqualTo(inputColor.a)); foundOpaqueUpper = true; }
                if (fraction > 0 && fraction < .08f && colors[i].a > 0 && colors[i].a < inputColor.a) foundPartialFade = true;
                minV = Mathf.Min(minV, uv[i].y); maxV = Mathf.Max(maxV, uv[i].y);
            }
            Assert.That(foundTransparentEdge && foundPartialFade && foundOpaqueUpper, Is.True);
            Assert.That(minV, Is.EqualTo(0).Within(.0001f)); Assert.That(maxV, Is.EqualTo(1).Within(.0001f));
        }

        static Button[] Buttons(ProductionHomeView view) => view.Buttons();

        static Text[] Labels(ProductionHomeView view) => new[]
        {
            view.HeroName, view.LevelText, view.GoldText, view.GemsText, view.EnergyText, view.PlayText,
            view.WorldMapText, view.ShopText, view.EventsText, view.CharactersText, view.QuestText,
            view.QuestProgressText, view.QuestRewardText, view.ProfileProgressText, view.DailyRewardBadgeText, view.EventsBadgeText
        };

        static GameObject PointerTarget(Button button, Camera camera)
        {
            var pointer = new PointerEventData(EventSystem.current)
            { position = ScreenBounds(button.GetComponent<RectTransform>(), camera).center, button = PointerEventData.InputButton.Left };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            return hits.Count > 0 && hits[0].gameObject.GetComponentInParent<Button>() == button ? hits[0].gameObject : null;
        }

        static void PointerClick(Button button, Camera camera, bool repeat)
        {
            GameObject target = PointerTarget(button, camera);
            Assert.That(target, Is.Not.Null, "The visible control must receive a real pointer event: " + button.name);
            var pointer = new PointerEventData(EventSystem.current)
            { position = ScreenBounds(button.GetComponent<RectTransform>(), camera).center, button = PointerEventData.InputButton.Left };
            ExecuteEvents.ExecuteHierarchy(target, pointer, ExecuteEvents.pointerClickHandler);
            if (repeat) ExecuteEvents.ExecuteHierarchy(target, pointer, ExecuteEvents.pointerClickHandler);
        }

        static string PointerDiagnostics(Button button, Camera camera)
        {
            var pointer = new PointerEventData(EventSystem.current)
            { position = ScreenBounds(button.GetComponent<RectTransform>(), camera).center };
            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, hits);
            var details = new StringBuilder("Hits=");
            foreach (RaycastResult hit in hits)
                details.Append(hit.gameObject.name).Append("(depth=").Append(hit.depth).Append(") ");
            Graphic graphic = button.GetComponent<Graphic>();
            details.Append(" targetDepth=").Append(graphic != null ? graphic.depth : -999);
            foreach (CanvasGroup group in button.GetComponentsInParent<CanvasGroup>())
                details.Append(" group=").Append(group.name).Append(" alpha=").Append(group.alpha)
                    .Append(" interactable=").Append(group.interactable).Append(" blocks=").Append(group.blocksRaycasts);
            return details.ToString();
        }

        static void AssertHud(ProductionHomeView view, PlayerSaveData save)
        {
            Assert.That(Digits(view.GoldText.text), Is.EqualTo(save.gold.ToString()));
            Assert.That(Digits(view.GemsText.text), Is.EqualTo(save.gems.ToString()));
            Assert.That(Digits(view.LevelText.text), Is.EqualTo(save.level.ToString()));
            Assert.That(view.EnergyText.text.Replace(" ", string.Empty), Is.EqualTo(save.energy + "/" + save.energyMax));
        }

        static string Digits(string value)
        {
            var result = new StringBuilder();
            foreach (char c in value) if (char.IsDigit(c)) result.Append(c);
            return result.ToString();
        }

        static string SaveWithoutClockTicks()
        {
            // GameState legitimately updates its full-energy clock while the page is open.
            PlayerSaveData snapshot = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            snapshot.lastEnergyUtcTicks = 0;
            return JsonUtility.ToJson(snapshot);
        }

        static void AssertNavigationGlyphsInsideBlankStrip(Text label)
        {
            Mesh mesh = label.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null, label.name);
            RectTransform card = label.transform.parent as RectTransform;
            Assert.That(card, Is.Not.Null, label.name + " generated card");
            // The generated caption strip reserves this central region for live ink. Check
            // actual glyph quads, not just Text rects, so a low baseline or clipped word fails.
            // Bounds are the conservative blank-ink regions measured directly in the
            // selected R6 PNGs (QA/NavGlyphBandGeometry.json), mapped through each whole
            // artwork transform. The screenshot reference itself is never sampled here.
            Rect sourceInkRegion = label.name == "CharactersLabel"
                ? Rect.MinMaxRect(203, 940, 1050, 1025)
                : label.name == "WorldMapLabel" ? Rect.MinMaxRect(204, 940, 1050, 1025)
                : label.name == "ShopLabel" ? Rect.MinMaxRect(202, 940, 1052, 1025)
                : Rect.MinMaxRect(204, 940, 1051, 1025);
            RectTransform artwork = card.Find("ReferenceArtwork") as RectTransform;
            Assert.That(artwork, Is.Not.Null);
            Vector2 artPosition = artwork.anchoredPosition;
            Vector2 artSize = artwork.sizeDelta;
            Rect blankInkRegion = Rect.MinMaxRect(
                artPosition.x + sourceInkRegion.xMin / 1254f * artSize.x,
                artPosition.y - sourceInkRegion.yMax / 1254f * artSize.y,
                artPosition.x + sourceInkRegion.xMax / 1254f * artSize.x,
                artPosition.y - sourceInkRegion.yMin / 1254f * artSize.y);
            Vector3[] vertices = mesh.vertices;
            int visibleGlyphs = 0;
            for (int start = 0; start + 3 < vertices.Length; start += 4)
            {
                Vector2 min = vertices[start], max = min;
                for (int i = 1; i < 4; i++)
                {
                    min = Vector2.Min(min, vertices[start + i]);
                    max = Vector2.Max(max, vertices[start + i]);
                }
                if (max.x - min.x < .001f || max.y - min.y < .001f) continue;
                visibleGlyphs++;
                Vector2 cardMin = card.InverseTransformPoint(label.transform.TransformPoint(min));
                Vector2 cardMax = card.InverseTransformPoint(label.transform.TransformPoint(max));
                AssertInside(Rect.MinMaxRect(cardMin.x, cardMin.y, cardMax.x, cardMax.y), blankInkRegion,
                    label.name + " rendered caption glyph");
            }
            int expectedGlyphs = 0;
            foreach (char letter in label.text) if (!char.IsWhiteSpace(letter)) expectedGlyphs++;
            Assert.That(visibleGlyphs, Is.EqualTo(expectedGlyphs), label.name + " must display its complete live caption.");
        }

        static void AssertPlayGlyphsInsideReferenceFace(ProductionHomeView view)
        {
            Text label = view.PlayText;
            Mesh mesh = label.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null, "Live Play lettering must have actual rendered glyphs.");
            RectTransform button = view.PlayButton.transform as RectTransform;
            // The final independently generated emerald face reserves this clear source
            // area for live lettering. Its whole1959x803 Sprite is never cropped.
            RectTransform artwork = view.PlayButton.targetGraphic.rectTransform;
            Vector2 artPosition = artwork.anchoredPosition, artSize = artwork.sizeDelta;
            Rect face = Rect.MinMaxRect(artPosition.x + 300f / 1959 * artSize.x, artPosition.y - 624f / 803 * artSize.y,
                artPosition.x + 1660f / 1959 * artSize.x, artPosition.y - 240f / 803 * artSize.y);
            Vector3[] vertices = mesh.vertices;
            Vector2 totalMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 totalMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            // Unity UI Shadow converts the indexed text quads into a triangle
            // stream. A visible letter then has six vertices, not four. Inspect each
            // consecutive pair of actual triangles so glyphs and their shadow copies
            // are counted correctly for both indexed and expanded mesh topology.
            int[] triangles = mesh.triangles;
            Assert.That(triangles.Length % 6, Is.Zero, "Play glyphs must have complete two-triangle surfaces.");
            int visibleQuads = 0;
            for (int start = 0; start + 5 < triangles.Length; start += 6)
            {
                Vector2 min = vertices[triangles[start]], max = min;
                for (int i = 1; i < 6; i++) { min = Vector2.Min(min, vertices[triangles[start + i]]); max = Vector2.Max(max, vertices[triangles[start + i]]); }
                if (max.x - min.x < .001f || max.y - min.y < .001f) continue;
                Vector2 a = button.InverseTransformPoint(label.transform.TransformPoint(min));
                Vector2 b = button.InverseTransformPoint(label.transform.TransformPoint(max));
                AssertInside(Rect.MinMaxRect(a.x, a.y, b.x, b.y), face, "Play lettering/emboss must remain inside the emerald face");
                totalMin = Vector2.Min(totalMin, a); totalMax = Vector2.Max(totalMax, b); visibleQuads++;
            }
            Assert.That(label.GetComponent<ProductionHomeGoldText>(), Is.Not.Null);
            var foregroundColors = new HashSet<Color32>();
            Color32 expectedShadow = label.GetComponent<Shadow>().effectColor;
            int shadowVertices = 0;
            Color32[] paintedColors = mesh.colors32;
            var bevelUvs = new List<Vector4>();
            mesh.GetUVs(0, bevelUvs);
            Assert.That(bevelUvs.Count, Is.EqualTo(vertices.Length));
            float highestForeground = float.NegativeInfinity, lowestForeground = float.PositiveInfinity;
            for (int i = 0; i < paintedColors.Length; i++)
            {
                Color32 color = paintedColors[i];
                if (color.a == 255)
                {
                    Assert.That(new Vector2(bevelUvs[i].z, bevelUvs[i].w).sqrMagnitude, Is.GreaterThan(0),
                        "Every live Play vertex carries its own font-atlas bevel direction.");
                    foregroundColors.Add(color);
                    highestForeground = Mathf.Max(highestForeground, vertices[i].y);
                    lowestForeground = Mathf.Min(lowestForeground, vertices[i].y);
                }
                else if (color.a != 0)
                {
                    Assert.That(color, Is.EqualTo(expectedShadow), "Gold treatment must preserve the native dark emboss shadow.");
                    Assert.That(new Vector2(bevelUvs[i].z, bevelUvs[i].w), Is.EqualTo(Vector2.zero),
                        "The retained shadow must not receive bevel lighting.");
                    shadowVertices++;
                }
            }
            ProductionHomeGoldText goldEffect = label.GetComponent<ProductionHomeGoldText>();
            for (int i = 0; i < paintedColors.Length; i++)
            {
                if (paintedColors[i].a != 255) continue;
                if (Mathf.Abs(vertices[i].y - highestForeground) < .001f)
                    Assert.That(paintedColors[i], Is.EqualTo(goldEffect.TopColor), "Upper live-letter vertices use the authored gold highlight.");
                if (Mathf.Abs(vertices[i].y - lowestForeground) < .001f)
                    Assert.That(paintedColors[i], Is.EqualTo(goldEffect.BottomColor), "Lower live-letter vertices use the authored gold depth.");
            }
            Assert.That(foregroundColors.Count, Is.GreaterThan(1), "Live foreground letters need visible gold shading.");
            Assert.That(shadowVertices, Is.GreaterThan(0));
            int expectedLetters = 0;
            foreach (char letter in label.text) if (!char.IsWhiteSpace(letter)) expectedLetters++;
            Assert.That(visibleQuads, Is.EqualTo(expectedLetters * 2), "Complete live letters plus the single retained native emboss shadow.");
            Assert.That((totalMin.x + totalMax.x) * .5f, Is.EqualTo(285).Within(2), "Play ink must remain optically centered.");
            if (DemoFeatureState.LanguageCode != "en")
            {
                // Original-reference read-only measurements are315px wide and61px
                // high. This verifies the local rendered approximation, not font identity.
                Assert.That(totalMax.x - totalMin.x, Is.InRange(308f, 325f));
                Assert.That(totalMax.y - totalMin.y, Is.InRange(58f, 65f));
            }
        }

        static void AssertQuestHeadingGlyphs(ProductionHomeView view)
        {
            Text label = view.QuestText;
            Assert.That(label.fontStyle, Is.EqualTo(FontStyle.Bold));
            Mesh mesh = label.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null);
            RectTransform panel = label.transform.parent as RectTransform;
            Vector3[] vertices = mesh.vertices;
            Vector2 low = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 high = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            int visible = 0;
            for (int start = 0; start + 3 < vertices.Length; start += 4)
            {
                Vector2 min = vertices[start], max = min;
                for (int i = 1; i < 4; i++) { min = Vector2.Min(min, vertices[start + i]); max = Vector2.Max(max, vertices[start + i]); }
                if (max.x - min.x < .001f || max.y - min.y < .001f) continue;
                low = Vector2.Min(low, panel.InverseTransformPoint(label.transform.TransformPoint(min)));
                high = Vector2.Max(high, panel.InverseTransformPoint(label.transform.TransformPoint(max)));
                visible++;
            }
            int expected = 0; foreach (char c in label.text) if (!char.IsWhiteSpace(c)) expected++;
            Assert.That(visible, Is.EqualTo(expected), "Quest heading must display every live character.");
            if (DemoFeatureState.LanguageCode != "en")
            {
                Assert.That(high.x - low.x, Is.InRange(320f, 346f), "Reference quest heading ink is approximately334px wide.");
                Assert.That(high.y - low.y, Is.InRange(29f, 35f), "Reference quest heading ink is approximately32px high.");
            }
        }

        static void AssertCompleteBalanceGlyphs(Text label)
        {
            // Inspect the actual rendered digit quads: preferred sizes alone can miss truncation.
            Mesh mesh = label.canvasRenderer.GetMesh();
            Assert.That(mesh, Is.Not.Null, label.name);
            RectTransform panel = label.transform.parent as RectTransform;
            Assert.That(panel, Is.Not.Null, label.name + " resource pill");
            // The original resource icon is embedded in this independent capsule sprite.
            // Numeric space begins after its source pixel bounds, with the real plus button at the right.
            Rect numericArea = panel.rect;
            numericArea.xMin += label == label.GetComponentInParent<ProductionHomeView>().GoldText ? 58f : 56f;
            RectTransform plus = panel.Find("ResourcePlus") as RectTransform;
            Assert.That(plus, Is.Not.Null, label.name + " resource Shop button");
            var plusCorners = new Vector3[4]; plus.GetWorldCorners(plusCorners);
            numericArea.xMax = panel.InverseTransformPoint(plusCorners[0]).x - 3f;
            numericArea.yMin += 6f; numericArea.yMax -= 6f;
            Vector3[] vertices = mesh.vertices;
            int visibleGlyphs = 0;
            for (int start = 0; start + 3 < vertices.Length; start += 4)
            {
                Vector2 min = vertices[start], max = min;
                for (int i = 1; i < 4; i++)
                {
                    min = Vector2.Min(min, vertices[start + i]);
                    max = Vector2.Max(max, vertices[start + i]);
                }
                if (max.x - min.x < .001f || max.y - min.y < .001f) continue;
                visibleGlyphs++;
                // The generated font quad includes transparent padding just beyond the
                // nominal text height. Its real containment boundary is the pill's numeric
                // area, with a gap after the icon and three units before the real plus target.
                Vector2 panelMin = panel.InverseTransformPoint(label.transform.TransformPoint(min));
                Vector2 panelMax = panel.InverseTransformPoint(label.transform.TransformPoint(max));
                AssertInside(Rect.MinMaxRect(panelMin.x, panelMin.y, panelMax.x, panelMax.y), numericArea,
                    label.name + " rendered balance glyph");
            }
            Assert.That(visibleGlyphs, Is.EqualTo(Digits(label.text).Length),
                label.name + " must render every digit of the real balance without truncation.");
        }

        static Rect ScreenBounds(RectTransform rect, Camera camera)
        {
            Vector3[] corners = new Vector3[4]; rect.GetWorldCorners(corners);
            Vector2 min = camera.WorldToScreenPoint(corners[0]), max = min;
            foreach (Vector3 corner in corners)
            { Vector2 point = camera.WorldToScreenPoint(corner); min = Vector2.Min(min, point); max = Vector2.Max(max, point); }
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        static void AssertInside(Rect actual, Rect safe, string label)
        {
            Assert.That(actual.xMin, Is.GreaterThanOrEqualTo(safe.xMin - 1f), label);
            Assert.That(actual.yMin, Is.GreaterThanOrEqualTo(safe.yMin - 1f), label);
            Assert.That(actual.xMax, Is.LessThanOrEqualTo(safe.xMax + 1f), label);
            Assert.That(actual.yMax, Is.LessThanOrEqualTo(safe.yMax + 1f), label);
        }

        static void AssertCoversDisplay(Rect actual, Vector2Int size, string label)
        {
            Assert.That(actual.xMin, Is.LessThanOrEqualTo(1f), label);
            Assert.That(actual.yMin, Is.LessThanOrEqualTo(1f), label);
            Assert.That(actual.xMax, Is.GreaterThanOrEqualTo(size.x - 1f), label);
            Assert.That(actual.yMax, Is.GreaterThanOrEqualTo(size.y - 1f), label);
        }

        static void AssertRect(Rect actual, Rect expected, string label)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(.05f), label);
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(.05f), label);
            Assert.That(actual.width, Is.EqualTo(expected.width).Within(.05f), label);
            Assert.That(actual.height, Is.EqualTo(expected.height).Within(.05f), label);
        }

        GameObject ActiveRoot(ScreenId id)
        {
            Assert.That(router.CurrentScreen, Is.EqualTo(id));
            foreach (Transform child in router.Viewport)
                if (child.name == "Screen_" + id && child.gameObject.activeInHierarchy) return child.gameObject;
            Assert.Fail("Missing active production root for " + id); return null;
        }

        ProductionHomeView HomeView()
        {
            ProductionHomeView view = ActiveRoot(ScreenId.Home).GetComponentInChildren<ProductionHomeView>();
            Assert.That(view, Is.Not.Null, "The aligned Home must be on the real route.");
            return view;
        }

        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            foreach (SplashTitleLivingMotion motion in router.GetComponentsInChildren<SplashTitleLivingMotion>(true)) motion.enabled = false;
            Canvas.ForceUpdateCanvases();
        }

        static IEnumerator WaitForNormalHomeButtonColors(ProductionHomeView view)
        {
            // Completing the custom page transition enables its CanvasGroup and
            // starts UGUI's separate disabled-to-normal color coroutines. Observe
            // their real completion before captures; never force production tints.
            float deadline = Time.realtimeSinceStartup + 2f;
            int stableFrames = 0;
            var pending = new StringBuilder();
            do
            {
                yield return null;
                pending.Clear();
                foreach (Button button in view.Buttons())
                {
                    Color expected = button.colors.normalColor * button.colors.colorMultiplier;
                    Color actual = button.targetGraphic.canvasRenderer.GetColor();
                    float error = Mathf.Max(Mathf.Abs(actual.r - expected.r), Mathf.Abs(actual.g - expected.g),
                        Mathf.Abs(actual.b - expected.b), Mathf.Abs(actual.a - expected.a));
                    if (!button.IsInteractable() || error > .0001f)
                        pending.Append(button.name).Append(": actual=").Append(actual)
                            .Append(", expected=").Append(expected).Append("; ");
                }
                stableFrames = pending.Length == 0 ? stableFrames + 1 : 0;
            }
            while (stableFrames < 2 && Time.realtimeSinceStartup < deadline);
            Assert.That(stableFrames, Is.EqualTo(2),
                "Home captures require completed native button color transitions: " + pending);
        }

        void AssertSingleInfrastructure()
        {
            Assert.That(UnityEngine.Object.FindObjectsByType<ScreenRouter>().Length, Is.EqualTo(1));
            Assert.That(router.GetComponentsInChildren<Canvas>(true).Length, Is.EqualTo(1));
            Assert.That(UnityEngine.Object.FindObjectsByType<EventSystem>().Length, Is.EqualTo(1));
        }

        static string OutputPath(string name)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_HOME_CAPTURE_DIR");
            Assert.That(Path.IsPathRooted(directory), Is.True); Directory.CreateDirectory(directory);
            return Path.Combine(directory, name);
        }

        [Serializable] sealed class CaptureEvidence
        {
            public string unityVersion, capture, fixture, language, graphicsDevice, gold, gems, energy, level;
            public int width, height;
            public Rect safeArea, environment, lowerShade, logo, ivan, firebird, footerBase;
            public string[] controls;
            public Rect[] controlBounds;
            public bool automaticFirstCanvasPass, allVisibleControlCentersRaycast, repeatedLayoutStable;
        }
    }
}
