from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,json,hashlib,uuid
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned');src=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png').convert('RGBA');data=json.loads((b/'Boxes.json').read_text());gen=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA');seg=Image.open(b/'Scenario/subjects/booster_medallion_trial01.png').convert('RGBA').resize((800,650),Image.Resampling.LANCZOS)
records={}
def poly(size,points):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def save(name,im,method):
 p=out/(name+'.png');im.save(p)
 meta=p.with_suffix('.png.meta')
 if not meta.exists():
  t=(out/'back_r2.png.meta').read_text();g=next(x for x in t.splitlines() if x.startswith('guid:'));meta.write_text(t.replace(g,'guid: '+uuid.uuid4().hex))
 records[name]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method}
contours={
'chest':[(5,205),(10,196),(8,184),(14,178),(16,168),(17,92),(11,90),(10,76),(17,68),(24,44),(36,27),(52,15),(65,8),(81,7),(105,16),(116,13),(131,10),(139,12),(154,7),(161,12),(170,13),(180,13),(190,14),(201,20),(223,19),(244,17),(262,23),(278,37),(291,61),(296,76),(303,81),(303,90),(298,98),(297,166),(304,170),(304,197),(308,204),(264,222),(247,230),(222,240),(212,246),(197,241),(179,238),(165,231),(141,227),(127,222),(112,226),(103,218),(48,214),(45,210),(25,214),(12,209)],
'milestone1':[(6,68),(8,29),(15,13),(27,5),(37,3),(67,8),(79,9),(90,17),(94,33),(94,74),(78,86),(58,83),(37,81),(16,76),(7,79)],
'milestone2':[(5,73),(6,32),(10,17),(21,6),(32,5),(47,9),(66,10),(79,11),(92,22),(97,42),(97,77),(81,91),(64,88),(37,84),(16,79),(7,82)],
'milestone3':[(5,71),(7,27),(13,14),(24,6),(37,7),(55,12),(72,11),(84,15),(94,29),(98,49),(96,78),(81,91),(58,86),(32,81),(12,80)],
'milestone4':[(9,87),(11,52),(17,27),(25,13),(37,6),(54,5),(72,12),(91,8),(110,12),(125,26),(133,47),(133,82),(137,102),(113,113),(100,111),(92,107),(64,101),(33,94),(17,96)],
'plaque':[(0,19),(14,15),(18,1),(34,3),(252,3),(265,0),(268,17),(279,24),(273,37),(264,42),(268,60),(280,69),(266,77),(249,75),(30,75),(14,79),(13,61),(1,51),(10,38)],
'badge':[(4,5),(18,9),(37,7),(51,1),(63,8),(91,8),(98,2),(94,23),(92,85),(74,104),(51,118),(29,107),(9,89),(8,25)]}
for name,box in data['source'].items():
 im=src.crop(box);x,y=data['atlas'][name];a=seg.crop((x,y,x+im.width,y+im.height)).getchannel('A');a=ImageChops.multiply(a,poly(im.size,contours[name]));im.putalpha(a)
 if name=='plaque':
  save('day7_title_ru_r5',im,'Original RGB and Day 7 lettering; Scenario alpha constrained to source plaque silhouette.')
  blank=im.copy();m=Image.new('L',im.size);ImageDraw.Draw(m).rectangle((49,21,226,65),fill=255);m=m.filter(ImageFilter.GaussianBlur(1));blank=Image.composite(gen.crop((520,30,800,111)),blank,m);blank.putalpha(a);save('day7_title_blank_r5',blank,'Original plaque with bounded Scenario lettering removal for live English.')
 elif name=='badge':
  m=Image.new('L',im.size);ImageDraw.Draw(m).rectangle((20,26,83,94),fill=255);m=m.filter(ImageFilter.GaussianBlur(1));im=Image.composite(gen.crop((560,200,661,322)),im,m);im.putalpha(a);save('month_badge_r5',im,'Original blue shield border; bounded Scenario repair removes sample count and word for saved live count.')
 else:save('day7_chest_r5' if name=='chest' else name+'_r5',im,'Original chest RGB; Scenario segmentation retained background, so local silhouette excludes adjacent rails, numbers and panel face.')
surface=src.crop((476,746,906,1101));m=Image.new('L',surface.size);ImageDraw.Draw(m).rectangle((18,18,412,337),fill=255);m=m.filter(ImageFilter.GaussianBlur(2));surface=Image.composite(gen.crop((30,30,460,385)),surface,m);surface.putalpha(Image.open(out/'day7_r3.png').getchannel('A'));save('day7_surface_r5',surface,'Original outer frame with bounded Scenario dark bronze/gold sparkled face repair.')
# Source rail exposed between chest 1 and chest 2; repeat its middle rather than stretching end ornaments.
rail=Image.new('RGBA',(715,24));strip=src.crop((332,1242,382,1266))
for x in range(0,715,50):rail.paste(strip,(x,0))
rail.putalpha(poly(rail.size,[(0,12),(9,0),(706,0),(714,12),(706,23),(9,23)]));save('month_rail_r5',rail,'Exposed original dark/gold rail segment [332,1242,382,1266], tiled with tapered ends; no baked progress value.')
fill=src.crop((181,1247,223,1258));save('month_fill_r5',fill,'Original gold progress material; native Image.fillAmount uses saved month claims / 28.')
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'ScenarioCU':15},indent=2))
for name,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(1000,800),color)
 positions={'day7_surface_r5':(20,10),'day7_chest_r5':(500,10),'day7_title_ru_r5':(30,390),'day7_title_blank_r5':(350,390),'month_badge_r5':(700,390),'milestone1_r5':(20,520),'milestone2_r5':(160,520),'milestone3_r5':(310,520),'milestone4_r5':(460,520),'month_rail_r5':(30,690)}
 for n,xy in positions.items():
  im=Image.open(out/(n+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+name+'.png'))
