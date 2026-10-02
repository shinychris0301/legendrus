from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np
b=Path(__file__).resolve().parent;s=Image.open(b/'OriginalReference.png').convert('RGB');im=s.crop((92,447,825,609));w,h=im.size
# Empty panel made from source navy and rails; do not retain portraits, names or controls.
donor=np.array(s.crop((501,519,545,582)));donor=np.concatenate([donor,donor[:,::-1]],axis=1);donor=np.concatenate([donor,donor[::-1]],axis=0)
field=Image.fromarray(np.tile(donor,(h//donor.shape[0]+1,w//donor.shape[1]+1,1))[:h,:w]);field.paste(im.crop((w-22,0,w,h)),(w-22,0));field.paste(im.crop((w-22,0,w,h)).transpose(Image.Transpose.FLIP_LEFT_RIGHT),(0,0))
# Continue top and bottom rails through the left portrait's formerly occluded region.
for yy,hh in [(0,13),(h-13,13)]:
 rail=im.crop((230,yy,w-22,yy+hh)).resize((w-44,hh),Image.Resampling.LANCZOS);field.paste(rail,(22,yy))
field.save(b/'Inputs/row.png')
