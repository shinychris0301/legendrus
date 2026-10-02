"""Source RGB + Scenario alpha; bounded local UI surface repairs; generated clean plate."""
from pathlib import Path
import json,hashlib
import numpy as np
from PIL import Image,ImageDraw
import cv2
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/R1';out=o/'Staging';out.mkdir(exist_ok=True)
p=json.loads((o/'Sources/Provenance.json').read_text());source=Image.open(r/p['source']).convert('RGB');audit=[]
def save(n,im,box=None,repair='none'):
 im.save(out/(n+'.png'));audit.append({'name':n,'size':im.size,'sourceBox':box,'repair':repair,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
def rounded(size,box,radius):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).rounded_rectangle(tuple(v*4 for v in box),radius=radius*4,fill=255);return m.resize(size,Image.Resampling.LANCZOS)
def repaired(im,boxes,donors):
 src=np.array(im.convert('RGB'));fixed=src.copy()
 for (x0,y0,x1,y1),(dx0,dy0,dx1,dy1) in zip(boxes,donors):
  tile=src[dy0:dy1,dx0:dx1];tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);hh,ww=y1-y0,x1-x0
  fixed[y0:y1,x0:x1]=np.tile(tile,(hh//tile.shape[0]+1,ww//tile.shape[1]+1,1))[:hh,:ww]
  m=np.zeros(src.shape[:2],np.uint8);m[y0:y1,x0:x1]=255
  blended=cv2.seamlessClone(fixed,src,m.copy(),((x0+x1)//2,(y0+y1)//2),cv2.NORMAL_CLONE);fixed[m>0]=blended[m>0]
 im2=Image.fromarray(fixed).convert('RGBA');im2.putalpha(im.getchannel('A'));return im2
for n in ('header','banner','foreground','next','map'):
 im=source.crop(p['boxes'][n]).convert('RGBA');alpha=Image.open(o/'Sources/Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS);a=np.array(alpha)
 if n=='foreground':a[652:,:166]=0;a[652:,715:]=0
 # Preserve intentionally opaque lettering faces, not scene background.
 if n=='header':a[62:148,152:790]=255
 if n=='banner':a[69:157,87:542]=255
 if n in ('next','map'):a[22:75,53:331]=255
 im.putalpha(Image.fromarray(a));save(n+'_ru',im,p['boxes'][n],'Scenario alpha with bounded silhouette/opaque-face corrections; original RGB unchanged')
 boxes={'header':[(168,72,788,134)],'banner':[(252,52,406,84),(83,88,548,144)],'foreground':[(337,641,540,688)],'next':[(112,20,281,76)],'map':[(88,24,281,71)]}[n]
 donors={'header':[(190,62,270,73)],'banner':[(115,60,159,85),(113,65,160,85)],'foreground':[(273,645,311,682)],'next':[(70,27,109,67)],'map':[(44,27,81,67)]}[n]
 blank=repaired(im,boxes,donors)
 if n=='foreground':
  pixels=np.array(blank);tile=np.array(im)[648:675,315:329,:3];tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0)
  pixels[639:688,331:548,:3]=np.tile(tile,(2,8,1))[:49,:217];blank=Image.fromarray(pixels)
 save(n+'_blank',blank,p['boxes'][n],'bounded text removal via source texture and Poisson blend; Victory face uses direct clean ribbon texture')
# Opaque carved-wood reward/footer board, retained original frame; inner contents rebuilt as real controls.
box=(5,1037,936,1672);im=source.crop(box).convert('RGBA');a=np.array(im);patch=np.array(source.crop((80,1135,145,1250)));patch=np.concatenate([patch,patch[:,::-1]],1);patch=np.concatenate([patch,patch[::-1]],0)
hh,ww=585,835;a[4:589,48:883,:3]=np.tile(patch,(hh//patch.shape[0]+1,ww//patch.shape[1]+1,1))[:hh,:ww]
original=np.array(source.crop(box));a[4:24,48:188,:3]=original[4:24,48:188];a[4:24,739:883,:3]=original[4:24,739:883]
im=Image.fromarray(a);im.putalpha(rounded(im.size,(5,4,925,630),43));save('footer',im,box,'central opaque wood restored from clean source wood; original outer frame preserved')
# Intentionally opaque teaser vignette inside its own frame, never a page background crop.
im=source.crop(p['boxes']['teaser']).convert('RGBA');im.putalpha(rounded(im.size,(3,3,787,198),24));save('teaser_ru',im,p['boxes']['teaser'],'local outer frame alpha; original RGB unchanged')
# English retains the forest illustration; replaces only the parchment caption area.
blank=repaired(im,[(70,23,490,172)],[(25,51,67,141)]);save('teaser_blank',blank,p['boxes']['teaser'],'source parchment repair for live English teaser')
im=source.crop(p['boxes']['reward']).convert('RGBA');im.putalpha(rounded(im.size,(3,3,131,136),17))
# Empty navy reference reward tile: clean navy donor between coin and border.
a=np.array(im);tile=np.array(source.crop((182,1150,197,1205)));tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);a[15:127,15:120,:3]=np.tile(tile,(3,4,1))[:112,:105];im=Image.fromarray(a);save('reward_card',im,p['boxes']['reward'],'source border retained; icon/amount removed for live reward categories')
for n in ('coin','gem'):
 im=source.crop(p['boxes'][n]).convert('RGBA');w,h=im.size
 m=Image.new('L',(w*4,h*4));d=ImageDraw.Draw(m)
 if n=='coin':d.ellipse((8,4,(w-4)*4,(h-3)*4),fill=255)
 else:d.polygon([(x*4,y*4) for x,y in [(9,26),(25,8),(57,8),(80,26),(43,77)]],fill=255)
 im.putalpha(m.resize(im.size,Image.Resampling.LANCZOS));save(n,im,p['boxes'][n],'local silhouette alpha; original RGB unchanged')
im=Image.open(o/'Sources/Scenario/environment/booster_medallion_trial01.png').convert('RGB');save('environment',im,None,'new Scenario clean sunset village plate')
for bg,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 for group,names in [('Main',['header_ru','banner_ru','foreground_ru','next_ru','map_ru','footer','teaser_ru','reward_card','coin','gem']),('Fallback',['header_blank','banner_blank','foreground_blank','next_blank','map_blank','teaser_blank'])]:
  sheet=Image.new('RGB',(1200,1700),col);x=y=10;rowh=0
  for n in names:
   im=Image.open(out/(n+'.png')).convert('RGBA');im.thumbnail((570,580))
   if x+im.width>1200:x=10;y+=rowh+25;rowh=0
   sheet.paste(im,(x,y),im);rowh=max(rowh,im.height);x+=im.width+25
  sheet.save(o/'QA'/(group+'AlphaOn'+bg+'.png'))
(o/'QA/ExtractionAudit.json').write_text(json.dumps(audit,indent=2)+'\n');print('Prepared',len(audit),'sprites')
