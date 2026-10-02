from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGBA');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ShopAligned');manifest={}
def save(n,im,note):
 p=out/(n+'.png')
 if n.startswith('nav_'):
  aa=np.array(im.getchannel('A'));count,labels,stats,_=cv2.connectedComponentsWithStats((aa>0).astype('uint8'),8)
  if count>1:aa[labels!=1+np.argmax(stats[1:,cv2.CC_STAT_AREA])]=0
  im.putalpha(Image.fromarray(aa))
 im.save(p);manifest[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':note}
def maskpoly(size,pts):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).polygon([(int(x*4),int(y*4))for x,y in pts],fill=255);return m.resize(size,Image.Resampling.LANCZOS)
def lettering(box,cream=False):
 im=src.crop(box);a=np.asarray(im).astype(float);r,g,bl=a[:,:,:3].transpose(2,0,1)
 if cream:raw=(r>165)&(g>125)&(r>bl*1.12)
 else:raw=(r>96)&(g>64)&(r>bl*1.4)&(g>bl*1.15)
 n,lab,stats,_=cv2.connectedComponentsWithStats(raw.astype('uint8'),8);m=np.zeros(raw.shape,'uint8')
 for k in range(1,n):
  if stats[k,cv2.CC_STAT_HEIGHT]>=8 and stats[k,cv2.CC_STAT_AREA]>=9:m[lab==k]=255
 # Seed from bright source lettering, recover shaded edge pixels within two pixels only.
 if np.any(m):
  gc=np.zeros(m.shape,'uint8');possible=cv2.dilate(m,np.ones((5,5),'uint8'))>0;gc[possible]=cv2.GC_PR_FGD;gc[m>0]=cv2.GC_FGD
  cv2.grabCut(np.asarray(im)[:,:,:3].copy(),gc,None,np.zeros((1,65)),np.zeros((1,65)),2,cv2.GC_INIT_WITH_MASK)
  m=np.where((gc==cv2.GC_FGD)|(gc==cv2.GC_PR_FGD),255,0).astype('uint8')
 im.putalpha(Image.fromarray(m).filter(ImageFilter.GaussianBlur(.25)));return im
# Same original opaque card surface, repaired only in the occupied face. Perimeter RGB remains original.
card=src.crop((39,393,470,884));repair=Image.open(b/'Scenario/card/booster_medallion_trial01.png').convert('RGBA').resize(card.size,Image.Resampling.LANCZOS)
m=Image.new('L',card.size);ImageDraw.Draw(m).rounded_rectangle((24,23,409,472),radius=26,fill=255);m=m.filter(ImageFilter.GaussianBlur(2));m.save(b/'CardFaceRepairMask.png');card=Image.composite(repair,card,m)
card.putalpha(maskpoly(card.size,[(42,1),(390,1),(420,10),(430,39),(429,453),(418,479),(389,490),(37,488),(6,476),(0,447),(2,35),(13,13)]));save('card_r2',card,'Original gold perimeter with single Scenario face repair under recorded mask; opaque parchment retained.')
# Separate static source words, never include sample prices or availability claims.
for i,box in enumerate([(138,416,380,454),(544,416,836,454),(129,918,375,957),(589,918,792,957)]):save('name_'+str(i)+'_ru_r2',lettering(box,True),'Original Russian product-name RGB with glyph-only alpha; English remains live.')
for i,box in enumerate([(72,313,225,347),(282,313,458,350),(525,313,660,350),(728,313,862,350)]):save('tab_'+str(i)+'_ru_r2',lettering(box),'Original category letterforms separated from the opaque tab material.')
# Source arch is an independent carved UI backing. Remove foreground plaque/header/portrait, leave original visible wood.
arch=src.crop((0,120,941,284));am=Image.new('L',src.size);am.paste(Image.open(out/'title_ru.png').getchannel('A'),(106,94));am=am.filter(ImageFilter.MaxFilter(7));am=np.array(am.crop((0,120,941,284)));keep=255-am
# Original portrait's pendant is above this woodwork: its occupied section remains transparent.
donor=ImageOps.mirror(arch);dm=Image.new('L',arch.size);ImageDraw.Draw(dm).rectangle((0,0,168,95),fill=255);dm=dm.filter(ImageFilter.GaussianBlur(4));arch=Image.composite(donor,arch,dm)
dm.save(b/'ArchHiddenPortraitRepairMask.png')
arch.putalpha(Image.fromarray(keep));save('arch_r2',arch,'Original carved UI backing behind title; expanded title mask excludes lettering; hidden portrait area repaired from opposite original carved arch.')
for i,(n,cx,cy,rx,ry) in enumerate([('offers',102,1535,81,76),('gems',287,1531,76,77),('bundles',472,1530,76,76),('boosters',656,1530,76,76),('vip',843,1537,75,78)]):
 box=(cx-rx,cy-ry,cx+rx,1586);im=src.crop(box)
 if i in (0,4):
  # Only badge-covered upper-right rim is reconstructed from its corresponding opposite rim.
  donor=ImageOps.mirror(im);cxlocal=(145 if i==0 else 881)-box[0];cylocal=(1483 if i==0 else 1485)-box[1]
  bm=Image.new('L',im.size);ImageDraw.Draw(bm).ellipse((cxlocal-20,cylocal-21,cxlocal+20,cylocal+21),fill=255);bm=bm.filter(ImageFilter.GaussianBlur(1));im=Image.composite(donor,im,bm)
 alpha=Image.new('L',(im.width*4,im.height*4));ImageDraw.Draw(alpha).ellipse((0,0,im.width*4-1,ry*8-1),fill=255);alpha=alpha.resize(im.size,Image.Resampling.LANCZOS);a=np.asarray(im).astype(float);r,g,bl=a[:,:,:3].transpose(2,0,1);gold=(r>70)&(r>g*1.12)&(g>bl*1.15)
 core=Image.new('L',im.size);ImageDraw.Draw(core).ellipse((9,9,im.width-10,ry*2-10),fill=255)
 aa=np.array(alpha);aa[(np.asarray(core)==0)&~gold]=0;im.putalpha(Image.fromarray(aa))
 save('nav_'+n+'_r2',im,'Original opaque medallion isolated along outer ellipse; no baked notification state. Badge-covered rim locally mirrored where applicable.')
