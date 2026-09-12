#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, re, sys

ROOT=Path(__file__).resolve().parents[1]
BASE_MANIFEST=Path('/mnt/data/v120_step04_baseline_manifest.json')
MILESTONE='v1.20-step04-functional-game-flow-demo-logic'
ALLOW_CHANGED={
'MILESTONE_VERSION.txt','README.md','Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md','Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenId.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs','Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs','Assets/LegendsOfRus/Scripts/Runtime/Services/AnalyticsService.cs'
}

def sha(p):
 h=hashlib.sha256()
 with open(p,'rb') as f:
  for b in iter(lambda:f.read(1024*1024),b''): h.update(b)
 return h.hexdigest()

def manifest(root):
 out={}
 for p in root.rglob('*'):
  if p.is_file():
   rel=p.relative_to(root).as_posix(); out[rel]=sha(p)
 return out

fail=[]
base_doc=json.loads(BASE_MANIFEST.read_text())
base={x['path']:x['sha256'] for x in base_doc['files']}
cur=manifest(ROOT)
deleted=sorted(set(base)-set(cur))
changed=sorted(p for p in base if p in cur and base[p]!=cur[p])
added=sorted(set(cur)-set(base))
unexpected=[p for p in changed if p not in ALLOW_CHANGED]
if deleted: fail.append('deleted baseline paths: '+str(deleted[:10]))
if unexpected: fail.append('unexpected inherited changes: '+str(unexpected[:20]))
if (ROOT/'MILESTONE_VERSION.txt').read_text().strip()!=MILESTONE: fail.append('milestone mismatch')
# inherited protections
png=[p for p in base if p.lower().endswith('.png')]
if any(base[p]!=cur.get(p) for p in png): fail.append('inherited PNG changed')
packages=[p for p in base if p.startswith('Packages/')]
if any(base[p]!=cur.get(p) for p in packages): fail.append('Packages changed')
settings=[p for p in base if p.startswith('ProjectSettings/')]
if any(base[p]!=cur.get(p) for p in settings): fail.append('ProjectSettings changed')
inherited_tests=[p for p in base if p.startswith('Assets/LegendsOfRus/Tests/')]
if any(base[p]!=cur.get(p) for p in inherited_tests): fail.append('inherited tests changed')
# functionality
router=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
if 'Hotspot(parent, "Back", 0, 1480, 125, 150, NavigateBack)' not in router: fail.append('world map back hotspot missing')
if 'Hotspot(parent, "Back", 0, 1400, 205, 272, NavigateBack)' not in router: fail.append('chapter map back hotspot missing')
if 'ScreenNavigationPolicy.TryGetBackDestination' not in router: fail.append('central back policy missing')
nav=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs').read_text()
if 'case ScreenId.ChapterMap:' not in nav or 'destination = ScreenId.WorldMap' not in nav: fail.append('chapter map parent missing')
boot=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs').read_text()
if 'router.Show(ScreenId.SplashTitle);' not in boot: fail.append('splash boot missing')
# enum append values textual order
sid=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenId.cs').read_text()
for name in ['SplashTitle','EventDetail','IndividualRanking','TeamHub','TeamRanking','TeamChat','FriendsList','Inventory','SpecialOffer','LanguageSelect','Login','TutorialOverlay','OfflineState','MaintenanceState','EmptyErrorState']:
 if name not in sid: fail.append('ScreenId missing '+name)
# plan
plan=json.loads((ROOT/'Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json').read_text())
if len(plan.get('pages',[]))!=36: fail.append('page plan count != 36')
if len({x.get('pageKey') for x in plan.get('pages',[])})!=36: fail.append('page plan keys not unique')
for x in plan.get('pages',[]):
 if not x.get('functionalGoal') or not x.get('assetRequirements'): fail.append('incomplete page plan '+str(x.get('pageKey')))
routes=plan.get('routeGraph',{})
if routes.get('ChapterMap')!=['WorldMap','PreLevel']: fail.append('ChapterMap route graph mismatch')
if routes.get('WorldMap')!=['Home','ChapterMap']: fail.append('WorldMap route graph mismatch')
# demo safety
state=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs').read_text()
for token in ['AddGold(','AddGems(','AddEnergy(','PlayerPrefs','GrantReward','GrantCurrency']:
 if token in state: fail.append('demo state forbidden token '+token)
activation=json.loads((ROOT/'Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json').read_text())
for k in ['productionLeaderboardBackendEnabled','productionSocialBackendEnabled','productionChatBackendEnabled','productionIdentityBackendEnabled','productionLiveOpsRewardAuthorityEnabled','automaticEconomyGrantFromDemoEnabled','automaticProductionPublishEnabled']:
 if activation.get(k) is not False: fail.append('activation must remain false '+k)
for k in ['functionalityFirstDemoEnabled','demoDesignAllowed','mapBackNavigationEnabled','extendedDemoRouteGraphEnabled']:
 if activation.get(k) is not True: fail.append('activation must be true '+k)
# prior save/addressables/hotfix boundaries
pkg=json.loads((ROOT/'Packages/manifest.json').read_text())
if pkg.get('dependencies',{}).get('com.unity.addressables')!='2.8.1': fail.append('Addressables not 2.8.1')
gs=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text()
if 'public int saveVersion = 6;' not in gs: fail.append('save v6 missing')
step18=(ROOT/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs').read_text()
if 'JsonUtility.ToJson(BuildSnapshot(), true) + "\\n"' not in step18: fail.append('Hotfix05 preflight string repair missing')
# Test counts
ft=(ROOT/'Assets/LegendsOfRus/Tests/FunctionalGameFlowEditMode/FunctionalGameFlowTests.cs').read_text()
test_count=len(re.findall(r'\[Test\]',ft)); case_count=len(re.findall(r'\[TestCase\(',ft))
# no AAB
if list(ROOT.rglob('*.aab')): fail.append('AAB bundled')
report={
 'version':'v1.20-step04',
 'status':'PASS_STATIC_SOURCE_VERIFICATION' if not fail else 'FAIL_STATIC_SOURCE_VERIFICATION',
 'candidate_files':len(cur),'baseline_files':len(base),'changed_inherited_paths':changed,'added_paths_count':len(added),'deleted_paths_count':len(deleted),
 'inherited_png_count':len(png),'inherited_pngs_identical':not any(base[p]!=cur.get(p) for p in png),
 'packages_identical':not any(base[p]!=cur.get(p) for p in packages),'projectsettings_identical':not any(base[p]!=cur.get(p) for p in settings),
 'inherited_tests_identical':not any(base[p]!=cur.get(p) for p in inherited_tests),
 'page_function_asset_plan_count':len(plan.get('pages',[])),
 'new_test_declarations':test_count,'new_testcase_declarations':case_count,'editor_self_test_expected':71,
 'map_back_policy':'ChapterMap->WorldMap->Home','production_backends_fail_closed':True,'failures':fail
}
out=ROOT/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP04.json'
out.write_text(json.dumps(report,indent=2))
print(json.dumps(report,indent=2))
sys.exit(1 if fail else 0)
