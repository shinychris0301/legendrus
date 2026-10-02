"""Repair original rail masks and diamond registration, without regenerating approved assets."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;qa=b.parent/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned')
src=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/R1/Sources/OriginalEventDetailReference.png').convert('RGBA')
# Remove old diamond footprints from the rail using an unobstructed rail span at identical Y.
railbox=(33,993,910,1030);rail=src.crop(railbox)
# One continuous donor span avoids hard colour joins at the former marker footprints.
bridge=src.crop((342,993,398,1030)).resize((838,37),Image.Resampling.LANCZOS)
joined=rail.copy();joined.paste(bridge,(19,0))
blend=Image.new('L',rail.size);bd=ImageDraw.Draw(blend);bd.rectangle((25,0,850,36),fill=255)
for n in range(6):
 bd.line((19+n,0,19+n,36),fill=int(255*(n+1)/7));bd.line((856-n,0,856-n,36),fill=int(255*(n+1)/7))
rail=Image.composite(joined,rail,blend)
# An unavailable event has no filled progress. Use the original unfilled navy channel,
# spanning the whole tapered opening instead of an aspect-fitted unrelated plaque.
channel=src.crop((846,1004,884,1019));arr=np.array(channel)
arr=np.pad(arr,((0,0),(0,rail.width-arr.shape[1]),(0,0)),mode='reflect')[:,:rail.width]
navy=Image.new('RGBA',rail.size);navy.paste(Image.fromarray(arr),(0,11))
m=Image.new('L',rail.size);d=ImageDraw.Draw(m)
d.polygon([(18,11),(860,11),(866,18),(860,25),(18,25),(13,18)],fill=255)
rail=Image.composite(navy,rail,m)
alpha=Image.new('L',rail.size);ImageDraw.Draw(alpha).polygon([(14,5),(865,5),(873,18),(865,31),(14,31),(4,18)],fill=255)
rail.putalpha(alpha.filter(ImageFilter.GaussianBlur(.25)));rail.save(art/'progress_track_r6.png')
# Each diamond is now extracted around its actual source centre, with complete outline.
for i,x in enumerate([131,291,462,632,805]):
 im=src.crop((x-24,985,x+24,1035));a=Image.new('L',im.size)
 radius=19 if i in (0,2,3) else 20
 ImageDraw.Draw(a).polygon([(24,25-radius),(24+radius,25),(24,25+radius),(24-radius,25)],fill=255)
 im.putalpha(a.filter(ImageFilter.GaussianBlur(.25)));im.save(art/('diamond'+str(i)+'_r6.png'))
# Broad corridors include complete scrollwork, unlike R5's thin rectangular strip mask.
box=(8,730,933,1504);frame=src.crop(box);gates=Image.new('L',src.size);d=ImageDraw.Draw(gates)
for r in [(14,737,928,754),(13,748,44,1492),(895,748,930,1492),(15,826,928,852),(15,1169,928,1190),(28,1212,430,1221),(465,1212,914,1221),(426,1190,466,1492),(14,1476,929,1501)]:d.rectangle(r,fill=255)
for r in [(11,735,78,776),(884,735,931,780),(11,808,76,876),(889,808,930,876),(11,1148,73,1225),(411,1172,487,1232),(887,1148,932,1232),(10,1458,74,1503),(409,1460,487,1503),(886,1458,932,1503)]:d.rectangle(r,fill=255)
# Neighbouring independent content is never part of the outer panel frame.
for r in [(645,752,899,831),(80,842,880,872),(35,985,906,1036),(32,1222,430,1478),(453,1233,914,1475),(75,1187,410,1213),(495,1187,893,1213)]:d.rectangle(r,fill=0)
a=np.array(gates.crop(box));rgb=np.array(frame)[:,:,:3].astype(float)
metal=(rgb[:,:,0]>35)&(rgb[:,:,1]>15)&(rgb[:,:,0]>rgb[:,:,2]*1.22)&(rgb[:,:,1]>rgb[:,:,2]*1.10)
seed=(metal&(a>0)).astype('uint8')*255
# Retain the original dark bevel immediately around the metal; no opaque corner blocks.
alpha=cv2.dilate(seed,np.ones((3,3),np.uint8));alpha=np.minimum(alpha,a)
# Continuous source rail cores retain their original dark bevels, not just bright gold pixels.
core=Image.new('L',src.size);dc=ImageDraw.Draw(core)
for r in [(50,740,889,748),(18,775,28,811),(18,875,28,1147),(915,781,923,807),(915,876,923,1147),(60,834,887,841),(62,1176,889,1184),(67,1215,410,1220),(480,1215,891,1220),(20,1233,28,1457),(439,1233,450,1459),(916,1233,924,1457),(66,1486,410,1494),(485,1486,888,1494)]:dc.rectangle(r,fill=255)
alpha=np.maximum(alpha,np.array(core.crop(box)))
frame.putalpha(Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(.25)));frame.save(art/'panel_rails_r6.png')
for key in ['objectives','progress','leaders','premium']:
 face=Image.open(art/(key+'_face_r5.png')).convert('RGBA');mask=Image.new('L',face.size)
 ImageDraw.Draw(mask).rounded_rectangle((0,0,face.width-1,face.height-1),radius=17,fill=255)
 face.putalpha(mask);face.save(art/(key+'_face_r6.png'))
# Alpha diagnostics are asset inspections, not implementation previews.
for k in ['progress_track_r6','diamond0_r6','diamond1_r6','diamond2_r6','diamond3_r6','diamond4_r6','panel_rails_r6']:
 im=Image.open(art/(k+'.png'));out=Image.new('RGB',(im.width*3,im.height+24));dr=ImageDraw.Draw(out)
 for i,c in enumerate(['black','white','#ed00d2']):
  x=i*im.width;dr.rectangle((x,24,x+im.width,out.height),fill=c);out.paste(im,(x,24),im)
 out.save(qa/(k+'_alpha.png'))
(b/'Geometry.json').write_text(json.dumps({'railBox':railbox,'markerCentersX':[131,291,462,632,805],'markerBoxY':[985,1035],'panelRailsBox':box,'progressState':'unavailable; original empty navy channel, no illustrative fill'},indent=2))
