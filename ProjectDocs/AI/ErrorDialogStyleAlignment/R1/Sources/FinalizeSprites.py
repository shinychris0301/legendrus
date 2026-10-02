from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,json,hashlib
b=Path('ProjectDocs/AI/ErrorDialogStyleAlignment/R1');s=b/'Sources';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned');out.mkdir(exist_ok=True);src=Image.open(s/'OriginalReference.png').convert('RGB');data=json.loads((s/'Assessment.json').read_text());records=[]
def tile(box,size):
 t=src.crop(box);w,h=t.size;tt=Image.new('RGB',(2*w,2*h));tt.paste(t,(0,0));tt.paste(ImageOps.mirror(t),(w,0));tt.paste(ImageOps.flip(t),(0,h));tt.paste(ImageOps.flip(ImageOps.mirror(t)),(w,h));o=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):o.paste(tt,(x,y))
 return o
for a in data['assets']:
 n=a['name'];im=Image.open(s/'Inputs'/f'{n}.png').convert('RGB');w,h=im.size;mask=Image.open(s/'RepairMasks'/f'{n}.png');alpha=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS)
 if n=='panel':
  # Broaden/blend occupied paper repairs. Use clean navy below source lettering, avoiding upper scrollwork.
  paper=mask.copy();ImageDraw.Draw(paper).rectangle((0,0,w,176),fill=0);paper=paper.filter(ImageFilter.MaxFilter(17)).filter(ImageFilter.GaussianBlur(3));head=mask.copy();ImageDraw.Draw(head).rectangle((0,176,w,h),fill=0)
  im=Image.composite(tile((606,605,748,811),im.size),src.crop(a['bounds']),paper);im=Image.composite(tile((276,555,400,565),im.size),im,head);mask=Image.fromarray(np.maximum(np.array(paper),np.array(head)))
  # Underlying Home notification intrudes across lower border: mirror the corresponding left border segment.
  rect=(613-124,1225-407,672-124,1266-407);donor=(272-124,1225-407,331-124,1266-407);im.paste(ImageOps.mirror(im.crop(donor)),rect[:2]);alpha.paste(ImageOps.mirror(alpha.crop(donor)),rect[:2]);ImageDraw.Draw(mask).rectangle(rect,fill=255)
  inner=Image.new('L',im.size);ImageDraw.Draw(inner).rectangle((39,184,w-41,h-64),fill=255);ImageDraw.Draw(inner).rectangle((85,82,w-85,157),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(inner)));mask.save(s/'RepairMasks/panel_final.png')
 rgb=np.array(im);original=np.array(src.crop(a['bounds']));assert np.array_equal(rgb[np.array(mask)==0],original[np.array(mask)==0]),n;assert hashlib.sha256((s/'Inputs'/f'{n}.png').read_bytes()).hexdigest()==a['inputSha256']
 rgba=im.convert('RGBA');rgba.putalpha(alpha);rgba.save(out/f'{n}.png');sheet=Image.new('RGB',(w*3,h))
 for i,c in enumerate(['black','white','#ef00bd']):back=Image.new('RGBA',im.size,c);back.alpha_composite(rgba);sheet.paste(back.convert('RGB'),(w*i,0))
 sheet.thumbnail((1500,1000));sheet.save(b/'QA'/f'{n}_AlphaReview.png');records.append({'name':n,'finalSha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'originalRGBOutsideFinalMaskUnchanged':True,'submittedInputHashVerified':True})
(b/'QA/AssetChecks.json').write_text(json.dumps(records,indent=2));print('Five assets finalized; source RGB/input checks passed.')
