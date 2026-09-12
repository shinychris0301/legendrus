using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsBaseline
{
    public static class LiveOpsBaselineValidation
    {
        public static string[] ValidateChain(V119EvidenceChainEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsBaselineBlockerCodes.ChainMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsBaselineBlockerCodes.ChainMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsBaselineBlockerCodes.ChainNotReviewed);
            if(e.SchemaVersion!=LiveOpsBaselineRequirements.SchemaVersion || !ValidId(e.ChainId,LiveOpsBaselineRequirements.ChainPrefix) || e.UnresolvedCriticalBlockers<0)
                b.Add(LiveOpsBaselineBlockerCodes.ChainInvalid);
            if(!ValidSha(e.SourceTreeSha256))b.Add(LiveOpsBaselineBlockerCodes.ChainSourceHashInvalid);
            if(!ValidSha(e.ArchiveSha256))b.Add(LiveOpsBaselineBlockerCodes.ChainArchiveHashInvalid);
            if(ReviewedStepCount(e)!=LiveOpsBaselineRequirements.RequiredV119StepCount)b.Add(LiveOpsBaselineBlockerCodes.ChainStepMissing);
            if(!e.UnityCompileEvidenceReviewed || !e.TestRunnerEvidenceReviewed)b.Add(LiveOpsBaselineBlockerCodes.ChainUnityEvidenceMissing);
            if(!e.PrivacyBoundaryReviewed)b.Add(LiveOpsBaselineBlockerCodes.ChainPrivacyBoundaryNotReviewed);
            if(!e.ProductionMutationBoundaryReviewed)b.Add(LiveOpsBaselineBlockerCodes.ChainMutationBoundaryNotReviewed);
            if(e.UnresolvedCriticalBlockers>0)b.Add(LiveOpsBaselineBlockerCodes.ChainCriticalBlockersOpen);
            return b.ToArray();
        }

        public static string[] ValidateBaseline(LiveOpsOperatingBaselineEvidence e,string expectedChainId)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsBaselineBlockerCodes.BaselineMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsBaselineBlockerCodes.BaselineMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineNotReviewed);
            if(e.SchemaVersion!=LiveOpsBaselineRequirements.SchemaVersion || !ValidId(e.BaselineId,LiveOpsBaselineRequirements.BaselinePrefix) || string.IsNullOrEmpty(e.ChainId) || e.RunbookSectionCount<0 || e.UnresolvedCriticalActions<0)
                b.Add(LiveOpsBaselineBlockerCodes.BaselineInvalid);
            if(!string.IsNullOrEmpty(expectedChainId) && e.ChainId!=expectedChainId)b.Add(LiveOpsBaselineBlockerCodes.BaselineChainMismatch);
            if(!ValidSha(e.BaselineSha256))b.Add(LiveOpsBaselineBlockerCodes.BaselineInvalid);
            if(e.RunbookSectionCount<LiveOpsBaselineRequirements.MinimumRunbookSections)b.Add(LiveOpsBaselineBlockerCodes.BaselineRunbooksIncomplete);
            if(!e.ObservabilityRunbookReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineObservabilityMissing);
            if(!e.IncidentRunbookReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineIncidentMissing);
            if(!e.HotfixRunbookReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineHotfixMissing);
            if(!e.LiveOpsRunbookReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineLiveOpsMissing);
            if(!e.PrivacySupportRunbookReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselinePrivacySupportMissing);
            if(!e.RollbackBoundaryReviewed)b.Add(LiveOpsBaselineBlockerCodes.BaselineRollbackMissing);
            if(!e.ArchivePinned)b.Add(LiveOpsBaselineBlockerCodes.BaselineArchiveNotPinned);
            if(e.UnresolvedCriticalActions>0)b.Add(LiveOpsBaselineBlockerCodes.BaselineCriticalActionsOpen);
            return b.ToArray();
        }

        public static string[] ValidateTransition(V120TransitionPlanEvidence e,string expectedBaselineId)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsBaselineBlockerCodes.TransitionMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsBaselineBlockerCodes.TransitionMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionNotReviewed);
            if(e.SchemaVersion!=LiveOpsBaselineRequirements.SchemaVersion || !ValidId(e.TransitionId,LiveOpsBaselineRequirements.TransitionPrefix) || string.IsNullOrEmpty(e.BaselineId) || e.TargetMilestone!=LiveOpsBaselineRequirements.V120Milestone || e.LeadTimeHours<0 || e.CarryoverBlockerCount<0)
                b.Add(LiveOpsBaselineBlockerCodes.TransitionInvalid);
            if(!string.IsNullOrEmpty(expectedBaselineId) && e.BaselineId!=expectedBaselineId)b.Add(LiveOpsBaselineBlockerCodes.TransitionBaselineMismatch);
            if(e.LeadTimeHours<LiveOpsBaselineRequirements.MinimumTransitionLeadHours)b.Add(LiveOpsBaselineBlockerCodes.TransitionLeadTimeInsufficient);
            if(!e.TransitionScopeReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionScopeMissing);
            if(!e.ChapterPipelineEntryReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionChapterPipelineMissing);
            if(!e.ContentOwnershipReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionContentOwnershipMissing);
            if(!e.AddressablesContentModelReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionAddressablesReviewMissing);
            if(!e.LocalizationPipelineReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionLocalizationMissing);
            if(!e.QaStrategyReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionQaMissing);
            if(!e.AutomationStrategyReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionAutomationMissing);
            if(!e.TeamCapacityReviewed)b.Add(LiveOpsBaselineBlockerCodes.TransitionCapacityMissing);
            if(e.CarryoverBlockerCount>LiveOpsBaselineRequirements.MaxCarryoverBlockers)b.Add(LiveOpsBaselineBlockerCodes.TransitionCarryoverBlockers);
            return b.ToArray();
        }

        public static int ReviewedStepCount(V119EvidenceChainEvidence e)
        {
            if(e==null)return 0;
            int n=0;
            if(e.Step01Reviewed)n++; if(e.Step02Reviewed)n++; if(e.Step03Reviewed)n++; if(e.Step04Reviewed)n++;
            if(e.Step05Reviewed)n++; if(e.Step06Reviewed)n++; if(e.Step07Reviewed)n++;
            return n;
        }

        static bool ValidId(string value,string prefix){return !string.IsNullOrEmpty(value)&&value.StartsWith(prefix,StringComparison.Ordinal)&&value.Length>prefix.Length;}
        static bool ValidSha(string value)
        {
            if(string.IsNullOrEmpty(value)||value.Length!=LiveOpsBaselineRequirements.Sha256HexLength)return false;
            for(int i=0;i<value.Length;i++){char c=value[i];bool hex=(c>='0'&&c<='9')||(c>='a'&&c<='f')||(c>='A'&&c<='F');if(!hex)return false;}
            return true;
        }
    }
}
