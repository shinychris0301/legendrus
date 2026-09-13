"""Retain actual Unity results, including failed iterations; never relabel previews as runtime."""
from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as ET
root=Path.cwd();d=root/'ProjectDocs/AI/PreLevelStyleAlignment/R1';out=d/'UnityVerification';tmp=Path('/tmp/lor-prelevel-r1')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
def write(name,data):(out/name).write_text(json.dumps(data,indent=2)+'\n')
history=[]
for folder,name,note in [
 ('baseline','BaselinePlayMode','Original Pre-Level capture test before production editing.'),
 ('playmode1','InitialPlayMode','3/4 passed. Completion advances selection to the next level; replay assertion needed explicit reselection. Capture review found lettering on raised panel trim.'),
 ('editmode','InitialEditMode','53/53 passed before bonus reward spacing adjustment.'),
 ('finalplay','InitialRegressionPlayMode','37/37 passed; direct level-10 review exposed bonus row touching gold trim. Added explicit lower-trim clearance check.'),
 ('editmode2','FinalEditMode','Focused final prefab/import and five earlier-page validation.'),
 ('finalplay2','FinalPlayMode','36/37 passed, including all Pre-Level checks. Unchanged Splash strict RU/EN/RU comparison failed on seven pixels, each delta <=1/255. Exact test rechecked separately without source or tolerance changes.'),
 ('splash-recheck','SplashRecheck','Same strict assertion and source, independent rerun; see the actual result.')]:
 source=tmp/folder/'results.xml';run=ET.parse(source).getroot();tests=[]
 for t in run.iter('test-case'):
  row=dict(t.attrib)
  if t.find('failure/message') is not None:row['failure']=t.findtext('failure/message')
  tests.append(row)
 if name=='FinalEditMode':assert run.attrib['result']=='Passed'
 if name=='FinalPlayMode':assert all(t['result']=='Passed' for t in tests if 'PreLevelAlignmentPlayModeTests' in t['fullname'])
 shutil.copyfile(source,out/(name+'.xml'));write(name+'.json',{'source':str(source),'sha256':sha(source),'run':run.attrib,'tests':tests,'note':note})
 history.append({'folder':folder,'run':run.attrib,'note':note,'xmlSha256':sha(source)})
write('TestRunHistory.json',history)
imports=[]
for name,expected,note in [('import1',False,'Invocation used an incorrect namespace; corrected.'),('import2',False,'Existing Pre-Level uses the R3 marker, not the earlier marker class. Owner corrected, original component retained.'),('import3',False,'Test attempted private CloseModal; replaced with actual visible Close interaction.'),('import4',True,'Native prefab saved and read back.'),('import-polish',True,'Lettering moved inside panel surfaces; exact marker/IDs retained.'),('import-rewards',True,'Bonus reward row lifted clear of lower trim.')]:
 p=tmp/(name+'.log');text=p.read_text();saved='Pre-Level alignment serialized prefab saved and read back at revision 1.' in text
 assert saved==expected
 errors=[line for line in text.splitlines() if 'error CS' in line or 'Shader error in' in line or line.startswith('NullReferenceException:')]
 if expected:assert not errors
 imports.append({'source':str(p),'sha256':sha(p),'exitCode':0 if expected else 1,'saveReadbackObserved':saved,'errors':errors,'note':note})
for folder in ['editmode2','finalplay2','splash-recheck']:
 p=tmp/folder/'unity.log';text=p.read_text();assert 'error CS' not in text and 'Shader error in' not in text
 imports.append({'source':str(p),'sha256':sha(p),'exitCode':2 if folder=='finalplay2' else (0 if ET.parse(tmp/folder/'results.xml').getroot().attrib['result']=='Passed' else 2),'compilerOrShaderErrors':[]})
write('ImportAndCompileHistory.json',imports)
captures=[]
for category in ['PreLevel','ChapterMap','WorldMap','Home','SplashRegression','LoadingRegression']:
 dest=out/category;dest.mkdir(exist_ok=True)
 for p in sorted((tmp/'finalplay2'/category).iterdir()):
  if p.suffix not in {'.png','.json'} or p.name.startswith('Diagnostic_'):continue
  q=dest/p.name;shutil.copyfile(p,q);assert sha(p)==sha(q)
  captures.append({'path':str(q.relative_to(out)),'source':str(p),'sha256':sha(q),'bytes':q.stat().st_size})
write('CaptureManifest.json',{'kind':'Unedited actual Unity renders and fixtures, simulated displays/safe areas; no offline page composites.','fileCount':len(captures),'files':captures})
dest=out/'SplashRecheck';dest.mkdir(exist_ok=True)
for p in (tmp/'splash-recheck/SplashRegression').iterdir():shutil.copyfile(p,dest/p.name)
for iteration in ['playmode1','finalplay']:
 dest=d/'QA/Iterations'/iteration;dest.mkdir(parents=True,exist_ok=True)
 for p in (tmp/iteration/'PreLevel').iterdir():shutil.copyfile(p,dest/p.name)
r=json.loads((root/'ProjectDocs/AI/ChapterMapStyleAlignment/R1/UnityVerification/Reproduction.json').read_text())
r['owner']='LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildPreLevelAlignment';r['environmentVariables'].append('LOR_PRELEVEL_CAPTURE_DIR')
r['EditModeFixtures'].append('LegendsOfRus.Tests.ProductionCoreLoopUi.PreLevelAlignmentEditModeTests')
r['PlayModeFixtures']+=['LegendsOfRus.Tests.PlayMode.PreLevelAlignmentPlayModeTests','LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.PreLevelVisibleBackHotspot_ReturnsToChapterMap']
r['finalRunFolders']={'EditMode':str(tmp/'editmode2'),'PlayMode':str(tmp/'finalplay2')};r['captureFolders']={x:str(tmp/'finalplay2'/x) for x in ['PreLevel','ChapterMap','WorldMap','Home','SplashRegression','LoadingRegression']};write('Reproduction.json',r)
print(json.dumps({'finalRuns':[x['run'] for x in history if x['folder'] in ['editmode2','finalplay2']],'captureFiles':len(captures)}))
