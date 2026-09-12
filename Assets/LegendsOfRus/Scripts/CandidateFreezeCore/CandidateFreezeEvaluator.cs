using System;
using System.Collections.Generic;

namespace LegendsOfRus.CandidateFreeze
{
    public static class CandidateFreezeEvaluator
    {
        public static CandidateFreezeResult Evaluate(CandidateFreezeEvidence evidence)
        {
            var blockers = new List<string>();
            Require(IsCandidateId(evidence.CandidateId), CandidateFreezeBlockerCodes.CandidateIdInvalid, blockers);
            Require(IsSha256(evidence.SourceTreeSha256), CandidateFreezeBlockerCodes.SourceTreeHashInvalid, blockers);
            Require(IsSha256(evidence.PackagesLockSha256), CandidateFreezeBlockerCodes.PackagesLockHashInvalid, blockers);
            Require(IsSha256(evidence.ProjectSettingsSha256), CandidateFreezeBlockerCodes.ProjectSettingsHashInvalid, blockers);
            Require(IsSha256(evidence.StoreChecklistSha256), CandidateFreezeBlockerCodes.StoreChecklistHashInvalid, blockers);
            Require(evidence.SignedArtifactEvidenceValid, CandidateFreezeBlockerCodes.ArtifactEvidenceInvalid, blockers);
            Require(evidence.DeviceEvidenceComplete, CandidateFreezeBlockerCodes.DeviceEvidenceIncomplete, blockers);
            Require(evidence.UnityCompilePassed && evidence.EditorSelfTestPassed && evidence.EditModePassed && evidence.PlayModePassed,
                CandidateFreezeBlockerCodes.UnityEvidenceMissing, blockers);
            Require(evidence.StoreReleaseChecklistApproved, CandidateFreezeBlockerCodes.StoreChecklistNotApproved, blockers);
            Require(evidence.PrivacyDataSafetyApproved, CandidateFreezeBlockerCodes.PrivacyDataSafetyNotApproved, blockers);
            Require(evidence.ProductionApplicationIdentifierApproved, CandidateFreezeBlockerCodes.ProductionIdentityNotApproved, blockers);
            Require(evidence.VersioningApproved, CandidateFreezeBlockerCodes.VersioningNotApproved, blockers);
            Require(evidence.ProductionSigningApproved, CandidateFreezeBlockerCodes.ProductionSigningNotApproved, blockers);
            Require(evidence.ControlledTestBuildAuthorized, CandidateFreezeBlockerCodes.ControlledBuildNotAuthorized, blockers);
            Require(evidence.CandidateFreezeAuthorized, CandidateFreezeBlockerCodes.FreezeNotAuthorized, blockers);
            return new CandidateFreezeResult { MayFreezeCandidate = blockers.Count == 0, Blockers = blockers.ToArray() };
        }

        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) return false;
            }
            return true;
        }

        public static bool IsCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(CandidateFreezeRequirements.CandidatePrefix, StringComparison.Ordinal)) return false;
            string suffix = value.Substring(CandidateFreezeRequirements.CandidatePrefix.Length);
            if (suffix.Length != 2) return false;
            return char.IsDigit(suffix[0]) && char.IsDigit(suffix[1]) && suffix != "00";
        }

        static void Require(bool condition, string code, List<string> blockers) { if (!condition) blockers.Add(code); }
    }
}
