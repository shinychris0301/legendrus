using System;

namespace LegendsOfRus.LaunchCandidate
{
    public static class LaunchCandidateValidation
    {
        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != LaunchCandidateRequirements.Sha256HexLength) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool digit = c >= '0' && c <= '9';
                bool lower = c >= 'a' && c <= 'f';
                bool upper = c >= 'A' && c <= 'F';
                if (!digit && !lower && !upper) return false;
            }
            return true;
        }

        public static bool IsCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(LaunchCandidateRequirements.CandidatePrefix, StringComparison.Ordinal)) return false;
            string suffix = value.Substring(LaunchCandidateRequirements.CandidatePrefix.Length);
            int n;
            return suffix.Length == 2 && int.TryParse(suffix, out n) && n > 0;
        }
    }
}
