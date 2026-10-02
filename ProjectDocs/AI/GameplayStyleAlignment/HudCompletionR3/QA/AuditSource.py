"""Audit against the accepted local baseline, not an earlier git revision."""
from pathlib import Path
import datetime,difflib,hashlib,json,re
root=Path.cwd(); delivery=root/'ProjectDocs/AI/GameplayStyleAlignment/HudCompletionR3'; backup=Path('/tmp/lor-gameplay-hud-r3/before')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
before=json.loads((delivery/'BeforeProductionHashes.json').read_text()); old={r['path']:r['sha256'] for r in before}
expected={
 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayRuntime.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs',
 'Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs',
 'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs'}
current={p:sha(root/p) for p in old}; changed={p for p in old if old[p]!=current[p]}; assert changed==expected,changed
new=[]
for base in ['Assets/LegendsOfRus/Scripts','Assets/LegendsOfRus/Tests','Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned']:
 for p in (root/base).rglob('*'):
  if p.is_file() and p.suffix in {'.cs','.meta','.png','.asset','.prefab','.asmdef','.asmref','.shader','.mat'} and str(p.relative_to(root)) not in old:new.append(str(p.relative_to(root)))
expected_new=set()
for name in ['ProductionGameplayHudStyle','ProductionGameplayStatus','ProductionGameplayGoalSymbol']:
 for ext in ['.cs','.cs.meta']:expected_new.add('Assets/LegendsOfRus/Scripts/Runtime/UI/'+name+ext)
for name in ['hud_header_r3','score_panel_r3','pause_gear_r3']:
 for ext in ['.png','.png.meta']:expected_new.add('Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/'+name+ext)
assert set(new)==expected_new,(set(new)-expected_new,expected_new-set(new))
all_guids={}
for p in (root/'Assets').rglob('*.meta'):
 m=re.search(r'^guid: (.+)$',p.read_text(),re.M)
 if m:all_guids.setdefault(m.group(1),[]).append(str(p.relative_to(root)))
for p in new:
 if p.endswith('.meta'):
  guid=re.search(r'^guid: (.+)$',(root/p).read_text(),re.M).group(1);assert len(all_guids[guid])==1
meta=[p for p in old if p.endswith('.meta')];assert all(old[p]==current[p] for p in meta)
prefab='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab'
ids=lambda t:set(re.findall(r'^--- !u!\d+ &(-?\d+)',t,re.M))
old_ids=ids((backup/prefab).read_text());new_ids=ids((root/prefab).read_text());assert old_ids<new_ids
protected={}
for page in ['Home','SplashTitle','Loading','WorldMap','ChapterMap','PreLevel']:
 paths=[p for p in old if ('/'+page+'.prefab') in p or (('/'+page+'Aligned') in p) or ('/Production'+page) in p]
 protected[page]={'count':len(paths),'unchanged':all(current[p]==old[p] for p in paths)}
for group,terms in [('FrontEnd',('/FrontEndHomeAligned','/SplashTitleConfig/','/SplashTitleReconstructed/')),('Packages',('Packages/',)),('ProjectSettings',('ProjectSettings/',)),('AndroidGuards',('Android','Signing','Keystore')),('RulesAndVfx',('/Runtime/Gameplay/','/GameplayCore/','PooledUiVfxLayer','GameplayMotion'))]:
 paths=[p for p in old if any(t in p for t in terms)]
 protected[group]={'count':len(paths),'unchanged':all(current[p]==old[p] for p in paths)}
assert all(p['unchanged'] for p in protected.values()),protected
diff=[]; manifest=[]
for p in sorted(changed):
 assert sha(backup/p)==old[p]
 diff.extend(difflib.unified_diff((backup/p).read_text().splitlines(True),(root/p).read_text().splitlines(True),fromfile='before/'+p,tofile='after/'+p))
 manifest.append({'path':p,'before':old[p],'sha256':current[p]})
for p in sorted(new):
 digest=sha(root/p);current[p]=digest;manifest.append({'path':p,'added':True,'sha256':digest})
 if not p.endswith('.png'):diff.extend(difflib.unified_diff([],(root/p).read_text().splitlines(True),fromfile='/dev/null',tofile='after/'+p))
for p in changed|set(new):
 if p.endswith('.cs'):
  assert all(line==line.rstrip() for line in (root/p).read_text().splitlines()),p
  assert (root/p).read_bytes().endswith(b'\n'),p
audit={'passed':True,'auditUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'baselineCount':len(before),'unchanged':len(before)-len(changed),'modified':sorted(changed),'addedProductionFiles':sorted(new),'metadataCount':len(meta),'allExistingMetadataBytesAndGuidsPreserved':True,'newGuidsUnique':True,'protectedGroups':protected,'originalGameplayPrefabIds':sorted(old_ids),'retainedOriginalIds':sorted(old_ids&new_ids),'removedOriginalIds':sorted(old_ids-new_ids),'finalPrefabObjectCount':len(new_ids),'runtimeAssetHashesRecorded':True,'scope':'Recorded pre-task production paths, new Gameplay resources/runtime/tests; no secrets or generated caches.'}
for name,data in [('FinalSourceAudit.json',audit),('FinalProductionHashes.json',[{'path':p,'sha256':h} for p,h in current.items()]),('SourceChangeManifest.json',manifest)]: (delivery/name).write_text(json.dumps(data,indent=2)+'\n')
(delivery/'AppliedSourceChanges.diff').write_text(''.join(diff))
print(json.dumps({'passed':True,'unchanged':audit['unchanged'],'modified':len(changed),'added':len(new),'metadata':len(meta),'preservedIds':len(old_ids),'protected':protected}))
