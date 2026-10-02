using System;
using LegendsOfRus.CandidateFreeze;
using NUnit.Framework;

namespace LegendsOfRus.Tests.CandidateFreeze
{
    public sealed class CandidateFreezeCoreEditModeTests
    {
        [Test] public void RequiredPhysicalProfiles_AreFive() => Assert.That(CandidateFreezeRequirements.RequiredPhysicalProfileCount, Is.EqualTo(5));
        [Test] public void CandidateId_Rc01_IsValid() => Assert.That(CandidateFreezeEvaluator.IsCandidateId("v1.18-step05-rc01"), Is.True);
        [Test] public void CandidateId_Rc00_IsRejected() => Assert.That(CandidateFreezeEvaluator.IsCandidateId("v1.18-step05-rc00"), Is.False);
        [Test] public void DefaultFreezeEvidence_FailsClosed() => Assert.That(CandidateFreezeEvaluator.Evaluate(default).MayFreezeCandidate, Is.False);
        [Test] public void CompleteFreezeEvidence_CanFreeze() => Assert.That(CandidateFreezeEvaluator.Evaluate(CompleteFreeze()).MayFreezeCandidate, Is.True);
        [Test] public void MissingArtifactEvidence_BlocksFreeze() { var e=CompleteFreeze(); e.SignedArtifactEvidenceValid=false; HasBlock(e, CandidateFreezeBlockerCodes.ArtifactEvidenceInvalid); }
        [Test] public void MissingDeviceEvidence_BlocksFreeze() { var e=CompleteFreeze(); e.DeviceEvidenceComplete=false; HasBlock(e, CandidateFreezeBlockerCodes.DeviceEvidenceIncomplete); }
        [Test] public void MissingUnityEvidence_BlocksFreeze() { var e=CompleteFreeze(); e.PlayModePassed=false; HasBlock(e, CandidateFreezeBlockerCodes.UnityEvidenceMissing); }
        [Test] public void MissingFreezeAuthorization_BlocksFreeze() { var e=CompleteFreeze(); e.CandidateFreezeAuthorized=false; HasBlock(e, CandidateFreezeBlockerCodes.FreezeNotAuthorized); }
        [Test] public void ValidFiveProfileDeviceSet_Passes() => Assert.That(PhysicalDeviceEvidenceValidator.Validate(ValidSet(), Hash('a')).Valid, Is.True);
        [Test] public void DifferentArtifactHash_FailsDeviceSet() { var s=ValidSet(); s.Records[0].ArtifactSha256=Hash('b'); Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void DuplicateProfile_FailsDeviceSet() { var s=ValidSet(); s.Records[4].ProfileId=s.Records[0].ProfileId; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void CrashObserved_FailsDeviceSet() { var s=ValidSet(); s.Records[0].NoCrashObserved=false; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void BlockingVisualDefect_FailsDeviceSet() { var s=ValidSet(); s.Records[1].NoBlockingVisualDefectObserved=false; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void TooShortTest_FailsDeviceSet() { var s=ValidSet(); s.Records[2].TestDurationMinutes=1; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void WrongAspect_FailsDeviceSet() { var s=ValidSet(); s.Records[0].ScreenWidth=1000; s.Records[0].ScreenHeight=1000; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void MissingAttestation_FailsDeviceSet() { var s=ValidSet(); s.Records[3].TesterAttested=false; Assert.That(PhysicalDeviceEvidenceValidator.Validate(s, Hash('a')).Valid, Is.False); }
        [Test] public void ShaValidator_RejectsNonHex() => Assert.That(CandidateFreezeEvaluator.IsSha256(new string('z',64)), Is.False);

        static CandidateFreezeEvidence CompleteFreeze() => new CandidateFreezeEvidence
        {
            CandidateId="v1.18-step05-rc01", SourceTreeSha256=Hash('a'), PackagesLockSha256=Hash('b'), ProjectSettingsSha256=Hash('c'), StoreChecklistSha256=Hash('d'),
            SignedArtifactEvidenceValid=true, DeviceEvidenceComplete=true, UnityCompilePassed=true, EditorSelfTestPassed=true, EditModePassed=true, PlayModePassed=true,
            StoreReleaseChecklistApproved=true, PrivacyDataSafetyApproved=true, ProductionApplicationIdentifierApproved=true, VersioningApproved=true,
            ProductionSigningApproved=true, ControlledTestBuildAuthorized=true, CandidateFreezeAuthorized=true
        };
        static void HasBlock(CandidateFreezeEvidence e,string code) => Assert.That(Array.IndexOf(CandidateFreezeEvaluator.Evaluate(e).Blockers,code), Is.GreaterThanOrEqualTo(0));
        static string Hash(char c) => new string(c,64);
        static PhysicalDeviceEvidenceSet ValidSet()
        {
            string h=Hash('a');
            return new PhysicalDeviceEvidenceSet { ArtifactSha256=h, Records=new[]
            {
                R("android_low_16_9_720p",720,1280,h), R("android_mid_18_9_1080p",1080,2160,h),
                R("android_mid_19_5_9_1080p",1080,2340,h), R("android_high_20_9_1080p",1080,2400,h),
                R("android_high_20_9_1440p",1440,3200,h)
            }};
        }
        static PhysicalDeviceEvidenceRecord R(string id,int w,int h,string sha) => new PhysicalDeviceEvidenceRecord
        {
            ProfileId=id, ArtifactSha256=sha, ScreenWidth=w, ScreenHeight=h, TestDurationMinutes=10,
            FreshInstallPassed=true, CoreLoopPassed=true, SuspendResumePassed=true, NoCrashObserved=true,
            NoBlockingVisualDefectObserved=true, TesterAttested=true
        };
    }
}
