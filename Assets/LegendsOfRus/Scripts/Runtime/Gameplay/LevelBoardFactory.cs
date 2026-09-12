using System;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    public static class LevelBoardFactory
    {
        public static BoardModel Create(LevelData level, int seed, int maxAttempts = 128)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));

            var model = new BoardModel(
                level.columns,
                level.rows,
                level.tileTypes,
                new SeededBoardRandom(seed)
            );

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                model.GeneratePlayableBoard();
                ApplyAuthoredCells(model, level.fixedCells);
                ApplyObstacles(model, level.obstacles);
                ApplyBoardItems(model, level.boardItems);
                if (!model.HasAnyMatches() && model.HasLegalMove())
                    return model;
            }

            throw new InvalidOperationException(
                $"Could not generate playable board for {level.id} after {maxAttempts} attempts."
            );
        }

        static void ApplyAuthoredCells(
            BoardModel model,
            LevelFixedCellData[] fixedCells
        )
        {
            if (fixedCells == null) return;

            for (int i = 0; i < fixedCells.Length; i++)
            {
                LevelFixedCellData authored = fixedCells[i];
                if (authored == null) continue;

                model.SetCell(
                    new BoardPosition(authored.x, authored.y),
                    new BoardCellData(
                        authored.tileType,
                        (SpecialGemKind)authored.special
                    )
                );
            }
        }

        static void ApplyObstacles(
            BoardModel model,
            LevelObstacleData[] obstacles
        )
        {
            if (obstacles == null) return;

            for (int i = 0; i < obstacles.Length; i++)
            {
                LevelObstacleData authored = obstacles[i];
                if (authored == null) continue;
                if (!LevelObstacleTypes.TryToBoardKind(authored.type, out BoardObstacleKind kind))
                {
                    throw new InvalidOperationException(
                        $"Unsupported obstacle type '{authored.type}'."
                    );
                }

                model.SetObstacle(
                    new BoardPosition(authored.x, authored.y),
                    new BoardObstacleData(kind, authored.layers)
                );
            }
        }

        static void ApplyBoardItems(
            BoardModel model,
            LevelBoardItemData[] boardItems
        )
        {
            if (boardItems == null) return;

            for (int i = 0; i < boardItems.Length; i++)
            {
                LevelBoardItemData authored = boardItems[i];
                if (authored == null) continue;
                if (!LevelBoardItemTypes.TryToBoardKind(
                    authored.type,
                    out BoardObjectiveItemKind kind
                ))
                {
                    throw new InvalidOperationException(
                        $"Unsupported board-item type '{authored.type}'."
                    );
                }

                model.SetObjectiveItem(
                    new BoardPosition(authored.x, authored.y),
                    kind
                );
            }
        }
    }
}
