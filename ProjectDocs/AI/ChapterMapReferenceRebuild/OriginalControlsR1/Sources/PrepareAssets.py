from pathlib import Path
from PIL import Image, ImageDraw
import numpy as np,cv2,json,shutil
p=Path(__file__).resolve().parent; dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned');dst.mkdir(parents=True,exist_ok=True)
imgs={};repairs={}
for n in ['banner','play','back','node','filled_stars','empty_stars','footer']:
 a=Image.open(p/'Inputs'/f'{n}.png').convert('RGBA');m=Image.open(p/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(a.size,Image.Resampling.LANCZOS);a.putalpha(m);imgs[n]=a
# Restore only intentionally opaque control faces. Preserve original RGB.
for n,box in [('banner',(137,93,650,206)),('play',(83,38,394,124)),('back',(41,29,135,121)),('node',(26,24,100,98))]:
 a=imgs[n];m=a.getchannel('A');d=ImageDraw.Draw(m);d.ellipse(box,fill=255) if n in ['back','node'] else d.rectangle(box,fill=255);a.putalpha(m)
# Bound masks to individual subjects; original RGB is never taken from Scenario.
a=imgs['node'];m=a.getchannel('A');d=ImageDraw.Draw(m);d.rectangle((0,111,126,116),fill=0);a.putalpha(m)
# One isolated full/empty star from each row, removing the medallion fragment above.
imgs['star_full']=imgs['filled_stars'].crop((3,3,38,37))
imgs['star_empty']=imgs['empty_stars'].crop((35,7,71,39))
# Bottom carved wood only: remove neighboring button and Back remnants.
a=imgs['footer'];m=a.getchannel('A');d=ImageDraw.Draw(m);d.polygon([(0,0),(188,0),(145,28),(0,28)],fill=0);d.rectangle((240,0,700,25),fill=0);d.polygon([(402,0),(539,0),(510,78),(430,78)],fill=0);a.putalpha(m)
# Local text removal only for live numbers and English variants.
def blank(src,key,boxes):
 a=np.array(imgs[src]); mask=np.zeros(a.shape[:2],np.uint8)
 for x0,y0,x1,y1 in boxes: mask[y0:y1,x0:x1]=255
 gold=(a[:,:,0].astype(int)>a[:,:,2].astype(int)*1.25)&(a[:,:,0]>95)&(a[:,:,1]>65)
 mask=cv2.dilate((gold*(mask>0)).astype(np.uint8)*255,np.ones((11,11),np.uint8))
 a[:,:,:3]=cv2.inpaint(a[:,:,:3],mask,5,cv2.INPAINT_TELEA);imgs[key]=Image.fromarray(a);Image.fromarray(mask).save(p/f'{key}_repair_mask.png');repairs[key]=boxes
blank('node','node_blank',[(32,40,96,90)])
blank('banner','banner_blank',[(254,75,560,112),(127,112,661,175)])
blank('play','play_blank',[(101,55,380,124)])
keep=['banner','banner_blank','play','play_blank','back','node_blank','star_full','star_empty','footer']
for n in ['node_blank','star_full','star_empty']:
 a=imgs[n];alpha=np.array(a.getchannel('A'));count,labels,stats,_=cv2.connectedComponentsWithStats((alpha>100).astype(np.uint8));idx=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);support=cv2.dilate((labels==idx).astype(np.uint8),np.ones((3,3),np.uint8));alpha*=support;a.putalpha(Image.fromarray(alpha))
for n in keep:imgs[n].save(dst/f'{n}.png');imgs[n].save(p.parent/'Assets'/f'{n}.png')
# Transparent resource QA, rendered over three contrasting backgrounds.
out=Image.new('RGB',(1200,1000),'black');draw=ImageDraw.Draw(out)
for col,bg in enumerate(['black','white','#cf1cca']):
 y=0
 for n in keep:
  a=imgs[n].copy();a.thumbnail((380,160 if n=='banner' else 92));out.paste(bg,(col*400,y,col*400+400,y+a.height+20));out.paste(a,(col*400+5,y+18),a);draw.text((col*400+5,y),n,fill='#777777');y+=a.height+24
out.save(p.parent/'QA/AlphaContactSheet.jpg')
(p/'AssetProvenance.json').write_text(json.dumps({'workflow':'Scenario Pixa alpha + original RGB; local alpha cleanup and explicit bounded English/numeral inpaint','assets':keep,'repairs':repairs,'jobs':7,'cuCost':sum(json.loads((p/'Scenario'/n/'Billing.json').read_text())['cuCost'] for n in ['banner','play','back','node','filled_stars','empty_stars','footer'])},indent=2))
