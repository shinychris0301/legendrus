using LegendsOfRus.GameplayCore;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Presentation-only identities used by the pooled board VFX layer.
    /// These values never participate in board planning or state hashes.
    /// </summary>
    public enum CoreLoopVfxCue
    {
        None = 0,
        SwapTrail,
        SwapCommitGlint,
        SwapRejectPulse,
        MatchBurst,
        MatchResolveGlint,
        CascadePulse,
        MajorCombinationPulse,
        SpecialActivationPulse,
        SpecialCreationHalo,
        SpecialRowSweep,
        SpecialColumnSweep,
        SpecialBurstPulse,
        SpecialColorWash,
        FallTrail,
        FallSettleSpark,
        ReshuffleWash,
        ReshuffleSettle,
        RecoveryWash,
        BoosterMaceRow,
        BoosterMaceColumn,
        BoosterFirebird,
        BoosterLivingLight,
        BoosterSeaPearl,
        BoosterMaceCompleted,
        BoosterFirebirdCompleted,
        BoosterLivingLightCompleted,
        BoosterSeaPearlCompleted,
        BoosterRejected
    }

    /// <summary>
    /// Allocation-free animation data copied into a prewarmed VFX slot.
    /// </summary>
    public readonly struct CoreLoopVfxStyle
    {
        public Color StartColor { get; }
        public Color EndColor { get; }
        public float DurationSeconds { get; }
        public float StartScale { get; }
        public float EndScale { get; }
        public float StartRotationDegrees { get; }
        public float RotationDegrees { get; }
        public bool UsesPulseEnvelope { get; }

        public CoreLoopVfxStyle(
            Color startColor,
            Color endColor,
            float durationSeconds,
            float startScale,
            float endScale,
            float startRotationDegrees = 0f,
            float rotationDegrees = 0f,
            bool usesPulseEnvelope = false
        )
        {
            StartColor = startColor;
            EndColor = endColor;
            DurationSeconds = Mathf.Max(0.01f, durationSeconds);
            StartScale = Mathf.Max(0f, startScale);
            EndScale = Mathf.Max(0f, endScale);
            StartRotationDegrees = startRotationDegrees;
            RotationDegrees = rotationDegrees;
            UsesPulseEnvelope = usesPulseEnvelope;
        }
    }

    /// <summary>
    /// Snapshot used by play-mode tests and development diagnostics.
    /// </summary>
    public readonly struct CoreLoopVfxDiagnostics
    {
        public int Capacity { get; }
        public int Active { get; }
        public int PeakActive { get; }
        public int SpawnRequests { get; }
        public int Emitted { get; }
        public int Dropped { get; }
        public int CueCount { get; }
        public int LastMotionSequence { get; }
        public int LastCueEmissionCount { get; }
        public CoreLoopVfxCue LastCue { get; }
        public GameplayBoosterKind LastBoosterKind { get; }
        public bool IsRaycastSafe { get; }

        public CoreLoopVfxDiagnostics(
            int capacity,
            int active,
            int peakActive,
            int spawnRequests,
            int emitted,
            int dropped,
            int cueCount,
            int lastMotionSequence,
            int lastCueEmissionCount,
            CoreLoopVfxCue lastCue,
            GameplayBoosterKind lastBoosterKind,
            bool isRaycastSafe
        )
        {
            Capacity = capacity;
            Active = active;
            PeakActive = peakActive;
            SpawnRequests = spawnRequests;
            Emitted = emitted;
            Dropped = dropped;
            CueCount = cueCount;
            LastMotionSequence = lastMotionSequence;
            LastCueEmissionCount = lastCueEmissionCount;
            LastCue = lastCue;
            LastBoosterKind = lastBoosterKind;
            IsRaycastSafe = isRaycastSafe;
        }
    }

    /// <summary>
    /// Compact deterministic record of one motion-to-VFX mapping.
    /// </summary>
    public readonly struct CoreLoopVfxTrace
    {
        public int MotionSequence { get; }
        public CoreLoopVfxCue Cue { get; }
        public BoardPosition Primary { get; }
        public BoardPosition Secondary { get; }
        public int Magnitude { get; }
        public int CascadeDepth { get; }
        public SpecialGemKind SpecialKind { get; }
        public SpecialCombinationKind CombinationKind { get; }
        public GameplayBoosterKind BoosterKind { get; }
        public string PlanHash { get; }
        public int RequestedEffects { get; }
        public int EmittedEffects { get; }
        public int DroppedEffects { get; }
        public int ActiveEffectsAfter { get; }

        public CoreLoopVfxTrace(
            int motionSequence,
            CoreLoopVfxCue cue,
            BoardPosition primary,
            BoardPosition secondary,
            int magnitude,
            int cascadeDepth,
            SpecialGemKind specialKind,
            SpecialCombinationKind combinationKind,
            GameplayBoosterKind boosterKind,
            string planHash,
            int requestedEffects,
            int emittedEffects,
            int droppedEffects,
            int activeEffectsAfter
        )
        {
            MotionSequence = motionSequence;
            Cue = cue;
            Primary = primary;
            Secondary = secondary;
            Magnitude = Mathf.Max(0, magnitude);
            CascadeDepth = Mathf.Max(0, cascadeDepth);
            SpecialKind = specialKind;
            CombinationKind = combinationKind;
            BoosterKind = boosterKind;
            PlanHash = planHash ?? string.Empty;
            RequestedEffects = requestedEffects;
            EmittedEffects = emittedEffects;
            DroppedEffects = droppedEffects;
            ActiveEffectsAfter = activeEffectsAfter;
        }
    }

    /// <summary>
    /// Immutable semantic request produced before any pool slot is touched.
    /// It is safe to compare in tests and contains no frame-time or slot data.
    /// </summary>
    public readonly struct CoreLoopVfxRequest
    {
        public int MotionSequence { get; }
        public CoreLoopVfxCue Cue { get; }
        public BoardPosition Primary { get; }
        public BoardPosition Secondary { get; }
        public int Magnitude { get; }
        public int CascadeDepth { get; }
        public SpecialGemKind SpecialKind { get; }
        public SpecialCombinationKind CombinationKind { get; }
        public GameplayBoosterKind BoosterKind { get; }
        public string PlanHash { get; }

        public CoreLoopVfxRequest(
            int motionSequence,
            CoreLoopVfxCue cue,
            BoardPosition primary,
            BoardPosition secondary,
            int magnitude,
            int cascadeDepth,
            SpecialGemKind specialKind,
            SpecialCombinationKind combinationKind,
            GameplayBoosterKind boosterKind,
            string planHash
        )
        {
            MotionSequence = motionSequence;
            Cue = cue;
            Primary = primary;
            Secondary = secondary;
            Magnitude = Mathf.Max(0, magnitude);
            CascadeDepth = Mathf.Max(0, cascadeDepth);
            SpecialKind = specialKind;
            CombinationKind = combinationKind;
            BoosterKind = boosterKind;
            PlanHash = planHash ?? string.Empty;
        }
    }

    /// <summary>
    /// Pure motion-to-VFX mapping boundary. It never reads or mutates runtime
    /// board, inventory, save, or pool state.
    /// </summary>
    public static class CoreLoopVfxMapper
    {
        public static bool TryCreateRequest(
            GameplayMotionEvent motion,
            out CoreLoopVfxRequest request
        )
        {
            request = default(CoreLoopVfxRequest);
            if (motion == null)
                return false;

            CoreLoopVfxCue cue = CoreLoopVfxPresenter.ResolveCue(motion);
            if (cue == CoreLoopVfxCue.None)
                return false;

            request = new CoreLoopVfxRequest(
                motion.Sequence,
                cue,
                motion.Primary,
                motion.Secondary,
                motion.AffectedCount,
                motion.ComboIndex,
                motion.SpecialKind,
                motion.CombinationKind,
                motion.BoosterKind,
                motion.PlanHash
            );
            return true;
        }
    }
}
