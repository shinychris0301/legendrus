from pathlib import Path
import hashlib,json,shutil,subprocess
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
if subprocess.run(['pgrep','-x','Unity'],capture_output=True).returncode==0:raise SystemExit('Unity must be closed before cleanup.')
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
record=base/'QA/RemovedTemporaryScenes.json'
removed=json.loads(record.read_text())['paths'] if record.exists() else []
for p in (root/'Assets').glob('InitTestScene*'):
 name=str(p.relative_to(root))
 if name not in before and (p.name.endswith('.unity') or p.name.endswith('.unity.meta')):
  p.unlink();removed.append(name)
record.write_text(json.dumps({'reason':'New Unity test-runner scene/meta artifacts from interrupted verification; absent from task-start inventory','paths':sorted(set(removed))},indent=2))
for suffix in ['', '.meta']:
 p=root/('Assets/AddressableAssetsData/link.xml'+suffix)
 saved=base/('Sources/link.xml'+suffix)
 if saved.exists():shutil.copy2(saved,p)
 elif str(p.relative_to(root)) not in before and p.exists():p.unlink()
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2));print(json.dumps(scope,indent=2))
