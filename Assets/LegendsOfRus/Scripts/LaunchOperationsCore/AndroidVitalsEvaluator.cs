using System;
using System.Collections.Generic;

namespace LegendsOfRus.LaunchOperations
{
    public static class AndroidVitalsEvaluator
    {
        public static LaunchHealthEvaluation Evaluate(AndroidVitalsSnapshot vitals, LaunchIncidentSummary incidents, ReleaseKind releaseKind, int currentRolloutPercent, bool manualRolloutActionAuthorized)
        {
            var blockers = new List<string>();
            if (vitals == null || !vitals.EvidenceRetained)
            {
                blockers.Add(LaunchOperationsBlockerCodes.VitalsEvidenceMissing);
                return Result(LaunchHealthClassification.Unknown, LaunchRolloutRecommendation.Observe, false, false, blockers);
            }
            if (!vitals.EvidenceReviewed) blockers.Add(LaunchOperationsBlockerCodes.VitalsEvidenceNotReviewed);
            if (vitals.WindowDays < 1 || vitals.WindowDays > LaunchOperationsRequirements.MaximumVitalsWindowDays) blockers.Add(LaunchOperationsBlockerCodes.VitalsWindowInvalid);
            if (!AllRatesValid(vitals)) blockers.Add(LaunchOperationsBlockerCodes.VitalsWindowInvalid);

            bool crashBad = vitals.UserPerceivedCrashRatePercent >= LaunchOperationsRequirements.UserPerceivedCrashOverallBadBehaviorPercent;
            bool anrBad = vitals.UserPerceivedAnrRatePercent >= LaunchOperationsRequirements.UserPerceivedAnrOverallBadBehaviorPercent;
            bool deviceCrashBad = vitals.MaxPhoneModelCrashRatePercent >= LaunchOperationsRequirements.UserPerceivedCrashPhoneModelBadBehaviorPercent;
            bool deviceAnrBad = vitals.MaxPhoneModelAnrRatePercent >= LaunchOperationsRequirements.UserPerceivedAnrPhoneModelBadBehaviorPercent;
            bool wakeLockBad = vitals.ExcessivePartialWakeLockRatePercent >= LaunchOperationsRequirements.ExcessivePartialWakeLockOverallBadBehaviorPercent;
            bool batteryBad = vitals.ExcessiveBatteryUsageRatePercent >= LaunchOperationsRequirements.ExcessiveBatteryUsageOverallBadBehaviorPercent;

            if (crashBad) blockers.Add(LaunchOperationsBlockerCodes.CrashThresholdExceeded);
            if (anrBad) blockers.Add(LaunchOperationsBlockerCodes.AnrThresholdExceeded);
            if (deviceCrashBad) blockers.Add(LaunchOperationsBlockerCodes.DeviceCrashThresholdExceeded);
            if (deviceAnrBad) blockers.Add(LaunchOperationsBlockerCodes.DeviceAnrThresholdExceeded);
            if (wakeLockBad) blockers.Add(LaunchOperationsBlockerCodes.WakeLockThresholdExceeded);
            if (batteryBad) blockers.Add(LaunchOperationsBlockerCodes.BatteryThresholdExceeded);

            bool incidentReviewed = incidents != null && incidents.RegisterRetained && incidents.RegisterReviewed;
            bool criticalIncident = incidents != null && incidents.OpenCritical > 0;
            bool highIncident = incidents != null && incidents.OpenHigh > 0;
            bool blockingRegression = incidents != null && incidents.BlockingRegressionOpen;
            if (incidents == null || !incidents.RegisterRetained || !incidents.RegisterReviewed) blockers.Add(LaunchOperationsBlockerCodes.IncidentRegisterNotReviewed);
            if (criticalIncident) blockers.Add(LaunchOperationsBlockerCodes.CriticalIncidentOpen);
            if (highIncident) blockers.Add(LaunchOperationsBlockerCodes.HighSeverityIncidentOpen);
            if (blockingRegression) blockers.Add(LaunchOperationsBlockerCodes.BlockingIncidentOpen);

            bool hardBad = crashBad || anrBad || deviceCrashBad || deviceAnrBad || criticalIncident || blockingRegression;
            bool operationalBad = hardBad || wakeLockBad || batteryBad || highIncident;
            bool watch = IsWatchBand(vitals) || (incidents != null && incidents.OpenMedium > 0) || (incidents != null && incidents.CrashOrAnrClusterUnderInvestigation);
            bool evidenceReady = vitals.EvidenceReviewed && vitals.WindowDays >= 1 && vitals.WindowDays <= LaunchOperationsRequirements.MaximumVitalsWindowDays && AllRatesValid(vitals) && incidentReviewed;

            LaunchHealthClassification classification = !evidenceReady ? LaunchHealthClassification.Unknown : hardBad ? LaunchHealthClassification.Critical : operationalBad ? LaunchHealthClassification.Unhealthy : watch ? LaunchHealthClassification.Watch : LaunchHealthClassification.Healthy;
            LaunchRolloutRecommendation recommendation;
            if (!evidenceReady) recommendation = LaunchRolloutRecommendation.Observe;
            else if (hardBad) recommendation = releaseKind == ReleaseKind.ProductionUpdate && currentRolloutPercent < 100 ? LaunchRolloutRecommendation.HaltStagedRollout : LaunchRolloutRecommendation.EscalateRollbackReview;
            else if (operationalBad || watch) recommendation = LaunchRolloutRecommendation.Hold;
            else recommendation = LaunchRolloutRecommendation.Continue;

            bool mayContinue = evidenceReady && recommendation == LaunchRolloutRecommendation.Continue && manualRolloutActionAuthorized;
            if (evidenceReady && recommendation == LaunchRolloutRecommendation.Continue && !manualRolloutActionAuthorized)
                blockers.Add(LaunchOperationsBlockerCodes.RolloutActionNotAuthorized);
            bool humanIncidentReview = hardBad || operationalBad || highIncident || criticalIncident || blockingRegression;
            return Result(classification, recommendation, mayContinue, humanIncidentReview, blockers);
        }

