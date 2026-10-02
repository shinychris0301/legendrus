using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed partial class GameState
    {
        bool continuationCommitInProgress;

        // Intentionally internal and unbound: a client-supplied price or successful ad
        // callback is not authority. This local atomic store is a CONT-02 building block.
        internal ContinuationTransactionResult TryReserveContinuation(ContinuationReservationRequest request)
        {
            if (continuationCommitInProgress) return ContinuationTransactionResult.Busy;
            if (request == null || !request.CreateRecord().IsValid()) return ContinuationTransactionResult.InvalidRequest;
            if (!ContinuationJournal.Validate(Save.continuationTransactions)) return ContinuationTransactionResult.InvalidHistory;
            var records = Save.continuationTransactions ?? new List<ContinuationTransactionData>();
            var previous = records.FirstOrDefault(r => r.transactionId == request.TransactionId);
            if (previous != null)
            {
                if (!previous.Matches(request)) return ContinuationTransactionResult.IdentityConflict;
                return previous.state == ContinuationReservationState.Reserved
                    ? ContinuationTransactionResult.AlreadyReserved : ContinuationTransactionResult.AlreadyCancelled;
            }
            if (records.Any(r => r.evidenceKey == request.EvidenceKey)) return ContinuationTransactionResult.EvidenceReused;
            if (!TryGetRetainedRound(out var round) || round.attemptId != request.AttemptId) return ContinuationTransactionResult.StaleAttempt;
            if (ContinuationJournal.Digest(round) != request.SnapshotDigest) return ContinuationTransactionResult.SnapshotChanged;
            if (records.Any(r => r.state == ContinuationReservationState.Reserved)) return ContinuationTransactionResult.PendingGrant;
            if (records.Count(r => r.attemptId == request.AttemptId) >= request.MaxPerAttempt) return ContinuationTransactionResult.LimitReached;
            if (records.Count >= ContinuationJournal.Capacity) return ContinuationTransactionResult.HistoryFull;
            if (Save.gems < request.GemCost) return ContinuationTransactionResult.InsufficientGems;

            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            // Also commits a preceding in-memory-only failure snapshot if its first write failed.
            candidate.retainedRound = round.Copy();
            candidate.continuationTransactions = candidate.continuationTransactions ?? new List<ContinuationTransactionData>();
            candidate.continuationTransactions.Add(request.CreateRecord());
            candidate.gems = checked(candidate.gems - request.GemCost);
            continuationCommitInProgress = true;
            try { return CommitRetained(candidate) ? ContinuationTransactionResult.Reserved : ContinuationTransactionResult.StorageFailure; }
            finally { continuationCommitInProgress = false; }
        }

        public bool TryGetPendingContinuation(out ContinuationTransactionData grant)
        {
            grant = null;
            if (!ContinuationJournal.Validate(Save.continuationTransactions) || !TryGetRetainedRound(out var round)) return false;
            var pending = Save.continuationTransactions?.FirstOrDefault(r => r.state == ContinuationReservationState.Reserved);
            if (pending == null || pending.attemptId != round.attemptId || pending.snapshotDigest != ContinuationJournal.Digest(round)) return false;
            grant = pending.Copy();
            return true;
        }

        bool ContinuationBlocksNewAttempt() => continuationCommitInProgress ||
            !ContinuationJournal.Validate(Save.continuationTransactions) ||
            (Save.continuationTransactions?.Any(r => r.state == ContinuationReservationState.Reserved) ?? false);

        // Called on a detached failure-finalization candidate. Refund, cancellation marker,
        // loss and checkpoint removal are written together, never as separate saves.
        bool CancelUnusedContinuation(PlayerSaveData candidate, RetainedRoundData round)
        {
            if (continuationCommitInProgress || !ContinuationJournal.Validate(candidate.continuationTransactions)) return false;
            var pending = candidate.continuationTransactions?.FirstOrDefault(r => r.state == ContinuationReservationState.Reserved);
            if (pending == null) return true;
            if (pending.attemptId != round.attemptId || pending.snapshotDigest != ContinuationJournal.Digest(round)) return false;
            if (pending.gemCost > int.MaxValue - candidate.gems) return false;
            candidate.gems += pending.gemCost;
            pending.state = ContinuationReservationState.Cancelled;
            return true;
        }
    }
}
