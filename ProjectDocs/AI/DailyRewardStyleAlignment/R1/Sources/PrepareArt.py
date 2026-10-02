"""Install reviewed Scenario assets; retain original RGB for the extracted chest."""
from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path('ProjectDocs/AI/DailyRewardStyleAlignment/R1');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned');out.mkdir(exist_ok=True)
manifest=[]
for n in ['header','card','chest','environment']:
 src=b/'Sources/Scenario'/n/'booster_medallion_trial01.png';im=Image.open(src).convert('RGBA')
 if n in ['header','card']:
  a=Image.open(b/'Sources/Scenario'/(n+'_alpha')/'booster_medallion_trial01.png').convert('RGBA');assert a.size==im.size;im.putalpha(a.getchannel('A'))
 if n=='chest':
  original=Image.open(b/'Sources/chest_source.png').convert('RGBA');original.putalpha(im.getchannel('A'));im=original
 if n!='environment':
  assert im.getchannel('A').getextrema()==(0,255)
  for color,name in [('black','Black'),('white','White'),('#ff00b0','Magenta')]:
   bg=Image.new('RGBA',im.size,color);bg.alpha_composite(im);bg.thumbnail((800,950));bg.save(b/f'QA/{n}On{name}.png')
 im.save(out/(n+'.png'));manifest.append({'file':str(out/(n+'.png')),'source':str(src),'dimensions':im.size,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
(b/'Sources/RuntimeProvenance.json').write_text(json.dumps(manifest,indent=2)+'\n')
