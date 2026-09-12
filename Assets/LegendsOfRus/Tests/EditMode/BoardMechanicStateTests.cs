using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardMechanicStateTests
    {
        [Test]
        public void SnapshotReplay_RestoresObstacleLayersAndObjectiveItems()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(12101));
            model.GeneratePlayableBoard();
            var obstaclePosition = new BoardPosition(1, 1);
            var itemPosition = new BoardPosition(2, 0);
            model.SetObstacle(
                obstaclePosition,
                new BoardObstacleData(BoardObstacleKind.Ice, 2)
            );
            model.SetObjectiveItem(
                itemPosition,
                BoardObjectiveItemKind.FirebirdFeather
            );

            BoardSnapshot snapshot = model.CreateSnapshot();
            string token = snapshot.ToReplayToken();
            Assert.That(token, Does.StartWith("v2|"));

            BoardSnapshot decoded = BoardReplayCodec.Decode(token);
            var restored = new BoardModel(4, 4, 4, new SeededBoardRandom(12102));
            restored.RestoreSnapshot(decoded);

            Assert.That(restored.ComputeStateHash(), Is.EqualTo(snapshot.StateHash));
            Assert.That(
                restored.GetObstacle(obstaclePosition),
                Is.EqualTo(new BoardObstacleData(BoardObstacleKind.Ice, 2))
            );
            Assert.That(
                restored.GetObjectiveItem(itemPosition),
                Is.EqualTo(BoardObjectiveItemKind.FirebirdFeather)
            );
            Assert.That(restored.GetTile(itemPosition), Is.EqualTo(BoardModel.EmptyTile));
        }

        [Test]
        public void ReplayCodec_RemainsCompatibleWithV1BoardTokens()
        {
            var model = new BoardModel(4, 4, 4, new SeededBoardRandom(12103));
            model.GeneratePlayableBoard();
            BoardSnapshot original = model.CreateSnapshot();
            string token = original.ToReplayToken();

            Assert.That(token, Does.Not.StartWith("v2|"));
            BoardSnapshot decoded = BoardReplayCodec.Decode(token);
            var restored = new BoardModel(4, 4, 4, new SeededBoardRandom(12104));
            restored.RestoreSnapshot(decoded);

            Assert.That(decoded.HasMechanicState, Is.False);
            Assert.That(restored.ComputeStateHash(), Is.EqualTo(original.StateHash));
        }

        [Test]
        public void Collapse_CollectsObjectiveItemAtExitAndKeepsObstacleStationary()
        {
            var model = new BoardModel(3, 4, 4, new SeededBoardRandom(12105));
            model.LoadRows(
                "...",
                ".1.",
                "...",
                "2.3"
            );
            var obstaclePosition = new BoardPosition(0, 0);
            model.SetObstacle(
                obstaclePosition,
                new BoardObstacleData(BoardObstacleKind.Chain, 1)
            );
            model.SetObjectiveItem(
                new BoardPosition(1, 1),
                BoardObjectiveItemKind.FirebirdFeather
            );

            BoardCollapseResult result = model.CollapseAndRefill();

            Assert.That(
                result.CollectedObjectiveItems,
                Has.Count.EqualTo(1)
            );
            Assert.That(
                result.CollectedObjectiveItems[0].Kind,
                Is.EqualTo(BoardObjectiveItemKind.FirebirdFeather)
            );
            Assert.That(
                model.GetObstacle(obstaclePosition),
                Is.EqualTo(new BoardObstacleData(BoardObstacleKind.Chain, 1))
            );
        }

        [Test]
        public void ClearAll_RemovesEveryMechanicLayer()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(12106));
            model.GeneratePlayableBoard();
            model.SetObstacle(
                new BoardPosition(0, 0),
                new BoardObstacleData(BoardObstacleKind.Crate, 3)
            );
            model.SetObjectiveItem(
                new BoardPosition(1, 1),
                BoardObjectiveItemKind.FirebirdFeather
            );

            model.ClearAll();

            Assert.That(model.CreateSnapshot().HasMechanicState, Is.False);
            Assert.That(model.GetCell(new BoardPosition(1, 1)).IsEmpty, Is.True);
        }
    }
}
