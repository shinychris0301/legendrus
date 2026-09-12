#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step04SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.10 Step 4 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            BoardBalanceProfile balance = BoardBalanceProfile.CommercialDefault;

            foreach (BoardQAPreset preset in BoardQAPresetFactory.SpecialCombinationPresets)
            {
                Check(
                    preset.Id + " resolves",
                    () =>
                    {
                        BoardResolutionPlan plan = Plan(preset, balance);
                        return plan.IsResolved;
                    },
                    ref passed,
                    ref total,
                    failures
                );

                Check(
                    preset.Id + " combination",
                    () =>
                    {
                        BoardResolutionPlan plan = Plan(preset, balance);
                        return
                            plan.Waves.Count > 0 &&
                            plan.Waves[0].Combination == preset.ExpectedCombination;
                    },
                    ref passed,
                    ref total,
                    failures
                );

                Check(
                    preset.Id + " footprint",
                    () =>
                    {
                        BoardResolutionPlan plan = Plan(preset, balance);
                        return
                            plan.Waves.Count > 0 &&
                            plan.Waves[0].ClearedCells.Count >= preset.ExpectedMinimumClear;
                    },
                    ref passed,
                    ref total,
                    failures
                );
            }

            foreach (BoardQAPreset preset in BoardQAPresetFactory.BoosterPresets)
            {
                Check(
                    preset.Id + " validation",
                    () =>
                    {
                        BoardModel model = From(preset.Snapshot);
                        BoardBoosterValidationResult result =
                            BoardBoosterCommandValidator.Validate(
                                model,
                                preset.BoosterRequest.Value,
                                balance
                            );
                        return preset.Id == "booster.invalid-empty"
                            ? !result.IsValid
                            : result.IsValid;
                    },
                    ref passed,
                    ref total,
                    failures
                );
            }

            Check(
                "watchdog timeout",
                () =>
                {
                    var watchdog = new BoardCommandWatchdog();
                    watchdog.Begin("qa", 1d, 2d);
                    return !watchdog.IsTimedOut(2.9d) && watchdog.IsTimedOut(3d);
                },
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log($"v1.10 Step 4 self-tests passed: {passed}/{total}");
                return;
            }

            Debug.LogError(
                $"v1.10 Step 4 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
        }

        static BoardResolutionPlan Plan(
            BoardQAPreset preset,
            BoardBalanceProfile balance
        )
        {
            BoardModel model = From(preset.Snapshot);
            return new BoardResolutionPlanner(model, balance).PlanSwap(
                preset.Primary,
                preset.Secondary
            );
        }

        static BoardModel From(BoardSnapshot snapshot)
        {
            var model = new BoardModel(
                snapshot.Width,
                snapshot.Height,
                snapshot.TileTypeCount,
                new SeededBoardRandom(5005)
            );
            model.RestoreSnapshot(snapshot);
            return model;
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
