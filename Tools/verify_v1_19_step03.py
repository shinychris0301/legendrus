#!/usr/bin/env python3
import argparse, hashlib, json, os
from pathlib import Path
ALLOWED_CHANGED={
 'README.md','MILESTONE_VERSION.txt','Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_19.json','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json','Assets/LegendsOfRus/Documentation/README_FIRST.md','Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md','Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_19_STEP03.json'
}
def sha(p):
 h=hashlib.sha256();
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
 if bp!=cp: fail.append('approved PNG hashes differ from Step 2')
 req=['Assets/LegendsOfRus/Scripts/PostLaunchReviewCore/LegendsOfRus.PostLaunchReviewCore.asmdef','Assets/LegendsOfRus/Scripts/PostLaunchReviewCore/PostLaunchReviewEvaluator.cs','Assets/LegendsOfRus/Scripts/PostLaunchReviewCore/TrendRetentionReviewEvaluator.cs','Assets/LegendsOfRus/Scripts/PostLaunchReviewCore/EconomyGuardrailEvaluator.cs','Assets/LegendsOfRus/Scripts/Editor/Step19PostLaunchTrendRetentionEconomyRunner.cs','Assets/LegendsOfRus/Scripts/Editor/Step19PostLaunchTrendRetentionEconomySelfTest.cs','Assets/LegendsOfRus/Resources/Config/post_launch_review_activation.json','Assets/LegendsOfRus/Tests/PostLaunchReviewEditMode/LegendsOfRus.PostLaunchReviewCore.Tests.asmdef','ProjectDocs/PostLaunchReview/guardrail_profile.template.json','ProjectDocs/PostLaunchReview/trend_window.template.json','ProjectDocs/PostLaunchReview/retention_cohort.template.json','ProjectDocs/PostLaunchReview/economy_flow.template.json']
 for p in req:
  if p not in ci: fail.append('missing required Step 3 file: '+p)
 core=list((c/'Assets/LegendsOfRus/Scripts/PostLaunchReviewCore').glob('*.cs'));runner=c/'Assets/LegendsOfRus/Scripts/Editor/Step19PostLaunchTrendRetentionEconomyRunner.cs';source='\n'.join(p.read_text(errors='replace') for p in core+[runner])
 banned=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','keystorePass','keyaliasPass','serviceAccountKey','playCredential','oauthToken','accessToken','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','PlayerSaveData','EconomyLedger','RuntimeEconomyTransactionEngine','GrantCurrency','GrantReward','SetBalance']
 for t in banned:
  if t in source: fail.append('banned action/mutation token: '+t)
 for p in core:
  x=p.read_text(errors='replace')
  if 'using UnityEngine' in x or 'using UnityEditor' in x: fail.append('engine-free core imports Unity: '+p.name)
 act=json.loads((c/'Assets/LegendsOfRus/Resources/Config/post_launch_review_activation.json').read_text())
 for k in ['guardrailProfileRetained','guardrailProfileReviewed','trendWindowEvidenceRetained','trendWindowEvidenceReviewed','retentionCohortEvidenceRetained','retentionCohortEvidenceReviewed','economyFlowEvidenceRetained','economyFlowEvidenceReviewed','manualEconomyChangeReviewAuthorized','automaticEconomyMutationEnabled','automaticRewardMutationEnabled','automaticLevelBalanceMutationEnabled','productionTelemetryUploadEnabled','productionAnalyticsProviderEnabled','productionMonetizationMutationEnabled','playConsoleActionEnabled']:
  if act.get(k) is not False: fail.append('activation flag must remain false: '+k)
 for k in ['localTrendRetentionReviewToolingApproved','localEconomyGuardrailReviewToolingApproved','localSanitizedSnapshotToolingApproved']:
  if act.get(k) is not True: fail.append('local tooling flag must be true: '+k)
 for name in ['guardrail_profile.json','trend_window.json','retention_cohort.json','economy_flow.json']:
  if (c/'ProjectDocs/PostLaunchReview'/name).exists(): fail.append('real evidence unexpectedly bundled: '+name)
 if list(c.rglob('*.aab')): fail.append('AAB unexpectedly bundled')
 tdir=c/'Assets/LegendsOfRus/Tests/PostLaunchReviewEditMode';tests=sum(p.read_text(errors='replace').count('[Test]') for p in tdir.glob('*.cs'));self_sites=(c/'Assets/LegendsOfRus/Scripts/Editor/Step19PostLaunchTrendRetentionEconomySelfTest.cs').read_text().count('Check(')
 if tests<75: fail.append('Step 3 test declaration count too small: '+str(tests))
 if self_sites<35: fail.append('Step 3 self-test check sites too small: '+str(self_sites))
 manifest=(c/'Packages/manifest.json').read_text()
 if '"com.unity.addressables": "2.8.1"' not in manifest: fail.append('Addressables 2.8.1 not retained')
 if 'TestAssemblies' not in (c/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef').read_text(): fail.append('Hotfix04 missing')
 if 'Array.IndexOf(parameterTypes, typeof(string))' not in (c/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text(): fail.append('NUnit type hotfix missing')
 if 'LegendsOfRus.ContentDeliveryCore' not in (c/'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef').read_text(): fail.append('ContentDeliveryCore test asmdef hotfix missing')
 if 'public int saveVersion = 6;' not in (c/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text(): fail.append('save v6 boundary changed')
 if (c/'MILESTONE_VERSION.txt').read_text().strip()!='v1.19-step03-post-launch-trend-retention-economy-guardrail-review': fail.append('milestone version incorrect')
 jf=[]
 for p in c.rglob('*.json'):
  try: json.loads(p.read_text())
  except Exception as e: jf.append(str(p.relative_to(c))+': '+str(e))
 if jf: fail.append('JSON parse failures: '+' | '.join(jf[:10]))
 result={'version':'v1.19-step03','milestone':'post-launch-trend-retention-economy-guardrail-review','status':'PASS_STATIC_SOURCE_VERIFICATION' if not fail else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not fail,'candidate_files':len(ci),'baseline_files':len(bi),'changed_paths':changed,'added_paths':added,'deleted_paths':deleted,'approved_png_files':len(cp),'approved_pngs_identical':bp==cp,'packages_identical':not any(p.startswith('Packages/') for p in changed),'project_settings_identical':not any(p.startswith('ProjectSettings/') for p in changed),'inherited_tests_identical':not inherited_tests,'step3_test_declarations':tests,'step3_self_test_check_sites':self_sites,'aab_files_bundled':len(list(c.rglob('*.aab'))),'automatic_economy_mutation_authorized':act.get('automaticEconomyMutationEnabled'),'manual_economy_change_review_authorized':act.get('manualEconomyChangeReviewAuthorized'),'production_telemetry_upload_enabled':act.get('productionTelemetryUploadEnabled'),'failures':fail,'notes':['Guardrail thresholds are project-specific reviewed evidence, not universal industry benchmarks.']}
 print(json.dumps(result,indent=2)); raise SystemExit(0 if not fail else 1)
if __name__=='__main__':main()
