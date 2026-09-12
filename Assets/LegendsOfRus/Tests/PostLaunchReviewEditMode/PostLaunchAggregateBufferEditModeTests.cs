using NUnit.Framework;
using LegendsOfRus.PostLaunchReview;

namespace LegendsOfRus.Tests.PostLaunchReview
{
    public sealed class PostLaunchAggregateBufferEditModeTests
    {

        [Test] public void DefaultCapacityIsThirty(){Assert.That(new PostLaunchAggregateBuffer().Capacity,Is.EqualTo(30));}
        [Test] public void ZeroCapacityThrows(){Assert.Throws<System.ArgumentOutOfRangeException>(()=>new PostLaunchAggregateBuffer(0));}
        [Test] public void TooLargeCapacityThrows(){Assert.Throws<System.ArgumentOutOfRangeException>(()=>new PostLaunchAggregateBuffer(91));}
        [Test] public void NullObservationThrows(){Assert.Throws<System.ArgumentNullException>(()=>new PostLaunchAggregateBuffer().Add(null));}
        [Test] public void AddIncrementsCount(){var b=new PostLaunchAggregateBuffer(2);b.Add(new AggregateObservation());Assert.That(b.Count,Is.EqualTo(1));}
        [Test] public void BufferIsBounded(){var b=new PostLaunchAggregateBuffer(2);b.Add(new AggregateObservation{DayIndex=1});b.Add(new AggregateObservation{DayIndex=2});b.Add(new AggregateObservation{DayIndex=3});Assert.That(b.Count,Is.EqualTo(2));}
        [Test] public void SnapshotIsOldestFirst(){var b=new PostLaunchAggregateBuffer(2);b.Add(new AggregateObservation{DayIndex=1});b.Add(new AggregateObservation{DayIndex=2});b.Add(new AggregateObservation{DayIndex=3});Assert.That(b.SnapshotOldestFirst()[0].DayIndex,Is.EqualTo(2));}
        [Test] public void SnapshotIsDefensiveCopy(){var b=new PostLaunchAggregateBuffer(2);var x=new AggregateObservation{DayIndex=1};b.Add(x);x.DayIndex=9;Assert.That(b.SnapshotOldestFirst()[0].DayIndex,Is.EqualTo(1));}
        [Test] public void ClearResetsCount(){var b=new PostLaunchAggregateBuffer(2);b.Add(new AggregateObservation());b.Clear();Assert.That(b.Count,Is.Zero);}
        [Test] public void ReuseAfterClearWorks(){var b=new PostLaunchAggregateBuffer(2);b.Add(new AggregateObservation{DayIndex=1});b.Clear();b.Add(new AggregateObservation{DayIndex=2});Assert.That(b.SnapshotOldestFirst()[0].DayIndex,Is.EqualTo(2));}
    }
}
