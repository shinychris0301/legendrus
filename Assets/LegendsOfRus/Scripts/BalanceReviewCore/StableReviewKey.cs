namespace LegendsOfRus.BalanceReview
{
    public static class StableReviewKey
    {
        // Deterministic FNV-1a over authored non-personal identifiers. The
        // buffer stores only this integer key, never free-form strings.
        public static int FromAuthoredId(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            unchecked
            {
                uint hash = 2166136261u;
                for (int i = 0; i < value.Length; i++)
                {
                    char c = value[i];
                    hash ^= (byte)(c & 0xff);
                    hash *= 16777619u;
                    hash ^= (byte)((c >> 8) & 0xff);
                    hash *= 16777619u;
                }
                int result = (int)(hash & 0x7fffffff);
                return result == 0 ? 1 : result;
            }
        }
    }
}
