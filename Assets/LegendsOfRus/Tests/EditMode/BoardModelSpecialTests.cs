using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardModelSpecialTests
    {
        [Test]
        public void RowSpecial_ChainsIntoColumnSpecial()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(3));
            model.LoadRows(
                "0123",
                "1230",
                "2301",
                "3012"
            );
            model.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Row);
            model.SetSpecial(new BoardPosition(3, 1), SpecialGemKind.Column);

            var triggered = new List<BoardPosition>();
            var clear = model.ExpandSpecialEffects(
                new[] { new BoardPosition(1, 1) },
                triggered
            );

            Assert.Contains(new BoardPosition(1, 1), triggered);
            Assert.Contains(new BoardPosition(3, 1), triggered);
            Assert.AreEqual(7, clear.Count);
        }

        [Test]
        public void BurstAtCorner_ClearsOnlyInsideBoard()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(3));
            model.LoadRows(
                "0123",
                "1230",
                "2301",
                "3012"
            );
            model.SetSpecial(new BoardPosition(0, 0), SpecialGemKind.Burst);

            var clear = model.ExpandSpecialEffects(
                new[] { new BoardPosition(0, 0) }
            );

            Assert.AreEqual(4, clear.Count);
            Assert.Contains(new BoardPosition(0, 0), new List<BoardPosition>(clear));
            Assert.Contains(new BoardPosition(1, 1), new List<BoardPosition>(clear));
        }

        [Test]
        public void ColorSpecial_ClearsAllCellsOfItsTileType()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(3));
            model.LoadRows(
                "0120",
                "1230",
                "2301",
                "3012"
            );
            var color = new BoardPosition(0, 0);
            model.SetSpecial(color, SpecialGemKind.Color);

            var clear = model.ExpandSpecialEffects(new[] { color });

            int expectedZeroes = 0;
            for (int y = 0; y < model.Height; y++)
            for (int x = 0; x < model.Width; x++)
                if (model.GetTile(new BoardPosition(x, y)) == 0) expectedZeroes++;

            Assert.AreEqual(expectedZeroes, clear.Count);
        }

        [Test]
        public void ClearingCells_RemovesSpecialMetadata()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(3));
            model.LoadRows(
                "012",
                "120",
                "201"
            );
            var position = new BoardPosition(1, 1);
            model.SetSpecial(position, SpecialGemKind.Row);

            model.ClearCell(position);

            Assert.AreEqual(BoardModel.EmptyTile, model.GetTile(position));
            Assert.AreEqual(SpecialGemKind.None, model.GetSpecial(position));
        }
    }
}
