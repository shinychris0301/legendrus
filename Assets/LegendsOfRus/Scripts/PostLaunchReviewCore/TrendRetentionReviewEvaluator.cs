using System.Collections.Generic;

namespace LegendsOfRus.PostLaunchReview
{
    public static class TrendRetentionReviewEvaluator
    {
        public static TrendRetentionReviewResult Evaluate(ReviewedGuardrailProfile profile, TrendWindowEvidence trend, RetentionCohortEvidence retention)
        {
            var blockers = new List<string>();
            Add(blockers, PostLaunchReviewValidation.ValidateProfile(profile));
            Add(blockers, PostLaunchReviewValidation.ValidateTrend(trend));
            Add(blockers, PostLaunchReviewValidation.ValidateRetention(retention));
            bool structural = blockers.Count == 0;
            var r = new TrendRetentionReviewResult { StructurallyValid = structural };
            if (!structural) { r.Blockers = blockers.ToArray(); return r; }

            bool enoughTrend = trend.BaselineActiveUsers >= profile.MinimumActiveUsers && trend.CurrentActiveUsers >= profile.MinimumActiveUsers;
            bool enoughRetention = retention.CohortSize >= profile.MinimumCohortSize;
            r.SampleSufficient = enoughTrend && enoughRetention;
            if (!enoughTrend) blockers.Add(PostLaunchReviewBlockerCodes.TrendSampleInsufficient);
            if (!enoughRetention) blockers.Add(PostLaunchReviewBlockerCodes.RetentionSampleInsufficient);

            r.ActiveUserRelativeDeclineBasisPoints = PostLaunchReviewValidation.PositiveRelativeDeclineBasisPoints(trend.BaselineActiveUsers, trend.CurrentActiveUsers);
            int baselineSessionsPerUser = PostLaunchReviewValidation.RateBasisPoints(trend.BaselineSessions, trend.BaselineActiveUsers);
            int currentSessionsPerUser = PostLaunchReviewValidation.RateBasisPoints(trend.CurrentSessions, trend.CurrentActiveUsers);
            r.SessionsPerUserRelativeDeclineBasisPoints = PostLaunchReviewValidation.PositiveRelativeDeclineBasisPoints(baselineSessionsPerUser, currentSessionsPerUser);
            int baselineCompletion = PostLaunchReviewValidation.RateBasisPoints(trend.BaselineLevelCompletes, trend.BaselineLevelStarts);
            int currentCompletion = PostLaunchReviewValidation.RateBasisPoints(trend.CurrentLevelCompletes, trend.CurrentLevelStarts);
            r.CompletionRateDropBasisPoints = PostLaunchReviewValidation.PositiveDrop(baselineCompletion, currentCompletion);

            r.D1BasisPoints = PostLaunchReviewValidation.RateBasisPoints(retention.D1Returned, retention.CohortSize);
            r.D7BasisPoints = PostLaunchReviewValidation.RateBasisPoints(retention.D7Returned, retention.CohortSize);
            r.D30BasisPoints = PostLaunchReviewValidation.RateBasisPoints(retention.D30Returned, retention.CohortSize);

            if (r.ActiveUserRelativeDeclineBasisPoints > profile.MaxActiveUserRelativeDeclineBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.ActiveUsersDeclined);
            if (r.SessionsPerUserRelativeDeclineBasisPoints > profile.MaxSessionsPerUserRelativeDeclineBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.SessionsPerUserDeclined);
            if (r.CompletionRateDropBasisPoints > profile.MaxCompletionRateDropBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.CompletionRateDeclined);
            if (PostLaunchReviewValidation.PositiveDrop(retention.BaselineD1BasisPoints, r.D1BasisPoints) > profile.MaxD1RetentionDropBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.D1RetentionDeclined);
            if (PostLaunchReviewValidation.PositiveDrop(retention.BaselineD7BasisPoints, r.D7BasisPoints) > profile.MaxD7RetentionDropBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.D7RetentionDeclined);
            if (PostLaunchReviewValidation.PositiveDrop(retention.BaselineD30BasisPoints, r.D30BasisPoints) > profile.MaxD30RetentionDropBasisPoints) blockers.Add(PostLaunchReviewBlockerCodes.D30RetentionDeclined);

            r.TrendHealthy = r.SampleSufficient && !ContainsAny(blockers, PostLaunchReviewBlockerCodes.ActiveUsersDeclined, PostLaunchReviewBlockerCodes.SessionsPerUserDeclined, PostLaunchReviewBlockerCodes.CompletionRateDeclined);
            r.RetentionHealthy = r.SampleSufficient && !ContainsAny(blockers, PostLaunchReviewBlockerCodes.D1RetentionDeclined, PostLaunchReviewBlockerCodes.D7RetentionDeclined, PostLaunchReviewBlockerCodes.D30RetentionDeclined);
            r.Blockers = blockers.ToArray();
            return r;
        }

        static void Add(List<string> dest, string[] values) { for(int i=0;i<values.Length;i++) if(!dest.Contains(values[i])) dest.Add(values[i]); }
        static bool ContainsAny(List<string> values, params string[] codes) { for(int i=0;i<codes.Length;i++) if(values.Contains(codes[i])) return true; return false; }
    }
}
