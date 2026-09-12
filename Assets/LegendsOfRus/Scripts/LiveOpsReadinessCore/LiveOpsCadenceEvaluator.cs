using System;
using System.Collections.Generic;

namespace LegendsOfRus.LiveOpsReadiness
{
    public static class LiveOpsCadenceEvaluator
    {
        public static CadenceReviewResult Evaluate(ReviewedLiveOpsGuardrailProfile profile, LiveOpsCadencePlanEvidence plan)
        {
            var blockers = new List<string>();
            Add(blockers, LiveOpsReadinessValidation.ValidateProfile(profile));
            Add(blockers, LiveOpsReadinessValidation.ValidateCadence(plan));
            if (profile == null || plan == null || blockers.Count > 0) return Result(false, 0, blockers);
            if (plan.MinutesUntilWindowStart < profile.MinimumLeadMinutes) blockers.Add(LiveOpsReadinessBlockerCodes.EventLeadTimeInsufficient);
            int maxConcurrency = 0;
            LiveOpsEventSlotEvidence[] events = plan.Events ?? Array.Empty<LiveOpsEventSlotEvidence>();
            for (int i = 0; i < events.Length; i++)
            {
                LiveOpsEventSlotEvidence current = events[i];
                if (current.EndMinute - current.StartMinute > profile.MaximumEventDurationMinutes) AddOne(blockers, LiveOpsReadinessBlockerCodes.EventDurationExceeded);
                int concurrency = 0;
                for (int j = 0; j < events.Length; j++) if (Overlaps(current, events[j])) concurrency++;
                if (concurrency > maxConcurrency) maxConcurrency = concurrency;
                if (concurrency > profile.MaximumConcurrentEvents) AddOne(blockers, LiveOpsReadinessBlockerCodes.ConcurrentEventsExceeded);
            }
            LiveOpsEventSlotEvidence[] sorted = (LiveOpsEventSlotEvidence[])events.Clone();
            Array.Sort(sorted, CompareStart);
            for (int i = 1; i < sorted.Length; i++)
            {
                if (sorted[i].StartMinute >= sorted[i-1].EndMinute && sorted[i].StartMinute - sorted[i-1].EndMinute < profile.MinimumGapMinutes) AddOne(blockers, LiveOpsReadinessBlockerCodes.EventGapInsufficient);
            }
            return Result(true, maxConcurrency, blockers);
        }

        static int CompareStart(LiveOpsEventSlotEvidence a, LiveOpsEventSlotEvidence b) { return a.StartMinute.CompareTo(b.StartMinute); }
        static bool Overlaps(LiveOpsEventSlotEvidence a, LiveOpsEventSlotEvidence b) { return a != null && b != null && a.StartMinute < b.EndMinute && b.StartMinute < a.EndMinute; }
        static CadenceReviewResult Result(bool structurallyValid, int maxConcurrency, List<string> blockers) { return new CadenceReviewResult { StructurallyValid = structurallyValid, CadenceSafe = structurallyValid && blockers.Count == 0, MaximumObservedConcurrency = maxConcurrency, Blockers = blockers.ToArray() }; }
        static void Add(List<string> d, string[] v) { for (int i = 0; i < v.Length; i++) AddOne(d, v[i]); }
        static void AddOne(List<string> d, string v) { if (!d.Contains(v)) d.Add(v); }
    }
}
