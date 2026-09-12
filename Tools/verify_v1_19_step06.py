#!/usr/bin/env python3
import argparse, hashlib, json, os
from pathlib import Path
ALLOWED_CHANGED={
 'README.md','MILESTONE_VERSION.txt','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json','Assets/LegendsOfRus/Documentation/README_FIRST.md','Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md'
}
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for chunk in iter(lambda:f.read(1024*1024),b''): h.update(chunk)
 return h.hexdigest()
def inv(root): return {str(p.relative_to(root)).replace(os.sep,'/'):sha(p) for p in root.rglob('*') if p.is_file()}
def main():
 ap=argparse.ArgumentParser();ap.add_argument('candidate');ap.add_argument('--baseline',required=True);a=ap.parse_args();c=Path(a.candidate).resolve();b=Path(a.baseline).resolve();fail=[]
 bi,ci=inv(b),inv(c);changed=sorted(p for p in bi if p in ci and bi[p]!=ci[p]);added=sorted(p for p in ci if p not in bi);deleted=sorted(p for p in bi if p not in ci)
 bad=[p for p in changed if p not in ALLOWED_CHANGED]
 if bad: fail.append('unexpected inherited changes: '+', '.join(bad))
 if deleted: fail.append('baseline files deleted: '+', '.join(deleted))
 for prefix in ('Packages/','ProjectSettings/'):
  d=[p for p in changed if p.startswith(prefix)]
  if d: fail.append(prefix+' changed: '+', '.join(d))
 inherited_tests=[p for p in changed if p.startswith('Assets/LegendsOfRus/Tests/')]
 if inherited_tests: fail.append('inherited tests changed: '+', '.join(inherited_tests))
 bp={p:h for p,h in bi.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')};cp={p:h for p,h in ci.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')}
 if len(cp)!=65: fail.append('approved PNG count != 65: '+str(len(cp)))
 if bp!=cp: fail.append('approved PNG hashes differ from Step 5')
 req=['Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/LegendsOfRus.LiveOpsOutcomeCore.asmdef','Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/LiveOpsOutcomeReviewEvaluator.cs','Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/ExperimentDecisionEvaluator.cs','Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore/RotationCloseoutEvaluator.cs','Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutRunner.cs','Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutSelfTest.cs','Assets/LegendsOfRus/Resources/Config/liveops_outcome_activation.json','Assets/LegendsOfRus/Tests/LiveOpsOutcomeEditMode/LegendsOfRus.LiveOpsOutcomeCore.Tests.asmdef','ProjectDocs/LiveOpsOutcome/outcome_evidence.template.json','ProjectDocs/LiveOpsOutcome/experiment_decision.template.json','ProjectDocs/LiveOpsOutcome/rotation_closeout.template.json','Assets/LegendsOfRus/Documentation/V1_19_STEP06_LIVEOPS_OUTCOME_REVIEW_EXPERIMENT_DECISION_ROTATION_CLOSEOUT.md','Assets/LegendsOfRus/Documentation/V1_19_STEP06_ACTIVATION_EVIDENCE.json','Assets/LegendsOfRus/Documentation/V1_19_STEP06_CLOSEOUT_CHECKLIST.json','README_V1_19_STEP06.md','Tools/verify_v1_19_step06.py']
 for p in req:
  if p not in ci: fail.append('missing required Step 6 file: '+p)
 core=list((c/'Assets/LegendsOfRus/Scripts/LiveOpsOutcomeCore').glob('*.cs'));runner=c/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutRunner.cs';source='\n'.join(p.read_text(errors='replace') for p in core+[runner])
 banned=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','keystorePass','keyaliasPass','serviceAccountKey','playCredential','oauthToken','accessToken','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','PlayerPrefs.Set','RemoteConfigService.Fetch','AnalyticsService.CustomData','RuntimeEconomyTransactionEngine','EconomyLedger','GrantCurrency','GrantReward','SetBalance','PlayerSaveData','Addressables.LoadAssetAsync','Addressables.Release']
 for t in banned:
  if t in source: fail.append('banned action/mutation token: '+t)
 for p in core:
  x=p.read_text(errors='replace')
  if 'using UnityEngine' in x or 'using UnityEditor' in x: fail.append('engine-free core imports Unity: '+p.name)
 act=json.loads((c/'Assets/LegendsOfRus/Resources/Config/liveops_outcome_activation.json').read_text())
 false_keys=['productionAnalyticsProviderEnabled','productionRemoteConfigEnabled','outcomeReviewAuthorized','experimentDecisionAuthorized','rotationCloseoutAuthorized','manualCloseoutReviewAuthorized','automaticContentMutationEnabled','automaticExperimentDecisionApplyEnabled','automaticExperimentArchiveEnabled','automaticRewardMutationEnabled','automaticEconomyMutationEnabled','automaticRemoteConfigMutationEnabled','playConsoleActionEnabled']
 for k in false_keys:
  if act.get(k) is not False: fail.append('activation flag must remain false: '+k)
 true_keys=['localOutcomeReviewToolingApproved','localExperimentDecisionToolingApproved','localRotationCloseoutToolingApproved','localSanitizedSnapshotToolingApproved']
 for k in true_keys:
  if act.get(k) is not True: fail.append('local tooling flag must be true: '+k)
 for name in ['outcome_evidence.json','experiment_decision.json','rotation_closeout.json']:
  if (c/'ProjectDocs/LiveOpsOutcome'/name).exists(): fail.append('real reviewed evidence unexpectedly bundled: '+name)
 if list(c.rglob('*.aab')): fail.append('AAB unexpectedly bundled')
 tdir=c/'Assets/LegendsOfRus/Tests/LiveOpsOutcomeEditMode';tests=sum(p.read_text(errors='replace').count('[Test]') for p in tdir.glob('*.cs'));self_text=(c/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOutcomeExperimentDecisionCloseoutSelfTest.cs').read_text();self_sites=self_text.count('Check(')
 if tests!=80: fail.append('Step 6 test declaration count != 80: '+str(tests))
 if 'total!=89' not in self_text: fail.append('Step 6 runtime self-test expected count is not pinned to 89')
 if self_sites<89: fail.append('Step 6 self-test check sites too small: '+str(self_sites))
 manifest=(c/'Packages/manifest.json').read_text()
 if '"com.unity.addressables": "2.8.1"' not in manifest: fail.append('Addressables 2.8.1 not retained')
 if 'TestAssemblies' not in (c/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef').read_text(): fail.append('Hotfix04 missing')
 if 'Array.IndexOf(parameterTypes, typeof(string))' not in (c/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text(): fail.append('NUnit type hotfix missing')
 if 'LegendsOfRus.ContentDeliveryCore' not in (c/'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef').read_text(): fail.append('ContentDeliveryCore test asmdef hotfix missing')
 if 'public int saveVersion = 6;' not in (c/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text(): fail.append('save v6 boundary changed')
 if (c/'MILESTONE_VERSION.txt').read_text().strip()!='v1.19-step06-liveops-outcome-experiment-decision-rotation-closeout': fail.append('milestone version incorrect')
 jf=[]
 for p in c.rglob('*.json'):
  try: json.loads(p.read_text())
  except Exception as e: jf.append(str(p.relative_to(c))+': '+str(e))
 if jf: fail.append('JSON parse failures: '+' | '.join(jf[:10]))
 result={'version':'v1.19-step06','milestone':'liveops-outcome-experiment-decision-rotation-closeout','status':'PASS_STATIC_SOURCE_VERIFICATION' if not fail else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not fail,'candidate_files':len(ci),'baseline_files':len(bi),'changed_paths':changed,'added_paths':added,'deleted_paths':deleted,'approved_png_files':len(cp),'approved_pngs_identical':bp==cp,'packages_identical':not any(p.startswith('Packages/') for p in changed),'project_settings_identical':not any(p.startswith('ProjectSettings/') for p in changed),'inherited_tests_identical':not inherited_tests,'step6_test_declarations':tests,'step6_self_test_check_sites':self_sites,'step6_runtime_self_test_expected':89,'aab_files_bundled':len(list(c.rglob('*.aab'))),'production_analytics_provider_enabled':act.get('productionAnalyticsProviderEnabled'),'production_remote_config_enabled':act.get('productionRemoteConfigEnabled'),'outcome_review_authorized':act.get('outcomeReviewAuthorized'),'experiment_decision_authorized':act.get('experimentDecisionAuthorized'),'rotation_closeout_authorized':act.get('rotationCloseoutAuthorized'),'manual_closeout_review_authorized':act.get('manualCloseoutReviewAuthorized'),'automatic_content_mutation_enabled':act.get('automaticContentMutationEnabled'),'automatic_experiment_decision_apply_enabled':act.get('automaticExperimentDecisionApplyEnabled'),'automatic_experiment_archive_enabled':act.get('automaticExperimentArchiveEnabled'),'automatic_reward_mutation_enabled':act.get('automaticRewardMutationEnabled'),'automatic_economy_mutation_enabled':act.get('automaticEconomyMutationEnabled'),'automatic_remote_config_mutation_enabled':act.get('automaticRemoteConfigMutationEnabled'),'failures':fail,'notes':['Outcome/experiment-decision/rotation-closeout recommendations are human-review signals only.','No production analytics, Remote Config, live content, experiment traffic, reward/economy, build or Play action is enabled.']}
 print(json.dumps(result,indent=2)); raise SystemExit(0 if not fail else 1)
if __name__=='__main__':main()
