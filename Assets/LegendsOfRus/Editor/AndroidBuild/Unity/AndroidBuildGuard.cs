#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LegendsOfRus.AndroidBuild
{
    internal sealed class AndroidBuildPreflightResult
    {
        internal AndroidBuildPreflightResult(
            AndroidBuildPolicyLoadResult policyLoad,
            AndroidBuildFacts facts,
            AndroidBuildDecision decision)
        {
            PolicyLoad = policyLoad;
            Facts = facts;
            Decision = decision;
        }

        internal AndroidBuildPolicyLoadResult PolicyLoad { get; private set; }
        internal AndroidBuildFacts Facts { get; private set; }
        internal AndroidBuildDecision Decision { get; private set; }
    }

    /// <summary>
    /// Global GUI, command-line, and batch-mode Android build boundary.
    /// Step 1 defines one fixed Development APK contract that remains blocked
    /// until a reviewed, lock-pinned follow-up approves it. No
    /// production-sensitive Android artifact is permitted.
    /// </summary>
    public sealed class AndroidBuildGuard : IPreprocessBuildWithReport
    {
        private const bool ProductionAndroidBuildCompiled = false;

        public int callbackOrder
        {
            get { return int.MaxValue; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report != null && report.summary.platform != BuildTarget.Android)
                return;

            try
            {
                AndroidBuildPolicyLoadResult policyLoad =
                    AndroidBuildPolicyLoader.Load();
                AndroidBuildFacts facts = AndroidBuildStateReader.ReadForBuild(
                    report,
                    policyLoad,
                    ProductionAndroidBuildCompiled
                );
                AndroidBuildDecision decision =
                    AndroidBuildPolicyEvaluator.Evaluate(
                        policyLoad != null ? policyLoad.Policy : null,
                        facts
                    );
                if (!decision.Allowed)
                    throw Denied(decision.ReasonCode);

                Debug.Log(
                    "Legends of Rus Android build guard: " +
                    AndroidBuildReasonCodes.DevelopmentApkAllowed +
                    ". Debug-signed development verification only."
                );
            }
            catch (BuildFailedException)
            {
                throw;
            }
            catch
            {
                throw Denied(AndroidBuildReasonCodes.UnexpectedFailure);
            }
        }

        internal static AndroidBuildPreflightResult EvaluateDevelopmentPreflight()
        {
            try
            {
                AndroidBuildPolicyLoadResult policyLoad =
                    AndroidBuildPolicyLoader.Load();
                AndroidBuildFacts facts =
                    AndroidBuildStateReader.ReadForPreflight(
                        policyLoad,
                        ProductionAndroidBuildCompiled
                    );
                AndroidBuildDecision decision =
                    AndroidBuildPolicyEvaluator.Evaluate(
                        policyLoad != null ? policyLoad.Policy : null,
                        facts
                    );
                return new AndroidBuildPreflightResult(
                    policyLoad,
                    facts,
                    decision
                );
            }
            catch
            {
                var facts = new AndroidBuildFacts
                {
                    IsAndroidBuild = true,
                    SettingsReadSucceeded = false,
                    ProductionAndroidBuildCompiled = false
                };
                return new AndroidBuildPreflightResult(
                    null,
                    facts,
                    new AndroidBuildDecision(
                        false,
                        AndroidBuildIntent.DevelopmentApk,
                        AndroidBuildReasonCodes.UnexpectedFailure
                    )
                );
            }
        }

        // Uses the scheduled options rather than the possibly different Build Profiles UI state.
        internal static AndroidBuildPreflightResult EvaluateScheduledPreflight(
            BuildPlayerOptions options)
        {
            try
            {
                AndroidBuildPolicyLoadResult policyLoad = AndroidBuildPolicyLoader.Load();
                AndroidBuildFacts facts = AndroidBuildStateReader.ReadForScheduledBuild(
                    options, policyLoad, ProductionAndroidBuildCompiled);
                AndroidBuildDecision decision = AndroidBuildPolicyEvaluator.Evaluate(
                    policyLoad != null ? policyLoad.Policy : null, facts);
                return new AndroidBuildPreflightResult(policyLoad, facts, decision);
            }
            catch
            {
                return new AndroidBuildPreflightResult(null,
                    new AndroidBuildFacts { IsAndroidBuild = true, SettingsReadSucceeded = false },
                    new AndroidBuildDecision(false, AndroidBuildIntent.DevelopmentApk,
                        AndroidBuildReasonCodes.UnexpectedFailure));
            }
        }

        internal static BuildFailedException Denied(string reasonCode)
        {
            return new BuildFailedException(
                "Legends of Rus Android build denied: " + reasonCode +
                ". " + AndroidBuildFix03Diagnostics.ExplainReason(reasonCode) +
                " No credential, keystore path, alias, or password was read or logged."
            );
        }
    }

    /// <summary>
    /// Reads the scheduled Player build options before compilation so
    /// build-only scripting defines cannot bypass the persisted settings
    /// checks in AndroidBuildGuard.
    /// </summary>
    public sealed class AndroidBuildPlayerOptionsGuard : BuildPlayerProcessor
    {
        public override int callbackOrder
        {
            get { return int.MaxValue; }
        }

        public override void PrepareForBuild(
            BuildPlayerContext buildPlayerContext)
        {
            if (buildPlayerContext == null)
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.SettingsReadFailed
                );

            try
            {
                BuildPlayerOptions options =
                    buildPlayerContext.BuildPlayerOptions;
                if (options.target != BuildTarget.Android)
                    return;

                AndroidDevelopmentBuildOptions.RememberScheduled(options.options);
                AndroidBuildPolicyLoadResult policyLoad =
                    AndroidBuildPolicyLoader.Load();
                if (policyLoad == null || !policyLoad.FilePresent)
                {
                    throw AndroidBuildGuard.Denied(
                        AndroidBuildReasonCodes.PolicyMissing
                    );
                }
                if (!policyLoad.HashMatches)
                {
                    throw AndroidBuildGuard.Denied(
                        AndroidBuildReasonCodes.PolicyHashMismatch
                    );
                }
                if (!policyLoad.IsValid || policyLoad.Policy == null)
                {
                    throw AndroidBuildGuard.Denied(
                        AndroidBuildReasonCodes.PolicyInvalid
                    );
                }

                ValidateOptions(options, policyLoad.Policy);
                if (!AndroidBuildStateReader.CustomBuildProfileAbsent())
                {
                    throw AndroidBuildGuard.Denied(
                        AndroidBuildReasonCodes.BuildProfileOverrideBlocked
                    );
                }
            }
            catch (BuildFailedException)
            {
                throw;
            }
            catch
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.UnexpectedFailure
                );
            }
        }

        internal static void ValidateOptions(
            BuildPlayerOptions options,
            AndroidBuildFoundationPolicy policy)
        {
            if (options.target != BuildTarget.Android)
                return;

            if (options.extraScriptingDefines != null &&
                options.extraScriptingDefines.Length != 0)
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.ScriptingDefinesUnsafe
                );
            }

            if (policy == null)
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.PolicyInvalid
                );

            if (options.targetGroup != BuildTargetGroup.Android ||
                options.subtarget != 0)
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.ScheduledTargetMismatch
                );
            }

            // A Development build is an intent, not one exact numerical combination.
            // LZ4/LZ4HC and StrictMode do not turn it into a release or auto-run build.
            AndroidDevelopmentBuildOptions.Validate(options.options);

            if (options.scenes == null ||
                options.scenes.Length != 1 ||
                !string.Equals(options.scenes[0], policy.requiredScene,
                    System.StringComparison.Ordinal))
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.BootstrapSceneMismatch
                );
            }

            if (!string.IsNullOrEmpty(options.assetBundleManifestPath))
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.AssetBundleManifestBlocked
                );
            }

            if (!AndroidBuildStateReader.ScheduledOutputIsExact(
                options.locationPathName,
                policy))
            {
                throw AndroidBuildGuard.Denied(
                    AndroidBuildReasonCodes.OutputPathInvalid
                );
            }
        }
    }
}
#endif
