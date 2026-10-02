from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter,ImageOps
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/OfflineStyleAlignment/ReferenceAlignmentR2');s=b/'Sources';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned');out.mkdir(exist_ok=True)
original=Image.open(s/'OriginalReference.png').convert('RGB');data=json.loads((s/'Assessment.json').read_text());records=[]
for a in data['assets']:
 n=a['name'];im=Image.open(s/'Inputs'/f'{n}.png').convert('RGB');w,h=im.size;rgb=np.array(im);alpha=Image.open(s/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS);mask=Image.open(s/'RepairMasks'/f'{n}.png')
 if n=='panel':
  # Feather a wider repair region, retaining original dividers and opaque navy face.
  mask=mask.filter(ImageFilter.MaxFilter(15)).filter(ImageFilter.GaussianBlur(3));md=ImageDraw.Draw(mask);md.rectangle((0,889-185,w,905-185),fill=0);md.rectangle((0,1037-185,w,1053-185),fill=0);mask.save(s/'RepairMasks'/'panel_final.png');src=original.crop(a['bounds']);tile=original.crop((175,568,222,834));tw,th=tile.size;t=Image.new('RGB',(tw*2,th*2));t.paste(tile,(0,0));t.paste(ImageOps.mirror(tile),(tw,0));t.paste(ImageOps.flip(tile),(0,th));t.paste(ImageOps.flip(ImageOps.mirror(tile)),(tw,th));patch=Image.new('RGB',(w,h))
  for y in range(0,h,th*2):
   for x in range(0,w,tw*2):patch.paste(t,(x,y))
  rgb=np.array(Image.composite(patch,src,mask));inner=Image.new('L',(w,h));ImageDraw.Draw(inner).rectangle((40,160,w-42,h-155),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(inner)))
 if n=='emblem':
  # Retain the intentionally opaque medallion face and recover only blue light outside it.
  yy,xx=np.mgrid[:h,:w];rr=np.sqrt(((xx-238)/169)**2+((yy-220)/169)**2);inside=np.clip((1-rr)*80,0,1);blue=np.clip((rgb[:,:,2].astype(float)-rgb[:,:,0]-12)/65,0,1);falloff=np.clip((1.32-rr)/.19,0,1);halo=blue*falloff*255;alpha=Image.fromarray(np.maximum(np.array(alpha),np.maximum(inside*255,halo)).astype('uint8'))
 rgba=Image.fromarray(rgb).convert('RGBA');rgba.putalpha(alpha);rgba.save(out/f'{n}.png');sheet=Image.new('RGB',(w*3,h))
 for i,c in enumerate(['black','white','#ef00bd']):back=Image.new('RGBA',(w,h),c);back.alpha_composite(rgba);sheet.paste(back.convert('RGB'),(w*i,0))
 sheet.thumbnail((1500,1000));sheet.save(b/'QA'/f'{n}_AlphaReview.png')
 src=np.array(original.crop(a['bounds']));m=np.array(mask);assert np.array_equal(rgb[m==0],src[m==0]),n
 assert hashlib.sha256((s/'Inputs'/f'{n}.png').read_bytes()).hexdigest()==a['inputSha256']
 records.append({'name':n,'finalSha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'sourceRGBOutsideRepairMaskUnchanged':True,'immutableInputHashVerified':True,'scenarioAlphaWithLocalCleanup':True})
(b/'QA'/'AssetChecks.json').write_text(json.dumps(records,indent=2))
print('Four sprites finalized; original RGB/mask and input checks passed.')
