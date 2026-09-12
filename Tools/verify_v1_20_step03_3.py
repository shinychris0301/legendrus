#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, os, sys
root=Path(__file__).resolve().parents[1]

def sha(p):
    h=hashlib.sha256();
    with p.open('rb') as f:
        for c in iter(lambda:f.read(1024*1024),b''): h.update(c)
    return h.hexdigest()

def fail(msg): print('FAIL:',msg); sys.exit(1)
required=[
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiGradient.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageMarker.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageSelfTest.cs',
 'Assets/LegendsOfRus/Resources/Config/production_ui_core_loop_v120_step03_3.json',
 'ProjectDocs/ProductionUI/core_loop_page_reconstruction_matrix.json']
for rel in required:
    if not (root/rel).is_file(): fail('missing '+rel)
config=json.loads((root/'Assets/LegendsOfRus/Resources/Config/production_ui_core_loop_v120_step03_3.json').read_text())
if config.get('cutoverPageCount')!=10: fail('cutoverPageCount')
if config.get('coreLoopScreenshotRuntimeCutoverAuthorized') is not True: fail('cutover authorization')
if config.get('screenshotDeletionAuthorized') is not False: fail('screenshot deletion must remain false')
if config.get('missingProductionPrefabMayUseScreenshot') is not False: fail('missing prefab screenshot fallback must be false')
router=(root/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
if router.find('ProductionCoreLoopPageRegistry.IsCutover(id)')<0: fail('router production cutover missing')
if router.find('DesignScreenRegistry.ResourceKey(id)')<router.find('ProductionCoreLoopPageRegistry.IsCutover(id)'): fail('legacy registry precedes production')
runtime=(root/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs').read_text()
if 'DesignScreenRegistry' in runtime or 'DesignScreens/' in runtime: fail('production runtime references screenshot registry')
shots=list((root/'Assets/LegendsOfRus/Resources/DesignScreens').glob('*.png'))
if len(shots)!=22: fail('expected 22 retained QA screenshot files')
kit=list((root/'Assets/LegendsOfRus/Art/Production/UI/CanonicalKit').glob('*.png'))
if len(kit)!=34: fail('expected 34 canonical kit sprites')
print(json.dumps({'status':'PASS_STATIC_SOURCE_VERIFICATION','cutoverPages':10,'retainedScreenshots':22,'canonicalUiSprites':34},indent=2))
