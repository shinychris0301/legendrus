"""Complete worker run against a private fixture, with simulated process closure."""
from pathlib import Path
import json,os,shutil,sys,time
sys.path.insert(0,str(Path(__file__).resolve().parent))
import repair_lib as r
import offline_worker as worker
real_run=r.RUN;fixture=real_run/'WorkerFixture';fixture.mkdir(mode=0o700,exist_ok=False)
store=fixture/'Store';store.mkdir(mode=0o700)
snapshot=real_run/'InitialBackup';manifest=json.loads((snapshot/'manifest.json').read_text())
for name in ['state_5.sqlite','thread_history_1.sqlite']:shutil.copyfile(snapshot/name,store/name);(store/name).chmod(0o600)
mapping={}
for row in manifest['files']:
 path=store/'sessions/2026/09/30'/Path(row['backup']).name;path.parent.mkdir(parents=True,exist_ok=True,mode=0o700);shutil.copyfile(row['backup'],path);path.chmod(0o600);mapping[row['threadId']]=path
with r.connect(store/'state_5.sqlite') as c:
 for tid,path in c.execute('SELECT id,rollout_path FROM threads').fetchall():c.execute('UPDATE threads SET rollout_path=? WHERE id=?',(str(mapping.get(tid,store/'missing-other-rollouts'/Path(path).name)),tid))
 c.commit();r.checkpoint(c)
locks=store/'thread-writer-locks';locks.mkdir(mode=0o700)
project=fixture/'Project';docs=project/'ProjectDocs/AI';docs.mkdir(parents=True,mode=0o700)
for name in ['CurrentTask.md','SessionLog.md']:(docs/name).write_text('Fixture continuity baseline\n')
job=fixture/'Job';job.mkdir(mode=0o700)
shutil.copyfile(real_run/'InstallationFixtureResults.json',job/'InstallationFixtureResults.json')
tool_files=['repair_lib.py','reconstruct.py','install.py','offline_worker.py']
r.write_json(job/'ToolHashes.json',{name:r.sha(Path(__file__).parent/name) for name in tool_files})
r.STORE=store;r.RUN=job;r.PROJECT=project;worker.STATUS=job/'Status.json'
checks={'writerGuardCalls':0,'noLiveChangesDuringSimulatedWriterPresence':False}
original=mapping[r.THREAD];original_hash=r.sha(original);started=time.monotonic()
def closure_fixture():
 checks['writerGuardCalls']+=1
 if time.monotonic()-started<3:
  assert original.exists() and r.sha(original)==original_hash
  assert not (job/'FinalBackup').exists()
  checks['noLiveChangesDuringSimulatedWriterPresence']=True
  return [{'pid':999999,'simulated':True}]
 return []
r.native_writers=closure_fixture
code=worker.run();status=json.loads(worker.STATUS.read_text());assert code==0 and status['state']=='completed' and status['liveRepairApplied']
completed=json.loads((job/'CompletedRepair.json').read_text());assert completed['liveTargetRowsExactlyMatchVerifiedReconstruction']
assert completed['isolatedNativeFirstRead']['itemsVisibleViaNativeApi']==completed['isolatedNativeRestartRead']['itemsVisibleViaNativeApi']==3242
assert completed['unrelatedThreadsAndOriginalRawPreserved']
assert 'Fixture continuity baseline' in (docs/'CurrentTask.md').read_text()
report={'result':'Passed','actualLiveStoreModified':False,'isolatedFullWorkerFixture':True,'actualNativeReconstructionAndSurgicalInstall':True,'writerClosureInThisFixtureSimulated':True,**checks,'completedStatusVerified':True,'nativeTurns':status['nativeTurns'],'nativeItems':status['nativeItems'],'originalQuarantinedFileHashPreserved':True,'unrelatedHistoryPreserved':True}
r.write_json(real_run/'WorkerFixtureResults.json',report);print(json.dumps(report,indent=2))
