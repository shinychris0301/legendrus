from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs'
s=p.read_text();i=s.rfind('    }');assert i>0
s=s[:i]+r'''
        [TestCase(false,false)] [TestCase(false,true)] [TestCase(true,false)] [TestCase(true,true)]
        public void PersistenceLockedLegacyWriteCannotOverwriteRepairedJournal(bool loadedLocked,bool corrupt)
        {
            var previous=GameState.Instance;var saved=new string[3];var present=new bool[3];
            for(int n=0;n<3;n++){present[n]=PlayerPrefs.HasKey(PersistedSaveKeys[n]);saved[n]=PlayerPrefs.GetString(PersistedSaveKeys[n]);}
            GameObject freshObject=null;bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            var state=previous;System.Action changed=null;
            try
            {
                foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);
                var future=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(previous.Save));future.saveVersion=999;
                PlayerPrefs.SetString(PersistedSaveKeys[0],corrupt?"{corrupt-persistence":JsonUtility.ToJson(future));PlayerPrefs.Save();
                if(loadedLocked){freshObject=new GameObject("LockedPersistenceLoadState");state=freshObject.AddComponent<GameState>();}
                else Assert.That(state.TryTogglePreference(0),Is.False);
                Assert.That(state.CanPersist,Is.False);var original=state.Save;string before=JsonUtility.ToJson(original);var write=state.LastWriteResult;var load=state.LastLoadResult;
                foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);PlayerPrefs.Save();
                var repaired=new PlayerSaveData{gold=4242,lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks};Assert.That(SaveService.TryReplaceHistory(repaired,out var repairedWrite),Is.True);
                var journal=new string[3];for(int n=0;n<3;n++)journal[n]=PlayerPrefs.GetString(PersistedSaveKeys[n]);
                int events=0;changed=()=>events++;state.Changed+=changed;
                state.Persist();
                for(int n=0;n<3;n++)Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[n]),Is.EqualTo(journal[n]),"Locked instance must not replace repaired history.");
                Assert.That(state.CanPersist,Is.False);Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(events,Is.Zero);
                Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(state.LastLoadResult,Is.SameAs(load));
            }
            finally
            {
                if(changed!=null)state.Changed-=changed;typeof(GameState).GetProperty("Instance").SetValue(null,previous);if(freshObject!=null)Object.DestroyImmediate(freshObject);
                for(int n=0;n<3;n++){PlayerPrefs.DeleteKey(PersistedSaveKeys[n]);if(present[n])PlayerPrefs.SetString(PersistedSaveKeys[n],saved[n]);}PlayerPrefs.Save();LogAssert.ignoreFailingMessages=ignored;
            }
        }
        [Test]
        public void PersistenceLockedLegacyRetryDoesNotRescanOrReplaceFailureEvidence()
        {
            var state=GameState.Instance;var saved=new string[3];var present=new bool[3];for(int n=0;n<3;n++){present[n]=PlayerPrefs.HasKey(PersistedSaveKeys[n]);saved[n]=PlayerPrefs.GetString(PersistedSaveKeys[n]);}
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            try
            {
                foreach(string key in PersistedSaveKeys)PlayerPrefs.DeleteKey(key);PlayerPrefs.SetString(PersistedSaveKeys[0],"{\"saveVersion\":999}");PlayerPrefs.Save();
                Assert.That(state.TryTogglePreference(0),Is.False);var write=state.LastWriteResult;
                state.Persist();Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(state.CanPersist,Is.False);
            }
            finally{for(int n=0;n<3;n++){PlayerPrefs.DeleteKey(PersistedSaveKeys[n]);if(present[n])PlayerPrefs.SetString(PersistedSaveKeys[n],saved[n]);}PlayerPrefs.Save();LogAssert.ignoreFailingMessages=ignored;}
        }
        [Test]
        public void PersistenceLegacyChangedReentryCommitsOnlyOnce()
        {
            var state=GameState.Instance;int events=0;System.Action changed=()=>{events++;if(events==1)state.Persist();};state.Changed+=changed;
            try{state.Persist();Assert.That(events,Is.EqualTo(1));Assert.That(state.CanPersist,Is.True);}
            finally{state.Changed-=changed;}
        }
'''+'\n'+s[i:];p.write_text(s)
