using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    internal sealed class PlayerSaveMigrationResult
    {
        internal PlayerSaveMigrationResult(
            bool succeeded,
            bool changed,
            int sourceVersion,
            PlayerSaveData data,
            string reasonCode)
        {
            Succeeded = succeeded;
            Changed = changed;
            SourceVersion = sourceVersion;
            Data = data;
            ReasonCode = reasonCode;
        }

        internal bool Succeeded { get; private set; }
        internal bool Changed { get; private set; }
        internal int SourceVersion { get; private set; }
        internal PlayerSaveData Data { get; private set; }
        internal string ReasonCode { get; private set; }
    }

    /// <summary>
    /// Deterministic, proposal-free migration of the inherited player payload.
    /// Storage envelope versions are deliberately separate from saveVersion.
    /// </summary>
    internal static class PlayerSaveMigrationRegistry
    {
        internal const int CurrentPayloadVersion = 6;
        internal const string Migrated = "SAVE_MIGRATION_APPLIED";
        internal const string AlreadyCurrent = "SAVE_MIGRATION_ALREADY_CURRENT";
        internal const string SourceMissing = "SAVE_MIGRATION_SOURCE_MISSING";
        internal const string SourceInvalid = "SAVE_MIGRATION_SOURCE_INVALID";
        internal const string FutureVersionBlocked =
            "SAVE_MIGRATION_FUTURE_VERSION_BLOCKED";
        internal const string LegacyProgressFailed =
            "SAVE_MIGRATION_LEGACY_PROGRESS_FAILED";

        internal static PlayerSaveMigrationResult Migrate(
            PlayerSaveData source,
            Func<PlayerSaveData, List<LevelProgressData>> buildLegacyProgress,
            long fallbackUtcTicks)
        {
            if (source == null)
                return Failure(SourceMissing, 0, null);
            if (source.saveVersion <= 0)
                return Failure(SourceInvalid, source.saveVersion, source);
            if (source.saveVersion > CurrentPayloadVersion)
                return Failure(
                    FutureVersionBlocked,
                    source.saveVersion,
                    source
                );
            if (fallbackUtcTicks <= 0)
                return Failure(SourceInvalid, source.saveVersion, source);

            PlayerSaveData candidate;
            try
            {
                candidate = JsonUtility.FromJson<PlayerSaveData>(
                    JsonUtility.ToJson(source)
                );
            }
            catch
            {
                return Failure(SourceInvalid, source.saveVersion, source);
            }
            if (candidate == null)
                return Failure(SourceInvalid, source.saveVersion, source);

            int sourceVersion = candidate.saveVersion;
            bool changed = false;

            if (candidate.saveVersion < 2)
            {
                candidate.saveVersion = 2;
                if (candidate.energyMax <= 0) candidate.energyMax = 5;

                candidate.boosterMace = candidate.boosterMace <= 0
                    ? 7
                    : candidate.boosterMace;
                candidate.boosterFirebird = candidate.boosterFirebird <= 0
                    ? 5
                    : candidate.boosterFirebird;
                candidate.boosterLivingLight = candidate.boosterLivingLight <= 0
                    ? 8
                    : candidate.boosterLivingLight;
                candidate.boosterSeaPearl = candidate.boosterSeaPearl <= 0
                    ? 10
                    : candidate.boosterSeaPearl;

                candidate.music = true;
                candidate.sfx = true;
                candidate.vibration = true;
                candidate.notifications = true;
                changed = true;
            }

            if (candidate.saveVersion < 3)
            {
                candidate.selectedChapter = Math.Max(1, candidate.chapter);
                candidate.selectedLevel = Math.Max(1, candidate.level);
                candidate.highestUnlockedLevel = Math.Max(1, candidate.level);

                try
                {
                    candidate.levelProgress = buildLegacyProgress != null
                        ? buildLegacyProgress(candidate)
                        : new List<LevelProgressData>();
                }
                catch
                {
                    return Failure(
                        LegacyProgressFailed,
                        sourceVersion,
                        source
                    );
                }
                if (candidate.levelProgress == null)
                    candidate.levelProgress = new List<LevelProgressData>();
                candidate.saveVersion = 3;
                changed = true;
            }

            if (candidate.saveVersion < 4)
            {
                if (candidate.completedTutorialSteps == null)
                    candidate.completedTutorialSteps = new List<string>();
                candidate.saveVersion = 4;
                changed = true;
            }

            if (candidate.saveVersion < 5)
            {
                if (candidate.completedStoryScenes == null)
                    candidate.completedStoryScenes = new List<string>();
                candidate.saveVersion = 5;
                changed = true;
            }

            if (candidate.saveVersion < 6)
            {
                if (candidate.acknowledgedChapterCompletions == null)
                {
                    candidate.acknowledgedChapterCompletions =
                        new List<int>();
                }
                candidate.saveVersion = 6;
                changed = true;
            }

            if (candidate.energyMax <= 0)
            {
                candidate.energyMax = 5;
                changed = true;
            }
            if (candidate.lastEnergyUtcTicks <= 0)
            {
                candidate.lastEnergyUtcTicks = fallbackUtcTicks;
                changed = true;
            }

            return new PlayerSaveMigrationResult(
                true,
                changed,
                sourceVersion,
                candidate,
                changed ? Migrated : AlreadyCurrent
            );
        }

        static PlayerSaveMigrationResult Failure(
            string reasonCode,
            int sourceVersion,
            PlayerSaveData source)
        {
            return new PlayerSaveMigrationResult(
                false,
                false,
                sourceVersion,
                source,
                reasonCode
            );
        }
    }
}
