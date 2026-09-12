using System;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Opt-in production cutover registry for non-core-loop pages.
    /// ProductionCoreLoopPageRegistry remains frozen at ten accepted routes.
    ///
    /// Step 10.3 generalizes the previously Shop-only state mechanism. A non-core
    /// functional page becomes active only when an explicit Resources state artifact
    /// exists for that page. The distributed project contains no new enabled states.
    /// </summary>
    public static class ProductionMetaPageCutoverRegistry
    {
        public const string ShopStateResourceKey = "ProductionUI/CutoverState/ShopCutoverState";
        public const string ShopStateMagic = "v1.20-step08.5-shop-development-cutover";
        public const string Step103StateMagicPrefix = "v1.20-step10.3-meta-page-development-cutover:";

        public static bool IsCutover(ScreenId id)
        {
            if (!IsSupportedMetaPage(id) || ProductionCoreLoopPageRegistry.IsCutover(id))
                return false;

            string key = StateResourceKey(id);
            TextAsset state = string.IsNullOrEmpty(key) ? null : Resources.Load<TextAsset>(key);
            if (state == null || string.IsNullOrEmpty(state.text))
                return false;

            bool enabled = state.text.IndexOf("\"enabled\": true", StringComparison.Ordinal) >= 0;
            if (!enabled)
                return false;

            string genericMagic = Step103StateMagicPrefix + id;
            if (state.text.IndexOf(genericMagic, StringComparison.Ordinal) >= 0)
                return true;

            // Preserve the already accepted Step 8.5 Shop cutover state format.
            return id == ScreenId.Shop && state.text.IndexOf(ShopStateMagic, StringComparison.Ordinal) >= 0;
        }

        public static string ResourceKey(ScreenId id)
        {
            return IsCutover(id) ? "ProductionUI/Pages/" + id : string.Empty;
        }

        public static string StateResourceKey(ScreenId id)
        {
            if (!IsSupportedMetaPage(id) || ProductionCoreLoopPageRegistry.IsCutover(id))
                return string.Empty;
            return id == ScreenId.Shop
                ? ShopStateResourceKey
                : "ProductionUI/CutoverState/" + id + "CutoverState";
        }

        public static bool IsSupportedMetaPage(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.SplashTitle:
                case ScreenId.Dialogue:
                case ScreenId.Characters:
                case ScreenId.DailyReward:
                case ScreenId.Profile:
                case ScreenId.Inventory:
                case ScreenId.Shop:
                case ScreenId.SpecialOffer:
                case ScreenId.OutOfEnergy:
                case ScreenId.BoosterSelect:
                case ScreenId.RewardClaim:
                case ScreenId.Events:
                case ScreenId.EventDetail:
                case ScreenId.IndividualRanking:
                case ScreenId.TeamHub:
                case ScreenId.TeamRanking:
                case ScreenId.TeamChat:
                case ScreenId.FriendsList:
                case ScreenId.Mail:
                case ScreenId.Settings:
                case ScreenId.LanguageSelect:
                case ScreenId.Login:
                case ScreenId.TutorialOverlay:
                case ScreenId.OfflineState:
                case ScreenId.MaintenanceState:
                case ScreenId.EmptyErrorState:
                    return true;
                default:
                    return false;
            }
        }
    }
}
