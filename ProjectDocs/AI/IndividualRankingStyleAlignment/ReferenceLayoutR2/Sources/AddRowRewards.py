from PIL import Image
from pathlib import Path
import json,hashlib
b=Path('ProjectDocs/AI/IndividualRankingStyleAlignment/ReferenceLayoutR2/Sources');r=Image.open(b/'OriginalReference.png').convert('RGB');m=json.loads((b/'AssetAssessment.json').read_text());sheet=Image.new('RGB',(768,256),(85,89,98))
for i,(name,box) in enumerate([('row_chest_gold',(741,612,881,690)),('row_chest_silver',(741,684,882,761)),('row_chest_bronze',(750,754,884,831))]):
 im=r.crop(box);im.save(b/'Crops'/f'{name}.png');im.save(b/'Inputs'/f'{name}.png');Image.new('L',im.size).save(b/'RepairMasks'/f'{name}.png');scale=min(224/im.width,224/im.height);size=(round(im.width*scale),round(im.height*scale));x=i*256+(256-size[0])//2;y=(256-size[1])//2;sheet.paste(im.resize(size,Image.Resampling.LANCZOS),(x,y));m['assets'].append({'name':name,'group':'icons','sourceBox':list(box),'repairedPixels':0,'classification':'source row reward isolation; reference row chest differs from top prize chest','retainOpaqueInterior':False,'inputSHA256':hashlib.sha256((b/'Inputs'/f'{name}.png').read_bytes()).hexdigest(),'atlas':'row_rewards','atlasBox':[x,y,x+size[0],y+size[1]]})
sheet.save(b/'Inputs/row_rewards_atlas.png');(b/'AssetAssessment.json').write_text(json.dumps(m,indent=2)+'\n')
