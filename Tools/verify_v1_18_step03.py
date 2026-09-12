#!/usr/bin/env python3
import argparse, hashlib, json, sys
from pathlib import Path


def sha(path: Path): return hashlib.sha256(path.read_bytes()).hexdigest()
def inventory(root: Path): return {p.relative_to(root).as_posix(): sha(p) for p in root.rglob('*') if p.is_file()}
def read(root: Path, rel: str): return (root/rel).read_text(encoding='utf-8')


def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--candidate', default=str(Path(__file__).resolve().parents[1]))
    ap.add_argument('--baseline', required=True)
    ap.add_argument('--output')
    args=ap.parse_args()
    candidate=Path(args.candidate).resolve(); baseline=Path(args.baseline).resolve()
    failures=[]; notes=[]
    b=inventory(baseline); c=inventory(candidate)
    deleted=sorted(set(b)-set(c))
    changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k])
    added=sorted(set(c)-set(b))

    allowed_changed={
      'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
      'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
      'Assets/LegendsOfRus/Documentation/README_FIRST.md',
      'MILESTONE_VERSION.txt','README.md'
    }
    unexpected=[x for x in changed if x not in allowed_changed]
    if unexpected: failures.extend('unexpected inherited change: '+x for x in unexpected)
    missing_expected=[x for x in allowed_changed if x not in changed]
    if missing_expected: notes.extend('allowlisted inherited file unchanged: '+x for x in missing_expected)

    for prefix in ['ProjectSettings/','Packages/','Assets/LegendsOfRus/Scripts/SaveCore/','Assets/LegendsOfRus/Tests/']:
        for rel,h in b.items():
            if rel.startswith(prefix) and c.get(rel)!=h:
                failures.append('protected changed/missing: '+rel)

    base_png={k:v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    cand_png={k:v for k,v in c.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    if len(base_png)!=65 or len(cand_png)!=65:
        failures.append(f'approved PNG count mismatch baseline={len(base_png)} candidate={len(cand_png)}')
    for rel,h in base_png.items():
        if cand_png.get(rel)!=h: failures.append('approved PNG changed/missing: '+rel)
    if deleted: failures.extend('baseline file deleted: '+x for x in deleted)

    json_files=list(candidate.rglob('*.json'))
    for p in json_files:
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: failures.append(f'json parse failure {p.relative_to(candidate)}: {e}')

    manifest=json.loads(read(candidate,'Packages/manifest.json'))
    if manifest.get('dependencies',{}).get('com.unity.addressables')!='2.8.1':
        failures.append('Addressables is not pinned to 2.8.1')

    core_asm=json.loads(read(candidate,'Assets/LegendsOfRus/Scripts/StoreReleaseCore/LegendsOfRus.StoreReleaseCore.asmdef'))
    if core_asm.get('noEngineReferences') is not True or core_asm.get('references')!=[]:
        failures.append('StoreReleaseCore is not engine-free')
    core_sources='\n'.join(p.read_text(encoding='utf-8') for p in (candidate/'Assets/LegendsOfRus/Scripts/StoreReleaseCore').glob('*.cs'))
    for forbidden in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','PlayerPrefs','deviceUniqueIdentifier','advertisingIdentifier','File.WriteAllText']:
        if forbidden in core_sources: failures.append('forbidden StoreReleaseCore API: '+forbidden)

    req=read(candidate,'Assets/LegendsOfRus/Scripts/StoreReleaseCore/StoreListingRequirements.cs')
    tokens=['AppNameMaxCharacters = 30','ShortDescriptionMaxCharacters = 80','FullDescriptionMaxCharacters = 4000',
            'IconWidth = 512','IconHeight = 512','FeatureGraphicWidth = 1024','FeatureGraphicHeight = 500',
            'MinimumScreenshotCount = 2','MaximumScreenshotsPerDeviceType = 8','ScreenshotMinimumDimension = 320',
            'ScreenshotMaximumDimension = 3840','RecommendedGameScreenshotCount = 3',
            'RecommendedPortraitScreenshotWidth = 1080','RecommendedPortraitScreenshotHeight = 1920',
            'ScreenshotAltTextMaxCharacters = 140']
    for token in tokens:
        if token not in req: failures.append('store requirement token missing: '+token)

    activation=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/store_release_activation.json'))
    expected_true=['localStoreListingValidationApproved','localReleaseChecklistApproved']
    expected_false=['storeListingTextApproved','storeListingAssetsApproved','privacyDataSafetyApproved','contentRatingCompleted',
                    'targetAudienceCompleted','adsDeclarationCompleted','appAccessDeclarationCompleted','productionSigningApproved',
                    'appBundleEvidenceRetained','releaseNotesApproved','controlledTestBuildAuthorized','storePublishingAuthorized',
                    'productionCommercialActivationApproved']
    for k in expected_true:
        if activation.get(k) is not True: failures.append('store activation should be true: '+k)
    for k in expected_false:
        if activation.get(k) is not False: failures.append('store/release gate unexpectedly enabled: '+k)
    if activation.get('requirementsSnapshotDate')!='2026-09-09': failures.append('requirements snapshot date mismatch')

    final_dir=candidate/'ProjectDocs/StoreListing/FinalAssets'
    graphics=[]
    if final_dir.exists():
        for ext in ('*.png','*.jpg','*.jpeg'): graphics.extend(final_dir.glob(ext))
    if graphics: failures.append('final store asset staging unexpectedly contains publishable graphics')

    inv=json.loads(read(candidate,'Assets/LegendsOfRus/Documentation/V1_18_STEP03_STORE_LISTING_ASSET_INVENTORY.json'))
    evidence=inv.get('projectEvidence',{})
    if evidence.get('referenceDesignScreens',{}).get('count')!=22: failures.append('reference design screen inventory is not 22')
    if evidence.get('finalPhoneScreenshots',{}).get('count')!=0: failures.append('final screenshot count must remain zero')
    if inv.get('publicationApproved') is not False: failures.append('store inventory publicationApproved unexpectedly true')

    qa=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json'))
    for k in ['remoteQaUploadApproved','productionQaProviderApproved','productionSigningApproved','controlledTestBuildAuthorized','storePublishingAuthorized']:
        if qa.get(k) is not False: failures.append('Step1 QA/release gate unexpectedly enabled: '+k)
    comm=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy runtime activation unexpectedly enabled')
    balance=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/balance_telemetry_review_activation.json'))
    for k in ['remoteTelemetryUploadApproved','productionAnalyticsBridgeApproved','automaticBalanceMutationApproved','tutorialExperimentMutationApproved']:
        if balance.get(k) is not False: failures.append('Step2 balance gate unexpectedly enabled: '+k)

    if 'Array.IndexOf(parameterTypes, typeof(string))' not in read(candidate,'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'):
        failures.append('Step4 NUnit Type hotfix missing')
    play_asm=json.loads(read(candidate,'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    if 'LegendsOfRus.ContentDeliveryCore' not in play_asm.get('references',[]): failures.append('Step5 ContentDeliveryCore PlayMode asmdef hotfix missing')

    test_decls=0
    for p in (candidate/'Assets/LegendsOfRus/Tests/StoreReleaseEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); test_decls+=t.count('[Test]')+t.count('[UnityTest]')
    self_checks=read(candidate,'Assets/LegendsOfRus/Scripts/Editor/Step18StoreListingReleaseChecklistSelfTest.cs').count('Check("')
    if self_checks!=30: failures.append(f'expected 30 self-test checks, found {self_checks}')

    report={
      'version':'v1.18-step03',
      'milestone':'store-listing-release-checklist-foundation',
      'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
      'passed':not failures,
      'unityVersion':'6000.5.9f1',
      'requirementsSnapshotDate':'2026-09-09',
      'unityExecutionEvidence':'NOT_RETAINED_FOR_EXACT_STEP03_BYTES',
      'unityCompileStatus':'PENDING_USER_UNITY_EXECUTION',
      'editModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'playModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'finalStoreListingTextApproved':False,
      'finalStoreListingAssetsApproved':False,
      'privacyDataSafetyApproved':False,
      'productionSigningApproved':False,
      'appBundleEvidenceRetained':False,
      'controlledTestBuildAuthorized':False,
      'storePublishingAuthorized':False,
      'productionCommercialActivationEnabled':False,
      'baselineProjectFiles':len(b), 'candidateProjectFiles':len(c),
      'changedInheritedPaths':changed, 'addedPaths':added, 'deletedPaths':deleted,
      'approvedPngFiles':len(cand_png),
      'approvedPngsIdenticalToStep02':all(cand_png.get(k)==v for k,v in base_png.items()) and len(cand_png)==len(base_png),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'referenceDesignScreensExcludedFromStoreEvidence':True,
      'finalStagedGraphics':len(graphics),
      'newStoreReleaseTestDeclarations':test_decls,
      'editorSelfTestCheckSites':self_checks,
      'jsonFilesParsed':len(json_files),
      'failures':failures,'notes':notes
    }
    out=Path(args.output) if args.output else candidate/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP03.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2))
    return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
