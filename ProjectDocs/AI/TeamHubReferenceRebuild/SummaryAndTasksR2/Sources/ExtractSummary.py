"""Extract source ornament/lettering. Reuse approved lacquer for hidden faces."""
from pathlib import Path
import json,re,uuid,hashlib
from PIL import Image,ImageDraw
import numpy as np
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamHubAligned')
r=Image.open(b/'Sources/OriginalReference.png').convert('RGBA')
donor=Image.open(art/'nav_frame.png').convert('RGB').crop((330,80,570,200))
assets=[];records=[]
def poly(size,pts):
 a=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in pts],fill=255);return a.resize(size,Image.Resampling.LANCZOS)
def save(n,im,box,method):
 p=art/(n+'.png');im.save(p);m=Path(str(p)+'.meta')
 if not m.exists():m.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'power.png.meta').read_text(),count=1))
 assets.append((n,im));records.append(dict(asset=str(p),sourceBounds=box,method=method,sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
# Small source pictogram: retain warm gold silhouette, remove dark surrounding lacquer.
box=(442,378,484,414);im=r.crop(box);a=np.array(im);c=a[:,:,:3].astype(float)
alpha=np.clip((c[:,:,0]-c[:,:,2]-6)*9,0,255).astype('uint8');alpha[28:,:]=0;im.putalpha(Image.fromarray(alpha))
save('members_icon_r2',im,box,'Source RGB, warm-gold silhouette mask; no regenerated icon')
# Standalone chest enclosure. Opaque circular lacquer deliberately retained.
box=(738,480,883,639);im=r.crop(box);w,h=im.size
pts=[(70,1),(81,10),(113,21),(121,33),(133,56),(137,71),(144,81),(133,94),(128,111),(140,116),(141,148),(132,157),(10,157),(1,148),(1,119),(11,110),(7,98),(0,80),(8,70),(13,49),(25,27),(48,14),(61,9)]
mask=poly(im.size,pts)
# Existing chest will be drawn on top. Clear only its interior, preserving the circular rim.
patch=donor.resize(im.size,Image.Resampling.LANCZOS);clear=Image.new('L',im.size);d=ImageDraw.Draw(clear);d.ellipse((19,23,130,138),fill=255);d.rectangle((0,110,w,h),fill=0);im.paste(patch,(0,0),clear);im.paste(r.crop((box[0],box[1]+104,box[2],box[3])),(0,104))
im.putalpha(mask);save('chest_enclosure_ru_r2',im,box,'Original circular rim and fixed Russian plaque; approved opaque lacquer replaces baked chest')
blank=im.copy();blank.paste(donor.resize((128,42),Image.Resampling.LANCZOS),(9,109));blank.putalpha(mask)
save('chest_enclosure_blank_r2',blank,box,'Same rim; plaque lettering replaced with approved lacquer for live English')
# Continuous task outer frame/header. Hollow centre allows the real cards to draw independently.
box=(24,652,918,891);im=r.crop(box);w,h=im.size
pts=[(24,0),(w-25,0),(w-15,9),(w-3,15),(w-1,h-24),(w-12,h-9),(w-26,h-1),(24,h-1),(10,h-9),(0,h-25),(0,20),(13,11)]
mask=poly(im.size,pts);ImageDraw.Draw(mask).rectangle((28,51,w-29,h-15),fill=0)
im.putalpha(mask);save('tasks_enclosure_ru_r2',im,box,'Original outer carving/header with source fixed title; centre cut out for native task cards')
blank=im.copy();blank.paste(donor.resize((390,36),Image.Resampling.LANCZOS),(252,4));blank.putalpha(mask)
save('tasks_enclosure_blank_r2',blank,box,'Same shell; source heading cleared for English')
# Reward plaque source frame; clear baked gem and amount together.
box=(73,813,310,862);im=r.crop(box);w,h=im.size
mask=poly(im.size,[(13,0),(w-14,0),(w-6,8),(w-2,17),(w-2,31),(w-10,h-3),(w-22,h-1),(16,h-1),(5,h-10),(0,31),(3,14)])
im.paste(donor.resize((w-30,h-14),Image.Resampling.LANCZOS),(15,7));im.putalpha(mask)
save('task_reward_plaque_r2',im,box,'Original source plaque edges; approved blank lacquer; live shared gem and reward count')
# First source task counter: retain its dark inset face, replace text with nearby blank face.
box=(162,780,302,816);im=r.crop(box);w,h=im.size
# Use the text-free top edge of the intentionally dark original inset as face material.
face=r.crop((250,782,284,785)).resize((w,h),Image.Resampling.BICUBIC);im=face.convert('RGBA')
mask=poly(im.size,[(6,0),(w-6,0),(w,7),(w,h-8),(w-6,h-1),(5,h-1),(0,h-7),(0,8)])
im.putalpha(mask);save('task_count_plaque_r2',im,box,'Original dark counter border; source blank inset face replaces baked coin/count')
sheet=Image.new('RGB',(930,len(assets)*185));d=ImageDraw.Draw(sheet)
for row,(n,im) in enumerate(assets):
 for col,color in enumerate(('black','white','#ff00dd')):
  tile=Image.new('RGBA',(310,165),color);s=im.copy();s.thumbnail((302,157));tile.alpha_composite(s,((310-s.width)//2,(165-s.height)//2));sheet.paste(tile.convert('RGB'),(col*310,row*185))
 d.text((4,row*185+168),n,fill='white')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(dict(sourceSHA256=hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),scenarioJobs=0,assets=records,limitations='Traced alpha and occluded lacquer are reconstructed; no exact-font claim.'),indent=2))
