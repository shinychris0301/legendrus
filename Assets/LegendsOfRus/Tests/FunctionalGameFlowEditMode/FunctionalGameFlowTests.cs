using NUnit.Framework;

namespace LegendsOfRus.Tests.FunctionalGameFlow
{
    public sealed class FunctionalGameFlowTests
    {
        [TestCase(ScreenId.WorldMap, ScreenId.Home)]
        [TestCase(ScreenId.ChapterMap, ScreenId.WorldMap)]
        [TestCase(ScreenId.PreLevel, ScreenId.ChapterMap)]
        [TestCase(ScreenId.EventDetail, ScreenId.Events)]
        [TestCase(ScreenId.TeamRanking, ScreenId.TeamHub)]
        [TestCase(ScreenId.TeamChat, ScreenId.TeamHub)]
        [TestCase(ScreenId.FriendsList, ScreenId.TeamHub)]
        [TestCase(ScreenId.SpecialOffer, ScreenId.Shop)]
        [TestCase(ScreenId.LanguageSelect, ScreenId.Settings)]
        [TestCase(ScreenId.Login, ScreenId.Profile)]
        [TestCase(ScreenId.Inventory, ScreenId.Home)]
        public void BackPolicy_UsesExpectedParent(ScreenId source, ScreenId expected)
        {
            ScreenId actual;
            Assert.IsTrue(ScreenNavigationPolicy.TryGetBackDestination(source, out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BackPolicy_HomeHasNoParent()
        {
            ScreenId ignored;
            Assert.IsFalse(ScreenNavigationPolicy.TryGetBackDestination(ScreenId.Home, out ignored));
        }

        [TestCase(ScreenId.SplashTitle)]
        [TestCase(ScreenId.EventDetail)]
        [TestCase(ScreenId.IndividualRanking)]
        [TestCase(ScreenId.TeamHub)]
        [TestCase(ScreenId.TeamRanking)]
        [TestCase(ScreenId.TeamChat)]
        [TestCase(ScreenId.FriendsList)]
        [TestCase(ScreenId.Inventory)]
        [TestCase(ScreenId.SpecialOffer)]
        [TestCase(ScreenId.LanguageSelect)]
        [TestCase(ScreenId.Login)]
        [TestCase(ScreenId.TutorialOverlay)]
        [TestCase(ScreenId.OfflineState)]
        [TestCase(ScreenId.MaintenanceState)]
        [TestCase(ScreenId.EmptyErrorState)]
        public void DemoRoute_IsRenderable(ScreenId id)
        {
            Assert.IsTrue(DemoFunctionalPageRuntime.IsDemoRoute(id));
        }

        [TestCase(ScreenId.Home)]
        [TestCase(ScreenId.WorldMap)]
        [TestCase(ScreenId.ChapterMap)]
        [TestCase(ScreenId.Gameplay)]
        [TestCase(ScreenId.Shop)]
        public void ExistingRoute_IsNotMistakenForDemoOnlyPage(ScreenId id)
        {
            Assert.IsFalse(DemoFunctionalPageRuntime.IsDemoRoute(id));
        }

        [Test]
        public void LegacyScreenIdNumericValuesRemainStable()
        {
            Assert.AreEqual(0, (int)ScreenId.Loading);
            Assert.AreEqual(1, (int)ScreenId.Home);
            Assert.AreEqual(2, (int)ScreenId.WorldMap);
            Assert.AreEqual(3, (int)ScreenId.ChapterMap);
            Assert.AreEqual(21, (int)ScreenId.ChapterComplete);
        }

        [Test]
        public void NewScreenIdsAreAppendOnly()
        {
            Assert.AreEqual(22, (int)ScreenId.SplashTitle);
            Assert.AreEqual(23, (int)ScreenId.EventDetail);
            Assert.AreEqual(36, (int)ScreenId.EmptyErrorState);
        }

        [Test]
        public void DemoFeatureState_IsSessionSafeAndDoesNotRepeatClaims()
        {
            DemoFeatureState.ResetSession();
            Assert.IsTrue(DemoFeatureState.TryClaimEventReward());
            Assert.IsFalse(DemoFeatureState.TryClaimEventReward());
            Assert.IsTrue(DemoFeatureState.EventRewardClaimed);
        }

        [Test]
        public void DemoFeatureState_FriendEnergyRequestIsOncePerSession()
        {
            DemoFeatureState.ResetSession();
            Assert.IsTrue(DemoFeatureState.TryRequestFriendEnergy());
            Assert.IsFalse(DemoFeatureState.TryRequestFriendEnergy());
        }

        [Test]
        public void DemoFeatureState_LanguageIsConstrained()
        {
            DemoFeatureState.ResetSession();
            DemoFeatureState.SetLanguage("en");
            Assert.AreEqual("en", DemoFeatureState.LanguageCode);
            DemoFeatureState.SetLanguage("unknown");
            Assert.AreEqual("ru", DemoFeatureState.LanguageCode);
        }

        [Test]
        public void DemoFeatureState_GuestAndChatAreLocalSessionState()
        {
            DemoFeatureState.ResetSession();
            DemoFeatureState.SignInGuest();
            Assert.IsTrue(DemoFeatureState.GuestSignedIn);
            Assert.AreEqual(1, DemoFeatureState.RecordTeamMessage());
            Assert.AreEqual(2, DemoFeatureState.RecordTeamMessage());
        }
    }
}
