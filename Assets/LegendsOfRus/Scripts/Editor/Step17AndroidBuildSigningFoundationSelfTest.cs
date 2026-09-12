#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LegendsOfRus.AndroidBuild;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17AndroidBuildSigningFoundationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("policy is present, exact, and SHA-256 pinned",
                PolicyIsPinned, ref passed, ref total, failures);
            Check("policy allows only a Development APK",
                PolicyIsDevelopmentOnly, ref passed, ref total, failures);
            Check("all production approvals remain false",
                ProductionApprovalsAreFalse,
                ref passed, ref total, failures);
            Check("strict policy keys reject duplicates",
                DuplicatePolicyKeyIsRejected,
                ref passed, ref total, failures);
            Check("synthetic exact Development APK facts are allowed",
                DevelopmentFactsAreAllowed,
                ref passed, ref total, failures);
            Check("unapproved resolved package lock is blocked",
                UnapprovedPackageLockIsBlocked,
                ref passed, ref total, failures);
            Check("release APK is blocked by the private build switch",
                ReleaseIsBlocked, ref passed, ref total, failures);
            Check("AAB is unconditionally blocked",
                AppBundleIsBlocked, ref passed, ref total, failures);
            Check("Gradle export is unconditionally blocked",
                GradleExportIsBlocked, ref passed, ref total, failures);
            Check("custom signing is unconditionally blocked",
                CustomSigningIsBlocked, ref passed, ref total, failures);
            Check("split APK output is unconditionally blocked",
                SplitApkIsBlocked, ref passed, ref total, failures);
            Check("split application binary is unconditionally blocked",
                SplitApplicationBinaryIsBlocked,
                ref passed, ref total, failures);
            Check("alternate development identity is blocked",
                AlternateIdentityIsBlocked,
                ref passed, ref total, failures);
            Check("unsafe Android scripting defines are blocked",
                UnsafeDefinesAreBlocked, ref passed, ref total, failures);
            Check("per-build Android scripting defines are blocked",
                PerBuildDefinesAreBlocked,
                ref passed, ref total, failures);
            Check("scheduled Android Player options are exact",
                ScheduledBuildContractIsExact,
                ref passed, ref total, failures);
            Check("commercial boundary drift is blocked",
                CommercialDriftIsBlocked,
                ref passed, ref total, failures);
            Check("global guard requests maximum callback order",
                GuardRequestsMaximumCallbackOrder,
                ref passed, ref total, failures);
            Check("player-options guard requests maximum callback order",
                PlayerOptionsGuardRequestsMaximumCallbackOrder,
                ref passed, ref total, failures);
            Check("custom Build Profile assets are absent",
                CustomBuildProfilesAreAbsent,
                ref passed, ref total, failures);
            Check("native Android debug symbols are disabled",
                NativeDebugSymbolsAreDisabled,
                ref passed, ref total, failures);
            Check("legacy Android game category flag is enabled",
                LegacyGameCategoryIsEnabled,
                ref passed, ref total, failures);
            Check("artifact and path boundary predicates are configured",
                ArtifactAndPathBoundaryPredicatesAreConfigured,
                ref passed, ref total, failures);
            Check("production Android build switch is private const false",
                ProductionSwitchIsPrivateFalse,
                ref passed, ref total, failures);
            Check("Unity settings reader fails closed without throwing",
                SettingsReaderIsFailClosed,
                ref passed, ref total, failures);
            Check("Android source never calls BuildPipeline.BuildPlayer",
                NoBuildPipelineInvocation,
                ref passed, ref total, failures);
            Check("Android source never mutates PlayerSettings",
                NoPlayerSettingsMutation,
                ref passed, ref total, failures);
            Check("credential-bearing Unity fields are never read",
                NoCredentialFieldReads,
                ref passed, ref total, failures);
            Check("Android source has no credential resolver or network path",
                NoCredentialResolverOrNetwork,
                ref passed, ref total, failures);
            Check("preflight evidence has no identity or output values",
                EvidenceIsSanitized,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.17 Step 1 Android build/signing foundation " +
                    "self-tests passed: " + passed + "/" + total +
                    ". Production Android builds and custom signing remain " +
                    "blocked."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        "v1.17 Step 1 self-tests passed: " + passed + "/" +
                        total + ".\n\nThis does not claim Unity Test Runner, " +
                        "APK build, signing, device, or production evidence.",
                        "OK"
                    );
                }
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.17 Step 1 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary
            );
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Test failures",
                    summary,
                    "OK"
                );
            }
            throw new InvalidOperationException(
                "v1.17 Step 1 self-tests failed."
            );
        }

        static bool PolicyIsPinned()
        {
            AndroidBuildPolicyLoadResult result =
                AndroidBuildPolicyLoader.Load();
            return
                result != null &&
                result.FilePresent &&
                result.HashMatches &&
                result.IsValid &&
                result.Policy != null &&
                AndroidBuildPolicyLoader.ExpectedSourceSha256 ==
                    "95eec67ffb9ef8e45ad657524cbfcaa3c852258f4de2c7a54dc53ad18f96751c";
        }

        static bool PolicyIsDevelopmentOnly()
        {
            AndroidBuildFoundationPolicy value =
                AndroidBuildPolicyLoader.Load()?.Policy;
            return
                value != null &&
                value.allowDevelopmentApk &&
                !value.allowReleaseApk &&
                !value.allowAppBundle &&
                !value.allowGradleExport &&
                !value.allowCustomSigning &&
                !value.productionBuildApproved;
        }

        static bool ProductionApprovalsAreFalse()
        {
            AndroidBuildFoundationPolicy value =
                AndroidBuildPolicyLoader.Load()?.Policy;
            return
                value != null &&
                !value.productionApplicationIdentifierApproved &&
                !value.versioningApproved &&
                !value.sdkPolicyApproved &&
                !value.signingIdentityApproved &&
                !value.playAppSigningApproved &&
                !value.nativePluginInventoryApproved &&
                !value.sixteenKbPageCompatibilityVerified &&
                !value.dataSafetyApproved &&
                !value.productionBuildApproved;
        }

        static bool DuplicatePolicyKeyIsRejected()
        {
            string source = File.ReadAllText(
                Path.Combine(ProjectRoot(), AndroidBuildPolicyLoader.AssetPath)
            );
            source = source.Replace(
                "\"schemaVersion\": 1,",
                "\"schemaVersion\": 1,\n  \"schemaVersion\": 1,"
            );
            return !AndroidBuildPolicyLoader.HasExactKeys(source);
        }

        static bool DevelopmentFactsAreAllowed()
        {
            AndroidBuildDecision value = AndroidBuildPolicyEvaluator.Evaluate(
                SyntheticPolicy(),
                SyntheticFacts()
            );
            return
                value != null &&
                value.Allowed &&
                value.Intent == AndroidBuildIntent.DevelopmentApk &&
                value.ReasonCode ==
                    AndroidBuildReasonCodes.DevelopmentApkAllowed;
        }

        static bool UnapprovedPackageLockIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.PackageLockApproved = false;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.PackageLockUnapproved
            );
        }

        static bool ReleaseIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.DevelopmentBuild = false;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.ProductionSwitchDisabled
            );
        }

        static bool AppBundleIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.AppBundle = true;
            return IsBlocked(facts, AndroidBuildReasonCodes.AppBundleBlocked);
        }

        static bool GradleExportIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.GradleExport = true;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.GradleExportBlocked
            );
        }

        static bool CustomSigningIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.CustomSigning = true;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.CustomSigningBlocked
            );
        }

        static bool SplitApkIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.SplitApkPerCpuArchitecture = true;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.SplitApkBlocked
            );
        }

        static bool SplitApplicationBinaryIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.SplitApplicationBinary = true;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.SplitApplicationBinaryBlocked
            );
        }

        static bool AlternateIdentityIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.DevelopmentIdentityMatches = false;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.DevelopmentIdentityMismatch
            );
        }

        static bool UnsafeDefinesAreBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.ScriptingDefinesSafe = false;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.ScriptingDefinesUnsafe
            );
        }

        static bool PerBuildDefinesAreBlocked()
        {
            var options = new BuildPlayerOptions
            {
                target = BuildTarget.Android,
                extraScriptingDefines = new[] { "UNREVIEWED_BUILD_DEFINE" }
            };
            try
            {
                AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    options,
                    SyntheticPolicy()
                );
                return false;
            }
            catch (BuildFailedException exception)
            {
                return
                    exception.Message.Contains(
                        AndroidBuildReasonCodes.ScriptingDefinesUnsafe
                    ) &&
                    !exception.Message.Contains("UNREVIEWED_BUILD_DEFINE");
            }
        }

        static bool ScheduledBuildContractIsExact()
        {
            BuildPlayerOptions exact = SyntheticScheduledOptions();
            try
            {
                AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    exact,
                    SyntheticPolicy()
                );
            }
            catch
            {
                return false;
            }

            BuildPlayerOptions uiBuild = SyntheticScheduledOptions();
            uiBuild.options |= BuildOptions.ShowBuiltPlayer;
            try
            {
                AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    uiBuild,
                    SyntheticPolicy()
                );
            }
            catch
            {
                return false;
            }

            BuildPlayerOptions alternate = SyntheticScheduledOptions();
            alternate.scenes = new string[0];
            if (!ScheduledOptionsAreBlocked(
                alternate,
                AndroidBuildReasonCodes.BootstrapSceneMismatch))
                return false;

            alternate = SyntheticScheduledOptions();
            alternate.options |= BuildOptions.IncludeTestAssemblies;
            if (!ScheduledOptionsAreBlocked(
                alternate,
                AndroidBuildReasonCodes.BuildOptionsUnsafe))
                return false;

            alternate = SyntheticScheduledOptions();
            alternate.assetBundleManifestPath = "Temp/unreviewed.manifest";
            if (!ScheduledOptionsAreBlocked(
                alternate,
                AndroidBuildReasonCodes.AssetBundleManifestBlocked))
                return false;

            alternate = SyntheticScheduledOptions();
            alternate.locationPathName =
                "Builds/Verification/unreviewed.apk";
            return ScheduledOptionsAreBlocked(
                alternate,
                AndroidBuildReasonCodes.OutputPathInvalid
            );
        }

        static bool ScheduledOptionsAreBlocked(
            BuildPlayerOptions options,
            string reasonCode)
        {
            try
            {
                AndroidBuildPlayerOptionsGuard.ValidateOptions(
                    options,
                    SyntheticPolicy()
                );
                return false;
            }
            catch (BuildFailedException exception)
            {
                return exception.Message.Contains(reasonCode);
            }
        }

        static BuildPlayerOptions SyntheticScheduledOptions()
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

        static bool CommercialDriftIsBlocked()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            facts.CommercialBoundaryIntact = false;
            return IsBlocked(
                facts,
                AndroidBuildReasonCodes.CommercialBoundaryChanged
            );
        }

        static bool GuardRequestsMaximumCallbackOrder()
        {
            return new AndroidBuildGuard().callbackOrder == int.MaxValue;
        }

        static bool PlayerOptionsGuardRequestsMaximumCallbackOrder()
        {
            return
                new AndroidBuildPlayerOptionsGuard().callbackOrder ==
                int.MaxValue;
        }

        static bool CustomBuildProfilesAreAbsent()
        {
            return AndroidBuildStateReader.CustomBuildProfileAbsent();
        }

        static bool NativeDebugSymbolsAreDisabled()
        {
            return AndroidBuildStateReader.NativeDebugSymbolsDisabled();
        }

        static bool LegacyGameCategoryIsEnabled()
        {
            return AndroidBuildStateReader.LegacyAndroidGameCategoryEnabled();
        }

        static bool ArtifactAndPathBoundaryPredicatesAreConfigured()
        {
            return
                AndroidBuildStateReader.ExactOutputPathTextMatches(
                    "Builds/Verification/game.apk",
                    "Builds/Verification/game.apk"
                ) &&
                !AndroidBuildStateReader.ExactOutputPathTextMatches(
                    "builds/Verification/game.apk",
                    "Builds/Verification/game.apk"
                ) &&
                AndroidBuildStateReader
                    .CaseFoldedFileSystemNameMatches("Builds", "builds") &&
                AndroidBuildStateReader
                    .IsForbiddenAndroidLibraryDirectoryName(
                        "unreviewed.AndroidLib"
                    ) &&
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactFileName(
                        "AndroidManifest.xml"
                    ) &&
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".dll") &&
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".java") &&
                AndroidBuildStateReader
                    .IsForbiddenAndroidArtifactExtension(".kt") &&
                AndroidBuildStateReader.NoEmbeddedPackages(ProjectRoot());
        }

        static bool ProductionSwitchIsPrivateFalse()
        {
            FieldInfo field = typeof(AndroidBuildGuard).GetField(
                "ProductionAndroidBuildCompiled",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                field != null &&
                field.IsPrivate &&
                field.IsLiteral &&
                Equals(field.GetRawConstantValue(), false);
        }

        static bool SettingsReaderIsFailClosed()
        {
            AndroidBuildPolicyLoadResult policyLoad =
                AndroidBuildPolicyLoader.Load();
            AndroidBuildFacts facts = AndroidBuildStateReader.ReadForBuild(
                null,
                policyLoad,
                false
            );
            AndroidBuildDecision decision =
                AndroidBuildPolicyEvaluator.Evaluate(
                    policyLoad != null ? policyLoad.Policy : null,
                    facts
                );
            return
                facts != null &&
                !facts.SettingsReadSucceeded &&
                decision != null &&
                !decision.Allowed &&
                decision.ReasonCode ==
                    AndroidBuildReasonCodes.SettingsReadFailed;
        }

        static bool NoBuildPipelineInvocation()
        {
            string needle = "BuildPipeline." + "BuildPlayer(";
            return !AndroidSource().Contains(needle);
        }

        static bool NoPlayerSettingsMutation()
        {
            string source = AndroidSource();
            string[] forbidden =
            {
                "SetApplication" + "Identifier(",
                "SetScripting" + "Backend(",
                "SetScriptingDefine" + "Symbols("
            };
            if (forbidden.Any(token => source.Contains(token)))
                return false;

            return !Regex.IsMatch(
                source,
                @"(?:PlayerSettings(?:\.Android)?|" +
                @"EditorUserBuildSettings)\.[A-Za-z0-9_]+\s*=(?!=)"
            );
        }

        static bool NoCredentialFieldReads()
        {
            string source = AndroidSource();
            string[] forbidden =
            {
                "PlayerSettings.Android." + "keystore" + "Name",
                "PlayerSettings.Android." + "keystore" + "Pass",
                "PlayerSettings.Android." + "keyalias" + "Name",
                "PlayerSettings.Android." + "keyalias" + "Pass"
            };
            return forbidden.All(token => !source.Contains(token));
        }

        static bool NoCredentialResolverOrNetwork()
        {
            string source = AndroidSource();
            string[] forbidden =
            {
                "Environment.Get" + "EnvironmentVariable",
                "Environment.Get" + "CommandLineArgs",
                "Editor" + "Prefs",
                "Session" + "State",
                "Player" + "Prefs",
                "System.Diagnostics." + "Process",
                "UnityWeb" + "Request",
                "Http" + "Client",
                "Web" + "Client"
            };
            return forbidden.All(token => !source.Contains(token));
        }

        static bool EvidenceIsSanitized()
        {
            AndroidBuildFacts facts = SyntheticFacts();
            AndroidBuildDecision decision = AndroidBuildPolicyEvaluator.Evaluate(
                SyntheticPolicy(),
                facts
            );
            string json = AndroidBuildEvidence.From(
                new AndroidBuildPreflightResult(null, facts, decision)
            ).ToSanitizedJson();
            string lower = json.ToLowerInvariant();
            return
                !json.Contains("com.example.legendsofrus.v117debug") &&
                !json.Contains("LegendsOfRus-v1.17-step01-development.apk") &&
                !lower.Contains("keystore") &&
                !lower.Contains("password") &&
                !lower.Contains("alias");
        }

        static bool IsBlocked(AndroidBuildFacts facts, string reasonCode)
        {
            AndroidBuildDecision value = AndroidBuildPolicyEvaluator.Evaluate(
                SyntheticPolicy(),
                facts
            );
            return
                value != null &&
                !value.Allowed &&
                value.ReasonCode == reasonCode;
        }

        static AndroidBuildFoundationPolicy SyntheticPolicy()
        {
            return new AndroidBuildFoundationPolicy
            {
                schemaVersion = 1,
                policyId = "v1.17-step01-android-build-signing-foundation",
                unityVersion = "6000.5.9f1",
                mode = "development-apk-only",
                requiredScene = "Assets/Scenes/Bootstrap.unity",
                developmentApplicationIdentifier =
                    "com.example.legendsofrus.v117debug",
                developmentBundleVersion = "0.0.0",
                developmentBundleVersionCode = 1,
                developmentOutputRelativePath =
                    "Builds/Verification/v1_17_step01/" +
                    "LegendsOfRus-v1.17-step01-development.apk",
                requiredTargetApiLevel = 36,
                minimumSupportedApiLevel = 26,
                maximumVersionCode = 2100000000,
                requireIl2Cpp = true,
                requireArm64 = true,
                requireGameCategory = true,
                requirePortrait = true,
                allowDevelopmentApk = true
            };
        }

        static AndroidBuildFacts SyntheticFacts()
        {
            return new AndroidBuildFacts
            {
                IsAndroidBuild = true,
                PolicyFilePresent = true,
                PolicyHashMatches = true,
                PolicyValid = true,
                SettingsReadSucceeded = true,
                UnityVersionMatches = true,
                ProjectSettingsPresent = true,
                PackageLockApproved = true,
                AndroidModuleAvailable = true,
                AndroidTargetActive = true,
                BuildProfileOverrideAbsent = true,
                BootstrapSceneOnly = true,
                DevelopmentIdentityMatches = true,
                DevelopmentVersionMatches = true,
                TargetApiMatches = true,
                MinimumApiSupported = true,
                Il2CppEnabled = true,
                Arm64Enabled = true,
                GameCategoryConfigured = true,
                PortraitConfigured = true,
                DevelopmentBuild = true,
                NativeDebugSymbolsDisabled = true,
                ScriptingDefinesSafe = true,
                OutputPathApproved = true,
                CommercialBoundaryIntact = true
            };
        }

        static string AndroidSource()
        {
            string root = Path.Combine(
                Application.dataPath,
                "LegendsOfRus/Editor/AndroidBuild"
            );
            return string.Join(
                "\n",
                Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                    .OrderBy(path => path, StringComparer.Ordinal)
                    .Select(File.ReadAllText)
                    .ToArray()
            );
        }

        static string ProjectRoot()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }

        static void Check(
            string name,
            Func<bool> assertion,
            ref int passed,
            ref int total,
            List<string> failures)
        {
            total++;
            try
            {
                if (assertion())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + name);
            }
            catch
            {
                failures.Add("FAIL: " + name + " (exception)");
            }
        }
    }
}
#endif
