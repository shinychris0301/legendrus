"""Preserve source roster ornament/medal RGB; reconstruct only occupied row faces."""
from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np
import hashlib,json,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamHubAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');records=[];assets=[]
def polygon(size,pts):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in pts],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def save(name,im,box,repair):
 p=art/(name+'.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'power.png.meta').read_text(),count=1))
 records.append(dict(asset=str(p),sourceBox=box,repair=repair,sha256=hashlib.sha256(p.read_bytes()).hexdigest()));assets.append((name,im))
# These medals include their original numerals, with no scenery outside the perimeter.
for i,y in enumerate([1077,1168,1258]):
 box=(51,y,132,y+81);im=ref.crop(box)
 pts=[(40,1),(46,12),(55,14),(63,11),(64,20),(71,28),(70,34),(79,40),(68,46),(68,56),(61,61),(61,69),(50,68),(41,80),(34,69),(24,69),(23,63),(13,58),(14,49),(2,41),(14,33),(13,24),(21,19),(24,11),(32,14)]
 mask=np.array(polygon(im.size,pts)).astype(float);rgb=np.array(im)[:,:,:3].astype(float);yy,xx=np.mgrid[:81,:81];core=(xx-40)**2+(yy-40)**2<=28**2;edge=np.clip((rgb.max(2)-45)/35,0,1);edge[core]=1;im.putalpha(Image.fromarray(np.rint(mask*edge).astype('uint8')));save('guild_medal_'+str(i)+'_r3',im,box,'Original RGB and numeral, hand-traced perimeter; no regeneration')
# Source row profile differs at the contribution end. Preserve the asymmetric carving.
for name,y,donorbox in [('leader',1073,(435,1089,631,1141)),('member',1164,(435,1178,631,1230))]:
 box=(39,y,890,y+85);im=ref.crop(box);w,h=im.size
 donor=ref.crop(donorbox);face=Image.new('RGBA',im.size)
 # Mirror adjacent tiles to avoid a hard texture seam.
 for yy in range(0,h,donor.height):
  for xx in range(0,w,donor.width):
   tile=donor.transpose(Image.Transpose.FLIP_LEFT_RIGHT) if (xx//donor.width)%2 else donor
   face.paste(tile,(xx,yy))
 clear=Image.new('L',im.size);d=ImageDraw.Draw(clear);d.rectangle((18,11,w-18,h-12),fill=255)
 im.paste(face,(0,0),clear)
 # The contribution compartment stays dark even on the bronze leader row.
 if name=='leader':
  dark=ref.crop((842,y+18,872,y+66))
  for x in range(641,w-16,dark.width):im.paste(dark.crop((0,0,min(dark.width,w-16-x),48)),(x,18))
 # A portrait crosses both rails; continue only its hidden rail footprint from the same row.
 for a,z in [(93,215)]:im.paste(im.crop((310,0,310+z-a,h)),(a,0))
 mask=polygon(im.size,[(13,1),(625,1),(640,3),(647,8),(w-10,8),(w-2,15),(w-2,h-16),(w-9,h-9),(644,h-9),(635,h-2),(12,h-2),(1,h-13),(1,14)])
 im.putalpha(mask);save('roster_'+name+'_r3',im,box,'Original outer carving; original blank middle lacquer fills text/icon faces; portrait-occluded rails continued from this row')
sheet=Image.new('RGB',(960,len(assets)*170));d=ImageDraw.Draw(sheet)
for i,(name,im) in enumerate(assets):
 for j,color in enumerate(['black','white','#ff00dd']):
  tile=Image.new('RGBA',(320,150),color);s=im.copy();s.thumbnail((310,140));tile.alpha_composite(s,((320-s.width)//2,(150-s.height)//2));sheet.paste(tile.convert('RGB'),(320*j,170*i))
 d.text((5,170*i+153),name,fill='white')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(dict(sourceSHA256=hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),scenarioJobs=0,assets=records,limitations='Hand-traced alpha and hidden face/rail reconstruction; no exact full-page match claim.'),indent=2))
