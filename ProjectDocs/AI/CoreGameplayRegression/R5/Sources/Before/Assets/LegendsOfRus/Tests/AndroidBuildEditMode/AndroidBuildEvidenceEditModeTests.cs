#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Linq;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidBuildEvidenceEditModeTests
    {
        [Test]
        public void Evidence_ContainsOnlySanitizedReadinessState()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(
                    AndroidBuildTestFactory.Policy(),
                    facts
                );
            var result = new AndroidBuildPreflightResult(
                null,
                facts,
                decision
            );

            AndroidBuildEvidence evidence = AndroidBuildEvidence.From(result);
            string json = evidence.ToSanitizedJson();

            Assert.IsTrue(evidence.packageLockApproved);
            Assert.IsTrue(evidence.nativeDebugSymbolsDisabled);
            StringAssert.Contains(
                AndroidBuildReasonCodes.DevelopmentApkAllowed,
                json
            );
            StringAssert.DoesNotContain(
                "com.example.legendsofrus.v117debug",
                json
            );
            StringAssert.DoesNotContain(
                "LegendsOfRus-v1.17-step01-development.apk",
                json
            );
            StringAssert.DoesNotContain("keystore", json.ToLowerInvariant());
            StringAssert.DoesNotContain("password", json.ToLowerInvariant());
            StringAssert.DoesNotContain("alias", json.ToLowerInvariant());
        }

        [Test]
        public void EvidenceType_HasNoSensitiveFieldNames()
        {
            string[] forbidden =
            {
                "password", "alias", "keystore", "secret", "token",
                "receipt", "identifierValue", "outputPathValue"
            };
            string[] names = typeof(AndroidBuildEvidence)
                .GetFields()
                .Select(field => field.Name.ToLowerInvariant())
                .ToArray();

            foreach (string word in forbidden)
            {
                Assert.IsFalse(
                    names.Any(name => name.Contains(word.ToLowerInvariant())),
                    "Sensitive field name present: " + word
                );
            }
        }

        [Test]
        public void BlockedEvidence_NeverClaimsProductionAuthorization()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.AppBundle = true;
            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(
                    AndroidBuildTestFactory.Policy(),
                    facts
                );
            AndroidBuildEvidence evidence = AndroidBuildEvidence.From(
                new AndroidBuildPreflightResult(null, facts, decision)
            );

            Assert.IsFalse(evidence.productionBuildAuthorized);
            Assert.IsFalse(evidence.productionAndroidBuildCompiled);
            Assert.AreEqual(
                "DEVELOPMENT_APK_PREFLIGHT_BLOCKED",
                evidence.status
            );
            Assert.AreEqual(
                AndroidBuildReasonCodes.AppBundleBlocked,
                evidence.reasonCode
            );
        }

        [Test]
        public void PreflightEvidence_ReportsOutputValidationAsDeferred()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.OutputPathApproved = false;
            facts.OutputPathValidationDeferred = true;
            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(
                    AndroidBuildTestFactory.Policy(),
                    facts
                );
            AndroidBuildEvidence evidence = AndroidBuildEvidence.From(
                new AndroidBuildPreflightResult(null, facts, decision)
            );

            Assert.IsTrue(decision.Allowed);
            Assert.IsFalse(evidence.outputPathValidated);
            Assert.IsTrue(evidence.outputPathValidationDeferred);
        }
    }
}
#endif
