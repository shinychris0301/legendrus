#if UNITY_EDITOR
using System;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Runtime consistency check for the accepted production cutover allowlist.
    /// Avoids compile-time constant comparisons while verifying both the declared
    /// count and the actual ScreenId routes handled by IsCutover.
    /// </summary>
    public static class ProductionCutoverFreezeGuard
    {
        public static bool MatchesExpectedCount(int expectedCount)
        {
            if (ProductionCoreLoopPageRegistry.CutoverPageCount != expectedCount)
                return false;

            int routedCount = 0;
            Array values = Enum.GetValues(typeof(ScreenId));
            for (int i = 0; i < values.Length; i++)
            {
                ScreenId id = (ScreenId)values.GetValue(i);
                if (ProductionCoreLoopPageRegistry.IsCutover(id))
                    routedCount++;
            }

            return routedCount == expectedCount;
        }
    }
}
#endif
