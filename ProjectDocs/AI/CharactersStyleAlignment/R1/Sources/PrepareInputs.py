from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGB')
regions={'title':(170,102,755,247),'portrait_frame':(10,308,445,843),'detail_frame':(443,309,929,843),'card_frame':(29,848,251,1089),'upgrade':(452,758,683,825),'select':(683,758,909,825)}
for i,(x,y,w,h) in enumerate([(65,861,171,162),(270,862,179,163),(482,862,185,163),(707,862,177,163),(48,1107,184,145),(266,1107,181,145),(484,1107,186,145),(709,1107,177,145)]):regions['portrait_'+str(i)]=(x,y,x+w,y+h)
def field(box,size):
 a=np.array(src.crop(box)).astype('float32');grain=a-cv2.GaussianBlur(a,(0,0),5);grain-=grain.mean((0,1));t=np.concatenate([grain,grain[:,::-1]],1);t=np.concatenate([t,t[::-1]],0);w,h=size;t=np.tile(t,((h+t.shape[0]-1)//t.shape[0],(w+t.shape[1]-1)//t.shape[1],1))[:h,:w];return Image.fromarray(np.uint8(np.clip(np.median(a.reshape(-1,3),0)+t*.6,0,255)))
records=[]
for n,box in regions.items():
 im=src.crop(box);w,h=im.size;mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 if n=='title':d.rectangle((104,53,506,116),fill=255);mask=mask.filter(ImageFilter.MaxFilter(7)).filter(ImageFilter.GaussianBlur(2));im=Image.composite(field((298,143,404,155),im.size),im,mask)
 elif n in ['portrait_frame','detail_frame','card_frame']:
  inset=19 if n=='card_frame' else 17
  d.rectangle((inset,inset,w-inset,h-inset),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(2));im=Image.composite(field((783,379,868,404),im.size),im,mask)
 elif n in ['upgrade','select']:
  d.rounded_rectangle((30,16,w-30,h-15),radius=5,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(2));donor=(502,771,615,780) if n=='upgrade' else (729,771,854,780);im=Image.composite(field(donor,im.size),im,mask)
 im.save(b/'Inputs'/f'{n}.png');mask.save(b/'RepairMasks'/f'{n}.png');records.append({'name':n,'bounds':box,'inputSha256':hashlib.sha256((b/'Inputs'/f'{n}.png').read_bytes()).hexdigest()})
(b/'Assessment.json').write_text(json.dumps({'source':'/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (1).png','sourceSha256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'assets':records,'decision':'Scenario alpha only. Preserve source faces/frame RGB; bounded clear lettering/occupied panel interiors. Reuse clean scenery, approved main Ivan/Firebird and native header/footer resources. No RGB regeneration.'},indent=2))
