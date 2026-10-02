using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    [Category("LegacyTenRoundFixture")]
    public sealed class StoryDataRuntimePlayModeTests
    {
        LegacyChapterTestFixture legacyFixture;
        [TearDown] public void RestoreActiveContent() { legacyFixture?.Dispose(); }

        static readonly string[] PersistedSaveKeys =
        {
            "LegendsOfRus_Save_v1",
            "LegendsOfRus_Save_v1_Backup",
            "LegendsOfRus_Save_v1_Staging"
        };

        GameObject stateObject;
        bool ownsStateObject;
        bool[] hadPersistedSaves;
        string[] persistedSaveValues;
        GameState previousState;
        string previousRuntimeSaveJson;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            legacyFixture = new LegacyChapterTestFixture();
            SnapshotAndClearSaveJournal();
            yield return null;
            if (GameState.Instance == null)
            {
                stateObject = new GameObject("StoryDataTestState");
                stateObject.AddComponent<GameState>();
                ownsStateObject = true;
            }
            else
            {
                previousState = GameState.Instance;
                previousRuntimeSaveJson = JsonUtility.ToJson(previousState.Save);
            }

            GameState.Instance.ResetProgress();
            StoryService.ResetRuntime();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            StoryService.ResetRuntime();
            if (GameState.Instance != null)
                GameState.Instance.ResetProgress();
            if (ownsStateObject && stateObject != null)
                Object.Destroy(stateObject);
            yield return null;
            RestoreSaveJournal();
            RestoreRuntimeState();
        }

        [Test]
        public void Chapter01StoryCatalog_ValidatesAgainstEveryDialogueTrigger()
        {
            ChapterStoryCatalogData story = LegacyChapterTestFixture.Story();
            StoryLocalizationData russian = LegacyChapterTestFixture.Russian();
            ChapterLevelCatalogData levels = LegacyChapterTestFixture.Levels();

            StoryValidationReport report = StoryCatalog.Validate(story, russian, levels);

            Assert.That(report.Errors, Is.Empty);
            Assert.That(story.schemaVersion, Is.EqualTo(3));
            Assert.That(story.scenes, Has.Length.EqualTo(8));
            Assert.That(story.milestones, Has.Length.EqualTo(7));
            Assert.That(story.reactions, Has.Length.EqualTo(20));
            Assert.That(story.introSceneId, Is.EqualTo("chapter01.intro"));
        }

        [Test]
        public void IntroSequence_ResolvesLocalizedLinesInStableOrder()
        {
            StorySceneData intro = StoryCatalog.FindScene("chapter01.intro");
            var sequence = new StorySequenceRuntime(intro, LegacyChapterTestFixture.Russian());

            Assert.That(sequence.CurrentLine.Speaker, Is.EqualTo("ДОБРЫНЯ"));
            Assert.That(sequence.CurrentLine.Text, Does.Contain("Родной Деревней"));
            Assert.That(sequence.Advance(), Is.True);
            Assert.That(sequence.CurrentLine.Speaker, Is.EqualTo("ИВАН"));
            Assert.That(sequence.Advance(), Is.True);
            Assert.That(sequence.Advance(), Is.False);
            Assert.That(sequence.IsComplete, Is.True);
            Assert.That(intro.returnRoute, Is.EqualTo(StoryReturnRoutes.PreLevel));
        }

        [Test]
        public void StoryProgress_NormalizesAndCompletesEachSceneOnlyOnce()
        {
            var completed = new List<string> { "chapter01.intro", "", "chapter01.intro" };

            StoryProgress.Normalize(completed);

            Assert.That(completed, Is.EqualTo(new[] { "chapter01.intro" }));
            Assert.That(StoryProgress.Complete(completed, "chapter01.firebird_found"), Is.True);
            Assert.That(StoryProgress.Complete(completed, "chapter01.firebird_found"), Is.False);
        }

        [Test]
        public void FirstLaunchIntro_CompletesToPreLevelAndDoesNotRepeat()
        {
            Assert.That(StoryService.TryBeginChapterIntro(), Is.True);
            Assert.That(StoryService.ActiveSceneId, Is.EqualTo("chapter01.intro"));

            StoryAdvanceResult result;
            do
            {
                result = StoryService.AdvanceDialogue();
            }
            while (result == StoryAdvanceResult.MoreLines);

            Assert.That(result, Is.EqualTo(StoryAdvanceResult.PreLevel));
            Assert.That(
                GameState.Instance.HasCompletedStoryScene("chapter01.intro"),
                Is.True
            );
            Assert.That(StoryService.TryBeginChapterIntro(), Is.False);
        }

        [Test]
        public void FinaleDialogue_ReturnsToChapterComplete()
        {
            StorySceneData finale = StoryCatalog.FindByTrigger("dialogue.chapter_finale");
            StoryChapterCompletionData completion = LegacyChapterTestFixture.Story().completion;

            Assert.That(finale, Is.Not.Null);
            Assert.That(finale.returnRoute, Is.EqualTo(StoryReturnRoutes.ChapterComplete));
            Assert.That(finale.lines, Has.Length.EqualTo(5));
            Assert.That(completion.finalLevelId, Is.EqualTo(10));
            Assert.That(completion.finaleSceneId, Is.EqualTo(finale.id));
        }

        void SnapshotAndClearSaveJournal()
        {
            hadPersistedSaves = new bool[PersistedSaveKeys.Length];
            persistedSaveValues = new string[PersistedSaveKeys.Length];
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                hadPersistedSaves[i] = PlayerPrefs.HasKey(key);
                persistedSaveValues[i] = hadPersistedSaves[i]
                    ? PlayerPrefs.GetString(key)
                    : string.Empty;
                PlayerPrefs.DeleteKey(key);
            }
            PlayerPrefs.Save();
        }

        void RestoreRuntimeState()
        {
            if (
                previousState == null ||
                GameState.Instance != previousState ||
                string.IsNullOrEmpty(previousRuntimeSaveJson)
            )
            {
                return;
            }

            PlayerSaveData restored = JsonUtility.FromJson<PlayerSaveData>(
                previousRuntimeSaveJson
            );
            if (restored != null)
                previousState.Save = restored;
        }

        void RestoreSaveJournal()
        {
            for (int i = 0; i < PersistedSaveKeys.Length; i++)
            {
                string key = PersistedSaveKeys[i];
                PlayerPrefs.DeleteKey(key);
                if (hadPersistedSaves[i])
                    PlayerPrefs.SetString(key, persistedSaveValues[i]);
            }
            PlayerPrefs.Save();
        }
    }
}
