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
    for rel in ['Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP08.json','Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_19_STEP01.json']:
        b.pop(rel,None); c.pop(rel,None)
    deleted=sorted(set(b)-set(c)); changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k]); added=sorted(set(c)-set(b))
    allowed={
      'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
      'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
      'Assets/LegendsOfRus/Documentation/README_FIRST.md',
      'MILESTONE_VERSION.txt','README.md'
    }
    for x in changed:
        if x not in allowed: failures.append('unexpected inherited change: '+x)
    if deleted: failures += ['baseline file deleted: '+x for x in deleted]

    for prefix in ['ProjectSettings/','Packages/','Assets/LegendsOfRus/Scripts/SaveCore/']:
        for rel,h in b.items():
            if rel.startswith(prefix) and c.get(rel)!=h: failures.append('protected changed/missing: '+rel)
    for rel in ['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs','Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs']:
        if b.get(rel)!=c.get(rel): failures.append('save/migration boundary changed: '+rel)
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

    asm=json.loads(read(croot,'Assets/LegendsOfRus/Scripts/LaunchOperationsCore/LegendsOfRus.LaunchOperationsCore.asmdef'))
    if asm.get('noEngineReferences') is not True: failures.append('LaunchOperationsCore not engine-free')
    if asm.get('references') not in ([],None): failures.append('LaunchOperationsCore should have no assembly references')
    if asm.get('autoReferenced') is not True: failures.append('LaunchOperationsCore must be auto-referenced for Editor runner visibility')
    testasm=json.loads(read(croot,'Assets/LegendsOfRus/Tests/LaunchOperationsEditMode/LegendsOfRus.LaunchOperationsCore.Tests.asmdef'))
    if 'LegendsOfRus.LaunchOperationsCore' not in testasm.get('references',[]): failures.append('LaunchOperations tests do not reference core')
    if 'TestAssemblies' not in testasm.get('optionalUnityReferences',[]): failures.append('LaunchOperations tests missing TestAssemblies')
    if 'nunit.framework.dll' not in testasm.get('precompiledReferences',[]): failures.append('LaunchOperations tests missing nunit.framework.dll')

    core='\n'.join(p.read_text(encoding='utf-8') for p in (croot/'Assets/LegendsOfRus/Scripts/LaunchOperationsCore').glob('*.cs'))
    runner=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step19ProductionLaunchOperationsRunner.cs')
    for token in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','Google.Apis','PlayDeveloperReporting']:
        if token in core: failures.append('forbidden LaunchOperationsCore token: '+token)
    for token in ['UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','keyStorePass','keyAliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','Google.Apis','PlayDeveloperReporting']:
        if token in runner: failures.append('forbidden Step19 runner token: '+token)

    req=read(croot,'Assets/LegendsOfRus/Scripts/LaunchOperationsCore/LaunchOperationsRequirements.cs')
    required_tokens=[
      'SchemaVersion = 1','HashAlgorithm = "SHA-256"','CandidatePrefix = "v1.18-step05-rc"',
      'FirstProductionReleaseRolloutPercent = 100','MaximumVitalsWindowDays = 28','ObservationBufferCapacity = 64',
      'UserPerceivedCrashOverallBadBehaviorPercent = 1.09','UserPerceivedAnrOverallBadBehaviorPercent = 0.47',
      'UserPerceivedCrashPhoneModelBadBehaviorPercent = 8.0','UserPerceivedAnrPhoneModelBadBehaviorPercent = 8.0',
      'ExcessivePartialWakeLockOverallBadBehaviorPercent = 5.0','ExcessiveBatteryUsageOverallBadBehaviorPercent = 1.0','WatchBandRatio = 0.75'
    ]
    for token in required_tokens:
        if token not in req: failures.append('Step19 requirement token missing: '+token)

    models=read(croot,'Assets/LegendsOfRus/Scripts/LaunchOperationsCore/LaunchOperationsModels.cs')
    for forbidden in ['TesterEmail','testerEmail','TesterName','testerName','GoogleGroup','googleGroup','DeviceId','deviceId','AndroidId','IMEI','AdvertisingId','RawFeedback','rawFeedback','Password','Credential','ServiceAccountKey','DeviceModel','deviceModel']:
        if forbidden in models: failures.append('identity/credential Step19 schema field present: '+forbidden)

    act=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/launch_operations_activation.json'))
    for k in ['localLaunchOperationsToolingApproved','localPostLaunchObservabilityToolingApproved']:
        if act.get(k) is not True: failures.append('local Step19 tooling not approved: '+k)
    false_gates=['launchCandidateReadinessVerified','productionReleaseAuthorized','storePublishingAuthorized','monitoringPlanApproved','incidentResponsePlanApproved','rollbackPlanApproved','supportCoverageApproved','releaseHistoryReviewed','outstandingReleasesCleared','manualLaunchStartAuthorized','launchStarted','releaseAvailableToUsers','androidVitalsEvidenceRetained','androidVitalsEvidenceReviewed','incidentRegisterRetained','incidentRegisterReviewed','launchWindowReviewRetained','manualRolloutActionAuthorized','automaticRolloutActionEnabled','productionTelemetryProviderEnabled','playDeveloperReportingApiEnabled','storePublishingActionEnabled']
    for k in false_gates:
        if act.get(k) is not False: failures.append('Step19 gate unexpectedly enabled: '+k)
    if act.get('requestedRolloutPercent')!=100: failures.append('first-release source rollout default must be 100 percent')
    if act.get('artifactSha256')!='': failures.append('source candidate must not fabricate production artifact hash')

    evidence=croot/'ProjectDocs/LaunchOperations'
    for rel in ['android_vitals_snapshot.template.json','launch_incident_summary.template.json','launch_window_review.template.json','README.md']:
        if not (evidence/rel).exists(): failures.append('Step19 evidence staging file missing: '+rel)
    for rel in ['android_vitals_snapshot.json','launch_incident_summary.json','launch_window_review.json','STEP01_SANITIZED_LAUNCH_OPERATIONS_SNAPSHOT.json']:
        if (evidence/rel).exists(): failures.append('source candidate unexpectedly contains retained Step19 evidence: '+rel)

    # All prior production-sensitive gates remain false in the source candidate.
    step8=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/launch_candidate_activation.json'))
    for k in ['productionAccessDecisionSatisfied','candidateFreezeVerified','artifactMatchesFreeze','closedTestExecutionEvidenceVerified','feedbackTriageReviewed','storeReleaseChecklistApproved','productionReleaseAuthorized','storePublishingAuthorized']:
        if step8.get(k) is not False: failures.append('Step8 gate unexpectedly enabled: '+k)
    step7=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_execution_activation.json'))
    for k in ['closedTestExecutionEvidenceRetained','feedbackTriageEvidenceRetained','productionAccessApplicationAuthorized','productionAccessDecisionRetained','productionAccessGranted','productionReleaseReadinessAuthorized','storePublishingAuthorized']:
        if step7.get(k) is not False: failures.append('Step7 gate unexpectedly enabled: '+k)
    step6=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_rollout_activation.json'))
    for k in ['closedTestUploadAuthorized','closedTestPublishAuthorized','closedTestGoNoGoAuthorized','productionAccessGranted','productionRolloutAuthorized','storePublishingAuthorized']:
        if step6.get(k) is not False: failures.append('Step6 gate unexpectedly enabled: '+k)
    step5=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json'))
    for k in ['candidateFreezeAuthorized','candidateFreezeManifestRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step5.get(k) is not False: failures.append('Step5 gate unexpectedly enabled: '+k)
    step4=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json'))
    for k in ['productionSigningApproved','controlledTestBuildAuthorized','signedAppBundleEvidenceRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step4.get(k) is not False: failures.append('Step4 build gate unexpectedly enabled: '+k)
    comm=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy activation unexpectedly enabled')

    tests=0
    for p in (croot/'Assets/LegendsOfRus/Tests/LaunchOperationsEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); tests+=t.count('[Test]')+t.count('[UnityTest]')
    checks=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step19ProductionLaunchOperationsSelfTest.cs').count('Check("')
    if tests!=40: failures.append(f'expected 40 new LaunchOperations test declarations, found {tests}')
    if checks!=88: failures.append(f'expected 88 Step19 self-test checks, found {checks}')

    aabs=list(croot.rglob('*.aab'))
    if aabs: failures.append('source candidate must contain zero AAB artifacts')
    packages_lock_present=(croot/'Packages/packages-lock.json').exists()
    if not packages_lock_present: notes.append('Packages/packages-lock.json is not retained in the source package; real production evidence must use the exact Unity-resolved lock.')
    notes.append('Google Play Android-vitals policy snapshot rechecked 2026-09-09: crash 1.09% overall, ANR 0.47% overall, 8% per-phone-model crash/ANR; thresholds are review signals only.')
    notes.append('Google Play staged rollouts remain update-only; first production publication remains 100% in the local policy.')

    report={
      'version':'v1.19-step01','milestone':'production-launch-operations-post-launch-observability-foundation',
      'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not failures,
      'unityVersion':'6000.5.9f1','unityCompileStatus':'PENDING_USER_UNITY_EXECUTION','editorSelfTestStatus':'PENDING_USER_UNITY_EXECUTION','editModeStatus':'PENDING_USER_UNITY_EXECUTION','playModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'launchCandidateReadinessVerified':False,'productionReleaseAuthorized':False,'storePublishingAuthorized':False,'realAndroidVitalsEvidenceRetained':False,'realIncidentEvidenceRetained':False,
      'automaticPlayActionPresent':False,'networkTelemetryProviderPresent':False,'playDeveloperReportingApiPresent':False,
      'androidVitalsThresholds':{'crashOverallPercent':1.09,'anrOverallPercent':0.47,'crashPerPhoneModelPercent':8.0,'anrPerPhoneModelPercent':8.0,'partialWakeLockOverallPercent':5.0,'excessiveBatteryOverallPercent':1.0},
      'baselineProjectFiles':len(b),'candidateProjectFiles':len(c),'baselineTreeSha256':tree_hash(b),'candidateTreeSha256':tree_hash(c),'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':len(cp),'approvedPngsIdenticalToStep08':len(cp)==len(bp) and all(cp.get(k)==v for k,v in bp.items()),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'aabFilesInSourceCandidate':len(aabs),'newLaunchOperationsTestDeclarations':tests,'editorSelfTestCheckSites':checks,'observationBufferCapacity':64,
      'packagesLockPresentInSourceCandidate':packages_lock_present,'failures':failures,'notes':notes
    }
    out=Path(a.output) if a.output else croot/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_19_STEP01.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2)); return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
