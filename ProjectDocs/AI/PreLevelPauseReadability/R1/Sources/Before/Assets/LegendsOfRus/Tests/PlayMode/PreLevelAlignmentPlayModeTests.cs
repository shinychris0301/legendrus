using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class PreLevelAlignmentPlayModeTests
    {
        ScreenRouter router;
        string previousSave, previousLanguage;
        readonly string[] keys = { "LegendsOfRus_Save_v1", "LegendsOfRus_Save_v1_Backup", "LegendsOfRus_Save_v1_Staging" };
        string[] values;
        bool[] existed;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            string isolated = Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            if (string.IsNullOrEmpty(isolated)) Assert.Ignore("Requires isolated test storage.");
            Assert.That(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME"), Is.EqualTo(isolated));
            Assert.That(Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(isolated) + Path.DirectorySeparatorChar), Is.True);
            router = ScreenRouter.Instance;
            Assert.That(router, Is.Not.Null);
            yield return SceneManager.LoadSceneAsync("Bootstrap", LoadSceneMode.Single);
            Assert.That(ScreenRouter.Instance, Is.SameAs(router));
            previousSave = JsonUtility.ToJson(GameState.Instance.Save);
            previousLanguage = DemoFeatureState.LanguageCode;
            values = new string[keys.Length]; existed = new bool[keys.Length];
            for (int i = 0; i < keys.Length; i++) { existed[i] = PlayerPrefs.HasKey(keys[i]); values[i] = PlayerPrefs.GetString(keys[i]); }
            GameState.Instance.ResetProgress(); StoryService.ResetRuntime(); DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.PreLevel); Settle();
            yield return null; yield return null;
            yield return WaitForNormalButtonColors();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (router != null) { router.Show(ScreenId.SplashTitle); Settle(); }
            StoryService.ResetRuntime();
            if (previousSave != null) GameState.Instance.Save = JsonUtility.FromJson<PlayerSaveData>(previousSave);
            if (previousLanguage != null) DemoFeatureState.SetLanguage(previousLanguage);
            if (values != null)
            {
                for (int i = 0; i < keys.Length; i++) { PlayerPrefs.DeleteKey(keys[i]); if (existed[i]) PlayerPrefs.SetString(keys[i], values[i]); }
                PlayerPrefs.Save();
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllAuthoredGoalsRewardsAndInventoryReflectTheSelectedLevelWithoutWritingSave()
        {
            var state=GameState.Instance;var view=Root().GetComponentInChildren<ProductionPreLevelView>();
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.ActionsBound,Is.True);
            Assert.That(Root().GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            Assert.That(Root().transform.Find("DynamicHUD"),Is.Null);
            state.Save.highestUnlockedLevel=ChapterLevelCatalog.LoadChapter1().levels.Length;
            foreach(var level in ChapterLevelCatalog.LoadChapter1().levels)
            {
                state.SelectLevel(1,level.levelId);string before=JsonUtility.ToJson(state.Save);view.RefreshContent();
                Assert.That(view.SelectedLevel.text,Is.EqualTo("УРОВЕНЬ "+level.levelId));
                Assert.That(view.MovesValue.text,Is.EqualTo(level.moveLimit.ToString()));
                for(int i=0;i<level.goals.Length;i++)
                {
                    Assert.That(view.Goals[i].Count.text.Replace(" ",""),Is.EqualTo(level.goals[i].target.ToString()));
                    if(level.goals[i].type!=LevelGoalTypes.CollectTile)
                    {
                        Assert.That(view.Goals[i].Icon.sprite,Is.Not.Null);
                        Assert.That(view.Goals[i].Icon.sprite,Is.Not.SameAs(view.GoalMedallion));
                    }
                }
                Assert.That(view.RewardGold.text,Is.EqualTo(level.rewards.gold.ToString()));
                Assert.That(view.RewardGems.text,Is.EqualTo(level.rewards.gems.ToString()));
                int[] bonuses={level.rewards.boosterMace,level.rewards.boosterFirebird,level.rewards.boosterLivingLight,level.rewards.boosterSeaPearl};
                for(int i=0;i<4;i++)
                {
                    Assert.That(view.BonusRoots[i].gameObject.activeSelf,Is.EqualTo(bonuses[i]>0));
                    Assert.That(view.BonusCounts[i].text,Is.EqualTo("+"+bonuses[i]));
                }
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before),"Rendering must not persist or grant rewards.");
            }
            state.SelectLevel(1,8);Capture("PreLevel_Level08_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            state.SelectLevel(1,10);Capture("PreLevel_Level10_Rewards_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("en");view.RefreshContent();
            state.SelectLevel(1,8);Capture("PreLevel_Level08_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru");view.RefreshContent();state.SelectLevel(1,1);
            state.CompleteLevel(ChapterLevelCatalog.Find(1,1),15000);
            state.SelectLevel(1,1);
            Assert.That(view.RewardStatus.text,Is.EqualTo("НАГРАДА ПОЛУЧЕНА"));
            Assert.That(view.RewardGold.text,Is.EqualTo("0"));Assert.That(view.RewardGems.text,Is.EqualTo("0"));
            Capture("PreLevel_RewardClaimed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            for(int i=0;i<4;i++)
            {
                var booster=ProductionPreLevelRuntime.Boosters[i];int count=state.BoosterCount(booster);
                state.AddBooster(booster,2);Assert.That(view.BoosterCounts[i].text,Is.EqualTo((count+2).ToString()));
                state.TryUseBooster(booster);Assert.That(view.BoosterCounts[i].text,Is.EqualTo((count+1).ToString()));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator EnergyGateCanCloseAndPlayAgainWithoutDoubleSpendingOrConsumingBoosters()
        {
            var state=GameState.Instance;var view=Root().GetComponentInChildren<ProductionPreLevelView>();
            state.SpendEnergy(state.Save.energy);int stock=state.BoosterCount(BoosterType.Firebird);
            view.PlayButton.onClick.Invoke();Settle();yield return null;yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.OutOfEnergy));
            view.PlayButton.onClick.Invoke();Assert.That(state.Save.energy,Is.Zero);
            CloseVisibleModal();Settle();yield return null;yield return null;
            state.AddEnergy(2);view.PlayButton.onClick.Invoke();view.PlayButton.onClick.Invoke();Settle();
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));Assert.That(state.Save.energy,Is.EqualTo(1));
            Assert.That(state.BoosterCount(BoosterType.Firebird),Is.EqualTo(stock));
            Assert.That(state.Save.selectedLevel,Is.EqualTo(1));
        }

        [UnityTest]
        public IEnumerator BoosterGuideBackAndHudRoutesRemainNativeAndInventoryIsNotSpent()
        {
            for(int i=0;i<4;i++)
            {
                var view=Root().GetComponentInChildren<ProductionPreLevelView>();string before=JsonUtility.ToJson(GameState.Instance.Save);
                view.BoosterButtons[i].onClick.Invoke();Settle();yield return null;yield return null;
                Assert.That(router.ActiveModal,Is.Null);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.PreLevel));
                Assert.That(view.InspectedBoosterIndex,Is.EqualTo(i));Assert.That(view.BoosterDetailsRoot.gameObject.activeSelf,Is.True);
                Assert.That(JsonUtility.ToJson(GameState.Instance.Save),Is.EqualTo(before));
                view.BoosterDetailsBack.onClick.Invoke();Settle();yield return null;yield return null;
                Assert.That(view.InspectedBoosterIndex,Is.EqualTo(-1));Assert.That(view.BoosterDetailsRoot.gameObject.activeSelf,Is.False);
            }
            Root().GetComponentInChildren<ProductionPreLevelView>().BackButton.onClick.Invoke();Settle();
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.ChapterMap));
            ScreenId[] targets={ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy};
            for(int i=0;i<targets.Length;i++)
            {
                router.Show(ScreenId.PreLevel);Settle();var view=Root().GetComponentInChildren<ProductionPreLevelView>();
                Button button=i==0?view.ProfileButton:i==1?view.SettingsButton:view.PlusButtons[i-2];
                button.onClick.Invoke();Settle();Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator BoosterGuideExplainsEachToolInBothLanguagesAndDoesNotSpendInventory()
        {
            var state=GameState.Instance;var view=Root().GetComponentInChildren<ProductionPreLevelView>();
            string before=JsonUtility.ToJson(state.Save);
            string[] ru={"ряд","3×3","этого цвета","5 ходов"};
            string[] en={"row","3×3","that color","5 moves"};
            foreach(string language in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(language);view.RefreshContent();
                for(int i=0;i<4;i++)
                {
                    view.BoosterButtons[i].onClick.Invoke();yield return null;
                    Assert.That(view.BoosterDetailText.text,Does.Contain(language=="en"?en[i]:ru[i]));
                    Assert.That(view.BoosterDetailText.text,Does.Contain(state.BoosterCount(ProductionPreLevelRuntime.Boosters[i]).ToString()));
                    Assert.That(view.BoosterDetailIcon.sprite,Is.SameAs(view.BoosterButtons[i].GetComponent<Image>().sprite));
                    Assert.That(router.ActiveModal,Is.Null);
                    // The existing five-second energy clock updates its timestamp at full
                    // energy. Compare a copy, keeping every balance/inventory assertion.
                    var actual=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
                    actual.lastEnergyUtcTicks=JsonUtility.FromJson<PlayerSaveData>(before).lastEnergyUtcTicks;
                    Assert.That(JsonUtility.ToJson(actual),Is.EqualTo(before));
                    Capture("PreLevel_Booster_"+i+"_"+language,new Vector2Int(941,1672),new Rect(0,0,941,1672),camera=>VerifyBoosterGuideText(view,camera));
                    view.BoosterDetailsBack.onClick.Invoke();yield return null;
                }
            }
            // Details refresh in place on language/inventory changes; zero stock remains inspectable.
            state.Save.boosterSeaPearl=0;
            view.BoosterButtons[3].onClick.Invoke();DemoFeatureState.SetLanguage("ru");view.RefreshContent();
            Assert.That(view.BoosterDetailText.text,Does.StartWith("В игре • В запасе: 0"));
            yield return null;
            Capture("PreLevel_Pearl_Empty_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672),camera=>VerifyBoosterGuideText(view,camera));
            state.AddBooster(BoosterType.SeaPearl,2);
            yield return null;
            Assert.That(view.BoosterDetailText.text,Does.StartWith("В игре • В запасе: 2"));
            Capture("PreLevel_Pearl_576x1280_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>VerifyBoosterGuideText(view,camera));
            int stock=state.BoosterCount(BoosterType.SeaPearl),energy=state.Save.energy;
            view.PlayButton.onClick.Invoke();view.PlayButton.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Gameplay));
            Assert.That(state.Save.energy,Is.EqualTo(energy-1));Assert.That(state.BoosterCount(BoosterType.SeaPearl),Is.EqualTo(stock));
            var board=router.Viewport.GetComponentInChildren<Match3Board>();Assert.That(board,Is.Not.Null);
            Assert.That(board.Moves,Is.EqualTo(ChapterLevelCatalog.ResolveSelected(state.Save).moveLimit));
            Assert.That(board.IsBoosterArmed,Is.False,"Inspecting a booster must not arm it on the board.");
            router.Show(ScreenId.PreLevel);Settle();view=Root().GetComponentInChildren<ProductionPreLevelView>();
            Assert.That(view.InspectedBoosterIndex,Is.EqualTo(-1));Assert.That(view.BoosterDetailsRoot.gameObject.activeSelf,Is.False);
        }

        static void VerifyBoosterGuideText(ProductionPreLevelView view,Camera camera)
        {
            Rect panel=Bounds(view.BoostersArea,camera);
            foreach(Text text in new[]{view.BoostersHeading,view.BoosterDetailText,view.BoosterDetailsBackText})
            {
                Assert.That(text.cachedTextGenerator.characterCountVisible,Is.GreaterThan(0),text.name);
                Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+1),text.name+" vertical overflow");
                var mesh=text.canvasRenderer.GetMesh();Assert.That(mesh,Is.Not.Null);Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                foreach(Vector3 vertex in mesh.vertices)
                {
                    Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,text.transform.TransformPoint(vertex));
                    Assert.That(panel.Contains(point),Is.True,text.name+" outside booster panel");
                }
            }
            Rect back=Bounds(view.BoosterDetailsBack.transform as RectTransform,camera);
            foreach(Vector3 vertex in view.BoosterDetailsBackText.canvasRenderer.GetMesh().vertices)
            {
                Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,view.BoosterDetailsBackText.transform.TransformPoint(vertex));
                Assert.That(Mathf.Abs(point.x-back.center.x),Is.LessThan(back.width*.25f),"Back lettering must clear the ornamental ends.");
            }
            Assert.That(view.BoosterDetailText.cachedTextGenerator.lineCount,Is.EqualTo(3));
            Assert.That(Bounds(view.BoosterDetailText.rectTransform,camera).Overlaps(Bounds(view.BoosterDetailsBack.transform as RectTransform,camera)),Is.False);
        }

        [UnityTest]
        public IEnumerator StockBadgesRemainLegibleForEmptyAndMultiDigitInventory()
        {
            var state=GameState.Instance;var view=Root().GetComponentInChildren<ProductionPreLevelView>();
            state.Save.boosterMace=0;state.Save.boosterFirebird=7;state.Save.boosterLivingLight=10;state.Save.boosterSeaPearl=999;
            view.RefreshState();yield return null;
            string[] expected={"0","7","10","999"};
            Capture("PreLevel_StockReadability_576_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
            {
                for(int i=0;i<4;i++)
                {
                    Text count=view.BoosterCounts[i];Assert.That(count.text,Is.EqualTo(expected[i]));
                    Assert.That(count.cachedTextGenerator.lineCount,Is.EqualTo(1),"Keep each stock count on one line.");
                    Rect badge=Bounds(count.transform.parent as RectTransform,camera);
                    Assert.That(badge.width,Is.GreaterThan(34),"Badge must remain legible at the narrow phone size.");
                    var mesh=count.canvasRenderer.GetMesh();Assert.That(mesh.vertexCount,Is.GreaterThan(0));
                    foreach(Vector3 vertex in mesh.vertices)
                    {
                        Vector2 point=RectTransformUtility.WorldToScreenPoint(camera,count.transform.TransformPoint(vertex));
                        Assert.That(Vector2.Distance(point,badge.center),Is.LessThan(badge.width*.46f),"Stock text crosses the raised rim: "+expected[i]);
                    }
                }
            });
        }

        [UnityTest]
        public IEnumerator CaptureReferenceAndPhonePortraits()
        {
            Capture("PreLevel_941x1672_RU", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            Capture("PreLevel_576x1280_safearea_RU", new Vector2Int(576,1280), new Rect(0,48,576,1172));
            Capture("PreLevel_1080x2400_safearea_RU", new Vector2Int(1080,2400), new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en"); yield return null; yield return null;
            Capture("PreLevel_941x1672_EN", new Vector2Int(941,1672), new Rect(0,0,941,1672));
            DemoFeatureState.SetLanguage("ru"); yield return null; yield return null;
            Capture("PreLevel_941x1672_RU_roundtrip", new Vector2Int(941,1672), new Rect(0,0,941,1672));

        }

        void CloseVisibleModal()
        {
            var energy=router.GameCanvas.GetComponentInChildren<ProductionOutOfEnergyView>();
            Assert.That(energy,Is.Not.Null);Assert.That(energy.Close.IsInteractable(),Is.True);
            energy.Close.onClick.Invoke();
        }

        GameObject Root()
        {
            foreach (Transform t in router.Viewport)
                if (t.name == "Screen_PreLevel" && t.gameObject.activeInHierarchy) return t.gameObject;
            Assert.Fail("No active Pre-Level."); return null;
        }

        void Settle()
        {
            for (int i = 0; i < 24; i++) router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
            Canvas.ForceUpdateCanvases();
        }

        IEnumerator WaitForNormalButtonColors()
        {
            float deadline=Time.realtimeSinceStartup+2f;
            int stable=0;
            do
            {
                yield return null;
                bool ready=true;
                foreach(Button button in Root().GetComponentsInChildren<Button>())
                {
                    Color expected=button.colors.normalColor*button.colors.colorMultiplier;
                    Color actual=button.targetGraphic.canvasRenderer.GetColor();
                    if(Mathf.Max(Mathf.Abs(actual.r-expected.r),Mathf.Abs(actual.g-expected.g),
                        Mathf.Abs(actual.b-expected.b),Mathf.Abs(actual.a-expected.a))>.0001f)ready=false;
                }
                stable=ready?stable+1:0;
            } while(stable<2&&Time.realtimeSinceStartup<deadline);
            Assert.That(stable,Is.EqualTo(2),"Capture only after native disabled-to-normal button fades finish.");
        }

        [Serializable] sealed class CaptureInfo
        {
            public string capture = "Actual Unity Play Mode capture; simulated display/safe area; real router and isolated new save";
            public string unityVersion, graphicsDevice, language;
            public int width, height, buttonCount;
            public Rect safeArea;
        }

        void Capture(string name, Vector2Int size, Rect safeArea, Action<Camera> inspect = null)
        {
            string directory = Environment.GetEnvironmentVariable("LOR_PRELEVEL_CAPTURE_DIR");
            Assert.That(directory, Is.Not.Null.And.Not.Empty);
            Directory.CreateDirectory(directory);
            Canvas canvas = router.GameCanvas;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            AdaptiveCanvasScaler adaptive = canvas.GetComponent<AdaptiveCanvasScaler>();
            SafeAreaFitter safe = router.SafeArea.GetComponent<SafeAreaFitter>();
            RenderMode mode = canvas.renderMode; Camera oldCamera = canvas.worldCamera;
            float distance = canvas.planeDistance, scale = canvas.scaleFactor;
            bool se = scaler.enabled, ae = adaptive.enabled, fe = safe.enabled;
            RenderTexture active = RenderTexture.active;
            var target = new RenderTexture(size.x,size.y,24,RenderTextureFormat.ARGB32);
            var cameraGo = new GameObject("PreLevelCaptureCamera",typeof(Camera));
            var texture = new Texture2D(size.x,size.y,TextureFormat.RGBA32,false);
            try
            {
                target.Create(); scaler.enabled = false; adaptive.enabled = false; safe.enabled = false;
                Camera camera = cameraGo.GetComponent<Camera>(); camera.enabled = false;
                camera.transform.position = new Vector3(0,0,-100); camera.orthographic = true;
                camera.nearClipPlane = .1f; camera.farClipPlane = 1000;
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color32(4,13,24,255);
                camera.targetTexture = target; canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera; canvas.planeDistance = 100;
                adaptive.Apply(safeArea,size);
                canvas.scaleFactor = AdaptiveCanvasScaler.ResolveScale(new Vector2Int(Mathf.RoundToInt(safeArea.width),Mathf.RoundToInt(safeArea.height)),941,1672);
                safe.Apply(safeArea,size);
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                // Rebuild glyph snapping after the capture camera's responsive layout has settled.
                foreach (Text text in Root().GetComponentsInChildren<Text>(true)) text.SetVerticesDirty();
                Canvas.ForceUpdateCanvases();
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionPreLevelView>(),camera,size,safeArea);
                inspect?.Invoke(camera);
                camera.Render(); RenderTexture.active = target;
                texture.ReadPixels(new Rect(0,0,size.x,size.y),0,0); texture.Apply();
                string output = Path.Combine(directory,name);
                Assert.That(File.Exists(output + ".png"), Is.False, "Keep earlier evidence.");
                File.WriteAllBytes(output + ".png",texture.EncodeToPNG());
                File.WriteAllText(output + ".json",JsonUtility.ToJson(new CaptureInfo
                {
                    unityVersion = Application.unityVersion, graphicsDevice = SystemInfo.graphicsDeviceType.ToString(),
                    language = DemoFeatureState.LanguageCode, width = size.x, height = size.y, safeArea = safeArea,
                    buttonCount = Root().GetComponentsInChildren<Button>(true).Length
                },true) + "\n");
            }
            finally
            {
                RenderTexture.active = active; canvas.renderMode = mode; canvas.worldCamera = oldCamera;
                canvas.planeDistance = distance; canvas.scaleFactor = scale;
                scaler.enabled = se; adaptive.enabled = ae; safe.enabled = fe;
                adaptive.ApplyNow(); safe.ApplyNow();
                UnityEngine.Object.Destroy(texture); UnityEngine.Object.Destroy(cameraGo);
                target.Release(); UnityEngine.Object.Destroy(target);
            }
        }

        static Rect Bounds(RectTransform rect,Camera camera)
        {
            var corners=new Vector3[4];rect.GetWorldCorners(corners);
            Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);
            return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        void VerifyLayoutAndInput(ProductionPreLevelView view,Camera camera,Vector2Int size,Rect safe)
        {
            Assert.That(view.HasRequiredBindings,Is.True);
            foreach(var graphic in view.GetComponentsInChildren<ProductionWorldMapGraphic>())
            {
                Assert.That(graphic.GetComponent<CanvasRenderer>(),Is.Not.Null,graphic.name);
                var mesh = graphic.canvasRenderer.GetMesh();
                Assert.That(mesh,Is.Not.Null,graphic.name);
                Assert.That(mesh.vertexCount,Is.GreaterThan(0),graphic.name+" must have rendered geometry");
            }
            Rect environment=Bounds(view.Environment.rectTransform,camera);
            Assert.That(environment.xMin,Is.LessThanOrEqualTo(1));Assert.That(environment.yMin,Is.LessThanOrEqualTo(1));
            Assert.That(environment.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(environment.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            foreach(Button button in view.GetComponentsInChildren<Button>())
            {
                if(!button.gameObject.activeInHierarchy)continue;
                Rect rect=Bounds(button.transform as RectTransform,camera);
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safe.xMin-1),button.name);
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safe.yMin-1),button.name);
                Assert.That(rect.xMax,Is.LessThanOrEqualTo(safe.xMax+1),button.name);
                Assert.That(rect.yMax,Is.LessThanOrEqualTo(safe.yMax+1),button.name);
                var results=new List<RaycastResult>();
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},results);
                Assert.That(results.Count,Is.GreaterThan(0),button.name);
                Assert.That(results[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button),button.name+" is obscured by "+results[0].gameObject.name);
            }
            foreach(var badge in view.GetComponentsInChildren<ProductionPreLevelBadge>())
            {
                var mesh=badge.canvasRenderer.GetMesh();Assert.That(mesh,Is.Not.Null);
                Assert.That(mesh.vertexCount,Is.GreaterThan(100));
            }
            Rect rewards=Bounds(view.RewardsArea,camera);
            foreach(var bonus in view.BonusRoots)
            {
                if(!bonus.gameObject.activeSelf)continue;
                Rect rect=Bounds(bonus,camera);
                Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(rewards.yMin+rewards.height*45/280),"Bonus rewards must clear the lower gold trim.");
                Assert.That(rect.Overlaps(Bounds(view.RewardGems.rectTransform,camera)),Is.False);
            }
            var layout=view.GetComponent<ProductionPreLevelLayout>();Vector2 old=view.Body.anchoredPosition;
            layout.ApplyNow();Canvas.ForceUpdateCanvases();Assert.That(Vector2.Distance(old,view.Body.anchoredPosition),Is.LessThan(.01f));
            Assert.That(Bounds(view.BoostersArea,camera).Overlaps(Bounds(view.RewardsArea,camera)),Is.False);
            Assert.That(Bounds(view.RewardsArea,camera).Overlaps(Bounds(view.PlayButton.transform as RectTransform,camera)),Is.False);
        }
    }
}
