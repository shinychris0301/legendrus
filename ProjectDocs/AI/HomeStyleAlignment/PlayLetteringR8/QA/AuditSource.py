"""Read-only production audit; retains hashes and the narrow owned text diff."""
from pathlib import Path
import datetime
import difflib
import hashlib
import json
import re

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/PlayLetteringR8'
before = json.loads((delivery / 'BeforeProductionHashes.json').read_text())
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
expected = {
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeGoldText.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeView.cs',
    'Assets/LegendsOfRus/Tests/PlayMode/HomeAlignmentPlayModeTests.cs',
    'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/HomeAlignmentDeliveryEditModeTests.cs',
    'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab'
}
changed = []
final = []
for row in before:
    digest = sha(root / row['path'])
    final.append({'path': row['path'], 'sha256': digest})
    if digest != row['sha256']:
        changed.append({'path': row['path'], 'before': row['sha256'], 'sha256': digest})
assert {r['path'] for r in changed} == expected, changed
recorded = {r['path'] for r in before}
new_files = []
addition_scope = ['Assets/LegendsOfRus/Scripts', 'Assets/LegendsOfRus/Tests',
    'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned']
for folder in addition_scope:
    for p in (root / folder).rglob('*'):
        if p.is_file() and p.suffix in {'.cs', '.meta', '.png', '.asset', '.prefab', '.asmdef', '.asmref', '.shader', '.mat'}:
            relative = p.relative_to(root).as_posix()
            if relative not in recorded: new_files.append(relative)
expected_new = { 'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_play_gold' + ext for ext in ['.mat', '.mat.meta', '.shader', '.shader.meta'] }
assert set(new_files) == expected_new, new_files
new_guids = []
for rel in new_files:
    if rel.endswith('.meta'):
        guid = re.search(r'^guid: (.+)$', (root/rel).read_text(), re.M).group(1)
        matches = [p for p in (root/'Assets').rglob('*.meta') if re.search(r'^guid: '+guid+r'$', p.read_text(), re.M)]
        assert len(matches) == 1, matches
        new_guids.append(guid)

current = {r['path']: r['sha256'] for r in final}
old = {r['path']: r['sha256'] for r in before}
metas = [p for p in old if p.endswith('.meta')]
art = [p for p in old if '/HomeAligned/' in p and p.endswith('.png')]
prior = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/FinalSourceAudit.json').read_text())
protected = {name: {'count': len(group['paths']), 'unchanged': all(current[p] == old[p] for p in group['paths'])}
    for name, group in prior['protectedGroups'].items()}

prefab_path = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab'
backup = Path('/tmp/lor-home-r8/before')
old_prefab = (backup / prefab_path).read_text()
new_prefab = (root / prefab_path).read_text()
ids = lambda text: set(re.findall(r'^--- !u!\d+ &(-?\d+)', text, re.M))
old_ids, new_ids = ids(old_prefab), ids(new_prefab)
def rects(text):
    blocks = re.split(r'(?=^--- !u!)', text, flags=re.M)
    return {re.match(r'--- !u!224 &(-?\d+)', b).group(1): b for b in blocks if b.startswith('--- !u!224 ')}
old_rects, new_rects = rects(old_prefab), rects(new_prefab)
rects_preserved = old_rects == new_rects
assert not old_ids - new_ids and rects_preserved
assert re.search(r'^  Revision: 11$', new_prefab, re.M)
assert len(re.findall(r'^  RaisedBadge: 1$', new_prefab, re.M)) == 2
assert all(current[p] == old[p] for p in metas + art)
assert all(g['unchanged'] for g in protected.values())

diff = []
whitespace = []
for row in changed:
    rel = row['path']
    p = root / rel
    assert sha(backup / rel) == old[rel]
    diff.extend(difflib.unified_diff((backup / rel).read_text().splitlines(keepends=True),
        p.read_text().splitlines(keepends=True), fromfile='before/' + rel, tofile='after/' + rel))
    if p.suffix == '.cs':
        issues = [i for i, line in enumerate(p.read_text().splitlines(), 1) if line.rstrip() != line]
        assert not issues and p.read_bytes().endswith(b'\n'), (rel, issues)
        whitespace.append(rel)
audit = {'auditUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'passed': True,
    'baselineCount': len(before), 'unchanged': len(before) - len(changed), 'modified': changed,
    'addedProductionFiles': new_files, 'newUniqueGuids': new_guids, 'additionScanScope': addition_scope,
    'allExistingMetadataBytesAndGuidsPreserved': True, 'metadataCount': len(metas),
    'allHomePngBytesUnchanged': True, 'homePngCount': len(art), 'protectedGroups': protected,
    'prefabIds': {'before': len(old_ids), 'after': len(new_ids), 'retained': len(old_ids & new_ids),
        'removed': sorted(old_ids-new_ids), 'added': sorted(new_ids-old_ids)},
    'allExistingRectTransformsByteIdentical': rects_preserved, 'rectTransformCount': len(old_rects),
    'serializedRevision': 11, 'raisedBadgeCount': 2, 'csharpWhitespaceChecksPassed': whitespace,
    'scope': 'Recorded pre-polish local production baseline and six owned before backups; no secret files or generated caches inspected.'}
(delivery / 'FinalSourceAudit.json').write_text(json.dumps(audit, indent=2) + '\n')
final.extend({'path': p, 'sha256': sha(root/p)} for p in sorted(new_files))
(delivery / 'FinalProductionHashes.json').write_text(json.dumps(final, indent=2) + '\n')
(delivery / 'SourceChangeManifest.json').write_text(json.dumps(changed + [{'path': p, 'added': True, 'sha256': sha(root/p)} for p in sorted(new_files)], indent=2) + '\n')
for rel in sorted(new_files):
    diff.extend(difflib.unified_diff([], (root/rel).read_text().splitlines(keepends=True), fromfile='/dev/null', tofile='after/' + rel))
(delivery / 'AppliedSourceChanges.diff').write_text(''.join(diff))
print(json.dumps({k: audit[k] for k in ['passed', 'baselineCount', 'unchanged', 'metadataCount', 'homePngCount',
    'allExistingRectTransformsByteIdentical', 'rectTransformCount', 'prefabIds']}))
