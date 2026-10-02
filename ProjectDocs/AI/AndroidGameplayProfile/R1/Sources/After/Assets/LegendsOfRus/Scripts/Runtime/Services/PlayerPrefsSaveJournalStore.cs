using LegendsOfRus.SaveCore;
using UnityEngine;

namespace LegendsOfRus
{
    internal sealed class PlayerPrefsSaveJournalStore : ISaveJournalStore
    {
        public bool TryRead(string key, out string value)
        {
            if (!PlayerPrefs.HasKey(DevelopmentGameplayProfile.PreferenceKey(key)))
            {
                value = null;
                return false;
            }

            value = PlayerPrefs.GetString(DevelopmentGameplayProfile.PreferenceKey(key), null);
            return true;
        }

        public void Write(string key, string value)
        {
            PlayerPrefs.SetString(DevelopmentGameplayProfile.PreferenceKey(key), value);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(DevelopmentGameplayProfile.PreferenceKey(key));
        }

        public void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}
