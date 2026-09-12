using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Serialized prefab entry point shipped in v1.20 Step 3.3R-FIX01.
    /// A prefab is immediately renderable after import; no Editor builder is required.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProductionUiFix01PageBootstrap : MonoBehaviour
    {
        [SerializeField] ScreenId screenId;

        public ScreenId ScreenId { get { return screenId; } }
        public string StyleVersion { get { return ProductionUiFix01Runtime.StyleVersion; } }

        public void Configure(ScreenId id)
        {
            screenId = id;
        }

        void Awake()
        {
            RectTransform rect = transform as RectTransform;
            if (rect == null)
                rect = gameObject.AddComponent<RectTransform>();
            ProductionUiFix01Runtime.BuildInto(screenId, rect);
        }
    }
}
