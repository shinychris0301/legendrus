from pathlib import Path
import ast,hashlib,json
base=Path(__file__).resolve().parent.parent
root=Path(__file__).resolve().parents[5]
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for directory in ['Assets','Packages','ProjectSettings','Backend'] for p in (root/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts}
record={'baselineFiles':len(before),'changed':sorted(k for k in before.keys()&after.keys() if before[k]!=after[k]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'unchanged':sum(before[k]==after[k] for k in before.keys()&after.keys())}
(base/'QA/Scope.json').write_text(json.dumps(record,indent=2))
for p in (base/'Sources').glob('*.py'):ast.parse(p.read_text())
print(json.dumps(record,indent=2))
assert not record['changed'] and not record['added'] and not record['deleted'],'Inspect unexpected scope changes before acceptance'
