#!/usr/bin/env python3
from pathlib import Path
import json, hashlib, sys
ROOT=Path(__file__).resolve().parents[1]
DOC=ROOT/'Assets/LegendsOfRus/Documentation'
MILESTONE='v1.20-step06-functional-acceptance-asset-readiness'

def sha(p):
    h=hashlib.sha256()
    with p.open('rb') as f:
        for b in iter(lambda:f.read(1024*1024),b''): h.update(b)
    return h.hexdigest()

def rel(p): return p.relative_to(ROOT).as_posix()
def digest(files):
    h=hashlib.sha256(); n=0
    for p in sorted(files,key=lambda x:rel(x)):
        if not p.is_file(): continue
        h.update(rel(p).encode()+b'\0'+sha(p).encode()+b'\n'); n+=1
    return h.hexdigest(),n

def files_under(p): return [x for x in p.rglob('*') if x.is_file()]

def fail(msg): print('FAIL:',msg); return False
ok=True
if (ROOT/'MILESTONE_VERSION.txt').read_text().strip()!=MILESTONE: ok=fail('milestone') and ok
freeze=json.loads((DOC/'V1_20_STEP06_FUNCTIONAL_FREEZE_MANIFEST.json').read_text())
for name,item in freeze['trees'].items():
    if name=='allProjectPngs': files=[p for p in ROOT.rglob('*.png') if p.is_file()]
    else: files=files_under(ROOT/item['path'])
    d,n=digest(files)
    if d!=item['treeSha256'] or n!=item['fileCount']:
        ok=fail(f'freeze tree {name}: {n}/{d} expected {item["fileCount"]}/{item["treeSha256"]}') and ok
for r,expected in freeze['criticalFiles'].items():
    p=ROOT/r
    if not p.exists() or sha(p)!=expected: ok=fail('critical file '+r) and ok
contracts=json.loads((DOC/'V1_20_STEP06_PAGE_ASSET_CONTRACTS.json').read_text())
if contracts['pageCount']!=36 or len(contracts['pages'])!=36: ok=fail('36 page contracts') and ok
if any(x['assetContract']['runtimeCompositeScreenshotAllowed'] for x in contracts['pages']): ok=fail('runtime screenshot composite allowed') and ok
if any(not x['assetContract']['preserveExistingInteractionRolesAndCallbacks'] for x in contracts['pages']): ok=fail('interaction lock') and ok
waves=json.loads((DOC/'V1_20_STEP06_RECONSTRUCTION_WAVES.json').read_text())
if sum(x['pageCount'] for x in waves['waves'])!=36: ok=fail('wave page count') and ok
ready=json.loads((DOC/'V1_20_STEP06_PRODUCTION_ASSET_RECONSTRUCTION_READINESS.json').read_text())
inv=ready['currentAssetInventory']
expect={'approvedReferenceScreenPngs':11,'direct36PageApprovedReferences':10,'canonicalUiKitPngs':34,'existingProductionPagePrefabs':10,'coreLoopReconstructedComponentPngs':38,'pagesWithReconstructedComponentCoverage':6,'approvedGameplayPngs':11}
for k,v in expect.items():
    if inv.get(k)!=v: ok=fail(f'inventory {k}') and ok
gate=json.loads((DOC/'V1_20_STEP06_FUNCTIONAL_ACCEPTANCE_GATE.json').read_text())
if gate['decision']['sourceFunctionalAcceptance']!='PASS': ok=fail('source acceptance') and ok
if gate['decision']['executableFunctionalAcceptance']!='PENDING': ok=fail('executable acceptance must remain pending') and ok
if gate['decision']['productionAssetReconstructionRuntimeCutover']!='BLOCKED': ok=fail('cutover must remain blocked') and ok
if list(ROOT.rglob('*.aab')): ok=fail('AAB bundled') and ok
if ok:
    print('PASS: v1.20 Step 6 static functional acceptance/readiness verification')
    print('NOTE: Unity/Android executable acceptance remains pending and is not implied by this PASS.')
    sys.exit(0)
sys.exit(1)
