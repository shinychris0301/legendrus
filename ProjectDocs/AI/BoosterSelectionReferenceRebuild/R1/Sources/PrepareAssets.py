from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,cv2,json,hashlib,uuid
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGBA');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned');boxes=json.loads((b/'Boxes.json').read_text());records={}
def save(n,im,method):
 p=out/(n+'.png');im.save(p);meta=p.with_suffix('.png.meta')
 if not meta.exists():
  t=(out/'panel.png.meta').read_text();g=next(x for x in t.splitlines() if x.startswith('guid:'));meta.write_text(t.replace(g,'guid: '+uuid.uuid4().hex))
 records[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method}
def poly(size,points):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
panel=src.crop(boxes['panel']);alpha=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(panel.size,Image.Resampling.LANCZOS)
# Pixa retained dark underlying board-gem fragments in two top scrollwork gaps.
rgb=np.asarray(panel).astype(float);gold=np.uint8((rgb[:,:,0]>45)&(rgb[:,:,0]>rgb[:,:,2]*1.2)&(rgb[:,:,1]>rgb[:,:,2]*1.08))*255
gold=cv2.dilate(gold,np.ones((3,3),np.uint8));aa=np.asarray(alpha).copy()
for x,y,x2,y2 in [(65,28,300,87),(501,35,655,87)]:aa[y:y2,x:x2]=np.minimum(aa[y:y2,x:x2],gold[y:y2,x:x2])
nn,ll,st,_=cv2.connectedComponentsWithStats(np.uint8(aa>8));largest=1+np.argmax(st[1:,cv2.CC_STAT_AREA]);aa[ll!=largest]=0;alpha=Image.fromarray(aa)
ImageDraw.Draw(alpha).ellipse((695,63,787,159),fill=0)
# Reuse the prior approved empty navy face. Do not include its generated frame/divider.
old=Image.open(out/'panel.png').convert('RGBA');material=old.crop((70,280,old.width-70,old.height-190)).resize(panel.size,Image.Resampling.LANCZOS)
mask=Image.new('L',panel.size);d=ImageDraw.Draw(mask)
d.polygon([(39,181),(70,111),(146,94),(325,75),(461,75),(646,94),(720,115),(753,172),(753,1020),(730,1075),(680,1120),(665,1151),(130,1151),(125,1120),(65,1075),(40,1020)],fill=255)
mask=mask.filter(ImageFilter.GaussianBlur(1.5))
panel=Image.composite(material,panel,mask)
# Keep the source carving above the repaired surface; the material must never cover it.
orn=Image.new('L',panel.size);od=ImageDraw.Draw(orn);od.rectangle((0,0,794,180),fill=255);od.rectangle((145,104,656,173),fill=0);od.ellipse((695,63,787,159),fill=0)
od.rectangle((0,1030,794,1193),fill=255);od.rectangle((154,986,640,1131),fill=0);od.rectangle((376,1132,422,1141),fill=0)
om=ImageChops.darker(orn,Image.fromarray(gold));om=ImageChops.darker(om,alpha)
# Sapphire crest is not gold, and remains wholly original.
crest=Image.new('L',panel.size);ImageDraw.Draw(crest).ellipse((366,5,430,95),fill=255);om=ImageChops.lighter(om,ImageChops.darker(crest,alpha))
panel.paste(src.crop(boxes['panel']),(0,0),om)
# Restore only original decorative divider pixels, not its differently lit navy strip.
div=src.crop((168,391,775,435));arr=np.asarray(div).astype(float);rr,gg,bb=arr[:,:,:3].transpose(2,0,1)
dm=np.uint8((rr>45)&(rr>bb*1.25)&(gg>bb*1.1))*255;dm=cv2.dilate(dm,np.ones((3,3),np.uint8));da=Image.fromarray(dm)
ImageDraw.Draw(da).ellipse((273,6,328,38),fill=255)
ImageDraw.Draw(da).rectangle((420,0,455,10),fill=0)
panel.paste(div,(96,173),da.filter(ImageFilter.GaussianBlur(.3)))
# Pixa cut through the opaque face beneath the old button; restore opaque repaired face.
opaque=mask.copy();ImageDraw.Draw(opaque).rectangle((0,0,794,176),fill=0)
alpha=ImageChops.lighter(alpha,opaque)
panel.putalpha(alpha);save('panel_r2',panel,'Original gold perimeter/divider with fresh Pixa alpha and bounded scroll-gap cleanup. Reused Scenario navy material fills occupied interior; intentionally opaque face restored.')
title=src.crop(boxes['title']);a=np.asarray(title).astype(float);r,g,bl=a[:,:,:3].transpose(2,0,1);m=np.uint8((r>65)&(r>g*1.02)&(g>bl*1.18))*255
n,ll,st,_=cv2.connectedComponentsWithStats(m);keep=np.zeros_like(m)
for i in range(1,n):
 if st[i,cv2.CC_STAT_AREA]>12 and st[i,cv2.CC_STAT_HEIGHT]>8:keep[ll==i]=255
