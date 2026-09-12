#!/usr/bin/env python3
import argparse, hashlib, json, os
from pathlib import Path
ALLOWED_CHANGED={'README.md','MILESTONE_VERSION.txt','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json','Assets/LegendsOfRus/Documentation/README_FIRST.md','Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md'}
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for chunk in iter(lambda:f.read(1024*1024),b''):h.update(chunk)
 return h.hexdigest()
def inv(root):return {str(p.relative_to(root)).replace(os.sep,'/'):sha(p) for p in root.rglob('*') if p.is_file()}
def main():
 ap=argparse.ArgumentParser();ap.add_argument('candidate');ap.add_argument('--baseline',required=True);a=ap.parse_args();c=Path(a.candidate).resolve();b=Path(a.baseline).resolve();fail=[]
 bi,ci=inv(b),inv(c);changed=sorted(p for p in bi if p in ci and bi[p]!=ci[p]);added=sorted(p for p in ci if p not in bi);deleted=sorted(p for p in bi if p not in ci)
 bad=[p for p in changed if p not in ALLOWED_CHANGED]
 if bad:fail.append('unexpected inherited changes: '+', '.join(bad))
 if deleted:fail.append('baseline files deleted: '+', '.join(deleted))
 for prefix in ('Packages/','ProjectSettings/'):
  d=[p for p in changed if p.startswith(prefix)]
  if d:fail.append(prefix+' changed: '+', '.join(d))
 inherited_tests=[p for p in changed if p.startswith('Assets/LegendsOfRus/Tests/')]
 if inherited_tests:fail.append('inherited tests changed: '+', '.join(inherited_tests))
 bp={p:h for p,h in bi.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')};cp={p:h for p,h in ci.items() if p.startswith('Assets/LegendsOfRus/') and p.lower().endswith('.png')}
 if len(cp)!=65:fail.append('approved PNG count != 65: '+str(len(cp)))
 if bp!=cp:fail.append('approved PNG hashes differ from Step 6')
 req=['Assets/LegendsOfRus/Scripts/LiveOpsCycleCore/LegendsOfRus.LiveOpsCycleCore.asmdef','Assets/LegendsOfRus/Scripts/LiveOpsCycleCore/LiveOpsCycleEvaluators.cs','Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceRunner.cs','Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceSelfTest.cs','Assets/LegendsOfRus/Resources/Config/liveops_cycle_activation.json','Assets/LegendsOfRus/Tests/LiveOpsCycleEditMode/LegendsOfRus.LiveOpsCycleCore.Tests.asmdef','ProjectDocs/LiveOpsCycle/cycle_retrospective.template.json','ProjectDocs/LiveOpsCycle/cycle_archive.template.json','ProjectDocs/LiveOpsCycle/next_cadence_plan.template.json','Assets/LegendsOfRus/Documentation/V1_19_STEP07_LIVEOPS_CYCLE_RETROSPECTIVE_ARCHIVE_NEXT_CADENCE.md','Assets/LegendsOfRus/Documentation/V1_19_STEP07_ACTIVATION_EVIDENCE.json','Assets/LegendsOfRus/Documentation/V1_19_STEP07_CYCLE_CLOSEOUT_CHECKLIST.json','README_V1_19_STEP07.md','Tools/verify_v1_19_step07.py']
 for p in req:
  if p not in ci:fail.append('missing required Step 7 file: '+p)
 core=list((c/'Assets/LegendsOfRus/Scripts/LiveOpsCycleCore').glob('*.cs'));runner=c/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceRunner.cs';source='\n'.join(p.read_text(errors='replace') for p in core+[runner])
 banned=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','keystorePass','keyaliasPass','serviceAccountKey','playCredential','oauthToken','accessToken','deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','PlayerPrefs.Set','RemoteConfigService.Fetch','AnalyticsService.CustomData','RuntimeEconomyTransactionEngine','EconomyLedger','GrantCurrency','GrantReward','SetBalance','PlayerSaveData','Addressables.LoadAssetAsync','Addressables.Release']
 for t in banned:
  if t in source:fail.append('banned action/mutation token: '+t)
 for p in core:
  x=p.read_text(errors='replace')
  if 'using UnityEngine' in x or 'using UnityEditor' in x:fail.append('engine-free core imports Unity: '+p.name)
 act=json.loads((c/'Assets/LegendsOfRus/Resources/Config/liveops_cycle_activation.json').read_text())
 false_keys=['productionAnalyticsProviderEnabled','productionRemoteConfigEnabled','retrospectiveReviewAuthorized','archiveReviewAuthorized','nextCadenceReviewAuthorized','manualCycleClosureAuthorized','liveOpsPublishingAuthorized','automaticContentMutationEnabled','automaticArchiveUploadEnabled','automaticExperimentAllocationEnabled','automaticRewardMutationEnabled','automaticEconomyMutationEnabled','automaticRemoteConfigMutationEnabled','playConsoleActionEnabled']
 for k in false_keys:
  if act.get(k) is not False:fail.append('activation flag must remain false: '+k)
 true_keys=['localRetrospectiveToolingApproved','localArchiveValidationToolingApproved','localNextCadencePlanningToolingApproved','localSanitizedSnapshotToolingApproved']
 for k in true_keys:
  if act.get(k) is not True:fail.append('local tooling flag must be true: '+k)
 for name in ['cycle_retrospective.json','cycle_archive.json','next_cadence_plan.json']:
  if (c/'ProjectDocs/LiveOpsCycle'/name).exists():fail.append('real reviewed evidence unexpectedly bundled: '+name)
 if list(c.rglob('*.aab')):fail.append('AAB unexpectedly bundled')
 tdir=c/'Assets/LegendsOfRus/Tests/LiveOpsCycleEditMode';tests=sum(p.read_text(errors='replace').count('[Test]') for p in tdir.glob('*.cs'));self_text=(c/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsCycleRetrospectiveArchiveNextCadenceSelfTest.cs').read_text();self_sites=self_text.count('Check(')
 EXPECT_TESTS=78;EXPECT_SELF=75
 if tests!=EXPECT_TESTS:fail.append('Step 7 test declaration count != %d: %d'%(EXPECT_TESTS,tests))
 if ('total!='+str(EXPECT_SELF)) not in self_text:fail.append('Step 7 runtime self-test expected count is not pinned to '+str(EXPECT_SELF))
 if self_sites<EXPECT_SELF:fail.append('Step 7 self-test check sites too small: '+str(self_sites))
 if '"com.unity.addressables": "2.8.1"' not in (c/'Packages/manifest.json').read_text():fail.append('Addressables 2.8.1 not retained')
 if 'TestAssemblies' not in (c/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef').read_text():fail.append('Hotfix04 missing')
 if 'Array.IndexOf(parameterTypes, typeof(string))' not in (c/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text():fail.append('NUnit type hotfix missing')
 if 'LegendsOfRus.ContentDeliveryCore' not in (c/'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef').read_text():fail.append('ContentDeliveryCore test asmdef hotfix missing')
 if 'public int saveVersion = 6;' not in (c/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text():fail.append('save v6 boundary changed')
 if (c/'MILESTONE_VERSION.txt').read_text().strip()!='v1.19-step07-liveops-cycle-retrospective-archive-next-cadence':fail.append('milestone version incorrect')
 jf=[]
 for p in c.rglob('*.json'):
  try:json.loads(p.read_text())
  except Exception as e:jf.append(str(p.relative_to(c))+': '+str(e))
 if jf:fail.append('JSON parse failures: '+' | '.join(jf[:10]))
 result={'version':'v1.19-step07','milestone':'liveops-cycle-retrospective-archive-next-cadence','status':'PASS_STATIC_SOURCE_VERIFICATION' if not fail else 'FAIL_STATIC_SOURCE_VERIFICATION','passed':not fail,'candidate_files':len(ci),'baseline_files':len(bi),'changed_paths':changed,'added_paths':added,'deleted_paths':deleted,'approved_png_files':len(cp),'approved_pngs_identical':bp==cp,'packages_identical':not any(p.startswith('Packages/') for p in changed),'project_settings_identical':not any(p.startswith('ProjectSettings/') for p in changed),'inherited_tests_identical':not inherited_tests,'step7_test_declarations':tests,'step7_self_test_check_sites':self_sites,'step7_runtime_self_test_expected':EXPECT_SELF,'aab_files_bundled':len(list(c.rglob('*.aab'))),'production_analytics_provider_enabled':act.get('productionAnalyticsProviderEnabled'),'production_remote_config_enabled':act.get('productionRemoteConfigEnabled'),'retrospective_review_authorized':act.get('retrospectiveReviewAuthorized'),'archive_review_authorized':act.get('archiveReviewAuthorized'),'next_cadence_review_authorized':act.get('nextCadenceReviewAuthorized'),'manual_cycle_closure_authorized':act.get('manualCycleClosureAuthorized'),'liveops_publishing_authorized':act.get('liveOpsPublishingAuthorized'),'automatic_archive_upload_enabled':act.get('automaticArchiveUploadEnabled'),'automatic_content_mutation_enabled':act.get('automaticContentMutationEnabled'),'failures':fail,'notes':['Retrospective/archive/next-cadence recommendations are human-review signals only.','No production analytics, Remote Config, archive upload, LiveOps publication, automatic mutation, build, credential, network or Play action is enabled.']}
 print(json.dumps(result,indent=2));raise SystemExit(0 if not fail else 1)
if __name__=='__main__':main()
