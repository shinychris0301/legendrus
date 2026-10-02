from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;ref=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB');sheet=Image.new('RGB',(1080,760),(210,210,210));records=[]
box=(8,993,935,1522);frame=ref.crop(box);w,h=frame.size
# Preserve the source outer carving; replace the content with approved R1 navy.
donor=ref.crop((783,244,882,291));a=np.array(donor);a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);face=Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
mask=Image.new('L',(w,h));ImageDraw.Draw(mask).rounded_rectangle((27,21,w-26,h-16),radius=9,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(1));frame=Image.composite(face,frame,mask)
frame.save(b/'FramePrepared.png');mask.save(b/'FrameRepairMask.png');sheet.paste(frame,(50,25));records.append({'name':'relic_collection_r2_frame','sourceBox':box,'sheetBox':[50,25,50+w,25+h],'scale':1})
for name,box,xy in [('heading_left_r2',(222,696,321,724),(50,610)),('heading_right_r2',(592,696,691,724),(520,610))]:
 im=ref.crop(box).resize((396,112),Image.Resampling.LANCZOS);sheet.paste(im,xy);records.append({'name':name,'sourceBox':box,'sheetBox':[xy[0],xy[1],xy[0]+396,xy[1]+112],'scale':4})
sheet.save(b/'Sheet.png');(b/'Sheet.json').write_text(json.dumps(records,indent=2))
