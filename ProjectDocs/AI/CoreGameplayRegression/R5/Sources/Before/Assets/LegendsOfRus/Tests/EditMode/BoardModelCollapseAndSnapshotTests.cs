using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardModelCollapseAndSnapshotTests
    {
        [Test]
        public void Collapse_MovesExistingCellsDownAndRefillsEveryEmptyCell()
        {
            var model = new BoardModel(
                3,
                4,
                5,
                new SequenceBoardRandom(0, 1, 2, 3, 4)
            );
            model.LoadRows(
                ".1.",
                "0..",
                ".2.",
                "3.4"
            );

            BoardCollapseResult result = model.CollapseAndRefill();

            Assert.AreEqual(4, model.GetTile(new BoardPosition(2, 3)));
            Assert.AreEqual(3, model.GetTile(new BoardPosition(0, 3)));
            Assert.AreEqual(0, model.GetTile(new BoardPosition(0, 2)));
            Assert.AreEqual(2, model.GetTile(new BoardPosition(1, 3)));
            Assert.AreEqual(1, model.GetTile(new BoardPosition(1, 2)));
            Assert.AreEqual(7, result.Spawns.Count);
            Assert.IsTrue(result.Changed);

            for (int y = 0; y < model.Height; y++)
            for (int x = 0; x < model.Width; x++)
                Assert.GreaterOrEqual(model.GetTile(new BoardPosition(x, y)), 0);
        }

        [Test]
        public void SpecialMetadata_FallsWithItsTile()
        {
            var model = new BoardModel(3, 4, 4, new SeededBoardRandom(2));
            model.LoadRows(
                "...",
                ".1.",
                "...",
                "203"
            );
            model.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Column);

            model.CollapseAndRefill();

            // The existing bottom 0 stays in place; the special falls onto row 2.
            Assert.AreEqual(0, model.GetTile(new BoardPosition(1, 3)));
            Assert.AreEqual(SpecialGemKind.None, model.GetSpecial(new BoardPosition(1, 3)));
            Assert.AreEqual(1, model.GetTile(new BoardPosition(1, 2)));
            Assert.AreEqual(SpecialGemKind.Column, model.GetSpecial(new BoardPosition(1, 2)));
        }

        [Test]
        public void Snapshot_RestoresTilesSpecialsAndHash()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(6));
            model.GeneratePlayableBoard();
            model.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Burst);
            BoardSnapshot snapshot = model.CreateSnapshot();

            model.Swap(new BoardPosition(0, 0), new BoardPosition(1, 0));
            model.ClearCell(new BoardPosition(1, 1));
            Assert.AreNotEqual(snapshot.StateHash, model.ComputeStateHash());

            model.RestoreSnapshot(snapshot);

            Assert.AreEqual(snapshot.StateHash, model.ComputeStateHash());
            Assert.AreEqual(SpecialGemKind.Burst, model.GetSpecial(new BoardPosition(1, 1)));
        }

        [Test]
        public void TrySwapAndKeep_ValidSwapLeavesSwappedBoard()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(1));
            model.LoadRows(
                "001",
                "001",
                "110"
            );

            bool valid = model.TrySwapAndKeep(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2),
                out var runs
            );

            Assert.IsTrue(valid);
            Assert.GreaterOrEqual(runs.Count, 1);
            Assert.AreEqual(0, model.GetTile(new BoardPosition(2, 1)));
            Assert.AreEqual(1, model.GetTile(new BoardPosition(2, 2)));
        }
    }
}
