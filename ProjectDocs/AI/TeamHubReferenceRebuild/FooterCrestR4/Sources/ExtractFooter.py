from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np
import hashlib,json,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamHubAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');assets=[];records=[]
def poly(size,pts):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in pts],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def save(n,im,box,repair):
 p=art/(n+'.png');im.save(p);m=Path(str(p)+'.meta')
 if not m.exists():m.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'power.png.meta').read_text(),count=1))
 assets.append((n,im));records.append(dict(asset=str(p),sourceBox=box,repair=repair,sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
box=(82,1509,204,1637);im=ref.crop(box)
mask=poly(im.size,[(7,6),(22,10),(43,6),(61,0),(80,6),(105,10),(118,5),(117,57),(110,80),(97,103),(77,119),(61,126),(38,118),(19,98),(8,76),(1,56),(3,24)])
im.putalpha(mask);save('footer_crest_r4',im,box,'Original opaque shield; traced outer gold perimeter')
box=(170,1518,844,1617);im=ref.crop(box);w,h=im.size
# Source red lacquer to the right of lettering; tile symmetrically for cleared English face/info backing.
donor=ref.crop((690,1535,774,1599));tile=Image.new('RGBA',im.size)
for y in range(0,h,donor.height):
 for x in range(0,w,donor.width):
  patch=donor.transpose(Image.Transpose.FLIP_LEFT_RIGHT) if (x//donor.width)%2 else donor
  if (y//donor.height)%2:patch=patch.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
  tile.paste(patch,(x,y))
# The crest hides the plaque's left end. Reconstruct only that hidden part.
im.paste(tile.crop((0,10,31,h-10)),(0,10));im.paste(im.crop((40,0,71,10)),(0,0));im.paste(im.crop((40,h-10,71,h)),(0,h-10))
im.paste(tile.crop((610,23,654,76)),(610,23))
mask=poly(im.size,[(0,2),(w-17,2),(w-2,17),(w-2,h-18),(w-18,h-2),(0,h-2)])
im.putalpha(mask);save('motto_ru_r4',im,box,'Original fixed Russian motto/frame; source red lacquer clears isolated info-button footprint and crest-hidden left end')
blank=im.copy();blank.paste(tile.crop((61,26,570,77)),(61,26));blank.putalpha(mask)
save('motto_blank_r4',blank,box,'Same source panel; source red lacquer clears Russian letters for live English')
box=(784,1549,827,1593);im=ref.crop(box);a=Image.new('L',(43*4,44*4));ImageDraw.Draw(a).ellipse((2*4,1*4,41*4,43*4),fill=255);im.putalpha(a.resize(im.size,Image.Resampling.LANCZOS));save('guild_info_r4',im,box,'Original circular information icon, independent interactive sprite')
sheet=Image.new('RGB',(960,len(assets)*190));d=ImageDraw.Draw(sheet)
for i,(name,im) in enumerate(assets):
 for j,c in enumerate(['black','white','#ff00dd']):
  t=Image.new('RGBA',(320,170),c);s=im.copy();s.thumbnail((310,160));t.alpha_composite(s,((320-s.width)//2,(170-s.height)//2));sheet.paste(t.convert('RGB'),(320*j,190*i))
 d.text((5,190*i+173),name,fill='white')
sheet.save(b/'QA/AssetAlpha.png');(b/'Sources/AssetProvenance.json').write_text(json.dumps(dict(sourceSHA256=hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),scenarioJobs=0,assets=records,limitations='Traced edges and hidden red surfaces are reconstructed; footer group scaled to preserve navigation.'),indent=2))
