#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, re, sys

ROOT = Path(__file__).resolve().parents[1]
BASE = Path('/mnt/data/v120_step03_3r3_candidate')
MILESTONE = 'v1.20-step03.3R-FIX01-real-asset-runtime-prefab-delivery'
ALLOW_CHANGED = {
    'README.md',
    'MILESTONE_VERSION.txt',
    'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs',
    'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs',
    'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs',
    'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs',
}
PAGES = ['Loading','Home','WorldMap','ChapterMap','PreLevel','Gameplay','Pause','Victory','Fail','ChapterComplete']
FORBIDDEN_RUNTIME = ['DesignScreens','ReferenceOnly','ReferenceMatch/','ReferenceMatchR3/']

def sha(path):
    h=hashlib.sha256()
    with path.open('rb') as f:
        for block in iter(lambda:f.read(1024*1024), b''): h.update(block)
    return h.hexdigest()

def files(root):
    return {p.relative_to(root).as_posix(): sha(p) for p in root.rglob('*') if p.is_file()}

def fail(msg, failures): failures.append(msg)

def same_tree(sub, failures):
    b=files(BASE/sub); c=files(ROOT/sub)
    if b != c: fail(f'{sub} changed', failures)

def main():
    failures=[]
    if not BASE.exists(): fail('baseline missing', failures)
    B=files(BASE); C=files(ROOT)
    changed=sorted(k for k in B.keys() & C.keys() if B[k] != C[k])
    added=sorted(C.keys()-B.keys())
    deleted=sorted(B.keys()-C.keys())
    unexpected=[x for x in changed if x not in ALLOW_CHANGED]
    if unexpected: fail('unexpected inherited changes: '+', '.join(unexpected), failures)
    if deleted: fail('deleted inherited files: '+', '.join(deleted), failures)

    # Every inherited PNG remains byte-identical.
    inherited_png=[k for k in B if k.lower().endswith('.png')]
    png_changed=[k for k in inherited_png if C.get(k) != B[k]]
    if png_changed: fail('inherited PNG changes: '+', '.join(png_changed), failures)

    same_tree(Path('Packages'), failures)
    same_tree(Path('ProjectSettings'), failures)

    # Every inherited test file remains identical; FIX01 test is additive.
    inherited_tests=[k for k in B if k.startswith('Assets/LegendsOfRus/Tests/')]
    test_changes=[k for k in inherited_tests if C.get(k) != B[k]]
    if test_changes: fail('inherited tests changed: '+', '.join(test_changes), failures)

    if (ROOT/'MILESTONE_VERSION.txt').read_text().strip() != MILESTONE:
        fail('milestone mismatch', failures)

    asset_root=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/Assets/Fix01'
    pngs=list(asset_root.rglob('*.png'))
    if len(pngs) != 59: fail(f'FIX01 PNG count {len(pngs)} != 59', failures)
    checks=[('Icons',20),('Kit',24),('Gems',6)]
    for folder,count in checks:
        actual=len(list((asset_root/folder).glob('*.png')))
        if actual != count: fail(f'{folder} count {actual} != {count}', failures)
    for f in ['bg_home_clean.png','bg_worldmap_clean.png','bg_chaptermap_clean.png','bg_prelevel_clean.png','bg_gameplay_clean.png','hero_ivan_firebird.png','portrait_ivan.png','ribbon_red.png','title_plaque_navy.png']:
        if not (asset_root/f).exists(): fail('missing asset '+f, failures)

    page_root=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/Pages'
    prefabs=list(page_root.glob('*.prefab'))
    if len(prefabs) != 10: fail(f'prefab count {len(prefabs)} != 10', failures)
    for p in PAGES:
        pf=page_root/(p+'.prefab'); meta=page_root/(p+'.prefab.meta')
        if not pf.exists(): fail('missing prefab '+p, failures); continue
        if not meta.exists(): fail('missing prefab meta '+p, failures)
        text=pf.read_text(errors='replace')
        for token in ['DesignScreens','ReferenceOnly']:
            if token in text: fail(f'{p} prefab contains forbidden token {token}', failures)
        if 'f1033a9a7f6c4c12a8b9d0e1f2a3b4c5' not in text:
            fail(f'{p} prefab missing FIX01 bootstrap script GUID', failures)

    runtime=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs').read_text()
    for token in FORBIDDEN_RUNTIME:
        if token in runtime: fail('FIX01 runtime contains forbidden token '+token, failures)
    loader=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRuntime.cs').read_text()
    if 'ProductionUiFix01Runtime.TryBuild(id, parent)' not in loader: fail('direct runtime fallback missing', failures)

    for name in ['Step20CoreLoopPageBuilder.cs','Step20CoreLoopReferenceMatchBuilder.cs','Step20CoreLoopReferenceMatchR3Builder.cs']:
        text=(ROOT/'Assets/LegendsOfRus/Scripts/Editor'/name).read_text()
        if 'Step20ProductionUiFix01SerializedPrefabBuilder.IsInstalled()' not in text: fail(name+' missing FIX01 auto guard', failures)
        if 'Step20ProductionUiFix01SerializedPrefabBuilder.BuildAll(false)' not in text: fail(name+' missing FIX01 reapply', failures)

    activation=json.loads((ROOT/'Assets/LegendsOfRus/Resources/Config/production_ui_fix01_v120_step03_3r.json').read_text())
    true_flags=['serializedPrefabDeliveryEnabled','directRuntimeAssetFallbackEnabled','independentRuntimeAssetCompositionEnabled','runtimeUiCutoverRepairAuthorized','legacyBuilderRegressionGuardEnabled','liveMatch3BoardPreserved','pauseOverLiveGameplayPreserved']
    false_flags=['designScreensRuntimeFallbackEnabled','referenceOnlyRuntimeUseEnabled','screenshotCropProductionUseEnabled','fullScreenScreenshotRuntimeUseAuthorized','lockedReferenceOverwriteAuthorized','screenshotDeletionAuthorized','automaticArtOverwriteEnabled','automaticBalanceMutationEnabled','automaticRewardMutationEnabled','productionContentPublishAuthorized','playConsoleActionEnabled','finalVisualSignoffGranted']
    for f in true_flags:
        if activation.get(f) is not True: fail(f+' not true', failures)
    for f in false_flags:
        if activation.get(f) is not False: fail(f+' not false', failures)
    if activation.get('serializedPrefabCount') != 10: fail('serializedPrefabCount != 10', failures)

    # Background plates must not simply be the full approved screens byte-for-byte.
    refs=ROOT/'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens'
    for plate,ref in [('bg_home_clean.png','Home.png'),('bg_worldmap_clean.png','WorldMap.png'),('bg_chaptermap_clean.png','ChapterMap.png'),('bg_prelevel_clean.png','PreLevel.png'),('bg_gameplay_clean.png','Gameplay.png')]:
        if sha(asset_root/plate) == sha(refs/ref): fail(f'{plate} byte-identical to full reference', failures)

    # Old screenshot-strip naming forbidden inside FIX01 assets.
    for p in pngs:
        n=p.name.lower()
        if any(t in n for t in ['_component','_scene','_shell']): fail('legacy strip-like FIX01 name '+p.name, failures)

    # Legacy screenshots remain present for QA/later waves, not deleted.
    design=list((ROOT/'Assets/LegendsOfRus/Resources/DesignScreens').glob('*.png'))
    if len(design) != 22: fail(f'DesignScreens count {len(design)} != 22', failures)

    screen_router=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
    if 'SetPixelRect(rt, 79, 357, 786, 942)' not in screen_router: fail('live board boundary changed', failures)
    if screen_router.find('ProductionCoreLoopPageRegistry.IsCutover(id)') < 0 or screen_router.find('DesignScreenRegistry.ResourceKey(id)') <= screen_router.find('ProductionCoreLoopPageRegistry.IsCutover(id)'):
        fail('production routing no longer precedes legacy routing', failures)

    package=json.loads((ROOT/'Packages/manifest.json').read_text())
    if package.get('dependencies',{}).get('com.unity.addressables') != '2.8.1': fail('Addressables not 2.8.1', failures)
    save=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs').read_text()
    if 'CurrentPayloadVersion = 6' not in save: fail('save-v6 pin missing', failures)
    hot=(ROOT/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text()
    if 'Array.IndexOf(parameterTypes, typeof(string))' not in hot: fail('NUnit Type hotfix missing', failures)
    if not (ROOT/'README_COMPILE_HOTFIX05.md').exists(): fail('Compile Hotfix05 documentation missing', failures)

    tests=(ROOT/'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ProductionUiFix01DeliveryEditModeTests.cs').read_text()
    test_attr=len(re.findall(r'\[Test\]',tests)); cases=len(re.findall(r'\[TestCase',tests))
    if test_attr != 20 or cases != 25: fail(f'FIX01 test attributes {test_attr}/{cases} != 20/25', failures)
    selftest=(ROOT/'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SelfTest.cs').read_text()
    if 'ExpectedChecks = 88' not in selftest: fail('self-test expected count != 88', failures)

    aabs=list(ROOT.rglob('*.aab'))
    if aabs: fail('AAB unexpectedly bundled', failures)

    report={
        'milestone':MILESTONE,
        'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
        'baseline_files':len(B),
        'candidate_files':len(C),
        'changed_inherited_count':len(changed),
        'changed_inherited_paths':changed,
        'added_count':len(added),
        'deleted_count':len(deleted),
        'inherited_png_count':len(inherited_png),
        'inherited_pngs_byte_identical':not png_changed,
        'fix01_runtime_png_count':len(pngs),
        'serialized_prefab_count':len(prefabs),
        'fix01_test_attributes':test_attr,
        'fix01_test_cases':cases,
        'fix01_self_test_expected_checks':88,
        'addressables':'2.8.1',
        'save_payload_version':6,
        'unity_execution':'PENDING_USER_UNITY_EXECUTION',
        'final_visual_signoff':False,
        'failures':failures,
    }
    out=ROOT/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP03_3R_FIX01.json'
    out.write_text(json.dumps(report,indent=2)+'\n')
    print(json.dumps(report,indent=2))
    return 1 if failures else 0

if __name__=='__main__': sys.exit(main())
