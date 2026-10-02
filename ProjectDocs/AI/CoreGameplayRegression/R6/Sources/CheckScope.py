from pathlib import Path
import ast,hashlib,json,difflib,subprocess
base=Path(__file__).resolve().parent.parent
root=Path(__file__).resolve().parents[5]
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for directory in ['Assets','Packages','ProjectSettings','Backend'] for p in (root/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
record={'baselineFiles':len(before),'changed':sorted(k for k in before.keys()&after.keys() if before[k]!=after[k]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'unchanged':sum(before[k]==after[k] for k in before.keys()&after.keys())}
(base/'QA/Scope.json').write_text(json.dumps(record,indent=2))
for p in (base/'Sources').glob('*.py'):ast.parse(p.read_text())
print(json.dumps(record,indent=2))
expected={'Assets/LegendsOfRus/Tests/CampaignPlayMode/MechanicsSpectacleTests.cs',
          'Assets/LegendsOfRus/Tests/PlayMode/AndroidInputCompatibilityPlayModeTests.cs'}
assert set(record['changed'])==expected and not record['added'] and not record['deleted'],record
assert record['unchanged']==7819
patch=[]
for relative in sorted(expected):
    saved=base/'Sources/Before'/relative
    assert hashlib.sha256(saved.read_bytes()).hexdigest()==before[relative]
    patch.extend(difflib.unified_diff(saved.read_text().splitlines(True),(root/relative).read_text().splitlines(True),fromfile='a/'+relative,tofile='b/'+relative))
(base/'Sources/TaskChanges.patch').write_text(''.join(patch))
subprocess.run(['git','apply','--reverse','--check',str(base/'Sources/TaskChanges.patch')],cwd=root,check=True)
assert all(before[path]==after[path] for path in before if path.endswith('.meta'))
