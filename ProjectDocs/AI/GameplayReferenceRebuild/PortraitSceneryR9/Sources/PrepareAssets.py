from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent
art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
dst=art/'GameplayPortraitSceneryR9';dst.mkdir(exist_ok=True)
base=Image.open(art/'GameplaySurfaceBoundariesR8/environment.png').convert('RGB')
patch=Image.open(b/'Scenario/portrait_scenery/booster_medallion_trial01.png').convert('RGB').resize((470,470),Image.Resampling.LANCZOS)
layer=base.copy();layer.paste(patch,(0,120))
# Replace the dark body across the former nameplate area, not just its lower
# seam. The upper blend is behind the portrait; the other edges blend gradually
# into the adjacent twilight. No hard rectangular paste or global recoloring.
y,x=np.indices((base.height,base.width),dtype=float)
def smooth(t):
 t=np.clip(t,0,1);return t*t*(3-2*t)
alpha=smooth((y-148)/52)*(1-smooth((y-320)/76))*(1-smooth((x-265)/120))
mask=Image.fromarray(np.round(alpha*255).astype('uint8'));mask.save(b/'EnvironmentRepairMask.png')
result=Image.composite(layer,base,mask);result.save(dst/'environment.png')
before=np.array(base);after=np.array(result);changed=np.any(before!=after,axis=2)
report={'newScenarioCalls':1,'billing':json.loads((b/'Scenario/portrait_scenery/Billing.json').read_text()),
 'changedPixels':int(changed.sum()),'unchangedPixels':int((~changed).sum()),
 'changedOutsideMask':int(np.count_nonzero(changed&(np.array(mask)==0))),
 'headerPortraitBoosterAssets':'Unchanged from R8',
 'method':'Replace the remaining dark scenery body with continuous blue-violet twilight from one Scenario crop edit. Bounded smooth composite; hidden scenery is reconstructed, not exact source recovery.'}
(b/'AssetProvenance.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
