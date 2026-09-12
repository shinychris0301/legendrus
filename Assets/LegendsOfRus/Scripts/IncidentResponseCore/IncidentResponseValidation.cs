namespace LegendsOfRus.IncidentResponse
{
    public static class IncidentResponseValidation
    {
        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != IncidentResponseRequirements.Sha256HexLength) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool hex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }

        public static bool IsIncidentId(string value)
        {
            return IsFixedNumericId(value, IncidentResponseRequirements.IncidentIdPrefix, IncidentResponseRequirements.NumericIdDigits);
        }

        public static bool IsHotfixId(string value)
        {
            return IsFixedNumericId(value, IncidentResponseRequirements.HotfixIdPrefix, IncidentResponseRequirements.NumericIdDigits);
        }

        public static bool IsHotfixCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(IncidentResponseRequirements.HotfixCandidatePrefix)) return false;
            string suffix = value.Substring(IncidentResponseRequirements.HotfixCandidatePrefix.Length);
            if (suffix.Length != IncidentResponseRequirements.HotfixCandidateDigits) return false;
            int n;
            return int.TryParse(suffix, out n) && n >= 1 && n <= 99;
        }

        public static bool IsBaseCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 96) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool ok = char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_';
                if (!ok) return false;
            }
            return true;
        }

        public static bool IsRolloutPercent(int value)
        {
            return value >= IncidentResponseRequirements.MinimumRolloutPercent && value <= IncidentResponseRequirements.MaximumRolloutPercent;
        }

        static bool IsFixedNumericId(string value, string prefix, int digits)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(prefix)) return false;
            string suffix = value.Substring(prefix.Length);
            if (suffix.Length != digits) return false;
            for (int i = 0; i < suffix.Length; i++) if (suffix[i] < '0' || suffix[i] > '9') return false;
            int n;
            return int.TryParse(suffix, out n) && n >= 1;
        }
    }
}
