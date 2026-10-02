from pathlib import Path
import ast, hashlib, json, re
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
for path in (BASE/'Sources').glob('*.py'):ast.parse(path.read_text())
counts={}
for name,expected in [('Edit',14),('PlayVerified',22)]:
    result=json.loads((BASE/'QA'/name/'summary.json').read_text())
    tree=ET.parse(BASE/'QA'/name/'results.xml');cases=tree.findall('.//test-case')
    assert result['exitCode']==0 and len(cases)==expected and all(c.get('result')=='Passed' for c in cases)
    counts[name]=expected
scope=json.loads((BASE/'QA/Scope.json').read_text())
assert scope['unchanged']==7817 and len(scope['changed'])==4 and len(scope['added'])==4 and not scope['deleted']
proof=json.loads((BASE/'QA/PrefabPreservation.json').read_text())
assert proof['originalGeometryFontsSpritesTextColorsTargetsAndOtherBindingsUnchanged'] and proof['newObjects']==20
matrix=json.loads((BASE/'QA/StockMeasurements.json').read_text())
assert len(matrix['configurations'])==48 and all(r['allExactCharactersVisible'] for r in matrix['configurations'])
browser=json.loads((BASE/'QA/Browser/Checks.json').read_text())
assert browser['passed']==7 and browser['failed']==0 and not browser['errors']
html=(BASE/'GameplayStock_R5_Review.html').read_text()
data=json.loads(re.search(r'<script id="data" type="application/json">(.*?)</script>',html,re.S).group(1))
for row in data['configurations']:assert (BASE/row['capture']).is_file()
for target in re.findall(r'href="([^"]+)"',html):assert (BASE/target).is_file()
old=json.loads((BASE/'Sources/ApkBefore.json').read_text());apk=ROOT/old['path']
assert apk.stat().st_size==old['bytes'] and sha(apk)==old['sha256']
assert Path(str(apk)+'.sha256').read_text()==old['sidecar'] and old['sha256'] in old['sidecar']
inventory=json.loads((ROOT/'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json').read_text())
recordings={f['path']:f['sha256'] for f in inventory['rawFiles'] if Path(f['path']).is_absolute()}
folder=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
current={str(p):sha(p) for p in folder.rglob('*.json')}
assert current==recordings
assert not (ROOT/'Assets/LegendsOfRus/Scripts/Editor/InstallStockPrefab.cs').exists()
assert not list((ROOT/'Assets').glob('InitTestScene*'))
assert not list(BASE.rglob('*.pyc'))
report={'result':'Passed','unityEditPassed':counts['Edit'],'unityPlayPassed':counts['PlayVerified'],
        'finalDistinctUnityChecks':sum(counts.values()),'browserChecksPassed':7,'nativeStockConfigurations':48,
        'nativeStockObservations':192,'finalNativePngs':len(list((BASE/'QA/PlayVerified').rglob('*.png'))),
        'projectScope':scope,'allPriorPrefabGeometryBindingsArtFontsRetained':True,
        'normalManualRecorderFilesUnchanged':len(current),'apkBytes':old['bytes'],'apkSha256':old['sha256'],
        'apkContainsNewChange':False,'phoneExecution':False,'newHumanPlaytestEvidence':False}
(BASE/'QA/FinalValidation.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps(report,indent=2))
