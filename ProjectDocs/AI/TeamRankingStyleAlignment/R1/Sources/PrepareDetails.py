from pathlib import Path
from PIL import Image
import numpy as np,json,cv2
b=Path('ProjectDocs/AI/TeamRankingStyleAlignment/R1/Sources')
ref=Image.open(b/'OriginalReference.png').convert('RGB');box=(180,1458,773,1608)
im=ref.crop(box);im.save(b/'Crops/details.png');rgb=np.array(im)
mask=np.zeros((im.height,im.width),'uint8');mask[58:106,69:523]=255
rgb=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
im=Image.fromarray(rgb);im.save(b/'Inputs/details_native.png')
im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS).save(b/'Inputs/details.png')
Image.fromarray(mask).save(b/'RepairMasks/details.png')
a=json.loads((b/'Assessment.json').read_text());a['assets']=[x for x in a['assets'] if x['name']!='details']
a['assets'].append({'name':'details','box':box,'repairRegions':[(249,1516,703,1564)],'operation':'source button extraction; bounded local caption inpaint after Scenario alpha isolation; RGB outside repair mask preserved'})
(b/'Assessment.json').write_text(json.dumps(a,indent=2))