for i,box in enumerate([(14,1581,194,1625),(197,1581,369,1626),(381,1581,556,1626),(565,1581,740,1626),(751,1581,935,1626)]):
 im=src.crop(box);w,h=im.size;im.putalpha(maskpoly(im.size,[(12,1),(w-13,1),(w-3,10),(w-9,18),(w-1,27),(w-12,h-2),(12,h-2),(2,27),(8,18),(2,10)]));a=np.asarray(im).astype(float);r,g,bl=a[:,:,:3].transpose(2,0,1);gold=(r>75)&(r>g*1.1)&(g>bl*1.15);core=maskpoly(im.size,[(18,7),(w-24,7),(w-13,22),(w-24,h-8),(18,h-8),(8,22)]);aa=np.array(im.getchannel('A'));aa[(np.asarray(core)==0)&~gold]=0;im.putalpha(Image.fromarray(aa));save('nav_label_'+str(i)+'_ru_r2',im,'Original opaque gold caption plaque including source Russian lettering; localizable replacement on English.')
# A wide blank source plaque for localized labels; the generic tab was too narrow when aspect-fitted.
im=Image.open(out/'nav_label_4_ru_r2.png').convert('RGBA');patch=Image.open(out/'tab.png').convert('RGBA').crop((36,26,195,64)).resize((144,29),Image.Resampling.LANCZOS)
m=Image.new('L',im.size);ImageDraw.Draw(m).rectangle((24,10,160,33),fill=255);m=m.filter(ImageFilter.GaussianBlur(1));layer=im.copy();layer.paste(patch,(20,8));oldalpha=im.getchannel('A');im=Image.composite(layer,im,m);im.putalpha(oldalpha);save('nav_label_blank_r2',im,'Original wide caption perimeter/alpha; lettering area repaired from existing blank navy tab material for English labels.')
# Feather the previous English title repair into the original opaque face; preserve source perimeter/alpha.
im=Image.open(out/'title_ru.png').convert('RGBA');blank=Image.open(out/'title_blank.png').convert('RGBA');m=np.zeros((im.height,im.width),'uint8');m[91:164,122:615]=255
inside=cv2.distanceTransform((m>0).astype('uint8'),cv2.DIST_L2,3);m=(np.clip(inside/3,0,1)*255).astype('uint8');im=Image.composite(blank,im,Image.fromarray(m));save('title_blank_r2',im,'Original title perimeter; existing English material blended across a three-pixel interior boundary without altering Russian source.')
(b/'AssetProvenance.json').write_text(json.dumps(manifest,indent=2))
for name,col in [('Black','black'),('White','white'),('Magenta','#ff00b0')]:
 sheet=Image.new('RGB',(1100,((len(manifest)+3)//4)*285),col);d=ImageDraw.Draw(sheet)
 for i,n in enumerate(manifest):
  a=Image.open(out/(n+'.png'));a.thumbnail((260,245));x=(i%4)*275;y=(i//4)*285;sheet.paste(a,(x,y+25),a);d.text((x,y),n,fill='gray')
 sheet.save(b.parent/'QA'/('Alpha'+name+'.png'))
print('Prepared',len(manifest),'source assets')
