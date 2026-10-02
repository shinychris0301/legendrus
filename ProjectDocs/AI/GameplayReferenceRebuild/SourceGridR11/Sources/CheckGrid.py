from pathlib import Path
from PIL import Image
import json,numpy as np
b=Path(__file__).resolve().parent.parent;q=b/'QA'
ref=Image.open(b.parent/'R1/Sources/OriginalReference.png').convert('RGB')
after=Image.open(q/'Gameplay/Gameplay_941x1672_RU.png').convert('RGB')
before=Image.open(q/'Before.png').convert('RGB')
for name,im in [('After',after),('Before',before),('Reference',ref)]:
 im.crop((65,485,372,808)).resize((614,646),Image.Resampling.NEAREST).save(q/f'Board{name}.png')
after.crop((159,581,179,605)).resize((240,288),Image.Resampling.NEAREST).save(q/'JointAfter.png')
ref.crop((168,582,188,606)).resize((240,288),Image.Resampling.NEAREST).save(q/'JointReference.png')
atlas=np.asarray(Image.open('Assets/LegendsOfRus/Resources/ProductionUI/GameplaySourceGridR11/grid.png'))
provenance=json.loads((b/'Sources/AssetProvenance.json').read_text());parts={}
for name,part in provenance['parts'].items():
 x,y=part['atlasTopLeft'];w,h=part['size'];a=atlas[y:y+h,x:x+w];r=np.asarray(ref.crop(part['sourceBox']))
 parts[name]={'RGBMatchesOriginal':bool(np.array_equal(a[:,:,:3],r)), 'hasTransparentPixels':bool((a[:,:,3]==0).any()),'hasOpaquePixels':bool((a[:,:,3]==255).any())}
 assert all(parts[name].values()),name
changed=np.any(np.asarray(after)!=np.asarray(before),axis=2)
# Whole screenshot comparison is diagnostic; fixtures may vary outside the board.
y,x=np.where(changed)
(q/'SourceGridChecks.json').write_text(json.dumps({'parts':parts,'nativeChangedPixelBounds':[int(x.min()),int(y.min()),int(x.max()+1),int(y.max()+1)],'changedPixelCount':int(changed.sum()),'nativeGridJunctionCount':72},indent=2))
print((q/'SourceGridChecks.json').read_text())
