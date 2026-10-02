from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib,shutil
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');records=[]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
for i in range(8):
 source=out/f'portrait_{i}.png';im=Image.open(source).convert('RGBA');a=np.array(im);h,w=a.shape[:2]
 # The source portrait ends just above baked lettering. Fade only that crop edge;
 # never repaint hair, faces, clothing or original RGB.
 mask=np.full((h,w),255,dtype=np.uint8)
 for y in range(h-8,h):mask[y]=round(255*(h-1-y)/7)
 a[:,:,3]=np.uint8(np.rint(a[:,:,3].astype(float)*mask/255));p=out/f'card_portrait_{i}_r12.png';Image.fromarray(a).save(p)
 Image.fromarray(mask).save(b/f'PortraitBaseMask_{i}.png')
 assert np.array_equal(a[:,:,:3],np.array(im)[:,:,:3])
 records.append({'asset':str(p),'sha256':sha(p),'source':str(source),'sourceSha256':sha(source),'upstream':'../../R1/Sources/Scenario/portrait_'+str(i),'originalRGBPreserved':True,'operation':'Original Scenario-isolated sprite; alpha-only eight-pixel crop-base taper. No enlargement or RGB regeneration.','mask':f'Sources/PortraitBaseMask_{i}.png'})
for name in ['town','forest']:
 source=b/'Scenario'/name/'booster_medallion_trial01.png';p=out/f'card_{name}_r12.png';shutil.copyfile(source,p)
 records.append({'asset':str(p),'sha256':sha(p),'source':str(source),'sourceSha256':sha(source),'operation':'Unmodified Scenario clean environment-only reconstruction of obscured scenery; not original recovered pixels.'})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'assets':records,'assessment':'Sources/Assessment.json','existingSpritesUnchanged':True,'limitations':'Source portrait resolution is limited; bases end above original lettering and are tapered locally. Hidden environments are reconstructed. No existing scene/frame/portrait RGB is repainted.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sheet=Image.new('RGB',(880,420),color)
 for i in range(8):
  im=Image.open(out/f'card_portrait_{i}_r12.png');x=i%4*220;y=i//4*210;sheet.paste(im,(x,y),im);ImageDraw.Draw(sheet).text((x,y+187),str(i),fill=(128,128,128))
 sheet.save(b.parent/'QA'/f'Alpha_{name}.png')
costs=[{'job':p.parent.name,**json.loads(p.read_text())} for p in (b/'Scenario').glob('*/Billing.json')]
(b.parent/'QA/ScenarioAudit.json').write_text(json.dumps({'jobs':costs,'totalCU':sum(x['cuCost'] for x in costs),'acceptedJobs':len(costs),'duplicateSubmissions':0,'newIsolationJobs':0,'reusedPortraitIsolations':8,'rgbGenerations':2,'purpose':'Only inaccessible background scenery; characters retain source RGB.'},indent=2))
print('Eight source-alpha variants and two unmodified scenery plates prepared.')
