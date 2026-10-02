from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;q=b.parent/'QA';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned')
g=Image.open(b/'Scenario/booster_medallion_trial01.png').convert('RGBA');boxes=json.loads((b/'SourceBoxes.json').read_text());audit={}
def matte(im,panel=False):
 a=np.array(im)[:,:,:3].astype(float);c=a.max(2)-a.min(2);m=(c>28)
 if panel:m|=a.max(2)<30
 mask=m.astype('uint8')*255
 if panel:
  n,l,st,_=cv2.connectedComponentsWithStats(mask);mask=(l==(1+np.argmax(st[1:,4]))).astype('uint8')*255
  cs,_=cv2.findContours(mask,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);cv2.drawContours(mask,cs,-1,255,-1)
 return mask
def save(key,im,src=None):
 im.save(out/(key+'_r3.png'))
 if src is not None:
  a=np.array(im);s=np.array(src);m=a[:,:,3]>0;audit[key]={'visiblePixels':int(m.sum()),'RGBMismatches':int(np.any(a[:,:,:3][m]!=s[:,:,:3][m],axis=1).sum())}
 # Alpha inspection artifact; not an implementation preview.
 canvas=Image.new('RGB',(max(im.width,360),3*(im.height+22)),(30,35,40));d=ImageDraw.Draw(canvas)
 for i,(color,label) in enumerate([('black','BLACK'),('white','WHITE'),((235,0,210),'MAGENTA')]):
  y=i*(im.height+22);d.text((5,y+3),label,fill='white');canvas.paste(color,(0,y+22,canvas.width,y+22+im.height));canvas.paste(im,(0,y+22),im)
 canvas.save(q/(key+'_alpha.png'))
# Preserve original panel ornament RGB; repaired material is restricted to occupied faces.
for key in ['countdown','balance','premium_inset']:
 src=Image.open(b/(key+'_source.png')).convert('RGBA');w,h=src.size
 gb=boxes[key]['atlasBox'] if key!='balance' else [581,420,825,504]
 fill=g.crop(gb).resize(src.size);mask=Image.new('L',src.size);d=ImageDraw.Draw(mask)
 if key=='countdown':
  d.rounded_rectangle((24,8,w-23,h-8),radius=18,fill=255)
  d.rectangle((0,0,31,14),fill=255) # Feather contamination beyond upper-left rail.
  d.rectangle((312,0,368,14),fill=255) # Existing talons on the upper rail.
 elif key=='balance':d.rectangle((8,7,w-9,h-8),fill=255)
 else:d.rounded_rectangle((9,8,w-10,h-9),radius=11,fill=255)
 mask=mask.filter(ImageFilter.GaussianBlur(.5))
 im=Image.composite(fill,src,mask)
 if key=='countdown':
  # Scenario kept old claws as vertical gold marks: replace only that short rail interval.
  donor=im.crop((250,0,278,14)).resize((56,14));im.paste(donor,(312,0))
 alpha=matte(fill,True)
 if key=="countdown":alpha[:4,50:455]=0
 if key!='countdown':
  alpha[:]=0;am=Image.new('L',src.size);ImageDraw.Draw(am).rounded_rectangle((1,1,w-2,h-2),radius=8,fill=255);alpha=np.array(am)
 im.putalpha(Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(.3)));mask.save(b/(key+'_repair_mask.png'));save(key,im,src)
