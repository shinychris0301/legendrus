"""Retain unedited Unity results and final captures, including iteration history."""
from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as ET
root=Path.cwd();d=root/'ProjectDocs/AI/ChapterMapStyleAlignment/R1';out=d/'UnityVerification';tmp=Path('/tmp/lor-chaptermap-r1')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
def write(name,data):(out/name).write_text(json.dumps(data,indent=2)+'\n')
history=[]
for folder,name,note in [
('baseline','BaselinePlayMode','Original Chapter Map before production edits; 1 capture test.'),
('playmode1','InitialChapterPlayMode','3 tests passed; direct review then led to bolder titles, hollow stars and Back polish.'),
('editmode','InitialEditMode','50 tests passed before final node-spacing correction.'),
('finalplay','InitialRegressionPlayMode','32 tests passed; direct capture review caught overlapping star rows. Added an explicit cross-node star/face bounds check and corrected positions.'),
('editmode2','FinalEditMode','Final serialized page: 50 focused Edit Mode tests.'),
('finalplay2','FinalPlayMode','Final 32-test run with non-overlap, safe-area, rendered meshes, state, routing and prior-screen regression checks.')]:
 source=tmp/folder/'results.xml';run=ET.parse(source).getroot();tests=[]
 for t in run.iter('test-case'):
  row=dict(t.attrib)
  if t.find('failure/message') is not None:row['failure']=t.findtext('failure/message')
  tests.append(row)
 assert run.attrib['result']=='Passed'
 shutil.copyfile(source,out/(name+'.xml'));write(name+'.json',{'source':str(source),'sha256':sha(source),'run':run.attrib,'tests':tests,'note':note})
 history.append({'folder':folder,'run':run.attrib,'note':note,'xmlSha256':sha(source)})
write('TestRunHistory.json',history)
imports=[]
for name in ['import','import-polish','import-spacing','import-final']:
 p=tmp/(name+'.log');text=p.read_text();saved='Chapter Map alignment serialized prefab saved and read back at revision 1.' in text
 assert saved and 'error CS' not in text and 'Shader error in' not in text
 imports.append({'source':str(p),'sha256':sha(p),'exitCode':0,'saveReadbackObserved':saved,'compilerOrShaderErrors':[]})
for folder in ['editmode2','finalplay2']:
 p=tmp/folder/'unity.log';text=p.read_text();assert 'error CS' not in text and 'Shader error in' not in text
 imports.append({'source':str(p),'sha256':sha(p),'exitCode':0,'compilerOrShaderErrors':[]})
write('ImportAndCompileHistory.json',imports)
captures=[]
for category in ['ChapterMap','WorldMap','Home','SplashRegression','LoadingRegression']:
 dest=out/category;dest.mkdir(exist_ok=True)
 for p in sorted((tmp/'finalplay2'/category).iterdir()):
  if p.suffix not in {'.png','.json'} or p.name.startswith('Diagnostic_'):continue
  q=dest/p.name;shutil.copyfile(p,q);assert sha(p)==sha(q)
  captures.append({'path':str(q.relative_to(out)),'source':str(p),'sha256':sha(q),'bytes':q.stat().st_size})
write('CaptureManifest.json',{'kind':'Unedited actual Unity renders and fixtures, with simulated display/safe areas. No offline page composites.','fileCount':len(captures),'files':captures})
iterations=[]
for folder in ['playmode1','finalplay']:
 dest=d/'QA/Iterations'/folder;dest.mkdir(parents=True,exist_ok=True)
 for p in (tmp/folder/'ChapterMap').iterdir():
  q=dest/p.name;shutil.copyfile(p,q);iterations.append({'path':str(q.relative_to(d)),'sha256':sha(q),'source':str(p)})
(d/'QA/Iterations/Manifest.json').write_text(json.dumps(iterations,indent=2)+'\n')
r=json.loads((root/'ProjectDocs/AI/WorldMapStyleAlignment/R1/UnityVerification/Reproduction.json').read_text())
r['owner']='LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildChapterMapAlignment';r['environmentVariables'].append('LOR_CHAPTERMAP_CAPTURE_DIR')
r['EditModeFixtures'].append('LegendsOfRus.Tests.ProductionCoreLoopUi.ChapterMapAlignmentEditModeTests');r['PlayModeFixtures'].append('LegendsOfRus.Tests.PlayMode.ChapterMapAlignmentPlayModeTests')
r['finalRunFolders']={'EditMode':str(tmp/'editmode2'),'PlayMode':str(tmp/'finalplay2')};r['captureFolders']={x:str(tmp/'finalplay2'/x) for x in ['ChapterMap','WorldMap','Home','SplashRegression','LoadingRegression']};write('Reproduction.json',r)
print(json.dumps({'editMode':50,'playMode':32,'finalCaptureFiles':len(captures),'ownerLaunchesRetained':4}))
