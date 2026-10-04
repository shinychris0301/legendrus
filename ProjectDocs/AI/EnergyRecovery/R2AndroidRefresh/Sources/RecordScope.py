"""Restore only generated build metadata, then prove all task-start project inputs retained."""
import json
import shutil
from Build import ROOT, BASE, QA, LOGS, GENERATED, hashes, require_idle, save, sha

require_idle()
before = json.loads((QA / 'BeforeHashes.json').read_text())
prebuild = json.loads((QA / 'BeforeBuildHashes.json').read_text())
cleaned = []
for relative in GENERATED:
    path = ROOT / relative
    if path.exists() and sha(path) == prebuild.get(relative):
        continue
    if relative in prebuild:
        saved = LOGS / 'Before' / relative
        assert saved.exists() and sha(saved) == prebuild[relative]
        shutil.copy2(saved, path)
        cleaned.append({'path': relative, 'action': 'restored pre-build generated metadata'})
    elif path.exists():
        path.unlink()
        cleaned.append({'path': relative, 'action': 'removed new generated metadata'})
after = hashes()
scope = {'baselineFiles': len(before), 'finalFiles': len(after),
    'unchanged': sum(after.get(path) == value for path, value in before.items()),
    'changed': sorted(path for path in before if path in after and before[path] != after[path]),
    'added': sorted(set(after) - set(before)), 'deleted': sorted(set(before) - set(after))}
save('Scope.json', scope)
save('GeneratedFileCleanup.json', cleaned)
assert before == after, scope
assert len(before) == 7839
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/EnergyRefreshBuildAudit.cs'
assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
cache = BASE / 'Sources/__pycache__'
if cache.exists():
    shutil.rmtree(cache)
print(json.dumps(scope, indent=2))
