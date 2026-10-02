from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
root=Path.cwd();b=Path(__file__).resolve().parent;prior=root/'ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources';dst=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayOrnamentsR5';dst.mkdir(exist_ok=True)
# Recover the complete source scrollwork from the original Scenario alpha, not the R1 ellipse support.
a=Image.open(prior/'Inputs/portrait.png').convert('RGBA')
m=Image.open(prior/'Scenario/portrait/booster_medallion_trial01.png').getchannel('A').resize(a.size,Image.Resampling.LANCZOS);a.putalpha(m)
# The removed example-level badge hides part of the ring. Reconstruct only that arc from its left counterpart.
mirror=a.transpose(Image.Transpose.FLIP_LEFT_RIGHT);a.paste(mirror.crop((174,164,232,229)),(174,164))
support=Image.new('L',a.size);d=ImageDraw.Draw(support)
d.polygon([(0,0),(214,0),(214,93),(232,113),(215,140),(220,178),(211,202),(176,214),(143,217),(128,229),(106,229),(90,216),(55,215),(20,209),(6,189),(0,145)],fill=255)
m=np.minimum(np.array(a.getchannel('A')),np.array(support))
# Remove the attached header stub and nameplate surface, keeping the complete bottom jewel.
m[40:48,205:]=0
bottom=Image.new('L',a.size);ImageDraw.Draw(bottom).polygon([(117,196),(133,211),(117,224),(102,211)],fill=255)
m[213:]=np.minimum(m[213:],np.array(bottom)[213:])
p=np.array(a);rgb=p[:,:,:3].astype(float)
gold=((rgb[:,:,0]>65)&(rgb[:,:,1]>35)&(rgb[:,:,0]>rgb[:,:,2]*1.3)&(m>0)).astype('uint8')*255
cs,_=cv2.findContours(cv2.morphologyEx(gold,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8)),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)
outer=np.zeros(m.shape,np.uint8);cv2.drawContours(outer,[max(cs,key=cv2.contourArea)],-1,255,-1)
m=Image.fromarray(np.minimum(m,outer));ImageDraw.Draw(m).ellipse((24,30,205,202),fill=0);a.putalpha(m);a.save(dst/'portrait.png')
# Restore the original caption curls; remove only the original word inside the clear center of the face.
a=Image.open(root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayReferenceAligned/card.png').convert('RGBA');src=Image.open(prior/'Inputs/card.png').convert('RGBA')
al=a.getchannel('A');a.paste(src.crop((0,218,192,288)),(0,218));a.putalpha(al)
# Original label is entirely in this central clear region, away from all metal.
p=np.array(a);rgb=p[:,:,:3].astype(float);allowed=np.zeros(p.shape[:2],np.uint8);allowed[221:248,50:156]=255
textmask=((rgb[:,:,0]>110)&(rgb[:,:,1]>70)).astype('uint8')*255
textmask=allowed
p[:,:,:3]=cv2.inpaint(p[:,:,:3],textmask,5,cv2.INPAINT_TELEA);a=Image.fromarray(p);a.putalpha(al);a.save(dst/'card.png')
# Continue the panel beneath the portrait, so no exposed cut edge ends at x230.
old=Image.open(root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayHeaderR4/header.png').convert('RGBA')
h=Image.new('RGBA',(824,186))
wing=Image.open(root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayRefinedR2/score.png').crop((30,25,145,177)).resize((135,172)).convert('RGBA');h.paste(wing,(0,6))
ref=Image.open(prior/'OriginalReference.png').convert('RGBA')
h.paste(ref.crop((250,58,365,66)).resize((135,8)),(0,1));h.paste(ref.crop((250,224,365,234)).resize((135,10)),(0,167));h.alpha_composite(old,(115,0))
# Discard external dark padding outside the connected metal contour. Faces remain opaque.
p=np.array(h);rgb=p[:,:,:3].astype(float);gold=((rgb[:,:,0]>95)&(rgb[:,:,1]>48)&(rgb[:,:,0]>rgb[:,:,2]*1.5)&(p[:,:,3]>0)).astype('uint8')*255
support=np.zeros(gold.shape,np.uint8)
for x in range(gold.shape[1]):
 ys=np.flatnonzero(gold[:,x])
 if len(ys):support[ys[0]:ys[-1]+1,x]=255
support=cv2.morphologyEx(support,cv2.MORPH_CLOSE,np.ones((3,7),np.uint8));p[:,:,3]=support;h=Image.fromarray(p);h.save(dst/'header.png')
# Source-art obstruction map for caption tests/inspection: only gold carving in the label band.
p=np.array(src);r=p[:,:,0].astype(float);g=p[:,:,1].astype(float);bb=p[:,:,2].astype(float)
orn=(r>95)&(g>48)&(r>bb*1.65)&(g>bb*1.45);orn[:218]=False;orn[258:]=False;orn[222:247,52:155]=False
Image.fromarray((orn*255).astype('uint8')).save(b/'CaptionOrnamentMask.png')
for n in ['portrait','header','card']:
 a=Image.open(dst/f'{n}.png').convert('RGBA')
 for color,name in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
  q=Image.new('RGBA',a.size,color+(255,));q.alpha_composite(a);q.save(b/f'{n}_{name}.png')
(b/'AssetProvenance.json').write_text(json.dumps({'newScenarioCalls':0,'portrait':'Original RGB and existing Scenario alpha, complete scrollwork; lower right badge-covered arc reflected from original left, not exact recovery. Original nameplate/example badge remain omitted.','card':'Original caption ornaments restored; only center source word at52,222..153,246 removed with a bounded center mask and local inpaint on the original dark wood.','header':'R4 joined panel extended115px behind portrait using existing dark wood and original rails; outside dark padding removed using connected metal contour.'},indent=2))
