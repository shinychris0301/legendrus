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
    public sealed class TeamChatAlignmentPlayModeTests
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
            ProductionTeamChatView.ResetLocalSession();router.Show(ScreenId.TeamChat);Settle();yield return null;yield return null;
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

        ProductionTeamChatView View()=>router.Viewport.GetComponentInChildren<ProductionTeamChatView>();
        GameObject Root()=>View().gameObject;
        void Settle(){for(int i=0;i<24;i++)router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        static string StableSave(){var copy=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(GameState.Instance.Save));copy.lastEnergyUtcTicks=0;return JsonUtility.ToJson(copy);}
        [UnityTest]public IEnumerator LiveHeaderAndLocalizedPhoneLayouts(){
            Assert.That(View().HasRequiredBindings&&View().ActionsBound,Is.True);Assert.That(router.Viewport.GetComponentsInChildren<DynamicHudBinder>(),Is.Empty);int before=GameState.Instance.Save.gold;GameState.Instance.AddGold(17);yield return null;Assert.That(View().GoldText.text,Is.EqualTo((before+17).ToString("N0",System.Globalization.CultureInfo.InvariantCulture).Replace(","," ")));
            string save=StableSave();yield return WaitForNormalButtonColors();
            Capture("TeamChat_941x1672_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamChat_576x1280_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Capture("TeamChat_1080x2400_safe_RU",new Vector2Int(1080,2400),new Rect(45,100,990,2160));
            DemoFeatureState.SetLanguage("en");View().RefreshContent();yield return null;Assert.That(View().PreviewLabel.text,Does.Contain("SAMPLE"));Capture("TeamChat_1080x2400_safe_EN",new Vector2Int(1080,2400),new Rect(45,100,990,2160));Capture("TeamChat_941x1672_EN",new Vector2Int(941,1672),new Rect(0,0,941,1672));Capture("TeamChat_576x1280_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator RoutesCommitOnceAndPreserveSave(){
            var targets=new[]{ScreenId.Profile,ScreenId.Settings,ScreenId.Shop,ScreenId.Shop,ScreenId.OutOfEnergy,ScreenId.TeamHub};string save=StableSave();
            for(int i=0;i<targets.Length;i++){router.Show(ScreenId.TeamChat);Settle();yield return null;var v=View();var buttons=new[]{v.ProfileButton,v.SettingsButton,v.PlusButtons[0],v.PlusButtons[1],v.PlusButtons[2],v.BackButton};int generation=router.RouteGeneration;buttons[i].onClick.Invoke();buttons[i].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(targets[i]));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));}Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator LocalComposerSendsOncePreservesLiteralTextAndSession(){
            var v=View();string save=StableSave();int before=DemoFeatureState.TeamMessagesSent;Assert.That(v.HasLiveChat,Is.False);Assert.That(v.SendButton.interactable,Is.False);
            v.Composer.text="   ";Assert.That(v.TrySendLocal(),Is.False);Assert.That(v.LocalMessageCount,Is.Zero);
            v.Bind(id=>{},()=>{});v.Bind(id=>{},()=>{});v.Composer.text="Привет, команда! <b>Это обычный текст</b>";v.SendButton.onClick.Invoke();v.SendButton.onClick.Invoke();Assert.That(v.LocalMessageCount,Is.EqualTo(1));Assert.That(DemoFeatureState.TeamMessagesSent,Is.EqualTo(before+1));Assert.That(v.Composer.text,Is.Empty);Assert.That(v.ChatScroll.verticalNormalizedPosition,Is.EqualTo(0).Within(.01f));
            var local=v.Messages.Find("LocalMessage/Message").GetComponent<Text>();Assert.That(local.supportRichText,Is.False);Assert.That(local.text,Does.Contain("<b>"));yield return WaitForNormalButtonColors();Capture("TeamChat_LocalMessage_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));
            router.Show(ScreenId.TeamHub);Settle();yield return null;router.Show(ScreenId.TeamChat);Settle();yield return null;Assert.That(View().LocalMessageCount,Is.EqualTo(1));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator LocalHistoryLimitAndLongTextScroll(){
            var v=View();string save=StableSave();for(int i=0;i<22;i++){v.Composer.text="Local message "+i;Assert.That(v.TrySendLocal(),Is.True);}
            Assert.That(v.LocalMessageCount,Is.EqualTo(20));v.Composer.text=new string('Ж',300);Assert.That(v.Composer.text.Length,Is.LessThanOrEqualTo(240));v.TrySendLocal();Assert.That(v.LocalMessageCount,Is.EqualTo(20));
            yield return null;Canvas.ForceUpdateCanvases();Assert.That(v.Messages.rect.height,Is.GreaterThan(v.ChatViewport.rect.height));Assert.That(v.ChatScroll.verticalNormalizedPosition,Is.EqualTo(0).Within(.01f));yield return WaitForNormalButtonColors();Capture("TeamChat_LongMessage_safe_RU",new Vector2Int(576,1280),new Rect(0,48,576,1172));Assert.That(StableSave(),Is.EqualTo(save));
        }
        [UnityTest]public IEnumerator PreviewActionsBlockBackgroundAndBackClosesFirst(){
            var v=View();string save=StableSave();int generation=router.RouteGeneration;v.Composer.text="Do not send while overlay is open";
            foreach(var b in new[]{v.ChestButton,v.ChestInfoButton,v.FooterButtons[0],v.FooterButtons[1],v.FooterButtons[2],v.EmojiButton,v.Messages.Find("SampleMessage_2/Help").GetComponent<Button>()}){b.onClick.Invoke();Assert.That(v.InfoPanel.activeSelf,Is.True);Assert.That(v.TrySendLocal(),Is.False);v.ProfileButton.onClick.Invoke();Assert.That(router.RouteGeneration,Is.EqualTo(generation));v.CloseInfoButton.onClick.Invoke();}
            v.FooterButtons[1].onClick.Invoke();yield return WaitForNormalButtonColors();Capture("TeamChat_Info_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));DemoFeatureState.SetLanguage("en");v.RefreshContent();Capture("TeamChat_Info_safe_EN",new Vector2Int(576,1280),new Rect(0,48,576,1172));router.NavigateBack();Assert.That(v.InfoPanel.activeSelf,Is.False);Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamChat));Assert.That(StableSave(),Is.EqualTo(save));router.NavigateBack();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));
        }
        [UnityTest]public IEnumerator TeamHubEntryAndMaximumBalances(){router.Show(ScreenId.TeamHub);Settle();yield return null;var hub=router.Viewport.GetComponentInChildren<ProductionTeamHubView>();hub.NavButtons[0].onClick.Invoke();Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamChat));var save=GameState.Instance.Save;save.gold=int.MaxValue;save.gems=int.MaxValue;View().RefreshState();yield return WaitForNormalButtonColors();Capture("TeamChat_MaxBalances_RU",new Vector2Int(941,1672),new Rect(0,0,941,1672));}
        [UnityTest]public IEnumerator NativeFooterAndComposerKeepLocalSessionAndReturnRoute()
        {
            string save=StableSave();
            foreach(string language in new[]{"ru","en"})
            {
                router.Show(ScreenId.TeamChat);Settle();yield return null;
                DemoFeatureState.SetLanguage(language);View().RefreshContent();yield return WaitForNormalButtonColors();
                int generation=router.RouteGeneration, count=View().LocalMessageCount;
                Capture("TeamChat_FooterTargets_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>
                {
                    foreach(var button in View().FooterButtons)
                    {
                        Click(button,camera);Assert.That(View().InfoPanel.activeSelf,Is.True);
                        // Refresh graphic depths after activating the modal inside this capture callback.
                        Canvas.ForceUpdateCanvases();camera.Render();VerifyModalShield(View(),camera,new Rect(0,48,576,1172));
                        Click(View().CloseInfoButton,camera);Assert.That(View().InfoPanel.activeSelf,Is.False);
                    }
                    Rect icon=Bounds(View().EmojiButton.image.rectTransform,camera);
                    Click(View().EmojiButton,camera,new Vector2(icon.xMin-4,icon.center.y));
                });
                Assert.That(View().InfoPanel.activeSelf,Is.True);Assert.That(router.RouteGeneration,Is.EqualTo(generation));
                yield return WaitForNormalButtonColors();
                Capture("TeamChat_TallStickers_"+language,new Vector2Int(1080,2400),new Rect(45,100,990,2160));
                DemoFeatureState.SetLanguage(language=="ru"?"en":"ru");View().RefreshContent();yield return WaitForNormalButtonColors();
                Capture("TeamChat_InfoRelocalized_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().CloseInfoButton,camera));
                Assert.That(View().InfoPanel.activeSelf,Is.False);
                View().Composer.text=language=="ru"?"Проверка локального сообщения":"Local message check";
                yield return WaitForNormalButtonColors();
                Capture("TeamChat_ComposerReady_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().SendButton,camera));
                Assert.That(View().LocalMessageCount,Is.EqualTo(count+1));Assert.That(View().Composer.text,Is.Empty);Assert.That(View().SendButton.interactable,Is.False);
                yield return WaitForNormalButtonColors();
                Capture("TeamChat_LocalSent_"+language,new Vector2Int(576,1280),new Rect(0,48,576,1172),camera=>Click(View().BackButton,camera));
                Settle();yield return null;Assert.That(router.CurrentScreen,Is.EqualTo(ScreenId.TeamHub));Assert.That(router.RouteGeneration,Is.EqualTo(generation+1));
            }
            Assert.That(StableSave(),Is.EqualTo(save));
        }
        static void Click(Button button,Camera camera,Vector2? point=null)
        {
            var pointer=new PointerEventData(EventSystem.current){position=point??Bounds((RectTransform)button.transform,camera).center};
            var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(pointer,hits);
            Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));
            ExecuteEvents.Execute(button.gameObject,pointer,ExecuteEvents.pointerClickHandler);
        }
        static void VerifyModalShield(ProductionTeamChatView v,Camera camera,Rect safeArea)
        {
            var shield=(RectTransform)v.InfoPanel.transform.Find("InputShield");Rect cover=Bounds(shield,camera);
            Assert.That(cover.yMin,Is.EqualTo(safeArea.yMin).Within(.5f));Assert.That(cover.yMax,Is.EqualTo(safeArea.yMax).Within(.5f));
            var points=new List<Vector2>{Bounds((RectTransform)v.Composer.transform,camera).center,Bounds(v.SendButton.image.rectTransform,camera).center,Bounds(v.BackButton.image.rectTransform,camera).center,new Vector2(safeArea.center.x,safeArea.yMin+2),new Vector2(safeArea.center.x,safeArea.yMax-2)};
            foreach(var button in v.FooterButtons)points.Add(Bounds(button.image.rectTransform,camera).center);
            foreach(var point in points)
            {
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject,Is.SameAs(shield.gameObject),"Modal blocks composer, navigation and entire safe height");
            }
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
            Assert.That(View().Composer.textComponent.supportRichText,Is.False);
            Assert.That(View().Heading.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            string directory = Environment.GetEnvironmentVariable("LOR_TEAM_CHAT_CAPTURE_DIR");
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
            var cameraGo = new GameObject("TeamChatCaptureCamera",typeof(Camera));
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
                VerifyLayoutAndInput(View(),camera,size,safeArea);
                inspect?.Invoke(camera);
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
        static void VerifyLayoutAndInput(ProductionTeamChatView v,Camera camera,Vector2Int size,Rect safeArea)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            Rect bg=Bounds(v.Environment.rectTransform,camera);Assert.That(bg.xMin,Is.LessThanOrEqualTo(1));Assert.That(bg.yMin,Is.LessThanOrEqualTo(1));Assert.That(bg.xMax,Is.GreaterThanOrEqualTo(size.x-1));Assert.That(bg.yMax,Is.GreaterThanOrEqualTo(size.y-1));
            Assert.That(v.Environment.rectTransform.rect.width/v.Environment.rectTransform.rect.height,Is.EqualTo(v.Environment.sprite.rect.width/v.Environment.sprite.rect.height).Within(.0001f));
            Assert.That(v.Header.HasBindings,Is.True);Assert.That(v.SendLabel.enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"));
            Assert.That(v.ChestFill.fillAmount,Is.EqualTo(7450f/15000f).Within(.0001f));
            Assert.That(Bounds(v.PreviewLabel.rectTransform,camera).xMax,Is.LessThan(Bounds((RectTransform)v.ChestButton.transform,camera).xMin),"Preview notice clears chest ornament");
            foreach(int sample in new[]{1,3,5}){
                var row=v.Messages.Find("SampleMessage_"+sample);var message=row.Find("Message").GetComponent<Text>();
                string iconName=sample==1?"MessageSwords":sample==3?"GiftLightning":"MessageStrength";
                var icon=row.Find(iconName).GetComponent<Image>();Assert.That(icon.raycastTarget,Is.False);
                Assert.That(Bounds(message.rectTransform,camera).xMax,Is.LessThan(Bounds(icon.rectTransform,camera).xMin),"Message leaves room for "+iconName);
            }
            Assert.That(v.Messages.Find("SampleMessage_3/GiftAmount").GetComponent<Text>().text,Is.EqualTo("+5"));
            foreach(int sample in new[]{2,6}){
                var help=v.Messages.Find("SampleMessage_"+sample+"/Help").GetComponent<Image>();
                Assert.That(help.transform.Find("Label").GetComponent<Text>().enabled,Is.EqualTo(DemoFeatureState.LanguageCode=="en"),"Source Russian lettering or live English, never doubled");
                Assert.That(help.rectTransform.rect.width/help.rectTransform.rect.height,Is.EqualTo(help.sprite.rect.width/help.sprite.rect.height).Within(.001f));
            }
            var ornament=v.Body.Find("FooterSapphire").GetComponent<Image>();
            Assert.That(ornament.raycastTarget,Is.False);
            Assert.That(Bounds(v.LocalStatus.rectTransform,camera).Overlaps(Bounds(ornament.rectTransform,camera)),Is.False,"Local status clears source sapphire carving");
            float unit=Bounds(v.Body,camera).width/941f;
            Assert.That(Bounds(v.Hud,camera).yMax,Is.EqualTo(safeArea.yMax).Within(1));
            Assert.That(Bounds((RectTransform)v.FooterButtons[0].transform,camera).yMin,Is.EqualTo(safeArea.yMin+28*unit).Within(1));
            Assert.That(Bounds(v.Composer.GetComponent<RectTransform>(),camera).yMin,Is.GreaterThan(Bounds((RectTransform)v.FooterButtons[0].transform,camera).yMax));
            Rect composer=Bounds((RectTransform)v.Composer.transform,camera),send=Bounds(v.SendButton.image.rectTransform,camera);
            var emoji=v.EmojiButton.image;Rect emojiBounds=Bounds(emoji.rectTransform,camera);Vector4 padding=emoji.raycastPadding;
            Rect target=Rect.MinMaxRect(emojiBounds.xMin+padding.x*unit,emojiBounds.yMin+padding.y*unit,emojiBounds.xMax-padding.z*unit,emojiBounds.yMax-padding.w*unit);
            Assert.That(composer.Contains(target.min)&&composer.Contains(target.max),Is.True,"Sticker target remains inside composer");
            Assert.That(target.xMin,Is.GreaterThan(Bounds(v.Composer.textComponent.rectTransform,camera).xMax),"Sticker target clears editable text");
            Assert.That(target.xMax,Is.LessThan(send.xMin),"Sticker target clears Send");
            foreach(var rect in new[]{target,send,Bounds(v.BackButton.image.rectTransform,camera)})Assert.That(Mathf.Min(rect.width,rect.height),Is.GreaterThanOrEqualTo(44),"Composer/navigation target at tested phone size");
            for(int i=0;i<3;i++)
            {
                var art=v.FooterButtons[i].image;Rect bounds=Bounds(art.rectTransform,camera);
                Assert.That(Mathf.Min(bounds.width,bounds.height),Is.GreaterThanOrEqualTo(44));
                Assert.That(bounds.yMin,Is.EqualTo(safeArea.yMin+(i==0?28:27)*unit).Within(.5f));
                Assert.That(art.type,Is.EqualTo(Image.Type.Simple));Assert.That(art.preserveAspect,Is.True);
                Assert.That(art.sprite.rect.size,Is.EqualTo(new Vector2(art.sprite.texture.width,art.sprite.texture.height)),"Whole footer icon and caption retained");
                if(i<2)Assert.That(bounds.xMax,Is.LessThan(Bounds(v.FooterButtons[i+1].image.rectTransform,camera).xMin));
            }
            Assert.That(Bounds(ornament.rectTransform,camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            Assert.That(Bounds((RectTransform)v.Body.Find("CarvedFooter"),camera).yMin,Is.EqualTo(safeArea.yMin).Within(.5f));
            if(v.InfoPanel.activeSelf)VerifyModalShield(v,camera,safeArea);
            else foreach(var point in new[]{new Vector2(target.xMin+1,target.center.y),new Vector2(target.xMax-1,target.center.y),new Vector2(target.center.x,target.yMin+1),new Vector2(target.center.x,target.yMax-1)})
            {
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=point},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(v.EmojiButton));
            }
            Rect vp=Bounds(v.ChatViewport,camera);Assert.That(safeArea.Contains(vp.min)&&safeArea.Contains(vp.max),Is.True);
            for(int i=0;i<v.Messages.childCount-1;i++){var row=(RectTransform)v.Messages.GetChild(i);var next=(RectTransform)v.Messages.GetChild(i+1);Assert.That(Bounds(row,camera).yMin,Is.GreaterThanOrEqualTo(Bounds(next,camera).yMax-.1f));}
            foreach(var button in v.GetComponentsInChildren<Button>())
            {
                if(v.InfoPanel.activeSelf&&button!=v.CloseInfoButton)continue;
                Rect rect=Bounds((RectTransform)button.transform,camera);
                if(button.transform.IsChildOf(v.Messages)&&(!vp.Contains(rect.min)||!vp.Contains(rect.max)))continue;
                Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(safeArea.xMin-.5f),button.name+" left safe edge");Assert.That(rect.xMax,Is.LessThanOrEqualTo(safeArea.xMax+.5f),button.name+" right safe edge");Assert.That(rect.yMin,Is.GreaterThanOrEqualTo(safeArea.yMin-.5f),button.name+" bottom safe edge");Assert.That(rect.yMax,Is.LessThanOrEqualTo(safeArea.yMax+.5f),button.name+" top safe edge");
                var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current){position=rect.center},hits);
                Assert.That(hits.Count,Is.GreaterThan(0));Assert.That(hits[0].gameObject.GetComponentInParent<Button>(),Is.SameAs(button));

            }
            foreach(var text in v.GetComponentsInChildren<Text>())
                if(text.enabled && !string.IsNullOrEmpty(text.text))
                {
                    if(text.transform.IsChildOf(v.Messages)&&(!vp.Overlaps(Bounds(text.rectTransform,camera))||text.canvasRenderer.cull))continue;
                    if(text.horizontalOverflow!=HorizontalWrapMode.Wrap)Assert.That(text.preferredWidth,Is.LessThanOrEqualTo(text.rectTransform.rect.width+2),text.transform.parent.name+"/"+text.name+" text width");
                    Assert.That(text.cachedTextGenerator.vertexCount,Is.GreaterThan(0),text.name+" visible glyph mesh");
                    float height=text.preferredHeight;
                    if(text.resizeTextForBestFit)
                    {
                        float used=text.cachedTextGenerator.fontSizeUsedForBestFit/text.pixelsPerUnit;
                        Assert.That(used,Is.InRange(text.resizeTextMinSize-1f,text.resizeTextMaxSize+1f),text.name+" fitted font size");
                        var settings=text.GetGenerationSettings(text.rectTransform.rect.size);settings.resizeTextForBestFit=false;settings.fontSize=Mathf.RoundToInt(used);
                        height=new TextGenerator().GetPreferredHeight(text.text,settings)/text.pixelsPerUnit;
                    }
                    Assert.That(height,Is.LessThanOrEqualTo(text.rectTransform.rect.height+2),text.transform.parent.name+"/"+text.name+" actual fitted text height");
                }
            Assert.That(v.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
        }
    }
}
