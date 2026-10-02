from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;base=b.parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned');records=json.loads((base/'AssetProvenance.json').read_text());im=Image.open(b/'Scenario/backgrounds/booster_medallion_trial01.png').convert('RGBA')
for i,n in enumerate(['firebird','yaga','merchant','knight']):
 plate=im.crop(((i%2)*512,(i//2)*384,(i%2+1)*512,(i//2+1)*384));a=np.tile(np.clip((511-np.arange(512))/64,0,1),(384,1));plate.putalpha(Image.fromarray(np.round(a*255).astype(np.uint8)));dest=out/('scene_'+n+'_r3.png');plate.save(dest)
 for rec in records:
  if rec['asset']=='scene_'+n:rec['sha256']=hashlib.sha256(dest.read_bytes()).hexdigest();rec['processing']+=' Rightmost64px alpha fade blends with native card surface; generated RGB unchanged.'
frame=Image.open(out/'frame_r3.png').convert('RGBA');alpha=Image.open(b/'Scenario/frame/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(frame.size,Image.Resampling.LANCZOS);frame.putalpha(alpha);dest=out/'frame_ornaments_r3.png';frame.save(dest);records.append({'asset':'frame_ornaments','output':str(dest),'sha256':hashlib.sha256(dest.read_bytes()).hexdigest(),'processing':'Same repaired original frame RGB with original Scenario hollow alpha; foreground overlay protects rails/curls from scene overlap.'})
# Reuse the already generated clean city behind the heading; no extra generation.
plate=im.crop((0,0,512,384)).resize((941,706),Image.Resampling.LANCZOS).crop((0,180,941,437));yy=np.arange(257);a=np.tile(np.minimum(np.clip(yy/30,0,1),np.clip((256-yy)/30,0,1))[:,None],(1,941));plate.putalpha(Image.fromarray(np.round(a*255).astype(np.uint8)));dest=out/'heading_scene_r3.png';plate.save(dest);records.append({'asset':'heading_scene','output':str(dest),'sha256':hashlib.sha256(dest.read_bytes()).hexdigest(),'processing':'Existing generated city plate resized/cropped for heading scenery;30px vertical alpha fade. No reference screenshot pixels.'})
(base/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
qa=Image.new('RGB',(1200,700),'#555555');y=0
for n in ['frame_ornaments_r3','heading_scene_r3']:
 pic=Image.open(out/(n+'.png'));pic.thumbnail((390,260))
 for i,c in enumerate(['black','white','#ff00ff']):
  t=Image.new('RGBA',pic.size,c);t.alpha_composite(pic);qa.paste(t.convert('RGB'),(i*400,y))
 y+=pic.height+20
qa.crop((0,0,1200,y)).save(base/'QA/FrameAndHeadingAlpha.png')
print('Scene-edge/frame-order correction prepared; no new API calls.')
