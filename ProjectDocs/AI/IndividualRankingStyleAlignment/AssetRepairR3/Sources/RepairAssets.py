"""R3 bounded source repairs; no source lettering or scenery used as runtime controls."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;r2=b.parents[1]/'ReferenceLayoutR2'/'Sources';ref=Image.open(r2/'OriginalReference.png').convert('RGB');records=[]
def shape(size,kind,box,radius=0):
 s=4;m=Image.new('L',(size[0]*s,size[1]*s));d=ImageDraw.Draw(m);q=tuple(v*s for v in box)
 if kind=='ellipse':d.ellipse(q,fill=255)
 elif kind=='rounded':d.rounded_rectangle(q,radius=radius*s,fill=255)
 else:d.polygon([(x*s,y*s)for x,y in box],fill=255)
 return m.resize(size,Image.Resampling.LANCZOS)
def surface(w,h,donor):
 a=np.array(ref.crop(donor));a=np.concatenate([a,a[:,::-1]],1);a=np.concatenate([a,a[::-1]],0)
 return np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w].copy()
def write(name,box,rgb,alpha,mask,reason,scale):
 original=np.array(ref.crop(box));changed=np.any(original!=rgb,2);assert not np.any(changed&(mask==0)),name
 Image.fromarray(rgb).save(b/'Final'/(name+'_source_rgb.png'));Image.fromarray(mask).save(b/'Masks'/(name+'.png'))
 out=Image.fromarray(rgb).convert('RGBA');out.putalpha(alpha);out=out.resize((rgb.shape[1]*scale,rgb.shape[0]*scale),Image.Resampling.LANCZOS);out.save(b/'Final'/(name+'.png'))
 records.append({'name':name,'sourceBox':box,'repairMask':'Masks/'+name+'.png','reason':reason,'originalRGBOutsideMask':'verified identical before interpolation','repairedPixels':int(np.count_nonzero(mask)),'scale':scale,'sha256':hashlib.sha256((b/'Final'/(name+'.png')).read_bytes()).hexdigest()})
# Retain R2 Scenario-derived title outline, replace the complete label footprint including dark shadows.
name='title';box=(113,107,856,276);original=np.array(ref.crop(box));h,w=original.shape[:2];rgb=original.copy()
mask=np.array(shape((w,h),'rounded',(47,62,700,136),6).filter(ImageFilter.GaussianBlur(1)))
mask[:65,340:380]=0
face=surface(w,h,(133,269,231,309));blend=mask[:,:,None]/255.;rgb=np.rint(rgb*(1-blend)+face*blend).astype(np.uint8)
alpha=Image.open(b/'Before/title.png').getchannel('A').resize((w,h),Image.Resampling.LANCZOS)
write(name,box,rgb,alpha,mask,'Complete old glyph/shadow footprint replaced with clean source lacquer donor; existing crest and frame preserved.',2)
# Retain connected border, its dark shadows and inset corner ornaments instead of a gold-pixel threshold.
for name,box in [('reward_panel',(16,379,925,578)),('table_frame',(17,575,924,1546)),('reward_frame',(65,419,345,551))]:
 original=np.array(ref.crop(box));h,w=original.shape[:2];rgb=original.copy()
 if name=='reward_frame':inner=(9,10,w-10,h-9);outer=(1,3,w-2,h-1);rad=5
 else:inner=(15,12,w-16,h-12);outer=(5,2,w-5,h-2);rad=20
 mask=np.array(shape((w,h),'rounded',inner,rad));tile=surface(w,h,(420,904,514,935));rgb=np.rint(rgb*(1-mask[:,:,None]/255.)+tile*(mask[:,:,None]/255.)).astype(np.uint8)
 # Bounded corner cleanup: remove blue scenery reflections outside the actual gold ornament.
 corner=np.zeros((h,w),np.uint8)
 for y in [0,h-38]:
  for x in [0,w-38]:corner[y:y+38,x:x+38]=1
 p=rgb.astype(float);fringe=((p[:,:,2]>p[:,:,0]*1.15)&(p[:,:,2]>35)&(corner>0)).astype(np.uint8)*255
 fringe=cv2.dilate(fringe,np.ones((3,3),np.uint8));rgb=cv2.inpaint(rgb,fringe,3,cv2.INPAINT_TELEA);mask=np.maximum(mask,fringe)
 alpha=shape((w,h),'rounded',outer,rad)
 if name=='table_frame':
  # Back overlaps the original lower-left table corner; use its unoccluded right counterpart.
  hh,ww=26,28;rgb[-hh:,:ww]=rgb[-hh:,-ww:][:,::-1];mask[-hh:,:ww]=255
 write(name,box,rgb,alpha,mask,'Source border contour and opaque shaded body retained. Blank inner face uses clean source donor; blue corner fringe repaired locally; table corner hidden by Back mirrored from right.',3 if name=='reward_frame' else 2)
# Only restore missing bottom caps; face/eyes/hair/upper frame remain exactly original.
generated=Image.open(b/'ScenarioPortraitCaps/booster_medallion_trial01.png').convert('RGB');assert generated.size==(1536,512)
repair_map={r['name']:r['box']for r in json.loads((b/'PortraitRepairMap.json').read_text())}
for name,box,cx,cy,rx,ry,stop in [
 ('portrait_solovey',(153,1329,248,1410),48,40,40,36,58),
 ('portrait_vodyanoi',(154,1388,248,1463),47,38,40,35,58),
 ('portrait_zmei',(153,1179,248,1258),49,38,40,34,60)]:
 original=np.array(ref.crop(box));h,w=original.shape[:2];rgb=original.copy();mask=np.zeros((h,w),np.uint8)
 alpha=shape((w,h),'ellipse',(cx-rx,cy-ry,cx+rx,cy+ry));a=np.array(alpha)
 # Keep the generated full portraits as evidence only; composite just the missing lower caps.
 crop=repair_map[name].copy();crop[3]=crop[1]+h*3
 repair=np.array(generated.crop(crop).resize((w,h),Image.Resampling.LANCZOS))
 yy,xx=np.mgrid[:h,:w];blend=np.clip((yy-(stop-4))/4.,0,1)
 # Recover the generated medallion silhouette from its neutral charcoal surround; fill interior holes.
 chroma=repair.max(2).astype(float)-repair.min(2);fg=((chroma>22)&(repair.max(2)>55)).astype(np.uint8)
 contours,_=cv2.findContours(fg,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);hull=cv2.convexHull(max(contours,key=cv2.contourArea));ga=np.zeros((h,w),np.uint8);cv2.fillConvexPoly(ga,hull,255)
 final_alpha=np.rint(a*(1-blend)+ga*blend).astype(np.uint8)
 cap=(blend>0)&(final_alpha>0);mask[cap]=255
 amount=(blend*cap)[:,:,None];rgb=np.rint(original*(1-amount)+repair*amount).astype(np.uint8)
 write(name,box,rgb,Image.fromarray(final_alpha),mask,'Only lower cap and curved rim from Scenario repair; original upper portrait RGB retained. Generated full portraits are not runtime resources.',3)

(b/'Repairs.json').write_text(json.dumps({'source':'../../ReferenceLayoutR2/Sources/OriginalReference.png','sourceSHA256':hashlib.sha256((r2/'OriginalReference.png').read_bytes()).hexdigest(),'newScenarioRequests':1,'reuse':'Existing R2 Scenario title outline and original source frames; one new Scenario repair of missing portrait caps. Generated upper faces discarded; existing source faces retained.','assets':records},indent=2)+'\n')
# Evidence only: isolated assets over three backgrounds, never a runtime collage.
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(255,0,180))]:
 im=Image.new('RGB',(1100,700),color);d=ImageDraw.Draw(im)
 for i,r in enumerate(records):
  art=Image.open(b/'Final'/(r['name']+'.png'));art.thumbnail((510,155),Image.Resampling.LANCZOS);x=i%2*550+(550-art.width)//2;y=i//2*175+18;im.paste(art,(x,y),art);d.text((i%2*550+10,i//2*175+2),r['name'],fill=(128,128,128))
 im.save(b.parent/'QA'/('AssetsOn'+label+'.jpg'),quality=95)
print('Prepared',len(records),'bounded repairs; source RGB outside masks unchanged.')
