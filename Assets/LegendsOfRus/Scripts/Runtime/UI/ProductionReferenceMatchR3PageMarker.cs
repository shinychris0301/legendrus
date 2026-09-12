using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Identifies a v1.20 Step 3.3R.3 page rebuilt from page-specific
    /// reference-derived production components while preserving live runtime
    /// content such as the Match-3 board and the gameplay page beneath Pause.
    /// </summary>
    public sealed class ProductionReferenceMatchR3PageMarker : MonoBehaviour
    {
        public const string CurrentVersion = "v1.20-step03.3R3";

        [SerializeField] ScreenId screenId;
        [SerializeField] string referenceMatchVersion = CurrentVersion;
        [SerializeField] string referenceSha256 = string.Empty;
        [SerializeField] string proofScope = string.Empty;
        [SerializeField] int productionComponentCount;
        [SerializeField] bool fullScreenScreenshotDependency;
        [SerializeField] bool preservesLiveGameplayBoard;
        [SerializeField] bool overlaysLiveGameplayForPause;

        public ScreenId ScreenId { get { return screenId; } }
        public string ReferenceMatchVersion { get { return referenceMatchVersion; } }
        public string ReferenceSha256 { get { return referenceSha256; } }
        public string ProofScope { get { return proofScope; } }
        public int ProductionComponentCount { get { return productionComponentCount; } }
        public bool FullScreenScreenshotDependency { get { return fullScreenScreenshotDependency; } }
        public bool PreservesLiveGameplayBoard { get { return preservesLiveGameplayBoard; } }
        public bool OverlaysLiveGameplayForPause { get { return overlaysLiveGameplayForPause; } }

        public void Configure(
            ScreenId id,
            string sha256,
            string scope,
            int componentCount,
            bool preserveLiveBoard,
            bool liveGameplayBehindPause)
        {
            screenId = id;
            referenceMatchVersion = CurrentVersion;
            referenceSha256 = sha256 ?? string.Empty;
            proofScope = scope ?? string.Empty;
            productionComponentCount = componentCount;
            fullScreenScreenshotDependency = false;
            preservesLiveGameplayBoard = preserveLiveBoard;
            overlaysLiveGameplayForPause = liveGameplayBehindPause;
        }
    }
}
