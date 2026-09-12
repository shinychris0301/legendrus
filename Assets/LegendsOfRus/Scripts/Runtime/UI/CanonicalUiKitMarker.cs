using UnityEngine;

namespace LegendsOfRus
{
    public enum CanonicalUiComponentRole
    {
        Unknown = 0,
        PrimaryButton = 1,
        SecondaryButton = 2,
        DangerButton = 3,
        NavyPanel = 4,
        ParchmentPanel = 5,
        ModalFrame = 6,
        HeaderFrame = 7,
        TopBar = 8,
        TabBar = 9,
        CurrencyBar = 10,
        BottomNavigation = 11,
        ProgressBar = 12,
        RewardTile = 13,
        BoosterSlot = 14,
        RankingRow = 15,
        IconButton = 16,
        NotificationBadge = 17,
        Toggle = 18,
        GoalPanel = 19,
        BoardFrame = 20,
        MapNode = 21
    }

    /// <summary>
    /// Marker placed on reusable v1.20 production-UI prefabs. It carries no
    /// behavior and exists so later screen reconstruction can validate that a
    /// page uses the canonical kit rather than a flattened DesignScreens image.
    /// </summary>
    public sealed class CanonicalUiKitMarker : MonoBehaviour
    {
        [SerializeField] CanonicalUiComponentRole role;
        [SerializeField] string styleVersion = "v1.20-step03.2";

        public CanonicalUiComponentRole Role { get { return role; } }
        public string StyleVersion { get { return styleVersion; } }

        public void Configure(CanonicalUiComponentRole value)
        {
            role = value;
            styleVersion = "v1.20-step03.2";
        }
    }
}
