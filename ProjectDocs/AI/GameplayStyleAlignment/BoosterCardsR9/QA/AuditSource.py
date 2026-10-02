"""Audit R9 against the verified local R8 baseline, not an earlier git revision.

Run from the project root after the canonical Unity rebuild and verification.
This script reads only the recorded production inventory and writes QA evidence.
"""
from pathlib import Path
import datetime, difflib, hashlib, json, re

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/GameplayStyleAlignment/BoosterCardsR9'
backup = Path('/tmp/lor-gameplay-booster-r9/before')
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
before = json.loads((delivery / 'BeforeProductionHashes.json').read_text())
old = {record['path']: record['sha256'] for record in before}
assert len(before) == len(old) == 1703, 'R9 requires the recorded 1,703-file R8 baseline.'
expected = {
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayRuntime.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayHudStyle.cs',
    'Assets/LegendsOfRus/Scripts/Editor/GameplayAlignmentArtImporter.cs',
    'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs',
    'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs',
    'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab',
    'Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs',
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
    'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/' + name + '_r9' + extension
    for name in ['booster_mace', 'booster_firebird', 'booster_livinglight', 'booster_seapearl', 'card_wood']
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
for page in ['Home', 'SplashTitle', 'Loading', 'WorldMap', 'ChapterMap', 'PreLevel']:
    paths = [path for path in old if ('/' + page + '.prefab') in path or ('/' + page + 'Aligned') in path or ('/Production' + page) in path]
    protected[page] = {'count': len(paths), 'unchanged': all(current[path] == old[path] for path in paths)}
for group, terms in [
    ('FrontEnd', ('/FrontEndHomeAligned', '/SplashTitleConfig/', '/SplashTitleReconstructed/')),
    ('Packages', ('Packages/',)),
    ('ProjectSettings', ('ProjectSettings/',)),
    ('AndroidGuards', ('Android', 'Signing', 'Keystore')),
    ('CoreRulesAndVfx', ('/GameplayCore/', 'PooledUiVfxLayer', 'GameplayMotion', 'CoreLoopVfxPresenter')),
    ('BoardGeometryAndData', ('/Match3Board.cs', '/ProductionGameplayLayout.cs', '/ProductionGameplayCell.cs', '/chapter01_levels.json')),
    ('PriorGameplayArtwork', ('/GameplayAligned/',)),
    ('PreLevelCodeAndTests', ('/ProductionPreLevel', '/PreLevelAlignment',)),
    ('OriginalBoosterInputAndTransactions', ('/ProductionGameplayBoosterCard.cs', '/BoosterService.cs',)),
]:
    paths = [path for path in old if any(term in path for term in terms)]
    protected[group] = {'count': len(paths), 'unchanged': all(current[path] == old[path] for path in paths)}
assert all(group['unchanged'] for group in protected.values()), protected
board_path = 'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs'
assert (root / board_path).read_bytes() == (backup / board_path).read_bytes(), 'Match3Board must remain byte-identical to R8.'


# Assert that shared or complex owners did not gain unrelated behavior changes.
# The allowlist protects the R8 gem mapping, original source atlas regions,
# current header/board geometry, original booster transactions and text wording.
def text_pair(relative):
    return (backup / relative).read_text(), (root / relative).read_text()

restricted = {}
art_path = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs'
old_art, new_art = text_pair(art_path)
file_declaration = r'^        public static readonly string\[\] Files=.*$'
booster_method = r'^        public static Sprite Booster\(int index\).*\n'
assert re.sub(file_declaration, '', old_art, flags=re.M) == re.sub(
    booster_method, '', re.sub(file_declaration, '', new_art, flags=re.M), flags=re.M
), 'Only the art inventory and Gameplay-only Booster mapping may change.'
old_files = re.findall(r'"([^"\n]+)"', re.search(file_declaration, old_art, re.M).group())
new_files = re.findall(r'"([^"\n]+)"', re.search(file_declaration, new_art, re.M).group())
assert new_files[:len(old_files)] == old_files
assert set(new_files[len(old_files):]) == {
    'booster_mace_r9', 'booster_firebird_r9', 'booster_livinglight_r9', 'booster_seapearl_r9', 'card_wood_r9'
}
restricted['Art'] = 'Only appended resources and the Gameplay-only Booster mapping changed; prior Load/Gem/Regions/Obstacle code is byte-identical.'

runtime_path = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayRuntime.cs'
old_runtime, new_runtime = text_pair(runtime_path)
for obsolete in [
    '            string[] names={"mace","firebird","livinglight","seapearl"};\n',
    '            string[] boosterArt={"mace","firebird","livinglight","seapearl"};\n',
]:
    assert obsolete in old_runtime
    old_runtime = old_runtime.replace(obsolete, '')
old_runtime = old_runtime.replace('ProductionGameplayArt.Load(names[i])', 'ProductionGameplayArt.Booster(i)')
old_runtime = old_runtime.replace('ProductionGameplayArt.Load(boosterArt[i])', 'ProductionGameplayArt.Booster(i)')
assert old_runtime == new_runtime, 'Runtime changes must only select Gameplay-specific booster sprites.'
restricted['Runtime'] = 'Only startup/reapplication booster sprite assignments changed.'

hud_path = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayHudStyle.cs'
old_hud, new_hud = text_pair(hud_path)
start = '            LayoutGoals(v,2);\n'
end = '            v.PauseButton.GetComponent<Image>().sprite='
def without_footer(text):
    assert text.count(start) == 1 and text.count(end) == 1
    return text.split(start)[0] + start + text.split(end)[1]
assert without_footer(old_hud) == without_footer(new_hud), 'HUD edits must stay within the booster-card loop.'
restricted['HudStyle'] = 'Header, board, pause, status and goal layout code is byte-identical outside the booster-card loop.'

view_path = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs'
old_view, new_view = text_pair(view_path)
old_view = old_view.replace('public const int CurrentRevision=8;', 'public const int CurrentRevision=9;')
# Treat explicit line breaks as spaces only in the localized booster-name arrays.
# This allows wrapping changes but fails on altered wording or gameplay logic.
def normalized_label_breaks(text):
    return re.sub(r'^            string\[\] (?:ru|english)=.*$',
                  lambda match: match.group().replace('\\n', ' '), text, flags=re.M)
assert normalized_label_breaks(old_view) == normalized_label_breaks(new_view), 'View changes must only update revision and label line breaks.'
restricted['View'] = 'Only revision and explicit label line breaks changed; booster names and transactions are preserved.'

importer_path = 'Assets/LegendsOfRus/Scripts/Editor/GameplayAlignmentArtImporter.cs'
old_importer, new_importer = text_pair(importer_path)
new_importer = re.sub(r'^            if\(name=="card_wood_r9"\)ti\.spriteBorder=.*;\n', '', new_importer, flags=re.M)
assert old_importer == new_importer, 'Only the new card sprite border may change in the importer.'
restricted['Importer'] = 'Only the new card border import is added; every prior import setting is preserved.'

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
    'scope': 'Recorded pre-task production paths and five new Gameplay booster/card PNG/meta pairs; no secrets or generated caches.',
    'restrictedSourceChanges': restricted,
    'preLevelBytesPreserved': True,
    'priorGameplayArtworkBytesPreserved': True,
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
