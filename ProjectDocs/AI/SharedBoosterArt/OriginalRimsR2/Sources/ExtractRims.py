from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,cv2,json,uuid,hashlib
b=Path(__file__).resolve().parent;r1=b.parents[1]/'R1/Sources';src=Image.open(r1/'OriginalReference.png').convert('RGBA');boxes=json.loads((r1/'Boxes.json').read_text());atlas=Image.open(r1/'Scenario/all/booster_medallion_trial01.png').convert('RGBA').resize((900,480),Image.Resampling.LANCZOS);out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters');names=['mace','firebird','shield','potion','scroll','lightning','bomb','swap'];report={}
def shape(size,kind):
 m=Image.new('L',(size[0]*4,size[1]*4));d=ImageDraw.Draw(m)
 if kind=='interior':
  d.ellipse((18*4,22*4,155*4,157*4),fill=255)
  for pts in [[(64,0),(111,0),(99,29),(87,36),(72,26)],[(61,179),(113,179),(103,152),(88,145),(73,153)],[(0,72),(24,84),(24,94),(0,105)],[(175,70),(151,83),(151,95),(175,107)]]:d.polygon([(x*4,y*4) for x,y in pts],fill=0)
 elif kind=='outer':
  d.ellipse((5*4,6*4,170*4,173*4),fill=255)
  for x,y in [(87,14),(87,161),(13,89),(162,89)]:d.polygon([(x*4,(y-18)*4),((x+18)*4,y*4),(x*4,(y+18)*4),((x-18)*4,y*4)],fill=255)
 return m.resize(size,Image.Resampling.LANCZOS)
for i,name in enumerate(names):
 orig=src.crop(boxes[i]['source']);rgb=np.array(orig)[:,:,:3];w,h=orig.size
 badge=Image.new('L',(w,h));d=ImageDraw.Draw(badge)
 # Only pixels actually occluded by the baked badges are replaceable.
 centers=[(40,156,23),(136,156,24)]
 for x,y,r in centers:d.ellipse((x-r,y-r,x+r,y+r),fill=255)
 bm=np.asarray(badge)
 # Construct the source band as its own annular layer. Never mirror the icon face.
 ring_mask=ImageChops.subtract(shape((w,h),'outer'),shape((w,h),'interior'))
 # Exclude the metal from the donor pool when repairing the hidden navy face.
 fill_mask=cv2.dilate(np.maximum(bm,np.asarray(ring_mask)),np.ones((3,3),np.uint8))
 clean=cv2.inpaint(rgb,fill_mask,3,cv2.INPAINT_TELEA)
 alpha=atlas.crop(boxes[i]['atlas']).getchannel('A');ImageDraw.Draw(alpha).ellipse((21,22,154,151),fill=255)
 # Mirror only annular pixels from the same source rim, at the original centreline.
 donor=rgb[::-1].copy();repaired=rgb.copy();band=np.asarray(ring_mask)>0;repair=(bm>0)&band
 repaired[repair]=donor[repair]
 # Gold-select the donor inside each patch to prevent interior blue/subject pixels
 # becoming additional metal ornament. The inner face is repaired separately.
 rr,gg,bb=donor.astype(float).transpose(2,0,1);gold=(rr>45)&(rr>bb*1.18)&(gg>bb*1.04)
 gold=cv2.dilate(np.uint8(gold)*255,np.ones((3,3),np.uint8))>0
 ra=np.asarray(ImageChops.darker(alpha,ring_mask)).copy();ra[bm>0]=np.asarray(ring_mask)[bm>0];ra[(bm>0)&~gold]=0
 r,g,blue=repaired.astype(float).transpose(2,0,1);metal=np.uint8((r>38)&(r>blue*1.12)&(g>blue*1.02))*255
 metal=cv2.dilate(metal,np.ones((3,3),np.uint8));jewels=Image.fromarray(metal)
 jd=ImageDraw.Draw(jewels)
 for cy in [15,164]:jd.polygon([(87,cy-13),(98,cy),(87,cy+14),(76,cy)],fill=255)
 ra=np.minimum(ra,np.asarray(jewels))
 ring=Image.fromarray(repaired,'RGB').convert('RGBA');ring.putalpha(Image.fromarray(ra))
 # Preserve every visible original pixel outside the badge footprints. Hidden faces
 # use local source inpainting; only the isolated source band is composited above it.
 face=orig.copy();face.paste(Image.fromarray(clean),(0,0),badge);fa=np.asarray(alpha).copy();fa[bm>0]=np.asarray(shape((w,h),'outer'))[bm>0];face.putalpha(Image.fromarray(fa))
 # Remove only the old badges outside the original outline, not the existing carving.
 face_alpha=np.array(face.getchannel('A'));face_alpha[bm>0]=np.asarray(shape((w,h),'outer'))[bm>0]
 face_alpha=np.minimum(face_alpha,np.asarray(shape((w,h),'outer')));face.putalpha(Image.fromarray(face_alpha));layer=Image.new('RGBA',(w,h));layer.paste(ring,(0,0));patch=Image.composite(layer,Image.new('RGBA',(w,h)),badge)
 face=Image.alpha_composite(face,patch)
 for suffix,im in [('_rim',ring),('_framed',face)]:
  canvas=Image.new('RGBA',(192,192));canvas.paste(im,(8,6));p=out/(name+suffix+'.png');canvas.save(p)
  meta=p.with_suffix('.png.meta')
  if not meta.exists():
   t=(out/(name+'_framed.png.meta')).read_text();t=t.replace(next(x for x in t.splitlines() if x.startswith('guid:')),'guid: '+uuid.uuid4().hex);meta.write_text(t)
 if i==0:badge.save(b.parent/'QA/RepairFootprint.png')
 arr=np.asarray(face);outside=(bm==0)&(arr[:,:,3]>0);report[name]={'visibleOutsideRepair':int(outside.sum()),'outsideRGBMismatch':int(np.any(arr[:,:,:3]!=rgb,axis=2)[outside].sum()),'repairPixels':int((bm>0).sum())}
(b/'Provenance.json').write_text(json.dumps({'method':'Separate source annular extraction; only badge footprints repaired; no whole-icon mirrored patches. Local hidden-face inpainting, upper-rim donor arcs. Original R1 Scenario alpha reused.','newScenarioJobs':0,'newCU':0,'assets':report},indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 im=Image.new('RGB',(8*192,384),color)
 for i,n in enumerate(names):
  for y,suf in [(0,'_rim'),(192,'_framed')]:
   a=Image.open(out/(n+suf+'.png'));im.paste(a,(i*192,y),a)
 im.save(b.parent/'QA'/('Alpha'+label+'.png'))
