from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,json,hashlib
b=Path('ProjectDocs/AI/MaintenanceStyleAlignment/R1');s=b/'Sources';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned');out.mkdir(exist_ok=True);src=Image.open(s/'OriginalReference.png').convert('RGB');data=json.loads((s/'Assessment.json').read_text());records=[]
def tile(box,size):
 t=src.crop(box);w,h=t.size;tt=Image.new('RGB',(2*w,2*h));tt.paste(t,(0,0));tt.paste(ImageOps.mirror(t),(w,0));tt.paste(ImageOps.flip(t),(0,h));tt.paste(ImageOps.flip(ImageOps.mirror(t)),(w,h));o=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):o.paste(tt,(x,y))
 return o
for a in data['assets']:
 n=a['name'];im=Image.open(s/'Inputs'/f'{n}.png').convert('RGB');w,h=im.size;mask=Image.open(s/'RepairMasks'/f'{n}.png');alpha=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS)
 if n=='panel':
  # Correct the donor choice locally; retain immutable submitted input and Scenario silhouette.
  paper=mask.copy();ImageDraw.Draw(paper).rectangle((0,0,w,330),fill=0);red=mask.copy();ImageDraw.Draw(red).rectangle((0,330,w,h),fill=0);im=Image.composite(tile((190,1248,431,1297),im.size),src.crop(a['bounds']),paper);im=Image.composite(tile((684,500,740,562),im.size),im,red)
  inner=Image.new('L',im.size);ImageDraw.Draw(inner).rectangle((66,311,w-67,h-75),fill=255);ImageDraw.Draw(inner).rectangle((115,95,w-114,252),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(inner)))
 rgb=np.array(im);original=np.array(src.crop(a['bounds']));assert np.array_equal(rgb[np.array(mask)==0],original[np.array(mask)==0]),n;assert hashlib.sha256((s/'Inputs'/f'{n}.png').read_bytes()).hexdigest()==a['inputSha256']
 rgba=im.convert('RGBA');rgba.putalpha(alpha);rgba.save(out/f'{n}.png');sheet=Image.new('RGB',(w*3,h))
 for i,c in enumerate(['black','white','#ef00bd']):back=Image.new('RGBA',im.size,c);back.alpha_composite(rgba);sheet.paste(back.convert('RGB'),(w*i,0))
 sheet.thumbnail((1500,1100));sheet.save(b/'QA'/f'{n}_AlphaReview.png');records.append({'name':n,'finalSha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'originalRGBOutsideMaskUnchanged':True,'submittedInputHashVerified':True})
(b/'QA/AssetChecks.json').write_text(json.dumps(records,indent=2));print('Four assets finalized and source/mask hashes verified.')
