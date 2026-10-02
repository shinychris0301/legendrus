using System.Collections.Generic;
using System.Linq;

namespace LegendsOfRus.GameplayCore
{
    // One creation per overlapping connected shape, with stable priority and placement.
    internal static class SparkMatchRules
    {
        public static Dictionary<BoardPosition, SpecialGemKind> Creations(
            BoardModel board, IReadOnlyList<BoardMatchRun> runs, BoardPosition? preferred)
        {
            var result = new Dictionary<BoardPosition, SpecialGemKind>();
            var pending = new List<BoardMatchRun>(runs);
            while (pending.Count > 0)
            {
                var group = new List<BoardMatchRun> { pending[0] };
                var cells = new HashSet<BoardPosition>(pending[0].Cells);
                pending.RemoveAt(0);
                bool changed;
                do
                {
                    changed = false;
                    for (int i = pending.Count - 1; i >= 0; i--)
                        if (pending[i].Cells.Exists(cells.Contains))
                        {
                            group.Add(pending[i]); cells.UnionWith(pending[i].Cells);
                            pending.RemoveAt(i); changed = true;
                        }
                } while (changed);

                var color = group.Where(r => !r.Square && r.Count >= 5).ToList();
                var horizontal = new HashSet<BoardPosition>(group.Where(r => !r.Square && r.Horizontal).SelectMany(r => r.Cells));
                var crosses = group.Where(r => !r.Square && !r.Horizontal).SelectMany(r => r.Cells).Where(horizontal.Contains).Distinct().ToList();
                var squares = group.Where(r => r.Square).ToList();
                var lines = group.Where(r => !r.Square && r.Count == 4).ToList();
                SpecialGemKind kind;
                List<BoardPosition> eligible;
                if (color.Count > 0) { kind = SpecialGemKind.Color; eligible = color.SelectMany(r => r.Cells).ToList(); }
                else if (crosses.Count > 0) { kind = SpecialGemKind.Burst; eligible = crosses; }
                else if (squares.Count > 0) { kind = SpecialGemKind.Spark; eligible = squares.SelectMany(r => r.Cells).ToList(); }
                else if (lines.Count > 0) { kind = lines[0].Horizontal ? SpecialGemKind.Column : SpecialGemKind.Row; eligible = new List<BoardPosition>(lines[0].Cells); }
                else continue;
                eligible = eligible.Distinct().Where(p => board.IsSwappableCell(p) && board.GetSpecial(p) == SpecialGemKind.None).OrderBy(p => p).ToList();
                if (eligible.Count == 0) continue;
                var at = preferred.HasValue && eligible.Contains(preferred.Value) ? preferred.Value : eligible[eligible.Count / 2];
                result.Add(at, kind);
            }
            return result;
        }
    }
}
