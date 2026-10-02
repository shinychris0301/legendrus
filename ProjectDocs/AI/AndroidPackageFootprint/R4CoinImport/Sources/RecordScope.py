"""Verify that rejected importer experiments left no production changes."""
from pathlib import Path
import hashlib
import json
import subprocess

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
before=json.loads((QA/'BeforeHashes.json').read_text())
after={str(p.relative_to(ROOT)):sha(p) for d in ('Assets','Packages','ProjectSettings') for p in (ROOT/d).rglob('*')
    if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),
    'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),
    'unchanged':sum(after.get(p)==h for p,h in before.items())}
prior=json.loads((BASE.parent/'R2FreshPackaging/QA/Artifact.json').read_text())
apk=ROOT/prior['path'];scope['apkBytes']=apk.stat().st_size;scope['apkSha256']=sha(apk)
scope['apkUnchanged']=scope['apkSha256']==prior['sha256']
(QA/'Scope.json').write_text(json.dumps(scope,indent=2)+'\n')
print(json.dumps(scope,indent=2))
assert not(scope['changed'] or scope['added'] or scope['deleted']) and scope['apkUnchanged']
