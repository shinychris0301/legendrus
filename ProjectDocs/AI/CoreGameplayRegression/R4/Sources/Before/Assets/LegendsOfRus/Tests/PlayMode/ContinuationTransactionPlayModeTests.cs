using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class ContinuationTransactionPlayModeTests
    {
        GameState State => GameState.Instance;
        ScreenRouter Router => ScreenRouter.Instance;
        string previous;
        string[] values;
        readonly string[] keys={"LegendsOfRus_Save_v1","LegendsOfRus_Save_v1_Backup","LegendsOfRus_Save_v1_Staging"};
        RetainedRoundData round;
        [SetUp] public void SetUp()
        {
            string config=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config,Does.StartWith("/tmp/lor-continuation-r1/"));Assert.That(Application.persistentDataPath,Does.StartWith(config+"/"));
            previous=JsonUtility.ToJson(State.Save);values=keys.Select(k=>PlayerPrefs.HasKey(k)?PlayerPrefs.GetString(k):null).ToArray();
            Router.Show(ScreenId.SplashTitle);Settle();State.ResetProgress();
            round=CreateFailure(State);Assert.That(State.TrySuspendRound(round),Is.True);
        }
        [TearDown] public void TearDown()
        {
            State.RetainedWriteOverride=null;State.ResetProgress();Router.Show(ScreenId.SplashTitle);Settle();
            State.Save=JsonUtility.FromJson<PlayerSaveData>(previous);
            for(int i=0;i<keys.Length;i++){PlayerPrefs.DeleteKey(keys[i]);if(values[i]!=null)PlayerPrefs.SetString(keys[i],values[i]);}PlayerPrefs.Save();
        }
        void Settle(){for(int i=0;i<24;i++)Router.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        internal static RetainedRoundData CreateFailure(GameState state)
        {
            var level=ChapterLevelCatalog.Find(1,1);int attempt=state.RegisterLevelAttempt(level);
            int seed=LegendsOfRus.GameplayCore.BoardSeedUtility.CreateRuntimeSeed(1,1,level.boardSeed+attempt);
            var b=LevelBoardFactory.Create(level,seed,rules:Match3Board.CurrentGameplayRules).CreateSnapshot();
            return new RetainedRoundData {attemptId=Guid.NewGuid().ToString("N"),attempt=attempt,chapter=1,level=1,
                levelFingerprint=RetainedRoundData.Fingerprint(level),rules=(int)Match3Board.CurrentGameplayRules,seed=seed,score=73,stateHash=b.StateHash,
                tiles=b.Tiles,specials=b.Specials.Select(x=>(int)x).ToArray(),obstacles=b.Obstacles.Select(x=>(int)x).ToArray(),
                layers=b.ObstacleLayers.Select(x=>(int)x).ToArray(),items=b.ObjectiveItems.Select(x=>(int)x).ToArray(),goals=level.goals.Select(_=>0).ToArray()};
        }
        internal static ContinuationReservationRequest Candidate(RetainedRoundData failed,ContinuationSource source=ContinuationSource.Gems,
            string id=null,string evidence=null,string digest=null,int? cost=null,int? moves=null,int limit=1,string policy="test-only.recipe") =>
            new ContinuationReservationRequest(id??Guid.NewGuid().ToString("N"),evidence??("fixture:"+Guid.NewGuid().ToString("N")),failed.attemptId,
                digest??ContinuationJournal.Digest(failed),policy,source,cost??(source==ContinuationSource.Gems?17:0),moves??(source==ContinuationSource.Gems?5:3),limit);
        static PlayerSaveWriteResult FailWrite(PlayerSaveData _) => new PlayerSaveWriteResult(PlayerSaveWriteStatus.StorageFailure,"fixture",0,"injected");
        [TestCase(ContinuationSource.Gems)][TestCase(ContinuationSource.RewardedAd)]
        public void DebitAndGrantCommitTogetherAndSurviveReload(ContinuationSource source)
        {
            var request=Candidate(round,source);int gems=State.Save.gems,energy=State.Save.energy,gold=State.Save.gold;
            string checkpoint=JsonUtility.ToJson(round);
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));
            State.Save=SaveService.Load();Assert.That(State.TryGetPendingContinuation(out var grant),Is.True);
            Assert.That(grant.grantedMoves,Is.EqualTo(request.GrantedMoves));Assert.That(State.Save.gems,Is.EqualTo(gems-request.GemCost));
            Assert.That(State.Save.gold,Is.EqualTo(gold));Assert.That(State.Save.energy,Is.EqualTo(energy));
            Assert.That(JsonUtility.ToJson(State.Save.retainedRound),Is.EqualTo(checkpoint));Assert.That(State.Save.retainedRound.moves,Is.Zero);
            Assert.That(State.GetLevelProgress(1,1).attempts,Is.EqualTo(1));Assert.That(State.GetLevelProgress(1,1).failures,Is.Zero);
            grant.gemCost=999;Assert.That(State.Save.continuationTransactions[0].gemCost,Is.EqualTo(request.GemCost));
        }
        [Test] public void DuplicateCallbackDoesNotWriteOrSpendTwice()
        {
            var request=Candidate(round);Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));
            string before=JsonUtility.ToJson(State.Save);State.RetainedWriteOverride=_=>throw new Exception("Duplicate must not write");
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.AlreadyReserved));
            Assert.That(JsonUtility.ToJson(State.Save),Is.EqualTo(before));
        }
        [Test] public void SameIdWithChangedTermsIsRejected()
        {
            var request=Candidate(round);State.TryReserveContinuation(request);
            var changed=Candidate(round,id:request.TransactionId,evidence:request.EvidenceKey,cost:18);
            Assert.That(State.TryReserveContinuation(changed),Is.EqualTo(ContinuationTransactionResult.IdentityConflict));
        }
        [Test] public void SameEvidenceWithAnotherIdCannotGrantAgain()
        {
            var request=Candidate(round);State.TryReserveContinuation(request);
            Assert.That(State.TryReserveContinuation(Candidate(round,evidence:request.EvidenceKey)),Is.EqualTo(ContinuationTransactionResult.EvidenceReused));
        }
        [Test] public void SecondPendingGrantIsRejectedEvenWithDifferentIdentity()
        {
            State.TryReserveContinuation(Candidate(round,limit:3));
            Assert.That(State.TryReserveContinuation(Candidate(round,limit:3)),Is.EqualTo(ContinuationTransactionResult.PendingGrant));
        }
        [Test] public void WrongAttemptAndChangedSnapshotCannotCharge()
        {
            int gems=State.Save.gems;var old=round.Copy();old.attemptId=Guid.NewGuid().ToString("N");
            Assert.That(State.TryReserveContinuation(Candidate(old)),Is.EqualTo(ContinuationTransactionResult.StaleAttempt));
            old=round.Copy();old.score++;
            Assert.That(State.TryReserveContinuation(Candidate(old)),Is.EqualTo(ContinuationTransactionResult.SnapshotChanged));Assert.That(State.Save.gems,Is.EqualTo(gems));
        }
        [TestCase(ContinuationSource.Gems,0,5,1)][TestCase(ContinuationSource.Gems,-1,5,1)]
        [TestCase(ContinuationSource.Gems,17,6,1)][TestCase(ContinuationSource.Gems,17,5,0)]
        [TestCase(ContinuationSource.RewardedAd,1,3,1)][TestCase(ContinuationSource.RewardedAd,0,0,1)]
        [TestCase((ContinuationSource)99,17,5,1)]
        public void InvalidRecipeCannotMutateSave(ContinuationSource source,int cost,int moves,int limit)
        {
            string before=JsonUtility.ToJson(State.Save);
            Assert.That(State.TryReserveContinuation(Candidate(round,source,cost:cost,moves:moves,limit:limit)),Is.EqualTo(ContinuationTransactionResult.InvalidRequest));
            Assert.That(JsonUtility.ToJson(State.Save),Is.EqualTo(before));
        }
        [Test] public void MissingPolicyOrMalformedIdentityIsRejected()
        {
            Assert.That(State.TryReserveContinuation(Candidate(round,policy:"")),Is.EqualTo(ContinuationTransactionResult.InvalidRequest));
            Assert.That(State.TryReserveContinuation(Candidate(round,id:"invalid")),Is.EqualTo(ContinuationTransactionResult.InvalidRequest));
            Assert.That(State.TryReserveContinuation(null),Is.EqualTo(ContinuationTransactionResult.InvalidRequest));
        }
        [Test] public void InsufficientAndExactBalance()
        {
            State.Save.gems=16;var request=Candidate(round);
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.InsufficientGems));Assert.That(State.Save.continuationTransactions,Is.Empty);
            State.Save.gems=17;Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));Assert.That(State.Save.gems,Is.Zero);
        }
        [Test] public void FailedWriteLeavesBothBalanceAndGrantUnchangedThenCanRetry()
        {
            var request=Candidate(round);string before=JsonUtility.ToJson(State.Save);State.RetainedWriteOverride=FailWrite;
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.StorageFailure));Assert.That(JsonUtility.ToJson(State.Save),Is.EqualTo(before));
            Assert.That(SaveService.Load().continuationTransactions,Is.Empty);
            State.RetainedWriteOverride=null;Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));
        }
        [Test] public void PrimaryCommittedCleanupFailureIsAdoptedWithoutSecondDebit()
        {
            var request=Candidate(round);int gems=State.Save.gems;
            State.RetainedWriteOverride=candidate=>{
                Assert.That(SaveService.TrySave(candidate,out var saved),Is.True);
                return new PlayerSaveWriteResult(PlayerSaveWriteStatus.StorageFailure,"cleanup",saved.Revision,"injected-after-primary",true);
            };
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));Assert.That(State.CanPersist,Is.False);
            State.Save=SaveService.Load();Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.AlreadyReserved));Assert.That(State.Save.gems,Is.EqualTo(gems-17));
        }
        [TestCase(ContinuationSource.Gems)][TestCase(ContinuationSource.RewardedAd)]
        public void DeclineRefundsUnusedReservationOnceAndRetainsReplayMarker(ContinuationSource source)
        {
            var request=Candidate(round,source);int gems=State.Save.gems;State.TryReserveContinuation(request);
            Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.True);Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.False);
            State.Save=SaveService.Load();Assert.That(State.Save.gems,Is.EqualTo(gems));Assert.That(State.Save.retainedRound,Is.Null);
            Assert.That(State.GetLevelProgress(1,1).failures,Is.EqualTo(1));Assert.That(State.Save.continuationTransactions.Single().state,Is.EqualTo(ContinuationReservationState.Cancelled));
            Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.AlreadyCancelled));Assert.That(State.TryGetPendingContinuation(out _),Is.False);
        }
        [Test] public void FailedRefundWriteCannotLoseReservationOrDuplicateRefund()
        {
            var request=Candidate(round);State.TryReserveContinuation(request);int gems=State.Save.gems;
            State.RetainedWriteOverride=FailWrite;Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.False);
            Assert.That(State.Save.gems,Is.EqualTo(gems));Assert.That(State.GetLevelProgress(1,1).failures,Is.Zero);Assert.That(State.TryGetPendingContinuation(out _),Is.True);
            State.RetainedWriteOverride=null;Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.True);Assert.That(State.Save.gems,Is.EqualTo(gems+17));
        }
        [Test] public void RefundOverflowDoesNotClearPendingEntitlement()
        {
            State.TryReserveContinuation(Candidate(round));State.Save.gems=int.MaxValue;
            Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.False);Assert.That(State.TryGetPendingContinuation(out _),Is.True);Assert.That(State.GetLevelProgress(1,1).failures,Is.Zero);
        }
        [Test] public void ReservedGrantBlocksOutOfBandAttemptAndResultMutation()
        {
            State.TryReserveContinuation(Candidate(round));var level=ChapterLevelCatalog.Find(1,1);
            Assert.Throws<InvalidOperationException>(()=>State.RegisterLevelAttempt(level));Assert.Throws<InvalidOperationException>(()=>State.CompleteLevel(level,999));
            Assert.Throws<InvalidOperationException>(()=>State.RecordLevelFailure(level,1));Assert.That(State.SelectLevel(1,1),Is.False);
        }
        [TestCase("schema")][TestCase("duplicate")][TestCase("null")]
        public void CorruptHistoryBlocksTransactionsAndDoesNotRefundUntrustedRecords(string damage)
        {
            State.TryReserveContinuation(Candidate(round));int gems=State.Save.gems;
            if(damage=="schema")State.Save.continuationTransactions[0].schema=999;
            if(damage=="duplicate")State.Save.continuationTransactions.Add(State.Save.continuationTransactions[0].Copy());
            if(damage=="null")State.Save.continuationTransactions.Add(null);
            Assert.That(State.TryReserveContinuation(Candidate(round)),Is.EqualTo(ContinuationTransactionResult.InvalidHistory));
            Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.False);Assert.That(State.Save.gems,Is.EqualTo(gems));
            Assert.Throws<InvalidOperationException>(()=>State.RegisterLevelAttempt(ChapterLevelCatalog.Find(1,1)));
        }
        [Test] public void HistoryCapacityNeverEvictsReplayProtection()
        {
            for(int i=0;i<ContinuationJournal.Capacity;i++){
                var entry=Candidate(round).CreateRecord();entry.attemptId=Guid.NewGuid().ToString("N");entry.state=ContinuationReservationState.Cancelled;State.Save.continuationTransactions.Add(entry);
            }
            Assert.That(State.TryReserveContinuation(Candidate(round)),Is.EqualTo(ContinuationTransactionResult.HistoryFull));Assert.That(State.Save.continuationTransactions.Count,Is.EqualTo(ContinuationJournal.Capacity));
        }
        [Test] public void ExplicitLimitAndOldEvidenceRemainEffectiveAfterRestart()
        {
            var request=Candidate(round);State.TryReserveContinuation(request);State.TryFinalizeRetainedFailure(round.attemptId);
            State.Save=SaveService.Load();var next=CreateFailure(State);State.TrySuspendRound(next);
            Assert.That(State.TryReserveContinuation(Candidate(next,evidence:request.EvidenceKey)),Is.EqualTo(ContinuationTransactionResult.EvidenceReused));
            var historical=Candidate(next).CreateRecord();historical.state=ContinuationReservationState.Cancelled;State.Save.continuationTransactions.Add(historical);
            Assert.That(State.TryReserveContinuation(Candidate(next,limit:1)),Is.EqualTo(ContinuationTransactionResult.LimitReached));
        }
        [Test] public void CommitObserverCannotReenterAndFinalizePendingGrant()
        {
            var request=Candidate(round);bool notified=false;
            Action changed=()=>{notified=true;Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Busy));Assert.That(State.TryFinalizeRetainedFailure(round.attemptId),Is.False);};
            State.Changed+=changed;try{Assert.That(State.TryReserveContinuation(request),Is.EqualTo(ContinuationTransactionResult.Reserved));Assert.That(notified,Is.True);}finally{State.Changed-=changed;}
            Assert.That(State.TryGetPendingContinuation(out _),Is.True);
        }
        [UnityTest] public IEnumerator ExistingFailureOffersRemainUnavailableAndRetryRefundsBeforeNewAttempt()
        {
            var request=Candidate(round);int gems=State.Save.gems,energy=State.Save.energy;State.TryReserveContinuation(request);
            Router.Show(ScreenId.Home);Settle();yield return null;
            var view=Router.Viewport.GetComponentInChildren<ProductionFailView>();Assert.That(view,Is.Not.Null);
            Assert.That(view.PaidOffer.interactable||view.AdOffer.interactable,Is.False);
            view.PaidOffer.onClick.Invoke();view.AdOffer.onClick.Invoke();Assert.That(State.Save.continuationTransactions.Count,Is.EqualTo(1));
            view.Retry.onClick.Invoke();view.Retry.onClick.Invoke();Settle();yield return null;
            Assert.That(State.Save.gems,Is.EqualTo(gems));Assert.That(State.Save.energy,Is.EqualTo(energy));Assert.That(State.GetLevelProgress(1,1).attempts,Is.EqualTo(2));
            Assert.That(State.GetLevelProgress(1,1).failures,Is.EqualTo(1));Assert.That(State.TryGetPendingContinuation(out _),Is.False);
        }
    }
}
