from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent.parent;root=Path.cwd();before=json.loads((b/'Sources/BeforeHashes.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};changed=[p for p,h in before.items() if after.get(p)!=h];checks=[]
ref=np.array(Image.open('ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA').crop((184,1454,758,1644)))
for n in ['source_quest_ru','source_quest_blank']:
 a=np.array(Image.open(b/'Sources/Before'/f'{n}.png'));z=np.array(Image.open(f'Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/{n}.png'));mask=np.zeros(a.shape[:2],bool);mask[118:168,434:549]=True
 assert np.array_equal(a[:,:,3],z[:,:,3]);assert np.array_equal(a[~mask],z[~mask]);assert np.array_equal(z[158:166,434:549,:3],ref[158:166,434:549,:3]);assert np.array_equal(z[30:118,435:549],a[30:118,435:549])
 checks.append({'sprite':n,'alphaAndDimensionsUnchanged':True,'outsideRepairUnchanged':True,'gemUnchanged':True,'lowerBorderOriginalRGBExact':True})
assert len(changed)==2,changed
(b/'QA/ScopeAndPixelAudit.json').write_text(json.dumps({'changed':changed,'new':[p for p in after if p not in before],'deleted':[p for p in before if p not in after],'checks':checks},indent=2));print('Scope and pixel checks passed')
