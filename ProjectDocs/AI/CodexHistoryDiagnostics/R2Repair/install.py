"""Install only one repaired thread; retain original rollout in private quarantine."""
from pathlib import Path
import hashlib,json,os,shutil,sqlite3
import repair_lib as r
from reconstruct import column_names

def scope_digest(connection,exclude_primary=True):
 result={}
 for table in r.TABLES:
  names=column_names(connection,table);primary=[x[1] for x in sorted(connection.execute('PRAGMA table_info('+table+')'),key=lambda x:x[5]) if x[5]]
  query='SELECT '+','.join(names)+' FROM '+table
  args=()
  if exclude_primary:query+=' WHERE thread_id<>?';args=(r.THREAD,)
  query+=' ORDER BY '+','.join(primary)
  count=0;digest=hashlib.sha256()
  for row in connection.execute(query,args):
   digest.update(json.dumps(row,separators=(',',':'),ensure_ascii=False).encode());digest.update(b'\n');count+=1
  result[table]={'rows':count,'sha256':digest.hexdigest()}
 return result

def apply(stage,snapshot,store,quarantine):
 prep=json.loads((stage/'Preparation.json').read_text());manifest=json.loads((snapshot/'manifest.json').read_text())
 target_backup=next(x for x in manifest['files'] if x['threadId']==r.THREAD)
 with r.connect(store/'state_5.sqlite',True) as c:
  original=Path(c.execute('SELECT rollout_path FROM threads WHERE id=?',(r.THREAD,)).fetchone()[0])
  assert c.execute('SELECT count(*) FROM threads WHERE rollout_path=?',(str(original),)).fetchone()[0]==1
  original_metadata=c.execute('SELECT * FROM threads WHERE id=?',(r.THREAD,)).fetchone();names=column_names(c,'threads')
 assert r.sha(original)==target_backup['sha256'],'Live raw file changed since snapshot'
 canonical=original.parent/('rollout-2026-09-30T09-50-16-'+r.THREAD+'.jsonl')
 assert not canonical.exists(),'Canonical target already exists'
 with r.connect(store/'thread_history_1.sqlite',True) as c:before_scope=scope_digest(c)
 with r.connect(stage/'thread_history_1.sqlite',True) as c:assert scope_digest(c)==before_scope,'Reconstruction changed unrelated history'
 temp=canonical.with_name(canonical.name+'.repair-tmp');shutil.copyfile(Path(prep['canonicalRollout']),temp);temp.chmod(0o600)
 assert r.sha(temp)==r.sha(Path(prep['canonicalRollout']))
 with temp.open('rb') as f:os.fsync(f.fileno())
 os.replace(temp,canonical)
 try:
  with r.connect(store/'thread_history_1.sqlite') as c:
   c.execute('ATTACH DATABASE ? AS reconstructed',(str(stage/'thread_history_1.sqlite'),))
   c.execute('ATTACH DATABASE ? AS metadata',(str(store/'state_5.sqlite'),))
   c.execute('BEGIN IMMEDIATE')
   for table in r.TABLES:
    cols=column_names(c,table);primary=[x[1] for x in sorted(c.execute('PRAGMA table_info('+table+')'),key=lambda x:x[5]) if x[5]]
    update=','.join(name+'=excluded.'+name for name in cols if name not in primary)
    c.execute('INSERT INTO main.'+table+' ('+','.join(cols)+') SELECT '+','.join(cols)+' FROM reconstructed.'+table+' WHERE thread_id=? ON CONFLICT('+','.join(primary)+') DO UPDATE SET '+update,(r.THREAD,))
   c.execute("UPDATE metadata.threads SET rollout_path=?,history_mode='paginated' WHERE id=?",(str(canonical),r.THREAD));c.commit()
   assert scope_digest(c)==before_scope
   assert c.execute('PRAGMA quick_check').fetchone()[0]=='ok'
   r.checkpoint(c)
  with r.connect(store/'state_5.sqlite') as c:
   after=c.execute('SELECT * FROM threads WHERE id=?',(r.THREAD,)).fetchone()
   assert [name for name,a,b in zip(names,original_metadata,after) if a!=b]==['rollout_path']
   assert c.execute('PRAGMA quick_check').fetchone()[0]=='ok';r.checkpoint(c)
 except Exception:
  # Keep every source/backup. If metadata did not commit, remove only our new file.
  with r.connect(store/'state_5.sqlite',True) as c:pointing=c.execute('SELECT rollout_path FROM threads WHERE id=?',(r.THREAD,)).fetchone()[0]
  if pointing!=str(canonical):canonical.unlink()
  raise
 quarantine.mkdir(parents=True,exist_ok=True,mode=0o700);quarantined=quarantine/original.name
 assert not quarantined.exists()
 os.replace(original,quarantined);quarantined.chmod(0o600)
 assert r.sha(quarantined)==target_backup['sha256']
 result={'appliedAt':r.now(),'threadId':r.THREAD,'canonicalRollout':str(canonical),'canonicalHash':r.sha(canonical),'originalRawPreservedAt':str(quarantined),'originalHash':r.sha(quarantined),'messageBodiesUnchangedByOrdinalReconciliation':True,'unrelatedHistoryScope':before_scope,'unrelatedHistoryByteValuesPreserved':True,'threadMetadataChangedFields':['rollout_path'],'originalRawRemovedFromScanDirectoryByRelocation':True,'backup':str(snapshot),'liveStore':str(store)}
 r.write_json(quarantine.parent/'InstallResult.json',result);return result

