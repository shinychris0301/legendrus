using System;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Fixed-capacity, prewarmed UI VFX pool. It is deliberately not a
    /// MonoBehaviour: CoreLoopVfxPresenter owns the single Update loop.
    /// </summary>
    public sealed class PooledUiVfxLayer
    {
        sealed class Slot
        {
            public GameObject GameObject;
            public RectTransform RectTransform;
            public Image Image;
            public CoreLoopEffectGraphic Effect;
            public bool Active;
            public CoreLoopVfxCue Cue;
            public CoreLoopVfxStyle Style;
            public Rect StartBounds;
            public Rect EndBounds;
            public float ElapsedSeconds;
            public float DelaySeconds;
        }

        RectTransform root;
        CanvasGroup canvasGroup;
        Slot[] slots;
        int activeCount;
        int peakActiveCount;
        int spawnRequestCount;
        int emittedEffectCount;
        int droppedEffectCount;
        CoreLoopVfxCue lastEmittedCue;

        public bool IsInitialized => root != null && slots != null;
        public RectTransform Root => root;
        public CanvasGroup RootCanvasGroup => canvasGroup;
        public int Capacity => slots != null ? slots.Length : 0;
        public int ActiveCount => activeCount;
        public int PeakActiveCount => peakActiveCount;
        public int SpawnRequestCount => spawnRequestCount;
        public int EmittedEffectCount => emittedEffectCount;
        public int DroppedEffectCount => droppedEffectCount;
        public CoreLoopVfxCue LastEmittedCue => lastEmittedCue;
        public int PrewarmedObjectCount => Capacity;
        public int CreatedEffectCount => Capacity;
        public int AvailableEffectCount => Mathf.Max(0, Capacity - activeCount);

        public bool IsRaycastSafe
        {
            get
            {
                if (!IsInitialized || canvasGroup == null)
                    return false;
                if (canvasGroup.blocksRaycasts || canvasGroup.interactable)
                    return false;

                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Image == null || slots[i].Image.raycastTarget)
                        return false;
                }

                return true;
            }
        }

        void PlaceBelowPersistentGrid()
        {
            root.SetAsLastSibling();
            // Cell intersections remain legible during full-board washes and clear feedback.
            var grid = root.parent.Find("SharedGrid");
            if (grid != null && grid.GetComponent<ProductionGameplayCell>() != null)
                grid.SetAsLastSibling();
        }

        public void Initialize(RectTransform parent, int requestedCapacity)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            if (IsInitialized)
            {
                root.SetParent(parent, false);
                Stretch(root);
                PlaceBelowPersistentGrid();
                ResetEffects();
                return;
            }

            int capacity = Mathf.Clamp(
                requestedCapacity,
                1,
                CoreLoopVfxProfile.MaximumPoolCapacity
            );

            var rootObject = new GameObject(
                "CoreLoopVfxLayer",
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(RectMask2D)
            );
            rootObject.transform.SetParent(parent, false);
            root = rootObject.GetComponent<RectTransform>();
            Stretch(root);
            PlaceBelowPersistentGrid();

            canvasGroup = rootObject.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.ignoreParentGroups = false;

            slots = new Slot[capacity];
            for (int i = 0; i < capacity; i++)
            {
                var effectObject = new GameObject(
                    $"PooledVfx_{i:00}",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(CoreLoopEffectGraphic)
                );
                effectObject.transform.SetParent(root, false);

                var effectRect = effectObject.GetComponent<RectTransform>();
                effectRect.anchorMin = new Vector2(0.5f, 0.5f);
                effectRect.anchorMax = new Vector2(0.5f, 0.5f);
                effectRect.offsetMin = Vector2.zero;
                effectRect.offsetMax = Vector2.zero;
                effectRect.pivot = new Vector2(0.5f, 0.5f);

                var image = effectObject.GetComponent<Image>();
                image.color = Color.clear;
                image.raycastTarget = false;
                image.maskable = true;
                image.preserveAspect = false;

                effectObject.SetActive(false);
                slots[i] = new Slot
                {
                    GameObject = effectObject,
                    RectTransform = effectRect,
                    Image = image,
                    Effect = effectObject.GetComponent<CoreLoopEffectGraphic>()
                };
            }
        }

        public void BringToFront()
        {
            if (root != null)
                PlaceBelowPersistentGrid();
        }

        public bool Emit(
            CoreLoopVfxCue cue,
            Rect startBounds,
            Rect endBounds,
            CoreLoopVfxStyle style,
            float delaySeconds = 0f
        )
        {
            spawnRequestCount++;
            if (!IsInitialized || cue == CoreLoopVfxCue.None)
            {
                droppedEffectCount++;
                return false;
            }

            Slot slot = null;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].Active)
                {
                    slot = slots[i];
                    break;
                }
            }

            if (slot == null)
            {
                droppedEffectCount++;
                return false;
            }

            slot.Active = true;
            slot.Cue = cue;
            slot.Effect.SetFrame(cue,0);
            slot.Style = style;
            slot.StartBounds = ClampBounds(startBounds);
            slot.EndBounds = ClampBounds(endBounds);
            slot.ElapsedSeconds = 0f;
            slot.DelaySeconds = Mathf.Max(0f, delaySeconds);

            ApplyBounds(slot.RectTransform, slot.StartBounds);
            slot.RectTransform.localScale = Vector3.one * style.StartScale;
            slot.RectTransform.localRotation = Quaternion.Euler(
                0f,
                0f,
                style.StartRotationDegrees
            );
            slot.Image.color = slot.DelaySeconds > 0f
                ? Color.clear
                : style.StartColor;
            slot.GameObject.SetActive(true);

            activeCount++;
            if (activeCount > peakActiveCount)
                peakActiveCount = activeCount;
            emittedEffectCount++;
            lastEmittedCue = cue;
            return true;
        }

        public void Tick(float unscaledDeltaTime)
        {
            if (!IsInitialized || activeCount == 0)
                return;

            float delta = Mathf.Max(0f, unscaledDeltaTime);
            for (int i = 0; i < slots.Length; i++)
            {
                Slot slot = slots[i];
                if (!slot.Active)
                    continue;

                slot.ElapsedSeconds += delta;
                if (slot.ElapsedSeconds < slot.DelaySeconds)
                    continue;

                float animationTime = slot.ElapsedSeconds - slot.DelaySeconds;
                float normalized = Mathf.Clamp01(
                    animationTime / slot.Style.DurationSeconds
                );
                float eased = CoreLoopMotionProfile.EaseOutCubic(normalized);

                ApplyBounds(
                    slot.RectTransform,
                    LerpBounds(slot.StartBounds, slot.EndBounds, eased)
                );

                float scale = Mathf.Lerp(
                    slot.Style.StartScale,
                    slot.Style.EndScale,
                    eased
                );
                if (slot.Style.UsesPulseEnvelope)
                {
                    float pulse = Mathf.Sin(normalized * Mathf.PI) * 0.08f;
                    scale *= 1f + pulse;
                }
                slot.RectTransform.localScale = Vector3.one * scale;
                slot.RectTransform.localRotation = Quaternion.Euler(
                    0f,
                    0f,
                    slot.Style.StartRotationDegrees +
                    slot.Style.RotationDegrees * eased
                );

                Color color = Color.Lerp(
                    slot.Style.StartColor,
                    slot.Style.EndColor,
                    eased
                );
                if (slot.Style.UsesPulseEnvelope)
                {
                    float envelope = Mathf.Sin(normalized * Mathf.PI);
                    color.a *= Mathf.Lerp(0.72f, 1f, envelope);
                }
                slot.Image.color = color;
                slot.Effect.SetFrame(slot.Cue,normalized);

                if (normalized >= 1f)
                    Release(slot);
            }
        }

        public void ResetEffects()
        {
            if (slots == null)
                return;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Active)
                    Release(slots[i]);
                else if (slots[i].GameObject != null)
                    slots[i].GameObject.SetActive(false);
            }
            activeCount = 0;
        }

        public void ClearAll()
        {
            ResetEffects();
        }

        public void Dispose()
        {
            ResetEffects();
            if (root != null)
                UnityEngine.Object.Destroy(root.gameObject);
            root = null;
            canvasGroup = null;
            slots = null;
            activeCount = 0;
        }

        public void ResetDiagnostics()
        {
            peakActiveCount = activeCount;
            spawnRequestCount = 0;
            emittedEffectCount = 0;
            droppedEffectCount = 0;
            lastEmittedCue = CoreLoopVfxCue.None;
        }

        public int GetActiveCount(CoreLoopVfxCue cue)
        {
            if (slots == null)
                return 0;

            int count = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Active && slots[i].Cue == cue)
                    count++;
            }
            return count;
        }

        public bool HasActiveCue(CoreLoopVfxCue cue)
        {
            return GetActiveCount(cue) > 0;
        }

        void Release(Slot slot)
        {
            if (!slot.Active)
                return;

            slot.Active = false;
            slot.Cue = CoreLoopVfxCue.None;
            if (slot.Image != null)
                slot.Image.color = Color.clear;
            if (slot.RectTransform != null)
            {
                slot.RectTransform.localScale = Vector3.one;
                slot.RectTransform.localRotation = Quaternion.identity;
            }
            if (slot.GameObject != null)
                slot.GameObject.SetActive(false);
            activeCount = Mathf.Max(0, activeCount - 1);
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
        }

        static void ApplyBounds(RectTransform rect, Rect normalizedBounds)
        {
            rect.anchorMin = new Vector2(
                normalizedBounds.xMin,
                normalizedBounds.yMin
            );
            rect.anchorMax = new Vector2(
                normalizedBounds.xMax,
                normalizedBounds.yMax
            );
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static Rect ClampBounds(Rect value)
        {
            float xMin = Mathf.Clamp01(Mathf.Min(value.xMin, value.xMax));
            float xMax = Mathf.Clamp01(Mathf.Max(value.xMin, value.xMax));
            float yMin = Mathf.Clamp01(Mathf.Min(value.yMin, value.yMax));
            float yMax = Mathf.Clamp01(Mathf.Max(value.yMin, value.yMax));

            const float minimumSize = 0.001f;
            if (xMax - xMin < minimumSize)
                xMax = Mathf.Min(1f, xMin + minimumSize);
            if (yMax - yMin < minimumSize)
                yMax = Mathf.Min(1f, yMin + minimumSize);

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        static Rect LerpBounds(Rect from, Rect to, float value)
        {
            return Rect.MinMaxRect(
                Mathf.Lerp(from.xMin, to.xMin, value),
                Mathf.Lerp(from.yMin, to.yMin, value),
                Mathf.Lerp(from.xMax, to.xMax, value),
                Mathf.Lerp(from.yMax, to.yMax, value)
            );
        }
    }
}
