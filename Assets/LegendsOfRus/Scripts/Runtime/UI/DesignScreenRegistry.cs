using System.Collections.Generic;

namespace LegendsOfRus
{
    public static class DesignScreenRegistry
    {
        static readonly Dictionary<ScreenId, string> Keys = new Dictionary<ScreenId, string>
        {
            { ScreenId.Loading, "DesignScreens/Loading" },
            { ScreenId.Home, "DesignScreens/Home" },
            { ScreenId.WorldMap, "DesignScreens/WorldMap" },
            { ScreenId.ChapterMap, "DesignScreens/ChapterMap" },
            { ScreenId.PreLevel, "DesignScreens/PreLevel" },
            { ScreenId.Gameplay, "DesignScreens/Gameplay" },
            { ScreenId.Dialogue, "DesignScreens/Dialogue" },
            { ScreenId.Victory, "DesignScreens/Victory" },
            { ScreenId.Fail, "DesignScreens/Fail" },
            { ScreenId.Shop, "DesignScreens/Shop" },
            { ScreenId.Chapter15, "DesignScreens/Chapter15" },

            { ScreenId.DailyReward, "DesignScreens/DailyReward" },
            { ScreenId.RewardClaim, "DesignScreens/RewardClaim" },
            { ScreenId.BoosterSelect, "DesignScreens/BoosterSelect" },
            { ScreenId.Events, "DesignScreens/Events" },
            { ScreenId.Profile, "DesignScreens/Profile" },
            { ScreenId.Settings, "DesignScreens/Settings" },
            { ScreenId.Pause, "DesignScreens/Pause" },
            { ScreenId.Characters, "DesignScreens/Characters" },
            { ScreenId.Mail, "DesignScreens/Mail" },
            { ScreenId.OutOfEnergy, "DesignScreens/OutOfEnergy" },
            { ScreenId.ChapterComplete, "DesignScreens/ChapterComplete" },
        };

        public static string ResourceKey(ScreenId id)
        {
            // v1.20 Step 8.5: Shop can be switched to a separately gated, rollback-safe
            // production prefab without changing the accepted 10-route core registry.
            // Returning an empty key lets ScreenRouter delegate to DemoFunctionalPageRuntime,
            // which in turn dispatches the active meta-page cutover through ProductionMetaPageRuntime.
            if (ProductionMetaPageCutoverRegistry.IsCutover(id))
                return string.Empty;

            return Keys.TryGetValue(id, out var key) ? key : string.Empty;
        }
    }
}
