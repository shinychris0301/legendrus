using System;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class SaveMigrationRecoveryPlayModeTests
    {
        const string PrimaryKey = "LegendsOfRus_Save_v1";
        const string BackupKey = "LegendsOfRus_Save_v1_Backup";
        const string StagingKey = "LegendsOfRus_Save_v1_Staging";

        static readonly string[] Keys =
        {
            PrimaryKey,
            BackupKey,
            StagingKey
        };

        bool[] previousPresence;
        string[] previousValues;

        [SetUp]
        public void SetUp()
        {
            previousPresence = new bool[Keys.Length];
            previousValues = new string[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
            {
                previousPresence[i] = PlayerPrefs.HasKey(Keys[i]);
                previousValues[i] = previousPresence[i]
                    ? PlayerPrefs.GetString(Keys[i])
                    : string.Empty;
                PlayerPrefs.DeleteKey(Keys[i]);
            }
            PlayerPrefs.Save();
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < Keys.Length; i++)
            {
                PlayerPrefs.DeleteKey(Keys[i]);
                if (previousPresence[i])
                    PlayerPrefs.SetString(Keys[i], previousValues[i]);
            }
            PlayerPrefs.Save();
        }

        [Test]
        public void RawLegacyLoad_IsNonMutatingAndPreservesDeclaredVersion()
        {
            PlayerSaveData legacy = CreateSave(5, 2718);
            string raw = JsonUtility.ToJson(legacy);
            PlayerPrefs.SetString(PrimaryKey, raw);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(result.Status, Is.EqualTo(PlayerSaveLoadStatus.LoadedPrimary));
            Assert.That(result.Data.saveVersion, Is.EqualTo(5));
            Assert.That(result.Data.gold, Is.EqualTo(2718));
            Assert.That(result.RequiresRewrite, Is.True);
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(raw));
            Assert.That(PlayerPrefs.HasKey(BackupKey), Is.False);
            Assert.That(PlayerPrefs.HasKey(StagingKey), Is.False);
        }

        [Test]
        public void UnversionedLegacyPayload_MapsToV1WithoutReadMutation()
        {
            const string raw =
                "{\"gold\":77,\"gems\":4,\"energy\":2," +
                "\"chapter\":1,\"level\":2}";
            PlayerPrefs.SetString(PrimaryKey, raw);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(result.Status, Is.EqualTo(PlayerSaveLoadStatus.LoadedPrimary));
            Assert.That(result.Data.saveVersion, Is.EqualTo(1));
            Assert.That(result.Data.gold, Is.EqualTo(77));
            Assert.That(result.RequiresRewrite, Is.True);
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(raw));
        }

        [Test]
        public void ExplicitZeroPayloadVersion_IsInvalidAndWriteLocked()
        {
            const string raw = "{\"saveVersion\":0,\"gold\":91}";
            PlayerPrefs.SetString(PrimaryKey, raw);
            PlayerPrefs.Save();

            PlayerSaveLoadResult load = SaveService.LoadWithResult();
            PlayerSaveWriteResult write;
            bool saved = SaveService.TrySave(CreateSave(6, 600), out write);

            Assert.That(load.Status, Is.EqualTo(PlayerSaveLoadStatus.InvalidPayload));
            Assert.That(saved, Is.False);
            Assert.That(write.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(raw));
        }

        [Test]
        public void CorruptPrimary_RecoversLastCommittedBackupWithoutReadMutation()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 100), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            Assert.That(
                SaveService.TrySave(CreateSave(6, 200), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            string backupBefore = PlayerPrefs.GetString(BackupKey);
            PlayerPrefs.SetString(PrimaryKey, "{corrupt-primary");
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(result.Status, Is.EqualTo(PlayerSaveLoadStatus.RecoveredBackup));
            Assert.That(result.Data.gold, Is.EqualTo(100));
            Assert.That(result.RequiresRewrite, Is.True);
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo("{corrupt-primary"));
            Assert.That(PlayerPrefs.GetString(BackupKey), Is.EqualTo(backupBefore));
        }

        [Test]
        public void MalformedBalancedLegacyPrimary_FallsBackToValidBackup()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 444), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            const string malformed =
                "{\"saveVersion\":6 \"gold\":999}";
            PlayerPrefs.SetString(PrimaryKey, malformed);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(
                result.Status,
                Is.EqualTo(PlayerSaveLoadStatus.RecoveredBackup)
            );
            Assert.That(result.Data.gold, Is.EqualTo(444));
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(malformed));
        }

        [Test]
        public void PartialRawV6Primary_FallsBackToValidBackupWithoutMutation()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 616), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            string backupBefore = PlayerPrefs.GetString(BackupKey);
            const string partial = "{\"saveVersion\":6}";
            PlayerPrefs.SetString(PrimaryKey, partial);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(
                result.Status,
                Is.EqualTo(PlayerSaveLoadStatus.RecoveredBackup)
            );
            Assert.That(result.Data.saveVersion, Is.EqualTo(6));
            Assert.That(result.Data.gold, Is.EqualTo(616));
            Assert.That(result.RequiresRewrite, Is.True);
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(partial));
            Assert.That(PlayerPrefs.GetString(BackupKey), Is.EqualTo(backupBefore));
        }

        [Test]
        public void CompleteRawV6Primary_RemainsAcceptedAndReadOnly()
        {
            PlayerSaveData expected = CreateSave(6, 617);
            expected.completedTutorialSteps.Add("swap");
            expected.completedStoryScenes.Add("chapter01.intro");
            expected.acknowledgedChapterCompletions.Add(1);
            string raw = JsonUtility.ToJson(expected);
            PlayerPrefs.SetString(PrimaryKey, raw);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(result.Status, Is.EqualTo(PlayerSaveLoadStatus.LoadedPrimary));
            Assert.That(result.Data.saveVersion, Is.EqualTo(6));
            Assert.That(result.Data.gold, Is.EqualTo(617));
            Assert.That(result.Data.completedTutorialSteps, Does.Contain("swap"));
            Assert.That(result.Data.completedStoryScenes, Does.Contain("chapter01.intro"));
            Assert.That(
                result.Data.acknowledgedChapterCompletions,
                Does.Contain(1)
            );
            Assert.That(result.RequiresRewrite, Is.True);
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(raw));
            Assert.That(PlayerPrefs.HasKey(BackupKey), Is.False);
            Assert.That(PlayerPrefs.HasKey(StagingKey), Is.False);
        }

        [Test]
        public void BothCommittedCopiesCorrupt_BlockWriteAndPreserveEvidence()
        {
            const string corruptPrimary = "{broken-primary";
            const string corruptBackup = "{broken-backup";
            PlayerPrefs.SetString(PrimaryKey, corruptPrimary);
            PlayerPrefs.SetString(BackupKey, corruptBackup);
            PlayerPrefs.Save();

            PlayerSaveLoadResult load = SaveService.LoadWithResult();
            PlayerSaveWriteResult write;
            bool saved = SaveService.TrySave(CreateSave(6, 999), out write);

            Assert.That(load.Status, Is.EqualTo(PlayerSaveLoadStatus.InvalidPayload));
            Assert.That(load.CanPersist, Is.False);
            Assert.That(saved, Is.False);
            Assert.That(write.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(corruptPrimary));
            Assert.That(PlayerPrefs.GetString(BackupKey), Is.EqualTo(corruptBackup));
        }

        [Test]
        public void FuturePrimary_BlocksOlderBackupAndCannotBeDowngraded()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 100), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            const string future = "{\"saveVersion\":7}";
            PlayerPrefs.SetString(PrimaryKey, future);
            PlayerPrefs.Save();

            PlayerSaveLoadResult load = SaveService.LoadWithResult();
            bool saved = SaveService.TrySave(CreateSave(6, 600), out write);

            Assert.That(
                load.Status,
                Is.EqualTo(PlayerSaveLoadStatus.UnsupportedFutureVersion)
            );
            Assert.That(saved, Is.False);
            Assert.That(write.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
            Assert.That(PlayerPrefs.GetString(PrimaryKey), Is.EqualTo(future));
        }

        [Test]
        public void ReplaceHistory_PreventsPreResetProgressRecovery()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 54321), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );

            PlayerSaveData reset = CreateSave(6, 12540);
            reset.level = 1;
            reset.highestUnlockedLevel = 1;
            Assert.That(
                SaveService.TryReplaceHistory(reset, out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            PlayerPrefs.SetString(PrimaryKey, "{corrupt-after-reset");
            PlayerPrefs.Save();

            PlayerSaveLoadResult recovered = SaveService.LoadWithResult();

            Assert.That(
                recovered.Status,
                Is.EqualTo(PlayerSaveLoadStatus.RecoveredBackup)
            );
            Assert.That(recovered.Data.gold, Is.EqualTo(12540));
            Assert.That(recovered.Data.level, Is.EqualTo(1));
            Assert.That(recovered.Data.highestUnlockedLevel, Is.EqualTo(1));
        }

        [Test]
        public void MissingPrimary_IsExplicitFirstRunAndDoesNotResurrectBackup()
        {
            PlayerSaveWriteResult write;
            Assert.That(
                SaveService.TryReplaceHistory(CreateSave(6, 8765), out write),
                Is.True,
                write != null ? write.ToString() : string.Empty
            );
            PlayerPrefs.DeleteKey(PrimaryKey);
            PlayerPrefs.Save();

            PlayerSaveLoadResult result = SaveService.LoadWithResult();

            Assert.That(result.Status, Is.EqualTo(PlayerSaveLoadStatus.FirstRun));
            Assert.That(result.Data.gold, Is.EqualTo(12540));
            Assert.That(PlayerPrefs.HasKey(BackupKey), Is.True);
        }

        static PlayerSaveData CreateSave(int version, int gold)
        {
            return new PlayerSaveData
            {
                saveVersion = version,
                gold = gold,
                gems = 37,
                energy = 3,
                energyMax = 5,
                lastEnergyUtcTicks = new DateTime(
                    2026,
                    9,
                    2,
                    12,
                    0,
                    0,
                    DateTimeKind.Utc
                ).Ticks,
                chapter = 1,
                level = 3,
                selectedChapter = 1,
                selectedLevel = 3,
                highestUnlockedLevel = 3
            };
        }
    }
}
