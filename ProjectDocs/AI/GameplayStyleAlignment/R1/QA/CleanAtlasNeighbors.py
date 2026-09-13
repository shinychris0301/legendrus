"""User-authorized local transparency cleanup of four isolated sheet elements; no RGB repainting."""
from pathlib import Path
import re,json
import numpy as np
from PIL import Image
from scipy.ndimage import label,binary_dilation,distance_transform_edt
root=Path(__file__).resolve().parents[5];doc=root/'ProjectDocs/AI/GameplayStyleAlignment/R1';art=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned'
regions=json.loads((doc/'AtlasRegions.json').read_text());records=[]
for entry in regions:
 n=entry['name']
 if n not in ['seapearl','feather','column','color']:continue
 x,y,w,h=entry['topLeftRect'];src=np.array(Image.open(doc/'Sources'/(entry['sheet']+'.png')).convert('RGBA'))[y:y+h,x:x+w].copy();out=src.copy()
 labs,num=label(src[:,:,3]>100);counts=np.bincount(labs.ravel());counts[0]=0
 nearest=distance_transform_edt(labs==0,return_distances=False,return_indices=True);ownership=labs[tuple(nearest)];keep=ownership==counts.argmax()
 # Restore alpha only for the central connected asset; remove disconnected neighbours.
 out[:,:,3]=np.where(keep,src[:,:,3],0);Image.fromarray(out).save(art/('isolated_'+n+'.png'))
 records.append({'name':n,'sourceSheet':entry['sheet'],'sourceRect':entry['topLeftRect'],'removedAlphaPixels':int(np.count_nonzero(src[:,:,3]!=out[:,:,3])),'rgbUnchanged':bool(np.array_equal(src[:,:,:3],out[:,:,:3])),'method':'Retain central largest opaque alpha component and its nearest soft fringe; preserve original RGB and retained alpha; no resampling'})
 piece=Image.fromarray(out);piece.thumbnail((240,240));proof=Image.new('RGB',(720,240))
 for j,c in enumerate(['black','white','#ff00d8']):
  bg=Image.new('RGBA',(240,240),c);bg.alpha_composite(piece,((240-piece.width)//2,(240-piece.height)//2));proof.paste(bg.convert('RGB'),(j*240,0))
 proof.save(doc/'QA'/('isolated_'+n+'_alpha.png'))
(doc/'TransparencyCleanup.json').write_text(json.dumps(records,indent=2)+'\n');print(records)
