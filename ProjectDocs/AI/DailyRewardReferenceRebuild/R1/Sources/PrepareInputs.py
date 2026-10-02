from pathlib import Path
from PIL import Image
import json
b=Path(__file__).resolve().parent;im=Image.open(b/'OriginalReference.png').convert('RGBA')
boxes={'header':(88,108,853,327),'card':(260,388,464,737),'active':(32,379,251,743),'day7':(476,746,906,1101),'month':(23,1100,920,1340),'tomorrow':(45,1338,894,1497),'claim':(225,1502,715,1631)}
for k,v in boxes.items():im.crop(v).save(b/'Inputs'/(k+'.png'))
positions={'card':(0,0),'day7':(250,0),'active':(700,0),'month':(0,400),'tomorrow':(0,680),'claim':(0,875),'header':(0,1080)}
atlas=Image.new('RGB',(1024,1536),'black')
for k,(x,y) in positions.items():atlas.paste(im.crop(boxes[k]),(x,y))
atlas.save(b/'Inputs/panel.png');(b/'Boxes.json').write_text(json.dumps({'source':boxes,'atlas':positions},indent=2))
