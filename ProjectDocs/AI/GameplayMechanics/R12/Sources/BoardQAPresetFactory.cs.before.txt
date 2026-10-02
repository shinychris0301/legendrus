using System;
using System.Collections.Generic;

namespace LegendsOfRus.GameplayCore
{
    public sealed class BoardQAPreset
    {
        public string Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public BoardSnapshot Snapshot { get; }
        public BoardPosition Primary { get; }
        public BoardPosition Secondary { get; }
        public SpecialCombinationKind ExpectedCombination { get; }
        public int ExpectedMinimumClear { get; }
        public BoardBoosterRequest? BoosterRequest { get; }

        public bool HasSwap => ExpectedCombination != SpecialCombinationKind.None;
        public bool HasBooster => BoosterRequest.HasValue;

        public BoardQAPreset(
            string id,
            string displayName,
            string description,
            BoardSnapshot snapshot,
            BoardPosition primary,
            BoardPosition secondary,
            SpecialCombinationKind expectedCombination,
            int expectedMinimumClear,
            BoardBoosterRequest? boosterRequest = null
        )
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? id;
            Description = description ?? string.Empty;
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            Primary = primary;
            Secondary = secondary;
            ExpectedCombination = expectedCombination;
            ExpectedMinimumClear = Math.Max(0, expectedMinimumClear);
            BoosterRequest = boosterRequest;
        }
    }

    public static class BoardQAPresetFactory
    {
        public const int Width = 8;
        public const int Height = 9;
        public const int TileTypes = 6;

        static readonly IReadOnlyList<BoardQAPreset> specialPresets =
            CreateSpecialPresets();
        static readonly IReadOnlyList<BoardQAPreset> boosterPresets =
            CreateBoosterPresets();

        public static IReadOnlyList<BoardQAPreset> SpecialCombinationPresets =>
            specialPresets;

        public static IReadOnlyList<BoardQAPreset> BoosterPresets =>
            boosterPresets;

        public static IEnumerable<BoardQAPreset> AllPresets()
        {
            for (int i = 0; i < specialPresets.Count; i++)
                yield return specialPresets[i];
            for (int i = 0; i < boosterPresets.Count; i++)
                yield return boosterPresets[i];
        }

        public static bool TryGet(string id, out BoardQAPreset preset)
        {
            foreach (BoardQAPreset candidate in AllPresets())
            {
                if (string.Equals(candidate.Id, id, StringComparison.Ordinal))
                {
                    preset = candidate;
                    return true;
                }
            }

            preset = null;
            return false;
        }

        static IReadOnlyList<BoardQAPreset> CreateSpecialPresets()
        {
            var result = new List<BoardQAPreset>();

            AddSpecial(result, "special.color-normal", "Color + normal", SpecialGemKind.Color, SpecialGemKind.None,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColorWithNormal, 8);
            AddSpecial(result, "special.color-row", "Color + row", SpecialGemKind.Color, SpecialGemKind.Row,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColorWithRow, 30);
            AddSpecial(result, "special.color-column", "Color + column", SpecialGemKind.Color, SpecialGemKind.Column,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColorWithColumn, 30);
            AddSpecial(result, "special.color-burst", "Color + burst", SpecialGemKind.Color, SpecialGemKind.Burst,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColorWithBurst, 30);
            AddSpecial(result, "special.color-color", "Color + color", SpecialGemKind.Color, SpecialGemKind.Color,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColorWithColor, Width * Height);

            // Vertical adjacency gives two distinct rows.
            AddSpecial(result, "special.row-row", "Row + row", SpecialGemKind.Row, SpecialGemKind.Row,
                new BoardPosition(3, 4), new BoardPosition(3, 5), SpecialCombinationKind.RowWithRow, Width * 2);
            // Horizontal adjacency gives two distinct columns.
            AddSpecial(result, "special.column-column", "Column + column", SpecialGemKind.Column, SpecialGemKind.Column,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.ColumnWithColumn, Height * 2);
            AddSpecial(result, "special.row-column", "Row + column", SpecialGemKind.Row, SpecialGemKind.Column,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.RowWithColumn, Width + Height - 1);
            AddSpecial(result, "special.row-burst", "Row + burst", SpecialGemKind.Row, SpecialGemKind.Burst,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.RowWithBurst, Width * 3);
            AddSpecial(result, "special.column-burst", "Column + burst", SpecialGemKind.Column, SpecialGemKind.Burst,
                new BoardPosition(3, 4), new BoardPosition(3, 5), SpecialCombinationKind.ColumnWithBurst, Height * 3);
            AddSpecial(result, "special.burst-burst", "Burst + burst", SpecialGemKind.Burst, SpecialGemKind.Burst,
                new BoardPosition(3, 4), new BoardPosition(4, 4), SpecialCombinationKind.BurstWithBurst, 25);

            return result;
        }

        static IReadOnlyList<BoardQAPreset> CreateBoosterPresets()
        {
            var result = new List<BoardQAPreset>();
            BoardPosition target = new BoardPosition(3, 4);

            AddBooster(result, "booster.mace-row", "Mace row", BoardBoosterCommandKind.MaceRow, target, false);
            AddBooster(result, "booster.mace-column", "Mace column", BoardBoosterCommandKind.MaceColumn, target, false);
            AddBooster(result, "booster.firebird", "Firebird area", BoardBoosterCommandKind.FirebirdArea, target, false);
            AddBooster(result, "booster.living-light", "Living Light color", BoardBoosterCommandKind.LivingLightColor, target, false);
            AddBooster(result, "booster.invalid-empty", "Invalid empty target", BoardBoosterCommandKind.FirebirdArea,
                new BoardPosition(0, 0), true);

            return result;
        }

        static void AddSpecial(
            List<BoardQAPreset> result,
            string id,
            string displayName,
            SpecialGemKind first,
            SpecialGemKind second,
            BoardPosition a,
            BoardPosition b,
            SpecialCombinationKind expected,
            int expectedMinimumClear
        )
        {
            BoardModel model = CreateBaseModel(StableSeed(id));
            model.SetCell(a, new BoardCellData(1, first));
            model.SetCell(b, new BoardCellData(2, second));

            result.Add(
                new BoardQAPreset(
                    id,
                    displayName,
                    "Deterministic QA board for " + displayName + ".",
                    model.CreateSnapshot(),
                    a,
                    b,
                    expected,
                    expectedMinimumClear
                )
            );
        }

        static void AddBooster(
            List<BoardQAPreset> result,
            string id,
            string displayName,
            BoardBoosterCommandKind kind,
            BoardPosition target,
            bool clearTarget
        )
        {
            BoardModel model = CreateBaseModel(StableSeed(id));
            if (clearTarget)
                model.ClearCell(target);

            result.Add(
                new BoardQAPreset(
                    id,
                    displayName,
                    "Deterministic QA board for " + displayName + ".",
                    model.CreateSnapshot(),
                    target,
                    target,
                    SpecialCombinationKind.None,
                    0,
                    new BoardBoosterRequest(kind, target)
                )
            );
        }


        static int StableSeed(string value)
        {
            unchecked
            {
                int hash = 17;
                if (!string.IsNullOrEmpty(value))
                {
                    for (int i = 0; i < value.Length; i++)
                        hash = hash * 31 + value[i];
                }
                return hash;
            }
        }

        static BoardModel CreateBaseModel(int seed)
        {
            var model = new BoardModel(
                Width,
                Height,
                TileTypes,
                new SeededBoardRandom(seed)
            );

            var values = new int[Width * Height];
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                values[y * Width + x] = (x + 2 * y) % TileTypes;

            model.LoadRowMajor(values);
            return model;
        }
    }
}
