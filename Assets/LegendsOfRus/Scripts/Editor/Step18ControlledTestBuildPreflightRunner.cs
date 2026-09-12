#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.ControlledTestBuild;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ControlledTestBuildPreflightRunner
    {
        const string ActivationPath = "Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json";
        const string SnapshotPath = "ProjectDocs/ClosedTestBuild/LocalPreflightSnapshot.json";

        [Serializable]
        sealed class Activation
        {
            public bool packageLockReviewed;
            public bool projectSettingsReviewed;
            public bool productionApplicationIdentifierApproved;
            public bool versioningApproved;
            public bool playAppSigningApproved;
            public bool uploadKeyWorkflowApproved;
            public bool productionSigningApproved;
            public bool controlledTestBuildAuthorized;
            public bool signedAppBundleEvidenceRetained;
            public bool closedTestUploadAuthorized;
            public bool storePublishingAuthorized;
            public bool packageNameRegistrationReviewed;
            public bool remoteBuildServiceApproved;
        }

        [Serializable]
        sealed class Snapshot
        {
            public string version = "v1.18-step04";
            public string requirementsSnapshotDate = ControlledTestBuildRequirements.RequirementsSnapshotDate;
            public int targetApiLevel;
            public bool appBundleSelected;
            public bool il2CppEnabled;
            public bool arm64Enabled;
            public bool mayPrepareReleaseCandidatePlan;
            public bool mayCreateSignedAppBundle;
            public bool mayUploadClosedTest;
            public string[] planBlockers;
            public string[] buildBlockers;
            public string[] uploadBlockers;
            public string[] warnings;
        }

        [MenuItem("Legends of Rus/QA/Print v1.18 Step 4 Controlled Test Build Preflight")]
        public static void Print()
        {
            Snapshot snapshot = BuildSnapshot();
            Debug.Log(JsonUtility.ToJson(snapshot, true));
        }

        [MenuItem("Legends of Rus/QA/Write v1.18 Step 4 Sanitized Preflight Snapshot")]
        public static void WriteSnapshot()
        {
            string full = Full(SnapshotPath);
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllText(full, JsonUtility.ToJson(BuildSnapshot(), true) + "\n");
            Debug.Log("Wrote sanitized Step 4 preflight snapshot: " + SnapshotPath +
                ". It contains no keystore path, passwords, device identifiers, or account credentials.");
        }

        static Snapshot BuildSnapshot()
        {
            Activation activation = LoadActivation();
            int targetApi = (int)PlayerSettings.Android.targetSdkVersion;
            bool il2Cpp = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) == ScriptingImplementation.IL2CPP;
            bool arm64 = (PlayerSettings.Android.targetArchitectures & AndroidArchitecture.ARM64) != 0;

            var evidence = new ControlledTestBuildEvidence
            {
                SourceStaticVerificationPassed = File.Exists(Full("Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP04.json")),
                UnityCompilePassed = false,
                EditorSelfTestPassed = false,
                EditModePassed = false,
                PlayModePassed = false,
                RequiredDeviceProfiles = 5,
                PassedDeviceProfiles = 0,
                PackageLockReviewed = activation.packageLockReviewed,
                ProjectSettingsReviewed = activation.projectSettingsReviewed,
                TargetApiLevel = targetApi,
                AppBundleSelected = EditorUserBuildSettings.buildAppBundle,
                Il2CppEnabled = il2Cpp,
                Arm64Enabled = arm64,
                SixteenKbPageCompatibilityVerified = false,
                ProductionApplicationIdentifierApproved = activation.productionApplicationIdentifierApproved,
                VersioningApproved = activation.versioningApproved,
                PlayAppSigningApproved = activation.playAppSigningApproved,
                UploadKeyWorkflowApproved = activation.uploadKeyWorkflowApproved,
                ProductionSigningApproved = activation.productionSigningApproved,
                ControlledTestBuildAuthorized = activation.controlledTestBuildAuthorized,
                SignedAppBundleEvidenceRetained = activation.signedAppBundleEvidenceRetained,
                StoreReleaseChecklistApproved = false,
                PrivacyDataSafetyApproved = false,
                ClosedTestUploadAuthorized = activation.closedTestUploadAuthorized,
                StorePublishingAuthorized = activation.storePublishingAuthorized,
                PackageNameRegistrationReviewed = activation.packageNameRegistrationReviewed,
                RemoteBuildServiceApproved = activation.remoteBuildServiceApproved
            };
            ControlledTestBuildResult result = ControlledTestBuildEvaluator.Evaluate(evidence);
            return new Snapshot
            {
                targetApiLevel = targetApi,
                appBundleSelected = evidence.AppBundleSelected,
                il2CppEnabled = il2Cpp,
                arm64Enabled = arm64,
                mayPrepareReleaseCandidatePlan = result.MayPrepareReleaseCandidatePlan,
                mayCreateSignedAppBundle = result.MayCreateSignedAppBundle,
                mayUploadClosedTest = result.MayUploadClosedTest,
                planBlockers = result.PlanBlockers,
                buildBlockers = result.BuildBlockers,
                uploadBlockers = result.UploadBlockers,
                warnings = result.Warnings
            };
        }

        static Activation LoadActivation()
        {
            string full = Full(ActivationPath);
            if (!File.Exists(full)) return new Activation();
            try { return JsonUtility.FromJson<Activation>(File.ReadAllText(full)) ?? new Activation(); }
            catch { return new Activation(); }
        }

        static string Full(string relative)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative));
        }
    }
}
#endif
