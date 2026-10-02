from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parents[1];dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned');ref=Image.open(b/'Sources/Reference.png').convert('RGB');assets=[];records=[]
for name,box in [('retry',(239,1054,706,1196)),('home',(267,1193,675,1300))]:
 old=Image.open(dst/(name+'.png')).convert('RGBA');im=ref.crop(box).convert('RGBA');assert im.size==old.size;im.putalpha(old.getchannel('A'));im.save(dst/(name+'_ru_r3.png'));assets.append(im)
 records.append({'asset':name+'_ru_r3','sourceBounds':box,'rgbUnchanged':bool(np.array_equal(np.array(im)[:,:,:3],np.array(ref.crop(box)))),'alphaUnchanged':True})
panel=Image.open(dst/'panel.png').convert('RGBA');box=(190,346,753,436);patch=ref.crop(box).convert('RGBA');panel.paste(patch,(box[0]-120,box[1]-185));panel.save(dst/'panel_ru_r3.png');assets.insert(0,panel);records.append({'asset':'panel_ru_r3','sourceRestoredHeadingBounds':box,'note':'Only source Russian title restored; existing panel and intentionally opaque surface retained'})
for n,c in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(1220,1350),c);sheet.paste(assets[0],(0,0),assets[0]);sheet.paste(assets[1],(710,100),assets[1]);sheet.paste(assets[2],(735,280),assets[2]);sheet.save(b/'QA'/('Alpha'+n+'.png'))
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'source':'Reference.png','scenarioJobs':0,'newGeneration':False,'method':'Reuse accepted Scenario extraction alpha; original source lettering RGB','assets':records},indent=2)+'\n');print(records)
