using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests
{
    public sealed class BoardDirectionResolverTests
    {
        [TestCase(0.80f, 0.10f, BoardDirection.Right)]
        [TestCase(-0.80f, 0.10f, BoardDirection.Left)]
        [TestCase(0.10f, 0.80f, BoardDirection.Up)]
        [TestCase(0.10f, -0.80f, BoardDirection.Down)]
        [TestCase(0.20f, 0.80f, BoardDirection.Up)]
        [TestCase(0.20f, -0.80f, BoardDirection.Down)]
        public void ResolveNormalized_ReturnsExpectedDirection(
            float x,
            float y,
            BoardDirection expected
        )
        {
            Assert.That(
                BoardDirectionResolver.ResolveNormalized(new Vector2(x, y)),
                Is.EqualTo(expected)
            );
        }

        [Test]
        public void ResolveNormalized_BelowThreshold_ReturnsNone()
        {
            Assert.That(
                BoardDirectionResolver.ResolveNormalized(new Vector2(0.10f, -0.12f)),
                Is.EqualTo(BoardDirection.None)
            );
        }

        [Test]
        public void ToBoardDelta_UsesTopOriginRows()
        {
            Assert.That(
                BoardDirectionResolver.ToBoardDelta(BoardDirection.Up),
                Is.EqualTo(new Vector2Int(0, -1))
            );
            Assert.That(
                BoardDirectionResolver.ToBoardDelta(BoardDirection.Down),
                Is.EqualTo(new Vector2Int(0, 1))
            );
        }
    }
}
