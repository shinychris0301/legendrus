using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsExecution
{
    public static class ExperimentObservationEvaluator
    {
        public static ExperimentObservationResult Evaluate(ExperimentObservationEvidence evidence)
        {
            var blockers=new List<string>();Add(blockers,LiveOpsExecutionValidation.ValidateObservation(evidence));
            if(evidence==null||evidence.Windows==null)return Result(false,false,false,false,false,0,0,0,0,ExperimentObservationRecommendation.InsufficientEvidence,blockers);
            long control=0,treatment=0;int guardrails=0;bool dataQuality=true;
            for(int i=0;i<evidence.Windows.Length;i++)
            {
                var w=evidence.Windows[i];
                if(w==null||w.WindowIndex<0||w.ControlSamples<0||w.TreatmentSamples<0||w.GuardrailBreaches<0){AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationInvalid);continue;}
                control+=w.ControlSamples;treatment+=w.TreatmentSamples;guardrails+=w.GuardrailBreaches;if(!w.DataQualityPassed)dataQuality=false;
            }
            bool structural=!blockers.Contains(LiveOpsExecutionBlockerCodes.ObservationInvalid)&&!blockers.Contains(LiveOpsExecutionBlockerCodes.ObservationMissing)&&!blockers.Contains(LiveOpsExecutionBlockerCodes.ObservationNotReviewed)&&!blockers.Contains(LiveOpsExecutionBlockerCodes.ObservationPrivacyReviewMissing);
            bool sample=control>=LiveOpsExecutionRequirements.MinimumExperimentSamplesPerVariant&&treatment>=LiveOpsExecutionRequirements.MinimumExperimentSamplesPerVariant;
            if(!sample)AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationSampleInsufficient);
            long total=control+treatment;int mismatch=0;if(total>0){long diff=Math.Abs(control-treatment);mismatch=(int)((diff*LiveOpsExecutionRequirements.BasisPointsTotal)/total);}
            bool ratio=mismatch<=LiveOpsExecutionRequirements.MaximumSampleRatioMismatchBasisPoints;if(!ratio)AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationSampleRatioMismatch);
            if(!dataQuality)AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationDataQualityFailure);
            bool guards=guardrails==0;if(!guards)AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationGuardrailBreach);
            if(evidence.StoppingRuleTriggered)AddOne(blockers,LiveOpsExecutionBlockerCodes.ObservationStoppingRuleTriggered);
            ExperimentObservationRecommendation r;
            if(!structural||!sample)r=ExperimentObservationRecommendation.InsufficientEvidence;
            else if(!dataQuality)r=ExperimentObservationRecommendation.PauseForDataQualityReview;
            else if(!ratio)r=ExperimentObservationRecommendation.PauseForSampleRatioReview;
            else if(!guards)r=ExperimentObservationRecommendation.StopForGuardrailReview;
            else if(evidence.StoppingRuleTriggered)r=ExperimentObservationRecommendation.StopForStoppingRuleReview;
            else if(evidence.ExperimentWindowComplete)r=ExperimentObservationRecommendation.ConcludeForHumanReview;
            else r=ExperimentObservationRecommendation.ContinueObservation;
            return Result(structural,sample,ratio,dataQuality,guards,(int)Math.Min(control,int.MaxValue),(int)Math.Min(treatment,int.MaxValue),mismatch,guardrails,r,blockers);
        }
        static ExperimentObservationResult Result(bool s,bool sample,bool ratio,bool dq,bool g,int c,int t,int m,int gb,ExperimentObservationRecommendation r,List<string>b){return new ExperimentObservationResult{StructurallyValid=s,SampleSufficient=sample,SampleRatioHealthy=ratio,DataQualityHealthy=dq,GuardrailsHealthy=g,TotalControlSamples=c,TotalTreatmentSamples=t,SampleRatioMismatchBasisPoints=m,TotalGuardrailBreaches=gb,Recommendation=r,Blockers=b.ToArray()};}
        static void Add(List<string>d,string[]v){for(int i=0;i<v.Length;i++)AddOne(d,v[i]);}
        static void AddOne(List<string>d,string v){if(!d.Contains(v))d.Add(v);}
    }
}
