using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardModelGenerationTests
    {
        [Test]
        public void SameSeed_GeneratesIdenticalBoardAndHash()
        {
            var first = new BoardModel(8, 9, 6, new SeededBoardRandom(11002));
            var second = new BoardModel(8, 9, 6, new SeededBoardRandom(11002));

            first.GeneratePlayableBoard();
            second.GeneratePlayableBoard();

            Assert.AreEqual(first.ToDebugString(), second.ToDebugString());
            Assert.AreEqual(first.ComputeStateHash(), second.ComputeStateHash());
        }

        [Test]
        public void GeneratedBoard_HasNoInitialMatchesAndHasLegalMove()
        {
            var model = new BoardModel(8, 9, 6, new SeededBoardRandom(9981));
            model.GeneratePlayableBoard();

            Assert.IsFalse(model.HasAnyMatches());
            Assert.IsTrue(model.HasLegalMove());
        }

        [Test]
        public void HasLegalMove_QueryDoesNotChangeBoardOrMutationVersion()
        {
            var model = new BoardModel(8, 9, 6, new SeededBoardRandom(4455));
            model.GeneratePlayableBoard();

            string before = model.ComputeStateHash();
            int version = model.MutationVersion;

            Assert.IsTrue(model.HasLegalMove());
            Assert.AreEqual(before, model.ComputeStateHash());
            Assert.AreEqual(version, model.MutationVersion);
        }

        [Test]
        public void DeadBoard_IsReshuffledIntoPlayableState()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(123));
            model.LoadRows(
                "001",
                "001",
                "122"
            );

            Assert.IsFalse(model.HasAnyMatches());
            Assert.IsFalse(model.HasLegalMove());

            BoardReshuffleResult result = model.EnsurePlayableBoard();

            Assert.Greater(result.Attempts, 0);
            Assert.IsFalse(model.HasAnyMatches());
            Assert.IsTrue(model.HasLegalMove());
        }

        [Test]
        public void ReplayToken_IsStableForSnapshot()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(77));
            model.GeneratePlayableBoard();

            BoardSnapshot snapshot = model.CreateSnapshot();
            string first = snapshot.ToReplayToken();
            string second = model.CreateSnapshot().ToReplayToken();

            Assert.AreEqual(first, second);
            Assert.AreEqual(snapshot.StateHash, model.ComputeStateHash());
        }
    }
}
