"""Verify the bounded builder change and unchanged APK against task-start hashes."""
from pathlib import Path
import hashlib
import json

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
def sha(p):
    with p.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
before=json.loads((QA/'BeforeHashes.json').read_text())
after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings') for p in (ROOT/folder).rglob('*')
    if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
expected=json.loads((BASE.parent/'R2FreshPackaging/QA/Artifact.json').read_text())
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),
    'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items()),
    'apkBytes':apk.stat().st_size,'apkSha256':sha(apk)}
scope['apkUnchanged']=scope['apkSha256']==expected['sha256']
(QA/'Scope.json').write_text(json.dumps(scope,indent=2)+'\n')
print(json.dumps(scope,indent=2))
expected_changes={f'Assets/LegendsOfRus/Scripts/Runtime/UI/Production{page}Runtime.cs' for page in ('WorldMap','ChapterMap','PreLevel')}
expected_added={'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/FreshPageBuilderEditModeTests.cs'+suffix for suffix in ('','.meta')}
assert set(scope['changed'])==expected_changes and set(scope['added'])==expected_added
assert not scope['deleted'] and scope['apkUnchanged']
