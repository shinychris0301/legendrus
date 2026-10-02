using UnityEngine;
namespace LegendsOfRus
{
    // Board-scale variants only. Approved gems and shared inventory-page icons are unchanged.
    public static class PrototypeBoardArt
    {
        public const string Root = "ProductionUI/MechanicsBoardArtR10/";
        public static Sprite Load(string name) => Resources.Load<Sprite>(Root + name);
    }
}
