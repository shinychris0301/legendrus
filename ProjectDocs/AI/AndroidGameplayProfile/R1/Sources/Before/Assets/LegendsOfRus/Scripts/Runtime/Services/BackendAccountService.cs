using System;
using System.Collections;
using UnityEngine;

namespace LegendsOfRus
{
    // Lives on the existing services root. No automatic network traffic or local-save merge.
    public sealed class BackendAccountService : MonoBehaviour
    {
        public static BackendAccountService Instance { get; private set; }
        BackendSessionClient session;
        public bool IsConfigured => session != null;
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }
#if UNITY_ANDROID && DEVELOPMENT_BUILD && !UNITY_EDITOR
        IEnumerator Start() => AndroidCredentialDevelopmentProbe.RunRequested(this);
#endif
        void OnDestroy()
        {
            session?.Dispose(); session = null;
            if (Instance == this) Instance = null;
        }
        public IEnumerator ReadServerWallet(Action<BackendWalletResult> completed)
        {
            if (session == null) { completed(new BackendWalletResult(BackendStatus.NotConfigured)); yield break; }
            yield return session.ReadWallet(completed);
        }
        // Explicit composition boundary. No endpoint is installed or auto-activated.
        // The Android adapter exists; hosted/device verification is still required.
        internal bool TryConfigureAndroid(string httpsOrigin)
        {
            if (IsConfigured || !AndroidBackendCredentialStore.TryCreate(out var store)) return false;
            BackendHttpTransport transport;
            try { transport = new BackendHttpTransport(httpsOrigin); }
            catch (ArgumentException) { return false; }
            Configure(store, transport);
            return true;
        }

        internal void Configure(IBackendCredentialStore store, IBackendTransport transport)
        {
#if UNITY_ANDROID && DEVELOPMENT_BUILD && !UNITY_EDITOR
            if (AndroidCredentialDevelopmentProbe.BlocksAccounts)
                throw new InvalidOperationException("Development credential probe owns the empty test vault");
#endif
            if (IsConfigured) throw new InvalidOperationException("Backend already configured");
            session = new BackendSessionClient(store, transport);
        }
    }
}
