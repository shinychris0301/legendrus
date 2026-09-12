using System.Linq;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardObstacleResolutionTests
    {
        [Test]
        public void ChainCrateObjectiveItemAndEmptyCell_BlockSwaps_WhileIceAllowsThem()
        {
            var board = CreateLatinBoard(4, 4, 4, 12201);
            var a = new BoardPosition(0, 0);
            var b = new BoardPosition(1, 0);

            board.SetObstacle(a, new BoardObstacleData(BoardObstacleKind.Ice, 1));
            Assert.That(board.CanSwap(a, b), Is.True);

            board.SetObstacle(a, new BoardObstacleData(BoardObstacleKind.Chain, 1));
            Assert.That(board.CanSwap(a, b), Is.False);

            board.SetObstacle(a, new BoardObstacleData(BoardObstacleKind.Crate, 1));
            Assert.That(board.CanSwap(a, b), Is.False);

            board.ClearObstacle(a);
            board.SetObjectiveItem(a, BoardObjectiveItemKind.FirebirdFeather);
            Assert.That(board.CanSwap(a, b), Is.False);

            board.ClearObjectiveItem(a);
            board.ClearCell(a);
            Assert.That(board.CanSwap(a, b), Is.False);
        }

        [Test]
        public void Planner_RejectsSwapThroughChainWithoutMutatingSource()
        {
            var board = CreateLatinBoard(4, 4, 4, 12202);
            var chained = new BoardPosition(0, 0);
            var neighbor = new BoardPosition(1, 0);
            board.SetObstacle(
                chained,
                new BoardObstacleData(BoardObstacleKind.Chain, 2)
            );
            string before = board.ComputeStateHash();

            BoardResolutionPlan plan = new BoardResolutionPlanner(board).PlanSwap(
                chained,
                neighbor
            );

            Assert.That(plan.Status, Is.EqualTo(BoardResolutionStatus.Rejected));
            Assert.That(plan.ConsumesMove, Is.False);
            Assert.That(plan.RejectionReason, Does.Contain("blocks this swap"));
            Assert.That(board.ComputeStateHash(), Is.EqualTo(before));
        }

        [Test]
        public void Match_DamagesIceAndChainOncePerWave()
        {
            var board = CreateDoubleMatchBoard(12203);
            board.SetObstacle(
                new BoardPosition(0, 1),
                new BoardObstacleData(BoardObstacleKind.Ice, 2)
            );
            board.SetObstacle(
                new BoardPosition(1, 1),
                new BoardObstacleData(BoardObstacleKind.Chain, 1)
            );

            BoardResolutionPlan plan = ResolveDoubleMatch(board);
            BoardResolutionWave first = plan.Waves[0];

            Assert.That(first.ObstacleInteractions.Count, Is.EqualTo(2));
            Assert.That(
                first.ObstacleInteractions.Single(
                    hit => hit.Position == new BoardPosition(0, 1)
                ).LayersAfter,
                Is.EqualTo(1)
            );
            Assert.That(
                first.ObstacleInteractions.Single(
                    hit => hit.Position == new BoardPosition(1, 1)
                ).Destroyed,
                Is.True
            );
            Assert.That(
                first.ObstacleInteractions.All(
                    hit => hit.Cause == BoardObstacleHitCause.DirectClear
                ),
                Is.True
            );
        }

        [Test]
        public void AdjacentMatch_DamagesCrateOnlyOnceForTheWave()
        {
            var board = CreateDoubleMatchBoard(12204);
            var crate = new BoardPosition(1, 0);
            board.SetObstacle(
                crate,
                new BoardObstacleData(BoardObstacleKind.Crate, 2)
            );

            BoardResolutionPlan plan = ResolveDoubleMatch(board);
            BoardResolutionWave first = plan.Waves[0];
            var crateHits = first.ObstacleInteractions
                .Where(hit => hit.Position == crate)
                .ToArray();

            Assert.That(crateHits, Has.Length.EqualTo(1));
            Assert.That(crateHits[0].Kind, Is.EqualTo(BoardObstacleKind.Crate));
            Assert.That(crateHits[0].LayersBefore, Is.EqualTo(2));
            Assert.That(crateHits[0].LayersAfter, Is.EqualTo(1));
            Assert.That(
                crateHits[0].Cause,
                Is.EqualTo(BoardObstacleHitCause.AdjacentMatch)
            );
        }

        [Test]
        public void Crate_SegmentsColumnAndPreventsSpawnsBelowIt()
        {
            var board = new BoardModel(
                3,
                5,
                4,
                new SequenceBoardRandom(0, 1, 2, 3)
            );
            board.LoadRows(
                ".1.",
                "...",
                "...",
                ".2.",
                "..."
            );
            var crate = new BoardPosition(1, 2);
            board.SetObstacle(
                crate,
                new BoardObstacleData(BoardObstacleKind.Crate, 1)
            );

            BoardCollapseResult result = board.CollapseAndRefill();

            Assert.That(board.GetObstacle(crate).Kind, Is.EqualTo(BoardObstacleKind.Crate));
            Assert.That(board.GetTile(new BoardPosition(1, 4)), Is.EqualTo(2));
            Assert.That(
                board.GetCell(new BoardPosition(1, 3)).IsEmpty,
                Is.True
            );
            Assert.That(
                result.Spawns.Any(spawn =>
                    spawn.To.X == 1 && spawn.To.Y > crate.Y),
                Is.False
            );
        }

        [Test]
        public void DeadBoardRecovery_PreservesCrateBlockedEmptyCells()
        {
            var board = new BoardModel(3, 3, 3, new SeededBoardRandom(12208));
            board.LoadRows("001", "001", "122");
            var crate = new BoardPosition(1, 1);
            var blockedEmpty = new BoardPosition(1, 2);
            board.SetObstacle(
                crate,
                new BoardObstacleData(BoardObstacleKind.Crate, 2)
            );
            board.ClearCell(blockedEmpty);

            BoardReshuffleResult result = board.EnsurePlayableBoard(128);

            Assert.That(result.Attempts, Is.GreaterThan(0));
            Assert.That(board.HasAnyMatches(), Is.False);
            Assert.That(board.HasLegalMove(), Is.True);
            Assert.That(board.GetCell(blockedEmpty).IsEmpty, Is.True);
            Assert.That(board.GetObstacle(crate).Kind, Is.EqualTo(BoardObstacleKind.Crate));
        }

        [Test]
        public void ObjectiveItem_CollectsOnlyWhenItCanReachBottomExit()
        {
            var board = new BoardModel(3, 4, 4, new SeededBoardRandom(12205));
            board.ClearAll();
            var itemStart = new BoardPosition(1, 1);
            board.SetObjectiveItem(
                itemStart,
                BoardObjectiveItemKind.FirebirdFeather
            );

            BoardCollapseResult collected = board.CollapseAndRefill();

            Assert.That(collected.CollectedObjectiveItems, Has.Count.EqualTo(1));
            Assert.That(collected.CollectedObjectiveItems[0].From, Is.EqualTo(itemStart));
            Assert.That(
                collected.CollectedObjectiveItems[0].Exit,
                Is.EqualTo(new BoardPosition(1, board.Height))
            );
            Assert.That(
                board.CreateSnapshot().ObjectiveItems.All(
                    item => item == BoardObjectiveItemKind.None
                ),
                Is.True
            );
        }

        [Test]
        public void ObjectiveItem_SurvivesDirectClearFootprint()
        {
            var board = CreateLatinBoard(5, 5, 5, 12207);
            var item = new BoardPosition(4, 2);
            board.SetObjectiveItem(
                item,
                BoardObjectiveItemKind.FirebirdFeather
            );

            BoardResolutionPlan plan = new BoardResolutionPlanner(board).PlanBooster(
                new BoardBoosterRequest(
                    BoardBoosterCommandKind.MaceRow,
                    new BoardPosition(2, 2)
                )
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(
                plan.Waves[0].ClearedCells.Any(cell => cell.Position == item),
                Is.False
            );
            int itemIndex = item.Y * board.Width + item.X;
            Assert.That(
                plan.Waves[0].SnapshotAfterClear.ObjectiveItems[itemIndex],
                Is.EqualTo(BoardObjectiveItemKind.FirebirdFeather)
            );
        }

        [Test]
        public void MechanicPlan_IsDeterministicAndKeepsSourceImmutable()
        {
            var board = CreateDoubleMatchBoard(12206);
            board.SetObstacle(
                new BoardPosition(0, 1),
                new BoardObstacleData(BoardObstacleKind.Ice, 2)
            );
            string before = board.ComputeStateHash();
            var planner = new BoardResolutionPlanner(board);

            BoardResolutionPlan first = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            BoardResolutionPlan second = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );

            Assert.That(second.PlanHash, Is.EqualTo(first.PlanHash));
            Assert.That(second.FinalSnapshot.StateHash, Is.EqualTo(first.FinalSnapshot.StateHash));
            Assert.That(board.ComputeStateHash(), Is.EqualTo(before));
        }

        static BoardModel CreateDoubleMatchBoard(int seed)
        {
            var board = new BoardModel(3, 3, 3, new SeededBoardRandom(seed));
            board.LoadRows("001", "001", "110");
            return board;
        }

        static BoardResolutionPlan ResolveDoubleMatch(BoardModel board)
        {
            BoardResolutionPlan plan = new BoardResolutionPlanner(board).PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            Assert.That(plan.IsResolved, Is.True);
            Assert.That(plan.Waves, Is.Not.Empty);
            return plan;
        }

        static BoardModel CreateLatinBoard(
            int width,
            int height,
            int tileTypes,
            int seed
        )
        {
            var board = new BoardModel(
                width,
                height,
                tileTypes,
                new SeededBoardRandom(seed)
            );
            var values = new int[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                values[y * width + x] = (x + y) % tileTypes;
            board.LoadRowMajor(values);
            return board;
        }
    }
}
