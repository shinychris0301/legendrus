from pathlib import Path
from PIL import Image,ImageDraw
import json,hashlib,shutil,numpy as np
b=Path('ProjectDocs/AI/TeamRankingStyleAlignment/R1/Sources');source=Path('/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 08_59_50 PM (11).png');shutil.copyfile(source,b/'OriginalReference.png');ref=Image.open(source).convert('RGB')
for d in ['Inputs','Crops','RepairMasks','Final']:(b/d).mkdir(exist_ok=True)
assets={
 'title_ru':(121,101,829,254),
 'podium_gold':(309,305,633,795),'podium_silver':(23,352,290,790),'podium_bronze':(638,357,884,790),
 'crest_wolf':(116,838,199,936),'crest_knot':(115,931,199,1019),'crest_stag':(115,1017,199,1107),'crest_eagle':(111,1103,201,1205),'crest_iron':(114,1196,201,1291),'crest_sun':(114,1285,199,1380),'crest_flame':(114,1374,201,1465),
 'reward_rainbow':(49,696,108,754),'reward_gem':(125,695,185,757),'reward_chest':(196,695,264,758),
 'members':(385,869,413,903),'contribution':(507,861,543,906)
}
# Exact local repairs remove baked dynamic strings without regenerating ornamentation or crests.
regions={
'podium_gold':[(333,573,609,606),(425,610,549,641),(439,650,576,684),(342,694,604,781)],
'podium_silver':[(43,574,271,605),(141,611,239,640),(134,651,244,684),(44,693,269,779)],
'podium_bronze':[(651,574,870,606),(729,611,831,640),(721,651,836,684),(644,694,868,779)]}
records=[]
for n,box in assets.items():
 im=ref.crop(box);rgb=np.array(im);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 for j,r in enumerate(regions.get(n,[])):
  local=(r[0]-box[0],r[1]-box[1],r[2]-box[0],r[3]-box[1]);d.rectangle(local,fill=255)
  # Source frame material, with no text or icons: lacquer donor from original ranking table.
  donor=ref.crop((311,915,380,922));patch=Image.new('RGB',(r[2]-r[0]+1,r[3]-r[1]+1))
  for y in range(0,patch.height,donor.height):
   for x in range(0,patch.width,donor.width):patch.paste(donor,(x,y))
  if j==0:
   arr=np.array(patch).astype(float)
   if n!='podium_silver':arr[:,:,0]*=1.65;arr[:,:,1]*=.75;arr[:,:,2]*=.38
   patch=Image.fromarray(np.clip(arr,0,255).astype('uint8'))
  im.paste(patch,(local[0],local[1]))
 ref.crop(box).save(b/'Crops'/(n+'.png'));mask.save(b/'RepairMasks'/(n+'.png'));im.save(b/'Inputs'/(n+'_native.png'));im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS).save(b/'Inputs'/(n+'.png'));records.append({'name':n,'box':box,'repairRegions':regions.get(n,[]),'operation':'source alpha isolation; original RGB except explicit live-label blanking'})
(b/'Assessment.json').write_text(json.dumps({'source':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'assets':records,'reuse':'Approved Ranking clean scenery, shared navigation, tabs, row frames, plain rank badge, selected fonts. No regeneration needed.'},indent=2))
