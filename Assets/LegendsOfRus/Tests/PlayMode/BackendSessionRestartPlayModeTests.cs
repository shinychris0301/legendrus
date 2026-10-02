using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class BackendSessionRestartPlayModeTests
    {
        // Disposable QA fixture only: created in a runner-owned 0700 temporary directory.
        // This plaintext store is NOT part of the runtime and is not an Android adapter.
        sealed class FixtureStore : IBackendCredentialStore
        {
            readonly string path;
            internal FixtureStore(string folder) { path=Path.Combine(folder,"synthetic-session.json"); }
            public bool TryLoad(out BackendSessionState state)
            { state=File.Exists(path)?JsonUtility.FromJson<BackendSessionState>(File.ReadAllText(path)):null;return true; }
            public bool TrySave(BackendSessionState state)
            { File.WriteAllText(path,JsonUtility.ToJson(state));return true; }
        }
        sealed class LostResponse : IBackendTransport
        {
            readonly IBackendTransport inner;
            internal LostResponse(IBackendTransport inner) { this.inner=inner; }
            public string Origin=>inner.Origin;
            public IEnumerator Send(string method,string path,string body,string bearer,Action<BackendHttpResult> done)
            {
                BackendHttpResult actual=default;
                yield return inner.Send(method,path,body,bearer,r=>actual=r);
                Assert.AreEqual(200,actual.Code,"Real backend committed before simulated delivery loss");
                done(new BackendHttpResult(0,null));
            }
            public void Dispose()=>inner.Dispose();
        }
        static string Folder()
        {
            string path=Environment.GetEnvironmentVariable("LOR_BACKEND_CREDENTIAL_TEST_DIR");
            Assert.IsTrue(path!=null&&path.StartsWith("/tmp/lor-backend-restart-",StringComparison.Ordinal));return path;
        }
        [UnityTest] public IEnumerator WriteCommittedRotationWithLostResponse()
        {
            if(Environment.GetEnvironmentVariable("LOR_BACKEND_SESSION_PHASE")!="write")Assert.Ignore("Separate-process writer only");
            var folder=Folder();var store=new FixtureStore(folder);string origin=Environment.GetEnvironmentVariable("LOR_BACKEND_TEST_ORIGIN");
            BackendWalletResult first=default,lost=default;
            using(var client=new BackendSessionClient(store,new BackendHttpTransport(origin,true)))yield return client.ReadWallet(r=>first=r);
            Assert.AreEqual(BackendStatus.Success,first.Status);File.WriteAllText(Path.Combine(folder,"expected-player.txt"),first.PlayerId);
            using(var client=new BackendSessionClient(store,new LostResponse(new BackendHttpTransport(origin,true))))yield return client.ReadWallet(r=>lost=r);
            Assert.AreEqual(BackendStatus.NetworkFailure,lost.Status);store.TryLoad(out var state);Assert.IsNotEmpty(state.pendingRequest);
        }
        [UnityTest] public IEnumerator ReadAndRecoverCommittedRotationAfterRestart()
        {
            if(Environment.GetEnvironmentVariable("LOR_BACKEND_SESSION_PHASE")!="read")Assert.Ignore("Separate-process reader only");
            var folder=Folder();var store=new FixtureStore(folder);string origin=Environment.GetEnvironmentVariable("LOR_BACKEND_TEST_ORIGIN");
            store.TryLoad(out var before);Assert.IsNotEmpty(before.pendingRequest);BackendWalletResult result=default;
            using(var client=new BackendSessionClient(store,new BackendHttpTransport(origin,true)))yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.Success,result.Status);Assert.AreEqual(File.ReadAllText(Path.Combine(folder,"expected-player.txt")),result.PlayerId);
            Assert.AreEqual(0,result.Gems);store.TryLoad(out var after);Assert.IsTrue(string.IsNullOrEmpty(after.pendingRequest));
            Assert.IsTrue(before.refreshToken!=after.refreshToken,"Retry commits previously rotated credential");
        }
    }
}
