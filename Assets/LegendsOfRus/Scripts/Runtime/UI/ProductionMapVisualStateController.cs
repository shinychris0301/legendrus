using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    public enum ProductionMapVisualMode
    {
        WorldChapters = 0,
        ChapterLevels = 1
    }

    [Serializable]
    public sealed class ProductionMapVisualNodeBinding
    {
        public int slotIndex;
        public RectTransform root;
        public Image frame;
        public Image selectedHalo;
        public Image lockIcon;
        public Text label;
        public Image[] stars = Array.Empty<Image>();

        [NonSerialized] public int logicalIndex;
    }

    /// <summary>
    /// v1.20 Step 7.4 visual-only progression binder for the reference-accurate map candidates.
    /// It does not own progression. GameState remains authoritative; this component only maps
    /// that state to live labels, stars, locks and selection visuals.
    /// </summary>
    public sealed class ProductionMapVisualStateController : MonoBehaviour
    {
        [SerializeField] ProductionMapVisualMode mode;
        [SerializeField] int logicalCapacity = 1;
        [SerializeField] Sprite filledStarSprite;
        [SerializeField] Sprite emptyStarSprite;
        [SerializeField] ProductionMapVisualNodeBinding[] nodes = Array.Empty<ProductionMapVisualNodeBinding>();

        GameState subscribedState;
        int windowStart = 1;

        public ProductionMapVisualMode Mode => mode;
        public int LogicalCapacity => logicalCapacity;
        public int VisibleSlotCount => nodes != null ? nodes.Length : 0;
        public int WindowStart => windowStart;

        public void Configure(
            ProductionMapVisualMode visualMode,
            int capacity,
            Sprite filledStar,
            Sprite emptyStar,
            ProductionMapVisualNodeBinding[] bindings
        )
        {
            mode = visualMode;
            logicalCapacity = Mathf.Max(1, capacity);
            filledStarSprite = filledStar;
            emptyStarSprite = emptyStar;
            nodes = bindings ?? Array.Empty<ProductionMapVisualNodeBinding>();
        }

        void OnEnable()
        {
            BindGameState();
            ApplyAuthoritativeState();
        }

        void OnDisable()
        {
            UnbindGameState();
        }

        void Update()
        {
            if (subscribedState != GameState.Instance)
            {
                BindGameState();
                ApplyAuthoritativeState();
            }
        }

        void BindGameState()
        {
            if (subscribedState == GameState.Instance)
                return;

            UnbindGameState();
            subscribedState = GameState.Instance;
            if (subscribedState != null)
                subscribedState.Changed += ApplyAuthoritativeState;
        }

        void UnbindGameState()
        {
            if (subscribedState != null)
                subscribedState.Changed -= ApplyAuthoritativeState;
            subscribedState = null;
        }

        public void ApplyAuthoritativeState()
        {
            GameState state = GameState.Instance;
            if (state == null || state.Save == null)
            {
                ApplyQaDefault();
                return;
            }

            if (mode == ProductionMapVisualMode.WorldChapters)
            {
                int selectedChapter = Mathf.Clamp(state.Save.selectedChapter, 1, logicalCapacity);
                int highestUnlocked = Mathf.Clamp(
                    Mathf.Max(selectedChapter, state.Save.lastCompletedChapter + 1),
                    1,
                    logicalCapacity
                );
                int[] chapterStars = new int[logicalCapacity + 1];

                // Chapter 1 is the only fully authored chapter in the current functional slice.
                ChapterCompletionSummary chapterOne = state.GetChapterCompletionSummary(1);
                if (chapterOne != null && chapterOne.MaximumStars > 0)
                {
                    chapterStars[1] = Mathf.Clamp(
                        Mathf.RoundToInt(3f * chapterOne.EarnedStars / chapterOne.MaximumStars),
                        0,
                        3
                    );
                }

                ApplySnapshot(selectedChapter, highestUnlocked, chapterStars, logicalCapacity);
                return;
            }

            int selectedLevel = Mathf.Clamp(state.Save.selectedLevel, 1, logicalCapacity);
            int highestLevel = Mathf.Clamp(state.Save.highestUnlockedLevel, 1, logicalCapacity);
            int[] levelStars = new int[logicalCapacity + 1];
            if (state.Save.levelProgress != null)
            {
                for (int i = 0; i < state.Save.levelProgress.Count; i++)
                {
                    LevelProgressData progress = state.Save.levelProgress[i];
                    if (progress == null || progress.chapterId != state.Save.selectedChapter)
                        continue;
                    if (progress.levelId >= 1 && progress.levelId <= logicalCapacity)
                        levelStars[progress.levelId] = Mathf.Clamp(progress.bestStars, 0, 3);
                }
            }
            ApplySnapshot(selectedLevel, highestLevel, levelStars, logicalCapacity);
        }

        public void ApplyQaDefault()
        {
            int count = Mathf.Max(1, logicalCapacity);
            int selected = mode == ProductionMapVisualMode.WorldChapters ? Mathf.Min(5, count) : 1;
            int highest = mode == ProductionMapVisualMode.WorldChapters ? Mathf.Min(5, count) : 3;
            int[] stars = new int[count + 1];
            for (int i = 1; i <= count; i++)
                stars[i] = i < highest ? 3 : 0;
            ApplySnapshot(selected, highest, stars, count);
        }

        /// <summary>
        /// QA-only preset that reproduces the logical labels visible in the approved reference.
        /// It does not change GameState and is never called by normal routed gameplay.
        /// </summary>
        public void ApplyReferenceQaPreset()
        {
            if (mode == ProductionMapVisualMode.WorldChapters)
            {
                int[] stars = new int[21];
                for (int i = 1; i <= 15; i++) stars[i] = 3;
                ApplySnapshot(5, 15, stars, 20, 1);
                return;
            }

            int[] referenceStars = new int[17];
            for (int i = 10; i <= 15; i++) referenceStars[i] = 3;
            referenceStars[16] = 0;
            ApplySnapshot(15, 16, referenceStars, 16, 10);
        }

        public void ApplySnapshot(int selected, int highestUnlocked, int[] starsByLogicalIndex, int logicalCount)
        {
            ApplySnapshot(selected, highestUnlocked, starsByLogicalIndex, logicalCount, 0);
        }

        void ApplySnapshot(
            int selected,
            int highestUnlocked,
            int[] starsByLogicalIndex,
            int logicalCount,
            int forcedWindowStart
        )
        {
            if (nodes == null || nodes.Length == 0)
                return;

            int count = Mathf.Max(1, logicalCount);
            int visible = nodes.Length;
            int maxStart = Mathf.Max(1, count - visible + 1);
            selected = Mathf.Clamp(selected, 1, count);
            highestUnlocked = Mathf.Clamp(highestUnlocked, 0, count);

            if (forcedWindowStart > 0)
                windowStart = Mathf.Clamp(forcedWindowStart, 1, maxStart);
            else
                windowStart = Mathf.Clamp(selected - visible / 2, 1, maxStart);

            for (int slot = 0; slot < visible; slot++)
            {
                int logical = windowStart + slot;
                ProductionMapVisualNodeBinding binding = nodes[slot];
                if (binding == null)
                    continue;

                binding.logicalIndex = logical;
                bool exists = logical <= count;
                if (binding.root != null)
                    binding.root.gameObject.SetActive(exists);
                if (!exists)
                    continue;

                bool unlocked = logical <= highestUnlocked;
                bool selectedNode = logical == selected;
                int earnedStars = 0;
                if (starsByLogicalIndex != null && logical >= 0 && logical < starsByLogicalIndex.Length)
                    earnedStars = Mathf.Clamp(starsByLogicalIndex[logical], 0, 3);

                if (binding.label != null)
                {
                    binding.label.text = logical.ToString();
                    binding.label.color = unlocked
                        ? new Color32(247, 215, 142, 255)
                        : new Color32(141, 137, 128, 255);
                }

                if (binding.frame != null)
                    binding.frame.color = unlocked
                        ? Color.white
                        : new Color32(100, 105, 112, 255);

                if (binding.selectedHalo != null)
                    binding.selectedHalo.gameObject.SetActive(selectedNode && unlocked);

                if (binding.lockIcon != null)
                    binding.lockIcon.gameObject.SetActive(!unlocked);

                if (binding.stars == null)
                    continue;

                for (int star = 0; star < binding.stars.Length; star++)
                {
                    Image image = binding.stars[star];
                    if (image == null) continue;
                    bool filled = unlocked && star < earnedStars;
                    image.sprite = filled ? filledStarSprite : emptyStarSprite;
                    image.color = unlocked ? Color.white : new Color32(120, 120, 120, 255);
                }
            }
        }

        public int GetLogicalIndexForSlot(int slot)
        {
            if (nodes == null || slot < 0 || slot >= nodes.Length || nodes[slot] == null)
                return 0;
            return nodes[slot].logicalIndex;
        }
    }
}
