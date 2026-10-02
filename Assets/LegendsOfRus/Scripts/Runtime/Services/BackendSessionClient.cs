using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace LegendsOfRus
{
    // Must be stored atomically in platform-protected credential storage, never PlayerPrefs
    // or PlayerSaveData. A failed load must not be represented as an empty/new account.
    [Serializable]
    public sealed class BackendSessionState
    {
        public int version = 1;
        public string origin, playerId, refreshToken, pendingRequest, nextRefreshToken;
        public BackendSessionState Copy() => JsonUtility.FromJson<BackendSessionState>(JsonUtility.ToJson(this));
    }

    public interface IBackendCredentialStore
    {
        bool TryLoad(out BackendSessionState state);
        bool TrySave(BackendSessionState state);
    }

    public interface IAsyncBackendCredentialStore : IBackendCredentialStore
    {
        IEnumerator Load(Action<bool, BackendSessionState> completed);
        IEnumerator Save(BackendSessionState state, Action<bool> completed);
    }

    public enum BackendStatus { Success, NotConfigured, StorageUnavailable, RecoveryRequired, InvalidResponse, NetworkFailure, Busy }

    public readonly struct BackendWalletResult
    {
        public readonly BackendStatus Status;
        public readonly string PlayerId;
        public readonly int Gems;
        public BackendWalletResult(BackendStatus status, string playerId = null, int gems = 0)
        { Status = status; PlayerId = playerId; Gems = gems; }
    }

    internal readonly struct BackendHttpResult
    {
        internal readonly int Code;
        internal readonly string Body;
        internal BackendHttpResult(int code, string body) { Code = code; Body = body; }
    }

    internal interface IBackendTransport : IDisposable
    {
        string Origin { get; }
        IEnumerator Send(string method, string path, string body, string bearer, Action<BackendHttpResult> completed);
    }

    internal sealed class BackendSessionClient : IDisposable
    {
        [Serializable] sealed class BootstrapRequest { public string refresh_token, request_id; }
        [Serializable] sealed class RefreshRequest { public string refresh_token, next_refresh_token, request_id; }
        [Serializable] sealed class SessionResponse { public string player_id, access_token; public long expires_at, refresh_expires_at; }
        [Serializable] sealed class WalletResponse { public string id; public int gems = -1; }
        readonly IBackendCredentialStore store;
        readonly IBackendTransport transport;
        readonly Func<long> now;
        string access, player;
        long expires;
        bool busy, disposed;

        internal BackendSessionClient(IBackendCredentialStore store, IBackendTransport transport, Func<long> clock = null)
        { this.store = store; this.transport = transport; now = clock ?? (() => DateTimeOffset.UtcNow.ToUnixTimeSeconds()); }

        internal IEnumerator ReadWallet(Action<BackendWalletResult> completed)
        {
            if (disposed) { completed(new BackendWalletResult(BackendStatus.NotConfigured)); yield break; }
            if (busy) { completed(new BackendWalletResult(BackendStatus.Busy)); yield break; }
            busy = true;
            try
            {
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    BackendStatus status = BackendStatus.NetworkFailure;
                    yield return EnsureSession(s => status = s);
                    if (status != BackendStatus.Success) { completed(new BackendWalletResult(status)); yield break; }
                    BackendHttpResult response = default;
                    yield return transport.Send("GET", "/v1/player", null, access, r => response = r);
                    if (response.Code == 401 && attempt == 0) { access = null; continue; }
                    if (response.Code != 200) { completed(new BackendWalletResult(Failure(response.Code))); yield break; }
                    var wallet = Parse<WalletResponse>(response.Body);
                    if (wallet == null || wallet.id != player || wallet.gems < 0)
                    { completed(new BackendWalletResult(BackendStatus.InvalidResponse)); yield break; }
                    // Read-only result. Never import this value into the offline save.
                    completed(new BackendWalletResult(BackendStatus.Success, wallet.id, wallet.gems));
                    yield break;
                }
            }
            finally { busy = false; }
        }

        IEnumerator EnsureSession(Action<BackendStatus> completed)
        {
            if (access != null && expires > now() + 30) { completed(BackendStatus.Success); yield break; }
            BackendSessionState state = null; bool loaded = false;
            yield return Load((ok, value) => { loaded = ok; state = value; });
            if (!loaded) { completed(BackendStatus.StorageUnavailable); yield break; }
            if (state == null)
            {
                state = new BackendSessionState { origin = transport.Origin, refreshToken = Secret(), pendingRequest = Guid.NewGuid().ToString() };
                bool saved = false; yield return Save(state, ok => saved = ok);
                if (!saved) { completed(BackendStatus.StorageUnavailable); yield break; }
            }
            if (!Valid(state)) { completed(BackendStatus.RecoveryRequired); yield break; }
            // At most bootstrap retry then renewal if its recovered access token has expired.
            for (int step = 0; step < 2; step++)
            {
                bool bootstrap = string.IsNullOrEmpty(state.playerId);
                if (!bootstrap && string.IsNullOrEmpty(state.pendingRequest))
                {
                    state = state.Copy(); state.pendingRequest = Guid.NewGuid().ToString(); state.nextRefreshToken = Secret();
                    bool pendingSaved = false; yield return Save(state, ok => pendingSaved = ok);
                    if (!pendingSaved) { completed(BackendStatus.StorageUnavailable); yield break; }
                }
                string payload = bootstrap
                    ? JsonUtility.ToJson(new BootstrapRequest { refresh_token = state.refreshToken, request_id = state.pendingRequest })
                    : JsonUtility.ToJson(new RefreshRequest { refresh_token = state.refreshToken, next_refresh_token = state.nextRefreshToken, request_id = state.pendingRequest });
                BackendHttpResult response = default;
                yield return transport.Send("POST", bootstrap ? "/v1/auth/bootstrap" : "/v1/auth/refresh", payload, null, r => response = r);
                if (response.Code != 200) { completed(Failure(response.Code)); yield break; }
                var session = Parse<SessionResponse>(response.Body);
                if (session == null || !Uuid(session.player_id) || !Hex(session.access_token) || session.expires_at <= 0 ||
                    session.refresh_expires_at <= session.expires_at || (!bootstrap && session.player_id != state.playerId))
                { completed(BackendStatus.InvalidResponse); yield break; }
                var committed = state.Copy(); committed.playerId = session.player_id;
                if (!bootstrap) committed.refreshToken = state.nextRefreshToken;
                committed.pendingRequest = null; committed.nextRefreshToken = null;
                bool committedSaved = false; yield return Save(committed, ok => committedSaved = ok);
                if (!committedSaved) { completed(BackendStatus.StorageUnavailable); yield break; }
                state = committed;
                if (session.expires_at <= now() + 30) continue;
                player = session.player_id; access = session.access_token; expires = session.expires_at;
                completed(BackendStatus.Success); yield break;
            }
            completed(BackendStatus.RecoveryRequired);
        }

        bool Valid(BackendSessionState state) => state.version == 1 && state.origin == transport.Origin && Hex(state.refreshToken) &&
            (string.IsNullOrEmpty(state.playerId)
                ? Uuid(state.pendingRequest) && string.IsNullOrEmpty(state.nextRefreshToken)
                : Uuid(state.playerId) && (string.IsNullOrEmpty(state.pendingRequest)
                    ? string.IsNullOrEmpty(state.nextRefreshToken)
                    : Uuid(state.pendingRequest) && Hex(state.nextRefreshToken) && state.nextRefreshToken != state.refreshToken));
        IEnumerator Load(Action<bool, BackendSessionState> completed)
        {
            if (store is IAsyncBackendCredentialStore asyncStore)
            {
                yield return asyncStore.Load((ok, state) => completed(ok, state?.Copy()));
                yield break;
            }
            BackendSessionState loaded = null; bool success;
            try { success = store.TryLoad(out loaded); loaded = loaded?.Copy(); }
            catch { success = false; }
            completed(success, loaded);
        }
        IEnumerator Save(BackendSessionState state, Action<bool> completed)
        {
            if (store is IAsyncBackendCredentialStore asyncStore)
            {
                yield return asyncStore.Save(state.Copy(), completed);
                yield break;
            }
            bool saved;
            try { saved = store.TrySave(state.Copy()); } catch { saved = false; }
            completed(saved);
        }
        static T Parse<T>(string body) where T : class { try { return JsonUtility.FromJson<T>(body); } catch { return null; } }
        static bool Uuid(string value) => Guid.TryParseExact(value, "D", out var id) && id.ToString() == value;
        static bool Hex(string value)
        {
            if (value == null || value.Length != 64) return false;
            foreach (char c in value) if (!(c >= '0' && c <= '9' || c >= 'a' && c <= 'f')) return false;
            return true;
        }
        static string Secret()
        {
            var bytes = new byte[32]; using (var random = RandomNumberGenerator.Create()) random.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
        static BackendStatus Failure(int code) => code == 401 || code == 409 ? BackendStatus.RecoveryRequired : BackendStatus.NetworkFailure;
        public void Dispose() { disposed = true; access = null; player = null; transport.Dispose(); }
    }
}
