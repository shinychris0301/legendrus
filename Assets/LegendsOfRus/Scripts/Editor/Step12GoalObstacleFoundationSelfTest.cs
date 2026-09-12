#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step12GoalObstacleFoundationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.12 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            ChapterLevelCatalogData released = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            LevelValidationReport releasedReport = LevelDataValidator.ValidateCatalog(
                released
            );
            Check(
                "released current-schema catalog validates",
                () =>
                    released.schemaVersion == LevelDataValidator.CurrentSchemaVersion &&
                    releasedReport.IsValid,
                ref passed,
                ref total,
                failures
            );
            Check(
                "all five goal contracts are registered",
                () =>
                    LevelGoalTypes.IsSupported(LevelGoalTypes.CollectTile) &&
                    LevelGoalTypes.IsSupported(LevelGoalTypes.Score) &&
                    LevelGoalTypes.IsSupported(LevelGoalTypes.ClearObstacle) &&
                    LevelGoalTypes.IsSupported(LevelGoalTypes.BringDown) &&
                    LevelGoalTypes.IsSupported(LevelGoalTypes.Survival),
                ref passed,
                ref total,
                failures
            );

            ChapterLevelCatalogData prototype = JsonUtility.FromJson<ChapterLevelCatalogData>(
                JsonUtility.ToJson(released)
            );
            LevelData level = prototype.levels[0];
            level.fixedCells = Array.Empty<LevelFixedCellData>();
            level.goals = new[]
            {
                new LevelGoalData
                {
                    type = LevelGoalTypes.ClearObstacle,
                    obstacleType = LevelObstacleTypes.Ice,
                    target = 1
                },
                new LevelGoalData
                {
                    type = LevelGoalTypes.BringDown,
                    itemType = LevelBoardItemTypes.FirebirdFeather,
                    target = 1
                }
            };
            level.obstacles = new[]
            {
                new LevelObstacleData
                {
                    x = 0,
                    y = 0,
                    type = LevelObstacleTypes.Ice,
                    layers = 2
                }
            };
            level.boardItems = new[]
            {
                new LevelBoardItemData
                {
                    x = 2,
                    y = 2,
                    type = LevelBoardItemTypes.FirebirdFeather
                }
            };

            LevelValidationReport prototypeReport = LevelDataValidator.ValidateCatalog(
                prototype
            );
            Check(
                "prototype mechanic contract validates",
                () => prototypeReport.IsValid,
                ref passed,
                ref total,
                failures
            );

            BoardModel board = null;
            Check(
                "factory maps obstacle and objective item",
                () =>
                {
                    board = LevelBoardFactory.Create(level, level.boardSeed);
                    return
                        board.GetObstacle(new BoardPosition(0, 0)) ==
                            new BoardObstacleData(BoardObstacleKind.Ice, 2) &&
                        board.GetObjectiveItem(new BoardPosition(2, 2)) ==
                            BoardObjectiveItemKind.FirebirdFeather;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "v2 replay restores complete deterministic state",
                () =>
                {
                    BoardSnapshot snapshot = board.CreateSnapshot();
                    BoardSnapshot decoded = BoardReplayCodec.Decode(
                        snapshot.ToReplayToken()
                    );
                    var restored = new BoardModel(
                        decoded.Width,
                        decoded.Height,
                        decoded.TileTypeCount,
                        new SeededBoardRandom(12199)
                    );
                    restored.RestoreSnapshot(decoded);
                    return
                        snapshot.HasMechanicState &&
                        restored.ComputeStateHash() == snapshot.StateHash;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "legacy replay remains readable",
                () =>
                {
                    var legacy = new BoardModel(
                        4,
                        4,
                        4,
                        new SeededBoardRandom(12198)
                    );
                    legacy.GeneratePlayableBoard();
                    string token = legacy.CreateSnapshot().ToReplayToken();
                    return
                        !token.StartsWith("v2|", StringComparison.Ordinal) &&
                        BoardReplayCodec.TryDecode(token, out BoardSnapshot decoded) &&
                        !decoded.HasMechanicState;
                },
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log($"v1.12 Step 1 self-tests passed: {passed}/{total}");
                return;
            }

            Debug.LogError(
                $"v1.12 Step 1 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static void Check(
            string name,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test())
                    passed++;
                else
                    failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
