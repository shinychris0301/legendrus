#!/usr/bin/env python3
import argparse, hashlib, json, sys
from pathlib import Path

def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
def inv(root): return {p.relative_to(root).as_posix(): sha(p) for p in root.rglob('*') if p.is_file()}
def read(root, rel): return (root/rel).read_text(encoding='utf-8')
def tree_hash(invmap):
    h=hashlib.sha256()
    for rel,digest in sorted(invmap.items()): h.update((rel+'\t'+digest+'\n').encode())
    return h.hexdigest()

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--candidate', default=str(Path(__file__).resolve().parents[1]))
    ap.add_argument('--baseline', required=True)
    ap.add_argument('--output')
    a=ap.parse_args(); croot=Path(a.candidate).resolve(); broot=Path(a.baseline).resolve(); failures=[]; notes=[]
    b=inv(broot); c=inv(croot)
    b.pop('Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP06.json', None)
    c.pop('Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP06.json', None)
    deleted=sorted(set(b)-set(c)); changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k]); added=sorted(set(c)-set(b))
    allowed={
      'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
      'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
      'Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_18_STEP06.json',
      'Assets/LegendsOfRus/Documentation/README_FIRST.md',
      'MILESTONE_VERSION.txt','README.md'
    }
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
    asm=json.loads(read(croot,'Assets/LegendsOfRus/Scripts/ClosedTestRolloutCore/LegendsOfRus.ClosedTestRolloutCore.asmdef'))
    if asm.get('noEngineReferences') is not True: failures.append('ClosedTestRolloutCore not engine-free')
    if asm.get('references') not in ([],None): failures.append('ClosedTestRolloutCore should have no assembly references')
    core='\n'.join(p.read_text(encoding='utf-8') for p in (croot/'Assets/LegendsOfRus/Scripts/ClosedTestRolloutCore').glob('*.cs'))
    runner=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ClosedTestRolloutReadinessRunner.cs')
    for token in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel']:
        if token in core: failures.append('forbidden ClosedTestRolloutCore token: '+token)
    for token in ['UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','Google.Apis','PlayDeveloperReporting']:
        if token in runner: failures.append('forbidden Step6 runner token: '+token)
    req=read(croot,'Assets/LegendsOfRus/Scripts/ClosedTestRolloutCore/ClosedTestRolloutRequirements.cs')
    for token in ['SchemaVersion = 1','PersonalAccountProductionAccessTesterCount = 12','PersonalAccountProductionAccessContinuousDays = 14','FirstProductionReleasePercent = 100','HashAlgorithm = "SHA-256"','CandidatePrefix = "v1.18-step05-rc"']:
        if token not in req: failures.append('rollout requirement token missing: '+token)
    models=read(croot,'Assets/LegendsOfRus/Scripts/ClosedTestRolloutCore/ClosedTestRolloutModels.cs')
    for forbidden in ['TesterEmail','testerEmail','TesterName','testerName','GoogleGroup','googleGroup','DeviceId','deviceId','AndroidId','IMEI','AdvertisingId']:
        if forbidden in models: failures.append('identity-bearing Step6 schema field present: '+forbidden)
    act=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_rollout_activation.json'))
    for k in ['localClosedTestReadinessToolingApproved','localGoNoGoToolingApproved','localRolloutReadinessToolingApproved']:
        if act.get(k) is not True: failures.append('local Step6 tooling not approved: '+k)
    false_gates=['candidateFreezeManifestRetained','candidateFreezeManifestVerified','artifactHashMatchesFreeze','playAppSetupComplete','testingTrackReleasePermissionApproved','closedTestTrackConfigured','testerAccessConfigured','countryAvailabilityReviewed','releaseNotesApproved','noOutstandingRelease','closedTestUploadAuthorized','closedTestPublishAuthorized','closedTestUploadEvidenceRetained','closedTestHealthEvidenceRetained','closedTestGoNoGoAuthorized','developerAccountTypeReviewed','productionAccessGranted','storeReleaseChecklistApproved','privacyDataSafetyApproved','contentRatingApproved','targetAudienceApproved','adsDeclarationApproved','appAccessDeclarationApproved','rollbackPlanReviewed','monitoringPlanReviewed','managedPublishingReviewed','productionRolloutAuthorized','storePublishingAuthorized']
    for k in false_gates:
        if act.get(k) is not False: failures.append('Step6 gate unexpectedly enabled: '+k)
    if act.get('isFirstProductionRelease') is not True or act.get('requestedRolloutPercent')!=100: failures.append('shipped production rollout proposal must remain first-release/100-percent placeholder')
    evidence=croot/'ProjectDocs/ClosedTestEvidence'
    for rel in ['closed_test_upload_evidence.template.json','closed_test_health_evidence.template.json','production_rollout_readiness.template.json','GoNoGo/README.md']:
        if not (evidence/rel).exists(): failures.append('Step6 evidence staging file missing: '+rel)
    for rel in ['closed_test_upload_evidence.json','closed_test_health_evidence.json','STEP06_ROLLOUT_READINESS_SNAPSHOT.json']:
        if (evidence/rel).exists(): failures.append('source candidate unexpectedly contains retained Step6 evidence: '+rel)
    aabs=list((evidence/'Artifacts').glob('*.aab')) if (evidence/'Artifacts').exists() else []
    if aabs: failures.append('source candidate must contain zero AAB artifacts')
    for rel in ['CANDIDATE_FREEZE_MANIFEST.json','CANDIDATE_FREEZE_MANIFEST.sha256']:
        if (evidence/rel).exists(): failures.append('source candidate unexpectedly contains freeze evidence: '+rel)
    # Prior fail-closed gates retained.
    step5=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json'))
    for k in ['candidateFreezeAuthorized','candidateFreezeManifestRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step5.get(k) is not False: failures.append('Step5 freeze/upload gate unexpectedly enabled: '+k)
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
    for p in (croot/'Assets/LegendsOfRus/Tests/ClosedTestRolloutEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); tests+=t.count('[Test]')+t.count('[UnityTest]')
    checks=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ClosedTestRolloutReadinessSelfTest.cs').count('Check("')
    if tests!=32: failures.append(f'expected 32 new ClosedTestRollout test declarations, found {tests}')
    if checks!=42: failures.append(f'expected 42 Step6 self-test checks, found {checks}')
    packages_lock_present=(croot/'Packages/packages-lock.json').exists()
    if not packages_lock_present: notes.append('Packages/packages-lock.json is not retained in the source package; actual freeze/upload readiness remains blocked until Unity resolves and the exact lock is reviewed.')
    notes.append('Google Play policy snapshot encoded for 2026-09-09: conditional 12 continuous testers / 14 days for applicable new personal accounts; staged rollout is not allowed for a first production release.')
    report={
      'version':'v1.18-step06','milestone':'closed-test-upload-go-no-go-and-rollout-readiness','status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not failures,
      'unityVersion':'6000.5.9f1','unityCompileStatus':'PENDING_USER_UNITY_EXECUTION','editorSelfTestStatus':'PENDING_USER_UNITY_EXECUTION','editModeStatus':'PENDING_USER_UNITY_EXECUTION','playModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'closedTestUploadEvidenceRetained':False,'closedTestHealthEvidenceRetained':False,'closedTestUploadAuthorized':False,'closedTestPublishAuthorized':False,'closedTestGoNoGoAuthorized':False,'developerAccountTypeReviewed':False,'productionAccessGranted':False,'productionRolloutAuthorized':False,'storePublishingAuthorized':False,
      'personalAccountRuleTesterCount':12,'personalAccountRuleContinuousDays':14,'firstProductionReleaseRequires100Percent':True,
      'packagesLockPresentInSourceCandidate':packages_lock_present,
      'baselineProjectFiles':len(b),'candidateProjectFiles':len(c),'baselineTreeSha256':tree_hash(b),'candidateTreeSha256':tree_hash(c),'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':len(cp),'approvedPngsIdenticalToStep05':len(cp)==len(bp) and all(cp.get(k)==v for k,v in bp.items()),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'aabFilesInSourceCandidate':len(aabs),'newClosedTestRolloutTestDeclarations':tests,'editorSelfTestCheckSites':checks,
      'playApiOrNetworkActionPresent':False,'automaticBuildInvocationPresent':False,'signingCredentialFieldsPresent':False,'testerIdentityPersistencePresent':False,'persistentDeviceIdentifiersPresent':False,
      'failures':failures,'notes':notes
    }
    out=Path(a.output) if a.output else croot/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP06.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2)); return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
