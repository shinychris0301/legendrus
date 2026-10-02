using System.Linq;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardResolutionPlannerTests
    {
        [Test]
        public void InvalidSwap_DoesNotMutateSource()
        {
            var model = CreateLatinBoard(5, 5, 5, 101);
            string before = model.ComputeStateHash();
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(0, 0),
                new BoardPosition(1, 0)
            );

            Assert.That(plan.Status, Is.EqualTo(BoardResolutionStatus.InvalidSwap));
            Assert.That(plan.ConsumesMove, Is.False);
            Assert.That(plan.FinalSnapshot.StateHash, Is.EqualTo(before));
            Assert.That(model.ComputeStateHash(), Is.EqualTo(before));
        }

        [Test]
        public void ValidSwap_ProducesAtomicWaveAndKeepsSourceUnchanged()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(7));
            model.LoadRows("001", "001", "110");
            string before = model.ComputeStateHash();
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(plan.ConsumesMove, Is.True);
            Assert.That(plan.Waves.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(plan.Waves[0].ClearedCells.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(plan.Waves[0].SnapshotAfterClear, Is.Not.Null);
            Assert.That(plan.Waves[0].SnapshotAfterCollapse, Is.Not.Null);
            Assert.That(model.ComputeStateHash(), Is.EqualTo(before));
        }

        [Test]
        public void SameBoardAndCommand_ProduceSamePlanHash()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(7));
            model.LoadRows("001", "001", "110");
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan first = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            BoardResolutionPlan second = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );

            Assert.That(second.PlanHash, Is.EqualTo(first.PlanHash));
            Assert.That(
                second.FinalSnapshot.StateHash,
                Is.EqualTo(first.FinalSnapshot.StateHash)
            );
        }

        [Test]
        public void FourMatch_CreatesRowSpecialAndProtectsCreationCell()
        {
            var model = new BoardModel(5, 5, 5, new SeededBoardRandom(31));
            model.LoadRows(
                "00102",
                "12010",
                "20121",
                "01202",
                "12012"
            );
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(2, 0),
                new BoardPosition(2, 1)
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(plan.Waves[0].Creations.Count, Is.EqualTo(1));
            Assert.That(
                plan.Waves[0].Creations[0].Kind,
                Is.EqualTo(SpecialGemKind.Row)
            );
            BoardPosition creation = plan.Waves[0].Creations[0].Position;
            Assert.That(
                plan.Waves[0].ClearedCells.Any(cell => cell.Position == creation),
                Is.False
            );
        }

        [Test]
        public void ColorWithNormal_ClearsPartnerColor()
        {
            var model = CreateLatinBoard(5, 5, 5, 44);
            model.SetSpecial(new BoardPosition(0, 0), SpecialGemKind.Color);
            model.SetTile(new BoardPosition(1, 0), 2);
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(0, 0),
                new BoardPosition(1, 0)
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(
                plan.Waves[0].Combination,
                Is.EqualTo(SpecialCombinationKind.ColorWithNormal)
            );
            Assert.That(
                plan.Waves[0].ClearedCells.Count(cell => cell.Cell.TileType == 2),
                Is.GreaterThanOrEqualTo(5)
            );
        }

        [Test]
        public void RowWithColumn_ProducesCombinationActivations()
        {
            var model = CreateLatinBoard(5, 5, 5, 52);
            model.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Row);
            model.SetSpecial(new BoardPosition(2, 1), SpecialGemKind.Column);
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(1, 1),
                new BoardPosition(2, 1)
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(
                plan.Waves[0].Combination,
                Is.EqualTo(SpecialCombinationKind.RowWithColumn)
            );
            Assert.That(
                plan.Waves[0].Activations.Any(a => a.Kind == SpecialGemKind.Row),
                Is.True
            );
            Assert.That(
                plan.Waves[0].Activations.Any(a => a.Kind == SpecialGemKind.Column),
                Is.True
            );
        }

        [Test]
        public void ColorWithColor_ClearsEntireBoard()
        {
            var model = CreateLatinBoard(5, 5, 5, 61);
            model.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Color);
            model.SetSpecial(new BoardPosition(2, 1), SpecialGemKind.Color);
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(1, 1),
                new BoardPosition(2, 1)
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(
                plan.Waves[0].Combination,
                Is.EqualTo(SpecialCombinationKind.ColorWithColor)
            );
            Assert.That(plan.Waves[0].ClearedCells.Count, Is.EqualTo(25));
        }

        [Test]
        public void BoosterRow_PlanContainsLineActivationAndNoMoveCost()
        {
            var model = CreateLatinBoard(5, 5, 5, 71);
            string before = model.ComputeStateHash();
            var planner = new BoardResolutionPlanner(model);

            BoardResolutionPlan plan = planner.PlanBooster(
                new BoardBoosterRequest(
                    BoardBoosterCommandKind.MaceRow,
                    new BoardPosition(2, 2)
                )
            );

            Assert.That(plan.IsResolved, Is.True);
            Assert.That(plan.ConsumesMove, Is.False);
            Assert.That(plan.Booster, Is.EqualTo(BoardBoosterCommandKind.MaceRow));
            Assert.That(
                plan.Waves[0].Activations.Any(a => a.Kind == SpecialGemKind.Row),
                Is.True
            );
            Assert.That(model.ComputeStateHash(), Is.EqualTo(before));
        }

        [Test]
        public void PlanFinalSnapshot_CanBeAppliedExactly()
        {
            var model = new BoardModel(3, 3, 3, new SeededBoardRandom(7));
            model.LoadRows("001", "001", "110");
            var planner = new BoardResolutionPlanner(model);
            BoardResolutionPlan plan = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );

            model.RestoreSnapshot(plan.FinalSnapshot);

            Assert.That(
                model.ComputeStateHash(),
                Is.EqualTo(plan.FinalSnapshot.StateHash)
            );
        }

        static BoardModel CreateLatinBoard(
            int width,
            int height,
            int tileTypes,
            int seed
        )
        {
            var model = new BoardModel(
                width,
                height,
                tileTypes,
                new SeededBoardRandom(seed)
            );

            var values = new int[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                values[y * width + x] = (x + y) % tileTypes;

            model.LoadRowMajor(values);
            return model;
        }
    }
}
