using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsExecution
{
    public static class ContentRotationEvaluator
    {
        public static RotationReviewResult Evaluate(ContentRotationEvidence evidence)
        {
            var blockers=new List<string>();
            Add(blockers,LiveOpsExecutionValidation.ValidateRotation(evidence));
            if(blockers.Count>0)return new RotationReviewResult{StructurallyValid=false,RotationSafe=false,EntryCount=evidence!=null&&evidence.Entries!=null?evidence.Entries.Length:0,Blockers=blockers.ToArray()};
            var entries=(ContentRotationEntryEvidence[])evidence.Entries.Clone();
            Array.Sort(entries,(a,b)=>a==null?(b==null?0:1):(b==null?-1:a.StartMinute.CompareTo(b.StartMinute)));
            for(int i=0;i<entries.Length;i++)
            {
                var x=entries[i];
                if(x==null||!LiveOpsExecutionValidation.HasPrefix(x.RotationId,LiveOpsExecutionRequirements.RotationPrefix)||string.IsNullOrEmpty(x.ContentId)||string.IsNullOrEmpty(x.ContentVersion)||x.EndMinute<=x.StartMinute||x.RequiredCooldownMinutes<0){AddOne(blockers,LiveOpsExecutionBlockerCodes.RotationInvalid);continue;}
                if(i>0)
                {
                    var p=entries[i-1];
                    if(p!=null)
                    {
                        if(x.StartMinute<p.EndMinute)AddOne(blockers,LiveOpsExecutionBlockerCodes.RotationOverlap);
                        else if(x.StartMinute-p.EndMinute<p.RequiredCooldownMinutes)AddOne(blockers,LiveOpsExecutionBlockerCodes.RotationCooldownInsufficient);
                    }
                }
            }
            return new RotationReviewResult{StructurallyValid=!blockers.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid),RotationSafe=blockers.Count==0,EntryCount=entries.Length,Blockers=blockers.ToArray()};
        }
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
