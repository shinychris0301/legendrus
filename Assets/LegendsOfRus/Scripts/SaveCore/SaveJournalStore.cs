using System;

namespace LegendsOfRus.SaveCore
{
    public interface ISaveJournalStore
    {
        bool TryRead(string key, out string value);
        void Write(string key, string value);
        void Delete(string key);
        void Flush();
    }

    public sealed class SaveJournalKeys
    {
        public SaveJournalKeys(string primaryKey, string backupKey, string stagingKey)
        {
            if (string.IsNullOrWhiteSpace(primaryKey))
                throw new ArgumentException("A primary save key is required.", "primaryKey");
            if (string.IsNullOrWhiteSpace(backupKey))
                throw new ArgumentException("A backup save key is required.", "backupKey");
            if (string.IsNullOrWhiteSpace(stagingKey))
                throw new ArgumentException("A staging save key is required.", "stagingKey");
            if (string.Equals(primaryKey, backupKey, StringComparison.Ordinal)
                || string.Equals(primaryKey, stagingKey, StringComparison.Ordinal)
                || string.Equals(backupKey, stagingKey, StringComparison.Ordinal))
            {
                throw new ArgumentException("Save journal keys must be distinct.");
            }

            PrimaryKey = primaryKey;
            BackupKey = backupKey;
            StagingKey = stagingKey;
        }

        public string PrimaryKey { get; private set; }
        public string BackupKey { get; private set; }
        public string StagingKey { get; private set; }
    }

    public enum LegacyPayloadInspectionStatus
    {
        NotRecognized = 0,
        Valid = 1,
        Invalid = 2
    }

    public sealed class LegacyPayloadInspection
    {
        LegacyPayloadInspection(LegacyPayloadInspectionStatus status, int payloadVersion)
        {
            Status = status;
            PayloadVersion = payloadVersion;
        }

        public LegacyPayloadInspectionStatus Status { get; private set; }
        public int PayloadVersion { get; private set; }

        public static LegacyPayloadInspection NotRecognized()
        {
            return new LegacyPayloadInspection(LegacyPayloadInspectionStatus.NotRecognized, 0);
        }

        public static LegacyPayloadInspection Invalid()
        {
            return new LegacyPayloadInspection(LegacyPayloadInspectionStatus.Invalid, 0);
        }

        public static LegacyPayloadInspection Valid(int payloadVersion)
        {
            return new LegacyPayloadInspection(LegacyPayloadInspectionStatus.Valid, payloadVersion);
        }
    }

    public interface ILegacySavePayloadInspector
    {
        LegacyPayloadInspection Inspect(string serializedPayload);
    }
}
