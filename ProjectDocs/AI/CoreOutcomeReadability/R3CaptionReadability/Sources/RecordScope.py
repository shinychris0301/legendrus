"""Verify task-only changes, preserved art/identities and precise caption geometry scope."""
from pathlib import Path
import hashlib
import json
import difflib
import re
import subprocess

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()
before = json.loads((QA / 'BeforeHashes.json').read_text())
after = {str(path.relative_to(ROOT)): sha(path) for folder in ('Assets', 'Packages', 'ProjectSettings')
         for path in (ROOT / folder).rglob('*') if path.is_file()
         and path.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key')
         and not path.name.startswith('.env')}
scope = {'baselineFiles': len(before), 'finalFiles': len(after),
    'unchanged': sum(after.get(path) == value for path, value in before.items()),
    'changed': sorted(path for path in before if path in after and after[path] != before[path]),
    'added': sorted(set(after) - set(before)), 'deleted': sorted(set(before) - set(after))}
expected = {'Assets/LegendsOfRus/Scripts/Runtime/UI/' + name + '.cs' for name in (
    'ProductionGameplaySourceControls', 'ProductionGameplayCaptionFit', 'ProductionGameplayView',
    'ProductionVictoryReferenceStyle', 'ProductionVictoryView')}
expected.update('Assets/LegendsOfRus/Resources/ProductionUI/Pages/' + name + '.prefab' for name in ('Gameplay', 'Victory'))
expected.update('Assets/LegendsOfRus/Tests/PlayMode/' + name + 'AlignmentPlayModeTests.cs' for name in ('Gameplay', 'Victory'))
expected.add('Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/VictoryAlignmentEditModeTests.cs')
added = {'Assets/LegendsOfRus/Tests/PlayMode/CoreCaptionAssertions.cs' + ending for ending in ('', '.meta')}
added.update('Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/CoreCaptionReadabilityEditModeTests.cs' + ending for ending in ('', '.meta'))
assert set(scope['changed']) == expected and set(scope['added']) == added and not scope['deleted'], scope
assert scope['unchanged'] == 7788 and scope['finalFiles'] == 7802
patch = []
for relative in scope['changed']:
    saved = BASE / 'Sources/Before' / relative
    assert sha(saved) == before[relative]
    patch.extend(difflib.unified_diff(saved.read_text().splitlines(True), (ROOT / relative).read_text().splitlines(True),
        fromfile='a/' + relative, tofile='b/' + relative))
for relative in scope['added']:
    patch.extend(difflib.unified_diff([], (ROOT / relative).read_text().splitlines(True), fromfile='/dev/null', tofile='b/' + relative))
(QA / 'TaskChanges.patch').write_text(''.join(patch))

def sections(text):
    return {re.match(r'--- !u!\d+ &(-?\d+)', part).group(1): part for part in
            re.split(r'(?m)(?=^--- !u!)', text) if part.startswith('--- !u!')}
def fields(section, key):
    return re.findall(r'(?m)^  ' + re.escape(key) + r':[^\n]*', section)
proof = {}
geometry_keys = ('m_LocalPosition', 'm_LocalRotation', 'm_LocalScale', 'm_AnchorMin', 'm_AnchorMax',
                 'm_AnchoredPosition', 'm_SizeDelta', 'm_Pivot')
for page, revision, objects in [('Gameplay', 23, 16), ('Victory', 12, 0)]:
    relative = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/' + page + '.prefab'
    old = sections((BASE / 'Sources/Before' / relative).read_text())
    new = sections((ROOT / relative).read_text())
    assert set(old) <= set(new) and len(new) - len(old) == objects
    names = {key: re.search(r'(?m)^  m_Name: (.*)', value).group(1) for key, value in old.items()
             if value.startswith('--- !u!1 ')}
    changed_geometry = []
    changed_sprites = []
    for key in old:
        assert fields(old[key], 'm_Font') == fields(new[key], 'm_Font'), (page, key, 'font')
        assert fields(old[key], 'm_TargetGraphic') == fields(new[key], 'm_TargetGraphic'), (page, key, 'target')
        changed = [field for field in geometry_keys if fields(old[key], field) != fields(new[key], field)]
        if changed:
            go = re.search(r'(?m)^  m_GameObject: \{fileID: (-?\d+)\}', old[key]).group(1)
            name = names[go]
            assert name in ({'BoosterLabel'} if page == 'Gameplay' else
                {'Heading', 'ScoreHeading', 'BoosterHeading', 'BoosterEmpty', 'Amount'}), (page, name, changed)
            assert set(changed) <= {'m_AnchoredPosition', 'm_SizeDelta'}, (page, name, changed)
            changed_geometry.append({'object': key, 'name': name, 'fields': changed})
        if fields(old[key], 'm_Sprite') != fields(new[key], 'm_Sprite'):
            go = re.search(r'(?m)^  m_GameObject: \{fileID: (-?\d+)\}', old[key]).group(1)
            assert page == 'Victory' and names[go] == 'RewardsPanel'
            changed_sprites.append({'object': key, 'before': fields(old[key], 'm_Sprite'), 'after': fields(new[key], 'm_Sprite')})
    assert sum(bool(fields(section, 'Revision')) for section in new.values()) == 1
    assert any(fields(section, 'Revision') == ['  Revision: ' + str(revision)] for section in new.values())
    proof[page] = {'beforeObjects': len(old), 'afterObjects': len(new), 'existingObjectIdsPreserved': True,
        'addedObjects': objects, 'existingFontAndNativeTargetReferencesPreserved': True,
        'changedExistingTextGeometry': changed_geometry, 'changedSpriteReferences': changed_sprites, 'currentRevision': revision}
other_prefabs = [path for path in before if path.endswith('.prefab') and path not in expected]
assert len(other_prefabs) == 70 and all(before[path] == after[path] for path in other_prefabs)
assert all(before[path] == after[path] for path in before if path.endswith('.meta'))
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/CoreCaptionReadabilityPrefabAudit.cs'
assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
scope.update(other70PrefabsByteIdentical=True, allSourceArtAndExistingMetadataBytesUnchanged=True,
    packagesProjectSettingsAndApprovedSplashUnchanged=True, temporaryInstallHelperRemoved=True)
(QA / 'Scope.json').write_text(json.dumps(scope, indent=2) + '\n')
(QA / 'PrefabPreservation.json').write_text(json.dumps(proof, indent=2) + '\n')
subprocess.run(['git', 'apply', '--reverse', '--check', str(QA / 'TaskChanges.patch')], cwd=ROOT, check=True)
print(json.dumps(scope, indent=2))
