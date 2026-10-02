from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,uuid,re,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');sheet=Image.open(b/'Sheet.png').convert('RGB');scenario=Image.open(b/'Scenario/letter_controls/booster_medallion_trial01.png').convert('RGBA');ref=Image.open(b/'Letter.png').convert('RGB');records=[]
def save(name,im,method):
 p=out/(name+'.png');im.save(p)
 if not p.with_suffix('.png.meta').exists():
  meta=(out/'title_ru.png.meta').read_text();meta=re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,meta,count=1);p.with_suffix('.png.meta').write_text(meta)
 records.append({'name':name,'method':method,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
for item in json.loads((b/'Sheet.json').read_text()):
 name=item['name'];rgb=sheet.crop(item['sheetBox']);W,H=rgb.size;m=Image.new('L',(W,H));d=ImageDraw.Draw(m)
 if name=='letter_title':
  a=np.array(rgb).astype(float);gold=np.clip((a[:,:,0]-a[:,:,2]-8)/24,0,1)*np.clip((a[:,:,1]-a[:,:,2]-3)/18,0,1)
  allow=Image.new('L',(W,H));da=ImageDraw.Draw(allow)
  poly=[(0,103),(22,88),(41,65),(89,53),(156,47),(205,32),(229,15),(262,17),(283,0),(305,19),(332,20),(348,36),(378,43),(470,54),(520,65),(547,88),(570,102),(550,124),(550,156),(504,169),(354,169),(325,190),(283,199),(239,184),(220,171),(69,174),(24,164),(12,143),(19,121)]
  da.polygon([(x*2,y*2)for x,y in poly],fill=255)
  m=Image.fromarray((gold*np.array(allow)).astype('uint8'));d=ImageDraw.Draw(m)
  face=[(82,73),(488,73),(514,79),(532,92),(540,106),(540,133),(524,151),(493,159),(65,159),(38,151),(26,133),(25,104),(43,84)]
  d.polygon([(x*2,y*2)for x,y in face],fill=255)
  d.ellipse((269*2,31*2,299*2,61*2),fill=255);d.ellipse((270*2,165*2,296*2,188*2),fill=255)
  mask=np.minimum(np.array(m),np.array(scenario.crop(item['sheetBox']).getchannel('A')));m=Image.fromarray(mask)
  im=rgb.convert('RGBA');im.putalpha(m);save('letter_title_ru_r5',im,'Original Letter title RGB with Scenario alpha refined by bounded gold/material and opaque face masks; original Russian lettering retained.')
  blank=np.array(rgb);masktext=np.zeros((H,W),np.uint8);masktext[85*2:149*2,143*2:421*2]=255
  # Source navy donor, mirrored tile; mask contains only title lettering within opaque face.
  donor=np.array(rgb.crop((70*2,93*2,119*2,138*2)));tile=np.concatenate([donor,donor[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);tex=np.tile(tile,(H//tile.shape[0]+1,W//tile.shape[1]+1,1))[:H,:W]
  feather=cv2.GaussianBlur(masktext,(0,0),3).astype(float)/255;blank=(blank*(1-feather[:,:,None])+tex*feather[:,:,None]).round().astype('uint8');en=Image.fromarray(blank).convert('RGBA');en.putalpha(m);save('letter_title_blank_r5',en,'Same original title silhouette; lettering footprint replaced with source navy donor for live English heading.')
 else:
  scale=4;mask=Image.new('L',(W*scale,H*scale));draw=ImageDraw.Draw(mask)
  # Contours in native crop pixels, rendered with supersampling.
  if name=='letter_back':cx,cy,rx,ry=81,87,69,72;tips=[(81,11),(81,156),(13,85),(151,85)]
  else:cx,cy,rx,ry=85,87,70,72;tips=[(85,11),(85,156),(14,85),(155,85)]
  q=2*scale;draw.ellipse(((cx-rx)*q,(cy-ry)*q,(cx+rx)*q,(cy+ry)*q),fill=255)
  for x,y in tips:draw.polygon([(x*q,(y-8)*q),((x+8)*q,y*q),(x*q,(y+8)*q),((x-8)*q,y*q)],fill=255)
  mask=mask.resize((W,H),Image.Resampling.LANCZOS);m=Image.fromarray(np.minimum(np.array(mask),np.array(scenario.crop(item['sheetBox']).getchannel('A'))));im=rgb.convert('RGBA');im.putalpha(m);save(name+'_r5',im,'Original circular footer control RGB; Scenario sheet retained backing, refined with supersampled circular rim/diamond contours.')
# Intentional opaque carved title backing. Source right-hand carving has no controls;
# mirror it for the left where the reference avatar overlaps the carving.
# Blend the center lacquer into the side carving instead of leaving a rectangular
# donor patch. The joins sit under the independent title's lateral ornaments.
navy=np.array(Image.open(out/'title_blank.png').convert('RGB').crop((430,245,650,350)).resize((941,153))).astype(float)
navy=navy-cv2.GaussianBlur(navy,(0,0),15)+np.array([14,23,31]);backarr=np.clip(navy,0,255)
side=np.array(ref.crop((751,145,941,284)).resize((190,139))).astype(float)
for right in (False,True):
 tile=side if right else side[:,::-1]
 weight=np.clip((np.arange(190) if right else np.arange(190)[::-1])/35,0,1)[None,:,None]
 x=751 if right else 0
 backarr[7:146,x:x+190]=backarr[7:146,x:x+190]*(1-weight)+tile*weight
back=Image.fromarray(backarr.round().astype('uint8'))
rail=ref.crop((748,272,930,283)).resize((941,11));back.paste(rail,(0,140))
save('letter_title_backing_r5',back.convert('RGBA'),'Opaque title backing: original right-side carving mirrored to replace avatar-occluded left; approved navy donor beneath the separate title and source lower rail. Reconstructed, not an exact hidden-pixel recovery.')
(b/'Provenance.json').write_text(json.dumps(records,indent=2))
canvas=Image.new('RGB',(1200,1000));
for row,name in enumerate(['letter_title_ru_r5','letter_title_blank_r5','letter_back_r5','letter_inbox_r5','letter_title_backing_r5']):
 im=Image.open(out/(name+'.png'));im.thumbnail((390,190))
 for col,color in enumerate([(0,0,0),(255,255,255),(255,0,150)]):
  canvas.paste(color,(col*400,row*200,(col+1)*400,(row+1)*200));canvas.paste(im,(col*400+(400-im.width)//2,row*200),im)
canvas.save(b.parent/'QA/AlphaBackgrounds.png')
print('Five source-derived title/footer assets prepared.')
