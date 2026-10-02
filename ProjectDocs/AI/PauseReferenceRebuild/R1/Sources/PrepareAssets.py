from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/PauseAligned');ref=Image.open(b/'OriginalReference.png').convert('RGB');audit={}
def save(name,rgb,alpha,box,repair=None):
 im=Image.fromarray(np.dstack([rgb,alpha]).astype('uint8'));p=out/(name+'.png');im.save(p)
 original=np.array(ref.crop(box));changed=np.any(rgb!=original,axis=2);Image.fromarray((changed*255).astype('uint8')).save(b/(name+'_RGBRepairMask.png'))
 audit[name]={'sourceBox':box,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'changedRGBPixels':int(changed.sum()),'repair':repair}
def gold(rgb):
 r,g,bb=[rgb[:,:,i].astype(float) for i in range(3)]
 return (np.clip(np.minimum.reduce([(r-bb-8)/20,(g-bb-3)/13,(r-26)/30]),0,1)*255).astype('uint8')
box=[174,406,770,1430];rgb=np.array(ref.crop(box));raw=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').resize((596,1024));a=np.array(raw.getchannel('A'))
face=Image.open(b/'Scenario/faces/booster_medallion_trial01.png').convert('RGB').resize((596,1024),Image.Resampling.LANCZOS)
m=Image.new('L',(596,1024));ImageDraw.Draw(m).polygon([(34,47),(563,47),(563,942),(523,982),(73,982),(34,942)],fill=255);m=m.filter(ImageFilter.GaussianBlur(3));f=np.array(m)/255.
rgb=(rgb*(1-f[:,:,None])+np.array(face)*f[:,:,None]).astype('uint8');a=np.maximum(a,np.array(m))
title=Image.new('L',ref.size);title.paste(Image.open(out/'title_ru.png').getchannel('A'),(160,253));a=np.minimum(a,255-np.array(title.crop(box)))
save('panel_r2',rgb,a,box,'Scenario alpha plus bounded blank interior from one Scenario repair; remove attached title alpha')
m.save(b/'PanelFaceMask.png')
# Borders and fixed captions are isolated original gold pixels, with transparent navy gaps.
box=[252,486,690,706];rgb=np.array(ref.crop(box));a=gold(rgb);keep=np.zeros(a.shape,np.uint8);keep[:10,:]=255;keep[-10:,:]=255;keep[:,:10]=255;keep[:,-10:]=255;keep[:14,53:383]=0;a=np.minimum(a,keep);save('goals_r2',rgb,a,box,'Only source thin outer gold border; interior shows continuous main panel material')
for name,box in [('goals_heading_ru_r2',[305,468,635,514]),('boosters_heading_ru_r2',[268,721,670,763]),('instructions_ru_r2',[284,634,665,693])]:
 rgb=np.array(ref.crop(box));a=gold(rgb);save(name,rgb,a,box,'Gold-on-navy alpha extraction; original RGB unchanged')
# English keeps original side flourishes around the live booster heading.
box=[268,721,670,763];rgb=np.array(ref.crop(box));a=gold(rgb);a[:,103:290]=0;save('boosters_flourishes_r2',rgb,a,box,'Remove lettering through alpha only')
box=[305,477,635,507];rgb=np.array(ref.crop(box));a=gold(rgb);a[:,36:282]=0;save('goals_flourishes_r2',rgb,a,box,'Original flanking ornaments; no lettering')
raw=Image.open(b/'Scenario/boosters/booster_medallion_trial01.png').convert('RGBA').resize((494,139),Image.Resampling.LANCZOS)
for i,n in enumerate(['mace','firebird','shield','potion']):
 box=[229+i*124,768,345+i*124,901];rgb=np.array(ref.crop(box));a=np.array(raw.getchannel('A').crop((3+i*124,3,119+i*124,136)))
 face=Image.new('L',(116,133));d=ImageDraw.Draw(face);d.rounded_rectangle((11,10,103,112),radius=12,fill=255);d.ellipse((74,90,104,121),fill=255);a=np.maximum(a,np.array(face))
 # Reuse already repaired empty quantity face, keep original artwork and ring pixels.
 old=np.array(Image.open(out/(n+'.png')).convert('RGB'));yy,xx=np.mgrid[0:133,0:116];m=((xx-89)/14)**2+((yy-106)/16)**2<=1;rgb[m]=old[m]
 save(n+'_r2',rgb,a,box,'Scenario silhouette; opaque card face restored; existing live-count face reused')
(b/'AssetProvenance.json').write_text(json.dumps({'referenceSHA256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'assets':audit},indent=2))
for name,bg in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(235,0,160))]:
 im=Image.new('RGB',(1080,1050),bg);d=ImageDraw.Draw(im);panel=Image.open(out/'panel_r2.png');panel.thumbnail((500,1024));im.paste(panel,(0,0),panel)
 for k,(n,y) in enumerate([('goals_r2',0),('goals_heading_ru_r2',240),('goals_flourishes_r2',300),('boosters_heading_ru_r2',340),('boosters_flourishes_r2',400),('instructions_ru_r2',470)]):
  asset=Image.open(out/(n+'.png'));im.paste(asset,(580,y),asset)
 for i,n in enumerate(['mace','firebird','shield','potion']):
  asset=Image.open(out/(n+'_r2.png'));im.paste(asset,(560+i*124,600),asset)
 im.save(b.parent/'QA'/('Alpha'+name+'.png'))
print('Prepared',len(audit),'versioned assets')
