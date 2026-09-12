using System;

namespace LegendsOfRus.CandidateFreeze
{
    public static class CandidateFreezeBlockerCodes
    {
        public const string ArtifactEvidenceInvalid = "CF_ARTIFACT_EVIDENCE_INVALID";
        public const string DeviceEvidenceIncomplete = "CF_DEVICE_EVIDENCE_INCOMPLETE";
        public const string SourceTreeHashInvalid = "CF_SOURCE_TREE_HASH_INVALID";
        public const string PackagesLockHashInvalid = "CF_PACKAGES_LOCK_HASH_INVALID";
        public const string ProjectSettingsHashInvalid = "CF_PROJECT_SETTINGS_HASH_INVALID";
        public const string StoreChecklistHashInvalid = "CF_STORE_CHECKLIST_HASH_INVALID";
        public const string UnityEvidenceMissing = "CF_UNITY_EVIDENCE_MISSING";
        public const string StoreChecklistNotApproved = "CF_STORE_CHECKLIST_NOT_APPROVED";
        public const string PrivacyDataSafetyNotApproved = "CF_PRIVACY_DATA_SAFETY_NOT_APPROVED";
        public const string ProductionIdentityNotApproved = "CF_PRODUCTION_IDENTITY_NOT_APPROVED";
        public const string VersioningNotApproved = "CF_VERSIONING_NOT_APPROVED";
        public const string ProductionSigningNotApproved = "CF_PRODUCTION_SIGNING_NOT_APPROVED";
        public const string ControlledBuildNotAuthorized = "CF_CONTROLLED_TEST_BUILD_NOT_AUTHORIZED";
        public const string FreezeNotAuthorized = "CF_CANDIDATE_FREEZE_NOT_AUTHORIZED";
        public const string CandidateIdInvalid = "CF_CANDIDATE_ID_INVALID";
    }

    [Serializable]
    public struct PhysicalDeviceEvidenceRecord
    {
        public string ProfileId;
        public string ArtifactSha256;
        public int ScreenWidth;
        public int ScreenHeight;
        public int TestDurationMinutes;
        public bool FreshInstallPassed;
        public bool CoreLoopPassed;
        public bool SuspendResumePassed;
        public bool NoCrashObserved;
        public bool NoBlockingVisualDefectObserved;
        public bool TesterAttested;
    }

    [Serializable]
    public sealed class PhysicalDeviceEvidenceSet
    {
        public int SchemaVersion = CandidateFreezeRequirements.SchemaVersion;
        public string ArtifactSha256 = string.Empty;
        public PhysicalDeviceEvidenceRecord[] Records = Array.Empty<PhysicalDeviceEvidenceRecord>();
    }

    [Serializable]
    public sealed class DeviceEvidenceValidationResult
    {
        public bool Valid;
        public int RequiredProfiles;
        public int PassedProfiles;
        public string[] Issues = Array.Empty<string>();
    }

    [Serializable]
    public struct CandidateFreezeEvidence
    {
        public string CandidateId;
        public string SourceTreeSha256;
        public string PackagesLockSha256;
        public string ProjectSettingsSha256;
        public string StoreChecklistSha256;
        public bool SignedArtifactEvidenceValid;
        public bool DeviceEvidenceComplete;
        public bool UnityCompilePassed;
        public bool EditorSelfTestPassed;
        public bool EditModePassed;
        public bool PlayModePassed;
        public bool StoreReleaseChecklistApproved;
        public bool PrivacyDataSafetyApproved;
        public bool ProductionApplicationIdentifierApproved;
        public bool VersioningApproved;
        public bool ProductionSigningApproved;
        public bool ControlledTestBuildAuthorized;
        public bool CandidateFreezeAuthorized;
    }

    [Serializable]
    public sealed class CandidateFreezeResult
    {
        public bool MayFreezeCandidate;
        public string[] Blockers = Array.Empty<string>();
    }

    [Serializable]
    public sealed class CandidateFreezeManifest
    {
        public int SchemaVersion = CandidateFreezeRequirements.SchemaVersion;
        public string CandidateId = string.Empty;
        public string SourceTreeSha256 = string.Empty;
        public string PackagesLockSha256 = string.Empty;
        public string ProjectSettingsSha256 = string.Empty;
        public string StoreChecklistSha256 = string.Empty;
        public string ArtifactFileName = string.Empty;
        public string ArtifactSha256 = string.Empty;
        public long ArtifactSizeBytes;
        public string VersionName = string.Empty;
        public int VersionCode;
        public int TargetApiLevel;
        public string DeviceEvidenceSha256 = string.Empty;
        public int RequiredDeviceProfiles = CandidateFreezeRequirements.RequiredPhysicalProfileCount;
        public int PassedDeviceProfiles;
        public bool Frozen;
    }
}
