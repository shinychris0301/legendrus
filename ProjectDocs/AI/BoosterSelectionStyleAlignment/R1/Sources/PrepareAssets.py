from pathlib import Path
from PIL import Image,ImageDraw,ImageChops
import json,hashlib
root=Path.cwd();base=Path(__file__).resolve().parent;out=root/'Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned';out.mkdir(parents=True,exist_ok=True)
records=[]
for key in ['panel','play','close']:
 source=base/('close_source.png' if key=='close' else f'Scenario/{key}/booster_medallion_trial01.png')
 rgb=Image.open(source).convert('RGBA');mask=Image.open(base/'Scenario'/(key if key=='close' else key+'_alpha')/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(rgb.size,Image.Resampling.LANCZOS)
 if key=='close':
  # Preserve original reference RGB; mask only neighbouring panel rails retained by API.
  contour=Image.new('L',(rgb.width*4,rgb.height*4));ImageDraw.Draw(contour).ellipse((1*4,4*4,87*4,90*4),fill=255);mask=ImageChops.multiply(mask,contour.resize(rgb.size,Image.Resampling.LANCZOS))
 rgb.putalpha(mask);box=mask.getbbox();rgb=rgb.crop(box);rgb.save(out/(key+'.png'))
 records.append({'asset':key,'source':str(source.relative_to(root)),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'alpha':'Scenario isolation'+(' plus local ellipse removing neighbouring rails' if key=='close' else ''),'crop':box,'rgb':'unaltered source RGB','outputSha256':hashlib.sha256((out/(key+'.png')).read_bytes()).hexdigest()})
q=base.parent/'QA';q.mkdir(exist_ok=True);diagnostic=Image.new('RGB',(960,840),'#333333')
for i,key in enumerate(['panel','play','close']):
 im=Image.open(out/(key+'.png'));im.thumbnail((310,260))
 for j,c in enumerate(['black','white','#ff00cc']):
  plate=Image.new('RGBA',im.size,c);plate.alpha_composite(im);diagnostic.paste(plate.convert('RGB'),(i*320,j*280))
diagnostic.save(q/'FinalAlphaDiagnostic.jpg');(base.parent/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
