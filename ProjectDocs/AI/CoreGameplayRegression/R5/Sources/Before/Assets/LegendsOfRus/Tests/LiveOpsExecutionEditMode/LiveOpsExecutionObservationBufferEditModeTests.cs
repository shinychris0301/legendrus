using NUnit.Framework;
using LegendsOfRus.LiveOpsExecution;

namespace LegendsOfRus.Tests.LiveOpsExecution
{
    public sealed class LiveOpsExecutionObservationBufferEditModeTests
    {
        [Test] public void DefaultCapacity(){Assert.AreEqual(60,new LiveOpsExecutionObservationBuffer().Capacity);}
        [Test] public void MinCapacity(){Assert.AreEqual(1,new LiveOpsExecutionObservationBuffer(0).Capacity);}
        [Test] public void MaxCapacity(){Assert.AreEqual(180,new LiveOpsExecutionObservationBuffer(999).Capacity);}
        [Test] public void Add_Increments(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(new LiveOpsExecutionAggregateObservation());Assert.AreEqual(1,b.Count);}
        [Test] public void Null_Ignored(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(null);Assert.AreEqual(0,b.Count);}
        [Test] public void Bounded(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=1});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=2});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=3});Assert.AreEqual(2,b.Count);}
        [Test] public void OldestAfterWrap(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=1});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=2});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=3});Assert.AreEqual(2,b.SnapshotOldestFirst()[0].WindowIndex);}
        [Test] public void NewestAfterWrap(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=1});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=2});b.Add(new LiveOpsExecutionAggregateObservation{WindowIndex=3});Assert.AreEqual(3,b.SnapshotOldestFirst()[1].WindowIndex);}
        [Test] public void Clear(){var b=new LiveOpsExecutionObservationBuffer(2);b.Add(new LiveOpsExecutionAggregateObservation());b.Clear();Assert.AreEqual(0,b.Count);}
        [Test] public void SnapshotEmpty(){Assert.AreEqual(0,new LiveOpsExecutionObservationBuffer(2).SnapshotOldestFirst().Length);}
    }
}
