#!/usr/bin/env python3
import argparse, hashlib, json, sys
from pathlib import Path

def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
def inv(root): return {p.relative_to(root).as_posix(): sha(p) for p in root.rglob('*') if p.is_file()}
def read(root, rel): return (root/rel).read_text(encoding='utf-8')

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--candidate', default=str(Path(__file__).resolve().parents[1]))
    ap.add_argument('--baseline', required=True)
    ap.add_argument('--output')
    a=ap.parse_args()
    croot=Path(a.candidate).resolve(); broot=Path(a.baseline).resolve(); failures=[]; notes=[]
    b=inv(broot); c=inv(croot)
    deleted=sorted(set(b)-set(c)); changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k]); added=sorted(set(c)-set(b))
    allowed={'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json','Assets/LegendsOfRus/Documentation/README_FIRST.md','MILESTONE_VERSION.txt','README.md'}
    for x in changed:
        if x not in allowed: failures.append('unexpected inherited change: '+x)
    if deleted: failures += ['baseline file deleted: '+x for x in deleted]
    for prefix in ['ProjectSettings/','Packages/','Assets/LegendsOfRus/Scripts/SaveCore/','Assets/LegendsOfRus/Tests/']:
        for rel,h in b.items():
            if rel.startswith(prefix) and c.get(rel)!=h: failures.append('protected changed/missing: '+rel)
    bp={k:v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    cp={k:v for k,v in c.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    if len(bp)!=65 or len(cp)!=65: failures.append(f'approved PNG count mismatch baseline={len(bp)} candidate={len(cp)}')
    for rel,h in bp.items():
        if cp.get(rel)!=h: failures.append('approved PNG changed/missing: '+rel)
    for p in croot.rglob('*.json'):
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: failures.append(f'json parse failure {p.relative_to(croot)}: {e}')
    manifest=json.loads(read(croot,'Packages/manifest.json'))
    if manifest.get('dependencies',{}).get('com.unity.addressables')!='2.8.1': failures.append('Addressables is not 2.8.1')
    if 'Array.IndexOf(parameterTypes, typeof(string))' not in read(croot,'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'): failures.append('Step4 NUnit hotfix missing')
    pa=json.loads(read(croot,'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    if 'LegendsOfRus.ContentDeliveryCore' not in pa.get('references',[]): failures.append('ContentDeliveryCore test asmdef hotfix missing')
    ba=json.loads(read(croot,'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef'))
    if 'TestAssemblies' not in ba.get('optionalUnityReferences',[]): failures.append('BalanceReview UnityTest hotfix missing')
    asm=json.loads(read(croot,'Assets/LegendsOfRus/Scripts/ControlledTestBuildCore/LegendsOfRus.ControlledTestBuildCore.asmdef'))
    if asm.get('noEngineReferences') is not True or asm.get('references')!=[]: failures.append('ControlledTestBuildCore not engine-free')
    core='\n'.join(p.read_text(encoding='utf-8') for p in (croot/'Assets/LegendsOfRus/Scripts/ControlledTestBuildCore').glob('*.cs'))
    for token in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','PlayerPrefs','deviceUniqueIdentifier','advertisingIdentifier','BuildPipeline.BuildPlayer']:
        if token in core: failures.append('forbidden controlled-test core token: '+token)
    req=read(croot,'Assets/LegendsOfRus/Scripts/ControlledTestBuildCore/ControlledTestBuildRequirements.cs')
    for token in ['RequirementsSnapshotDate = "2026-09-09"','RequiredTargetApiLevel = 36','AppBundleRequired = true','Il2CppRequired = true','Arm64Required = true','SixteenKbNativePageSizeRequired = true','PackageRegistrationEffectiveDate = "2026-09-30"']:
        if token not in req: failures.append('requirement token missing: '+token)
    act=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json'))
    if act.get('localControlledTestPreflightApproved') is not True: failures.append('local preflight not approved')
    for k in ['packageLockReviewed','projectSettingsReviewed','productionApplicationIdentifierApproved','versioningApproved','playAppSigningApproved','uploadKeyWorkflowApproved','productionSigningApproved','controlledTestBuildAuthorized','signedAppBundleEvidenceRetained','closedTestUploadAuthorized','storePublishingAuthorized','packageNameRegistrationReviewed','remoteBuildServiceApproved']:
        if act.get(k) is not False: failures.append('controlled-test gate unexpectedly enabled: '+k)
    if act.get('requiredTargetApiLevel')!=36: failures.append('activation target API is not 36')
    android=json.loads(read(croot,'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json'))
    if android.get('requiredTargetApiLevel')!=36: failures.append('Android policy target API drifted')
    for k in ['allowAppBundle','allowCustomSigning','productionBuildApproved','signingIdentityApproved','playAppSigningApproved','sixteenKbPageCompatibilityVerified','dataSafetyApproved']:
        if android.get(k) is not False: failures.append('legacy Android build guard unexpectedly weakened: '+k)
    runner=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs')
    for token in ['BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','keystoreName','keyaliasName','deviceUniqueIdentifier','advertisingIdentifier','UnityWebRequest','HttpClient']:
        if token in runner: failures.append('forbidden preflight runner token: '+token)
    store=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/store_release_activation.json'))
    for k in ['storeListingTextApproved','storeListingAssetsApproved','privacyDataSafetyApproved','productionSigningApproved','appBundleEvidenceRetained','controlledTestBuildAuthorized','storePublishingAuthorized']:
        if store.get(k) is not False: failures.append('Step3 store gate unexpectedly enabled: '+k)
    qa=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json'))
    for k in ['remoteQaUploadApproved','productionQaProviderApproved','productionSigningApproved','controlledTestBuildAuthorized','storePublishingAuthorized']:
        if qa.get(k) is not False: failures.append('QA gate unexpectedly enabled: '+k)
    comm=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy activation unexpectedly enabled')
    tests=0
    for p in (croot/'Assets/LegendsOfRus/Tests/ControlledTestBuildEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); tests+=t.count('[Test]')+t.count('[UnityTest]')
    checks=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs').count('Check("')
    if tests!=16: failures.append(f'expected 16 new test declarations, found {tests}')
    if checks!=31: failures.append(f'expected 31 self-test checks, found {checks}')
    report={
      'version':'v1.18-step04','milestone':'controlled-test-build-release-candidate-foundation','status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not failures,
      'unityVersion':'6000.5.9f1','requirementsSnapshotDate':'2026-09-09','requiredTargetApiLevel':36,
      'unityCompileStatus':'PENDING_USER_UNITY_EXECUTION','editorSelfTestStatus':'PENDING_USER_UNITY_EXECUTION','editModeStatus':'PENDING_USER_UNITY_EXECUTION','playModeStatus':'PENDING_USER_UNITY_EXECUTION','deviceMatrixStatus':'PENDING_RETAINED_PHYSICAL_DEVICE_EVIDENCE',
      'productionApplicationIdentifierApproved':False,'versioningApproved':False,'playAppSigningApproved':False,'uploadKeyWorkflowApproved':False,'productionSigningApproved':False,'controlledTestBuildAuthorized':False,'signedAppBundleEvidenceRetained':False,'closedTestUploadAuthorized':False,'storePublishingAuthorized':False,
      'baselineProjectFiles':len(b),'candidateProjectFiles':len(c),'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':len(cp),'approvedPngsIdenticalToStep03':len(cp)==len(bp) and all(cp.get(k)==v for k,v in bp.items()),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'legacyAndroidBuildGuardStillDevelopmentOnly':True,'automaticBuildPipelineInvocationPresent':False,'credentialFieldsReadByStep04':False,
      'newControlledTestBuildTestDeclarations':tests,'editorSelfTestCheckSites':checks,'failures':failures,'notes':notes
    }
    out=Path(a.output) if a.output else croot/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP04.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2))
    return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
