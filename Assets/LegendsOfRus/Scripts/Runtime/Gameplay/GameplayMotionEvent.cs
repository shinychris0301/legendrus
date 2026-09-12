using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    /// <summary>
    /// Presentation-only milestones emitted while an already deterministic
    /// BoardResolutionPlan is displayed. Consumers may add particles, sound,
    /// camera response, or telemetry, but must never mutate puzzle state.
    /// </summary>
    public enum GameplayMotionKind
    {
        SwapStarted = 0,
        SwapCommitted,
        SwapRejected,
        SwapReturnStarted,
        SwapReturned,
        MatchWaveStarted,
        MatchWaveCompleted,
        CascadeStarted,
        SpecialActivationStarted,
        SpecialCreationStarted,
        FallStarted,
        FallCompleted,
        ReshuffleStarted,
        ReshuffleCompleted,
        CommandRecovered,
        BoosterActivated,
        BoosterCompleted,
        BoosterRejected
    }

    /// <summary>
    /// Presentation identity for a booster command. Mace orientation is
    /// explicit so consumers never infer gameplay intent from coordinates or
    /// localized copy.
    /// </summary>
    public enum GameplayBoosterKind
    {
        None = 0,
        BogatyrMaceRow,
        BogatyrMaceColumn,
        Firebird,
        LivingLight,
        SeaPearl
    }

    public sealed class GameplayMotionEvent
    {
        public int Sequence { get; }
        public GameplayMotionKind Kind { get; }
        public BoardPosition Primary { get; }
        public BoardPosition Secondary { get; }
        public int ComboIndex { get; }
        public int AffectedCount { get; }
        public SpecialGemKind SpecialKind { get; }
        public string PlanHash { get; }
        public SpecialCombinationKind CombinationKind { get; }
        public GameplayBoosterKind BoosterKind { get; }

        public GameplayMotionEvent(
            int sequence,
            GameplayMotionKind kind,
            BoardPosition primary,
            BoardPosition secondary,
            int comboIndex,
            int affectedCount,
            SpecialGemKind specialKind,
            string planHash,
            SpecialCombinationKind combinationKind = SpecialCombinationKind.None,
            GameplayBoosterKind boosterKind = GameplayBoosterKind.None
        )
        {
            Sequence = sequence < 1 ? 1 : sequence;
            Kind = kind;
            Primary = primary;
            Secondary = secondary;
            ComboIndex = comboIndex < 0 ? 0 : comboIndex;
            AffectedCount = affectedCount < 0 ? 0 : affectedCount;
            SpecialKind = specialKind;
            PlanHash = planHash ?? string.Empty;
            CombinationKind = combinationKind;
            BoosterKind = boosterKind;
        }

        public override string ToString()
        {
            return
                $"#{Sequence} {Kind} primary={Primary} secondary={Secondary} " +
                $"combo={ComboIndex} affected={AffectedCount} " +
                $"special={SpecialKind} combination={CombinationKind} " +
                $"booster={BoosterKind} plan={PlanHash}";
        }
    }
}
