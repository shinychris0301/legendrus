from pathlib import Path
from PIL import Image,ImageDraw,ImageChops
import json
b=Path('ProjectDocs/AI/NavigationStyleAlignment/R1');s=b/'Sources';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/NavigationAligned');out.mkdir(exist_ok=True)
source=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png').convert('RGBA');boxes=json.loads((s/'IconBoxes.json').read_text());ai=Image.open(s/'Scenario/icons/booster_medallion_trial01.png').convert('RGBA').resize((640,128),Image.Resampling.LANCZOS)
for i,(k,box) in enumerate(boxes.items()):
 im=source.crop(box);w,h=im.size;mask=Image.new('L',(w*4,h*4));d=ImageDraw.Draw(mask)
 if k=='coin':d.ellipse((3*4,4*4,55*4,56*4),fill=255)
 elif k=='plus':d.ellipse((1*4,3*4,42*4,44*4),fill=255)
 elif k=='gem':d.polygon([(x*4,y*4) for x,y in [(0,19),(12,3),(36,2),(54,15),(53,25),(29,56),(24,56),(1,27)]],fill=255)
 elif k=='energy':d.polygon([(x*4,y*4) for x,y in [(14,3),(39,2),(25,24),(39,24),(7,58),(14,32),(0,33)]],fill=255)
 else:d.ellipse((3*4,3*4,82*4,82*4),fill=255)
 mask=mask.resize((w,h),Image.Resampling.LANCZOS);a=ai.getchannel('A').crop((i*128+20,20,i*128+20+w,20+h));im.putalpha(ImageChops.multiply(mask,a));im.save(out/(k+'.png'))
# Retain the existing isolated blank capsule, trim only its genuine transparent margin.
im=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/resource_capsule.png');im.crop((29,101,2143,618)).save(out/'capsule.png')
qa=Image.new('RGB',(600,360));
for row,col in enumerate(['black','white','#e000a0']):
 bg=Image.new('RGBA',(600,120),col)
 for i,k in enumerate(boxes):
  im=Image.open(out/(k+'.png'));im.thumbnail((90,100));bg.alpha_composite(im,(i*115+10,10))
 qa.paste(bg.convert('RGB'),(0,row*120))
qa.save(b/'Sources/AlphaQA.png')
