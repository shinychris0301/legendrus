#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class ChapterLevelAuthoringWindow : EditorWindow
    {
        const string AssetPath =
            "Assets/LegendsOfRus/Resources/CampaignActive/levels.json";

        ChapterLevelCatalogData catalog;
        LevelValidationReport report;
        Vector2 levelScroll;
        Vector2 detailScroll;
        int selectedIndex;
        bool dirty;

        [MenuItem("Legends of Rus/Levels/Open Chapter 1 Authoring")]
        public static void Open()
        {
            GetWindow<ChapterLevelAuthoringWindow>("Chapter 1 Levels");
        }

        void OnEnable()
        {
            LoadCatalog();
        }

        void OnGUI()
        {
            DrawToolbar();
            if (catalog == null)
            {
                EditorGUILayout.HelpBox(
                    "The Chapter 1 JSON catalog could not be loaded.",
                    MessageType.Error
                );
                return;
            }

            EditorGUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            DrawLevelList();
            DrawSelectedLevel();
            EditorGUILayout.EndHorizontal();
            DrawValidationReport();
        }

        void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(60f)))
            {
                if (!dirty || EditorUtility.DisplayDialog(
                    "Discard unsaved changes?",
                    "Reload the Chapter 1 JSON and discard edits in this window?",
                    "Reload",
                    "Cancel"
                ))
                {
                    LoadCatalog();
                }
            }

            if (GUILayout.Button("Validate", EditorStyles.toolbarButton, GUILayout.Width(65f)))
                ValidateCatalog(logResult: true);

            GUI.enabled = dirty;
            if (GUILayout.Button("Save JSON", EditorStyles.toolbarButton, GUILayout.Width(75f)))
                SaveCatalog();
            GUI.enabled = true;

            if (GUILayout.Button("Select JSON", EditorStyles.toolbarButton, GUILayout.Width(80f)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetPath);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label(dirty ? "Unsaved" : "Saved", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }

        void DrawLevelList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(235f));
            EditorGUILayout.LabelField(
                $"{catalog.chapterName} ({catalog.levels?.Length ?? 0})",
                EditorStyles.boldLabel
            );

            levelScroll = EditorGUILayout.BeginScrollView(levelScroll);
            LevelData[] levels = catalog.levels ?? Array.Empty<LevelData>();
            for (int i = 0; i < levels.Length; i++)
            {
                LevelData level = levels[i];
                string label = level == null
                    ? $"{i + 1}. <null>"
                    : $"{level.levelId:00}  {level.displayName}";
                if (GUILayout.Toggle(selectedIndex == i, label, "Button"))
                    selectedIndex = i;
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Duplicate")) DuplicateSelected();
            if (GUILayout.Button("Delete")) DeleteSelected();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        void DrawSelectedLevel()
        {
            LevelData[] levels = catalog.levels;
            if (levels == null || levels.Length == 0)
            {
                EditorGUILayout.HelpBox("No levels in the catalog.", MessageType.Warning);
                return;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, levels.Length - 1);
            LevelData level = levels[selectedIndex];
            if (level == null)
            {
                level = new LevelData { levelId = selectedIndex + 1 };
                levels[selectedIndex] = level;
                dirty = true;
            }

            EditorGUILayout.BeginVertical();
            detailScroll = EditorGUILayout.BeginScrollView(detailScroll);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            level.id = EditorGUILayout.TextField("Stable ID", level.id);
            level.chapterId = EditorGUILayout.IntField("Chapter", level.chapterId);
            level.levelId = EditorGUILayout.IntField("Level", level.levelId);
            level.displayName = EditorGUILayout.TextField("Display name", level.displayName);
            level.difficulty = EditorGUILayout.TextField("Difficulty tag", level.difficulty);
            level.difficultyRating = EditorGUILayout.IntSlider(
                "Difficulty rating",
                level.difficultyRating,
                1,
                10
            );

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Board", EditorStyles.boldLabel);
            level.columns = EditorGUILayout.IntSlider("Columns", level.columns, 3, 10);
            level.rows = EditorGUILayout.IntSlider("Rows", level.rows, 3, 12);
            level.tileTypes = EditorGUILayout.IntSlider("Gem types", level.tileTypes, 3, 6);
            level.moveLimit = EditorGUILayout.IntSlider("Move limit", level.moveLimit, 1, 99);
            level.startingScore = EditorGUILayout.IntField("Starting score", level.startingScore);
            level.boardSeed = EditorGUILayout.IntField("Board seed", level.boardSeed);

            DrawGoals(level);
            DrawFixedCells(level);
            DrawObstacles(level);
            DrawBoardItems(level);
            DrawRewards(level);
            DrawStarThresholds(level);
            DrawStoryTriggers(level);
            DrawTutorialSteps(level);

            if (EditorGUI.EndChangeCheck())
            {
                dirty = true;
                report = null;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawGoals(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Goals (approved HUD supports 1–2)", EditorStyles.boldLabel);
            if (level.goals == null)
                level.goals = Array.Empty<LevelGoalData>();
            for (int i = 0; i < level.goals.Length; i++)
            {
                if (level.goals[i] == null)
                    level.goals[i] = new LevelGoalData();
                LevelGoalData goal = level.goals[i];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                goal.type = EditorGUILayout.TextField("Type", goal.type);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.goals, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();

                goal.target = EditorGUILayout.IntField("Target", goal.target);

                if (string.Equals(goal.type, LevelGoalTypes.CollectTile, StringComparison.Ordinal))
                {
                    goal.tileType = EditorGUILayout.IntField("Gem type", goal.tileType);
                }
                else if (
                    string.Equals(goal.type, LevelGoalTypes.ClearObstacle, StringComparison.Ordinal)
                )
                {
                    goal.obstacleType = EditorGUILayout.TextField(
                        "Obstacle type",
                        goal.obstacleType
                    );
                }
                else if (
                    string.Equals(goal.type, LevelGoalTypes.BringDown, StringComparison.Ordinal)
                )
                {
                    goal.itemType = EditorGUILayout.TextField("Item type", goal.itemType);
                }

                EditorGUILayout.EndVertical();
            }
            GUI.enabled = level.goals.Length < 2;
            if (GUILayout.Button("Add goal"))
            {
                ArrayUtility.Add(ref level.goals, new LevelGoalData());
                dirty = true;
            }
            GUI.enabled = true;
        }

        void DrawFixedCells(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Authored starting cells (released after board creation)",
                EditorStyles.boldLabel
            );
            if (level.fixedCells == null)
                level.fixedCells = Array.Empty<LevelFixedCellData>();
            for (int i = 0; i < level.fixedCells.Length; i++)
            {
                if (level.fixedCells[i] == null)
                    level.fixedCells[i] = new LevelFixedCellData();
                LevelFixedCellData cell = level.fixedCells[i];
                EditorGUILayout.BeginHorizontal();
                cell.x = EditorGUILayout.IntField("X", cell.x);
                cell.y = EditorGUILayout.IntField("Y", cell.y);
                cell.tileType = EditorGUILayout.IntField("Gem", cell.tileType);
                cell.special = EditorGUILayout.IntPopup(
                    "Special",
                    cell.special,
                    new[] { "None", "Row", "Column", "Burst", "Color" },
                    new[] { 0, 1, 2, 3, 4 }
                );
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.fixedCells, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add authored cell"))
            {
                ArrayUtility.Add(ref level.fixedCells, new LevelFixedCellData());
                dirty = true;
            }
        }

        void DrawObstacles(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Persistent obstacle layer", EditorStyles.boldLabel);
            if (level.obstacles == null)
                level.obstacles = Array.Empty<LevelObstacleData>();
            for (int i = 0; i < level.obstacles.Length; i++)
            {
                if (level.obstacles[i] == null)
                    level.obstacles[i] = new LevelObstacleData();
                LevelObstacleData obstacle = level.obstacles[i];
                EditorGUILayout.BeginHorizontal();
                obstacle.x = EditorGUILayout.IntField("X", obstacle.x);
                obstacle.y = EditorGUILayout.IntField("Y", obstacle.y);
                obstacle.type = EditorGUILayout.TextField("Type", obstacle.type);
                obstacle.layers = EditorGUILayout.IntField("Layers", obstacle.layers);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.obstacles, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add obstacle"))
            {
                ArrayUtility.Add(ref level.obstacles, new LevelObstacleData());
                dirty = true;
            }
        }

        void DrawBoardItems(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Bring-down objective items", EditorStyles.boldLabel);
            if (level.boardItems == null)
                level.boardItems = Array.Empty<LevelBoardItemData>();
            for (int i = 0; i < level.boardItems.Length; i++)
            {
                if (level.boardItems[i] == null)
                    level.boardItems[i] = new LevelBoardItemData();
                LevelBoardItemData item = level.boardItems[i];
                EditorGUILayout.BeginHorizontal();
                item.x = EditorGUILayout.IntField("X", item.x);
                item.y = EditorGUILayout.IntField("Y", item.y);
                item.type = EditorGUILayout.TextField("Type", item.type);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.boardItems, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add board item"))
            {
                ArrayUtility.Add(ref level.boardItems, new LevelBoardItemData());
                dirty = true;
            }
        }

        void DrawRewards(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("First-clear rewards", EditorStyles.boldLabel);
            if (level.rewards == null)
                level.rewards = new LevelRewardData();
            level.rewards.gold = EditorGUILayout.IntField("Gold", level.rewards.gold);
            level.rewards.gems = EditorGUILayout.IntField("Gems", level.rewards.gems);
            level.rewards.boosterMace = EditorGUILayout.IntField("Bogatyr mace", level.rewards.boosterMace);
            level.rewards.boosterFirebird = EditorGUILayout.IntField("Firebird", level.rewards.boosterFirebird);
            level.rewards.boosterLivingLight = EditorGUILayout.IntField("Living light", level.rewards.boosterLivingLight);
            level.rewards.boosterSeaPearl = EditorGUILayout.IntField("Sea pearl", level.rewards.boosterSeaPearl);
        }

        void DrawStarThresholds(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Star score thresholds", EditorStyles.boldLabel);
            if (level.starScoreThresholds == null || level.starScoreThresholds.Length != 3)
                level.starScoreThresholds = new[] { level.startingScore + 500, level.startingScore + 1200, level.startingScore + 2000 };

            level.starScoreThresholds[0] = EditorGUILayout.IntField("1 star", level.starScoreThresholds[0]);
            level.starScoreThresholds[1] = EditorGUILayout.IntField("2 stars", level.starScoreThresholds[1]);
            level.starScoreThresholds[2] = EditorGUILayout.IntField("3 stars", level.starScoreThresholds[2]);
        }

        void DrawStoryTriggers(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Story triggers", EditorStyles.boldLabel);
            if (level.storyTriggers == null)
                level.storyTriggers = Array.Empty<string>();
            for (int i = 0; i < level.storyTriggers.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                level.storyTriggers[i] = EditorGUILayout.TextField(level.storyTriggers[i]);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.storyTriggers, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add story trigger"))
            {
                ArrayUtility.Add(ref level.storyTriggers, string.Empty);
                dirty = true;
            }
        }

        void DrawTutorialSteps(LevelData level)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Tutorial sequence (triggered in authored order)",
                EditorStyles.boldLabel
            );
            if (level.tutorialSteps == null)
                level.tutorialSteps = Array.Empty<LevelTutorialStepData>();

            for (int i = 0; i < level.tutorialSteps.Length; i++)
            {
                if (level.tutorialSteps[i] == null)
                    level.tutorialSteps[i] = new LevelTutorialStepData();
                LevelTutorialStepData step = level.tutorialSteps[i];

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Step {i + 1}", EditorStyles.boldLabel);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    ArrayUtility.RemoveAt(ref level.tutorialSteps, i);
                    dirty = true;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
                step.id = EditorGUILayout.TextField("Stable ID", step.id);
                step.mechanic = EditorGUILayout.TextField("Mechanic", step.mechanic);
                step.trigger = EditorGUILayout.TextField("Trigger", step.trigger);
                step.title = EditorGUILayout.TextField("Title", step.title);
                step.body = EditorGUILayout.TextArea(step.body, GUILayout.MinHeight(48f));
                EditorGUILayout.EndVertical();
            }

            GUI.enabled = level.tutorialSteps.Length < 4;
            if (GUILayout.Button("Add tutorial step"))
            {
                ArrayUtility.Add(ref level.tutorialSteps, new LevelTutorialStepData());
                dirty = true;
            }
            GUI.enabled = true;
        }

        void DrawValidationReport()
        {
            if (report == null) return;

            MessageType type = report.IsValid ? MessageType.Info : MessageType.Error;
            EditorGUILayout.HelpBox(report.Summary, type);
            for (int i = 0; i < report.Errors.Count; i++)
                EditorGUILayout.HelpBox(report.Errors[i], MessageType.Error);
            for (int i = 0; i < report.Warnings.Count; i++)
                EditorGUILayout.HelpBox(report.Warnings[i], MessageType.Warning);
        }

        void LoadCatalog()
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetPath);
            if (asset == null)
            {
                catalog = null;
                report = null;
                return;
            }

            catalog = JsonUtility.FromJson<ChapterLevelCatalogData>(asset.text);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Math.Max(0, (catalog?.levels?.Length ?? 1) - 1));
            dirty = false;
            ValidateCatalog(logResult: false);
        }

        void SaveCatalog()
        {
            catalog.schemaVersion = LevelDataValidator.CurrentSchemaVersion;
            ValidateCatalog(logResult: false);
            if (report == null || !report.IsValid)
            {
                EditorUtility.DisplayDialog(
                    "Catalog is invalid",
                    "Fix all validation errors before saving.",
                    "OK"
                );
                return;
            }

            File.WriteAllText(
                AssetPath,
                JsonUtility.ToJson(catalog, true) + Environment.NewLine,
                new UTF8Encoding(false)
            );
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
            ChapterLevelCatalog.InvalidateCache();
            dirty = false;
            Debug.Log("Saved and validated Chapter 1 level catalog.");
        }

        void ValidateCatalog(bool logResult)
        {
            report = LevelDataValidator.ValidateCampaignCatalog(catalog);
            var story = Resources.Load<TextAsset>(ActiveCampaignContent.ResourceRoot + "story");
            foreach (var language in new[] { "ru", "en" })
            {
                var locale = Resources.Load<TextAsset>(ActiveCampaignContent.ResourceRoot + "story_" + language);
                if (!story || !locale) { report.Errors.Add("Missing active story/localization: " + language); continue; }
                report.Errors.AddRange(StoryCatalog.ValidateCampaign(
                    JsonUtility.FromJson<ChapterStoryCatalogData>(story.text),
                    JsonUtility.FromJson<StoryLocalizationData>(locale.text), catalog, false).Errors);
            }
            if (!logResult) return;

            if (report.IsValid)
                Debug.Log("Chapter 1 level validation passed: " + report.Summary);
            else
                Debug.LogError("Chapter 1 level validation failed:\n" + string.Join("\n", report.Errors));
        }

        void DuplicateSelected()
        {
            if (catalog?.levels == null || catalog.levels.Length == 0) return;

            LevelData source = catalog.levels[Mathf.Clamp(selectedIndex, 0, catalog.levels.Length - 1)];
            LevelData copy = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(source));
            copy.levelId = catalog.levels.Length + 1;
            copy.id = $"chapter{catalog.chapterId:00}.level{copy.levelId:00}";
            copy.displayName += " (копия)";
            ArrayUtility.Add(ref catalog.levels, copy);
            selectedIndex = catalog.levels.Length - 1;
            dirty = true;
            report = null;
        }

        void DeleteSelected()
        {
            if (catalog?.levels == null || catalog.levels.Length == 0) return;
            if (!EditorUtility.DisplayDialog(
                "Delete level?",
                "Remove the selected level from the in-memory catalog?",
                "Delete",
                "Cancel"
            ))
            {
                return;
            }

            ArrayUtility.RemoveAt(ref catalog.levels, selectedIndex);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Math.Max(0, catalog.levels.Length - 1));
            dirty = true;
            report = null;
        }
    }
}
#endif
