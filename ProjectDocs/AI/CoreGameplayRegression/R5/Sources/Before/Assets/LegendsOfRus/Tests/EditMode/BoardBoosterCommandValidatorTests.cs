using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardBoosterCommandValidatorTests
    {
        [Test]
        public void EmptyTarget_IsRejected()
        {
            BoardQAPresetFactory.TryGet("booster.invalid-empty", out BoardQAPreset preset);
            BoardModel model = From(preset.Snapshot);

            BoardBoosterValidationResult result =
                BoardBoosterCommandValidator.Validate(
                    model,
                    preset.BoosterRequest.Value
                );

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Code, Is.EqualTo(BoardBoosterValidationCode.EmptyTarget));
        }

        [TestCase("booster.mace-row", 8)]
        [TestCase("booster.mace-column", 9)]
        [TestCase("booster.firebird", 9)]
        public void FixedShapeBoosters_HaveExpectedFootprint(
            string id,
            int expected
        )
        {
            BoardQAPresetFactory.TryGet(id, out BoardQAPreset preset);
            BoardModel model = From(preset.Snapshot);

            BoardBoosterValidationResult result =
                BoardBoosterCommandValidator.Validate(
                    model,
                    preset.BoosterRequest.Value
                );

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.AffectedCells.Count, Is.EqualTo(expected));
        }

        [Test]
        public void LivingLight_AffectsEveryGemOfTargetColor()
        {
            BoardQAPresetFactory.TryGet("booster.living-light", out BoardQAPreset preset);
            BoardModel model = From(preset.Snapshot);
            int tile = model.GetTile(preset.Primary);
            int expected = 0;

            for (int y = 0; y < model.Height; y++)
            for (int x = 0; x < model.Width; x++)
                if (model.GetTile(new BoardPosition(x, y)) == tile)
                    expected++;

            BoardBoosterValidationResult result =
                BoardBoosterCommandValidator.Validate(
                    model,
                    preset.BoosterRequest.Value
                );

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.AffectedCells.Count, Is.EqualTo(expected));
        }

        static BoardModel From(BoardSnapshot snapshot)
        {
            var model = new BoardModel(
                snapshot.Width,
                snapshot.Height,
                snapshot.TileTypeCount,
                new SeededBoardRandom(1001)
            );
            model.RestoreSnapshot(snapshot);
            return model;
        }
    }
}
