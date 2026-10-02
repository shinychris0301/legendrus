using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class AndroidCredentialStorePlayModeTests
    {
        // Simulated I/O; AES-GCM is exercised separately with the production Java core.
        sealed class Io : IProtectedCredentialIo
        {
            internal string Record;
            internal bool FailRead, FailWrite, Delay;
            internal int Writes;
            public IEnumerator Read(Action<bool,string> completed)
            { if(Delay)yield return null;completed(!FailRead,Record); }
            public IEnumerator Write(string value,Action<bool> completed)
            { Writes++;if(Delay)yield return null;if(!FailWrite)Record=value;completed(!FailWrite); }
        }
        const string Player="947d71f1-ac24-457c-8662-4af2e9969d49";
        static BackendSessionState State() => new BackendSessionState {
            origin="https://backend.example.test",playerId=Player,refreshToken=new string('a',64)
        };
        sealed class Transport : IBackendTransport
        {
            readonly Io io;
            internal Transport(Io io){this.io=io;}
            internal int Calls;
            public string Origin=>"https://backend.example.test";
            public IEnumerator Send(string method,string path,string body,string token,Action<BackendHttpResult> completed)
            {
                Calls++;
                Assert.IsNotEmpty(io.Record,"Credentials must be durably written before network");
                if(path=="/v1/player")completed(new BackendHttpResult(200,"{\"id\":\""+Player+"\",\"gems\":17}"));
                else completed(new BackendHttpResult(200,"{\"player_id\":\""+Player+"\",\"access_token\":\""+new string('b',64)+"\",\"expires_at\":90000,\"refresh_expires_at\":2593000}"));
                yield break;
            }
            public void Dispose(){}
        }
        [UnityTest] public IEnumerator EmptyVaultAndReadFailureRemainDistinct()
        {
            var io=new Io();var store=new AndroidBackendCredentialStore(io);bool ok=false;BackendSessionState state=State();
            yield return store.Load((success,value)=>{ok=success;state=value;});Assert.IsTrue(ok);Assert.IsNull(state);
            io.FailRead=true;yield return store.Load((success,value)=>{ok=success;state=value;});Assert.IsFalse(ok);Assert.IsNull(state);
        }
        [UnityTest] public IEnumerator AsyncRoundTripRetainsPendingRotation()
        {
            var io=new Io { Delay=true };var store=new AndroidBackendCredentialStore(io);var state=State();
            state.pendingRequest=Guid.NewGuid().ToString();state.nextRefreshToken=new string('c',64);bool saved=false;BackendSessionState loaded=null;
            yield return store.Save(state,ok=>saved=ok);Assert.IsTrue(saved);
            yield return store.Load((ok,value)=>{Assert.IsTrue(ok);loaded=value;});
            Assert.AreEqual(state.pendingRequest,loaded.pendingRequest);Assert.IsTrue(state.nextRefreshToken==loaded.nextRefreshToken);
            loaded.playerId="changed";Assert.IsFalse(io.Record.Contains("changed"));
        }
        [UnityTest] public IEnumerator FailedWriteKeepsPreviousRecord()
        {
            var io=new Io { Record=JsonUtility.ToJson(State()),FailWrite=true };var before=io.Record;var store=new AndroidBackendCredentialStore(io);bool saved=true;
            yield return store.Save(State(),ok=>saved=ok);Assert.IsFalse(saved);Assert.IsTrue(before==io.Record);
        }
        [UnityTest] public IEnumerator MalformedAndFutureRecordsNeverBecomeFreshAccount()
        {
            foreach(string value in new[]{"", "null", "{}", "invalid",JsonUtility.ToJson(new BackendSessionState{version=99}),new string('x',16385)})
            {
                var store=new AndroidBackendCredentialStore(new Io { Record=value });bool ok=true;
                yield return store.Load((success,state)=>{ok=success;Assert.IsNull(state);});Assert.IsFalse(ok);
            }
        }
        [UnityTest] public IEnumerator InvalidStateIsRejectedBeforeNativeWrite()
        {
            for(int variant=0;variant<7;variant++)
            {
                var state=State();
                switch(variant){case 0:state.version=99;break;case 1:state.origin="http://backend.example.test";break;case 2:state.refreshToken="weak";break;case 3:state.playerId="other";break;case 4:state.pendingRequest="bad";break;case 5:state.nextRefreshToken=new string('c',64);break;case 6:state.origin="https://user:pass@example.test";break;}
                var io=new Io();var store=new AndroidBackendCredentialStore(io);bool ok=true;
                yield return store.Save(state,success=>ok=success);Assert.IsFalse(ok);Assert.AreEqual(0,io.Writes);
            }
        }
        [Test] public void SynchronousCallsCannotBlockOrUsePlaintextFallback()
        {
            var store=new AndroidBackendCredentialStore(new Io());Assert.IsFalse(store.TryLoad(out var state));Assert.IsNull(state);Assert.IsFalse(store.TrySave(State()));
#if UNITY_EDITOR
            Assert.IsFalse(AndroidBackendCredentialStore.TryCreate(out _));
            Assert.IsFalse(BackendAccountService.Instance.TryConfigureAndroid("https://backend.example.test"));
#endif
        }
        [UnityTest] public IEnumerator SessionClientAwaitsProtectedStoreBeforeSending()
        {
            var io=new Io { Delay=true };var transport=new Transport(io);var store=new AndroidBackendCredentialStore(io);BackendWalletResult result=default;
            using(var client=new BackendSessionClient(store,transport,()=>1000))yield return client.ReadWallet(r=>result=r);
            Assert.AreEqual(BackendStatus.Success,result.Status);Assert.AreEqual(17,result.Gems);Assert.AreEqual(2,io.Writes);Assert.AreEqual(2,transport.Calls);
        }
        [UnityTest] public IEnumerator StorageFailurePreventsAnyBootstrapRequest()
        {
            foreach(bool readFailure in new[]{true,false})
            {
                var io=new Io { FailRead=readFailure,FailWrite=!readFailure };var transport=new Transport(io);BackendWalletResult result=default;
                using(var client=new BackendSessionClient(new AndroidBackendCredentialStore(io),transport,()=>1000))yield return client.ReadWallet(r=>result=r);
                Assert.AreEqual(BackendStatus.StorageUnavailable,result.Status);Assert.AreEqual(0,transport.Calls);
            }
        }
        [UnityTest] public IEnumerator ConcurrentProtectedOperationsAreRejected()
        {
            var io=new Io { Delay=true };var store=new AndroidBackendCredentialStore(io);var running=store.Save(State(),_=>{});
            Assert.IsTrue(running.MoveNext());bool ok=true;yield return store.Load((success,_)=>ok=success);Assert.IsFalse(ok);
            yield return running.Current;while(running.MoveNext())yield return running.Current;
        }
#if UNITY_EDITOR
        [TestCase("CredentialVault")][TestCase("AndroidCredentialBridge")]
        public void JavaSourcesAreImportedForAndroidOnly(string name)
        {
            var importer=AssetImporter.GetAtPath("Assets/LegendsOfRus/Plugins/AndroidCredentials/"+name+".java") as PluginImporter;
            Assert.IsNotNull(importer);Assert.IsFalse(importer.GetCompatibleWithAnyPlatform());Assert.IsFalse(importer.GetCompatibleWithEditor());Assert.IsTrue(importer.GetCompatibleWithPlatform(BuildTarget.Android));
        }
#endif
    }
}
