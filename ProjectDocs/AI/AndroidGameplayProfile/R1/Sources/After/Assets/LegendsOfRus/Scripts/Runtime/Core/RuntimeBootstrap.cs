using UnityEngine;

namespace LegendsOfRus
{
    public static class RuntimeBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.orientation = ScreenOrientation.Portrait;

            ScreenRouter existingRouter =
                Object.FindAnyObjectByType<ScreenRouter>();
            ProductionFeedbackProvider existingProvider =
                ProductionFeedbackProvider.Instance ??
                Object.FindAnyObjectByType<ProductionFeedbackProvider>();

            GameObject root;
            if (existingProvider != null)
            {
                root = existingProvider.gameObject;
            }
            else
            {
                root = new GameObject(
                    existingRouter == null
                        ? "LegendsOfRus_Runtime"
                        : "LegendsOfRus_RuntimeServices"
                );
                Object.DontDestroyOnLoad(root);
            }

            RuntimeOptimizationService optimization =
                RuntimeOptimizationService.Instance ??
                FindOrAdd<RuntimeOptimizationService>(root);
            optimization.BeginBootstrap();

            GameState state = GameState.Instance ?? FindOrAdd<GameState>(root);
            optimization.MarkLocalStateReady();

            CoreLoopFeedbackService coreFeedback =
                CoreLoopFeedbackService.Instance ??
                FindOrAdd<CoreLoopFeedbackService>(root);
            MetaUiFeedbackService metaFeedback =
                MetaUiFeedbackService.Instance ??
                FindOrAdd<MetaUiFeedbackService>(root);
            FindOrAdd<MonetizationService>(root);
            FindOrAdd<ContentDeliveryService>(root);
            FindOrAdd<PrivacySupportService>(root);
            FindOrAdd<BackendAccountService>(root);
            FindOrAdd<ClosedTestQaService>(root);
            FindOrAdd<BalanceTelemetryReviewService>(root);
            // Runtime analytics is consent-gated even if a future trusted
            // provider is installed before this scene-load hook.
            PrivacyAwareAnalytics.TryBeginSession();
            FindOrAdd<QACaptureService>(root);

            ProductionFeedbackProvider provider =
                existingProvider != null
                    ? existingProvider
                    : root.AddComponent<ProductionFeedbackProvider>();
            provider.Initialize(coreFeedback, metaFeedback, state);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            FindOrAdd<RuntimePerformanceProbe>(root);
#if UNITY_EDITOR || UNITY_ANDROID
            if (DevelopmentGameplayProfile.Active)
                FindOrAdd<DevelopmentGameplayProfileBadge>(root);
#endif
#endif

            if (existingRouter != null)
            {
                optimization.MarkUiConstructionReady();
                optimization.CompleteBootstrap();
                return;
            }

            var router = root.AddComponent<ScreenRouter>();
            router.Initialize();
            optimization.MarkUiConstructionReady();
            router.Show(ScreenId.SplashTitle);
            optimization.CompleteBootstrap();
        }

        static T FindOrAdd<T>(GameObject host) where T : Component
        {
            T existing = Object.FindAnyObjectByType<T>();
            return existing != null ? existing : host.AddComponent<T>();
        }
    }
}
