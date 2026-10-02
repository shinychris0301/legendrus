from pathlib import Path
from PIL import Image
import numpy as np,cv2,json
s=Path('ProjectDocs/AI/ProfileStyleAlignment/R1/Sources');source=Image.open(s/'OriginalReference.png').convert('RGB');donor=source.crop((819,245,882,278));centers=[(56,902),(238,904),(409,905),(584,905),(752,905)]
for r in json.loads((s/'Assessment.json').read_text())['assets']:
 n=r['name']
 if not n.startswith(('hero_','relic_')):continue
 orig=np.array(Image.open(s/'Crops'/(n+'.png')).convert('RGB'));rgb=orig.copy();h,w=rgb.shape[:2];mask=np.zeros((h,w),'uint8')
 y0,y1=(927-r['box'][1],955-r['box'][1]) if n.startswith('hero_') else (166,211)
 x0,x1=14,w-12;rgb[y0:y1,x0:x1]=np.array(donor.resize((x1-x0,y1-y0),Image.Resampling.LANCZOS));mask[y0:y1,x0:x1]=255
 if n.startswith('hero_'):
  cx,cy=centers[int(n[-1])];cx-=r['box'][0];cy-=r['box'][1];m=np.zeros_like(mask);cv2.circle(m,(cx,cy),15,255,-1);rgb[m>0]=(8,22,27);mask=np.maximum(mask,m)
 Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(mask).save(s/'RepairMasks'/(n+'.png'))
(s/'LocalRepairNotes.md').write_text('Original submitted Scenario PNGs remain immutable. Final native card captions are replaced with a clean navy surface sampled from original reference rectangle [819,245,882,278], leaving card borders and illustrations intact. Hero level digits are cleared inside radius-15 badge faces; live em-dashes indicate preview status. Original Main portrait pencil is locally inpainted and reintroduced as an independent button. Wide footer crops replace two rejected clipped attempts. Scenario alpha preserves exterior contours; bounded opaque interiors and original golden star pixels restore erased framed-card detail. No original portrait environment is used outside its intended card/frame. Russian title keeps source lettering; English title face cleared for live text.\n')
