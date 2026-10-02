from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGB')
# Source tab groups remain opaque inside their carved contour. No scenery crops.
groups=[((13,284,414,348),[(14,318),(22,309),(28,302),(32,295),(42,287),(386,287),(395,296),(401,299),(406,310),(413,315),(404,324),(400,333),(387,344),(40,344),(27,336),(24,329)]),((529,284,916,348),[(530,318),(538,307),(546,302),(552,291),(563,287),(886,287),(895,295),(902,299),(907,308),(915,315),(908,325),(902,333),(890,344),(561,344),(548,335),(541,328)])]
alpha=Image.new('L',ref.size)
for box,points in groups:
 m=Image.new('L',(ref.width*3,ref.height*3));ImageDraw.Draw(m).polygon([(x*3,y*3) for x,y in points],fill=255);m=m.resize(ref.size,Image.Resampling.LANCZOS);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(m)))
# Mask only lettering plus emboss shadow; original carving is outside these boxes.
mask=np.zeros((ref.height,ref.width),np.uint8)
for box in [(51,300,193,334),(222,300,379,334),(584,300,697,334),(768,300,862,334)]:
 x,y,x2,y2=box;mask[y:y2,x:x2]=255
blank=Image.fromarray(cv2.inpaint(np.array(ref),mask,5,cv2.INPAINT_NS));blank.save(b/'Sources/ClearedTabs.png')
faces=[[(50,300),(187,300),(197,307),(202,312),(202,323),(190,335),(50,335),(41,325),(40,312)],[(219,299),(380,299),(386,306),(398,317),(385,334),(219,334),(216,328),(217,305)],[(561,300),(711,300),(717,307),(719,325),(710,334),(560,334),(549,327),(543,316),(554,306)],[(743,300),(883,300),(890,307),(894,317),(888,326),(881,334),(743,334),(736,328),(736,306)]]
boxes=[(13,284,208,348),(208,284,414,348),(529,284,728,348),(728,284,916,348)]
shared=Path('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned')
navy=Image.open(shared/'tab.png').convert('RGB').crop((100,48,255,87));blue=Image.open(shared/'tab_active.png').convert('RGB').crop((100,48,255,87));records=[];assets=[]
for i,box in enumerate(boxes):
 for active in [False,True]:
  im=blank.copy()
  if i==3 and not active:
   a=np.array(im).astype(float);m=(a[:,:,2]>a[:,:,0]*1.2)&(a[:,:,2]>a[:,:,1]*1.08);a[m]*=np.array([.65,.65,.52]);im=Image.fromarray(np.uint8(a))
  face=Image.new('L',ref.size);ImageDraw.Draw(face).polygon(faces[i],fill=255);face=face.filter(ImageFilter.GaussianBlur(.4))
  bounds=face.getbbox();patch=Image.new('RGB',ref.size)
  material=(blue if i>=2 and active else navy).resize((bounds[2]-bounds[0],bounds[3]-bounds[1]),Image.Resampling.LANCZOS)
  if i<2 and active:
   a=np.array(material).astype(float);a*=np.array([1.5,1.18,.80]);material=Image.fromarray(np.uint8(np.clip(a,0,255)))
  # Restore all states from the same original material family, keeping carved rails fixed.
  patch.paste(material,bounds[:2]);im=Image.composite(patch,im,face);im.putalpha(alpha);im=im.crop(box)
  n=f'tab_{i}_{"active" if active else "normal"}_r6';out=art/(n+'.png');im.save(out)
  meta=Path(str(out)+'.meta')
  if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details_ru_r2.png.meta').read_text(),count=1))
  records.append(dict(name=n,sourceBox=box,sha256=hashlib.sha256(out.read_bytes()).hexdigest(),operation='Original contour/rails; text cleared for localization; approved IndividualRanking tab navy/sapphire faces reused; scope selected face warmed'))
  assets.append((n,im))
sheet=Image.new('RGB',(1050,8*125));d=ImageDraw.Draw(sheet)
for i,(n,im) in enumerate(assets):
 for j,c in enumerate(['black','white','#ff00dd']):
  d.rectangle((j*350,i*125,(j+1)*350,(i+1)*125),fill=c);thumb=im.resize((im.width*3//2,96));sheet.paste(thumb,(j*350+10,i*125+2),thumb)
 d.text((8,i*125+103),n,fill='#bbbbbb')
sheet.save(b/'QA/AssetAlpha.png');(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
