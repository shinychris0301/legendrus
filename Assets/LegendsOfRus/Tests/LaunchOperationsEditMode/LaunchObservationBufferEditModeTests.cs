using System;
using LegendsOfRus.LaunchOperations;
using NUnit.Framework;

namespace LegendsOfRus.Tests.LaunchOperations
{
    public sealed class LaunchObservationBufferEditModeTests
    {
        [Test] public void DefaultCapacity_Is64() => Assert.That(new LaunchObservationBuffer().Capacity, Is.EqualTo(64));
        [Test] public void ZeroCapacity_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new LaunchObservationBuffer(0));
        [Test] public void AboveMaximumCapacity_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => new LaunchObservationBuffer(65));
        [Test] public void Add_IncrementsCount() { var b=new LaunchObservationBuffer(2); b.Add(E(),1,0,0); Assert.That(b.Count, Is.EqualTo(1)); }
        [Test] public void Buffer_IsBounded() { var b=new LaunchObservationBuffer(2); b.Add(E(),1,0,0); b.Add(E(),1,0,0); b.Add(E(),1,0,0); Assert.That(b.Count, Is.EqualTo(2)); }
        [Test] public void Buffer_KeepsNewestRecords() { var b=new LaunchObservationBuffer(2); b.Add(E(LaunchHealthClassification.Healthy),1,0,0); b.Add(E(LaunchHealthClassification.Watch),2,0,0); b.Add(E(LaunchHealthClassification.Critical),3,1,1); var s=b.Snapshot(); Assert.That(s[0].Classification==LaunchHealthClassification.Watch && s[1].Classification==LaunchHealthClassification.Critical, Is.True); }
        [Test] public void Snapshot_SequenceIsMonotonic() { var b=new LaunchObservationBuffer(3); b.Add(E(),1,0,0); b.Add(E(),1,0,0); var s=b.Snapshot(); Assert.That(s[1].Sequence>s[0].Sequence, Is.True); }
        [Test] public void Clear_ResetsCount() { var b=new LaunchObservationBuffer(2); b.Add(E(),1,0,0); b.Clear(); Assert.That(b.Count, Is.EqualTo(0)); }
        [Test] public void NullEvaluation_Throws() { var b=new LaunchObservationBuffer(2); Assert.Throws<ArgumentNullException>(() => b.Add(null,1,0,0)); }
        [Test] public void Snapshot_StoresIncidentCounts() { var b=new LaunchObservationBuffer(2); b.Add(E(),1,2,3); var s=b.Snapshot(); Assert.That(s[0].OpenHighIncidents==2 && s[0].OpenCriticalIncidents==3, Is.True); }

        static LaunchHealthEvaluation E(LaunchHealthClassification c=LaunchHealthClassification.Healthy) => new LaunchHealthEvaluation { Classification=c, Recommendation=LaunchRolloutRecommendation.Continue };
    }
}
