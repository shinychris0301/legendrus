using System.Collections.Generic;
using LegendsOfRus.SaveCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.SaveRecovery
{
    [TestFixture]
    public sealed class SaveJournalWriteEditModeTests
    {
        static readonly SaveJournalKeys Keys = new SaveJournalKeys("save.primary", "save.backup", "save.staging");
        readonly SaveEnvelopeCodec codec = new SaveEnvelopeCodec();

        [Test]
        public void FirstSave_UsesJournalOrderWithoutInventingBackup()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();

            SaveJournalWriteResult result = Repository(store).TrySave("first", 6);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Revision, Is.EqualTo(1));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Write:" + Keys.StagingKey,
                    "Flush",
                    "Write:" + Keys.PrimaryKey,
                    "Flush",
                    "Delete:" + Keys.StagingKey,
                    "Flush"
                },
                store.MutationLog);
            AssertLoad(store, SaveJournalLoadStatus.LoadedPrimary, "first", 1);
            Assert.That(store.TryReadDurable(Keys.BackupKey, out _), Is.False);
            Assert.That(store.TryReadDurable(Keys.StagingKey, out _), Is.False);
        }

        [Test]
        public void OrdinarySave_BacksUpPriorValidPrimaryBeforePrimaryCommit()
        {
            DurablePendingFakeStore store = StoreWithHistory();

            SaveJournalWriteResult result = Repository(store).TrySave("new", 6);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Revision, Is.EqualTo(3));
            CollectionAssert.AreEqual(OrdinaryMutationOrder(), store.MutationLog);
            AssertLoad(store, SaveJournalLoadStatus.LoadedPrimary, "new", 3);
            AssertEnvelopePayload(store, Keys.BackupKey, "old", 2, SaveEnvelopeIntent.Ordinary);
            Assert.That(store.TryReadDurable(Keys.StagingKey, out _), Is.False);
        }

        [Test]
        public void OrdinarySave_DoesNotOverwriteBackupWithCorruptPrimary()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.CorruptDurable(Keys.PrimaryKey);

            SaveJournalWriteResult result = Repository(store).TrySave("new", 6);

            Assert.That(result.Success, Is.True);
            AssertEnvelopePayload(store, Keys.BackupKey, "older", 1, SaveEnvelopeIntent.Ordinary);
            AssertLoad(store, SaveJournalLoadStatus.LoadedPrimary, "new", 2);
        }

        [Test]
        public void DirectOrdinarySaveWithMissingPrimary_ReplacesStaleCompanionHistory()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.BackupKey, Envelope("stale-history", 6, 8));

            SaveJournalWriteResult result = Repository(store).TrySave("new-history", 6);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Mode, Is.EqualTo(SaveJournalWriteMode.ReplaceHistory));
            Assert.That(result.Revision, Is.EqualTo(9));
            CollectionAssert.AreEqual(ReplaceMutationOrder(), store.MutationLog);
            AssertEnvelopePayload(
                store,
                Keys.PrimaryKey,
                "new-history",
                9,
                SaveEnvelopeIntent.ReplaceHistory);
            AssertEnvelopePayload(
                store,
                Keys.BackupKey,
                "new-history",
                9,
                SaveEnvelopeIntent.ReplaceHistory);
            store.CorruptDurable(Keys.PrimaryKey);
            AssertLoad(store, SaveJournalLoadStatus.RecoveredBackup, "new-history", 9);
        }

        [Test]
        public void ReplaceHistory_CommitsNewValueToPrimaryAndBackupBeforeRemovingStage()
        {
            DurablePendingFakeStore store = StoreWithHistory();

            SaveJournalWriteResult result = Repository(store).TryReplaceHistory("reset", 6);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Mode, Is.EqualTo(SaveJournalWriteMode.ReplaceHistory));
            Assert.That(result.Revision, Is.EqualTo(3));
            Assert.That(result.PrimaryDurablyCommitted, Is.True);
            CollectionAssert.AreEqual(ReplaceMutationOrder(), store.MutationLog);
            AssertEnvelopePayload(store, Keys.PrimaryKey, "reset", 3, SaveEnvelopeIntent.ReplaceHistory);
            AssertEnvelopePayload(store, Keys.BackupKey, "reset", 3, SaveEnvelopeIntent.ReplaceHistory);
            Assert.That(store.TryReadDurable(Keys.StagingKey, out _), Is.False);
        }

        [TestCase(1, SaveJournalWritePhase.StagingWrite)]
        [TestCase(2, SaveJournalWritePhase.StagingFlush)]
        [TestCase(3, SaveJournalWritePhase.BackupWrite)]
        [TestCase(4, SaveJournalWritePhase.BackupFlush)]
        [TestCase(5, SaveJournalWritePhase.PrimaryWrite)]
        [TestCase(6, SaveJournalWritePhase.PrimaryFlush)]
        [TestCase(7, SaveJournalWritePhase.StagingDelete)]
        [TestCase(8, SaveJournalWritePhase.StagingDeleteFlush)]
        public void OrdinarySave_ReportsEveryInterruptedPhaseSafely(
            int failureOperation,
            SaveJournalWritePhase expectedPhase)
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.FailAtMutationOperation = failureOperation;

            SaveJournalWriteResult result = Repository(store).TrySave("new-secret", 6);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.StorageFailure));
            Assert.That(result.Phase, Is.EqualTo(expectedPhase));
            Assert.That(result.ToString(), Does.Not.Contain("new-secret"));

            DurablePendingFakeStore restarted = store.CrashRestart();
            SaveJournalLoadResult loaded = Repository(restarted).Load();
            Assert.That(loaded.Success, Is.True);
            Assert.That(loaded.Payload, Is.EqualTo(failureOperation <= 6 ? "old" : "new-secret"));
        }

        [Test]
        public void OrdinaryInterruptedWrite_UsesPrimaryBeforeCommitAndNewPrimaryAfterCommit()
        {
            for (int failureOperation = 1; failureOperation <= 8; failureOperation++)
            {
                DurablePendingFakeStore store = StoreWithHistory();
                store.FailAtMutationOperation = failureOperation;
                Repository(store).TrySave("new", 6);

                SaveJournalLoadResult loaded = Repository(store.CrashRestart()).Load();

                Assert.That(loaded.Success, Is.True, "operation " + failureOperation);
                Assert.That(
                    loaded.Payload,
                    Is.EqualTo(failureOperation <= 6 ? "old" : "new"),
                    "operation " + failureOperation);
            }
        }

        [Test]
        public void OrdinaryStaging_DoesNotBlockBackupRecoveryAfterInterruptedWrite()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.FailAtMutationOperation = 3; // Staging is durable; backup write is interrupted.
            Repository(store).TrySave("new", 6);
            DurablePendingFakeStore restarted = store.CrashRestart();
            restarted.CorruptDurable(Keys.PrimaryKey);

            SaveJournalLoadResult loaded = Repository(restarted).Load();

            Assert.That(loaded.Status, Is.EqualTo(SaveJournalLoadStatus.RecoveredBackup));
            Assert.That(loaded.Payload, Is.EqualTo("older"));
            Assert.That(loaded.StagingState, Is.EqualTo(SaveCandidateState.Valid));
        }

        [Test]
        public void ReplaceHistory_InterruptionMatrixNeverLoadsStagingAndSwitchesAtPrimaryCommit()
        {
            for (int failureOperation = 1; failureOperation <= 8; failureOperation++)
            {
                DurablePendingFakeStore store = StoreWithHistory();
                store.FailAtMutationOperation = failureOperation;
                Repository(store).TryReplaceHistory("reset", 6);

                SaveJournalLoadResult loaded = Repository(store.CrashRestart()).Load();

                Assert.That(loaded.Success, Is.True, "operation " + failureOperation);
                Assert.That(
                    loaded.Payload,
                    Is.EqualTo(failureOperation <= 4 ? "old" : "reset"),
                    "operation " + failureOperation);
                Assert.That(loaded.Source, Is.EqualTo(SaveJournalSlot.Primary));
            }
        }

        [TestCase(1, SaveJournalWritePhase.StagingWrite)]
        [TestCase(2, SaveJournalWritePhase.StagingFlush)]
        [TestCase(3, SaveJournalWritePhase.PrimaryWrite)]
        [TestCase(4, SaveJournalWritePhase.PrimaryFlush)]
        [TestCase(5, SaveJournalWritePhase.BackupWrite)]
        [TestCase(6, SaveJournalWritePhase.BackupFlush)]
        [TestCase(7, SaveJournalWritePhase.StagingDelete)]
        [TestCase(8, SaveJournalWritePhase.StagingDeleteFlush)]
        public void ReplaceHistory_ReportsEveryInterruptedPhase(
            int failureOperation,
            SaveJournalWritePhase expectedPhase)
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.FailAtMutationOperation = failureOperation;

            SaveJournalWriteResult result = Repository(store).TryReplaceHistory("reset", 6);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.StorageFailure));
            Assert.That(result.Phase, Is.EqualTo(expectedPhase));
            Assert.That(result.Mode, Is.EqualTo(SaveJournalWriteMode.ReplaceHistory));
            Assert.That(
                result.PrimaryDurablyCommitted,
                Is.EqualTo(failureOperation >= 5),
                "operation " + failureOperation);
        }

        [Test]
        public void ReplaceHistory_PostCommitFailureIsDistinguishedFromFullSuccess()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.FailAtMutationOperation = 5; // Primary flush succeeded; backup write failed.

            SaveJournalWriteResult result =
                Repository(store).TryReplaceHistory("reset", 6);
            SaveJournalLoadResult restarted = Repository(store.CrashRestart()).Load();

            Assert.That(result.Success, Is.False);
            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.StorageFailure));
            Assert.That(result.Phase, Is.EqualTo(SaveJournalWritePhase.BackupWrite));
            Assert.That(result.PrimaryDurablyCommitted, Is.True);
            Assert.That(restarted.Status, Is.EqualTo(SaveJournalLoadStatus.LoadedPrimary));
            Assert.That(restarted.Payload, Is.EqualTo("reset"));
        }

        [Test]
        public void ReplaceHistory_InterruptionAndCorruptionCannotResurrectOldBackup()
        {
            for (int failureOperation = 3; failureOperation <= 8; failureOperation++)
            {
                DurablePendingFakeStore store = StoreWithHistory();
                store.FailAtMutationOperation = failureOperation;
                Repository(store).TryReplaceHistory("reset", 6);
                DurablePendingFakeStore restarted = store.CrashRestart();
                restarted.CorruptDurable(Keys.PrimaryKey);

                SaveJournalLoadResult loaded = Repository(restarted).Load();

                if (failureOperation <= 6)
                {
                    Assert.That(loaded.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable),
                        "operation " + failureOperation);
                    Assert.That(loaded.Payload, Is.Null);
                    Assert.That(loaded.DiagnosticCode, Is.EqualTo("SAVE_HISTORY_REPLACEMENT_INTERRUPTED"));
                }
                else
                {
                    Assert.That(loaded.Status, Is.EqualTo(SaveJournalLoadStatus.RecoveredBackup),
                        "operation " + failureOperation);
                    Assert.That(loaded.Payload, Is.EqualTo("reset"));
                }
            }
        }

        [TestCase(SaveJournalSlot.Primary)]
        [TestCase(SaveJournalSlot.Backup)]
        [TestCase(SaveJournalSlot.Staging)]
        public void FutureEvidenceInAnySlotBlocksWritesWithoutMutation(SaveJournalSlot slot)
        {
            DurablePendingFakeStore store = StoreWithHistory();
            string key = slot == SaveJournalSlot.Primary
                ? Keys.PrimaryKey
                : slot == SaveJournalSlot.Backup
                    ? Keys.BackupKey
                    : Keys.StagingKey;
            store.SeedDurable(key, Envelope("future", 7, 3));

            SaveJournalWriteResult ordinary = Repository(store).TrySave("new", 6);
            SaveJournalWriteResult replacement = Repository(store).TryReplaceHistory("reset", 6);

            Assert.That(ordinary.Status, Is.EqualTo(SaveJournalWriteStatus.UnsupportedFutureVersion));
            Assert.That(replacement.Status, Is.EqualTo(SaveJournalWriteStatus.UnsupportedFutureVersion));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [Test]
        public void DifferentlyShapedFutureEnvelopeBlocksOrdinaryAndReplacementWrites()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.SeedDurable(
                Keys.StagingKey,
                SaveEnvelopeCodec.EnvelopeMarker
                    + "\nformat=2\nfutureHeader=alpha\nfuturePayload=opaque");

            SaveJournalWriteResult ordinary = Repository(store).TrySave("new", 6);
            SaveJournalWriteResult replacement = Repository(store).TryReplaceHistory("reset", 6);

            Assert.That(ordinary.Status, Is.EqualTo(SaveJournalWriteStatus.UnsupportedFutureVersion));
            Assert.That(ordinary.Phase, Is.EqualTo(SaveJournalWritePhase.Scan));
            Assert.That(replacement.Status, Is.EqualTo(SaveJournalWriteStatus.UnsupportedFutureVersion));
            Assert.That(replacement.Phase, Is.EqualTo(SaveJournalWritePhase.Scan));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [Test]
        public void OversizedFutureEnvelopeBlocksWriteWithoutV1ParsingOrMutation()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.SeedDurable(
                Keys.StagingKey,
                SaveEnvelopeCodec.EnvelopeMarker
                    + "\nformat=2\n"
                    + new string('x', SaveEnvelopeCodec.MaximumEnvelopeCharacters));

            SaveJournalWriteResult result = Repository(store).TrySave("new", 6);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.UnsupportedFutureVersion));
            Assert.That(result.Phase, Is.EqualTo(SaveJournalWritePhase.Scan));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [Test]
        public void StorageReadFailureBlocksWriteAtScanWithoutMutation()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.FailingReadKey = Keys.BackupKey;

            SaveJournalWriteResult result = Repository(store).TrySave("new", 6);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.StorageFailure));
            Assert.That(result.Phase, Is.EqualTo(SaveJournalWritePhase.Scan));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [TestCase(null, 6)]
        [TestCase("payload", 0)]
        [TestCase("payload", 7)]
        public void InvalidInputFailsBeforeAnyStorageMutation(string payload, int version)
        {
            DurablePendingFakeStore store = StoreWithHistory();

            SaveJournalWriteResult result = Repository(store).TrySave(payload, version);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.InvalidInput));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        [Test]
        public void HighestObservedRevisionIncludesInterruptedStage()
        {
            DurablePendingFakeStore store = StoreWithHistory();
            store.SeedDurable(Keys.StagingKey, Envelope("interrupted", 6, 9));

            SaveJournalWriteResult result = Repository(store).TrySave("retry", 6);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Revision, Is.EqualTo(10));
        }

        [Test]
        public void ExhaustedRevisionFailsWithoutMutation()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("last", 6, long.MaxValue));

            SaveJournalWriteResult result = Repository(store).TrySave("new", 6);

            Assert.That(result.Status, Is.EqualTo(SaveJournalWriteStatus.RevisionExhausted));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

        DurablePendingFakeStore StoreWithHistory()
        {
            DurablePendingFakeStore store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, Envelope("old", 6, 2));
            store.SeedDurable(Keys.BackupKey, Envelope("older", 6, 1));
            return store;
        }

        SaveJournalRepository Repository(DurablePendingFakeStore store)
        {
            return new SaveJournalRepository(store, codec, new FakeLegacyPayloadInspector(), Keys, 6);
        }

        string Envelope(string payload, int version, long revision)
        {
            SaveEnvelopeEncodeResult result = codec.TryEncode(payload, version, revision);
            Assert.That(result.Success, Is.True);
            return result.Envelope;
        }

        void AssertEnvelopePayload(
            DurablePendingFakeStore store,
            string key,
            string expectedPayload,
            long expectedRevision,
            SaveEnvelopeIntent expectedIntent)
        {
            string serialized;
            Assert.That(store.TryReadDurable(key, out serialized), Is.True);
            SaveEnvelopeDecodeResult decoded = codec.Decode(serialized, 6);
            Assert.That(decoded.Success, Is.True);
            Assert.That(decoded.Payload, Is.EqualTo(expectedPayload));
            Assert.That(decoded.Revision, Is.EqualTo(expectedRevision));
            Assert.That(decoded.Intent, Is.EqualTo(expectedIntent));
        }

        void AssertLoad(
            DurablePendingFakeStore store,
            SaveJournalLoadStatus expectedStatus,
            string expectedPayload,
            long expectedRevision)
        {
            SaveJournalLoadResult loaded = Repository(store).Load();
            Assert.That(loaded.Status, Is.EqualTo(expectedStatus));
            Assert.That(loaded.Payload, Is.EqualTo(expectedPayload));
            Assert.That(loaded.Revision, Is.EqualTo(expectedRevision));
        }

        static IList<string> OrdinaryMutationOrder()
        {
            return new[]
            {
                "Write:" + Keys.StagingKey,
                "Flush",
                "Write:" + Keys.BackupKey,
                "Flush",
                "Write:" + Keys.PrimaryKey,
                "Flush",
                "Delete:" + Keys.StagingKey,
                "Flush"
            };
        }

        static IList<string> ReplaceMutationOrder()
        {
            return new[]
            {
                "Write:" + Keys.StagingKey,
                "Flush",
                "Write:" + Keys.PrimaryKey,
                "Flush",
                "Write:" + Keys.BackupKey,
                "Flush",
                "Delete:" + Keys.StagingKey,
                "Flush"
            };
        }
    }
}
