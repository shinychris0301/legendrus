"""Independent read-only R6 production audit; writes its evidence JSON only."""
from pathlib import Path
import argparse
import collections
import datetime
import hashlib
import json
import re

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6'
before_dir = Path('/tmp/lor-home-r6/before')
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--import-log', type=Path, help='Explicit completed final Home import log; otherwise choose the highest numbered import3-or-later log.')
args = parser.parse_args()
if args.import_log:
    log_path = args.import_log
else:
    logs = []
    for p in Path('/tmp/lor-home-r6').glob('import*/unity.log'):
        match = re.fullmatch(r'import(\d+)', p.parent.name)
        if match and int(match.group(1)) >= 3:
            logs.append((int(match.group(1)), p))
    assert logs, 'Wait for import3 or a later completed final owner import.'
    _, log_path = max(logs, key=lambda pair: pair[0])
readback_line = 'Home alignment serialized prefab saved and read back at revision 9.'
assert readback_line in log_path.read_text(), 'Wait for the owning Home importer to save/read back revision 9.'
before = json.loads((delivery / 'BeforeHashes.json').read_text())
recorded = {r['path']: r['sha256'] for r in before}
sha = lambda p: hashlib.sha256(Path(p).read_bytes()).hexdigest()
current_hashes = {}
modified, missing = [], []
for row in before:
    p = root / row['path']
    if not p.is_file():
        missing.append(row['path'])
        continue
    current_hashes[row['path']] = digest = sha(p)
    if digest != row['sha256']:
        modified.append({'path': row['path'], 'before': row['sha256'], 'sha256': digest})

allowed_sources = {
    'ProductionHomeRuntime.cs', 'ProductionHomeView.cs', 'ProductionHomeLayout.cs',
    'ProductionHomeShade.cs', 'ProductionHomeArtFade.cs', 'ProductionHomeButtonFeedback.cs',
    'ProductionHomePanel.cs', 'HomeAlignmentPlayModeTests.cs', 'HomeAlignmentDeliveryEditModeTests.cs',
    'LockedDesignImporter.cs', 'Step20ProductionUiFix01SerializedPrefabBuilder.cs',
    'Step20ProductionUiFix01SelfTest.cs', 'ProductionHomeGoldText.cs'
}
def home_owned(p):
    return '/HomeAligned/' in p or p.endswith('/Pages/Home.prefab') or Path(p).name in allowed_sources
outside_home = [r for r in modified if not home_owned(r['path'])]

