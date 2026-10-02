from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib,cv2
b=Path(__file__).resolve().parent;dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned')
atlas=Image.open(b/'Inputs/controls.png').convert('RGBA');seg=Image.open(b/'Scenario/controls/booster_medallion_trial01.png').convert('RGBA').resize(atlas.size,Image.Resampling.LANCZOS)
bounds=json.loads((b/'Bounds.json').read_text());result=[]
for name,record in bounds.items():
 box=record['atlasBounds'];im=atlas.crop(box);alpha=seg.crop(box).getchannel('A');w,h=im.size
 # Bound source panel outlines to remove unrelated scrollwork in the crop corners.
 points=([(0,16),(10,1),(204,1),(214,12),(215,24),(223,28),(216,34),(214,46),(204,57),(12,57),(4,48),(2,36),(0,32)] if name=='sort' else [(0,20),(9,12),(15,2),(241,2),(253,13),(256,27),(249,41),(241,52),(16,52),(8,42),(0,36)])
 bound=Image.new('L',(w,h));ImageDraw.Draw(bound).polygon([(x*3,y*3) for x,y in points],fill=255)
 a=np.minimum(np.asarray(alpha),np.asarray(bound)).copy()
 # Restore opaque navy face that isolation may interpret as background.
 core=Image.new('L',(w,h));ImageDraw.Draw(core).rectangle((26*3,10*3,(w//3-23)*3,(h//3-12)*3),fill=255)
 a=np.maximum(a,np.asarray(core));im.putalpha(Image.fromarray(a))
 im.save(dst/(name+'_ru_r3.png'))
 # Blank only caption footprint, preserving all metalwork and original people pictogram.
 blank=im.copy();mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
 r=(58,16,206,43) if name=='sort' else (23,15,236,38)
 d.rectangle(tuple(v*3 for v in r),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(5))
 # Smooth interpolation between clean face strips above and below lettering.
 rgb=np.asarray(im).astype(float);top=13 if name=='sort' else 12;bottom=46 if name=='sort' else 42
 arow=np.mean(rgb[(top-1)*3:top*3],axis=0);brow=np.mean(rgb[bottom*3:(bottom+1)*3],axis=0)
 arow[:]=np.median(arow[65*3:190*3],axis=0);brow[:]=np.median(brow[65*3:190*3],axis=0)
 t=np.clip((np.arange(h)-top*3)/((bottom-top)*3),0,1)[:,None,None]
 donor=Image.fromarray(np.uint8(np.clip(arow[None,:,:]*(1-t)+brow[None,:,:]*t,0,255)))
 blank=Image.composite(donor,im,mask);blank.putalpha(im.getchannel('A'));blank.save(dst/(name+'_blank_r3.png'))
 result.extend([name+'_ru_r3',name+'_blank_r3'])
proof=Image.new('RGB',(1000,len(result)*160),(35,35,35));draw=ImageDraw.Draw(proof)
for j,name in enumerate(result):
 im=Image.open(dst/(name+'.png')).convert('RGBA');im.thumbnail((315,120),Image.Resampling.LANCZOS)
 for k,col in enumerate([(0,0,0),(255,255,255),(255,0,150)]):
  x=k*333;draw.rectangle((x,j*160,x+332,j*160+159),fill=col);proof.paste(im,(x+8,j*160+30),im)
 draw.text((8,j*160+5),name,fill=(170,170,170))
proof.save(b.parent/'QA/AssetAlpha.png')
(b/'AssetProvenance.json').write_text(json.dumps({'source':'OriginalReference.png','bounds':bounds,'model':'model_pixa-background-removal','notes':'Source RGB retained under Scenario alpha plus bounded corner exclusions and opaque core restoration. Blank caption donor surface approximate. Threefold upsampling adds no detail.','assets':{n:hashlib.sha256((dst/(n+'.png')).read_bytes()).hexdigest() for n in result}},indent=2))
