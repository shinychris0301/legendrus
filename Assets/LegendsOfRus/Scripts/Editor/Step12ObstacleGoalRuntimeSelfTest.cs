#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step12ObstacleGoalRuntimeSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.12 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            ChapterLevelCatalogData released = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            Check(
                "released catalog still validates",
                () => LevelDataValidator.ValidateCatalog(released).IsValid,
                ref passed,
                ref total,
                failures
            );

            var blocked = CreateDoubleMatchBoard(12291);
            blocked.SetObstacle(
                new BoardPosition(2, 1),
                new BoardObstacleData(BoardObstacleKind.Chain, 1)
            );
            Check(
                "chain blocks swap",
                () =>
                    new BoardResolutionPlanner(blocked).PlanSwap(
                        new BoardPosition(2, 1),
                        new BoardPosition(2, 2)
                    ).Status == BoardResolutionStatus.Rejected,
                ref passed,
                ref total,
                failures
            );

            var mechanics = CreateDoubleMatchBoard(12292);
            var ice = new BoardPosition(0, 1);
            var crate = new BoardPosition(1, 0);
            mechanics.SetObstacle(
                ice,
                new BoardObstacleData(BoardObstacleKind.Ice, 1)
            );
            mechanics.SetObstacle(
                crate,
                new BoardObstacleData(BoardObstacleKind.Crate, 2)
            );
            string sourceHash = mechanics.ComputeStateHash();
            var planner = new BoardResolutionPlanner(mechanics);
            BoardResolutionPlan first = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            BoardResolutionPlan second = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );

            Check(
                "mechanic plan resolves",
                () => first.IsResolved && first.Waves.Count > 0,
                ref passed,
                ref total,
                failures
            );
            Check(
                "ice direct hit is emitted",
                () => first.Waves[0].ObstacleInteractions.Any(
                    hit => hit.Position == ice && hit.Destroyed
                ),
                ref passed,
                ref total,
                failures
            );
            Check(
                "crate adjacent hit is de-duplicated",
                () => first.Waves[0].ObstacleInteractions.Count(
                    hit => hit.Position == crate
                ) == 1,
                ref passed,
                ref total,
                failures
            );
            Check(
                "mechanic plan is deterministic",
                () => first.PlanHash == second.PlanHash,
                ref passed,
                ref total,
                failures
            );
            Check(
                "planner keeps source immutable",
                () => mechanics.ComputeStateHash() == sourceHash,
                ref passed,
                ref total,
                failures
            );

            var clearGoal = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.ClearObstacle,
                        obstacleType = LevelObstacleTypes.Ice,
                        target = 1
                    }
                },
                0
            );
            clearGoal.ApplyWave(first.Waves[0], first.Waves[0].ScoreDelta);
            Check(
                "clear-obstacle goal consumes destruction event",
                () => clearGoal.AllComplete,
                ref passed,
                ref total,
                failures
            );

            var itemBoard = new BoardModel(
                3,
                4,
                4,
                new SeededBoardRandom(12293)
            );
            itemBoard.ClearAll();
            itemBoard.SetObjectiveItem(
                new BoardPosition(1, 1),
                BoardObjectiveItemKind.FirebirdFeather
            );
            BoardCollapseResult itemCollapse = itemBoard.CollapseAndRefill();
            Check(
                "bring-down item exits at board bottom",
                () =>
                    itemCollapse.CollectedObjectiveItems.Count == 1 &&
                    itemCollapse.CollectedObjectiveItems[0].Exit ==
                        new BoardPosition(1, itemBoard.Height),
                ref passed,
                ref total,
                failures
            );

            var survival = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.Survival,
                        target = 1
                    }
                },
                0
            );
            survival.RecordConsumedMove();
            Check(
                "survival goal advances on consumed move",
                () => survival.AllComplete,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.12 Step 2 obstacle/goal runtime self-tests passed: " +
                    $"{passed}/{total}; plan={first.PlanHash}"
                );
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Tests",
                    $"v1.12 Step 2 self-tests passed: {passed}/{total}\n\n" +
                    $"Plan: {first.PlanHash}",
                    "OK"
                );
                return;
            }

            Debug.LogError(
                $"v1.12 Step 2 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static BoardModel CreateDoubleMatchBoard(int seed)
        {
            var board = new BoardModel(3, 3, 3, new SeededBoardRandom(seed));
            board.LoadRows("001", "001", "110");
            return board;
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
