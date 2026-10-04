"""Commit every pinned project/evidence path in bounded batches and publish normally."""
from pathlib import Path
import hashlib,json,os,re,subprocess,sys,time
ROOT=Path(__file__).resolve().parents[4]
JOB=Path(sys.argv[1]);BEFORE=json.loads((JOB/'Before.json').read_text())
BUDGET=384*1024**2
def run(*args,data=None,env=None):
    r=subprocess.run(['git',*args],cwd=ROOT,input=data,capture_output=True,env=env)
    if r.returncode:
        (JOB/'LastGitFailure.stderr').write_bytes(r.stderr)
        raise RuntimeError('Git '+args[0]+' failed; private diagnostic retained.')
    return r.stdout
def save(name,obj):
    p=JOB/name;t=p.with_suffix(p.suffix+'.new');t.write_text(json.dumps(obj,indent=2)+'\n');t.replace(p)
def sha(path):
    with path.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def names(args):return [p.decode() for p in run(*args).split(b'\0') if p]
assert run('rev-parse','HEAD').decode().strip()==BEFORE['head']
assert run('branch','--show-current').decode().strip()==BEFORE['branch']
assert not names(('diff','--cached','--name-only','-z')),'Existing staging must be preserved'
run('diff','--check')
roots=['ProjectDocs/AI/'+d for d in ('SpendingRecovery','EnergyRecovery','NarrativeRecovery','TutorialRecovery','SelectionRecovery','RoundLifecycleRecovery','SettingsRecovery','PersistenceRecovery','StartupRecovery')]
ignored=names(('ls-files','--others','--ignored','--exclude-standard','-z','--',*roots))
assert all('/QA/Build/' in n and Path(n).name in ('Build.launcher.log','Build.unity.log','summary.json') for n in ignored)
paths=sorted(set(names(('diff','--name-only','-z'))+names(('ls-files','--others','--exclude-standard','-z'))+ignored))
assert all(n.startswith(('Assets/LegendsOfRus/','ProjectDocs/AI/')) for n in paths)
patterns={
 'private-key':re.compile(rb'-----BEGIN (?:RSA |EC |OPENSSH |DSA |ENCRYPTED )?PRIVATE KEY-----'),
 'github-token':re.compile(rb'(?:gh[pousr]_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{50,})'),
 'openai-key':re.compile(rb'\bsk-(?:proj-|svcacct-)?[A-Za-z0-9_\-]{32,}')}
rows=[];findings=[];text_checked=0
for name in paths:
    p=ROOT/name;assert p.is_file() and not p.is_symlink()
    assert p.suffix.lower() not in ('.keystore','.jks','.pem','.key','.p12','.apk','.aab') and not p.name.startswith('.env')
    assert not any(part.lower() in ('library','temp','obj','logs','usersettings','.utmp') for part in p.parts[len(ROOT.parts):])
    if p.suffix!='.png':
        text_checked+=1;data=p.read_bytes()
        for label,pattern in patterns.items():
            if pattern.search(data):findings.append({'path':name,'pattern':label})
    rows.append({'path':name,'bytes':p.stat().st_size,'sha256':sha(p)})
save('CandidateReview.json',{'paths':len(rows),'bytes':sum(r['bytes'] for r in rows),'textFilesChecked':text_checked,'findings':findings,'explicitQABuildDiagnosticsIncluded':ignored})
assert not findings,'Sensitive-content findings require review; values were not printed'
images=[r for r in rows if Path(r['path']).suffix=='.png'];text=[r for r in rows if Path(r['path']).suffix!='.png']
assert sum(r['bytes'] for r in text)<BUDGET
batches=[text];queued=[];size=0
for row in images:
    assert row['bytes']<BUDGET
    if queued and size+row['bytes']>BUDGET:batches.append(queued);queued=[];size=0
    queued.append(row);size+=row['bytes']
if queued:batches.append(queued)
save('Plan.json',{'baseCommit':BEFORE['head'],'branch':BEFORE['branch'],'budgetBytes':BUDGET,'paths':rows,'batches':[{'files':len(b),'bytes':sum(r['bytes'] for r in b)} for b in batches]})
remote=json.loads((JOB/'RemoteBefore.json').read_text())
env=os.environ.copy();env['GIT_TERMINAL_PROMPT']='0'
if remote.get('vsCodeGitSocket'):
    env.update(GIT_ASKPASS='/usr/share/code/resources/app/extensions/git/dist/askpass.sh',VSCODE_GIT_ASKPASS_NODE='/usr/share/code/code',VSCODE_GIT_ASKPASS_MAIN='/usr/share/code/resources/app/extensions/git/dist/askpass-main.js',VSCODE_GIT_IPC_HANDLE=remote['vsCodeGitSocket'])
