using LegendsOfRus.ContentDelivery;
using NUnit.Framework;

namespace LegendsOfRus.Tests.ContentDelivery
{
    public sealed class ContentDeliveryCoreEditModeTests
    {
        [TestCase("lor.screen.home")]
        [TestCase("lor.chapter.01.data-levels")]
        [TestCase("lor.validation.local-probe")]
        public void StableAddresses_AreAccepted(string address)
        {
            Assert.IsTrue(ContentDeliveryPolicy.IsAddressValid(address));
        }

        [TestCase("")]
        [TestCase("LOR.screen.Home")]
        [TestCase("lor screen home")]
        [TestCase("/lor/screen/home")]
        public void UnsafeAddresses_AreRejected(string address)
        {
            Assert.IsFalse(ContentDeliveryPolicy.IsAddressValid(address));
        }

        [Test]
        public void CatalogValidator_AcceptsLegacyFallbackAndValidationProbe()
        {
            var entries = new[]
            {
                new ContentCatalogEntry
                {
                    address = "lor.screen.home",
                    legacyResourceKey = "DesignScreens/Home",
                    group = ContentDeliveryPolicy.SharedDeferredGroup,
                    labels = new[] { ContentDeliveryPolicy.LocalLabel, ContentDeliveryPolicy.ScreenLabel },
                    tier = ContentDeliveryTier.SharedDeferred,
                    kind = ContentAssetKind.Sprite,
                    addressableReady = false
                },
                new ContentCatalogEntry
                {
                    address = ContentDeliveryPolicy.ValidationProbeAddress,
                    legacyResourceKey = string.Empty,
                    group = ContentDeliveryPolicy.ValidationLocalGroup,
                    labels = new[] { ContentDeliveryPolicy.LocalLabel, ContentDeliveryPolicy.ValidationLabel },
                    tier = ContentDeliveryTier.ValidationLocal,
                    kind = ContentAssetKind.Text,
                    addressableReady = true
                }
            };

            ContentCatalogValidationResult result =
                ContentDeliveryCatalogValidator.Validate(entries);
            Assert.IsTrue(result.IsValid, result.Reason);
        }

        [Test]
        public void CatalogValidator_RejectsDuplicateAddresses()
        {
            var entry = new ContentCatalogEntry
            {
                address = "lor.screen.home",
                legacyResourceKey = "DesignScreens/Home",
                group = ContentDeliveryPolicy.SharedDeferredGroup,
                labels = new[] { ContentDeliveryPolicy.LocalLabel },
                tier = ContentDeliveryTier.SharedDeferred,
                kind = ContentAssetKind.Sprite,
                addressableReady = false
            };

            ContentCatalogValidationResult result =
                ContentDeliveryCatalogValidator.Validate(new[] { entry, entry });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("address_duplicate", result.Reason);
        }

        [Test]
        public void CatalogValidator_RejectsPrematureProductionAddressableEntry()
        {
            var entry = new ContentCatalogEntry
            {
                address = "lor.screen.home",
                legacyResourceKey = "DesignScreens/Home",
                group = ContentDeliveryPolicy.SharedDeferredGroup,
                labels = new[] { ContentDeliveryPolicy.LocalLabel },
                tier = ContentDeliveryTier.SharedDeferred,
                kind = ContentAssetKind.Sprite,
                addressableReady = true
            };

            ContentCatalogValidationResult result =
                ContentDeliveryCatalogValidator.Validate(new[] { entry });
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("production_entry_prematurely_addressable", result.Reason);
        }
    }
}
