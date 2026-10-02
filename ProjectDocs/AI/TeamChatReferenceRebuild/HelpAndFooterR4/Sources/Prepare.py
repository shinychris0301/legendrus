from pathlib import Path
from PIL import Image,ImageDraw
import json
b=Path(__file__).resolve().parent
s=Image.open(b/'OriginalReference.png').convert('RGBA')
# Source pixels, including original Russian lettering. No generation/repainting.
boxes={'help':(678,721,843,782),'footer':(300,1615,650,1672)}
for name,box in boxes.items():
 im=s.crop(box)
 if name=='footer':
  # Keep only the lower carving, excluding the adjacent Help Requests plaque.
  mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
  d.polygon([(0,36),(40,27),(83,19),(108,9),(140,15),(157,4),(168,0),(179,9),(194,15),(219,8),(246,20),(299,28),(350,39),(350,57),(0,57)],fill=255)
  canvas=Image.new('RGBA',im.size,(255,255,255,255));canvas.paste(im,(0,0),mask);im=canvas
 im.save(b/'Inputs'/(name+'.png'))
(b/'ExtractionBounds.json').write_text(json.dumps(boxes,indent=2))
