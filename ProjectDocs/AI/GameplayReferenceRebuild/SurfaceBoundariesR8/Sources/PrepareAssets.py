from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter
import numpy as np, cv2, json

b=Path(__file__).resolve().parent
art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
dst=art/'GameplaySurfaceBoundariesR8';dst.mkdir(exist_ok=True)
source=Path('ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources')
report={'newScenarioCalls':1,'assets':{}}

# The user-identified translucent-looking strip is the rectangular R1 scenery
# infill boundary, not a live shadow. Use the one accepted Scenario seam repair
# only around that join; preserve the entire remaining plate byte-for-byte.
base=Image.open(art/'GameplayRefinedR2/environment.png').convert('RGB')
patch=Image.open(b/'Scenario/scenery_seam/booster_medallion_trial01.png').convert('RGB').resize((400,400),Image.Resampling.LANCZOS)
layer=base.copy();layer.paste(patch,(0,100))
mask=Image.new('L',base.size);d=ImageDraw.Draw(mask)
d.polygon([(0,196),(130,197),(245,213),(291,240),(295,298),(248,321),(141,342),(0,346)],fill=255)
mask=mask.filter(ImageFilter.GaussianBlur(17))
environment=Image.composite(layer,base,mask);environment.save(dst/'environment.png');mask.save(b/'EnvironmentRepairMask.png')
changed=np.any(np.array(environment)!=np.array(base),axis=2)
report['assets']['environment']={'changedPixels':int(changed.sum()),'unchangedPixels':int((~changed).sum()),
 'changedOutsideRepairMask':int(np.count_nonzero(changed&(np.array(mask)==0))),
 'method':'One Scenario repair of a 400px scenery crop, feathered only around the old rectangular portrait-underlay join. All pixels outside the recorded support remain unchanged.'}
report['scenarioBilling']=json.loads((b/'Scenario/scenery_seam/Billing.json').read_text())

# Repair the wood *inside* the original ornament. Previously the broad material
# rectangle painted over both inward top curls and left the sample badge's
# bottom arc. Source metal is restored over the material, without repainting it.
a=Image.open(art/'GameplayOrnamentsR5/card.png').convert('RGBA');old=np.array(a)
src=Image.open(source/'Inputs/card.png').convert('RGBA');alpha=a.getchannel('A')
# Restore top carving and its recesses; source hammer starts below this support.
restore=Image.new('L',a.size);d=ImageDraw.Draw(restore)
d.rectangle((0,0,191,34),fill=255)
d.polygon([(0,30),(47,30),(46,47),(38,51),(39,64),(27,77),(0,79)],fill=255)
d.polygon([(141,30),(191,30),(191,79),(165,77),(155,66),(155,55),(145,48)],fill=255)
a=Image.composite(src,a,restore)
# Remove the old count badge residue; this local repair sits below the live badge.
# Continue the same existing card wood, rather than introducing a differently
# scaled/darker board texture at the old badge's location.
wood=a.copy();wood.paste(a.crop((55,150,108,181)).resize((55,35)),(118,191))
repair=Image.new('L',a.size);ImageDraw.Draw(repair).rectangle((120,193,170,223),fill=255)
repair=repair.filter(ImageFilter.GaussianBlur(1));a=Image.composite(wood,a,repair)
# Complete the original horizontal divider through the region formerly occupied
# by the reference's stock badge. Its hidden section uses a source rail donor.
rail=src.crop((42,212,112,218));a.paste(rail.resize((146,6)),(24,212))
# Original lower carving and opaque caption face; avoid the previous inpaint
# that introduced a dark triangular smear beside the right curl.
lower=src.crop((0,224,192,288));a.paste(lower,(0,224))
# A clean, original dark caption-wood strip immediately before the word supplies
# the occupied center. This avoids feeding text shadows or corner gold into an
# inpaint fill. Keep the source curls and bottom rail outside the narrow support.
face=src.crop((46,223,53,248)).resize((110,28),Image.Resampling.BICUBIC)
layer=a.copy();layer.paste(face,(47,221))
caption=Image.new('L',a.size);ImageDraw.Draw(caption).rectangle((48,222,156,248),fill=255)
caption=caption.filter(ImageFilter.GaussianBlur(.6));a=Image.composite(layer,a,caption)
a.putalpha(alpha);a.save(dst/'card.png');restore.save(b/'RestoredCardOrnamentMask.png')
report['assets']['card']={'changedRGBPixels':int(np.any(np.array(a)[:,:,:3]!=old[:,:,:3],axis=2).sum()),
 'alphaUnchanged':bool(np.array_equal(np.array(a)[:,:,3],old[:,:,3])),
 'restoredTopCarvingPixels':int(np.count_nonzero(np.array(restore))),
 'method':'Original top/lower ornament RGB over bounded wood repair; source rail donor behind removed example badge; clean source caption wood replaces lettering. Hidden divider is reconstructed, not exact recovery.'}
for name in ['card']:
 a=Image.open(dst/f'{name}.png').convert('RGBA')
 for color,label in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
  bg=Image.new('RGBA',a.size,color+(255,));bg.alpha_composite(a);bg.save(b/f'{name}_Alpha{label}.png')
(b/'AssetProvenance.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