title.putalpha(Image.fromarray(cv2.dilate(keep,np.ones((3,3),np.uint8))).filter(ImageFilter.GaussianBlur(.3)));save('title_ru_r2',title,'Original Russian title RGB including shading, isolated lettering alpha.')
play=src.crop(boxes['play']);pa=Image.open(b/'Scenario/play/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(play.size,Image.Resampling.LANCZOS)
contour=poly(play.size,[(0,82),(20,67),(27,35),(48,12),(85,6),(215,9),(240,0),(260,9),(401,9),(430,16),(451,45),(458,68),(479,81),(451,99),(444,125),(420,140),(264,142),(240,152),(219,141),(65,140),(40,124),(27,97)])
pa=ImageChops.darker(pa,contour);ImageDraw.Draw(pa).rectangle((0,144,218,157),fill=0);ImageDraw.Draw(pa).rectangle((265,144,479,157),fill=0);play.putalpha(pa);save('play_ru_r2',play,'Original Play button RGB and ИГРАТЬ lettering; fresh Scenario alpha constrained to remove adjoining panel ornament.')
blank=Image.open(out/'play.png').convert('RGBA').resize(play.size,Image.Resampling.LANCZOS);m=Image.new('L',play.size);ImageDraw.Draw(m).rectangle((92,38,383,117),fill=255)
blank=Image.composite(blank,play,m.filter(ImageFilter.GaussianBlur(1)));blank.putalpha(pa);save('play_blank_r2',blank,'Original button border with reused Scenario empty emerald face under a bounded lettering mask for live English.')
preview_records=json.loads((b/'PreviewBoxes.json').read_text())
preview_alpha=Image.open(b/'Scenario/previews/booster_medallion_trial01.png').convert('RGBA').resize((480,480),Image.Resampling.LANCZOS)
for i,rec in enumerate(preview_records):
 im=src.crop(rec['source']);im.putalpha(preview_alpha.crop(rec['atlas']).getchannel('A'))
 # Sample counts and plus buttons are not functional previews. Repair only their circles
 # using the unobstructed matching upper rim; retain the central original subject.
 donor=im.transpose(Image.Transpose.FLIP_TOP_BOTTOM);m=Image.new('L',im.size);md=ImageDraw.Draw(m)
 centers=[(39,155),(137,156)] if i<2 else [(38,156),(133,156)]
 for x,y in centers:md.ellipse((x-25,y-25,x+25,y+25),fill=255)
 im=Image.composite(donor,im,m.filter(ImageFilter.GaussianBlur(.7)))
 save('preview_'+str(i)+'_r2',im,'Original preview subject/frame RGB with Scenario atlas alpha; count/plus circles reconstructed from mirrored upper rim. Preview only, no stock or equip control.')
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'ScenarioJobs':3,'ScenarioCU':9,'reusedMaterialSHA256':{n:hashlib.sha256((out/n).read_bytes()).hexdigest() for n in ['panel.png','play.png']},'sourceSHA256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest()},indent=2))
for n,c in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(1370,1230),c)
 for key,xy in [('panel_r2',(0,0)),('title_ru_r2',(820,100)),('play_ru_r2',(820,300)),('play_blank_r2',(820,550)),('preview_0_r2',(820,810)),('preview_1_r2',(1040,810)),('preview_2_r2',(820,1020)),('preview_3_r2',(1040,1020))]:
  im=Image.open(out/(key+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+n+'.png'))
