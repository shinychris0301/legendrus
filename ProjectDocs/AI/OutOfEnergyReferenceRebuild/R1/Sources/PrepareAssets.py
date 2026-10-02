from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib,uuid
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGBA')
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/OutOfEnergyAligned');boxes=json.loads((b/'Boxes.json').read_text());records={}
def save(n,im,method):
 p=out/(n+'.png');im.save(p);meta=p.with_suffix('.png.meta')
 if not meta.exists():
  t=(out/'panel.png.meta').read_text();g=next(x for x in t.splitlines() if x.startswith('guid:'));meta.write_text(t.replace(g,'guid: '+uuid.uuid4().hex))
 records[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method}
def poly(size,points):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def lettering(box):
 im=src.crop(box);a=np.asarray(im).astype(float);r,g,bl=a[:,:,:3].transpose(2,0,1)
 mask=np.uint8((r>65)&(r>g*1.02)&(g>bl*1.18))*255
 n,ll,st,_=cv2.connectedComponentsWithStats(mask);keep=np.zeros_like(mask)
 for i in range(1,n):
  if st[i,cv2.CC_STAT_AREA]>8 and st[i,cv2.CC_STAT_HEIGHT]>6:keep[ll==i]=255
 im.putalpha(Image.fromarray(cv2.dilate(keep,np.ones((3,3),np.uint8))).filter(ImageFilter.GaussianBlur(.3)));return im
panel=src.crop(boxes['panel']);alpha=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(panel.size)
# Remove unrelated Play-button fragments below the original main bottom rail.
draw=ImageDraw.Draw(alpha);draw.rectangle((0,1080,335,1110),fill=0);draw.rectangle((460,1080,795,1110),fill=0);draw.rectangle((0,1097,795,1110),fill=0)
old=Image.open(out/'panel.png').convert('RGBA').resize(panel.size,Image.Resampling.LANCZOS)
# Use only the empty upper material so generated rail positions cannot duplicate source rails.
tile=old.crop((100,150,696,510)).resize((712,430),Image.Resampling.LANCZOS);face=Image.new('RGBA',(712,914))
for y in range(0,914,tile.height):face.paste(tile if (y//tile.height)%2==0 else ImageOps.flip(tile),(0,y))
old.paste(face,(42,127))
mask=Image.new('L',panel.size);ImageDraw.Draw(mask).rectangle((42,127,753,1040),fill=255)
panel=Image.composite(old,panel,mask.filter(ImageFilter.GaussianBlur(2)))
# Restore the exact source inner rail perimeters over the already approved blank material.
for box in [(209,798,733,918),(117,932,822,1064),(117,1071,822,1189),(117,1199,822,1314)]:
 x,y,x2,y2=box;im=src.crop(box);ring=Image.new('L',im.size,255);ImageDraw.Draw(ring).rounded_rectangle((10,9,im.width-11,im.height-10),radius=7,fill=0);panel.paste(im,(x-71,y-274),ring)
aa=np.asarray(alpha).copy();nn,ll,st,_=cv2.connectedComponentsWithStats(np.uint8(aa>8));largest=1+np.argmax(st[1:,cv2.CC_STAT_AREA]);aa[ll!=largest]=0;alpha=Image.fromarray(aa)
ImageDraw.Draw(alpha).ellipse((710,60,786,139),fill=0)
panel.putalpha(alpha);save('panel_r2',panel,'Original perimeter and inner rails, fresh Pixa alpha, reused approved Scenario blank navy material only in occupied interior.')
med=src.crop(boxes['medallion']);ma=Image.open(b/'Scenario/medallion/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(med.size);med.putalpha(ma);save('medallion_r2',med,'Original source medallion RGB with fresh Scenario alpha. Baked zero covered by separate opaque live badge.')
badge=src.crop(boxes['badge']);a=np.asarray(badge).copy();m=np.zeros(a.shape[:2],np.uint8);cv2.ellipse(m,(39,39),(15,26),0,0,360,255,-1);a[:,:,:3]=cv2.inpaint(a[:,:,:3],m,5,cv2.INPAINT_TELEA);badge=Image.fromarray(a)
ba=Image.new('L',badge.size);ImageDraw.Draw(ba).ellipse((1,1,76,77),fill=255);badge.putalpha(ba.filter(ImageFilter.GaussianBlur(.35)));save('badge_r2',badge,'Original navy/gold badge, bounded zero-glyph inpaint for live count.')
save('title_ru_r2',lettering(boxes['title']),'Original Russian title RGB including embossing, connected lettering alpha.')
for i,box in enumerate([(264,966,576,1003),(264,1100,570,1137),(264,1233,566,1270)]):save('action_title_'+str(i)+'_ru_r2',lettering(box),'Original Russian action heading RGB, isolated lettering.')
for key in ['ask','watch','buy']:
 im=src.crop(boxes[key]);w,h=im.size;al=poly(im.size,[(2,20),(21,1),(w-21,1),(w-2,20),(w-2,h-19),(w-20,h-2),(20,h-2),(2,h-20)])
 im.putalpha(al)
 if key!='buy':save(key+'_ru_r2',im,'Original source button with Russian caption and gold rim; remains disabled until corresponding service exists.')
 blank=Image.open(out/('green.png' if key=='buy' else 'blue.png')).convert('RGBA').resize(im.size,Image.Resampling.LANCZOS)
 m=Image.new('L',im.size);ImageDraw.Draw(m).rectangle((17,14,w-18,h-15),fill=255)
 blank=Image.composite(blank,im,m.filter(ImageFilter.GaussianBlur(1)));blank.putalpha(al);save(key+'_blank_r2',blank,'Original button rim; reused Scenario blank face removes unconfigured price or lettering for English.')
energy=src.crop(boxes['energy']);energy.putalpha(poly(energy.size,[(29,6),(65,5),(46,36),(73,37),(16,96),(31,56),(6,55)]));save('energy_r2',energy,'Original lightning offer icon RGB, contour alpha.')
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'ScenarioJobs':2,'ScenarioCU':6,'reusedMaterial':'ProductionUI/OutOfEnergyAligned/panel.png, green.png, blue.png: previous Scenario repair outputs; no new RGB generation'},indent=2))
for n,c in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(1280,1160),c);im=Image.open(out/'panel_r2.png');sheet.paste(im,(0,0),im)
 for key,xy in [('medallion_r2',(825,0)),('title_ru_r2',(825,280)),('badge_r2',(825,350)),('energy_r2',(940,350)),('ask_ru_r2',(825,470)),('watch_ru_r2',(825,570)),('buy_blank_r2',(825,675)),('action_title_0_ru_r2',(825,800)),('action_title_1_ru_r2',(825,855)),('action_title_2_ru_r2',(825,910))]:
  im=Image.open(out/(key+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+n+'.png'))
