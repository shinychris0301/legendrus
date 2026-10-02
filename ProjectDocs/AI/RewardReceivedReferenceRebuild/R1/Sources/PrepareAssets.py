from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,cv2,json,hashlib,uuid
b=Path(__file__).resolve().parent;src=Image.open(b/'OriginalReference.png').convert('RGBA');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/RewardReceivedAligned');boxes=json.loads((b/'Boxes.json').read_text());gen=Image.open(b/'Scenario/repair/booster_medallion_trial01.png').convert('RGBA');records={}
def poly(size,points):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def save(n,im,method):
 p=out/(n+'.png');im.save(p);meta=p.with_suffix('.png.meta')
 if not meta.exists():
  t=(out/'panel.png.meta').read_text();g=next(x for x in t.splitlines() if x.startswith('guid:'));meta.write_text(t.replace(g,'guid: '+uuid.uuid4().hex))
 records[n]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method}
panel=src.crop(boxes['panel']);alpha=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(panel.size,Image.Resampling.LANCZOS)
ImageDraw.Draw(alpha).polygon([(42,137),(105,111),(197,95),(599,95),(704,109),(756,137),(756,861),(720,916),(702,937),(99,937),(65,921),(40,868)],fill=255)
material=gen.crop((20,10,815,1111)).resize(panel.size,Image.Resampling.LANCZOS)
m=poly(panel.size,[(40,138),(86,113),(140,100),(656,100),(711,116),(754,144),(754,863),(735,910),(694,935),(645,940),(645,981),(154,981),(154,940),(91,935),(55,910),(40,873)])
m=m.filter(ImageFilter.GaussianBlur(1));panel=Image.composite(material,panel,m)
# The original button's bottom crest belongs to the button sprite, not the empty panel.
ImageDraw.Draw(alpha).rectangle((156,949,643,981),fill=0);aa=np.asarray(alpha).copy();nn,ll,st,_=cv2.connectedComponentsWithStats(np.uint8(aa>8));largest=1+np.argmax(st[1:,cv2.CC_STAT_AREA]);aa[ll!=largest]=0;alpha=Image.fromarray(aa);panel.putalpha(alpha);save('panel_r2',panel,'Original source outer perimeter/Pixa alpha; bounded Scenario empty navy face; duplicate button crest cleared.')
chest=src.crop(boxes['chest']);ca=Image.open(b/'Scenario/chest/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(chest.size,Image.Resampling.LANCZOS);chest.putalpha(ca);save('chest_r2',chest,'Original chest, coins, gems and inner light RGB with fresh Scenario Pixa alpha.')
# Preserve source rays/sparks separately; reject blue/dark panel texture and fade crop boundaries.
glow=src.crop((124,580,818,900));a=np.asarray(glow).astype(float);r,g,blue=a[:,:,:3].transpose(2,0,1);warm=np.clip((r-blue*1.45-12)/145,0,1)*np.clip((g-blue*1.1)/70,0,1);h,w=warm.shape;yy,xx=np.mgrid[:h,:w];edge=np.clip(np.minimum.reduce([xx/20,(w-1-xx)/20,yy/12,(h-1-yy)/12]),0,1)
bright=np.clip((np.minimum(r,g)-145)/95,0,1)*np.clip((r-blue*.85-5)/80,0,1);opaque=Image.new('L',glow.size);opaque.paste(ca,(42,4));glowAlpha=np.maximum(warm*220,bright*255)*(1-np.asarray(opaque)/255.)*edge;glow.putalpha(Image.fromarray(np.uint8(glowAlpha)));save('chest_glow_r2',glow,'Source warm rays and sparks separated from chest and navy material by chroma/luminance mask, faded crop perimeter.')
title=src.crop(boxes['title']);a=np.asarray(title).astype(float);r,g,blue=a[:,:,:3].transpose(2,0,1);mask=np.uint8((r>75)&(r>g*1.04)&(g>blue*1.15))*255;n,labels,stats,_=cv2.connectedComponentsWithStats(mask)
keep=np.zeros_like(mask)
for i in range(1,n):
 if stats[i,cv2.CC_STAT_AREA]>12 and stats[i,cv2.CC_STAT_HEIGHT]>8:keep[labels==i]=255
keep=cv2.dilate(keep,np.ones((3,3),np.uint8));title.putalpha(Image.fromarray(keep).filter(ImageFilter.GaussianBlur(.3)));save('title_ru_r2',title,'Original Russian heading RGB; connected gold-letter masks retain embossing without navy crop.')
button=src.crop(boxes['button']);ba=poly(button.size,[(5,62),(26,48),(39,30),(58,15),(206,14),(224,7),(234,14),(384,14),(404,30),(417,47),(444,62),(419,77),(412,95),(392,104),(257,107),(239,118),(226,129),(210,119),(193,107),(65,105),(41,94),(30,77)])
button.putalpha(ba);save('button_ru_r2',button,'Original ЗАБРАТЬ button including sapphire lower ornament and source text, local contour alpha.')
blank=button.copy();patch=gen.crop((50,1155,501,1293)).resize(button.size);m=Image.new('L',button.size);ImageDraw.Draw(m).rectangle((93,30,355,95),fill=255);blank=Image.composite(patch,blank,m.filter(ImageFilter.GaussianBlur(1)));blank.putalpha(ba);save('button_blank_r2',blank,'Source button with bounded Scenario lettering repair for live English/empty receipt Close.')
card=src.crop(boxes['card']);ac=poly(card.size,[(0,17),(14,0),(110,1),(121,14),(121,116),(109,129),(12,129),(0,115)]);m=Image.new('L',card.size);ImageDraw.Draw(m).rectangle((10,14,112,119),fill=255);card=Image.composite(gen.crop((598,1155,722,1286)).resize(card.size),card,m.filter(ImageFilter.GaussianBlur(1)));card.putalpha(ac);save('card_r2',card,'Original fine gold card border with bounded Scenario icon/count clearing.')
icons={'coin':((155,982,252,1062),[(1,60),(15,47),(17,31),(33,19),(44,17),(47,10),(66,1),(80,1),(89,11),(88,46),(94,59),(86,73),(52,79),(20,76),(4,68)]),'gem':((291,981,388,1064),[(2,32),(24,5),(67,2),(91,26),(96,35),(49,82)]),'energy':((437,979,497,1062),[(17,1),(59,0),(32,34),(59,33),(12,82),(24,49),(0,49)])}
for n,(box,pts) in icons.items():
 im=src.crop(box);alpha=poly(im.size,pts)
 if n=='coin':
  arr=np.asarray(im).astype(float);mask=np.uint8((arr[:,:,0]>arr[:,:,2]*1.4)&(arr[:,:,0]>45))*255
  mask=cv2.morphologyEx(mask,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8));contours,_=cv2.findContours(mask,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);mask[:]=0;cv2.drawContours(mask,[max(contours,key=cv2.contourArea)],-1,255,-1);alpha=Image.fromarray(mask).filter(ImageFilter.GaussianBlur(.3))
 im.putalpha(alpha);save(n+'_r2',im,'Original '+n+' reward icon RGB, contour isolated from original reward card.')
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'ScenarioJobs':3,'ScenarioCU':19,'generatedAtlasMeasuredRegions':{'panel':[20,10,815,1111],'button':[50,1155,501,1293],'card':[598,1155,722,1286]}},indent=2))
for n,c in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(1600,1050),c)
 for key,xy in [('panel_r2',(0,0)),('chest_r2',(830,100)),('chest_glow_r2',(815,440)),('title_ru_r2',(830,0)),('button_ru_r2',(825,790)),('card_r2',(1310,790)),('coin_r2',(840,950)),('gem_r2',(960,950)),('energy_r2',(1080,950))]:
  im=Image.open(out/(key+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+n+'.png'))
