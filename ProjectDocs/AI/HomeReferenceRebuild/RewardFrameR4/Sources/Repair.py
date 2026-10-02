from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned');ref=Image.open('ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA').crop((184,1454,758,1644))
# Restore the reward plaque from original RGB, including its complete lower rail.
box=(434,118,549,168)
# Repair only the old painted amount. A clean side-face donor cannot pull cyan from the gem.
mask=Image.new('L',ref.size);ImageDraw.Draw(mask).rectangle((452,123,522,155),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(1))
# Limit feather support to the unframed interior and preserve the gem tip/glow above it.
a=np.array(mask);a[:121]=0;a[158:]=0;mask=Image.fromarray(a)
surface=ref.copy();donor=ref.crop((523,120,534,149)).resize((79,39),Image.Resampling.BICUBIC);surface.paste(donor,(448,120));repair=Image.composite(surface,ref,mask)
for n in ['source_quest_ru','source_quest_blank']:
 im=Image.open(b/'Before'/f'{n}.png').convert('RGBA');alpha=im.getchannel('A');im.paste(repair.crop(box),box[:2]);im.putalpha(alpha);im.save(art/f'{n}.png')
mask.save(b/'AmountRepairMask.png')
(b/'RepairProvenance.json').write_text(json.dumps({'source':'ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/OriginalReference.png','sourcePanelBounds':[184,1454,758,1644],'restoredRegionPanelCoordinates':box,'donorPanelCoordinates':[523,120,534,149],'method':'Original lower reward plaque restored, amount only repaired with a clean original side-face donor; existing alpha unchanged.','scenarioCalls':0,'scenarioCredits':0},indent=2))
for color,name in [('black','Black'),('white','White'),('#ff00ff','Magenta')]:
 canvas=Image.new('RGB',(574,400),color)
 for i,n in enumerate(['source_quest_ru','source_quest_blank']):
  im=Image.open(art/f'{n}.png');canvas.paste(im,(0,i*200),im)
 canvas.save(b.parent/'QA'/f'Alpha_{name}.png')
Image.open(art/'source_quest_ru.png').crop((426,26,557,176)).resize((524,600)).save(b.parent/'QA/RewardDetail_Asset.png')
