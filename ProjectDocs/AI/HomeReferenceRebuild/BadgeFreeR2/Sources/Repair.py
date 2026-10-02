from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,uuid,re
b=Path(__file__).resolve().parent; art=Path('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned');records={}
for n,cx,cy,rad,axis in [('source_gift',142,24,31,166),('source_events_ru',174,33,29,199),('source_events_blank',174,33,29,199)]:
 im=Image.open(b/'Before'/f'{n}.png').convert('RGBA');a=np.array(im);ys,xs=np.indices(a.shape[:2]); donor=a[ys,np.clip(axis-xs,0,im.width-1)].copy();donor[(axis-xs)<0]=0
 mask=Image.new('L',im.size);ImageDraw.Draw(mask).ellipse((cx-rad,cy-rad,cx+rad,cy+rad),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(.65)); repaired=Image.composite(Image.fromarray(donor),im,mask)
 
 if n=='source_gift':
  aa=np.array(repaired);aa[:22,171:,3]=0;repaired=Image.fromarray(aa)
 repaired.save(art/f'{n}.png');mask.save(b/f'{n}_repair_mask.png');records[n]={'method':'Bounded opposite-rim source donor reflected about the panel center; hidden ornament is reconstructed','mirrorXSum':axis,'mask':f'{n}_repair_mask.png'}
source=Image.open('ProjectDocs/AI/HomeReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA');box=(620,1222,673,1275);badge=source.crop(box);mask=Image.new('L',badge.size);ImageDraw.Draw(mask).ellipse((1,1,51,51),fill=255);badge.putalpha(mask);badge.save(art/'source_events_badge.png')
meta=art/'source_events_badge.png.meta'
if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'source_daily_badge.png.meta').read_text(),count=1))
records['source_events_badge']={'method':'Original RGB with circular alpha','sourceBounds':box};(b/'AssetRepairs.json').write_text(json.dumps(records,indent=2))
for color,n in [('black','Black'),('white','White'),('#ff00ff','Magenta')]:
 canvas=Image.new('RGB',(650,275),color)
 for i,key in enumerate(['source_gift','source_events_ru','source_events_blank']):
  im=Image.open(art/f'{key}.png');canvas.paste(im,(i*215,20),im)
 canvas.save(b.parent/'QA'/f'Alpha_{n}.png')
