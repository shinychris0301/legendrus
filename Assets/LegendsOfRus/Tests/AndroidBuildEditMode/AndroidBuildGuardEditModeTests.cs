#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Reflection;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidBuildGuardEditModeTests
    {
        [Test]
        public void ExactDevelopmentApk_IsOnlyAllowedIntent()
        {
            AndroidBuildDecision decision = EvaluateAllowed();

            Assert.IsTrue(decision.Allowed);
            Assert.AreEqual(
                AndroidBuildIntent.DevelopmentApk,
                decision.Intent
            );
            Assert.AreEqual(
                AndroidBuildReasonCodes.DevelopmentApkAllowed,
                decision.ReasonCode
            );
        }

        [Test]
        public void AppBundle_IsUnconditionallyBlocked()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            AndroidBuildFoundationPolicy policy =
                AndroidBuildTestFactory.Policy();
            facts.AppBundle = true;
            policy.allowAppBundle = true;

            AssertBlocked(
                policy,
                facts,
                AndroidBuildReasonCodes.AppBundleBlocked
            );
        }

        [Test]
        public void GradleExport_IsUnconditionallyBlocked()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.GradleExport = true;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.GradleExportBlocked
            );
        }

        [Test]
        public void CustomSigning_IsUnconditionallyBlocked()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.CustomSigning = true;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.CustomSigningBlocked
            );
        }

        [Test]
        public void SplitApkPerCpuArchitecture_IsUnconditionallyBlocked()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.SplitApkPerCpuArchitecture = true;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.SplitApkBlocked
            );
        }

        [Test]
        public void SplitApplicationBinary_IsUnconditionallyBlocked()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.SplitApplicationBinary = true;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.SplitApplicationBinaryBlocked
            );
        }

        [Test]
        public void ReleaseApk_IsBlockedByCompiledSwitch()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.DevelopmentBuild = false;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.ProductionSwitchDisabled
            );
        }

        [Test]
        public void ReleaseApk_RemainsBlockedWithSyntheticSwitchAndApprovals()
        {
            AndroidBuildFoundationPolicy policy =
                AndroidBuildTestFactory.Policy();
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.DevelopmentBuild = false;
            facts.ProductionAndroidBuildCompiled = true;
            policy.allowReleaseApk = true;
            ApproveEverything(policy);

            AssertBlocked(
                policy,
                facts,
                AndroidBuildReasonCodes.ReleaseBuildBlocked
            );
        }

        [TestCase(
            AndroidBuildReasonCodes.PolicyMissing,
            nameof(AndroidBuildFacts.PolicyFilePresent))]
        [TestCase(
            AndroidBuildReasonCodes.PolicyHashMismatch,
            nameof(AndroidBuildFacts.PolicyHashMatches))]
        [TestCase(
            AndroidBuildReasonCodes.PolicyInvalid,
            nameof(AndroidBuildFacts.PolicyValid))]
        [TestCase(
            AndroidBuildReasonCodes.UnityVersionMismatch,
            nameof(AndroidBuildFacts.UnityVersionMatches))]
        [TestCase(
            AndroidBuildReasonCodes.ProjectSettingsMissing,
            nameof(AndroidBuildFacts.ProjectSettingsPresent))]
        [TestCase(
            AndroidBuildReasonCodes.PackageLockUnapproved,
            nameof(AndroidBuildFacts.PackageLockApproved))]
        [TestCase(
            AndroidBuildReasonCodes.ModuleUnavailable,
            nameof(AndroidBuildFacts.AndroidModuleAvailable))]
        [TestCase(
            AndroidBuildReasonCodes.TargetNotActive,
            nameof(AndroidBuildFacts.AndroidTargetActive))]
        [TestCase(
            AndroidBuildReasonCodes.BuildProfileOverrideBlocked,
            nameof(AndroidBuildFacts.BuildProfileOverrideAbsent))]
        [TestCase(
            AndroidBuildReasonCodes.CommercialBoundaryChanged,
            nameof(AndroidBuildFacts.CommercialBoundaryIntact))]
        [TestCase(
            AndroidBuildReasonCodes.ScriptingDefinesUnsafe,
            nameof(AndroidBuildFacts.ScriptingDefinesSafe))]
        [TestCase(
            AndroidBuildReasonCodes.BootstrapSceneMismatch,
            nameof(AndroidBuildFacts.BootstrapSceneOnly))]
        [TestCase(
            AndroidBuildReasonCodes.DevelopmentIdentityMismatch,
            nameof(AndroidBuildFacts.DevelopmentIdentityMatches))]
        [TestCase(
            AndroidBuildReasonCodes.VersioningInvalid,
            nameof(AndroidBuildFacts.DevelopmentVersionMatches))]
        [TestCase(
            AndroidBuildReasonCodes.TargetApiMismatch,
            nameof(AndroidBuildFacts.TargetApiMatches))]
        [TestCase(
            AndroidBuildReasonCodes.MinimumApiUnsupported,
            nameof(AndroidBuildFacts.MinimumApiSupported))]
        [TestCase(
            AndroidBuildReasonCodes.Il2CppRequired,
            nameof(AndroidBuildFacts.Il2CppEnabled))]
        [TestCase(
            AndroidBuildReasonCodes.Arm64Required,
            nameof(AndroidBuildFacts.Arm64Enabled))]
        [TestCase(
            AndroidBuildReasonCodes.GameCategoryRequired,
            nameof(AndroidBuildFacts.GameCategoryConfigured))]
        [TestCase(
            AndroidBuildReasonCodes.PortraitRequired,
            nameof(AndroidBuildFacts.PortraitConfigured))]
        [TestCase(
            AndroidBuildReasonCodes.NativeDebugSymbolsBlocked,
            nameof(AndroidBuildFacts.NativeDebugSymbolsDisabled))]
        [TestCase(
            AndroidBuildReasonCodes.OutputPathInvalid,
            nameof(AndroidBuildFacts.OutputPathApproved))]
        public void RequiredFactFalse_IsDeterministicallyBlocked(
            string reasonCode,
            string propertyName)
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            PropertyInfo property = typeof(AndroidBuildFacts).GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );
            Assert.IsNotNull(property);
            property.SetValue(facts, false, null);

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                reasonCode
            );
        }

        [Test]
        public void SettingsReaderFailure_IsFailClosed()
        {
            AndroidBuildFacts facts = AndroidBuildTestFactory.AllowedFacts();
            facts.SettingsReadSucceeded = false;

            AssertBlocked(
                AndroidBuildTestFactory.Policy(),
                facts,
                AndroidBuildReasonCodes.SettingsReadFailed
            );
        }

        [Test]
        public void NonAndroidBuild_PassesWithoutPolicyAuthority()
        {
            var facts = new AndroidBuildFacts
            {
                IsAndroidBuild = false,
                SettingsReadSucceeded = true
            };

            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(null, facts);

            Assert.IsTrue(decision.Allowed);
            Assert.AreEqual(
                AndroidBuildReasonCodes.NonAndroidBuildIgnored,
                decision.ReasonCode
            );
        }

        [Test]
        public void Guard_RequestsMaximumCallbackOrderAndPrivateDisabledSwitch()
        {
            var guard = new AndroidBuildGuard();
            var optionsGuard = new AndroidBuildPlayerOptionsGuard();
            FieldInfo field = typeof(AndroidBuildGuard).GetField(
                "ProductionAndroidBuildCompiled",
                BindingFlags.Static | BindingFlags.NonPublic
            );

            Assert.AreEqual(int.MaxValue, guard.callbackOrder);
            Assert.AreEqual(int.MaxValue, optionsGuard.callbackOrder);
            Assert.IsNotNull(field);
            Assert.IsTrue(field.IsLiteral);
            Assert.AreEqual(false, field.GetRawConstantValue());
        }

        [Test]
        public void StateReader_ClosesPluginManifestAndEditorPathBoundaries()
        {
            Assert.IsTrue(
                AndroidBuildStateReader.ExactOutputPathTextMatches(
                    "Builds/Verification/game.apk",
                    "Builds/Verification/game.apk"
                )
            );
            Assert.IsFalse(
                AndroidBuildStateReader.ExactOutputPathTextMatches(
                    "builds/Verification/game.apk",
                    "Builds/Verification/game.apk"
                )
            );
            Assert.IsTrue(
                AndroidBuildStateReader.CaseFoldedFileSystemNameMatches(
                    "Builds",
                    "builds"
                )
            );
            Assert.IsTrue(
                AndroidBuildStateReader
                    .IsForbiddenAndroidLibraryDirectoryName(
                        "unreviewed.AndroidLib"
                    )
            );
            Assert.IsTrue(
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactFileName(
                        "AndroidManifest.xml"
                    )
            );
            Assert.IsTrue(
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".dll")
            );
            Assert.IsTrue(
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".java")
            );
            Assert.IsTrue(
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".kt")
            );
            Assert.IsTrue(
                AndroidBuildStateReader.NoEmbeddedPackages(
                    System.IO.Path.GetFullPath(
                        System.IO.Path.Combine(
                            UnityEngine.Application.dataPath,
                            ".."
                        )
                    )
                )
            );
            Assert.IsFalse(
                AndroidBuildStateReader
                    .IsForbiddenAndroidLibraryDirectoryName("reviewed-folder")
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsPerBuildAndroidDefines()
        {
            var options = new BuildPlayerOptions
            {
                target = BuildTarget.Android,
                extraScriptingDefines = new[] { "UNREVIEWED_BUILD_DEFINE" }
            };

            BuildFailedException exception =
                Assert.Throws<BuildFailedException>(
                    () => AndroidBuildPlayerOptionsGuard.ValidateOptions(
                        options,
                        AndroidBuildTestFactory.Policy()
                    )
                );

            StringAssert.Contains(
                AndroidBuildReasonCodes.ScriptingDefinesUnsafe,
                exception.Message
            );
            StringAssert.DoesNotContain(
                "UNREVIEWED_BUILD_DEFINE",
                exception.Message
            );
        }

        [Test]
        public void PlayerOptionsGuard_AllowsEmptyAndroidExtraDefines()
        {
            BuildPlayerOptions options = ExactScheduledOptions();

            Assert.DoesNotThrow(
                () => AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    options,
                    AndroidBuildTestFactory.Policy()
                )
            );
        }

        [Test]
        public void PlayerOptionsGuard_AllowsUiShowBuiltPlayerOnly()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.options |= BuildOptions.ShowBuiltPlayer;

            Assert.DoesNotThrow(
                () => AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    options,
                    AndroidBuildTestFactory.Policy()
                )
            );
        }

        [Test]
        public void PlayerOptionsGuard_IgnoresNonAndroidBuilds()
        {
            var options = new BuildPlayerOptions
            {
                target = BuildTarget.StandaloneWindows64,
                extraScriptingDefines = new[] { "NON_ANDROID_DEFINE" }
            };

            Assert.DoesNotThrow(
                () => AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    options,
                    null
                )
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsAlternateScene()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.scenes = new[] { "Assets/Scenes/Alternate.unity" };

            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.BootstrapSceneMismatch
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsEmptySceneList()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.scenes = new string[0];

            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.BootstrapSceneMismatch
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsAdditionalBuildOption()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.options |= BuildOptions.IncludeTestAssemblies;

            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.BuildOptionsUnsafe
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsAssetBundleManifest()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.assetBundleManifestPath = "Temp/unreviewed.manifest";

            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.AssetBundleManifestBlocked
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsAlternateTargetContext()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.targetGroup = BuildTargetGroup.Unknown;
            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.ScheduledTargetMismatch
            );

            options = ExactScheduledOptions();
            options.subtarget = 1;
            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.ScheduledTargetMismatch
            );
        }

        [Test]
        public void PlayerOptionsGuard_RejectsAlternateOutput()
        {
            BuildPlayerOptions options = ExactScheduledOptions();
            options.locationPathName = "Builds/Verification/unreviewed.apk";

            AssertScheduledBlocked(
                options,
                AndroidBuildReasonCodes.OutputPathInvalid
            );
        }

        static BuildPlayerOptions ExactScheduledOptions()
        {
            return new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Bootstrap.unity" },
                locationPathName =
                    "Builds/Verification/v1_17_step01/" +
                    "LegendsOfRus-v1.17-step01-development.apk",
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                subtarget = 0,
                options = BuildOptions.Development,
                extraScriptingDefines = new string[0],
                assetBundleManifestPath = string.Empty
            };
        }

        static void AssertScheduledBlocked(
            BuildPlayerOptions options,
            string reasonCode)
        {
            BuildFailedException exception =
                Assert.Throws<BuildFailedException>(
                    () => AndroidBuildPlayerOptionsGuard.ValidateOptions(
                        options,
                        AndroidBuildTestFactory.Policy()
                    )
                );
            StringAssert.Contains(reasonCode, exception.Message);
        }

        static AndroidBuildDecision EvaluateAllowed()
        {
            return AndroidBuildPolicyEvaluator.Evaluate(
                AndroidBuildTestFactory.Policy(),
                AndroidBuildTestFactory.AllowedFacts()
            );
        }

        static void AssertBlocked(
            AndroidBuildFoundationPolicy policy,
            AndroidBuildFacts facts,
            string reasonCode)
        {
            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(policy, facts);
            Assert.IsFalse(decision.Allowed);
            Assert.AreEqual(reasonCode, decision.ReasonCode);
        }

        static void ApproveEverything(AndroidBuildFoundationPolicy policy)
        {
            policy.productionApplicationIdentifierApproved = true;
            policy.versioningApproved = true;
            policy.sdkPolicyApproved = true;
            policy.signingIdentityApproved = true;
            policy.playAppSigningApproved = true;
            policy.nativePluginInventoryApproved = true;
            policy.sixteenKbPageCompatibilityVerified = true;
            policy.dataSafetyApproved = true;
            policy.productionBuildApproved = true;
        }
    }
}
#endif
