from pathlib import Path
import hashlib,json
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned')
prior=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
protected=[p for p in prior if p.endswith(('.png','.meta','.ttf','.otf')) or ('/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Profile.prefab'))]
changed=[p for p in protected if not Path(p).is_file() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=prior[p]]
assert not changed,changed
results={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'assets':{}}
for item in json.loads((b/'Sources/Provenance.json').read_text()):
 p=art/(item['name']+'.png');assert hashlib.sha256(p.read_bytes()).hexdigest()==item['sha256']
 a=np.array(Image.open(p).convert('RGBA'));assert np.any(a[:,:,3]>0)
 results['assets'][item['name']]={'size':[a.shape[1],a.shape[0]],'opaquePixels':int((a[:,:,3]==255).sum()),'transparentPixels':int((a[:,:,3]==0).sum()),'sha256MatchesProvenance':True}
results['preparedRgbChecks']={}
for item in json.loads((b/'Sources/Sheet.json').read_text()):
 source=Image.open(b/'Sources/Prepared'/(item['name']+'.png')).convert('RGB');source=source.resize((source.width*item['scale'],source.height*item['scale']),Image.Resampling.LANCZOS)
 final=np.array(Image.open(art/(item['name']+'.png')).convert('RGBA'));visible=final[:,:,3]>0
 assert np.array_equal(np.array(source)[visible],final[:,:,:3][visible]),item['name']
 results['preparedRgbChecks'][item['name']]='Final visible RGB equals the documented prepared source/donor composite, not necessarily untouched original pixels.'
results['billing']=json.loads((b/'Sources/Scenario/profile_surfaces/Billing.json').read_text())
(b/'QA/IntegrityChecks.json').write_text(json.dumps(results,indent=2)+'\n')
print('PASS: protected existing art, fonts, metadata and other page prefabs unchanged;',len(results['assets']),'new assets verified')
