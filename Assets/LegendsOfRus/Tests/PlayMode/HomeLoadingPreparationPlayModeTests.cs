using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class HomeLoadingPreparationPlayModeTests
    {
        sealed class Request : IHomeLoadingRequest
        {
            public bool IsDone { get; set; }
            public float Progress { get; set; }
            public UnityEngine.Object Asset { get; set; }
        }

        GameObject prefab;
        Texture2D texture;

        [SetUp]
        public void SetUp()
        {
            prefab = new GameObject("HomeLoadingTestPrefab");
            texture = new Texture2D(1, 1);
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(prefab);
            UnityEngine.Object.DestroyImmediate(texture);
        }

        [Test]
        public void Progress_FollowsRequestsAndOnlyValidatedAssetsCompleteIt()
        {
            var requests = new List<Request>();
            using (var preparation = new HomeLoadingPreparation((key, type) =>
            {
                var request = new Request
                {
                    Progress = 0.5f,
                    Asset = type == typeof(GameObject) ? (UnityEngine.Object)prefab : texture
                };
                requests.Add(request);
                return request;
            }))
            {
                var progress = new List<float>();
                IEnumerator work = preparation.Run(progress.Add);
                Assert.That(work.MoveNext(), Is.True);
                Assert.That(preparation.CompletedResourceCount, Is.Zero);
                Assert.That(preparation.Progress01,
                    Is.EqualTo(0.5f / preparation.TotalResourceCount).Within(0.00001f));

                requests[0].Progress = 0.1f;
                Assert.That(work.MoveNext(), Is.True);
                Assert.That(preparation.Progress01,
                    Is.EqualTo(0.5f / preparation.TotalResourceCount).Within(0.00001f));
                requests[0].Progress = 1f;
                Assert.That(work.MoveNext(), Is.True);
                Assert.That(preparation.IsComplete, Is.False);
                Assert.That(preparation.CompletedResourceCount, Is.Zero);

                while (true)
                {
                    requests[requests.Count - 1].IsDone = true;
                    if (!work.MoveNext())
                        break;
                }
                Assert.That(preparation.IsComplete, Is.True);
                Assert.That(preparation.CompletedResourceCount, Is.EqualTo(preparation.TotalResourceCount));
                Assert.That(preparation.Progress01, Is.EqualTo(1f));
                for (int i = 1; i < progress.Count; i++)
                    Assert.That(progress[i], Is.GreaterThanOrEqualTo(progress[i - 1]));
            }
        }

        [Test]
        public void CancelledRequest_CannotReportCompletionOrNavigateAfterLateResult()
        {
            var request = new Request { Asset = prefab, Progress = 0.7f };
            using (var preparation = new HomeLoadingPreparation((key, type) => request))
            {
                var progress = new List<float>();
                IEnumerator work = preparation.Run(progress.Add);
                Assert.That(work.MoveNext(), Is.True);
                int beforeCancellation = progress.Count;
                preparation.Cancel();
                request.IsDone = true;
                request.Progress = 1f;
                Assert.That(work.MoveNext(), Is.False);
                Assert.That(progress.Count, Is.EqualTo(beforeCancellation));
                Assert.That(preparation.CompletedResourceCount, Is.Zero);
                Assert.That(preparation.TryBuildHome(() => Assert.Fail("Stale attempt navigated.")), Is.False);
            }
        }

        [Test]
        public void MissingResource_BlocksHomeAndNewAttemptCanRetrySuccessfully()
        {
            using (var failed = new HomeLoadingPreparation((key, type) =>
                new Request { IsDone = true, Asset = type == typeof(GameObject) ? prefab : null }))
            {
                Assert.That(failed.Run(null).MoveNext(), Is.False);
                Assert.That(failed.HasFailed, Is.True);
                Assert.That(failed.FailureReason, Does.StartWith("home_resource_unavailable:"));
                Assert.That(failed.IsComplete, Is.False);
                Assert.That(failed.Progress01, Is.LessThan(1f));
                Assert.That(failed.TryBuildHome(() => Assert.Fail("Failed load navigated.")), Is.False);
            }

            using (HomeLoadingPreparation retry = CompletedPreparation())
            {
                Assert.That(retry.IsComplete, Is.True);
                Assert.That(retry.HasFailed, Is.False);
                Assert.That(retry.TryBuildHome(() => { }), Is.True);
            }
        }

        [Test]
        public void RequestException_IsReportedAsFailureWithoutNavigating()
        {
            using (var preparation = new HomeLoadingPreparation((key, type) =>
                throw new InvalidOperationException("Injected resource failure")))
            {
                Assert.That(preparation.Run(null).MoveNext(), Is.False);
                Assert.That(preparation.HasFailed, Is.True);
                Assert.That(preparation.FailureReason, Does.Contain("InvalidOperationException"));
                Assert.That(preparation.TryBuildHome(() => Assert.Fail("Failed load navigated.")), Is.False);
            }
        }

        [Test]
        public void PreparedAssets_AreConsumedByHomeExactlyOnceAndReleasedAfterBuild()
        {
            using (HomeLoadingPreparation preparation = CompletedPreparation())
            {
                string homeKey = ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Home);
                Assert.That(HomeLoadingPreparation.LoadPreparedPrefab(homeKey), Is.Null);
                int builds = 0;
                Assert.That(preparation.TryBuildHome(() =>
                {
                    builds++;
                    Assert.That(HomeLoadingPreparation.LoadPreparedPrefab(homeKey), Is.SameAs(prefab));
                    foreach (string key in ProductionUiFix01Runtime.GetHomeTextureResourceKeys())
                        Assert.That(HomeLoadingPreparation.LoadPreparedTexture(key), Is.SameAs(texture), key);
                }), Is.True);
                Assert.That(HomeLoadingPreparation.LoadPreparedPrefab(homeKey), Is.Null);
                Assert.That(preparation.TryBuildHome(() => builds++), Is.False);
                Assert.That(builds, Is.EqualTo(1));
            }
        }

        [UnityTest]
        public IEnumerator ActualHomeResources_LoadAsTheTypesConsumedByCurrentHome()
        {
            using (var preparation = new HomeLoadingPreparation())
            {
                yield return preparation.Run(null);
                Assert.That(preparation.HasFailed, Is.False, preparation.FailureReason);
                Assert.That(preparation.IsComplete, Is.True);
                Assert.That(preparation.Progress01, Is.EqualTo(1f));
                preparation.TryBuildHome(() =>
                {
                    Assert.That(HomeLoadingPreparation.LoadPreparedPrefab(
                        ProductionCoreLoopPageRegistry.ResourceKey(ScreenId.Home)), Is.Not.Null);
                    foreach (string key in ProductionUiFix01Runtime.GetHomeTextureResourceKeys())
                        Assert.That(HomeLoadingPreparation.LoadPreparedTexture(key), Is.Not.Null, key);
                });
            }
        }

        [UnityTest]
        public IEnumerator LoadingBack_CancelsPreparationAndKeepsSplashAfterLateFrames()
        {
            ScreenRouter router = ScreenRouter.Instance;
            GameObject ownedRouter = null;
            if (router == null)
            {
                ownedRouter = new GameObject("LoadingCancellationRouter", typeof(ScreenRouter));
                router = ownedRouter.GetComponent<ScreenRouter>();
                router.Initialize();
            }
            ScreenId previous = router.CurrentScreen;
            try
            {
                router.Show(ScreenId.Loading);
                int loadingGeneration = router.RouteGeneration;
                router.NavigateBack();
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.SplashTitle));
                Assert.That(router.RouteGeneration, Is.EqualTo(loadingGeneration + 1));
                for (int i = 0; i < 4; i++)
                    yield return null;
                Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.SplashTitle));
                Assert.That(router.RouteGeneration, Is.EqualTo(loadingGeneration + 1));
            }
            finally
            {
                if (ownedRouter != null)
                    UnityEngine.Object.Destroy(ownedRouter);
                else
                    router.Show(previous == ScreenId.Loading ? ScreenId.SplashTitle : previous);
            }
        }

        HomeLoadingPreparation CompletedPreparation()
        {
            var preparation = new HomeLoadingPreparation((key, type) => new Request
            {
                IsDone = true,
                Progress = 1f,
                Asset = type == typeof(GameObject) ? (UnityEngine.Object)prefab : texture
            });
            Assert.That(preparation.Run(null).MoveNext(), Is.False);
            return preparation;
        }
    }
}
