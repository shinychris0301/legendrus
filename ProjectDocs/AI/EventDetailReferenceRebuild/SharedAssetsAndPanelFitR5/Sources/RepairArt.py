"""Preserve original chest RGB; reuse accepted Scenario lacquer and single original panel rails."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import cv2,numpy as np,json
b=Path(__file__).resolve().parent;qa=b.parent/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned')
old=Path('ProjectDocs/AI/EventDetailReferenceRebuild/OriginalReferenceR2/Sources')
src=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/R1/Sources/OriginalEventDetailReference.png').convert('RGBA')
boxes=json.loads((old/'SourceBoxes.json').read_text())
# Silhouettes traced at the 300x235 inspection scale, including lids, handles and feet.
paths=[[(22,198),(28,114),(23,112),(23,84),(33,82),(40,56),(57,34),(76,24),(91,22),(118,24),(129,19),(149,19),(163,24),(185,29),(209,27),(235,24),(252,29),(267,44),(279,67),(284,82),(289,83),(290,110),(283,119),(285,196),(212,224),(176,221),(173,211),(56,199),(52,202)],
[(17,198),(23,113),(17,108),(16,82),(29,79),(36,57),(51,35),(74,22),(91,20),(114,23),(130,18),(151,20),(160,26),(192,28),(213,23),(225,18),(239,23),(250,20),(264,30),(277,49),(285,78),(291,85),(291,111),(282,116),(285,197),(213,224),(185,218),(182,211),(49,199)],
[(23,204),(30,125),(24,122),(23,95),(34,91),(39,69),(51,49),(65,37),(78,34),(94,38),(119,42),(137,36),(151,33),(170,40),(194,40),(213,35),(230,29),(244,37),(257,36),(271,49),(282,71),(286,95),(291,97),(290,123),(284,125),(286,202),(213,231),(178,223),(173,216),(59,206)],
[(18,203),(25,118),(18,111),(18,86),(29,83),(34,61),(49,38),(70,26),(86,26),(101,31),(126,34),(144,29),(166,31),(179,36),(199,34),(215,28),(228,20),(243,23),(246,29),(260,32),(275,49),(281,70),(284,87),(289,91),(288,115),(280,123),(284,205),(207,231),(177,225),(175,216),(48,205)],
[(24,209),(28,121),(23,117),(22,96),(32,94),(24,76),(30,74),(36,91),(45,70),(66,55),(91,49),(96,41),(97,29),(116,23),(137,23),(151,16),(165,19),(180,24),(193,26),(206,33),(204,47),(191,51),(196,55),(216,50),(230,42),(239,44),(247,52),(265,58),(278,78),(283,94),(289,98),(289,124),(282,129),(286,207),(209,233),(175,226),(172,220),(55,211)],
[(27,200),(31,116),(27,110),(27,81),(41,78),(49,53),(65,35),(82,26),(105,30),(121,24),(144,13),(163,23),(181,27),(195,36),(212,32),(236,33),(251,44),(263,45),(278,63),(286,79),(292,84),(292,109),(285,115),(288,201),(209,232),(178,223),(174,216),(58,204)]]
audit={}
for k,points in zip(['chest0','chest1','chest2','chest3','chest4','premium_chest'],paths):
 im=Image.open(old/(k+'_source.png')).convert('RGBA');w,h=im.size
 mask=Image.new('L',im.size);ImageDraw.Draw(mask).polygon([(x*w/300,y*h/235) for x,y in points],fill=255)
 # Only a narrow edge is classified; the complete dark interior remains foreground.
 a=np.array(mask);m=np.full((h,w),cv2.GC_BGD,np.uint8);m[cv2.dilate(a,np.ones((3,3),np.uint8))>0]=cv2.GC_PR_BGD;m[a>0]=cv2.GC_PR_FGD;m[cv2.erode(a,np.ones((3,3),np.uint8))>0]=cv2.GC_FGD
 cv2.grabCut(np.array(im)[:,:,:3].copy(),m,None,np.zeros((1,65)),np.zeros((1,65)),3,cv2.GC_INIT_WITH_MASK)
 alpha=np.isin(m,[1,3]).astype('uint8')*255
 cs,_=cv2.findContours(alpha,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);cv2.drawContours(alpha,cs,-1,255,-1)
 im.putalpha(Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(.25)))
 pad=Image.new('RGBA',(w+16,h+16));pad.paste(im,(8,8));pad.save(art/(k+'_r5.png'))
 audit[k]={'sourceSize':[w,h],'transparentPadding':8,'sourceRGBPreserved':True}
# Separate opaque faces meet without overlapping. All shared borders are drawn only once.
faceBoxes={'objectives':(17,739,925,837),'progress':(17,837,925,1180),'leaders':(17,1180,447,1498),'premium':(447,1180,925,1498)}
for k,box in faceBoxes.items():
 # Tile accepted blank material at its native pixel scale; exclude every old frame/header edge.
 donor=Image.open(art/('premium.png' if k=='premium' else 'progress.png')).convert('RGBA')
 tile=np.array(donor.crop((70,75,400,260) if k=='premium' else (170,70,600,250)))
 width,height=box[2]-box[0],box[3]-box[1]
 tiled=np.pad(tile,((0,max(0,height-tile.shape[0])),(0,max(0,width-tile.shape[1])),(0,0)),mode='reflect')[:height,:width].copy()
 if k in ('leaders','premium'):tiled[:35,:,:3]=(tiled[:35,:,:3].astype(float)*.45).astype('uint8')
 face=Image.fromarray(tiled);face.putalpha(255);face.save(art/(k+'_face_r5.png'))

frame=src.crop((17,739,925,1498));m=Image.new('L',src.size);d=ImageDraw.Draw(m)
for r in [(17,739,925,748),(17,748,29,1490),(911,748,925,1490),(17,833,925,841),(17,1174,925,1185),(28,1212,433,1220),(457,1212,910,1220),(435,1180,457,1492),(17,1482,925,1497)]:d.rectangle(r,fill=255)
for x,y in [(17,739),(891,739),(17,814),(891,814),(17,837),(891,837),(17,1152),(891,1152),(17,1180),(412,1180),(447,1180),(891,1180),(17,1464),(412,1464),(447,1464),(891,1464)]:d.rectangle((x,y,x+33,y+33),fill=255)
# Exterior corner silhouettes, no rectangular scenery remnants.
m=m.crop((17,739,925,1498));a=np.array(m);rgb=np.array(frame)[:,:,:3].astype(float)
outer=np.zeros(a.shape,bool);outer[:20,:10]=True;outer[:20,-10:]=True;outer[-20:,:10]=True;outer[-20:,-10:]=True
warm=(rgb[:,:,0]>rgb[:,:,2]*1.5)&(rgb[:,:,0]>50)
a[outer&~warm]=0;frame.putalpha(Image.fromarray(a));frame.save(art/'panel_rails_r5.png')
for k in ['chest0','chest1','chest2','chest3','chest4','premium_chest','panel_rails']:
 im=Image.open(art/(k+'_r5.png'));canvas=Image.new('RGB',(im.width*3,im.height+24));d=ImageDraw.Draw(canvas)
 for j,c in enumerate(['black','white','#ed00d2']):
  x=j*im.width;d.rectangle((x,24,x+im.width,canvas.height),fill=c);canvas.paste(im,(x,24),im)
 canvas.save(qa/(k+'_alpha.png'))
(b/'ArtAudit.json').write_text(json.dumps(audit,indent=2))
