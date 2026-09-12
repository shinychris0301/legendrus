using LegendsOfRus.SaveCore;
using UnityEngine;

namespace LegendsOfRus
{
    internal sealed class PlayerPrefsSaveJournalStore : ISaveJournalStore
    {
        public bool TryRead(string key, out string value)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                value = null;
                return false;
            }

            value = PlayerPrefs.GetString(key, null);
            return true;
        }

        public void Write(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}
