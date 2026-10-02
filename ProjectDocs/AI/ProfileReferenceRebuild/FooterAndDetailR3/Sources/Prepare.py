from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;ref=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB');sheet=Image.new('RGB',(1600,790),(210,210,210));items=[]
def tile(box,size):
 a=np.array(ref.crop(box));a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);w,h=size;return Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
box=(158,1528,905,1648);im=ref.crop(box);mask=Image.new('L',im.size);ImageDraw.Draw(mask).rounded_rectangle((20,14,697,110),radius=5,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(1));im=Image.composite(tile((701,134,830,206),im.size),im,mask)
# Keep the actual two vertical source separators between actions.
for x in (474,646):im.paste(ref.crop((x-2,1543,x+2,1636)),(x-2-box[0],1543-box[1]))
im.save(b/'FooterPrepared.png');mask.save(b/'FooterRepairMask.png');large=im.resize((1494,240),Image.Resampling.LANCZOS);sheet.paste(large,(35,25));items.append({'name':'footer_strip_r3','sourceBox':box,'sheetBox':[35,25,1529,265],'scale':2})
for name,box,xy in [('detail_sword_r3',(62,1275,253,1505),(35,300)),('preview_tag_r3',(614,1280,795,1320),(500,300))]:
 im=ref.crop(box)
 if name=='preview_tag_r3':
  m=Image.new('L',im.size);ImageDraw.Draw(m).rectangle((15,10,im.width-15,im.height-10),fill=255);m=m.filter(ImageFilter.GaussianBlur(.7));im=Image.composite(tile((783,244,882,291),im.size),im,m);m.save(b/'TagRepairMask.png')
 im=im.resize((im.width*2,im.height*2),Image.Resampling.LANCZOS);sheet.paste(im,xy);items.append({'name':name,'sourceBox':box,'sheetBox':[xy[0],xy[1],xy[0]+im.width,xy[1]+im.height],'scale':2})
sheet.save(b/'Sheet.png');(b/'Sheet.json').write_text(json.dumps(items,indent=2))
