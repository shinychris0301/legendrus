from pathlib import Path
import hashlib,json,re,shutil,ast
import xml.etree.ElementTree as E
from PIL import Image
import numpy as np
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/R1';uv=o/'UnityVerification';uv.mkdir(exist_ok=True)
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
for name in ['ImportChapterComplete','EditMode','PlayMode','ChapterComplete','Pause']:
 shutil.copytree(Path('/tmp/lor-chaptercomplete-r1/run4')/name,uv/name,dirs_exist_ok=True)
shutil.copytree(Path('/tmp/lor-chaptercomplete-r1/finale1/PlayMode'),uv/'FinalePlayMode',dirs_exist_ok=True)
for run in ['run1','run2','run3']:
 for stage in (Path('/tmp/lor-chaptercomplete-r1')/run).iterdir():
  if stage.is_dir() and stage.name in ['ImportChapterComplete','EditMode','PlayMode']:
   target=o/'QA/EarlierRuns'/run/stage.name;target.mkdir(parents=True,exist_ok=True)
   for f in stage.iterdir():
    if f.suffix in ['.log','.xml','.json']:shutil.copy2(f,target/f.name)
before={x['path']:x['sha256'] for x in json.loads((o/'BeforeProductionHashes.json').read_text())};after={str(p.relative_to(r)):sha(p) for p in sorted((r/'Assets/LegendsOfRus').rglob('*')) if p.is_file()}
changed=[p for p,h in before.items() if p in after and after[p]!=h];removed=[p for p in before if p not in after];added=[p for p in after if p not in before]
assert not removed
expected=['Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab','Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs','Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs']
assert set(changed)==set(expected),(changed,expected)
metas=[p for p in before if p.endswith('.meta')];assert all(before[p]==after[p] for p in metas)
pref='Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterComplete.prefab';old=(Path('/tmp/lor-chaptercomplete-r1/before')/pref).read_text();new=(r/pref).read_text();ids=lambda s:set(re.findall(r'^--- !u!\d+ &(-?\d+)',s,re.M));assert ids(old)<=ids(new)
prior=json.loads((r/'ProjectDocs/AI/PauseStyleAlignment/R1/UnityVerification/EvidenceHashes.json').read_text());assert all(sha(r/x['path'])==x['sha256'] for x in prior)
guidPaths={}
for p in (r/'Assets').rglob('*.meta'):
 m=re.search(r'^guid: ([a-f0-9]+)',p.read_text(),re.M)
 if m:guidPaths.setdefault(m[1],[]).append(str(p.relative_to(r)))
newGuids=[]
for name in added:
 if name.endswith('.meta'):
  g=re.search(r'^guid: ([a-f0-9]+)',(r/name).read_text(),re.M).group(1);assert len(guidPaths[g])==1;newGuids.append(g)
audit=json.loads((o/'QA/ExtractionAudit.json').read_text());art=r/'Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned'
assert all(sha(art/(x['name']+'.png'))==x['sha256'] for x in audit)
provenance=json.loads((o/'Sources/Provenance.json').read_text());src=Image.open(r/provenance['source']).convert('RGB');assert sha(r/provenance['source'])==provenance['sha256']
rgb=[]
for name in ['header','banner','foreground','next','map']:
 assert np.array_equal(np.array(src.crop(provenance['boxes'][name])),np.array(Image.open(art/(name+'_ru.png')).convert('RGB')));rgb.append(name)
testResults={}
for stage in ['EditMode','PlayMode','FinalePlayMode']:
 node=E.parse(uv/stage/'results.xml').getroot();assert node.get('failed')=='0' and node.get('skipped')=='0';testResults[stage]={k:node.get(k) for k in ['total','passed','failed','skipped']}
for f in o.rglob('*.py'):ast.parse(f.read_text(),filename=str(f))
result={'baselineFiles':len(before),'unchangedFiles':len(before)-len(changed),'changedExisting':changed,'newFiles':added,'removedFiles':removed,'preservedMetaFiles':len(metas),'preservedOriginalPrefabObjects':len(ids(old)),'finalPrefabObjects':len(ids(new)),'uniqueNewGuids':len(newGuids),'preservedPauseEvidenceFiles':len(prior),'spriteAuditMatches':len(audit),'originalRgbVerified':rgb,'tests':testResults,'scenarioCuTotal':27}
(o/'QA/FinalAudit.json').write_text(json.dumps(result,indent=2)+'\n')
(o/'AfterProductionHashes.json').write_text(json.dumps([{'path':p,'sha256':h} for p,h in after.items()],indent=2)+'\n')
(uv/'EvidenceHashes.json').write_text(json.dumps([{'path':str(p.relative_to(r)),'sha256':sha(p)} for p in sorted(uv.rglob('*')) if p.is_file() and p.name!='EvidenceHashes.json'],indent=2)+'\n')
print(json.dumps({k:v for k,v in result.items() if k!='newFiles'},indent=2));print('New files',len(added))
