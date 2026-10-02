from pathlib import Path
from PIL import Image,ImageDraw,ImageOps
import numpy as np,cv2,json,hashlib
s=Path(__file__).resolve().parent;b=s.parent;dest=Path('Assets/LegendsOfRus/Resources/ProductionUI/TutorialAligned');dest.mkdir(exist_ok=True)
source=Image.open(s/'OriginalReference.png').convert('RGB');audit=[]
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name'];p=s/'Inputs'/f'{n}.png';assert hashlib.sha256(p.read_bytes()).hexdigest()==r['inputSha256'];im=Image.open(p);rgb=np.array(im);mask=np.array(Image.open(s/'RepairMasks'/f'{n}.png'));orig=np.array(source.crop(r['bounds']));assert np.array_equal(rgb[mask==0],orig[mask==0]);w,h=im.size
 # Use a clean source navy patch, tiled with mirrored edges, instead of stretching a panel donor.
 if n=='panel':
  rgb=orig.copy();mask[:]=0
  for xx,yy,ww,hh in [(258,94,372,79),(117,170,660,87),(40,276,803,1036)]:mask[yy:yy+hh,xx:xx+ww]=255
  tile=source.crop((84,1210,160,1320));tw,th=tile.size
  tile2=Image.new('RGB',(tw*2,th*2));tile2.paste(tile,(0,0));tile2.paste(ImageOps.mirror(tile),(tw,0));tile2.paste(ImageOps.flip(tile),(0,th));tile2.paste(ImageOps.flip(ImageOps.mirror(tile)),(tw,th))
  patch=Image.new('RGB',(w,h))
  for yy in range(0,h,th*2):
   for xx in range(0,w,tw*2):patch.paste(tile2,(xx,yy))
  rgb[mask>0]=np.array(patch)[mask>0]
 elif n=='next':
  mask[25:99,68:519]=255;rgb=cv2.inpaint(orig,mask,9,cv2.INPAINT_TELEA)
 a=np.array(Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
 if n=='panel':
  a[105:1327,42:842]=255
  rgb[80:265,760:885]=rgb[80:265,:125][:,::-1];mask[80:265,760:885]=255
  a[80:265,760:885]=a[80:265,:125][:,::-1]
  # Preserve crest, curved shoulders and carved perimeter. Exclude neighboring header scenery.
  keep=Image.new('L',(w,h));ImageDraw.Draw(keep).polygon([(0,124),(30,85),(172,81),(214,58),(308,29),(354,23),(438,0),(489,21),(554,27),(642,59),(672,83),(855,85),(w,124),(w,h),(0,h)],fill=255);a=np.minimum(a,np.array(keep));a[265:,856:]=0;a[265:,:19]=0;a[1339:]=0
 else:
  contours,_=cv2.findContours((a>128).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);solid=np.zeros_like(a);cv2.drawContours(solid,[max(contours,key=cv2.contourArea)],-1,255,-1);a=np.maximum(a,solid)
 if n=='close':
  keep=Image.new('L',(w,h));ImageDraw.Draw(keep).ellipse((0,0,118,122),fill=255);a=np.minimum(a,np.array(keep))
 Image.fromarray(mask).save(s/'RepairMasks'/f'{n}_final.png')
 assert np.array_equal(rgb[mask==0],orig[mask==0])
 out=Image.fromarray(np.dstack([rgb,a]));out.save(s/'Final'/f'{n}.png');out.save(dest/f'{n}.png');audit.append({**r,'runtime':str(dest/f'{n}.png'),'sha256':hashlib.sha256((dest/f'{n}.png').read_bytes()).hexdigest(),'sourceRgbOutsideRepairMaskVerified':True})
for label,c in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(1150,1400),c)
 for n,xy,maxsize in [('panel',(0,0),(885,1345)),('close',(900,20),(200,220)),('next',(900,280),(240,180))]:
  im=Image.open(dest/f'{n}.png');im.thumbnail(maxsize);sheet.paste(im,xy,im)
 sheet.save(b/'QA'/f'AssetsOn{label}.png')
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2)+'\n')
print('Prepared 3 Tutorial assets')
