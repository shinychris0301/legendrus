using System.Collections.Generic;

namespace LegendsOfRus.LaunchCandidate
{
    public static class ProductionAccessDecisionEvaluator
    {
        public static ProductionAccessDecisionResult Evaluate(ProductionAccessDecisionEvidence evidence)
        {
            var blockers = new List<string>();
            Require(evidence != null && evidence.DeveloperAccountRuleReviewed, LaunchCandidateBlockerCodes.AccountRuleNotReviewed, blockers);
            Require(evidence != null && evidence.ProductionAccessRequirementResolved, LaunchCandidateBlockerCodes.DecisionRequirementUnresolved, blockers);

            if (evidence != null && evidence.DeveloperAccountRuleReviewed && evidence.ProductionAccessRequirementResolved)
            {
                if (evidence.ProductionAccessDecisionRequired)
                {
                    Require(evidence.DecisionEvidenceRetained, LaunchCandidateBlockerCodes.DecisionEvidenceMissing, blockers);
                    Require(evidence.DecisionReviewed, LaunchCandidateBlockerCodes.DecisionNotReviewed, blockers);
                    Require(evidence.DecisionStatus == ProductionAccessDecisionStatus.Granted, LaunchCandidateBlockerCodes.DecisionNotGranted, blockers);
                }
                else
                {
                    Require(evidence.DecisionStatus == ProductionAccessDecisionStatus.NotRequired, LaunchCandidateBlockerCodes.DecisionNotReviewed, blockers);
                }
            }

            bool satisfied = blockers.Count == 0;
            return new ProductionAccessDecisionResult
            {
                Satisfied = satisfied,
                ProductionAccessGrantedOrNotRequired = satisfied,
                Blockers = blockers.ToArray()
            };
        }

        static void Require(bool condition, string blocker, List<string> blockers)
        {
            if (!condition) blockers.Add(blocker);
        }
    }
}
