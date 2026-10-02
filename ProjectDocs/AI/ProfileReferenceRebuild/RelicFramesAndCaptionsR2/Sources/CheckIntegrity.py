from pathlib import Path
from PIL import Image
import hashlib,json,numpy as np,sys
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.otf','.ttf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Profile.prefab')]
assert all(Path(p).is_file() and hashlib.sha256(Path(p).read_bytes()).hexdigest()==before[p] for p in protected)
sheet=Image.open(b/'Sources/Sheet.png').convert('RGB');record={'protectedExistingFiles':len(protected),'changedProtectedFiles':[],'visibleRgbChecks':{}}
for item in json.loads((b/'Sources/Provenance.json').read_text()):
 p=art/(item['name']+'.png');assert hashlib.sha256(p.read_bytes()).hexdigest()==item['sha256'];a=np.array(Image.open(p).convert('RGBA'));src=np.array(sheet.crop(item['sheetBox']));visible=a[:,:,3]>0;assert np.array_equal(a[:,:,:3][visible],src[visible]);record['visibleRgbChecks'][item['name']]={'unchangedPreparedRgb':True,'visiblePixels':int(visible.sum())}
run=Path('/tmp/lor-profile-relic-r2')/sys.argv[1];old=b.parent/'R1/UnityCaptures';record['nativeComparisons']={}
for name in ('Profile_941x1672_RU.png','Profile_941x1672_EN.png','Profile_Items_RU.png','Profile_History_RU.png'):
 a=np.array(Image.open(old/name));c=np.array(Image.open(run/'Profile'/name));d=np.any(a!=c,axis=2);record['nativeComparisons'][name]={'changedPixels':int(d.sum()),'top677ChangedPixels':int(d[:677].sum()),'footerFrom1527ChangedPixels':int(d[1527:].sum())}
record['billing']=json.loads((b/'Sources/Scenario/relic_frame/Billing.json').read_text());(b/'QA/IntegrityChecks.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
