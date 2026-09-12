namespace LegendsOfRus
{
    /// <summary>
    /// Explicit allowlist for the first screenshot-to-production cutover wave.
    /// Screens not listed here continue using the inherited DesignScreenRegistry
    /// until their own reconstruction checkpoint.
    /// </summary>
    public static class ProductionCoreLoopPageRegistry
    {
        public const int CutoverPageCount = 10;

        public static bool IsCutover(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.Loading:
                case ScreenId.Home:
                case ScreenId.WorldMap:
                case ScreenId.ChapterMap:
                case ScreenId.PreLevel:
                case ScreenId.Gameplay:
                case ScreenId.Pause:
                case ScreenId.Victory:
                case ScreenId.Fail:
                case ScreenId.ChapterComplete:
                    return true;
                default:
                    return false;
            }
        }

        public static string ResourceKey(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.Loading: return "ProductionUI/Pages/Loading";
                case ScreenId.Home: return "ProductionUI/Pages/Home";
                case ScreenId.WorldMap: return "ProductionUI/Pages/WorldMap";
                case ScreenId.ChapterMap: return "ProductionUI/Pages/ChapterMap";
                case ScreenId.PreLevel: return "ProductionUI/Pages/PreLevel";
                case ScreenId.Gameplay: return "ProductionUI/Pages/Gameplay";
                case ScreenId.Pause: return "ProductionUI/Pages/Pause";
                case ScreenId.Victory: return "ProductionUI/Pages/Victory";
                case ScreenId.Fail: return "ProductionUI/Pages/Fail";
                case ScreenId.ChapterComplete: return "ProductionUI/Pages/ChapterComplete";
                default: return string.Empty;
            }
        }
    }
}
