from pathlib import Path
import hashlib,json,difflib,subprocess,ast
base=Path(__file__).resolve().parent.parent
root=Path(__file__).resolve().parents[5]
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for directory in ['Assets','Packages','ProjectSettings','Backend'] for p in (root/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts}
changed=sorted(k for k in before.keys()&after.keys() if before[k]!=after[k])
record={'baselineFiles':len(before),'changed':changed,'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'unchanged':sum(before[k]==after[k] for k in before.keys()&after.keys())}
(base/'QA/Scope.json').write_text(json.dumps(record,indent=2))
patch=[]
for key in changed:
    snapshot=base/'Sources/Before'/key
    assert hashlib.sha256(snapshot.read_bytes()).hexdigest()==before[key],key
    patch.extend(difflib.unified_diff(snapshot.read_text().splitlines(True),(root/key).read_text().splitlines(True),fromfile='a/'+key,tofile='b/'+key))
(base/'Sources/Changes.patch').write_text(''.join(patch))
for p in (base/'Sources').glob('*.py'):ast.parse(p.read_text())
# Check only this task's additions; the working tree already contained older prefab whitespace.
for key in changed:
    result=subprocess.run(['git','diff','--no-index','--check',str(base/'Sources/Before'/key),str(root/key)],capture_output=True,text=True)
    assert result.returncode in (0,1) and not result.stdout and not result.stderr,(key,result.stdout,result.stderr)
record['taskWhitespaceCheck']='passed against task-start snapshots'
(base/'QA/Scope.json').write_text(json.dumps(record,indent=2))
print(json.dumps(record,indent=2))
