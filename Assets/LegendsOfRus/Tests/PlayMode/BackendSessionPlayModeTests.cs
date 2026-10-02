using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class BackendSessionPlayModeTests
    {
        sealed class Store : IBackendCredentialStore
        {
            internal BackendSessionState State;
            internal bool FailRead;
            internal int Saves, FailSaveNumber;
            public bool TryLoad(out BackendSessionState state) { state = State?.Copy(); return !FailRead; }
            public bool TrySave(BackendSessionState state)
            { if (++Saves == FailSaveNumber) return false; State = state.Copy(); return true; }
        }
        sealed class Transport : IBackendTransport
        {
            public string Origin => "https://backend.example.test";
            internal readonly Queue<BackendHttpResult> Replies = new Queue<BackendHttpResult>();
            internal readonly List<string> Bodies = new List<string>();
            internal readonly List<string> Routes = new List<string>();
            internal bool Delay;
            public IEnumerator Send(string method, string path, string body, string bearer, Action<BackendHttpResult> callback)
            { Routes.Add(path); Bodies.Add(body); if (Delay) yield return null; callback(Replies.Dequeue()); }
            public void Dispose() { }
        }
        const string Player = "5cb00898-3fb9-4a35-b51f-f39cb27dbbdd";
        const long Now = 1000;
        static BackendHttpResult Session(string player = Player, long expires = 87400) => new BackendHttpResult(200,
            "{\"player_id\":\"" + player + "\",\"access_token\":\"" + new string('a',64) + "\",\"expires_at\":" + expires + ",\"refresh_expires_at\":2593000}");
        static BackendHttpResult Wallet(string player = Player, int gems = 15) => new BackendHttpResult(200,"{\"id\":\""+player+"\",\"gems\":"+gems+"}");
        static BackendSessionState Existing() => new BackendSessionState { origin = "https://backend.example.test", playerId = Player, refreshToken = new string('b',64) };
        static BackendSessionClient Client(Store s, Transport t) => new BackendSessionClient(s,t,()=>Now);

        [UnityTest] public IEnumerator BootstrapPersistsBeforeRequestAndKeepsWalletSeparate()
        {
            var store = new Store(); var http = new Transport(); http.Replies.Enqueue(Session()); http.Replies.Enqueue(Wallet());
            string localBefore = JsonUtility.ToJson(GameState.Instance.Save);
            BackendWalletResult result = default;
            using (var client = Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.Success,result.Status); Assert.AreEqual(15,result.Gems);
            Assert.AreEqual(Player,store.State.playerId); Assert.IsTrue(string.IsNullOrEmpty(store.State.pendingRequest));
            Assert.AreEqual(2,store.Saves); Assert.AreEqual("/v1/auth/bootstrap",http.Routes[0]);
            Assert.IsTrue(!JsonUtility.ToJson(store.State).Contains(new string('a',64)),"Access token is memory-only");
            Assert.AreEqual(localBefore,JsonUtility.ToJson(GameState.Instance.Save),"No local progress/wallet reconciliation implied");
        }
        [UnityTest] public IEnumerator FailedCredentialLoadNeverCreatesNewAccount()
        {
            var store=new Store { FailRead=true };var http=new Transport(); BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.StorageUnavailable,result.Status);Assert.AreEqual(0,http.Routes.Count);
        }
        [UnityTest] public IEnumerator FailedPreRequestWriteMakesNoNetworkRequest()
        {
            var store=new Store { FailSaveNumber=1 };var http=new Transport();BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.StorageUnavailable,result.Status);Assert.AreEqual(0,http.Routes.Count);
        }
        [UnityTest] public IEnumerator LostBootstrapResponseReusesExactRequestAfterClientRestart()
        {
            var store=new Store();var first=new Transport();first.Replies.Enqueue(new BackendHttpResult(0,null));
            using(var client=Client(store,first)) yield return client.ReadWallet(_=>{});
            var second=new Transport();second.Replies.Enqueue(Session());second.Replies.Enqueue(Wallet());BackendWalletResult result=default;
            using(var client=Client(store,second)) yield return client.ReadWallet(r=>result=r);
            Assert.IsTrue(first.Bodies[0]==second.Bodies[0],"Pending bootstrap must be byte-identical");Assert.AreEqual(BackendStatus.Success,result.Status);
        }
        [UnityTest] public IEnumerator FailedBootstrapCommitRecoversSameIdentity()
        {
            var store=new Store { FailSaveNumber=2 };var first=new Transport();first.Replies.Enqueue(Session());BackendWalletResult result=default;
            using(var client=Client(store,first)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.StorageUnavailable,result.Status);Assert.IsTrue(string.IsNullOrEmpty(store.State.playerId));Assert.AreEqual(1,first.Routes.Count);
            var retry=new Transport();retry.Replies.Enqueue(Session());retry.Replies.Enqueue(Wallet());
            using(var client=Client(store,retry)) yield return client.ReadWallet(r=>result=r);
            Assert.IsTrue(first.Bodies[0]==retry.Bodies[0]);Assert.AreEqual(BackendStatus.Success,result.Status);
        }
        [UnityTest] public IEnumerator LostRefreshAndFailedCommitKeepDurablePendingRotation()
        {
            var store=new Store { State=Existing(),FailSaveNumber=2 };var first=new Transport();first.Replies.Enqueue(Session());BackendWalletResult result=default;
            string old=store.State.refreshToken;
            using(var client=Client(store,first)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.StorageUnavailable,result.Status);Assert.IsTrue(store.State.refreshToken==old);Assert.IsNotEmpty(store.State.nextRefreshToken);
            var retry=new Transport();retry.Replies.Enqueue(Session());retry.Replies.Enqueue(Wallet());
            using(var client=Client(store,retry)) yield return client.ReadWallet(r=>result=r);
            Assert.IsTrue(first.Bodies[0]==retry.Bodies[0]);Assert.IsTrue(store.State.refreshToken!=old);Assert.IsTrue(string.IsNullOrEmpty(store.State.pendingRequest));
            Assert.AreEqual(BackendStatus.Success,result.Status);
        }
        [UnityTest] public IEnumerator RejectedRefreshDoesNotFallbackToNewGuest()
        {
            var store=new Store { State=Existing() };var http=new Transport();http.Replies.Enqueue(new BackendHttpResult(401,null));BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.RecoveryRequired,result.Status);Assert.AreEqual(1,http.Routes.Count);Assert.AreEqual(Player,store.State.playerId);
        }
        [UnityTest] public IEnumerator MismatchedIdentityCannotReplaceStoredPlayer()
        {
            var store=new Store { State=Existing() };var http=new Transport();http.Replies.Enqueue(Session(Guid.NewGuid().ToString()));BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.InvalidResponse,result.Status);Assert.AreEqual(Player,store.State.playerId);
        }
        [UnityTest] public IEnumerator OriginAndSchemaMismatchFailBeforeNetwork()
        {
            foreach(bool version in new[]{false,true})
            {
                var state=Existing();if(version)state.version=99;else state.origin="https://other.example.test";
                var store=new Store { State=state };var http=new Transport();BackendWalletResult result=default;
                using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
                Assert.AreEqual(BackendStatus.RecoveryRequired,result.Status);Assert.AreEqual(0,http.Routes.Count);
            }
        }
        [UnityTest] public IEnumerator UnauthorizedWalletRenewsOnceWithoutChangingIdentity()
        {
            var store=new Store();var http=new Transport();http.Replies.Enqueue(Session());http.Replies.Enqueue(new BackendHttpResult(401,null));http.Replies.Enqueue(Session());http.Replies.Enqueue(Wallet());BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.Success,result.Status);Assert.AreEqual("/v1/auth/refresh",http.Routes[2]);
        }
        [UnityTest] public IEnumerator ExpiredRecoveredBootstrapRenewsBeforeReadingWallet()
        {
            var store=new Store();var http=new Transport();http.Replies.Enqueue(Session(expires:900));http.Replies.Enqueue(Session());http.Replies.Enqueue(Wallet());BackendWalletResult result=default;
            using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.Success,result.Status);Assert.AreEqual("/v1/auth/refresh",http.Routes[1]);
        }
        [UnityTest] public IEnumerator InvalidPayloadAndWalletRejected()
        {
            foreach(string invalid in new[]{"not json","{}","{\"player_id\":\"invalid\"}"})
            {
                var store=new Store();var http=new Transport();http.Replies.Enqueue(new BackendHttpResult(200,invalid));BackendWalletResult result=default;
                using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
                Assert.AreEqual(BackendStatus.InvalidResponse,result.Status);
            }
            foreach(var wallet in new[]{Wallet(Guid.NewGuid().ToString()),Wallet(gems:-1),new BackendHttpResult(200,"{}"),new BackendHttpResult(200,"{\"id\":\""+Player+"\"}")})
            {
                var store=new Store();var http=new Transport();http.Replies.Enqueue(Session());http.Replies.Enqueue(wallet);BackendWalletResult result=default;
                using(var client=Client(store,http)) yield return client.ReadWallet(r=>result=r);
                Assert.AreEqual(BackendStatus.InvalidResponse,result.Status);
            }
        }
        [UnityTest] public IEnumerator ConcurrentReadReturnsBusyAndUsesOneBootstrap()
        {
            var store=new Store();var http=new Transport { Delay=true };http.Replies.Enqueue(Session());http.Replies.Enqueue(Wallet());
            using(var client=Client(store,http))
            {
                var running=client.ReadWallet(_=>{});Assert.IsTrue(running.MoveNext());
                BackendWalletResult other=default;yield return client.ReadWallet(r=>other=r);
                Assert.AreEqual(BackendStatus.Busy,other.Status);
                yield return running.Current;while(running.MoveNext())yield return running.Current;
                Assert.AreEqual(2,http.Routes.Count);
            }
        }
        [TestCase("http://backend.example.test")]
        [TestCase("https://user:password@backend.example.test")]
        [TestCase("https://backend.example.test/path")]
        [TestCase("https://backend.example.test/?query=1")]
        [TestCase("https://backend.example.test/#fragment")]
        public void UnsafeEndpointRejected(string origin) => Assert.Throws<ArgumentException>(()=>new BackendHttpTransport(origin));

        [UnityTest] public IEnumerator BootstrapServiceIsPresentButDoesNotConnectOrChangeLogin()
        {
            Assert.IsNotNull(BackendAccountService.Instance);Assert.IsFalse(BackendAccountService.Instance.IsConfigured);
            BackendWalletResult result=default;yield return BackendAccountService.Instance.ReadServerWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.NotConfigured,result.Status);
        }

        [UnityTest] public IEnumerator RealHttpBootstrapAndRefreshKeepIdentityAcrossClientRecreation()
        {
            string origin=Environment.GetEnvironmentVariable("LOR_BACKEND_TEST_ORIGIN");
            if(string.IsNullOrEmpty(origin))Assert.Ignore("Requires isolated loopback backend verification runner");
            var store=new Store();BackendWalletResult first=default,second=default;
            using(var client=new BackendSessionClient(store,new BackendHttpTransport(origin,true)))yield return client.ReadWallet(r=>first=r);
            Assert.AreEqual(BackendStatus.Success,first.Status);
            using(var client=new BackendSessionClient(store,new BackendHttpTransport(origin,true)))yield return client.ReadWallet(r=>second=r);
            Assert.AreEqual(BackendStatus.Success,second.Status);Assert.AreEqual(first.PlayerId,second.PlayerId);Assert.AreEqual(0,second.Gems);
        }
    }
}
