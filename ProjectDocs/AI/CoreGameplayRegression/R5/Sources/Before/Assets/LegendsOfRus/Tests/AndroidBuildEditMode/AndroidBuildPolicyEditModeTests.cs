#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System.IO;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidBuildPolicyEditModeTests
    {
        [Test]
        public void ShippedPolicy_IsPinnedStrictAndDevelopmentOnly()
        {
            AndroidBuildPolicyLoadResult result =
                AndroidBuildPolicyLoader.Load();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.FilePresent);
            Assert.IsTrue(result.HashMatches);
            Assert.IsTrue(result.IsValid);
            Assert.IsNotNull(result.Policy);
            Assert.IsTrue(result.Policy.allowDevelopmentApk);
            Assert.IsFalse(result.Policy.allowReleaseApk);
            Assert.IsFalse(result.Policy.allowAppBundle);
            Assert.IsFalse(result.Policy.allowGradleExport);
            Assert.IsFalse(result.Policy.allowCustomSigning);
            Assert.IsFalse(
                AndroidBuildPolicyEvaluator.ProductionApprovalsAreComplete(
                    result.Policy
                )
            );
        }

        [Test]
        public void StrictKeys_RejectDuplicateKey()
        {
            string source = PolicySource();
            source = source.Replace(
                "\"schemaVersion\": 1,",
                "\"schemaVersion\": 1,\n  \"schemaVersion\": 1,"
            );

            Assert.IsFalse(AndroidBuildPolicyLoader.HasExactKeys(source));
        }

        [Test]
        public void StrictKeys_RejectUnknownKey()
        {
            string source = PolicySource();
            source = "{\n  \"unknownAuthority\": true," +
                source.Substring(1);

            Assert.IsFalse(AndroidBuildPolicyLoader.HasExactKeys(source));
        }

        [Test]
        public void StrictKeys_RejectMissingKey()
        {
            string source = PolicySource();
            source = source.Replace(
                "  \"dataSafetyApproved\": false,\n",
                string.Empty
            );

            Assert.IsFalse(AndroidBuildPolicyLoader.HasExactKeys(source));
        }

        [Test]
        public void ExactPolicy_RejectsSyntheticProductionApproval()
        {
            AndroidBuildFoundationPolicy policy =
                AndroidBuildTestFactory.Policy();
            policy.productionBuildApproved = true;

            Assert.IsFalse(
                AndroidBuildPolicyLoader.IsExactFoundationPolicy(policy)
            );
        }

        static string PolicySource()
        {
            string root = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..")
            );
            return File.ReadAllText(
                Path.Combine(root, AndroidBuildPolicyLoader.AssetPath)
            );
        }
    }
}
#endif
