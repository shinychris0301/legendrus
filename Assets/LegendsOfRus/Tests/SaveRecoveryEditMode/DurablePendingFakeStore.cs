using System;
using System.Collections.Generic;
using LegendsOfRus.SaveCore;

namespace LegendsOfRus.Tests.SaveRecovery
{
    internal sealed class DurablePendingFakeStore : ISaveJournalStore
    {
        readonly Dictionary<string, string> durable = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly Dictionary<string, string> pending = new Dictionary<string, string>(StringComparer.Ordinal);
        readonly List<string> mutationLog = new List<string>();
        int mutationOperationCount;

        public int FailAtMutationOperation { get; set; }
        public string FailingReadKey { get; set; }
        public int MutationOperationCount { get { return mutationOperationCount; } }
        public IList<string> MutationLog { get { return mutationLog.AsReadOnly(); } }

        public bool TryRead(string key, out string value)
        {
            if (string.Equals(FailingReadKey, key, StringComparison.Ordinal))
                throw new InvalidOperationException("Injected read failure.");
            return pending.TryGetValue(key, out value);
        }

        public void Write(string key, string value)
        {
            BeforeMutation("Write:" + key);
            pending[key] = value;
        }

        public void Delete(string key)
        {
            BeforeMutation("Delete:" + key);
            pending.Remove(key);
        }

        public void Flush()
        {
            BeforeMutation("Flush");
            Copy(pending, durable);
        }

        public void SeedDurable(string key, string value)
        {
            durable[key] = value;
            pending[key] = value;
        }

        public void RemoveDurable(string key)
        {
            durable.Remove(key);
            pending.Remove(key);
        }

        public void CorruptDurable(string key)
        {
            durable[key] = "CORRUPT_SAVE_RECORD";
            pending[key] = durable[key];
        }

        public bool TryReadDurable(string key, out string value)
        {
            return durable.TryGetValue(key, out value);
        }

        public DurablePendingFakeStore CrashRestart()
        {
            DurablePendingFakeStore restarted = new DurablePendingFakeStore();
            Copy(durable, restarted.durable);
            Copy(durable, restarted.pending);
            return restarted;
        }

        void BeforeMutation(string operation)
        {
            mutationOperationCount++;
            mutationLog.Add(operation);
            if (FailAtMutationOperation > 0 && mutationOperationCount == FailAtMutationOperation)
                throw new InvalidOperationException("Injected mutation failure.");
        }

        static void Copy(Dictionary<string, string> source, Dictionary<string, string> destination)
        {
            destination.Clear();
            foreach (KeyValuePair<string, string> pair in source)
                destination.Add(pair.Key, pair.Value);
        }
    }

    internal sealed class FakeLegacyPayloadInspector : ILegacySavePayloadInspector
    {
        const string Prefix = "LEGACY|v=";

        public LegacyPayloadInspection Inspect(string serializedPayload)
        {
            if (serializedPayload == null || !serializedPayload.StartsWith(Prefix, StringComparison.Ordinal))
                return LegacyPayloadInspection.NotRecognized();
            int separator = serializedPayload.IndexOf('|', Prefix.Length);
            if (separator < 0)
                return LegacyPayloadInspection.Invalid();
            int version;
            if (!int.TryParse(serializedPayload.Substring(Prefix.Length, separator - Prefix.Length), out version))
                return LegacyPayloadInspection.Invalid();
            return LegacyPayloadInspection.Valid(version);
        }
    }
}
