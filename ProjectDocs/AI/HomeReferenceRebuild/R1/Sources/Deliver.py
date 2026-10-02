from pathlib import Path
import json,shutil,xml.etree.ElementTree as E,hashlib,difflib
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parent;r=b.parent;qa=r/'QA';temp=Path('/tmp/lor-home-reference-r1')
# Retain commands/logs/captures without packaging Unity caches or isolated preference directories.
for phase in ['Baseline','Build01','Build02','Build03','Build04','Build05','Build06','Build07','Build08','Verify01','Verify02','Verify03','Verify04','Verify05','Verify06','Verify07']:
 src=temp/phase;dst=qa/'UnityVerification'/phase
 if src.exists():shutil.copytree(src,dst,dirs_exist_ok=True,ignore=shutil.ignore_patterns('test-config'))
summary=[]
for p in sorted((qa/'UnityVerification').glob('*/*/results.xml')):
 doc=E.parse(p).getroot();summary.append({'path':str(p.relative_to(r)),'passed':doc.attrib.get('passed'),'failed':doc.attrib.get('failed'),'skipped':doc.attrib.get('skipped'),'failures':[{'name':t.attrib['name'],'message':t.findtext('failure/message')}for t in doc.iter('test-case')if t.attrib.get('result')=='Failed']})
final=[a for a in summary if '/Verify07/'in a['path']];assert len(final)==2 and all(a['failed']=='0'for a in final),final
(qa/'TestSummary.json').write_text(json.dumps({'final':final,'finalPassed':sum(int(a['passed'])for a in final),'history':summary},indent=2))
protected={}
prior=Path('ProjectDocs/AI/LoadingReferenceRebuild/R1/QA/UnityVerification/Verify04')
for family in ['Splash','Loading']:
 checks=[]
 for p in (qa/'UnityVerification/Verify07'/family).glob('*.png'):
  old=prior/family/p.name
  if p.name.startswith('ActualLoading')or not old.exists():continue
  a=np.array(Image.open(p));z=np.array(Image.open(old));checks.append({'name':p.name,'pixelIdentical':bool(np.array_equal(a,z)),'maxChannelDelta':int(np.max(np.abs(a.astype(int)-z.astype(int))))})
 protected[family]=checks
(qa/'ProtectedCaptureComparison.json').write_text(json.dumps(protected,indent=2))
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
provenance={'original':json.loads((b/'Assessment.json').read_text()),'workflow':'Scenario alpha applied to original RGB; native live values; static Russian lettering retained. Generate only clean hidden environment.','finalAssets':json.loads((b/'FinalManifest.json').read_text()),'scenario':json.loads((qa/'ScenarioAudit.json').read_text()),'limitations':['Reconstructed hidden architecture','Fine hair/tail alpha','English blank surfaces and live fallback font','Gift rim repair when badge unavailable'],'runtimeArtRoot':'Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/'}
(r/'AssetProvenance.json').write_text(json.dumps(provenance,indent=2))
changes=json.loads((qa/'SourceChanges.json').read_text());diff=[]
for path in changes['changed']:
 p=Path(path);old=b/'BeforeCode'/p.name
 if old.exists():diff.extend(difflib.unified_diff(old.read_text().splitlines(True),p.read_text().splitlines(True),fromfile='before/'+path,tofile='after/'+path))
(r/'AppliedChanges.diff').write_text(''.join(diff))
shutil.copy2(temp/'verify07.py',b/'Verify.py')
print('Final passed',sum(int(a['passed'])for a in final),'protected captures',protected)
