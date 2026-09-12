#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step11LevelSystemSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.11 Level System Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(forceReload: true);
            LevelValidationReport report = LevelDataValidator.ValidateCatalog(catalog);

            Check("catalog validation", () => report.IsValid, ref passed, ref total, failures);
            Check(
                "ten curated levels",
                () => catalog.levels != null && catalog.levels.Length == 10,
                ref passed,
                ref total,
                failures
            );

            if (catalog.levels != null)
            {
                for (int i = 0; i < catalog.levels.Length; i++)
                {
                    LevelData level = catalog.levels[i];
                    Check(
                        level.id + " lookup",
                        () => ReferenceEquals(
                            ChapterLevelCatalog.Find(level.chapterId, level.levelId),
                            level
                        ),
                        ref passed,
                        ref total,
                        failures
                    );
                    Check(
                        level.id + " deterministic playable board",
                        () =>
                        {
                            BoardModel first = LevelBoardFactory.Create(level, level.boardSeed);
                            BoardModel second = LevelBoardFactory.Create(level, level.boardSeed);
                            return
                                first.Width == level.columns &&
                                first.Height == level.rows &&
                                !first.HasAnyMatches() &&
                                first.HasLegalMove() &&
                                first.ComputeStateHash() == second.ComputeStateHash();
                        },
                        ref passed,
                        ref total,
                        failures
                    );
                }
            }

            if (failures.Count == 0)
            {
                Debug.Log($"v1.11 level-system self-tests passed: {passed}/{total}");
                return;
            }

            Debug.LogError(
                $"v1.11 level-system self-tests failed: {passed}/{total}\n" +
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
