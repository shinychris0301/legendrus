using System;
using System.Reflection;
using LegendsOfRus.CommercialCore.Consent;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Commercial
{
    public sealed class ConsentPolicyEditModeTests
    {
        [Test]
        public void RestrictedDefault_IsValidIdentifierFreeAndNonGranting()
        {
            ConsentSnapshot snapshot = ConsentSnapshot.Restricted;
            ConsentValidationResult validation = ConsentSnapshotValidator.Validate(snapshot);

            Assert.That(validation.IsValid, Is.True, validation.ReasonCode);
            Assert.That(snapshot.SchemaVersion, Is.EqualTo(ConsentSnapshot.CurrentSchemaVersion));
            Assert.That(snapshot.PolicyVersion, Is.EqualTo(ConsentSnapshot.CurrentPolicyVersion));
            Assert.That(snapshot.Audience, Is.EqualTo(ConsentAudience.Unknown));
            Assert.That(snapshot.Analytics, Is.EqualTo(ConsentDecision.Unknown));
            Assert.That(snapshot.RewardedAdvertising, Is.EqualTo(ConsentDecision.Unknown));

            PropertyInfo[] properties = typeof(ConsentSnapshot).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                Assert.That(properties[i].CanWrite, Is.False, properties[i].Name);

            string[] forbiddenFragments =
            {
                "name", "email", "birth", "date", "age", "account",
                "device", "advertisingid", "userid", "timestamp"
            };
            for (int i = 0; i < properties.Length; i++)
            {
                string normalized = properties[i].Name.Replace("_", string.Empty).ToLowerInvariant();
                for (int j = 0; j < forbiddenFragments.Length; j++)
                    Assert.That(normalized, Does.Not.Contain(forbiddenFragments[j]));
            }
        }

        [Test]
        public void MissingFutureAndCorruptSnapshots_FailWithStableReasons()
        {
            AssertInvalid(null, ConsentReasonCodes.SnapshotMissing);
            AssertInvalid(
                new ConsentSnapshot(
                    ConsentSnapshot.CurrentSchemaVersion + 1,
                    ConsentSnapshot.CurrentPolicyVersion,
                    ConsentAudience.Unknown,
                    ConsentDecision.Unknown,
                    ConsentDecision.Unknown
                ),
                ConsentReasonCodes.SchemaUnsupported
            );
            AssertInvalid(
                new ConsentSnapshot(
                    ConsentSnapshot.CurrentSchemaVersion,
                    ConsentSnapshot.CurrentPolicyVersion + 1,
                    ConsentAudience.Unknown,
                    ConsentDecision.Unknown,
                    ConsentDecision.Unknown
                ),
                ConsentReasonCodes.PolicyUnsupported
            );
            AssertInvalid(
                new ConsentSnapshot(
                    ConsentSnapshot.CurrentSchemaVersion,
                    ConsentSnapshot.CurrentPolicyVersion,
                    (ConsentAudience)999,
                    ConsentDecision.Unknown,
                    ConsentDecision.Unknown
                ),
                ConsentReasonCodes.AudienceInvalid
            );
            AssertInvalid(
                new ConsentSnapshot(
                    ConsentSnapshot.CurrentSchemaVersion,
                    ConsentSnapshot.CurrentPolicyVersion,
                    ConsentAudience.GeneralAudience,
                    (ConsentDecision)999,
                    ConsentDecision.Unknown
                ),
                ConsentReasonCodes.AnalyticsDecisionInvalid
            );
            AssertInvalid(
                new ConsentSnapshot(
                    ConsentSnapshot.CurrentSchemaVersion,
                    ConsentSnapshot.CurrentPolicyVersion,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Unknown,
                    (ConsentDecision)999
                ),
                ConsentReasonCodes.RewardedAdvertisingDecisionInvalid
            );
        }

        [TestCase(ConsentAudience.Unknown)]
        [TestCase(ConsentAudience.Child)]
        public void RestrictedAudience_CannotContainAnyGrant(ConsentAudience audience)
        {
            var analyticsGrant = new ConsentSnapshot(
                ConsentSnapshot.CurrentSchemaVersion,
                ConsentSnapshot.CurrentPolicyVersion,
                audience,
                ConsentDecision.Granted,
                ConsentDecision.Denied
            );
            var adGrant = new ConsentSnapshot(
                ConsentSnapshot.CurrentSchemaVersion,
                ConsentSnapshot.CurrentPolicyVersion,
                audience,
                ConsentDecision.Denied,
                ConsentDecision.Granted
            );

            AssertInvalid(analyticsGrant, ConsentReasonCodes.RestrictedAudienceGrant);
            AssertInvalid(adGrant, ConsentReasonCodes.RestrictedAudienceGrant);
        }

        [Test]
        public void DisabledProvider_AlwaysReturnsNonNullRestrictedSnapshot()
        {
            ConsentProviderResult result = DisabledConsentProvider.Instance.GetConsent();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsReady, Is.False);
            Assert.That(result.Status, Is.EqualTo(ConsentProviderStatus.Disabled));
            Assert.That(result.ReasonCode, Is.EqualTo(ConsentReasonCodes.ProviderDisabled));
            Assert.That(result.Snapshot, Is.SameAs(ConsentSnapshot.Restricted));
        }

        [Test]
        public void ReadyFactory_InvalidSnapshotFailsClosed()
        {
            ConsentProviderResult result = ConsentProviderResult.Ready(
                new ConsentSnapshot(
                    2,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Granted,
                    ConsentDecision.Granted
                )
            );

            Assert.That(result.IsReady, Is.False);
            Assert.That(result.Status, Is.EqualTo(ConsentProviderStatus.InvalidSnapshot));
            Assert.That(result.ReasonCode, Is.EqualTo(ConsentReasonCodes.SchemaUnsupported));
            Assert.That(result.Snapshot, Is.SameAs(ConsentSnapshot.Restricted));
        }

        [Test]
        public void ExternalActivationDisabled_ShortCircuitsBeforeProviderAccess()
        {
            var provider = new CountingProvider(GeneralGranted());

            ConsentEligibilityResult analytics = ConsentEligibility.ForAnalytics(
                provider,
                false
            );
            ConsentEligibilityResult advertising =
                ConsentEligibility.ForRewardedAdvertising(provider, false);

            Assert.That(analytics.IsEligible, Is.False);
            Assert.That(analytics.ReasonCode, Is.EqualTo(ConsentReasonCodes.ExternalActivationDisabled));
            Assert.That(advertising.IsEligible, Is.False);
            Assert.That(advertising.ReasonCode, Is.EqualTo(ConsentReasonCodes.ExternalActivationDisabled));
            Assert.That(provider.Calls, Is.Zero);
        }

        [Test]
        public void MissingNullAndThrowingProviders_FailWithoutExceptionEscape()
        {
            ConsentEligibilityResult missing = ConsentEligibility.ForAnalytics(
                null,
                true
            );
            ConsentEligibilityResult nullResult = ConsentEligibility.ForAnalytics(
                new NullProvider(),
                true
            );
            ConsentEligibilityResult throwing = ConsentEligibility.ForAnalytics(
                new ThrowingProvider(),
                true
            );

            Assert.That(missing.ReasonCode, Is.EqualTo(ConsentReasonCodes.ProviderMissing));
            Assert.That(nullResult.ReasonCode, Is.EqualTo(ConsentReasonCodes.ProviderFailed));
            Assert.That(throwing.ReasonCode, Is.EqualTo(ConsentReasonCodes.ProviderFailed));
            Assert.That(missing.IsEligible, Is.False);
            Assert.That(nullResult.IsEligible, Is.False);
            Assert.That(throwing.IsEligible, Is.False);
        }

        [TestCase(ConsentAudience.Unknown)]
        [TestCase(ConsentAudience.Child)]
        public void ValidRestrictedAudience_RemainsIneligibleForBothPurposes(
            ConsentAudience audience
        )
        {
            var snapshot = new ConsentSnapshot(
                ConsentSnapshot.CurrentSchemaVersion,
                ConsentSnapshot.CurrentPolicyVersion,
                audience,
                ConsentDecision.Denied,
                ConsentDecision.Denied
            );
            var provider = new InMemoryDevelopmentConsentProvider(snapshot);

            ConsentEligibilityResult analytics = ConsentEligibility.ForAnalytics(
                provider,
                true
            );
            ConsentEligibilityResult advertising =
                ConsentEligibility.ForRewardedAdvertising(provider, true);

            Assert.That(analytics.IsEligible, Is.False);
            Assert.That(advertising.IsEligible, Is.False);
            Assert.That(analytics.ReasonCode, Is.EqualTo(ConsentReasonCodes.AudienceRestricted));
            Assert.That(advertising.ReasonCode, Is.EqualTo(ConsentReasonCodes.AudienceRestricted));
        }

        [Test]
        public void GeneralAudienceUnknownAndDeniedDecisions_ArePurposeSpecific()
        {
            var unknown = new InMemoryDevelopmentConsentProvider(
                new ConsentSnapshot(
                    1,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Unknown,
                    ConsentDecision.Unknown
                )
            );
            var denied = new InMemoryDevelopmentConsentProvider(
                new ConsentSnapshot(
                    1,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Denied,
                    ConsentDecision.Denied
                )
            );

            Assert.That(
                ConsentEligibility.ForAnalytics(unknown, true).ReasonCode,
                Is.EqualTo(ConsentReasonCodes.AnalyticsDecisionRequired)
            );
            Assert.That(
                ConsentEligibility.ForRewardedAdvertising(unknown, true).ReasonCode,
                Is.EqualTo(ConsentReasonCodes.RewardedAdvertisingDecisionRequired)
            );
            Assert.That(
                ConsentEligibility.ForAnalytics(denied, true).ReasonCode,
                Is.EqualTo(ConsentReasonCodes.AnalyticsDenied)
            );
            Assert.That(
                ConsentEligibility.ForRewardedAdvertising(denied, true).ReasonCode,
                Is.EqualTo(ConsentReasonCodes.RewardedAdvertisingDenied)
            );
        }

        [Test]
        public void GeneralAudienceGrants_AreIndependentByPurpose()
        {
            var analyticsOnly = new InMemoryDevelopmentConsentProvider(
                new ConsentSnapshot(
                    1,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Granted,
                    ConsentDecision.Denied
                )
            );
            var adsOnly = new InMemoryDevelopmentConsentProvider(
                new ConsentSnapshot(
                    1,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Denied,
                    ConsentDecision.Granted
                )
            );

            Assert.That(ConsentEligibility.ForAnalytics(analyticsOnly, true).IsEligible, Is.True);
            Assert.That(ConsentEligibility.ForRewardedAdvertising(analyticsOnly, true).IsEligible, Is.False);
            Assert.That(ConsentEligibility.ForAnalytics(adsOnly, true).IsEligible, Is.False);
            Assert.That(ConsentEligibility.ForRewardedAdvertising(adsOnly, true).IsEligible, Is.True);
        }

        [Test]
        public void GeneralAudienceDoubleGrant_RequiresAndPassesSeparateActivation()
        {
            var provider = new InMemoryDevelopmentConsentProvider(GeneralGranted());

            ConsentEligibilityResult analytics = ConsentEligibility.ForAnalytics(
                provider,
                true
            );
            ConsentEligibilityResult advertising =
                ConsentEligibility.ForRewardedAdvertising(provider, true);

            Assert.That(analytics.IsEligible, Is.True, analytics.ReasonCode);
            Assert.That(advertising.IsEligible, Is.True, advertising.ReasonCode);
            Assert.That(analytics.ReasonCode, Is.EqualTo(ConsentReasonCodes.Eligible));
            Assert.That(advertising.ReasonCode, Is.EqualTo(ConsentReasonCodes.Eligible));
        }

        [Test]
        public void InvalidDevelopmentReplacement_FailsClosedAndReplacesPriorGrant()
        {
            var provider = new InMemoryDevelopmentConsentProvider(GeneralGranted());
            Assert.That(
                ConsentEligibility.ForAnalytics(provider, true).IsEligible,
                Is.True
            );

            ConsentProviderResult replaced = provider.Replace(
                new ConsentSnapshot(
                    2,
                    1,
                    ConsentAudience.GeneralAudience,
                    ConsentDecision.Granted,
                    ConsentDecision.Granted
                )
            );

            Assert.That(replaced.IsReady, Is.False);
            Assert.That(replaced.Status, Is.EqualTo(ConsentProviderStatus.InvalidSnapshot));
            Assert.That(replaced.Snapshot, Is.SameAs(ConsentSnapshot.Restricted));
            Assert.That(
                ConsentEligibility.ForAnalytics(provider, true).IsEligible,
                Is.False
            );
        }

        [Test]
        public void ResetRestricted_RevokesBothPurposesInMemory()
        {
            var provider = new InMemoryDevelopmentConsentProvider(GeneralGranted());

            ConsentProviderResult reset = provider.ResetRestricted();

            Assert.That(reset.IsReady, Is.True);
            Assert.That(reset.ReasonCode, Is.EqualTo(ConsentReasonCodes.DevelopmentProviderReset));
            Assert.That(reset.Snapshot, Is.SameAs(ConsentSnapshot.Restricted));
            Assert.That(ConsentEligibility.ForAnalytics(provider, true).IsEligible, Is.False);
            Assert.That(ConsentEligibility.ForRewardedAdvertising(provider, true).IsEligible, Is.False);
        }

        static ConsentSnapshot GeneralGranted()
        {
            return new ConsentSnapshot(
                ConsentSnapshot.CurrentSchemaVersion,
                ConsentSnapshot.CurrentPolicyVersion,
                ConsentAudience.GeneralAudience,
                ConsentDecision.Granted,
                ConsentDecision.Granted
            );
        }

        static void AssertInvalid(ConsentSnapshot snapshot, string reasonCode)
        {
            ConsentValidationResult result = ConsentSnapshotValidator.Validate(snapshot);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.ReasonCode, Is.EqualTo(reasonCode));
        }

        sealed class CountingProvider : IConsentProvider
        {
            readonly ConsentProviderResult result;
            public int Calls;

            public CountingProvider(ConsentSnapshot snapshot)
            {
                result = ConsentProviderResult.Ready(snapshot);
            }

            public ConsentProviderResult GetConsent()
            {
                Calls++;
                return result;
            }
        }

        sealed class NullProvider : IConsentProvider
        {
            public ConsentProviderResult GetConsent()
            {
                return null;
            }
        }

        sealed class ThrowingProvider : IConsentProvider
        {
            public ConsentProviderResult GetConsent()
            {
                throw new InvalidOperationException("provider");
            }
        }
    }
}
