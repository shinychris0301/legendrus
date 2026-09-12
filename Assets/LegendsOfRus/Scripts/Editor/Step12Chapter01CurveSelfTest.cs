#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step12Chapter01CurveSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.12 Step 3 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(true);
            LevelValidationReport data = LevelDataValidator.ValidateCatalog(catalog);
            LevelValidationReport progression = Chapter01ProgressionValidator.Validate(catalog);

            Check("schema-v3 catalog validates", () => data.IsValid, ref passed, ref total, failures);
            Check("progression contract validates", () => progression.IsValid, ref passed, ref total, failures);
            Check("ten levels are released", () => catalog.levels.Length == 10, ref passed, ref total, failures);
            Check("two recovery levels are authored", () =>
                catalog.levels[3].difficulty == "recovery" &&
                catalog.levels[6].difficulty == "recovery",
                ref passed, ref total, failures);
            Check("finale is highest rated", () =>
            {
                int finale = catalog.levels[9].difficultyRating;
                for (int i = 0; i < 9; i++)
                    if (catalog.levels[i].difficultyRating >= finale) return false;
                return true;
            }, ref passed, ref total, failures);

            var seen = new HashSet<string>(StringComparer.Ordinal);
            var introductions = new Dictionary<string, int>(StringComparer.Ordinal);
            bool oneAtATime = true;
            for (int i = 0; i < catalog.levels.Length; i++)
            {
                HashSet<string> active = Chapter01ProgressionValidator.CollectActiveMechanics(
                    catalog.levels[i]
                );
                int count = 0;
                foreach (string mechanic in active)
                {
                    if (!seen.Add(mechanic)) continue;
                    introductions[mechanic] = catalog.levels[i].levelId;
                    count++;
                }
                oneAtATime &= count <= 1;
            }
            Check("mechanics debut one at a time", () => oneAtATime, ref passed, ref total, failures);
            Check("match debuts on level 1", () => introductions[LevelMechanicKeys.Match] == 1, ref passed, ref total, failures);
            Check("score debuts on level 2", () => introductions[LevelMechanicKeys.Score] == 2, ref passed, ref total, failures);
            Check("ice debuts on level 3", () => introductions[LevelMechanicKeys.Ice] == 3, ref passed, ref total, failures);
            Check("chain debuts on level 5", () => introductions[LevelMechanicKeys.Chain] == 5, ref passed, ref total, failures);
            Check("crate debuts on level 6", () => introductions[LevelMechanicKeys.Crate] == 6, ref passed, ref total, failures);
            Check("bring-down debuts on level 8", () => introductions[LevelMechanicKeys.BringDown] == 8, ref passed, ref total, failures);
            Check("survival debuts on level 9", () => introductions[LevelMechanicKeys.Survival] == 9, ref passed, ref total, failures);

            bool boardsPlayable = true;
            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelData level = catalog.levels[i];
                BoardModel board = LevelBoardFactory.Create(level, level.boardSeed);
                boardsPlayable &= !board.HasAnyMatches() && board.HasLegalMove();
            }
            Check("all mechanic boards generate playable", () => boardsPlayable, ref passed, ref total, failures);

            LevelTutorialStepData[] levelOneSteps = catalog.levels[0].tutorialSteps;
            var sequence = new LevelTutorialSequence(levelOneSteps);
            int firstSignal = sequence.Signal(LevelTutorialTriggerTypes.LevelStart);
            int duplicateSignal = sequence.Signal(LevelTutorialTriggerTypes.LevelStart);
            Check("tutorial start trigger queues once", () => firstSignal == 1 && duplicateSignal == 0, ref passed, ref total, failures);
            Check("tutorial start order is stable", () =>
                sequence.DequeueNext()?.id == "tutorial.match.start",
                ref passed, ref total, failures);
            Check("follow-up trigger is deferred", () =>
                sequence.Signal(LevelTutorialTriggerTypes.FirstMove) == 1 &&
                sequence.DequeueNext()?.id == "tutorial.match.progress",
                ref passed, ref total, failures);

            var rejectedSwapState = new GameplayStateMachine();
            Check("blocked swap preflight returns directly to idle", () =>
                rejectedSwapState.TryTransition(GameplayPhase.Swapping) &&
                rejectedSwapState.TryTransition(GameplayPhase.Idle) &&
                rejectedSwapState.CanAcceptPointerInput,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log($"v1.12 Step 3 Chapter 1 curve self-tests passed: {passed}/{total}.");
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Tests",
                    $"v1.12 Step 3 self-tests passed: {passed}/{total}",
                    "OK"
                );
                return;
            }

            Debug.LogError(
                $"v1.12 Step 3 self-tests failed: {passed}/{total}\n" +
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
                if (test()) passed++;
                else failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
