using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.SaveMigration
{
    public sealed class PlayerSaveMigrationRegistryEditModeTests
    {
        const long FixedFallbackUtcTicks = 638712864000000000L;

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void LiteralLegacyVersion_MigratesToV6WithoutMutatingSource(
            int sourceVersion
        )
        {
            PlayerSaveData source = CreateLegacySave(sourceVersion);
            string sourceBefore = SaveJson(source);
            int legacyProgressFactoryCalls = 0;

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    delegate(PlayerSaveData candidate)
                    {
                        legacyProgressFactoryCalls++;
                        Assert.That(candidate, Is.Not.SameAs(source));
                        return new List<LevelProgressData>
                        {
                            Progress(9, 11, 2, 901)
                        };
                    },
                    FixedFallbackUtcTicks
                );

            Assert.That(result.Succeeded, Is.True, result.ReasonCode);
            Assert.That(result.Changed, Is.True);
            Assert.That(result.SourceVersion, Is.EqualTo(sourceVersion));
            Assert.That(result.ReasonCode,
                Is.EqualTo(PlayerSaveMigrationRegistry.Migrated));
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Not.SameAs(source));
            Assert.That(result.Data.saveVersion,
                Is.EqualTo(PlayerSaveMigrationRegistry.CurrentPayloadVersion));
            Assert.That(result.Data.energyMax, Is.EqualTo(5));
            Assert.That(result.Data.lastEnergyUtcTicks,
                Is.EqualTo(FixedFallbackUtcTicks));
            Assert.That(result.Data.completedTutorialSteps, Is.Not.Null);
            Assert.That(result.Data.completedStoryScenes, Is.Not.Null);
            Assert.That(result.Data.acknowledgedChapterCompletions, Is.Not.Null);
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(legacyProgressFactoryCalls,
                Is.EqualTo(sourceVersion < 3 ? 1 : 0));

            if (sourceVersion < 3)
            {
                Assert.That(result.Data.selectedChapter, Is.EqualTo(4));
                Assert.That(result.Data.selectedLevel, Is.EqualTo(7));
                Assert.That(result.Data.highestUnlockedLevel, Is.EqualTo(7));
                Assert.That(result.Data.levelProgress.Count, Is.EqualTo(1));
                Assert.That(result.Data.levelProgress[0].chapterId,
                    Is.EqualTo(9));
                Assert.That(result.Data.levelProgress[0].levelId,
                    Is.EqualTo(11));
            }
            else
            {
                Assert.That(result.Data.selectedChapter, Is.EqualTo(8));
                Assert.That(result.Data.selectedLevel, Is.EqualTo(9));
                Assert.That(result.Data.highestUnlockedLevel, Is.EqualTo(10));
                Assert.That(result.Data.levelProgress.Count, Is.EqualTo(1));
                Assert.That(result.Data.levelProgress[0].chapterId,
                    Is.EqualTo(41));
                Assert.That(result.Data.levelProgress[0].levelId,
                    Is.EqualTo(42));
            }

            if (sourceVersion == 1)
            {
                Assert.That(result.Data.boosterMace, Is.EqualTo(7));
                Assert.That(result.Data.boosterFirebird, Is.EqualTo(5));
                Assert.That(result.Data.boosterLivingLight, Is.EqualTo(8));
                Assert.That(result.Data.boosterSeaPearl, Is.EqualTo(10));
                Assert.That(result.Data.music, Is.True);
                Assert.That(result.Data.sfx, Is.True);
                Assert.That(result.Data.vibration, Is.True);
                Assert.That(result.Data.notifications, Is.True);
            }

            AssertPreservedVersionSpecificCollections(sourceVersion, result.Data);
        }

        [Test]
        public void CurrentV6_AllFieldsArePreservedAndMigrationIsIdempotent()
        {
            PlayerSaveData source = CreateCompleteCurrentSave();
            string expectedJson = SaveJson(source);

            PlayerSaveMigrationResult first =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    FailIfLegacyProgressIsRequested,
                    FixedFallbackUtcTicks
                );

            Assert.That(first.Succeeded, Is.True, first.ReasonCode);
            Assert.That(first.Changed, Is.False);
            Assert.That(first.SourceVersion, Is.EqualTo(6));
            Assert.That(first.ReasonCode,
                Is.EqualTo(PlayerSaveMigrationRegistry.AlreadyCurrent));
            Assert.That(first.Data, Is.Not.SameAs(source));
            Assert.That(SaveJson(first.Data), Is.EqualTo(expectedJson));
            Assert.That(SaveJson(source), Is.EqualTo(expectedJson));
            AssertCompleteCurrentFixture(first.Data);

            PlayerSaveMigrationResult second =
                PlayerSaveMigrationRegistry.Migrate(
                    first.Data,
                    FailIfLegacyProgressIsRequested,
                    FixedFallbackUtcTicks + 12345L
                );

            Assert.That(second.Succeeded, Is.True, second.ReasonCode);
            Assert.That(second.Changed, Is.False);
            Assert.That(second.ReasonCode,
                Is.EqualTo(PlayerSaveMigrationRegistry.AlreadyCurrent));
            Assert.That(second.Data, Is.Not.SameAs(first.Data));
            Assert.That(SaveJson(second.Data), Is.EqualTo(expectedJson));
        }

        [TestCase(-1, "SAVE_MIGRATION_SOURCE_INVALID")]
        [TestCase(0, "SAVE_MIGRATION_SOURCE_INVALID")]
        [TestCase(7, "SAVE_MIGRATION_FUTURE_VERSION_BLOCKED")]
        [TestCase(2147483647, "SAVE_MIGRATION_FUTURE_VERSION_BLOCKED")]
        public void InvalidOrFutureVersion_FailsWithoutSourceMutation(
            int sourceVersion,
            string expectedReasonCode
        )
        {
            PlayerSaveData source = CreateCompleteCurrentSave();
            source.saveVersion = sourceVersion;
            string sourceBefore = SaveJson(source);
            int legacyProgressFactoryCalls = 0;

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    delegate(PlayerSaveData ignored)
                    {
                        legacyProgressFactoryCalls++;
                        return new List<LevelProgressData>();
                    },
                    FixedFallbackUtcTicks
                );

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.SourceVersion, Is.EqualTo(sourceVersion));
            Assert.That(result.ReasonCode, Is.EqualTo(expectedReasonCode));
            Assert.That(result.Data, Is.SameAs(source));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
            Assert.That(legacyProgressFactoryCalls, Is.Zero);
        }

        [Test]
        public void MissingEnergyTimestamp_UsesFixedFallbackExactlyOnce()
        {
            PlayerSaveData source = CreateCompleteCurrentSave();
            source.lastEnergyUtcTicks = 0;
            string sourceBefore = SaveJson(source);

            PlayerSaveMigrationResult first =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    FailIfLegacyProgressIsRequested,
                    FixedFallbackUtcTicks
                );

            Assert.That(first.Succeeded, Is.True, first.ReasonCode);
            Assert.That(first.Changed, Is.True);
            Assert.That(first.ReasonCode,
                Is.EqualTo(PlayerSaveMigrationRegistry.Migrated));
            Assert.That(first.Data.lastEnergyUtcTicks,
                Is.EqualTo(FixedFallbackUtcTicks));
            Assert.That(source.lastEnergyUtcTicks, Is.Zero);
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));

            PlayerSaveMigrationResult second =
                PlayerSaveMigrationRegistry.Migrate(
                    first.Data,
                    FailIfLegacyProgressIsRequested,
                    FixedFallbackUtcTicks + 1L
                );

            Assert.That(second.Succeeded, Is.True, second.ReasonCode);
            Assert.That(second.Changed, Is.False);
            Assert.That(second.Data.lastEnergyUtcTicks,
                Is.EqualTo(FixedFallbackUtcTicks));
        }

        [Test]
        public void SuccessfulMigration_DetachesListsAndNestedProgress()
        {
            PlayerSaveData source = CreateCompleteCurrentSave();
            string sourceBefore = SaveJson(source);

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    FailIfLegacyProgressIsRequested,
                    FixedFallbackUtcTicks
                );

            Assert.That(result.Succeeded, Is.True, result.ReasonCode);
            Assert.That(result.Data.levelProgress,
                Is.Not.SameAs(source.levelProgress));
            Assert.That(result.Data.levelProgress[0],
                Is.Not.SameAs(source.levelProgress[0]));
            Assert.That(result.Data.completedTutorialSteps,
                Is.Not.SameAs(source.completedTutorialSteps));
            Assert.That(result.Data.completedStoryScenes,
                Is.Not.SameAs(source.completedStoryScenes));
            Assert.That(result.Data.acknowledgedChapterCompletions,
                Is.Not.SameAs(source.acknowledgedChapterCompletions));

            result.Data.levelProgress[0].bestScore = -9001;
            result.Data.levelProgress.Add(Progress(50, 51, 0, 0));
            result.Data.completedTutorialSteps.Add("mutated-tutorial");
            result.Data.completedStoryScenes.Clear();
            result.Data.acknowledgedChapterCompletions[0] = -1;

            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        [Test]
        public void LegacyProgressFactoryFailure_FailsWithoutSourceMutation()
        {
            PlayerSaveData source = CreateLegacySave(2);
            string sourceBefore = SaveJson(source);

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    delegate(PlayerSaveData ignored)
                    {
                        throw new InvalidOperationException("scripted failure");
                    },
                    FixedFallbackUtcTicks
                );

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Changed, Is.False);
            Assert.That(result.SourceVersion, Is.EqualTo(2));
            Assert.That(result.ReasonCode,
                Is.EqualTo(PlayerSaveMigrationRegistry.LegacyProgressFailed));
            Assert.That(result.Data, Is.SameAs(source));
            Assert.That(SaveJson(source), Is.EqualTo(sourceBefore));
        }

        static PlayerSaveData CreateLegacySave(int version)
        {
            var source = new PlayerSaveData
            {
                saveVersion = version,
                gold = 101,
                gems = 202,
                energy = 3,
                energyMax = 0,
                lastEnergyUtcTicks = 0,
                chapter = 4,
                level = 7,
                stars = 12,
                storyStage = 13,
                selectedChapter = 8,
                selectedLevel = 9,
                highestUnlockedLevel = 10,
                boosterMace = 0,
                boosterFirebird = -1,
                boosterLivingLight = 0,
                boosterSeaPearl = -2,
                music = false,
                sfx = false,
                vibration = false,
                notifications = false,
                levelProgress = new List<LevelProgressData>
                {
                    Progress(41, 42, 3, 4242)
                },
                completedTutorialSteps = version >= 4
                    ? new List<string> { "tutorial-v4" }
                    : null,
                completedStoryScenes = version >= 5
                    ? new List<string> { "story-v5" }
                    : null,
                acknowledgedChapterCompletions = version >= 6
                    ? new List<int> { 6 }
                    : null
            };
            return source;
        }

        static PlayerSaveData CreateCompleteCurrentSave()
        {
            return new PlayerSaveData
            {
                saveVersion = 6,
                gold = 12001,
                gems = 607,
                energy = 4,
                energyMax = 9,
                lastEnergyUtcTicks = 638701234567890123L,
                chapter = 3,
                level = 8,
                stars = 47,
                storyStage = 12,
                selectedChapter = 2,
                selectedLevel = 6,
                highestUnlockedLevel = 17,
                lastCompletedChapter = 2,
                lastCompletedLevel = 5,
                lastRewardGold = 345,
                lastRewardGems = 17,
                lastRewardEligibleForDouble = true,
                lastRewardDoubled = true,
                levelProgress = new List<LevelProgressData>
                {
                    new LevelProgressData
                    {
                        chapterId = 2,
                        levelId = 5,
                        bestStars = 3,
                        bestScore = 98765,
                        attempts = 11,
                        retries = 4,
                        failures = 3,
                        completions = 2,
                        rewardClaimed = true
                    }
                },
                completedTutorialSteps = new List<string>
                {
                    "tutorial.swap",
                    "tutorial.booster"
                },
                completedStoryScenes = new List<string>
                {
                    "story.chapter01.intro",
                    "story.chapter01.finale"
                },
                acknowledgedChapterCompletions = new List<int> { 1, 2 },
                boosterMace = 13,
                boosterFirebird = 14,
                boosterLivingLight = 15,
                boosterSeaPearl = 16,
                lastDailyRewardUtc = "2031-04-05T06:07:08.0000000Z",
                music = false,
                sfx = true,
                vibration = false,
                notifications = true
            };
        }

        static LevelProgressData Progress(
            int chapterId,
            int levelId,
            int bestStars,
            int bestScore
        )
        {
            return new LevelProgressData
            {
                chapterId = chapterId,
                levelId = levelId,
                bestStars = bestStars,
                bestScore = bestScore,
                attempts = 5,
                retries = 2,
                failures = 1,
                completions = 3,
                rewardClaimed = true
            };
        }

        static List<LevelProgressData> FailIfLegacyProgressIsRequested(
            PlayerSaveData ignored
        )
        {
            Assert.Fail("Current saves must not invoke the legacy progress factory.");
            return null;
        }

        static void AssertPreservedVersionSpecificCollections(
            int sourceVersion,
            PlayerSaveData migrated
        )
        {
            if (sourceVersion >= 4)
            {
                Assert.That(migrated.completedTutorialSteps,
                    Is.EqualTo(new[] { "tutorial-v4" }));
            }
            else
            {
                Assert.That(migrated.completedTutorialSteps, Is.Empty);
            }

            if (sourceVersion >= 5)
            {
                Assert.That(migrated.completedStoryScenes,
                    Is.EqualTo(new[] { "story-v5" }));
            }
            else
            {
                Assert.That(migrated.completedStoryScenes, Is.Empty);
            }

            Assert.That(migrated.acknowledgedChapterCompletions, Is.Empty);
        }

        static void AssertCompleteCurrentFixture(PlayerSaveData actual)
        {
            Assert.That(actual.saveVersion, Is.EqualTo(6));
            Assert.That(actual.gold, Is.EqualTo(12001));
            Assert.That(actual.gems, Is.EqualTo(607));
            Assert.That(actual.energy, Is.EqualTo(4));
            Assert.That(actual.energyMax, Is.EqualTo(9));
            Assert.That(actual.lastEnergyUtcTicks,
                Is.EqualTo(638701234567890123L));
            Assert.That(actual.chapter, Is.EqualTo(3));
            Assert.That(actual.level, Is.EqualTo(8));
            Assert.That(actual.stars, Is.EqualTo(47));
            Assert.That(actual.storyStage, Is.EqualTo(12));
            Assert.That(actual.selectedChapter, Is.EqualTo(2));
            Assert.That(actual.selectedLevel, Is.EqualTo(6));
            Assert.That(actual.highestUnlockedLevel, Is.EqualTo(17));
            Assert.That(actual.lastCompletedChapter, Is.EqualTo(2));
            Assert.That(actual.lastCompletedLevel, Is.EqualTo(5));
            Assert.That(actual.lastRewardGold, Is.EqualTo(345));
            Assert.That(actual.lastRewardGems, Is.EqualTo(17));
            Assert.That(actual.lastRewardEligibleForDouble, Is.True);
            Assert.That(actual.lastRewardDoubled, Is.True);
            Assert.That(actual.levelProgress.Count, Is.EqualTo(1));
            Assert.That(actual.levelProgress[0].chapterId, Is.EqualTo(2));
            Assert.That(actual.levelProgress[0].levelId, Is.EqualTo(5));
            Assert.That(actual.levelProgress[0].bestStars, Is.EqualTo(3));
            Assert.That(actual.levelProgress[0].bestScore, Is.EqualTo(98765));
            Assert.That(actual.levelProgress[0].attempts, Is.EqualTo(11));
            Assert.That(actual.levelProgress[0].retries, Is.EqualTo(4));
            Assert.That(actual.levelProgress[0].failures, Is.EqualTo(3));
            Assert.That(actual.levelProgress[0].completions, Is.EqualTo(2));
            Assert.That(actual.levelProgress[0].rewardClaimed, Is.True);
            Assert.That(actual.completedTutorialSteps,
                Is.EqualTo(new[] { "tutorial.swap", "tutorial.booster" }));
            Assert.That(actual.completedStoryScenes,
                Is.EqualTo(new[]
                {
                    "story.chapter01.intro",
                    "story.chapter01.finale"
                }));
            Assert.That(actual.acknowledgedChapterCompletions,
                Is.EqualTo(new[] { 1, 2 }));
            Assert.That(actual.boosterMace, Is.EqualTo(13));
            Assert.That(actual.boosterFirebird, Is.EqualTo(14));
            Assert.That(actual.boosterLivingLight, Is.EqualTo(15));
            Assert.That(actual.boosterSeaPearl, Is.EqualTo(16));
            Assert.That(actual.lastDailyRewardUtc,
                Is.EqualTo("2031-04-05T06:07:08.0000000Z"));
            Assert.That(actual.music, Is.False);
            Assert.That(actual.sfx, Is.True);
            Assert.That(actual.vibration, Is.False);
            Assert.That(actual.notifications, Is.True);
        }

        static string SaveJson(PlayerSaveData save)
        {
            return JsonUtility.ToJson(save);
        }
    }
}