        static bool AllRatesValid(AndroidVitalsSnapshot v)
        {
            return LaunchOperationsValidation.IsPercent(v.UserPerceivedCrashRatePercent) &&
                   LaunchOperationsValidation.IsPercent(v.UserPerceivedAnrRatePercent) &&
                   LaunchOperationsValidation.IsPercent(v.MaxPhoneModelCrashRatePercent) &&
                   LaunchOperationsValidation.IsPercent(v.MaxPhoneModelAnrRatePercent) &&
                   LaunchOperationsValidation.IsPercent(v.ExcessivePartialWakeLockRatePercent) &&
                   LaunchOperationsValidation.IsPercent(v.ExcessiveBatteryUsageRatePercent);
        }

        static bool IsWatchBand(AndroidVitalsSnapshot v)
        {
            double ratio = LaunchOperationsRequirements.WatchBandRatio;
            return v.UserPerceivedCrashRatePercent >= LaunchOperationsRequirements.UserPerceivedCrashOverallBadBehaviorPercent * ratio ||
                   v.UserPerceivedAnrRatePercent >= LaunchOperationsRequirements.UserPerceivedAnrOverallBadBehaviorPercent * ratio ||
                   v.MaxPhoneModelCrashRatePercent >= LaunchOperationsRequirements.UserPerceivedCrashPhoneModelBadBehaviorPercent * ratio ||
                   v.MaxPhoneModelAnrRatePercent >= LaunchOperationsRequirements.UserPerceivedAnrPhoneModelBadBehaviorPercent * ratio ||
                   v.ExcessivePartialWakeLockRatePercent >= LaunchOperationsRequirements.ExcessivePartialWakeLockOverallBadBehaviorPercent * ratio ||
                   v.ExcessiveBatteryUsageRatePercent >= LaunchOperationsRequirements.ExcessiveBatteryUsageOverallBadBehaviorPercent * ratio;
        }

        static LaunchHealthEvaluation Result(LaunchHealthClassification classification, LaunchRolloutRecommendation recommendation, bool mayContinue, bool incidentReview, List<string> blockers)
        {
            return new LaunchHealthEvaluation
            {
                Classification = classification,
                Recommendation = recommendation,
                MayContinueOrIncreaseRollout = mayContinue,
                HumanIncidentReviewRequired = incidentReview,
                Blockers = blockers.ToArray()
            };
        }
    }
}
