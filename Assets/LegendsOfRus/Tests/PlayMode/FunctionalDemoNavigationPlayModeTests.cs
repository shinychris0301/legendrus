using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class FunctionalDemoNavigationPlayModeTests
    {
        GameObject ownedRouterObject;
        ScreenRouter router;
        bool reusedRouter;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            router = ScreenRouter.Instance;
            if (router == null)
            {
                ownedRouterObject = new GameObject(
                    "Step20FunctionalQaRouter",
                    typeof(ScreenRouter)
                );
                router = ownedRouterObject.GetComponent<ScreenRouter>();
                router.Initialize();
            }
            else
            {
                reusedRouter = true;
            }

            DemoFeatureState.ResetSession();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            DemoFeatureState.ResetSession();

            if (ownedRouterObject != null)
                Object.Destroy(ownedRouterObject);
            else if (reusedRouter && router != null)
                router.Show(ScreenId.Home);

            router = null;
            reusedRouter = false;
            yield return null;
        }

        [UnityTest]
        public IEnumerator MapBackHotspots_FollowChapterMapWorldMapHomeHierarchy()
        {
            router.Show(ScreenId.ChapterMap);
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap));

            GameObject chapterMap = GameObject.Find("Screen_ChapterMap");
            Assert.That(chapterMap, Is.Not.Null);
            AnimatedHotspot chapterBack = FindHotspot(chapterMap, "Hotspot_Back");
            Assert.That(chapterBack, Is.Not.Null);
            Assert.That(chapterBack.Role, Is.EqualTo(MetaUiControlRole.Back));

            Activate(chapterBack, 501);
            TickRouter(8);
            Assert.That(
                router.CurrentScreen,
                Is.EqualTo(ScreenId.WorldMap),
                "Visible Chapter Map Back must return to World Map."
            );

            GameObject worldMap = GameObject.Find("Screen_WorldMap");
            Assert.That(worldMap, Is.Not.Null);
            AnimatedHotspot worldBack = FindHotspot(worldMap, "Hotspot_Back");
            Assert.That(worldBack, Is.Not.Null);
            Assert.That(worldBack.Role, Is.EqualTo(MetaUiControlRole.Back));

            Canvas.ForceUpdateCanvases();
            Rect worldBackRect = worldBack.GetComponent<RectTransform>().rect;
            Assert.That(worldBackRect.width, Is.GreaterThanOrEqualTo(180f));
            Assert.That(worldBackRect.height, Is.GreaterThanOrEqualTo(180f));

            Activate(worldBack, 502);
            TickRouter(8);
            Assert.That(
                router.CurrentScreen,
                Is.EqualTo(ScreenId.Home),
                "Visible World Map Back must return to Home."
            );

            yield return null;
        }

        [UnityTest]
        public IEnumerator PreLevelVisibleBackHotspot_ReturnsToChapterMap()
        {
            router.Show(ScreenId.PreLevel);
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.PreLevel));

            GameObject preLevel = GameObject.Find("Screen_PreLevel");
            Assert.That(preLevel, Is.Not.Null);
            AnimatedHotspot back = FindHotspot(preLevel, "Hotspot_Back");
            Assert.That(back, Is.Not.Null, "Pre-Level visible Back icon must have an interaction hotspot.");
            Assert.That(back.Role, Is.EqualTo(MetaUiControlRole.Back));

            Activate(back, 503);
            TickRouter(8);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.ChapterMap));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ExtendedDemoRoutes_RenderWithAtLeastOneInteraction()
        {
            ScreenId[] routes =
            {
                ScreenId.SplashTitle,
                ScreenId.EventDetail,
                ScreenId.IndividualRanking,
                ScreenId.TeamHub,
                ScreenId.TeamRanking,
                ScreenId.TeamChat,
                ScreenId.FriendsList,
                ScreenId.Inventory,
                ScreenId.SpecialOffer,
                ScreenId.LanguageSelect,
                ScreenId.Login,
                ScreenId.TutorialOverlay,
                ScreenId.OfflineState,
                ScreenId.MaintenanceState,
                ScreenId.EmptyErrorState
            };

            for (int i = 0; i < routes.Length; i++)
            {
                ScreenId id = routes[i];
                router.Show(id);
                TickRouter(12);

                Assert.That(router.CurrentScreen, Is.EqualTo(id), "route " + id);
                GameObject root = GameObject.Find("Screen_" + id);
                Assert.That(root, Is.Not.Null, "root " + id);

                CanvasGroup group = root.GetComponent<CanvasGroup>();
                Assert.That(group, Is.Not.Null, "canvas group " + id);
                Assert.That(group.alpha, Is.EqualTo(1f).Within(0.001f), "visible " + id);
                Assert.That(group.blocksRaycasts, Is.True, "raycasts " + id);

                AnimatedHotspot[] hotspots = root.GetComponentsInChildren<AnimatedHotspot>(true);
                Assert.That(hotspots.Length, Is.GreaterThan(0), "interaction " + id);
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator NavigateBack_FriendsListReturnsToTeamHub()
        {
            router.Show(ScreenId.FriendsList);
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.FriendsList));

            router.NavigateBack();
            TickRouter(12);
            Assert.That(router.CurrentScreen, Is.EqualTo(ScreenId.TeamHub));
            yield return null;
        }

        void TickRouter(int count)
        {
            Assert.That(router, Is.Not.Null);
            Assert.That(router.MotionPresenter, Is.Not.Null);
            for (int i = 0; i < count; i++)
                router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);
        }

        static void Activate(AnimatedHotspot hotspot, int pointerId)
        {
            var pointer = new PointerEventData(EventSystem.current)
            {
                pointerId = pointerId,
                position = new Vector2(100f, 100f)
            };
            hotspot.OnPointerDown(pointer);
            hotspot.OnPointerUp(pointer);
            hotspot.OnPointerClick(pointer);
        }

        static AnimatedHotspot FindHotspot(GameObject root, string name)
        {
            AnimatedHotspot[] hotspots = root.GetComponentsInChildren<AnimatedHotspot>(true);
            for (int i = 0; i < hotspots.Length; i++)
                if (hotspots[i].gameObject.name == name)
                    return hotspots[i];
            return null;
        }
    }
}
