#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, os, re, sys
root=Path(__file__).resolve().parents[1]
baseline_env=os.environ.get('LOR_STEP07_BASELINE','').strip()
baseline=Path(baseline_env) if baseline_env else None
fail=[]

def hfile(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for c in iter(lambda:f.read(1024*1024),b''):h.update(c)
 return h.hexdigest()
def inv(r): return {str(p.relative_to(r)).replace(os.sep,'/'):hfile(p) for p in r.rglob('*') if p.is_file() and not any(x in p.parts for x in ('Library','Temp','Logs','UserSettings'))}
def boolcfg(k):
 data=json.loads((root/'Assets/LegendsOfRus/Resources/Config/liveops_baseline_activation.json').read_text())
 return data.get(k)
ci=inv(root)
allowed_changed={
'MILESTONE_VERSION.txt','README.md','Assets/LegendsOfRus/Documentation/README_FIRST.md','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json'}
changed=[];added=[];deleted=[];basecount=None
packages_identical=project_settings_identical=inherited_tests_identical=approved_pngs_identical=None
if baseline and baseline.exists():
 bi=inv(baseline);basecount=len(bi)
 changed=sorted(p for p in bi if p in ci and bi[p]!=ci[p]);added=sorted(p for p in ci if p not in bi);deleted=sorted(p for p in bi if p not in ci)
 illegal=[p for p in changed if p not in allowed_changed]
 if illegal: fail.append('illegal inherited changes: '+','.join(illegal))
 if deleted: fail.append('deleted baseline files: '+','.join(deleted))
 packages_identical=all(ci.get(p)==v for p,v in bi.items() if p.startswith('Packages/'))
 project_settings_identical=all(ci.get(p)==v for p,v in bi.items() if p.startswith('ProjectSettings/'))
 inherited_tests_identical=all(ci.get(p)==v for p,v in bi.items() if '/Tests/' in ('/'+p) and p in ci)
 pngs=[p for p in bi if p.lower().endswith('.png') and p.startswith('Assets/LegendsOfRus/')]
 approved_pngs_identical=all(ci.get(p)==bi[p] for p in pngs)
 if not packages_identical: fail.append('Packages changed')
 if not project_settings_identical: fail.append('ProjectSettings changed')
 if not inherited_tests_identical: fail.append('inherited tests changed')
 if not approved_pngs_identical: fail.append('approved PNG changed')
else:
 pngs=[p for p in ci if p.lower().endswith('.png') and p.startswith('Assets/LegendsOfRus/')]

required=['Assets/LegendsOfRus/Scripts/LiveOpsBaselineCore/LegendsOfRus.LiveOpsBaselineCore.asmdef','Assets/LegendsOfRus/Scripts/LiveOpsBaselineCore/LiveOpsBaselineEvaluators.cs','Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOperatingBaselineTransitionSelfTest.cs','Assets/LegendsOfRus/Tests/LiveOpsBaselineEditMode/LiveOpsBaselineCoreTests.cs','ProjectDocs/LiveOpsBaseline/v119_evidence_chain.template.json','ProjectDocs/LiveOpsBaseline/liveops_operating_baseline.template.json','ProjectDocs/LiveOpsBaseline/v120_transition_plan.template.json','README_V1_19_STEP08.md']
for p in required:
 if not (root/p).exists(): fail.append('missing '+p)
config=json.loads((root/'Assets/LegendsOfRus/Resources/Config/liveops_baseline_activation.json').read_text())
for k in ['productionAnalyticsProviderEnabled','productionRemoteConfigEnabled','evidenceChainReviewAuthorized','operatingBaselineReviewAuthorized','v120TransitionReviewAuthorized','manualV119ClosureAuthorized','v120PreparationAuthorized','liveOpsPublishingAuthorized','automaticContentMutationEnabled','automaticArchiveUploadEnabled','automaticExperimentAllocationEnabled','automaticRewardMutationEnabled','automaticEconomyMutationEnabled','automaticRemoteConfigMutationEnabled','playConsoleActionEnabled']:
 if config.get(k) is not False: fail.append('gate not false '+k)
core=''.join(p.read_text(errors='ignore') for p in (root/'Assets/LegendsOfRus/Scripts/LiveOpsBaselineCore').glob('*.cs'))
runner=(root/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOperatingBaselineTransitionRunner.cs').read_text(errors='ignore')
joined=core+runner
for token in ['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','keystorePass','keyaliasPass','serviceAccountKey','deviceUniqueIdentifier','advertisingIdentifier','PlayerPrefs.Set','RuntimeEconomyTransactionEngine','GrantCurrency','GrantReward']:
 if token in joined: fail.append('banned token '+token)
if 'using UnityEngine' in core or 'using UnityEditor' in core: fail.append('core is not engine-free')
if '"com.unity.addressables": "2.8.1"' not in (root/'Packages/manifest.json').read_text(): fail.append('Addressables pin missing')
if 'public int saveVersion = 6;' not in (root/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text(): fail.append('save v6 missing')
if (root/'MILESTONE_VERSION.txt').read_text().strip()!='v1.19-step08-liveops-operating-baseline-v1.20-transition-review': fail.append('milestone mismatch')
tests_text=(root/'Assets/LegendsOfRus/Tests/LiveOpsBaselineEditMode/LiveOpsBaselineCoreTests.cs').read_text()
test_count=len(re.findall(r'\[Test\]',tests_text))
self_text=(root/'Assets/LegendsOfRus/Scripts/Editor/Step19LiveOpsOperatingBaselineTransitionSelfTest.cs').read_text()
m=re.search(r'if\(total!=(\d+)\)',self_text); self_expected=int(m.group(1)) if m else None
report={'version':'v1.19-step08','passed':not fail,'failures':fail,'candidate_files':len(ci),'baseline_files':basecount,'changed_paths':changed,'added_paths':added,'deleted_paths':deleted,'approved_png_files':len(pngs),'approved_pngs_identical':approved_pngs_identical,'packages_identical':packages_identical,'project_settings_identical':project_settings_identical,'inherited_tests_identical':inherited_tests_identical,'step8_test_declarations':test_count,'step8_runtime_self_test_expected':self_expected,'aab_files_bundled':sum(1 for p in ci if p.lower().endswith('.aab')),'production_analytics_provider_enabled':config.get('productionAnalyticsProviderEnabled'),'production_remote_config_enabled':config.get('productionRemoteConfigEnabled'),'manual_v119_closure_authorized':config.get('manualV119ClosureAuthorized'),'v120_preparation_authorized':config.get('v120PreparationAuthorized'),'liveops_publishing_authorized':config.get('liveOpsPublishingAuthorized')}
print(json.dumps(report,indent=2))
sys.exit(0 if not fail else 1)
