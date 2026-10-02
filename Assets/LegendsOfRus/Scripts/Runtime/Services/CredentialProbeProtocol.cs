#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;

namespace LegendsOfRus
{
    [Serializable] public sealed class CredentialProbeState
    {
        public int version = 1;
        public string runId, stage, recordHash, alternateHash;
        public int processId;
        public string Payload => "LegendsOfRus disposable credential probe v1 " + runId;
        public bool Valid => version == 1 && Guid.TryParseExact(runId, "N", out _) && processId > 0 &&
            (stage == "initializing" || stage == "restart" || stage == "interrupt" || stage == "corrupt" || stage == "keymissing") &&
            (stage == "initializing" || Hash(recordHash)) && (string.IsNullOrEmpty(alternateHash) || Hash(alternateHash));
        public static bool Hash(string text)
        {
            if (text == null || text.Length != 64) return false;
            foreach (char c in text) if (!(c >= '0' && c <= '9' || c >= 'a' && c <= 'f')) return false;
            return true;
        }
    }
    [Serializable] public sealed class CredentialProbeReport
    {
        public int version = 1;
        public string command, status;
        public string[] checks;
        public bool authenticatedSessionTested = false;
    }
    public interface ICredentialProbeHost
    {
        bool Enabled { get; }
        bool AccountConfigured { get; }
        int ProcessId { get; }
        bool OwnershipDirectoryExists { get; }
        bool StoreEmpty { get; } // Includes base, sidecars AND alias existence; no credential read.
        CredentialProbeState LoadState();
        void SaveState(CredentialProbeState state, bool create);
        bool RecordMatches(CredentialProbeState state, bool allowAlternate = false);
        string RecordHash();
        bool Read(out string value);
        bool Write(string value);
        void BeginInterruptedWrite(CredentialProbeState state);
        bool InterruptedFilesMatch(CredentialProbeState state);
        byte[] Corrupt(CredentialProbeState state); // Records expected alternate hash BEFORE mutation.
        void Restore(CredentialProbeState state, byte[] original);
        void RemoveKey(CredentialProbeState state);
        void Cleanup(CredentialProbeState state); // Must validate ownership before each destructive step.
    }
    public static class CredentialProbeProtocol
    {
        public static bool KnownCommand(string command) => command == "prepare" || command == "restart" || command == "verify" || command == "cleanup";
        public static CredentialProbeReport Run(string command, ICredentialProbeHost host)
        {
            var checks = new List<string>();
            var report = new CredentialProbeReport { command = KnownCommand(command) ? command : "invalid", status = "refused", checks = new string[0] };
            if (!KnownCommand(command) || !host.Enabled || host.AccountConfigured) return report;
            try
            {
                if (command == "prepare")
                {
                    // Never read or overwrite an existing vault, even if its content looks synthetic.
                    if (host.OwnershipDirectoryExists || !host.StoreEmpty) return report;
                    var state = new CredentialProbeState { runId = Guid.NewGuid().ToString("N"), stage = "initializing", processId = host.ProcessId };
                    host.SaveState(state, true);
                    Require(host.Read(out var empty) && empty == null); checks.Add("pristine_read");
                    Require(host.Write(state.Payload)); checks.Add("write");
                    Require(host.Read(out var value) && value == state.Payload); checks.Add("read_back");
                    state.recordHash = host.RecordHash(); Require(CredentialProbeState.Hash(state.recordHash));
                    state.stage = "restart"; host.SaveState(state, false);
                    report.status = "awaiting_restart";
                }
                else
                {
                    if (!host.OwnershipDirectoryExists) return report;
                    var state = host.LoadState();
                    if (state == null || !state.Valid || state.stage == "initializing") return report;
                    if (command == "cleanup")
                    {
                        if (state.stage == "interrupt" || !host.RecordMatches(state, true)) return report;
                        host.Cleanup(state); report.status = "cleaned";
                    }
                    else if (command == "restart")
                    {
                        if (state.stage != "restart" || state.processId == host.ProcessId || !host.RecordMatches(state)) return report;
                        Require(host.Read(out var value) && value == state.Payload); checks.Add("process_restart_read");
                        state.stage = "interrupt"; state.processId = host.ProcessId; host.SaveState(state, false);
                        host.BeginInterruptedWrite(state);
                        Require(host.InterruptedFilesMatch(state));
                        report.status = "awaiting_force_stop";
                    }
                    else
                    {
                        if (state.stage != "interrupt" || state.processId == host.ProcessId || !host.InterruptedFilesMatch(state)) return report;
                        Require(host.Read(out var value) && value == state.Payload && host.RecordMatches(state)); checks.Add("interrupted_write_recovery");
                        state.stage = "corrupt";
                        byte[] original = host.Corrupt(state);
                        Require(!host.Read(out _) && !host.Write(state.Payload) && host.RecordMatches(state, true)); checks.Add("corruption_rejected");
                        host.Restore(state, original);
                        Require(host.Read(out value) && value == state.Payload); checks.Add("owned_fixture_restored");
                        state.stage = "keymissing"; state.alternateHash = null; host.SaveState(state, false);
                        host.RemoveKey(state);
                        Require(!host.Read(out _) && !host.Write(state.Payload) && host.RecordMatches(state)); checks.Add("missing_key_rejected");
                        host.Cleanup(state); checks.Add("owned_fixture_cleanup");
                        Require(host.StoreEmpty && !host.OwnershipDirectoryExists);
                        report.status = "passed";
                    }
                }
            }
            catch { report.status = "failed_retained"; } // No exception/payload/record/key material in results.
            report.checks = checks.ToArray();
            return report;
        }
        static void Require(bool condition) { if (!condition) throw new InvalidOperationException("probe_check_failed"); }
    }
}
#endif
