from pathlib import Path
from PIL import Image,ImageDraw
import json,numpy as np,cv2,re,uuid,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');ref=Image.open(b/'Reference.png').convert('RGB');sheet=Image.open(b/'Sheet.png').convert('RGB');cut=Image.open(b/'Scenario/profile_surfaces/booster_medallion_trial01.png').convert('RGBA');records=[]
def save(name,im,method):
 p=out/(name+'.png');im.save(p)
 if not p.with_suffix('.png.meta').exists():
  s=(out/'title_ru.png.meta').read_text();s=re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,s,count=1);p.with_suffix('.png.meta').write_text(s)
 records.append({'name':name,'method':method,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
for item in json.loads((b/'Sheet.json').read_text()):
 name=item['name'];rgb=Image.open(b/'Prepared'/(name+'.png')).convert('RGB');rgb=rgb.resize((rgb.width*item['scale'],rgb.height*item['scale']),Image.Resampling.LANCZOS);W,H=rgb.size;alpha=cut.crop(item['sheetBox']).getchannel('A').resize((W,H),Image.Resampling.LANCZOS);a=np.array(alpha)
 # Preserve intentionally opaque navy/wood and thin dark frame recesses.
 mask=Image.new('L',(W,H));d=ImageDraw.Draw(mask);q=item['scale'];d.rounded_rectangle((1*q,1*q,W-1*q,H-1*q),radius=7*q,fill=255)
 interior=Image.new('L',(W,H));ImageDraw.Draw(interior).rectangle((12*q,12*q,W-12*q,H-12*q),fill=255)
 alpha=Image.fromarray(np.maximum(np.minimum(a,np.array(mask)),np.array(interior)));
 if name=='footer_surface_r1':alpha=mask # Original submitted crop omitted the right rail; extended source uses a bounded opaque wood face.
 im=rgb.convert('RGBA');im.putalpha(alpha);method='Prepared original frame RGB, Scenario isolation with bounded exterior and intentionally opaque interior restoration. Navy/wood donors remove old content; source identity banner retained.'
 if name=='stats_surface_r1':method+=' RGB comes from the independently prepared panel, avoiding a tab overlap on the submitted alpha sheet.'
 if name=='collection_surface_r1':method+=' Final clean donor mask expanded to remove caption fragments; original submitted sheet retained.'
 if name=='footer_surface_r1':method='Expanded original footer crop restores its right rail; navy-free clean source wood donor and bounded opaque rounded silhouette. Scenario sheet crop was too narrow and its exterior alpha is not used for this final footer.'
 save(name,im,method)
 if name=='footer_surface_r1':
  arr=np.array(im);y,x=np.indices((H,W));weight=np.exp(-(((x-W*.5)/(W*.32))**2+((y-H*.43)/(H*.42))**2)*2)*.48
  weight*=np.array(interior)/255
  arr[:,:,:3]=(arr[:,:,:3]*(1-weight[:,:,None])+np.array([10,84,139])*weight[:,:,None]).round().astype('uint8');save('footer_active_surface_r1',Image.fromarray(arr),'Same source wooden footer frame; bounded sapphire selection tint on blank wood. Live label and approved original icon remain separate.')
a=np.array(ref.crop((701,134,830,206)));a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);a=np.tile(a,(2,4,1))[:101,:941];save('wood_band_r1',Image.fromarray(a).convert('RGBA'),'Clean source wood donor mirrored across an opaque title backing, no full-page reference used.')
(b/'Provenance.json').write_text(json.dumps(records,indent=2))
canvas=Image.new('RGB',(1200,200*len(records)))
for row,item in enumerate(records):
 im=Image.open(out/(item['name']+'.png')).convert('RGBA');im.thumbnail((388,190))
 for col,color in enumerate(((0,0,0),(255,255,255),(255,0,150))):
  canvas.paste(color,(col*400,row*200,(col+1)*400,row*200+200));canvas.paste(im,(col*400+(400-im.width)//2,row*200),im)
canvas.save(b.parent/'QA/AlphaBackgrounds.png');print(len(records),'new panel assets')
