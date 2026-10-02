using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;
namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class ContinuationRestartPlayModeTests
    {
        string FileName => Environment.GetEnvironmentVariable("LOR_PERSISTENCE_CHECKPOINT");
        void Guard(string phase)
        {
            if(Environment.GetEnvironmentVariable("LOR_PERSISTENCE_PHASE")!=phase)Assert.Ignore("Requires isolated continuation restart phase.");
            string root=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");Assert.That(root,Does.StartWith("/tmp/lor-continuation-r1/"));
            Assert.That(Application.persistentDataPath,Does.StartWith(root+"/"));Assert.That(FileName,Does.StartWith("/tmp/lor-continuation-r1/"));
        }
        static ContinuationReservationRequest From(ContinuationTransactionData r)=>new ContinuationReservationRequest(r.transactionId,r.evidenceKey,r.attemptId,r.snapshotDigest,r.policyId,r.source,r.gemCost,r.grantedMoves,r.maxPerAttempt);
        [Test] public void WriteReservation()
        {
            Guard("continuation-write");var state=GameState.Instance;state.ResetProgress();var round=ContinuationTransactionPlayModeTests.CreateFailure(state);
            Assert.That(state.TrySuspendRound(round),Is.True);var request=ContinuationTransactionPlayModeTests.Candidate(round);
            Assert.That(state.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));
            File.WriteAllText(FileName,JsonUtility.ToJson(state.Save));
        }
        [Test] public void ReadReservationAndRefund()
        {
            Guard("continuation-read");var state=GameState.Instance;var expected=JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(FileName));
            Assert.That(state.TryGetPendingContinuation(out var receipt),Is.True);Assert.That(JsonUtility.ToJson(receipt),Is.EqualTo(JsonUtility.ToJson(expected.continuationTransactions[0])));
            Assert.That(state.Save.gems,Is.EqualTo(expected.gems));Assert.That(state.TryReserveContinuation(From(receipt)),Is.EqualTo(ContinuationTransactionResult.AlreadyReserved));
            Assert.That(state.TryFinalizeRetainedFailure(receipt.attemptId),Is.True);Assert.That(state.Save.gems,Is.EqualTo(expected.gems+receipt.gemCost));
        }
        [Test] public void ReadRefundAndRejectReplay()
        {
            Guard("continuation-refunded");var state=GameState.Instance;var expected=JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(FileName));var receipt=expected.continuationTransactions[0];
            Assert.That(state.Save.retainedRound,Is.Null);Assert.That(state.GetLevelProgress(1,1).failures,Is.EqualTo(1));
            Assert.That(state.Save.gems,Is.EqualTo(expected.gems+receipt.gemCost));Assert.That(state.TryReserveContinuation(From(receipt)),Is.EqualTo(ContinuationTransactionResult.AlreadyCancelled));
            Assert.That(state.Save.continuationTransactions[0].state,Is.EqualTo(ContinuationReservationState.Cancelled));
        }
    }
}
