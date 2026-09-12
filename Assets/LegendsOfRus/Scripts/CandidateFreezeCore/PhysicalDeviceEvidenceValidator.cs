using System;
using System.Collections.Generic;

namespace LegendsOfRus.CandidateFreeze
{
    public static class PhysicalDeviceEvidenceValidator
    {
        public static DeviceEvidenceValidationResult Validate(PhysicalDeviceEvidenceSet set, string expectedArtifactSha256)
        {
            var issues = new List<string>();
            int passed = 0;
            if (set == null)
            {
                return Result(false, 0, new[] { "DEVICE_EVIDENCE_SET_MISSING" });
            }
            if (set.SchemaVersion != CandidateFreezeRequirements.SchemaVersion)
                issues.Add("DEVICE_EVIDENCE_SCHEMA_UNSUPPORTED");
            if (!IsSha256(expectedArtifactSha256))
                issues.Add("DEVICE_EVIDENCE_EXPECTED_ARTIFACT_SHA_INVALID");
            if (!string.Equals(set.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase))
                issues.Add("DEVICE_EVIDENCE_SET_ARTIFACT_SHA_MISMATCH");

            var seen = new HashSet<string>(StringComparer.Ordinal);
            PhysicalDeviceEvidenceRecord[] records = set.Records ?? Array.Empty<PhysicalDeviceEvidenceRecord>();
            for (int i = 0; i < records.Length; i++)
            {
                PhysicalDeviceEvidenceRecord r = records[i];
                if (!IsRequiredProfile(r.ProfileId)) { issues.Add("DEVICE_PROFILE_UNKNOWN:" + Safe(r.ProfileId)); continue; }
                if (!seen.Add(r.ProfileId)) { issues.Add("DEVICE_PROFILE_DUPLICATE:" + r.ProfileId); continue; }
                if (!string.Equals(r.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase))
                    issues.Add("DEVICE_PROFILE_ARTIFACT_SHA_MISMATCH:" + r.ProfileId);
                if (r.ScreenWidth <= 0 || r.ScreenHeight <= 0 || r.ScreenHeight < r.ScreenWidth)
                    issues.Add("DEVICE_PROFILE_SCREEN_INVALID:" + r.ProfileId);
                if (!AspectMatches(r.ProfileId, r.ScreenWidth, r.ScreenHeight))
                    issues.Add("DEVICE_PROFILE_ASPECT_MISMATCH:" + r.ProfileId);
                if (r.TestDurationMinutes < CandidateFreezeRequirements.MinimumTestDurationMinutes)
                    issues.Add("DEVICE_PROFILE_DURATION_TOO_SHORT:" + r.ProfileId);
                if (!r.FreshInstallPassed) issues.Add("DEVICE_PROFILE_FRESH_INSTALL_FAILED:" + r.ProfileId);
                if (!r.CoreLoopPassed) issues.Add("DEVICE_PROFILE_CORE_LOOP_FAILED:" + r.ProfileId);
                if (!r.SuspendResumePassed) issues.Add("DEVICE_PROFILE_SUSPEND_RESUME_FAILED:" + r.ProfileId);
                if (!r.NoCrashObserved) issues.Add("DEVICE_PROFILE_CRASH_OBSERVED:" + r.ProfileId);
                if (!r.NoBlockingVisualDefectObserved) issues.Add("DEVICE_PROFILE_BLOCKING_VISUAL_DEFECT:" + r.ProfileId);
                if (!r.TesterAttested) issues.Add("DEVICE_PROFILE_ATTESTATION_MISSING:" + r.ProfileId);

                bool recordPass = string.Equals(r.ArtifactSha256, expectedArtifactSha256, StringComparison.OrdinalIgnoreCase) &&
                    r.ScreenWidth > 0 && r.ScreenHeight >= r.ScreenWidth && AspectMatches(r.ProfileId, r.ScreenWidth, r.ScreenHeight) &&
                    r.TestDurationMinutes >= CandidateFreezeRequirements.MinimumTestDurationMinutes && r.FreshInstallPassed &&
                    r.CoreLoopPassed && r.SuspendResumePassed && r.NoCrashObserved && r.NoBlockingVisualDefectObserved && r.TesterAttested;
                if (recordPass) passed++;
            }

            for (int i = 0; i < CandidateFreezeRequirements.RequiredProfileIds.Length; i++)
                if (!seen.Contains(CandidateFreezeRequirements.RequiredProfileIds[i]))
                    issues.Add("DEVICE_PROFILE_REQUIRED_MISSING:" + CandidateFreezeRequirements.RequiredProfileIds[i]);

            return Result(issues.Count == 0 && passed == CandidateFreezeRequirements.RequiredPhysicalProfileCount, passed, issues.ToArray());
        }

        static DeviceEvidenceValidationResult Result(bool valid, int passed, string[] issues) => new DeviceEvidenceValidationResult
        {
            Valid = valid,
            RequiredProfiles = CandidateFreezeRequirements.RequiredPhysicalProfileCount,
            PassedProfiles = passed,
            Issues = issues ?? Array.Empty<string>()
        };

        static bool IsRequiredProfile(string profileId)
        {
            for (int i = 0; i < CandidateFreezeRequirements.RequiredProfileIds.Length; i++)
                if (string.Equals(CandidateFreezeRequirements.RequiredProfileIds[i], profileId, StringComparison.Ordinal)) return true;
            return false;
        }

        static bool AspectMatches(string profileId, int width, int height)
        {
            if (width <= 0 || height <= 0) return false;
            double actual = (double)height / width;
            double expected = profileId.Contains("16_9") ? 16.0 / 9.0 :
                profileId.Contains("18_9") ? 18.0 / 9.0 :
                profileId.Contains("19_5_9") ? 19.5 / 9.0 : 20.0 / 9.0;
            return Math.Abs(actual - expected) <= 0.04;
        }

        static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))) return false;
            }
            return true;
        }

        static string Safe(string value) => string.IsNullOrEmpty(value) ? "missing" : value;
    }
}
