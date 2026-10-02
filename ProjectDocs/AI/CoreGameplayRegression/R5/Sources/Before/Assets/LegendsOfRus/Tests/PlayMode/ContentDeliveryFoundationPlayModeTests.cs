using NUnit.Framework;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class ContentDeliveryFoundationPlayModeTests
    {
        [Test]
        public void Activation_RemoteDeliveryAndCatalogUpdatesRemainDisabled()
        {
            ContentDeliveryActivationSnapshot activation =
                ContentDeliveryActivation.Load();

            Assert.IsFalse(activation.LocalAddressablesApproved);
            Assert.IsFalse(activation.RemoteCatalogApproved);
            Assert.IsFalse(activation.RemoteContentApproved);
            Assert.IsFalse(activation.CatalogUpdatesApproved);
            Assert.IsFalse(activation.FinalRuntimeApproved);
            Assert.IsFalse(activation.MayUseLocalAddressables);
            Assert.IsFalse(activation.AnyRemoteFeatureApproved);
            Assert.IsFalse(ContentDeliveryActivation.RemoteDeliveryCompiled);
            Assert.IsFalse(ContentDeliveryActivation.CatalogUpdatesCompiled);
        }

        [Test]
        public void Manifest_LoadsWithLegacyFallbackPlan()
        {
            Assert.IsTrue(
                RuntimeContentCatalog.TryLoad(out RuntimeContentCatalog catalog, out string reason),
                reason
            );
            Assert.IsNotNull(catalog);
            Assert.Greater(catalog.Count, 20);
            Assert.IsTrue(catalog.TryGet("lor.screen.home", out var home));
            Assert.AreEqual("DesignScreens/Home", home.legacyResourceKey);
            Assert.IsFalse(home.addressableReady);
        }
    }
}
