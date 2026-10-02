from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,json,hashlib,uuid
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned');boxes=json.loads((b/'Boxes.json').read_text())['source'];src=Image.open(b/'OriginalReference.png').convert('RGBA');atlas=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA')
# Returned atlas moved the parts. These rectangles were measured from the inspected output, not assumed from the input.
regions={'card':(0,0,204,538),'day7':(250,0,718,540),'active':(748,0,979,540),'month':(0,576,930,822),'tomorrow':(0,864,873,1031),'claim':(0,1068,510,1200),'header':(0,1254,806,1493)}
records={}
def save(n,im,method):
 p=out/(n+'.png');im.save(p);meta=p.with_suffix('.png.meta')
 if not meta.exists():
  template=(out/'back_r2.png.meta').read_text();old=next(x for x in template.splitlines() if x.startswith('guid:'));meta.write_text(template.replace(old,'guid: '+uuid.uuid4().hex))
 records[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method}
def poly(size,points):
 m=Image.new('L',size);ImageDraw.Draw(m).polygon(points,fill=255);return m
def repair(name,rects,alpha=None):
 im=src.crop(boxes[name]);generated=atlas.crop(regions[name])
 if name in ('card','active'):generated=generated.crop((25,85,generated.width-25,generated.height-65))
 generated=generated.resize(im.size,Image.Resampling.LANCZOS);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 for r in rects:d.rectangle(r,fill=255)
 mask=mask.filter(ImageFilter.GaussianBlur(1));mask.save(b/(name+'RepairMask.png'));im=Image.composite(generated,im,mask)
 if alpha is not None:im.putalpha(alpha)
 return im
# Exact source title plus Pixa alpha; clear unrelated portrait/resource edges, retain the opaque navy face.
h=src.crop(boxes['header']);a=Image.open(b/'Scenario/header/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(h.size,Image.Resampling.LANCZOS)
outline=poly(h.size,[(0,145),(20,122),(25,103),(50,81),(116,65),(240,50),(275,32),(333,35),(361,15),(382,0),(405,19),(425,33),(464,36),(491,56),(650,67),(697,89),(727,115),(765,145),(729,162),(713,196),(671,211),(400,211),(382,219),(357,208),(91,211),(30,198),(19,168)])
a=ImageChops.multiply(a,outline);ImageDraw.Draw(a).polygon([(45,106),(79,87),(239,78),(300,72),(349,88),(417,88),(471,76),(665,84),(704,102),(722,143),(699,189),(670,198),(77,198),(41,181),(28,145)],fill=255);h.putalpha(a);save('header_ru_r3',h,'Original Russian title RGB, new Scenario alpha plus bounded removal of unrelated HUD/portrait fragments; opaque plaque face restored.')
blank=repair('header',[(57,108,709,178)],a);save('header_blank_r3',blank,'Same source perimeter with bounded Scenario lettering repair for live English.')
for name,key in [('card','card_r3'),('active','card_active_r3'),('day7','day7_r3')]:
 w,h=src.crop(boxes[name]).size
 if name=='active':a=poly((w,h),[(4,36),(21,15),(85,15),(108,0),(132,15),(194,15),(215,37),(215,h-26),(195,h-7),(22,h-7),(4,h-26)])
 else:a=poly((w,h),[(1,24),(22,2),(w-24,2),(w-2,24),(w-2,h-24),(w-23,h-2),(22,h-2),(1,h-24)])
 rects=[(20,25,w-21,h-22)] if name!='card' else [(20,24,w-20,64),(20,75,w-20,h-20)]
 im=repair(name,rects,a)
 if name=='active':
  original=src.crop(boxes[name]);im.paste(original.crop((25,74,w-25,80)),(25,74));im.putalpha(a)
 save(key,im,'Original gold border with bounded Scenario '+name+' interior; live day/count/status overlays.')
a=poly((897,240),[(3,63),(17,35),(339,35),(356,20),(394,25),(445,0),(493,26),(531,21),(553,35),(871,35),(894,55),(894,208),(871,221),(478,221),(447,239),(418,221),(19,221),(3,206)])
im=repair('month',[(30,47,865,212)],a);save('month_r3',im,'Original monthly frame and crest; Scenario face repair removes sample count, chests and progress.')
a=poly((849,159),[(0,78),(20,60),(36,27),(67,5),(772,5),(806,27),(829,57),(848,77),(823,97),(805,135),(768,151),(459,151),(424,159),(394,151),(73,151),(37,130),(19,101)])
im=repair('tomorrow',[(75,22,768,135)],a);save('tomorrow_r3',im,'Original next-reward frame with bounded Scenario clearing for actual next-day message.')
a=Image.open(b/'Scenario/claim/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((490,129),Image.Resampling.LANCZOS)
ImageDraw.Draw(a).rectangle((60,28,426,96),fill=255)
im=repair('claim',[(79,33,405,100)],a);save('claim_blank_r3',im,'Original claim-button border; source OK text cleared using bounded Scenario material for functional Claim/Claimed localization.')
(b/'AssetProvenance.json').write_text(json.dumps({'returnedAtlasRegions':regions,'assets':records},indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(1000,1350),color)
 for key,xy in [('header_ru_r3',(90,0)),('card_r3',(0,250)),('card_active_r3',(220,250)),('day7_r3',(470,250)),('month_r3',(35,640)),('tomorrow_r3',(60,900)),('claim_blank_r3',(250,1110))]:
  im=Image.open(out/(key+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
print('Prepared',len(records),'sprites')
