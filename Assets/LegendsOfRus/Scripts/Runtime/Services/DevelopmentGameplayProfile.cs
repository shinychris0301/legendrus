using System;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Cold-launch-only, development QA namespace. Never imports or deletes normal
    /// player data. All stores resolve this before their first read, including
    /// scene-authored GameState instances that awake before RuntimeBootstrap.
    /// </summary>
    internal static class DevelopmentGameplayProfile
    {
        internal const string Prefix = "LegendsOfRus_GameplayQA_v1_";
        internal const string IntentExtra = "lor_gameplay_qa";
        const string Marker = Prefix + "Owner";
        const string Owner = "disposable-gameplay-v1";
        static bool initialized, active;
        static Exception initializationFailure;
        internal static bool Active { get { EnsureInitialized(); return active; } }
        internal static string PreferenceKey(string key) => Active ? Prefix + key : key;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetSession()
        {
            initialized = active = false;
            initializationFailure = null;
        }

        static void EnsureInitialized()
        {
            if (initializationFailure != null)
                throw new InvalidOperationException("Gameplay QA initialization blocked", initializationFailure);
            if (initialized) return;
            try
            {
                string command = null;
#if UNITY_EDITOR
                command = Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA");
#elif UNITY_ANDROID && DEVELOPMENT_BUILD
                using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var intent = activity.Call<AndroidJavaObject>("getIntent"))
                {
                    command = intent.Call<string>("getStringExtra", IntentExtra);
                    // Consumed once. Profile switching requires another cold process.
                    intent.Call("removeExtra", IntentExtra);
                    if (!string.IsNullOrEmpty(command))
                    {
                        using (var info = activity.Call<AndroidJavaObject>("getApplicationInfo"))
                            if (activity.Call<string>("getPackageName") != "com.example.legendsofrus.v117debug"
                                || (info.Get<int>("flags") & 2) == 0)
                                throw new InvalidOperationException("Gameplay QA requires the approved development package");
                    }
                }
#endif
                if (string.IsNullOrEmpty(command)) { initialized = true; return; }
                if (command != "fresh" && command != "resume" && command != "empty-energy")
                    throw new InvalidOperationException("Unknown gameplay QA command");
                if (command == "resume" && PlayerPrefs.GetString(Marker, "") != Owner)
                    throw new InvalidOperationException("Create a gameplay QA profile before resuming");

                active = true;
                initialized = true; // Recursive SaveService calls below remain in QA.
                if (command != "resume")
                {
                    // Exact owned slots only; no DeleteAll, key enumeration or normal fallback.
                    foreach (string key in new[] { "LegendsOfRus_Save_v1", SaveService.BackupKey,
                        SaveService.StagingKey, LanguagePreference.Key, PlayerPrefsComplianceStateStore.Key })
                        PlayerPrefs.DeleteKey(Prefix + key);
                    PlayerPrefs.DeleteKey(Marker);
                    PlayerPrefs.Save();
                    var fixture = new PlayerSaveData { lastEnergyUtcTicks = DateTime.UtcNow.Ticks };
                    if (command == "empty-energy") fixture.energy = 0;
                    if (!SaveService.TryReplaceHistory(fixture, out var result) || !result.Success)
                        throw new InvalidOperationException("Gameplay QA fixture could not be committed");
                    PlayerPrefs.SetString(Marker, Owner);
                    PlayerPrefs.Save();
                }
                Debug.Log("LOR_GAMEPLAY_QA active; disposable save; online actions disabled");
            }
            catch (Exception error)
            {
                // Fail closed. A later store call must never fall through to normal data.
                initializationFailure = error;
                throw;
            }
        }
    }
}
