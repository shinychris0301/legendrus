from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/VictoryReferenceR1');q=b.parent/'QA'
ref=Image.open(b.parent.parent/'R1/Sources/OriginalReference.png').convert('RGB')
boxes=json.loads((b.parent.parent/'R1/Sources/ExtractionPlan.json').read_text())
gen=Image.open(b/'Scenario/faces/booster_medallion_trial01.png').convert('RGB').resize((763,795),Image.Resampling.LANCZOS)
navy=gen.crop((70,150,695,480));audit={}
def patch(a,texture,rect,radius=3):
 m=Image.new('L',(a.shape[1],a.shape[0]));ImageDraw.Draw(m).rectangle(rect,fill=255)
 # Feather inward; never paint outside the declared support.
 f=np.minimum(np.array(m),np.array(m.filter(ImageFilter.GaussianBlur(radius))))/255.
 tex=np.array(texture.resize(m.size,Image.Resampling.LANCZOS));a[:,:,:3]=(a[:,:,:3]*(1-f[:,:,None])+tex*f[:,:,None]).astype('uint8')
def gold(a,src,rect):
 x0,y0,x1,y1=rect;r=src[y0:y1,x0:x1].astype(float)
 # Source gold only. Opaque navy is supplied by the clean face, not a crop rectangle.
 alpha=np.clip((r[:,:,0]-r[:,:,2]-9)/16,0,1)*np.clip((r[:,:,0]-35)/25,0,1)
 old=a[y0:y1,x0:x1,:3];a[y0:y1,x0:x1,:3]=(old*(1-alpha[:,:,None])+r*alpha[:,:,None]).astype('uint8')
for n in ['title','panel','resource_card','card','continue','video']:
 old=np.array(Image.open(b/'BeforeArt'/f'{n}.png').convert('RGBA'));a=old.copy();src=np.array(ref.crop(boxes[n]))
 if n=='panel':
  patch(a,navy,(53,16,710,129));patch(a,navy,(28,145,735,512))
  for x0,y0,x1,y1 in [(349,975,395,1002),(544,975,590,1002),(349,1138,395,1164),(544,1138,590,1164)]:gold(a,src,(x0-90,y0-828,x1-90,y1-828))
 elif n=='title':
  m=Image.new('L',(a.shape[1],a.shape[0]));ImageDraw.Draw(m).polygon([(43,89),(588,89),(573,191),(72,195)],fill=255)
  f=np.array(m.filter(ImageFilter.GaussianBlur(4)))/255.
  tex=np.array(navy.resize(m.size,Image.Resampling.LANCZOS));a[:,:,:3]=(a[:,:,:3]*(1-f[:,:,None])+tex*f[:,:,None]).astype('uint8')
 elif n in ('card','resource_card'):
  patch(a,navy,(13,12,a.shape[1]-13,a.shape[0]-13))
  if n=='card':gold(a,src,(8,122,123,128))
 else:
  texture=gen.crop((170,576,610,640) if n=='continue' else (220,708,690,772))
  patch(a,texture,(82,22,535,102) if n=='continue' else (196,19,678,117),4)
 Image.fromarray(a).save(out/f'{n}.png')
 changed=np.any(a!=old,axis=2);Image.fromarray((changed*255).astype('uint8')).save(b/f'{n}_ChangedMask.png')
 audit[n]={'sha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'changedPixels':int(changed.sum()),'unchangedPixels':int((~changed).sum()),'alphaUnchanged':bool(np.array_equal(a[:,:,3],old[:,:,3]))}
files=[out/f'{n}.png' for n in audit]
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(255,0,180))]:
 sheet=Image.new('RGB',(960,660),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(files):
  im=Image.open(p);im.thumbnail((456,190),Image.Resampling.LANCZOS);x=i%2*480+(480-im.width)//2;y=i//2*220+25;sheet.paste(im,(x,y),im);d.text((i%2*480+12,i//2*220+7),p.stem,fill=(128,128,128))
 sheet.save(q/f'Alpha{label}.png')
(b/'AssetProvenance.json').write_text(json.dumps({'method':'One Scenario bounded face repair; original alpha and exterior borders preserved; source heading flourishes and booster divider restored locally. Interior reconstructed, not exact original pixels.','assets':audit},indent=2))
print(json.dumps(audit,indent=2))
# User follow-up: keep original Russian lettering together with its panel/button.
# Existing Scenario alpha is reused; no replacement paid isolation is necessary.
lettering={}
for base in ['title','continue','panel','video']:
 a=np.array(Image.open(out/f'{base}.png').convert('RGBA'));src=np.array(ref.crop(boxes[base]))
 if base in ('title','continue'):a[:,:,:3]=src
 elif base=='panel':
  for rect in [(340,21,421,51),(312,145,447,176),(313,308,449,337)]:gold(a,src,rect)
 else:
  # Preserve WATCH AD and x2; keep COINS live rather than promise all rewards.
  gold(a,src,(225,74,553,116));gold(a,src,(582,34,674,114))
 p=out/f'{base}_ru.png';Image.fromarray(a).save(p)
 lettering[base]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'sourceBox':boxes[base],'method':'Original RGB with existing Scenario alpha' if base in ('title','continue') else 'Original gold lettering composited onto repaired face; variable score and coin-only bonus remain live'}
(b/'LetteringProvenance.json').write_text(json.dumps(lettering,indent=2))
# Latest user request: all original ad lettering, without partial rewording.
a=np.array(Image.open(out/'video.png').convert('RGBA'));a[:,:,:3]=np.array(ref.crop(boxes['video']));Image.fromarray(a).save(out/'video_ru.png')
lettering['video']['method']='Entire original button RGB with existing Scenario alpha. Coin-only explanatory text is a separate live line.'
lettering['video']['sha256']=hashlib.sha256((out/'video_ru.png').read_bytes()).hexdigest()
(b/'LetteringProvenance.json').write_text(json.dumps(lettering,indent=2))
# Warm optical bloom derived from the isolated source silhouette; not new bird art.
mask=Image.new('L',(941,828));bird=Image.open(out/'firebird.png').convert('RGBA');mask.paste(bird.getchannel('A'),(12,70))
tight=np.array(mask.filter(ImageFilter.GaussianBlur(3)),dtype=float);medium=np.array(mask.filter(ImageFilter.GaussianBlur(13)),dtype=float);wide=np.array(mask.filter(ImageFilter.GaussianBlur(32)),dtype=float)
alpha=np.clip(.5*tight+.75*medium+.5*wide,0,220).astype('uint8')
rgb=np.zeros((828,941,3),dtype='uint8');rgb[:]=[255,145,24];Image.fromarray(np.dstack([rgb,alpha])).save(out/'firebird_glow.png')
(b/'GlowProvenance.json').write_text(json.dumps({'method':'Gaussian optical glow derived from existing Scenario-isolated Firebird alpha, unchanged bird RGB; warm constant glow color','size':[941,828],'blurRadii':[3,13,32],'weights':[.5,.75,.5],'maxAlpha':220,'rgbaSHA256':hashlib.sha256((out/'firebird_glow.png').read_bytes()).hexdigest()},indent=2))
