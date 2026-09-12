using System;
using System.Collections.Generic;

namespace LegendsOfRus.ControlledTestBuild
{
    public static class ControlledTestBuildEvaluator
    {
        public static ControlledTestBuildResult Evaluate(ControlledTestBuildEvidence evidence)
        {
            var plan = new List<string>();
            var build = new List<string>();
            var upload = new List<string>();
            var warnings = new List<string>();

            Require(evidence.SourceStaticVerificationPassed, ControlledTestBuildBlockerCodes.StaticVerificationMissing, plan);
            Require(evidence.UnityCompilePassed, ControlledTestBuildBlockerCodes.UnityCompileMissing, plan);
            Require(evidence.EditorSelfTestPassed, ControlledTestBuildBlockerCodes.SelfTestMissing, plan);
            Require(evidence.EditModePassed, ControlledTestBuildBlockerCodes.EditModeMissing, plan);
            Require(evidence.PlayModePassed, ControlledTestBuildBlockerCodes.PlayModeMissing, plan);
            Require(evidence.RequiredDeviceProfiles > 0 && evidence.PassedDeviceProfiles >= evidence.RequiredDeviceProfiles,
                ControlledTestBuildBlockerCodes.DeviceMatrixIncomplete, plan);
            Require(evidence.PackageLockReviewed, ControlledTestBuildBlockerCodes.PackageLockUnreviewed, plan);
            Require(evidence.ProjectSettingsReviewed, ControlledTestBuildBlockerCodes.ProjectSettingsUnreviewed, plan);
            Require(evidence.TargetApiLevel >= ControlledTestBuildRequirements.RequiredTargetApiLevel,
                ControlledTestBuildBlockerCodes.TargetApiTooLow, plan);
            Require(evidence.AppBundleSelected, ControlledTestBuildBlockerCodes.AppBundleNotSelected, plan);
            Require(evidence.Il2CppEnabled, ControlledTestBuildBlockerCodes.Il2CppMissing, plan);
            Require(evidence.Arm64Enabled, ControlledTestBuildBlockerCodes.Arm64Missing, plan);
            Require(evidence.SixteenKbPageCompatibilityVerified,
                ControlledTestBuildBlockerCodes.SixteenKbEvidenceMissing, plan);

            build.AddRange(plan);
            Require(evidence.ProductionApplicationIdentifierApproved,
                ControlledTestBuildBlockerCodes.ProductionIdentityMissing, build);
            Require(evidence.VersioningApproved, ControlledTestBuildBlockerCodes.VersioningMissing, build);
            Require(evidence.PlayAppSigningApproved, ControlledTestBuildBlockerCodes.PlayAppSigningMissing, build);
            Require(evidence.UploadKeyWorkflowApproved, ControlledTestBuildBlockerCodes.UploadKeyWorkflowMissing, build);
            Require(evidence.ProductionSigningApproved, ControlledTestBuildBlockerCodes.ProductionSigningMissing, build);
            Require(evidence.ControlledTestBuildAuthorized,
                ControlledTestBuildBlockerCodes.ControlledBuildNotAuthorized, build);

            upload.AddRange(build);
            Require(evidence.SignedAppBundleEvidenceRetained,
                ControlledTestBuildBlockerCodes.ArtifactEvidenceMissing, upload);
            Require(evidence.StoreReleaseChecklistApproved,
                ControlledTestBuildBlockerCodes.StoreChecklistIncomplete, upload);
            Require(evidence.PrivacyDataSafetyApproved,
                ControlledTestBuildBlockerCodes.PrivacyDataSafetyMissing, upload);
            Require(evidence.ClosedTestUploadAuthorized,
                ControlledTestBuildBlockerCodes.ClosedTestUploadNotAuthorized, upload);
            Require(evidence.StorePublishingAuthorized,
                ControlledTestBuildBlockerCodes.PublishingNotAuthorized, upload);

            if (!evidence.PackageNameRegistrationReviewed)
                warnings.Add(ControlledTestBuildBlockerCodes.PackageRegistrationReviewPending);
            if (!evidence.RemoteBuildServiceApproved)
                warnings.Add(ControlledTestBuildBlockerCodes.RemoteBuildDisabled);

            return new ControlledTestBuildResult
            {
                MayPrepareReleaseCandidatePlan = plan.Count == 0,
                MayCreateSignedAppBundle = build.Count == 0,
                MayUploadClosedTest = upload.Count == 0,
                PlanBlockers = plan.ToArray(),
                BuildBlockers = build.ToArray(),
                UploadBlockers = upload.ToArray(),
                Warnings = warnings.ToArray()
            };
        }

        static void Require(bool condition, string code, List<string> blockers)
        {
            if (!condition) blockers.Add(code);
        }
    }
}
