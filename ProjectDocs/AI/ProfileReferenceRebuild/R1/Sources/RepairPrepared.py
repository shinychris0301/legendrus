from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,shutil
b=Path(__file__).resolve().parent;ref=Image.open(b/'Reference.png').convert('RGB')
def tiled(donor,size):
 a=np.array(donor);a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0);w,h=size;return Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
repairs={}
for name,box,pad,donor in [('collection_surface_r1',(8,677,934,999),(27,17,27,10),ref.crop((783,244,882,291))),('footer_surface_r1',(647,1534,906,1647),(12,12,12,12),ref.crop((701,134,830,206)))]:
 p=b/'Prepared'/(name+'.png');backup=b/'Prepared'/(name+'_BeforeRepair.png')
 if not backup.exists():shutil.copy2(p,backup)
 im=ref.crop(box);w,h=im.size;l,t,r,bot=pad;m=Image.new('L',im.size);ImageDraw.Draw(m).rounded_rectangle((l,t,w-r,h-bot),radius=8,fill=255);m=m.filter(ImageFilter.GaussianBlur(1));Image.composite(tiled(donor,im.size),im,m).save(p)
 m.save(b/'Prepared'/(name+'_FinalRepairMask.png'));repairs[name]={'sourceBox':box,'reason':'Clear residual upper caption strokes/edge card fragments' if name.startswith('collection') else 'Expand source crop to retain complete right footer rail','method':'Source border plus same clean navy/wood donor; original submitted sheet remains immutable.'}
(b/'PreparedRepairs.json').write_text(json.dumps(repairs,indent=2))
