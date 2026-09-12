using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsExecution
{
    public static class LiveOpsExecutionValidation
    {
        public static bool HasPrefix(string value,string prefix){return !string.IsNullOrEmpty(value)&&value.StartsWith(prefix,StringComparison.Ordinal);}
        public static int RateBasisPoints(long part,long total){if(total<=0||part<=0)return 0;if(part>=total)return LiveOpsExecutionRequirements.BasisPointsTotal;return (int)((part*LiveOpsExecutionRequirements.BasisPointsTotal)/total);}
        public static int[] NormalizeCounts(int a,int b){return new[]{a<0?0:a,b<0?0:b};}
        public static string[] ValidateExecution(LiveOpsExecutionEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsExecutionBlockerCodes.ExecutionMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsExecutionBlockerCodes.ExecutionMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsExecutionBlockerCodes.ExecutionNotReviewed);
            if(e.SchemaVersion!=LiveOpsExecutionRequirements.SchemaVersion||!HasPrefix(e.ExecutionId,LiveOpsExecutionRequirements.ExecutionPrefix)||string.IsNullOrEmpty(e.ApprovedReadinessId)||e.WindowEndMinute<=e.WindowStartMinute||!e.PreflightPassed||!e.SupportCoverageConfirmed||!e.RollbackPlanConfirmed)b.Add(LiveOpsExecutionBlockerCodes.ExecutionInvalid);
            return Unique(b);
        }
        public static string[] ValidateRotation(ContentRotationEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsExecutionBlockerCodes.RotationMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsExecutionBlockerCodes.RotationMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsExecutionBlockerCodes.RotationNotReviewed);
            if(e.SchemaVersion!=LiveOpsExecutionRequirements.SchemaVersion||!HasPrefix(e.RotationPlanId,LiveOpsExecutionRequirements.RotationPrefix)||e.Entries==null||e.Entries.Length==0||e.Entries.Length>LiveOpsExecutionRequirements.MaxRotationEntries)b.Add(LiveOpsExecutionBlockerCodes.RotationInvalid);
            return Unique(b);
        }
        public static string[] ValidateObservation(ExperimentObservationEvidence e)
        {
            var b=new List<string>();
            if(e==null){b.Add(LiveOpsExecutionBlockerCodes.ObservationMissing);return b.ToArray();}
            if(!e.EvidenceRetained)b.Add(LiveOpsExecutionBlockerCodes.ObservationMissing);
            if(!e.EvidenceReviewed)b.Add(LiveOpsExecutionBlockerCodes.ObservationNotReviewed);
            if(e.SchemaVersion!=LiveOpsExecutionRequirements.SchemaVersion||!HasPrefix(e.ObservationId,LiveOpsExecutionRequirements.ObservationPrefix)||string.IsNullOrEmpty(e.ExperimentId)||e.Windows==null||e.Windows.Length==0||e.Windows.Length>LiveOpsExecutionRequirements.MaxObservationWindows)b.Add(LiveOpsExecutionBlockerCodes.ObservationInvalid);
            if(!e.PrivacyReviewed)b.Add(LiveOpsExecutionBlockerCodes.ObservationPrivacyReviewMissing);
            return Unique(b);
        }
        static string[] Unique(List<string>b){var r=new List<string>();for(int i=0;i<b.Count;i++)if(!r.Contains(b[i]))r.Add(b[i]);return r.ToArray();}
    }
}
