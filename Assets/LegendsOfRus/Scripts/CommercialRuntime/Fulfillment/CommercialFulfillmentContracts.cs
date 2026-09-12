using LegendsOfRus.EconomyCore;

namespace LegendsOfRus.CommercialRuntime.Fulfillment
{
    public enum CommercialFulfillmentPreparationState
    {
        Rejected = 0,
        CandidatePrepared = 1
    }

    public sealed class CommercialFulfillmentCandidate
    {
        readonly PlayerSaveData detachedCandidate;
        readonly RuntimeEconomySnapshot snapshot;

        public string ActionId { get; private set; }
        public string DurableMarkerProposal { get; private set; }
        public string TransactionKey { get; private set; }
        public string SourceFingerprint { get; private set; }
        public bool Changed { get; private set; }
        public bool IsPersisted { get { return false; } }
        public bool IsEntitlementCompleted { get { return false; } }

        public RuntimeEconomySnapshot Snapshot
        {
            get { return snapshot.DetachedCopy(); }
        }

        internal CommercialFulfillmentCandidate(
            string actionId,
            string durableMarkerProposal,
            string transactionKey,
            string sourceFingerprint,
            bool changed,
            PlayerSaveData candidate,
            RuntimeEconomySnapshot snapshot
        )
        {
            ActionId = actionId ?? string.Empty;
            DurableMarkerProposal = durableMarkerProposal ?? string.Empty;
            TransactionKey = transactionKey ?? string.Empty;
            SourceFingerprint = sourceFingerprint ?? string.Empty;
            Changed = changed;
            detachedCandidate = CommercialFulfillmentIdentity.CloneSave(candidate);
            this.snapshot = snapshot != null
                ? snapshot.DetachedCopy()
                : new RuntimeEconomySnapshot();
        }

        /// <summary>
        /// Returns another detached copy for verification only. This is not a
        /// persistence or live-state commit API.
        /// </summary>
        public PlayerSaveData CreateDetachedSaveCopy()
        {
            return CommercialFulfillmentIdentity.CloneSave(detachedCandidate);
        }
    }

    public sealed class CommercialFulfillmentPreparationResult
    {
        public CommercialFulfillmentPreparationState State { get; private set; }
        public string ReasonCode { get; private set; }
        public string DetailCode { get; private set; }
        public CommercialFulfillmentCandidate Candidate { get; private set; }

        public bool IsPrepared
        {
            get
            {
                return
                    State == CommercialFulfillmentPreparationState.CandidatePrepared &&
                    Candidate != null;
            }
        }

        internal CommercialFulfillmentPreparationResult(
            CommercialFulfillmentPreparationState state,
            string reasonCode,
            string detailCode,
            CommercialFulfillmentCandidate candidate
        )
        {
            State = state;
            ReasonCode = reasonCode ?? CommercialFulfillmentReasonCodes.InternalError;
            DetailCode = detailCode ?? string.Empty;
            Candidate = candidate;
        }

        internal static CommercialFulfillmentPreparationResult Rejected(
            string reasonCode,
            string detailCode = null
        )
        {
            return new CommercialFulfillmentPreparationResult(
                CommercialFulfillmentPreparationState.Rejected,
                reasonCode,
                detailCode,
                null
            );
        }

        internal static CommercialFulfillmentPreparationResult Prepared(
            CommercialFulfillmentCandidate candidate
        )
        {
            return new CommercialFulfillmentPreparationResult(
                CommercialFulfillmentPreparationState.CandidatePrepared,
                CommercialFulfillmentReasonCodes.None,
                string.Empty,
                candidate
            );
        }
    }

    /// <summary>
    /// Future atomic-store request shape. The current milestone deliberately
    /// supplies no durable local, file, or network implementation.
    /// </summary>
    internal sealed class CommercialAtomicCommitRequest
    {
        readonly PlayerSaveData detachedCandidate;

        public string DurableMarker { get; private set; }
        public string ExpectedSourceFingerprint { get; private set; }

        internal CommercialAtomicCommitRequest(
            string durableMarker,
            string expectedSourceFingerprint,
            PlayerSaveData candidate
        )
        {
            DurableMarker = durableMarker ?? string.Empty;
            ExpectedSourceFingerprint = expectedSourceFingerprint ?? string.Empty;
            detachedCandidate = CommercialFulfillmentIdentity.CloneSave(candidate);
        }

        public PlayerSaveData CreateDetachedSaveCopy()
        {
            return CommercialFulfillmentIdentity.CloneSave(detachedCandidate);
        }
    }

    internal sealed class CommercialAtomicCommitResult
    {
        public bool Committed { get; private set; }
        public string ReasonCode { get; private set; }

        CommercialAtomicCommitResult(string reasonCode)
        {
            Committed = false;
            ReasonCode = reasonCode ?? CommercialFulfillmentReasonCodes.InternalError;
        }

        internal static CommercialAtomicCommitResult Rejected(string reasonCode)
        {
            return new CommercialAtomicCommitResult(reasonCode);
        }
    }

    internal interface ICommercialAtomicFulfillmentStore
    {
        CommercialAtomicCommitResult TryCommit(
            CommercialAtomicCommitRequest request
        );
    }

    /// <summary>
    /// Shipped persistence boundary. It always fails closed because save v6 has
    /// no durable commercial marker that can be committed atomically with a
    /// balance change.
    /// </summary>
    internal sealed class UnavailableCommercialAtomicFulfillmentStore :
        ICommercialAtomicFulfillmentStore
    {
        public CommercialAtomicCommitResult TryCommit(
            CommercialAtomicCommitRequest request
        )
        {
            return CommercialAtomicCommitResult.Rejected(
                CommercialFulfillmentReasonCodes.AtomicStoreUnavailable
            );
        }
    }
}
