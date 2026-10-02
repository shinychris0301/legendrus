from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned');ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');box=(180,1458,761,1600);im=ref.crop(box);w,h=im.size
points=[(0,77),(26,56),(32,26),(50,16),(78,23),(235,22),(267,12),(290,0),(311,22),(480,23),(510,15),(535,23),(548,53),(578,77),(549,91),(530,113),(507,124),(323,124),(290,140),(273,128),(77,126),(40,121),(25,103),(20,89)]
a=Image.new('L',(w*4,h*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255);mask=np.array(a.resize(im.size,Image.Resampling.LANCZOS)).astype(float)
face=Image.new('L',im.size);ImageDraw.Draw(face).polygon([(62,37),(515,37),(532,69),(511,116),(60,116),(44,76)],fill=255)
rgb=np.array(im)[:,:,:3].astype(float);edge=np.clip((rgb.max(2)-30)/35,0,1);edge[np.array(face)>0]=1;mask=np.rint(mask*edge).astype('uint8');im.putalpha(Image.fromarray(mask))
p=art/'details_ru_r2.png';im.save(p);m=Path(str(p)+'.meta')
if not m.exists():m.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details.png.meta').read_text(),count=1))
assert np.array_equal(np.array(im)[:,:,:3],np.array(ref.crop(box))[:,:,:3])
sheet=Image.new('RGB',(960,210));draw=ImageDraw.Draw(sheet)
for i,c in enumerate(['black','white','#ff00dd']):
 t=Image.new('RGBA',(320,180),c);s=im.copy();s.thumbnail((310,170));t.alpha_composite(s,((320-s.width)//2,(180-s.height)//2));sheet.paste(t.convert('RGB'),(i*320,0))
draw.text((10,190),'Original Details button and Russian lettering; source RGB retained',fill='white');sheet.save(b/'QA/AssetAlpha.png');(b/'Sources/AssetProvenance.json').write_text(json.dumps({'sourceSHA256':hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),'asset':str(p),'bounds':box,'sourceRGBUnchanged':True,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'scenarioJobs':0,'notes':'Traced/color-refined alpha; existing approved blank details sprite retained for English; no generation'},indent=2))
