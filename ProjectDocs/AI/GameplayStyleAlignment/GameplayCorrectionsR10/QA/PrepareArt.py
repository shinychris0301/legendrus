from pathlib import Path
from PIL import Image,ImageDraw
from scipy import ndimage
import numpy as np,json,shutil,hashlib
root=Path.cwd();docs=root/'ProjectDocs/AI/GameplayStyleAlignment/GameplayCorrectionsR10';dst=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned';records=[]
for job in json.loads((docs/'SpecialGenerationPrompts.json').read_text()):shutil.copy2(job['path'],docs/'Sources'/f"{job['name']}.png")
proof=Image.new('RGB',(960,1600),'#202020');draw=ImageDraw.Draw(proof)
for i,p in enumerate(sorted((docs/'Sources').glob('*.png'))):
 a=np.array(Image.open(p).convert('RGBA'));out=a.copy();kind='generated alpha preserved'
 if a[:,:,3].min()==255:
  rgb=a[:,:,:3].astype('int16');neutral=(rgb.max(2)-rgb.min(2)<35)&(rgb.min(2)>50);seed=np.zeros(neutral.shape,bool);seed[0,:]=seed[-1,:]=True;seed[:,0]=seed[:,-1]=True
  if 'portrait' in p.stem:seed[a.shape[0]//2,a.shape[1]//2]=True
  bg=ndimage.binary_propagation(seed&neutral,mask=neutral);out[:,:,3]=np.where(bg,0,255);kind='authorized alpha-only edge-connected neutral checker cleanup, plus center seed for portrait'
 if p.stem.startswith('special_'):
  labels,count=ndimage.label(out[:,:,3]>8);sizes=np.bincount(labels.ravel());sizes[0]=0;main=labels==sizes.argmax();keep=ndimage.binary_dilation(main,iterations=3);out[:,:,3]=np.where(keep,out[:,:,3],0);kind+='; authorized alpha-only removal of disconnected specks'
 bounds=Image.fromarray(out[:,:,3]).getbbox();assert bounds
 x0,y0,x1,y1=bounds;x0=max(0,x0-4);y0=max(0,y0-4);x1=min(a.shape[1],x1+4);y1=min(a.shape[0],y1+4)
 im=Image.fromarray(out[y0:y1,x0:x1]);im.save(dst/p.name)
 assert np.array_equal(np.array(im)[:,:,:3],a[y0:y1,x0:x1,:3])
 records.append({'name':p.stem,'sourceSHA256':hashlib.sha256(p.read_bytes()).hexdigest(),'runtimeSHA256':hashlib.sha256((dst/p.name).read_bytes()).hexdigest(),'trim':[x0,y0,x1,y1],'dimensions':im.size,'method':kind,'retainedRGBIdentical':True})
 thumb=im.copy();thumb.thumbnail((290,130))
 for j,c in enumerate(['black','white','#ec14cd']):
  tile=Image.new('RGBA',(320,140),c);tile.alpha_composite(thumb,((320-thumb.width)//2,(140-thumb.height)//2));proof.paste(tile.convert('RGB'),(j*320,i*160+20))
 draw.text((8,i*160+4),p.stem,fill='white')
proof.save(docs/'QA/AlphaProofs.png');(docs/'ArtProvenance.json').write_text(json.dumps(records,indent=2)+'\n');print([(x['name'],x['dimensions']) for x in records])
