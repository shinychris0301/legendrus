"""Read-only audit against the accepted Home R10 production baseline."""
from pathlib import Path
import datetime
import difflib
import hashlib
import json
import re

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/FrontEndHomeAlignment/R1'
backup = Path('/tmp/lor-front-home-r1/before')
before = json.loads((delivery / 'BeforeProductionHashes.json').read_text())
old = {r['path']: r['sha256'] for r in before}
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
current = {p: sha(root / p) for p in old}
changed = [{'path': p, 'before': old[p], 'sha256': current[p]} for p in old if old[p] != current[p]]
prefix = 'Assets/LegendsOfRus/'
expected = {prefix + p for p in (
    'Resources/ProductionUI/Pages/SplashTitle.prefab',
    'Resources/ProductionUI/Pages/Loading.prefab',
    'Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json',
    'Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs',
    'Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs',
    'Scripts/Editor/SplashTitleR3Validation.cs',
    'Scripts/Runtime/UI/SplashTitleView.cs',
    'Scripts/Runtime/UI/ProductionLoadingRuntime.cs',
    'Scripts/Runtime/UI/ProductionLoadingView.cs',
    'Tests/ProductionCoreLoopUiEditMode/HomeAlignmentDeliveryEditModeTests.cs',
    'Tests/ProductionCoreLoopUiEditMode/SplashTitleBuildWiringEditModeTests.cs',
    'Tests/PlayMode/SplashResponsivePlayModeTests.cs',
    'Tests/PlayMode/LoadingAlignmentPlayModeTests.cs')}
assert {r['path'] for r in changed} == expected, changed
new_folder = prefix + 'Resources/ProductionUI/FrontEndHomeAligned'
added = sorted([new_folder + '.meta'] + [p.relative_to(root).as_posix() for p in (root / new_folder).iterdir() if p.is_file()])
assert len(added) == 9 and all(p not in old for p in added), added
new_guids = {}
all_meta = list((root / 'Assets').rglob('*.meta'))
for p in added:
    if not p.endswith('.meta'): continue
    guid = re.search(r'^guid: (.+)$', (root / p).read_text(), re.M).group(1)
    matches = [str(q) for q in all_meta if re.search(r'^guid: '+guid+r'$', q.read_text(), re.M)]
    assert len(matches) == 1, matches
    new_guids[p] = guid
metas = [p for p in old if p.endswith('.meta')]
pngs = [p for p in old if p.endswith('.png')]
assert all(old[p] == current[p] for p in metas + pngs)
prior = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/FinalSourceAudit.json').read_text())
protected = {k: {'paths': v['paths'], 'unchanged': all(old[p] == current[p] for p in v['paths'])}
             for k, v in prior['protectedGroups'].items() if k not in ('Splash', 'Loading')}
home = [p for p in old if '/HomeAligned/' in p or p.endswith('/Home.prefab') or '/Scripts/' in p and 'ProductionHome' in p]
protected['HomeR10'] = {'paths': home, 'unchanged': all(old[p] == current[p] for p in home)}
assert all(v['unchanged'] for v in protected.values())

def blocks(text):
    return {m.group(2): (m.group(1), m.group(0)) for m in re.finditer(r'^--- !u!(\d+) &(-?\d+)\n.*?(?=^--- !u!|\Z)', text, re.M | re.S)}

prefabs = {}
for page, revision in [('SplashTitle', 7), ('Loading', 4)]:
    rel = prefix + 'Resources/ProductionUI/Pages/' + page + '.prefab'
    previous, final = (backup / rel).read_text(), (root / rel).read_text()
    a, b = blocks(previous), blocks(final)
    assert a.keys() <= b.keys(), page
    old_rects = {k: v[1] for k, v in a.items() if v[0] == '224'}
    new_rects = {k: v[1] for k, v in b.items() if v[0] == '224'}
    assert old_rects.keys() == new_rects.keys()
    changed_rects = [k for k in old_rects if old_rects[k] != new_rects[k]]
    names = {}
    for k in changed_rects:
        go = re.search(r'  m_GameObject: \{fileID: (-?\d+)\}', old_rects[k]).group(1)
        names[k] = re.search(r'^  m_Name: (.+)$', a[go][1], re.M).group(1)
    assert sorted(names.values()) == (['Artwork', 'Logo'] if page == 'SplashTitle' else ['ApprovedSplashLogo']), names
    # In particular all Button, label, character, safe-area and footer geometry remains byte-identical.
    assert re.search(r'^  Revision: '+str(revision)+r'$', final, re.M)
    prefabs[page] = {'revision': revision, 'beforeIds': len(a), 'afterIds': len(b),
        'retainedIds': len(a.keys() & b.keys()), 'removedIds': list(a.keys()-b.keys()),
        'addedIds': list(b.keys()-a.keys()), 'rectTransformCount': len(old_rects),
        'changedArtRects': names, 'allOtherRectBlocksUnchanged': True}

diff = []
for row in changed:
    rel = row['path']
    assert sha(backup / rel) == old[rel]
    diff.extend(difflib.unified_diff((backup / rel).read_text().splitlines(keepends=True),
        (root / rel).read_text().splitlines(keepends=True), fromfile='before/' + rel, tofile='after/' + rel))
    if rel.endswith('.cs'):
        assert all(line.rstrip() == line for line in (root / rel).read_text().splitlines())
        assert (root / rel).read_bytes().endswith(b'\n')
for rel in added:
    if not rel.endswith('.png'):
        diff.extend(difflib.unified_diff([], (root/rel).read_text().splitlines(keepends=True), fromfile='/dev/null', tofile='after/' + rel))
audit = {'auditUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'passed': True,
    'baselineCount': len(old), 'unchanged': len(old)-len(changed), 'modified': changed,
    'addedProductionFiles': added, 'newUniqueGuids': new_guids,
    'existingMetadataCount': len(metas), 'allExistingMetadataUnchanged': True,
    'existingPngCount': len(pngs), 'allExistingPngsUnchanged': True,
    'protectedGroups': protected, 'prefabs': prefabs,
    'scope': 'Recorded pre-task production hashes and exact owned file backups; no generated caches or secret files included.'}
def write(name, value): (delivery / name).write_text(json.dumps(value, indent=2)+'\n')
write('FinalSourceAudit.json', audit)
write('FinalProductionHashes.json', [{'path': p, 'sha256': h} for p,h in current.items()] + [{'path': p, 'sha256': sha(root/p)} for p in added])
write('SourceChangeManifest.json', changed + [{'path': p, 'added': True, 'sha256': sha(root/p)} for p in added])
(delivery / 'AppliedSourceChanges.diff').write_text(''.join(diff))
print(json.dumps({'passed': True, 'modified': len(changed), 'added': len(added), 'prefabs': prefabs}))
