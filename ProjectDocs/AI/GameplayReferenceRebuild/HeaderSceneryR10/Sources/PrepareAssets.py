from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,cv2
b=Path(__file__).resolve().parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
dst=art/'GameplayHeaderSceneryR10';dst.mkdir(exist_ok=True)
base=Image.open(art/'GameplayPortraitSceneryR9/environment.png').convert('RGB')
source=Image.open('ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGB')
plate=source.copy()
# Restore actual visible sky beneath the whole header. The only source UI in
# this strip is the level plaque: mask it out, retain source sky around it and
# use already accepted Scenario sky only for its genuinely hidden background.
hidden=Image.new('L',base.size)
# Include the complete source plaque and its shadow, not its already-trimmed
# runtime alpha, which omits some source metal and would contaminate the fill.
ImageDraw.Draw(hidden).rectangle((395,225,614,286),fill=255)
generated=Image.open('ProjectDocs/AI/GameplayReferenceRebuild/PortraitSceneryR9/Sources/Scenario/portrait_scenery/booster_medallion_trial01.png').convert('RGB').resize((470,470),Image.Resampling.LANCZOS)
donor=generated.crop((259,86,470,139)).resize((219,61),Image.Resampling.LANCZOS)
# Match the donor to the surrounding original sky in gradient space; a direct
# alpha paste leaves a brighter plaque-shaped patch around the live ornament.
localMask=np.full((61,219),255,np.uint8)
destination=source.copy();destination.paste(base.crop((390,220,620,240)),(390,220))
cloned=cv2.seamlessClone(np.array(donor),np.array(destination),localMask,(504,255),cv2.NORMAL_CLONE)
plate=Image.composite(Image.fromarray(cloned),source,hidden)
y,x=np.indices((base.height,base.width),dtype=float)
def smooth(t):
 t=np.clip(t,0,1);return t*t*(3-2*t)
# Reference moves/goals ornaments end before y239; score stars extend lower.
# Begin below each source silhouette, not through its metal edge.
top=np.where(x<724,237,246)
alpha=smooth((x-265)/65)*smooth((y-top)/5)*(1-smooth((y-330)/36))
mask=Image.fromarray(np.round(alpha*255).astype('uint8'))
result=Image.composite(plate,base,mask);result.save(dst/'environment.png')
mask.save(b/'EnvironmentRepairMask.png');hidden.save(b/'SourcePlaqueExclusion.png')
old=np.array(base);new=np.array(result);original=np.array(source);changed=np.any(old!=new,axis=2)
visible=(np.array(mask)==255)&(np.array(hidden)==0)
assert np.array_equal(new[visible],original[visible])
report={'newScenarioCalls':0,'newCU':0,'changedPixels':int(changed.sum()),'unchangedPixels':int((~changed).sum()),
 'sourceSkyPixelsRestoredExactly':int(visible.sum()),'changesOutsideMask':int(np.count_nonzero(changed&(np.array(mask)==0))),
 'method':'Restore original visible sky below moves/goals/score and around the level plaque; exclude the complete source plaque and shadow and fill only its hidden background using existing R9 Scenario sky. Smooth join to R9 portrait scenery. UI sprites/layout/booster repairs unchanged.'}
(b/'AssetProvenance.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
