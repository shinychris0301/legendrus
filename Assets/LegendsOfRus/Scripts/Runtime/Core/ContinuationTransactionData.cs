using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace LegendsOfRus
{
    public enum ContinuationSource { Gems = 1, RewardedAd = 2 }
    public enum ContinuationReservationState { Reserved = 1, Cancelled = 2 }

    // Local recovery receipt, not proof of payment or a server entitlement.
    [Serializable]
    public sealed class ContinuationTransactionData
    {
        public int schema = 1;
        public string transactionId, evidenceKey, attemptId, snapshotDigest, policyId;
        public ContinuationSource source;
        public ContinuationReservationState state;
        public int gemCost, grantedMoves, maxPerAttempt;

        public ContinuationTransactionData Copy() => JsonUtility.FromJson<ContinuationTransactionData>(JsonUtility.ToJson(this));

        internal bool Matches(ContinuationReservationRequest request) =>
            transactionId == request.TransactionId && evidenceKey == request.EvidenceKey &&
            attemptId == request.AttemptId && snapshotDigest == request.SnapshotDigest &&
            policyId == request.PolicyId && source == request.Source && gemCost == request.GemCost &&
            grantedMoves == request.GrantedMoves && maxPerAttempt == request.MaxPerAttempt;

        internal bool IsValid() => schema == 1 && Guid.TryParseExact(transactionId, "N", out _) &&
            Guid.TryParseExact(attemptId, "N", out _) && ContinuationJournal.ValidToken(evidenceKey) &&
            ContinuationJournal.ValidToken(policyId) && snapshotDigest != null && snapshotDigest.Length == 64 &&
            snapshotDigest.All(c => c >= '0' && c <= '9' || c >= 'A' && c <= 'F') &&
            (source == ContinuationSource.Gems || source == ContinuationSource.RewardedAd) &&
            (state == ContinuationReservationState.Reserved || state == ContinuationReservationState.Cancelled) &&
            maxPerAttempt > 0 && maxPerAttempt <= ContinuationJournal.Capacity &&
            (source == ContinuationSource.Gems ? gemCost > 0 && grantedMoves == 5 : gemCost == 0 && grantedMoves > 0 && grantedMoves <= 1000);
    }

    // Only a future reviewed commercial bridge may construct this after validating its
    // authority evidence and approved recipe. No production caller or provider is bound yet.
    // Test candidates are not production authorization and never enable an offer.
    internal sealed class ContinuationReservationRequest
    {
        internal readonly string TransactionId, EvidenceKey, AttemptId, SnapshotDigest, PolicyId;
        internal readonly ContinuationSource Source;
        internal readonly int GemCost, GrantedMoves, MaxPerAttempt;

        internal ContinuationReservationRequest(string transactionId, string evidenceKey, string attemptId,
            string snapshotDigest, string policyId, ContinuationSource source, int gemCost, int grantedMoves, int maxPerAttempt)
        {
            TransactionId = transactionId; EvidenceKey = evidenceKey; AttemptId = attemptId;
            SnapshotDigest = snapshotDigest; PolicyId = policyId; Source = source;
            GemCost = gemCost; GrantedMoves = grantedMoves; MaxPerAttempt = maxPerAttempt;
        }

        internal ContinuationTransactionData CreateRecord() => new ContinuationTransactionData {
            transactionId = TransactionId, evidenceKey = EvidenceKey, attemptId = AttemptId,
            snapshotDigest = SnapshotDigest, policyId = PolicyId, source = Source,
            gemCost = GemCost, grantedMoves = GrantedMoves, maxPerAttempt = MaxPerAttempt,
            state = ContinuationReservationState.Reserved
        };
    }

    public enum ContinuationTransactionResult
    {
        Reserved, AlreadyReserved, AlreadyCancelled, InvalidRequest, InvalidHistory,
        IdentityConflict, EvidenceReused, StaleAttempt, SnapshotChanged, PendingGrant,
        LimitReached, HistoryFull, InsufficientGems, StorageFailure, Busy
    }

    internal static class ContinuationJournal
    {
        // Local operational bound, not a gameplay limit. Never prune replay tombstones.
        internal const int Capacity = 256;
        internal static bool ValidToken(string value) => !string.IsNullOrEmpty(value) && value.Length <= 128 &&
            value.All(c => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '-' || c == '_' || c == '.' || c == ':');

        internal static bool Validate(List<ContinuationTransactionData> records)
        {
            if (records == null) return true; // Older save without this optional collection.
            if (records.Count > Capacity) return false;
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var evidence = new HashSet<string>(StringComparer.Ordinal);
            int reserved = 0;
            foreach (var entry in records)
            {
                if (entry == null || !entry.IsValid() || !ids.Add(entry.transactionId) || !evidence.Add(entry.evidenceKey)) return false;
                if (entry.state == ContinuationReservationState.Reserved) reserved++;
            }
            return reserved <= 1;
        }

        internal static string Digest(RetainedRoundData round)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonUtility.ToJson(round)))).Replace("-", "");
        }
    }
}
