from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;q=b.parent/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned');g=Image.open(b/'Scenario/booster_medallion_trial01.png').convert('RGBA');boxes=json.loads((b/'SourceBoxes.json').read_text());audit={}
def save(key,im,src=None,mask=None):
 if key in ['reward_rim','footer_chest']:
  unused=b/'UnusedBeforeReuseInstruction';unused.mkdir(exist_ok=True);im.save(unused/(key+'_r4.png'))
 else:im.save(art/(key+'_r4.png'))
 if src is not None:
  a=np.array(im);s=np.array(src);m=a[:,:,3]>0
  if mask is not None:m&=np.array(mask)==0
  audit[key]={'sourcePixelsCompared':int(m.sum()),'RGBMismatches':int(np.any(a[:,:,:3][m]!=s[:,:,:3][m],axis=1).sum())}
 canvas=Image.new('RGB',(max(im.width,210),3*(im.height+22)),(30,35,40));d=ImageDraw.Draw(canvas)
 for i,(color,label) in enumerate([('black','BLACK'),('white','WHITE'),((235,0,210),'MAGENTA')]):
  y=i*(im.height+22);d.text((5,y+3),label,fill='white');canvas.paste(color,(0,y+22,canvas.width,y+22+im.height));canvas.paste(im,(0,y+22),im)
 canvas.save(q/(key+'_alpha.png'))
for key in ['footer','row','row_player']:
 src=Image.open(b/(key+'_source.png')).convert('RGBA');fill=g.crop(boxes[key]['atlasBox']).resize(src.size);w,h=src.size;m=Image.new('L',src.size);d=ImageDraw.Draw(m)
 if key=='footer':
  d.ellipse((29,-8,212,174),fill=255);d.ellipse((738,-6,906,174),fill=255);d.ellipse((855,-10,926,66),fill=255)
  play=Image.open(art/'play.png').getchannel('A');m.paste(play,(211,-3))
  # Include isolated gem/curls around the control and original glow footprints.
  m=m.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(2))
 else:
  d.rectangle((3,3,w-4,h-4),fill=255);m=m.filter(ImageFilter.GaussianBlur(.45))
 im=Image.composite(fill,src,m);im.putalpha(255);m.save(b/(key+'_repair_mask.png'));save(key,im,src,m)
# Source control rings with repaired faces and the old quantity-badge footprint.
for key,box,center,radius in [('back_rim',(453,232,604,385),(77,79),75),('reward_rim',(689,234,841,388),(85,82),76)]:
 src=Image.open(b/(key+'_source.png')).convert('RGBA');cx,cy=center;r=radius
 fill=src.copy();guide=g.crop(box).resize((r*2,r*2));fill.paste(guide,(cx-r,cy-r));m=Image.new('L',src.size);d=ImageDraw.Draw(m);d.ellipse((cx-r+10,cy-r+10,cx+r-10,cy+r-10),fill=255)
 if key=='reward_rim':d.rectangle((107,0,180,63),fill=255)
 m=m.filter(ImageFilter.GaussianBlur(.6));im=Image.composite(fill,src,m)
 a=Image.new('L',src.size);d=ImageDraw.Draw(a);d.ellipse((cx-r,cy-r,cx+r,cy+r),fill=255)
 for x,y in [(cx,cy-r),(cx-r,cy),(cx+r,cy),(cx,cy+r)]:d.polygon([(x,y-4),(x+4,y),(x,y+4),(x-4,y)],fill=255)
 im.putalpha(a.filter(ImageFilter.GaussianBlur(.3)));m.save(b/(key+'_repair_mask.png'));save(key,im,src,m)
# Three original portraits; opaque portrait faces, only outer corners removed.
for i in range(3):
 src=Image.open(b/('portrait'+str(i)+'_source.png')).convert('RGBA');im=src.copy();a=Image.new('L',im.size);d=ImageDraw.Draw(a);d.ellipse((3,3,61,61),fill=255);im.putalpha(a.filter(ImageFilter.GaussianBlur(.25)));save('portrait'+str(i),im,src)
# Keep the original rank-metal silhouettes, with neutral live rank placeholders.
for i in range(3):
 key='rank'+str(i);src=Image.open(b/(key+'_source.png')).convert('RGBA');fill=g.crop(boxes[key]['atlasBox']).resize(src.size);m=Image.new('L',src.size);ImageDraw.Draw(m).rounded_rectangle((15,14,37,47),radius=4,fill=255);m=m.filter(ImageFilter.GaussianBlur(.6));im=Image.composite(fill,src,m)
 a=Image.new('L',im.size);ImageDraw.Draw(a).polygon([(26,1),(31,6),(39,10),(46,15),(49,24),(49,42),(42,51),(26,61),(11,52),(3,41),(3,24),(7,15),(16,9),(23,6)],fill=255);im.putalpha(a.filter(ImageFilter.GaussianBlur(.25)));m.save(b/(key+'_repair_mask.png'));save(key,im,src,m)
# Approved native Back arrow, separated from its old ring; source RGB remains unchanged.
src=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned/back_r2.png').convert('RGBA').crop((39,38,99,88));im=src.copy();a=Image.new('L',im.size);ImageDraw.Draw(a).polygon([(1,25),(29,1),(29,16),(57,16),(57,36),(29,36),(29,48)],fill=255);im.putalpha(a.filter(ImageFilter.GaussianBlur(.3)));save('back_arrow',im,src)
# Original closed chest in the reference's right footer circle.
src=Image.open(b/'OriginalReference.png').convert('RGBA').crop((771,1537,883,1644));src.save(b/'chest_source.png');im=src.copy();a=Image.new('L',im.size);ImageDraw.Draw(a).polygon([(14,3),(36,2),(53,8),(72,8),(91,9),(99,17),(104,33),(105,49),(100,51),(101,76),(95,80),(67,92),(65,97),(54,96),(48,91),(5,82),(4,72),(7,66),(7,51),(4,48),(3,33),(6,31),(7,19)],fill=255);im.putalpha(a.filter(ImageFilter.GaussianBlur(.3)));save('footer_chest',im,src)
(b/'ArtAudit.json').write_text(json.dumps(audit,indent=2));print(json.dumps(audit,indent=2))
