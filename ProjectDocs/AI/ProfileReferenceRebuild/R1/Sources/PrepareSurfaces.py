from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,cv2
b=Path(__file__).resolve().parent;ref=Image.open(b/'Reference.png').convert('RGB');items=[];sheet=Image.new('RGB',(1900,1840),(211,211,211));(b/'Prepared').mkdir(exist_ok=True)
# Clean empty lacquer from the original identity panel, wood from its title surround.
navy=ref.crop((783,244,882,291));wood=ref.crop((701,134,830,206))
def texture(donor,size):
 a=np.array(donor);a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);w,h=size;return Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
configs=[('identity_surface_r1',(410,223,912,383),(12,12,13,12),(20,20),'navy'),('stats_surface_r1',(410,390,646,592),(12,12,12,12),(1060,20),'navy'),('collection_surface_r1',(8,677,934,999),(31,24,31,13),(20,440),'navy'),('detail_surface_r1',(53,1266,892,1519),(23,16,18,16),(20,1120),'navy'),('tab_surface_r1',(247,600,470,670),(23,15,23,14),(1440,20),'navy'),('tab_active_surface_r1',(21,600,247,670),(25,15,25,14),(1440,190),'blue'),('footer_surface_r1',(643,1534,872,1647),(12,12,12,12),(1420,430),'wood')]
for name,box,pad,xy,material in configs:
 im=ref.crop(box);w,h=im.size;l,t,r,bot=pad;mask=Image.new('L',im.size);d=ImageDraw.Draw(mask);d.rounded_rectangle((l,t,w-r,h-bot),radius=8,fill=255)
 # Identity crest retained; only the text/progress footprint is blanked.
 if name=='identity_surface_r1':mask=Image.new('L',im.size);ImageDraw.Draw(mask).rounded_rectangle((116,10,w-15,h-12),radius=5,fill=255)
 face=texture(wood if material=='wood' else navy,im.size)
 if material=='blue':
  a=np.array(face).astype(float);a=a*np.array([.48,1.15,1.75])+np.array([0,8,18]);face=Image.fromarray(np.clip(a,0,255).astype('uint8'))
 mask=mask.filter(ImageFilter.GaussianBlur(1));im=Image.composite(face,im,mask)
 if name=='stats_surface_r1':
  # Original fine horizontal divider repeated, excluding the old icon/labels.
  line=ref.crop((473,454,628,460));im.paste(line.resize((204,4)),(16,65));im.paste(line.resize((204,4)),(16,129))
 im.save(b/'Prepared'/(name+'.png'));mask.save(b/'Prepared'/(name+'_RepairMask.png'))
 # Keep broad collection panel at native scale; other assets at 2x.
 scale=1 if name in ('collection_surface_r1','detail_surface_r1') else 2
 sized=im.resize((w*scale,h*scale),Image.Resampling.LANCZOS);sheet.paste(sized,xy);items.append({'name':name,'sourceBox':box,'sheetBox':[xy[0],xy[1],xy[0]+sized.width,xy[1]+sized.height],'scale':scale,'material':material})
sheet.save(b/'Sheet.png');(b/'Sheet.json').write_text(json.dumps(items,indent=2));print('Prepared seven source-frame surfaces.')
