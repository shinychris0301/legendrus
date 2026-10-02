from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np, json,hashlib,uuid,re
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');assets=[]
def save(n,im,notes):
 p=art/(n+'.png');im.save(p);m=Path(str(p)+'.meta')
 if not m.exists():m.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details_ru_r2.png.meta').read_text(),count=1))
 assets.append((n,im,notes))
def tile(donor,size):
 out=Image.new('RGBA',size)
 for y in range(0,size[1],donor.height):
  for x in range(0,size[0],donor.width):
   p=donor.transpose(Image.Transpose.FLIP_LEFT_RIGHT) if x//donor.width%2 else donor
   if y//donor.height%2:p=p.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
   out.paste(p,(x,y))
 return out
# Preserve each source ribbon and its trim; only lettering footprint receives same-ribbon lacquer.
for n,box,ribbon,letters,donor in [
 ('gold',(309,305,633,795),(309,558,633,623),(410,570,545,599),(360,581,399,595)),
 ('silver',(23,352,290,790),(23,559,290,620),(69,572,251,601),(60,582,67,595)),
 ('bronze',(638,357,884,790),(638,561,884,620),(677,573,840,601),(661,583,674,595))]:
 im=Image.open(art/('podium_'+n+'.png')).convert('RGBA');scale=im.width/(box[2]-box[0]);base=im.resize((box[2]-box[0],box[3]-box[1]),Image.Resampling.LANCZOS)
 rb=ref.crop(ribbon);region=(letters[0]-ribbon[0],letters[1]-ribbon[1],letters[2]-ribbon[0],letters[3]-ribbon[1]);mask=Image.new('L',rb.size);ImageDraw.Draw(mask).rectangle(region,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(.8))
 patch=tile(ref.crop(donor),rb.size)
 if n!='gold':patch=patch.filter(ImageFilter.GaussianBlur(3)) # Smooth repetition only on repaired inner faces.
 if n=='bronze':ImageDraw.Draw(mask).rectangle((750-ribbon[0],600-ribbon[1],758-ribbon[0],605-ribbon[1]),fill=255)
 rb=Image.composite(patch,rb,mask)
 # Existing Scenario-isolated podium alpha protects surrounding scenery.
 rimMask=Image.new('L',rb.size);draw=ImageDraw.Draw(rimMask)
 bottom=50 if n=='bronze' else 49
 draw.polygon([(0,0),(rb.width,0),(rb.width,bottom+12),(rb.width-20,bottom),(20,bottom),(0,bottom+12)],fill=255)
 # Restrict restoration to the ribbon silhouette; keep count surfaces and all other pixels unchanged.
 restored=im.copy();restored.paste(rb.resize((round(rb.width*scale),round(rb.height*scale)),Image.Resampling.LANCZOS),(round((ribbon[0]-box[0])*scale),round((ribbon[1]-box[1])*scale)),rimMask.resize((round(rb.width*scale),round(rb.height*scale)),Image.Resampling.LANCZOS));restored.putalpha(im.getchannel('A'))
 save('podium_'+n+'_r3',restored,dict(sourceBox=box,ribbonBox=ribbon,lettersBox=letters,donorBox=donor,repair='Same-ribbon lacquer replaces lettering; existing podium/alpha retained elsewhere'))
# Extract a continuous original reward enclosure. Native contents are separate sprites/text.
box=(38,684,280,783);im=ref.crop(box);w,h=im.size
# Trace outer gold perimeter and retain its corner/edge pixels.
a=Image.new('L',(w*4,h*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in [(2,13),(10,10),(11,2),(w-16,2),(w-13,8),(w-4,10),(w-2,17),(w-3,h-13),(w-12,h-3),(10,h-4),(2,h-12)]],fill=255);a=a.resize(im.size,Image.Resampling.LANCZOS)
# Dark lacquer from an empty source table surface covers the icon/count interiors.
surface=tile(ref.crop((280,1027,349,1039)),im.size)
# Individual faces preserve the original angled corner lines and separators.
for pts in [[(8,19),(16,13),(62,13),(70,19),(70,83),(65,88),(13,88),(7,82)],[(81,21),(87,14),(141,14),(149,23),(148,84),(141,89),(87,89),(80,84)],[(159,16),(220,16),(231,22),(231,80),(223,88),(159,88)]]:
 m=Image.new('L',im.size);ImageDraw.Draw(m).polygon(pts,fill=255);im=Image.composite(surface,im,m)
# The original rainbow/chest tips crossed into the top face. Clear only those inner-face remnants.
m=Image.new('L',im.size);d=ImageDraw.Draw(m);d.rectangle((28,7,68,20),fill=255);d.rectangle((153,10,230,27),fill=255);im=Image.composite(surface,im,m)
im.putalpha(a);save('reward_enclosure_r3',im,dict(sourceBox=box,repair='Original perimeter and inner chamfered rails retained; approved source lacquer clears icon/count faces'))
sheet=Image.new('RGB',(1050,len(assets)*270));d=ImageDraw.Draw(sheet)
for i,(n,im,_) in enumerate(assets):
 for j,c in enumerate(['black','white','#ff00dd']):
  d.rectangle((j*350,i*270,(j+1)*350,(i+1)*270),fill=c);thumb=im.copy();thumb.thumbnail((330,240));sheet.paste(thumb,(j*350+(350-thumb.width)//2,i*270),thumb)
 d.text((6,i*270+249),n,fill='#bbbbbb')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps([dict(name=n,notes=note,sha256=hashlib.sha256((art/(n+'.png')).read_bytes()).hexdigest()) for n,im,note in assets],indent=2))
