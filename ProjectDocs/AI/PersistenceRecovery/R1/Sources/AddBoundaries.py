from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();i=s.rfind('    }')
s=s[:i]+r'''
        string[] PersistenceSnapshotJournal()
        {
            var snapshot=new string[3];for(int n=0;n<3;n++)snapshot[n]=PlayerPrefs.HasKey(PersistedSaveKeys[n])?PlayerPrefs.GetString(PersistedSaveKeys[n]):null;return snapshot;
        }
        void PersistenceRestoreJournal(string[] snapshot)
        {
            for(int n=0;n<3;n++){PlayerPrefs.DeleteKey(PersistedSaveKeys[n]);if(snapshot[n]!=null)PlayerPrefs.SetString(PersistedSaveKeys[n],snapshot[n]);}PlayerPrefs.Save();
        }
        void PersistenceAssertJournal(string[] snapshot)
        {
            for(int n=0;n<3;n++){Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[n]),Is.EqualTo(snapshot[n]!=null));if(snapshot[n]!=null)Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[n]),Is.EqualTo(snapshot[n]));}
        }
        string PersistenceEncodeEnvelope(PlayerSaveData save,long revision)
        {
            System.Type codec=null;foreach(var assembly in System.AppDomain.CurrentDomain.GetAssemblies()){codec=assembly.GetType("LegendsOfRus.SaveCore.SaveEnvelopeCodec");if(codec!=null)break;}
            Assert.That(codec,Is.Not.Null);var encode=codec.GetMethod("TryEncode",new[]{typeof(string),typeof(int),typeof(long)});
            var result=encode.Invoke(System.Activator.CreateInstance(codec),new object[]{JsonUtility.ToJson(save),save.saveVersion,revision});
            Assert.That((bool)result.GetType().GetProperty("Success").GetValue(result),Is.True);return (string)result.GetType().GetProperty("Envelope").GetValue(result);
        }
        void PersistenceSeedBlockedJournal(int reason)
        {
            PersistenceRestoreJournal(new string[3]);string raw;
            if(reason==0)raw="{\"saveVersion\":999}";
            else if(reason==1)raw="{broken-persistence";
            else
            {
                raw=PersistenceEncodeEnvelope(GameState.Instance.Save,long.MaxValue);
            }
            PlayerPrefs.SetString(PersistedSaveKeys[0],raw);PlayerPrefs.Save();
        }
        [TestCase(false)] [TestCase(true)]
        public void PersistenceHealthyAcknowledgementPreservesLiveOwnersAndPendingReceipt(bool legacy)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);state.RegisterLevelAttempt(level);state.RecordLevelFailure(level,123);
            Assert.That(state.TryBeginBoosterCommand(BoosterType.Firebird,out string command),Is.True);var original=state.Save;var progress=state.GetLevelProgress(1,1);var failure=state.LastFailureResult;
            original.gold=4242;string before=JsonUtility.ToJson(original);int events=0;bool reentryAccepted=true;
            System.Action changed=()=>{events++;reentryAccepted=state.TryPersist();state.Persist();Assert.That(JsonUtility.ToJson(SaveService.Load()),Is.EqualTo(before));};state.Changed+=changed;
            try{if(legacy)state.Persist();else Assert.That(state.TryPersist(),Is.True);}
            finally{state.Changed-=changed;}
            Assert.That(events,Is.EqualTo(1));Assert.That(reentryAccepted,Is.False);Assert.That(state.Save,Is.SameAs(original));Assert.That(state.GetLevelProgress(1,1),Is.SameAs(progress));Assert.That(state.LastFailureResult,Is.SameAs(failure));
            Assert.That(state.Save.boosterCommand.commandId,Is.EqualTo(command));Assert.That(state.HasPendingBoosterCommand,Is.True);Assert.That(state.CanPersist,Is.True);Assert.That(state.TryPersist(),Is.True);
        }
        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void PersistenceAcknowledgementRejectsAndKeepsFirstDiagnostic(int reason)
        {
            var state=GameState.Instance;var original=state.Save;string before=JsonUtility.ToJson(original);var old=PersistenceSnapshotJournal();System.Action changed=null;
            try
            {
                PersistenceSeedBlockedJournal(reason);var journal=PersistenceSnapshotJournal();int events=0;changed=()=>events++;state.Changed+=changed;
                Assert.That(state.TryPersist(),Is.False);Assert.That(state.CanPersist,Is.False);var write=state.LastWriteResult;Assert.That(write.Status,Is.EqualTo(reason==2?PlayerSaveWriteStatus.RevisionExhausted:PlayerSaveWriteStatus.WriteBlocked));
                PersistenceAssertJournal(journal);Assert.That(state.TryPersist(),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));Assert.That(events,Is.Zero);Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));
            }
            finally{if(changed!=null)state.Changed-=changed;PersistenceRestoreJournal(old);}
        }
        [Test]
        public void PersistenceFreshStateAfterJournalRepairCanCommitWithoutUnlockingOldState()
        {
            var state=GameState.Instance;var old=PersistenceSnapshotJournal();GameObject freshObject=null;
            try
            {
                PersistenceSeedBlockedJournal(0);Assert.That(state.TryPersist(),Is.False);var write=state.LastWriteResult;
                PersistenceRestoreJournal(new string[3]);Assert.That(SaveService.TryReplaceHistory(new PlayerSaveData{gold=4242,lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks},out var replacement),Is.True);
                var journal=PersistenceSnapshotJournal();Assert.That(state.TryPersist(),Is.False);Assert.That(state.LastWriteResult,Is.SameAs(write));PersistenceAssertJournal(journal);
                freshObject=new GameObject("FreshPersistenceRecoveryState");var fresh=freshObject.AddComponent<GameState>();Assert.That(fresh.CanPersist,Is.True);Assert.That(fresh.Save.gold,Is.EqualTo(4242));var save=fresh.Save;
                Assert.That(fresh.TryPersist(),Is.True);Assert.That(fresh.Save,Is.SameAs(save));Assert.That(SaveService.Load().gold,Is.EqualTo(4242));Assert.That(state.CanPersist,Is.False);
            }
            finally{typeof(GameState).GetProperty("Instance").SetValue(null,state);if(freshObject!=null)Object.DestroyImmediate(freshObject);PersistenceRestoreJournal(old);}
        }
        [TestCase(0)] [TestCase(1)]
        public void PersistenceExplicitResetRejectionPreservesProgressAndPresentation(int reason)
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);state.RegisterLevelAttempt(level);state.RecordLevelFailure(level,12);state.CompleteLevel(level,level.starScoreThresholds[0]);
            var original=state.Save;string before=JsonUtility.ToJson(original);var win=state.LastCompletionResult;var fail=state.LastFailureResult;var old=PersistenceSnapshotJournal();int events=0;System.Action changed=()=>events++;state.Changed+=changed;
            bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            try{PersistenceSeedBlockedJournal(reason);var journal=PersistenceSnapshotJournal();state.ResetProgress();PersistenceAssertJournal(journal);Assert.That(state.Save,Is.SameAs(original));Assert.That(JsonUtility.ToJson(original),Is.EqualTo(before));Assert.That(state.LastCompletionResult,Is.SameAs(win));Assert.That(state.LastFailureResult,Is.SameAs(fail));Assert.That(events,Is.Zero);Assert.That(state.CanPersist,Is.False);}
            finally{state.Changed-=changed;PersistenceRestoreJournal(old);LogAssert.ignoreFailingMessages=ignored;}
        }
        [Test]
        public void PersistenceExplicitResetCanReplaceRepairedHistoryAndRecoveryCannotResurrectOldProgress()
        {
            var state=GameState.Instance;var level=ChapterLevelCatalog.Find(1,1);state.RegisterLevelAttempt(level);state.RecordLevelFailure(level,12);state.CompleteLevel(level,level.starScoreThresholds[0]);var old=PersistenceSnapshotJournal();int events=0;System.Action changed=()=>events++;
            try
            {
                PersistenceSeedBlockedJournal(0);Assert.That(state.TryPersist(),Is.False);PersistenceRestoreJournal(new string[3]);Assert.That(SaveService.TryReplaceHistory(new PlayerSaveData{gold=4242,lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks},out var repaired),Is.True);
                state.Changed+=changed;state.ResetProgress();state.Changed-=changed;Assert.That(events,Is.EqualTo(1));Assert.That(state.CanPersist,Is.True);Assert.That(state.LastCompletionResult,Is.Null);Assert.That(state.LastFailureResult,Is.Null);Assert.That(state.Save.gold,Is.EqualTo(12540));Assert.That(state.GetLevelProgress(1,1),Is.Null);
                PlayerPrefs.SetString(PersistedSaveKeys[0],"{broken-after-reset");PlayerPrefs.Save();var recovered=SaveService.LoadWithResult();Assert.That(recovered.Status,Is.EqualTo(PlayerSaveLoadStatus.RecoveredBackup));Assert.That(recovered.Data.gold,Is.EqualTo(12540));Assert.That(recovered.Data.levelProgress,Is.Empty);
            }
            finally{state.Changed-=changed;PersistenceRestoreJournal(old);}
        }
        [TestCase(false)] [TestCase(true)]
        public void PersistenceStartupLegacyAndRecoveredBackupRewriteRetainOriginalLoadEvidence(bool backup)
        {
            var previous=GameState.Instance;var old=PersistenceSnapshotJournal();GameObject freshObject=null;
            try
            {
                PersistenceRestoreJournal(new string[3]);var seed=new PlayerSaveData{gold=3131,lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks};
                if(backup){Assert.That(SaveService.TryReplaceHistory(seed,out var write),Is.True);PlayerPrefs.SetString(PersistedSaveKeys[0],"{broken-startup");}
                else{seed.saveVersion=5;PlayerPrefs.SetString(PersistedSaveKeys[0],JsonUtility.ToJson(seed));}PlayerPrefs.Save();
                freshObject=new GameObject("PersistenceStartupRewriteState");var fresh=freshObject.AddComponent<GameState>();Assert.That(fresh.LastLoadResult.Status,Is.EqualTo(backup?PlayerSaveLoadStatus.RecoveredBackup:PlayerSaveLoadStatus.LoadedPrimary));Assert.That(fresh.LastLoadResult.RequiresRewrite,Is.True);
                Assert.That(fresh.Save.gold,Is.EqualTo(3131));Assert.That(fresh.Save.saveVersion,Is.EqualTo(6));Assert.That(fresh.CanPersist,Is.True);Assert.That(fresh.LastWriteResult.Success,Is.True);Assert.That(SaveService.LoadWithResult().Status,Is.EqualTo(PlayerSaveLoadStatus.LoadedPrimary));Assert.That(SaveService.Load().gold,Is.EqualTo(3131));
            }
            finally{typeof(GameState).GetProperty("Instance").SetValue(null,previous);if(freshObject!=null)Object.DestroyImmediate(freshObject);PersistenceRestoreJournal(old);}
        }
        [Test]
        public void PersistenceStartupExhaustedMigrationRewriteLocksOrdinaryRetryWithoutChangingJournal()
        {
            var previous=GameState.Instance;var old=PersistenceSnapshotJournal();GameObject freshObject=null;bool ignored=LogAssert.ignoreFailingMessages;LogAssert.ignoreFailingMessages=true;
            try
            {
                PersistenceRestoreJournal(new string[3]);var seed=new PlayerSaveData{saveVersion=5,gold=3131,lastEnergyUtcTicks=System.DateTime.UtcNow.Ticks};PlayerPrefs.SetString(PersistedSaveKeys[0],PersistenceEncodeEnvelope(seed,long.MaxValue));PlayerPrefs.Save();var journal=PersistenceSnapshotJournal();
                freshObject=new GameObject("PersistenceStartupExhaustedState");var fresh=freshObject.AddComponent<GameState>();Assert.That(fresh.CanPersist,Is.False);Assert.That(fresh.LastWriteResult.Status,Is.EqualTo(PlayerSaveWriteStatus.RevisionExhausted));var write=fresh.LastWriteResult;
                Assert.That(fresh.TryPersist(),Is.False);Assert.That(fresh.LastWriteResult,Is.SameAs(write));PersistenceAssertJournal(journal);Assert.That(fresh.LastLoadResult.Data.saveVersion,Is.EqualTo(5));
            }
            finally{typeof(GameState).GetProperty("Instance").SetValue(null,previous);if(freshObject!=null)Object.DestroyImmediate(freshObject);PersistenceRestoreJournal(old);LogAssert.ignoreFailingMessages=ignored;}
        }
        [Test]
        public void PersistenceObserverExceptionReleasesGuardAfterDurableWrite()
        {
            var state=GameState.Instance;System.Action changed=()=>throw new System.InvalidOperationException("test.persistence.observer");state.Changed+=changed;
            try{Assert.Throws<System.InvalidOperationException>(()=>state.TryPersist());}
            finally{state.Changed-=changed;}
            Assert.That(state.LastWriteResult.Success,Is.True);Assert.That(state.CanPersist,Is.True);long revision=state.LastWriteResult.Revision;Assert.That(state.TryPersist(),Is.True);Assert.That(state.LastWriteResult.Revision,Is.EqualTo(revision+1));
        }
'''+'\n'+s[i:];p.write_text(s)
