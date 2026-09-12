using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>Identifies a screenshot-free v1.20 Step 3.3 core-loop page.</summary>
    public sealed class ProductionCoreLoopPageMarker : MonoBehaviour
    {
        public const string CurrentStyleVersion = "v1.20-step03.3";

        [SerializeField] ScreenId screenId;
        [SerializeField] string styleVersion = CurrentStyleVersion;
        [SerializeField] bool screenshotFree = true;
        [SerializeField] Vector2 referenceResolution = new Vector2(941f, 1672f);

        public ScreenId ScreenId { get { return screenId; } }
        public string StyleVersion { get { return styleVersion; } }
        public bool ScreenshotFree { get { return screenshotFree; } }
        public Vector2 ReferenceResolution { get { return referenceResolution; } }

        public void Configure(ScreenId id)
        {
            screenId = id;
            styleVersion = CurrentStyleVersion;
            screenshotFree = true;
            referenceResolution = new Vector2(941f, 1672f);
        }
    }
}
