from pathlib import Path
from PIL import Image
import json,shutil
b=Path(__file__).resolve().parent;(b/'Inputs').mkdir(exist_ok=True)
shutil.copyfile('/home/k2/Downloads/match3final/victory.png',b/'OriginalReference.png')
a=Image.open(b/'OriginalReference.png')
boxes={'resource_card':(129,1005,349,1133),'coin':(155,1025,243,1114),'gem':(372,1024,464,1115),'title':(158,21,790,243),'firebird':(12,70,929,713),'star':(370,598,582,809),'panel':(90,828,853,1360),'card':(134,1167,265,1338),'continue':(157,1360,786,1481),'video':(94,1481,846,1623),'footer':(317,1612,625,1672)}
for n,box in boxes.items():a.crop(box).save(b/'Inputs'/(n+'.png'))
(b/'ExtractionPlan.json').write_text(json.dumps(boxes,indent=2))
