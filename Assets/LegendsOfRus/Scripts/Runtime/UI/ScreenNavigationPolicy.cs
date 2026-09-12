namespace LegendsOfRus
{
    /// <summary>
    /// Deterministic parent routing for screen-level Back navigation.
    /// Modal and gameplay pause behavior remain owned by ScreenRouter.
    /// </summary>
    public static class ScreenNavigationPolicy
    {
        public static bool TryGetBackDestination(ScreenId current, out ScreenId destination)
        {
            switch (current)
            {
                case ScreenId.Loading:
                    destination = ScreenId.SplashTitle; return true;
                case ScreenId.WorldMap:
                    destination = ScreenId.Home; return true;
                case ScreenId.ChapterMap:
                    destination = ScreenId.WorldMap; return true;
                case ScreenId.PreLevel:
                    destination = ScreenId.ChapterMap; return true;
                case ScreenId.Victory:
                case ScreenId.Fail:
                    destination = ScreenId.ChapterMap; return true;
                case ScreenId.Shop:
                case ScreenId.DailyReward:
                case ScreenId.Events:
                case ScreenId.Profile:
                case ScreenId.Settings:
                case ScreenId.Characters:
                case ScreenId.Mail:
                case ScreenId.IndividualRanking:
                case ScreenId.TeamHub:
                case ScreenId.Inventory:
                    destination = ScreenId.Home; return true;
                case ScreenId.EventDetail:
                    destination = ScreenId.Events; return true;
                case ScreenId.TeamRanking:
                case ScreenId.TeamChat:
                case ScreenId.FriendsList:
                    destination = ScreenId.TeamHub; return true;
                case ScreenId.SpecialOffer:
                    destination = ScreenId.Shop; return true;
                case ScreenId.LanguageSelect:
                    destination = ScreenId.Settings; return true;
                case ScreenId.Login:
                    destination = ScreenId.Profile; return true;
                case ScreenId.Chapter15:
                    destination = ScreenId.WorldMap; return true;
                case ScreenId.ChapterComplete:
                    destination = ScreenId.WorldMap; return true;
                case ScreenId.OfflineState:
                case ScreenId.MaintenanceState:
                case ScreenId.EmptyErrorState:
                case ScreenId.TutorialOverlay:
                case ScreenId.SplashTitle:
                    destination = ScreenId.Home; return true;
                default:
                    destination = ScreenId.Home; return false;
            }
        }
    }
}
