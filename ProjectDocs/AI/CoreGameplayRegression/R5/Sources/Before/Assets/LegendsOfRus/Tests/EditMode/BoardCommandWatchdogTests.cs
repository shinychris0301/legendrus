using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests
{
    public sealed class BoardCommandWatchdogTests
    {
        [Test]
        public void CommandTimesOutAtDeadline()
        {
            var watchdog = new BoardCommandWatchdog();
            long token = watchdog.Begin("swap", 10d, 2d);

            Assert.That(watchdog.IsTimedOut(11.99d), Is.False);
            Assert.That(watchdog.IsTimedOut(12d), Is.True);
            Assert.That(watchdog.Complete(token), Is.True);
            Assert.That(watchdog.IsActive, Is.False);
        }

        [Test]
        public void WrongTokenCannotCompleteCommand()
        {
            var watchdog = new BoardCommandWatchdog();
            long token = watchdog.Begin("booster", 5d, 3d);

            Assert.That(watchdog.Complete(token + 1), Is.False);
            Assert.That(watchdog.IsActive, Is.True);
            Assert.That(watchdog.Complete(token), Is.True);
        }

        [Test]
        public void ForceTimeoutMakesCommandImmediatelyExpired()
        {
            var watchdog = new BoardCommandWatchdog();
            watchdog.Begin("qa", 20d, 10d);
            watchdog.ForceTimeout(21d);

            Assert.That(watchdog.IsTimedOut(21d), Is.True);
        }
    }
}
