using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>
    /// Deterministic Match-3 board rules with no MonoBehaviour, GameObject,
    /// animation, input, UI, save, economy, or scene dependencies.
    ///
    /// This is the authoritative puzzle state used by the runtime controller
    /// and by EditMode regression tests.
    /// </summary>
    public sealed class BoardModel
    {
        public const int EmptyTile = -1;

        readonly int[,] tiles;
        readonly SpecialGemKind[,] specials;
        readonly BoardObstacleKind[,] obstacles;
        readonly byte[,] obstacleLayers;
        readonly BoardObjectiveItemKind[,] objectiveItems;
        readonly IBoardRandom random;

        public int Width { get; }
        public int Height { get; }
        public int TileTypeCount { get; }
        public int MutationVersion { get; private set; }
        public int? Seed { get; }

        public BoardModel(
            int width,
            int height,
            int tileTypeCount,
            IBoardRandom random
        )
        {
            if (width < 3) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 3) throw new ArgumentOutOfRangeException(nameof(height));
            if (tileTypeCount < 3) throw new ArgumentOutOfRangeException(nameof(tileTypeCount));

            Width = width;
            Height = height;
            TileTypeCount = tileTypeCount;
            this.random = random ?? throw new ArgumentNullException(nameof(random));

            if (random is SeededBoardRandom seeded)
                Seed = seeded.Seed;

            tiles = new int[width, height];
            specials = new SpecialGemKind[width, height];
            obstacles = new BoardObstacleKind[width, height];
            obstacleLayers = new byte[width, height];
            objectiveItems = new BoardObjectiveItemKind[width, height];
            ClearAll();
        }

        public bool IsInside(BoardPosition position)
        {
            return
                position.X >= 0 && position.X < Width &&
                position.Y >= 0 && position.Y < Height;
        }

        public bool AreAdjacent(BoardPosition a, BoardPosition b)
        {
            return
                IsInside(a) &&
                IsInside(b) &&
                Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) == 1;
        }

        public bool CanSwap(BoardPosition a, BoardPosition b)
        {
            if (!AreAdjacent(a, b)) return false;
            return IsSwappableCell(a) && IsSwappableCell(b);
        }

        public bool IsSwappableCell(BoardPosition position)
        {
            if (!IsInside(position)) return false;
            BoardObstacleKind obstacle = obstacles[position.X, position.Y];
            if (
                obstacle == BoardObstacleKind.Chain ||
                obstacle == BoardObstacleKind.Crate
            )
            {
                return false;
            }

            BoardCellData cell = GetCell(position);
            return cell.HasTile && !cell.HasObjectiveItem;
        }

        public int GetTile(BoardPosition position)
        {
            Validate(position);
            return tiles[position.X, position.Y];
        }

        public SpecialGemKind GetSpecial(BoardPosition position)
        {
            Validate(position);
            return specials[position.X, position.Y];
        }

        public BoardCellData GetCell(BoardPosition position)
        {
            Validate(position);
            return new BoardCellData(
                tiles[position.X, position.Y],
                specials[position.X, position.Y],
                objectiveItems[position.X, position.Y]
            );
        }

        public BoardObstacleData GetObstacle(BoardPosition position)
        {
            Validate(position);
            return new BoardObstacleData(
                obstacles[position.X, position.Y],
                obstacleLayers[position.X, position.Y]
            );
        }

        public BoardObjectiveItemKind GetObjectiveItem(BoardPosition position)
        {
            Validate(position);
            return objectiveItems[position.X, position.Y];
        }

        public void SetCell(BoardPosition position, BoardCellData cell)
        {
            Validate(position);
            ValidateCell(cell);
            if (
                obstacles[position.X, position.Y] == BoardObstacleKind.Crate &&
                !cell.IsEmpty
            )
            {
                throw new InvalidOperationException(
                    "A crate cell cannot contain a gem or objective item."
                );
            }

            tiles[position.X, position.Y] = cell.TileType;
            specials[position.X, position.Y] = cell.HasTile
                ? cell.Special
                : SpecialGemKind.None;
            objectiveItems[position.X, position.Y] = cell.ObjectiveItem;
            MutationVersion++;
        }

        public void SetObstacle(BoardPosition position, BoardObstacleData obstacle)
        {
            Validate(position);
            ValidateObstacle(obstacle.Kind, obstacle.Layers);

            obstacles[position.X, position.Y] = obstacle.Kind;
            obstacleLayers[position.X, position.Y] = (byte)obstacle.Layers;
            if (obstacle.Kind == BoardObstacleKind.Crate)
            {
                tiles[position.X, position.Y] = EmptyTile;
                specials[position.X, position.Y] = SpecialGemKind.None;
                objectiveItems[position.X, position.Y] = BoardObjectiveItemKind.None;
            }
            MutationVersion++;
        }

        public bool DamageObstacle(
            BoardPosition position,
            out BoardObstacleData before,
            out BoardObstacleData after
        )
        {
            Validate(position);
            before = GetObstacle(position);
            if (before.IsEmpty)
            {
                after = before;
                return false;
            }

            int remaining = Math.Max(0, before.Layers - 1);
            if (remaining == 0)
            {
                obstacles[position.X, position.Y] = BoardObstacleKind.None;
                obstacleLayers[position.X, position.Y] = 0;
                after = new BoardObstacleData(BoardObstacleKind.None, 0);
            }
            else
            {
                obstacleLayers[position.X, position.Y] = (byte)remaining;
                after = new BoardObstacleData(before.Kind, remaining);
            }

            MutationVersion++;
            return true;
        }

        public void ClearObstacle(BoardPosition position)
        {
            Validate(position);
            obstacles[position.X, position.Y] = BoardObstacleKind.None;
            obstacleLayers[position.X, position.Y] = 0;
            MutationVersion++;
        }

        public void SetObjectiveItem(
            BoardPosition position,
            BoardObjectiveItemKind objectiveItem
        )
        {
            Validate(position);
            ValidateObjectiveItem(objectiveItem);
            if (
                objectiveItem != BoardObjectiveItemKind.None &&
                obstacles[position.X, position.Y] == BoardObstacleKind.Crate
            )
            {
                throw new InvalidOperationException(
                    "A crate cell cannot contain an objective item."
                );
            }

            objectiveItems[position.X, position.Y] = objectiveItem;
            if (objectiveItem != BoardObjectiveItemKind.None)
            {
                tiles[position.X, position.Y] = EmptyTile;
                specials[position.X, position.Y] = SpecialGemKind.None;
            }
            MutationVersion++;
        }

        public void ClearObjectiveItem(BoardPosition position)
        {
            Validate(position);
            objectiveItems[position.X, position.Y] = BoardObjectiveItemKind.None;
            MutationVersion++;
        }

        public void SetTile(BoardPosition position, int tileType)
        {
            Validate(position);
            ValidateTile(tileType, allowEmpty: true);
            if (
                tileType >= 0 &&
                obstacles[position.X, position.Y] == BoardObstacleKind.Crate
            )
            {
                throw new InvalidOperationException("A crate cell cannot contain a gem.");
            }

            tiles[position.X, position.Y] = tileType;
            objectiveItems[position.X, position.Y] = BoardObjectiveItemKind.None;
            if (tileType < 0)
                specials[position.X, position.Y] = SpecialGemKind.None;
            MutationVersion++;
        }

        public void SetSpecial(BoardPosition position, SpecialGemKind special)
        {
            Validate(position);
            if (!Enum.IsDefined(typeof(SpecialGemKind), special))
                throw new ArgumentOutOfRangeException(nameof(special));
            if (tiles[position.X, position.Y] < 0 && special != SpecialGemKind.None)
                throw new InvalidOperationException("An empty board cell cannot contain a special gem.");

            specials[position.X, position.Y] = special;
            MutationVersion++;
        }

        public void ClearCell(BoardPosition position)
        {
            Validate(position);
            tiles[position.X, position.Y] = EmptyTile;
            specials[position.X, position.Y] = SpecialGemKind.None;
            objectiveItems[position.X, position.Y] = BoardObjectiveItemKind.None;
            MutationVersion++;
        }

        public void ClearCells(IEnumerable<BoardPosition> positions)
        {
            if (positions == null) throw new ArgumentNullException(nameof(positions));

            bool changed = false;
            foreach (var position in positions)
            {
                if (!IsInside(position)) continue;
                tiles[position.X, position.Y] = EmptyTile;
                specials[position.X, position.Y] = SpecialGemKind.None;
                objectiveItems[position.X, position.Y] = BoardObjectiveItemKind.None;
                changed = true;
            }

            if (changed) MutationVersion++;
        }

        public void ClearAll()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                tiles[x, y] = EmptyTile;
                specials[x, y] = SpecialGemKind.None;
                obstacles[x, y] = BoardObstacleKind.None;
                obstacleLayers[x, y] = 0;
                objectiveItems[x, y] = BoardObjectiveItemKind.None;
            }

            MutationVersion++;
        }

        public void LoadRowMajor(
            IReadOnlyList<int> rowMajorTiles,
            IReadOnlyList<SpecialGemKind> rowMajorSpecials = null,
            IReadOnlyList<BoardObstacleKind> rowMajorObstacles = null,
            IReadOnlyList<byte> rowMajorObstacleLayers = null,
            IReadOnlyList<BoardObjectiveItemKind> rowMajorObjectiveItems = null
        )
        {
            if (rowMajorTiles == null)
                throw new ArgumentNullException(nameof(rowMajorTiles));
            if (rowMajorTiles.Count != Width * Height)
                throw new ArgumentException("Tile count does not match board dimensions.");
            if (rowMajorSpecials != null && rowMajorSpecials.Count != rowMajorTiles.Count)
                throw new ArgumentException("Special count does not match tile count.");
            if (rowMajorObstacles != null && rowMajorObstacles.Count != rowMajorTiles.Count)
                throw new ArgumentException("Obstacle count does not match tile count.");
            if (
                rowMajorObstacleLayers != null &&
                rowMajorObstacleLayers.Count != rowMajorTiles.Count
            )
                throw new ArgumentException("Obstacle-layer count does not match tile count.");
            if (
                (rowMajorObstacles == null) != (rowMajorObstacleLayers == null)
            )
                throw new ArgumentException("Obstacle kinds and layers must be supplied together.");
            if (
                rowMajorObjectiveItems != null &&
                rowMajorObjectiveItems.Count != rowMajorTiles.Count
            )
                throw new ArgumentException("Objective-item count does not match tile count.");

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                int index = y * Width + x;
                int tile = rowMajorTiles[index];
                ValidateTile(tile, allowEmpty: true);

                SpecialGemKind special = rowMajorSpecials != null
                    ? rowMajorSpecials[index]
                    : SpecialGemKind.None;

                BoardObstacleKind obstacle = rowMajorObstacles != null
                    ? rowMajorObstacles[index]
                    : BoardObstacleKind.None;
                int layers = rowMajorObstacleLayers != null
                    ? rowMajorObstacleLayers[index]
                    : 0;
                BoardObjectiveItemKind objectiveItem = rowMajorObjectiveItems != null
                    ? rowMajorObjectiveItems[index]
                    : BoardObjectiveItemKind.None;

                if (tile < 0) special = SpecialGemKind.None;
                ValidateObstacle(obstacle, layers);
                ValidateObjectiveItem(objectiveItem);
                if (objectiveItem != BoardObjectiveItemKind.None && tile >= 0)
                    throw new ArgumentException(
                        $"Cell {index} cannot contain both a gem and an objective item."
                    );
                if (
                    obstacle == BoardObstacleKind.Crate &&
                    (tile >= 0 || objectiveItem != BoardObjectiveItemKind.None)
                )
                {
                    throw new ArgumentException(
                        $"Crate cell {index} cannot contain a gem or objective item."
                    );
                }

                tiles[x, y] = tile;
                specials[x, y] = special;
                obstacles[x, y] = obstacle;
                obstacleLayers[x, y] = (byte)layers;
                objectiveItems[x, y] = objectiveItem;
            }

            MutationVersion++;
        }

        public void LoadRows(params string[] rows)
        {
            if (rows == null || rows.Length != Height)
                throw new ArgumentException("Row count must match board height.");

            var values = new int[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                string row = rows[y]?.Replace(" ", string.Empty) ?? string.Empty;
                if (row.Length != Width)
                    throw new ArgumentException($"Row {y} must contain exactly {Width} cells.");

                for (int x = 0; x < Width; x++)
                {
                    char c = row[x];
                    int tile = c == '.' ? EmptyTile : c - '0';
                    ValidateTile(tile, allowEmpty: true);
                    values[y * Width + x] = tile;
                }
            }

            LoadRowMajor(values);
        }

        public void FillWithoutInitialMatches()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (obstacles[x, y] == BoardObstacleKind.Crate)
                    {
                        tiles[x, y] = EmptyTile;
                        specials[x, y] = SpecialGemKind.None;
                        objectiveItems[x, y] = BoardObjectiveItemKind.None;
                        continue;
                    }

                    if (objectiveItems[x, y] != BoardObjectiveItemKind.None)
                    {
                        tiles[x, y] = EmptyTile;
                        specials[x, y] = SpecialGemKind.None;
                        continue;
                    }

                    int chosen = ChooseFillTile(x, y);
                    tiles[x, y] = chosen;
                    specials[x, y] = SpecialGemKind.None;
                }
            }

            MutationVersion++;
        }

        public void GeneratePlayableBoard(int maxAttempts = 128)
        {
            if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                FillWithoutInitialMatches();
                if (!HasAnyMatches() && HasLegalMove())
                    return;
            }

            throw new InvalidOperationException(
                $"Could not generate a playable {Width}x{Height} board after {maxAttempts} attempts."
            );
        }

        public bool WouldCreateInitialMatch(int x, int y, int tileType)
        {
            var position = new BoardPosition(x, y);
            Validate(position);
            ValidateTile(tileType, allowEmpty: false);

            if (
                x >= 2 &&
                tiles[x - 1, y] == tileType &&
                tiles[x - 2, y] == tileType
            )
            {
                return true;
            }

            if (
                y >= 2 &&
                tiles[x, y - 1] == tileType &&
                tiles[x, y - 2] == tileType
            )
            {
                return true;
            }

            return false;
        }

        int ChooseFillTile(int x, int y)
        {
            int first = random.Next(0, TileTypeCount);

            for (int offset = 0; offset < TileTypeCount; offset++)
            {
                int candidate = (first + offset) % TileTypeCount;
                if (!WouldCreateInitialMatch(x, y, candidate))
                    return candidate;
            }

            throw new InvalidOperationException("No valid tile candidate was available.");
        }

        public void Swap(BoardPosition a, BoardPosition b)
        {
            if (!CanSwap(a, b))
                throw new InvalidOperationException(
                    $"Cells {a} and {b} cannot be swapped in their current state."
                );

            SwapRaw(a, b);
            MutationVersion++;
        }

        public bool SwapCreatesMatch(BoardPosition a, BoardPosition b)
        {
            if (!CanSwap(a, b)) return false;

            SwapRaw(a, b);
            bool creates = HasAnyMatches();
            SwapRaw(a, b);
            return creates;
        }

        public bool TrySwapAndKeep(
            BoardPosition a,
            BoardPosition b,
            out List<BoardMatchRun> runs
        )
        {
            runs = new List<BoardMatchRun>();
            if (!CanSwap(a, b)) return false;

            SwapRaw(a, b);
            runs = FindRuns();
            if (runs.Count > 0)
            {
                MutationVersion++;
                return true;
            }

            SwapRaw(a, b);
            return false;
        }

        public List<BoardMatchRun> FindRuns()
        {
            var runs = new List<BoardMatchRun>();

            for (int y = 0; y < Height; y++)
            {
                int start = 0;

                for (int x = 1; x <= Width; x++)
                {
                    bool same =
                        x < Width &&
                        tiles[x, y] >= 0 &&
                        tiles[x, y] == tiles[x - 1, y];

                    if (same) continue;

                    int length = x - start;
                    if (length >= 3 && tiles[x - 1, y] >= 0)
                    {
                        var run = new BoardMatchRun
                        {
                            Horizontal = true,
                            Type = tiles[x - 1, y]
                        };

                        for (int k = start; k < x; k++)
                            run.Cells.Add(new BoardPosition(k, y));

                        runs.Add(run);
                    }

                    start = x;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                int start = 0;

                for (int y = 1; y <= Height; y++)
                {
                    bool same =
                        y < Height &&
                        tiles[x, y] >= 0 &&
                        tiles[x, y] == tiles[x, y - 1];

                    if (same) continue;

                    int length = y - start;
                    if (length >= 3 && tiles[x, y - 1] >= 0)
                    {
                        var run = new BoardMatchRun
                        {
                            Horizontal = false,
                            Type = tiles[x, y - 1]
                        };

                        for (int k = start; k < y; k++)
                            run.Cells.Add(new BoardPosition(x, k));

                        runs.Add(run);
                    }

                    start = y;
                }
            }

            return runs;
        }

        public bool HasAnyMatches() => FindRuns().Count > 0;

        public HashSet<BoardPosition> CollectMatchedCells(
            IEnumerable<BoardMatchRun> runs
        )
        {
            if (runs == null) throw new ArgumentNullException(nameof(runs));

            var result = new HashSet<BoardPosition>();
            foreach (var run in runs)
            {
                if (run == null) continue;
                foreach (var position in run.Cells)
                    result.Add(position);
            }

            return result;
        }

        public Dictionary<BoardPosition, SpecialGemKind> DetermineSpecialCreations(
            IReadOnlyList<BoardMatchRun> runs,
            BoardPosition? preferred
        )
        {
            if (runs == null) throw new ArgumentNullException(nameof(runs));

            var result = new Dictionary<BoardPosition, SpecialGemKind>();
            var horizontal = new HashSet<BoardPosition>();
            var vertical = new HashSet<BoardPosition>();

            foreach (var run in runs)
            {
                if (run == null) continue;

                foreach (var position in run.Cells)
                {
                    if (run.Horizontal) horizontal.Add(position);
                    else vertical.Add(position);
                }
            }

            foreach (var position in horizontal)
            {
                if (vertical.Contains(position))
                    result[position] = SpecialGemKind.Burst;
            }

            foreach (var run in runs)
            {
                if (run == null || run.Cells.Count < 4) continue;

                BoardPosition cell = ChooseCreationCell(run, preferred);
                if (result.ContainsKey(cell)) continue;

                result[cell] = run.Cells.Count >= 5
                    ? SpecialGemKind.Color
                    : run.Horizontal
                        ? SpecialGemKind.Row
                        : SpecialGemKind.Column;
            }

            return result;
        }

        BoardPosition ChooseCreationCell(
            BoardMatchRun run,
            BoardPosition? preferred
        )
        {
            if (preferred.HasValue && run.Cells.Contains(preferred.Value))
                return preferred.Value;

            return run.Cells[run.Cells.Count / 2];
        }

        public HashSet<BoardPosition> ExpandSpecialEffects(
            IEnumerable<BoardPosition> initial,
            IList<BoardPosition> triggered = null
        )
        {
            if (initial == null) throw new ArgumentNullException(nameof(initial));

            var clear = new HashSet<BoardPosition>();
            var queue = new Queue<BoardPosition>();
            var processed = new HashSet<BoardPosition>();

            foreach (var position in initial)
            {
                if (!IsInside(position)) continue;
                if (clear.Add(position)) queue.Enqueue(position);
            }

            while (queue.Count > 0)
            {
                BoardPosition position = queue.Dequeue();
                if (!processed.Add(position)) continue;

                SpecialGemKind special = GetSpecial(position);
                if (special == SpecialGemKind.None) continue;

                triggered?.Add(position);

                switch (special)
                {
                    case SpecialGemKind.Row:
                        for (int x = 0; x < Width; x++)
                            AddClear(new BoardPosition(x, position.Y), clear, queue);
                        break;

                    case SpecialGemKind.Column:
                        for (int y = 0; y < Height; y++)
                            AddClear(new BoardPosition(position.X, y), clear, queue);
                        break;

                    case SpecialGemKind.Burst:
                        for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                            AddClear(
                                new BoardPosition(position.X + dx, position.Y + dy),
                                clear,
                                queue
                            );
                        break;

                    case SpecialGemKind.Color:
                        int tileType = GetTile(position);
                        for (int y = 0; y < Height; y++)
                        for (int x = 0; x < Width; x++)
                        {
                            var candidate = new BoardPosition(x, y);
                            if (GetTile(candidate) == tileType)
                                AddClear(candidate, clear, queue);
                        }
                        break;
                }
            }

            return clear;
        }

        void AddClear(
            BoardPosition position,
            HashSet<BoardPosition> clear,
            Queue<BoardPosition> queue
        )
        {
            if (!IsInside(position)) return;
            if (clear.Add(position)) queue.Enqueue(position);
        }

        public BoardCollapseResult CollapseAndRefill()
        {
            var result = new BoardCollapseResult();

            for (int x = 0; x < Width; x++)
            {
                int segmentStart = 0;
                int spawnOrder = 0;

                for (int y = 0; y <= Height; y++)
                {
                    bool boundary = y == Height;
                    if (!boundary)
                    {
                        boundary = obstacles[x, y] == BoardObstacleKind.Crate;
                    }

                    if (!boundary) continue;

                    int segmentEnd = y - 1;
                    CollapseSegment(
                        x,
                        segmentStart,
                        segmentEnd,
                        segmentStart == 0,
                        segmentEnd == Height - 1,
                        ref spawnOrder,
                        result
                    );
                    segmentStart = y + 1;
                }
            }

            if (result.Changed) MutationVersion++;
            return result;
        }

        void CollapseSegment(
            int x,
            int startY,
            int endY,
            bool canReceiveSpawns,
            bool reachesBottomExit,
            ref int spawnOrder,
            BoardCollapseResult result
        )
        {
            if (endY < startY) return;

            var occupied = new List<PositionedCell>(endY - startY + 1);
            for (int y = endY; y >= startY; y--)
            {
                var position = new BoardPosition(x, y);
                BoardCellData cell = GetCell(position);
                if (!cell.IsEmpty)
                    occupied.Add(new PositionedCell(position, cell));
            }

            int firstRemaining = 0;
            if (reachesBottomExit)
            {
                while (
                    firstRemaining < occupied.Count &&
                    occupied[firstRemaining].Cell.HasObjectiveItem
                )
                {
                    PositionedCell collected = occupied[firstRemaining++];
                    result.CollectedObjectiveItems.Add(
                        new BoardObjectiveItemCollection(
                            collected.Position,
                            new BoardPosition(x, Height),
                            collected.Cell.ObjectiveItem
                        )
                    );
                }
            }

            for (int y = startY; y <= endY; y++)
                SetCellRaw(
                    new BoardPosition(x, y),
                    new BoardCellData(EmptyTile)
                );

            int writeY = endY;
            for (int i = firstRemaining; i < occupied.Count; i++)
            {
                PositionedCell positioned = occupied[i];
                var to = new BoardPosition(x, writeY--);
                SetCellRaw(to, positioned.Cell);
                if (positioned.Position != to)
                {
                    result.Moves.Add(
                        new BoardCellMove(
                            positioned.Position,
                            to,
                            positioned.Cell
                        )
                    );
                }
            }

            if (!canReceiveSpawns) return;

            while (writeY >= startY)
            {
                var to = new BoardPosition(x, writeY--);
                var cell = new BoardCellData(
                    random.Next(0, TileTypeCount),
                    SpecialGemKind.None
                );
                SetCellRaw(to, cell);
                result.Spawns.Add(
                    new BoardCellSpawn(to, cell, spawnOrder++)
                );
            }
        }

        public bool HasLegalMove()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var a = new BoardPosition(x, y);

                    if (x + 1 < Width)
                    {
                        var b = new BoardPosition(x + 1, y);
                        if (SwapCreatesMatch(a, b)) return true;
                    }

                    if (y + 1 < Height)
                    {
                        var b = new BoardPosition(x, y + 1);
                        if (SwapCreatesMatch(a, b)) return true;
                    }
                }
            }

            return false;
        }

        public BoardReshuffleResult EnsurePlayableBoard(int maxShuffleAttempts = 64)
        {
            if (maxShuffleAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(maxShuffleAttempts));

            if (!HasAnyMatches() && HasLegalMove())
                return new BoardReshuffleResult(0, false);

            var positions = new List<BoardPosition>(Width * Height);
            var cells = new List<BoardCellData>(Width * Height);
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var position = new BoardPosition(x, y);
                BoardCellData cell = GetCell(position);
                if (!cell.HasTile) continue;
                positions.Add(position);
                cells.Add(cell);
            }

            for (int attempt = 1; attempt <= maxShuffleAttempts; attempt++)
            {
                Shuffle(cells);
                PlaceCells(positions, cells);

                if (!HasAnyMatches() && HasLegalMove())
                {
                    MutationVersion++;
                    return new BoardReshuffleResult(attempt, false);
                }
            }

            for (int attempt = 1; attempt <= maxShuffleAttempts; attempt++)
            {
                RegenerateCells(positions);
                if (!HasAnyMatches() && HasLegalMove())
                {
                    MutationVersion++;
                    return new BoardReshuffleResult(
                        maxShuffleAttempts + attempt,
                        true
                    );
                }
            }

            throw new InvalidOperationException(
                $"Could not restore a playable board after " +
                $"{maxShuffleAttempts * 2} reshuffle attempts."
            );
        }

        void Shuffle(List<BoardCellData> cells)
        {
            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                BoardCellData temp = cells[i];
                cells[i] = cells[j];
                cells[j] = temp;
            }
        }

        void PlaceCells(
            IReadOnlyList<BoardPosition> positions,
            IReadOnlyList<BoardCellData> cells
        )
        {
            if (positions.Count != cells.Count)
                throw new ArgumentException("Position and cell counts must match.");

            for (int i = 0; i < positions.Count; i++)
                SetCellRaw(positions[i], cells[i]);
        }

        void RegenerateCells(IReadOnlyList<BoardPosition> positions)
        {
            for (int i = 0; i < positions.Count; i++)
                SetCellRaw(
                    positions[i],
                    new BoardCellData(EmptyTile)
                );

            for (int i = 0; i < positions.Count; i++)
            {
                BoardPosition position = positions[i];
                SetCellRaw(
                    position,
                    new BoardCellData(
                        ChooseFillTile(position.X, position.Y),
                        SpecialGemKind.None
                    )
                );
            }
        }

        public BoardSnapshot CreateSnapshot()
        {
            var tileCopy = new int[Width * Height];
            var specialCopy = new SpecialGemKind[Width * Height];
            var obstacleCopy = new BoardObstacleKind[Width * Height];
            var obstacleLayerCopy = new byte[Width * Height];
            var objectiveItemCopy = new BoardObjectiveItemKind[Width * Height];

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                int index = y * Width + x;
                tileCopy[index] = tiles[x, y];
                specialCopy[index] = specials[x, y];
                obstacleCopy[index] = obstacles[x, y];
                obstacleLayerCopy[index] = obstacleLayers[x, y];
                objectiveItemCopy[index] = objectiveItems[x, y];
            }

            return new BoardSnapshot(
                Width,
                Height,
                TileTypeCount,
                tileCopy,
                specialCopy,
                obstacleCopy,
                obstacleLayerCopy,
                objectiveItemCopy,
                ComputeStateHash()
            );
        }

        public void RestoreSnapshot(BoardSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (
                snapshot.Width != Width ||
                snapshot.Height != Height ||
                snapshot.TileTypeCount != TileTypeCount
            )
            {
                throw new ArgumentException("Snapshot dimensions or tile type count do not match.");
            }

            LoadRowMajor(
                snapshot.Tiles,
                snapshot.Specials,
                snapshot.Obstacles,
                snapshot.ObstacleLayers,
                snapshot.ObjectiveItems
            );
        }

        public string ComputeStateHash()
        {
            unchecked
            {
                const ulong offset = 1469598103934665603UL;
                const ulong prime = 1099511628211UL;
                ulong hash = offset;

                hash = (hash ^ (uint)Width) * prime;
                hash = (hash ^ (uint)Height) * prime;
                hash = (hash ^ (uint)TileTypeCount) * prime;

                for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    hash = (hash ^ (uint)(tiles[x, y] + 2)) * prime;
                    hash = (hash ^ (uint)specials[x, y]) * prime;
                }

                bool hasMechanicState = false;
                for (int y = 0; y < Height && !hasMechanicState; y++)
                for (int x = 0; x < Width; x++)
                {
                    if (
                        obstacles[x, y] != BoardObstacleKind.None ||
                        obstacleLayers[x, y] != 0 ||
                        objectiveItems[x, y] != BoardObjectiveItemKind.None
                    )
                    {
                        hasMechanicState = true;
                        break;
                    }
                }

                // Preserve v1.11 hashes for boards without v1.12 mechanic state.
                if (hasMechanicState)
                {
                    hash = (hash ^ 0x56313201U) * prime;
                    for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                    {
                        hash = (hash ^ (uint)obstacles[x, y]) * prime;
                        hash = (hash ^ obstacleLayers[x, y]) * prime;
                        hash = (hash ^ (uint)objectiveItems[x, y]) * prime;
                    }
                }

                return hash.ToString("X16");
            }
        }

        public string ToDebugString()
        {
            var builder = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int tile = tiles[x, y];
                    builder.Append(tile < 0 ? '.' : (char)('0' + tile));
                }

                if (y + 1 < Height) builder.AppendLine();
            }

            return builder.ToString();
        }

        void SwapRaw(BoardPosition a, BoardPosition b)
        {
            BoardCellData temp = GetCell(a);
            SetCellRaw(a, GetCell(b));
            SetCellRaw(b, temp);
        }

        void SetCellRaw(BoardPosition position, BoardCellData cell)
        {
            tiles[position.X, position.Y] = cell.TileType;
            specials[position.X, position.Y] = cell.HasTile
                ? cell.Special
                : SpecialGemKind.None;
            objectiveItems[position.X, position.Y] = cell.ObjectiveItem;
        }

        readonly struct PositionedCell
        {
            public readonly BoardPosition Position;
            public readonly BoardCellData Cell;

            public PositionedCell(
                BoardPosition position,
                BoardCellData cell
            )
            {
                Position = position;
                Cell = cell;
            }
        }

        void Validate(BoardPosition position)
        {
            if (!IsInside(position))
                throw new ArgumentOutOfRangeException(nameof(position), position, "Outside board bounds.");
        }

        void ValidateTile(int tileType, bool allowEmpty)
        {
            if (allowEmpty && tileType == EmptyTile) return;
            if (tileType < 0 || tileType >= TileTypeCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tileType),
                    tileType,
                    $"Tile type must be between 0 and {TileTypeCount - 1}."
                );
            }
        }

        void ValidateCell(BoardCellData cell)
        {
            ValidateTile(cell.TileType, allowEmpty: true);
            if (!Enum.IsDefined(typeof(SpecialGemKind), cell.Special))
                throw new ArgumentOutOfRangeException(nameof(cell), "Invalid special-gem kind.");
            ValidateObjectiveItem(cell.ObjectiveItem);
            if (!cell.HasTile && cell.Special != SpecialGemKind.None)
                throw new ArgumentException("An empty board cell cannot contain a special gem.");
            if (cell.HasTile && cell.HasObjectiveItem)
                throw new ArgumentException(
                    "A board cell cannot contain both a gem and an objective item."
                );
        }

        static void ValidateObstacle(BoardObstacleKind kind, int layers)
        {
            if (!Enum.IsDefined(typeof(BoardObstacleKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind));
            if (kind == BoardObstacleKind.None && layers != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(layers),
                    "An empty obstacle must have zero layers."
                );
            if (kind != BoardObstacleKind.None && (layers < 1 || layers > 3))
                throw new ArgumentOutOfRangeException(
                    nameof(layers),
                    "Obstacle layers must be in [1, 3]."
                );
        }

        static void ValidateObjectiveItem(BoardObjectiveItemKind objectiveItem)
        {
            if (!Enum.IsDefined(typeof(BoardObjectiveItemKind), objectiveItem))
                throw new ArgumentOutOfRangeException(nameof(objectiveItem));
        }
    }
}
