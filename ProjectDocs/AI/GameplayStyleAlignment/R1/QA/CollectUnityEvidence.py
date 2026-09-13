"""Preserve unedited Unity evidence, including unsuccessful iterations."""
from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as E
root=Path(__file__).resolve().parents[5];doc=root/'ProjectDocs/AI/GameplayStyleAlignment/R1';tmp=Path('/tmp/lor-gameplay-r1');out=doc/'UnityVerification';out.mkdir(exist_ok=True)
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
def write(n,data):(out/n).write_text(json.dumps(data,indent=2)+'\n')
runs=[('baseline-results.xml','BaselinePlayMode','Legacy Gameplay, existing tutorial visible.'),('capture1-results.xml','FirstNativeCapture','Five native portraits before mask, tone, tile-size and atlas-neighbour refinements.'),('focused1/results.xml','InitialGameplayPlayMode','66/68. Four new Gameplay tests passed; two older runtime assertions needed score-goal and expected watchdog-log corrections.'),('edit1/results.xml','InitialUiEditMode','56/56. Additional intended namespace filter did not match gameplay-core tests; these were run separately.'),('coreedit1/results.xml','GameplayCoreEditMode','64/66. Two failures retained in unchanged pure core/test source. See QA/ExistingCoreTestFailures.json.'),('final1/EditMode/results.xml','BeforeMatchColorUiEditMode','Final native prefab/import and all six earlier-page checks.'),('final1/PlayMode/results.xml','BeforeMatchColorPlayMode','104/105. All Gameplay/runtime checks passed. Existing strict Splash roundtrip assertion failed by seven pixels; source and tolerance preserved.'),('final2/EditMode/results.xml','FinalUiEditMode','Final UI checks after adding the special match-colour marker.'),('final2/PlayMode/results.xml','FinalPlayMode','Final combined checks after adding the special match-colour marker; exact result retained.'),('splash-recheck/results.xml','SplashRecheck','Same strict Splash assertion rerun once without source/tolerance changes.')]
history=[]
for source,name,note in runs:
 p=tmp/source
 if not p.exists():continue
 r=E.parse(p).getroot();tests=[]
 for t in r.iter('test-case'):
  row=dict(t.attrib)
  if t.find('failure/message') is not None:row['failure']=t.findtext('failure/message')
  tests.append(row)
 shutil.copyfile(p,out/(name+'.xml'));write(name+'.json',{'run':r.attrib,'source':str(p),'sha256':sha(p),'note':note,'tests':tests});history.append({'name':name,'run':r.attrib,'note':note,'sha256':sha(p)})
write('TestRunHistory.json',history)
imports=[]
for i in range(1,6):
 p=tmp/f'import{i}.log';s=p.read_text();saved='Gameplay alignment serialized prefab saved and read back at revision 1.' in s
 errors=[line for line in s.splitlines() if 'error CS' in line or 'Shader error in' in line or line.startswith('NullReferenceException:')]
 imports.append({'source':str(p),'sha256':sha(p),'saveReadbackObserved':saved,'compilerOrShaderErrors':errors})
write('ImportAndCompileHistory.json',imports)
captures=[]
for category in ['Gameplay','Home','Splash','Loading','WorldMap','ChapterMap','PreLevel']:
 dest=out/category;dest.mkdir(exist_ok=True)
 for p in sorted((tmp/'final2'/category).glob('*')):
  if p.suffix not in ['.png','.json']:continue
  q=dest/p.name;shutil.copyfile(p,q);assert sha(p)==sha(q);captures.append({'path':str(q.relative_to(out)),'source':str(p),'sha256':sha(q),'bytes':q.stat().st_size})
for source,category in [(tmp/'baseline','Before'),(tmp/'splash-recheck/Splash','SplashRecheck')]:
 dest=out/category;dest.mkdir(exist_ok=True)
 for p in source.glob('*'):
  if p.suffix in ['.png','.json']:q=dest/p.name;shutil.copyfile(p,q);assert sha(q)==sha(p)
for source in ['capture1','focused1/Gameplay']:
 dest=doc/'QA/Iterations'/source.replace('/','_');dest.mkdir(parents=True,exist_ok=True)
 for p in (tmp/source).glob('*'):
  if p.suffix in ['.png','.json']:shutil.copyfile(p,dest/p.name)
write('CaptureManifest.json',{'kind':'Unedited actual Unity Play Mode renders and capture metadata; displays and safe areas simulated.','fileCount':len(captures),'files':captures})
repro=json.loads((root/'ProjectDocs/AI/PreLevelStyleAlignment/R1/UnityVerification/Reproduction.json').read_text());repro['owner']='LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildGameplayAlignment';repro['environmentVariables'].append('LOR_GAMEPLAY_CAPTURE_DIR');repro['verificationScript']='ProjectDocs/AI/GameplayStyleAlignment/R1/QA/RunFinalVerification.py';repro['finalRunFolders']={mode:str(tmp/'final2'/mode) for mode in ['EditMode','PlayMode']};repro['captureFolders']={k:str(tmp/'final2'/k) for k in ['Gameplay','Home','Splash','Loading','WorldMap','ChapterMap','PreLevel']}
for mode in ['EditMode','PlayMode']:
 command=json.loads((tmp/'final2'/mode/'command.json').read_text());repro[mode+'Fixtures']=command[command.index('-testFilter')+1].split(';')
write('Reproduction.json',repro);print('Saved',len(history),'test runs and',len(captures),'final capture files')
