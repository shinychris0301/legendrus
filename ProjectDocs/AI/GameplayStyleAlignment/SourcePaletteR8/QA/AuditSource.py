"""Audit R8 against the verified local R7 baseline, not an earlier git revision."""
from pathlib import Path
import datetime, difflib, hashlib, json, re

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/GameplayStyleAlignment/SourcePaletteR8'
backup = Path('/tmp/lor-gameplay-palette-r8/before')
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
before = json.loads((delivery / 'BeforeProductionHashes.json').read_text())
old = {record['path']: record['sha256'] for record in before}
expected = {
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs',
    'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs',
    'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs',
    'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab',
    'Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab',
}
current = {path: sha(root / path) for path in old}
changed = {path for path in old if old[path] != current[path]}
assert changed == expected, {'unexpected': sorted(changed - expected), 'missing': sorted(expected - changed)}

new = []
for base in [
    'Assets/LegendsOfRus/Scripts',
    'Assets/LegendsOfRus/Tests',
    'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned',
]:
    for path in (root / base).rglob('*'):
        if path.is_file() and path.suffix in {'.cs', '.meta', '.png', '.asset', '.prefab', '.asmdef', '.asmref', '.shader', '.mat'}:
            relative = str(path.relative_to(root))
            if relative not in old:
                new.append(relative)
expected_new = {
    'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/gem_' + name + '_r8' + extension
    for name in ['ruby', 'sapphire', 'emerald', 'gold', 'amethyst', 'orange']
    for extension in ['.png', '.png.meta']
}
assert set(new) == expected_new, (set(new) - expected_new, expected_new - set(new))

all_guids = {}
for path in (root / 'Assets').rglob('*.meta'):
    match = re.search(r'^guid: (.+)$', path.read_text(), re.M)
    if match:
        all_guids.setdefault(match.group(1), []).append(str(path.relative_to(root)))
for path in new:
    if path.endswith('.meta'):
        guid = re.search(r'^guid: (.+)$', (root / path).read_text(), re.M).group(1)
        assert len(all_guids[guid]) == 1, all_guids[guid]
metadata = [path for path in old if path.endswith('.meta')]
assert all(old[path] == current[path] for path in metadata)

ids = lambda text: set(re.findall(r'^--- !u!\d+ &(-?\d+)', text, re.M))
prefab_ids = {}
for page in ['Gameplay', 'PreLevel']:
    path = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/' + page + '.prefab'
    old_ids = ids((backup / path).read_text())
    new_ids = ids((root / path).read_text())
    assert old_ids == new_ids, {'page': page, 'removed': sorted(old_ids - new_ids), 'added': sorted(new_ids - old_ids)}
    prefab_ids[page] = {
        'originalIds': sorted(old_ids),
        'retainedOriginalIds': sorted(old_ids & new_ids),
        'removedOriginalIds': sorted(old_ids - new_ids),
        'addedIds': sorted(new_ids - old_ids),
        'finalObjectCount': len(new_ids),
    }

protected = {}
for page in ['Home', 'SplashTitle', 'Loading', 'WorldMap', 'ChapterMap']:
    paths = [path for path in old if ('/' + page + '.prefab') in path or ('/' + page + 'Aligned') in path or ('/Production' + page) in path]
    protected[page] = {'count': len(paths), 'unchanged': all(current[path] == old[path] for path in paths)}
for group, terms in [
    ('FrontEnd', ('/FrontEndHomeAligned', '/SplashTitleConfig/', '/SplashTitleReconstructed/')),
    ('Packages', ('Packages/',)),
    ('ProjectSettings', ('ProjectSettings/',)),
    ('AndroidGuards', ('Android', 'Signing', 'Keystore')),
    ('CoreRulesAndVfx', ('/GameplayCore/', 'PooledUiVfxLayer', 'GameplayMotion', 'CoreLoopVfxPresenter')),
    ('BoardGeometryAndData', ('/Match3Board.cs', '/ProductionGameplayHudStyle.cs', '/ProductionGameplayLayout.cs', '/ProductionGameplayCell.cs', '/GameplayAlignmentArtImporter.cs', '/chapter01_levels.json')),
    ('PriorGameplayArtwork', ('/GameplayAligned/',)),
]:
    paths = [path for path in old if any(term in path for term in terms)]
    protected[group] = {'count': len(paths), 'unchanged': all(current[path] == old[path] for path in paths)}
assert all(group['unchanged'] for group in protected.values()), protected
board_path = 'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs'
assert (root / board_path).read_bytes() == (backup / board_path).read_bytes(), 'Match3Board must remain byte-identical to R7.'

diff = []
manifest = []
for path in sorted(changed):
    assert sha(backup / path) == old[path]
    diff.extend(difflib.unified_diff(
        (backup / path).read_text().splitlines(True),
        (root / path).read_text().splitlines(True),
        fromfile='before/' + path, tofile='after/' + path,
    ))
    manifest.append({'path': path, 'before': old[path], 'sha256': current[path]})
for path in sorted(new):
    digest = sha(root / path)
    current[path] = digest
    manifest.append({'path': path, 'added': True, 'sha256': digest})
    if not path.endswith('.png'):
        diff.extend(difflib.unified_diff([], (root / path).read_text().splitlines(True), fromfile='/dev/null', tofile='after/' + path))
for path in changed | set(new):
    if path.endswith('.cs'):
        assert all(line == line.rstrip() for line in (root / path).read_text().splitlines()), path
        assert (root / path).read_bytes().endswith(b'\n'), path

gameplay_ids = prefab_ids['Gameplay']
audit = {
    'passed': True,
    'auditUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'baselineCount': len(before),
    'unchanged': len(before) - len(changed),
    'modified': sorted(changed),
    'addedProductionFiles': sorted(new),
    'metadataCount': len(metadata),
    'allExistingMetadataBytesAndGuidsPreserved': True,
    'newGuidsUnique': True,
    'protectedGroups': protected,
    'originalGameplayPrefabIds': gameplay_ids['originalIds'],
    'retainedOriginalIds': gameplay_ids['retainedOriginalIds'],
    'removedOriginalIds': gameplay_ids['removedOriginalIds'],
    'finalPrefabObjectCount': gameplay_ids['finalObjectCount'],
    'prefabIds': prefab_ids,
    'match3BoardBytesPreserved': True,
    'runtimeAssetHashesRecorded': True,
    'scope': 'Recorded pre-task production paths and six new Gameplay gem PNG/meta pairs; no secrets or generated caches.',
}
for name, data in [
    ('FinalSourceAudit.json', audit),
    ('FinalProductionHashes.json', [{'path': path, 'sha256': digest} for path, digest in current.items()]),
    ('SourceChangeManifest.json', manifest),
]:
    (delivery / name).write_text(json.dumps(data, indent=2) + '\n')
(delivery / 'AppliedSourceChanges.diff').write_text(''.join(diff))
print(json.dumps({
    'passed': True, 'unchanged': audit['unchanged'], 'modified': len(changed),
    'added': len(new), 'metadata': len(metadata),
    'preservedPrefabIds': {page: data['finalObjectCount'] for page, data in prefab_ids.items()},
    'protected': protected,
}))
