using NUnit.Framework;
using LegendsOfRus.LiveOpsBaseline;

namespace LegendsOfRus.Tests.LiveOpsBaseline
{
    public sealed class LiveOpsBaselineCoreTests
    {
        [Test] public void ChainHealthy(){Assert.That(V119EvidenceChainEvaluator.Evaluate(C()).ChainReady,Is.True);}
        [Test] public void ChainMissing(){Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(null),LiveOpsBaselineBlockerCodes.ChainMissing),Is.True);}
        [Test] public void ChainNotRetained(){var x=C();x.EvidenceRetained=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainMissing),Is.True);}
        [Test] public void ChainNotReviewed(){var x=C();x.EvidenceReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainNotReviewed),Is.True);}
        [Test] public void ChainBadId(){var x=C();x.ChainId="bad";Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainInvalid),Is.True);}
        [Test] public void ChainBadSourceHash(){var x=C();x.SourceTreeSha256="bad";Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainSourceHashInvalid),Is.True);}
        [Test] public void ChainBadArchiveHash(){var x=C();x.ArchiveSha256="bad";Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainArchiveHashInvalid),Is.True);}
        [Test] public void ChainStep1Missing(){var x=C();x.Step01Reviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainStepMissing),Is.True);}
        [Test] public void ChainStep7Missing(){var x=C();x.Step07Reviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainStepMissing),Is.True);}
        [Test] public void ChainUnityCompileMissing(){var x=C();x.UnityCompileEvidenceReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainUnityEvidenceMissing),Is.True);}
        [Test] public void ChainTestRunnerMissing(){var x=C();x.TestRunnerEvidenceReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainUnityEvidenceMissing),Is.True);}
        [Test] public void ChainPrivacyMissing(){var x=C();x.PrivacyBoundaryReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainPrivacyBoundaryNotReviewed),Is.True);}
        [Test] public void ChainMutationMissing(){var x=C();x.ProductionMutationBoundaryReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainMutationBoundaryNotReviewed),Is.True);}
        [Test] public void ChainCriticalOpen(){var x=C();x.UnresolvedCriticalBlockers=1;Assert.That(Has(LiveOpsBaselineValidation.ValidateChain(x),LiveOpsBaselineBlockerCodes.ChainCriticalBlockersOpen),Is.True);}
        [Test] public void ChainStepCountSeven(){Assert.That(LiveOpsBaselineValidation.ReviewedStepCount(C()),Is.EqualTo(7));}

        [Test] public void BaselineHealthy(){Assert.That(OperatingBaselineEvaluator.Evaluate(B(),C().ChainId).BaselineReady,Is.True);}
        [Test] public void BaselineMissing(){Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(null,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineMissing),Is.True);}
        [Test] public void BaselineNotReviewed(){var x=B();x.EvidenceReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineNotReviewed),Is.True);}
        [Test] public void BaselineBadId(){var x=B();x.BaselineId="bad";Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineInvalid),Is.True);}
        [Test] public void BaselineChainMismatch(){var x=B();x.ChainId="other";Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineChainMismatch),Is.True);}
        [Test] public void BaselineRunbooksLow(){var x=B();x.RunbookSectionCount=4;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineRunbooksIncomplete),Is.True);}
        [Test] public void BaselineObservabilityMissing(){var x=B();x.ObservabilityRunbookReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineObservabilityMissing),Is.True);}
        [Test] public void BaselineIncidentMissing(){var x=B();x.IncidentRunbookReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineIncidentMissing),Is.True);}
        [Test] public void BaselineHotfixMissing(){var x=B();x.HotfixRunbookReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineHotfixMissing),Is.True);}
        [Test] public void BaselineLiveOpsMissing(){var x=B();x.LiveOpsRunbookReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineLiveOpsMissing),Is.True);}
        [Test] public void BaselinePrivacySupportMissing(){var x=B();x.PrivacySupportRunbookReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselinePrivacySupportMissing),Is.True);}
        [Test] public void BaselineRollbackMissing(){var x=B();x.RollbackBoundaryReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineRollbackMissing),Is.True);}
        [Test] public void BaselineArchiveMissing(){var x=B();x.ArchivePinned=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineArchiveNotPinned),Is.True);}
        [Test] public void BaselineCriticalOpen(){var x=B();x.UnresolvedCriticalActions=1;Assert.That(Has(LiveOpsBaselineValidation.ValidateBaseline(x,C().ChainId),LiveOpsBaselineBlockerCodes.BaselineCriticalActionsOpen),Is.True);}

        [Test] public void TransitionHealthy(){Assert.That(V120TransitionEvaluator.Evaluate(T(),B().BaselineId).TransitionReady,Is.True);}
        [Test] public void TransitionMissing(){Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(null,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionMissing),Is.True);}
        [Test] public void TransitionBadMilestone(){var x=T();x.TargetMilestone="v2.0";Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionInvalid),Is.True);}
        [Test] public void TransitionBaselineMismatch(){var x=T();x.BaselineId="other";Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionBaselineMismatch),Is.True);}
        [Test] public void TransitionLeadLow(){var x=T();x.LeadTimeHours=23;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionLeadTimeInsufficient),Is.True);}
        [Test] public void TransitionScopeMissing(){var x=T();x.TransitionScopeReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionScopeMissing),Is.True);}
        [Test] public void TransitionChapterPipelineMissing(){var x=T();x.ChapterPipelineEntryReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionChapterPipelineMissing),Is.True);}
        [Test] public void TransitionOwnershipMissing(){var x=T();x.ContentOwnershipReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionContentOwnershipMissing),Is.True);}
        [Test] public void TransitionAddressablesMissing(){var x=T();x.AddressablesContentModelReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionAddressablesReviewMissing),Is.True);}
        [Test] public void TransitionLocalizationMissing(){var x=T();x.LocalizationPipelineReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionLocalizationMissing),Is.True);}
        [Test] public void TransitionQaMissing(){var x=T();x.QaStrategyReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionQaMissing),Is.True);}
        [Test] public void TransitionAutomationMissing(){var x=T();x.AutomationStrategyReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionAutomationMissing),Is.True);}
        [Test] public void TransitionCapacityMissing(){var x=T();x.TeamCapacityReviewed=false;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionCapacityMissing),Is.True);}
        [Test] public void TransitionCarryoverBlocks(){var x=T();x.CarryoverBlockerCount=1;Assert.That(Has(LiveOpsBaselineValidation.ValidateTransition(x,B().BaselineId),LiveOpsBaselineBlockerCodes.TransitionCarryoverBlockers),Is.True);}
        [Test] public void TransitionLeadFloorPasses(){var x=T();x.LeadTimeHours=24;Assert.That(V120TransitionEvaluator.Evaluate(x,B().BaselineId).TransitionReady,Is.True);}

        [Test] public void ReviewHealthyRecommendsClosure(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),false,false,false,false,false,false,false,false);Assert.That(r.Recommendation,Is.EqualTo(LiveOpsBaselineRecommendation.ReadyForManualV119Closure));}
        [Test] public void ReviewStructuralReadyDespiteProvidersOff(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),false,false,false,false,false,false,false,false);Assert.That(r.ReadyForHumanReview,Is.True);}
        [Test] public void ReviewCannotCloseWithoutAuth(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),true,true,false,false,false,false,false,false);Assert.That(r.MayCloseV119Manually,Is.False);}
        [Test] public void ReviewCanCloseWithReviewAndClosureAuth(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),true,true,true,true,true,true,false,false);Assert.That(r.MayCloseV119Manually,Is.True);}
        [Test] public void ReviewCannotBeginV120WithoutAuth(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),true,true,true,true,true,true,false,false);Assert.That(r.MayBeginV120Preparation,Is.False);}
        [Test] public void ReviewCanBeginV120WithAuth(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),true,true,true,true,true,true,true,false);Assert.That(r.MayBeginV120Preparation,Is.True);}
        [Test] public void PublishAuthorizationSeparate(){var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),T(),true,true,true,true,true,true,true,false);Assert.That(Has(r.ActionBlockers,LiveOpsBaselineBlockerCodes.LiveOpsPublishingNotAuthorized),Is.True);}
        [Test] public void ChainBlockerWinsRecommendation(){var x=C();x.Step03Reviewed=false;var r=LiveOpsBaselineReviewEvaluator.Evaluate(x,B(),T(),true,true,true,true,true,true,true,false);Assert.That(r.Recommendation,Is.EqualTo(LiveOpsBaselineRecommendation.CompleteV119Chain));}
        [Test] public void BaselineBlockerWinsAfterChain(){var x=B();x.ArchivePinned=false;var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),x,T(),true,true,true,true,true,true,true,false);Assert.That(r.Recommendation,Is.EqualTo(LiveOpsBaselineRecommendation.CompleteOperatingBaseline));}
        [Test] public void TransitionBlockerWinsAfterBaseline(){var x=T();x.CarryoverBlockerCount=1;var r=LiveOpsBaselineReviewEvaluator.Evaluate(C(),B(),x,true,true,true,true,true,true,true,false);Assert.That(r.Recommendation,Is.EqualTo(LiveOpsBaselineRecommendation.ReviseV120Transition));}

        [Test] public void BufferCapacityClamp(){var x=new LiveOpsBaselineObservationBuffer(999);Assert.That(x.Capacity,Is.EqualTo(LiveOpsBaselineRequirements.MaxObservationCapacity));}
        [Test] public void BufferMinimumCapacity(){var x=new LiveOpsBaselineObservationBuffer(0);Assert.That(x.Capacity,Is.EqualTo(1));}
        [Test] public void BufferBounded(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=1});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=2});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=3});Assert.That(x.Count,Is.EqualTo(2));}
        [Test] public void BufferOldestFirst(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=1});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=2});x.Add(new LiveOpsBaselineAggregateObservation{WindowIndex=3});Assert.That(x.SnapshotOldestFirst()[0].WindowIndex,Is.EqualTo(2));}
        [Test] public void BufferClear(){var x=new LiveOpsBaselineObservationBuffer(2);x.Add(new LiveOpsBaselineAggregateObservation());x.Clear();Assert.That(x.Count,Is.EqualTo(0));}
        [Test] public void BufferRejectsNull(){var x=new LiveOpsBaselineObservationBuffer(2);Assert.Throws<System.ArgumentNullException>(()=>x.Add(null));}

        static V119EvidenceChainEvidence C(){return new V119EvidenceChainEvidence{EvidenceRetained=true,EvidenceReviewed=true,ChainId="LOR-V119-CHAIN-1",SourceTreeSha256=Sha(),ArchiveSha256=Sha(),Step01Reviewed=true,Step02Reviewed=true,Step03Reviewed=true,Step04Reviewed=true,Step05Reviewed=true,Step06Reviewed=true,Step07Reviewed=true,UnityCompileEvidenceReviewed=true,TestRunnerEvidenceReviewed=true,PrivacyBoundaryReviewed=true,ProductionMutationBoundaryReviewed=true};}
        static LiveOpsOperatingBaselineEvidence B(){return new LiveOpsOperatingBaselineEvidence{EvidenceRetained=true,EvidenceReviewed=true,BaselineId="LOR-OPS-BASELINE-1",ChainId="LOR-V119-CHAIN-1",BaselineSha256=Sha(),RunbookSectionCount=6,ObservabilityRunbookReviewed=true,IncidentRunbookReviewed=true,HotfixRunbookReviewed=true,LiveOpsRunbookReviewed=true,PrivacySupportRunbookReviewed=true,RollbackBoundaryReviewed=true,ArchivePinned=true};}
        static V120TransitionPlanEvidence T(){return new V120TransitionPlanEvidence{EvidenceRetained=true,EvidenceReviewed=true,TransitionId="LOR-V120-TRANSITION-1",BaselineId="LOR-OPS-BASELINE-1",TargetMilestone="v1.20",LeadTimeHours=48,TransitionScopeReviewed=true,ChapterPipelineEntryReviewed=true,ContentOwnershipReviewed=true,AddressablesContentModelReviewed=true,LocalizationPipelineReviewed=true,QaStrategyReviewed=true,AutomationStrategyReviewed=true,TeamCapacityReviewed=true};}
        static string Sha(){return new string('a',64);}
        static bool Has(string[] a,string v){if(a==null)return false;for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
    }
}
