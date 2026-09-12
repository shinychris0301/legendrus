using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class GameplayStateMachineTests
    {
        [Test]
        public void BusySwapPipeline_BlocksPointerInputUntilIdle()
        {
            var state = new GameplayStateMachine();

            Assert.That(state.CanAcceptPointerInput, Is.True);
            Assert.That(state.TryTransition(GameplayPhase.Swapping), Is.True);
            Assert.That(state.CanAcceptPointerInput, Is.False);
            Assert.That(state.TryTransition(GameplayPhase.ResolvingMatches), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.Falling), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.ResolvingMatches), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.Idle), Is.True);
            Assert.That(state.CanAcceptPointerInput, Is.True);
        }

        [Test]
        public void InvalidSwap_ReturnsToIdle()
        {
            var state = new GameplayStateMachine();

            Assert.That(state.TryTransition(GameplayPhase.Swapping), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.InvalidSwapReturning), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.Idle), Is.True);
            Assert.That(state.Current, Is.EqualTo(GameplayPhase.Idle));
        }

        [Test]
        public void PreflightRejectedSwap_ReturnsDirectlyToIdle()
        {
            var state = new GameplayStateMachine();

            Assert.That(state.TryTransition(GameplayPhase.Swapping), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.Idle), Is.True);
            Assert.That(state.Current, Is.EqualTo(GameplayPhase.Idle));
            Assert.That(state.CanAcceptPointerInput, Is.True);
        }

        [Test]
        public void BoosterTargeting_CanPauseAndResumeWithoutLosingMode()
        {
            var state = new GameplayStateMachine();

            Assert.That(state.TryTransition(GameplayPhase.BoosterTargeting), Is.True);
            Assert.That(state.TryPause(), Is.True);
            Assert.That(state.Current, Is.EqualTo(GameplayPhase.Paused));
            Assert.That(state.TryResume(), Is.True);
            Assert.That(state.Current, Is.EqualTo(GameplayPhase.BoosterTargeting));
        }

        [Test]
        public void CannotPauseDuringResolution()
        {
            var state = new GameplayStateMachine();

            Assert.That(state.TryTransition(GameplayPhase.Swapping), Is.True);
            Assert.That(state.TryTransition(GameplayPhase.ResolvingMatches), Is.True);
            Assert.That(state.TryPause(), Is.False);
            Assert.That(state.Current, Is.EqualTo(GameplayPhase.ResolvingMatches));
        }
    }
}
