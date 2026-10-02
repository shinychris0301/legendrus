using LegendsOfRus.IncidentResponse;
using NUnit.Framework;

namespace LegendsOfRus.Tests.IncidentResponse
{
    public sealed class IncidentTriageEvaluatorEditModeTests
    {
        [Test]
        public void MissingRegister_IsNotClear()
        {
            var r = IncidentTriageEvaluator.Evaluate(null);
            Assert.That(r.ProductionHealthClear, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.RegisterMissing);
        }

        [Test]
        public void UnreviewedRegister_IsNotClear()
        {
            var r = IncidentTriageEvaluator.Evaluate(new IncidentRegisterEvidence { EvidenceRetained = true });
            Assert.That(r.ProductionHealthClear, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.RegisterNotReviewed);
        }

        [Test]
        public void EmptyReviewedRegister_IsClear()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister());
            Assert.That(r.ProductionHealthClear, Is.True);
            Assert.That(r.Recommendation, Is.EqualTo(IncidentResponseRecommendation.ContinueMonitoring));
        }

        [Test]
        public void CriticalIncident_EscalatesRolloutControl()
        {
            var e = CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Critical));
            var r = IncidentTriageEvaluator.Evaluate(e);
            Assert.That(r.OpenCritical, Is.EqualTo(1));
            Assert.That(r.Recommendation, Is.EqualTo(IncidentResponseRecommendation.EscalateRolloutControlReview));
        }

        [Test]
        public void HighIncident_RequestsHotfixReview()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.High)));
            Assert.That(r.OpenHigh, Is.EqualTo(1));
            Assert.That(r.Recommendation, Is.EqualTo(IncidentResponseRecommendation.HotfixQualificationReview));
        }

        [Test]
        public void MediumIncident_HoldsRolloutReview()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Medium)));
            Assert.That(r.Recommendation, Is.EqualTo(IncidentResponseRecommendation.HoldRolloutReview));
        }

        [Test]
        public void LowIncident_DoesNotBlockProductionHealth()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Low)));
            Assert.That(r.ProductionHealthClear, Is.True);
        }

        [Test]
        public void DataLossRisk_BlocksEvenAtLowSeverity()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.Low); i.UserDataLossRisk = true;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.ProductionHealthClear, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.DataLossRiskOpen);
        }

        [Test]
        public void PurchaseIntegrityRisk_BlocksEvenAtLowSeverity()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.Low); i.PurchaseIntegrityRisk = true;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.ProductionHealthClear, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.PurchaseIntegrityRiskOpen);
        }

        [Test]
        public void PendingRetest_BlocksHealthClear()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.Medium); i.State = IncidentLifecycleState.FixedPendingRetest; i.RetestRequired = true;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.PendingRetest, Is.EqualTo(1));
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.RetestPending);
        }

        [Test]
        public void PassedRetest_AllowsHealthClear_WhenNoHighRiskOpen()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.Medium); i.State = IncidentLifecycleState.FixedPendingRetest; i.RetestRequired = true; i.RetestPassed = true;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.PendingRetest, Is.EqualTo(0));
            Assert.That(r.ProductionHealthClear, Is.True);
        }

        [Test]
        public void DuplicateIncidentId_IsStructuralFailure()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("LOR-INC-0001", IncidentSeverity.Low), Incident("LOR-INC-0001", IncidentSeverity.Low)));
            Assert.That(r.RegisterStructurallyValid, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.IncidentIdDuplicate);
        }

        [Test]
        public void InvalidIncidentId_IsStructuralFailure()
        {
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(Incident("INC1", IncidentSeverity.Low)));
            Assert.That(r.RegisterStructurallyValid, Is.False);
        }

        [Test]
        public void ClosedHighIncident_DoesNotCountOpenHigh()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.High); i.State = IncidentLifecycleState.Closed;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.OpenHigh, Is.EqualTo(0));
        }

        [Test]
        public void ClosedIncidentThatRequiredRetest_MustHavePassedRetest()
        {
            var i = Incident("LOR-INC-0001", IncidentSeverity.High); i.State = IncidentLifecycleState.Closed; i.RetestRequired = true;
            var r = IncidentTriageEvaluator.Evaluate(CleanRegister(i));
            Assert.That(r.RegisterStructurallyValid, Is.False);
            CollectionAssert.Contains(r.Blockers, IncidentResponseBlockerCodes.IncidentStateInvalid);
        }

        static IncidentRecord Incident(string id, IncidentSeverity severity) => new IncidentRecord { IncidentId = id, Severity = severity, State = IncidentLifecycleState.Investigating };
        static IncidentRegisterEvidence CleanRegister(params IncidentRecord[] incidents) => new IncidentRegisterEvidence { EvidenceRetained = true, EvidenceReviewed = true, Incidents = incidents };
    }
}
