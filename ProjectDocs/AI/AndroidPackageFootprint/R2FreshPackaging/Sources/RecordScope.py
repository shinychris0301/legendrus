"""Remove only this run's generated metadata, then compare all source/config hashes."""
import json
import shutil
from Build import ROOT, QA, LOGS, GENERATED, hashes, require_idle, save, sha

require_idle()
before = json.loads((QA / 'BeforeHashes.json').read_text())
cleaned = []
for relative in GENERATED:
    path = ROOT / relative
    saved = LOGS / 'Before' / relative
    if path.exists() and sha(path) == before.get(relative):
        continue
    if relative in before:
        assert saved.exists() and sha(saved) == before[relative]
        shutil.copy2(saved, path)
        cleaned.append({'path': relative, 'action': 'restored task-start generated metadata'})
    elif path.exists():
        path.unlink()
        cleaned.append({'path': relative, 'action': 'removed newly generated metadata'})
for path in (ROOT / 'Assets').glob('InitTestScene*'):
    relative = str(path.relative_to(ROOT))
    if relative not in before and path.name.endswith(('.unity', '.unity.meta')):
        path.unlink()
        cleaned.append({'path': relative, 'action': 'removed new temporary test scene'})
after = hashes()
scope = {'roots': ['Assets', 'Packages', 'ProjectSettings'],
    'changed': sorted(p for p in before if p in after and before[p] != after[p]),
    'added': sorted(set(after) - set(before)), 'deleted': sorted(set(before) - set(after)),
    'unchanged': sum(after.get(p) == value for p, value in before.items()),
    'unityBuild': True, 'editModeTests': True, 'playModeTests': False,
    'deviceRun': False, 'backendActivation': False}
save('Scope.json', scope)
save('GeneratedFileCleanup.json', cleaned)
print(json.dumps(scope, indent=2))
assert not (scope['changed'] or scope['added'] or scope['deleted']), 'Unexpected project changes; inspect without resetting user edits'
