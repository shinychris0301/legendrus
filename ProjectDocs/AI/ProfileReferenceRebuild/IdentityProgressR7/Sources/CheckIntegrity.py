from pathlib import Path
from PIL import Image
import hashlib,json,numpy as np,sys
b=Path(__file__).resolve().parents[1];prior=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());protected=[p for p in prior if p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Profile.prefab')]
assert all(Path(p).is_file() and hashlib.sha256(Path(p).read_bytes()).hexdigest()==prior[p] for p in protected)
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':[],'fontRoles':json.loads((b/'Sources/TypographyRoles.json').read_text()),'comparisons':{}}
run=Path('/tmp/lor-profile-progress-r7')/sys.argv[1];old=b.parent/'RelicSelectionR6/UnityCaptures'
for name in ('Profile_941x1672_RU.png','Profile_941x1672_EN.png','Profile_Items_RU.png','Profile_History_RU.png'):
 a=np.array(Image.open(old/name));c=np.array(Image.open(run/'Profile'/name));d=np.any(a!=c,axis=2);report['comparisons'][name]={'totalChangedPixels':int(d.sum()),'headerAndTitleTop217ChangedPixels':int(d[:217].sum())}
for name in ('Profile_941x1672_RU.png','Profile_941x1672_EN.png','Profile_Items_RU.png','Profile_History_RU.png'):
 a=np.array(Image.open(old/name));c=np.array(Image.open(run/'Profile'/name));d=np.any(a!=c,axis=2)
 d[319:364,530:885]=False
 report['comparisons'][name]['outsideIdentityProgressChangedPixels']=int(d.sum())
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
