#if UNITY_EDITOR
using System;
using System.Linq;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class BoardResolutionPlanSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.10 Step 3 Resolution Plan Self-Tests")]
        public static void Run()
        {
            int passed = 0;

            var invalid = CreateLatinBoard(5, 5, 5, 100);
            string invalidBefore = invalid.ComputeStateHash();
            BoardResolutionPlan invalidPlan = new BoardResolutionPlanner(invalid).PlanSwap(
                new BoardPosition(0, 0),
                new BoardPosition(1, 0)
            );
            Expect(invalidPlan.IsInvalidSwap, "invalid swap status"); passed++;
            Expect(!invalidPlan.ConsumesMove, "invalid swap does not consume move"); passed++;
            Expect(invalid.ComputeStateHash() == invalidBefore, "invalid plan keeps source"); passed++;

            var valid = new BoardModel(3, 3, 3, new SeededBoardRandom(7));
            valid.LoadRows("001", "001", "110");
            string validBefore = valid.ComputeStateHash();
            var planner = new BoardResolutionPlanner(valid);
            BoardResolutionPlan first = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            BoardResolutionPlan second = planner.PlanSwap(
                new BoardPosition(2, 1),
                new BoardPosition(2, 2)
            );
            Expect(first.IsResolved, "valid plan resolves"); passed++;
            Expect(first.Waves.Count > 0, "valid plan has waves"); passed++;
            Expect(first.Waves[0].ClearedCells.Count >= 3, "valid plan clears match"); passed++;
            Expect(first.Waves[0].SnapshotAfterClear != null, "after-clear snapshot"); passed++;
            Expect(first.Waves[0].SnapshotAfterCollapse != null, "after-collapse snapshot"); passed++;
            Expect(first.PlanHash == second.PlanHash, "same command plan hash"); passed++;
            Expect(valid.ComputeStateHash() == validBefore, "planner keeps source immutable"); passed++;

            var combo = CreateLatinBoard(5, 5, 5, 200);
            combo.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Row);
            combo.SetSpecial(new BoardPosition(2, 1), SpecialGemKind.Column);
            BoardResolutionPlan comboPlan = new BoardResolutionPlanner(combo).PlanSwap(
                new BoardPosition(1, 1),
                new BoardPosition(2, 1)
            );
            Expect(comboPlan.IsResolved, "special combination resolves"); passed++;
            Expect(
                comboPlan.Waves[0].Combination == SpecialCombinationKind.RowWithColumn,
                "row-column combination kind"
            ); passed++;
            Expect(
                comboPlan.Waves[0].Activations.Any(a => a.Kind == SpecialGemKind.Row),
                "row activation command"
            ); passed++;
            Expect(
                comboPlan.Waves[0].Activations.Any(a => a.Kind == SpecialGemKind.Column),
                "column activation command"
            ); passed++;

            var booster = CreateLatinBoard(5, 5, 5, 300);
            string boosterBefore = booster.ComputeStateHash();
            BoardResolutionPlan boosterPlan = new BoardResolutionPlanner(booster).PlanBooster(
                new BoardBoosterRequest(
                    BoardBoosterCommandKind.FirebirdArea,
                    new BoardPosition(2, 2)
                )
            );
            Expect(boosterPlan.IsResolved, "booster plan resolves"); passed++;
            Expect(!boosterPlan.ConsumesMove, "booster plan does not consume move"); passed++;
            Expect(boosterPlan.Waves[0].ClearedCells.Count >= 9, "firebird area clear"); passed++;
            Expect(booster.ComputeStateHash() == boosterBefore, "booster planner keeps source"); passed++;

            valid.RestoreSnapshot(first.FinalSnapshot);
            Expect(
                valid.ComputeStateHash() == first.FinalSnapshot.StateHash,
                "final snapshot applies exactly"
            ); passed++;

            const int expected = 19;
            Expect(passed == expected, "self-test count");

            Debug.Log(
                $"Legends of Rus v1.10 Step 3 resolution-plan self-tests PASSED " +
                $"({passed}/{expected}). Plan={first.PlanHash}, final={first.FinalSnapshot.StateHash}"
            );
            EditorUtility.DisplayDialog(
                "Legends of Rus — Tests",
                $"v1.10 Step 3 resolution-plan self-tests passed: {passed}/{expected}\n\n" +
                $"Plan: {first.PlanHash}\nFinal board: {first.FinalSnapshot.StateHash}",
                "OK"
            );
        }

        static BoardModel CreateLatinBoard(
            int width,
            int height,
            int tileTypes,
            int seed
        )
        {
            var model = new BoardModel(
                width,
                height,
                tileTypes,
                new SeededBoardRandom(seed)
            );
            var values = new int[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                values[y * width + x] = (x + y) % tileTypes;
            model.LoadRowMajor(values);
            return model;
        }

        static void Expect(bool condition, string name)
        {
            if (!condition)
                throw new InvalidOperationException(
                    $"Resolution-plan self-test failed: {name}"
                );
        }
    }
}
#endif
