#!/usr/bin/env python3
from pathlib import Path
import json, sys, struct
root=Path(__file__).resolve().parents[1]

def fail(msg):
    print('FAIL:',msg); sys.exit(1)

def png_size(p):
    b=p.read_bytes()[:24]
    if len(b)<24 or b[:8]!=b'\x89PNG\r\n\x1a\n': return None
    return struct.unpack('>II',b[16:24])

milestone=(root/'MILESTONE_VERSION.txt').read_text().strip()
expected='v1.20-step03.3R1-core-loop-reference-audit-fidelity-gap-matrix'
if milestone!=expected: fail('milestone mismatch: '+milestone)

matrix_path=root/'Assets/LegendsOfRus/Documentation/V1_20_STEP03_3R1_CORE_LOOP_FIDELITY_GAP_MATRIX.json'
if not matrix_path.is_file(): fail('missing fidelity matrix')
m=json.loads(matrix_path.read_text())
if m.get('auditedP0SurfaceCount')!=14: fail('auditedP0SurfaceCount must be 14')
if m.get('approvedReferenceCount')!=8: fail('approvedReferenceCount must be 8')
if m.get('provisionalReferenceCount')!=4: fail('provisionalReferenceCount must be 4')
if m.get('missingReferenceCount')!=2: fail('missingReferenceCount must be 2')
if m.get('referenceMatchSignoffGranted') is not False: fail('reference match signoff must remain false')
if m.get('screenshotDeletionAuthorized') is not False: fail('screenshot deletion must remain false')
if m.get('genericStyleSubstitutionAllowed') is not False: fail('generic style substitution must remain false')
if len(m.get('pages',[]))!=14: fail('matrix page record count must be 14')

approved=[p for p in m['pages'] if p.get('referenceStatus')=='approved']
provisional=[p for p in m['pages'] if p.get('referenceStatus')=='provisional_capture']
missing=[p for p in m['pages'] if p.get('referenceStatus')=='missing']
for p in approved+provisional:
    rel=p.get('referencePath','')
    f=root/rel
    if not f.is_file(): fail('missing reference file: '+rel)
    if png_size(f)!=(941,1672): fail('reference resolution mismatch: '+rel)
for p in missing:
    if p.get('referencePath'): fail('missing-reference record unexpectedly has a path: '+p.get('pageKey',''))

boundary=json.loads((root/'Assets/LegendsOfRus/Documentation/V1_20_STEP03_3R1_ACTIVATION_BOUNDARY.json').read_text())
false_keys=['referenceMatchSignoffGranted','runtimeRoutingChangeAuthorized','screenshotDeletionAuthorized','lockedArtOverwriteAuthorized','automaticArtGenerationAuthorized','automaticPrefabCutoverAuthorized','productionPublishAuthorized']
for k in false_keys:
    if boundary.get(k) is not False: fail(k+' must be false')
if boundary.get('auditOnlyCheckpoint') is not True: fail('auditOnlyCheckpoint must be true')
if boundary.get('referenceMatchRequiredBeforeVisualCompleteClaim') is not True: fail('reference-match gate must be true')

# Step 3.3 baseline invariants that must still exist.
if len(list((root/'Assets/LegendsOfRus/Resources/DesignScreens').glob('*.png')))!=22: fail('expected 22 retained screenshot reference/legacy files')
if len(list((root/'Assets/LegendsOfRus/Art/Production/UI/CanonicalKit').glob('*.png')))!=34: fail('expected 34 canonical kit sprites')
router=(root/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
if 'ProductionCoreLoopPageRegistry.IsCutover(id)' not in router: fail('Step 3.3 production cutover marker missing')
runtime=(root/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs').read_text()
if 'DesignScreens/' in runtime or 'DesignScreenRegistry' in runtime: fail('production core-loop runtime must remain screenshot-free')

result={
 'status':'PASS_STATIC_SOURCE_VERIFICATION',
 'milestone':milestone,
 'auditedP0Surfaces':14,
 'approvedReferences':8,
 'provisionalReferences':4,
 'missingReferences':2,
 'retainedDesignScreens':22,
 'canonicalUiSprites':34,
 'referenceMatchSignoffGranted':False,
 'runtimeRoutingChangedInThisCheckpoint':False,
 'nextBatch':'v1.20 Step 3.3R.2 — Home / World Map / Chapter Map Visual Rebuild'
}
print(json.dumps(result,indent=2,ensure_ascii=False))
