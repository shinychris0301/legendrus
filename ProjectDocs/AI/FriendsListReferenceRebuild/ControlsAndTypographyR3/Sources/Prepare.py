from pathlib import Path
from PIL import Image
import json
b=Path(__file__).resolve().parent
src=Image.open(b/'OriginalReference.png').convert('RGB')
bounds={'sort':(86,1424,309,1482),'remove':(598,1430,854,1485)}
atlas=Image.new('RGB',(1100,600),(255,0,255)); placements={}
for i,(name,box) in enumerate(bounds.items()):
 im=src.crop(box).resize(((box[2]-box[0])*3,(box[3]-box[1])*3),Image.Resampling.LANCZOS)
 x,y=50,50+i*280;atlas.paste(im,(x,y));placements[name]={'sourceBounds':box,'atlasBounds':[x,y,x+im.width,y+im.height]}
atlas.save(b/'Inputs/controls.png');(b/'Bounds.json').write_text(json.dumps(placements,indent=2))
