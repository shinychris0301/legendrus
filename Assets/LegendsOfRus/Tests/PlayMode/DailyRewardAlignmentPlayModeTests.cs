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
    public sealed class DailyRewardAlignmentPlayModeTests
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
            GameState.Instance.ResetProgress(); GameState.Instance.Save.highestUnlockedLevel=10; StoryService.ResetRuntime(); DemoFeatureState.SetLanguage("ru");
            router.Show(ScreenId.Shop);Settle();router.Show(ScreenId.DailyReward);Settle();yield return null;yield return null;
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

        ProductionDailyRewardView View()=>router.Viewport.GetComponentInChildren<ProductionDailyRewardView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest]public IEnumerator LiveBalancesLocalizationAndSafeAreaCaptures()
        {
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);
            Assert.That(router.Viewport.GetComponentInChildren<AnimatedHotspot>(),Is.Null);
            Assert.That(router.Viewport.GetComponentsInChildren<ProductionDailyRewardView>().Length,Is.EqualTo(1));
            int gold=GameState.Instance.Save.gold;GameState.Instance.AddGold(23);yield return null;
            Assert.That(View().GoldText.text,Is.EqualTo((gold+23).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=JsonUtility.ToJson(GameState.Instance.Save);yield return WaitForNormalButtonColors();
            Capture("DailyReward_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("DailyReward_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            Capture("DailyReward_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");yield return null;View().RefreshContent();
            Assert.That(View().Title.text,Is.EqualTo("DAILY REWARD"));
            Assert.That(View().Title.enabled,Is.True);
            Assert.That(View().PlayerHeader.HasBindings,Is.True);
            Capture("DailyReward_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            Capture("DailyReward_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));
            AssertUnchangedSave(save);
        }
        [UnityTest]public IEnumerator ClaimPersistsOnceAndReopenKeepsClaimedState()
        {
            var s=GameState.Instance;int gold=s.Save.gold;var v=View();
            v.Bind(()=>s.ClaimDailyReward(),()=>{},id=>{});v.Bind(()=>s.ClaimDailyReward(),()=>{},id=>{});
            v.ClaimButton.onClick.Invoke();v.ClaimButton.onClick.Invoke();yield return null;
            Assert.That(s.Save.gold,Is.EqualTo(gold+500));Assert.That(s.Save.dailyRewardCycleDay,Is.EqualTo(1));Assert.That(v.ClaimButton.interactable,Is.False);Assert.That(v.States[0].text,Is.EqualTo("ПОЛУЧЕНО"));
            var loaded=SaveService.Load();Assert.That(loaded.gold,Is.EqualTo(gold+500));Assert.That(loaded.dailyRewardCycleDay,Is.EqualTo(1));Assert.That(loaded.lastDailyRewardUtc,Is.EqualTo(s.Save.lastDailyRewardUtc));
            router.Show(ScreenId.Home);Settle();router.Show(ScreenId.DailyReward);Settle();yield return null;
            Assert.That(View().ClaimButton.interactable,Is.False);yield return WaitForNormalButtonColors();Capture("DailyReward_Claimed_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
        }
        [UnityTest]public IEnumerator DaySevenIsAvailableAndNavigationRemainsSingle()
        {
            var s=GameState.Instance;s.Save.lastDailyRewardUtc=DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");s.Save.dailyRewardCycleDay=6;s.Save.dailyRewardMonthUtc=DateTime.UtcNow.ToString("yyyyMM");s.Save.dailyRewardMonthClaims=6;s.Persist();yield return null;
            Assert.That(View().States[6].text,Is.EqualTo("ДОСТУПНО"));yield return WaitForNormalButtonColors();Capture("DailyReward_Day7_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            int gold=s.Save.gold,gems=s.Save.gems,bird=s.Save.boosterFirebird;View().ClaimButton.onClick.Invoke();View().ClaimButton.onClick.Invoke();yield return null;
            Assert.That(s.Save.gold,Is.EqualTo(gold+1000));Assert.That(s.Save.gems,Is.EqualTo(gems+40));Assert.That(s.Save.boosterFirebird,Is.EqualTo(bird+1));
            Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.RewardClaim));Settle();yield return null;
            router.Viewport.GetComponentInChildren<ProductionRewardReceivedView>().Confirm.onClick.Invoke();Settle();yield return null;
            Assert.That(router.ActiveModal,Is.Null);
            int generation=router.RouteGeneration;var back=View().ConfirmButton;back.onClick.Invoke();back.onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
        }
        [UnityTest]public IEnumerator FailedSaveDoesNotGrantOrConsumeAReward()
        {
            var state=GameState.Instance;string before=JsonUtility.ToJson(state.Save);
            foreach(string key in keys)PlayerPrefs.DeleteKey(key);
            var future=JsonUtility.FromJson<PlayerSaveData>(before);future.saveVersion=999;
            string raw=JsonUtility.ToJson(future);PlayerPrefs.SetString(keys[0],raw);PlayerPrefs.Save();
            Assert.That(SaveService.LoadWithResult().Status,Is.EqualTo(PlayerSaveLoadStatus.UnsupportedFutureVersion));
            Assert.That(state.ClaimDailyReward(),Is.False);Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));Assert.That(PlayerPrefs.GetString(keys[0]),Is.EqualTo(raw));
            yield return null;
        }

        [UnityTest]public IEnumerator OverflowClaimPreservesAllBalancesEligibilityAndJournal()
        {
            var state=GameState.Instance;
            int[] days={1,2,4,5,6,7,1};
            for(int scenario=0;scenario<days.Length;scenario++)
            {
                SeedOverflowClaim(days[scenario],scenario);
                string before=JsonUtility.ToJson(state.Save);
                var journal=new string[keys.Length];var present=new bool[keys.Length];
                for(int i=0;i<keys.Length;i++){present[i]=PlayerPrefs.HasKey(keys[i]);journal[i]=PlayerPrefs.GetString(keys[i]);}
                var priorWrite=state.LastWriteResult;
                int events=0;Action changed=()=>events++;state.Changed+=changed;
                try
                {
                    DailyRewardGrant receipt=default;
                    bool claimed=true;
                    Assert.DoesNotThrow(()=>claimed=state.ClaimDailyReward(out receipt),"Overflow scenario "+scenario);
                    Assert.That(claimed,Is.False);Assert.That(receipt,Is.EqualTo(default(DailyRewardGrant)));
                    Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                    Assert.That(state.CanPersist,Is.True,"Arithmetic rejection must not lock unrelated saving.");
                    Assert.That(state.LastWriteResult,Is.EqualTo(priorWrite));Assert.That(events,Is.Zero);
                    Assert.That(DailyRewardSchedule.CanClaim(state.Save,DateTime.UtcNow),Is.True);
                    for(int i=0;i<keys.Length;i++)
                    {
                        Assert.That(PlayerPrefs.HasKey(keys[i]),Is.EqualTo(present[i]));
                        Assert.That(PlayerPrefs.GetString(keys[i]),Is.EqualTo(journal[i]));
                    }
                }
                finally{state.Changed-=changed;}
            }
            yield return null;
        }

        [UnityTest]public IEnumerator OverflowButtonShowsFailureAndRecoveredClaimOpensExactlyOneReceipt()
        {
            foreach(string lang in new[]{"ru","en"})
            {
                router.Show(ScreenId.Home);Settle();
                SeedOverflowClaim(7,5);
                DemoFeatureState.SetLanguage(lang);
                router.Show(ScreenId.DailyReward);Settle();yield return null;
                var state=GameState.Instance;var view=View();
                string before=JsonUtility.ToJson(state.Save);
                Assert.That(view.ClaimButton.interactable,Is.True);
                Assert.DoesNotThrow(()=>view.ClaimButton.onClick.Invoke());
                Assert.DoesNotThrow(()=>view.ClaimButton.onClick.Invoke());
                Assert.That(router.ActiveModal,Is.Null,"Rejected rewards must not publish a receipt.");
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(before));
                Assert.That(view.Status.text,Is.EqualTo(lang=="en"?
                    "Reward not saved. Please try again after resolving storage.":"Не удалось сохранить награду. Проверьте сохранение."));
                Assert.That(view.ClaimButton.interactable,Is.True,"Claim remains retryable.");

                // Free one inventory slot through the ordinary transaction, then retry the same day.
                Assert.That(state.TryUseBooster(BoosterType.Firebird),Is.True);
                int gold=state.Save.gold,gems=state.Save.gems;
                view.ClaimButton.onClick.Invoke();Settle();yield return null;
                Assert.That(router.ActiveModal,Is.EqualTo(ScreenId.RewardClaim));
                Assert.That(state.Save.gold,Is.EqualTo(gold+1000));Assert.That(state.Save.gems,Is.EqualTo(gems+40));
                Assert.That(state.Save.boosterFirebird,Is.EqualTo(int.MaxValue));
                Assert.That(state.Save.dailyRewardCycleDay,Is.EqualTo(7));
                string committed=JsonUtility.ToJson(state.Save);
                Assert.That(state.ClaimDailyReward(),Is.False);
                var receipt=router.Viewport.GetComponentInChildren<ProductionRewardReceivedView>();
                Assert.That(receipt,Is.Not.Null);receipt.Confirm.onClick.Invoke();Settle();yield return null;
                Assert.That(JsonUtility.ToJson(state.Save),Is.EqualTo(committed));
                Assert.That(router.ActiveModal,Is.Null);Assert.That(View().ClaimButton.interactable,Is.False);
                Assert.That(SaveService.Load().boosterFirebird,Is.EqualTo(int.MaxValue));
                Assert.That(SaveService.Load().lastDailyRewardUtc,Is.EqualTo(state.Save.lastDailyRewardUtc));
                Capture("DailyReward_OverflowRecovered_"+lang,new Vector2Int(941,1672),new Rect(0,0,941,1672));
            }
        }

        void SeedOverflowClaim(int day,int scenario)
        {
            var now=DateTime.UtcNow;
            var save=new PlayerSaveData{energy=4,lastEnergyUtcTicks=now.Ticks,
                lastDailyRewardUtc=now.AddDays(-1).ToString("yyyyMMdd"),dailyRewardCycleDay=day==1?7:day-1,
                dailyRewardMonthUtc=now.ToString("yyyyMM"),dailyRewardMonthClaims=0};
            switch(scenario)
            {
                case 0:save.gold=int.MaxValue;break;
                case 1:save.gems=int.MaxValue;break;
                case 2:save.boosterLivingLight=int.MaxValue;break;
                case 3:save.boosterSeaPearl=int.MaxValue;break;
                case 4:save.boosterMace=int.MaxValue;break;
                case 5:save.boosterFirebird=int.MaxValue;break;
                case 6:save.dailyRewardMonthClaims=int.MaxValue;break;
            }
            GameState.Instance.Save=save;GameState.Instance.Persist();
            Assert.That(GameState.Instance.CanPersist,Is.True);
            Assert.That(DailyRewardSchedule.Day(GameState.Instance.Save,now),Is.EqualTo(day));
        }
        [UnityTest]public IEnumerator EveryNextRewardDescriptionFitsInBothLanguages()
        {
            foreach(string lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);
                for(int day=1;day<=7;day++)
                {
                    GameState.Instance.Save.lastDailyRewardUtc=DateTime.UtcNow.ToString("yyyyMMdd");GameState.Instance.Save.dailyRewardCycleDay=day;
                    View().RefreshContent();Canvas.ForceUpdateCanvases();yield return null;
                    Assert.That(View().Tomorrow.preferredHeight,Is.LessThanOrEqualTo(View().Tomorrow.rectTransform.rect.height+2),lang+" day "+day);
                    Assert.That(View().ClaimButton.interactable,Is.False);
                }
            }
        }

        [UnityTest]public IEnumerator OriginalButtonsFollowEachAvailableDayAndConfirmDoesNotClaim()
        {
            foreach(string lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);
                for(int day=1;day<=7;day++)
                {
                    var save=GameState.Instance.Save;save.lastDailyRewardUtc=DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd");save.dailyRewardCycleDay=day==1?7:day-1;
                    View().RefreshContent();Canvas.ForceUpdateCanvases();yield return null;
                    var v=View();Assert.That(v.ClaimButton.transform.parent,Is.SameAs(v.Cards[day-1].transform));
                    Assert.That(v.ClaimButton.gameObject.activeSelf,Is.True);Assert.That(v.States[day-1].gameObject.activeSelf,Is.False);
                    Assert.That(v.ClaimText.enabled,Is.EqualTo(lang=="en"));Assert.That(v.ConfirmText.enabled,Is.EqualTo(lang=="en"));
                    var icon=v.RewardIcons[day-1].rectTransform;var amount=v.Amounts[day-1].rectTransform;
                    Assert.That(-icon.anchoredPosition.y+icon.rect.height,Is.LessThan(277));
                    Assert.That(-amount.anchoredPosition.y+amount.rect.height,Is.LessThan(277));
                }
            }
            string before=JsonUtility.ToJson(GameState.Instance.Save);int generation=router.RouteGeneration;
            var confirm=View().ConfirmButton;confirm.onClick.Invoke();confirm.onClick.Invoke();Settle();yield return null;
            Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.Home));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            AssertUnchangedSave(before);
        }

        [UnityTest]public IEnumerator MonthlyBadgeProgressAndSourceRewardsReflectSavedCount()
        {
            foreach(string lang in new[]{"ru","en"})
            {
                DemoFeatureState.SetLanguage(lang);
                foreach(int count in new[]{0,5,28,31})
                {
                    var save=GameState.Instance.Save;save.dailyRewardMonthUtc=DateTime.UtcNow.ToString("yyyyMM");save.dailyRewardMonthClaims=count;
                    View().RefreshContent();yield return null;var v=View();
                    Assert.That(v.MonthCount.text,Is.EqualTo(count+"\n"+(lang=="en"?"Days":"Дней")));
                    Assert.That(v.MonthFill.fillAmount,Is.EqualTo(Mathf.Clamp01(count/28f)).Within(.0001f));
                    var month=v.MonthTitle.transform.parent;
                    var rail=month.Find("Rail").GetComponent<Image>();
                    Assert.That(rail.sprite.name,Is.EqualTo("month_rail_r6"));Assert.That(v.MonthFill.sprite.name,Is.EqualTo("month_fill_r6"));
                    Assert.That(v.MonthFill.rectTransform.anchoredPosition,Is.EqualTo(rail.rectTransform.anchoredPosition));
                    Assert.That(v.MonthFill.rectTransform.sizeDelta,Is.EqualTo(rail.rectTransform.sizeDelta));
                    Assert.That(v.MonthFill.type,Is.EqualTo(Image.Type.Filled));Assert.That(v.MonthFill.fillMethod,Is.EqualTo(Image.FillMethod.Horizontal));
                    Assert.That(month.Find("CountBadge").GetSiblingIndex(),Is.LessThan(v.MonthCount.transform.GetSiblingIndex()));
                    Assert.That(v.Cards[6].transform.Find("DaySevenPlaque").GetSiblingIndex(),Is.LessThan(v.DayLabels[6].transform.GetSiblingIndex()));
                    for(int i=0;i<4;i++)Assert.That(month.Find("Milestone_"+i).GetComponent<Image>().sprite.name,Is.EqualTo("milestone"+(i+1)+"_r5"));
                    Assert.That(v.DayLabels[6].enabled,Is.EqualTo(lang=="en"));
                    Assert.That(v.Cards[6].transform.Find("DaySevenPlaque").GetComponent<Image>().sprite.name,Is.EqualTo(lang=="en"?"day7_title_blank_r5":"day7_title_ru_r5"));
                    var icon=v.RewardIcons[6].rectTransform;
                    Assert.That(icon.anchoredPosition.x+icon.rect.width/2,Is.EqualTo(215).Within(.01f));
                    Assert.That(v.MonthNote.text,Does.Contain(lang=="en"?"not available":"недоступны"));
                }
                if(lang=="ru")
                {
                    GameState.Instance.Save.dailyRewardMonthClaims=28;View().RefreshContent();yield return WaitForNormalButtonColors();
                    Capture("DailyReward_Month28_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
                }
            }
        }

        static void AssertUnchangedSave(string snapshot)
        {
            var before=JsonUtility.FromJson<PlayerSaveData>(snapshot);
            var after=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));
            // GameState refreshes this clock while energy is full. Every other saved field must match.
            if(before.energy>=before.energyMax && after.energy==before.energy)
            {
                Assert.That(after.lastEnergyUtcTicks,Is.GreaterThanOrEqualTo(before.lastEnergyUtcTicks));
                after.lastEnergyUtcTicks=before.lastEnergyUtcTicks;
            }
            Assert.That(JsonUtility.ToJson(after),Is.EqualTo(snapshot));
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
                    if(button.transition==Selectable.Transition.None)continue;
                    Color expected=(button.IsInteractable()?button.colors.normalColor:button.colors.disabledColor)*button.colors.colorMultiplier;
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
            string directory = Environment.GetEnvironmentVariable("LOR_DAILY_REWARD_CAPTURE_DIR");
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
            var cameraGo = new GameObject("ChapterCompleteCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(Root().GetComponentInChildren<ProductionDailyRewardView>(),camera,size,safeArea);
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
            var corners=new Vector3[4];rect.GetWorldCorners(corners);Vector2 a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]),b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);
        }
        static void VerifyLayoutAndInput(ProductionDailyRewardView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                Rect rect=Bounds((RectTransform)button.transform,camera);
                Assert.That(rect.xMin>=safeArea.xMin-.5f && rect.yMin>=safeArea.yMin-.5f && rect.xMax<=safeArea.xMax+.5f && rect.yMax<=safeArea.yMax+.5f,Is.True,button.name+" outside safe area");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.preferredHeight,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" text height");
                }
            for(int i=0;i<7;i++)
            {
                var state=v.States[i].rectTransform;
                Assert.That(-state.anchoredPosition.y+state.rect.height,Is.LessThan(v.Cards[i].rectTransform.rect.height-12),"Status must clear lower gold border");
            }
            Assert.That(-v.MonthNote.rectTransform.anchoredPosition.y+v.MonthNote.rectTransform.rect.height,Is.LessThan(215),"Monthly note must clear bottom crest");
            foreach(var icon in v.RewardIcons)Assert.That(Bounds(icon.rectTransform,camera).Overlaps(Bounds(v.ClaimButton.GetComponent<RectTransform>(),camera)),Is.False);
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
