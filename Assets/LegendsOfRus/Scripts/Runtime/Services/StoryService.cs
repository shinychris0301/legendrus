namespace LegendsOfRus
{
    public enum StoryAdvanceResult
    {
        None,
        MoreLines,
        PreLevel,
        ChapterMap,
        ChapterComplete
    }

    public static class StoryService
    {
        static StorySequenceRuntime activeSequence;

        public static LocalizedStoryLine CurrentLine => activeSequence?.CurrentLine;
        public static string ActiveSceneId => activeSequence?.Scene?.id ?? string.Empty;

        public static void ResetRuntime()
        {
            activeSequence = null;
        }

        public static bool TryBeginChapterIntro()
        {
            var state = GameState.Instance;
            if (state == null) return false;

            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (catalog == null || string.IsNullOrWhiteSpace(catalog.introSceneId))
                return false;

            return TryBeginScene(StoryCatalog.FindScene(catalog.introSceneId));
        }

        public static bool ShouldShowChapterIntro()
        {
            GameState state = GameState.Instance;
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            return
                state != null &&
                catalog != null &&
                !string.IsNullOrWhiteSpace(catalog.introSceneId) &&
                !state.HasCompletedStoryScene(catalog.introSceneId);
        }

        public static bool TryBeginPostVictoryDialogue()
        {
            return TryBeginScene(FindPostVictoryScene());
        }

        public static bool ShouldShowDialogueAfterVictory()
        {
            GameState state = GameState.Instance;
            if (state == null) return false;

            LevelData completed = ChapterLevelCatalog.Find(
                state.Save.lastCompletedChapter,
                state.Save.lastCompletedLevel
            );
            return completed != null && completed.HasStoryTriggerPrefix("dialogue.");
        }

        public static LocalizedStoryReaction GetLocalizedReaction(
            LevelData level,
            string outcome
        )
        {
            if (level == null || !StoryReactionOutcomes.IsValid(outcome))
                return null;

            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (catalog == null || level.chapterId != catalog.chapterId)
                return null;

            StoryReactionData reaction = StoryCatalog.FindReaction(level.levelId, outcome);
            return StoryCatalog.LocalizeReaction(reaction, StoryCatalog.LoadRussian());
        }

        public static LocalizedChapterCompletion GetLocalizedChapterCompletion()
        {
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            return StoryCatalog.LocalizeCompletion(
                catalog?.completion,
                StoryCatalog.LoadRussian()
            );
        }

        public static ChapterCompletionSummary GetChapterCompletionSummary()
        {
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            return catalog != null
                ? GameState.Instance?.GetChapterCompletionSummary(catalog.chapterId)
                : null;
        }

        static StorySceneData FindPostVictoryScene()
        {
            GameState state = GameState.Instance;
            if (state == null) return null;

            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (catalog?.milestones == null) return null;

            for (int i = 0; i < catalog.milestones.Length; i++)
            {
                StoryMilestoneData milestone = catalog.milestones[i];
                if (milestone == null) continue;

                LevelProgressData progress = state.GetLevelProgress(
                    catalog.chapterId,
                    milestone.afterLevelId
                );
                if (progress == null || progress.completions <= 0) continue;

                StorySceneData scene = StoryCatalog.FindScene(milestone.sceneId);
                LevelData completed = ChapterLevelCatalog.Find(
                    catalog.chapterId,
                    milestone.afterLevelId
                );
                if (
                    scene != null &&
                    completed != null &&
                    completed.HasStoryTrigger(scene.trigger) &&
                    !state.HasCompletedStoryScene(scene.id)
                )
                {
                    return scene;
                }
            }

            return null;
        }

        static bool TryBeginScene(StorySceneData scene)
        {
            GameState state = GameState.Instance;
            if (state == null || scene == null) return false;

            if (activeSequence != null && !activeSequence.IsComplete)
            {
                return string.Equals(
                    activeSequence.Scene?.id,
                    scene.id,
                    System.StringComparison.Ordinal
                );
            }

            if (state.HasCompletedStoryScene(scene.id)) return false;

            activeSequence = new StorySequenceRuntime(scene, StoryCatalog.LoadRussian());
            if (activeSequence.IsComplete || activeSequence.CurrentLine == null)
            {
                activeSequence = null;
                return false;
            }

            PrivacyAwareAnalytics.TryStorySceneStart(scene.id);
            return true;
        }

        public static StoryAdvanceResult AdvanceDialogue()
        {
            if (activeSequence == null) return StoryAdvanceResult.None;
            if (activeSequence.Advance()) return StoryAdvanceResult.MoreLines;

            StorySceneData completed = activeSequence.Scene;
            activeSequence = null;
            GameState state = GameState.Instance;
            bool completedNow = state != null && state.CompleteStoryScene(completed?.id);
            if (completedNow)
            {
                PrivacyAwareAnalytics.TryStorySceneComplete(
                    completed?.id ?? string.Empty
                );
            }

            switch (completed?.returnRoute)
            {
                case StoryReturnRoutes.PreLevel:
                    return StoryAdvanceResult.PreLevel;
                case StoryReturnRoutes.ChapterComplete:
                    ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
                    if (
                        state != null &&
                        catalog?.completion != null &&
                        string.Equals(
                            completed?.id,
                            catalog.completion.finaleSceneId,
                            System.StringComparison.Ordinal
                        ) &&
                        state.HasPendingChapterCompletion(catalog.chapterId)
                    )
                    {
                        if (completedNow)
                            PrivacyAwareAnalytics.TryChapterComplete(catalog.chapterId);
                        return StoryAdvanceResult.ChapterComplete;
                    }

                    UnityEngine.Debug.LogError(
                        "Rejected ChapterComplete route without a completed final level and finale scene."
                    );
                    return StoryAdvanceResult.ChapterMap;
                case StoryReturnRoutes.ChapterMap:
                    return StoryAdvanceResult.ChapterMap;
                default:
                    return StoryAdvanceResult.None;
            }
        }

        public static bool ShouldShowChapterComplete()
        {
            GameState state = GameState.Instance;
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            return
                state != null &&
                catalog != null &&
                state.HasPendingChapterCompletion(catalog.chapterId);
        }

        public static bool AcknowledgeChapterComplete()
        {
            GameState state = GameState.Instance;
            ChapterStoryCatalogData catalog = StoryCatalog.LoadChapter1();
            if (state == null || catalog == null) return false;

            bool acknowledged = state.AcknowledgeChapterCompletion(catalog.chapterId);
            if (acknowledged)
            {
                PrivacyAwareAnalytics.TryChapterCompleteAcknowledged(
                    catalog.chapterId
                );
            }
            return acknowledged;
        }
    }
}
