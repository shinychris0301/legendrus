using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace LegendsOfRus
{
    internal interface IProtectedCredentialIo
    {
        IEnumerator Read(Action<bool, string> completed);
        IEnumerator Write(string value, Action<bool> completed);
    }

    // Async only: never block Unity's main thread on Keystore or disk I/O. Synchronous
    // interface methods fail closed; BackendSessionClient dispatches the async interface.
    internal sealed class AndroidBackendCredentialStore : IAsyncBackendCredentialStore
    {
        readonly IProtectedCredentialIo io;
        bool busy;
        internal AndroidBackendCredentialStore(IProtectedCredentialIo io) { this.io = io ?? throw new ArgumentNullException(nameof(io)); }
        public bool TryLoad(out BackendSessionState state) { state = null; return false; }
        public bool TrySave(BackendSessionState state) => false;

        public IEnumerator Load(Action<bool, BackendSessionState> completed)
        {
            if (busy) { completed(false, null); yield break; }
            busy = true;
            try
            {
                bool success = false; string json = null;
                yield return io.Read((ok, value) => { success = ok; json = value; });
                if (!success) { completed(false, null); yield break; }
                if (json == null) { completed(true, null); yield break; } // Explicit pristine vault only.
                var state = Decode(json);
                completed(state != null, state);
            }
            finally { busy = false; }
        }
        public IEnumerator Save(BackendSessionState state, Action<bool> completed)
        {
            if (busy) { completed(false); yield break; }
            string json = Encode(state);
            if (json == null) { completed(false); yield break; }
            busy = true;
            try { yield return io.Write(json, completed); }
            finally { busy = false; }
        }
        static string Encode(BackendSessionState state)
        {
            try { if (state == null) return null; string json = JsonUtility.ToJson(state); return Decode(json) != null ? json : null; }
            catch { return null; }
        }
        static BackendSessionState Decode(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json) || Encoding.UTF8.GetByteCount(json) > 16384) return null;
                var state = JsonUtility.FromJson<BackendSessionState>(json);
                if (state == null || state.version != 1 || !Hex(state.refreshToken) ||
                    !Uri.TryCreate(state.origin, UriKind.Absolute, out var origin) || origin.Scheme != "https" ||
                    origin.UserInfo.Length != 0 || origin.AbsolutePath != "/" || origin.Query.Length != 0 || origin.Fragment.Length != 0) return null;
                if (string.IsNullOrEmpty(state.playerId))
                    return Uuid(state.pendingRequest) && string.IsNullOrEmpty(state.nextRefreshToken) ? state : null;
                if (!Uuid(state.playerId)) return null;
                return string.IsNullOrEmpty(state.pendingRequest)
                    ? (string.IsNullOrEmpty(state.nextRefreshToken) ? state : null)
                    : (Uuid(state.pendingRequest) && Hex(state.nextRefreshToken) && state.nextRefreshToken != state.refreshToken ? state : null);
            }
            catch { return null; }
        }
        static bool Uuid(string value) => Guid.TryParseExact(value, "D", out var id) && id.ToString() == value;
        static bool Hex(string value)
        {
            if (value == null || value.Length != 64) return false;
            foreach (char c in value) if (!(c >= '0' && c <= '9' || c >= 'a' && c <= 'f')) return false;
            return true;
        }

        internal static bool TryCreate(out AndroidBackendCredentialStore store)
        {
            store = null;
#if UNITY_ANDROID && !UNITY_EDITOR
#if DEVELOPMENT_BUILD
            if (AndroidCredentialDevelopmentProbe.BlocksAccounts) return false;
#endif
            store = new AndroidBackendCredentialStore(new AndroidIo());
            return true;
#else
            return false; // No desktop/Editor plaintext substitute.
#endif
        }

        // Compiled in the Editor too so JNI call signatures are checked by C# compilation.
        // Android Keystore execution itself requires an Android device.
        sealed class AndroidIo : IProtectedCredentialIo
        {
            const string Bridge = "com.legendsofrus.credentials.AndroidCredentialBridge";
            static AndroidJavaObject Begin(string method, string json = null)
            {
                try
                {
                    using (var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (var activity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (var plugin = new AndroidJavaClass(Bridge))
                        return json == null ? plugin.CallStatic<AndroidJavaObject>(method, activity) : plugin.CallStatic<AndroidJavaObject>(method, activity, json);
                }
                catch { return null; } // Never log native exceptions containing credential material.
            }
            static bool Poll(AndroidJavaObject operation, out bool done, out bool success, out string value)
            {
                done = true; success = false; value = null;
                try
                {
                    done = operation.Get<bool>("done");
                    if (done) { success = operation.Get<bool>("success"); if (success) value = operation.Get<string>("value"); }
                    return true;
                }
                catch { return false; }
            }
            public IEnumerator Read(Action<bool, string> completed)
            {
                using (var operation = Begin("beginRead"))
                {
                    if (operation == null) { completed(false, null); yield break; }
                    while (true)
                    {
                        if (!Poll(operation, out bool done, out bool success, out string value)) { completed(false, null); yield break; }
                        if (done) { completed(success, value); yield break; }
                        yield return null;
                    }
                }
            }
            public IEnumerator Write(string value, Action<bool> completed)
            {
                using (var operation = Begin("beginWrite", value))
                {
                    if (operation == null) { completed(false); yield break; }
                    while (true)
                    {
                        if (!Poll(operation, out bool done, out bool success, out _)) { completed(false); yield break; }
                        if (done) { completed(success); yield break; }
                        yield return null;
                    }
                }
            }
        }
    }
}
