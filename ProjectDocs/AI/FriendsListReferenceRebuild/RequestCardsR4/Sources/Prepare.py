from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,json
b=Path(__file__).resolve().parent;s=Image.open(b/'RequestsReference.png').convert('RGB');(b/'RefinedInputs').mkdir(exist_ok=True)
# Clean navy source patch, mirrored to avoid hard seams in reconstructed hidden faces.
p=s.crop((480,466,552,529));tile=Image.new('RGB',(144,126));tile.paste(p,(0,0));tile.paste(ImageOps.mirror(p),(72,0));tile.paste(ImageOps.flip(tile.crop((0,0,144,63))),(0,63))
def face(w,h):
 out=Image.new('RGB',(w,h))
 for y in range(0,h,126):
  for x in range(0,w,144):out.paste(tile,(x,y))
 return out
card=s.crop((27,447,913,630));w,h=card.size
# Clear all content while retaining original top/bottom rails and right-corner scrolls.
mask=Image.new('L',card.size);ImageDraw.Draw(mask).rectangle((28,15,w-28,h-16),fill=255)
card=Image.composite(face(w,h),card,mask)
# The original portrait intersects left portions of top/bottom rails; extend their clean right donor.
for y,hgt in [(0,17),(h-17,17)]:
 donor=card.crop((300,y,550,y+hgt));card.paste(donor.resize((247,hgt)),(26,y))
# Left trim is occluded by the original portrait: restore a mirrored right trim.
card.paste(ImageOps.mirror(card.crop((w-28,0,w,h))),(0,0))
card.save(b/'RefinedInputs/card.png')
outer=s.crop((7,419,934,1401));w,h=outer.size
# Empty enclosure: keep only original outer carved rails and bottom center jewel.
mask=Image.new('L',outer.size);d=ImageDraw.Draw(mask);d.rectangle((22,22,w-23,h-55),fill=255)
outer=Image.composite(face(w,h),outer,mask)
# Remove row rails/portrait tips above the isolated bottom ornament.
# Preserve the complete source sapphire/scrollwork below the cleared interior.
# Clear middle top tab-ornament overlap with an uninterrupted donor section.
donor=outer.crop((20,0,70,23));outer.paste(donor.resize((w-48,23)),(24,0))
outer.save(b/'RefinedInputs/surround.png')
atlas=Image.new('RGB',(1840,1460),(255,0,255));atlas.paste(card.resize((1772,366),Image.Resampling.LANCZOS),(30,25));atlas.paste(outer,(30,440));atlas.save(b/'RefinedInputs/frames.png')
(b/'Bounds.json').write_text(json.dumps({'card':{'sourceBounds':[27,447,913,630],'atlasBounds':[30,25,1802,391],'scale':2},'surround':{'sourceBounds':[7,419,934,1401],'atlasBounds':[30,440,957,1422],'scale':1}},indent=2))
