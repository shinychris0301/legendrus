from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,json,hashlib,uuid
b=Path(__file__).resolve().parent;im=Image.open(b/'Scenario/icons/booster_medallion_trial01.png').convert('RGBA');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned');records=[];sprites=[]
for i,name in enumerate(['ranking','team','friends']):
 box=(i*512,0,(i+1)*512,1024);crop=im.crop(box);a=np.array(crop.getchannel('A'));mask=Image.fromarray(np.where(a>=16,255,0).astype('uint8'));bounds=mask.getbbox();print(name,bounds,'center alpha',a[600,250]);
 # The generated mask peaks at 254; normalize the fully opaque interior without changing RGB.
 rgba=np.array(crop);rgba[:,:,3]=np.where(a>=245,255,np.where(a<8,0,a)).astype('uint8');crop=Image.fromarray(rgba).crop(bounds);crop=ImageOps.expand(crop,8,fill=(0,0,0,0))
 path=out/('shortcut_'+name+'_r6.png');crop.save(path);sprites.append(crop)
 meta=Path(str(path)+'.meta')
 if not meta.exists():
  text=(out/'source_shop_blank.png.meta').read_text();import re;text=re.sub(r'guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,text,count=1);text=re.sub(r'spriteID: [0-9a-f]+','spriteID: '+uuid.uuid4().hex,text);meta.write_text(text)
 records.append({'asset':str(path),'sourceThird':box,'trim':bounds,'size':crop.size,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
(b/'AssetProvenance.json').write_text(json.dumps({'operation':'Split generated Scenario RGBA sheet into three sprites; trim alpha>=16 bounds, eight-pixel margin; alpha<8 cleared, alpha>=245 made opaque; RGB unchanged. Generated reinterpretation, not exact source extraction.','assets':records},indent=2))
qa=b.parent/'QA';sheet=Image.new('RGB',(900,1050));draw=ImageDraw.Draw(sheet)
for row,bg in enumerate(['black','white','#ee16a4']):
 for col,im in enumerate(sprites):
  x,y=col*300,row*350;draw.rectangle((x,y,x+300,y+350),fill=bg);thumb=im.copy();thumb.thumbnail((265,320));sheet.paste(thumb,(x+(300-thumb.width)//2,y+(350-thumb.height)//2),thumb)
sheet.save(qa/'AlphaBackgrounds.png')
