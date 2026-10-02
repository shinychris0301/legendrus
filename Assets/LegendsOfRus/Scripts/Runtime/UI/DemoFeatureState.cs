using System;

namespace LegendsOfRus
{
    /// <summary>
    /// Local-session state for functionality-first demo routes. No network or
    /// production identity is implied. Production providers can replace these
    /// boundaries later without changing page routing.
    /// </summary>
    public static class DemoFeatureState
    {
        static bool eventRewardClaimed;
        static bool friendEnergyRequested;
        static bool guestSignedIn;
        static int teamMessagesSent;
        static string languageCode;

        public static bool EventRewardClaimed { get { return eventRewardClaimed; } }
        public static bool FriendEnergyRequested { get { return friendEnergyRequested; } }
        public static bool GuestSignedIn { get { return guestSignedIn; } }
        public static int TeamMessagesSent { get { return teamMessagesSent; } }
        public static string LanguageCode { get { return languageCode ?? (languageCode = LanguagePreference.Load()); } }

        public static bool TryClaimEventReward()
        {
            if (eventRewardClaimed)
                return false;
            eventRewardClaimed = true;
            return true;
        }

        public static bool TryRequestFriendEnergy()
        {
            if (friendEnergyRequested)
                return false;
            friendEnergyRequested = true;
            return true;
        }

        public static int RecordTeamMessage()
        {
            teamMessagesSent = Math.Max(0, teamMessagesSent) + 1;
            return teamMessagesSent;
        }

        public static void SetLanguage(string code)
        {
            if (string.Equals(code, "en", StringComparison.OrdinalIgnoreCase))
                languageCode = "en";
            else
                languageCode = "ru";
        }

        // User selection persists; SetLanguage remains a session override for previews/tests.
        public static void SelectLanguage(string code)
        {
            SetLanguage(code);
            LanguagePreference.Save(languageCode);
        }

        public static void SignInGuest()
        {
            guestSignedIn = true;
        }

        public static void ResetSession()
        {
            eventRewardClaimed = false;
            friendEnergyRequested = false;
            guestSignedIn = false;
            teamMessagesSent = 0;
            languageCode = null; // Reload the device preference on the next access.
        }
    }
}
