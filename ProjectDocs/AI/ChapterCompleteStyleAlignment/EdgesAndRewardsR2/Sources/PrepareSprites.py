"""Preserve source RGB; use Scenario alpha for expanded silhouettes and bounded local UI masks."""
from pathlib import Path
import json,hashlib
import numpy as np
from PIL import Image,ImageDraw
import cv2
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/EdgesAndRewardsR2';out=o/'Staging';out.mkdir(exist_ok=True);prod=r/'Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned';prev=Path('/tmp/lor-chaptercomplete-r2/before')/prod.relative_to(r)
p=json.loads((o/'Sources/Provenance.json').read_text());src=Image.open(r/p['source']).convert('RGB');audit=[]
def save(n,im,role):
 im.save(out/(n+'.png'));audit.append({'name':n,'size':im.size,'role':role,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
for n in ['header','foreground']:
 im=src.crop(p['boxes'][n]).convert('RGBA');a=np.array(Image.open(o/'Sources/Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A'))
 if n=='header':
  # The lower chapter ribbon is already an independent sprite. Keep components connected to the header.
  count,labels,stats,centers=cv2.connectedComponentsWithStats((a>8).astype('uint8'),8)
  keep=[]
  for i in range(1,count):
   x,y,w,h,area=stats[i]
   if y<185:keep.append(i)
  a[~np.isin(labels,keep)]=0
  # Explicit exclusion of any isolated chapter-ribbon pixels inside the scenery aperture.
  a[195:,180:785]=0;a[240:,120:830]=0
  # Preserve the intentionally opaque navy title face.
  a[62:148,152:790]=255
 else:
  # Footer border is a separate opaque panel. Keep the Victory ribbon and remove only attached rail pieces.
  a[646:,:166]=0;a[646:,715:]=0
 im.putalpha(Image.fromarray(a));save(n+'_ru',im,'original source RGB + Scenario alpha; bounded component cleanup')
 blank=im.copy();pixels=np.array(blank);old=np.array(Image.open(prev/(n+'_blank.png')).convert('RGBA'))
 # Preserve R1's exact text-free face only; do not propagate its clipped edge alpha.
 boxes=[(168,62,788,139)] if n=='header' else [(331,639,548,688)]
 for x0,y0,x1,y1 in boxes:pixels[y0:y1,x0:x1,:3]=old[y0:y1,x0:x1,:3]
 blank=Image.fromarray(pixels);save(n+'_blank',blank,'expanded original silhouette + retained R1 localized face repair')
def gold(im):
 a=np.array(im.convert('RGB')).astype(float);red,green,blue=a[:,:,0],a[:,:,1],a[:,:,2]
 alpha=np.clip((red-45)/40,0,1)*np.clip((green-25)/25,0,1)*255
 alpha[(red<green*1.07)|(green<blue*1.15)]=0
 im=im.convert('RGBA');im.putalpha(Image.fromarray(alpha.astype('uint8')));return im
im=gold(src.crop(p['boxes']['rewards_heading']));a=np.array(im);a[:8,:,3]=0;a[:17,:232,3]=0;a[:17,418:,3]=0;im=Image.fromarray(a);save('rewards_heading_ru',im,'isolated original gold heading and fine divider ornaments; local luminance/chroma alpha')
a=np.array(im);a[:,232:418,3]=0;save('rewards_heading_blank',Image.fromarray(a),'same ornaments with original Russian lettering excluded for live English')
im=gold(src.crop(p['boxes']['divider'])).crop((0,12,802,26));save('reward_divider',im,'isolated original gold divider; local luminance/chroma alpha')
for name,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 for group,names in [('Main',['header_ru','foreground_ru','rewards_heading_ru','reward_divider']),('Fallback',['header_blank','foreground_blank','rewards_heading_blank'])]:
  sheet=Image.new('RGB',(1000,1320),col);y=10
  for n in names:
   im=Image.open(out/(n+'.png')).convert('RGBA');sheet.paste(im,(15,y),im);y+=im.height+20
  sheet.save(o/'QA'/(group+'AlphaOn'+name+'.png'))
(o/'QA/ExtractionAudit.json').write_text(json.dumps(audit,indent=2)+'\n');print('Prepared',len(audit),'sprites')
