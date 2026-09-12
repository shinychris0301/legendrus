using System;
using System.Collections.Generic;

namespace LegendsOfRus.QA
{
    public static class ClosedTestPreflightCodes
    {
        public const string StaticVerificationMissing = "QA_STATIC_VERIFICATION_MISSING";
        public const string UnityCompileMissing = "QA_UNITY_COMPILE_EVIDENCE_MISSING";
        public const string SelfTestMissing = "QA_EDITOR_SELF_TEST_EVIDENCE_MISSING";
        public const string EditModeMissing = "QA_EDITMODE_EVIDENCE_MISSING";
        public const string PlayModeMissing = "QA_PLAYMODE_EVIDENCE_MISSING";
        public const string DeviceMatrixIncomplete = "QA_DEVICE_MATRIX_INCOMPLETE";
        public const string AndroidBuildEvidenceMissing = "QA_ANDROID_BUILD_EVIDENCE_MISSING";
        public const string PrivacyDataSafetyMissing = "QA_PRIVACY_DATA_SAFETY_NOT_APPROVED";
        public const string StoreListingMissing = "QA_STORE_LISTING_NOT_APPROVED";
        public const string ProductionSigningMissing = "QA_PRODUCTION_SIGNING_NOT_APPROVED";
        public const string ControlledBuildNotAuthorized = "QA_CONTROLLED_TEST_BUILD_NOT_AUTHORIZED";
        public const string PublishingNotAuthorized = "QA_PUBLISHING_NOT_AUTHORIZED";
        public const string RemoteQaUploadDisabled = "QA_REMOTE_UPLOAD_DISABLED_LOCAL_ONLY";
        public const string ProductionCommercialDisabled = "QA_PRODUCTION_COMMERCIAL_DISABLED_SANDBOX_ONLY";
    }

    [Serializable]
    public struct ClosedTestPreflightEvidence
    {
        public bool SourceStaticVerificationPassed;
        public bool UnityCompilePassed;
        public bool EditorSelfTestPassed;
        public bool EditModePassed;
        public bool PlayModePassed;
        public int RequiredDeviceProfiles;
        public int PassedDeviceProfiles;
        public bool AndroidDevelopmentBuildEvidenceRetained;
        public bool PrivacyDataSafetyApproved;
        public bool StoreListingAssetsApproved;
        public bool ProductionSigningApproved;
        public bool ControlledTestBuildAuthorized;
        public bool PublishingAuthorized;
        public bool RemoteQaUploadEnabled;
        public bool ProductionCommercialActivationEnabled;
    }

    [Serializable]
    public sealed class ClosedTestPreflightResult
    {
        public bool MayBuildControlledTest;
        public bool MayPublishClosedTest;
        public string[] Blockers = Array.Empty<string>();
        public string[] Warnings = Array.Empty<string>();
    }

    public static class ClosedTestPreflightEvaluator
    {
        public static ClosedTestPreflightResult Evaluate(
            ClosedTestPreflightEvidence evidence)
        {
            var buildBlockers = new List<string>();
            var warnings = new List<string>();

            Require(evidence.SourceStaticVerificationPassed,
                ClosedTestPreflightCodes.StaticVerificationMissing, buildBlockers);
            Require(evidence.UnityCompilePassed,
                ClosedTestPreflightCodes.UnityCompileMissing, buildBlockers);
            Require(evidence.EditorSelfTestPassed,
                ClosedTestPreflightCodes.SelfTestMissing, buildBlockers);
            Require(evidence.EditModePassed,
                ClosedTestPreflightCodes.EditModeMissing, buildBlockers);
            Require(evidence.PlayModePassed,
                ClosedTestPreflightCodes.PlayModeMissing, buildBlockers);
            Require(
                evidence.RequiredDeviceProfiles > 0 &&
                evidence.PassedDeviceProfiles >= evidence.RequiredDeviceProfiles,
                ClosedTestPreflightCodes.DeviceMatrixIncomplete,
                buildBlockers);
            Require(evidence.AndroidDevelopmentBuildEvidenceRetained,
                ClosedTestPreflightCodes.AndroidBuildEvidenceMissing, buildBlockers);
            Require(evidence.PrivacyDataSafetyApproved,
                ClosedTestPreflightCodes.PrivacyDataSafetyMissing, buildBlockers);
            Require(evidence.StoreListingAssetsApproved,
                ClosedTestPreflightCodes.StoreListingMissing, buildBlockers);
            Require(evidence.ProductionSigningApproved,
                ClosedTestPreflightCodes.ProductionSigningMissing, buildBlockers);
            Require(evidence.ControlledTestBuildAuthorized,
                ClosedTestPreflightCodes.ControlledBuildNotAuthorized, buildBlockers);

            if (!evidence.RemoteQaUploadEnabled)
                warnings.Add(ClosedTestPreflightCodes.RemoteQaUploadDisabled);
            if (!evidence.ProductionCommercialActivationEnabled)
                warnings.Add(ClosedTestPreflightCodes.ProductionCommercialDisabled);

            bool mayBuild = buildBlockers.Count == 0;
            var allBlockers = new List<string>(buildBlockers);
            if (!evidence.PublishingAuthorized)
                allBlockers.Add(ClosedTestPreflightCodes.PublishingNotAuthorized);

            return new ClosedTestPreflightResult
            {
                MayBuildControlledTest = mayBuild,
                MayPublishClosedTest = mayBuild && evidence.PublishingAuthorized,
                Blockers = allBlockers.ToArray(),
                Warnings = warnings.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition)
                blockers.Add(code);
        }
    }
}
