namespace LegendsOfRus.LaunchOperations
{
    public static class LaunchOperationsValidation
    {
        public static bool IsSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != LaunchOperationsRequirements.Sha256HexLength) return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool hex = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
                if (!hex) return false;
            }
            return true;
        }

        public static bool IsCandidateId(string value)
        {
            if (string.IsNullOrEmpty(value) || !value.StartsWith(LaunchOperationsRequirements.CandidatePrefix)) return false;
            string suffix = value.Substring(LaunchOperationsRequirements.CandidatePrefix.Length);
            if (suffix.Length != 2) return false;
            int n;
            return int.TryParse(suffix, out n) && n >= 1 && n <= 99;
        }

        public static bool IsPercent(double value) => value >= 0.0 && value <= 100.0;
    }
}
