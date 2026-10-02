from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;q=b.parent/'QA';q.mkdir(exist_ok=True)
production=Path('Assets/LegendsOfRus/Resources/ProductionUI/VictoryReferenceR1')
files=sorted(p for p in production.glob('*.png') if p.stem!='environment')
for name,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(255,0,180))]:
 sheet=Image.new('RGB',(900,4*235),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(files):
  im=Image.open(p).convert('RGBA');im.thumbnail((284,202),Image.Resampling.LANCZOS);x=(i%3)*300+(300-im.width)//2;y=(i//3)*235+29
  sheet.paste(im,(x,y),im);d.text(((i%3)*300+8,(i//3)*235+7),p.stem,fill=(128,128,128))
 sheet.save(q/('Alpha'+name+'.png'))
source=Image.open(b/'OriginalReference.png').convert('RGB');boxes=json.loads((b/'ExtractionPlan.json').read_text());assets={}
for p in files:
 im=Image.open(p).convert('RGBA');a=np.array(im);key='star' if p.stem=='star_empty' else p.stem;r=np.array(source.crop(boxes[key]))
 changed=np.any(a[:,:,:3]!=r,axis=2);Image.fromarray((changed*255).astype('uint8')).save(b/(p.stem+'_RGBRepairMask.png'))
 assets[p.stem]={'size':list(im.size),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'changedRGBPixels':int(changed.sum()),'preservedRGBPixels':int((~changed).sum()),'transparentPixels':int((a[:,:,3]==0).sum()),'opaquePixels':int((a[:,:,3]==255).sum()),'repairMask':p.stem+'_RGBRepairMask.png'}
jobs=[]
for directory in sorted((b/'Scenario').iterdir()):
 if not (directory/'Status.json').exists():continue
 status=json.loads((directory/'Status.json').read_text());billing=json.loads((directory/'Billing.json').read_text()) if (directory/'Billing.json').exists() else {}
 jobs.append({'name':directory.name,'status':status['status'],'billing':billing,'outputSHA256':hashlib.sha256((directory/'booster_medallion_trial01.png').read_bytes()).hexdigest()})
(q/'AssetChecks.json').write_text(json.dumps({'assets':assets,'jobs':jobs},indent=2));print(json.dumps({'assets':len(assets)+1,'jobs':jobs},indent=2))
