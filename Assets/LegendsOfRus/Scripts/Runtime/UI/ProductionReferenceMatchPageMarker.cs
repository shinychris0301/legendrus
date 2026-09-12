using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Marks a core-loop production page whose visual tree has been rebuilt
    /// against a pinned approved reference using page-specific production
    /// components instead of a Resources/DesignScreens full-screen image.
    /// </summary>
    public sealed class ProductionReferenceMatchPageMarker : MonoBehaviour
    {
        public const string CurrentVersion = "v1.20-step03.3R2";

        [SerializeField] ScreenId screenId;
        [SerializeField] string referenceMatchVersion = CurrentVersion;
        [SerializeField] string referenceSha256 = string.Empty;
        [SerializeField] int productionComponentCount;
        [SerializeField] bool fullScreenScreenshotDependency;
        [SerializeField] bool referenceMatchStaticComposition;

        public ScreenId ScreenId { get { return screenId; } }
        public string ReferenceMatchVersion { get { return referenceMatchVersion; } }
        public string ReferenceSha256 { get { return referenceSha256; } }
        public int ProductionComponentCount { get { return productionComponentCount; } }
        public bool FullScreenScreenshotDependency { get { return fullScreenScreenshotDependency; } }
        public bool ReferenceMatchStaticComposition { get { return referenceMatchStaticComposition; } }

        public void Configure(ScreenId id, string sha256, int componentCount)
        {
            screenId = id;
            referenceMatchVersion = CurrentVersion;
            referenceSha256 = sha256 ?? string.Empty;
            productionComponentCount = componentCount;
            fullScreenScreenshotDependency = false;
            referenceMatchStaticComposition = true;
        }
    }
}
