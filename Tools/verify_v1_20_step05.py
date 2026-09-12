#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, re, sys

ROOT = Path(__file__).resolve().parents[1]
BASELINE = ROOT / 'Assets/LegendsOfRus/Documentation/V1_20_STEP05_BASELINE_STEP04_SHA256.json'
MILESTONE = 'v1.20-step05-functional-qa-full-demo-verification'

ALLOW_CHANGED = {
    'MILESTONE_VERSION.txt',
    'README.md',
    'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
    'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs',
    'Assets/LegendsOfRus/Tests/FunctionalGameFlowEditMode/FunctionalGameFlowTests.cs',
}

EXPECTED_ADDED = {
    'README_V1_20_STEP05.md',
    'README_HOTFIX_V1_20_STEP05.md',
    'Tools/verify_v1_20_step05.py',
    'Assets/LegendsOfRus/Documentation/V1_20_STEP05_BASELINE_STEP04_SHA256.json',
    'Assets/LegendsOfRus/Documentation/V1_20_STEP05_FUNCTIONAL_QA_MATRIX.json',
    'Assets/LegendsOfRus/Documentation/V1_20_STEP05_ASSET_PLAN_AUDIT.json',
    'Assets/LegendsOfRus/Documentation/V1_20_STEP05_FULL_DEMO_FLOW_CHECKLIST.md',
    'Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json',
    'Assets/LegendsOfRus/Scripts/Editor/Step20FunctionalQaSelfTest.cs',
    'Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs',
}


def sha(path):
    h = hashlib.sha256()
    with open(path, 'rb') as f:
        for chunk in iter(lambda: f.read(1024 * 1024), b''):
            h.update(chunk)
    return h.hexdigest()


def manifest(root):
    out = {}
    for p in root.rglob('*'):
        if p.is_file():
            rel = p.relative_to(root).as_posix()
            if rel == 'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP05.json':
                continue
            if rel == 'Assets/LegendsOfRus/Documentation/V1_20_STEP05_CHANGED_FILES.txt':
                continue
            out[rel] = sha(p)
    return out


fail = []
if not BASELINE.exists():
    print('missing baseline manifest')
    sys.exit(1)
base_doc = json.loads(BASELINE.read_text())
base = {x['path']: x['sha256'] for x in base_doc['files']}
cur = manifest(ROOT)

deleted = sorted(set(base) - set(cur))
changed = sorted(p for p in base if p in cur and base[p] != cur[p])
added = sorted(set(cur) - set(base))
unexpected_changed = sorted(p for p in changed if p not in ALLOW_CHANGED)
unexpected_added = sorted(p for p in added if p not in EXPECTED_ADDED)
missing_added = sorted(EXPECTED_ADDED - set(added))

if deleted:
    fail.append('deleted baseline paths: ' + str(deleted[:20]))
if unexpected_changed:
    fail.append('unexpected inherited changes: ' + str(unexpected_changed[:30]))
if unexpected_added:
    fail.append('unexpected added paths: ' + str(unexpected_added[:30]))
if missing_added:
    fail.append('missing expected additions: ' + str(missing_added))

if (ROOT / 'MILESTONE_VERSION.txt').read_text().strip() != MILESTONE:
    fail.append('milestone mismatch')

png = [p for p in base if p.lower().endswith('.png')]
packages = [p for p in base if p.startswith('Packages/')]
settings = [p for p in base if p.startswith('ProjectSettings/')]
if any(base[p] != cur.get(p) for p in png):
    fail.append('inherited PNG changed')
if any(base[p] != cur.get(p) for p in packages):
    fail.append('Packages changed')
if any(base[p] != cur.get(p) for p in settings):
    fail.append('ProjectSettings changed')

