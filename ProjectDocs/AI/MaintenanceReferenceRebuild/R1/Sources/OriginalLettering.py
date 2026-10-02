from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parents[1];dst=b/'ProposedAssets';dst.mkdir(exist_ok=True)
old=Path('Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned');ref=Image.open(b/'Sources/Reference.png').convert('RGB')
records=[];images=[]
for name,box,last in [('panel',(60,311,880,1523),300),('timer',(166,854,774,1046),74),('button',(206,1302,738,1436),134)]:
 a=np.array(Image.open(old/(name+'.png')).convert('RGBA'));original=a.copy();rgb=np.array(ref.crop(box));assert a.shape[:2]==rgb.shape[:2]
 a[:last,:,:3]=rgb[:last];im=Image.fromarray(a);im.save(dst/(name+'_ru_r2.png'));images.append(im)
 assert np.array_equal(a[:,:,3],original[:,:,3]);assert np.array_equal(a[last:],original[last:]);assert np.array_equal(a[:last,:,:3],rgb[:last])
 records.append({'asset':name+'_ru_r2.png','sourceBounds':box,'restoredRows':last,'sourceRGBExact':True,'alphaUnchanged':True,'remainingRowsUnchanged':True})
for n,c in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(1440,1212),c)
 for im,pos in zip(images,[(0,0),(826,90),(862,360)]):sheet.paste(im,pos,im)
 chest=Image.open(old/'chest.png').convert('RGBA');sheet.paste(chest,(890,610),chest)
 sheet.save(b/'QA'/('Alpha'+n+'.png'))
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'source':'Reference.png','sourceSha256':hashlib.sha256((b/'Sources/Reference.png').read_bytes()).hexdigest(),'scenarioJobs':0,'newGeneration':False,'method':'Original source RGB with prior accepted Scenario alpha; occupied body and dynamic time retained from existing cleared assets','assets':records},indent=2)+'\n')
print('Three variants staged; source RGB and alpha checks passed.')
