using System.Collections.Generic;

namespace LegendsOfRus.IncidentResponse
{
    public static class IncidentTriageEvaluator
    {
        public static IncidentTriageResult Evaluate(IncidentRegisterEvidence evidence)
        {
            var blockers = new List<string>();
            if (evidence == null || !evidence.EvidenceRetained)
            {
                blockers.Add(IncidentResponseBlockerCodes.RegisterMissing);
                return Result(false, false, 0, 0, 0, 0, 0, IncidentResponseRecommendation.Observe, blockers);
            }
            if (!evidence.EvidenceReviewed) blockers.Add(IncidentResponseBlockerCodes.RegisterNotReviewed);

            IncidentRecord[] incidents = evidence.Incidents ?? new IncidentRecord[0];
            if (incidents.Length > IncidentResponseRequirements.MaximumIncidentRecords)
                blockers.Add(IncidentResponseBlockerCodes.IncidentCountExceeded);

            var ids = new HashSet<string>();
            int critical = 0, high = 0, medium = 0, low = 0, pendingRetest = 0;
            bool dataLoss = false, purchaseIntegrity = false, structural = incidents.Length <= IncidentResponseRequirements.MaximumIncidentRecords;

            for (int i = 0; i < incidents.Length; i++)
            {
                IncidentRecord incident = incidents[i];
                if (incident == null || !IncidentResponseValidation.IsIncidentId(incident.IncidentId))
                {
                    blockers.Add(IncidentResponseBlockerCodes.IncidentIdInvalid);
                    structural = false;
                    continue;
                }
                if (!ids.Add(incident.IncidentId))
                {
                    blockers.Add(IncidentResponseBlockerCodes.IncidentIdDuplicate);
                    structural = false;
                }

                bool open = incident.State != IncidentLifecycleState.Closed;
                if (open)
                {
                    if (incident.Severity == IncidentSeverity.Critical) critical++;
                    else if (incident.Severity == IncidentSeverity.High) high++;
                    else if (incident.Severity == IncidentSeverity.Medium) medium++;
                    else low++;
                    dataLoss |= incident.UserDataLossRisk || incident.Category == IncidentCategory.DataLoss;
                    purchaseIntegrity |= incident.PurchaseIntegrityRisk || incident.Category == IncidentCategory.PurchaseIntegrity;
                }

                if (incident.State == IncidentLifecycleState.FixedPendingRetest && incident.RetestRequired && !incident.RetestPassed)
                    pendingRetest++;
                if (incident.State == IncidentLifecycleState.Closed && incident.RetestRequired && !incident.RetestPassed)
                {
                    blockers.Add(IncidentResponseBlockerCodes.IncidentStateInvalid);
                    structural = false;
                }
            }

            if (critical > 0) blockers.Add(IncidentResponseBlockerCodes.CriticalIncidentOpen);
            if (high > 0) blockers.Add(IncidentResponseBlockerCodes.HighIncidentOpen);
            if (dataLoss) blockers.Add(IncidentResponseBlockerCodes.DataLossRiskOpen);
            if (purchaseIntegrity) blockers.Add(IncidentResponseBlockerCodes.PurchaseIntegrityRiskOpen);
            if (pendingRetest > 0) blockers.Add(IncidentResponseBlockerCodes.RetestPending);

            bool evidenceReady = structural && evidence.EvidenceReviewed;
            bool healthClear = evidenceReady && critical == 0 && high == 0 && !dataLoss && !purchaseIntegrity && pendingRetest == 0;
            IncidentResponseRecommendation recommendation;
            if (!evidenceReady) recommendation = IncidentResponseRecommendation.Observe;
            else if (critical > 0 || dataLoss || purchaseIntegrity) recommendation = IncidentResponseRecommendation.EscalateRolloutControlReview;
            else if (high > 0 || pendingRetest > 0) recommendation = IncidentResponseRecommendation.HotfixQualificationReview;
            else if (medium > 0) recommendation = IncidentResponseRecommendation.HoldRolloutReview;
            else recommendation = IncidentResponseRecommendation.ContinueMonitoring;

            return Result(structural, healthClear, critical, high, medium, low, pendingRetest, recommendation, blockers);
        }

        static IncidentTriageResult Result(bool structural, bool healthClear, int critical, int high, int medium, int low, int pendingRetest, IncidentResponseRecommendation recommendation, List<string> blockers)
        {
            return new IncidentTriageResult
            {
                RegisterStructurallyValid = structural,
                ProductionHealthClear = healthClear,
                OpenCritical = critical,
                OpenHigh = high,
                OpenMedium = medium,
                OpenLow = low,
                PendingRetest = pendingRetest,
                Recommendation = recommendation,
                Blockers = blockers.ToArray()
            };
        }
    }
}
