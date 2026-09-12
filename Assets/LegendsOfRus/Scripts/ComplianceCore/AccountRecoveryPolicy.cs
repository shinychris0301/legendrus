namespace LegendsOfRus.Compliance
{
    /// <summary>
    /// Format-only boundary for a future server-issued recovery code. Candidate
    /// codes are never persisted by this foundation and no backend is compiled.
    /// </summary>
    public static class AccountRecoveryPolicy
    {
        public const int MinimumCandidateLength = 8;
        public const int MaximumCandidateLength = 64;
        public const bool CandidatePersistenceAllowed = false;
        public const bool RecoveryBackendCompiled = false;
        public const bool AccountDeletionBackendCompiled = false;

        public static bool IsCandidateCodeValid(string value)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length < MinimumCandidateLength ||
                value.Length > MaximumCandidateLength)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool allowed =
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    c == '-';
                if (!allowed)
                    return false;
            }
            return true;
        }
    }
}
