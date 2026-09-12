namespace LegendsOfRus.Compliance
{
    public static class SupportExportPolicy
    {
        public const int CurrentSchemaVersion = 1;
        public const int MaximumTextLength = 192;

        public static bool IsSafeText(string value)
        {
            if (value == null || value.Length > MaximumTextLength)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '\0' || c == '\r' || c == '\n' || char.IsControl(c))
                    return false;
            }
            return true;
        }

        public static string SafeOrUnavailable(string value)
        {
            return IsSafeText(value) ? value : "unavailable";
        }
    }
}
