from pathlib import Path
import json,hashlib,shutil,xml.etree.ElementTree as ET
root=Path.cwd(); b=root/'ProjectDocs/AI/CampaignFoundation/R5'
before=json.loads((b/'Sources/BeforeHashes.json').read_text())
for name in ['link.xml','link.xml.meta']:
 dest=root/'Assets/AddressableAssetsData'/name; src=b/'Sources'/name
 assert hashlib.sha256(src.read_bytes()).hexdigest()==before[str(dest.relative_to(root))]
 if not dest.exists(): shutil.copy2(src,dest)
 assert hashlib.sha256(dest.read_bytes()).hexdigest()==before[str(dest.relative_to(root))]
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for folder in ['Assets','Packages','ProjectSettings'] for p in (root/folder).rglob('*') if p.is_file()}
scope={'unchangedCount':sum(after.get(k)==v for k,v in before.items()),'modified':[k for k,v in before.items() if k in after and after[k]!=v],'deleted':[k for k in before if k not in after],'added':[k for k in after if k not in before]}
(b/'QA/Scope.json').write_text(json.dumps(scope,indent=2))
results=[]
for p in sorted((b/'QA/Verify01').glob('*/results.xml')):
 r=ET.parse(p).getroot();results.append({'suite':p.parent.name,**{k:r.get(k) for k in ['result','total','passed','failed','skipped']}})
source=Path('/tmp/lor-campaign-foundation-r5/Verify01/Journey')
shutil.copytree(source,b/'QA/Journey',dirs_exist_ok=True)
summary={'unity':'6000.5.9f1','run':'Verify01','results':results,'newR5EditModeCases':13,'newR5RealBoardPlayModeCases':2,'unityCaptures':len(list((b/'QA/Journey').rglob('*.png'))),'limitations':['Native journeys use the legacy runtime; campaign integration is development-only.','Synthetic full-chapter fixtures are not authored content.','Memory interruption simulation and disk reopen are not phone power-loss tests.','No Android/IL2CPP campaign-save acceptance or full-project suite.']}
(b/'QA/Verification.json').write_text(json.dumps(summary,indent=2))
print(json.dumps(scope,indent=2));print(json.dumps(summary,indent=2))
