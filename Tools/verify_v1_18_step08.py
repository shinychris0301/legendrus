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
    for rel in ['Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP08.json']:
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

    asm=json.loads(read(croot,'Assets/LegendsOfRus/Scripts/LaunchCandidateCore/LegendsOfRus.LaunchCandidateCore.asmdef'))
    if asm.get('noEngineReferences') is not True: failures.append('LaunchCandidateCore not engine-free')
    if asm.get('references') not in ([],None): failures.append('LaunchCandidateCore should have no assembly references')
    core='\n'.join(p.read_text(encoding='utf-8') for p in (croot/'Assets/LegendsOfRus/Scripts/LaunchCandidateCore').glob('*.cs'))
    runner=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ProductionAccessDecisionLaunchCandidateRunner.cs')
    for token in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','Google.Apis']:
        if token in core: failures.append('forbidden LaunchCandidateCore token: '+token)
    for token in ['UnityWebRequest','HttpClient','BuildPipeline.BuildPlayer','keystorePass','keyaliasPass','keyStorePass','keyAliasPass','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','Google.Apis','PlayDeveloperReporting']:
        if token in runner: failures.append('forbidden Step8 runner token: '+token)

    req=read(croot,'Assets/LegendsOfRus/Scripts/LaunchCandidateCore/LaunchCandidateRequirements.cs')
    for token in ['SchemaVersion = 1','HashAlgorithm = "SHA-256"','CandidatePrefix = "v1.18-step05-rc"','MinimumTargetApiLevel = 36','FirstProductionReleaseRolloutPercent = 100','MinimumUpdateRolloutPercent = 1','MaximumRolloutPercent = 100']:
        if token not in req: failures.append('Step8 requirement token missing: '+token)
    models=read(croot,'Assets/LegendsOfRus/Scripts/LaunchCandidateCore/LaunchCandidateModels.cs')
    for forbidden in ['TesterEmail','testerEmail','TesterName','testerName','GoogleGroup','googleGroup','DeviceId','deviceId','AndroidId','IMEI','AdvertisingId','RawFeedback','rawFeedback','Password','Credential','ServiceAccountKey']:
        if forbidden in models: failures.append('identity/credential Step8 schema field present: '+forbidden)

    act=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/launch_candidate_activation.json'))
    for k in ['localProductionAccessDecisionToolingApproved','localLaunchCandidateReadinessToolingApproved']:
        if act.get(k) is not True: failures.append('local Step8 tooling not approved: '+k)
    false_gates=['developerAccountRuleReviewed','productionAccessRequirementResolved','productionAccessDecisionEvidenceRetained','productionAccessDecisionReviewed','productionAccessDecisionSatisfied','candidateFreezeManifestRetained','candidateFreezeVerified','artifactMatchesFreeze','closedTestExecutionEvidenceVerified','feedbackTriageReviewed','storeReleaseChecklistApproved','privacyAndDataSafetyApproved','legalDocumentsApproved','appContentDeclarationsApproved','productionApplicationIdApproved','versioningApproved','playAppSigningConfigured','uploadKeyWorkflowReviewed','androidTechnicalRequirementsVerified','releaseNotesApproved','productionCountriesRegionsApproved','supportReadinessApproved','launchMonitoringApproved','rollbackPlanApproved','releaseHistoryReviewed','outstandingReleasesCleared','productionReleaseAuthorized','storePublishingAuthorized']
    for k in false_gates:
        if act.get(k) is not False: failures.append('Step8 gate unexpectedly enabled: '+k)
    if act.get('requestedRolloutPercent')!=100: failures.append('first-release source rollout default must be 100 percent')

    evidence=croot/'ProjectDocs/ClosedTestEvidence'
    for rel in ['production_access_decision.template.json','launch_candidate_readiness.template.json','LaunchCandidate/README.md']:
        if not (evidence/rel).exists(): failures.append('Step8 evidence staging file missing: '+rel)
    for rel in ['production_access_decision.json','launch_candidate_readiness.json','STEP08_LAUNCH_CANDIDATE_READINESS_SNAPSHOT.json']:
        if (evidence/rel).exists(): failures.append('source candidate unexpectedly contains retained Step8 evidence: '+rel)
    aabs=list((evidence/'Artifacts').glob('*.aab')) if (evidence/'Artifacts').exists() else []
    if aabs: failures.append('source candidate must contain zero AAB artifacts')
    for rel in ['CANDIDATE_FREEZE_MANIFEST.json','CANDIDATE_FREEZE_MANIFEST.sha256']:
        if (evidence/rel).exists(): failures.append('source candidate unexpectedly contains freeze evidence: '+rel)

    # Prior fail-closed gates retained.
    step7=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_execution_activation.json'))
    for k in ['closedTestExecutionEvidenceRetained','feedbackTriageEvidenceRetained','productionAccessApplicationAuthorized','productionAccessDecisionRetained','productionAccessGranted','productionReleaseReadinessAuthorized','storePublishingAuthorized']:
        if step7.get(k) is not False: failures.append('Step7 gate unexpectedly enabled: '+k)
    step6=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/closed_test_rollout_activation.json'))
    for k in ['closedTestUploadAuthorized','closedTestPublishAuthorized','closedTestGoNoGoAuthorized','developerAccountTypeReviewed','productionAccessGranted','productionRolloutAuthorized','storePublishingAuthorized']:
        if step6.get(k) is not False: failures.append('Step6 gate unexpectedly enabled: '+k)
    step5=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json'))
    for k in ['candidateFreezeAuthorized','candidateFreezeManifestRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step5.get(k) is not False: failures.append('Step5 freeze/upload gate unexpectedly enabled: '+k)
    step4=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json'))
    for k in ['productionSigningApproved','controlledTestBuildAuthorized','signedAppBundleEvidenceRetained','closedTestUploadAuthorized','storePublishingAuthorized']:
        if step4.get(k) is not False: failures.append('Step4 build gate unexpectedly enabled: '+k)
    comm=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(croot,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy activation unexpectedly enabled')

    tests=0
    for p in (croot/'Assets/LegendsOfRus/Tests/LaunchCandidateEditMode').glob('*.cs'):
        t=p.read_text(encoding='utf-8'); tests+=t.count('[Test]')+t.count('[UnityTest]')
    checks=read(croot,'Assets/LegendsOfRus/Scripts/Editor/Step18ProductionAccessDecisionLaunchCandidateSelfTest.cs').count('Check("')
    if tests!=39: failures.append(f'expected 39 new LaunchCandidate test declarations, found {tests}')
    if checks!=70: failures.append(f'expected 70 Step8 self-test checks, found {checks}')

    packages_lock_present=(croot/'Packages/packages-lock.json').exists()
    if not packages_lock_present: notes.append('Packages/packages-lock.json is not retained in the source package; final launch evidence must be tied to the exact Unity-resolved package lock before publication.')
    notes.append('Google Play policy snapshot rechecked 2026-09-09: applicable new personal accounts need required closed testing before production access; staged rollouts are for updates, not first production publication.')

    report={
      'version':'v1.18-step08','milestone':'production-access-decision-launch-candidate-readiness','status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not failures,
      'unityVersion':'6000.5.9f1','unityCompileStatus':'PENDING_USER_UNITY_EXECUTION','editorSelfTestStatus':'PENDING_USER_UNITY_EXECUTION','editModeStatus':'PENDING_USER_UNITY_EXECUTION','playModeStatus':'PENDING_USER_UNITY_EXECUTION',
      'productionAccessDecisionEvidenceRetained':False,'productionAccessDecisionSatisfied':False,'candidateFreezeManifestRetained':False,'launchCandidateEvidenceRetained':False,'productionReleaseAuthorized':False,'storePublishingAuthorized':False,
      'minimumTargetApiLevel':36,'firstProductionReleaseRolloutPercent':100,'packagesLockPresentInSourceCandidate':packages_lock_present,
      'baselineProjectFiles':len(b),'candidateProjectFiles':len(c),'baselineTreeSha256':tree_hash(b),'candidateTreeSha256':tree_hash(c),'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':len(cp),'approvedPngsIdenticalToStep07':len(cp)==len(bp) and all(cp.get(k)==v for k,v in bp.items()),
      'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
      'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
      'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
      'aabFilesInSourceCandidate':len(aabs),'newLaunchCandidateTestDeclarations':tests,'editorSelfTestCheckSites':checks,
      'playApiOrNetworkActionPresent':False,'automaticBuildInvocationPresent':False,'signingCredentialFieldsPresent':False,'testerIdentityPersistencePresent':False,'persistentDeviceIdentifiersPresent':False,
      'failures':failures,'notes':notes
    }
    out=Path(a.output) if a.output else croot/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP08.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2)); return 0 if not failures else 1

if __name__=='__main__': sys.exit(main())
