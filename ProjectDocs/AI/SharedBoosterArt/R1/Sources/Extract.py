from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageChops
import json,uuid,hashlib,numpy as np
b=Path(__file__).resolve().parent;s=Image.open(b/'OriginalReference.png').convert('RGBA');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters');out.mkdir(exist_ok=True)
names=['mace','firebird','shield','potion','scroll','lightning','bomb','swap'];boxes=json.loads((b/'Boxes.json').read_text());subs=json.loads((b/'SubjectBoxes.json').read_text());a=Image.open(b/'Scenario/all/booster_medallion_trial01.png').convert('RGBA').resize((900,480),Image.Resampling.LANCZOS)
# Contours follow the original subjects, excluding attached rim, badges and backdrop.
polys=[[(1,113),(7,115),(15,104),(20,98),(27,91),(38,74),(46,62),(52,60),(54,52),(62,53),(66,59),(62,72),(72,79),(86,82),(99,72),(108,55),(110,44),(107,38),(99,34),(89,33),(77,37),(73,27),(70,23),(75,18),(71,16),(65,20),(56,19),(49,11),(39,7),(30,12),(22,22),(19,33),(24,42),(35,47),(42,47),(39,55),(30,70),(20,84),(15,87),(16,94),(4,108)],
[(14,55),(1,44),(7,15),(20,4),(51,0),(51,25),(63,17),(72,21),(79,39),(86,21),(90,0),(102,3),(111,19),(117,40),(113,72),(86,83),(71,83),(78,104),(70,128),(49,134),(34,128),(23,111),(12,99),(7,83),(25,89),(29,72)],
[(7,21),(24,22),(42,18),(54,7),(66,17),(82,22),(105,22),(105,49),(101,71),(91,93),(74,110),(55,121),(37,112),(20,95),(13,74),(8,47)],
[(12,91),(9,80),(11,68),(24,54),(37,43),(49,39),(53,27),(57,22),(56,13),(63,6),(66,0),(70,0),(71,8),(83,13),(87,18),(80,24),(78,32),(77,40),(85,52),(96,64),(99,78),(94,95),(85,109),(68,117),(47,119),(29,113),(18,102)],
[(15,10),(23,5),(33,8),(71,9),(77,4),(88,4),(89,23),(82,27),(78,75),(85,82),(85,94),(75,98),(31,95),(17,94),(12,89),(13,80),(19,75),(25,24),(16,23)],
[(56,3),(78,8),(95,21),(105,42),(105,66),(92,90),(74,106),(49,111),(24,101),(12,85),(5,65),(6,39),(19,18),(37,7)],
[(53,32),(56,24),(65,21),(66,10),(77,2),(79,8),(74,16),(70,22),(74,28),(85,35),(96,49),(101,70),(96,89),(80,106),(61,116),(43,115),(24,105),(12,92),(5,70),(8,50),(24,35),(40,29)],
[(49,9),(69,13),(83,24),(93,19),(99,41),(96,68),(86,78),(84,82),(72,88),(47,87),(32,80),(17,78),(22,71),(17,56),(16,42),(20,29),(30,17)]]
records={}
def save(n,im,method):
 p=out/(n+'.png');im.save(p);m=p.with_suffix('.png.meta')
 if not m.exists():
  t=Path('Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned/preview_0_r2.png.meta').read_text();t=t.replace(next(x for x in t.splitlines() if x.startswith('guid:')),'guid: '+uuid.uuid4().hex);m.write_text(t)
 records[n]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':method,'size':im.size}
for i,name in enumerate(names):
 orig=s.crop(boxes[i]['source']);im=orig.copy();alpha=a.crop(boxes[i]['atlas']).getchannel('A');ImageDraw.Draw(alpha).ellipse((22,24,153,148),fill=255);im.putalpha(alpha)
 # Reconstruct only the two obscured lower arcs from the same rim's upper arcs.
 # The centre subject and bottom sapphire stay at their original positions.
 mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 for x in [40,136]:d.ellipse((x-23,155-23,x+23,155+23),fill=255)
 mask=mask.filter(ImageFilter.GaussianBlur(.5));im=Image.composite(im.transpose(Image.Transpose.FLIP_TOP_BOTTOM),im,mask)
 shape=Image.new('L',im.size);sd=ImageDraw.Draw(shape);sd.ellipse((6,7,169,172),fill=255)
 for x,y in [(87,9),(87,168),(12,90),(163,90)]:sd.polygon([(x,y-13),(x+13,y),(x,y+13),(x-13,y)],fill=255)
 im.putalpha(ImageChops.darker(im.getchannel('A'),shape.filter(ImageFilter.GaussianBlur(.4))))
 canvas=Image.new('RGBA',(192,192));canvas.paste(im,(8,6));save(name+'_framed',canvas,'Original subject and frame RGB; Scenario alpha; bounded lower stock/plus rim repair from upper arcs.')
 subj=s.crop(subs[i]['source']);m=Image.new('L',(subj.width*4,subj.height*4));ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in polys[i]],fill=255);m=m.resize(subj.size,Image.Resampling.LANCZOS)
 if i==1:
  rgb=np.asarray(subj).astype(float);r,g,blue=rgb[:,:,:3].transpose(2,0,1);warm=np.clip((r-blue*1.5-8)/25,0,1);m=ImageChops.darker(m,Image.fromarray(np.uint8(warm*255)))
 subj.putalpha(m);bbox=m.getbbox();subj=subj.crop((max(0,bbox[0]-3),max(0,bbox[1]-3),min(subj.width,bbox[2]+3),min(subj.height,bbox[3]+3)))
 save(name,subj,'Original RGB with source-traced subject contour. Pixa retained the circular background; manual bounded alpha separation removes it. No regenerated subject pixels.')
(b/'Provenance.json').write_text(json.dumps({'assets':records,'ScenarioJobs':2,'ScenarioCU':6,'sourceSHA256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest()},indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(8*196,390),color)
 for i,n in enumerate(names):
  for y,suffix in [(0,'_framed'),(196,'')]:
   im=Image.open(out/(n+suffix+'.png'));sheet.paste(im,(i*196+(192-im.width)//2,y+(192-im.height)//2),im)
 sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
