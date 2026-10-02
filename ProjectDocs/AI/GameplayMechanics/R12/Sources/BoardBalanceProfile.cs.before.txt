using System;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>
    /// Central commercial balance values for puzzle clear shapes and scoring.
    /// Keeping these values outside the planner makes future remote-config and
    /// level-difficulty tuning possible without rewriting resolution rules.
    /// </summary>
    public sealed class BoardBalanceProfile
    {
        public static BoardBalanceProfile CommercialDefault { get; } =
            new BoardBalanceProfile();

        public int MatchBaseScorePerCell { get; }
        public int CascadeScoreStepPerCell { get; }
        public int MaximumCascadeScoreSteps { get; }

        public int BurstRadius { get; }
        public int BurstBurstRadius { get; }
        public int LineBurstBandRadius { get; }

        public BoardBalanceProfile(
            int matchBaseScorePerCell = 50,
            int cascadeScoreStepPerCell = 10,
            int maximumCascadeScoreSteps = 5,
            int burstRadius = 1,
            int burstBurstRadius = 2,
            int lineBurstBandRadius = 1
        )
        {
            MatchBaseScorePerCell = Math.Max(1, matchBaseScorePerCell);
            CascadeScoreStepPerCell = Math.Max(0, cascadeScoreStepPerCell);
            MaximumCascadeScoreSteps = Math.Max(0, maximumCascadeScoreSteps);
            BurstRadius = Math.Max(1, burstRadius);
            BurstBurstRadius = Math.Max(BurstRadius, burstBurstRadius);
            LineBurstBandRadius = Math.Max(1, lineBurstBandRadius);
        }

        public int ScoreForMatchCell(int comboIndex)
        {
            int steps = Math.Max(0, comboIndex - 1);
            steps = Math.Min(steps, MaximumCascadeScoreSteps);
            return MatchBaseScorePerCell + steps * CascadeScoreStepPerCell;
        }

        public int ScoreForCombinationCell(SpecialCombinationKind kind)
        {
            switch (kind)
            {
                case SpecialCombinationKind.ColorWithColor:
                    return 20;
                case SpecialCombinationKind.ColorWithRow:
                case SpecialCombinationKind.ColorWithColumn:
                case SpecialCombinationKind.ColorWithBurst:
                    return 30;
                case SpecialCombinationKind.ColorWithNormal:
                    return 35;
                case SpecialCombinationKind.RowWithBurst:
                case SpecialCombinationKind.ColumnWithBurst:
                    return 34;
                case SpecialCombinationKind.BurstWithBurst:
                    return 30;
                case SpecialCombinationKind.RowWithRow:
                case SpecialCombinationKind.ColumnWithColumn:
                case SpecialCombinationKind.RowWithColumn:
                    return 40;
                default:
                    return MatchBaseScorePerCell;
            }
        }

        public int ScoreForBoosterCell(BoardBoosterCommandKind kind)
        {
            switch (kind)
            {
                case BoardBoosterCommandKind.MaceRow:
                case BoardBoosterCommandKind.MaceColumn:
                    return 25;
                case BoardBoosterCommandKind.FirebirdArea:
                    return 30;
                case BoardBoosterCommandKind.LivingLightColor:
                    return 20;
                default:
                    return 0;
            }
        }
    }
}
