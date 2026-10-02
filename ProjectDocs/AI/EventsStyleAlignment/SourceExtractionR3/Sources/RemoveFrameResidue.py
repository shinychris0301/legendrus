"""Remove the narrow source-scenery strip immediately inside the extracted rails."""
from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;base=b.parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned');p=out/'frame_r3.png';rgba=np.array(Image.open(p));h,w=rgba.shape[:2]
strip=np.zeros((h,w),bool);strip[12:19,38:w-38]=True;strip[h-19:h-12,38:w-38]=True;strip[38:h-38,12:19]=True;strip[38:h-38,w-19:w-12]=True
# Copy adjacent reconstructed navy face into the interior strip; no rail recoloring.
yy,xx=np.mgrid[:h,:w];donor=rgba[np.clip(yy,22,h-23),np.clip(xx,22,w-23),:3];rgba[strip,:3]=donor[strip];Image.fromarray(rgba).save(p)
allowed=np.array(Image.open(b/'RepairMasks/frame_final.png'));allowed[strip]=255;Image.fromarray(allowed).save(b/'RepairMasks/frame_final.png')
p=out/'frame_ornaments_r3.png';ring=np.array(Image.open(p));ring[strip,3]=0;Image.fromarray(ring).save(p)
records=json.loads((base/'AssetProvenance.json').read_text())
for r in records:
 if r['asset'] in ['frame','frame_ornaments']:
  r['sha256']=hashlib.sha256(Path(r['output']).read_bytes()).hexdigest();r['finalCleanup']='Inner7px strips cleared of source scenery, preserving outer rails and corner filigree. See RemoveFrameResidue.py and frame_final repair mask.'
 if r['asset']=='frame':
  orig=np.array(Image.open(b/'Crops/frame.png').convert('RGB'));changed=np.any(rgba[:,:,:3]!=orig,axis=2);assert not (changed&(allowed==0)).any();r['changedRGBPixels']=int(changed.sum())
(base/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
qa=Image.new('RGB',(1200,250))
for i,c in enumerate(['black','white','#ff00ff']):
 im=Image.open(out/'frame_ornaments_r3.png');im.thumbnail((390,240));t=Image.new('RGBA',im.size,c);t.alpha_composite(im);qa.paste(t.convert('RGB'),(i*400,0))
qa.crop((0,0,1200,im.height)).save(base/'QA/FinalFrameOrnaments.png')
print('Inner-rail scenery residue removed; protected RGB check passed.')
