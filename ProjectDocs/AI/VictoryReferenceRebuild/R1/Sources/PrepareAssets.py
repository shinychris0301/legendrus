from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/VictoryReferenceR1');out.mkdir(parents=True,exist_ok=True)
ref=Image.open(b/'OriginalReference.png').convert('RGB');boxes=json.loads((b/'ExtractionPlan.json').read_text());audit={}
def poly(size,points):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in points],fill=255);return m.resize(size,Image.Resampling.LANCZOS)
def local(points,box):return [(x-box[0],y-box[1]) for x,y in points]
def source(n):
 crop=ref.crop(boxes[n]);raw=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').resize(crop.size,Image.Resampling.LANCZOS);a=np.array(raw.getchannel('A'));return np.array(crop),a
def save(n,rgb,a,original=None):
 im=Image.fromarray(np.dstack([rgb,a]).astype('uint8'));im.save(out/(n+'.png'))
 audit[n]={'size':list(im.size),'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest(),'rgbChangedPixels':int(np.any(rgb!=original,axis=2).sum()) if original is not None else None}
 for label,color in [('Black',(0,0,0,255)),('White',(255,255,255,255)),('Magenta',(255,0,180,255))]:
  bg=Image.new('RGBA',im.size,color);bg.alpha_composite(im);bg.convert('RGB').save(b/(n+'_Alpha'+label+'.png'))
# Clean navy donor from the unlettered left interior of the original title.
donor_array=np.array(ref.crop((148,883,314,947)))
bright=(donor_array.max(axis=2)>84).astype('uint8')*255
donor_array=cv2.inpaint(donor_array,cv2.dilate(bright,np.ones((3,3),np.uint8)),5,cv2.INPAINT_TELEA)
donor=Image.fromarray(donor_array).filter(ImageFilter.GaussianBlur(1.4))
def texture(size):
 # Mirrored repeats avoid hard seams; gradual sampling retains the dark source tone.
 tile=np.array(donor);tile=np.concatenate([tile,tile[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0)
 return np.tile(tile,(size[1]//tile.shape[0]+1,size[0]//tile.shape[1]+1,1))[:size[1],:size[0]]
def fill(rgb,rect,tex=True):
 x0,y0,x1,y1=rect
 mask=np.zeros(rgb.shape[:2],np.uint8);mask[y0:y1,x0:x1]=255
 if tex:
  blend=np.asarray(Image.fromarray(mask).filter(ImageFilter.GaussianBlur(2)))/255.
  return (rgb*(1-blend[:,:,None])+texture((rgb.shape[1],rgb.shape[0]))*blend[:,:,None]).astype('uint8')
 return cv2.inpaint(rgb,mask,4,cv2.INPAINT_TELEA)
# Title: preserve opaque navy face and original gold; remove only static letters for live localization.
rgb,a=source('title');original=rgb.copy();face=poly((rgb.shape[1],rgb.shape[0]),local([(192,108),(745,108),(746,213),(199,218)],boxes['title']));a=np.maximum(a,np.array(face))
# Remove wing fragments from this layer; Firebird is drawn in front.
keep=poly((rgb.shape[1],rgb.shape[0]),local([(154,18),(790,18),(790,119),(745,199),(727,240),(217,240),(196,201),(158,127)],boxes['title']));a=np.minimum(a,np.array(keep))
rgb=fill(rgb,(109,94,527,184));save('title',rgb,a,original)
# Firebird: keep original source colour; separate attached title fragments and stars.
rgb,a=source('firebird');original=rgb.copy();mask=Image.fromarray(a);d=ImageDraw.Draw(mask)
d.rectangle((271-boxes['firebird'][0],112-boxes['firebird'][1],687-boxes['firebird'][0],198-boxes['firebird'][1]),fill=0)
d.polygon(local([(158,70),(790,70),(776,111),(172,111)],boxes['firebird']),fill=0)
d.polygon(local([(705,85),(809,85),(793,144),(779,169),(748,181),(724,195),(713,209),(700,204)],boxes['firebird']),fill=0)
d.polygon(local([(298,213),(322,215),(331,246),(314,246)],boxes['firebird']),fill=0)
# Remove all title frame alpha remaining between the upward wings.
d.polygon(local([(309,205),(646,205),(618,247),(522,247),(494,238),(410,244),(335,246)],boxes['firebird']),fill=0)
# Star silhouette exclusion includes anti-aliased edges; covered feathers cannot be recovered.
for pts in [[(474,598),(508,667),(582,678),(526,727),(544,805),(474,767),(404,809),(418,730),(369,681),(441,668)],[(274,642),(303,704),(370,715),(321,761),(333,833),(274,798),(213,833),(224,766),(178,714),(245,701)],[(675,646),(704,706),(772,717),(724,766),(734,833),(675,799),(612,834),(624,767),(579,715),(646,704)]]:
 d.polygon(local(pts,boxes['firebird']),fill=0)
a=np.array(mask)
components,labels,stats,_=cv2.connectedComponentsWithStats((a>24).astype('uint8'),8)
if components>1:
 keep=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);support=cv2.dilate((labels==keep).astype('uint8'),np.ones((3,3),np.uint8));a=a*support
a=np.minimum(a,np.array(poly(mask.size,[(0,0),(mask.width,0),(mask.width,mask.height),(0,mask.height)])))
save('firebird',rgb,a,original)
# Remove Firebird pixels from the separately isolated title layer.
title=Image.open(out/'title.png');ta=np.array(title.getchannel('A'));full=Image.new('L',ref.size);full.paste(Image.fromarray(a),boxes['firebird'][:2]);ta=np.minimum(ta,255-np.asarray(full.crop(boxes['title'])));save('title',np.array(title)[:,:,:3],ta,np.array(ref.crop(boxes['title'])))
# The main panel is its original carved shell and opaque navy surface, cleared of sample content.
rgb,a=source('panel');original=rgb.copy();a[24:-18,28:-28]=255
rgb=fill(rgb,(53,16,710,129));rgb=fill(rgb,(28,145,735,512))
# Gold celebration rays outside the source panel belong to stars, not its shell.
a[:8,65:365]=0;a[:8,402:695]=0
save('panel',rgb,a,original)
# Isolated frame faces preserve original border pixels; all samples are removed.
for n,rect in [('card',(13,12,119,158)),('resource_card',(13,12,207,113))]:
 rgb,a=source(n);original=rgb.copy();rgb=fill(rgb,rect)
 # Pixa selected the hammer instead of its frame on the booster crop. Recover the
 # unobstructed original frame with a bounded opaque panel silhouette.
 w,h=rgb.shape[1],rgb.shape[0];a=np.array(poly((w,h),[(13,1),(w-13,1),(w-1,13),(w-1,h-13),(w-13,h-1),(13,h-1),(1,h-13),(1,13)]))
 save(n,rgb,a,original)
# Actions retain their native shape and original frame/icon. Lettering is live RU/EN.
for n,rect in [('continue',(90,28,527,96)),('video',(204,25,670,111))]:
 rgb,a=source(n);original=rgb.copy();rgb=fill(rgb,rect,False)
 donor_box=(228,1393,266,1447) if n=='continue' else (285,1514,310,1594)
 clean=np.array(ref.crop(donor_box));clean=np.concatenate([clean,clean[:,::-1]],axis=1)
 clean=np.concatenate([clean,clean[::-1]],axis=0)
 patched=np.tile(clean,(rgb.shape[0]//clean.shape[0]+1,rgb.shape[1]//clean.shape[1]+1,1))[:rgb.shape[0],:rgb.shape[1]]
 face=Image.new('L',(rgb.shape[1],rgb.shape[0]));ImageDraw.Draw(face).rounded_rectangle((rect[0]-8,rect[1]-6,rect[2]+8,rect[3]+6),radius=8,fill=255)
 blend=np.array(face.filter(ImageFilter.GaussianBlur(5)))/255.
 rgb=(rgb*(1-blend[:,:,None])+patched*blend[:,:,None]).astype('uint8')
 # Ensure source green/blue face remains opaque; original alpha isolation often holes letters.
 a[rect[1]:rect[3],rect[0]:rect[2]]=255;save(n,rgb,a,original)
for n in ['footer','coin','gem']:
 rgb,a=source(n)
 if n=='footer':a[:11]=0
 save(n,rgb,a,rgb.copy())
rgb,a=source('star');save('star',rgb,a,rgb.copy())
# Empty slot retains the original contour and rim, with a quiet dark face.
core=cv2.erode(a,np.ones((9,9),np.uint8));t=core.astype(float)/255
empty=(rgb*(1-t[:,:,None])+np.array([25,28,28])*t[:,:,None]).astype('uint8');save('star_empty',empty,a,rgb)
# Generated hidden scenery remains independent; visible-source fidelity is not claimed here.
env=Image.open(b/'Scenario/environment/booster_medallion_trial01.png').convert('RGB');env.save(out/'environment.png')
audit['environment']={'size':list(env.size),'method':'Scenario hidden-scenery reconstruction; output RGB copied unchanged'}
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2))
print('Prepared',len(audit),'assets')
