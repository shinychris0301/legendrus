from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps,ImageEnhance
import numpy as np,json,hashlib
root=Path.cwd();base=root/'ProjectDocs/AI/ShopStyleAlignment/R1';out=base/'Prepared';out.mkdir(exist_ok=True)
src=Image.open(root/'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Shop.png').convert('RGBA')
wood=Image.open(root/'ProjectDocs/AI/ChapterCompleteStyleAlignment/SceneFrameR3/Sources/ScenarioWood/booster_medallion_trial01.png').convert('RGBA')
navy=Image.open(root/'ProjectDocs/AI/ChapterCompleteStyleAlignment/PanelClarityR5/Sources/ScenarioNavy/booster_medallion_trial01.png').convert('RGBA')
manifest={}
def save(n,im,how):
 im.save(out/(n+'.png'));manifest[n]={'size':im.size,'method':how,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()}
def polygon(im,pts):
 m=Image.new('L',(im.width*4,im.height*4));ImageDraw.Draw(m).polygon([(int(x*4),int(y*4)) for x,y in pts],fill=255);im=im.copy();im.putalpha(m.resize(im.size,Image.Resampling.LANCZOS));return im
for n in ('bag','chest','cart','magic'):
 im=Image.open(base/'Sources/Crops'/f'{n}.png').convert('RGBA');alpha=Image.open(base/'Sources/Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS);im.putalpha(alpha);save(n,im,'Original RGB; Scenario isolated alpha, resampled only to original dimensions.')
# Original independent plaque; traced exterior excludes surrounding wood/arch.
b=(106,94,839,283);pts=[(472.0, 95.0), (477.0, 105.5), (481.5, 112.0), (499.5, 111.0), (512.5, 120.5), (521.0, 134.0), (529.0, 122.0), (544.0, 128.0), (551.5, 142.5), (565.0, 140.5), (584.5, 150.0), (595.5, 158.0), (634.0, 157.0), (646.5, 149.0), (657.5, 132.5), (666.5, 148.0), (678.0, 152.5), (686.5, 157.0), (692.0, 154.5), (699.0, 165.5), (714.0, 168.0), (731.0, 180.0), (738.0, 191.5), (747.0, 204.0), (754.0, 201.0), (762.0, 196.5), (769.5, 199.0), (774.5, 208.0), (780.0, 213.0), (789.0, 217.5), (775.5, 222.0), (769.5, 225.0), (774.0, 234.5), (767.0, 245.0), (759.5, 239.5), (749.5, 241.5), (734.5, 256.5), (717.5, 265.5), (699.0, 271.5), (694.0, 278.0), (684.0, 275.0), (677.0, 272.0), (259.5, 272.0), (255.0, 277.5), (247.0, 275.5), (244.5, 271.0), (228.5, 269.0), (214.5, 262.5), (202.5, 252.5), (193.5, 241.5), (182.5, 238.0), (176.0, 245.0), (169.0, 240.5), (165.5, 232.5), (169.5, 225.0), (158.0, 220.5), (149.5, 218.5), (162.0, 211.5), (165.0, 202.5), (173.0, 196.5), (181.0, 196.0), (186.5, 201.0), (187.5, 205.5), (195.5, 193.0), (204.5, 180.5), (217.0, 172.5), (232.0, 168.0), (245.0, 166.5), (249.0, 159.5), (260.0, 161.0), (267.0, 152.5), (274.0, 144.0), (278.5, 133.0), (287.0, 145.5), (289.0, 154.0), (301.5, 152.0), (312.0, 158.5), (332.5, 157.5), (347.5, 155.5), (363.5, 146.5), (378.5, 142.5), (393.5, 142.5), (404.5, 132.5), (421.5, 123.0), (430.5, 130.0), (435.5, 123.0), (445.5, 112.5), (460.5, 113.0), (463.0, 106.0)]
im=polygon(src.crop(b),[(x-b[0],y-b[1]) for x,y in pts]);save('title_ru',im,'Original independent opaque title plaque, traced alpha exterior; static Russian lettering retained.')
blank=im.copy();patch=ImageOps.fit(navy,(493,73));blank.paste(patch,(122,91));save('title_blank',blank,'Same original plaque; inner lettering area replaced by clean Scenario navy material for live localized heading.')
card=Image.open(base/'Sources/Scenario/card/booster_medallion_trial01.png').convert('RGBA');card=ImageOps.fit(card,(431,491));card=polygon(card,[(25,4),(405,4),(427,24),(427,466),(406,488),(25,488),(3,466),(3,27)]);save('card',card,'Scenario reconstructed empty card material/frame; exterior silhouette mask; opaque face retained.')
# Buttons/tabs are independent opaque UI surfaces; remove all price/labels.
for n,box,inner in [('tab',(254,284,488,376),(28,24,207,68)),('buy',(84,784,418,861),(46,15,288,64)),('quantity',(127,714,356,778),(15,6,214,59))]:
 im=src.crop(box);w,h=im.size
 if n=='buy':
  patch=ImageOps.fit(navy,(inner[2]-inner[0],inner[3]-inner[1]));a=np.asarray(patch).copy();grain=np.asarray(patch.convert('L'),dtype=float);grain=grain/max(1,grain.mean());a[:,:,:3]=(grain[:,:,None]*np.array([7,44,19])).clip(0,255);patch=Image.fromarray(a)
 elif n=='quantity':patch=ImageEnhance.Brightness(ImageOps.fit(wood,(inner[2]-inner[0],inner[3]-inner[1]))).enhance(.9)
 else:patch=ImageOps.fit(navy,(inner[2]-inner[0],inner[3]-inner[1]))
 # soft transition inside empty face, never cross the preserved perimeter
 mask=Image.new('L',patch.size);ImageDraw.Draw(mask).rectangle((4,4,patch.width-5,patch.height-5),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(2));im.paste(patch,inner[:2],mask)
 im=polygon(im,[(19,2),(w-18,2),(w-3,17),(w-3,h-18),(w-19,h-3),(19,h-3),(2,h-18),(2,19)])
 save(n,im,'Original independent UI perimeter; clean material replaces original lettering/numbers. Opaque face retained.')
# Separate original nav medallions; no scenery, labels, or notification dots.
for n,cx,cy,r in [('offers',102,1535,62),('gems',287,1531,69),('bundles',472,1530,69),('boosters',656,1530,68),('vip',843,1537,60)]:
 box=(cx-r,cy-r,cx+r,cy+r);im=src.crop(box);m=Image.new('L',(r*8,r*8));ImageDraw.Draw(m).ellipse((0,0,r*8-1,r*8-1),fill=255);mask=m.resize(im.size,Image.Resampling.LANCZOS); md=ImageDraw.Draw(mask); md.rectangle((0,1580-box[1],im.width,im.height),fill=0)
 if n=='offers':
  # Restore the small badge-overlap arc from the opposite side of this same medallion.
  patch=ImageOps.mirror(im).crop((83,0,124,31));im.paste(patch,(83,0))
 if n=='vip':
  patch=ImageOps.mirror(im).crop((79,0,120,26));im.paste(patch,(79,0))
 im.putalpha(mask);save('nav_'+n,im,'Original intentionally opaque medallion face, circular isolation excludes surrounding wood, caption and notification badge.')
for name,box in [('rail_left',(5,388,26,1400)),('rail_right',(917,388,938,1400)),('rail_top',(7,373,934,393)),('rail_bottom',(4,1394,936,1411)),('footer_ornament',(3,1627,938,1671))]:
 save(name,src.crop(box),'Original independent carved UI rail/panel surface, intentionally opaque; excludes all product/counter/text/scenery content.')
save('environment',ImageEnhance.Brightness(wood.resize((1024,1024),Image.Resampling.LANCZOS)).enhance(.56),'Reuse prior Scenario clean carved wood material, darkened for Shop.')
# Source long perimeter shell is UI-only. New clean navy centre; no resource values/portrait baked in.
im=ImageOps.fit(navy,(941,120));d=ImageDraw.Draw(im);d.rounded_rectangle((7,7,934,116),radius=28,outline=(139,93,28),width=3);d.rounded_rectangle((12,12,929,111),radius=24,outline=(78,52,21),width=2);save('hud',im,'Clean Scenario navy material with code-native restrained gold border; live separate HUD.')
im=ImageOps.fit(navy,(711,400));d=ImageDraw.Draw(im);d.rounded_rectangle((4,4,706,395),radius=24,outline=(139,93,28),width=3);d.rounded_rectangle((10,10,700,389),radius=20,outline=(78,52,21),width=2);save('empty_panel',im,'Clean navy material fitted without stretching, native gold border, live empty-category message.')
(base/'Sources/PreparationManifest.json').write_text(json.dumps(manifest,indent=2)+'\n')
# Alpha QA on black, white and saturated backgrounds, every transparent resource.
files=[p for p in out.glob('*.png') if Image.open(p).getextrema()[-1][0]<255]
for bg in ('black','white','#ff00aa'):
 sheet=Image.new('RGB',(1000,((len(files)+3)//4)*220),bg);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(files):
  a=Image.open(p);a.thumbnail((235,185));x=(i%4)*250;y=(i//4)*220;sheet.paste(a,(x,y+22),a);d.text((x,y),p.stem,fill='gray')
 sheet.save(base/'QA'/('Alpha_'+bg.replace('#','')+'.jpg'))
