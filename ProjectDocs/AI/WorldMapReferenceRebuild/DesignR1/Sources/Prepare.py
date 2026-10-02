from pathlib import Path
from PIL import Image,ImageDraw,ImageFile
import numpy as np,cv2,shutil,json,hashlib
b=Path(__file__).resolve().parent;out=b.parent/'Assets';records={}
for i in range(1,6):
 p=b/f'Scenario/terrain{i}/booster_medallion_trial01.png';im=Image.open(p).convert('RGB');w,h=im.size
 for j in range(4):
  n=(i-1)*4+j+1;box=(0,round(j*h/4),w,round((j+1)*h/4));dest=out/f'region_{n:02}.png';im.crop(box).save(dest);records[dest.name]={'method':'Scenario terrain band, no recolor or repaint','source':str(p),'crop':box}
for name in ['navigation','title_full']:
 p=b/'Inputs'/f'{name}.png';im=Image.open(p).convert('RGBA')
 if name=='title_full':
  # Expanded Scenario PNG has 207 complete of 256 rows. Only its first 44 rows
  # are needed for the crest; do not use missing/truncated lower pixels.
  import struct,zlib
  raw=(b/'Scenario/title_full/booster_medallion_trial01.png').read_bytes();pos=8;data=b''
  while pos+8<=len(raw):
   length=struct.unpack('>I',raw[pos:pos+4])[0]
   if raw[pos+4:pos+8]==b'IDAT':data+=raw[pos+8:pos+8+length]
   pos+=12+length
  complete=len(zlib.decompressobj().decompress(data))//(780*4+1);assert complete>=64
  ImageFile.LOAD_TRUNCATED_IMAGES=True
  partial=Image.open(b/'Scenario/title_full/booster_medallion_trial01.png').convert('RGBA').getchannel('A');ImageFile.LOAD_TRUNCATED_IMAGES=False
  mask=Image.new('L',im.size);old=Image.open(b/'Scenario/title/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((567,166),Image.Resampling.LANCZOS);mask.paste(old,(9,20))
  crest=partial.crop((0,0,780,48)).resize((585,36),Image.Resampling.LANCZOS);mask.paste(crest.crop((0,0,585,28)),(0,0))
  (b/'TitleRecovery.json').write_text(json.dumps({'completeDecodedRows':complete,'usedExpandedRows':48,'usedSourceRows':28,'bodyFrom':'title original complete extraction','reason':'Expanded output truncated on server; two downloads same hash. No replacement generation.'},indent=2))
 else:mask=Image.open(b/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS)
 a=np.array(mask)
 if name=='title_full':
  a[:30,:170]=0;a[:23,350:]=0
  num,labs,stats,_=cv2.connectedComponentsWithStats((a>20).astype('uint8'));big=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);a[labs!=big]=0
  ImageDraw.Draw(mask).rectangle((80,62,500,127),fill=255)
  a[62:127,80:500]=255;im.putalpha(Image.fromarray(a));im.save(out/'title.png');records['title.png']={'source':str(p),'method':'Original RGB with Scenario alpha and disconnected-header exclusion; opaque lettering face restored'}
 else:
  d=ImageDraw.Draw(mask)
  for box in [(105,34,319,231),(368,32,575,233),(633,34,838,232)]:d.ellipse(box,fill=255)
  im.putalpha(mask)
  for key,box in [('map',(0,0,342,342)),('tasks',(342,0,605,342)),('guild',(605,0,941,342))]:
   im.crop(box).save(out/f'nav_{key}.png');records[f'nav_{key}.png']={'source':str(p),'method':'Original RGB with Scenario alpha, separate native-control artwork','crop':box}
# Reuse approved Home header, existing extracted lock and Back, and embedded project fonts unchanged.
h=Path('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned')
for n in ['source_header','source_avatar','source_gold','source_gems','source_energy','source_plus','source_settings']:
 shutil.copy2(h/f'{n}.png',out/f'{n}.png');records[f'{n}.png']={'source':str(h/f'{n}.png'),'method':'Approved Home source reuse, byte-identical'}
for src,n in [('LoginAligned/lock.png','lock.png'),('DailyRewardAligned/back_r2.png','back.png'),('IndividualRankingFonts/PlayerNames.ttf','Body.ttf'),('IndividualRankingFonts/RankNumbers.ttf','Numbers.ttf')]:
 p=Path('Assets/LegendsOfRus/Resources/ProductionUI')/src;shutil.copy2(p,out/n);records[n]={'source':str(p),'method':'Existing project resource reuse, byte-identical'}
for n,r in records.items():r['sha256']=hashlib.sha256((out/n).read_bytes()).hexdigest()
(b/'AssetManifest.json').write_text(json.dumps(records,indent=2))
for bg,n in [('black','Black'),('white','White'),('#ff00ff','Magenta')]:
 canvas=Image.new('RGB',(1000,570),bg)
 for key,x,y in [('title.png',180,0),('nav_map.png',0,210),('nav_tasks.png',342,210),('nav_guild.png',605,210)]:
  im=Image.open(out/key);canvas.paste(im,(x,y),im)
 canvas.save(b.parent/'QA'/f'Alpha_{n}.png')
