from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib,re,uuid
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');sheet=Image.open(b/'Sheet.png').convert('RGB');cut=Image.open(b/'Scenario/footer_detail/booster_medallion_trial01.png').convert('RGBA');ref=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB');records=[]
def save(name,im,method):
 p=out/(name+'.png');im.save(p)
 if not p.with_suffix('.png.meta').exists():
  s=(out/'title_ru.png.meta').read_text();s=re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,s,count=1);p.with_suffix('.png.meta').write_text(s)
 records.append({'name':name,'method':method,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
for item in json.loads((b/'Sheet.json').read_text()):
 name=item['name'];rgb=sheet.crop(item['sheetBox']);alpha=cut.crop(item['sheetBox']).getchannel('A');w,h=rgb.size
 if name=='detail_sword_r3':
  rgb=ref.crop((56,1270,260,1509)).resize((408,478),Image.Resampling.LANCZOS);w,h=rgb.size
  # Expanded original crop restores curls omitted by the initial tight submission.
  alpha=Image.new('L',(w,h));ImageDraw.Draw(alpha).rounded_rectangle((4,4,w-5,h-5),radius=28,fill=255)
  method='Expanded original source box [56,1270,260,1509] at 2x with bounded rounded alpha; no new sword pixels. Restores curls beyond submitted crop; intentionally opaque navy portrait face.'
 else:
  outer=Image.new('L',(w,h));ImageDraw.Draw(outer).rounded_rectangle((2,2,w-3,h-3),radius=20 if name=='footer_strip_r3' else 17,fill=255)
  inside=Image.new('L',(w,h));ImageDraw.Draw(inside).rectangle((26,22,w-26,h-18),fill=255)
  alpha=Image.fromarray(np.maximum(np.minimum(np.array(alpha),np.array(outer)),np.array(inside)))
  method='Prepared original panel RGB with Scenario alpha, bounded outside and opaque interior restored. Clean wood/navy donors replace old content. Footer retains original divider strips.'
 im=rgb.convert('RGBA');im.putalpha(alpha);save(name,im,method)
 if name=='footer_strip_r3':
  for i,(x,width) in enumerate(((20,292),(322,162),(496,196))):
   cell=im.crop((x*2,30,(x+width)*2,214));cell.putalpha(255);save('footer_cell_'+str(i)+'_r3',cell,'Exact blank wood face from prepared continuous footer; crop aligns with native action coordinates.')
   arr=np.array(cell);H,W=arr.shape[:2];y,xx=np.indices((H,W));weight=np.exp(-(((xx-W*.5)/(W*.38))**2+((y-H*.43)/(H*.55))**2)*2)*.48
   # Fade exactly to zero at all edges, avoiding selection rectangles over the carving.
   weight*=np.sin(np.pi*xx/max(1,W-1))**2*np.sin(np.pi*y/max(1,H-1))**2
   arr[:,:,:3]=np.round(arr[:,:,:3]*(1-weight[:,:,None])+np.array([10,84,139])*weight[:,:,None]).astype('uint8');save('footer_cell_'+str(i)+'_active_r3',Image.fromarray(arr),'Same aligned wood face with bounded sapphire selection tint; outer pixels unchanged.')
save('detail_r3_frame',Image.open(out/'detail_surface_r1.png').convert('RGBA'),'Approved R1 detail panel pixels reused unchanged; new importer borders allow nine-slice corner preservation.')
(b/'Provenance.json').write_text(json.dumps(records,indent=2)+'\n')
canvas=Image.new('RGB',(1200,len(records)*180))
for row,item in enumerate(records):
 im=Image.open(out/(item['name']+'.png'));im.thumbnail((385,170))
 for col,color in enumerate(['black','white','#ff0099']):
  canvas.paste(color,(col*400,row*180,(col+1)*400,row*180+180));canvas.paste(im,(col*400+7,row*180+4),im)
canvas.save(b.parent/'QA/AlphaBackgrounds.png');print('Created',len(records),'assets')
