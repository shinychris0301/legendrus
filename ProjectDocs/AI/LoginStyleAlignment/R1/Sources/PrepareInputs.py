from pathlib import Path
from PIL import Image
import numpy as np,cv2,hashlib,json,shutil
b=Path('ProjectDocs/AI/LoginStyleAlignment/R1');s=b/'Sources'
for n in ['Inputs','RepairMasks','Final','Scenario']:(s/n).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True);source=Path('/home/k2/Downloads/match3final/4.login.png');shutil.copy2(source,s/'OriginalReference.png');im=Image.open(source).convert('RGB')
boxes={'subject':(0,393,941,1270),'panel':(133,794,813,1672),'field':(199,940,738,1005),'login':(210,1098,731,1200),'guest':(208,1335,733,1429),'link':(327,1431,616,1482),'user':(220,946,257,989),'lock':(222,1030,253,1074),'eye':(671,1034,721,1070),'vk':(265,1248,344,1323),'ok':(372,1248,451,1323),'mail':(484,1248,564,1323),'google':(596,1248,675,1323),'checkbox':(253,1490,304,1536),'frame':(0,0,941,1672)}
records=[]
for name,box in boxes.items():
 a=np.array(im.crop(box));mask=np.zeros(a.shape[:2],np.uint8);h,w=mask.shape
 if name=='panel':
  # Keep the wood surround and paper border. Clear all embedded form controls with an approved clean parchment donor.
  mask[144:767,56:622]=255
  donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/paper.png').convert('RGB') if Path('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/paper.png').exists() else None
  if donor is None:
   paths=list(Path('Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned').glob('paper.png'));donor=Image.open(paths[0]).convert('RGB')
  # Parchment donor is recorded; retain original source wood/frame/header outside this mask.
  a[144:767,56:622]=np.array(donor.resize((566,623),Image.Resampling.LANCZOS))
 elif name=='field':mask[7:58,15:518]=255;a=cv2.inpaint(a,mask,7,cv2.INPAINT_TELEA)
 elif name=='login':mask[22:77,161:359]=255;a=cv2.inpaint(a,mask,7,cv2.INPAINT_TELEA)
 elif name=='guest':mask[26:69,63:460]=255;a=cv2.inpaint(a,mask,7,cv2.INPAINT_TELEA)
 elif name=='link':mask[9:38,64:276]=255;a=cv2.inpaint(a,mask,5,cv2.INPAINT_TELEA)
 elif name=='checkbox':mask[8:43,8:44]=255;a=cv2.inpaint(a,mask,5,cv2.INPAINT_TELEA)
 elif name=='subject':
  # Cover the foreground form so isolation targets only Ivan and the Firebird. Those occluded pixels are never claimed as recovered.
  cv2.rectangle(mask,(133,450),(813,h),255,-1);a[mask>0]=0
 elif name=='frame':
  mask[110:1200,130:820]=255;mask[225:1440,76:864]=255;mask[1360:1640,135:816]=255;a[mask>0]=0
 Image.fromarray(a).save(s/'Inputs'/(name+'_native.png'));factor=1 if name in ['subject','panel','frame'] else 2
 Image.fromarray(a).resize((w*factor,h*factor),Image.Resampling.LANCZOS).save(s/'Inputs'/(name+'.png'));Image.fromarray(mask).save(s/'RepairMasks'/(name+'.png'))
 records.append({'name':name,'bounds':box,'repairPixels':int((mask>0).sum()),'inputSha256':hashlib.sha256((s/'Inputs'/(name+'.png')).read_bytes()).hexdigest(),'factor':factor})
(s/'Assessment.json').write_text(json.dumps({'source':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'assets':records,'reuse':['approved Home logo','clean FrontEnd dusk environment','existing fonts and Back control'],'method':'Scenario alpha isolation with original RGB outside explicit repairs. Hidden paper uses approved clean donor. Subject is occluded behind the independently rendered panel; no hidden anatomy recovery. No full reference runtime.'},indent=2))
shutil.copy2('ProjectDocs/AI/SettingsStyleAlignment/LanguageModalR2/Sources/ScenarioRequests.py',s/'ScenarioRequests.py');shutil.copy2('/tmp/lor-login-r1/prepare.py',s/'PrepareInputs.py')
print('Prepared',len(records),'source isolation inputs')
