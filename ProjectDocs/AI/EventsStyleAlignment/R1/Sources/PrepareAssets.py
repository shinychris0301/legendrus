from pathlib import Path
from PIL import Image
import json,hashlib
root=Path.cwd();b=Path(__file__).resolve().parent;out=root/'Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned';out.mkdir(parents=True,exist_ok=True);records=[]
for key in ['title','frame','firebird','yaga','merchant','knight']:
 source=b/'Scenario'/key/'booster_medallion_trial01.png';im=Image.open(source).convert('RGBA');box=None
 if key in ['title','frame']:
  alpha=Image.open(b/'Scenario'/(key+'_alpha')/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS);im.putalpha(alpha);box=alpha.getbbox();im=im.crop(box)
 else:
  # Gentle edge transparency integrates each new painting into its own opaque card.
  mask=Image.new('L',im.size);w,h=im.size;mask.putdata([round(255*max(0,min(1,x/8,(w-1-x)/42,y/8,(h-1-y)/8))) for y in range(h) for x in range(w)]);im.putalpha(mask)
 im.save(out/(key+'.png'));records.append({'asset':key,'source':str(source.relative_to(root)),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'crop':box,'processing':'Scenario alpha, transparent margin trim' if box else 'Generated illustration, local 8px outer/42px right alpha fade','rgb':'unaltered generated RGB','outputSha256':hashlib.sha256((out/(key+'.png')).read_bytes()).hexdigest()})
q=b.parent/'QA';q.mkdir(exist_ok=True);diagnostic=Image.new('RGB',(1024,720),'#303030')
for i,key in enumerate(['title','frame']):
 im=Image.open(out/(key+'.png'));im.thumbnail((502,220))
 for j,c in enumerate(['black','white','#ff00cc']):
  plate=Image.new('RGBA',im.size,c);plate.alpha_composite(im);diagnostic.paste(plate.convert('RGB'),(i*512,j*240))
diagnostic.save(q/'FrameAlphaDiagnostic.jpg')
# Generated scene sprites have intentionally opaque central painting surfaces.
diag=Image.new('RGB',(1024,1080),'#303030')
for i,key in enumerate(['firebird','yaga','merchant','knight']):
 im=Image.open(out/(key+'.png'));im.thumbnail((256,320))
 for j,c in enumerate(['black','white','#ff00cc']):
  plate=Image.new('RGBA',im.size,c);plate.alpha_composite(im);diag.paste(plate.convert('RGB'),(i*256,j*360))
diag.save(q/'IllustrationAlphaDiagnostic.jpg');(b.parent/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
