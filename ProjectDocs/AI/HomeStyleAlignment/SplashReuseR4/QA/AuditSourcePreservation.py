"""Read-only production/source audit; writes evidence JSON only."""
from pathlib import Path
import hashlib, json, re, struct

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/SplashReuseR4'
before = json.loads((delivery / 'BeforeHashes.json').read_text())
sha = lambda p: hashlib.sha256(Path(p).read_bytes()).hexdigest()
modified, missing = [], []
for row in before:
    p = root / row['path']
    if not p.exists():
        missing.append(row['path'])
    elif sha(p) != row['sha256']:
        modified.append({'path': row['path'], 'before': row['sha256'], 'sha256': sha(p)})
new_paths = ['Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_firebird_generated.png' + suffix for suffix in ['', '.meta']]
new = [{'path': p, 'sha256': sha(root/p)} for p in new_paths if (root/p).exists()]
protected_modified = [r for r in modified if not (
    '/HomeAligned/' in r['path'] or r['path'].endswith('/Pages/Home.prefab') or
    Path(r['path']).name in ['ProductionHomeRuntime.cs','ProductionHomeView.cs','ProductionHomeLayout.cs',
    'LockedDesignImporter.cs','Step20ProductionUiFix01SerializedPrefabBuilder.cs',
    'HomeAlignmentPlayModeTests.cs','HomeAlignmentDeliveryEditModeTests.cs'])]
old_prefab = Path('/tmp/lor-home-r4/before/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab')
prefab = root/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab'
ids = lambda p: set(re.findall(r'^--- !u!\d+ &(-?\d+)', p.read_text(), re.M))
old_ids, final_ids = ids(old_prefab), ids(prefab)
root_prompts = json.loads((delivery/'RootAssetPrompts.json').read_text())
nav_prompts = json.loads((delivery/'NavFooterPrompts.json').read_text())['assets']
selected = {}
for row in root_prompts:
    if not row.get('status','').startswith('rejected'):
        selected[row['asset']] = row['generatedSource']
for row in nav_prompts:
    if row.get('selected'):
        selected[row['filename']] = row['generatedPath']
asset_rows = []
for p in sorted((delivery/'Sources').glob('*.png')):
    runtime = root/'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned'/p.name
    b = p.read_bytes()
    entry = {'asset':p.name,'width':struct.unpack('>I',b[16:20])[0],'height':struct.unpack('>I',b[20:24])[0],
        'pngColorType':b[25],'sha256':sha(p),'runtimeIdentical':sha(runtime)==sha(p),
        'newlyGenerated':p.name in selected}
    if p.name in selected:
        entry['originalToolOutputIdentical'] = sha(selected[p.name]) == sha(p)
    asset_rows.append(entry)
audit = {'baselineCount':len(before),'unchanged':len(before)-len(modified)-len(missing),
    'modified':modified,'missing':missing,'protectedModified':protected_modified,'added':new,
    'existingMetaChanges':[r['path'] for r in modified if r['path'].endswith('.meta')],
    'prefabIds':{'before':len(old_ids),'after':len(final_ids),'retained':len(old_ids & final_ids),
        'removed':sorted(old_ids-final_ids),'added':len(final_ids-old_ids)},
    'assets':asset_rows,'newlyGeneratedCount':len(selected),
    'scope':'Before inventory includes existing safe Assets/Packages/ProjectSettings. Existing local user changes form the baseline.'}
(delivery/'FinalSourceAudit.json').write_text(json.dumps(audit,indent=2)+'\n')
(delivery/'SourceChangeManifest.json').write_text(json.dumps({'productionFiles':modified+new,'count':len(modified)+len(new)},indent=2)+'\n')
print(json.dumps({k:audit[k] for k in ['baselineCount','unchanged','missing','protectedModified','existingMetaChanges','prefabIds','newlyGeneratedCount']},indent=2))
print('Modified:',len(modified),'Added:',len(new),'Asset byte checks:',all(r['runtimeIdentical'] and r.get('originalToolOutputIdentical',True) for r in asset_rows))
assert not missing and not protected_modified
assert all(r['runtimeIdentical'] and r.get('originalToolOutputIdentical',True) for r in asset_rows)
assert not (old_ids-final_ids)
