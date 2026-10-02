using System;
using System.Collections;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class Chapter01MechanicCurvePlayModeTests
    {
        [Test]
        public void ReleasedCatalog_UsesSchema3AndPassesProgressionContract()
        {
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            LevelValidationReport data = LevelDataValidator.ValidateCatalog(catalog);
            LevelValidationReport progression = Chapter01ProgressionValidator.Validate(catalog);

            Assert.That(data.Errors, Is.Empty);
            Assert.That(progression.Errors, Is.Empty);
            Assert.That(catalog.schemaVersion, Is.EqualTo(3));
            Assert.That(catalog.levels, Has.Length.EqualTo(10));
        }

        [Test]
        public void MechanicRollout_IntroducesOneMechanicAtATimeInTeachingOrder()
        {
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            var seen = new HashSet<string>();
            var introductions = new Dictionary<string, int>();

            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelData level = catalog.levels[i];
                HashSet<string> active = Chapter01ProgressionValidator.CollectActiveMechanics(
                    level
                );
                int introducedHere = 0;
                foreach (string mechanic in active)
                {
                    if (!seen.Add(mechanic)) continue;
                    introductions[mechanic] = level.levelId;
                    introducedHere++;
                }
                Assert.That(
                    introducedHere,
                    Is.LessThanOrEqualTo(1),
                    $"Level {level.levelId} introduces too many mechanics."
                );
            }

            Assert.That(introductions[LevelMechanicKeys.Match], Is.EqualTo(1));
            Assert.That(introductions[LevelMechanicKeys.Score], Is.EqualTo(2));
            Assert.That(introductions[LevelMechanicKeys.Ice], Is.EqualTo(3));
            Assert.That(introductions[LevelMechanicKeys.Chain], Is.EqualTo(5));
            Assert.That(introductions[LevelMechanicKeys.Crate], Is.EqualTo(6));
            Assert.That(introductions[LevelMechanicKeys.BringDown], Is.EqualTo(8));
            Assert.That(introductions[LevelMechanicKeys.Survival], Is.EqualTo(9));
        }

        [Test]
        public void EveryIntroducedMechanic_HasAnExplicitStartTutorial()
        {
            ChapterLevelCatalogData catalog = ChapterLevelCatalog.LoadChapter1(
                forceReload: true
            );
            var tutorialMechanics = new HashSet<string>();
            for (int i = 0; i < catalog.levels.Length; i++)
            {
                LevelTutorialStepData[] steps = catalog.levels[i].tutorialSteps;
                for (int j = 0; j < steps.Length; j++)
                {
                    if (steps[j].trigger == LevelTutorialTriggerTypes.LevelStart)
                        tutorialMechanics.Add(steps[j].mechanic);
                }
            }

            CollectionAssert.IsSubsetOf(
                LevelMechanicKeys.Chapter01Rollout,
                tutorialMechanics
            );
        }

        [Test]
        public void DifficultyCurve_UsesTwoRecoveryLevelsAndHighestFinale()
        {
            LevelData[] levels = ChapterLevelCatalog.LoadChapter1(true).levels;
            Assert.That(levels[3].difficulty, Is.EqualTo("recovery"));
            Assert.That(levels[6].difficulty, Is.EqualTo("recovery"));
            Assert.That(
                levels[3].difficultyRating,
                Is.LessThanOrEqualTo(levels[2].difficultyRating - 2)
            );
            Assert.That(
                levels[6].difficultyRating,
                Is.LessThanOrEqualTo(levels[5].difficultyRating - 2)
            );
            Assert.That(levels[3].moveLimit, Is.GreaterThanOrEqualTo(levels[2].moveLimit + 2));
            Assert.That(levels[6].moveLimit, Is.GreaterThanOrEqualTo(levels[5].moveLimit + 2));

            for (int i = 0; i < levels.Length - 1; i++)
                Assert.That(levels[9].difficultyRating, Is.GreaterThan(levels[i].difficultyRating));
        }

        [Test]
        public void EveryReleasedLevel_GeneratesAPlayableMechanicBoard()
        {
            LevelData[] levels = ChapterLevelCatalog.LoadChapter1(true).levels;
            for (int i = 0; i < levels.Length; i++)
            {
                LevelData level = levels[i];
                BoardModel board = LevelBoardFactory.Create(level, level.boardSeed);
                Assert.That(board.HasAnyMatches(), Is.False, level.id);
                Assert.That(board.HasLegalMove(), Is.True, level.id);
                Assert.That(board.Width, Is.EqualTo(level.columns), level.id);
                Assert.That(board.Height, Is.EqualTo(level.rows), level.id);
            }
        }

        [Test]
        public void TutorialSequence_PreservesOrderAndSignalsEachStepOnce()
        {
            var steps = new[]
            {
                new LevelTutorialStepData
                {
                    id = "start.one",
                    trigger = LevelTutorialTriggerTypes.LevelStart
                },
                new LevelTutorialStepData
                {
                    id = "start.two",
                    trigger = LevelTutorialTriggerTypes.LevelStart
                },
                new LevelTutorialStepData
                {
                    id = "move.one",
                    trigger = LevelTutorialTriggerTypes.FirstMove
                }
            };
            var sequence = new LevelTutorialSequence(steps);

            Assert.That(sequence.Signal(LevelTutorialTriggerTypes.LevelStart), Is.EqualTo(2));
            Assert.That(sequence.Signal(LevelTutorialTriggerTypes.LevelStart), Is.Zero);
            Assert.That(sequence.DequeueNext().id, Is.EqualTo("start.one"));
            Assert.That(sequence.Signal(LevelTutorialTriggerTypes.FirstMove), Is.EqualTo(1));
            Assert.That(sequence.DequeueNext().id, Is.EqualTo("start.two"));
            Assert.That(sequence.DequeueNext().id, Is.EqualTo("move.one"));
            Assert.That(sequence.HasPending, Is.False);
        }

        [UnityTest]
        public IEnumerator TutorialPresenter_PausesAndResumesGameplay()
        {
            LevelData source = ChapterLevelCatalog.Find(1, 1);
            LevelData level = JsonUtility.FromJson<LevelData>(JsonUtility.ToJson(source));
            string tutorialId = "test.tutorial." + Guid.NewGuid().ToString("N");
            level.tutorialSteps = new[]
            {
                new LevelTutorialStepData
                {
                    id = tutorialId,
                    mechanic = LevelMechanicKeys.Match,
                    trigger = LevelTutorialTriggerTypes.LevelStart,
                    title = "Test",
                    body = "Test tutorial body."
                }
            };

            var canvasObject = new GameObject(
                "TutorialTestCanvas",
                typeof(RectTransform),
                typeof(Canvas)
            );
            var boardObject = new GameObject(
                "TutorialTestBoard",
                typeof(RectTransform),
                typeof(Match3Board)
            );
            boardObject.transform.SetParent(canvasObject.transform, false);
            Match3Board board = boardObject.GetComponent<Match3Board>();
            board.Initialize(level, null, null);

            var presenterObject = new GameObject(
                "TutorialTestPresenter",
                typeof(RectTransform),
                typeof(LevelTutorialPresenter)
            );
            presenterObject.transform.SetParent(canvasObject.transform, false);
            LevelTutorialPresenter presenter =
                presenterObject.GetComponent<LevelTutorialPresenter>();
            presenter.Initialize(board, level);
            yield return null;

            Assert.That(presenter.IsPresenting, Is.True);
            Assert.That(board.Phase, Is.EqualTo(GameplayPhase.Paused));

            presenter.Advance();
            yield return null;
            Assert.That(presenter.IsPresenting, Is.False);
            Assert.That(board.CanAcceptPointerInput, Is.True);

            UnityEngine.Object.Destroy(canvasObject);
            yield return null;
        }
    }
}
