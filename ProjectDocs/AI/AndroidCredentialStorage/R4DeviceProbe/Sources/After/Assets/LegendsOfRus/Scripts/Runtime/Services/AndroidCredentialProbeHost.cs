#if UNITY_EDITOR || (UNITY_ANDROID && DEVELOPMENT_BUILD)
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;

namespace LegendsOfRus
{
    // Called only on the attached JNI worker thread. The reviewed Java plugins are unchanged.
    internal sealed class AndroidCredentialProbeHost : ICredentialProbeHost, IDisposable
    {
        internal const string DirectoryName = "lor-credential-development-probe";
        internal const string ReportName = "lor-credential-probe-report.json";
        const string Alias = "lor.backend.credentials.v1";
        const string RecordName = "lor-backend-credentials-v1.bin";
        static readonly byte[] Partial = Encoding.ASCII.GetBytes("LOR DISPOSABLE PARTIAL WRITE V1");
        readonly AndroidJavaObject context;
        readonly string directory, record, marker;
        readonly bool configured;
        AndroidJavaObject pendingAtomic, pendingOutput;
        public bool Enabled { get; }
        public bool AccountConfigured => configured;
        public int ProcessId { get; }
        public bool OwnershipDirectoryExists => Directory.Exists(directory);
        public bool StoreEmpty => !File.Exists(record) && !File.Exists(record + ".new") && !File.Exists(record + ".bak") && !HasKey();

