using System;
using System.Collections.Generic;

namespace LegendsOfRus.GameplayCore
{
    public enum BoardBoosterValidationCode
    {
        Valid = 0,
        MissingCommand = 1,
        OutsideBoard = 2,
        EmptyTarget = 3,
        UnsupportedCommand = 4,
        NoAffectedCells = 5
    }

    public sealed class BoardBoosterValidationResult
    {
        public BoardBoosterValidationCode Code { get; }
        public string Reason { get; }
        public IReadOnlyList<BoardPosition> AffectedCells { get; }
        public int TargetTileType { get; }

        public bool IsValid => Code == BoardBoosterValidationCode.Valid;

        public BoardBoosterValidationResult(
            BoardBoosterValidationCode code,
            string reason,
            IReadOnlyList<BoardPosition> affectedCells,
            int targetTileType
        )
        {
            Code = code;
            Reason = reason ?? string.Empty;
            AffectedCells = affectedCells ?? Array.Empty<BoardPosition>();
            TargetTileType = targetTileType;
        }
    }

    /// <summary>
    /// Pure command validation used by both runtime and tests. A booster command
    /// is accepted only when its target and resulting footprint are valid.
    /// </summary>
    public static class BoardBoosterCommandValidator
    {
        public static BoardBoosterValidationResult Validate(
            BoardModel board,
            BoardBoosterRequest request,
            BoardBalanceProfile balance = null, bool allowSealTarget = false
        )
        {
            if (board == null) throw new ArgumentNullException(nameof(board));
            balance = balance ?? BoardBalanceProfile.CommercialDefault;

            if (request.Kind == BoardBoosterCommandKind.None)
            {
                return Reject(
                    BoardBoosterValidationCode.MissingCommand,
                    "No booster command was supplied."
                );
            }

            if (!board.IsInside(request.Target))
            {
                return Reject(
                    BoardBoosterValidationCode.OutsideBoard,
                    "The booster target is outside the board."
                );
            }

            BoardCellData target = board.GetCell(request.Target);
            if (target.IsEmpty && !(allowSealTarget && board.GetObstacle(request.Target).Kind == BoardObstacleKind.RunicSeal && request.Kind != BoardBoosterCommandKind.LivingLightColor))
            {
                return Reject(
                    BoardBoosterValidationCode.EmptyTarget,
                    "The booster target is empty."
                );
            }

            var affected = new List<BoardPosition>();

            switch (request.Kind)
            {
                case BoardBoosterCommandKind.MaceRow:
                    for (int x = 0; x < board.Width; x++)
                        AddIfOccupied(board, new BoardPosition(x, request.Target.Y), affected);
                    break;

                case BoardBoosterCommandKind.MaceColumn:
                    for (int y = 0; y < board.Height; y++)
                        AddIfOccupied(board, new BoardPosition(request.Target.X, y), affected);
                    break;

                case BoardBoosterCommandKind.FirebirdArea:
                    for (int dy = -balance.BurstRadius; dy <= balance.BurstRadius; dy++)
                    for (int dx = -balance.BurstRadius; dx <= balance.BurstRadius; dx++)
                    {
                        AddIfOccupied(
                            board,
                            new BoardPosition(request.Target.X + dx, request.Target.Y + dy),
                            affected
                        );
                    }
                    break;

                case BoardBoosterCommandKind.LivingLightColor:
                    for (int y = 0; y < board.Height; y++)
                    for (int x = 0; x < board.Width; x++)
                    {
                        var position = new BoardPosition(x, y);
                        BoardCellData cell = board.GetCell(position);
                        if (!cell.IsEmpty && cell.TileType == target.TileType)
                            affected.Add(position);
                    }
                    break;

                default:
                    return Reject(
                        BoardBoosterValidationCode.UnsupportedCommand,
                        "The booster command is not supported."
                    );
            }

            affected.Sort();
            if (affected.Count == 0)
            {
                return Reject(
                    BoardBoosterValidationCode.NoAffectedCells,
                    "The booster command would not affect any populated cells."
                );
            }

            return new BoardBoosterValidationResult(
                BoardBoosterValidationCode.Valid,
                string.Empty,
                affected,
                target.TileType
            );
        }

        static BoardBoosterValidationResult Reject(
            BoardBoosterValidationCode code,
            string reason
        )
        {
            return new BoardBoosterValidationResult(
                code,
                reason,
                Array.Empty<BoardPosition>(),
                BoardModel.EmptyTile
            );
        }

        static void AddIfOccupied(
            BoardModel board,
            BoardPosition position,
            List<BoardPosition> affected
        )
        {
            if (!board.IsInside(position)) return;
            if (!board.GetCell(position).IsEmpty || board.GetObstacle(position).Kind == BoardObstacleKind.RunicSeal)
                affected.Add(position);
        }
    }
}
