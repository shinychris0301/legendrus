#!/usr/bin/env python3
import argparse
import hashlib
import json
from pathlib import Path
import sys


def sha(path: Path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def inventory(root: Path):
    return {p.relative_to(root).as_posix(): sha(p) for p in root.rglob('*') if p.is_file()}


def read(root: Path, rel: str):
    return (root / rel).read_text(encoding='utf-8')


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--candidate', default=str(Path(__file__).resolve().parents[1]))
    parser.add_argument('--baseline', required=True)
    parser.add_argument('--output')
    args = parser.parse_args()
    candidate = Path(args.candidate).resolve()
    baseline = Path(args.baseline).resolve()

    failures=[]
    notes=[]
    b=inventory(baseline); c=inventory(candidate)
    deleted=sorted(set(b)-set(c))
    changed=sorted(k for k in set(b)&set(c) if b[k]!=c[k])
    added=sorted(set(c)-set(b))

    # Required exact inherited protection.
    def identical_prefix(prefix):
        for rel,h in b.items():
            if rel.startswith(prefix) and c.get(rel)!=h:
                failures.append('protected changed/missing: '+rel)

    identical_prefix('ProjectSettings/')
    identical_prefix('Packages/')
    identical_prefix('Assets/LegendsOfRus/Scripts/SaveCore/')
    identical_prefix('Assets/LegendsOfRus/Tests/')

    # New tests are allowed, but inherited test bytes must be identical.
    failures[:] = [f for f in failures if not (
        f.startswith('protected changed/missing: Assets/LegendsOfRus/Tests/BalanceReviewEditMode/') or
        f.startswith('protected changed/missing: Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/'))]

    # Approved art must remain byte-identical.
    baseline_png={k:v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    candidate_png={k:v for k,v in c.items() if k.startswith('Assets/LegendsOfRus/') and k.endswith('.png')}
    if len(baseline_png)!=65 or len(candidate_png)!=65:
        failures.append(f'approved PNG count mismatch baseline={len(baseline_png)} candidate={len(candidate_png)}')
    for rel,h in baseline_png.items():
        if candidate_png.get(rel)!=h:
            failures.append('approved PNG changed/missing: '+rel)

    if deleted:
        failures.extend('baseline file deleted: '+x for x in deleted)

    # JSON parse inventory.
    json_files=list(candidate.rglob('*.json'))
    for p in json_files:
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: failures.append(f'json parse failure {p.relative_to(candidate)}: {e}')

    manifest=json.loads(read(candidate,'Packages/manifest.json'))
    if manifest.get('dependencies',{}).get('com.unity.addressables')!='2.8.1':
        failures.append('Addressables is not pinned to 2.8.1')

    runtime_asm=json.loads(read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef'))
    if 'LegendsOfRus.BalanceReviewCore' not in runtime_asm.get('references',[]):
        failures.append('Runtime asmdef missing direct BalanceReviewCore reference')

    core_asm=json.loads(read(candidate,'Assets/LegendsOfRus/Scripts/BalanceReviewCore/LegendsOfRus.BalanceReviewCore.asmdef'))
    if core_asm.get('noEngineReferences') is not True or core_asm.get('references')!=[]:
        failures.append('BalanceReviewCore is not engine-free')

    core_sources='\n'.join(p.read_text(encoding='utf-8') for p in (candidate/'Assets/LegendsOfRus/Scripts/BalanceReviewCore').glob('*.cs'))
    for forbidden in ['using UnityEngine','UnityEditor','UnityWebRequest','HttpClient','PlayerPrefs','deviceUniqueIdentifier','advertisingIdentifier']:
        if forbidden in core_sources:
            failures.append('forbidden core API: '+forbidden)

    service_source=read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/Services/BalanceTelemetryReviewService.cs')
    activation_source=read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/Services/BalanceTelemetryReviewActivation.cs')
    combined=service_source+'\n'+activation_source
    for forbidden in ['UnityWebRequest','HttpClient','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','SystemInfo.deviceName','File.WriteAllText','PlayerPrefs.SetString']:
        if forbidden in combined:
            failures.append('forbidden review runtime API: '+forbidden)

    activation=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/balance_telemetry_review_activation.json'))
    expected={
        'localObservationBufferApproved':True,
        'localReviewReportApproved':True,
        'remoteTelemetryUploadApproved':False,
        'productionAnalyticsBridgeApproved':False,
        'automaticBalanceMutationApproved':False,
        'tutorialExperimentMutationApproved':False,
    }
    for k,v in expected.items():
        if activation.get(k)!=v: failures.append(f'activation mismatch {k}: {activation.get(k)!r}')

    bootstrap=read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs')
    if 'FindOrAdd<BalanceTelemetryReviewService>(root);' not in bootstrap:
        failures.append('balance review service not bootstrap-wired')
    router=read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs')
    for token in ['TryRecordLevelStart','TryRecordLevelWin','TryRecordLevelFail']:
        if token not in router: failures.append('gameplay review wiring missing '+token)
    tutorial=read(candidate,'Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs')
    for token in ['TryRecordTutorialShown','TryRecordTutorialCompleted']:
        if token not in tutorial: failures.append('tutorial review wiring missing '+token)

    qa=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json'))
    for k in ['remoteQaUploadApproved','productionQaProviderApproved','productionSigningApproved','controlledTestBuildAuthorized','storePublishingAuthorized']:
        if qa.get(k) is not False: failures.append('Step1 QA/release gate unexpectedly enabled: '+k)
    comm=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if comm.get(k) is not False: failures.append('commercial gate unexpectedly enabled: '+k)
    econ=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    if econ.get('runtimeActivationApproved') is not False: failures.append('economy runtime activation unexpectedly enabled')
    content=json.loads(read(candidate,'Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json'))
    for k,v in content.items():
        if k.lower().startswith('remote') and v is True:
            failures.append('remote content gate unexpectedly enabled: '+k)

    if 'Array.IndexOf(parameterTypes, typeof(string))' not in read(candidate,'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'):
        failures.append('Step4 NUnit Type hotfix missing')
    play_asm=json.loads(read(candidate,'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    if 'LegendsOfRus.ContentDeliveryCore' not in play_asm.get('references',[]):
        failures.append('Step5 ContentDeliveryCore PlayMode asmdef hotfix missing')

    test_decls=0
    for p in list((candidate/'Assets/LegendsOfRus/Tests/BalanceReviewEditMode').glob('*.cs'))+list((candidate/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode').glob('*.cs')):
        text=p.read_text(encoding='utf-8')
        test_decls += text.count('[Test]')+text.count('[UnityTest]')
    self_test_checks=read(candidate,'Assets/LegendsOfRus/Scripts/Editor/Step18BalanceTelemetryTutorialFunnelReviewSelfTest.cs').count('Check("')

    report={
        'version':'v1.18-step02',
        'milestone':'balance-telemetry-tutorial-funnel-review-foundation',
        'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
        'passed':not failures,
        'unityVersion':'6000.5.9f1',
        'unityExecutionEvidence':'NOT_RETAINED_FOR_EXACT_STEP02_BYTES',
        'unityCompileStatus':'PENDING_USER_UNITY_EXECUTION',
        'editModeStatus':'PENDING_USER_UNITY_EXECUTION',
        'playModeStatus':'PENDING_USER_UNITY_EXECUTION',
        'representativeClosedTestReviewEvidence':'NOT_RETAINED',
        'productionAnalyticsUploadEnabled':False,
        'automaticBalanceMutationEnabled':False,
        'tutorialExperimentMutationEnabled':False,
        'productionSigningEnabled':False,
        'controlledTestBuildAuthorized':False,
        'storePublishingAuthorized':False,
        'baselineProjectFiles':len(b),
        'candidateProjectFiles':len(c),
        'changedInheritedPaths':changed,
        'addedPaths':added,
        'deletedPaths':deleted,
        'approvedPngFiles':len(candidate_png),
        'approvedPngsIdenticalToStep01':all(candidate_png.get(k)==v for k,v in baseline_png.items()) and len(candidate_png)==len(baseline_png),
        'projectSettingsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('ProjectSettings/')),
        'packagesIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Packages/')),
        'inheritedTestsIdentical':all(c.get(k)==v for k,v in b.items() if k.startswith('Assets/LegendsOfRus/Tests/')),
        'addressablesVersion':'2.8.1',
        'balanceReviewDefaultCapacity':128,
        'balanceReviewHardMaxCapacity':256,
        'newBalanceReviewTestDeclarations':test_decls,
        'editorSelfTestCheckSites':self_test_checks,
        'jsonFilesParsed':len(json_files),
        'failures':failures,
        'notes':notes,
    }
    out=Path(args.output) if args.output else candidate/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP02.json'
    out.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,indent=2))
    return 0 if not failures else 1

if __name__=='__main__':
    sys.exit(main())
