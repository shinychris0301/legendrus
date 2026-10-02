from pathlib import Path
from PIL import Image,ImageDraw
import json,hashlib,uuid,re
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');assets=[]
def save(n,im,note):
 p=art/(n+'.png');im.save(p);m=Path(str(p)+'.meta')
 if not m.exists():m.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details_ru_r2.png.meta').read_text(),count=1))
 assets.append((n,im,note))
def tile(donor,size):
 im=Image.new('RGBA',size)
 for y in range(0,size[1],donor.height):
  for x in range(0,size[0],donor.width):
   p=donor.transpose(Image.Transpose.FLIP_LEFT_RIGHT) if x//donor.width%2 else donor
   if y//donor.height%2:p=p.transpose(Image.Transpose.FLIP_TOP_BOTTOM)
   im.paste(p,(x,y))
 return im
def alpha(size,pts):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in pts],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
donorBox=(625,850,664,919);donor=ref.crop(donorBox)
box=(25,796,916,1462);im=ref.crop(box);w,h=im.size;surface=tile(donor,im.size)
# Clear every header/row content footprint, retain the continuous original outer enclosure.
im.paste(surface.crop((12,10,w-12,h-11)),(12,10))
# Original clear header divider; avoid the guild crest directly beneath it.
im.paste(ref.crop((39,838,901,843)),(14,42))
# Clear clipped crest fragments from the inner edge of the original lower border.
im.paste(ref.crop((224,1449,325,1455)),(85,h-13))
# Outer frame had a different highlighted-row height and footer objects touching its bottom.
# Continue clean original rails across those obsolete overlaps.
im.paste(tile(ref.crop((25,961,37,984)),(12,122)),(0,302))
im.paste(tile(ref.crop((904,961,916,984)),(12,122)),(w-12,302))
for left,right in [(42,193),(429,462),(775,842)]:
 im.paste(tile(ref.crop((240,1450,350,1462)),(right-left,12)),(left,h-12))
im.putalpha(alpha(im.size,[(0,13),(12,0),(w-13,0),(w-1,13),(w-1,h-13),(w-13,h-1),(12,h-1),(0,h-13)]))
save('table_panel_r4',im,dict(sourceBox=box,donorBox=donorBox,repair='Original outer enclosure/header divider with opaque source-navy surface; baked table data removed'))
# Thin original separator from a content-free stretch; ends sit within the outer frame.
box=(207,924,889,929);im=ref.crop(box);save('row_divider_r4',im,dict(sourceBox=box,repair='Original horizontal divider, no generated line'))
box=(23,1103,917,1206);im=ref.crop(box);w,h=im.size;surface=tile(donor,im.size)
im.paste(surface.crop((13,12,w-13,h-12)),(13,12))
# Original crest and separate title plaque overlap the top rail; continue its clean neighbouring span.
top=ref.crop((224,1103,334,1115));bottom=ref.crop((226,1194,336,1206))
for left,right in [(85,183),(334,557)]:
 im.paste(tile(top,(right-left,12)),(left,0))
im.paste(tile(bottom,(110,12)),(83,h-12))
im.putalpha(alpha(im.size,[(1,12),(11,3),(w-13,3),(w-2,12),(w-2,h-13),(w-12,h-3),(11,h-3),(1,h-13)]))
save('row_highlight_r4',im,dict(sourceBox=box,donorBox=donorBox,repair='Original gold highlight perimeter; opaque navy face and source-rail continuations under former crest/plaque'))
sheet=Image.new('RGB',(1080,750));d=ImageDraw.Draw(sheet)
for i,(n,im,_) in enumerate(assets):
 for j,c in enumerate(['black','white','#ff00dd']):
  d.rectangle((j*360,i*250,(j+1)*360,(i+1)*250),fill=c);thumb=im.copy();thumb.thumbnail((342,222));sheet.paste(thumb,(j*360+(360-thumb.width)//2,i*250+(222-thumb.height)//2),thumb)
 d.text((6,i*250+231),n,fill='#bbbbbb')
sheet.save(b/'QA/AssetAlpha.png');(b/'Sources/AssetProvenance.json').write_text(json.dumps([dict(name=n,notes=note,sha256=hashlib.sha256((art/(n+'.png')).read_bytes()).hexdigest()) for n,im,note in assets],indent=2))
