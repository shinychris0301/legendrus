from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib,cv2
b=Path(__file__).resolve().parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned');ref=Image.open('ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA').crop((184,1454,758,1644))
# Restore the original gold contour before clearing only the occupied interior.
box=(40,98,420,151);clean=ref.crop(box)
mask=Image.new('L',ref.size);ImageDraw.Draw(mask).polygon([(63,108),(393,108),(399,117),(404,125),(400,133),(393,140),(62,140),(56,135),(52,126),(56,117)],fill=255)
mask=mask.filter(ImageFilter.GaussianBlur(.35))
donor=ref.crop((292,111,381,139)).resize((353,33),Image.Resampling.BICUBIC)
surface=ref.copy();surface.paste(donor,(52,108));repaired=Image.composite(surface,ref,mask)
# Remove only the thin cyan antialias remnants of the old blue fill along the gold rails.
a=np.array(repaired);rgb=a[:,:,:3].astype(float);fringe=((rgb[:,:,2]>rgb[:,:,0]+6)&(rgb[:,:,2]>rgb[:,:,1]*.8)&(rgb[:,:,2]>10)).astype('uint8')*255
bounded=np.zeros(fringe.shape,np.uint8);bounded[98:151,40:420]=fringe[98:151,40:420];a[:,:,:3]=cv2.inpaint(a[:,:,:3],bounded,2,cv2.INPAINT_TELEA);repaired=Image.fromarray(a);Image.fromarray(bounded).save(b/'BlueFringeMask.png')
for n in ['source_quest_ru','source_quest_blank']:
 old=Image.open(b/'Before'/f'{n}.png').convert('RGBA');a=old.getchannel('A');old.paste(repaired.crop(box),box[:2]);old.putalpha(a);old.save(art/f'{n}.png')
mask.save(b/'InteriorMask.png')
(b/'RepairProvenance.json').write_text(json.dumps({'source':'../R1/Sources/OriginalReference.png','sourcePanelBounds':[184,1454,758,1644],'restoredRegionPanelCoordinates':box,'interiorDonorPanelCoordinates':[292,111,381,139],'method':'Restore original source RGB for complete progress track; source dark empty-track donor inside bounded polygon only. Thin blue edge remnants cleared with a bounded color mask. Existing alpha, labels, gem and panel decoration unchanged.','scenarioCalls':0,'scenarioCredits':0},indent=2))
for color,name in [('black','Black'),('white','White'),('#ff00ff','Magenta')]:
 canvas=Image.new('RGB',(574,400),color)
 for i,n in enumerate(['source_quest_ru','source_quest_blank']):
  im=Image.open(art/f'{n}.png');canvas.paste(im,(0,i*200),im)
 canvas.save(b.parent/'QA'/f'Alpha_{name}.png')
