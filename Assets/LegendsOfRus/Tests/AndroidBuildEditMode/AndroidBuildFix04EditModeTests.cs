#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Linq;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidBuildFix04EditModeTests
    {
        [TestCase(BuildOptions.None)]
        [TestCase(BuildOptions.ShowBuiltPlayer)]
        [TestCase(BuildOptions.CompressWithLz4)]
        [TestCase(BuildOptions.CompressWithLz4HC)]
        [TestCase(BuildOptions.StrictMode)]
        [TestCase(BuildOptions.CompressWithLz4 | BuildOptions.StrictMode)]
        [TestCase(BuildOptions.ShowBuiltPlayer | BuildOptions.CompressWithLz4HC | BuildOptions.StrictMode)]
        public void DevelopmentSupportsNormalOptionalFlags(BuildOptions extra)
        {
            var options = BuildOptions.Development | extra;
            Assert.DoesNotThrow(() => AndroidDevelopmentBuildOptions.Validate(options));
            Assert.AreEqual(BuildOptions.None, AndroidDevelopmentBuildOptions.Disallowed(options));
        }

        [Test]
        public void EverySubsetOfAllowedMaskIsAcceptedWhenDevelopmentIsPresent()
        {
            var flags = new[] { BuildOptions.ShowBuiltPlayer, BuildOptions.CompressWithLz4,
                BuildOptions.CompressWithLz4HC, BuildOptions.StrictMode };
            for (int subset = 0; subset < (1 << flags.Length); subset++)
            {
                BuildOptions options = BuildOptions.Development;
                for (int index = 0; index < flags.Length; index++)
                    if ((subset & (1 << index)) != 0) options |= flags[index];
                Assert.DoesNotThrow(() => AndroidDevelopmentBuildOptions.Validate(options));
            }
        }

        [TestCase(BuildOptions.None)]
        [TestCase(BuildOptions.AutoRunPlayer | BuildOptions.CompressWithLz4 | BuildOptions.StrictMode)]
        [TestCase(BuildOptions.ShowBuiltPlayer | BuildOptions.CompressWithLz4)]
        public void ReleaseIsRejectedWithSpecificDevelopmentReason(BuildOptions options)
        {
            var error = Assert.Throws<BuildFailedException>(() => AndroidDevelopmentBuildOptions.Validate(options));
            StringAssert.Contains("ANDROID_DEVELOPMENT_BUILD_REQUIRED", error.Message);
            StringAssert.Contains("Development=False", error.Message);
        }

        [TestCase(BuildOptions.AutoRunPlayer)]
        [TestCase(BuildOptions.AllowDebugging)]
        [TestCase(BuildOptions.ConnectWithProfiler)]
        [TestCase(BuildOptions.IncludeTestAssemblies)]
        [TestCase(BuildOptions.BuildScriptsOnly)]
        [TestCase(BuildOptions.EnableDeepProfilingSupport)]
        [TestCase(BuildOptions.EnableCodeCoverage)]
        public void OtherModesAreNotSilentlyAccepted(BuildOptions flag)
        {
            var options = AndroidDevelopmentBuildOptions.BuildOnlyDefaults | flag;
            var error = Assert.Throws<BuildFailedException>(() => AndroidDevelopmentBuildOptions.Validate(options));
            StringAssert.Contains("ANDROID_BUILD_OPTIONS_UNSAFE", error.Message);
            Assert.AreEqual(flag, AndroidDevelopmentBuildOptions.Disallowed(options));
        }

        [Test]
        public void ActualReproductionStillRejectsReleaseAndAutoRun()
        {
            // Reproduces the active public-flag portion of the reported log. No claim about legacy alias values.
            var request = BuildOptions.AutoRunPlayer | BuildOptions.CompressWithLz4 | BuildOptions.StrictMode;
            Assert.IsFalse(AndroidDevelopmentBuildOptions.HasDevelopment(request));
            Assert.AreEqual(BuildOptions.AutoRunPlayer, AndroidDevelopmentBuildOptions.Disallowed(request));
            Assert.Throws<BuildFailedException>(() => AndroidDevelopmentBuildOptions.Validate(request));
            Assert.Throws<BuildFailedException>(() => AndroidDevelopmentBuildOptions.Validate(request | BuildOptions.Development));
            Assert.DoesNotThrow(() => AndroidDevelopmentBuildOptions.Validate(
                (request & ~BuildOptions.AutoRunPlayer) | BuildOptions.Development));
        }

        [Test]
        public void RequestFactoryUsesBootstrapPolicyPathAndNeverRequestsRun()
        {
            var policy = AndroidBuildTestFactory.Policy();
            var request = AndroidDevelopmentBuildCommand.CreateRequest(policy);
            Assert.AreEqual(BuildTarget.Android, request.target);
            Assert.AreEqual(BuildTargetGroup.Android, request.targetGroup);
            Assert.AreEqual(0, request.subtarget);
            CollectionAssert.AreEqual(new[] { policy.requiredScene }, request.scenes);
            Assert.AreEqual(policy.developmentOutputRelativePath, request.locationPathName);
            Assert.AreEqual(AndroidDevelopmentBuildOptions.BuildOnlyDefaults, request.options);
            Assert.AreEqual(BuildOptions.None, request.options & BuildOptions.AutoRunPlayer);
            Assert.IsEmpty(request.extraScriptingDefines);
            Assert.IsEmpty(request.assetBundleManifestPath);
        }

        [Test]
        public void RequestFactoryRejectsMissingPolicy()
        {
            Assert.Throws<ArgumentNullException>(() => AndroidDevelopmentBuildCommand.CreateRequest(null));
        }

        [Test]
        public void ZeroAndCompositeAliasesAreNotShownAsActive()
        {
            var definitions = new[] {
                new AndroidBuildOptionDefinition("SyntheticLegacyAlias", 0, true),
                new AndroidBuildOptionDefinition("None", 0, false),
                new AndroidBuildOptionDefinition("One", 1, false),
                new AndroidBuildOptionDefinition("Two", 2, false),
                new AndroidBuildOptionDefinition("Composite", 3, false)
            };
            Assert.AreEqual("None", AndroidDevelopmentBuildOptions.FormatBits(0, definitions));
            Assert.AreEqual("One, Two", AndroidDevelopmentBuildOptions.FormatBits(3, definitions));
        }

        [Test]
        public void DuplicateSingleBitAliasesPreferNonObsoleteName()
        {
            var definitions = new[] {
                new AndroidBuildOptionDefinition("OldAlias", 1, true),
                new AndroidBuildOptionDefinition("ModernName", 1, false)
            };
            Assert.AreEqual("ModernName", AndroidDevelopmentBuildOptions.FormatBits(1, definitions));
        }

        [Test]
        public void UnknownAndHighBitsAreNotLost()
        {
            Assert.AreEqual("UnknownBits(0x80000000)",
                AndroidDevelopmentBuildOptions.FormatBits(0x80000000u, null));
            Assert.AreEqual(0x80000000u,
                AndroidDevelopmentBuildOptions.RawBits(unchecked((BuildOptions)(int)0x80000000u)));
        }

        [Test]
        public void InstalledZeroValuedNamesNeverAppearInDevelopmentFormatting()
        {
            var definitions = AndroidDevelopmentBuildOptions.ReadEnumEvidence();
            var formatted = AndroidDevelopmentBuildOptions.FormatActive(BuildOptions.Development)
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var definition in definitions.Where(value => value.zeroValue))
                CollectionAssert.DoesNotContain(formatted, definition.name);
        }

        [Test]
        public void FixedDiagnosticsPrintNumericMaskAndNoFalseDisallowedNames()
        {
            string message = AndroidBuildFix03Diagnostics.DescribeOptions(AndroidDevelopmentBuildOptions.BuildOnlyDefaults);
            StringAssert.Contains("RequestedBits=0x", message);
            StringAssert.Contains("Development=True", message);
            StringAssert.Contains("AutoRunPlayer=False", message);
            StringAssert.Contains("DisallowedBits=0x00000000", message);
            StringAssert.Contains("DisallowedFlags=None", message);
        }

        [Test]
        public void PackageLockApprovalStillMustBeSatisfied()
        {
            var facts = AndroidBuildTestFactory.AllowedFacts();
            facts.PackageLockApproved = false;
            var result = AndroidBuildPolicyEvaluator.Evaluate(AndroidBuildTestFactory.Policy(), facts);
            Assert.IsFalse(result.Allowed);
            Assert.AreEqual(AndroidBuildReasonCodes.PackageLockUnapproved, result.ReasonCode);
        }
    }
}
#endif
