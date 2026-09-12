using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public sealed class DynamicHudBinder : MonoBehaviour
    {
        public Text Gold;
        public Text Gems;
        public Text Energy;

        void OnEnable()
        {
            if (GameState.Instance != null)
            {
                GameState.Instance.Changed += Refresh;
                Refresh();
            }
        }

        void OnDisable()
        {
            if (GameState.Instance != null)
                GameState.Instance.Changed -= Refresh;
        }

        public void Refresh()
        {
            var state = GameState.Instance;
            if (state == null) return;

            state.RefreshEnergyFromClock();

            if (Gold != null) Gold.text = state.Save.gold.ToString("N0").Replace(",", " ");
            if (Gems != null) Gems.text = state.Save.gems.ToString("N0").Replace(",", " ");
            if (Energy != null) Energy.text = $"{state.Save.energy}/{state.Save.energyMax}";
        }
    }
}
