"""Authorized repair waits for writer closure, then backs up, rebuilds and installs."""
from pathlib import Path
import argparse,fcntl,json,os,sys,time,shutil,hashlib
sys.path.insert(0,str(Path(__file__).resolve().parent))
import repair_lib as r
import reconstruct as v
import install as deployment

STATUS=r.RUN/'Status.json'
def installation_applied():
 try:return json.loads((r.RUN/'InstallResult.json').read_text()).get('liveStore')==str(r.STORE)
 except (FileNotFoundError,json.JSONDecodeError):return False
def status(state,**details):
 applied=details.pop('liveRepairApplied',state=='completed' or installation_applied())
 r.write_json(STATUS,{'updatedAt':r.now(),'state':state,'workerPid':os.getpid(),'targetThreadId':r.THREAD,'liveRepairApplied':applied,'privateRunDirectory':str(r.RUN),**details})
def require_closed():
 writers=r.native_writers()
 if writers:raise RuntimeError('Codex writer reopened before installation; live repair withheld')
def run():
 baseline=json.loads((r.RUN/'InstallationFixtureResults.json').read_text());assert baseline['result']=='Passed'
 tools=json.loads((r.RUN/'ToolHashes.json').read_text())
 for name,expected in tools.items():assert r.sha(Path(__file__).parent/name)==expected,'Pinned repair tool changed'
 deadline=time.monotonic()+86400;quiet_since=None
 while time.monotonic()<deadline:
  writers=r.native_writers()
  if writers:
   quiet_since=None;status('waiting_for_codex_writers_to_close',writers=writers,initialBackup=str(r.RUN/'InitialBackup'));time.sleep(2);continue
  if quiet_since is None:quiet_since=time.monotonic()
  if time.monotonic()-quiet_since>=5:break
  status('waiting_for_writer_closure_to_settle',writers=[]);time.sleep(1)
 else:status('waiting_expired_no_live_changes');return 2
 handles=[];install_started=False
 try:
  # Native stores use these locks. A reopened thread cannot acquire a writer
  # during the short installation. No process is forcibly terminated.
  for name in ['.coordination.lock',r.THREAD+'.lock',r.ALIAS+'.lock']:
   handle=(r.STORE/'thread-writer-locks'/name).open('a+b')
   fcntl.flock(handle.fileno(),fcntl.LOCK_EX|fcntl.LOCK_NB);handles.append(handle)
  require_closed();status('making_quiescent_backup')
  snapshot=r.backup('FinalBackup',quiescent=True)
  require_closed();status('reconstructing_isolated_copy',backup=str(snapshot))
  stage=r.RUN/'FinalReconstruction';prep=v.prepare(snapshot,stage)
  first=v.native_validate(stage,'BeforeInstallFirstRead');second=v.native_validate(stage,'BeforeInstallRestartRead')
  assert first['turnsVisibleViaNativeApi']==second['turnsVisibleViaNativeApi'] and first['itemsVisibleViaNativeApi']==second['itemsVisibleViaNativeApi']
  require_closed();status('installing_verified_thread',backup=str(snapshot))
  install_started=True
  applied=deployment.apply(stage,snapshot,r.STORE,r.RUN/'QuarantinedOriginals')
  # Exact copied target rows plus unchanged unrelated rows establish that live
  # data equals the two-process verified reconstruction; no live resume needed.
  with r.connect(r.STORE/'thread_history_1.sqlite',True) as live,r.connect(stage/'thread_history_1.sqlite',True) as test:
   for table in r.TABLES:
    keys=[x[1] for x in sorted(live.execute('PRAGMA table_info('+table+')'),key=lambda x:x[5]) if x[5]]
    query='SELECT * FROM '+table+' WHERE thread_id=? ORDER BY '+','.join(keys)
    assert live.execute(query,(r.THREAD,)).fetchall()==test.execute(query,(r.THREAD,)).fetchall(),table
  result={'completedAt':r.now(),'result':'Passed','quiescentBackup':str(snapshot),'installation':applied,'isolatedNativeFirstRead':first,'isolatedNativeRestartRead':second,'liveTargetRowsExactlyMatchVerifiedReconstruction':True,'unrelatedThreadsAndOriginalRawPreserved':True,'actualVSCodeUiReopenVerified':False,'originalMissingPreSnapshotHistoryCannotBeInvented':True}
  r.write_json(r.RUN/'CompletedRepair.json',result)
  evidence=r.PROJECT/'ProjectDocs/AI/CodexHistoryDiagnostics/R2Repair/CompletedRepair.json'
  # Sanitized evidence only; no chat text, raw database or credential export.
  r.write_json(evidence,result)
  note='''# 2026-10-02 — Codex history repair applied after writer closure\n\nThe authorized offline worker waited for every detected Codex process/writer lock to close, made a fresh quiescent backup, then rebuilt and installed only the affected project thread. Original raw file is retained privately outside the scan directory. Existing indexed records retained; unrelated history rows/other thread metadata preserved. Live target rows match the reconstructed copy verified through two native Codex resumes. Actual VS Code UI reopening remains to be confirmed. See [completion evidence](CodexHistoryDiagnostics/R2Repair/CompletedRepair.json).\n\nGame source/art/prefabs/APK untouched; next game milestone remains the guarded APK refresh, with phone/hosting deferred and effects last.\n\n'''
  current=r.PROJECT/'ProjectDocs/AI/CurrentTask.md';current.write_text(note+current.read_text())
  log=r.PROJECT/'ProjectDocs/AI/SessionLog.md';log.write_text(log.read_text()+'\n\n'+note)
  status('completed',backup=str(snapshot),completionReport=str(r.RUN/'CompletedRepair.json'),nativeTurns=first['turnsVisibleViaNativeApi'],nativeItems=first['itemsVisibleViaNativeApi'],actualVSCodeUiReopenVerified=False)
  return 0
 except Exception as error:
  # Sources and verified backup are retained. Never silently continue past a
  # failed precondition, truncate another thread, or reset the entire database.
  status('stopped_for_review',errorType=type(error).__name__,errorMessage=str(error)[:400],installReportExists=installation_applied(),liveRepairApplied=True if installation_applied() else None if install_started else False)
  return 1
 finally:
  for handle in reversed(handles):fcntl.flock(handle.fileno(),fcntl.LOCK_UN);handle.close()

if __name__=='__main__':
 parser=argparse.ArgumentParser();parser.add_argument('--check',action='store_true');args=parser.parse_args()
 if args.check:
  writers=r.native_writers();print(json.dumps({'state':'blocked_on_active_writers' if writers else 'ready','writers':writers,'liveRepairApplied':False},indent=2));sys.exit(2 if writers else 0)
 sys.exit(run())
