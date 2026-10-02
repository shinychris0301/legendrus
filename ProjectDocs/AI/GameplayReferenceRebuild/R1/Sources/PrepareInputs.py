from pathlib import Path
from PIL import Image
import shutil,json
b=Path(__file__).resolve().parent;(b/'Inputs').mkdir(exist_ok=True)
shutil.copyfile('/home/k2/Downloads/match3final/077-fantasy_russian_match_3_adventure.png',b/'OriginalReference.png')
a=Image.open(b/'OriginalReference.png')
boxes={'portrait':(8,15,240,244),'moves':(230,57,374,237),'goals':(373,57,724,237),'score':(724,57,939,243),'board':(52,472,889,1249),'card':(88,1335,280,1623),'level':(403,233,606,278),'pause':(837,1570,941,1672)}
boxes.update({'gem_ruby':(84,490,168,591),'gem_sapphire':(186,489,275,590),'gem_emerald':(282,492,369,587),'gem_gold':(377,491,469,589),'gem_amethyst':(477,487,565,591),'gem_orange':(576,491,665,591)})
for n,box in boxes.items():
    crop=a.crop(box)
    if n.startswith('gem_'):crop=crop.resize((crop.width*3,crop.height*3),Image.Resampling.LANCZOS)
    crop.save(b/'Inputs'/(n+'.png'))
(b/'ExtractionPlan.json').write_text(json.dumps(boxes,indent=2))
