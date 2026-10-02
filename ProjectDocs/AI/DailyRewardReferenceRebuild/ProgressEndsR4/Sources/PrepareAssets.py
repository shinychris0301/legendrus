from pathlib import Path
from PIL import Image,ImageDraw,ImageOps
import json,hashlib,uuid,shutil
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned');src=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png').convert('RGBA')
if not (b/'BeforeAssetsHashes.json').exists():
 (b/'BeforeAssetsHashes.json').write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()},indent=2))
 shutil.copy2('Assets/LegendsOfRus/Resources/ProductionUI/Pages/DailyReward.prefab',b/'DailyReward_before.prefab.txt')
def poly(size,points):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
# Reference cap and exposed center share the same source y range, preserving rail thickness.
cap=src.crop((158,1237,192,1270));strip=src.crop((332,1237,382,1270))
cap.putalpha(poly(cap.size,[(3,15),(8,13),(14,5),(15,1),(18,1),(22,4),(34,4),(34,29),(22,29),(17,32),(14,29),(13,26),(7,21),(3,19)]))
# Remove only the baked gold progress from the cap's interior; keep its original gold rim.
dark=strip.crop((10,10,44,23)).resize(cap.size);mask=poly(cap.size,[(13,17),(23,9),(34,9),(34,23),(23,23)])
cap=Image.composite(dark,cap,mask);cap.putalpha(poly(cap.size,[(3,15),(8,13),(14,5),(15,1),(18,1),(22,4),(34,4),(34,29),(22,29),(17,32),(14,29),(13,26),(7,21),(3,19)]))
rail=Image.new('RGBA',(715,37))
for x in range(34,681,50):rail.paste(strip.crop((0,4,50,29)),(x,6))
rail.paste(cap,(0,2));rail.paste(ImageOps.mirror(cap),(681,2))
fill=Image.new('RGBA',rail.size);gold=src.crop((181,1247,223,1258)).resize((715,15));fill.paste(gold,(0,10));fill.putalpha(poly(fill.size,[(14,19),(23,11),(692,11),(701,19),(692,25),(23,25)]))
records={}
for n,im in [('month_rail_r6',rail),('month_fill_r6',fill)]:
 p=out/(n+'.png');im.save(p);meta=p.with_suffix('.png.meta')
 if not meta.exists():
  s=(out/'month_rail_r5.png.meta').read_text();g=next(x for x in s.splitlines() if x.startswith('guid:'));meta.write_text(s.replace(g,'guid: '+uuid.uuid4().hex))
 records[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'newScenarioCalls':0,'method':'Original left cap [158,1237,192,1270], mirrored right cap; original exposed middle rail material. Replaced baked cap progress with dark source donor and kept gold rim. Full/partial fill is separate and tapered inside both caps. Two transparent vertical padding pixels.'},indent=2))
for n,c in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(755,230),c)
 for y,amount in [(20,0),(90,0.5),(160,1)]:
  sheet.paste(rail,(20,y),rail);part=fill.crop((0,0,round(715*amount),37));sheet.paste(part,(20,y),part)
 sheet.save(b.parent/'QA'/('Alpha'+n+'.png'))
