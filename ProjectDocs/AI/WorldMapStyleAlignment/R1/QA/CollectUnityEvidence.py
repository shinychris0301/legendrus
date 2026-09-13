"""Retain unedited Unity results/captures, plus all earlier iteration outcomes."""
from pathlib import Path
import hashlib,json,shutil,xml.etree.ElementTree as ET
root=Path.cwd();d=root/'ProjectDocs/AI/WorldMapStyleAlignment/R1';out=d/'UnityVerification';tmp=Path('/tmp/lor-worldmap-r1')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
def write(name,data):(out/name).write_text(json.dumps(data,indent=2)+'\n')
history=[]
for folder,name,note in [
('baseline','BaselinePlayMode','Original page, before production edits.'),
('playmode1','InitialMapPlayMode','Navigation/layout tests passed; direct capture review revealed invisible custom graphics. No CanvasRenderer yet; this is not final visual evidence.'),
('playmode2','RenderedMapPlayMode','CanvasRenderer and mesh checks added; title/nav alignment and mipmaps corrected. Three map tests passed.'),
('editmode','InitialEditMode','47 focused Edit Mode tests passed before final chapter-number adjustment.'),
('editmode2','FinalEditMode','47 focused Edit Mode tests passed against final serialized World Map.'),
('finalplay','PanelFixturePlayMode','28/29 passed. Immediate same-frame panel activation did not yet register Close for the capture raycast. Fixture corrected to wait two render frames.'),
('finalplay2','StablePanelPlayMode','29/29 passed. External pixel comparison then found capture-time native HUD color fades and one stale snapped glyph after resolution changes; no accepted page source drift.'),
('finalplay3','GlyphCapturePlayMode','29/29 passed after waiting for normal native colors. One chapter number still crossed a pixel-snap boundary after resolution/language changes.'),
('finalplay4','FinalPlayMode','29/29 passed after disabling pixel snapping only on responsive World Map chapter numbers.')]:
 source=tmp/folder/'results.xml';run=ET.parse(source).getroot();tests=[]
 for t in run.iter('test-case'):
  row=dict(t.attrib)
  if t.find('failure/message') is not None:row['failure']=t.findtext('failure/message')
  tests.append(row)
 if name.startswith('Final'):assert run.attrib['result']=='Passed'
 shutil.copyfile(source,out/(name+'.xml'));write(name+'.json',{'source':str(source),'sha256':sha(source),'run':run.attrib,'tests':tests,'note':note})
 history.append({'folder':folder,'run':run.attrib,'note':note,'xmlSha256':sha(source)})
write('TestRunHistory.json',history)
imports=[]
for name,note,exitcode in [
('import','Initial import: sprite resolution failed.',1),
('import-retry','Map importer and refresh added; sprite resolution still failed.',1),
('import-diagnostic','Diagnostic showed all five textures imported as Cubemap.',1),
('import-final','Texture2D fixed; exposed incorrect cloned HUD resource path.',1),
('import-bindings','HUD paths corrected; serialized prefab saved/read back.',0),
('import-renderers','Mesh assertion used an unavailable GetMesh overload; test compilation failed.',1),
('import-renderers-fixed','Correct no-argument GetMesh API; native graphic serialization saved/read back.',0),
('import-glyph-stability','Final owner saved/read back with chapter-number pixel snapping disabled; no accepted shared glyph changes.',0)]:
 p=tmp/(name+'.log');text=p.read_text();imports.append({'source':str(p),'sha256':sha(p),'exitCode':exitcode,'note':note,'saveReadbackObserved':'World Map alignment serialized prefab saved and read back at revision 1.' in text})
assert imports[-1]['saveReadbackObserved']
for folder in ['editmode2','finalplay4']:
 p=tmp/folder/'unity.log';text=p.read_text();assert 'error CS' not in text and 'Shader error in' not in text
 imports.append({'source':str(p),'sha256':sha(p),'exitCode':0,'compilerOrShaderErrors':[]})
write('ImportAndCompileHistory.json',imports)
captures=[]
for category in ['WorldMap','Home','SplashRegression','LoadingRegression']:
 dest=out/category;dest.mkdir(exist_ok=True)
 for p in sorted((tmp/'finalplay4'/category).iterdir()):
  if p.suffix not in {'.png','.json'} or p.name.startswith('Diagnostic_'):continue
  q=dest/p.name;shutil.copyfile(p,q);assert sha(p)==sha(q)
  captures.append({'path':str(q.relative_to(out)),'source':str(p),'sha256':sha(q),'bytes':q.stat().st_size})
write('CaptureManifest.json',{'kind':'Unedited actual Unity renders and fixtures, with simulated display and safe areas. No offline page composites.','fileCount':len(captures),'files':captures})
r=json.loads((root/'ProjectDocs/AI/HomeStyleAlignment/SelectiveLightingR11/UnityVerification/Reproduction.json').read_text())
r['owner']='LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildWorldMapAlignment';r['environmentVariables'].append('LOR_WORLDMAP_CAPTURE_DIR')
r['EditModeFixtures'].append('LegendsOfRus.Tests.ProductionCoreLoopUi.WorldMapAlignmentEditModeTests')
r['PlayModeFixtures']+=['LegendsOfRus.Tests.PlayMode.WorldMapAlignmentPlayModeTests','LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.MapBackHotspots_FollowChapterMapWorldMapHomeHierarchy']
r['finalRunFolders']={'EditMode':str(tmp/'editmode2'),'PlayMode':str(tmp/'finalplay4')};r['captureFolders']={x:str(tmp/'finalplay4'/x) for x in ['WorldMap','Home','SplashRegression','LoadingRegression']}
r['scope']='Focused Edit Mode and graphics-enabled Play Mode, actual camera renders and simulated safe areas. No Android build, physical device, full suite or performance validation.';write('Reproduction.json',r)
print(json.dumps({'editMode':47,'playMode':29,'finalCaptureFiles':len(captures),'importLaunchesRetained':8}))
