using System;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>Device UI preference, independent of progress/account save journals.</summary>
    public static class LanguagePreference
    {
        public const string Key = "LegendsOfRus_Language_v1";
        public static string Normalize(string code) =>
            string.Equals(code, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "ru";
        public static string Load() => Normalize(PlayerPrefs.GetString(DevelopmentGameplayProfile.PreferenceKey(Key), "ru"));
        public static void Save(string code)
        {
            string normalized = Normalize(code);
            if (PlayerPrefs.HasKey(DevelopmentGameplayProfile.PreferenceKey(Key)) && PlayerPrefs.GetString(DevelopmentGameplayProfile.PreferenceKey(Key)) == normalized) return;
            PlayerPrefs.SetString(DevelopmentGameplayProfile.PreferenceKey(Key), normalized);
            PlayerPrefs.Save();
        }
    }
}
