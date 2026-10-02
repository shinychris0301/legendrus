from pathlib import Path
import hashlib
import json
import subprocess

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
assert not any(name.strip() in ('Unity','Unity Main Thre') for name in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
record=json.loads((BASE/'Sources/TemporaryFixture.json').read_text())
path=ROOT/record['path'];meta=path.with_suffix('.cs.meta')
assert hashlib.sha256(path.read_bytes()).hexdigest()==record['sourceSha256']
assert hashlib.sha256(path.read_bytes()).hexdigest()==hashlib.sha256((BASE/'Sources/FixturePrototype.cs').read_bytes()).hexdigest()
assert 'guid: '+record['guid'] in meta.read_text()
removed=[str(path.relative_to(ROOT)),str(meta.relative_to(ROOT))]
path.unlink();meta.unlink()
(BASE/'QA/TemporaryFixtureCleanup.json').write_text(json.dumps({'removedExactOwnedFiles':removed,'temporaryGuid':record['guid'],'noUnityProcessDuringCleanup':True},indent=2)+'\n')
print('Removed only the archived temporary rehearsal source and its own metadata.')
