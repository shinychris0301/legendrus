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
        // Explicit composition boundary. No production endpoint or insecure credential
        // store is installed. Platform-protected persistence is the next integration gate.
        internal void Configure(IBackendCredentialStore store, IBackendTransport transport)
        {
            if (IsConfigured) throw new InvalidOperationException("Backend already configured");
            session = new BackendSessionClient(store, transport);
        }
    }
}
