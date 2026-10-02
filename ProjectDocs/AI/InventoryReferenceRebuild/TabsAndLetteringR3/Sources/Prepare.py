from pathlib import Path
from PIL import Image
import json
b=Path(__file__).resolve().parent;s=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB');sheet=Image.new('RGB',(1024,500),(206,206,206));items=[]
for i,(box,xy) in enumerate(zip([(17,286,247,358),(256,290,471,357),(479,291,689,357),(698,290,921,357)],[(25,30),(535,30),(25,275),(535,275)])):
 im=s.crop(box);im.save(b/('Tab'+str(i)+'Source.png'));im=im.resize((im.width*2,im.height*2),Image.Resampling.LANCZOS);sheet.paste(im,xy);items.append({'index':i,'sourceBox':box,'sheetBox':[xy[0],xy[1],xy[0]+im.width,xy[1]+im.height]})
sheet.save(b/'Sheet.png');(b/'Sheet.json').write_text(json.dumps(items,indent=2))
