from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,uuid,shutil
b=Path(__file__).resolve().parent;dst=b.parent/'Assets';imgs={};repairs={}
for n in ['goals','moves','boosters','rewards','mace','firebird','shield','potion','chest']:
 a=Image.open(b/'Inputs'/f'{n}.png').convert('RGBA');m=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(a.size,Image.Resampling.LANCZOS);a.putalpha(m);imgs[n]=a
# Navy source texture donor; all repaired content areas are explicit and kept inside original thin rails.
orig=Image.open(b/'OriginalReference.png').convert('RGB');donor=orig.crop((279,540,311,601))
def surface(n,box):
 a=imgs[n];x0,y0,x1,y1=box;texture=Image.new('RGB',(x1-x0,y1-y0))
 texture=donor.resize(texture.size,Image.Resampling.BICUBIC)
 # Feather interior repairs, retaining original RGB on the border.
 mask=Image.new('L',a.size);ImageDraw.Draw(mask).rectangle(box,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(3));lay=Image.new('RGBA',a.size);lay.paste(texture,(x0,y0));alpha=a.getchannel('A');a=Image.composite(lay,a,mask);a.putalpha(alpha);imgs[n]=a;repairs.setdefault(n,[]).append(box)
def blank(n,boxes):
 a=np.array(imgs[n]);mask=np.zeros(a.shape[:2],np.uint8)
 for x0,y0,x1,y1 in boxes:mask[y0:y1,x0:x1]=255
 rgb=a[:,:,:3];gold=(rgb[:,:,0].astype(int)>rgb[:,:,2].astype(int)*1.25)&(rgb[:,:,0]>95)&(rgb[:,:,1]>65)
 mask=cv2.dilate(((mask>0)&gold).astype(np.uint8)*255,np.ones((5,5),np.uint8));a[:,:,:3]=cv2.inpaint(rgb,mask,3,cv2.INPAINT_TELEA);imgs[n]=Image.fromarray(a);Image.fromarray(mask).save(b/(n+'_text_mask.png'));repairs.setdefault(n,[]).extend(boxes)
# Restore opaque panel interiors and suppress adjacent medallion/banner silhouettes.
for n,box in [('goals',(20,55,509,333)),('boosters',(24,52,679,294))]:
 m=imgs[n].getchannel('A');ImageDraw.Draw(m).rectangle(box,fill=255);imgs[n].putalpha(m)
surface('goals',(70,126,458,301));blank('goals',[(210,69,319,109)])
surface('boosters',(28,105,675,289));blank('boosters',[(182,67,516,103)])
# Full rewards surface is intentionally opaque. Frame extraction needs a second subject-focused input.
surface('rewards',(80,86,630,278));blank('rewards',[(163,24,527,61)])
a=imgs['rewards'];m=Image.new('L',a.size);ImageDraw.Draw(m).rounded_rectangle((8,10,a.width-8,a.height-9),radius=32,fill=255);a.putalpha(m);a.save(b/'Inputs/rewards_frame.png')
# Isolate the moves medallion from neighboring horizontal rails and panel scrolls.
m=imgs['moves'].getchannel('A');support=Image.new('L',m.size);d=ImageDraw.Draw(support);d.ellipse((8,10,207,199),fill=255);d.polygon([(106,0),(124,20),(203,83),(214,99),(198,116),(125,184),(106,205),(88,184),(16,117),(0,103),(18,87),(89,21)],fill=255);m=Image.fromarray(np.minimum(np.array(m),np.array(support)));imgs['moves'].putalpha(m);blank('moves',[(46,43,161,74),(57,80,158,153)])
# Keep stock badge faces but erase example quantities. Suppress a neighboring medallion at either edge.
for n in ['mace','firebird','shield','potion']:
 a=imgs[n];support=Image.new('L',a.size);d=ImageDraw.Draw(support);cx=80 if n!='potion' else 76
 d.ellipse((cx-76,11,cx+76,166),fill=255);d.polygon([(cx,0),(cx+15,18),(cx+78,73),(cx+85,86),(cx+70,100),(cx+13,157),(cx,179),(cx-15,158),(cx-78,100),(cx-85,86),(cx-72,73),(cx-14,18)],fill=255);d.ellipse((104,116,156,170),fill=255)
 a.putalpha(Image.fromarray(np.minimum(np.array(a.getchannel('A')),np.array(support))));blank(n,[(112,126,152,164)])
for n,a in imgs.items():a.save(dst/(n+'.png'))
(b/'Repairs.json').write_text(json.dumps({'regions':repairs,'surfaceDonor':[279,540,311,601],'note':'Source RGB retained except recorded content/text repairs. Opaque panel restoration and bounded removal of neighboring controls; not exact recovery of hidden pixels.'},indent=2))
