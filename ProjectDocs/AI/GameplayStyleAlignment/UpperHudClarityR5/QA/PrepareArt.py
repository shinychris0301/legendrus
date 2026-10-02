from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib,shutil
root=Path.cwd();docs=root/'ProjectDocs/AI/GameplayStyleAlignment/UpperHudClarityR5';art=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned'
base=Path('/home/k2/.codex/generated_images/01a09624-160e-7f10-a32e-7cc91172628e')
sources={'goal_score_r5':'exec-ae7a3f52-74d0-4de9-8e2b-fadb78cadd60.png','goal_survival_r5':'exec-f7c6a5d2-720c-4774-b68c-f4854b896242.png'}
records=[]
for name,file in sources.items():
 src=base/file;copy=docs/'Sources'/f'{name}.png';shutil.copy2(src,copy)
 image=Image.open(copy).convert('RGBA');pixels=np.array(image);a=pixels[:,:,3].copy()
 # Existing explicit authorization permits local transparency cleanup. These
 # outputs already have true exterior alpha; restore near-opaque artwork only.
 pixels[:,:,3][a>=240]=255
 pixels[:,:,3][a<4]=0 # Remove sub-1.6% exterior specks from the generated alpha.
 image=Image.fromarray(pixels);bounds=image.getchannel('A').getbbox();assert bounds
 # Remove only fully transparent canvas margins, never component artwork.
 final=image.crop(bounds);out=art/f'{name}.png';final.save(out)
 old=np.array(Image.open(copy).convert('RGBA'))[bounds[1]:bounds[3],bounds[0]:bounds[2]]
 assert np.array_equal(np.array(final)[:,:,:3],old[:,:,:3])
 for color,label in [('#000000','black'),('#ffffff','white'),('#ed16cd','magenta')]:
  proof=Image.new('RGBA',final.size,color);proof.alpha_composite(final);proof.convert('RGB').save(docs/'QA'/f'{name}_{label}.png')
 records.append({'name':name,'source':str(src),'sourceCopy':str(copy.relative_to(root)),'sourceSHA256':hashlib.sha256(copy.read_bytes()).hexdigest(),'runtime':str(out.relative_to(root)),'runtimeSHA256':hashlib.sha256(out.read_bytes()).hexdigest(),'sourceDimensions':list(Image.open(copy).size),'transparentMarginTrim':list(bounds),'runtimeDimensions':list(final.size),'alphaOnlyCleanup':'Values >=240 restored to opaque 255; values <4 cleared as faint alpha specks; intermediate alpha preserved. Source RGB exact inside retained bounds.','opaquePixelsRestored':int(((a>=240)&(a<255)).sum())})
(docs/'GeneratedAssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n');print(json.dumps(records,indent=2))
