from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];src=b/'Sources';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned');ref=Image.open(src/'OriginalReference.png').convert('RGBA');items=json.loads((src/'Assessment.json').read_text());records=[];assets=[]
for item in items:
 name=item['name'];box=item['sourceBox'];im=ref.crop(box);w,h=im.size
 raw=Image.open(src/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA');scenario=np.array(raw.getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
 rgb=np.array(im)[:,:,:3].astype(float);warm=np.clip((rgb[:,:,0]-rgb[:,:,2]+1)/7,0,1)
 # Remove navy trapped between metalwork/cloth/candles; preserve opaque cloth and metal interiors explicitly.
 a=np.uint8(np.minimum(scenario,warm*255));a=cv2.morphologyEx(a,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8))
 opaque=Image.new('L',(w,h));d=ImageDraw.Draw(opaque)
 if name=='banner_left':
  d.polygon([(51,51),(126,53),(135,127),(158,190),(110,199),(69,188),(58,160)],fill=255)
  d.polygon([(161,178),(194,178),(201,202),(187,212),(158,210),(151,197)],fill=255)
  d.rectangle((41,51,46,192),fill=255)
  a[:23,:76]=0;a[:23,89:]=0;a[50:,:27]=0
 elif name=='banner_right':
  d.polygon([(73,52),(156,52),(150,140),(127,191),(71,200),(49,193),(64,147)],fill=255)
  d.polygon([(7,178),(38,178),(48,200),(38,211),(8,211),(1,199)],fill=255)
  d.rectangle((143,49,152,189),fill=255)
  a[:23,:110]=0;a[:23,126:]=0;a[:,198:]=0
 else:
  # Original sapphire is intentionally blue; do not remove it with the navy cleanup.
  d.polygon([(271,36),(281,49),(271,62),(261,49)],fill=255)
 a=np.maximum(a,np.minimum(np.array(opaque),scenario))
 if name!='footer_carving':
  shape=Image.new('L',(w,h));sd=ImageDraw.Draw(shape)
  points=([(20,29),(28,26),(69,26),(76,7),(84,7),(90,24),(143,27),(150,33),(146,47),(150,74),(156,93),(147,100),(136,91),(146,143),(163,186),(169,174),(172,139),(179,139),(182,175),(195,178),(201,201),(209,211),(207,221),(177,225),(57,225),(32,215),(36,205),(43,195),(42,67),(38,59),(37,89),(32,100),(29,90),(31,61),(36,47),(22,43)] if name=='banner_left' else [(59,26),(107,26),(113,7),(122,7),(126,25),(177,25),(189,31),(185,45),(156,46),(157,78),(166,94),(169,119),(158,129),(153,119),(153,187),(160,198),(174,202),(176,211),(165,220),(145,225),(50,225),(8,225),(1,207),(5,180),(15,177),(17,161),(24,161),(29,175),(42,172),(48,184),(67,146),(73,101),(59,111),(49,105),(51,83),(56,66),(59,47),(53,43)])
  sd.polygon(points,fill=255);a=np.minimum(a,np.array(shape))
 # Discard detached dust/neighbor fragments; retain all substantial original parts.
 count,labels,stats,_=cv2.connectedComponentsWithStats((a>32).astype(np.uint8),8)
 keep=np.zeros_like(a)
 for i in range(1,count):
  if stats[i,cv2.CC_STAT_AREA]>=4:keep[labels==i]=255
 a=np.minimum(a,cv2.dilate(keep,np.ones((3,3),np.uint8)))
 support=np.array(Image.open(src/'Inputs'/(name+'_support.png')).convert('L'));a=np.minimum(a,support)
 if name=='footer_carving':
  button=Image.open(art/'details_ru_r7.png').getchannel('A').resize((593,150),Image.Resampling.LANCZOS)
  cut=Image.new('L',(w,h));cut.paste(button,(180-box[0],1458-box[1]));a=np.minimum(a,255-np.array(cut))
 original=ref.crop(box).resize((w*3,h*3),Image.Resampling.LANCZOS);im=original.copy();im.putalpha(Image.fromarray(a).resize(im.size,Image.Resampling.LANCZOS));assert np.array_equal(np.array(im)[:,:,:3],np.array(original)[:,:,:3])
 p=art/(name+'_r8.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details.png.meta').read_text(),count=1))
 records.append(dict(name=name,sourceBox=box,originalRGBPreserved=True,alphaOperation='Scenario isolation with local navy removal, cloth/metal opacity guards and neighbor exclusion',sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
 assets.append((name,im))
sheet=Image.new('RGB',(1200,len(assets)*270));d=ImageDraw.Draw(sheet)
for i,(n,im) in enumerate(assets):
 for j,c in enumerate(['black','white','#ff00dd']):
  panel=Image.new('RGBA',(400,245),c);t=im.copy();t.thumbnail((385,235));panel.alpha_composite(t,((400-t.width)//2,(245-t.height)//2));sheet.paste(panel.convert('RGB'),(j*400,i*270))
 d.text((8,i*270+248),n,fill='white')
sheet.save(b/'QA/AssetAlpha.png');(src/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