previous = json.loads((root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceRevisionR5/QA/IndependentSourceAudit.json').read_text())
protected_groups = {}
for name, group in previous['protectedGroups'].items():
    if name == 'Background':
        continue  # The latest user explicitly authorized replacing Home's background.
    paths = group['paths']
    changes = [p for p in paths if p not in recorded or current_hashes.get(p) != recorded[p]]
    protected_groups[name] = {'checkedCount': len(paths), 'unchanged': not changes, 'changedOrMissing': changes, 'paths': paths}

backup_rows = []
for p in sorted(before_dir.rglob('*')):
    if p.is_file():
        rel = p.relative_to(before_dir).as_posix()
        backup_rows.append({'path': rel, 'matchesRecordedBeforeHash': rel in recorded and sha(p) == recorded[rel]})

meta_rows = []
guids = collections.defaultdict(list)
for row in before:
    if not row['path'].endswith('.meta'):
        continue
    p = root / row['path']
    m = re.search(r'^guid:\s*(\w+)', p.read_text(), re.M) if p.is_file() else None
    guid = m.group(1) if m else None
    if guid:
        guids[guid].append(row['path'])
    unchanged = current_hashes.get(row['path']) == row['sha256']
    prior = before_dir / row['path']
    prior_m = re.search(r'^guid:\s*(\w+)', prior.read_text(), re.M) if prior.is_file() else None
    guid_preserved = unchanged or bool(prior_m and prior_m.group(1) == guid)
    meta_rows.append({'path': row['path'], 'byteUnchanged': unchanged, 'guid': guid, 'guidPreserved': guid_preserved})

home_path = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab'
ids = lambda p: set(re.findall(r'^--- !u!\d+ &(-?\d+)', p.read_text(), re.M))
old_ids = ids(before_dir / home_path)
new_ids = ids(root / home_path)
prefab_revision = int(re.search(r'^  Revision:\s*(\d+)', (root / home_path).read_text(), re.M).group(1))
view_revision = int(re.search(r'CurrentRevision\s*=\s*(\d+)', (root / 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeView.cs').read_text()).group(1))

provenance = json.loads((delivery / 'SelectedAssetProvenance.json').read_text())
assets = []
for row in provenance['selected']:
    paths = {'original': Path(row['original']), 'source': root / row['source'], 'runtime': root / row['runtime']}
    hashes = {name: sha(p) for name, p in paths.items()}
    assets.append({'key': row['key'], 'recordedSha256': row['sha256'], 'hashes': hashes,
        'allHashesMatchRecord': all(digest == row['sha256'] for digest in hashes.values()),
        'originalSourceRuntimeBytesIdentical': paths['original'].read_bytes() == paths['source'].read_bytes() == paths['runtime'].read_bytes()})
retained = []
for name in ['progress_frame', 'progress_fill']:
    p = f'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/{name}.png'
    source = delivery / 'Sources' / f'{name}.png'
    retained.append({'key': name, 'matchesBeforeRuntime': current_hashes.get(p) == recorded[p],
        'sourceRuntimeBytesIdentical': source.read_bytes() == (root / p).read_bytes()})

added = []
scan_roots = ['Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned', 'Assets/LegendsOfRus/Scripts', 'Assets/LegendsOfRus/Tests']
safe_exts = {'.cs', '.meta', '.png', '.asmdef', '.asmref', '.json', '.asset', '.prefab', '.unity', '.txt', '.md', '.shader', '.hlsl', '.cginc'}
for scan in scan_roots:
    for p in sorted((root / scan).rglob('*')):
        rel = p.relative_to(root).as_posix()
        if p.is_file() and rel not in recorded and p.suffix in safe_exts:
            added.append({'path': rel, 'sha256': sha(p)})

expected_additions = {
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeGoldText.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeGoldText.cs.meta'
}
added_paths = {r['path'] for r in added}
unexpected_additions = sorted(added_paths - expected_additions)
missing_additions = sorted(expected_additions - added_paths)
new_metadata = []
for row in added:
    if not row['path'].endswith('.meta'):
        continue
    p = root / row['path']
    match = re.search(r'^guid:\s*([0-9a-fA-F]{32})\s*$', p.read_text(), re.M)
    guid = match.group(1) if match else None
    unique_against_existing = bool(guid and guid not in guids)
    if guid:
        guids[guid].append(row['path'])
    new_metadata.append({'path': row['path'], 'sha256': row['sha256'], 'guid': guid,
        'uniqueAgainstExistingBaseline': unique_against_existing,
        'referencedByHomePrefab': bool(guid and f'guid: {guid}' in (root / home_path).read_text())})

audit = {
    'auditUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'scope': 'Independent read-only comparison of the recorded 1487-file safe production baseline and owned text backups. Existing local user changes form the baseline. No Unity launch or production mutation by this auditor. No keystore/credential files inspected.',
    'importReadbackEvidence': {'log': str(log_path), 'requiredLineObserved': readback_line,
        'selection': 'Explicit CLI log' if args.import_log else 'Highest numbered available import3-or-later log; current highest log must contain completed readback.'},
    'baselineCount': len(before), 'unchanged': len(before)-len(modified)-len(missing),
    'modified': modified, 'missing': missing, 'modifiedOutsideHomeOwnedScope': outside_home,
    'addedAuditedFiles': added, 'addedFileScanScope': scan_roots,
    'additionPolicy': {'expectedExactPaths': sorted(expected_additions), 'unexpected': unexpected_additions,
        'missingExpected': missing_additions, 'exactlyExpectedTwoFiles': added_paths == expected_additions},
    'protectedGroups': protected_groups,
    'metadata': {'baselineCount': len(meta_rows), 'byteUnchanged': sum(r['byteUnchanged'] for r in meta_rows),
        'allExistingGuidsPreserved': all(r['guidPreserved'] for r in meta_rows),
        'changed': [r for r in meta_rows if not r['byteUnchanged']],
        'missingGuid': [r['path'] for r in meta_rows if not r['guid']],
        'new': new_metadata, 'allAuditedGuidCount': sum(len(paths) for paths in guids.values()),
        'duplicates': {g: ps for g, ps in guids.items() if len(ps)>1}},
    'ownedBackupIntegrity': {'count': len(backup_rows), 'allMatchRecordedBefore': all(r['matchesRecordedBeforeHash'] for r in backup_rows), 'files': backup_rows},
    'prefabIds': {'before': len(old_ids), 'after': len(new_ids), 'retained': len(old_ids & new_ids),
        'removed': sorted(old_ids-new_ids), 'added': sorted(new_ids-old_ids)},
    'currentRevision': {'prefab': prefab_revision, 'view': view_revision, 'bothNine': prefab_revision == view_revision == 9},
    'selectedNewAssetCount': len(assets), 'selectedAssetChecks': assets, 'retainedAssetChecks': retained,
    'notPerformed': ['Unity compilation or runtime execution by this auditor', 'Play/Edit Mode tests by this auditor', 'APK/AAB build', 'Physical-phone check', 'Visual exact-match verification from source hashes']
}
audit['passed'] = (not missing and not outside_home and all(g['unchanged'] for g in protected_groups.values())
    and audit['currentRevision']['bothNine']
    and audit['additionPolicy']['exactlyExpectedTwoFiles']
    and len(new_metadata) == 1 and all(r['uniqueAgainstExistingBaseline'] and r['referencedByHomePrefab'] for r in new_metadata)
    and audit['metadata']['allExistingGuidsPreserved'] and not audit['metadata']['duplicates']
    and audit['ownedBackupIntegrity']['allMatchRecordedBefore'] and not (old_ids-new_ids)
    and len(assets)==23 and all(r['allHashesMatchRecord'] and r['originalSourceRuntimeBytesIdentical'] for r in assets)
    and all(r['matchesBeforeRuntime'] and r['sourceRuntimeBytesIdentical'] for r in retained))
(delivery / 'QA/IndependentSourceAudit.json').write_text(json.dumps(audit, indent=2)+'\n')
print(json.dumps({k: audit[k] for k in ['baselineCount','unchanged','missing','modifiedOutsideHomeOwnedScope','metadata','prefabIds','selectedNewAssetCount','passed']}, indent=2))
print('Modified', len(modified), 'Added in audited scope', len(added))
