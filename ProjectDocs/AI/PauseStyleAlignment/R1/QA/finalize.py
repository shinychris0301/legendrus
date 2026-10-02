from pathlib import Path
import json,hashlib,re,shutil,difflib,ast,subprocess,xml.etree.ElementTree as ET
from html.parser import HTMLParser
from PIL import Image
import numpy as np
r=Path.cwd();o=r/'ProjectDocs/AI/PauseStyleAlignment/R1';sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
write=lambda p,x:p.write_text(json.dumps(x,indent=2,ensure_ascii=False)+'\n')
b=json.loads((o/'BeforeProductionHashes.json').read_text());baseline={x['path']:x['sha256'] for x in b};paths=sorted(p for p in (r/'Assets/LegendsOfRus').rglob('*') if p.is_file());now={str(p.relative_to(r)):sha(p) for p in paths}
changed=[p for p,h in baseline.items() if now.get(p)!=h];new=sorted(set(now)-set(baseline));deleted=sorted(set(baseline)-set(now));assert not deleted
expected=['Assets/LegendsOfRus/Resources/ProductionUI/Pages/Pause.prefab','Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs','Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs','Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs','Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayAlignmentEditModeTests.cs']
assert sorted(changed)==sorted(expected),(changed,expected)
metas=[p for p in baseline if p.endswith('.meta')];assert all(now[p]==baseline[p] for p in metas)
ids=lambda s:set(re.findall(r'^--- !u!\d+ &(\d+)',s,re.M))
path='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Pause.prefab';oldids=ids((Path('/tmp/lor-pause-r1/before')/path).read_text());newids=ids((r/path).read_text());assert not oldids-newids
allguids={}
for p in (r/'Assets').rglob('*.meta'):
 m=re.search(r'^guid: (\w+)',p.read_text(),re.M)
 if m:allguids.setdefault(m[1],[]).append(str(p))
newmetas=[p for p in new if p.endswith('.meta')]
for p in newmetas:assert len(allguids[re.search(r'^guid: (\w+)',(r/p).read_text(),re.M)[1]])==1
prior=json.loads((r/'ProjectDocs/AI/FailStyleAlignment/OriginalLetteringR12/UnityVerification/EvidenceHashes.json').read_text());assert all(sha(r/e['path'])==e['sha256'] for e in prior)
for p in changed+[p for p in new if p.endswith('.cs')]:
 if p.endswith('.cs'):assert all(line.rstrip()==line for line in (r/p).read_text().splitlines()),p
ex=json.loads((o/'QA/ExtractionAudit.json').read_text());provenance=json.loads((o/'Sources/Provenance.json').read_text());source=Image.open(r/provenance['source']).convert('RGB')
assert sha(r/provenance['source'])==provenance['sha256']
for a in ex:
 p=r/'Assets/LegendsOfRus/Resources/ProductionUI/PauseAligned'/(a['name']+'.png');assert sha(p)==a['sha256']
 if a['name'].endswith('_ru'):assert np.array_equal(np.asarray(Image.open(p).convert('RGB')),np.asarray(source.crop(a['source'])))
for p in o.rglob('*.py'):ast.parse(p.read_text())
assert not subprocess.check_output(['git','status','--short','--','ProjectSettings','Packages'],text=True)
audit={'baselineCount':len(b),'unchangedCount':len(b)-len(changed),'changedFiles':changed,'newFiles':new,'deletedFiles':deleted,'existingMetasPreserved':len(metas),'newUniqueGuids':len(newmetas),'oldPrefabObjectCount':len(oldids),'newPrefabObjectCount':len(newids),'missingOldPrefabIds':[],'priorFailureR12EvidenceHashesPreserved':len(prior),'installedSpritesMatchAudit':len(ex),'originalRussianSpriteRgbChecks':5,'projectSettingsPackagesGitChanges':False}
write(o/'QA/SourceAudit.json',audit);write(o/'FinalProductionHashes.json',[{'path':p,'sha256':h} for p,h in now.items()])
diff=[]
for p in changed:
 old=Path('/tmp/lor-pause-r1/before')/p
 if old.exists():diff+=difflib.unified_diff(old.read_text().splitlines(True),(r/p).read_text().splitlines(True),fromfile='before/'+p,tofile='after/'+p)
(o/'AppliedSourceChanges.diff').write_text(''.join(diff))
v=o/'UnityVerification';v.mkdir(exist_ok=False);results={}
for part in ('ImportPause','EditMode','PlayMode','Pause','Fail','Gameplay'):
 s=Path('/tmp/lor-pause-r1/run4')/part;dest=v/part;dest.mkdir()
 for p in s.iterdir():
  if p.suffix in ('.json','.png','.xml','.log'):shutil.copy2(p,dest/p.name)
 if (s/'results.xml').exists():results[part]={k:ET.parse(s/'results.xml').getroot().get(k) for k in ('result','total','passed','failed','skipped')}
 if (s/'command.json').exists():results[part+'ExitCode']=json.loads((s/'command.json').read_text())['exitCode']
 if part in ('Pause','Fail','Gameplay'):results[part+'CapturePairs']=len(list(dest.glob('*.png')))
write(o/'QA/UnityResults.json',results)
# Retain earlier failures as historical evidence, not final results.
h=o/'QA/EarlierRuns';h.mkdir()
for run in ('run1','run2','run3'):
 for platform in ('EditMode','PlayMode'):
  p=Path('/tmp/lor-pause-r1')/run/platform/'results.xml'
  if p.exists():shutil.copy2(p,h/(run+'_'+platform+'.xml'))
evidence=[{'path':str(p.relative_to(r)),'sha256':sha(p)} for p in sorted(v.rglob('*')) if p.is_file()];write(v/'EvidenceHashes.json',evidence)
class Links(HTMLParser):
 def __init__(self):super().__init__();self.paths=[];self.options=0
 def handle_starttag(self,t,attrs):
  d=dict(attrs)
  if t=='option':self.options+=1;self.paths.append(d['value'])
  for k in ('href','src'):
   if k in d:self.paths.append(d[k])
l=Links();l.feed((o/'Pause_Alignment_R1_Review.html').read_text());assert all((o/p).exists() for p in l.paths)
write(o/'QA/DeliveryChecks.json',{'finalProductionHashes':len(now),'unityEvidenceHashes':len(evidence),'htmlChoicesResolve':l.options,'htmlPathsResolve':len(l.paths),'pythonScriptsParsed':len(list(o.rglob('*.py'))),'sourceWhitespaceClean':True,'finalCaptureVisualReview':['Pause_941x1672_RU','Pause_ThreeGoals_576_EN','Pause_576x1280_safe_RU'],'earlierCaptureVisualReview':['run3 Pause_941x1672_RU','run3 Pause_941x1672_EN'],'alphaReviewBackgrounds':['black','white','magenta'],'notRun':['APK/AAB','physical Android','full suite','browser interaction','extended manual play','profiling','exact font identification','live purchase/ad integration']})
print(json.dumps({**audit,**results,'finalHashes':len(now),'evidenceHashes':len(evidence)},indent=2))
