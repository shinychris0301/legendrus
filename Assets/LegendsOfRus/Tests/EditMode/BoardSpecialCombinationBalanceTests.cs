using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardSpecialCombinationBalanceTests
    {
        [Test]
        public void EveryCombinationPreset_ResolvesExpectedShapeAndScore()
        {
            BoardBalanceProfile balance = BoardBalanceProfile.CommercialDefault;

            foreach (BoardQAPreset preset in BoardQAPresetFactory.SpecialCombinationPresets)
            {
                BoardModel model = From(preset.Snapshot);
                BoardResolutionPlan plan = new BoardResolutionPlanner(model, balance)
                    .PlanSwap(preset.Primary, preset.Secondary);

                Assert.That(plan.IsResolved, Is.True, preset.Id);
                Assert.That(plan.Waves.Count, Is.GreaterThan(0), preset.Id);

                BoardResolutionWave first = plan.Waves[0];
                Assert.That(first.Combination, Is.EqualTo(preset.ExpectedCombination), preset.Id);
                Assert.That(
                    first.ClearedCells.Count,
                    Is.GreaterThanOrEqualTo(preset.ExpectedMinimumClear),
                    preset.Id
                );
                Assert.That(
                    first.ScoreDelta,
                    Is.EqualTo(
                        first.ClearedCells.Count *
                        balance.ScoreForCombinationCell(preset.ExpectedCombination)
                    ),
                    preset.Id
                );
            }
        }

        [Test]
        public void BurstWithBurst_IsBalancedToSingleFiveByFiveFootprint()
        {
            BoardQAPresetFactory.TryGet("special.burst-burst", out BoardQAPreset preset);
            BoardModel model = From(preset.Snapshot);
            BoardResolutionPlan plan = new BoardResolutionPlanner(model).PlanSwap(
                preset.Primary,
                preset.Secondary
            );

            Assert.That(plan.Waves[0].ClearedCells.Count, Is.EqualTo(25));
        }

        [Test]
        public void PresetIds_AreUnique()
        {
            var ids = new HashSet<string>();
            foreach (BoardQAPreset preset in BoardQAPresetFactory.AllPresets())
                Assert.That(ids.Add(preset.Id), Is.True, preset.Id);
        }

        static BoardModel From(BoardSnapshot snapshot)
        {
            var model = new BoardModel(
                snapshot.Width,
                snapshot.Height,
                snapshot.TileTypeCount,
                new SeededBoardRandom(2002)
            );
            model.RestoreSnapshot(snapshot);
            return model;
        }
    }
}
