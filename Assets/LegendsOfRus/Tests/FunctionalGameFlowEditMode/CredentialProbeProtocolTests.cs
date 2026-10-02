#if UNITY_EDITOR
using System;
using System.Text;
using System.Security.Cryptography;
using NUnit.Framework;

namespace LegendsOfRus.Tests.FunctionalGameFlow
{
    public sealed class CredentialProbeProtocolTests
    {
        // Protocol fixture only. Actual AES/AtomicFile/JNI acceptance requires the Android runner.
        sealed class Host : ICredentialProbeHost
        {
            public bool Enabled { get; set; } = true;
            public bool AccountConfigured { get; set; }
            public int ProcessId { get; set; } = 11;
            public bool OwnershipDirectoryExists { get; set; }
            internal bool Key, Partial, Corrupted, FailWrite, FailMarker, FailRead, AcceptCorrupt, AcceptMissingKey;
            internal byte[] Record;
            internal string Payload;
            internal CredentialProbeState State;
            internal int Reads, Writes, Faults, KeyDeletes, Cleanups;
            public bool StoreEmpty => Record == null && !Key && !Partial;
            static string Hash(byte[] bytes)
            { if (bytes == null) return null; using(var sha=SHA256.Create())return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-","").ToLowerInvariant(); }
            public CredentialProbeState LoadState() => State == null ? null : new CredentialProbeState {
                version=State.version, runId=State.runId, stage=State.stage, processId=State.processId, recordHash=State.recordHash, alternateHash=State.alternateHash
            };
            public void SaveState(CredentialProbeState state, bool create)
            {
                if(FailMarker)throw new InvalidOperationException("synthetic marker failure");
                State=state;State=LoadState();OwnershipDirectoryExists=true;
            }
            public bool RecordMatches(CredentialProbeState s,bool allowAlternate=false) => !Partial && Record != null &&
                (Hash(Record)==s.recordHash || (allowAlternate && Hash(Record)==s.alternateHash));
            public string RecordHash()=>Hash(Record);
            public bool Read(out string value)
            {
                Reads++;value=null;
                if(FailRead || (Corrupted && !AcceptCorrupt) || (Record!=null && !Key && !AcceptMissingKey))return false;
                if(Record==null)return !Key;
                Partial=false;value=Payload;return true;
            }
            public bool Write(string value)
            {
                Writes++;
                if(FailWrite || Corrupted || (Record!=null && !Key))return false;
                Key=true;Payload=value;Record=Encoding.UTF8.GetBytes("synthetic encrypted fixture: "+value);return true;
            }
            public void BeginInterruptedWrite(CredentialProbeState state){Partial=true;}
            public bool InterruptedFilesMatch(CredentialProbeState state)=>Partial && Hash(Record)==state.recordHash;
            public byte[] Corrupt(CredentialProbeState state)
            {
                Faults++;var old=(byte[])Record.Clone();Record[Record.Length-1]^=64;Corrupted=true;
                state.alternateHash=Hash(Record);SaveState(state,false);return old;
            }
            public void Restore(CredentialProbeState state,byte[] original){Record=original;Corrupted=false;state.alternateHash=null;SaveState(state,false);}
            public void RemoveKey(CredentialProbeState state){KeyDeletes++;Key=false;}
            public void Cleanup(CredentialProbeState state){Cleanups++;Key=false;Record=null;State=null;OwnershipDirectoryExists=false;}
        }
        static CredentialProbeReport Run(string command,Host host)=>CredentialProbeProtocol.Run(command,host);
        static Host Prepared()
        {
            var h=new Host();Assert.AreEqual("awaiting_restart",Run("prepare",h).status);return h;
        }
        static Host Interrupted()
        {
            var h=Prepared();h.ProcessId++;Assert.AreEqual("awaiting_force_stop",Run("restart",h).status);h.ProcessId++;return h;
        }
        static void NoVaultAccess(Host h){Assert.AreEqual(0,h.Reads);Assert.AreEqual(0,h.Writes);Assert.AreEqual(0,h.Faults);Assert.AreEqual(0,h.KeyDeletes);Assert.AreEqual(0,h.Cleanups);}
        [Test] public void ThreeProcessesCompleteAndRemoveOnlyOwnedFixture()
        {
            var h=Interrupted();var result=Run("verify",h);Assert.AreEqual("passed",result.status);
            CollectionAssert.AreEqual(new[]{"interrupted_write_recovery","corruption_rejected","owned_fixture_restored","missing_key_rejected","owned_fixture_cleanup"},result.checks);
            Assert.IsTrue(h.StoreEmpty);Assert.IsFalse(h.OwnershipDirectoryExists);Assert.IsFalse(result.authenticatedSessionTested);
        }
        [TestCase(null)] [TestCase("")] [TestCase("delete")] [TestCase("../prepare")]
        public void InvalidCommandCannotTouchVault(string command){var h=new Host();Assert.AreEqual("refused",Run(command,h).status);NoVaultAccess(h);}
        [Test] public void NonDevelopmentHostCannotTouchVault(){var h=new Host{Enabled=false};Assert.AreEqual("refused",Run("prepare",h).status);NoVaultAccess(h);}
        [Test] public void ConfiguredAccountCannotTouchVault(){var h=new Host{AccountConfigured=true};Assert.AreEqual("refused",Run("prepare",h).status);NoVaultAccess(h);}
        [TestCase(true,false,false)] [TestCase(false,true,false)] [TestCase(false,false,true)] [TestCase(true,true,true)]
        public void ExistingRecordKeyOrSidecarCannotBeReadOrDeleted(bool record,bool key,bool sidecar)
        {
            var h=new Host{Record=record?new byte[]{1}:null,Key=key,Partial=sidecar};Assert.AreEqual("refused",Run("prepare",h).status);NoVaultAccess(h);
        }
        [Test] public void ExistingOwnershipDirectoryCannotBeOverwritten(){var h=new Host{OwnershipDirectoryExists=true};Assert.AreEqual("refused",Run("prepare",h).status);NoVaultAccess(h);}
        [TestCase("restart")] [TestCase("verify")] [TestCase("cleanup")]
        public void MissingOrMalformedOwnershipRefuses(string command)
        {
            var h=new Host{OwnershipDirectoryExists=true,State=new CredentialProbeState{runId="invalid"}};Assert.AreEqual("refused",Run(command,h).status);NoVaultAccess(h);
        }
        [Test] public void MarkerFailurePrecedesAnyCredentialAccess(){var h=new Host{FailMarker=true};Assert.AreEqual("failed_retained",Run("prepare",h).status);NoVaultAccess(h);}
        [Test] public void FailedWriteRetainsOwnershipAndDoesNotClaimPass()
        {
            var h=new Host{FailWrite=true};Assert.AreEqual("failed_retained",Run("prepare",h).status);Assert.IsTrue(h.OwnershipDirectoryExists);Assert.AreEqual(0,h.Cleanups);
            Assert.AreEqual("refused",Run("cleanup",h).status);
        }
        [Test] public void SameProcessCannotPretendToHaveRestarted(){var h=Prepared();int reads=h.Reads;Assert.AreEqual("refused",Run("restart",h).status);Assert.AreEqual(reads,h.Reads);Assert.IsFalse(h.Partial);}
        [Test] public void SameProcessCannotPretendInterruptedRecovery(){var h=Interrupted();h.ProcessId--;Assert.AreEqual("refused",Run("verify",h).status);Assert.AreEqual(0,h.Faults);}
        [TestCase("restart")] [TestCase("cleanup")]
        public void ChangedCiphertextBlocksReadAndCleanup(string command)
        {
            var h=Prepared();h.ProcessId++;h.Record[0]^=1;int reads=h.Reads;Assert.AreEqual("refused",Run(command,h).status);Assert.AreEqual(reads,h.Reads);Assert.AreEqual(0,h.Cleanups);
        }
        [Test] public void WrongPhaseIsNotARecoveryPass(){var h=Prepared();h.ProcessId++;Assert.AreEqual("refused",Run("verify",h).status);Assert.AreEqual(0,h.Faults);}
        [Test] public void MissingPartialWriteDoesNotCountAsInterruption(){var h=Interrupted();h.Partial=false;int reads=h.Reads;Assert.AreEqual("refused",Run("verify",h).status);Assert.AreEqual(reads,h.Reads);}
        [Test] public void RecoveryFailureStopsBeforeFaultInjection(){var h=Interrupted();h.FailRead=true;Assert.AreEqual("failed_retained",Run("verify",h).status);Assert.AreEqual(0,h.Faults);Assert.AreEqual(0,h.KeyDeletes);}
        [Test] public void IncorrectCorruptionAcceptanceIsFailure(){var h=Interrupted();h.AcceptCorrupt=true;Assert.AreEqual("failed_retained",Run("verify",h).status);Assert.AreEqual(0,h.KeyDeletes);Assert.AreEqual(0,h.Cleanups);}
        [Test] public void IncorrectMissingKeyAcceptanceIsFailure(){var h=Interrupted();h.AcceptMissingKey=true;Assert.AreEqual("failed_retained",Run("verify",h).status);Assert.AreEqual(0,h.Cleanups);}
        [Test] public void KnownFixtureCanBeCleanedWithoutResettingApp(){var h=Prepared();Assert.AreEqual("cleaned",Run("cleanup",h).status);Assert.IsTrue(h.StoreEmpty);}
        [Test] public void PendingWriteCannotBeCleanedWhileUnfinished(){var h=Interrupted();Assert.AreEqual("refused",Run("cleanup",h).status);Assert.AreEqual(0,h.Cleanups);}
    }
}
#endif