if __name__=='__main__':
 live=r.RUN/'InstallFixture';live.mkdir(mode=0o700,exist_ok=False)
 snapshot=r.RUN/'InitialBackup'
 for name in ['state_5.sqlite','thread_history_1.sqlite']:shutil.copyfile(snapshot/name,live/name);(live/name).chmod(0o600)
 manifest=json.loads((snapshot/'manifest.json').read_text());raw=Path(next(x['backup'] for x in manifest['files'] if x['threadId']==r.THREAD));target=live/'sessions/2026/09/30'/raw.name;target.parent.mkdir(parents=True,mode=0o700);shutil.copyfile(raw,target);target.chmod(0o600)
 with r.connect(live/'state_5.sqlite') as c:
  for tid,path in c.execute('SELECT id,rollout_path FROM threads').fetchall():c.execute('UPDATE threads SET rollout_path=? WHERE id=?',(str(target) if tid==r.THREAD else str(live/'missing-other-rollouts'/Path(path).name),tid))
  c.commit();r.checkpoint(c)
 result=apply(r.RUN/'Rehearsal4',snapshot,live,r.RUN/'InstallFixtureArtifacts/QuarantinedOriginals')
 # Native verification points to the installed fixture, without exposing live paths.
 prep=json.loads((r.RUN/'Rehearsal4/Preparation.json').read_text());prep['canonicalRollout']=result['canonicalRollout'];r.write_json(live/'Preparation.json',prep)
 (live/'config.toml').write_text('[analytics]\nenabled=false\n[features]\nplugins=false\n');(live/'config.toml').chmod(0o600)
 from reconstruct import native_validate
 a=native_validate(live,'InstalledFirstRead');b=native_validate(live,'InstalledRestartRead')
 assert a['itemsVisibleViaNativeApi']==b['itemsVisibleViaNativeApi']==3242
 r.write_json(r.RUN/'InstallationFixtureResults.json',{'result':'Passed','installScopePassed':True,'first':a,'restart':b})
 print(json.dumps({'result':'Passed','unrelatedHistoryPreserved':True,'originalRawFilePreserved':True,'nativeTurns':a['turnsVisibleViaNativeApi'],'nativeItems':a['itemsVisibleViaNativeApi'],'nativeRestartCountsUnchanged':True},indent=2))