policy = (ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs').read_text()
if not re.search(r'case ScreenId\.WorldMap:\s*destination = ScreenId\.Home;', policy):
    fail.append('WorldMap -> Home policy missing')
if not re.search(r'case ScreenId\.ChapterMap:\s*destination = ScreenId\.WorldMap;', policy):
    fail.append('ChapterMap -> WorldMap policy missing')
if not re.search(r'case ScreenId\.TeamRanking:\s*case ScreenId\.TeamChat:\s*case ScreenId\.FriendsList:\s*destination = ScreenId\.TeamHub;', policy):
    fail.append('FriendsList -> TeamHub policy missing')

router = (ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
for token in [
    'void NavigateBackFrom(ScreenId expectedSource)',
    '() => NavigateBackFrom(ScreenId.WorldMap)',
    '() => NavigateBackFrom(ScreenId.ChapterMap)',
    '() => NavigateBackFrom(ScreenId.PreLevel)',
    'ScreenNavigationPolicy.TryGetBackDestination(current, out destination)',
]:
    if token not in router:
        fail.append('router token missing: ' + token)
if not re.search(r'"Back",\s*0,\s*1450,\s*210,\s*205,\s*\(\) => NavigateBackFrom\(ScreenId\.WorldMap\)', router, re.S):
    fail.append('WorldMap expanded Back hotspot mismatch')

edit_test = (ROOT / 'Assets/LegendsOfRus/Tests/FunctionalGameFlowEditMode/FunctionalGameFlowTests.cs').read_text()
if '[TestCase(ScreenId.FriendsList, ScreenId.TeamHub)]' not in edit_test:
    fail.append('FriendsList EditMode parent test missing')

play_path = ROOT / 'Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs'
if not play_path.exists():
    fail.append('Step 5 PlayMode test missing')
    play_test = ''
else:
    play_test = play_path.read_text()
for token in [
    'MapBackHotspots_FollowChapterMapWorldMapHomeHierarchy',
    'PreLevelVisibleBackHotspot_ReturnsToChapterMap',
    'ExtendedDemoRoutes_RenderWithAtLeastOneInteraction',
    'NavigateBack_FriendsListReturnsToTeamHub',
    'Hotspot_Back',
]:
    if token not in play_test:
        fail.append('PlayMode coverage missing: ' + token)

selftest = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/Step20FunctionalQaSelfTest.cs'
if not selftest.exists() or 'Run v1.20 Step 5 Functional QA Self-Tests' not in selftest.read_text():
    fail.append('Step 5 Editor self-test missing')

plan_path = ROOT / 'Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json'
plan = json.loads(plan_path.read_text())
pages = plan.get('pages', [])
if len(pages) != 36:
    fail.append('page plan count != 36')
if len({x.get('pageKey') for x in pages}) != 36:
    fail.append('page plan keys not unique')
if any(not x.get('functionalGoal') or not x.get('assetRequirements') for x in pages):
    fail.append('page plan has incomplete functionalGoal/assetRequirements')
asset_entries = sum(len(x.get('assetRequirements', [])) for x in pages)
if asset_entries != 254:
    fail.append('asset requirement count changed from audited 254')

route_graph = plan.get('routeGraph', {})
if route_graph.get('WorldMap') != ['Home', 'ChapterMap']:
    fail.append('WorldMap route graph mismatch')
if route_graph.get('ChapterMap') != ['WorldMap', 'PreLevel']:
    fail.append('ChapterMap route graph mismatch')
if route_graph.get('FriendsList') != ['TeamHub']:
    fail.append('FriendsList route graph mismatch')

acceptance = json.loads((ROOT / 'Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json').read_text())
if acceptance.get('screenCount') != 36 or acceptance.get('reachableFromSplashCount') != 36:
    fail.append('Step 5 route acceptance does not cover all 36 screens')
if acceptance.get('routes', {}).get('IndividualRanking') != ['Home']:
    fail.append('IndividualRanking -> Home acceptance route missing')
if acceptance.get('allScreensReachableFromSplash') is not True:
    fail.append('not all Step 5 acceptance screens are reachable from SplashTitle')

matrix = json.loads((ROOT / 'Assets/LegendsOfRus/Documentation/V1_20_STEP05_FUNCTIONAL_QA_MATRIX.json').read_text())
if matrix.get('assetPlanAudit', {}).get('result') != 'PASS_36_OF_36':
    fail.append('Step 5 asset plan audit not PASS')
if matrix.get('unityExecution') != 'PENDING_USER_UNITY_EXECUTION':
    fail.append('Unity execution must remain pending in static package')

state = (ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs').read_text()
for token in ['AddGold(', 'AddGems(', 'AddEnergy(', 'PlayerPrefs', 'GrantReward', 'GrantCurrency']:
    if token in state:
        fail.append('demo state forbidden token ' + token)

activation = json.loads((ROOT / 'Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json').read_text())
for key in [
    'productionLeaderboardBackendEnabled', 'productionSocialBackendEnabled',
    'productionChatBackendEnabled', 'productionIdentityBackendEnabled',
    'productionLiveOpsRewardAuthorityEnabled', 'automaticEconomyGrantFromDemoEnabled',
    'automaticProductionPublishEnabled'
]:
    if activation.get(key) is not False:
        fail.append('production/demo safety gate must remain false: ' + key)

pkg = json.loads((ROOT / 'Packages/manifest.json').read_text())
if pkg.get('dependencies', {}).get('com.unity.addressables') != '2.8.1':
    fail.append('Addressables not 2.8.1')
gs = (ROOT / 'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text()
if 'public int saveVersion = 6;' not in gs:
    fail.append('save v6 missing')
if list(ROOT.rglob('*.aab')):
    fail.append('AAB bundled')

report = {
    'milestone': MILESTONE,
    'status': 'PASS_STATIC_SOURCE_AND_PACKAGE_VERIFICATION' if not fail else 'FAIL_STATIC_SOURCE_AND_PACKAGE_VERIFICATION',
    'baselineMilestone': base_doc.get('milestone'),
    'candidateFiles': len(cur),
    'baselineFiles': len(base),
    'changedInheritedPaths': changed,
    'addedPaths': added,
    'deletedPathsCount': len(deleted),
    'inheritedPngCount': len(png),
    'inheritedPngsIdentical': not any(base[p] != cur.get(p) for p in png),
    'packagesIdentical': not any(base[p] != cur.get(p) for p in packages),
    'projectSettingsIdentical': not any(base[p] != cur.get(p) for p in settings),
    'mapBackPolicy': 'ChapterMap -> WorldMap -> Home',
    'worldMapVisibleBackTouchTarget': '210x205 reference units',
    'friendsListBackPolicy': 'FriendsList -> TeamHub',
    'pageFunctionAssetPlanCount': len(pages),
    'assetRequirementEntries': asset_entries,
    'step05PlayModeUnityTestsAdded': len(re.findall(r'\[UnityTest\]', play_test)),
    'productionBackendsFailClosed': True,
    'savePayloadVersion': 6,
    'addressablesVersion': '2.8.1',
    'unityCompileAndTestEvidence': 'PENDING_USER_UNITY_EXECUTION',
    'androidBackEvidence': 'PENDING_USER_ANDROID_EXECUTION',
    'failures': fail,
}
out = ROOT / 'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP05.json'
out.write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
sys.exit(1 if fail else 0)
