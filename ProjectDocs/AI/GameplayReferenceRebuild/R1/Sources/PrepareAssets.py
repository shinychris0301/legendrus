from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib,uuid,shutil
b=Path(__file__).resolve().parent;out=b.parent/'Assets';out.mkdir(exist_ok=True)
src=Image.open(b/'OriginalReference.png').convert('RGBA');imgs={}
for n in json.loads((b/'ExtractionPlan.json').read_text()):
 a=Image.open(b/'Inputs'/(n+'.png')).convert('RGBA');m=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(a.size,Image.Resampling.LANCZOS);a.putalpha(m);imgs[n]=a
repairs={}
def fill(n,box,donor):
 a=imgs[n];s=Image.open(b/'Inputs'/(n+'.png')).convert('RGB').crop(donor);x,y,r,bot=box
 texture=Image.new('RGB',(r-x,bot-y))
 for yy in range(0,texture.height,s.height):
  for xx in range(0,texture.width,s.width):
   tile=s.transpose(Image.Transpose.FLIP_LEFT_RIGHT) if (xx//s.width)%2 else s
   if (yy//s.height)%2:tile=tile.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
   texture.paste(tile,(xx,yy))
 lay=a.copy();lay.paste(texture,(x,y));m=Image.new('L',a.size);ImageDraw.Draw(m).rectangle((x,y,r-1,bot-1),fill=255);m=m.filter(ImageFilter.GaussianBlur(2))
 al=a.getchannel('A');a=Image.composite(lay,a,m);a.putalpha(al);imgs[n]=a;repairs.setdefault(n,[]).append({'target':box,'donor':donor})
def erase_gold(n,box):
 a=np.array(imgs[n]);r=a[:,:,0].astype(float);g=a[:,:,1].astype(float);bb=a[:,:,2].astype(float)
 mask=((r>100)&(g>65)&(r>bb*1.2)).astype(np.uint8)*255
 allowed=np.zeros(mask.shape,np.uint8);x,y,right,bot=box;allowed[y:bot,x:right]=255
 mask=cv2.dilate(cv2.bitwise_and(mask,allowed),np.ones((3,3),np.uint8));mask=cv2.bitwise_and(mask,allowed)
 a[:,:,:3]=cv2.inpaint(a[:,:,:3],mask,3,cv2.INPAINT_TELEA);imgs[n]=Image.fromarray(a)
 repairs.setdefault(n,[]).append({'textMaskBox':box})
def opaque(n,box):
 a=imgs[n];m=a.getchannel('A');ImageDraw.Draw(m).rectangle(box,fill=255);a.putalpha(m)
# Keep opaque faces. The automatic panel mattes incorrectly treated content as the subject in two crops.
for n,box in [('moves',(8,10,135,172)),('score',(12,13,201,171)),('card',(15,37,178,259)),('level',(19,10,183,33))]:opaque(n,box)
a=imgs['goals'];m=Image.new('L',a.size);ImageDraw.Draw(m).rounded_rectangle((3,3,348,177),radius=17,fill=255);a.putalpha(m)
# Original red lacquer, parchment and wood donors; every covered value is native text.
erase_gold('moves',(27,22,125,133))
fill('goals',(33,22,322,152),(96,56,109,99))
fill('score',(17,20,195,172),(18,94,195,107))
m=imgs['score'].getchannel('A');ImageDraw.Draw(m).rectangle((0,177,214,185),fill=0);imgs['score'].putalpha(m)
erase_gold('level',(23,8,180,32))
# Remove the source card's example hammer, name and badge from a continuous wood face.
fill('card',(27,44,172,216),(27,85,49,123))
fill('card',(27,218,169,257),(27,85,49,123))
# Scenario repaired only the formerly occupied material surfaces. Original borders remain unchanged.
materials=Image.open(b/'Scenario/surfaces/booster_medallion_trial01.png').convert('RGB')
def material(n,box,quadrant):
 a=imgs[n];x,y,r,bot=box;q=materials.crop(quadrant).resize((r-x,bot-y),Image.Resampling.LANCZOS)
 lay=a.copy();lay.paste(q,(x,y));m=Image.new('L',a.size);ImageDraw.Draw(m).rectangle((x,y,r-1,bot-1),fill=255);m=m.filter(ImageFilter.GaussianBlur(2));al=a.getchannel('A');a=Image.composite(lay,a,m);a.putalpha(al);imgs[n]=a
 repairs.setdefault(n,[]).append({'generatedMaterialTarget':box,'sourceQuadrant':quadrant})
material('moves',(25,21,127,135),(16,16,496,496))
material('goals',(33,22,322,152),(528,16,1008,496))
material('score',(17,20,195,186),(528,528,1008,1008))
# Replace the edge occupied by the baked example stars with a reflected original top rail.
rail=Image.open(b/'Inputs/score.png').convert('RGBA').crop((0,0,215,14)).transpose(Image.Transpose.FLIP_TOP_BOTTOM)
rail.putalpha(imgs['score'].getchannel('A').crop((0,0,215,14)).transpose(Image.Transpose.FLIP_TOP_BOTTOM))
imgs['score'].paste(rail,(0,172))
repairs.setdefault('score',[]).append({'bottomRail':'reflected original top 14px; removes baked sample-star tips'})
material('card',(27,44,172,211),(16,528,496,1008))
material('card',(27,218,169,257),(16,528,496,1008))
material('level',(23,8,180,34),(528,528,1008,1008))
# Recover the original divider between the object and label wells, outside the live stock badge.
imgs['card'].paste(Image.open(b/'Inputs/card.png').convert('RGBA').crop((24,212,122,218)),(24,212))
# Board: exact source perimeter only, no pieces, grid or scene inside its opening.
a=imgs['board'];m=Image.new('L',a.size);d=ImageDraw.Draw(m);d.rounded_rectangle((3,3,834,774),radius=17,fill=255);d.rectangle((16,17,821,760),fill=0);a.putalpha(m)
m=imgs['level'].getchannel('A');ImageDraw.Draw(m).rectangle((0,0,202,4),fill=0);imgs['level'].putalpha(m)
# Isolate the portrait ring, omit the reference's decorative level-32 badge and nameplate.
a=imgs['portrait'];mirror=a.transpose(Image.Transpose.FLIP_LEFT_RIGHT);a.paste(mirror.crop((174,164,232,229)),(174,164));m=a.getchannel('A');support=Image.new('L',a.size);d=ImageDraw.Draw(support);d.ellipse((0,9,227,221),fill=255);d.polygon([(100,17),(118,1),(135,17)],fill=255);d.polygon([(0,109),(17,102),(20,127)],fill=255);d.polygon([(202,102),(228,116),(204,128)],fill=255);m=Image.fromarray(np.minimum(np.array(m),np.array(support)));ImageDraw.Draw(m).ellipse((23,29,204,210),fill=0);a.putalpha(m);imgs['portrait']=a
# Gear input included a neighbouring card corner. Keep only the separate circular control.
a=imgs['pause'];support=Image.new('L',a.size);ImageDraw.Draw(support).ellipse((2,2,103,101),fill=255);a.putalpha(Image.fromarray(np.minimum(np.array(a.getchannel('A')),np.array(support))))
# Scenario clean wood material for an intentionally opaque board substrate.
materials.crop((16,528,496,1008)).resize((808,748),Image.Resampling.LANCZOS).save(out/'wood.png')
for n,a in imgs.items():a.save(out/(n+'.png'))
# Keep the original exposed city, moon, river and foreground; infill only UI-covered areas.
gen=Image.open(b/'Scenario/environment/booster_medallion_trial01.png').convert('RGBA').resize(src.size,Image.Resampling.LANCZOS)
m=Image.new('L',src.size);d=ImageDraw.Draw(m)
for box in [(0,0,257,279),(226,52,941,244),(398,231,609,280),(48,468,893,1253),(80,1328,880,1630),(832,1566,941,1672)]:d.rectangle(box,fill=255)
m=m.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(3));Image.composite(gen,src,m).convert('RGB').save(out/'environment.png');m.save(b/'EnvironmentRepairMask.png')
dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/GameplayReferenceAligned');dst.mkdir(exist_ok=True)
if not dst.with_suffix('.meta').exists():dst.with_suffix('.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n')
for p in out.glob('*.png'):
 shutil.copyfile(p,dst/p.name);meta=dst/(p.name+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n')
# Inspect alpha at a useful scale on all three backgrounds.
names=[n for n in imgs if n!='board'];qa=b.parent/'QA';qa.mkdir(exist_ok=True)
for kind,nameset in [('Panels',[n for n in names if not n.startswith('gem_')]),('Gems',[n for n in names if n.startswith('gem_')])]:
 sheet=Image.new('RGB',(1200,len(nameset)*175));dr=ImageDraw.Draw(sheet)
 for col,bg in enumerate(['black','white','#ef19c1']):
  for row,n in enumerate(nameset):
   a=imgs[n].copy();a.thumbnail((380,145));x=col*400;y=row*175;sheet.paste(bg,(x,y,x+400,y+175));sheet.paste(a,(x+10,y+25),a);dr.text((x+10,y+5),n,fill='#888888')
 sheet.save(qa/(kind+'Alpha.jpg'))
(b/'AssetProvenance.json').write_text(json.dumps({'sourceSHA256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'jobs':16,'cuCost':sum(json.loads(p.read_text()).get('cuCost',0) for p in (b/'Scenario').glob('*/Billing.json')),'unchangedSceneryPixels':int((np.array(m)==0).sum()),'repairs':repairs,'notes':['Scenario goals/board masks selected contents; deterministic panel perimeter masks replace them','Original RGB under alpha; upscaling gems 3x is interpolation, not recovered detail','Portrait reference badge removed by local reflected rim repair; existing Ivan remains','Environment visible pixels retained; hidden scenery inferred','Scenario material sheet repairs occupied panel faces and supplies clean board wood'],'assets':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in dst.glob('*.png')}},indent=2))
