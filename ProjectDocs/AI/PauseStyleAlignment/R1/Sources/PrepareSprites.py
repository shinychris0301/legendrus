"""Source extraction and bounded surface repairs; authorized extraction-first workflow."""
from pathlib import Path
import json,hashlib
import numpy as np
from PIL import Image,ImageDraw
import cv2
r=Path.cwd();o=r/'ProjectDocs/AI/PauseStyleAlignment/R1';out=o/'Staging';out.mkdir(exist_ok=True)
info=json.loads((o/'Sources/Provenance.json').read_text());src=Image.open(r/info['source']).convert('RGB');audit=[]
def save(n,im,source=None,repair=None):
 im.save(out/(n+'.png'));audit.append({'name':n,'size':im.size,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest(),'source':source,'repair':repair})
def maskpoly(size,points):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in points],fill=255);return m.resize(size,Image.Resampling.LANCZOS)
for n in ('title','resume','restart','settings','exit'):
 original=Image.open(o/'Sources/Crops'/f'{n}.png').convert('RGBA');alpha=Image.open(o/'Sources/Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(original.size,Image.Resampling.LANCZOS)
 d=ImageDraw.Draw(alpha)
 if n=='title':d.rectangle((95,82,525,157),fill=255)
 else:d.rectangle((61,24,397,88),fill=255)
 
 if n=='title':
  clip=maskpoly(original.size,[(0,0),(621,0),(621,167),(540,167),(510,179),(338,179),(311,197),(284,179),(111,179),(76,167),(0,167)]);alpha=Image.fromarray(np.minimum(np.array(alpha),np.array(clip)))
 original.putalpha(alpha);save(n+'_ru',original,info['boxes'][n],'Scenario alpha; original RGB unchanged')
 blank=np.array(original.convert('RGB'));fixed=blank.copy();mask=np.zeros(blank.shape[:2],np.uint8)
 if n=='title':x0,y0,x1,y1=163,84,466,157;tile=blank[84:157,112:159]
 else:x0,y0,x1,y1=68,24,396,88;tile=blank[24:88,55:68]
 tile=np.concatenate([tile,tile[:,::-1]],axis=1);hh,ww=y1-y0,x1-x0
 fixed[y0:y1,x0:x1]=np.tile(tile,(1,ww//tile.shape[1]+1,1))[:hh,:ww]
 mask[y0:y1,x0:x1]=255
 blend=cv2.seamlessClone(fixed,blank,mask.copy(),((x0+x1)//2,(y0+y1)//2),cv2.NORMAL_CLONE)
 blank[mask>0]=blend[mask>0];im=Image.fromarray(blank).convert('RGBA');im.putalpha(alpha);save(n+'_blank',im,info['boxes'][n],'source texture clone and bounded Poisson blend for live English')

panel=src.crop(info['boxes']['panel']).convert('RGBA');a=np.array(panel);patch=np.array(src.crop((213,460,225,760)))
for y in range(38,983):
 for x in range(38,558):
  yy=(y-38)%(patch.shape[0]*2);yy=min(yy,patch.shape[0]*2-1-yy);xx=(x-38)%(patch.shape[1]*2);xx=min(xx,patch.shape[1]*2-1-xx);a[y,x,:3]=patch[yy,xx]
panel=Image.fromarray(a);panel.putalpha(maskpoly(panel.size,[(30,0),(565,0),(579,28),(583,62),(583,930),(594,951),(590,986),(568,1011),(510,1012),(308,1023),(269,1012),(63,1011),(23,1016),(2,992),(2,963),(10,938),(10,68),(3,36)]));save('panel',panel,info['boxes']['panel'],'interior reconstructed from source lacquer, hand-isolated frame')
goal=src.crop((252,486,690,706)).convert('RGBA');g=np.array(goal);g[5:213,6:432,:3]=np.array(panel)[100:308,65:491,:3];g[:12,87:357,:3]=np.array(panel)[100:112,100:370,:3];goal=Image.fromarray(g);save('goals',goal,[252,486,690,706],'source border; repaired opaque face for live goals')
for n in ('mace','firebird','shield','potion'):
 im=src.crop(info['boxes'][n]).convert('RGBA');mask=Image.new('L',(464,532));d=ImageDraw.Draw(mask);d.rounded_rectangle((12,16,432,476),radius=68,fill=255);d.ellipse((268,336,448,516),fill=255);im.putalpha(mask.resize(im.size,Image.Resampling.LANCZOS));ar=np.array(im);yy,xx=np.mgrid[0:133,0:116];m=((xx-89)/14)**2+((yy-106)/16)**2<=1;ar[m,:3]=[18,58,30];save(n,Image.fromarray(ar),info['boxes'][n],'source card; count face repaired for live stock')
for bg,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 sheet=Image.new('RGB',(1100,1100),col);d=ImageDraw.Draw(sheet);y=12
 for n in ['title_ru','resume_ru','restart_ru','settings_ru','exit_ru']:
  im=Image.open(out/(n+'.png'));im.thumbnail((610,195));sheet.paste(im,(8,y),im);d.text((625,y+10),n,fill=(170,170,170));y+=im.height+12
 im=Image.open(out/'panel.png');im.thumbnail((350,630));sheet.paste(im,(730,100),im)
 for i,n in enumerate(['mace','firebird','shield','potion']):
  im=Image.open(out/(n+'.png'));sheet.paste(im,(10+i*140,900),im)
 sheet.save(o/'QA'/('AlphaOn'+bg+'.png'))
(o/'QA/ExtractionAudit.json').write_text(json.dumps(audit,indent=2)+'\n');print('Prepared',len(audit),'assets')
