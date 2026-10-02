using System;
using LegendsOfRus.ControlledTestBuild;
using NUnit.Framework;

namespace LegendsOfRus.Tests.ControlledTestBuild
{
    public sealed class ControlledTestBuildCoreEditModeTests
    {
        [Test] public void Requirements_TargetApi_Is36() => Assert.That(ControlledTestBuildRequirements.RequiredTargetApiLevel, Is.EqualTo(36));
        [Test] public void DefaultEvidence_FailsClosed() => Assert.That(ControlledTestBuildEvaluator.Evaluate(default).MayPrepareReleaseCandidatePlan, Is.False);
        [Test] public void TechnicalEvidence_AllowsPlanOnly()
        {
            var r = ControlledTestBuildEvaluator.Evaluate(Technical());
            Assert.That(r.MayPrepareReleaseCandidatePlan, Is.True);
            Assert.That(r.MayCreateSignedAppBundle, Is.False);
            Assert.That(r.MayUploadClosedTest, Is.False);
        }
        [Test] public void TargetApi35_IsBlocked()
        {
            var e=Technical(); e.TargetApiLevel=35;
            Assert.That(Array.IndexOf(ControlledTestBuildEvaluator.Evaluate(e).PlanBlockers, ControlledTestBuildBlockerCodes.TargetApiTooLow), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void MissingAab_IsBlocked()
        {
            var e=Technical(); e.AppBundleSelected=false;
            Assert.That(Array.IndexOf(ControlledTestBuildEvaluator.Evaluate(e).PlanBlockers, ControlledTestBuildBlockerCodes.AppBundleNotSelected), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void MissingArm64_IsBlocked()
        {
            var e=Technical(); e.Arm64Enabled=false;
            Assert.That(Array.IndexOf(ControlledTestBuildEvaluator.Evaluate(e).PlanBlockers, ControlledTestBuildBlockerCodes.Arm64Missing), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void Missing16KbEvidence_IsBlocked()
        {
            var e=Technical(); e.SixteenKbPageCompatibilityVerified=false;
            Assert.That(Array.IndexOf(ControlledTestBuildEvaluator.Evaluate(e).PlanBlockers, ControlledTestBuildBlockerCodes.SixteenKbEvidenceMissing), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void SigningApproval_IsRequiredForAabCreation()
        {
            Assert.That(Array.IndexOf(ControlledTestBuildEvaluator.Evaluate(Technical()).BuildBlockers, ControlledTestBuildBlockerCodes.ProductionSigningMissing), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void FullBuildApprovals_AllowSignedAabGate()
        {
            var e=BuildReady(); var r=ControlledTestBuildEvaluator.Evaluate(e);
            Assert.That(r.MayCreateSignedAppBundle, Is.True); Assert.That(r.MayUploadClosedTest, Is.False);
        }
        [Test] public void ArtifactEvidence_IsRequiredForUpload()
        {
            var r=ControlledTestBuildEvaluator.Evaluate(BuildReady());
            Assert.That(Array.IndexOf(r.UploadBlockers, ControlledTestBuildBlockerCodes.ArtifactEvidenceMissing), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void FullUploadEvidence_AllowsClosedTestUpload()
        {
            var e=BuildReady(); e.SignedAppBundleEvidenceRetained=true; e.StoreReleaseChecklistApproved=true; e.PrivacyDataSafetyApproved=true;
            e.ClosedTestUploadAuthorized=true; e.StorePublishingAuthorized=true;
            Assert.That(ControlledTestBuildEvaluator.Evaluate(e).MayUploadClosedTest, Is.True);
        }
        [Test] public void PackageRegistrationReview_IsWarningNotCurrentBuildBlocker()
        {
            var r=ControlledTestBuildEvaluator.Evaluate(Technical());
            Assert.That(Array.IndexOf(r.Warnings, ControlledTestBuildBlockerCodes.PackageRegistrationReviewPending), Is.GreaterThanOrEqualTo(0));
        }
        [Test] public void ValidAabEvidence_Passes()
        {
            Assert.That(ControlledTestArtifactValidator.Validate(ValidArtifact()).Valid, Is.True);
        }
        [Test] public void ApkEvidence_Fails()
        {
            var a=ValidArtifact(); a.FileName="test.apk";
            Assert.That(ControlledTestArtifactValidator.Validate(a).Valid, Is.False);
        }
        [Test] public void InvalidSha_Fails()
        {
            var a=ValidArtifact(); a.Sha256="123";
            Assert.That(ControlledTestArtifactValidator.Validate(a).Valid, Is.False);
        }
        [Test] public void Api35Artifact_Fails()
        {
            var a=ValidArtifact(); a.TargetApiLevel=35;
            Assert.That(ControlledTestArtifactValidator.Validate(a).Valid, Is.False);
        }

        static ControlledTestBuildEvidence Technical() => new ControlledTestBuildEvidence
        {
            SourceStaticVerificationPassed=true, UnityCompilePassed=true, EditorSelfTestPassed=true, EditModePassed=true, PlayModePassed=true,
            RequiredDeviceProfiles=5, PassedDeviceProfiles=5, PackageLockReviewed=true, ProjectSettingsReviewed=true, TargetApiLevel=36,
            AppBundleSelected=true, Il2CppEnabled=true, Arm64Enabled=true, SixteenKbPageCompatibilityVerified=true
        };
        static ControlledTestBuildEvidence BuildReady()
        {
            var e=Technical(); e.ProductionApplicationIdentifierApproved=true; e.VersioningApproved=true; e.PlayAppSigningApproved=true;
            e.UploadKeyWorkflowApproved=true; e.ProductionSigningApproved=true; e.ControlledTestBuildAuthorized=true; return e;
        }
        static ControlledTestArtifactEvidence ValidArtifact() => new ControlledTestArtifactEvidence
        {
            FileName="LegendsOfRus.aab", Sha256=new string('b',64), SizeBytes=1234, VersionName="0.1.0", VersionCode=2,
            TargetApiLevel=36, Signed=true, AppBundle=true, Arm64Included=true, SixteenKbPageCompatibilityVerified=true
        };
    }
}
