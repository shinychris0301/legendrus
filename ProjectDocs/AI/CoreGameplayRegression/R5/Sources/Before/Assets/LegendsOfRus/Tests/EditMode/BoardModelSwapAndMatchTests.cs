using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardModelSwapAndMatchTests
    {
        [Test]
        public void SwapCreatesMatch_RestoresStateAfterQuery()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(1));
            model.LoadRows(
                "001",
                "001",
                "110"
            );

            var a = new BoardPosition(2, 1);
            var b = new BoardPosition(2, 2);
            string before = model.ComputeStateHash();
            int version = model.MutationVersion;

            Assert.IsTrue(model.SwapCreatesMatch(a, b));
            Assert.AreEqual(before, model.ComputeStateHash());
            Assert.AreEqual(version, model.MutationVersion);
        }

        [Test]
        public void TrySwapAndKeep_InvalidSwapRestoresBoard()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(1));
            model.LoadRows(
                "001",
                "001",
                "110"
            );

            string before = model.ComputeStateHash();
            bool valid = model.TrySwapAndKeep(
                new BoardPosition(0, 0),
                new BoardPosition(1, 0),
                out List<BoardMatchRun> runs
            );

            Assert.IsFalse(valid);
            Assert.AreEqual(0, runs.Count);
            Assert.AreEqual(before, model.ComputeStateHash());
        }

        [Test]
        public void HorizontalFour_CreatesRowSpecialAtPreferredCell()
        {
            var model = EmptyModel(5, 5, 5);
            model.LoadRows(
                "0000.",
                ".....",
                ".....",
                ".....",
                "....."
            );

            var runs = model.FindRuns();
            var preferred = new BoardPosition(1, 0);
            var creations = model.DetermineSpecialCreations(runs, preferred);

            Assert.AreEqual(1, runs.Count);
            Assert.IsTrue(runs[0].Horizontal);
            Assert.AreEqual(SpecialGemKind.Row, creations[preferred]);
        }

        [Test]
        public void VerticalFour_CreatesColumnSpecialAtPreferredCell()
        {
            var model = EmptyModel(5, 5, 5);
            model.LoadRows(
                "0....",
                "0....",
                "0....",
                "0....",
                "....."
            );

            var preferred = new BoardPosition(0, 2);
            var creations = model.DetermineSpecialCreations(model.FindRuns(), preferred);

            Assert.AreEqual(SpecialGemKind.Column, creations[preferred]);
        }

        [Test]
        public void FiveInLine_CreatesColorSpecial()
        {
            var model = EmptyModel(5, 5, 5);
            model.LoadRows(
                "11111",
                ".....",
                ".....",
                ".....",
                "....."
            );

            var preferred = new BoardPosition(3, 0);
            var creations = model.DetermineSpecialCreations(model.FindRuns(), preferred);

            Assert.AreEqual(SpecialGemKind.Color, creations[preferred]);
        }

        [Test]
        public void TIntersection_CreatesBurstAtIntersection()
        {
            var model = EmptyModel(5, 5, 5);
            model.LoadRows(
                "..2..",
                "..2..",
                "22222",
                ".....",
                "....."
            );

            var intersection = new BoardPosition(2, 2);
            var creations = model.DetermineSpecialCreations(model.FindRuns(), intersection);

            Assert.AreEqual(SpecialGemKind.Burst, creations[intersection]);
        }

        [Test]
        public void LIntersection_CreatesBurstAtIntersection()
        {
            var model = EmptyModel(5, 5, 5);
            model.LoadRows(
                "3....",
                "3....",
                "333..",
                ".....",
                "....."
            );

            var intersection = new BoardPosition(0, 2);
            var creations = model.DetermineSpecialCreations(model.FindRuns(), intersection);

            Assert.AreEqual(SpecialGemKind.Burst, creations[intersection]);
        }

        static BoardModel EmptyModel(int width, int height, int tileTypes)
        {
            return new BoardModel(width, height, tileTypes, new SeededBoardRandom(9));
        }
    }
}
