from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');records={}
for name,old,ref,box,points in [
 ('letter_surface_r3','letter_frame','Letter',(10,288,932,1531),[(16,16),(112,16),(112,36),(90,42),(76,49),(60,65),(47,93),(22,112),(16,112)]),
 ('preview_surface_r3','preview_paper_r2','Inbox',(48,1032,894,1476),[(0,0),(85,0),(85,15),(63,22),(48,31),(34,45),(24,72),(0,87)])]:
 original=Image.open(b/(ref+'.png')).convert('RGB').crop(box);w,h=original.size;previous=Image.open(out/(old+'.png')).convert('RGBA');before=np.array(previous);updated=previous.copy()
 total=Image.new('L',previous.size)
 for side in ['tl','tr','bl','br']:
  pts=[(w-x if 'r' in side else x,h-y if 'b' in side else y) for x,y in points]
  mask=Image.new('L',(w,h));ImageDraw.Draw(mask).polygon(pts,fill=255)
  mask=mask.resize(previous.size,Image.Resampling.LANCZOS).filter(ImageFilter.GaussianBlur(5))
  # The original bottom of the large letter is covered by Back/Inbox controls;
  # use the already established mirrored upper-corner repair, now with its whole scrollwork.
  donor=original.transpose(Image.Transpose.FLIP_TOP_BOTTOM) if name=='letter_surface_r3' and 'b' in side else original
  donor=donor.resize(previous.size,Image.Resampling.LANCZOS).convert('RGBA')
  updated=Image.composite(donor,updated,mask);total=Image.fromarray(np.maximum(np.array(total),np.array(mask)))
 updated.putalpha(previous.getchannel('A'));updated.save(out/(name+'.png'));total.save(b/(name+'_RepairMask.png'))
 after=np.array(updated);assert np.array_equal(before[:,:,3],after[:,:,3]);assert np.array_equal(before[np.array(total)==0],after[np.array(total)==0])
 records[name]={'source':ref+'.png','bounds':box,'previous':old+'.png','method':'Restore original inward scrollwork over erroneous rectangular fill. Feather only the bounded collar into approved clean paper. Large-letter lower corners mirror source upper corners to avoid original footer controls.','alphaUnchanged':True,'outsideMaskUnchanged':True,'changedPixels':int(np.any(before!=after,axis=2).sum()),'sha256':hashlib.sha256((out/(name+'.png')).read_bytes()).hexdigest()}
 # Detail comparison for QA; not a production implementation preview.
 diag=Image.new('RGB',(1080,360),(20,28,37))
 for i,im in enumerate([previous,updated,original.resize(previous.size,Image.Resampling.LANCZOS)]):
  c=im.crop((0,0,360,360)).convert('RGBA');diag.paste(c,(i*360,0),c)
 diag.save(b.parent/'QA'/(name+'_CornerBeforeAfterSource.png'))
(b/'PanelSurfaceRepairs.json').write_text(json.dumps(records,indent=2));print('Restored both panel corner contours; alpha and outside-mask pixels unchanged.')
