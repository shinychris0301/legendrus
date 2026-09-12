using System;
using System.Collections.Generic;

namespace LegendsOfRus.ControlledTestBuild
{
    public static class ControlledTestArtifactValidator
    {
        public static ControlledTestArtifactValidationResult Validate(ControlledTestArtifactEvidence evidence)
        {
            var issues = new List<string>();
            if (string.IsNullOrWhiteSpace(evidence.FileName) || !evidence.FileName.EndsWith(".aab", StringComparison.OrdinalIgnoreCase))
                issues.Add("ARTIFACT_FILENAME_NOT_AAB");
            if (!IsSha256(evidence.Sha256)) issues.Add("ARTIFACT_SHA256_INVALID");
            if (evidence.SizeBytes <= 0) issues.Add("ARTIFACT_SIZE_INVALID");
            if (string.IsNullOrWhiteSpace(evidence.VersionName)) issues.Add("ARTIFACT_VERSION_NAME_MISSING");
            if (evidence.VersionCode <= 0) issues.Add("ARTIFACT_VERSION_CODE_INVALID");
            if (evidence.TargetApiLevel < ControlledTestBuildRequirements.RequiredTargetApiLevel)
                issues.Add("ARTIFACT_TARGET_API_BELOW_36");
            if (!evidence.Signed) issues.Add("ARTIFACT_NOT_SIGNED");
            if (!evidence.AppBundle) issues.Add("ARTIFACT_NOT_APP_BUNDLE");
            if (!evidence.Arm64Included) issues.Add("ARTIFACT_ARM64_MISSING");
            if (!evidence.SixteenKbPageCompatibilityVerified) issues.Add("ARTIFACT_16KB_NOT_VERIFIED");
            return new ControlledTestArtifactValidationResult { Valid = issues.Count == 0, Issues = issues.ToArray() };
        }

        static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool hex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }
    }
}