# Generated colours never replace visible bird colours. Use Scenario contour only as a segmentation guide.
src=Image.open(b/'bird_source.png').convert('RGBA');rgb=np.array(src)[:,:,:3];guide=g.crop(boxes['bird']['atlasBox']);prior=matte(guide)
# Original does not contain feathers behind the countdown; exclude that UI/scenery area.
plaque=np.array(Image.open(out/"countdown_r3.png").getchannel("A"))
exclusion=np.zeros(prior.shape,np.uint8);exclusion[279:342,110:600]=plaque
prior[exclusion>0]=0
m=np.full(prior.shape,cv2.GC_BGD,np.uint8)
near=cv2.dilate(prior,np.ones((5,5),np.uint8))>0
m[near]=cv2.GC_PR_BGD;m[prior>0]=cv2.GC_PR_FGD
m[cv2.erode(prior,np.ones((3,3),np.uint8))>0]=cv2.GC_FGD
cv2.grabCut(rgb,m,None,np.zeros((1,65)),np.zeros((1,65)),5,cv2.GC_INIT_WITH_MASK)
a=np.isin(m,[1,3]).astype('uint8')*255
# Retain shadowed/blue interiors inside source-traced feather ribbons, not only gold-coloured pixels.
for polygon in [ [(37,309),(42,283),(59,250),(80,220),(105,196),(94,245),(81,286),(63,328),(40,337)], [(89,250),(105,267),(126,292),(112,330),(92,337),(84,313)], [(79,145),(102,125),(130,103),(160,85),(145,116),(119,143),(91,170)] ]:
 mm=Image.new('L',src.size);ImageDraw.Draw(mm).polygon(polygon,fill=255);a=np.maximum(a,np.array(mm))
# Remove tiny isolated mask specks, retain connected feather tips and major source spark cores.
n,l,st,_=cv2.connectedComponentsWithStats(a);valid=np.flatnonzero(st[:,4]>=7);valid=valid[valid!=0];a=np.isin(l,valid).astype('uint8')*255
# Exclude original countdown UI under the silhouette, with the new plaque overlapping at y681.
a[exclusion>0]=0
am=Image.fromarray(a);dd=ImageDraw.Draw(am)
# Remove fragments of the overlying title rail from the top of the source crop.
dd.polygon([(275,0),(400,0),(400,88),(362,88),(344,66),(326,53),(306,31),(292,12),(282,7)],fill=0)
dd.polygon([(0,0),(178,0),(178,10),(151,24),(155,29),(137,34),(117,31),(103,22),(90,8),(86,0)],fill=0)
a=np.array(am);a[-4:]=0
bird=src.copy();bird.putalpha(am.filter(ImageFilter.GaussianBlur(.25)));save('firebird',bird,src)
# Original starbursts/feather light, kept near the clean silhouette only.
r=rgb.astype(float);warm=(r[:,:,0]>150)&(r[:,:,0]>r[:,:,2]*1.35)&(r[:,:,1]>r[:,:,2]*1.15)
near=cv2.dilate(a,np.ones((11,11),np.uint8))>0
sparks=np.where(warm&near,np.clip((r[:,:,0]-r[:,:,2])*1.4,0,175),0).astype('uint8')
# Restrict off-feather spark highlights to recognizable original starbursts.
star=Image.new('L',src.size);sd=ImageDraw.Draw(star)
for x,y,rad in [(145,696-402,26),(332,666-402,30),(282,542-402,12)]:sd.ellipse((x-108-rad,y-rad,x-108+rad,y+rad),fill=255)
bright=warm&(np.array(star)>0)&(r[:,:,1]>100)
sparks=np.maximum(sparks,np.where(bright,np.clip((r[:,:,0]-r[:,:,2])*1.3,0,215),0).astype('uint8'))
sparks[exclusion>0]=0;sparks[-4:]=0
sparks[:35,:165]=np.minimum(sparks[:35,:165],a[:35,:165])
sparks[:88,275:400]=np.minimum(sparks[:88,275:400],a[:88,275:400])
light=src.copy();light.putalpha(Image.fromarray(sparks));save('firebird_sparks',light,src)
halo=Image.new('RGBA',src.size,(255,156,32,0));halo.putalpha(Image.fromarray(sparks).filter(ImageFilter.GaussianBlur(7)).point(lambda x:int(x*.32)));save('firebird_halo',halo)
(b/'ArtAudit.json').write_text(json.dumps(audit,indent=2));print(json.dumps(audit,indent=2))
