using System;
using System.Collections.Generic;
using System.Text;

namespace LegendsOfRus.GameplayCore
{
    public readonly struct BoardPosition : IEquatable<BoardPosition>, IComparable<BoardPosition>
    {
        public readonly int X;
        public readonly int Y;

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(BoardPosition other)
        {
            int byY = Y.CompareTo(other.Y);
            return byY != 0 ? byY : X.CompareTo(other.X);
        }

        public bool Equals(BoardPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is BoardPosition other && Equals(other);
        public override int GetHashCode() => unchecked((X * 397) ^ Y);
        public override string ToString() => $"({X},{Y})";

        public static bool operator ==(BoardPosition left, BoardPosition right) => left.Equals(right);
        public static bool operator !=(BoardPosition left, BoardPosition right) => !left.Equals(right);
        public static BoardPosition operator +(BoardPosition left, BoardPosition right) =>
            new BoardPosition(left.X + right.X, left.Y + right.Y);
    }

    public enum SpecialGemKind
    {
        None = 0,
        Row = 1,
        Column = 2,
        Burst = 3,
        Color = 4,
        Spark = 5
    }

    public readonly struct BoardCellData : IEquatable<BoardCellData>
    {
        public readonly int TileType;
        public readonly SpecialGemKind Special;
        public readonly BoardObjectiveItemKind ObjectiveItem;

        public BoardCellData(
            int tileType,
            SpecialGemKind special = SpecialGemKind.None,
            BoardObjectiveItemKind objectiveItem = BoardObjectiveItemKind.None
        )
        {
            TileType = tileType;
            Special = special;
            ObjectiveItem = objectiveItem;
        }

        public bool HasTile => TileType >= 0;
        public bool HasObjectiveItem => ObjectiveItem != BoardObjectiveItemKind.None;
        public bool IsEmpty => !HasTile && !HasObjectiveItem;

        public bool Equals(BoardCellData other) =>
            TileType == other.TileType &&
            Special == other.Special &&
            ObjectiveItem == other.ObjectiveItem;

        public override bool Equals(object obj) => obj is BoardCellData other && Equals(other);
        public override int GetHashCode() =>
            unchecked((((TileType * 397) ^ (int)Special) * 397) ^ (int)ObjectiveItem);

        public override string ToString()
        {
            if (HasObjectiveItem) return $"item:{ObjectiveItem}";
            return IsEmpty ? "." : $"{TileType}:{(int)Special}";
        }
    }

    public sealed class BoardMatchRun
    {
        public bool Horizontal;
        public bool Square;
        public int Type;
        public readonly List<BoardPosition> Cells = new List<BoardPosition>();

        public int Count => Cells.Count;

        public override string ToString()
        {
            return $"{(Horizontal ? "H" : "V")}[type={Type}, count={Cells.Count}]";
        }
    }

    public readonly struct BoardCellMove
    {
        public readonly BoardPosition From;
        public readonly BoardPosition To;
        public readonly BoardCellData Cell;

        public BoardCellMove(BoardPosition from, BoardPosition to, BoardCellData cell)
        {
            From = from;
            To = to;
            Cell = cell;
        }
    }

    public readonly struct BoardCellSpawn
    {
        public readonly BoardPosition To;
        public readonly BoardCellData Cell;
        public readonly int SpawnOrder;

        public BoardCellSpawn(BoardPosition to, BoardCellData cell, int spawnOrder)
        {
            To = to;
            Cell = cell;
            SpawnOrder = spawnOrder;
        }
    }

    public sealed class BoardCollapseResult
    {
        public readonly List<BoardCellMove> Moves = new List<BoardCellMove>();
        public readonly List<BoardCellSpawn> Spawns = new List<BoardCellSpawn>();
        public readonly List<BoardObjectiveItemCollection> CollectedObjectiveItems =
            new List<BoardObjectiveItemCollection>();

        public bool Changed =>
            Moves.Count > 0 ||
            Spawns.Count > 0 ||
            CollectedObjectiveItems.Count > 0;
    }

    public readonly struct BoardReshuffleResult
    {
        public readonly int Attempts;
        public readonly bool Regenerated;

        public BoardReshuffleResult(int attempts, bool regenerated)
        {
            Attempts = attempts;
            Regenerated = regenerated;
        }
    }

    public sealed class BoardSnapshot
    {
        public int Width { get; }
        public int Height { get; }
        public int TileTypeCount { get; }
        public int[] Tiles { get; }
        public SpecialGemKind[] Specials { get; }
        public BoardObstacleKind[] Obstacles { get; }
        public byte[] ObstacleLayers { get; }
        public BoardObjectiveItemKind[] ObjectiveItems { get; }
        public string StateHash { get; }

        public BoardSnapshot(
            int width,
            int height,
            int tileTypeCount,
            int[] tiles,
            SpecialGemKind[] specials,
            string stateHash
        ) : this(
            width,
            height,
            tileTypeCount,
            tiles,
            specials,
            new BoardObstacleKind[tiles?.Length ?? 0],
            new byte[tiles?.Length ?? 0],
            new BoardObjectiveItemKind[tiles?.Length ?? 0],
            stateHash
        )
        {
        }

        public BoardSnapshot(
            int width,
            int height,
            int tileTypeCount,
            int[] tiles,
            SpecialGemKind[] specials,
            BoardObstacleKind[] obstacles,
            byte[] obstacleLayers,
            BoardObjectiveItemKind[] objectiveItems,
            string stateHash
        )
        {
            int expected = width * height;
            if (tiles == null) throw new ArgumentNullException(nameof(tiles));
            if (specials == null) throw new ArgumentNullException(nameof(specials));
            if (obstacles == null) throw new ArgumentNullException(nameof(obstacles));
            if (obstacleLayers == null) throw new ArgumentNullException(nameof(obstacleLayers));
            if (objectiveItems == null) throw new ArgumentNullException(nameof(objectiveItems));
            if (
                tiles.Length != expected ||
                specials.Length != expected ||
                obstacles.Length != expected ||
                obstacleLayers.Length != expected ||
                objectiveItems.Length != expected
            )
            {
                throw new ArgumentException("Snapshot layer lengths must match its dimensions.");
            }

            Width = width;
            Height = height;
            TileTypeCount = tileTypeCount;
            Tiles = tiles;
            Specials = specials;
            Obstacles = obstacles;
            ObstacleLayers = obstacleLayers;
            ObjectiveItems = objectiveItems;
            StateHash = stateHash ?? string.Empty;
        }

        public bool HasMechanicState
        {
            get
            {
                for (int i = 0; i < Obstacles.Length; i++)
                {
                    if (
                        Obstacles[i] != BoardObstacleKind.None ||
                        ObstacleLayers[i] != 0 ||
                        ObjectiveItems[i] != BoardObjectiveItemKind.None
                    )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public string ToReplayToken()
        {
            var builder = new StringBuilder();
            bool hasMechanicState = HasMechanicState;
            if (hasMechanicState) builder.Append("v2|");
            builder.Append(Width).Append('x').Append(Height).Append('x').Append(TileTypeCount).Append(':');

            for (int i = 0; i < Tiles.Length; i++)
            {
                if (i > 0) builder.Append(',');
                builder.Append(Tiles[i]).Append('/').Append((int)Specials[i]);
                if (hasMechanicState)
                {
                    builder
                        .Append('/')
                        .Append((int)Obstacles[i])
                        .Append('/')
                        .Append(ObstacleLayers[i])
                        .Append('/')
                        .Append((int)ObjectiveItems[i]);
                }
            }

            builder.Append('#').Append(StateHash);
            return builder.ToString();
        }
    }
}
