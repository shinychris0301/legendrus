namespace LegendsOfRus.AndroidBuild
{
    internal static class AndroidBuildPolicyEvaluator
    {
        internal static AndroidBuildDecision Evaluate(
            AndroidBuildFoundationPolicy policy,
            AndroidBuildFacts facts)
        {
            if (facts == null)
            {
                return Denied(
                    AndroidBuildIntent.NonAndroid,
                    AndroidBuildReasonCodes.SettingsReadFailed
                );
            }

            if (!facts.IsAndroidBuild)
            {
                return new AndroidBuildDecision(
                    true,
                    AndroidBuildIntent.NonAndroid,
                    AndroidBuildReasonCodes.NonAndroidBuildIgnored
                );
            }

            AndroidBuildIntent intent = Classify(facts);

            if (!facts.SettingsReadSucceeded)
                return Denied(intent, AndroidBuildReasonCodes.SettingsReadFailed);
            if (!facts.PolicyFilePresent)
                return Denied(intent, AndroidBuildReasonCodes.PolicyMissing);
            if (!facts.PolicyHashMatches)
                return Denied(intent, AndroidBuildReasonCodes.PolicyHashMismatch);
            if (!facts.PolicyValid || policy == null)
                return Denied(intent, AndroidBuildReasonCodes.PolicyInvalid);
            if (!facts.UnityVersionMatches)
                return Denied(intent, AndroidBuildReasonCodes.UnityVersionMismatch);
            if (!facts.ProjectSettingsPresent)
                return Denied(intent, AndroidBuildReasonCodes.ProjectSettingsMissing);
            if (!facts.PackageLockApproved)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.PackageLockUnapproved
                );
            if (!facts.AndroidModuleAvailable)
                return Denied(intent, AndroidBuildReasonCodes.ModuleUnavailable);
            if (!facts.AndroidTargetActive)
                return Denied(intent, AndroidBuildReasonCodes.TargetNotActive);
            if (!facts.BuildProfileOverrideAbsent)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.BuildProfileOverrideBlocked
                );
            if (!facts.CommercialBoundaryIntact)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.CommercialBoundaryChanged
                );
            if (!facts.ScriptingDefinesSafe)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.ScriptingDefinesUnsafe
                );
            if (facts.CustomSigning)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.CustomSigningBlocked
                );
            if (facts.GradleExport)
                return Denied(
                    AndroidBuildIntent.GradleExport,
                    AndroidBuildReasonCodes.GradleExportBlocked
                );
            if (facts.AppBundle)
                return Denied(
                    AndroidBuildIntent.AppBundle,
                    AndroidBuildReasonCodes.AppBundleBlocked
                );
            if (facts.SplitApkPerCpuArchitecture)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.SplitApkBlocked
                );
            if (facts.SplitApplicationBinary)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.SplitApplicationBinaryBlocked
                );
            if (!facts.NativeDebugSymbolsDisabled)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.NativeDebugSymbolsBlocked
                );
            if (!facts.DevelopmentBuild)
            {
                if (!facts.ProductionAndroidBuildCompiled)
                {
                    return Denied(
                        AndroidBuildIntent.ReleaseApk,
                        AndroidBuildReasonCodes.ProductionSwitchDisabled
                    );
                }

                if (!policy.allowReleaseApk ||
                    !ProductionApprovalsAreComplete(policy))
                {
                    return Denied(
                        AndroidBuildIntent.ReleaseApk,
                        AndroidBuildReasonCodes.ReleaseBuildBlocked
                    );
                }

                // Step 1 has no production build path even when a synthetic
                // policy object claims every approval. A later milestone must
                // make a reviewed source change to open that boundary.
                return Denied(
                    AndroidBuildIntent.ReleaseApk,
                    AndroidBuildReasonCodes.ReleaseBuildBlocked
                );
            }

            if (!policy.allowDevelopmentApk)
                return Denied(intent, AndroidBuildReasonCodes.PolicyInvalid);
            if (!facts.BootstrapSceneOnly)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.BootstrapSceneMismatch
                );
            if (!facts.DevelopmentIdentityMatches)
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.DevelopmentIdentityMismatch
                );
            if (!facts.DevelopmentVersionMatches)
                return Denied(intent, AndroidBuildReasonCodes.VersioningInvalid);
            if (!facts.TargetApiMatches)
                return Denied(intent, AndroidBuildReasonCodes.TargetApiMismatch);
            if (!facts.MinimumApiSupported)
            {
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.MinimumApiUnsupported
                );
            }
            if (policy.requireIl2Cpp && !facts.Il2CppEnabled)
                return Denied(intent, AndroidBuildReasonCodes.Il2CppRequired);
            if (policy.requireArm64 && !facts.Arm64Enabled)
                return Denied(intent, AndroidBuildReasonCodes.Arm64Required);
            if (policy.requireGameCategory && !facts.GameCategoryConfigured)
            {
                return Denied(
                    intent,
                    AndroidBuildReasonCodes.GameCategoryRequired
                );
            }
            if (policy.requirePortrait && !facts.PortraitConfigured)
                return Denied(intent, AndroidBuildReasonCodes.PortraitRequired);
            if (!facts.OutputPathApproved &&
                !facts.OutputPathValidationDeferred)
                return Denied(intent, AndroidBuildReasonCodes.OutputPathInvalid);

            return new AndroidBuildDecision(
                true,
                AndroidBuildIntent.DevelopmentApk,
                AndroidBuildReasonCodes.DevelopmentApkAllowed
            );
        }

        internal static AndroidBuildIntent Classify(AndroidBuildFacts facts)
        {
            if (facts == null || !facts.IsAndroidBuild)
                return AndroidBuildIntent.NonAndroid;
            if (facts.GradleExport)
                return AndroidBuildIntent.GradleExport;
            if (facts.AppBundle)
                return AndroidBuildIntent.AppBundle;
            return facts.DevelopmentBuild
                ? AndroidBuildIntent.DevelopmentApk
                : AndroidBuildIntent.ReleaseApk;
        }

        internal static bool ProductionApprovalsAreComplete(
            AndroidBuildFoundationPolicy policy)
        {
            return
                policy != null &&
                policy.productionApplicationIdentifierApproved &&
                policy.versioningApproved &&
                policy.sdkPolicyApproved &&
                policy.signingIdentityApproved &&
                policy.playAppSigningApproved &&
                policy.nativePluginInventoryApproved &&
                policy.sixteenKbPageCompatibilityVerified &&
                policy.dataSafetyApproved &&
                policy.productionBuildApproved;
        }

        static AndroidBuildDecision Denied(
            AndroidBuildIntent intent,
            string reasonCode)
        {
            return new AndroidBuildDecision(false, intent, reasonCode);
        }
    }
}
