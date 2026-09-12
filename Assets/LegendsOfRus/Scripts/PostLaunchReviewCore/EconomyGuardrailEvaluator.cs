using System.Collections.Generic;

namespace LegendsOfRus.PostLaunchReview
{
    public static class EconomyGuardrailEvaluator
    {
        public static EconomyGuardrailResult Evaluate(ReviewedGuardrailProfile profile, EconomyFlowEvidence economy)
        {
            var blockers = new List<string>();
            Add(blockers, PostLaunchReviewValidation.ValidateProfile(profile));
            Add(blockers, PostLaunchReviewValidation.ValidateEconomy(economy));
            bool structural = blockers.Count == 0;
            var r = new EconomyGuardrailResult { StructurallyValid = structural };
            if (!structural) { r.Blockers = blockers.ToArray(); return r; }

            r.SampleSufficient = economy.ActiveUsers >= profile.MinimumActiveUsers;
            if (!r.SampleSufficient) blockers.Add(PostLaunchReviewBlockerCodes.EconomySampleInsufficient);
            if (economy.SoftCurrencyEarned <= 0)
            {
                blockers.Add(PostLaunchReviewBlockerCodes.SoftCurrencyFlowMissing);
                r.SoftCurrencySinkSourceRatioBasisPoints = 0;
            }
            else r.SoftCurrencySinkSourceRatioBasisPoints = PostLaunchReviewValidation.RateBasisPoints(economy.SoftCurrencySpent, economy.SoftCurrencyEarned);

            long hardNet = economy.HardCurrencyGranted > economy.HardCurrencySpent ? economy.HardCurrencyGranted-economy.HardCurrencySpent : 0;
            long boosterNet = economy.BoostersGranted > economy.BoostersConsumed ? economy.BoostersGranted-economy.BoostersConsumed : 0;
            r.HardCurrencyNetGrantPerActiveUser = economy.ActiveUsers > 0 ? ClampInt(hardNet/economy.ActiveUsers) : 0;
            r.BoosterNetGrantPerActiveUser = economy.ActiveUsers > 0 ? ClampInt(boosterNet/economy.ActiveUsers) : 0;
            r.RewardGrantRelativeIncreaseBasisPoints = economy.BaselineRewardUnitsGranted > 0 && economy.CurrentRewardUnitsGranted > economy.BaselineRewardUnitsGranted
                ? PostLaunchReviewValidation.RateBasisPoints(economy.CurrentRewardUnitsGranted-economy.BaselineRewardUnitsGranted, economy.BaselineRewardUnitsGranted) : 0;

            if (economy.SoftCurrencyEarned > 0 && r.SoftCurrencySinkSourceRatioBasisPoints < profile.MinSoftCurrencySinkSourceRatioBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceLow);
            if (economy.SoftCurrencyEarned > 0 && r.SoftCurrencySinkSourceRatioBasisPoints > profile.MaxSoftCurrencySinkSourceRatioBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceHigh);
            if (r.HardCurrencyNetGrantPerActiveUser > profile.MaxHardCurrencyNetGrantPerActiveUser) blockers.Add(PostLaunchReviewBlockerCodes.HardCurrencyNetGrantHigh);
            if (r.BoosterNetGrantPerActiveUser > profile.MaxBoosterNetGrantPerActiveUser) blockers.Add(PostLaunchReviewBlockerCodes.BoosterNetGrantHigh);
            if (r.RewardGrantRelativeIncreaseBasisPoints > profile.MaxRewardGrantRelativeIncreaseBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.RewardGrantDeltaHigh);

            r.EconomyHealthy = r.SampleSufficient && !ContainsEconomyGuardrail(blockers);
            r.Blockers = blockers.ToArray();
            return r;
        }

        static bool ContainsEconomyGuardrail(List<string> b)
        {
            return b.Contains(PostLaunchReviewBlockerCodes.SoftCurrencyFlowMissing) || b.Contains(PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceLow) || b.Contains(PostLaunchReviewBlockerCodes.SoftCurrencySinkSourceHigh) || b.Contains(PostLaunchReviewBlockerCodes.HardCurrencyNetGrantHigh) || b.Contains(PostLaunchReviewBlockerCodes.BoosterNetGrantHigh) || b.Contains(PostLaunchReviewBlockerCodes.RewardGrantDeltaHigh);
        }
        static void Add(List<string> dest, string[] values) { for(int i=0;i<values.Length;i++) if(!dest.Contains(values[i])) dest.Add(values[i]); }
        static int ClampInt(long value) { return value > int.MaxValue ? int.MaxValue : (int)value; }
    }
}
