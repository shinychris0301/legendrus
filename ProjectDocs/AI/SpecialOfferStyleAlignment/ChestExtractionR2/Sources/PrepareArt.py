"""Fresh Scenario alpha, source RGB, traced price-plaque exclusion; no reused art/matte."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageChops
import hashlib,json,numpy as np
root=Path.cwd(); base=root/'ProjectDocs/AI/SpecialOfferStyleAlignment/ChestExtractionR2'
source=Image.open(base/'Sources/OriginalChestRegion.png').convert('RGB')
alpha=Image.open(base/'Sources/Scenario/booster_medallion_trial01.png').convert('RGBA').getchannel('A')
assert alpha.size==source.size
# Follow the visible underside of coins/chest, excluding the attached price plaque.
edge=[(0,260),(70,259),(97,249),(115,246),(121,256),(133,260),(151,260),(165,254),(194,259),(225,262),(262,262),(274,256),(290,255),(305,249),(314,256),(335,260),(360,251),(387,240)]
scale=4;mask=Image.new('L',(source.width*scale,source.height*scale));d=ImageDraw.Draw(mask)
d.polygon([(x*scale,y*scale) for x,y in [(0,0),(387,0)]+list(reversed(edge))],fill=255)
mask=mask.resize(source.size,Image.Resampling.LANCZOS)
alpha=ImageChops.multiply(alpha,mask)
alpha.save(base/'Sources/ChestAlpha.png')
rgba=source.convert('RGBA');rgba.putalpha(alpha)
bounds=alpha.getbbox();rgba=rgba.crop(bounds)
out=root/'Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned/chest_r2.png';rgba.save(out)
a=np.asarray(rgba);orig=np.asarray(source.crop(bounds));assert np.array_equal(a[:,:,:3],orig)
qa=base/'QA';qa.mkdir(exist_ok=True)
for name,color in [('Black','black'),('White','white'),('Magenta','#ff00b0')]:
 bg=Image.new('RGBA',rgba.size,color);bg.alpha_composite(rgba);bg.convert('RGB').resize((rgba.width*2,rgba.height*2)).save(qa/f'ChestOn{name}.png')
manifest={'source':'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Shop.png','sourceRegion':[490,453,877,782],'trimBounds':bounds,'scenarioJob':'job_8trr4FcfDkdMCccxSiZh5DmZ','model':'model_pixa-background-removal','cuCost':3,'runtime':str(out.relative_to(root)),'sha256':hashlib.sha256(out.read_bytes()).hexdigest(),'originalRGBExact':True,'localEdit':'New alpha mask excludes price plaque along visible chest/coin contour. No hidden pixels reconstructed; minor source occlusion remains.','previousSpriteOrMaskReused':False}
(base/'Sources/RuntimeProvenance.json').write_text(json.dumps(manifest,indent=2)+'\n')
print(json.dumps(manifest,indent=2))
