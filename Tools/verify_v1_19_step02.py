#!/usr/bin/env python3
import argparse, hashlib, json, os
from pathlib import Path

ALLOWED_CHANGED = {
    'README.md', 'MILESTONE_VERSION.txt',
    'Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_19.json',
    'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
    'Assets/LegendsOfRus/Documentation/README_FIRST.md',
    'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
    'Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_19_STEP02.json',
}

def sha(p):
    h=hashlib.sha256()
    with p.open('rb') as f:
        for chunk in iter(lambda:f.read(1024*1024), b''): h.update(chunk)
    return h.hexdigest()

def inventory(root):
    return {str(p.relative_to(root)).replace(os.sep,'/'):sha(p) for p in root.rglob('*') if p.is_file()}

def main():
    ap=argparse.ArgumentParser(); ap.add_argument('candidate'); ap.add_argument('--baseline', required=True); args=ap.parse_args()
    c=Path(args.candidate).resolve(); b=Path(args.baseline).resolve()
    failures=[]; notes=[]
    bi=inventory(b); ci=inventory(c)
    changed=sorted(p for p in bi if p in ci and bi[p]!=ci[p])
    added=sorted(p for p in ci if p not in bi)
    deleted=sorted(p for p in bi if p not in ci)
    bad_changed=[p for p in changed if p not in ALLOWED_CHANGED]
    if bad_changed: failures.append('unexpected inherited changes: '+', '.join(bad_changed))
    if deleted: failures.append('baseline files deleted: '+', '.join(deleted))

    # Protected project/package boundaries.
    for prefix in ('Packages/','ProjectSettings/'):
        diffs=[p for p in changed if p.startswith(prefix)]
        if diffs: failures.append(prefix+' changed: '+', '.join(diffs))
    inherited_tests=[p for p in changed if p.startswith('Assets/LegendsOfRus/Tests/')]
    if inherited_tests: failures.append('inherited tests changed: '+', '.join(inherited_tests))

    base_png={p:h for p,h in bi.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')}
    cand_png={p:h for p,h in ci.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')}
    if len(cand_png)!=65: failures.append('approved PNG count != 65: '+str(len(cand_png)))
    if base_png!=cand_png: failures.append('approved PNG inventory/hash differs from Step 1')

    req=[
      'Assets/LegendsOfRus/Scripts/IncidentResponseCore/LegendsOfRus.IncidentResponseCore.asmdef',
      'Assets/LegendsOfRus/Scripts/IncidentResponseCore/IncidentTriageEvaluator.cs',
      'Assets/LegendsOfRus/Scripts/IncidentResponseCore/HotfixQualificationEvaluator.cs',
      'Assets/LegendsOfRus/Scripts/IncidentResponseCore/RolloutControlReviewEvaluator.cs',
      'Assets/LegendsOfRus/Scripts/Editor/Step19IncidentResponseHotfixRolloutControlRunner.cs',
      'Assets/LegendsOfRus/Scripts/Editor/Step19IncidentResponseHotfixRolloutControlSelfTest.cs',
      'Assets/LegendsOfRus/Resources/Config/incident_response_activation.json',
      'Assets/LegendsOfRus/Tests/IncidentResponseEditMode/LegendsOfRus.IncidentResponseCore.Tests.asmdef',
      'ProjectDocs/IncidentResponse/incident_register.template.json',
      'ProjectDocs/IncidentResponse/hotfix_candidate_evidence.template.json',
      'ProjectDocs/IncidentResponse/rollout_control_review.template.json',
    ]
    for p in req:
        if p not in ci: failures.append('missing required Step 2 file: '+p)

    # Core is engine free and no action APIs exist in operational implementation.
    core_files=list((c/'Assets/LegendsOfRus/Scripts/IncidentResponseCore').glob('*.cs'))
    runner=c/'Assets/LegendsOfRus/Scripts/Editor/Step19IncidentResponseHotfixRolloutControlRunner.cs'
    source='\n'.join(p.read_text(errors='replace') for p in core_files+[runner])
    banned=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','PlayDeveloperReporting','keystorePass','keyaliasPass','keyStorePass','keyAliasPass','serviceAccountKey','playCredential','oauthToken','accessToken','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','AndroidId','IMEI','TesterEmail','testerEmail','TesterName','testerName','GoogleGroup','googleGroup']
    for token in banned:
        if token in source: failures.append('banned action/identity token in Step 2 implementation: '+token)
    for p in core_files:
        txt=p.read_text(errors='replace')
        if 'using UnityEngine' in txt or 'using UnityEditor' in txt: failures.append('engine-free core imports Unity: '+p.name)

    activation=json.loads((c/'Assets/LegendsOfRus/Resources/Config/incident_response_activation.json').read_text())
    false_flags=['incidentRegisterRetained','incidentRegisterReviewed','hotfixCandidateEvidenceRetained','hotfixQualificationReviewed','rolloutControlEvidenceRetained','rolloutControlEvidenceReviewed','manualHotfixReleaseAuthorized','manualRolloutActionAuthorized','automaticPlayActionsEnabled','automaticBuildEnabled','automaticSigningEnabled','playPublishingApiEnabled','playConsoleCredentialAccessEnabled','productionTelemetryMutationEnabled']
    for k in false_flags:
        if activation.get(k) is not False: failures.append('activation flag must remain false: '+k)
    for k in ['localIncidentReviewToolingApproved','localHotfixQualificationToolingApproved','localRolloutControlReviewToolingApproved']:
        if activation.get(k) is not True: failures.append('local tooling flag must be true: '+k)

    # No real evidence or artifacts included.
    for name in ['incident_register.json','hotfix_candidate_evidence.json','rollout_control_review.json']:
        if (c/'ProjectDocs/IncidentResponse'/name).exists(): failures.append('real evidence file unexpectedly bundled: '+name)
    aabs=list(c.rglob('*.aab'))
    if aabs: failures.append('AAB files unexpectedly bundled')

    # Tests and self-test inventory.
    test_dir=c/'Assets/LegendsOfRus/Tests/IncidentResponseEditMode'
    test_count=sum(p.read_text(errors='replace').count('[Test]') for p in test_dir.glob('*.cs'))
    self_text=(c/'Assets/LegendsOfRus/Scripts/Editor/Step19IncidentResponseHotfixRolloutControlSelfTest.cs').read_text()
    self_checks=self_text.count('Check("')
    if test_count!=66: failures.append('Step 2 [Test] count != 66: '+str(test_count))
    if self_checks!=95: failures.append('Step 2 self-test check count != 95: '+str(self_checks))

    # Retained compatibility fixes and key boundaries.
    manifest=(c/'Packages/manifest.json').read_text()
    if '"com.unity.addressables": "2.8.1"' not in manifest: failures.append('Addressables 2.8.1 not retained')
    if 'TestAssemblies' not in (c/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef').read_text(): failures.append('Hotfix04 missing')
    if 'Array.IndexOf(parameterTypes, typeof(string))' not in (c/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text(): failures.append('NUnit Type hotfix missing')
    if 'LegendsOfRus.ContentDeliveryCore' not in (c/'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef').read_text(): failures.append('ContentDeliveryCore test asmdef hotfix missing')
    if 'public int saveVersion = 6;' not in (c/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text(): failures.append('save v6 boundary changed')
    if (c/'MILESTONE_VERSION.txt').read_text().strip()!='v1.19-step02-incident-response-hotfix-rollout-control-review': failures.append('milestone version incorrect')

    # All JSON must parse.
    json_fail=[]
    for p in c.rglob('*.json'):
        try: json.loads(p.read_text())
        except Exception as e: json_fail.append(str(p.relative_to(c))+': '+str(e))
    if json_fail: failures.append('JSON parse failures: '+' | '.join(json_fail[:10]))

    result={
      'version':'v1.19-step02',
      'milestone':'incident-response-hotfix-and-rollout-control-review',
      'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
      'passed':not failures,
      'candidate_files':len(ci), 'baseline_files':len(bi),
      'changed_paths':changed, 'added_paths':added, 'deleted_paths':deleted,
      'approved_png_files':len(cand_png), 'approved_pngs_identical':base_png==cand_png,
      'packages_identical':not any(p.startswith('Packages/') for p in changed),
      'project_settings_identical':not any(p.startswith('ProjectSettings/') for p in changed),
      'inherited_tests_identical':not inherited_tests,
      'step2_test_declarations':test_count, 'step2_self_test_checks':self_checks,
      'aab_files_bundled':len(aabs),
      'automatic_play_actions_authorized':activation.get('automaticPlayActionsEnabled'),
      'manual_hotfix_release_authorized':activation.get('manualHotfixReleaseAuthorized'),
      'manual_rollout_action_authorized':activation.get('manualRolloutActionAuthorized'),
      'failures':failures, 'notes':notes
    }
    print(json.dumps(result,indent=2))
    raise SystemExit(0 if not failures else 1)
if __name__=='__main__': main()
