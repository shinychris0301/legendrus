using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    public sealed class LevelGoalData
    {
        public string type = LevelGoalTypes.CollectTile;
        public int tileType = -1;
        public string obstacleType = string.Empty;
        public string itemType = string.Empty;
        public int target = 10;
    }

    [Serializable]
    public sealed class LevelFixedCellData
    {
        public int x;
        public int y;
        public int tileType;
        public int special;
    }

    [Serializable]
    public sealed class LevelRewardData
    {
        public int gold = 100;
        public int gems;
        public int boosterMace;
        public int boosterFirebird;
        public int boosterLivingLight;
        public int boosterSeaPearl;
    }

    [Serializable]
    public sealed class LevelTutorialStepData
    {
        public string id = "tutorial.match.start";
        public string mechanic = LevelMechanicKeys.Match;
        public string trigger = LevelTutorialTriggerTypes.LevelStart;
        public string title = "Первый ход";
        [TextArea(2, 5)] public string body = "Соберите три одинаковых самоцвета в ряд.";
    }

    [Serializable]
    public sealed class LevelData
    {
        public string id = "chapter01.level01";
        public int chapterId = 1;
        public int levelId = 1;
        public string displayName = "Первый огонь";
        public int columns = 8;
        public int rows = 7;
        public int tileTypes = 6;
        public int moveLimit = 28;
        public int startingScore = 0;
        public int boardSeed = 1101;
        public string difficulty = "tutorial";
        public int difficultyRating = 1;
        public LevelGoalData[] goals = Array.Empty<LevelGoalData>();
        public LevelFixedCellData[] fixedCells = Array.Empty<LevelFixedCellData>();
        public LevelObstacleData[] obstacles = Array.Empty<LevelObstacleData>();
        public LevelBoardItemData[] boardItems = Array.Empty<LevelBoardItemData>();
        public LevelRewardData rewards = new LevelRewardData();
        public int[] starScoreThresholds = { 460, 1060, 1860 };
        public string[] storyTriggers = Array.Empty<string>();
        public LevelTutorialStepData[] tutorialSteps = Array.Empty<LevelTutorialStepData>();

        public int EvaluateStars(int score)
        {
            if (starScoreThresholds == null || starScoreThresholds.Length != 3)
                return 1;

            if (score >= starScoreThresholds[2]) return 3;
            if (score >= starScoreThresholds[1]) return 2;
            return 1;
        }

        public bool HasStoryTrigger(string trigger)
        {
            if (string.IsNullOrEmpty(trigger) || storyTriggers == null)
                return false;

            for (int i = 0; i < storyTriggers.Length; i++)
                if (string.Equals(storyTriggers[i], trigger, StringComparison.Ordinal))
                    return true;

            return false;
        }

        public bool HasStoryTriggerPrefix(string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || storyTriggers == null)
                return false;

            for (int i = 0; i < storyTriggers.Length; i++)
            {
                string trigger = storyTriggers[i];
                if (
                    !string.IsNullOrEmpty(trigger) &&
                    trigger.StartsWith(prefix, StringComparison.Ordinal)
                )
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public sealed class ChapterLevelCatalogData
    {
        public int schemaVersion = 3;
        public int chapterId = 1;
        public string chapterName = "Родная Деревня";
        public LevelData[] levels = Array.Empty<LevelData>();
    }

    public sealed class LevelValidationReport
    {
        public readonly List<string> Errors = new List<string>();
        public readonly List<string> Warnings = new List<string>();

        public bool IsValid => Errors.Count == 0;

        public string Summary
        {
            get
            {
                return IsValid
                    ? $"Valid ({Warnings.Count} warning(s))"
                    : $"Invalid ({Errors.Count} error(s), {Warnings.Count} warning(s))";
            }
        }
    }

    public static class LevelDataValidator
    {
        public const int CurrentSchemaVersion = 3;

        public static LevelValidationReport ValidateCatalog(
            ChapterLevelCatalogData catalog
        )
        {
            var report = new LevelValidationReport();
            if (catalog == null)
            {
                report.Errors.Add("Catalog is null.");
                return report;
            }

            if (catalog.schemaVersion < 1 || catalog.schemaVersion > CurrentSchemaVersion)
            {
                report.Errors.Add(
                    $"Unsupported schemaVersion {catalog.schemaVersion}; expected 1–{CurrentSchemaVersion}."
                );
            }
            else if (catalog.schemaVersion < CurrentSchemaVersion)
            {
                report.Warnings.Add(
                    $"schemaVersion {catalog.schemaVersion} is legacy; save as {CurrentSchemaVersion} before authoring tutorial and difficulty-curve data."
                );
            }
            if (catalog.chapterId < 1)
                report.Errors.Add("chapterId must be positive.");
            if (string.IsNullOrWhiteSpace(catalog.chapterName))
                report.Errors.Add("chapterName is required.");
            if (catalog.levels == null || catalog.levels.Length == 0)
            {
                report.Errors.Add("Catalog must contain at least one level.");
                return report;
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            var levelIds = new HashSet<int>();
            int previousLevelId = 0;

            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelData level = catalog.levels[i];
                string prefix = $"levels[{i}]";
                ValidateLevel(level, catalog.chapterId, prefix, report);
                if (level == null) continue;

                if (!string.IsNullOrWhiteSpace(level.id) && !ids.Add(level.id))
                    report.Errors.Add($"{prefix}.id '{level.id}' is duplicated.");
                if (!levelIds.Add(level.levelId))
                    report.Errors.Add($"{prefix}.levelId {level.levelId} is duplicated.");
                if (level.levelId <= previousLevelId)
                    report.Errors.Add($"{prefix}.levelId must be strictly ascending.");

                previousLevelId = level.levelId;
            }

            if (catalog.chapterId == 1 && catalog.levels.Length != 10)
                report.Errors.Add("Chapter 1 must contain exactly 10 curated levels.");

            for (int i = 0; i < catalog.levels.Length; i++)
            {
                if (catalog.levels[i] != null && catalog.levels[i].levelId != i + 1)
                    report.Errors.Add($"Chapter progression must be contiguous; index {i} must be level {i + 1}.");
            }

            return report;
        }

        static void ValidateLevel(
            LevelData level,
            int expectedChapterId,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level == null)
            {
                report.Errors.Add($"{prefix} is null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(level.id))
                report.Errors.Add($"{prefix}.id is required.");
            if (level.chapterId != expectedChapterId)
                report.Errors.Add($"{prefix}.chapterId must equal catalog chapterId {expectedChapterId}.");
            if (level.levelId < 1)
                report.Errors.Add($"{prefix}.levelId must be positive.");
            if (string.IsNullOrWhiteSpace(level.displayName))
                report.Errors.Add($"{prefix}.displayName is required.");
            if (level.columns < 3 || level.columns > 10)
                report.Errors.Add($"{prefix}.columns must be in [3, 10].");
            if (level.rows < 3 || level.rows > 12)
                report.Errors.Add($"{prefix}.rows must be in [3, 12].");
            if (level.tileTypes < 3 || level.tileTypes > 6)
                report.Errors.Add($"{prefix}.tileTypes must be in [3, 6] for the approved gem set.");
            if (level.moveLimit < 1 || level.moveLimit > 99)
                report.Errors.Add($"{prefix}.moveLimit must be in [1, 99].");
            if (level.startingScore < 0)
                report.Errors.Add($"{prefix}.startingScore cannot be negative.");
            if (level.boardSeed == 0)
                report.Errors.Add($"{prefix}.boardSeed must be non-zero.");
            if (string.IsNullOrWhiteSpace(level.difficulty))
                report.Errors.Add($"{prefix}.difficulty is required.");
            if (level.difficultyRating < 1 || level.difficultyRating > 10)
                report.Errors.Add($"{prefix}.difficultyRating must be in [1, 10].");

            ValidateGoals(level, prefix, report);
            ValidateFixedCells(level, prefix, report);
            ValidateObstacles(level, prefix, report);
            ValidateBoardItems(level, prefix, report);
            ValidateRewards(level, prefix, report);
            ValidateStarThresholds(level, prefix, report);
            ValidateStoryTriggers(level, prefix, report);
            ValidateTutorialSteps(level, prefix, report);
        }

        static void ValidateGoals(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.goals == null || level.goals.Length < 1 || level.goals.Length > 3)
            {
                report.Errors.Add($"{prefix}.goals must contain one to three goals.");
                return;
            }

            var goalKeys = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < level.goals.Length; i++)
            {
                LevelGoalData goal = level.goals[i];
                string goalPrefix = $"{prefix}.goals[{i}]";
                if (goal == null)
                {
                    report.Errors.Add($"{goalPrefix} is null.");
                    continue;
                }

                if (goal.target < 1)
                    report.Errors.Add($"{goalPrefix}.target must be positive.");

                if (!LevelGoalTypes.IsSupported(goal.type))
                {
                    report.Errors.Add(
                        $"{goalPrefix}.type '{goal.type}' is not supported in v1.12."
                    );
                    continue;
                }

                string key = goal.type;
                switch (goal.type)
                {
                    case LevelGoalTypes.CollectTile:
                        if (goal.tileType < 0 || goal.tileType >= level.tileTypes)
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.tileType is outside this level's tile range."
                            );
                        }
                        key += ":" + goal.tileType;
                        break;

                    case LevelGoalTypes.Score:
                        if (goal.target <= level.startingScore)
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.target must exceed startingScore."
                            );
                        }
                        break;

                    case LevelGoalTypes.ClearObstacle:
                        if (!LevelObstacleTypes.TryToBoardKind(goal.obstacleType, out _))
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.obstacleType '{goal.obstacleType}' is not supported."
                            );
                        }
                        else if (goal.target > CountObstacles(level, goal.obstacleType))
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.target exceeds authored '{goal.obstacleType}' obstacles."
                            );
                        }
                        key += ":" + goal.obstacleType;
                        break;

                    case LevelGoalTypes.BringDown:
                        if (!LevelBoardItemTypes.TryToBoardKind(goal.itemType, out _))
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.itemType '{goal.itemType}' is not supported."
                            );
                        }
                        else if (goal.target > CountBoardItems(level, goal.itemType))
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.target exceeds authored '{goal.itemType}' items."
                            );
                        }
                        key += ":" + goal.itemType;
                        break;

                    case LevelGoalTypes.Survival:
                        if (goal.target > level.moveLimit)
                        {
                            report.Errors.Add(
                                $"{goalPrefix}.target cannot exceed moveLimit."
                            );
                        }
                        break;
                }

                if (!goalKeys.Add(key))
                    report.Errors.Add($"{goalPrefix} duplicates another goal '{key}'.");
            }
        }

        static void ValidateFixedCells(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.fixedCells == null) return;

            var positions = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < level.fixedCells.Length; i++)
            {
                LevelFixedCellData cell = level.fixedCells[i];
                string cellPrefix = $"{prefix}.fixedCells[{i}]";
                if (cell == null)
                {
                    report.Errors.Add($"{cellPrefix} is null.");
                    continue;
                }

                if (cell.x < 0 || cell.x >= level.columns || cell.y < 0 || cell.y >= level.rows)
                    report.Errors.Add($"{cellPrefix} is outside the board.");
                if (cell.tileType < 0 || cell.tileType >= level.tileTypes)
                    report.Errors.Add($"{cellPrefix}.tileType is outside this level's tile range.");
                if (cell.special < 0 || cell.special > 4)
                    report.Errors.Add($"{cellPrefix}.special must be in [0, 4].");
                if (!positions.Add($"{cell.x}:{cell.y}"))
                    report.Errors.Add($"{cellPrefix} duplicates another authored cell.");
            }
        }

        static void ValidateObstacles(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.obstacles == null) return;

            var positions = new HashSet<string>(StringComparer.Ordinal);
            var fixedPositions = CollectFixedPositions(level);
            for (int i = 0; i < level.obstacles.Length; i++)
            {
                LevelObstacleData obstacle = level.obstacles[i];
                string obstaclePrefix = $"{prefix}.obstacles[{i}]";
                if (obstacle == null)
                {
                    report.Errors.Add($"{obstaclePrefix} is null.");
                    continue;
                }

                if (!IsInside(level, obstacle.x, obstacle.y))
                    report.Errors.Add($"{obstaclePrefix} is outside the board.");
                if (!LevelObstacleTypes.TryToBoardKind(obstacle.type, out _))
                {
                    report.Errors.Add(
                        $"{obstaclePrefix}.type '{obstacle.type}' is not supported."
                    );
                }
                else
                {
                    int maximum = LevelObstacleTypes.MaximumLayers(obstacle.type);
                    if (obstacle.layers < 1 || obstacle.layers > maximum)
                    {
                        report.Errors.Add(
                            $"{obstaclePrefix}.layers must be in [1, {maximum}] for '{obstacle.type}'."
                        );
                    }
                }

                string position = PositionKey(obstacle.x, obstacle.y);
                if (!positions.Add(position))
                    report.Errors.Add($"{obstaclePrefix} duplicates another obstacle position.");
                if (
                    string.Equals(obstacle.type, LevelObstacleTypes.Crate, StringComparison.Ordinal) &&
                    fixedPositions.Contains(position)
                )
                {
                    report.Errors.Add(
                        $"{obstaclePrefix} cannot place a crate over an authored gem."
                    );
                }
            }
        }

        static void ValidateBoardItems(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.boardItems == null) return;

            var positions = new HashSet<string>(StringComparer.Ordinal);
            var fixedPositions = CollectFixedPositions(level);
            var cratePositions = CollectCratePositions(level);
            for (int i = 0; i < level.boardItems.Length; i++)
            {
                LevelBoardItemData item = level.boardItems[i];
                string itemPrefix = $"{prefix}.boardItems[{i}]";
                if (item == null)
                {
                    report.Errors.Add($"{itemPrefix} is null.");
                    continue;
                }

                if (!IsInside(level, item.x, item.y))
                    report.Errors.Add($"{itemPrefix} is outside the board.");
                if (!LevelBoardItemTypes.TryToBoardKind(item.type, out _))
                {
                    report.Errors.Add(
                        $"{itemPrefix}.type '{item.type}' is not supported."
                    );
                }

                string position = PositionKey(item.x, item.y);
                if (!positions.Add(position))
                    report.Errors.Add($"{itemPrefix} duplicates another board-item position.");
                if (fixedPositions.Contains(position))
                    report.Errors.Add($"{itemPrefix} cannot replace an authored gem.");
                if (cratePositions.Contains(position))
                    report.Errors.Add($"{itemPrefix} cannot occupy a crate position.");
            }
        }

        static int CountObstacles(LevelData level, string type)
        {
            int count = 0;
            if (level.obstacles == null) return count;
            for (int i = 0; i < level.obstacles.Length; i++)
            {
                LevelObstacleData obstacle = level.obstacles[i];
                if (
                    obstacle != null &&
                    string.Equals(obstacle.type, type, StringComparison.Ordinal)
                )
                    count++;
            }
            return count;
        }

        static int CountBoardItems(LevelData level, string type)
        {
            int count = 0;
            if (level.boardItems == null) return count;
            for (int i = 0; i < level.boardItems.Length; i++)
            {
                LevelBoardItemData item = level.boardItems[i];
                if (item != null && string.Equals(item.type, type, StringComparison.Ordinal))
                    count++;
            }
            return count;
        }

        static HashSet<string> CollectFixedPositions(LevelData level)
        {
            var positions = new HashSet<string>(StringComparer.Ordinal);
            if (level.fixedCells == null) return positions;
            for (int i = 0; i < level.fixedCells.Length; i++)
            {
                LevelFixedCellData cell = level.fixedCells[i];
                if (cell != null) positions.Add(PositionKey(cell.x, cell.y));
            }
            return positions;
        }

        static HashSet<string> CollectCratePositions(LevelData level)
        {
            var positions = new HashSet<string>(StringComparer.Ordinal);
            if (level.obstacles == null) return positions;
            for (int i = 0; i < level.obstacles.Length; i++)
            {
                LevelObstacleData obstacle = level.obstacles[i];
                if (
                    obstacle != null &&
                    string.Equals(obstacle.type, LevelObstacleTypes.Crate, StringComparison.Ordinal)
                )
                    positions.Add(PositionKey(obstacle.x, obstacle.y));
            }
            return positions;
        }

        static bool IsInside(LevelData level, int x, int y) =>
            x >= 0 && x < level.columns && y >= 0 && y < level.rows;

        static string PositionKey(int x, int y) => $"{x}:{y}";

        static void ValidateRewards(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            LevelRewardData reward = level.rewards;
            if (reward == null)
            {
                report.Errors.Add($"{prefix}.rewards is required.");
                return;
            }

            if (
                reward.gold < 0 ||
                reward.gems < 0 ||
                reward.boosterMace < 0 ||
                reward.boosterFirebird < 0 ||
                reward.boosterLivingLight < 0 ||
                reward.boosterSeaPearl < 0
            )
            {
                report.Errors.Add($"{prefix}.rewards cannot contain negative values.");
            }
        }

        static void ValidateStarThresholds(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            int[] thresholds = level.starScoreThresholds;
            if (thresholds == null || thresholds.Length != 3)
            {
                report.Errors.Add($"{prefix}.starScoreThresholds must contain exactly three values.");
                return;
            }

            if (
                thresholds[0] <= level.startingScore ||
                thresholds[1] <= thresholds[0] ||
                thresholds[2] <= thresholds[1]
            )
            {
                report.Errors.Add($"{prefix}.starScoreThresholds must increase above startingScore.");
            }
        }

        static void ValidateStoryTriggers(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.storyTriggers == null) return;

            var triggers = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < level.storyTriggers.Length; i++)
            {
                string trigger = level.storyTriggers[i];
                if (string.IsNullOrWhiteSpace(trigger))
                    report.Errors.Add($"{prefix}.storyTriggers[{i}] cannot be empty.");
                else if (!triggers.Add(trigger))
                    report.Errors.Add($"{prefix}.storyTriggers[{i}] is duplicated.");
            }
        }

        static void ValidateTutorialSteps(
            LevelData level,
            string prefix,
            LevelValidationReport report
        )
        {
            if (level.tutorialSteps == null) return;
            if (level.tutorialSteps.Length > 4)
            {
                report.Errors.Add(
                    $"{prefix}.tutorialSteps supports at most four concise steps per level."
                );
            }

            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < level.tutorialSteps.Length; i++)
            {
                LevelTutorialStepData step = level.tutorialSteps[i];
                string stepPrefix = $"{prefix}.tutorialSteps[{i}]";
                if (step == null)
                {
                    report.Errors.Add($"{stepPrefix} is null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(step.id))
                    report.Errors.Add($"{stepPrefix}.id is required.");
                else if (!ids.Add(step.id))
                    report.Errors.Add($"{stepPrefix}.id '{step.id}' is duplicated in this level.");

                if (!LevelMechanicKeys.IsSupported(step.mechanic))
                {
                    report.Errors.Add(
                        $"{stepPrefix}.mechanic '{step.mechanic}' is not supported."
                    );
                }
                if (!LevelTutorialTriggerTypes.IsSupported(step.trigger))
                {
                    report.Errors.Add(
                        $"{stepPrefix}.trigger '{step.trigger}' is not supported."
                    );
                }
                if (string.IsNullOrWhiteSpace(step.title))
                    report.Errors.Add($"{stepPrefix}.title is required.");
                else if (step.title.Length > 64)
                    report.Errors.Add($"{stepPrefix}.title must be at most 64 characters.");
                if (string.IsNullOrWhiteSpace(step.body))
                    report.Errors.Add($"{stepPrefix}.body is required.");
                else if (step.body.Length > 240)
                    report.Errors.Add($"{stepPrefix}.body must be at most 240 characters.");
            }
        }
    }

    public static class ChapterLevelCatalog
    {
        public const string ResourcePath = "Config/chapter01_levels";
        static ChapterLevelCatalogData cached;

        public static ChapterLevelCatalogData LoadChapter1(bool forceReload = false)
        {
            if (!forceReload && cached != null)
                return cached;

            TextAsset text = Resources.Load<TextAsset>(ResourcePath);
            if (text == null)
            {
                Debug.LogError($"Missing level catalog at Resources/{ResourcePath}.json");
                cached = CreateFallbackCatalog();
                return cached;
            }

            try
            {
                ChapterLevelCatalogData parsed =
                    JsonUtility.FromJson<ChapterLevelCatalogData>(text.text);
                LevelValidationReport report = LevelDataValidator.ValidateCatalog(parsed);
                LevelValidationReport progression = Chapter01ProgressionValidator.Validate(parsed);
                if (!report.IsValid || !progression.IsValid)
                {
                    Debug.LogError(
                        "Chapter 1 level catalog is invalid:\n" +
                        string.Join("\n", report.Errors) +
                        (progression.Errors.Count > 0
                            ? "\n" + string.Join("\n", progression.Errors)
                            : string.Empty)
                    );
                    cached = CreateFallbackCatalog();
                }
                else
                {
                    cached = parsed;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError("Could not parse Chapter 1 level catalog: " + exception.Message);
                cached = CreateFallbackCatalog();
            }

            return cached;
        }

        public static LevelData Find(int chapterId, int levelId)
        {
            ChapterLevelCatalogData catalog = LoadChapter1();
            if (catalog == null || catalog.levels == null || catalog.chapterId != chapterId)
                return null;

            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelData level = catalog.levels[i];
                if (level != null && level.levelId == levelId)
                    return level;
            }

            return null;
        }

        public static LevelData ResolveSelected(PlayerSaveData save)
        {
            ChapterLevelCatalogData catalog = LoadChapter1();
            if (catalog.levels == null || catalog.levels.Length == 0)
                return CreateFallbackLevel();

            int selected = save != null ? save.selectedLevel : 1;
            LevelData level = Find(catalog.chapterId, selected);
            return level ?? catalog.levels[0];
        }

        public static LevelData Next(LevelData current)
        {
            if (current == null) return null;
            return Find(current.chapterId, current.levelId + 1);
        }

        public static void InvalidateCache()
        {
            cached = null;
        }

        static ChapterLevelCatalogData CreateFallbackCatalog()
        {
            return new ChapterLevelCatalogData
            {
                schemaVersion = LevelDataValidator.CurrentSchemaVersion,
                chapterId = 1,
                chapterName = "Родная Деревня",
                levels = new[] { CreateFallbackLevel() }
            };
        }

        static LevelData CreateFallbackLevel()
        {
            return new LevelData
            {
                id = "chapter01.level01.fallback",
                chapterId = 1,
                levelId = 1,
                displayName = "Первый огонь",
                columns = 8,
                rows = 7,
                tileTypes = 6,
                moveLimit = 28,
                startingScore = 0,
                boardSeed = 1101,
                difficulty = "tutorial",
                difficultyRating = 1,
                goals = new[]
                {
                    new LevelGoalData { tileType = 0, target = 15 },
                    new LevelGoalData { tileType = 1, target = 18 }
                },
                fixedCells = Array.Empty<LevelFixedCellData>(),
                obstacles = Array.Empty<LevelObstacleData>(),
                boardItems = Array.Empty<LevelBoardItemData>(),
                rewards = new LevelRewardData { gold = 120 },
                starScoreThresholds = new[] { 460, 1060, 1860 },
                storyTriggers = new[] { "dialogue.firebird_found" },
                tutorialSteps = new[]
                {
                    new LevelTutorialStepData
                    {
                        id = "tutorial.match.start",
                        mechanic = LevelMechanicKeys.Match,
                        trigger = LevelTutorialTriggerTypes.LevelStart,
                        title = "Первый ход",
                        body = "Соберите три одинаковых самоцвета в ряд."
                    }
                }
            };
        }
    }
}
