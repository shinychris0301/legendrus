from pathlib import Path
b=Path('/tmp/lor-splash-reference-r1/Candidate')
def stage(p,s):
 f=b/p;f.parent.mkdir(parents=True,exist_ok=True);f.write_text(s)
p='Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/SplashTitleBuildWiringEditModeTests.cs';s=Path(p).read_text();a=s.index('        public void SplashAndLoadingShareExactHomeBrandingWithSeparateDuskCharacters()');z=s.index('        [TestCase("masha_dusk"',a)
s=s[:a]+'''        public void SplashUsesSelectedOriginalWithLiveStartAndPreservesLoadingBranding()
        {
            const string resources="Assets/LegendsOfRus/Resources/";
            GameObject splashRoot=LoadCanonicalPrefab();var splash=splashRoot.GetComponent<SplashTitleView>();
            var home=AssetDatabase.LoadAssetAtPath<GameObject>(resources+"ProductionUI/Pages/Home.prefab").GetComponent<ProductionHomeView>();
            var loading=AssetDatabase.LoadAssetAtPath<GameObject>(resources+"ProductionUI/Pages/Loading.prefab").GetComponent<ProductionLoadingView>();
            Assert.That(splash.HasRequiredBindings&&loading.HasRequiredBindings,Is.True);
            Assert.That(splashRoot.transform.Find("Logo").GetComponent<Image>().sprite.name,Is.EqualTo("source_logo"));
            Assert.That(splashRoot.transform.Find("Ivan").GetComponent<Image>().sprite.name,Is.EqualTo("source_ivan_firebird"));
            Assert.That(splashRoot.transform.Find("SourceOuterFrame").GetComponent<Image>().raycastTarget,Is.False);
            Assert.That(splashRoot.transform.Find("Masha").gameObject.activeSelf,Is.False);
            Assert.That(splashRoot.transform.Find("Firebird").gameObject.activeSelf,Is.False);
            Assert.That(splash.StartButton.targetGraphic.GetComponent<Image>().sprite.name,Is.EqualTo("source_start_blank"));
            Assert.That(splash.StartText.GetComponent<ProductionHomeGoldText>().BeveledFace,Is.True);
            Assert.That(splash.StartText.text,Is.EqualTo("НАЖМИТЕ, ЧТОБЫ НАЧАТЬ"));
            Assert.That(splash.ReferenceTitleFont,Is.Not.Null);Assert.That(splash.ReferenceVersionFont,Is.Not.Null);
            foreach(var button in new[]{splash.LanguageButton,splash.SettingsButton,splash.AccountButton,splash.HelpButton})Assert.That(button.gameObject.activeSelf,Is.False,"Selected source exposes only Start; auxiliary routes remain elsewhere");
            Assert.That(loading.Logo.sprite,Is.SameAs(home.Logo.sprite));
            Assert.That(loading.Environment.sprite.name,Is.EqualTo("loading_environment_dusk"));
            Assert.That(loading.Ivan.sprite.name,Is.EqualTo("ivan_loading_dusk"));
        }

''' +s[z:];stage(p,s)
p='Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/HomeAlignmentDeliveryEditModeTests.cs';s=Path(p).read_text().replace('Assert.That(home.Logo.sprite, Is.SameAs(splashLogo.sprite), "Splash now shares the accepted Home crest.");','Assert.That(splashLogo.sprite.name, Is.EqualTo("source_logo"), "Splash uses its separately selected original; Home remains unchanged.");').replace('Assert.That((home.PlayButton.targetGraphic as Image).sprite, Is.SameAs(splashStart.sprite));','Assert.That(splashStart.sprite.name, Is.EqualTo("source_start_blank"));');stage(p,s)
p='Assets/LegendsOfRus/Tests/PlayMode/SplashResponsivePlayModeTests.cs';s=Path(p).read_text().replace('SplashPortraits_PreserveArtAndAlignHeaderWithSafeTop','SplashPortraits_PreserveReferenceCompositionAndSafeControls').replace('Is.EqualTo("Start")','Is.EqualTo("TAP TO START")').replace('SplashLanguage_VisibleMovedButtonReceivesPointerAndNavigatesOnce','SplashStart_VisibleSourceButtonReceivesPointerAndNavigatesOnce')
a=s.index('        public IEnumerator SplashStart_VisibleSourceButton');z=s.index('        internal static void AssertCapturePixelsEqual',a)
s=s[:a]+'''        public IEnumerator SplashStart_VisibleSourceButtonReceivesPointerAndNavigatesOnce()
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

''' +s[z:]
s=s.replace('                    Color expected = button.colors.normalColor','                    if(!button.gameObject.activeInHierarchy)continue;\n                    Color expected = button.colors.normalColor')
a=s.index('                Assert.That(logo.rect.size.x');z=s.index('                Rect logoBounds',a)
s=s[:a]+'''                AssertAuthoredRect(view,"Logo",new Rect(66,88,806,601));
                AssertAuthoredRect(view,"Ivan",new Rect(16,644,910,1010));
                AssertAuthoredRect(view,"StartButton",new Rect(159,1356,629,198));
                AssertAuthoredRect(view,"SourceOuterFrame",new Rect(0,0,941,1672));
                Assert.That(view.transform.Find("Masha").gameObject.activeSelf,Is.False);
                Assert.That(view.transform.Find("Firebird").gameObject.activeSelf,Is.False);
''' +s[z:]
a=s.index('                    AssertTop(logoBounds, safeRect');z=s.index('                for (int i = 0; i < 12;',a)
s=s[:a]+'''                    foreach(var bounds in new[]{logoBounds,versionBounds,ScreenBounds(view.StartButton.GetComponent<RectTransform>(),camera)}) {
                        Assert.That(bounds.xMin,Is.GreaterThanOrEqualTo(safeRect.xMin-1));Assert.That(bounds.xMax,Is.LessThanOrEqualTo(safeRect.xMax+1));
                        Assert.That(bounds.yMin,Is.GreaterThanOrEqualTo(safeRect.yMin-1));Assert.That(bounds.yMax,Is.LessThanOrEqualTo(safeRect.yMax+1));
                    }
                }
''' +s[z:];s=s.replace('if (!Baseline) AssertShadeContinuity(shade);','if (!Baseline) Assert.That(shade.color.a,Is.Zero,"Reference has no extra bottom contrast band");')
a=s.index('            RectTransform logo =',s.index('        static void AssertAutomaticFirstPass'));z=s.index('\n        void ApplyLayouts',a)
s=s[:a]+'''            Assert.That(view.GetComponent<SplashTitleForegroundLayout>().ReferenceComposition,Is.True);
            AssertAuthoredRect(view,"Logo",new Rect(66,88,806,601));
            Assert.That(view.VersionText.rectTransform.anchoredPosition,Is.EqualTo(new Vector2(414,-1570)));
            Assert.That(view.StartButton.gameObject.activeSelf,Is.True);
            foreach(var hidden in new[]{view.LanguageButton,view.SettingsButton,view.AccountButton,view.HelpButton})Assert.That(hidden.gameObject.activeSelf,Is.False);
        }
''' +s[z:];stage(p,s)
print('Staged source-composition and Start interaction assertions; existing measured Loading tests will run unchanged.')
