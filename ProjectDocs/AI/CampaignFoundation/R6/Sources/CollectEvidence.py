from pathlib import Path
import json,hashlib,shutil,xml.etree.ElementTree as ET
root=Path.cwd(); b=root/'ProjectDocs/AI/CampaignFoundation/R6'
before=json.loads((b/'Sources/BeforeHashes.json').read_text())
for name in ['link.xml','link.xml.meta']:
 dest=root/'Assets/AddressableAssetsData'/name; src=b/'Sources'/name
 assert hashlib.sha256(src.read_bytes()).hexdigest()==before[str(dest.relative_to(root))]
 shutil.copy2(src,dest)
 assert hashlib.sha256(dest.read_bytes()).hexdigest()==before[str(dest.relative_to(root))]
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for folder in ['Assets','Packages','ProjectSettings'] for p in (root/folder).rglob('*') if p.is_file()}
scope={'unchangedCount':sum(after.get(k)==v for k,v in before.items()),'modified':[k for k,v in before.items() if k in after and after[k]!=v],'deleted':[k for k in before if k not in after],'added':[k for k in after if k not in before]}
(b/'QA/Scope.json').write_text(json.dumps(scope,indent=2))
results=[]
for p in sorted((b/'QA/Verify04').glob('*/results.xml')):
 r=ET.parse(p).getroot();results.append({'suite':p.parent.name,**{k:r.get(k) for k in ['result','total','passed','failed','skipped']}})
source=Path('/tmp/lor-campaign-foundation-r6/Verify04/Journey')
shutil.copytree(source,b/'QA/Journey',dirs_exist_ok=True)
summary={'unity':'6000.5.9f1','run':'Verify04','results':results,'newR6EditModeCases':11,'newR6UIPlayModeCases':5,'unityCaptures':len(list((b/'QA/Journey').rglob('*.png'))),'priorRunFailure':'Verify03 PlayMode native signal 11, no results.xml; not counted as passed. Final suites run in separate Editor processes.','limitations':['Development-only in-memory preview, no live startup cutover.','Button callbacks and board commands are automated; no physical touch/device acceptance.','No new authored levels or scenario mapping.','No Android/IL2CPP campaign, full-project suite or physical power-loss test.']}
(b/'QA/Verification.json').write_text(json.dumps(summary,indent=2))
print(json.dumps(scope,indent=2));print(json.dumps(summary,indent=2))
