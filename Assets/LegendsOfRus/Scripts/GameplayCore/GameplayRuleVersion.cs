using System;
namespace LegendsOfRus.GameplayCore
{
    // Stable resolver/replay identifiers. Match3Board selects the adopted profile for new rounds;
    // Legacy remains available for historical replays and sandbox comparison.
    public enum GameplayRuleVersion { Legacy = 1, PrototypeR1 = 2, PrototypeR2 = 3, PrototypeR3 = 4, PrototypeR4 = 5 }
    [Serializable]
    public sealed class MechanicsReplayCommand
    {
        public GameplayRuleVersion Rules;
        public string BeforeToken;
        public bool Tap;
        public int X, Y, ToX, ToY;
        public string ExpectedPlanHash;
        public BoardResolutionPlan Resolve()
        {
            if(!Enum.IsDefined(typeof(GameplayRuleVersion),Rules))throw new ArgumentException("Unknown mechanics rules.");
            var s=BoardReplayCodec.Decode(BeforeToken);
            var m=new BoardModel(s.Width,s.Height,s.TileTypeCount,new SeededBoardRandom(1));m.RestoreSnapshot(s);
            var planner=new BoardResolutionPlanner(m,null,Rules);
            var p=Tap?planner.PlanTapSpecial(new BoardPosition(X,Y)):planner.PlanSwap(new BoardPosition(X,Y),new BoardPosition(ToX,ToY));
            if(!string.IsNullOrEmpty(ExpectedPlanHash)&&p.PlanHash!=ExpectedPlanHash)throw new InvalidOperationException("Replay rules/command do not match the recorded plan.");
            return p;
        }
    }
}
