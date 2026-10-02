using System;
using System.Collections.Generic;

namespace LegendsOfRus
{
    /// <summary>
    /// Validates the authored Chapter 1 teaching order and its declared
    /// difficulty curve without coupling the reusable level schema to one campaign.
    /// </summary>
    public static class Chapter01ProgressionValidator
    {
        public static LevelValidationReport Validate(ChapterLevelCatalogData catalog)
        {
            var report = new LevelValidationReport();
            if (catalog == null)
            {
                report.Errors.Add("Chapter 1 progression catalog is null.");
                return report;
            }
            if (catalog.chapterId != 1)
            {
                report.Errors.Add("Chapter 1 progression validator requires chapterId 1.");
                return report;
            }
            if (catalog.levels == null || catalog.levels.Length != 10)
            {
                report.Errors.Add("Chapter 1 progression requires exactly ten levels.");
                return report;
            }

            var introduced = new HashSet<string>(StringComparer.Ordinal);
            var introductionLevel = new Dictionary<string, int>(StringComparer.Ordinal);
            var tutorialIds = new HashSet<string>(StringComparer.Ordinal);
            int recoveryCount = 0;

            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelData level = catalog.levels[i];
                if (level == null) continue;

                HashSet<string> active = CollectActiveMechanics(level);
                var newlyIntroduced = new List<string>();
                foreach (string mechanic in active)
                    if (!introduced.Contains(mechanic)) newlyIntroduced.Add(mechanic);
                newlyIntroduced.Sort(StringComparer.Ordinal);

                if (newlyIntroduced.Count > 1)
                {
                    report.Errors.Add(
                        $"Level {level.levelId} introduces more than one mechanic: " +
                        string.Join(", ", newlyIntroduced) + "."
                    );
                }

                bool recovery = string.Equals(
                    level.difficulty,
                    "recovery",
                    StringComparison.Ordinal
                );
                if (recovery)
                {
                    recoveryCount++;
                    if (newlyIntroduced.Count > 0)
                        report.Errors.Add($"Recovery level {level.levelId} cannot introduce a mechanic.");
                    ValidateRecoveryLevel(catalog.levels, i, report);
                }

                for (int j = 0; j < newlyIntroduced.Count; j++)
                {
                    string mechanic = newlyIntroduced[j];
                    introductionLevel[mechanic] = level.levelId;
                    if (!HasStartTutorial(level, mechanic))
                    {
                        report.Errors.Add(
                            $"Level {level.levelId} first introduces '{mechanic}' without a level_start tutorial."
                        );
                    }
                }

                LevelTutorialStepData[] tutorials = level.tutorialSteps ??
                    Array.Empty<LevelTutorialStepData>();
                for (int j = 0; j < tutorials.Length; j++)
                {
                    LevelTutorialStepData step = tutorials[j];
                    if (step == null) continue;
                    if (!string.IsNullOrEmpty(step.id) && !tutorialIds.Add(step.id))
                        report.Errors.Add($"Tutorial id '{step.id}' is duplicated across Chapter 1.");
                    if (!active.Contains(step.mechanic))
                    {
                        report.Errors.Add(
                            $"Level {level.levelId} teaches inactive mechanic '{step.mechanic}'."
                        );
                    }
                }

                introduced.UnionWith(active);
            }

            ValidateRollout(introductionLevel, report);
            ValidateDifficultyCurve(catalog.levels, recoveryCount, report);
            return report;
        }

        public static HashSet<string> CollectActiveMechanics(LevelData level)
        {
            var mechanics = new HashSet<string>(StringComparer.Ordinal);
            if (level == null) return mechanics;

            LevelGoalData[] goals = level.goals ?? Array.Empty<LevelGoalData>();
            for (int i = 0; i < goals.Length; i++)
            {
                LevelGoalData goal = goals[i];
                if (goal == null) continue;
                if (string.Equals(goal.type, LevelGoalTypes.CollectTile, StringComparison.Ordinal))
                    mechanics.Add(LevelMechanicKeys.Match);
                else if (string.Equals(goal.type, LevelGoalTypes.Score, StringComparison.Ordinal))
                    mechanics.Add(LevelMechanicKeys.Score);
                else if (string.Equals(goal.type, LevelGoalTypes.BringDown, StringComparison.Ordinal))
                    mechanics.Add(LevelMechanicKeys.BringDown);
                else if (string.Equals(goal.type, LevelGoalTypes.Survival, StringComparison.Ordinal))
                    mechanics.Add(LevelMechanicKeys.Survival);
                else if (string.Equals(goal.type, LevelGoalTypes.ClearObstacle, StringComparison.Ordinal))
                    AddObstacleMechanic(mechanics, goal.obstacleType);
            }

            LevelObstacleData[] obstacles = level.obstacles ?? Array.Empty<LevelObstacleData>();
            for (int i = 0; i < obstacles.Length; i++)
                if (obstacles[i] != null) AddObstacleMechanic(mechanics, obstacles[i].type);

            LevelBoardItemData[] items = level.boardItems ?? Array.Empty<LevelBoardItemData>();
            if (items.Length > 0) mechanics.Add(LevelMechanicKeys.BringDown);
            return mechanics;
        }

