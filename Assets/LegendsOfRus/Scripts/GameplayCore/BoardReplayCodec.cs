using System;
using System.Globalization;

namespace LegendsOfRus.GameplayCore
{
    public static class BoardReplayCodec
    {
        public static BoardSnapshot Decode(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Replay token is empty.", nameof(token));

            string[] hashSplit = token.Split('#');
            string body = hashSplit[0];
            string stateHash = hashSplit.Length > 1 ? hashSplit[1] : string.Empty;
            bool hasMechanicLayers = body.StartsWith("v2|", StringComparison.Ordinal);
            if (hasMechanicLayers)
                body = body.Substring(3);

            int colon = body.IndexOf(':');
            if (colon <= 0 || colon >= body.Length - 1)
                throw new FormatException("Replay token is missing its dimension or cell section.");

            string[] dimensions = body.Substring(0, colon).Split('x');
            if (dimensions.Length != 3)
                throw new FormatException("Replay dimensions must be width x height x tileTypeCount.");

            int width = ParsePositive(dimensions[0], "width");
            int height = ParsePositive(dimensions[1], "height");
            int tileTypeCount = ParsePositive(dimensions[2], "tileTypeCount");

            string[] cells = body.Substring(colon + 1).Split(',');
            int expected = width * height;
            if (cells.Length != expected)
                throw new FormatException($"Replay contains {cells.Length} cells; expected {expected}.");

            var tiles = new int[expected];
            var specials = new SpecialGemKind[expected];
            var obstacles = new BoardObstacleKind[expected];
            var obstacleLayers = new byte[expected];
            var objectiveItems = new BoardObjectiveItemKind[expected];

            for (int i = 0; i < cells.Length; i++)
            {
                string[] fields = cells[i].Split('/');
                int expectedFields = hasMechanicLayers ? 5 : 2;
                if (fields.Length != expectedFields)
                {
                    throw new FormatException(
                        hasMechanicLayers
                            ? $"Cell {i} is not encoded as tile/special/obstacle/layers/item."
                            : $"Cell {i} is not encoded as tile/special."
                    );
                }

                if (!int.TryParse(fields[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int tile))
                    throw new FormatException($"Cell {i} has an invalid tile value.");
                if (!int.TryParse(fields[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int specialValue))
                    throw new FormatException($"Cell {i} has an invalid special value.");

                if (tile < BoardModel.EmptyTile || tile >= tileTypeCount)
                    throw new FormatException($"Cell {i} tile is outside the valid range.");
                if (!Enum.IsDefined(typeof(SpecialGemKind), specialValue))
                    throw new FormatException($"Cell {i} special is outside the valid range.");

                tiles[i] = tile;
                specials[i] = tile < 0 ? SpecialGemKind.None : (SpecialGemKind)specialValue;

                if (!hasMechanicLayers) continue;

                if (
                    !int.TryParse(
                        fields[2],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int obstacleValue
                    ) ||
                    !Enum.IsDefined(typeof(BoardObstacleKind), obstacleValue)
                )
                    throw new FormatException($"Cell {i} obstacle is outside the valid range.");
                if (
                    !int.TryParse(
                        fields[3],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int layerValue
                    )
                )
                    throw new FormatException($"Cell {i} has an invalid obstacle-layer value.");
                if (
                    !int.TryParse(
                        fields[4],
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int itemValue
                    ) ||
                    !Enum.IsDefined(typeof(BoardObjectiveItemKind), itemValue)
                )
                    throw new FormatException($"Cell {i} objective item is outside the valid range.");

                BoardObstacleKind obstacle = (BoardObstacleKind)obstacleValue;
                if (
                    (obstacle == BoardObstacleKind.None && layerValue != 0) ||
                    (obstacle != BoardObstacleKind.None && (layerValue < 1 || layerValue > (obstacle == BoardObstacleKind.RunicSeal ? 2 : 3)))
                )
                    throw new FormatException($"Cell {i} obstacle layers are outside the valid range.");
                if (itemValue != 0 && tile >= 0)
                    throw new FormatException($"Cell {i} contains both a gem and an objective item.");

                if (obstacle == BoardObstacleKind.RunicSeal && (tile >= 0 || itemValue != 0 || specials[i] != SpecialGemKind.None))
                    throw new FormatException($"Cell {i} contains an occupied runic seal.");
                obstacles[i] = obstacle;
                obstacleLayers[i] = (byte)layerValue;
                objectiveItems[i] = (BoardObjectiveItemKind)itemValue;
            }

            return new BoardSnapshot(
                width,
                height,
                tileTypeCount,
                tiles,
                specials,
                obstacles,
                obstacleLayers,
                objectiveItems,
                stateHash
            );
        }

        public static bool TryDecode(string token, out BoardSnapshot snapshot)
        {
            try
            {
                snapshot = Decode(token);
                return true;
            }
            catch
            {
                snapshot = null;
                return false;
            }
        }

        static int ParsePositive(string value, string name)
        {
            if (
                !int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ||
                parsed <= 0
            )
            {
                throw new FormatException($"Replay {name} is invalid.");
            }

            return parsed;
        }
    }
}
