from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;(b/'Inputs').mkdir(exist_ok=True);(b/'Crops').mkdir(exist_ok=True);(b/'RepairMasks').mkdir(exist_ok=True)
ref=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/Events.png').convert('RGB')
boxes={'title':(214,118,747,300),'tab':(332,306,613,380),'action':(674,972,899,1052),'status':(473,495,747,535),'frame':(20,1064,919,1341),'firebird':(38,392,426,712),'yaga':(39,742,423,1046),'merchant':(39,1078,422,1328),'knight':(39,1355,422,1607),'rewards_firebird':(439,633,670,710),'rewards_yaga':(440,974,665,1047),'rewards_merchant':(440,1258,666,1326),'rewards_knight':(440,1527,666,1607)}
records=[]
for name,box in boxes.items():
 im=ref.crop(box);im.save(b/'Crops'/(name+'.png'));rgb=np.array(im);h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8)
 if name in ['title','tab','action','status']:
  bounds={'title':(68, sixty:=64,458,153),'tab':(87,19,202,55),'action':(49,25,174,59),'status':(2,4,110,36)}[name]
  x0,y0,x1,y1=bounds;region=rgb[y0:y1,x0:x1].astype(float);gold=(region[:,:,0]>region[:,:,2]*1.28)&(region[:,:,0]>70)&(region[:,:,1]>40)
  mask[y0:y1,x0:x1]=gold.astype(np.uint8)*255;mask=cv2.dilate(mask,np.ones((5,5),np.uint8));fixed=cv2.inpaint(rgb,mask,4,cv2.INPAINT_TELEA)
 elif name=='frame':
  # Clear the whole composite interior before isolating only its source frame.
  # Original rails/corners are protected; restored face is source-texture reconstruction.
  mask[18:h-18,38:w-38]=255;mask[38:h-38,18:w-18]=255
  tile=np.array(ref.crop((765,1130,890,1174)));tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0)
  donor=np.tile(tile,(h//tile.shape[0]+1,w//tile.shape[1]+1,1))[:h,:w];fixed=rgb.copy();fixed[mask>0]=donor[mask>0]
 else:fixed=rgb.copy()
 Image.fromarray(mask).save(b/'RepairMasks'/(name+'.png'));Image.fromarray(fixed).save(b/'Inputs'/(name+'.png'))
 records.append({'name':name,'sourceBox':box,'classification':'source isolation with bounded live-label repair' if name in ['title','tab','action','status'] else 'source frame isolation with reconstructed empty face' if name=='frame' else 'original foreground isolation; discard scenery' if not name.startswith('rewards') else 'original reward examples isolation; no quantities or grants','inputSHA256':hashlib.sha256((b/'Inputs'/(name+'.png')).read_bytes()).hexdigest(),'localRGBRepairPixels':int((mask>0).sum()),'regenerationRequired':False})
(b/'AssetAssessment.json').write_text(json.dumps({'source':'Assets/LegendsOfRus/Resources/DesignScreens/Events.png','sourceSHA256':hashlib.sha256(Path('Assets/LegendsOfRus/Resources/DesignScreens/Events.png').read_bytes()).hexdigest(),'assets':records,'backgrounds':'Existing clean environment plates will be assessed for reuse behind isolated subjects. Original hidden scenery cannot be extracted.','rewardAuthorization':'User approved clearly labelled original reward examples; dates/actual recipes unconfirmed.'},indent=2)+'\n')
print('Prepared',len(records),'assessed source components; production untouched.')
