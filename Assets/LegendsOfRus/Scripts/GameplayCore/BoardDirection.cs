using UnityEngine;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>
    /// Semantic directions in board coordinates.
    /// The board stores row 0 at the top, so Up maps to row -1.
    /// </summary>
    public enum BoardDirection
    {
        None = 0,
        Left,
        Right,
        Up,
        Down
    }

    public static class BoardDirectionResolver
    {
        public const float DefaultThresholdInCells = 0.20f;

        /// <summary>
        /// Resolves a drag normalized by the source cell's width/height.
        /// Positive local Y means the finger moved upward in Unity UI space.
        /// </summary>
        public static BoardDirection ResolveNormalized(
            Vector2 normalizedDelta,
            float thresholdInCells = DefaultThresholdInCells
        )
        {
            float threshold = Mathf.Max(0f, thresholdInCells);
            float absX = Mathf.Abs(normalizedDelta.x);
            float absY = Mathf.Abs(normalizedDelta.y);

            if (Mathf.Max(absX, absY) < threshold)
                return BoardDirection.None;

            if (absX > absY)
                return normalizedDelta.x > 0f
                    ? BoardDirection.Right
                    : BoardDirection.Left;

            return normalizedDelta.y > 0f
                ? BoardDirection.Up
                : BoardDirection.Down;
        }

        public static bool TryResolveNormalized(
            Vector2 normalizedDelta,
            out BoardDirection direction,
            float thresholdInCells = DefaultThresholdInCells
        )
        {
            direction = ResolveNormalized(normalizedDelta, thresholdInCells);
            return direction != BoardDirection.None;
        }

        public static Vector2Int ToBoardDelta(BoardDirection direction)
        {
            switch (direction)
            {
                case BoardDirection.Left:
                    return Vector2Int.left;
                case BoardDirection.Right:
                    return Vector2Int.right;
                case BoardDirection.Up:
                    return new Vector2Int(0, -1);
                case BoardDirection.Down:
                    return new Vector2Int(0, 1);
                default:
                    return Vector2Int.zero;
            }
        }
    }
}
