from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib,uuid,re
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');sheet=Image.open(b/'Sheet.png').convert('RGB');result=Image.open(b/'Scenario/relic_frame/booster_medallion_trial01.png').convert('RGBA');record=[]
for item in json.loads((b/'Sheet.json').read_text()):
 name=item['name'];rgb=sheet.crop(item['sheetBox']);w,h=rgb.size;alpha=result.crop(item['sheetBox']).getchannel('A')
 if name.endswith('_frame'):
  outer=Image.new('L',(w,h));ImageDraw.Draw(outer).rounded_rectangle((1,1,w-2,h-2),radius=17,fill=255)
  inner=Image.new('L',(w,h));ImageDraw.Draw(inner).rectangle((20,20,w-21,h-17),fill=255)
  alpha=Image.fromarray(np.maximum(np.minimum(np.array(alpha),np.array(outer)),np.array(inner)))
  method='Original carved boundary with clean navy donor face. Scenario alpha constrained outside rounded contour; opaque interior restored. Bottom border is partly inherited from the source detail/footer junction.'
 else:
  a=np.array(rgb).astype(float);gold=np.clip((a[:,:,0]-a[:,:,2]-9)/18,0,1)*np.clip((a[:,:,0]-28)/36,0,1)
  # Remove the navy lacquer between gold curls, retaining original gold RGB.
  alpha=Image.fromarray((gold*255).round().astype('uint8'))
  method='Original ornament RGB, locally separated gold from navy after Scenario sheet isolation. No lettering or regenerated pixels.'
 im=rgb.convert('RGBA');im.putalpha(alpha);p=out/(name+'.png');im.save(p)
 if not p.with_suffix('.png.meta').exists():
  s=(out/'title_ru.png.meta').read_text();s=re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,s,count=1);p.with_suffix('.png.meta').write_text(s)
 record.append(dict(item,method=method,sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(b/'Provenance.json').write_text(json.dumps(record,indent=2)+'\n')
canvas=Image.new('RGB',(1200,780))
for row,item in enumerate(record):
 im=Image.open(out/(item['name']+'.png'));im.thumbnail((386,240))
 for c,color in enumerate(['black','white','#ff0099']):
  canvas.paste(color,(c*400,row*260,c*400+400,row*260+260));canvas.paste(im,(c*400+7,row*260+5),im)
canvas.save(b.parent/'QA/AlphaBackgrounds.png')
print('Created',len(record),'source-derived sprites')
