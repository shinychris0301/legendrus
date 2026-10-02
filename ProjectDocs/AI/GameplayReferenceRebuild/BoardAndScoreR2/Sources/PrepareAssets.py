from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib,uuid,shutil
b=Path(__file__).resolve().parent;r1=b.parents[1]/'R1';out=b.parent/'Assets';out.mkdir(exist_ok=True);dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/GameplayRefinedR2');dst.mkdir(exist_ok=True)
base=Image.open(r1/'Assets/environment.png').convert('RGB');low=Image.open(b/'Scenario/environment/booster_medallion_trial01.png').convert('RGB').resize((941,1272),Image.Resampling.LANCZOS);layer=base.copy();layer.paste(low,(0,400));m=Image.new('L',base.size);d=ImageDraw.Draw(m);d.rectangle((0,455,940,1671),fill=255)
for y in range(400,455):d.line((0,y,940,y),fill=round((y-400)/55*255))
Image.composite(layer,base,m).save(out/'environment.png');m.save(b/'EnvironmentRepairMask.png')
wood=Image.open(r1/'Assets/wood.png').convert('L').filter(ImageFilter.GaussianBlur(2));wood=ImageOps.autocontrast(wood);wood=ImageOps.colorize(wood,(35,29,20),(62,51,34));wood.save(out/'wood.png')
# Retain original frame sides and replace only the previously reconstructed bottom rail.
a=Image.open(r1/'Assets/score.png').convert('RGBA');material=Image.open(r1/'Sources/Scenario/surfaces/booster_medallion_trial01.png').convert('RGBA').crop((528,528,1008,1008)).resize((181,18),Image.Resampling.LANCZOS)
a.paste(material,(17,168));al=a.getchannel('A');d=ImageDraw.Draw(al);d.rectangle((0,180,214,185),fill=0);a.putalpha(al)
# A restrained rail stays behind the extracted star silhouettes.
d=ImageDraw.Draw(a);d.line((14,178,201,178),fill=(72,45,17,255),width=3);d.line((14,177,201,177),fill=(168,119,48,255),width=1);a.save(out/'score.png')
for name in ['star_filled','star_empty']:
 rgb=Image.open(b/'Inputs'/(name+'.png')).convert('RGBA');mask=Image.open(b/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(rgb.size,Image.Resampling.LANCZOS);arr=np.array(mask);count,labels,stats,_=cv2.connectedComponentsWithStats((arr>32).astype('uint8'));largest=1+int(np.argmax(stats[1:,cv2.CC_STAT_AREA]));keep=(labels==largest).astype('uint8');contours,_=cv2.findContours(keep,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);filled=np.zeros_like(keep);cv2.drawContours(filled,contours,-1,1,-1);arr=np.where((filled>0)&(keep==0),255,arr*keep).astype('uint8');rgb.putalpha(Image.fromarray(arr));rgb.save(out/(name+'.png'))
if not dst.with_suffix('.meta').exists():dst.with_suffix('.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n')
for p in out.glob('*.png'):
 shutil.copyfile(p,dst/p.name);meta=dst/(p.name+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n')
qa=b.parent/'QA';qa.mkdir(exist_ok=True);sheet=Image.new('RGB',(900,450))
for col,bg in enumerate(['black','white','#ef19c1']):
 for row,name in enumerate(['star_filled','star_empty','score']):
  a=Image.open(out/(name+'.png')).convert('RGBA');a.thumbnail((260,140));sheet.paste(bg,(col*300,row*150,(col+1)*300,(row+1)*150));sheet.paste(a,(col*300+10,row*150+5),a)
sheet.save(qa/'Alpha.jpg')
(b/'AssetProvenance.json').write_text(json.dumps({'jobs':3,'cuCost':sum(json.loads(p.read_text()).get('cuCost',0) for p in (b/'Scenario').glob('*/Billing.json')),'notes':['One lower-scenery repair blended y400–455; original upper scenery retained','Two Scenario source-star masks; disconnected rail fragments removed and original dark interiors retained','Subdued existing wood texture; reconstructed score lower rail simplified locally','Six regular gem sprites unchanged'],'assets':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in dst.glob('*.png')}},indent=2))
