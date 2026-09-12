namespace LegendsOfRus
{
    public enum ScreenId
    {
        Loading,
        Home,
        WorldMap,
        ChapterMap,
        PreLevel,
        Gameplay,
        Dialogue,
        Victory,
        Fail,
        Shop,
        Chapter15,

        DailyReward,
        RewardClaim,
        BoosterSelect,
        Events,
        Profile,
        Settings,
        Pause,
        Characters,
        Mail,
        OutOfEnergy,
        ChapterComplete,

        // v1.20 Step 4: append-only functional-demo routes. Keep all legacy
        // numeric values stable because serialized prefabs store ScreenId as int.
        SplashTitle,
        EventDetail,
        IndividualRanking,
        TeamHub,
        TeamRanking,
        TeamChat,
        FriendsList,
        Inventory,
        SpecialOffer,
        LanguageSelect,
        Login,
        TutorialOverlay,
        OfflineState,
        MaintenanceState,
        EmptyErrorState
    }
}
