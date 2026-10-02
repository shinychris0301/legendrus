from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib,shutil
b=Path('ProjectDocs/AI/SettingsStyleAlignment/LanguageModalR2');s=b/'Sources'
for folder in ['Inputs','RepairMasks','Final','Scenario']:(s/folder).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True);source=Path('/home/k2/Downloads/match3final/setting-language select.png');shutil.copy2(source,s/'OriginalReference.png');im=Image.open(source).convert('RGB')
boxes={'language_panel_ru':(179,573,760,1028),'language_selected':(240,765,695,868),'language_unselected':(240,888,695,991),'language_close':(693,605,764,681)};records=[]
for name,box in boxes.items():
 a=np.array(im.crop(box));mask=np.zeros(a.shape[:2],'uint8');h,w=mask.shape
 if name=='language_panel_ru':
  for x,y,x1,y1 in [(241,766,694,867),(241,889,694,990)]:mask[y-box[1]:y1-box[1],x-box[0]:x1-box[0]]=255
  # Clear the underlying Close image, which has its own independent button sprite.
  cv2.circle(mask,(727-box[0],643-box[1]),38,255,-1)
 elif name in ['language_selected','language_unselected']:
  hsv=cv2.cvtColor(a,cv2.COLOR_RGB2HSV);gold=((hsv[:,:,0]>10)&(hsv[:,:,0]<40)&(hsv[:,:,1]>65)&(hsv[:,:,2]>80)).astype('uint8')*255
  area=np.zeros_like(mask);area[22:80,106:343]=255;mask=cv2.dilate(cv2.bitwise_and(gold,area),np.ones((5,5),'uint8'))
 native=cv2.inpaint(a,mask,5,cv2.INPAINT_TELEA) if mask.any() else a
 Image.fromarray(native).save(s/'Inputs'/(name+'_native.png'));Image.fromarray(native).resize((w*2,h*2),Image.Resampling.LANCZOS).save(s/'Inputs'/(name+'.png'));Image.fromarray(mask).save(s/'RepairMasks'/(name+'.png'))
 records.append({'name':name,'bounds':box,'repairPixels':int((mask>0).sum()),'inputSha256':hashlib.sha256((s/'Inputs'/(name+'.png')).read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'original':str(source),'originalSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'approach':'Four Scenario source-alpha isolations; retain source RGB outside bounded control/lettering clearing. Russian modal title kept; English live title. Clean repaired panel under independently rendered buttons, no full reference screenshot runtime. Existing Settings R1 background/layout untouched.','assets':records},indent=2))
shutil.copy2('ProjectDocs/AI/SettingsStyleAlignment/R1/Sources/ScenarioRequests.py',s/'ScenarioRequests.py');shutil.copy2('/tmp/lor-settings-language-r2/prepare.py',s/'InitialPreparation.py')
entry='# 2026-09-18 — Settings language modal R2 started\n\nUser supplied the dedicated language-selection modal and asked to apply it on Settings. Found matching local `setting-language select.png`. Isolate panel, selected/unselected buttons and red Close through Scenario; preserve active Settings R1 page and existing language/Pause behavior. Snapshot and current dirty tree inspected. New Unity checks pending.\n\n'
for name in ['CurrentTask.md','SessionLog.md']:
 p=Path('ProjectDocs/AI')/name;p.write_text(entry+p.read_text())
print('Prepared four component inputs')
