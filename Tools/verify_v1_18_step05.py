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
    a=ap.parse_args(); croot=Path(a.candidate).resolve(); broot=Path(a.baseline).resolve(); failures=[]; notes=[]
    b=inv(broot); c=inv(croot)
    deleted=sorted(set(b)-set(c)); changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k]); added=sorted(set(c)-set(b))
    allowed={'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json','Assets/LegendsOfRus/Documentation/README_FIRST.md','MILESTONE_VERSION.txt','README.md'}
    for x in changed:
        if x not in allowed: failures.append('unexpected inherited change: '+x)
    if deleted: failures += ['baseline file deleted: '+x for x in deleted]
    for prefix in ['ProjectSettings/','Packages/','Assets/LegendsOfRus/Scripts/SaveCore/']:
        for rel,h in b.items():
            if rel.startswith(prefix) and c.get(rel)!=h: failures.append('protected changed/missing: '+rel)
    for rel,h in b.items():
        if rel.startswith('Assets/LegendsOfRus/Tests/') and c.get(rel)!=h: failures.append('inherited test changed/missing: '+rel)
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
    if 'Array.IndexOf(parameterTypes, typeof(string))' not in read(croot,'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'): failures.append('NUnit Type containment hotfix missing')
    pa=json.loads(read(croot,'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    if 'LegendsOfRus.ContentDeliveryCore' not in pa.get('references',[]): failures.append('ContentDeliveryCore test asmdef hotfix missing')
    ba=json.loads(read(croot,'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef'))
    if 'TestAssemblies' not in ba.get('optionalUnityReferences',[]): failures.append('BalanceReview UnityTest hotfix missing')
    asm=json.loads(read(croot,'Assets/LegendsOfRus/Scripts/CandidateFreezeCore/LegendsOfRus.CandidateFreezeCore.asmdef'))
    if asm.get('noEngineReferences') is not True: failures.append('CandidateFreezeCore not engine-free')
    if asm.get('references')!=['LegendsOfRus.ControlledTestBuildCore']: failures.append('CandidateFreezeCore reference boundary drifted')
    core='\n'.join(p.read_text(encoding='utf-8') for p in (croot/'Assets/LegendsOfRus/Scripts/CandidateFreezeCore').glob('*.cs'))
    for token in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','PlayerPrefs','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass']:
        if token in core: failures.append('forbidden CandidateFreezeCore token: '+token)
    req=read(croot,'Assets/LegendsOfRus/Scripts/CandidateFreezeCore/CandidateFreezeRequirements.cs')
    for token in ['SchemaVersion = 1','RequiredPhysicalProfileCount = 5','MinimumTestDurationMinutes = 5','HashAlgorithm = "SHA-256"','CandidatePrefix = "v1.18-step05-rc"']:
        if token not in req: failures.append('freeze requirement token missing: '+token)
    for profile in ['android_low_16_9_720p','android_mid_18_9_1080p','android_mid_19_5_9_1080p','android_high_20_9_1080p','android_high_20_9_1440p']:
        if profile not in req: failures.append('required physical profile missing: '+profile)
    act=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json'))
    if act.get('localEvidenceValidationApproved') is not True or act.get('localFreezeManifestToolingApproved') is not True: failures.append('local Step5 evidence tooling not approved')
    if act.get('requiredPhysicalProfiles')!=5: failures.append('activation physical profile count is not 5')
    for k in ['unityCompilePassed','editorSelfTestPassed','editModePassed','playModePassed','signedAppBundleEvidenceRetained','physicalDeviceEvidenceRetained','packageLockReviewed','projectSettingsReviewed','storeReleaseChecklistApproved','privacyDataSafetyApproved','productionApplicationIdentifierApproved','versioningApproved','productionSigningApproved','controlledTestBuildAuthorized','candidateFreezeAuthorized','candidateFreezeManifestRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if act.get(k) is not False: failures.append('candidate-freeze gate unexpectedly enabled: '+k)
    runner=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18CandidateFreezeEvidenceRunner.cs')
    for token in ['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','keystorePass','keyaliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel']:
        if token in runner: failures.append('forbidden candidate-freeze runner token: '+token)
    evidence_dir=croot/'ProjectDocs/ClosedTestEvidence'
    aabs=list((evidence_dir/'Artifacts').glob('*.aab')) if (evidence_dir/'Artifacts').exists() else []
    if aabs: failures.append('source candidate must contain zero AAB artifacts')
    for rel in ['signed_aab_evidence.json','physical_device_evidence.json','CANDIDATE_FREEZE_MANIFEST.json','CANDIDATE_FREEZE_MANIFEST.sha256']:
        if (evidence_dir/rel).exists(): failures.append('source candidate unexpectedly contains retained evidence: '+rel)
    for rel in ['signed_aab_evidence.template.json','physical_device_evidence.template.json','README.md','Artifacts/README.md']:
        if not (evidence_dir/rel).exists(): failures.append('evidence staging file missing: '+rel)
    # Existing Step4 / store / commercial / economy gates must remain fail-closed.
    step4=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json'))
    for k in ['productionSigningApproved','controlledTestBuildAuthorized','signedAppBundleEvidenceRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step4.get(k) is not False: failures.append('Step4 build gate unexpectedly enabled: '+k)
    store=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/store_release_activation.json'))
    for k in ['storeListingTextApproved','storeListingAssetsApproved','privacyDataSafetyApproved','productionSigningApproved','appBundleEvidenceRetained','controlledTestBuildAuthorized','storePublishingAuthorized']:
        if store.get(k) is not False: failures.append('store gate unexpectedly enabled: '+k)
    comm=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy activation unexpectedly enabled')
    tests=0
    for p in (croot/'Assets/LegendsOfRus/Tests/CandidateFreezeEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); tests+=t.count('[Test]')+t.count('[UnityTest]')
    checks=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18CandidateFreezeSelfTest.cs').count('Check("')
    if tests!=18: failures.append(f'expected 18 new CandidateFreeze test declarations, found {tests}')
    if checks!=36: failures.append(f'expected 36 Step5 self-test checks, found {checks}')
    packages_lock_present=(croot/'Packages/packages-lock.json').exists()
    if not packages_lock_present: notes.append('Packages/packages-lock.json is not retained in the source package; candidate freeze must remain blocked until Unity resolves and the exact lock is reviewed.')
    report={
      'version':'v1.18-step05','milestone':'controlled-test-artifact-device-evidence-and-candidate-freeze','status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not failures,
      'unityVersion':'6000.5.9f1','unityCompileStatus':'PENDING_USER_UNITY_EXECUTION','editorSelfTestStatus':'PENDING_USER_UNITY_EXECUTION','editModeStatus':'PENDING_USER_UNITY_EXECUTION','playModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'signedAppBundleEvidenceRetained':False,'physicalDeviceEvidenceRetained':False,'requiredPhysicalDeviceProfiles':5,'candidateFreezeAuthorized':False,'candidateFreezeManifestRetained':False,'closedTestUploadAuthorized':False,'storePublishingAuthorized':False,
      'packagesLockPresentInSourceCandidate':packages_lock_present,
      'baselineProjectFiles':len(b),'candidateProjectFiles':len(c),'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':len(cp),'approvedPngsIdenticalToStep04':len(cp)==len(bp) and all(cp.get(k)==v for k,v in bp.items()),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'aabFilesInSourceCandidate':len(aabs),'newCandidateFreezeTestDeclarations':tests,'editorSelfTestCheckSites':checks,
      'automaticBuildInvocationPresent':False,'networkUploadPresent':False,'signingCredentialFieldsPresent':False,'persistentDeviceIdentifiersPresent':False,
      'failures':failures,'notes':notes
    }
    out=Path(a.output) if a.output else croot/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP05.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2)); return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
