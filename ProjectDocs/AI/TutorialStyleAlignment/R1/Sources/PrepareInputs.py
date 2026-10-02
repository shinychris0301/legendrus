from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib,shutil
b=Path('ProjectDocs/AI/TutorialStyleAlignment/R1');s=b/'Sources';src=Image.open('/home/k2/Downloads/match3final/tuturial.png').convert('RGB');shutil.copy2('/home/k2/Downloads/match3final/tuturial.png',s/'OriginalReference.png')
# Opaque navy donor already used by approved source-derived panels.
donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned/language_panel_blank.png').convert('RGB');donor=donor.crop((60,330,1050,690))
assets=[]
for name,box in [('panel',(32,104,917,1449)),('close',(799,160,918,283)),('next',(173,1263,770,1388))]:
 rgb=np.array(src.crop(box));h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8)
 if name=='panel':
  # Preserve perimeter/crest and original divider. Replace occupied opaque surfaces.
  for x,y,ww,hh in [(120,88,650,176),(40,276,803,1036),(752,48,133,145)]:
   rgb[y:y+hh,x:x+ww]=np.array(donor.resize((ww,hh)));mask[y:y+hh,x:x+ww]=255
 elif name=='next':
  mask[40:88,72:514]=255;rgb=cv2.inpaint(rgb,mask,7,cv2.INPAINT_TELEA)
 Image.fromarray(rgb).save(s/'Inputs'/f'{name}.png');Image.fromarray(mask).save(s/'RepairMasks'/f'{name}.png');assets.append({'name':name,'bounds':box,'inputSha256':hashlib.sha256((s/'Inputs'/f'{name}.png').read_bytes()).hexdigest(),'repairPixels':int((mask>0).sum())})
(s/'Assessment.json').write_text(json.dumps({'source':str(s/'OriginalReference.png'),'sha256':hashlib.sha256((s/'OriginalReference.png').read_bytes()).hexdigest(),'adaptation':'User approved Special Offer panel style for Help. Preserve original borders/crest/close/emerald control, replace commercial content with live localized guidance; opaque interior uses approved navy donor. Reuse actual gameplay illustrations.','assets':assets},indent=2))
shutil.copy2('ProjectDocs/AI/LoginStyleAlignment/R1/Sources/ScenarioRequests.py',s/'ScenarioRequests.py')
shutil.copy2('/tmp/lor-tutorial-r1/prepare.py',s/'PrepareInputs.py')
print('Prepared 3 source-isolation inputs')