        internal AndroidCredentialProbeHost(AndroidJavaObject context, bool accountConfigured)
        {
            this.context = context; configured = accountConfigured;
            using (var info = context.Call<AndroidJavaObject>("getApplicationInfo"))
                Enabled = context.Call<string>("getPackageName") == "com.example.legendsofrus.v117debug" && (info.Get<int>("flags") & 2) != 0;
            using (var process = new AndroidJavaClass("android.os.Process")) ProcessId = process.CallStatic<int>("myPid");
            using (var file = context.Call<AndroidJavaObject>("getNoBackupFilesDir"))
            {
                string root = file.Call<string>("getAbsolutePath");
                directory = Path.Combine(root, DirectoryName); record = Path.Combine(root, RecordName);
                marker = Path.Combine(directory, "state.json");
            }
        }
        AndroidJavaObject KeyStore()
        {
            using (var type = new AndroidJavaClass("java.security.KeyStore"))
            {
                var store = type.CallStatic<AndroidJavaObject>("getInstance", "AndroidKeyStore");
                try { store.Call("load", new object[] { null }); return store; }
                catch { store.Dispose(); throw; }
            }
        }
        bool HasKey() { using (var store = KeyStore()) return store.Call<bool>("containsAlias", Alias); }
        public CredentialProbeState LoadState()
        {
            if (!File.Exists(marker) || new FileInfo(marker).Length > 2048) return null;
            var state = JsonUtility.FromJson<CredentialProbeState>(File.ReadAllText(marker));
            return state != null && state.Valid ? state : null;
        }
        public void SaveState(CredentialProbeState state, bool create)
        {
            Require(state != null && state.Valid);
            if (create)
            {
                Require(!OwnershipDirectoryExists && StoreEmpty);
                Directory.CreateDirectory(directory);
            }
            else Require(LoadState()?.runId == state.runId);
            AtomicText(marker, JsonUtility.ToJson(state));
        }
        static void AtomicText(string path, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value); string temporary = path + ".tmp";
            using (var stream = new FileStream(temporary, FileMode.Create, FileAccess.Write, FileShare.None))
            { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
            if (File.Exists(path)) File.Replace(temporary, path, null); else File.Move(temporary, path);
        }
        static string Hash(byte[] bytes)
        {
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "").ToLowerInvariant();
        }
        static string FileHash(string path)
        {
            if (!File.Exists(path)) return null;
            long length = new FileInfo(path).Length;
            if (length <= 0 || length > 16416) return "invalid";
            return Hash(File.ReadAllBytes(path));
        }
        bool Owns(CredentialProbeState state)
        {
            var saved = LoadState();
            return state != null && state.Valid && saved != null && saved.runId == state.runId && saved.recordHash == state.recordHash;
        }
        public bool RecordMatches(CredentialProbeState state, bool allowAlternate = false)
        {
            if (!Owns(state) || File.Exists(record + ".new") || File.Exists(record + ".bak")) return false;
            string hash = FileHash(record);
            return hash == state.recordHash || (allowAlternate && CredentialProbeState.Hash(state.alternateHash) && LoadState().alternateHash == state.alternateHash && hash == state.alternateHash);
        }
        public string RecordHash()
        {
            Require(LoadState() != null && !File.Exists(record + ".new") && !File.Exists(record + ".bak"));
            return FileHash(record);
        }
        bool Operation(string method, string payload, out string value)
        {
            value = null;
            Require(Enabled && !configured && LoadState() != null);
            using (var type = new AndroidJavaClass("com.legendsofrus.credentials.AndroidCredentialBridge"))
            using (var operation = payload == null ? type.CallStatic<AndroidJavaObject>(method, context) : type.CallStatic<AndroidJavaObject>(method, context, payload))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                while (!operation.Get<bool>("done"))
                {
                    if (watch.Elapsed.TotalSeconds > 30) throw new TimeoutException("probe_timeout");
                    Thread.Sleep(20);
                }
                bool success = operation.Get<bool>("success");
                if (success) value = operation.Get<string>("value");
                return success;
            }
        }
        public bool Read(out string value) => Operation("beginRead", null, out value);
        public bool Write(string value) => Operation("beginWrite", value, out _);
        public void BeginInterruptedWrite(CredentialProbeState state)
        {
            Require(RecordMatches(state) && HasKey());
            using (var file = new AndroidJavaObject("java.io.File", record))
                pendingAtomic = new AndroidJavaObject("android.util.AtomicFile", file);
            pendingOutput = pendingAtomic.Call<AndroidJavaObject>("startWrite");
            pendingOutput.Call("write", Partial);
            using (var fd = pendingOutput.Call<AndroidJavaObject>("getFD")) fd.Call("sync");
            // Deliberately leave the write open. The host runner force-stops this process.
        }
        public bool InterruptedFilesMatch(CredentialProbeState state)
        {
            if (!Owns(state)) return false;
            string main = FileHash(record), next = FileHash(record + ".new"), backup = FileHash(record + ".bak");
            string partialHash = Hash(Partial);
            // AtomicFile uses .bak on older Android and .new on newer Android.
            return (main == state.recordHash && next == partialHash && backup == null) ||
                (main == partialHash && backup == state.recordHash && next == null);
        }
        public byte[] Corrupt(CredentialProbeState state)
        {
            Require(RecordMatches(state));
            byte[] original = File.ReadAllBytes(record); byte[] corrupt = (byte[])original.Clone(); corrupt[corrupt.Length - 1] ^= 0x40;
            state.alternateHash = Hash(corrupt); SaveState(state, false);
            WriteBytes(record, corrupt); return original;
        }
        public void Restore(CredentialProbeState state, byte[] original)
        {
            Require(Owns(state) && Hash(original) == state.recordHash && FileHash(record) == state.alternateHash && RecordMatches(state, true));
            WriteBytes(record, original); state.alternateHash = null; SaveState(state, false);
        }
        static void WriteBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            { stream.Write(bytes, 0, bytes.Length); stream.Flush(true); }
        }
        public void RemoveKey(CredentialProbeState state)
        {
            Require(RecordMatches(state));
            using (var store = KeyStore()) store.Call("deleteEntry", Alias);
            Require(!HasKey());
        }
        public void Cleanup(CredentialProbeState state)
        {
            Require(RecordMatches(state, true));
            using (var store = KeyStore()) store.Call("deleteEntry", Alias);
            Require(!HasKey() && RecordMatches(state, true));
            File.Delete(record);
            Require(StoreEmpty && Owns(state));
            File.Delete(marker);
            if (File.Exists(marker + ".tmp")) File.Delete(marker + ".tmp");
            Directory.Delete(directory, false);
        }
        internal void WriteReport(CredentialProbeReport report)
        {
            Require(Enabled);
            AtomicText(Path.Combine(Path.GetDirectoryName(directory), ReportName), JsonUtility.ToJson(report, true));
        }
        public void Dispose()
        {
            // Java global references stay retained while an interrupted write is awaiting force-stop.
            if (pendingOutput != null) return;
            pendingAtomic?.Dispose(); context.Dispose();
        }
        static void Require(bool condition) { if (!condition) throw new InvalidOperationException("probe_ownership_or_state"); }
    }
}
#endif
