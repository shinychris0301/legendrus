using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>A resource operation, kept injectable so failure and cancellation can be tested.</summary>
    public interface IHomeLoadingRequest
    {
        bool IsDone { get; }
        float Progress { get; }
        UnityEngine.Object Asset { get; }
    }

    /// <summary>
    /// Loads precisely the prefab and textures used by the current Home composition.
    /// Progress is the equal-weight fraction of these actual resource operations, not time,
    /// bytes downloaded, or the bootstrap checkpoints that already finished before Splash.
    /// </summary>
    public sealed class HomeLoadingPreparation : IDisposable
    {
        sealed class ResourceOperation : IHomeLoadingRequest
        {
            readonly ResourceRequest request;
            public ResourceOperation(ResourceRequest request) { this.request = request; }
            public bool IsDone => request.isDone;
            public float Progress => request.progress;
            public UnityEngine.Object Asset => request.asset;
        }

        static HomeLoadingPreparation buildingHome;
        readonly Dictionary<string, UnityEngine.Object> assets =
            new Dictionary<string, UnityEngine.Object>(StringComparer.Ordinal);
        readonly Func<string, Type, IHomeLoadingRequest> beginLoad;
        readonly List<string> keys = new List<string>();
        bool started;
        bool consumed;

        public int TotalResourceCount => keys.Count;
        public int CompletedResourceCount { get; private set; }
        public float Progress01 { get; private set; }
        public bool IsComplete { get; private set; }
        public bool IsCancelled { get; private set; }
        public string FailureReason { get; private set; } = string.Empty;
        public bool HasFailed => !string.IsNullOrEmpty(FailureReason);

        public HomeLoadingPreparation(Func<string, Type, IHomeLoadingRequest> beginLoad = null)
        {
            this.beginLoad = beginLoad ?? BeginResourceLoad;
            keys.Add(ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Home));
            IReadOnlyList<string> textures = ProductionUiFix01Runtime.GetHomeTextureResourceKeys();
            for (int i = 0; i < textures.Count; i++)
                if (!keys.Contains(textures[i]))
                    keys.Add(textures[i]);
        }

        public IEnumerator Run(Action<float> progressChanged)
        {
            if (started || IsCancelled)
                yield break;
            started = true;
            ReportProgress(0f, progressChanged);

            for (int i = 0; i < keys.Count; i++)
            {
                if (IsCancelled)
                    yield break;

                string key = keys[i];
                Type assetType = i == 0 ? typeof(GameObject) : typeof(Texture2D);
                IHomeLoadingRequest operation = null;
                try
                {
                    operation = beginLoad(key, assetType);
                }
                catch (Exception exception)
                {
                    FailureReason = "home_resource_request_failed: " + key + " (" +
                        exception.GetType().Name + ")";
                }
                if (operation == null)
                {
                    if (!HasFailed)
                        FailureReason = "home_resource_request_missing: " + key;
                    yield break;
                }

                while (!operation.IsDone)
                {
                    if (IsCancelled)
                        yield break;
                    // An operation reporting 1 before its asset is available must not
                    // complete the page. Only a validated last result may report 100%.
                    float operationProgress = Mathf.Min(Mathf.Clamp01(operation.Progress), 0.999f);
                    ReportProgress((CompletedResourceCount + operationProgress) / keys.Count, progressChanged);
                    yield return null;
                }
                if (IsCancelled)
                    yield break;

                UnityEngine.Object asset = operation.Asset;
                if (asset == null || !assetType.IsInstanceOfType(asset))
                {
                    FailureReason = "home_resource_unavailable: " + key;
                    yield break;
                }
                assets.Add(key, asset);
                CompletedResourceCount++;
                if (CompletedResourceCount == keys.Count)
                    IsComplete = true;
                ReportProgress((float)CompletedResourceCount / keys.Count, progressChanged);
            }
        }

        /// <summary>
        /// The existing prefab/bootstrap and texture loader consume the held results during
        /// the synchronous Home route. The scope cannot be reused for a second navigation.
        /// </summary>
        public bool TryBuildHome(Action buildHome)
        {
            if (!IsComplete || IsCancelled || HasFailed || consumed || buildHome == null)
                return false;
            consumed = true;
            HomeLoadingPreparation previous = buildingHome;
            buildingHome = this;
            try
            {
                buildHome();
            }
            finally
            {
                buildingHome = previous;
                assets.Clear();
            }
            return true;
        }

        public static Texture2D LoadPreparedTexture(string key)
        {
            return PreparedAsset(key) as Texture2D;
        }

        public static GameObject LoadPreparedPrefab(string key)
        {
            return PreparedAsset(key) as GameObject;
        }

        public void Cancel()
        {
            IsCancelled = true;
            assets.Clear();
            // Resources requests have no cancellation API. Dropping our references stops
            // consumption/navigation; Unity owns any in-flight completion. UnloadAsset here
            // would invalidate textures also used by existing pages and the shared sprite cache.
        }

        public void Dispose() { Cancel(); }

        static UnityEngine.Object PreparedAsset(string key)
        {
            UnityEngine.Object asset;
            return buildingHome != null && buildingHome.assets.TryGetValue(key, out asset)
                ? asset : null;
        }

        static IHomeLoadingRequest BeginResourceLoad(string key, Type type)
        {
            return new ResourceOperation(Resources.LoadAsync(key, type));
        }

        void ReportProgress(float progress, Action<float> changed)
        {
            // A request can report a lower estimate later; the count of validated resources
            // never decreases, and the display must not jump backwards within this attempt.
            Progress01 = Mathf.Max(Progress01, Mathf.Clamp01(progress));
            changed?.Invoke(Progress01);
        }
    }
}
