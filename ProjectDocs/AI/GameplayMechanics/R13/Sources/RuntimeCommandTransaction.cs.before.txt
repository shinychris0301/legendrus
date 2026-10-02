using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    internal sealed class RuntimeCommandTransaction
    {
        public long Token { get; }
        public string Label { get; }
        public BoardSnapshot BeforeSnapshot { get; }
        public int MovesBefore { get; }
        public int ScoreBefore { get; }
        public LevelGoalRuntimeSnapshot GoalsBefore { get; }
        public BoosterType? ConsumedBooster { get; private set; }

        public RuntimeCommandTransaction(
            long token,
            string label,
            BoardSnapshot beforeSnapshot,
            int movesBefore,
            int scoreBefore,
            LevelGoalRuntimeSnapshot goalsBefore
        )
        {
            Token = token;
            Label = label ?? string.Empty;
            BeforeSnapshot = beforeSnapshot;
            MovesBefore = movesBefore;
            ScoreBefore = scoreBefore;
            GoalsBefore = goalsBefore;
        }

        public void MarkBoosterConsumed(BoosterType booster)
        {
            ConsumedBooster = booster;
        }
    }
}