        static void AddObstacleMechanic(HashSet<string> mechanics, string obstacleType)
        {
            if (string.Equals(obstacleType, LevelObstacleTypes.Ice, StringComparison.Ordinal))
                mechanics.Add(LevelMechanicKeys.Ice);
            else if (string.Equals(obstacleType, LevelObstacleTypes.Chain, StringComparison.Ordinal))
                mechanics.Add(LevelMechanicKeys.Chain);
            else if (string.Equals(obstacleType, LevelObstacleTypes.Crate, StringComparison.Ordinal))
                mechanics.Add(LevelMechanicKeys.Crate);
        }

        static bool HasStartTutorial(LevelData level, string mechanic)
        {
            LevelTutorialStepData[] tutorials = level.tutorialSteps ??
                Array.Empty<LevelTutorialStepData>();
            for (int i = 0; i < tutorials.Length; i++)
            {
                LevelTutorialStepData step = tutorials[i];
                if (
                    step != null &&
                    string.Equals(step.mechanic, mechanic, StringComparison.Ordinal) &&
                    string.Equals(
                        step.trigger,
                        LevelTutorialTriggerTypes.LevelStart,
                        StringComparison.Ordinal
                    )
                )
                {
                    return true;
                }
            }

            return false;
        }

        static void ValidateRollout(
            Dictionary<string, int> introductionLevel,
            LevelValidationReport report
        )
        {
            int previousLevel = 0;
            for (int i = 0; i < LevelMechanicKeys.Chapter01Rollout.Length; i++)
            {
                string mechanic = LevelMechanicKeys.Chapter01Rollout[i];
                if (!introductionLevel.TryGetValue(mechanic, out int levelId))
                {
                    report.Errors.Add($"Chapter 1 never activates required mechanic '{mechanic}'.");
                    continue;
                }
                if (levelId <= previousLevel)
                {
                    report.Errors.Add(
                        $"Mechanic '{mechanic}' is introduced out of the required teaching order."
                    );
                }
                previousLevel = Math.Max(previousLevel, levelId);
            }

            if (
                introductionLevel.TryGetValue(LevelMechanicKeys.Match, out int matchLevel) &&
                matchLevel != 1
            )
            {
                report.Errors.Add("Basic matching must be introduced on Chapter 1 level 1.");
            }
        }

        static void ValidateRecoveryLevel(
            LevelData[] levels,
            int index,
            LevelValidationReport report
        )
        {
            LevelData recovery = levels[index];
            if (index < 2 || levels[index - 1] == null || levels[index - 2] == null)
            {
                report.Errors.Add($"Recovery level {recovery.levelId} must follow a local peak.");
                return;
            }

            LevelData peak = levels[index - 1];
            LevelData beforePeak = levels[index - 2];
            if (recovery.difficultyRating > peak.difficultyRating - 2)
            {
                report.Errors.Add(
                    $"Recovery level {recovery.levelId} must drop at least two difficulty points after level {peak.levelId}."
                );
            }
            if (peak.difficultyRating <= beforePeak.difficultyRating)
            {
                report.Errors.Add(
                    $"Recovery level {recovery.levelId} does not follow a rising local peak."
                );
            }
            if (recovery.moveLimit < peak.moveLimit + 2)
            {
                report.Errors.Add(
                    $"Recovery level {recovery.levelId} must grant at least two more moves than level {peak.levelId}."
                );
            }
        }

        static void ValidateDifficultyCurve(
            LevelData[] levels,
            int recoveryCount,
            LevelValidationReport report
        )
        {
            if (recoveryCount < 2)
                report.Errors.Add("Chapter 1 requires at least two authored recovery levels.");

            int maximumBeforeFinale = 0;
            for (int i = 0; i < levels.Length; i++)
            {
                LevelData level = levels[i];
                if (level == null) continue;
                if (i > 0 && levels[i - 1] != null)
                {
                    int increase = level.difficultyRating - levels[i - 1].difficultyRating;
                    if (increase > 2)
                    {
                        report.Errors.Add(
                            $"Difficulty jumps by {increase} points between levels {levels[i - 1].levelId} and {level.levelId}; maximum is 2."
                        );
                    }
                }
                if (i < levels.Length - 1)
                    maximumBeforeFinale = Math.Max(maximumBeforeFinale, level.difficultyRating);
            }

            LevelData finale = levels[levels.Length - 1];
            if (finale == null) return;
            if (!string.Equals(finale.difficulty, "finale", StringComparison.Ordinal))
                report.Errors.Add("Chapter 1 level 10 must use the 'finale' difficulty tag.");
            if (finale.difficultyRating <= maximumBeforeFinale)
                report.Errors.Add("The Chapter 1 finale must be the highest-rated level.");
        }
    }
}