publish=remote.get('verified') is True
if publish:assert remote['head']==BEFORE['head'],'Remote branch diverged; keep local work and review'
progress={'phase':'committing','baseCommit':BEFORE['head'],'branch':BEFORE['branch'],'totalBatches':len(batches),'batches':[],'remotePublicationAvailable':publish}
parent=BEFORE['head']
for index,batch in enumerate(batches,1):
    assert run('rev-parse','HEAD').decode().strip()==parent
    for row in batch:assert sha(ROOT/row['path'])==row['sha256'],row['path']
    regular=b''.join(r['path'].encode()+b'\0' for r in batch if r['path'] not in ignored)
    forced=b''.join(r['path'].encode()+b'\0' for r in batch if r['path'] in ignored)
    if regular:run('add','--pathspec-from-file=-','--pathspec-file-nul',data=regular)
    if forced:run('add','--force','--pathspec-from-file=-','--pathspec-file-nul',data=forced)
    assert set(names(('diff','--cached','--name-only','-z')))=={r['path'] for r in batch}
    if index==1:
        active=[r['path'] for r in batch if r['path'].startswith('Assets/') or '/Before/' not in r['path'] and Path(r['path']).suffix=='.py']
        run('diff','--cached','--check','--',*active)
    message=('Fix acknowledged save recovery and commit verified development records' if index==1 else f'Retain complete recovery QA evidence batch {index-1}/{len(batches)-1}')
    run('-c','gc.auto=0','commit','-m',message)
    commit=run('rev-parse','HEAD').decode().strip();assert run('rev-parse',commit+'^').decode().strip()==parent
    for row in batch:
        oid=run('rev-parse',':'+row['path']).decode().strip();expected=run('hash-object','--',row['path']).decode().strip();assert oid==expected
        row['blob']=oid
    item={'index':index,'commit':commit,'files':len(batch),'bytesBeforeGitCompression':sum(r['bytes'] for r in batch),'remoteVerified':False}
    progress['batches'].append(item);save('Progress.json',progress)
    print(f'Committed batch {index}/{len(batches)}: {commit[:8]} ({len(batch)} files)',flush=True)
    if publish:
        log=JOB/f'Push-{index}.log'
        with log.open('wb') as stream:
            r=subprocess.run(['git','-c','pack.threads=2','-c','pack.window=0','push','--progress','origin',commit+':refs/heads/'+BEFORE['branch']],cwd=ROOT,stdout=stream,stderr=stream,env=env)
        if r.returncode:
            publish=False;progress['remotePublicationFailureAtBatch']=index
            print('Remote upload failed; continuing all authorized local commits. Private logs retained.',flush=True)
        else:
            current=run('ls-remote','origin','refs/heads/'+BEFORE['branch'],env=env).decode().split()[0];assert current==commit
            item['remoteVerified']=True;print(f'Uploaded and verified batch {index}/{len(batches)}',flush=True)
        save('Progress.json',progress)
    parent=commit
tree={}
for record in run('ls-tree','-r','-z','HEAD').split(b'\0'):
    if record:
        info,path=record.split(b'\t',1);tree[path.decode()]=info.split()[2].decode()
assert all(tree[r['path']]==r['blob'] and sha(ROOT/r['path'])==r['sha256'] for r in rows)
assert not names(('diff','--name-only','-z')) and not names(('ls-files','--others','--exclude-standard','-z'))
progress.update(phase='snapshot_complete',projectSnapshotCommit=parent,allUpdatedBlobIdsMatchReviewedSnapshot=True,updatedPaths=len(rows),textFilesScreened=text_checked,remoteSnapshotVerified=all(r['remoteVerified'] for r in progress['batches']))
save('Progress.json',progress);save('FinalManifest.json',rows)
print(json.dumps({k:progress[k] for k in ('phase','projectSnapshotCommit','updatedPaths','remoteSnapshotVerified')},indent=2),flush=True)
