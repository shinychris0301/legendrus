from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;ref=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB')
def tile(box,size):
 a=np.array(ref.crop(box));a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);w,h=size;return Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
box=(4,356,938,1111);im=ref.crop(box);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
# Interior contains item art; use the source's clean navy between its framed cards.
d.rectangle((33,27,902,728),fill=255)
im=Image.composite(tile((209,401,215,475),im.size),im,mask)
# The next panel's crest overlaps the bottom rail; continue the unobstructed source rail.
im.paste(ref.crop((320,1085,430,1111)),(98,729));d.rectangle((98,729,207,754),fill=255)
im.save(b/'GridPrepared.png');mask.save(b/'GridRepairMask.png')
box2=(35,1412,283,1486);q=ref.crop(box2);qm=Image.new('L',q.size);ImageDraw.Draw(qm).rectangle((23,9,226,64),fill=255);qm=qm.filter(ImageFilter.GaussianBlur(1))
q=Image.composite(tile((67,1445,98,1467),q.size),q,qm);q.save(b/'QuantityPrepared.png');qm.save(b/'QuantityRepairMask.png')
sheet=Image.new('RGB',(1024,1024),(206,206,206));sheet.paste(im,(40,30));large=q.resize((744,222),Image.Resampling.LANCZOS);sheet.paste(large,(140,790));sheet.save(b/'Sheet.png')
(b/'Sheet.json').write_text(json.dumps([{'name':'grid_frame_r2','sourceBox':box,'sheetBox':[40,30,974,785],'scale':1,'opaqueInterior':[32,27,903,729]},{'name':'quantity_frame_r2','sourceBox':box2,'sheetBox':[140,790,884,1012],'scale':3,'opaqueInterior':[15,7,232,68]}],indent=2))
