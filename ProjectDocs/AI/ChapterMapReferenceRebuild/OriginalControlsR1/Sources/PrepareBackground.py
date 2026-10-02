from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import numpy as np,cv2,json,hashlib
p=Path(__file__).resolve().parent;src=Image.open(p/'OriginalReference.png').convert('RGB');gen=Image.open(p/'Scenario/backgrounds/booster_medallion_trial01.png').convert('RGB').resize(src.size,Image.Resampling.LANCZOS)
mask=Image.new('L',src.size);d=ImageDraw.Draw(mask);d.rectangle((0,0,940,365),fill=255);d.polygon([(0,0),(185,0),(185,164),(135,241),(44,247),(0,200)],fill=255)
plan=json.loads((p/'ExtractionPlan.json').read_text())
for n in ['banner','play','back']:
 a=Image.open(p.parent/'Assets'/f'{n}.png').getchannel('A');m=Image.new('L',src.size);m.paste(a,tuple(plan['boxes'][n][:2]));mask=ImageChops.lighter(mask,m)
d=ImageDraw.Draw(mask);d.rectangle((0,1598,940,1671),fill=255);d.rectangle((232,1460,713,1663),fill=255)
regions=[(432,578,548,683,430,662,548,709),(362,744,475,852,363,832,472,880),(484,840,606,961,485,932,606,982),(256,905,373,1021,253,993,375,1044),(467,1029,590,1159,471,1129,591,1180),(252,1157,376,1280,250,1254,380,1303),(365,1307,502,1438,364,1412,501,1470)]
for a,b,c,e,f,g,h,i in regions:d.ellipse((a,b,c,e),fill=255);d.rectangle((f,g,h,i),fill=255)
# Expanded support removes anti-aliased ornament shadows. Original scenery beyond the support remains exact.
mask=mask.filter(ImageFilter.MaxFilter(15));mask=mask.point(lambda x:255 if x>4 else 0);support=mask.filter(ImageFilter.GaussianBlur(5));support=ImageChops.lighter(mask,support);top=Image.new('L',src.size);td=ImageDraw.Draw(top);td.rectangle((0,0,940,365),fill=255);top=top.filter(ImageFilter.GaussianBlur(18));support=ImageChops.lighter(support,top);mask.save(p/'EnvironmentRemovalMask.png');support.save(p/'EnvironmentRepairSupport.png')
# Poisson blend generated infill into the source at each level-badge hole to avoid seams.
patched=np.array(gen);orig=np.array(src)
for x0,y0,x1,y1,_,_,_,y2 in regions[:-1]:
 x0=max(0,x0-16);x1=min(940,x1+16);y0-=16;y1=min(1671,y2+16)
 local=np.array(mask)[y0:y1,x0:x1];local[:2]=0;local[-2:]=0;local[:,:2]=0;local[:,-2:]=0
 tile=np.array(gen)[y0:y1,x0:x1];bg=orig.copy();blended=cv2.seamlessClone(tile,bg,local,(x0+(x1-x0)//2,y0+(y1-y0)//2),cv2.NORMAL_CLONE);patched[y0:y1,x0:x1]=blended[y0:y1,x0:x1]
final=Image.composite(Image.fromarray(patched),src,support)
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned/environment.png');final.save(out);final.save(p.parent/'Assets/environment.png')
unchanged=np.array(support)==0;equal=np.all(np.array(final)[unchanged]==orig[unchanged]);(p/'EnvironmentProvenance.json').write_text(json.dumps({'source':'OriginalReference.png','generatedInfill':'Scenario/backgrounds/booster_medallion_trial01.png','operation':'Original visible scenery retained; Scenario infill only inside removed interface support, per-node Poisson seam blend. No reference interface retained.','originalPixelsPreserved':int(unchanged.sum()),'exactOutsideRepairSupport':bool(equal),'width':941,'height':1672,'cuCost':json.loads((p/'Scenario/backgrounds/Billing.json').read_text())['cuCost'],'outputSha256':hashlib.sha256(out.read_bytes()).hexdigest()},indent=2))
