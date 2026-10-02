from pathlib import Path
import hashlib,json,shutil
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2
b=Path('ProjectDocs/AI/SettingsStyleAlignment/R1');s=b/'Sources'
for sub in ['Inputs','RepairMasks','Scenario','Final']:(s/sub).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True)
source=Path('/home/k2/Downloads/match3final/6.settings.png');shutil.copy2(source,s/'OriginalReference.png');im=Image.open(source).convert('RGB')
crops={'title_ru':(115,115,824,347),'paper':(102,346,835,789),'row':(104,789,836,898),'logout':(104,1345,837,1469),'frame':(57,284,879,1647),'switch':(658,389,805,458),'arrow':(742,812,803,875),'back':(380,1477,560,1645)}
for name,y in zip(['music','sfx','vibration','notifications','language','cloud','account','support','privacy','logout'],[374,472,572,668,794,903,1016,1128,1239,1351]):crops['icon_'+name]=(133,y,236,y+105)
records=[]
for name,box in crops.items():
 a=im.crop(box);rgb=np.array(a);mask=np.zeros(rgb.shape[:2],np.uint8);h,w=mask.shape
 if name=='paper':
  # Surface/labels/controls cannot survive in a live settings panel. Reuse approved clean parchment interior, keep source border.
  donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned/paper.png').convert('RGB');donor=donor.crop((80,80,donor.width-80,donor.height-80)).resize((w-40,h-43),Image.Resampling.LANCZOS);rgb[22:h-21,20:w-20]=np.array(donor);mask[22:h-21,20:w-20]=255
 if name in ['row','logout']:
  mask[12:h-12,25:w-25]=255
  # Preserve border; replace occupied face with a clean source navy texture donor.
  donor=im.crop((367,811,540,873)).resize((w-50,h-24),Image.Resampling.LANCZOS)
  arr=np.array(donor)
  if name=='logout':arr=(arr.astype(float)*[2.8,.35,.26]).clip(0,255).astype('uint8')
  rgb[12:h-12,25:w-25]=arr
 native=Image.fromarray(rgb);native.save(s/'Inputs'/f'{name}_native.png');native.resize((w*2,h*2),Image.Resampling.LANCZOS).save(s/'Inputs'/f'{name}.png');Image.fromarray(mask).save(s/'RepairMasks'/f'{name}.png')
 records.append({'name':name,'bounds':box,'sourceRgbPreservedOutsideMask':bool(np.all(np.array(a)[mask==0]==rgb[mask==0])),'repairPixels':int((mask>0).sum()),'inputSha256':hashlib.sha256((s/'Inputs'/f'{name}.png').read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'originalFile':str(source),'originalSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'approach':'Individual source alpha isolation; original RGB outside explicit repairs. Paper uses approved clean Inventory paper; row faces use source navy donor. Outer frame interior removed, no complete screenshot runtime. Existing clean scenery/header/fonts reused.','assets':records},indent=2))
shutil.copy2('ProjectDocs/AI/InventoryStyleAlignment/R1/Sources/ScenarioRequests.py',s/'ScenarioRequests.py')
shutil.copy2('/tmp/lor-settings-r1/prepare.py',s/'InitialPreparation.py')
entry='# 2026-09-18 — Visual Page28 Settings started\n\nUser approved native Settings. Dedicated original `6.settings.png` matches DesignScreens byte-for-byte. Existing preferences persist through GameState; Settings uses legacy hotspots in full-page and Pause-modal routes. Implement source-derived native switches/rows, preserve preferences and origin/paused-board Back behavior. No authenticated cloud/account or final support/privacy UI is established; represent availability honestly. Scenario isolation and fresh verification pending. Preserve Gameplay/earlier pages/GUIDs/packages/settings.\n\n'
for name in ['CurrentTask.md','SessionLog.md']:
 p=Path('ProjectDocs/AI')/name;p.write_text(entry+p.read_text())
print('Prepared',len(records),'individual Scenario inputs')
