"""Remove source-border residue and match the complete R3 ring to darker source gold."""
from pathlib import Path
from PIL import Image,ImageDraw
import cv2,numpy as np,json,hashlib,shutil
b=Path(__file__).resolve().parent;qa=b.parent/'QA';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters')
r1=b.parents[1]/'R1/Sources';source=Image.open(r1/'OriginalReference.png').convert('RGB');boxes=json.loads((r1/'Boxes.json').read_text())
names=['mace','firebird','shield','potion','scroll','lightning','bomb','swap'];stage=b/'Composed';stage.mkdir(exist_ok=True)
generated=Image.open(b.parents[1]/'ContinuousRimR3/Sources/Scenario/rim/booster_medallion_trial01.png').convert('RGBA')
isolated=Image.open(b.parents[1]/'ContinuousRimR3/Sources/Scenario/alpha/booster_medallion_trial01.png').convert('RGBA')
generated.putalpha(isolated.getchannel('A'))
# Preserve the generated band as a complete piece: no patched arc donors.
ring=generated.resize((192,192),Image.Resampling.LANCZOS)
# Tone-only correction of the single continuous metal layer; alpha/geometry unchanged.
# Sampled unobscured original upper arcs: RGB median 103/66/23, versus 141/74/17.
rim_pixels=np.array(ring);metal=rim_pixels[:,:,:3].astype(float)
weight=np.clip((metal[:,:,0]-metal[:,:,2])/25.,0,1)[:,:,None]
antique=metal.copy();antique[:,:,0]*=.76;antique[:,:,1]*=.88
antique[:,:,2]=metal[:,:,2]*.85+metal[:,:,0]*.07
rim_pixels[:,:,:3]=np.uint8(np.clip(np.rint(metal*(1-weight)+antique*weight),0,255))
ring=Image.fromarray(rim_pixels)
ra=np.array(ring.getchannel('A'));binary=np.uint8(ra<128)
_,labels=cv2.connectedComponents(binary);hole=np.uint8(labels==labels[96,96])*255
assert hole[0,0]==0 and hole[96,96]==255,'The isolated ring must be closed.'
facealpha=cv2.dilate(hole,np.ones((5,5),np.uint8))
records={}
for i,n in enumerate(names):
 original=source.crop(boxes[i]['source']);rgb=np.asarray(original).copy()
 # Exclude ALL old metal and both original badge footprints from the face.
 keep=Image.new('L',original.size);d=ImageDraw.Draw(keep);d.ellipse((27,29,147,151),fill=255)
 d.polygon([(175,67),(147,77),(134,89),(149,102),(175,112)],fill=0)
 d.polygon([(0,67),(27,77),(40,89),(25,102),(0,112)],fill=0)
 d.polygon([(64,0),(111,0),(99,29),(87,36),(72,26)],fill=0)
 d.polygon([(61,179),(113,179),(103,152),(88,145),(73,153)],fill=0)
 for x,y,r in [(40,156,25),(136,156,26)]:d.ellipse((x-r,y-r,x+r,y+r),fill=0)
 mask=255-np.asarray(keep)
 # Only the empty backing is extended underneath the new band. Subject RGB stays original.
 subject=Image.open(b/'Before'/(n+'.png')).convert('RGBA');sa=np.asarray(subject)
 match=cv2.matchTemplate(rgb,sa[:,:,:3],cv2.TM_SQDIFF,mask=np.uint8(sa[:,:,3]>128)*255)
 _,_,loc,_=cv2.minMaxLoc(match);sx,sy=loc
 subjectmask=np.zeros(mask.shape,np.uint8);subjectmask[sy:sy+subject.height,sx:sx+subject.width]=sa[:,:,3]
 # Do not let gold from the subject bleed into the reconstructed navy backing.
 donors=cv2.dilate(np.maximum(mask,subjectmask),np.ones((3,3),np.uint8))
 clean=cv2.inpaint(rgb,donors,3,cv2.INPAINT_TELEA);clean[mask==0]=rgb[mask==0]
 face=Image.new('RGBA',(192,192),(4,13,22,255));face.paste(Image.fromarray(clean),(8,6));face.putalpha(Image.fromarray(facealpha))
 face.alpha_composite(subject,(8+sx,6+sy))
 framed=Image.alpha_composite(face,ring)
 for suffix,im in [('_rim',ring),('_framed',framed)]:im.save(stage/(n+suffix+'.png'))
 a=np.asarray(framed)[6:185,8:183];visible=(np.asarray(keep)==255)&(ra[6:185,8:183]==0)
 records[n]={'preservedSourcePixels':int(visible.sum()),'sourceRGBMismatch':int(np.any(a[:,:,:3]!=rgb,axis=2)[visible].sum()),'toolOnlyUnchanged':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()==hashlib.sha256((b/'Before'/(n+'.png')).read_bytes()).hexdigest()}
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 im=Image.new('RGB',(8*192,384),color)
 for i,n in enumerate(names):
  for y,suf in [(0,'_rim'),(192,'_framed')]:
   a=Image.open(stage/(n+suf+'.png'));im.paste(a,(i*192,y),a)
 im.save(qa/('Alpha'+label+'.png'))
comparison=Image.new('RGB',(3*384,384),'#0a1726')
for x,p in enumerate([b/'RimReference.png',b/'Before/mace_framed.png',stage/'mace_framed.png']):
 im=Image.open(p).convert('RGBA');im.thumbnail((370,370));im=Image.open(p).convert('RGBA').resize((350,350),Image.Resampling.LANCZOS);comparison.paste(im,(x*384+17,17),im)
comparison.save(qa/'OriginalBeforeAfter.png')
(b/'Provenance.json').write_text(json.dumps({'method':'Tighter central backing mask and explicit old side-ornament exclusions remove residual source metal. Source subjects overlaid at original positions. Whole R3 metal layer darkened to original antique-gold palette; cool sapphires and alpha preserved.','toneTransform':{'redGain':.76,'greenGain':.88,'blueGain':.85,'blueFromRed':.07,'warmWeight':'clip((R-B)/25,0,1)','originalGoldMedianRGB':[103,66,23],'R3GoldMedianRGB':[141,74,17]},'newGenerationJobs':0,'newAlphaJobs':0,'newScenarioCU':0,'reused':'R3 full rim generation and alpha','assets':records},indent=2))
print(json.dumps(records,indent=2))
