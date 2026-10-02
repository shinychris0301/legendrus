using System;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>
    /// Stationary board-layer mechanics. Their behavior is implemented by the
    /// deterministic resolver; the state itself remains independent of Unity.
    /// </summary>
    public enum BoardObstacleKind
    {
        None = 0,
        Ice = 1,
        Chain = 2,
        Crate = 3,
        RunicSeal = 4
    }

    /// <summary>
    /// A movable level-objective item that occupies a board cell instead of a gem.
    /// </summary>
    public enum BoardObjectiveItemKind
    {
        None = 0,
        FirebirdFeather = 1
    }

    public readonly struct BoardObstacleData : IEquatable<BoardObstacleData>
    {
        public readonly BoardObstacleKind Kind;
        public readonly int Layers;

        public BoardObstacleData(BoardObstacleKind kind, int layers)
        {
            if (!Enum.IsDefined(typeof(BoardObstacleKind), kind))
                throw new ArgumentOutOfRangeException(nameof(kind));

            if (kind == BoardObstacleKind.None)
            {
                if (layers != 0)
                    throw new ArgumentOutOfRangeException(
                        nameof(layers),
                        "An empty obstacle must have zero layers."
                    );
            }
            else if (layers < 1 || layers > 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(layers),
                    "Obstacle layers must be in [1, 3]."
                );
            }

            if (kind == BoardObstacleKind.RunicSeal && layers > 2)
                throw new ArgumentOutOfRangeException(nameof(layers), "Runic seals have at most two layers.");
            Kind = kind;
            Layers = layers;
        }

        public bool IsEmpty => Kind == BoardObstacleKind.None;
        public static bool OccupiesCell(BoardObstacleKind kind) => kind == BoardObstacleKind.Crate || kind == BoardObstacleKind.RunicSeal;

        public bool Equals(BoardObstacleData other) =>
            Kind == other.Kind && Layers == other.Layers;

        public override bool Equals(object obj) =>
            obj is BoardObstacleData other && Equals(other);

        public override int GetHashCode() => unchecked(((int)Kind * 397) ^ Layers);

        public override string ToString() => IsEmpty ? "None" : $"{Kind}:{Layers}";

        public static bool operator ==(BoardObstacleData left, BoardObstacleData right) =>
            left.Equals(right);

        public static bool operator !=(BoardObstacleData left, BoardObstacleData right) =>
            !left.Equals(right);
    }

    public readonly struct BoardObjectiveItemCollection
    {
        public readonly BoardPosition From;
        public readonly BoardPosition Exit;
        public readonly BoardObjectiveItemKind Kind;

        public BoardObjectiveItemCollection(
            BoardPosition from,
            BoardPosition exit,
            BoardObjectiveItemKind kind
        )
        {
            From = from;
            Exit = exit;
            Kind = kind;
        }
    }
}
