using System;
using LegendsOfRus.SaveCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.SaveRecovery
{
    [TestFixture]
    public sealed class SaveJournalLoadEditModeTests
    {
        static readonly SaveJournalKeys Keys = new SaveJournalKeys("save.primary", "save.backup", "save.staging");
        readonly SaveEnvelopeCodec codec = new SaveEnvelopeCodec();

        [Test]
        public void MissingPrimaryAndJournal_ReturnsExplicitNoSave()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.NoSave));
            Assert.That(result.Success, Is.False);
            Assert.That(result.PrimaryWasMissing, Is.True);
            Assert.That(result.PrimaryState, Is.EqualTo(SaveCandidateState.Missing));
            Assert.That(result.Source, Is.EqualTo(SaveJournalSlot.None));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_PRIMARY_MISSING_NO_COMMITTED_SAVE"));
        }

        [Test]
        public void SupportedPrimary_IsAuthoritative()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("primary", 6, 4));
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 3));

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.LoadedPrimary, SaveJournalSlot.Primary, "primary", 6, 4);
            Assert.That(result.PrimaryWasMissing, Is.False);
        }

        [Test]
        public void SupportedPrimary_WinsEvenWhenStagingHasNewerRevision()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("committed", 6, 4));
            store.SeedDurable(Keys.StagingKey, Envelope("uncommitted", 6, 99));

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.LoadedPrimary, SaveJournalSlot.Primary, "committed", 6, 4);
            Assert.That(result.StagingState, Is.EqualTo(SaveCandidateState.Valid));
        }

        [Test]
        public void LegacyPrimary_IsReturnedThroughInjectedInspector()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            const string legacy = "LEGACY|v=5|{opaque-body}";
            store.SeedDurable(Keys.PrimaryKey, legacy);

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.LoadedPrimary, SaveJournalSlot.Primary, legacy, 5, 0);
            Assert.That(result.Format, Is.EqualTo(SavePayloadFormat.Legacy));
        }

        [Test]
        public void CorruptPrimary_RecoversCommittedBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("primary", 6, 4));
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 3));
            store.CorruptDurable(Keys.PrimaryKey);

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.RecoveredBackup, SaveJournalSlot.Backup, "backup", 6, 3);
            Assert.That(result.PrimaryState, Is.EqualTo(SaveCandidateState.Invalid));
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_BACKUP_RECOVERED_PRIMARY_INVALID"));
        }

        [Test]
        public void MissingPrimary_NeverResurrectsBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 3));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.NoSave));
            Assert.That(result.Success, Is.False);
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.PrimaryWasMissing, Is.True);
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_PRIMARY_MISSING_NO_COMMITTED_SAVE"));
        }

        [Test]
        public void StagingOnly_IsNeverPromotedWhenPrimaryIsMissing()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.StagingKey, Envelope("uncommitted-secret", 6, 5));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.NoSave));
            Assert.That(result.Success, Is.False);
            Assert.That(result.Source, Is.EqualTo(SaveJournalSlot.None));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.StagingState, Is.EqualTo(SaveCandidateState.Valid));
            Assert.That(result.ToString(), Does.Not.Contain("uncommitted-secret"));
        }

        [Test]
        public void NewerStaging_BlocksStaleBackupAfterPrimaryCorruption()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("old-history", 6, 5));
            store.SeedDurable(
                Keys.StagingKey,
                Envelope("new-reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_HISTORY_REPLACEMENT_INTERRUPTED"));
        }

        [Test]
        public void NewerOrdinaryStaging_DoesNotSacrificeLastKnownGoodBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 5));
            store.SeedDurable(Keys.StagingKey, Envelope("uncommitted", 6, 6));

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.RecoveredBackup, SaveJournalSlot.Backup, "backup", 6, 5);
        }

        [Test]
        public void EqualRevisionReplacementStaging_DoesNotBlockReplacedBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            // A committed history replacement writes the identical envelope to backup.
            store.SeedDurable(Keys.BackupKey, Envelope("reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));
            store.SeedDurable(
                Keys.StagingKey,
                Envelope("reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.RecoveredBackup, SaveJournalSlot.Backup, "reset", 6, 6);
        }

        [Test]
        public void EqualRevisionReplacementStagingWithOrdinaryBackup_BlocksInconsistentIntent()
        {
            var store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("reset", 6, 6));
            store.SeedDurable(Keys.StagingKey, Envelope("reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));
            var result = Repository(store).Load();
            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable));
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_HISTORY_REPLACEMENT_INTERRUPTED"));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [Test]
        public void EqualRevisionReplacementStagingWithDifferentContent_BlocksInconsistentBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(
                Keys.BackupKey,
                Envelope("unexpected-history", 6, 6, SaveEnvelopeIntent.ReplaceHistory));
            store.SeedDurable(
                Keys.StagingKey,
                Envelope("intended-reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_HISTORY_REPLACEMENT_INTERRUPTED"));
        }

        [Test]
        public void InvalidStaging_DoesNotBlockCommittedBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 3));
            store.SeedDurable(Keys.StagingKey, "TRUNCATED");

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.RecoveredBackup, SaveJournalSlot.Backup, "backup", 6, 3);
            Assert.That(result.StagingState, Is.EqualTo(SaveCandidateState.Invalid));
        }

        [TestCase(SaveJournalSlot.Primary)]
        [TestCase(SaveJournalSlot.Backup)]
        [TestCase(SaveJournalSlot.Staging)]
        public void FuturePayloadEvidenceInAnySlot_BlocksLoad(SaveJournalSlot futureSlot)
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("supported", 6, 5));
            Seed(store, futureSlot, Envelope("future-secret", 7, 6));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.UnsupportedFutureVersion));
            Assert.That(result.Success, Is.False);
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.ToString(), Does.Not.Contain("future-secret"));
        }

        [Test]
        public void FutureLegacyPayload_BlocksLoad()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "LEGACY|v=7|{future}");
            store.SeedDurable(Keys.BackupKey, Envelope("supported", 6, 1));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.UnsupportedFutureVersion));
            Assert.That(result.Payload, Is.Null);
        }

        [Test]
        public void DifferentlyShapedFutureEnvelope_BlocksOlderBackupRecovery()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("older-backup", 6, 4));
            store.SeedDurable(Keys.StagingKey, DifferentlyShapedFutureEnvelope());

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.UnsupportedFutureVersion));
            Assert.That(result.Success, Is.False);
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.StagingState, Is.EqualTo(SaveCandidateState.UnsupportedFuture));
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_FUTURE_VERSION_BLOCKED"));
        }

        [Test]
        public void OversizedFutureEnvelope_StillBlocksOlderBackupRecovery()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("older-backup", 6, 4));
            store.SeedDurable(
                Keys.StagingKey,
                SaveEnvelopeCodec.EnvelopeMarker
                    + "\nformat=2\n"
                    + new string('x', SaveEnvelopeCodec.MaximumEnvelopeCharacters));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.UnsupportedFutureVersion));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.StagingState, Is.EqualTo(SaveCandidateState.UnsupportedFuture));
        }

        [Test]
        public void TamperedFutureClaim_IsCorruptRatherThanTrustedAsFuture()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            string tampered = Envelope("payload", 6, 2).Replace("payloadVersion=6", "payloadVersion=7");
            store.SeedDurable(Keys.PrimaryKey, tampered);
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 1));

            SaveJournalLoadResult result = Repository(store).Load();

            AssertSuccess(result, SaveJournalLoadStatus.RecoveredBackup, SaveJournalSlot.Backup, "backup", 6, 1);
            Assert.That(result.PrimaryState, Is.EqualTo(SaveCandidateState.Invalid));
        }

        [Test]
        public void Load_IsStrictlyNonMutating()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("backup", 6, 1));

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Success, Is.True);
            Assert.That(store.MutationOperationCount, Is.Zero);
            Assert.That(store.MutationLog, Is.Empty);
        }

        [TestCase("save.primary")]
        [TestCase("save.backup")]
        [TestCase("save.staging")]
        public void AnyStorageReadFailure_FailsClosed(string failingKey)
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("supported", 6, 1));
            store.FailingReadKey = failingKey;

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.StorageFailure));
            Assert.That(result.Payload, Is.Null);
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_STORAGE_READ_FAILED"));
        }

        [Test]
        public void InvalidJournalWithoutRecovery_IsUnrecoverable()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "not-a-save");

            SaveJournalLoadResult result = Repository(store).Load();

            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable));
            Assert.That(result.Success, Is.False);
            Assert.That(result.PrimaryState, Is.EqualTo(SaveCandidateState.Invalid));
        }

        [Test]
        public void JournalKeysRejectDuplicateOrMissingKeys()
        {
            Assert.Throws<ArgumentException>(() => new SaveJournalKeys("", "b", "s"));
            Assert.Throws<ArgumentException>(() => new SaveJournalKeys("p", "p", "s"));
            Assert.Throws<ArgumentException>(() => new SaveJournalKeys("p", "b", "b"));
        }

        SaveJournalRepository Repository(DurablePendingFakeStore store)
        {
            return new SaveJournalRepository(store, codec, new FakeLegacyPayloadInspector(), Keys, 6);
        }

        string Envelope(string payload, int version, long revision)
        {
            return Envelope(payload, version, revision, SaveEnvelopeIntent.Ordinary);
        }

        string Envelope(string payload, int version, long revision, SaveEnvelopeIntent intent)
        {
            SaveEnvelopeEncodeResult result = codec.TryEncode(payload, version, revision, intent);
            Assert.That(result.Success, Is.True);
            return result.Envelope;
        }

        static void Seed(DurablePendingFakeStore store, SaveJournalSlot slot, string value)
        {
            string key = slot == SaveJournalSlot.Primary
                ? Keys.PrimaryKey
                : slot == SaveJournalSlot.Backup
                    ? Keys.BackupKey
                    : Keys.StagingKey;
            store.SeedDurable(key, value);
        }

        static string DifferentlyShapedFutureEnvelope()
        {
            return SaveEnvelopeCodec.EnvelopeMarker
                + "\nformat=2\nfutureHeader=alpha\nfuturePayload=opaque";
        }

        static void AssertSuccess(
            SaveJournalLoadResult result,
            SaveJournalLoadStatus status,
            SaveJournalSlot slot,
            string payload,
            int version,
            long revision)
        {
            Assert.That(result.Success, Is.True);
            Assert.That(result.Status, Is.EqualTo(status));
            Assert.That(result.Source, Is.EqualTo(slot));
            Assert.That(result.Payload, Is.EqualTo(payload));
            Assert.That(result.PayloadVersion, Is.EqualTo(version));
            Assert.That(result.Revision, Is.EqualTo(revision));
        }
    }
}
