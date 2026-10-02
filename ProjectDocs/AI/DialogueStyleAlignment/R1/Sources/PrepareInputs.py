from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGB')
regions={'panel':(45,1022,894,1640),'continue':(195,1454,747,1592),'ivan':(0,254,490,1124),'dobrynya':(474,261,941,1124)}
def tile(box,size):
 t=src.crop(box);w,h=t.size;tt=Image.new('RGB',(w*2,h*2));tt.paste(t,(0,0));tt.paste(ImageOps.mirror(t),(w,0));tt.paste(ImageOps.flip(t),(0,h));tt.paste(ImageOps.flip(ImageOps.mirror(t)),(w,h));o=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):o.paste(tt,(x,y))
 return o
records=[]
for n,box in regions.items():
 im=src.crop(box);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 if n=='panel':
  # One continuous paper replacement avoids mismatched rectangular patches.
  d.rounded_rectangle((26,153,823,567),radius=35,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(8));im=Image.composite(tile((86,1162,183,1204),im.size),im,mask)
  head=Image.new('L',im.size);ImageDraw.Draw(head).rectangle((280,65,570,133),fill=255);head=head.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(3));im=Image.composite(tile((294,1081,614,1091),im.size),im,head);mask=Image.fromarray(np.maximum(np.array(mask),np.array(head)))
 elif n=='continue':
  d.rectangle((99,40,454,103),fill=255);mask=mask.filter(ImageFilter.MaxFilter(7)).filter(ImageFilter.GaussianBlur(2));im=Image.composite(tile((300,1480,625,1495),im.size),im,mask)
 im.save(b/'Inputs'/f'{n}.png');mask.save(b/'RepairMasks'/f'{n}.png');records.append({'name':n,'bounds':box,'inputSha256':hashlib.sha256((b/'Inputs'/f'{n}.png').read_bytes()).hexdigest()})
(b/'Assessment.json').write_text(json.dumps({'source':'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Dialogue.png','sourceSha256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'assets':records,'decision':'Recover visible character/UI RGB via Scenario alpha. Hidden background cannot be recovered; one clean hall plate generated. All speaker/body/control text native.'},indent=2))
