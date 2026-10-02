from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;root=Path('Assets/LegendsOfRus/Resources/ProductionUI/GameplaySourceGridR11');root.mkdir(exist_ok=True)
src=Image.open('ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA')
atlas=Image.new('RGBA',(128,128));records={}
specs=[('horizontal',(190,588,270,598),(4,4)),('vertical',(173,501,183,581),(92,4)),('joint',(168,582,188,606),(4,24))]
for name,box,pos in specs:
 im=src.crop(box);w,h=im.size
 if name=='horizontal':
  alpha=np.repeat(np.array([0,40,160,255,255,255,255,255,120,0],dtype='uint8')[:,None],w,axis=1);mask=Image.fromarray(alpha)
 elif name=='vertical':
  alpha=np.repeat(np.array([0,40,160,255,255,255,255,255,120,0],dtype='uint8')[None,:],h,axis=0);mask=Image.fromarray(alpha)
 else:
  mask=Image.new('L',(w*8,h*8));d=ImageDraw.Draw(mask)
  d.ellipse((2*8,4*8,19*8,21*8),fill=255)
  d.rectangle((7*8,0,13*8,h*8),fill=255);d.rectangle((0,9*8,w*8,15*8),fill=255)
  mask=mask.resize((w,h),Image.Resampling.LANCZOS)
 im.putalpha(mask);im.save(b/(name+'.png'));atlas.paste(im,pos)
 assert np.array_equal(np.array(im)[:,:,:3],np.array(src.crop(box))[:,:,:3])
 records[name]={'sourceBox':box,'atlasTopLeft':pos,'size':[w,h],'RGBMatchesSource':True}
 for col,label in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
  bg=Image.new('RGBA',im.size,col+(255,));bg.alpha_composite(im);bg.resize((w*6,h*6)).save(b/(name+'_Alpha'+label+'.png'))
atlas.save(root/'grid.png')
(b/'AssetProvenance.json').write_text(json.dumps({'newScenarioCalls':0,'newCU':0,'method':'Original reference RGB from unobstructed rail/joint regions; bounded alpha isolates metal and its narrow contact edge, with opaque joint recess. Native grid remains independent and interactive. No new generation or recoloring.','atlasSize':[128,128],'parts':records},indent=2))
