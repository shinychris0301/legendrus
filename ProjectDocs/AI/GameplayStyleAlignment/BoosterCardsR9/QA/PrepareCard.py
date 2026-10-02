from pathlib import Path
from PIL import Image
from scipy import ndimage
import json,numpy as np,shutil,hashlib
root=Path.cwd();docs=root/'ProjectDocs/AI/GameplayStyleAlignment/BoosterCardsR9';job=json.loads((docs/'CardLabelWellCorrection.json').read_text())
source=docs/'Sources/card_wood_r9.png';shutil.copy2(job['path'],source);original=np.array(Image.open(source).convert('RGBA'));rgb=original[:,:,:3].astype('int16')
neutral=(rgb.max(2)-rgb.min(2)<35)&(rgb.min(2)>50);seeds=np.zeros(neutral.shape,bool);seeds[0,:]=seeds[-1,:]=True;seeds[:,0]=seeds[:,-1]=True
bg=ndimage.binary_propagation(seeds&neutral,mask=neutral);out=original.copy();out[:,:,3]=np.where(bg,0,255);bounds=Image.fromarray(out[:,:,3]).getbbox();assert bounds
x0,y0,x1,y1=bounds;x0=max(0,x0-4);x1=min(out.shape[1],x1+4);y0=max(0,y0-4);y1=min(out.shape[0],y1+4)
im=Image.fromarray(out[y0:y1,x0:x1]);target=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/card_wood_r9.png';im.save(target);assert np.array_equal(np.array(im)[:,:,:3],original[y0:y1,x0:x1,:3]);assert (np.array(im)[100:-100,100:-100,3]==255).all()
for label,color in [('black','#000000'),('white','#ffffff'),('magenta','#ed16cd')]:
 proof=Image.new('RGBA',im.size,color);proof.alpha_composite(im);proof.convert('RGB').save(docs/'QA'/f'card_wood_r9_{label}.png')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
record={'method':'Built-in imagegen, authorized local alpha-only checkerboard cleanup','source':str(source.relative_to(root)),'sourceSHA256':sha(source),'runtime':str(target.relative_to(root)),'runtimeSHA256':sha(target),'sourceDimensions':list(Image.open(source).size),'runtimeDimensions':list(im.size),'trimXYXY':[x0,y0,x1,y1],'rgbPreserved':True,'interiorWoodFullyOpaque':True,'alphaMask':'Edge-connected neutral pixels (RGB channel range<35, minimum>50); retained artwork alpha255, background0. Four-pixel margin when available.'}
(docs/'CardProvenance.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record))
