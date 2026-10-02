using System;
using LegendsOfRus.GameplayCore;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Converts the immutable gameplay motion timeline into bounded,
    /// non-interactive UI effects. This class never mutates puzzle state.
    /// </summary>
    public sealed class CoreLoopVfxPresenter : MonoBehaviour
    {
        [SerializeField, Range(1, CoreLoopVfxProfile.MaximumPoolCapacity)]
        int poolCapacity = CoreLoopVfxProfile.DefaultPoolCapacity;

        Match3Board board;
        PooledUiVfxLayer layer;
        int columns = 8;
        int rows = 9;
        bool subscribed;
        int cueCount;
        int lastMotionSequence;
        int lastCueEmissionCount;
        CoreLoopVfxCue lastCue;
        GameplayBoosterKind lastBoosterKind;
        CoreLoopVfxTrace lastEffectTrace;
        bool warnedTraceObserverFailure;

        public event Action<CoreLoopVfxTrace> EffectTracePublished;

        public bool IsInitialized => layer != null && layer.IsInitialized;
        public int PoolCapacity => layer != null ? layer.Capacity : 0;
        public int ActiveEffectCount => layer != null ? layer.ActiveCount : 0;
        public int PeakActiveEffectCount =>
            layer != null ? layer.PeakActiveCount : 0;
        public int SpawnRequestCount =>
            layer != null ? layer.SpawnRequestCount : 0;
        public int EmittedEffectCount =>
            layer != null ? layer.EmittedEffectCount : 0;
        public int DroppedEffectCount =>
            layer != null ? layer.DroppedEffectCount : 0;
        public int CueCount => cueCount;
        public int LastMotionSequence => lastMotionSequence;
        public int LastCueEmissionCount => lastCueEmissionCount;
        public CoreLoopVfxCue LastCue => lastCue;
        public GameplayBoosterKind LastBoosterKind => lastBoosterKind;
        public CoreLoopVfxTrace LastEffectTrace => lastEffectTrace;
        public PooledUiVfxLayer Layer => layer;
        public RectTransform VfxRoot => layer != null ? layer.Root : null;
        public bool IsRaycastSafe => layer != null && layer.IsRaycastSafe;
        public int CreatedEffectCount =>
            layer != null ? layer.CreatedEffectCount : 0;
        public int AvailableEffectCount =>
            layer != null ? layer.AvailableEffectCount : 0;

        public CoreLoopVfxDiagnostics Diagnostics =>
            new CoreLoopVfxDiagnostics(
                PoolCapacity,
                ActiveEffectCount,
                PeakActiveEffectCount,
                SpawnRequestCount,
                EmittedEffectCount,
                DroppedEffectCount,
                CueCount,
                LastMotionSequence,
                LastCueEmissionCount,
                LastCue,
                LastBoosterKind,
                IsRaycastSafe
            );

        public void Initialize(Match3Board source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            Initialize(source, source.ColumnCount, source.RowCount);
        }

        public void Initialize(
            Match3Board source,
            int columnCount,
            int rowCount
        )
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var boardRoot = source.transform as RectTransform;
            if (boardRoot == null)
                throw new InvalidOperationException(
                    "Core-loop UI VFX require a RectTransform board root."
                );

            Initialize(source, boardRoot, columnCount, rowCount);
        }

        public void Initialize(
            Match3Board source,
            RectTransform boardRoot,
            int columnCount,
            int rowCount
        )
        {
            Configure(source, boardRoot, columnCount, rowCount, poolCapacity);
        }

        /// <summary>
        /// Boardless initialization seam for focused play-mode tests.
        /// </summary>
        public void Initialize(
            RectTransform boardRoot,
            int columnCount,
            int rowCount,
            int requestedCapacity = CoreLoopVfxProfile.DefaultPoolCapacity
        )
        {
            Configure(
                null,
                boardRoot,
                columnCount,
                rowCount,
                requestedCapacity
            );
        }

        public void Present(GameplayMotionEvent motion)
        {
            if (!IsInitialized || motion == null)
                return;

            if (motion.Kind == GameplayMotionKind.CommandRecovered)
                layer.ResetEffects();

            CoreLoopVfxCue cue = ResolveCue(motion);
            lastCue = cue;
            lastMotionSequence = motion.Sequence;
            lastBoosterKind = motion.BoosterKind;
            cueCount++;

            int requestedBefore = layer.SpawnRequestCount;
            int emittedBefore = layer.EmittedEffectCount;
            int droppedBefore = layer.DroppedEffectCount;
            // Spark has its own approved icon flight and mesh accents.
            bool candidateSettle = board != null && board.RuleVersion >= GameplayRuleVersion.PrototypeR2 &&
                (cue == CoreLoopVfxCue.FallSettleSpark || cue == CoreLoopVfxCue.MatchResolveGlint);
            bool causalMesh = board != null && board.RuleVersion >= GameplayRuleVersion.PrototypeR4 &&
                (motion.Kind == GameplayMotionKind.MatchWaveStarted || motion.Kind == GameplayMotionKind.SpecialActivationStarted);
            // R4 has dedicated formation, impact, flight and booster renderers. The pooled
            // defaults otherwise add creation/cascade/booster rings on top of those renderers.
            // Keep the non-circular swap trail; preserve cue/trace diagnostics below.
            bool dedicatedPresentation = board != null && board.RuleVersion >= GameplayRuleVersion.PrototypeR4 &&
                cue != CoreLoopVfxCue.SwapTrail;
            if (motion.SpecialKind != SpecialGemKind.Spark && !candidateSettle && !causalMesh && !dedicatedPresentation) EmitMotion(cue, motion);
            lastCueEmissionCount = layer.EmittedEffectCount - emittedBefore;
            lastEffectTrace = new CoreLoopVfxTrace(
                motion.Sequence,
                cue,
                motion.Primary,
                motion.Secondary,
                motion.AffectedCount,
                motion.ComboIndex,
                motion.SpecialKind,
                motion.CombinationKind,
                motion.BoosterKind,
                motion.PlanHash,
                layer.SpawnRequestCount - requestedBefore,
                lastCueEmissionCount,
                layer.DroppedEffectCount - droppedBefore,
                layer.ActiveCount
            );

            Action<CoreLoopVfxTrace> traceHandlers = EffectTracePublished;
            if (traceHandlers != null)
            {
                foreach (Delegate candidate in traceHandlers.GetInvocationList())
                {
                    try
                    {
                        ((Action<CoreLoopVfxTrace>)candidate)(lastEffectTrace);
                    }
                    catch (Exception exception)
                    {
                        if (warnedTraceObserverFailure)
                            continue;
                        warnedTraceObserverFailure = true;
                        Debug.LogWarning(
                            "Core-loop VFX trace observer failed and was " +
                            "isolated: " + exception.Message
                        );
                    }
                }
            }
        }

        public void ResetEffects()
        {
            if (layer != null)
                layer.ResetEffects();
        }

        public void ClearAll()
        {
            ResetEffects();
        }

        public void ResetDiagnostics()
        {
            cueCount = 0;
            lastMotionSequence = 0;
            lastCueEmissionCount = 0;
            lastCue = CoreLoopVfxCue.None;
            lastBoosterKind = GameplayBoosterKind.None;
            lastEffectTrace = default(CoreLoopVfxTrace);
            warnedTraceObserverFailure = false;
            if (layer != null)
                layer.ResetDiagnostics();
        }

        public bool HasActiveCue(CoreLoopVfxCue cue)
        {
            return layer != null && layer.HasActiveCue(cue);
        }

        public int GetActiveCueCount(CoreLoopVfxCue cue)
        {
            return layer != null ? layer.GetActiveCount(cue) : 0;
        }

        /// <summary>
        /// Deterministic clock seam used by tests without waiting for frames.
        /// </summary>
        public void TickForTesting(float unscaledDeltaTime)
        {
            if (layer != null)
                layer.Tick(unscaledDeltaTime);
        }

        public static CoreLoopVfxCue ResolveCue(GameplayMotionEvent motion)
        {
            if (motion == null)
                return CoreLoopVfxCue.None;

            switch (motion.Kind)
            {
                case GameplayMotionKind.SwapStarted:
                    return CoreLoopVfxCue.SwapTrail;
                case GameplayMotionKind.SwapCommitted:
                case GameplayMotionKind.SwapReturned:
                    return CoreLoopVfxCue.SwapCommitGlint;
                case GameplayMotionKind.SwapRejected:
                case GameplayMotionKind.SwapReturnStarted:
                    return CoreLoopVfxCue.SwapRejectPulse;
                case GameplayMotionKind.MatchWaveStarted:
                    return CoreLoopVfxCue.MatchBurst;
                case GameplayMotionKind.MatchWaveCompleted:
                    return CoreLoopVfxCue.MatchResolveGlint;
                case GameplayMotionKind.CascadeStarted:
                    return CoreLoopVfxCue.CascadePulse;
                case GameplayMotionKind.SpecialCreationStarted:
                    return CoreLoopVfxCue.SpecialCreationHalo;
                case GameplayMotionKind.SpecialActivationStarted:
                    return ResolveSpecialCue(motion.SpecialKind);
                case GameplayMotionKind.FallStarted:
                    return CoreLoopVfxCue.FallTrail;
                case GameplayMotionKind.FallCompleted:
                    return CoreLoopVfxCue.FallSettleSpark;
                case GameplayMotionKind.ReshuffleStarted:
                    return CoreLoopVfxCue.ReshuffleWash;
                case GameplayMotionKind.ReshuffleCompleted:
                    return CoreLoopVfxCue.ReshuffleSettle;
                case GameplayMotionKind.CommandRecovered:
                    return CoreLoopVfxCue.RecoveryWash;
                case GameplayMotionKind.BoosterActivated:
                    return ResolveBoosterActivationCue(motion.BoosterKind);
                case GameplayMotionKind.BoosterCompleted:
                    return ResolveBoosterCompletionCue(motion.BoosterKind);
                case GameplayMotionKind.BoosterRejected:
                    return CoreLoopVfxCue.BoosterRejected;
                default:
                    return CoreLoopVfxCue.None;
            }
        }

        void Configure(
            Match3Board source,
            RectTransform boardRoot,
            int columnCount,
            int rowCount,
            int requestedCapacity
        )
        {
            if (boardRoot == null)
                throw new ArgumentNullException(nameof(boardRoot));

            Unsubscribe();
            board = source;
            columns = Mathf.Max(1, columnCount);
            rows = Mathf.Max(1, rowCount);
            poolCapacity = Mathf.Clamp(
                requestedCapacity,
                1,
                CoreLoopVfxProfile.MaximumPoolCapacity
            );

            if (layer == null)
                layer = new PooledUiVfxLayer();
            layer.Initialize(boardRoot, poolCapacity);
            layer.BringToFront();

            if (isActiveAndEnabled)
                Subscribe();
        }

        void Update()
        {
            if (layer != null)
                layer.Tick(Time.unscaledDeltaTime);
        }

        void OnEnable()
        {
            Subscribe();
        }

        void OnDisable()
        {
            Unsubscribe();
            ResetEffects();
        }

        void OnDestroy()
        {
            Unsubscribe();
            if (layer != null)
            {
                layer.Dispose();
                layer = null;
            }
        }

        void Subscribe()
        {
            if (subscribed || board == null)
                return;
            board.MotionEventPublished += Present;
            subscribed = true;
        }

        void Unsubscribe()
        {
            if (!subscribed)
                return;
            if (board != null)
                board.MotionEventPublished -= Present;
            subscribed = false;
        }

        void EmitMotion(CoreLoopVfxCue cue, GameplayMotionEvent motion)
        {
            if (cue == CoreLoopVfxCue.None)
                return;

            switch (motion.Kind)
            {
                case GameplayMotionKind.SwapStarted:
                    EmitSwapTrail(cue, motion.Primary, motion.Secondary);
                    break;
                case GameplayMotionKind.SwapCommitted:
                case GameplayMotionKind.SwapReturned:
                    EmitAtPosition(cue, motion.Primary, 0f);
                    EmitAtPosition(cue, motion.Secondary, 0.025f);
                    break;
                case GameplayMotionKind.SwapRejected:
                case GameplayMotionKind.SwapReturnStarted:
                    EmitAtPosition(cue, motion.Primary, 0f, 0.04f);
                    EmitAtPosition(cue, motion.Secondary, 0.035f, 0.04f);
                    break;
                case GameplayMotionKind.MatchWaveStarted:
                    EmitBursts(cue, motion, motion.AffectedCount);
                    break;
                case GameplayMotionKind.MatchWaveCompleted:
                    EmitBursts(cue, motion, Mathf.Min(4, motion.AffectedCount));
                    break;
                case GameplayMotionKind.CascadeStarted:
                    EmitCascade(cue, motion.ComboIndex);
                    break;
                case GameplayMotionKind.SpecialCreationStarted:
                    EmitAtPosition(cue, motion.Primary, 0f, 0.08f);
                    break;
                case GameplayMotionKind.SpecialActivationStarted:
                    EmitSpecial(cue, motion);
                    break;
                case GameplayMotionKind.FallStarted:
                    EmitFullBoard(cue, 0.08f, 0f);
                    break;
                case GameplayMotionKind.FallCompleted:
                    EmitBursts(cue, motion, Mathf.Min(6, motion.AffectedCount));
                    break;
                case GameplayMotionKind.ReshuffleStarted:
                    EmitFullBoard(cue, 0.02f, 0f);
                    break;
                case GameplayMotionKind.ReshuffleCompleted:
                    EmitFullBoard(cue, 0.035f, 0f);
                    break;
                case GameplayMotionKind.CommandRecovered:
                    EmitFullBoard(cue, 0.02f, 0f);
                    break;
                case GameplayMotionKind.BoosterActivated:
                    EmitBoosterActivation(cue, motion);
                    break;
                case GameplayMotionKind.BoosterCompleted:
                    EmitBoosterCompletion(cue, motion);
                    break;
                case GameplayMotionKind.BoosterRejected:
                    EmitAtPosition(cue, motion.Primary, 0f, 0.055f);
                    break;
            }
        }

        void EmitSwapTrail(
            CoreLoopVfxCue cue,
            BoardPosition primary,
            BoardPosition secondary
        )
        {
            Rect connection = ConnectionRect(primary, secondary);
            Rect expanded = Expand(connection, 0.012f, 0.012f);
            Emit(cue, connection, expanded, 0f);
        }

        void EmitCascade(CoreLoopVfxCue cue, int comboIndex)
        {
            int pulseCount = Mathf.Clamp(
                comboIndex <= 0 ? 1 : comboIndex,
                1,
                CoreLoopVfxProfile.MaximumCascadePulses
            );
            for (int i = 0; i < pulseCount; i++)
            {
                float inset = 0.045f + i * 0.035f;
                Rect start = FullBoardRect(inset);
                Rect end = FullBoardRect(Mathf.Max(0.01f, inset - 0.025f));
                Emit(cue, start, end, i * 0.045f);
            }
        }

        void EmitSpecial(CoreLoopVfxCue cue, GameplayMotionEvent motion)
        {
            switch (motion.SpecialKind)
            {
                case SpecialGemKind.Row:
                    Emit(
                        cue,
                        RowRect(motion.Primary),
                        Expand(RowRect(motion.Primary), 0f, 0.018f),
                        0f
                    );
                    break;
                case SpecialGemKind.Column:
                    Emit(
                        cue,
                        ColumnRect(motion.Primary),
                        Expand(ColumnRect(motion.Primary), 0.018f, 0f),
                        0f
                    );
                    break;
                case SpecialGemKind.Burst:
                    Emit(
                        cue,
                        CellRect(motion.Primary, 0.32f),
                        AreaRect(motion.Primary, 1),
                        0f
                    );
                    break;
                case SpecialGemKind.Color:
                    EmitFullBoard(cue, 0.025f, 0f);
                    break;
                default:
                    EmitAtPosition(cue, motion.Primary, 0f, 0.08f);
                    break;
            }

            bool majorCombination =
                motion.BoosterKind == GameplayBoosterKind.None &&
                motion.AffectedCount > 0 &&
                (
                    motion.CombinationKind != SpecialCombinationKind.None ||
                    motion.AffectedCount >= 2
                );
            if (majorCombination)
            {
                EmitFullBoard(
                    CoreLoopVfxCue.MajorCombinationPulse,
                    0.06f,
                    0.035f
                );
            }
        }

        void EmitBoosterActivation(
            CoreLoopVfxCue cue,
            GameplayMotionEvent motion
        )
        {
            switch (motion.BoosterKind)
            {
                case GameplayBoosterKind.BogatyrMaceRow:
                    Emit(
                        cue,
                        RowRect(motion.Primary),
                        Expand(RowRect(motion.Primary), 0f, 0.025f),
                        0f
                    );
                    break;
                case GameplayBoosterKind.BogatyrMaceColumn:
                    Emit(
                        cue,
                        ColumnRect(motion.Primary),
                        Expand(ColumnRect(motion.Primary), 0.025f, 0f),
                        0f
                    );
                    break;
                case GameplayBoosterKind.Firebird:
                    Emit(
                        cue,
                        CellRect(motion.Primary, 0.34f),
                        AreaRect(motion.Primary, 1),
                        0f
                    );
                    EmitAtPosition(cue, motion.Primary, 0.05f, 0.11f);
                    break;
                case GameplayBoosterKind.LivingLight:
                    EmitFullBoard(cue, 0.02f, 0f);
                    EmitAtPosition(cue, motion.Primary, 0.055f, 0.10f);
                    break;
                case GameplayBoosterKind.SeaPearl:
                    EmitFullBoard(cue, 0.12f, 0f);
                    EmitFullBoard(cue, 0.075f, 0.04f);
                    EmitFullBoard(cue, 0.03f, 0.08f);
                    break;
                default:
                    EmitAtPosition(cue, motion.Primary, 0f, 0.08f);
                    break;
            }
        }

        void EmitBoosterCompletion(
            CoreLoopVfxCue cue,
            GameplayMotionEvent motion
        )
        {
            if (motion.BoosterKind == GameplayBoosterKind.SeaPearl)
            {
                EmitFullBoard(cue, 0.025f, 0f);
                return;
            }

            if (motion.BoosterKind == GameplayBoosterKind.LivingLight)
            {
                EmitFullBoard(cue, 0.055f, 0f);
                return;
            }

            EmitAtPosition(cue, motion.Primary, 0f, 0.06f);
            if (motion.BoosterKind == GameplayBoosterKind.Firebird)
                EmitAtPosition(cue, motion.Primary, 0.04f, 0.12f);
        }

        void EmitBursts(
            CoreLoopVfxCue cue,
            GameplayMotionEvent motion,
            int requestedCount,
            float delayStep = 0.022f
        )
        {
            if (motion.ClearedCells.Count > 0)
            {
                // Bound the burst count and use real resolver cells, including cascades.
                int bursts = Mathf.Min(motion.ClearedCells.Count, CoreLoopVfxProfile.MaximumBurstEffectsPerCue);
                for (int i = 0; i < bursts; i++)
                {
                    var cell = motion.ClearedCells[i * motion.ClearedCells.Count / bursts];
                    EmitAtPosition(cue, cell.Position, i * delayStep, .018f);
                }
                return;
            }
            int count = Mathf.Clamp(
                requestedCount <= 0
                    ? 1
                    : Mathf.CeilToInt(requestedCount / 8f),
                1,
                CoreLoopVfxProfile.MaximumBurstEffectsPerCue
            );

            uint seed = StableSeed(motion);
            for (int i = 0; i < count; i++)
            {
                // Motion events intentionally do not carry cleared/fall cell
                // lists. Use restrained board-level rings instead of flashing
                // fabricated cells that may not have participated in a wave.
                float seedInset = (seed % 7u) * 0.002f;
                float inset = 0.055f + seedInset + i * 0.035f;
                EmitFullBoard(cue, inset, i * delayStep);
            }
        }

        void EmitAtPosition(
            CoreLoopVfxCue cue,
            BoardPosition position,
            float delay,
            float expansion = 0.035f
        )
        {
            Rect start = CellRect(position, 0.28f);
            Rect end = Expand(start, expansion, expansion);
            Emit(cue, start, end, delay);
        }

        void EmitFullBoard(
            CoreLoopVfxCue cue,
            float inset,
            float delay
        )
        {
            Rect start = FullBoardRect(inset);
            Rect end = FullBoardRect(Mathf.Max(0.005f, inset * 0.35f));
            Emit(cue, start, end, delay);
        }

        void Emit(
            CoreLoopVfxCue cue,
            Rect start,
            Rect end,
            float delay
        )
        {
            layer.Emit(
                cue,
                start,
                end,
                CoreLoopVfxProfile.Resolve(cue),
                delay
            );
        }

        Rect CellRect(BoardPosition position, float insetFraction)
        {
            int x = IsInside(position)
                ? position.X
                : Mathf.Clamp(columns / 2, 0, columns - 1);
            int y = IsInside(position)
                ? position.Y
                : Mathf.Clamp(rows / 2, 0, rows - 1);
            float inset = Mathf.Clamp(insetFraction, 0f, 0.48f);

            float xMin = (x + inset) / columns;
            float xMax = (x + 1f - inset) / columns;
            float yMin = 1f - (y + 1f - inset) / rows;
            float yMax = 1f - (y + inset) / rows;
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        Rect RowRect(BoardPosition position)
        {
            int y = IsInside(position)
                ? position.Y
                : Mathf.Clamp(rows / 2, 0, rows - 1);
            float inset = 0.34f;
            float yMin = 1f - (y + 1f - inset) / rows;
            float yMax = 1f - (y + inset) / rows;
            return Rect.MinMaxRect(0.015f, yMin, 0.985f, yMax);
        }

        Rect ColumnRect(BoardPosition position)
        {
            int x = IsInside(position)
                ? position.X
                : Mathf.Clamp(columns / 2, 0, columns - 1);
            float inset = 0.34f;
            float xMin = (x + inset) / columns;
            float xMax = (x + 1f - inset) / columns;
            return Rect.MinMaxRect(xMin, 0.015f, xMax, 0.985f);
        }

        Rect AreaRect(BoardPosition position, int radius)
        {
            int centerX = IsInside(position)
                ? position.X
                : Mathf.Clamp(columns / 2, 0, columns - 1);
            int centerY = IsInside(position)
                ? position.Y
                : Mathf.Clamp(rows / 2, 0, rows - 1);
            int xMinCell = Mathf.Max(0, centerX - radius);
            int xMaxCell = Mathf.Min(columns - 1, centerX + radius);
            int yMinCell = Mathf.Max(0, centerY - radius);
            int yMaxCell = Mathf.Min(rows - 1, centerY + radius);

            float xMin = (float)xMinCell / columns;
            float xMax = (float)(xMaxCell + 1) / columns;
            float yMin = 1f - (float)(yMaxCell + 1) / rows;
            float yMax = 1f - (float)yMinCell / rows;
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        Rect ConnectionRect(BoardPosition a, BoardPosition b)
        {
            if (!IsInside(a) || !IsInside(b))
                return CellRect(IsInside(a) ? a : b, 0.30f);

            float ax = (a.X + 0.5f) / columns;
            float ay = 1f - (a.Y + 0.5f) / rows;
            float bx = (b.X + 0.5f) / columns;
            float by = 1f - (b.Y + 0.5f) / rows;
            float halfWidth = 0.13f / columns;
            float halfHeight = 0.13f / rows;

            if (Mathf.Abs(a.X - b.X) >= Mathf.Abs(a.Y - b.Y))
            {
                return Rect.MinMaxRect(
                    Mathf.Min(ax, bx) - halfWidth,
                    ay - halfHeight,
                    Mathf.Max(ax, bx) + halfWidth,
                    ay + halfHeight
                );
            }

            return Rect.MinMaxRect(
                ax - halfWidth,
                Mathf.Min(ay, by) - halfHeight,
                ax + halfWidth,
                Mathf.Max(ay, by) + halfHeight
            );
        }

        static Rect FullBoardRect(float inset)
        {
            float safeInset = Mathf.Clamp(inset, 0f, 0.49f);
            return Rect.MinMaxRect(
                safeInset,
                safeInset,
                1f - safeInset,
                1f - safeInset
            );
        }

        static Rect Expand(Rect value, float horizontal, float vertical)
        {
            return Rect.MinMaxRect(
                Mathf.Clamp01(value.xMin - horizontal),
                Mathf.Clamp01(value.yMin - vertical),
                Mathf.Clamp01(value.xMax + horizontal),
                Mathf.Clamp01(value.yMax + vertical)
            );
        }

        bool IsInside(BoardPosition position)
        {
            return
                position.X >= 0 &&
                position.X < columns &&
                position.Y >= 0 &&
                position.Y < rows;
        }

        static uint StableSeed(GameplayMotionEvent motion)
        {
            uint hash = 2166136261u;
            hash = (hash ^ (uint)motion.Sequence) * 16777619u;
            hash = (hash ^ (uint)motion.ComboIndex) * 16777619u;
            hash = (hash ^ (uint)motion.AffectedCount) * 16777619u;
            hash = (hash ^ (uint)motion.Kind) * 16777619u;
            hash = (hash ^ (uint)motion.BoosterKind) * 16777619u;

            string planHash = motion.PlanHash;
            for (int i = 0; i < planHash.Length; i++)
                hash = (hash ^ planHash[i]) * 16777619u;
            return hash;
        }

        static CoreLoopVfxCue ResolveSpecialCue(SpecialGemKind kind)
        {
            switch (kind)
            {
                case SpecialGemKind.Row:
                    return CoreLoopVfxCue.SpecialRowSweep;
                case SpecialGemKind.Column:
                    return CoreLoopVfxCue.SpecialColumnSweep;
                case SpecialGemKind.Burst:
                    return CoreLoopVfxCue.SpecialBurstPulse;
                case SpecialGemKind.Color:
                    return CoreLoopVfxCue.SpecialColorWash;
                default:
                    return CoreLoopVfxCue.SpecialActivationPulse;
            }
        }

        static CoreLoopVfxCue ResolveBoosterActivationCue(
            GameplayBoosterKind kind
        )
        {
            switch (kind)
            {
                case GameplayBoosterKind.BogatyrMaceRow:
                    return CoreLoopVfxCue.BoosterMaceRow;
                case GameplayBoosterKind.BogatyrMaceColumn:
                    return CoreLoopVfxCue.BoosterMaceColumn;
                case GameplayBoosterKind.Firebird:
                    return CoreLoopVfxCue.BoosterFirebird;
                case GameplayBoosterKind.LivingLight:
                    return CoreLoopVfxCue.BoosterLivingLight;
                case GameplayBoosterKind.SeaPearl:
                    return CoreLoopVfxCue.BoosterSeaPearl;
                default:
                    return CoreLoopVfxCue.SpecialActivationPulse;
            }
        }

        static CoreLoopVfxCue ResolveBoosterCompletionCue(
            GameplayBoosterKind kind
        )
        {
            switch (kind)
            {
                case GameplayBoosterKind.BogatyrMaceRow:
                case GameplayBoosterKind.BogatyrMaceColumn:
                    return CoreLoopVfxCue.BoosterMaceCompleted;
                case GameplayBoosterKind.Firebird:
                    return CoreLoopVfxCue.BoosterFirebirdCompleted;
                case GameplayBoosterKind.LivingLight:
                    return CoreLoopVfxCue.BoosterLivingLightCompleted;
                case GameplayBoosterKind.SeaPearl:
                    return CoreLoopVfxCue.BoosterSeaPearlCompleted;
                default:
                    return CoreLoopVfxCue.MatchResolveGlint;
            }
        }
    }
}
