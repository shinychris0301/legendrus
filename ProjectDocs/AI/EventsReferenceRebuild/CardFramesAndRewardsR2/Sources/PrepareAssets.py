"""Reuse Scenario-isolated source sprites. Remove opaque corner residue and trim reward icons.
No RGB generation, new API calls or inferred reward functionality.
"""
from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib
base=Path(__file__).resolve().parent.parent
art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned');records=[]
p=art/'frame_ornaments_r3.png';im=np.array(Image.open(p).convert('RGBA'));rgb=im[:,:,:3].astype(float);a=im[:,:,3].copy();h,w=a.shape
# The old service mask includes opaque rectangular navy pockets, and scenery below rails.
# Preserve the gilding and a one-pixel dark bevel; clear only perimeter matte residue.
gold=(rgb[:,:,0]>55)&(rgb[:,:,0]>rgb[:,:,2]*1.45)&(rgb[:,:,1]>rgb[:,:,2]*1.18)&(rgb[:,:,1]>24)
metal=cv2.dilate(gold.astype(np.uint8),np.ones((3,3),np.uint8))
metal=cv2.GaussianBlur(metal.astype(float),(3,3),.45)
a=np.minimum(a,np.round(metal*255).astype(np.uint8));im[:,:,3]=a
out=art/'frame_ornaments_r5.png';Image.fromarray(im).save(out)
records.append({'output':str(out),'source':str(p),'operation':'Original Scenario frame mask refined by source gold/bevel boundary; RGB unchanged','changedAlphaPixels':int((a!=np.array(Image.open(p))[:,:,3]).sum()),'RGBMismatches':0,'sha256':hashlib.sha256(out.read_bytes()).hexdigest()})
canvas=Image.new('RGB',(899*3,277))
for i,c in enumerate(['black','white','#ff00ff']):
 bg=Image.new('RGBA',(899,277),c);bg.alpha_composite(Image.fromarray(im));canvas.paste(bg.convert('RGB'),(i*899,0))
canvas.save(base/'QA/FrameAlpha.png')
ranges={'firebird':[(0,66),(66,155),(155,231)],'yaga':[(0,76),(76,150),(150,225)],'knight':[(0,75),(75,148),(148,226)]}
for name in ['firebird','yaga','merchant','knight']:
 for i in range(3):
  if name=='merchant' and i!=1:
   source=art/('reward_merchant_'+('chest' if i==0 else 'medal')+'_r3.png');src=Image.open(source).convert('RGBA');box=(0,0,*src.size)
  else:
   source=art/('rewards_'+name+'_r3.png');src=Image.open(source).convert('RGBA');xs=(76,156) if name=='merchant' else ranges[name][i];box=(xs[0],0,xs[1],src.height)
  icon=src.crop(box);bounds=icon.getbbox();assert bounds
  icon=icon.crop(bounds);out=art/(f'reward_{name}_{i}_r5.png');icon.save(out)
  records.append({'output':str(out),'source':str(source),'cropBox':box,'trimBox':bounds,'dimensions':icon.size,'operation':'Exact RGBA crop of existing Scenario source-isolated reward; no painting or resampling','sha256':hashlib.sha256(out.read_bytes()).hexdigest()})
(base/'QA/AssetProvenance.json').write_text(json.dumps({'newScenarioRequests':0,'cuCost':0,'assets':records},indent=2)+'\n')
canvas=Image.new('RGB',(900,460),'#333333')
for r,name in enumerate(['firebird','yaga','merchant','knight']):
 for c,color in enumerate(['black','white','#ff00ff']):
  plate=Image.new('RGBA',(300,110),color)
  for i in range(3):
   icon=Image.open(art/(f'reward_{name}_{i}_r5.png'));plate.alpha_composite(icon,(i*100+5,10))
  canvas.paste(plate.convert('RGB'),(c*300,r*115))
canvas.save(base/'QA/RewardAlpha.png')
print('Prepared one refined frame and twelve tightly trimmed source reward icons; no new Scenario requests.')
