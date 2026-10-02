using System.Collections.Generic;

namespace LegendsOfRus.GameplayCore
{
    public sealed partial class BoardResolutionPlanner
    {
        void ResolveSparkCombination(BoardModel board, BoardPosition a, BoardPosition b,
            BoardCellData afterA, BoardCellData afterB, Queue<BoardSpecialActivationInstruction> queue,
            HashSet<BoardPosition> clear, HashSet<string> processed)
        {
            var spark = afterA.Special == SpecialGemKind.Spark ? a : b;
            var partner = spark == a ? b : a;
            var partnerCell = board.GetCell(partner);
            // Both ingredients are consumed; the carried payload fires only at the landing cell.
            processed.Add($"{partner.X}:{partner.Y}:{(int)partnerCell.Special}");
            if (partnerCell.Special == SpecialGemKind.Color)
            {
                int color = StandaloneRules ? MostCommonOrdinaryColor(board) : board.GetTile(spark);
                if (StandaloneRules) processed.Add($"{spark.X}:{spark.Y}:{(int)SpecialGemKind.Spark}");
                for (int y = 0; y < board.Height; y++)
                for (int x = 0; x < board.Width; x++)
                {
                    var p = new BoardPosition(x,y);
                    if (board.GetTile(p) != color || (StandaloneRules && !board.IsOrdinaryMatchCell(p))) continue;
                    clear.Add(p);
                    // Transformation replaces any existing special on this color for this wave.
                    if (board.GetSpecial(p) != SpecialGemKind.Spark)
                        processed.Add($"{p.X}:{p.Y}:{(int)board.GetSpecial(p)}");
                    queue.Enqueue(new BoardSpecialActivationInstruction(p,SpecialGemKind.Spark,color,true));
                }
            }
            else
            {
                var payload = partnerCell.Special == SpecialGemKind.Row || partnerCell.Special == SpecialGemKind.Column || partnerCell.Special == SpecialGemKind.Burst
                    ? partnerCell.Special : SpecialGemKind.None;
                queue.Enqueue(new BoardSpecialActivationInstruction(spark,SpecialGemKind.Spark,board.GetTile(spark),true,
                    payload:payload, flightCount:partnerCell.Special == SpecialGemKind.Spark ? 3 : 1));
            }
        }

        void ResolveSparkFlight(BoardModel board, BoardSpecialActivationInstruction launch,
            HashSet<BoardPosition> clear, List<BoardSpecialActivationInstruction> activations)
        {
            clear.Add(launch.Position);
            foreach (var d in new[] { new BoardPosition(-1,0), new BoardPosition(1,0), new BoardPosition(0,-1), new BoardPosition(0,1) })
            {
                var p = launch.Position + d;
                if (board.IsInside(p)) clear.Add(p);
            }
            for (int i = 0; i < launch.FlightCount; i++)
            {
                var target = SparkTarget(board, clear, launch.Position);
                var resolved = new BoardSpecialActivationInstruction(launch.Position,SpecialGemKind.Spark,launch.TileType,
                    launch.ForcedByCombination,target,launch.Payload);
                activations.Add(resolved);
                clear.Add(target);
                if (launch.Payload != SpecialGemKind.None)
                    ApplyActivationEffect(board,new BoardSpecialActivationInstruction(target,launch.Payload,board.GetTile(target),true),clear);
            }
        }

        static BoardPosition SparkTarget(BoardModel board, HashSet<BoardPosition> covered, BoardPosition fallback)
        {
            BoardPosition best = fallback;
            int bestScore = -1;
            for (int pass = 0; pass < 2; pass++)
            {
                for (int y = 0; y < board.Height; y++)
                for (int x = 0; x < board.Width; x++)
                {
                    var p = new BoardPosition(x,y);
                    if (pass == 0 && covered.Contains(p)) continue;
                    var cell = board.GetCell(p);
                    var obstacle = board.GetObstacle(p);
                    if (!cell.HasTile && obstacle.Kind == BoardObstacleKind.None) continue;
                    int score = obstacle.Kind != BoardObstacleKind.None ? 100 : cell.Special != SpecialGemKind.None ? 50 : 1;
                    if (score > bestScore) { bestScore = score; best = p; }
                }
                if (bestScore >= 0) return best;
            }
            return best;
        }
    }
}
