using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    public sealed class StoryLineData
    {
        public string speakerKey = string.Empty;
        public string textKey = string.Empty;
    }

    [Serializable]
    public sealed class StorySceneData
    {
        public string id = string.Empty;
        public string trigger = string.Empty;
        public string returnRoute = StoryReturnRoutes.ChapterMap;
        public StoryLineData[] lines = Array.Empty<StoryLineData>();
    }

    [Serializable]
    public sealed class StoryMilestoneData
    {
        public string id = string.Empty;
        public int afterLevelId;
        public string sceneId = string.Empty;
        public bool major;
    }

    [Serializable]
    public sealed class StoryReactionData
    {
        public string id = string.Empty;
        public int levelId;
        public string outcome = string.Empty;
        public string speakerKey = string.Empty;
        public string textKey = string.Empty;
    }

    [Serializable]
    public sealed class StoryChapterCompletionData
    {
        public int finalLevelId;
        public string finaleSceneId = string.Empty;
        public string completionTrigger = string.Empty;
        public string summaryTitleKey = string.Empty;
        public string summaryFormatKey = string.Empty;
        public string rewardNoteKey = string.Empty;
    }

    [Serializable]
    public sealed class ChapterStoryCatalogData
    {
        public int schemaVersion = 3;
        public int chapterId = 1;
        public string introSceneId = string.Empty;
        public StorySceneData[] scenes = Array.Empty<StorySceneData>();
        public StoryMilestoneData[] milestones = Array.Empty<StoryMilestoneData>();
        public StoryReactionData[] reactions = Array.Empty<StoryReactionData>();
        public StoryChapterCompletionData completion = new StoryChapterCompletionData();
    }

    [Serializable]
    public sealed class StoryLocalizationEntry
    {
        public string key = string.Empty;
        public string value = string.Empty;
    }

    [Serializable]
    public sealed class StoryLocalizationData
    {
        public int schemaVersion = 1;
        public string locale = "ru";
        public StoryLocalizationEntry[] entries = Array.Empty<StoryLocalizationEntry>();
    }

    public sealed class LocalizedStoryLine
    {
        public string Speaker { get; }
        public string Text { get; }

        public LocalizedStoryLine(string speaker, string text)
        {
            Speaker = speaker ?? string.Empty;
            Text = text ?? string.Empty;
        }
    }

    public sealed class LocalizedStoryReaction
    {
        public string Id { get; }
        public int LevelId { get; }
        public string Outcome { get; }
        public string Speaker { get; }
        public string Text { get; }

        public LocalizedStoryReaction(
            string id,
            int levelId,
            string outcome,
            string speaker,
            string text
        )
        {
            Id = id ?? string.Empty;
            LevelId = levelId;
            Outcome = outcome ?? string.Empty;
            Speaker = speaker ?? string.Empty;
            Text = text ?? string.Empty;
        }
    }

    public sealed class LocalizedChapterCompletion
    {
        public string Title { get; }
        public string SummaryFormat { get; }
        public string RewardNote { get; }

        public LocalizedChapterCompletion(
            string title,
            string summaryFormat,
            string rewardNote
        )
        {
            Title = title ?? string.Empty;
            SummaryFormat = summaryFormat ?? string.Empty;
            RewardNote = rewardNote ?? string.Empty;
        }
    }

    public static class StoryReactionOutcomes
    {
        public const string Victory = "victory";
        public const string Failure = "failure";

        public static bool IsValid(string outcome)
        {
            return
                string.Equals(outcome, Victory, StringComparison.Ordinal) ||
                string.Equals(outcome, Failure, StringComparison.Ordinal);
        }
    }

    public static class StoryReturnRoutes
    {
        public const string PreLevel = "PreLevel";
        public const string ChapterMap = "ChapterMap";
        public const string ChapterComplete = "ChapterComplete";

        public static bool IsValid(string route)
        {
            return
                string.Equals(route, PreLevel, StringComparison.Ordinal) ||
                string.Equals(route, ChapterMap, StringComparison.Ordinal) ||
                string.Equals(route, ChapterComplete, StringComparison.Ordinal);
        }
    }

    public sealed class StoryValidationReport
    {
        public readonly List<string> Errors = new List<string>();
        public bool IsValid => Errors.Count == 0;
    }

    public static class StoryCatalog
    {

        static ChapterStoryCatalogData cachedCatalog;
        static StoryLocalizationData cachedRussian;

        public static ChapterStoryCatalogData LoadChapter1(bool forceReload = false)
        {
            if (forceReload || cachedCatalog == null)
            {
                try
                {
                    cachedCatalog = ActiveCampaignContent.Load(forceReload).LoadStory(1);
                }
                catch (Exception exception)
                {
                    cachedCatalog = null;
                    Debug.LogError($"Invalid Chapter 1 story JSON: {exception.Message}");
                }
            }

            return cachedCatalog;
        }

        public static StoryLocalizationData LoadRussian(bool forceReload = false)
        {
            if (forceReload || cachedRussian == null)
            {
                try
                {
                    cachedRussian = ActiveCampaignContent.Load(forceReload).LoadLocalization(1, "ru");
                }
                catch (Exception exception)
                {
                    cachedRussian = null;
                    Debug.LogError($"Invalid Russian story localization JSON: {exception.Message}");
                }
            }

            return cachedRussian;
        }

        static StoryLocalizationData cachedEnglish;
        public static StoryLocalizationData LoadDialogueLanguage(string language)
        {
            if (language != "en") return LoadRussian();
            if (cachedEnglish == null)
            {
                cachedEnglish = ActiveCampaignContent.Load().LoadLocalization(1, "en");
            }
            return cachedEnglish ?? LoadRussian();
        }

        public static StorySceneData FindScene(string sceneId)
        {
            ChapterStoryCatalogData catalog = LoadChapter1();
            if (catalog?.scenes == null || string.IsNullOrWhiteSpace(sceneId))
                return null;

            for (int i = 0; i < catalog.scenes.Length; i++)
            {
                StorySceneData scene = catalog.scenes[i];
                if (scene != null && string.Equals(scene.id, sceneId, StringComparison.Ordinal))
                    return scene;
            }

            return null;
        }

        public static StorySceneData FindByTrigger(string trigger)
        {
            ChapterStoryCatalogData catalog = LoadChapter1();
            if (catalog?.scenes == null || string.IsNullOrWhiteSpace(trigger))
                return null;

            for (int i = 0; i < catalog.scenes.Length; i++)
            {
                StorySceneData scene = catalog.scenes[i];
                if (scene != null && string.Equals(scene.trigger, trigger, StringComparison.Ordinal))
                    return scene;
            }

            return null;
        }

        public static StoryMilestoneData FindMilestoneAfterLevel(int levelId)
        {
            ChapterStoryCatalogData catalog = LoadChapter1();
            if (catalog?.milestones == null || levelId <= 0) return null;

            for (int i = 0; i < catalog.milestones.Length; i++)
            {
                StoryMilestoneData milestone = catalog.milestones[i];
                if (milestone != null && milestone.afterLevelId == levelId)
                    return milestone;
            }

            return null;
        }

        public static StoryReactionData FindReaction(int levelId, string outcome)
        {
            ChapterStoryCatalogData catalog = LoadChapter1();
            if (
                catalog?.reactions == null ||
                levelId <= 0 ||
                !StoryReactionOutcomes.IsValid(outcome)
            )
            {
                return null;
            }

            for (int i = 0; i < catalog.reactions.Length; i++)
            {
                StoryReactionData reaction = catalog.reactions[i];
                if (
                    reaction != null &&
                    reaction.levelId == levelId &&
                    string.Equals(reaction.outcome, outcome, StringComparison.Ordinal)
                )
                {
                    return reaction;
                }
            }

            return null;
        }

        public static LocalizedStoryReaction LocalizeReaction(
            StoryReactionData reaction,
            StoryLocalizationData localization = null
        )
        {
            if (reaction == null) return null;
            localization = localization ?? LoadRussian();
            string speaker = Localize(reaction.speakerKey, localization);
            string body = Localize(reaction.textKey, localization);
            if (string.IsNullOrWhiteSpace(speaker) || string.IsNullOrWhiteSpace(body))
                return null;

            return new LocalizedStoryReaction(
                reaction.id,
                reaction.levelId,
                reaction.outcome,
                speaker,
                body
            );
        }

        public static LocalizedChapterCompletion LocalizeCompletion(
            StoryChapterCompletionData completion = null,
            StoryLocalizationData localization = null
        )
        {
            ChapterStoryCatalogData catalog = LoadChapter1();
            completion = completion ?? catalog?.completion;
            if (completion == null) return null;

            localization = localization ?? LoadRussian();
            string title = Localize(completion.summaryTitleKey, localization);
            string summary = Localize(completion.summaryFormatKey, localization);
            string rewardNote = Localize(completion.rewardNoteKey, localization);
            if (
                string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(summary) ||
                string.IsNullOrWhiteSpace(rewardNote)
            )
            {
                return null;
            }

            return new LocalizedChapterCompletion(title, summary, rewardNote);
        }

        public static string Localize(string key, StoryLocalizationData localization = null)
        {
            localization = localization ?? LoadRussian();
            if (localization?.entries == null || string.IsNullOrWhiteSpace(key))
                return string.Empty;

            for (int i = 0; i < localization.entries.Length; i++)
            {
                StoryLocalizationEntry entry = localization.entries[i];
                if (entry != null && string.Equals(entry.key, key, StringComparison.Ordinal))
                    return entry.value ?? string.Empty;
            }

            return string.Empty;
        }

        public static StoryValidationReport Validate(
            ChapterStoryCatalogData catalog,
            StoryLocalizationData localization,
            ChapterLevelCatalogData levels = null
        )
        {
            return ValidateCore(catalog, localization, levels, true, true);
        }

        public static StoryValidationReport ValidateCampaign(ChapterStoryCatalogData catalog,
            StoryLocalizationData localization, ChapterLevelCatalogData levels, bool finaleReady)
        {
            var report = ValidateCore(catalog, localization, levels, false, finaleReady);
            if (catalog != null && (catalog.chapterId < 1 || catalog.chapterId > 20))
                report.Errors.Add("Campaign story chapter must be 1–20.");
            if (levels == null) report.Errors.Add("Campaign story requires its level catalog.");
            if (finaleReady && (levels?.levels == null || levels.levels.Length != 500 || catalog?.completion?.finalLevelId != 500))
                report.Errors.Add("Campaign finale requires round 500.");
            if (!finaleReady && catalog != null)
            {
                if (catalog.completion != null && catalog.completion.finalLevelId != 0)
                    report.Errors.Add("Partial campaign cannot declare chapter completion.");
                foreach (var scene in catalog.scenes ?? Array.Empty<StorySceneData>())
                    if (scene != null && scene.returnRoute == StoryReturnRoutes.ChapterComplete)
                        report.Errors.Add("Partial campaign cannot route to ChapterComplete.");
                foreach (var level in levels?.levels ?? Array.Empty<LevelData>())
                    if (level != null && level.HasStoryTrigger("chapter.complete"))
                        report.Errors.Add("Partial campaign cannot contain a chapter.complete trigger.");
            }
            return report;
        }

        static StoryValidationReport ValidateCore(ChapterStoryCatalogData catalog,
            StoryLocalizationData localization, ChapterLevelCatalogData levels, bool legacy, bool finaleReady)
        {
            var report = new StoryValidationReport();
            if (catalog == null)
            {
                report.Errors.Add("Story catalog is missing or invalid JSON.");
                return report;
            }
            if (localization == null)
            {
                report.Errors.Add("Russian story localization is missing or invalid JSON.");
                return report;
            }
            if (catalog.schemaVersion != 3)
                report.Errors.Add("Story catalog schemaVersion must be 3.");
            if (legacy && catalog.chapterId != 1)
                report.Errors.Add("Story catalog chapterId must be 1.");
            if (levels != null && levels.chapterId != catalog.chapterId)
                report.Errors.Add("Story and level catalogs must use the same chapterId.");
            if (localization.schemaVersion != 1)
                report.Errors.Add("Story localization schemaVersion must be 1.");
            if (legacy ? localization.locale != "ru" : (localization.locale != "ru" && localization.locale != "en"))
                report.Errors.Add("Story localization locale must be 'ru'.");

            var localizedKeys = new HashSet<string>(StringComparer.Ordinal);
            if (localization.entries == null || localization.entries.Length == 0)
            {
                report.Errors.Add("Story localization must contain entries.");
            }
            else
            {
                for (int i = 0; i < localization.entries.Length; i++)
                {
                    StoryLocalizationEntry entry = localization.entries[i];
                    if (entry == null || string.IsNullOrWhiteSpace(entry.key))
                    {
                        report.Errors.Add($"Localization entry {i} has no key.");
                        continue;
                    }
                    if (!localizedKeys.Add(entry.key))
                        report.Errors.Add($"Localization key '{entry.key}' is duplicated.");
                    if (string.IsNullOrWhiteSpace(entry.value))
                        report.Errors.Add($"Localization key '{entry.key}' has no value.");
                }
            }

            var sceneIds = new HashSet<string>(StringComparer.Ordinal);
            var scenesById = new Dictionary<string, StorySceneData>(StringComparer.Ordinal);
            var triggers = new HashSet<string>(StringComparer.Ordinal);
            if (catalog.scenes == null || catalog.scenes.Length == 0)
            {
                report.Errors.Add("Story catalog must contain scenes.");
            }
            else
            {
                for (int i = 0; i < catalog.scenes.Length; i++)
                {
                    StorySceneData scene = catalog.scenes[i];
                    if (scene == null || string.IsNullOrWhiteSpace(scene.id))
                    {
                        report.Errors.Add($"Story scene {i} has no id.");
                        continue;
                    }
                    if (!sceneIds.Add(scene.id))
                        report.Errors.Add($"Story scene id '{scene.id}' is duplicated.");
                    else
                        scenesById.Add(scene.id, scene);
                    if (!string.IsNullOrWhiteSpace(scene.trigger) && !triggers.Add(scene.trigger))
                        report.Errors.Add($"Story trigger '{scene.trigger}' is duplicated.");
                    if (!StoryReturnRoutes.IsValid(scene.returnRoute))
                        report.Errors.Add($"Scene '{scene.id}' has invalid returnRoute '{scene.returnRoute}'.");
                    if (scene.lines == null || scene.lines.Length == 0)
                    {
                        report.Errors.Add($"Scene '{scene.id}' must contain lines.");
                        continue;
                    }

                    for (int lineIndex = 0; lineIndex < scene.lines.Length; lineIndex++)
                    {
                        StoryLineData line = scene.lines[lineIndex];
                        if (line == null)
                        {
                            report.Errors.Add($"Scene '{scene.id}' line {lineIndex} is null.");
                            continue;
                        }
                        ValidateLocalizedKey(report, localizedKeys, scene.id, lineIndex, "speakerKey", line.speakerKey);
                        ValidateLocalizedKey(report, localizedKeys, scene.id, lineIndex, "textKey", line.textKey);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(catalog.introSceneId) || !sceneIds.Contains(catalog.introSceneId))
                report.Errors.Add("introSceneId must reference an existing scene.");

            Dictionary<int, StoryMilestoneData> milestonesByLevel = ValidateMilestones(
                report,
                catalog,
                scenesById,
                levels,
                legacy
            );
            ValidateReactions(report, catalog, localizedKeys, levels);
            if (finaleReady) ValidateCompletion(
                report,
                catalog,
                localization,
                localizedKeys,
                scenesById,
                milestonesByLevel,
                levels
            );

            if (levels?.levels != null)
            {
                for (int i = 0; i < levels.levels.Length; i++)
                {
                    LevelData level = levels.levels[i];
                    if (level?.storyTriggers == null) continue;
                    for (int triggerIndex = 0; triggerIndex < level.storyTriggers.Length; triggerIndex++)
                    {
                        string trigger = level.storyTriggers[triggerIndex];
                        if (
                            !string.IsNullOrWhiteSpace(trigger) &&
                            trigger.StartsWith("dialogue.", StringComparison.Ordinal) &&
                            !triggers.Contains(trigger)
                        )
                        {
                            report.Errors.Add($"Level {level.levelId} dialogue trigger '{trigger}' has no story scene.");
                        }
                        else if (
                            !string.IsNullOrWhiteSpace(trigger) &&
                            trigger.StartsWith("dialogue.", StringComparison.Ordinal)
                        )
                        {
                            if (!milestonesByLevel.TryGetValue(level.levelId, out StoryMilestoneData milestone))
                            {
                                report.Errors.Add($"Level {level.levelId} dialogue trigger '{trigger}' has no deterministic milestone mapping.");
                            }
                            else if (
                                string.IsNullOrWhiteSpace(milestone.sceneId) ||
                                !scenesById.TryGetValue(milestone.sceneId, out StorySceneData mappedScene) ||
                                !string.Equals(mappedScene.trigger, trigger, StringComparison.Ordinal)
                            )
                            {
                                report.Errors.Add($"Level {level.levelId} milestone does not map dialogue trigger '{trigger}'.");
                            }
                        }
                    }
                }
            }

            return report;
        }

        static Dictionary<int, StoryMilestoneData> ValidateMilestones(
            StoryValidationReport report,
            ChapterStoryCatalogData catalog,
            Dictionary<string, StorySceneData> scenesById,
            ChapterLevelCatalogData levels,
            bool legacy
        )
        {
            var byLevel = new Dictionary<int, StoryMilestoneData>();
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var mappedScenes = new HashSet<string>(StringComparer.Ordinal);
            var majorLevels = new List<int>();
            int previousLevel = 0;

            if (catalog.milestones == null || catalog.milestones.Length == 0)
            {
                report.Errors.Add("Story catalog must contain post-victory milestones.");
                return byLevel;
            }

            for (int i = 0; i < catalog.milestones.Length; i++)
            {
                StoryMilestoneData milestone = catalog.milestones[i];
                if (milestone == null || string.IsNullOrWhiteSpace(milestone.id))
                {
                    report.Errors.Add($"Story milestone {i} has no id.");
                    continue;
                }
                if (!ids.Add(milestone.id))
                    report.Errors.Add($"Story milestone id '{milestone.id}' is duplicated.");
                if (milestone.afterLevelId <= 0)
                    report.Errors.Add($"Milestone '{milestone.id}' has invalid afterLevelId.");
                else if (byLevel.ContainsKey(milestone.afterLevelId))
                    report.Errors.Add($"Level {milestone.afterLevelId} has multiple story milestones.");
                else
                    byLevel.Add(milestone.afterLevelId, milestone);
                if (milestone.afterLevelId <= previousLevel)
                    report.Errors.Add("Story milestones must be ordered by ascending afterLevelId.");
                previousLevel = Math.Max(previousLevel, milestone.afterLevelId);

                if (string.IsNullOrWhiteSpace(milestone.sceneId))
                {
                    report.Errors.Add($"Milestone '{milestone.id}' has no sceneId.");
                }
                else if (!scenesById.TryGetValue(milestone.sceneId, out StorySceneData scene))
                {
                    report.Errors.Add($"Milestone '{milestone.id}' references missing scene '{milestone.sceneId}'.");
                }
                else
                {
                    if (!mappedScenes.Add(scene.id))
                        report.Errors.Add($"Story scene '{scene.id}' is mapped by multiple milestones.");
                    if (
                        string.IsNullOrWhiteSpace(scene.trigger) ||
                        !scene.trigger.StartsWith("dialogue.", StringComparison.Ordinal)
                    )
                    {
                        report.Errors.Add($"Milestone '{milestone.id}' must reference a dialogue-triggered scene.");
                    }

                    LevelData level = FindLevel(levels, milestone.afterLevelId);
                    if (levels?.levels != null && level == null)
                        report.Errors.Add($"Milestone '{milestone.id}' references missing level {milestone.afterLevelId}.");
                    else if (level != null && !level.HasStoryTrigger(scene.trigger))
                        report.Errors.Add($"Milestone '{milestone.id}' scene trigger is not authored on level {milestone.afterLevelId}.");
                }

                if (milestone.major)
                    majorLevels.Add(milestone.afterLevelId);
            }

            if (legacy && string.Join(",", majorLevels) != "1,3,6,10")
                report.Errors.Add("Major Chapter 1 story milestones must be levels 1, 3, 6, and 10.");

            foreach (KeyValuePair<string, StorySceneData> pair in scenesById)
            {
                StorySceneData scene = pair.Value;
                if (
                    !string.IsNullOrWhiteSpace(scene.trigger) &&
                    scene.trigger.StartsWith("dialogue.", StringComparison.Ordinal) &&
                    !mappedScenes.Contains(scene.id)
                )
                {
                    report.Errors.Add($"Dialogue scene '{scene.id}' has no deterministic milestone mapping.");
                }
            }

            return byLevel;
        }

        static void ValidateReactions(
            StoryValidationReport report,
            ChapterStoryCatalogData catalog,
            HashSet<string> localizedKeys,
            ChapterLevelCatalogData levels
        )
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            var pairs = new HashSet<string>(StringComparer.Ordinal);
            if (catalog.reactions == null || catalog.reactions.Length == 0)
            {
                report.Errors.Add("Story catalog must contain outcome reactions.");
                return;
            }

            for (int i = 0; i < catalog.reactions.Length; i++)
            {
                StoryReactionData reaction = catalog.reactions[i];
                if (reaction == null || string.IsNullOrWhiteSpace(reaction.id))
                {
                    report.Errors.Add($"Story reaction {i} has no id.");
                    continue;
                }
                if (!ids.Add(reaction.id))
                    report.Errors.Add($"Story reaction id '{reaction.id}' is duplicated.");
                if (FindLevel(levels, reaction.levelId) == null && levels?.levels != null)
                    report.Errors.Add($"Reaction '{reaction.id}' references missing level {reaction.levelId}.");
                if (!StoryReactionOutcomes.IsValid(reaction.outcome))
                    report.Errors.Add($"Reaction '{reaction.id}' has invalid outcome '{reaction.outcome}'.");
                string pair = $"{reaction.levelId}:{reaction.outcome}";
                if (!pairs.Add(pair))
                    report.Errors.Add($"Reaction pair '{pair}' is duplicated.");
                ValidateReactionKey(report, localizedKeys, reaction.id, "speakerKey", reaction.speakerKey);
                ValidateReactionKey(report, localizedKeys, reaction.id, "textKey", reaction.textKey);
            }

            if (levels?.levels == null) return;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                LevelData level = levels.levels[i];
                if (level == null) continue;
                if (!pairs.Contains($"{level.levelId}:{StoryReactionOutcomes.Victory}"))
                    report.Errors.Add($"Level {level.levelId} has no victory reaction.");
                if (!pairs.Contains($"{level.levelId}:{StoryReactionOutcomes.Failure}"))
                    report.Errors.Add($"Level {level.levelId} has no failure reaction.");
            }
        }

        static void ValidateCompletion(
            StoryValidationReport report,
            ChapterStoryCatalogData catalog,
            StoryLocalizationData localization,
            HashSet<string> localizedKeys,
            Dictionary<string, StorySceneData> scenesById,
            Dictionary<int, StoryMilestoneData> milestonesByLevel,
            ChapterLevelCatalogData levels
        )
        {
            StoryChapterCompletionData completion = catalog.completion;
            if (completion == null)
            {
                report.Errors.Add("Story catalog must contain Chapter Complete presentation data.");
                return;
            }

            if (completion.finalLevelId <= 0)
                report.Errors.Add("Chapter Complete finalLevelId must be positive.");
            if (!string.Equals(completion.completionTrigger, "chapter.complete", StringComparison.Ordinal))
                report.Errors.Add("Chapter Complete trigger must be 'chapter.complete'.");

            LevelData finalLevel = FindLevel(levels, completion.finalLevelId);
            if (levels?.levels != null)
            {
                if (finalLevel == null)
                {
                    report.Errors.Add($"Chapter Complete references missing level {completion.finalLevelId}.");
                }
                else
                {
                    int terminalLevelId = 0;
                    int completionTriggerCount = 0;
                    for (int i = 0; i < levels.levels.Length; i++)
                    {
                        LevelData level = levels.levels[i];
                        if (level == null) continue;
                        terminalLevelId = Math.Max(terminalLevelId, level.levelId);
                        if (level.HasStoryTrigger(completion.completionTrigger))
                            completionTriggerCount++;
                    }

                    if (completion.finalLevelId != terminalLevelId)
                        report.Errors.Add("Chapter Complete finalLevelId must be the terminal released level.");
                    if (!finalLevel.HasStoryTrigger(completion.completionTrigger))
                        report.Errors.Add("The final level must contain the Chapter Complete trigger.");
                    if (completionTriggerCount != 1)
                        report.Errors.Add("Exactly one released level must contain the Chapter Complete trigger.");
                }
            }

            StorySceneData finale = null;
            if (string.IsNullOrWhiteSpace(completion.finaleSceneId))
            {
                report.Errors.Add("Chapter Complete finaleSceneId is required.");
            }
            else if (!scenesById.TryGetValue(completion.finaleSceneId, out finale))
            {
                report.Errors.Add($"Chapter Complete references missing finale scene '{completion.finaleSceneId}'.");
            }
            else
            {
                if (!string.Equals(finale.returnRoute, StoryReturnRoutes.ChapterComplete, StringComparison.Ordinal))
                    report.Errors.Add("The finale scene must return to ChapterComplete.");
                if (finale.lines == null || finale.lines.Length < 4)
                    report.Errors.Add("The finale presentation must contain at least four localized lines.");
                if (finalLevel != null && !finalLevel.HasStoryTrigger(finale.trigger))
                    report.Errors.Add("The final level must contain the finale dialogue trigger.");
            }

            if (!milestonesByLevel.TryGetValue(completion.finalLevelId, out StoryMilestoneData milestone))
            {
                report.Errors.Add("The final level must have a deterministic story milestone.");
            }
            else if (
                !milestone.major ||
                !string.Equals(milestone.sceneId, completion.finaleSceneId, StringComparison.Ordinal)
            )
            {
                report.Errors.Add("The final level must map a major milestone to the finale scene.");
            }

            int chapterCompleteRoutes = 0;
            foreach (KeyValuePair<string, StorySceneData> pair in scenesById)
            {
                StorySceneData scene = pair.Value;
                if (!string.Equals(scene.returnRoute, StoryReturnRoutes.ChapterComplete, StringComparison.Ordinal))
                    continue;

                chapterCompleteRoutes++;
                if (!string.Equals(scene.id, completion.finaleSceneId, StringComparison.Ordinal))
                    report.Errors.Add($"Scene '{scene.id}' cannot route to ChapterComplete.");
            }
            if (chapterCompleteRoutes != 1)
                report.Errors.Add("Exactly one story scene must route to ChapterComplete.");

            ValidateCompletionKey(report, localizedKeys, "summaryTitleKey", completion.summaryTitleKey);
            ValidateCompletionKey(report, localizedKeys, "summaryFormatKey", completion.summaryFormatKey);
            ValidateCompletionKey(report, localizedKeys, "rewardNoteKey", completion.rewardNoteKey);

            string summaryFormat = Localize(completion.summaryFormatKey, localization);
            if (
                !string.IsNullOrWhiteSpace(summaryFormat) &&
                (!summaryFormat.Contains("{0}") ||
                 !summaryFormat.Contains("{1}") ||
                 !summaryFormat.Contains("{2}") ||
                 !summaryFormat.Contains("{3}"))
            )
            {
                report.Errors.Add("Chapter Complete summary format must contain placeholders {0}, {1}, {2}, and {3}.");
            }
        }

        static LevelData FindLevel(ChapterLevelCatalogData levels, int levelId)
        {
            if (levels?.levels == null) return null;
            for (int i = 0; i < levels.levels.Length; i++)
            {
                LevelData level = levels.levels[i];
                if (level != null && level.levelId == levelId)
                    return level;
            }
            return null;
        }

        static void ValidateReactionKey(
            StoryValidationReport report,
            HashSet<string> localizedKeys,
            string reactionId,
            string field,
            string key
        )
        {
            if (string.IsNullOrWhiteSpace(key))
                report.Errors.Add($"Reaction '{reactionId}' has no {field}.");
            else if (!localizedKeys.Contains(key))
                report.Errors.Add($"Reaction '{reactionId}' references missing localization key '{key}'.");
        }

        static void ValidateCompletionKey(
            StoryValidationReport report,
            HashSet<string> localizedKeys,
            string field,
            string key
        )
        {
            if (string.IsNullOrWhiteSpace(key))
                report.Errors.Add($"Chapter Complete has no {field}.");
            else if (!localizedKeys.Contains(key))
                report.Errors.Add($"Chapter Complete references missing localization key '{key}'.");
        }

        static void ValidateLocalizedKey(
            StoryValidationReport report,
            HashSet<string> localizedKeys,
            string sceneId,
            int lineIndex,
            string field,
            string key
        )
        {
            if (string.IsNullOrWhiteSpace(key))
                report.Errors.Add($"Scene '{sceneId}' line {lineIndex} has no {field}.");
            else if (!localizedKeys.Contains(key))
                report.Errors.Add($"Scene '{sceneId}' line {lineIndex} references missing localization key '{key}'.");
        }
    }

    public sealed class StorySequenceRuntime
    {
        readonly StorySceneData scene;
        readonly StoryLocalizationData localization;
        int lineIndex;

        public StorySceneData Scene => scene;
        public int LineIndex => lineIndex;
        public bool IsComplete { get; private set; }

        public LocalizedStoryLine CurrentLine
        {
            get
            {
                if (IsComplete || scene?.lines == null || lineIndex >= scene.lines.Length)
                    return null;
                StoryLineData line = scene.lines[lineIndex];
                return line == null
                    ? null
                    : new LocalizedStoryLine(
                        StoryCatalog.Localize(line.speakerKey, localization),
                        StoryCatalog.Localize(line.textKey, localization)
                    );
            }
        }

        public string CurrentSpeakerKey => IsComplete || scene?.lines == null || lineIndex >= scene.lines.Length
            ? string.Empty : scene.lines[lineIndex]?.speakerKey ?? string.Empty;

        public LocalizedStoryLine LocalizeCurrentLine(StoryLocalizationData requested)
        {
            if (CurrentLine == null) return null;
            var line = scene.lines[lineIndex];
            string speaker = StoryCatalog.Localize(line.speakerKey, requested);
            string body = StoryCatalog.Localize(line.textKey, requested);
            return string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(body)
                ? CurrentLine : new LocalizedStoryLine(speaker, body);
        }

        public StorySequenceRuntime(StorySceneData scene, StoryLocalizationData localization)
        {
            this.scene = scene;
            this.localization = localization;
            lineIndex = 0;
            IsComplete = scene?.lines == null || scene.lines.Length == 0;
        }

        public bool Advance()
        {
            if (IsComplete) return false;
            if (lineIndex + 1 < scene.lines.Length)
            {
                lineIndex++;
                return true;
            }

            IsComplete = true;
            return false;
        }
    }

    public static class StoryProgress
    {
        public static void Normalize(List<string> completedSceneIds)
        {
            if (completedSceneIds == null) return;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = completedSceneIds.Count - 1; i >= 0; i--)
            {
                string id = completedSceneIds[i];
                if (string.IsNullOrWhiteSpace(id) || !seen.Add(id))
                    completedSceneIds.RemoveAt(i);
            }
        }

        public static bool HasCompleted(List<string> completedSceneIds, string sceneId)
        {
            if (completedSceneIds == null || string.IsNullOrWhiteSpace(sceneId))
                return false;
            for (int i = 0; i < completedSceneIds.Count; i++)
                if (string.Equals(completedSceneIds[i], sceneId, StringComparison.Ordinal))
                    return true;
            return false;
        }

        public static bool Complete(List<string> completedSceneIds, string sceneId)
        {
            if (completedSceneIds == null || string.IsNullOrWhiteSpace(sceneId))
                return false;
            if (HasCompleted(completedSceneIds, sceneId)) return false;
            completedSceneIds.Add(sceneId);
            return true;
        }
    }
}
