using System;
using System.Collections;
using LegendsOfRus.CommercialCore.Monetization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public sealed class ScreenRouter : MonoBehaviour, IMetaUiTransitionObserver
    {
        enum ModalMotionState
        {
            Closed,
            Opening,
            Open,
            Closing
        }

        public static ScreenRouter Instance { get; private set; }

        // Splash/Title exposes Settings/Account/Help/Language before the player reaches Home.
        // This small transient parent override keeps Back returning to Splash without replacing
        // the deterministic navigation policy used everywhere else.
        ScreenId? splashAuxiliaryRoot;
        ScreenId? tutorialReturnPage;

        public ScreenId CurrentScreen => current;
        public ScreenId? ActiveModal => activeModalId;
        public int RouteGeneration => routeGeneration;
        public MetaUiMotionPresenter MotionPresenter => motionPresenter;
        public Canvas GameCanvas => canvas;
        public RectTransform SafeArea => safeArea;
        public RectTransform Viewport => viewport;
        public bool IsScreenTransitioning =>
            contentTransition != null &&
            (
                contentTransition.Phase == MetaUiTransitionPhase.Entering ||
                contentTransition.Phase == MetaUiTransitionPhase.Exiting
            );
        public bool IsModalTransitioning =>
            modalMotionState == ModalMotionState.Opening ||
            modalMotionState == ModalMotionState.Closing;
        public bool HasActiveModalBlocker
        {
            get
            {
                Image blocker = modalRoot != null
                    ? modalRoot.GetComponent<Image>()
                    : null;
                return blocker != null && blocker.raycastTarget &&
                    modalRoot.activeInHierarchy;
            }
        }

        const float DesignW = 941f;
        const float DesignH = 1672f;

        Canvas canvas;
        RectTransform safeArea;
        RectTransform viewport;

        GameObject contentRoot;
        System.Collections.Generic.IReadOnlyList<StoryRecapEntry> recapEntries;
        GameObject modalRoot;
        GameObject modalVisualRoot;
        ScreenId? activeModalId;
        DailyRewardGrant? pendingRewardReceipt;
        int pendingRewardDay;
        ScreenId? pendingModalId;

        MetaUiMotionPresenter motionPresenter;
        MetaUiTransition contentTransition;
        MetaUiTransition modalTransition;
        ModalMotionState modalMotionState;
        Action modalAfterClose;
        int modalContinuationGeneration;
        int routeGeneration;

        ScreenId current;
        bool settingsReturnToPause;
        ScreenId? settingsReturnPage;
        bool friendsReturnToHome;
        ScreenId? languageReturnPage;
        ScreenId? loginReturnPage;
        Match3Board activeBoard;
        string activeAnalyticsLevelId = string.Empty;
        int activeAnalyticsAttemptNumber;
        LocalizedStoryReaction activeOutcomeReaction;
        Text outcomeSpeaker, outcomeBody;
        string outcomeLanguage;
        bool initialized;
        Coroutine loadingRoutine;
        HomeLoadingPreparation homePreparation;
        ProductionLoadingView loadingView;
        bool loadingFailed;

        public void Initialize()
        {
            if (initialized) return;
            if (Instance != null && Instance != this)
            {
                enabled = false;
                return;
            }

            Instance = this;
            motionPresenter = GetComponent<MetaUiMotionPresenter>();
            if (motionPresenter == null)
                motionPresenter = gameObject.AddComponent<MetaUiMotionPresenter>();
            EnsureEventSystem();
            BuildCanvas();
            initialized = true;
        }

        void OnDestroy()
        {
            CancelLoadingPreparation();
            if (Instance == this)
                Instance = null;
        }

        void Update()
        {
            RefreshOutcomeReactionLanguage();
            if (!RuntimeInput.EscapePressedThisFrame()) return;

            if (modalMotionState == ModalMotionState.Opening ||
                modalMotionState == ModalMotionState.Closing)
                return;

            MetaUiActionGate modalGate = modalVisualRoot != null
                ? modalVisualRoot.GetComponent<MetaUiActionGate>()
                : null;
            if (modalGate != null && modalGate.IsBusy)
                return;

            if (modalRoot != null)
            {
                if (activeModalId == ScreenId.Pause)
                {
                    ResumeGameplayFromPause();
                }
                else if (activeModalId == ScreenId.Settings || activeModalId == ScreenId.TutorialOverlay)
                {
                    NavigateBack();
                }
                else
                {
                    CloseModal();
                }
                return;
            }

            if (contentTransition != null &&
                contentTransition.Phase == MetaUiTransitionPhase.Entering)
                return;

            MetaUiActionGate screenGate = contentRoot != null
                ? contentRoot.GetComponent<MetaUiActionGate>()
                : null;
            if (screenGate != null && screenGate.IsBusy)
                return;

            NavigateBack();
        }

        void NavigateBackFrom(ScreenId expectedSource)
        {
            // Visible Back controls are dispatched after a short press animation.
            // Bind the callback to the screen that created it so a stale/delayed
            // callback can never navigate a different route generation.
            if (current != expectedSource)
                return;

            // Respect a Splash-launched auxiliary flow even when a visible Back
            // control uses this source-bound helper rather than NavigateBack directly.
            if (splashAuxiliaryRoot.HasValue && expectedSource == splashAuxiliaryRoot.Value)
            {
                NavigateBack();
                return;
            }

            ScreenId destination;
            if (ScreenNavigationPolicy.TryGetBackDestination(expectedSource, out destination))
                Show(destination);
            else
                Show(ScreenId.Home);
        }

        public void NavigateBack()
        {
            if(activeModalId==ScreenId.Dialogue){if(!IsModalTransitioning)CloseModal();return;}
            if(activeModalId==ScreenId.EmptyErrorState)
            {
                if(!IsModalTransitioning)
                {
                    if(IsGameplayInventoryError(activeBoard))CloseGameplayInventoryError(activeBoard);
                    else if(IsPendingGameplayVictory(activeBoard))CloseRejectedGameplayVictory(activeBoard);
                    else CloseModal();
                }
                return;
            }
            if(activeModalId==ScreenId.TutorialOverlay){if(IsModalTransitioning)return;var guide=modalVisualRoot.GetComponentInChildren<ProductionTutorialView>();if(guide!=null&&guide.TryDismissDetails())return;RequestCloseModal(()=>activeBoard?.TryResume());return;}
            if(current==ScreenId.TutorialOverlay){var guide=contentRoot.GetComponentInChildren<ProductionTutorialView>();if(guide!=null&&guide.TryDismissDetails())return;Show(tutorialReturnPage ?? ScreenId.SplashTitle);return;}
            if (current == ScreenId.Login && contentRoot != null)
            {
                var login = contentRoot.GetComponentInChildren<ProductionLoginView>();
                if (login != null && login.TryDismiss()) return;
                Show(loginReturnPage ?? ScreenId.SplashTitle);
                return;
            }
            if (current == ScreenId.LanguageSelect)
            {
                Show(languageReturnPage ?? ScreenId.SplashTitle);
                return;
            }
            if (activeModalId == ScreenId.Settings)
            {
                if (IsModalTransitioning) return;
                var settings=modalVisualRoot.GetComponentInChildren<ProductionSettingsView>();
                if (settings!=null && settings.TryDismiss()) return;
                if (settingsReturnToPause) { settingsReturnToPause=false; ShowModal(ScreenId.Pause); }
                else CloseModal();
                return;
            }
            if (current==ScreenId.Settings && contentRoot!=null)
            {
                var settings=contentRoot.GetComponentInChildren<ProductionSettingsView>();
                if (settings!=null && settings.TryDismiss()) return;
                Show(settingsReturnPage ?? ScreenId.Home);
                return;
            }
            if(current==ScreenId.Characters&&contentRoot!=null){var characters=contentRoot.GetComponentInChildren<ProductionCharactersView>();if(characters!=null&&characters.TryDismiss())return;}
            if(current==ScreenId.Inventory&&contentRoot!=null){var inventory=contentRoot.GetComponentInChildren<ProductionInventoryView>();if(inventory!=null&&inventory.TryDismiss())return;}
            if(current==ScreenId.Profile&&contentRoot!=null)
            {
                var profile=contentRoot.GetComponentInChildren<ProductionProfileView>();
                if(profile!=null&&profile.TryDismiss())return;
            }
            if(current==ScreenId.Mail&&contentRoot!=null)
            {
                var mail=contentRoot.GetComponentInChildren<ProductionMailView>();
                if(mail!=null&&mail.TryDismiss())return;
            }
            if(current==ScreenId.FriendsList&&contentRoot!=null)
            {
                var friends=contentRoot.GetComponentInChildren<ProductionFriendsListView>();
                if(friends!=null&&friends.InfoPanel.activeSelf){friends.CloseInfo();return;}
                if(friendsReturnToHome){Show(ScreenId.Home);return;}
            }
            if(current==ScreenId.TeamChat&&contentRoot!=null)
            {
                var chat=contentRoot.GetComponentInChildren<ProductionTeamChatView>();
                if(chat!=null&&chat.InfoPanel.activeSelf){chat.CloseInfo();return;}
            }
            if(current==ScreenId.TeamRanking&&contentRoot!=null)
            {
                var ranking=contentRoot.GetComponentInChildren<ProductionTeamRankingView>();
                if(ranking!=null&&ranking.InfoPanel.activeSelf){ranking.CloseInfo();return;}
            }
            if(current==ScreenId.TeamHub&&contentRoot!=null)
            {
                var team=contentRoot.GetComponentInChildren<ProductionTeamHubView>();
                if(team!=null&&team.InfoPanel.activeSelf){team.CloseInfo();return;}
            }
            if(current==ScreenId.IndividualRanking&&contentRoot!=null)
            {
                var ranking=contentRoot.GetComponentInChildren<ProductionIndividualRankingView>();
                if(ranking!=null&&ranking.RulesPanel.activeSelf){ranking.CloseRules();return;}
            }
            if (current == ScreenId.WorldMap && contentRoot != null)
            {
                ProductionWorldMapView map = contentRoot.GetComponentInChildren<ProductionWorldMapView>();
                if (map != null && map.TaskPanel.activeSelf) { map.TaskPanel.SetActive(false); return; }
            }
            if (splashAuxiliaryRoot.HasValue && current == splashAuxiliaryRoot.Value)
            {
                splashAuxiliaryRoot = null;
                Show(ScreenId.SplashTitle);
                return;
            }

            if (current == ScreenId.Gameplay)
            {
                ShowPause();
                return;
            }

            if (current == ScreenId.Dialogue)
                return;

            if (current == ScreenId.ChapterComplete)
            {
                ContinueAfterChapterComplete(ScreenId.WorldMap);
                return;
            }

            if (current == ScreenId.Home)
            {
                Application.Quit();
                return;
            }

            ScreenId destination;
            if (ScreenNavigationPolicy.TryGetBackDestination(current, out destination))
                Show(destination);
            else
                Show(ScreenId.Home);
        }

        void OpenSplashAuxiliary(ScreenId destination)
        {
            if (current != ScreenId.SplashTitle)
                return;

            splashAuxiliaryRoot = destination;
            Show(destination);
        }

        Action GuardSplashAction(Action action)
        {
            // Press feedback is asynchronous. A callback belongs to the specific
            // Splash instance that created it, not to a later visit to the same page.
            // Show increments routeGeneration, so the first accepted navigation
            // also invalidates any other pending Splash navigation callback.
            int expectedGeneration = routeGeneration;
            return () =>
            {
                if (this == null || current != ScreenId.SplashTitle ||
                    routeGeneration != expectedGeneration ||
                    contentRoot == null || !contentRoot.activeInHierarchy ||
                    modalRoot != null)
                    return;

                action?.Invoke();
            };
        }

        void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                var legacy = EventSystem.current.GetComponent<StandaloneInputModule>();
                if (legacy != null)
                    Destroy(legacy);

                if (EventSystem.current.GetComponent<InputSystemUIInputModule>() == null)
                    EventSystem.current.gameObject.AddComponent<InputSystemUIInputModule>();

                return;
            }

            var eventSystemGo = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(InputSystemUIInputModule)
            );
            eventSystemGo.transform.SetParent(transform, false);
        }

        void BuildCanvas()
        {
            var canvasGo = new GameObject(
                "GameCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            canvasGo.transform.SetParent(transform, false);

            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(941, 1672);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            // AdaptiveCanvasScaler selects the correct limiting dimension at runtime.
            scaler.matchWidthOrHeight = 0f;
            var adaptiveScaler = canvasGo.AddComponent<AdaptiveCanvasScaler>();
            adaptiveScaler.DesignWidth = DesignW;
            adaptiveScaler.DesignHeight = DesignH;
            adaptiveScaler.ApplyNow();

            var letterbox = new GameObject("LetterboxBackground", typeof(RectTransform), typeof(Image));
            letterbox.transform.SetParent(canvasGo.transform, false);
            Stretch(letterbox.GetComponent<RectTransform>());
            letterbox.GetComponent<Image>().color = new Color32(4, 13, 24, 255);

            var safe = new GameObject("SafeArea", typeof(RectTransform));
            safe.transform.SetParent(canvasGo.transform, false);
            safeArea = safe.GetComponent<RectTransform>();
            Stretch(safeArea);
            SafeAreaFitter safeAreaFitter = safe.AddComponent<SafeAreaFitter>();
            safeAreaFitter.ApplyNow();

            var vp = new GameObject("ApprovedDesignViewport", typeof(RectTransform), typeof(AspectRatioFitter));
            vp.transform.SetParent(safe.transform, false);
            viewport = vp.GetComponent<RectTransform>();
            Stretch(viewport);

            var aspect = vp.GetComponent<AspectRatioFitter>();
            aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspect.aspectRatio = DesignW / DesignH;
        }

        public void Show(ScreenId id)
        {
            if (GameState.Instance != null && GameState.Instance.TryGetRetainedRound(out _))
            {
                if (current == ScreenId.Fail && (id == ScreenId.Home || id == ScreenId.ChapterMap || id == ScreenId.WorldMap))
                {
                    if (!FinalizeRetainedRound()) return;
                }
                else if (id == ScreenId.Home || id == ScreenId.Gameplay || id == ScreenId.PreLevel)
                {
                    GameState.Instance.RestoreRetainedFailurePresentation();
                    id = ScreenId.Fail;
                }
            }
            if(id==ScreenId.TutorialOverlay && current==ScreenId.Gameplay){
                if(activeModalId==ScreenId.TutorialOverlay||IsModalTransitioning||IsScreenTransitioning)return;
                if(activeBoard==null)return;
                if(activeBoard.Phase!=LegendsOfRus.GameplayCore.GameplayPhase.Paused&&!activeBoard.TryPause())return;
                if(!ShowModal(ScreenId.TutorialOverlay))activeBoard.TryResume();return;
            }
            if(id==ScreenId.EmptyErrorState){ShowErrorDialog();return;}
            if(id!=current)friendsReturnToHome=id==ScreenId.FriendsList&&current==ScreenId.Home;
            if(id==ScreenId.TutorialOverlay&&current!=id)tutorialReturnPage=current;
            else if(id!=ScreenId.TutorialOverlay)tutorialReturnPage=null;
            if (id == ScreenId.Login && current != id) loginReturnPage = current;
            else if (id != ScreenId.Login) loginReturnPage = null;
            if (id == ScreenId.LanguageSelect && current != id) languageReturnPage = current;
            else if (id != ScreenId.LanguageSelect) languageReturnPage = null;
            if (id==ScreenId.Settings && current!=ScreenId.Settings && current!=ScreenId.LanguageSelect && current!=ScreenId.Login) settingsReturnPage=current;
            else if (id!=ScreenId.Settings && id!=ScreenId.LanguageSelect && id!=ScreenId.Login) settingsReturnPage=null;
            CancelLoadingPreparation();
            // The transient Back target belongs only to the directly opened auxiliary
            // page. Preserve it on a refresh of that page; clear it on every departure,
            // including forward exits that do not happen to pass through Home.
            if (splashAuxiliaryRoot.HasValue && id != splashAuxiliaryRoot.Value)
                splashAuxiliaryRoot = null;

            RuntimeOptimizationService optimization =
                RuntimeOptimizationService.Instance;
            optimization?.BeginScreenRoute();

            routeGeneration++;
            CancelModalImmediately();
            settingsReturnToPause = false;

            if (
                current == ScreenId.Gameplay &&
                activeAnalyticsAttemptNumber > 0 &&
                id != ScreenId.Victory &&
                id != ScreenId.Fail
            )
            {
                CancelActiveGameplayAnalytics();
            }

            current = id;
            activeBoard = null;
            if (id != ScreenId.Victory && id != ScreenId.Fail)
                activeOutcomeReaction = null;

            RetireContentRoot();

            contentRoot = CreateRoot($"Screen_{id}", viewport);
            var root = contentRoot.GetComponent<RectTransform>();
            contentRoot.AddComponent<MetaUiActionGate>();

            var group = contentRoot.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            bool designReady = BuildDesignLayer(id, root);
            if (designReady)
                BuildInteractionLayer(id, root);

            if (designReady && id == ScreenId.Dialogue && root.GetComponentInChildren<ProductionDialogueView>() == null)
                BuildStoryDialogueOverlay(root);
            else if (designReady && (id == ScreenId.Victory || id == ScreenId.Fail))
                BuildOutcomeReactionOverlay(root, id);
            else if (designReady && id == ScreenId.ChapterComplete)
                BuildChapterCompleteOverlay(root);

            if (designReady && UsesTopHud(id))
                BuildDynamicResourceHud(root);

            contentTransition = contentRoot.AddComponent<MetaUiTransition>();
            contentTransition.Initialize(
                motionPresenter,
                MetaUiMotionProfile.ResolveScreenPreset(id)
            );

            PrivacyAwareAnalytics.TryScreenView(id);
            if (id == ScreenId.Gameplay)
                BeginGameplayAnalytics();
            else if (id == ScreenId.Shop)
                RecordShopOfferViews();
            else if (id == ScreenId.Victory)
                PrivacyAwareAnalytics.TryRewardedAdOfferView(
                    MonetizationAllowlists.RewardedDoublePlacementId,
                    "reward_double"
                );

            if (id == ScreenId.Loading && designReady)
                BindLoadingPreparation();

            optimization?.CompleteScreenRoute(
                id != ScreenId.Gameplay && id != ScreenId.Dialogue
            );
        }

        void BindLoadingPreparation()
        {
            loadingView = contentRoot.GetComponentInChildren<ProductionLoadingView>(true);
            if (loadingView == null)
            {
                Debug.LogError("Loading page has no ProductionLoadingView; Home preparation was not started.");
                return;
            }
            int generation = routeGeneration;
            loadingView.BindRetry(() =>
            {
                if (current == ScreenId.Loading && generation == routeGeneration && loadingFailed)
                    StartLoadingPreparation(generation);
            });
            StartLoadingPreparation(generation);
        }

        void StartLoadingPreparation(int generation)
        {
            if (loadingRoutine != null || loadingView == null)
                return;
            homePreparation?.Dispose();
            homePreparation = new HomeLoadingPreparation();
            loadingFailed = false;
            loadingView.SetIndeterminate();
            loadingRoutine = StartCoroutine(PrepareHome(generation, homePreparation));
        }

        IEnumerator PrepareHome(int generation, HomeLoadingPreparation preparation)
        {
            // Let the newly built Loading page render before issuing resource requests.
            yield return null;
            if (!IsCurrentLoadingAttempt(generation, preparation))
                yield break;

            // Bootstrap owns service creation and save recovery. Home requires its existing
            // local state; this screen must not create a second service/startup system.
            if (GameState.Instance == null || GameState.Instance.Save == null)
            {
                FailLoading("home_local_state_unavailable");
                yield break;
            }

            yield return preparation.Run(progress =>
            {
                if (IsCurrentLoadingAttempt(generation, preparation))
                    loadingView.SetProgress(progress);
            });
            if (!IsCurrentLoadingAttempt(generation, preparation))
                yield break;
            if (!preparation.IsComplete || preparation.HasFailed)
            {
                FailLoading(preparation.FailureReason);
                yield break;
            }

            loadingView.SetProgress(1f);
            // Present completed work for one frame. There is no artificial minimum duration.
            yield return null;
            if (!IsCurrentLoadingAttempt(generation, preparation))
                yield break;
            if (GameState.Instance == null || GameState.Instance.Save == null)
            {
                FailLoading("home_local_state_unavailable");
                yield break;
            }

            // Detach before Show retires Loading so that its normal cancellation path cannot
            // clear the resources while the Home prefab's Awake consumes them.
            loadingRoutine = null;
            homePreparation = null;
            try
            {
                preparation.TryBuildHome(() => Show(ScreenId.Home));
            }
            finally
            {
                preparation.Dispose();
            }
        }

        bool IsCurrentLoadingAttempt(int generation, HomeLoadingPreparation preparation)
        {
            return this != null && current == ScreenId.Loading && routeGeneration == generation &&
                ReferenceEquals(homePreparation, preparation) && !preparation.IsCancelled &&
                loadingView != null && contentRoot != null && contentRoot.activeInHierarchy;
        }

        void FailLoading(string reason)
        {
            loadingRoutine = null;
            loadingFailed = true;
            homePreparation?.Dispose();
            loadingView.ShowError();
            Debug.LogWarning("Home preparation failed: " + reason);
        }

        void CancelLoadingPreparation()
        {
            if (loadingRoutine != null)
                StopCoroutine(loadingRoutine);
            loadingRoutine = null;
            homePreparation?.Dispose();
            homePreparation = null;
            if (loadingView != null)
                loadingView.BindRetry(null);
            loadingView = null;
            loadingFailed = false;
        }

        GameObject CreateRoot(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            Stretch(go.GetComponent<RectTransform>());
            return go;
        }

        void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        bool BuildDesignLayer(ScreenId id, RectTransform parent)
        {
            // v1.20 Step 3.3: the first core-loop wave is no longer allowed to
            // render a flattened DesignScreens screenshot. Production prefabs
            // are loaded first; a missing prefab produces a screenshot-free
            // emergency surface instead of silently regressing to the mockup.
            if (ProductionCoreLoopPageRegistry.IsCutover(id))
                return ProductionCoreLoopPageRuntime.TryBuild(id, parent);

            // Dispatch explicitly enabled meta pages before legacy/demo presentation.
            // In the supplied 01-R archive this was reached indirectly through
            // DesignScreenRegistry -> DemoFunctionalPageRuntime. This makes the
            // authority explicit; it does not promote the old R art to R2 approval.
            if (ProductionMetaPageCutoverRegistry.IsCutover(id))
                return ProductionMetaPageRuntime.TryBuild(id, parent);

            string key = DesignScreenRegistry.ResourceKey(id);
            if (string.IsNullOrEmpty(key))
                return DemoFunctionalPageRuntime.TryBuild(id, parent);

            Sprite sprite = Resources.Load<Sprite>(key);
            if (sprite == null)
            {
                Debug.LogError($"Missing design sprite for {id}: {key}");
                return false;
            }

            var imageGo = new GameObject("ApprovedDesign", typeof(RectTransform), typeof(Image));
            imageGo.transform.SetParent(parent, false);
            Stretch(imageGo.GetComponent<RectTransform>());

            var image = imageGo.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return true;
        }

        void BuildInteractionLayer(ScreenId id, RectTransform parent)
        {
            switch (id)
            {
                case ScreenId.Home:
                    ProductionHomeView home = parent.GetComponentInChildren<ProductionHomeView>(true);
                    if (home == null || !home.HasRequiredBindings)
                    {
                        Debug.LogError("Home page has no complete ProductionHomeView; visible navigation could not be bound.");
                        return;
                    }
                    home.BindNavigation(target => Show(target), OpenChapterMap);
                    return;

                case ScreenId.WorldMap:
                    ProductionWorldMapView worldMap = parent.GetComponentInChildren<ProductionWorldMapView>();
                    if (worldMap == null || !worldMap.HasRequiredBindings)
                    {
                        Debug.LogError("World Map has no complete native bindings.");
                        return;
                    }
                    worldMap.BindNavigation(target => Show(target), OpenChapterMap,
                        () => NavigateBackFrom(ScreenId.WorldMap), worldMap.Notice.Show);
                    return;

                case ScreenId.ChapterMap:
                    ProductionChapterMapView chapterMap=parent.GetComponentInChildren<ProductionChapterMapView>();
                    if(chapterMap==null||!chapterMap.HasRequiredBindings)
                    {
                        Debug.LogError("Chapter Map has no complete native bindings.");return;
                    }
                    chapterMap.BindNavigation(target=>Show(target),OpenSelectedLevel,
                        ()=>NavigateBackFrom(ScreenId.ChapterMap),chapterMap.Notice.Show);
                    return;

                case ScreenId.PreLevel:
                    var preparation=parent.GetComponentInChildren<ProductionPreLevelView>();
                    if(preparation==null||!preparation.HasRequiredBindings)
                        throw new InvalidOperationException("Pre-Level native bindings are missing.");
                    preparation.BindNavigation(target=>Show(target),
                        ()=>{StartLevel();return current!=ScreenId.PreLevel;},
                        ()=>NavigateBackFrom(ScreenId.PreLevel),()=>ShowModal(ScreenId.BoosterSelect));
                    return;

                case ScreenId.Gameplay:
                    BuildGameplayOverlay(parent);
                    break;

                case ScreenId.Dialogue:
                    var dialogue = parent.GetComponentInChildren<ProductionDialogueView>();
                    if (dialogue == null || !dialogue.HasRequiredBindings) throw new InvalidOperationException("Dialogue native bindings are missing.");
                    dialogue.Bind(ContinueAfterDialogue);
                    break;

                case ScreenId.Victory:
                    var victory=parent.GetComponentInChildren<ProductionVictoryView>();
                    if(victory==null||!victory.HasRequiredBindings)throw new InvalidOperationException("Victory native bindings are missing.");
                    victory.BindNavigation(ContinueAfterVictory,RequestRewardedDouble,target=>Show(target));
                    return;

                case ScreenId.Fail:
                    var failure=parent.GetComponentInChildren<ProductionFailView>();
                    if(failure==null||!failure.HasRequiredBindings)throw new InvalidOperationException("Failure native bindings are missing.");
                    failure.BindNavigation(RestartCurrentLevel,()=>Show(ScreenId.ChapterMap),target=>Show(target));
                    return;

                case ScreenId.Shop:
                    var shop=parent.GetComponentInChildren<ProductionShopView>();
                    if(shop!=null&&shop.HasRequiredBindings){shop.Bind(target=>Show(target),NavigateBack,offer=>RequestPurchase(offer,null));return;}
                    throw new InvalidOperationException("Shop native bindings are missing.");

                case ScreenId.DailyReward:
                    var daily=parent.GetComponentInChildren<ProductionDailyRewardView>();
                    if(daily==null||!daily.HasRequiredBindings)throw new InvalidOperationException("DailyReward native bindings are missing.");
                    daily.Bind(ClaimDailyRewardWithReceipt,NavigateBack,target=>Show(target));
                    return;

                case ScreenId.BoosterSelect:
                    var boosterPage=parent.GetComponentInChildren<ProductionBoosterSelectionView>();
                    if(boosterPage==null||!boosterPage.HasRequiredBindings)throw new InvalidOperationException("Booster Selection native bindings are missing.");
                    boosterPage.Bind(false,NavigateBack,()=>{StartLevel();return current!=ScreenId.BoosterSelect;});
                    return;

                case ScreenId.OutOfEnergy:
                    var energyPage=parent.GetComponentInChildren<ProductionOutOfEnergyView>();
                    if(energyPage==null||!energyPage.HasRequiredBindings)throw new InvalidOperationException("Out of Energy native bindings are missing.");
                    energyPage.Bind(false,NavigateBack,()=>RequestPurchase("energy",NavigateBack),RequestDemoFriendEnergy,RequestRewardedEnergy);
                    return;

                case ScreenId.RewardClaim:
                    var receiptPage=parent.GetComponentInChildren<ProductionRewardReceivedView>();
                    if(receiptPage==null||!receiptPage.HasRequiredBindings)throw new InvalidOperationException("Reward Received native bindings are missing.");
                    receiptPage.Bind(null,0,NavigateBack);
                    return;

                case ScreenId.Events:
                    var eventsPage=parent.GetComponentInChildren<ProductionEventsView>();
                    if(eventsPage==null||!eventsPage.HasRequiredBindings)throw new InvalidOperationException("Events native bindings are missing.");
                    eventsPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.Profile:
                    var profilePage=parent.GetComponentInChildren<ProductionProfileView>();
                    if(profilePage==null||!profilePage.HasRequiredBindings)throw new InvalidOperationException("Profile native bindings are missing.");
                    profilePage.Bind(target=>Show(target),NavigateBack,OpenStoryRecap);
                    return;

                case ScreenId.Settings:
                    BindNativeSettings(parent);
                    break;

                case ScreenId.Characters:
                    var charactersPage=parent.GetComponentInChildren<ProductionCharactersView>();
                    if(charactersPage==null||!charactersPage.HasRequiredBindings)throw new InvalidOperationException("Characters native bindings are missing.");
                    charactersPage.Bind(target=>Show(target),NavigateBack,category=>{Show(ScreenId.Shop);var shop=contentRoot.GetComponentInChildren<ProductionShopView>();if(shop!=null)shop.SelectCategory(category);});
                    return;

                case ScreenId.Mail:
                    var mailPage=parent.GetComponentInChildren<ProductionMailView>();
                    if(mailPage==null||!mailPage.HasRequiredBindings)throw new InvalidOperationException("Mail native bindings are missing.");
                    mailPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.SplashTitle:
                    SplashTitleView splash = parent.GetComponentInChildren<SplashTitleView>(true);
                    if (splash == null || !splash.HasRequiredBindings)
                    {
                        Debug.LogError("SplashTitle native control bindings are missing. No legacy hotspots were created.");
                        break;
                    }
                    splash.BindActions(
                        GuardSplashAction(() => Show(ScreenId.Loading)),
                        GuardSplashAction(() => OpenSplashAuxiliary(ScreenId.LanguageSelect)),
                        GuardSplashAction(() => OpenSplashAuxiliary(ScreenId.Settings)),
                        GuardSplashAction(() => OpenSplashAuxiliary(ScreenId.Login)),
                        GuardSplashAction(() => OpenSplashAuxiliary(ScreenId.TutorialOverlay)));
                    break;

                case ScreenId.EventDetail:
                    var eventDetailPage=parent.GetComponentInChildren<ProductionEventDetailView>();
                    if(eventDetailPage==null||!eventDetailPage.HasRequiredBindings)throw new InvalidOperationException("Event Detail native bindings are missing.");
                    eventDetailPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.IndividualRanking:
                    var rankingPage=parent.GetComponentInChildren<ProductionIndividualRankingView>();
                    if(rankingPage==null||!rankingPage.HasRequiredBindings)throw new InvalidOperationException("Individual Ranking native bindings are missing.");
                    rankingPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.TeamHub:
                    var teamHubPage=parent.GetComponentInChildren<ProductionTeamHubView>();
                    if(teamHubPage==null||!teamHubPage.HasRequiredBindings)throw new InvalidOperationException("Team Hub native bindings are missing.");
                    teamHubPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.TeamRanking:
                    var teamRankingPage=parent.GetComponentInChildren<ProductionTeamRankingView>();
                    if(teamRankingPage==null||!teamRankingPage.HasRequiredBindings)throw new InvalidOperationException("Team Ranking native bindings are missing.");
                    teamRankingPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.TeamChat:
                    var chatPage=parent.GetComponentInChildren<ProductionTeamChatView>();
                    if(chatPage==null||!chatPage.HasRequiredBindings)throw new InvalidOperationException("Team Chat native bindings are missing.");
                    chatPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.FriendsList:
                    var friendsPage=parent.GetComponentInChildren<ProductionFriendsListView>();
                    if(friendsPage==null||!friendsPage.HasRequiredBindings)throw new InvalidOperationException("Friends List native bindings are missing.");
                    friendsPage.Bind(target=>Show(target),NavigateBack);
                    return;

                case ScreenId.Inventory:
                    var inventoryPage=parent.GetComponentInChildren<ProductionInventoryView>();
                    if(inventoryPage==null||!inventoryPage.HasRequiredBindings)throw new InvalidOperationException("Inventory native bindings are missing.");
                    inventoryPage.Bind(target=>Show(target),NavigateBack,category=>{Show(ScreenId.Shop);var shop=contentRoot.GetComponentInChildren<ProductionShopView>();if(shop!=null)shop.SelectCategory(category);});
                    return;

                case ScreenId.SpecialOffer:
                    var offer=parent.GetComponentInChildren<ProductionSpecialOfferView>();
                    if(offer==null||!offer.HasRequiredBindings)throw new InvalidOperationException("Special Offer native bindings are missing.");
                    offer.Bind(NavigateBack,()=>Show(ScreenId.Shop),idToBuy=>RequestPurchase(idToBuy,null));
                    return;

                case ScreenId.LanguageSelect:
                    var languagePage = parent.GetComponentInChildren<ProductionLanguageSelectView>();
                    if (languagePage == null || !languagePage.HasRequiredBindings)
                        throw new InvalidOperationException("Language Select native bindings are missing.");
                    languagePage.Bind(NavigateBack);
                    return;

                case ScreenId.Login:
                    var loginPage = parent.GetComponentInChildren<ProductionLoginView>();
                    if (loginPage == null || !loginPage.HasRequiredBindings) throw new InvalidOperationException("Login native bindings are missing.");
                    loginPage.Bind(ContinueAsDemoGuest, NavigateBack);
                    return;

                case ScreenId.TutorialOverlay:
                    var help=parent.GetComponentInChildren<ProductionTutorialView>();
                    if(help==null||!help.HasRequiredBindings)throw new InvalidOperationException("Tutorial native bindings are missing.");
                    help.Bind(NavigateBack,()=>{if(GameState.Instance.SelectLevel(1,1))Show(ScreenId.PreLevel);},target=>Show(target),openShop:category=>{Show(ScreenId.Shop);var shop=contentRoot.GetComponentInChildren<ProductionShopView>();if(shop)shop.SelectCategory(category);});
                    return;

                case ScreenId.OfflineState:
                    var offline=parent.GetComponentInChildren<ProductionOfflineView>();
                    if(offline==null||!offline.HasRequiredBindings)throw new InvalidOperationException("Offline native bindings are missing.");
                    offline.Bind(NavigateBack,null,target=>Show(target),category=>{Show(ScreenId.Shop);var offlineShop=contentRoot.GetComponentInChildren<ProductionShopView>();if(offlineShop)offlineShop.SelectCategory(category);});
                    return;


                case ScreenId.MaintenanceState:
                    var maintenance=parent.GetComponentInChildren<ProductionMaintenanceView>();
                    if(maintenance==null||!maintenance.HasRequiredBindings)throw new InvalidOperationException("Maintenance native bindings are missing.");
                    maintenance.Bind(NavigateBack);
                    return;

                case ScreenId.ChapterComplete:
                    var chapterComplete=parent.GetComponentInChildren<ProductionChapterCompleteView>();
                    if(chapterComplete==null||!chapterComplete.HasRequiredBindings)throw new InvalidOperationException("Chapter Complete requires its aligned native prefab.");
                    chapterComplete.Bind(()=>{int generation=RouteGeneration;ContinueAfterChapterComplete(ScreenId.WorldMap);return RouteGeneration!=generation;});
                    break;

                case ScreenId.Chapter15:
                    Hotspot(parent, "Back", 0, 0, 180, 180, () => Show(ScreenId.WorldMap));
                    break;
            }
        }

        void BindNativeSettings(RectTransform parent)
        {
            var view=parent.GetComponentInChildren<ProductionSettingsView>();
            if(view==null || !view.HasRequiredBindings)throw new InvalidOperationException("Settings native bindings are missing.");
            view.Bind(target=>Show(target),NavigateBack,category=>{
                Show(ScreenId.Shop);
                var shop=contentRoot.GetComponentInChildren<ProductionShopView>();
                if(shop!=null)shop.SelectCategory(category);
            });
        }

        void ClaimDemoEventReward()
        {
            if (DemoFeatureState.TryClaimEventReward())
                Toast("Демо-награда отмечена как полученная. Валюта не начисляется без авторитетного LiveOps-рецепта.");
            else
                Toast("Демо-награда уже получена в этой сессии.");
        }

        void SendDemoTeamMessage()
        {
            int count = DemoFeatureState.RecordTeamMessage();
            Toast("Локальное демо-сообщение отправлено. Счётчик: " + count + ". Серверный чат не подключён.");
        }

        void RequestDemoFriendEnergy()
        {
            if (DemoFeatureState.TryRequestFriendEnergy())
                Toast("Демо-запрос энергии записан локально. Энергия не начисляется без серверного подтверждения.");
            else
                Toast("Запрос энергии уже отправлен в этой демо-сессии.");
        }

        void ContinueAsDemoGuest()
        {
            DemoFeatureState.SignInGuest();
            Toast("Гостевая демо-сессия активна. Облачная учётная запись не подключена.");
            Show(ScreenId.Home);
        }

        void RequestPurchase(string offerId, Action onCommitted)
        {
            string productId;
            string placementId;
            bool analyticsIdentityKnown = TryResolvePurchaseAnalyticsIdentity(
                offerId,
                out productId,
                out placementId
            );
            if (analyticsIdentityKnown)
                PrivacyAwareAnalytics.TryPurchaseStart(productId, placementId);

            RuntimeMonetizationActionResult result =
                MonetizationService.Instance != null
                    ? MonetizationService.Instance.TryPurchase(offerId)
                    : null;
            if (analyticsIdentityKnown)
            {
                PrivacyAwareAnalytics.TryPurchaseResult(
                    productId,
                    placementId,
                    PurchaseAnalyticsOutcome(result)
                );
            }
            if (
                result == null ||
                !result.IsVerifiedAndDurablyCommitted
            )
            {
                Toast(
                    "Покупки сейчас недоступны. " +
                    "Основная игра продолжает работать."
                );
                return;
            }

            onCommitted?.Invoke();
        }

        void RequestRewardedDouble()
        {
            PrivacyAwareAnalytics.TryRewardedAdStart(
                MonetizationAllowlists.RewardedDoublePlacementId,
                "reward_double"
            );
            RuntimeMonetizationActionResult result =
                MonetizationService.Instance != null
                    ? MonetizationService.Instance.TryRewardedDouble()
                    : null;
            PrivacyAwareAnalytics.TryRewardedAdResult(
                MonetizationAllowlists.RewardedDoublePlacementId,
                "reward_double",
                RewardedAnalyticsOutcome(result),
                result != null && result.IsVerifiedAndDurablyCommitted
            );
            if (
                result == null ||
                !result.IsVerifiedAndDurablyCommitted
            )
            {
                Toast(
                    "Видео с наградой сейчас недоступно. " +
                    "Вы можете забрать обычную награду."
                );
                return;
            }

            Toast("Проверенная награда успешно начислена.");
        }

        void RequestRewardedEnergy()
        {
            PrivacyAwareAnalytics.TryRewardedAdStart(
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "energy_1"
            );
            RuntimeMonetizationActionResult result =
                MonetizationService.Instance != null
                    ? MonetizationService.Instance.TryRewardedEnergy()
                    : null;
            PrivacyAwareAnalytics.TryRewardedAdResult(
                MonetizationAllowlists.RewardedEnergyPlacementId,
                "energy_1",
                RewardedAnalyticsOutcome(result),
                result != null && result.IsVerifiedAndDurablyCommitted
            );
            if (
                result == null ||
                !result.IsVerifiedAndDurablyCommitted
            )
            {
                Toast(
                    "Видео с энергией сейчас недоступно. " +
                    "Окно можно закрыть и продолжить позже."
                );
                return;
            }

            RequestCloseModal(StartLevel);
        }

        void RecordShopOfferViews()
        {
            PrivacyAwareAnalytics.TryPurchaseOfferView(
                MonetizationAllowlists.StarterProductId,
                "shop_starter"
            );
            PrivacyAwareAnalytics.TryPurchaseOfferView(
                MonetizationAllowlists.HeroProductId,
                "shop_hero"
            );
            PrivacyAwareAnalytics.TryPurchaseOfferView(
                MonetizationAllowlists.FirebirdProductId,
                "shop_firebird"
            );
        }

        static bool TryResolvePurchaseAnalyticsIdentity(
            string offerId,
            out string productId,
            out string placementId
        )
        {
            productId = string.Empty;
            placementId = string.Empty;
            switch (offerId)
            {
                case MonetizationAllowlists.StarterOfferId:
                    productId = MonetizationAllowlists.StarterProductId;
                    placementId = "shop_starter";
                    return true;
                case MonetizationAllowlists.HeroOfferId:
                    productId = MonetizationAllowlists.HeroProductId;
                    placementId = "shop_hero";
                    return true;
                case MonetizationAllowlists.FirebirdOfferId:
                    productId = MonetizationAllowlists.FirebirdProductId;
                    placementId = "shop_firebird";
                    return true;
                case MonetizationAllowlists.EnergyOfferId:
                    productId = MonetizationAllowlists.EnergyProductId;
                    placementId = "out_of_energy_purchase";
                    return true;
                default:
                    return false;
            }
        }

        static string PurchaseAnalyticsOutcome(
            RuntimeMonetizationActionResult result
        )
        {
            if (result == null)
                return "failed";
            if (result.IsVerifiedAndDurablyCommitted)
                return "verified";
            return result.State == RuntimeMonetizationActionState.CandidateOnly
                ? "rejected"
                : "unavailable";
        }

        static string RewardedAnalyticsOutcome(
            RuntimeMonetizationActionResult result
        )
        {
            if (result == null)
                return "failed";
            if (result.IsVerifiedAndDurablyCommitted)
                return "completed";
            return result.State == RuntimeMonetizationActionState.CandidateOnly
                ? "failed"
                : "unavailable";
        }

        void StartLevel()
        {
            if (GameState.Instance.TryGetRetainedRound(out _)) { Show(ScreenId.Fail); return; }
            if (!GameState.Instance.CanPersist) { ShowErrorDialog(); return; }
            if (!GameState.Instance.TryRecoverPendingBoosterCommand() || !GameState.Instance.CanPersist)
            {
                ShowErrorDialog(GameState.Instance.CanPersist ? (Action)StartLevel : null);
                return;
            }
            GameState.Instance.EnsureSelectedLevelIsUnlocked();
            if (!GameState.Instance.SpendEnergy())
            {
                if (!GameState.Instance.CanPersist)
                {
                    ShowErrorDialog();
                    return;
                }
                LevelData selected = ChapterLevelCatalog.ResolveSelected(
                    GameState.Instance.Save
                );
                if (selected != null)
                {
                    PrivacyAwareAnalytics.TryEnergyGateBlocked(
                        selected.id,
                        1,
                        Mathf.Max(0, GameState.Instance.Save.energy)
                    );
                }
                ShowModal(ScreenId.OutOfEnergy);
                return;
            }

            Show(ScreenId.Gameplay);
        }

        void OpenSelectedLevel()
        {
            GameState.Instance.EnsureSelectedLevelIsUnlocked();
            if (StoryService.TryBeginChapterIntro())
                Show(ScreenId.Dialogue);
            else if (TryShowPendingChapterFlow())
                return;
            else
                Show(ScreenId.PreLevel);
        }

        void OpenChapterMap()
        {
            if (StoryService.ShouldShowChapterIntro())
            {
                Show(ScreenId.ChapterMap);
                return;
            }

            if (TryShowPendingChapterFlow())
                return;

            Show(ScreenId.ChapterMap);
        }

        bool TryShowPendingChapterFlow()
        {
            if (StoryService.TryBeginPostVictoryDialogue())
            {
                Show(ScreenId.Dialogue);
                return true;
            }

            if (StoryService.ShouldShowChapterComplete())
            {
                Show(ScreenId.ChapterComplete);
                return true;
            }

            return false;
        }

        void RestartCurrentLevel()
        {
            if (!FinalizeRetainedRound()) return;
            Show(ScreenId.Gameplay); // Preserve the existing retry energy policy.
        }

        bool FinalizeRetainedRound()
        {
            var state = GameState.Instance;
            if (!state.TryGetRetainedRound(out var retained)) return true;
            if (state.TryFinalizeRetainedFailure(retained.attemptId)) return true;
            // Rebuild resets the native button's one-shot navigation latch after a failed write.
            Show(ScreenId.Fail);
            Toast(DemoFeatureState.LanguageCode == "ru"
                ? "Не удалось сохранить раунд. Повторите попытку."
                : "Could not save the round. Please try again.");
            return false;
        }

        void ContinueAfterVictory()
        {
            if (TryShowPendingChapterFlow())
                return;

            Show(ScreenId.ChapterMap);
        }

        void ContinueAfterDialogue()
        {
            switch (StoryService.AdvanceDialogue())
            {
                case StoryAdvanceResult.SaveRejected:
                    var dialogue = contentRoot.GetComponentInChildren<ProductionDialogueView>();
                    // Re-arm the existing one-shot control without replacing the current page.
                    if (dialogue != null) dialogue.Bind(ContinueAfterDialogue);
                    ShowErrorDialog();
                    break;
                case StoryAdvanceResult.MoreLines:
                    Show(ScreenId.Dialogue);
                    break;
                case StoryAdvanceResult.PreLevel:
                    Show(ScreenId.PreLevel);
                    break;
                case StoryAdvanceResult.ChapterComplete:
                    Show(ScreenId.ChapterComplete);
                    break;
                case StoryAdvanceResult.ChapterMap:
                    Show(ScreenId.ChapterMap);
                    break;
                default:
                    Show(ScreenId.ChapterMap);
                    break;
            }
        }

        void ContinueAfterChapterComplete(ScreenId destination)
        {
            if (
                StoryService.ShouldShowChapterComplete() &&
                !StoryService.AcknowledgeChapterComplete()
            )
            {
                ShowErrorDialog();
                return;
            }

            Show(destination);
        }

        void ClaimDailyReward()
        {
            if (!ClaimDailyRewardWithReceipt()) Toast("Сегодняшняя награда уже получена или не сохранена.");
        }

        bool ClaimDailyRewardWithReceipt()
        {
            if (modalMotionState != ModalMotionState.Closed || !GameState.Instance.ClaimDailyReward(out var granted)) return false;
            pendingRewardReceipt = granted;
            pendingRewardDay = GameState.Instance.Save.dailyRewardCycleDay;
            if (!ShowModal(ScreenId.RewardClaim)) { pendingRewardReceipt = null; pendingRewardDay = 0; }
            return true; // Persistence succeeded; the modal cannot grant or roll back the transaction.
        }

        void ShowPause()
        {
            if (current != ScreenId.Gameplay ||
                modalMotionState != ModalMotionState.Closed ||
                activeBoard == null)
                return;

            if (!activeBoard.TryPause())
            {
                Toast("Дождитесь завершения хода.");
                return;
            }

            if (!ShowModal(ScreenId.Pause))
                activeBoard.TryResume();
        }

        /// <summary>Show an error over the current meta page. Retry runs once after close, only if the route is still current.
        /// Supply the actual failed operation; null disables Retry. Gameplay integration is intentionally deferred.</summary>
        public bool ShowErrorDialog(Action retryFailedOperation=null)
        {
            if(current==ScreenId.Gameplay||modalMotionState!=ModalMotionState.Closed||IsScreenTransitioning)return false;
            if(!ShowModal(ScreenId.EmptyErrorState))return false;
            var error=modalVisualRoot.GetComponentInChildren<ProductionErrorDialogView>();
            error.Bind(CloseModal,retryFailedOperation==null?(Action)null:()=>RequestCloseModal(retryFailedOperation));
            return true;
        }

        void OpenStoryRecap()
        {
            if(current!=ScreenId.Profile || HasActiveModalBlocker || IsScreenTransitioning)return;
            var profile=contentRoot.GetComponentInChildren<ProductionProfileView>();
            if(profile==null || !profile.CanAct() || profile.Section!=ProfileSection.History)return;
            recapEntries=StoryRecap.Read(GameState.Instance!=null?GameState.Instance.Save:null);
            if(recapEntries.Count==0){recapEntries=null;return;}
            if(!ShowModal(ScreenId.Dialogue))recapEntries=null;
        }

        bool ShowModal(ScreenId id)
        {
            if (!IsSupportedModal(id) || (id==ScreenId.Dialogue && (current!=ScreenId.Profile || recapEntries==null)))
            {
                Debug.LogError($"Unsupported modal route rejected: {id}");
                return false;
            }

            if (modalMotionState != ModalMotionState.Closed)
            {
                if (modalMotionState == ModalMotionState.Closing)
                {
                    if (modalAfterClose != null)
                        return false;
                    pendingModalId = id;
                    return true;
                }

                if (activeModalId == id && !pendingModalId.HasValue)
                    return true;

                pendingModalId = id;
                RequestCloseModal(null);
                return true;
            }

            return BuildModal(id);
        }

        bool BuildModal(ScreenId id)
        {
            modalRoot = CreateRoot($"Modal_{id}", viewport);
            var blockerRect = modalRoot.GetComponent<RectTransform>();

            // This blocker is outside the animated visual group. It remains
            // raycast-active through the final exit frame, so gameplay and the
            // underlying screen never receive input through a fading modal.
            var blocker = modalRoot.AddComponent<Image>();
            blocker.color = new Color(0f, 0f, 0f, 0.001f);
            blocker.raycastTarget = true;

            modalVisualRoot = CreateRoot($"ModalVisual_{id}", blockerRect);
            var visualRect = modalVisualRoot.GetComponent<RectTransform>();
            modalVisualRoot.AddComponent<MetaUiActionGate>();
            CanvasGroup visualGroup = modalVisualRoot.AddComponent<CanvasGroup>();
            visualGroup.alpha = 0f;
            visualGroup.interactable = false;
            visualGroup.blocksRaycasts = false;

            if (!BuildDesignLayer(id, visualRect))
            {
                RetireModalRootImmediately();
                return false;
            }

            activeModalId = id;
            modalMotionState = ModalMotionState.Opening;
            if (id == ScreenId.Pause)
                settingsReturnToPause = false;

            BuildModalInteractionLayer(id, visualRect);
            modalTransition = modalVisualRoot.AddComponent<MetaUiTransition>();
            modalTransition.Initialize(
                motionPresenter,
                MetaUiMotionProfile.ResolveModalPreset(id),
                this
            );
            PrivacyAwareAnalytics.TryScreenView(id);
            if (id == ScreenId.OutOfEnergy)
            {
                PrivacyAwareAnalytics.TryPurchaseOfferView(
                    MonetizationAllowlists.EnergyProductId,
                    "out_of_energy_purchase"
                );
                PrivacyAwareAnalytics.TryRewardedAdOfferView(
                    MonetizationAllowlists.RewardedEnergyPlacementId,
                    "energy_1"
                );
            }
            return true;
        }

        void BuildModalInteractionLayer(ScreenId id, RectTransform parent)
        {
            switch (id)
            {
                case ScreenId.Dialogue:
                    var dialogue=parent.GetComponentInChildren<ProductionDialogueView>();
                    if(dialogue==null||!dialogue.HasRequiredBindings)throw new InvalidOperationException("Recap requires native Dialogue bindings.");
                    dialogue.gameObject.AddComponent<ProductionStoryRecapView>().Bind(dialogue,recapEntries,
                        ()=>activeModalId==ScreenId.Dialogue && !IsModalTransitioning && !IsScreenTransitioning,CloseModal);
                    break;
                case ScreenId.TutorialOverlay:
                    var guide=parent.GetComponentInChildren<ProductionTutorialView>();
                    if(guide==null||!guide.HasRequiredBindings)throw new InvalidOperationException("Tutorial modal native bindings missing.");
                    guide.Bind(NavigateBack,()=>RequestCloseModal(()=>activeBoard?.TryResume()),null,true);
                    break;
                case ScreenId.EmptyErrorState:
                    var error=parent.GetComponentInChildren<ProductionErrorDialogView>();
                    if(error==null||!error.HasRequiredBindings)throw new InvalidOperationException("Error Dialog native bindings are missing.");
                    error.Bind(CloseModal,null);
                    break;
                case ScreenId.Pause:
                    var pause = parent.GetComponentInChildren<ProductionPauseView>();
                    if (pause == null || !pause.HasRequiredBindings)
                        throw new InvalidOperationException("Pause requires its aligned native prefab.");
                    pause.Bind(activeBoard, ResumeGameplayFromPause,
                        () => RequestCloseModal(RestartAfterPauseClose),
                        () => { settingsReturnToPause = true; ShowModal(ScreenId.Settings); },
                        () => RequestCloseModal(ExitAfterPauseClose));
                    break;

                case ScreenId.OutOfEnergy:
                    var energy=parent.GetComponentInChildren<ProductionOutOfEnergyView>();
                    if(energy==null||!energy.HasRequiredBindings)throw new InvalidOperationException("Out of Energy native bindings are missing.");
                    energy.Bind(true,CloseModal,()=>RequestPurchase("energy",()=>RequestCloseModal(StartLevel)),RequestDemoFriendEnergy,RequestRewardedEnergy);
                    break;

                case ScreenId.BoosterSelect:
                    var boosters=parent.GetComponentInChildren<ProductionBoosterSelectionView>();
                    if(boosters==null||!boosters.HasRequiredBindings)throw new InvalidOperationException("Booster Selection native bindings are missing.");
                    boosters.Bind(true,CloseModal,()=>{RequestCloseModal(StartLevel);return true;});
                    break;

                case ScreenId.RewardClaim:
                    var received = parent.GetComponentInChildren<ProductionRewardReceivedView>();
                    if (received == null || !received.HasRequiredBindings) throw new InvalidOperationException("Reward Received native bindings are missing.");
                    received.Bind(pendingRewardReceipt, pendingRewardDay, CloseModal);
                    break;

                case ScreenId.Settings:
                    BindNativeSettings(parent);
                    break;
            }
        }

        void CloseModal()
        {
            RequestCloseModal(null);
        }

        void RequestCloseModal(Action afterClose)
        {
            if (modalMotionState == ModalMotionState.Closed || modalRoot == null)
            {
                afterClose?.Invoke();
                return;
            }

            if (modalMotionState == ModalMotionState.Closing)
                return;

            modalAfterClose = afterClose;
            modalContinuationGeneration = routeGeneration;
            modalMotionState = ModalMotionState.Closing;

            if (modalTransition != null)
                modalTransition.PlayExit();
            else
                CompleteModalClose();
        }

        public void OnMetaUiTransitionCompleted(
            MetaUiTransition transition,
            MetaUiTransitionPhase phase
        )
        {
            if (transition == null || transition != modalTransition)
                return;

            if (phase == MetaUiTransitionPhase.Entered &&
                modalMotionState == ModalMotionState.Opening)
            {
                modalMotionState = ModalMotionState.Open;
                return;
            }

            if (phase == MetaUiTransitionPhase.Exited &&
                modalMotionState == ModalMotionState.Closing)
                CompleteModalClose();
        }

        void CompleteModalClose()
        {
            Action continuation = modalAfterClose;
            int continuationGeneration = modalContinuationGeneration;
            ScreenId? replacement = pendingModalId;

            RetireModalRootImmediately();

            if (replacement.HasValue)
            {
                if (!BuildModal(replacement.Value))
                {
                    settingsReturnToPause = false;
                    if (current == ScreenId.Gameplay && activeBoard != null)
                        activeBoard.TryResume();
                }
                return;
            }

            settingsReturnToPause = false;
            if (continuationGeneration == routeGeneration)
                continuation?.Invoke();
        }

        void CancelModalImmediately()
        {
            pendingModalId = null;
            modalAfterClose = null;
            modalContinuationGeneration = 0;
            settingsReturnToPause = false;
            RetireModalRootImmediately();
        }

        void RetireModalRootImmediately()
        {
            recapEntries=null;
            if (activeModalId == ScreenId.RewardClaim) { pendingRewardReceipt = null; pendingRewardDay = 0; }
            GameObject retiringRoot = modalRoot;
            modalRoot = null;
            modalVisualRoot = null;
            modalTransition = null;
            activeModalId = null;
            pendingModalId = null;
            modalAfterClose = null;
            modalContinuationGeneration = 0;
            modalMotionState = ModalMotionState.Closed;

            if (retiringRoot != null)
            {
                retiringRoot.SetActive(false);
                Destroy(retiringRoot);
            }
        }

        static bool IsSupportedModal(ScreenId id)
        {
            return id == ScreenId.Dialogue || id == ScreenId.TutorialOverlay || id == ScreenId.Pause ||
                id == ScreenId.Settings ||
                id == ScreenId.OutOfEnergy ||
                id == ScreenId.BoosterSelect ||
                id == ScreenId.RewardClaim ||
                id == ScreenId.EmptyErrorState;
        }

        void RetireContentRoot()
        {
            GameObject retiringRoot = contentRoot;
            contentRoot = null;
            contentTransition = null;

            if (retiringRoot != null)
            {
                retiringRoot.SetActive(false);
                Destroy(retiringRoot);
                RuntimeOptimizationService.Instance?.NotifyScreenRetired();
            }
        }

        void ResumeGameplayFromPause()
        {
            RequestCloseModal(ResumeAfterPauseClose);
        }

        void ResumeAfterPauseClose()
        {
            if (activeBoard != null && !activeBoard.TryResume() && IsGameplayInventoryError(activeBoard))
                StartCoroutine(ShowGameplayInventoryError(activeBoard));
        }

        void RestartAfterPauseClose()
        {
            CancelActiveGameplayAnalytics();
            activeBoard?.TryResume();
            RestartCurrentLevel();
        }

        void ExitAfterPauseClose()
        {
            CancelActiveGameplayAnalytics();
            activeBoard?.TryResume();
            Show(ScreenId.ChapterMap);
        }

        bool IsGameplayInventoryError(Match3Board board) => board != null && board == activeBoard &&
            current == ScreenId.Gameplay && board.Phase == LegendsOfRus.GameplayCore.GameplayPhase.Paused &&
            board.HasInventoryRecoveryError;

        void CloseGameplayInventoryError(Match3Board board)
        {
            RequestCloseModal(() => { if(IsGameplayInventoryError(board))Show(ScreenId.ChapterMap); });
        }

        IEnumerator ShowGameplayInventoryError(Match3Board board)
        {
            while(IsGameplayInventoryError(board) && (IsScreenTransitioning || modalMotionState != ModalMotionState.Closed))
                yield return null;
            if(!IsGameplayInventoryError(board) || !ShowModal(ScreenId.EmptyErrorState))yield break;
            var error=modalVisualRoot.GetComponentInChildren<ProductionErrorDialogView>();
            Action retry=GameState.Instance.CanPersist ? (Action)(() => RequestCloseModal(() =>
            {
                if(IsGameplayInventoryError(board) && !board.TryResume())StartCoroutine(ShowGameplayInventoryError(board));
            })) : null;
            error.Bind(() => CloseGameplayInventoryError(board),retry);
        }

        bool IsPendingGameplayVictory(Match3Board board)
        {
            return board != null && board == activeBoard && current == ScreenId.Gameplay &&
                board.Phase == LegendsOfRus.GameplayCore.GameplayPhase.Completed;
        }

        void CloseRejectedGameplayVictory(Match3Board board)
        {
            RequestCloseModal(() => { if(IsPendingGameplayVictory(board))Show(ScreenId.ChapterMap); });
        }

        IEnumerator ShowGameplayCompletionError(Match3Board board, int score)
        {
            // Wait only for existing UI transitions; stale boards/routes never receive this retry.
            while(IsPendingGameplayVictory(board) && (IsScreenTransitioning || modalMotionState != ModalMotionState.Closed))
                yield return null;
            if(!IsPendingGameplayVictory(board) || !ShowModal(ScreenId.EmptyErrorState))yield break;
            var error=modalVisualRoot.GetComponentInChildren<ProductionErrorDialogView>();
            Action retry=GameState.Instance.CanPersist
                ? (Action)(() => RequestCloseModal(() => CommitGameplayVictory(board,score))) : null;
            error.Bind(() => CloseRejectedGameplayVictory(board),retry);
        }

        void CommitGameplayVictory(Match3Board board, int score)
        {
            if(!IsPendingGameplayVictory(board))return;
            LevelData outcomeLevel=board.CurrentLevel;
            if(!GameState.Instance.TryCompleteLevel(outcomeLevel,score,out var completion))
            {
                StartCoroutine(ShowGameplayCompletionError(board,score));
                return;
            }
            PrivacyAwareAnalytics.TryLevelComplete(completion.Level.id,activeAnalyticsAttemptNumber,
                score,completion.Stars,completion.FirstClearRewardGranted);
            BalanceTelemetryReviewService.Instance?.TryRecordLevelWin(outcomeLevel,
                activeAnalyticsAttemptNumber,board.Moves,score);
            ClearActiveGameplayAnalytics();
            ShowOutcome(ScreenId.Victory,outcomeLevel,StoryReactionOutcomes.Victory);
        }

        void BuildGameplayOverlay(RectTransform parent)
        {
            var view = parent.GetComponentInChildren<ProductionGameplayView>();
            if (view == null || !view.HasRequiredBindings)
                throw new InvalidOperationException("Gameplay requires the aligned native prefab.");
            var boardGo = new GameObject("RuntimeMatch3Board", typeof(RectTransform), typeof(Match3Board));
            boardGo.transform.SetParent(view.BoardMount, false);
            Stretch(boardGo.GetComponent<RectTransform>());

            var board = boardGo.GetComponent<Match3Board>();
            activeBoard = board;
            board.InventoryRecoveryRequired += failed => StartCoroutine(ShowGameplayInventoryError(failed));
            activeBoard.Initialize(
                onWin: score => CommitGameplayVictory(board,score),
                onFail: score =>
                {
                    LevelData outcomeLevel = activeBoard.CurrentLevel;
                    bool retained = GameState.Instance.TrySuspendRound(activeBoard.CaptureFailedRound());
                    ClearActiveGameplayAnalytics();
                    ShowOutcome(
                        ScreenId.Fail,
                        outcomeLevel,
                        StoryReactionOutcomes.Failure
                    );
                    if (!retained) Toast(DemoFeatureState.LanguageCode == "ru"
                        ? "Раунд не сохранён. Не закрывайте игру; повторите попытку или выйдите через меню."
                        : "Round not saved. Keep the game open; retry or leave through the menu.");
                }
            );

            view.Bind(activeBoard, ShowPause);
            view.GetComponent<ProductionGameplayLayout>().ApplyNow();

            var tutorialGo = new GameObject(
                "LevelTutorialPresenter",
                typeof(RectTransform),
                typeof(LevelTutorialPresenter)
            );
            tutorialGo.transform.SetParent(parent, false);
            Stretch(tutorialGo.GetComponent<RectTransform>());
            tutorialGo.GetComponent<LevelTutorialPresenter>().Initialize(
                activeBoard,
                activeBoard.CurrentLevel
            );
        }

        void BeginGameplayAnalytics()
        {
            ClearActiveGameplayAnalytics();
            GameState state = GameState.Instance;
            LevelData level = activeBoard != null
                ? activeBoard.CurrentLevel
                : null;
            if (state == null || level == null)
                return;

            LevelProgressData progress = state.GetLevelProgress(
                level.chapterId,
                level.levelId
            );
            if (progress == null || progress.attempts <= 0)
                return;

            activeAnalyticsLevelId = level.id ?? string.Empty;
            activeAnalyticsAttemptNumber = progress.attempts;
            PrivacyAwareAnalytics.TryLevelStart(
                activeAnalyticsLevelId,
                activeAnalyticsAttemptNumber,
                activeAnalyticsAttemptNumber > 1
            );
            BalanceTelemetryReviewService.Instance?.TryRecordLevelStart(
                level, activeAnalyticsAttemptNumber);
        }

        void CancelActiveGameplayAnalytics()
        {
            if (
                activeAnalyticsAttemptNumber > 0 &&
                !string.IsNullOrEmpty(activeAnalyticsLevelId)
            )
            {
                PrivacyAwareAnalytics.TryCancelLevelAttempt(
                    activeAnalyticsLevelId,
                    activeAnalyticsAttemptNumber
                );
            }
            ClearActiveGameplayAnalytics();
        }

        void ClearActiveGameplayAnalytics()
        {
            activeAnalyticsLevelId = string.Empty;
            activeAnalyticsAttemptNumber = 0;
        }

        void BuildPreLevelDataOverlay(RectTransform parent)
        {
            PlayerSaveData save = GameState.Instance != null
                ? GameState.Instance.Save
                : null;
            LevelData level = ChapterLevelCatalog.ResolveSelected(save);
            LevelGoalData[] goals = level.goals;
            int firstGoal = goals != null && goals.Length > 0 && goals[0] != null
                ? goals[0].target
                : 0;
            int secondGoal = goals != null && goals.Length > 1 && goals[1] != null
                ? goals[1].target
                : 0;
            LevelRewardData reward = level.rewards ?? new LevelRewardData();

            CreateMaskedValue(parent, "PreLevelGoal1", 320, 620, 85, 72, firstGoal.ToString(), 38);
            CreateMaskedValue(parent, "PreLevelGoal2", 525, 620, 85, 72, secondGoal.ToString(), 38);
            CreateMaskedValue(parent, "PreLevelMoves", 405, 735, 130, 92, level.moveLimit.ToString(), 54);
            CreateMaskedValue(parent, "PreLevelGold", 595, 1210, 115, 62, reward.gold.ToString(), 34);
            CreateMaskedValue(parent, "PreLevelGems", 595, 1290, 115, 62, reward.gems.ToString(), 34);
        }

        void BuildStoryDialogueOverlay(RectTransform parent)
        {
            LocalizedStoryLine line = StoryService.CurrentLine;
            if (line == null)
            {
                Debug.LogError("Dialogue screen opened without an active localized story line.");
                return;
            }

            var speakerMask = new GameObject(
                "LocalizedSpeakerMask",
                typeof(RectTransform),
                typeof(Image)
            );
            speakerMask.transform.SetParent(parent, false);
            SetPixelRect(speakerMask.GetComponent<RectTransform>(), 292, 1088, 357, 78);
            Image speakerMaskImage = speakerMask.GetComponent<Image>();
            speakerMaskImage.color = new Color32(5, 25, 39, 255);
            speakerMaskImage.raycastTarget = false;

            Text speaker = CreateText(
                speakerMask.GetComponent<RectTransform>(),
                "LocalizedSpeaker",
                line.Speaker,
                37,
                TextAnchor.MiddleCenter
            );
            Stretch(speaker.rectTransform);
            speaker.fontStyle = FontStyle.Bold;
            speaker.color = new Color32(247, 207, 113, 255);

            var bodyMask = new GameObject(
                "LocalizedBodyMask",
                typeof(RectTransform),
                typeof(Image)
            );
            bodyMask.transform.SetParent(parent, false);
            SetPixelRect(bodyMask.GetComponent<RectTransform>(), 105, 1207, 731, 205);
            Image bodyMaskImage = bodyMask.GetComponent<Image>();
            bodyMaskImage.color = new Color32(224, 196, 143, 255);
            bodyMaskImage.raycastTarget = false;

            Text body = CreateText(
                bodyMask.GetComponent<RectTransform>(),
                "LocalizedBody",
                line.Text,
                31,
                TextAnchor.UpperLeft
            );
            body.rectTransform.anchorMin = Vector2.zero;
            body.rectTransform.anchorMax = Vector2.one;
            body.rectTransform.offsetMin = new Vector2(25, 13);
            body.rectTransform.offsetMax = new Vector2(-25, -13);
            body.color = new Color32(47, 28, 14, 255);
            body.horizontalOverflow = HorizontalWrapMode.Wrap;
            body.verticalOverflow = VerticalWrapMode.Truncate;
            body.lineSpacing = 1.05f;
        }

        void ShowOutcome(ScreenId id, LevelData level, string outcome)
        {
            activeOutcomeReaction = StoryService.GetLocalizedReaction(level, outcome, DemoFeatureState.LanguageCode);
            Show(id);
        }

        void RefreshOutcomeReactionLanguage()
        {
            if (!outcomeSpeaker || !outcomeBody || activeOutcomeReaction == null ||
                outcomeLanguage == DemoFeatureState.LanguageCode) return;
            var localized = StoryCatalog.LocalizeReaction(
                StoryCatalog.FindReaction(activeOutcomeReaction.LevelId, activeOutcomeReaction.Outcome),
                StoryCatalog.LoadDialogueLanguage(DemoFeatureState.LanguageCode));
            if (localized == null) return;
            activeOutcomeReaction = localized;
            outcomeSpeaker.text = localized.Speaker;
            outcomeBody.text = localized.Text;
            outcomeLanguage = DemoFeatureState.LanguageCode;
        }

        void BuildOutcomeReactionOverlay(RectTransform parent, ScreenId id)
        {
            if(id==ScreenId.Victory&&parent.GetComponentInChildren<ProductionVictoryView>()!=null)return;
            if(id==ScreenId.Fail&&parent.GetComponentInChildren<ProductionFailView>()!=null)return;
            LocalizedStoryReaction reaction = activeOutcomeReaction;
            if (reaction == null) return;

            float y = id == ScreenId.Victory ? 420f : 445f;
            var panel = new GameObject(
                "OutcomeReaction",
                typeof(RectTransform),
                typeof(Image),
                typeof(Outline)
            );
            panel.transform.SetParent(parent, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            SetPixelRect(panelRect, 90, y, 761, 118);

            Image background = panel.GetComponent<Image>();
            background.color = new Color32(5, 21, 36, 238);
            background.raycastTarget = false;

            Outline outline = panel.GetComponent<Outline>();
            outline.effectColor = new Color32(220, 164, 67, 235);
            outline.effectDistance = new Vector2(2f, -2f);
            outline.useGraphicAlpha = true;

            Text speaker = CreateText(
                panelRect,
                "ReactionSpeaker",
                reaction.Speaker,
                23,
                TextAnchor.MiddleLeft
            );
            speaker.rectTransform.anchorMin = new Vector2(0.035f, 0.64f);
            speaker.rectTransform.anchorMax = new Vector2(0.965f, 0.94f);
            speaker.rectTransform.offsetMin = Vector2.zero;
            speaker.rectTransform.offsetMax = Vector2.zero;
            speaker.fontStyle = FontStyle.Bold;
            speaker.color = new Color32(247, 207, 113, 255);

            Text body = CreateText(
                panelRect,
                "ReactionBody",
                reaction.Text,
                25,
                TextAnchor.UpperLeft
            );
            body.rectTransform.anchorMin = new Vector2(0.035f, 0.08f);
            body.rectTransform.anchorMax = new Vector2(0.965f, 0.64f);
            body.rectTransform.offsetMin = Vector2.zero;
            body.rectTransform.offsetMax = Vector2.zero;
            body.color = new Color32(244, 232, 203, 255);
            body.horizontalOverflow = HorizontalWrapMode.Wrap;
            body.verticalOverflow = VerticalWrapMode.Truncate;

            outcomeSpeaker = speaker; outcomeBody = body;
            outcomeLanguage = null;
            RefreshOutcomeReactionLanguage();

            MetaUiTransition reactionTransition =
                panel.AddComponent<MetaUiTransition>();
            reactionTransition.Initialize(
                motionPresenter,
                id == ScreenId.Victory
                    ? MetaUiMotionPreset.OutcomeVictory
                    : MetaUiMotionPreset.OutcomeFailure
            );
        }

        void BuildChapterCompleteOverlay(RectTransform parent)
        {
            var view=parent.GetComponentInChildren<ProductionChapterCompleteView>();
            if(view!=null)view.RefreshContent();
        }

        bool UsesTopHud(ScreenId id)
        {
            if((id==ScreenId.Events||id==ScreenId.EventDetail||id==ScreenId.IndividualRanking||id==ScreenId.TeamHub||id==ScreenId.TeamRanking||id==ScreenId.TeamChat||id==ScreenId.FriendsList||id==ScreenId.Mail||id==ScreenId.Profile||id==ScreenId.Inventory||id==ScreenId.Characters||id==ScreenId.Settings)&&ProductionMetaPageCutoverRegistry.IsCutover(id))return false; // Native live header.
            if (id == ScreenId.DailyReward && ProductionMetaPageCutoverRegistry.IsCutover(id)) return false; // Native live HUD.
            if (id == ScreenId.SpecialOffer && ProductionMetaPageCutoverRegistry.IsCutover(id)) return false; // Native live HUD.
            if (id == ScreenId.Shop && ProductionMetaPageCutoverRegistry.IsCutover(id)) return false; // Native live HUD.
            if (id == ScreenId.PreLevel) return false; // Native page owns its live HUD.
            if (id == ScreenId.ChapterMap) return false; // Native page owns its live HUD.
            if (id == ScreenId.WorldMap) return false; // The aligned page owns its live Home-style HUD.
            switch (id)
            {
                case ScreenId.WorldMap:
                case ScreenId.ChapterMap:
                case ScreenId.PreLevel:
                case ScreenId.Shop:
                case ScreenId.Events:
                case ScreenId.Profile:
                case ScreenId.Settings:
                case ScreenId.Characters:
                case ScreenId.Mail:
                case ScreenId.DailyReward:
                case ScreenId.EventDetail:
                case ScreenId.IndividualRanking:
                case ScreenId.TeamHub:
                case ScreenId.TeamRanking:
                case ScreenId.TeamChat:
                case ScreenId.FriendsList:
                case ScreenId.Inventory:
                case ScreenId.SpecialOffer:
                    return true;
                default:
                    return false;
            }
        }

        void BuildDynamicResourceHud(RectTransform parent)
        {
            var binderGo = new GameObject("DynamicHUD", typeof(RectTransform), typeof(DynamicHudBinder));
            binderGo.transform.SetParent(parent, false);
            Stretch(binderGo.GetComponent<RectTransform>());

            var binder = binderGo.GetComponent<DynamicHudBinder>();

            binder.Gold = CreateMaskedValue(binderGo.GetComponent<RectTransform>(), "Gold", 390, 42, 120, 55, "", 26);
            binder.Gems = CreateMaskedValue(binderGo.GetComponent<RectTransform>(), "Gems", 575, 42, 90, 55, "", 26);
            binder.Energy = CreateMaskedValue(binderGo.GetComponent<RectTransform>(), "Energy", 750, 42, 100, 55, "", 24);
            binder.Refresh();
        }

        Text CreateMaskedValue(RectTransform parent, string name, float x, float y, float w, float h, string value, int size)
        {
            var mask = new GameObject($"{name}_Mask", typeof(RectTransform), typeof(Image));
            mask.transform.SetParent(parent, false);
            SetPixelRect(mask.GetComponent<RectTransform>(), x, y, w, h);
            Image maskImage = mask.GetComponent<Image>();
            maskImage.color = new Color32(6, 20, 36, 245);
            maskImage.raycastTarget = false;

            var text = CreateText(mask.GetComponent<RectTransform>(), name, value, size, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform);
            text.color = new Color32(248, 224, 169, 255);
            return text;
        }

        Text CreateText(RectTransform parent, string name, string value, int size, TextAnchor anchor)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            var t = go.GetComponent<Text>();
            t.text = value;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null)
                t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            t.fontSize = size;
            t.alignment = anchor;
            t.color = Color.white;
            t.raycastTarget = false;
            return t;
        }

        public void Toast(string message)
        {
            if(current==ScreenId.Victory&&contentRoot!=null)
            {
                var victory=contentRoot.GetComponentInChildren<ProductionVictoryView>();
                if(victory!=null){victory.SetStatus(message);return;}
            }
            // Gameplay owns one styled status slot; other screens keep their existing presentation.
            if (current == ScreenId.Gameplay && contentRoot != null && !HasActiveModalBlocker)
            {
                var view = contentRoot.GetComponentInChildren<ProductionGameplayView>();
                if (view != null && view.Status != null && view.Status.HasRequiredBindings)
                {
                    view.Status.Show(message);
                    return;
                }
            }
            StartCoroutine(ToastRoutine(message));
        }

        IEnumerator ToastRoutine(string message)
        {
            var go = new GameObject("Toast", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(viewport, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.12f, 0.08f);
            rt.anchorMax = new Vector2(0.88f, 0.17f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image toastImage = go.GetComponent<Image>();
            toastImage.color = new Color32(5, 19, 34, 240);
            toastImage.raycastTarget = false;

            var text = CreateText(rt, "Text", message, 25, TextAnchor.MiddleCenter);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(20, 8);
            text.rectTransform.offsetMax = new Vector2(-20, -8);
            text.color = new Color32(246, 218, 161, 255);

            yield return new WaitForSecondsRealtime(2.2f);
            if (go != null) Destroy(go);
        }

        void DemoShortcut(
            RectTransform parent,
            string name,
            string label,
            float x,
            float yTop,
            float width,
            float height,
            Action action
        )
        {
            var go = new GameObject(
                "DemoShortcut_" + name,
                typeof(RectTransform),
                typeof(Image),
                typeof(AnimatedHotspot)
            );
            go.transform.SetParent(parent, false);
            RectTransform rt = go.GetComponent<RectTransform>();
            SetPixelRect(rt, x, yTop, width, height);

            Image image = go.GetComponent<Image>();
            image.color = new Color32(8, 30, 54, 245);
            image.raycastTarget = true;

            Text text = CreateText(rt, "Label", label, 19, TextAnchor.MiddleCenter);
            Stretch(text.rectTransform);
            text.color = new Color32(246, 218, 161, 255);

            MetaUiActionGate gate = parent.GetComponentInParent<MetaUiActionGate>();
            go.GetComponent<AnimatedHotspot>().Initialize(
                action,
                MetaUiControlRole.Standard,
                motionPresenter,
                gate
            );
        }

        void Hotspot(
            RectTransform parent,
            string name,
            float x,
            float yTop,
            float width,
            float height,
            Action action,
            MetaUiControlRole role = MetaUiControlRole.Standard,
            bool mapNodeFeedback = false
        )
        {
            var go = new GameObject(
                $"Hotspot_{name}",
                typeof(RectTransform),
                typeof(Image),
                typeof(AnimatedHotspot)
            );
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            SetPixelRect(rt, x, yTop, width, height);

            var image = go.GetComponent<Image>();
            image.raycastTarget = true;
            image.color = new Color(1f, 0.82f, 0.35f, 0.001f);

            if (role == MetaUiControlRole.Standard)
                role = ResolveControlRole(name);

            if (mapNodeFeedback)
            {
                MetaUiMapNodeFeedback feedback =
                    go.AddComponent<MetaUiMapNodeFeedback>();
                feedback.Initialize(motionPresenter);
            }

            MetaUiActionGate gate = role == MetaUiControlRole.Gameplay
                ? null
                : parent.GetComponentInParent<MetaUiActionGate>();
            go.GetComponent<AnimatedHotspot>().Initialize(
                action,
                role,
                motionPresenter,
                gate
            );
        }

        static MetaUiControlRole ResolveControlRole(string name)
        {
            switch (name)
            {
                case "Back":
                case "Close":
                case "Exit":
                    return MetaUiControlRole.Back;
                case "Claim":
                case "ClaimAll":
                case "DoubleReward":
                    return MetaUiControlRole.Reward;
                case "Play":
                case "StartLevel":
                case "Continue":
                case "Retry":
                case "Resume":
                case "Buy":
                case "WatchAd":
                case "Use":
                    return MetaUiControlRole.Primary;
                default:
                    return MetaUiControlRole.Standard;
            }
        }

        void SetPixelRect(RectTransform rt, float x, float yTop, float width, float height)
        {
            rt.anchorMin = PixelMin(x, yTop, width, height);
            rt.anchorMax = PixelMax(x, yTop, width, height);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        Vector2 PixelMin(float x, float yTop, float width, float height)
        {
            return new Vector2(x / DesignW, 1f - (yTop + height) / DesignH);
        }

        Vector2 PixelMax(float x, float yTop, float width, float height)
        {
            return new Vector2((x + width) / DesignW, 1f - yTop / DesignH);
        }
    }
}
