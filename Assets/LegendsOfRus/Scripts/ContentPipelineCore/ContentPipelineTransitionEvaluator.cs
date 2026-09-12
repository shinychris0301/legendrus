using System;
using System.Collections.Generic;

namespace LegendsOfRus.ContentPipeline
{
    public static class ContentPipelineTransitionEvaluator
    {
        public static ContentPipelineValidationResult ValidateEvidence(ContentPipelineTransitionEvidence evidence)
        {
            var blockers = new List<string>();
            if (evidence == null)
            {
                blockers.Add(ChapterPipelineBlockerCodes.EvidenceMissing);
                return new ContentPipelineValidationResult(blockers.ToArray());
            }
            if (!evidence.EvidenceRetained) Add(blockers, ChapterPipelineBlockerCodes.EvidenceMissing);
            if (!evidence.EvidenceReviewed) Add(blockers, ChapterPipelineBlockerCodes.EvidenceNotReviewed);
            if (string.IsNullOrWhiteSpace(evidence.TransitionId)) Add(blockers, ChapterPipelineBlockerCodes.TransitionIdMissing);
            if (!ChapterPipelinePolicy.IsSha256(evidence.SourceTreeSha256)) Add(blockers, ChapterPipelineBlockerCodes.SourceHashInvalid);
            if (!evidence.LegacyChapter01LevelsValidated || !evidence.LegacyChapter01StoryValidated || !evidence.LegacyRussianLocalizationValidated)
                Add(blockers, ChapterPipelineBlockerCodes.LegacyValidationMissing);
            if (!evidence.ManifestContractReviewed) Add(blockers, ChapterPipelineBlockerCodes.ManifestReviewMissing);
            if (!evidence.TemplateContractReviewed) Add(blockers, ChapterPipelineBlockerCodes.TemplateReviewMissing);
            if (!evidence.StableIdConventionReviewed) Add(blockers, ChapterPipelineBlockerCodes.StableIdReviewMissing);
            if (!evidence.AddressablesNamespaceReviewed) Add(blockers, ChapterPipelineBlockerCodes.AddressablesReviewMissing);
            if (!evidence.ContentOwnershipReviewed) Add(blockers, ChapterPipelineBlockerCodes.OwnershipReviewMissing);
            if (!evidence.LocalizationKeyConventionReviewed) Add(blockers, ChapterPipelineBlockerCodes.LocalizationReviewMissing);
            if (!evidence.QaAutomationReviewed) Add(blockers, ChapterPipelineBlockerCodes.QaReviewMissing);
            if (!evidence.MigrationPlanReviewed) Add(blockers, ChapterPipelineBlockerCodes.MigrationPlanMissing);
            if (!evidence.LockedArtNonDestructiveBoundaryReviewed) Add(blockers, ChapterPipelineBlockerCodes.LockedArtBoundaryMissing);
            if (evidence.UnresolvedCriticalBlockers > 0) Add(blockers, ChapterPipelineBlockerCodes.CriticalBlockersOpen);
            return new ContentPipelineValidationResult(blockers.ToArray());
        }

        public static ContentPipelineReviewResult Evaluate(
            ChapterPipelineManifest manifest,
            ChapterTemplateDefinition template,
            ContentPipelineTransitionEvidence evidence,
            bool chapterMigrationAuthorized,
            bool addressablesMutationAuthorized,
            bool runtimeCutoverAuthorized,
            bool automaticArtGenerationAuthorized,
            bool lockedArtOverwriteAuthorized)
        {
            var review = new List<string>();
            Append(review, ChapterPipelineValidator.ValidateManifest(manifest).Blockers);
            Append(review, ChapterPipelineValidator.ValidateTemplate(template).Blockers);
            Append(review, ValidateEvidence(evidence).Blockers);

            var actions = new List<string>();
            if (!chapterMigrationAuthorized) actions.Add(ChapterPipelineBlockerCodes.ChapterMigrationNotAuthorized);
            if (!addressablesMutationAuthorized) actions.Add(ChapterPipelineBlockerCodes.AddressablesMutationNotAuthorized);
            if (!runtimeCutoverAuthorized) actions.Add(ChapterPipelineBlockerCodes.RuntimeCutoverNotAuthorized);
            if (!automaticArtGenerationAuthorized) actions.Add(ChapterPipelineBlockerCodes.AutomaticArtGenerationNotAuthorized);
            if (!lockedArtOverwriteAuthorized) actions.Add(ChapterPipelineBlockerCodes.LockedArtOverwriteNotAuthorized);

            return new ContentPipelineReviewResult(review.ToArray(), actions.ToArray());
        }

        static void Append(List<string> destination, string[] source)
        {
            if (source == null) return;
            for (int i = 0; i < source.Length; i++)
                if (!destination.Contains(source[i])) destination.Add(source[i]);
        }

        static void Add(List<string> blockers, string value)
        {
            if (!blockers.Contains(value)) blockers.Add(value);
        }
    }
}
