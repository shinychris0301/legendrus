from pathlib import Path
from PIL import Image,ImageDraw
import json,hashlib,shutil
b=Path(__file__).resolve().parent
source=Path('/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (3).png')
ref=Image.open(source).convert('RGB');shutil.copyfile(source,b/'OriginalReference.png')
boxes={'title_ru':(178,96,763,267),'banner':(15,250,397,661),'portrait_ivan':(135,1053,252,1162),'portrait_olga':(136,1150,252,1259),'portrait_svyatoslav':(135,1248,252,1359),'portrait_velimir':(135,1339,252,1447),'portrait_lada':(135,1430,252,1534),'chat':(89,908,198,979),'ranking':(309,908,398,979),'tasks':(514,906,611,979),'donate':(704,908,871,978),'power':(431,442,510,524),'chest':(763,497,873,588),'task_banner':(73,714,154,817),'task_swords':(363,713,444,805),'task_potion':(638,711,704,804),'task_frame':(54,702,330,881),'nav_frame':(33,894,248,1028),'donate_frame':(667,894,908,1028)}
(b/'Crops').mkdir(exist_ok=True);(b/'Inputs').mkdir(exist_ok=True);(b/'RepairMasks').mkdir(exist_ok=True)
records=[]
for name,box in boxes.items():
 im=ref.crop(box);im.save(b/'Crops'/(name+'.png'));mask=Image.new('L',im.size)
 if name.endswith('_frame'):
  donor=ref.crop((515,1190,630,1231));tile=Image.new('RGB',im.size)
  if name=='donate_frame':donor=ref.crop((679,919,703,947))
  for y in range(0,im.height,donor.height):
   for x in range(0,im.width,donor.width):tile.paste(donor,(x,y))
  inset=17;ImageDraw.Draw(mask).rectangle((inset,inset,im.width-inset,im.height-inset),fill=255);im.paste(tile,(0,0),mask)
 im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS).save(b/'Inputs'/(name+'.png'));mask.save(b/'RepairMasks'/(name+'.png'));records.append({'name':name,'box':box,'repair':'none' if not name.endswith('_frame') else 'blank interior uses clean source lacquer donor; border RGB retained','sourceSize':im.size})
# Isolate groups with generous gutters; per-item coordinates retained for local alpha application.
for group,names in {'portraits':[n for n in boxes if n.startswith('portrait_')],'icons':['chat','ranking','tasks','donate','power','chest','task_banner','task_swords','task_potion']}.items():
 sheet=Image.new('RGB',(1200,400*((len(names)+2)//3)),(63,65,71))
 for i,n in enumerate(names):
  im=Image.open(b/'Crops'/(n+'.png'));ratio=min(320/im.width,320/im.height);sz=(round(im.width*ratio),round(im.height*ratio));x=(i%3)*400+(400-sz[0])//2;y=(i//3)*400+(400-sz[1])//2;sheet.paste(im.resize(sz,Image.Resampling.LANCZOS),(x,y));next(r for r in records if r['name']==n).update(atlas=group,atlasBox=[x,y,x+sz[0],y+sz[1]])
 sheet.save(b/'Inputs'/(group+'.png'))
(b/'Assessment.json').write_text(json.dumps({'reference':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'referenceStatus':'discovered by visual inspection among generic filenames; guild hub Sokoly Rusi','assets':records,'environment':'UI occludes most scenery; Scenario clean environment reconstruction needed','data':'sample guild roster/progress labelled preview; no online team service'},indent=2,ensure_ascii=False))
