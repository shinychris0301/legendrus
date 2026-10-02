from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;q=b.parent/'QA';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/FailAligned');ref=Image.open(b/'OriginalReference.png').convert('RGB');boxes=json.loads((b/'ExtractionPlan.json').read_text());audit={}
title=Image.new('L',ref.size);title.paste(Image.open(out/'title_ru_r12.png').getchannel('A'),(123,523))
raw_alphas={}
def save(n,rgb,a,source=None):
 p=out/f'{n}.png';Image.fromarray(np.dstack([rgb,a]).astype('uint8')).save(p)
 audit[n]={'size':[rgb.shape[1],rgb.shape[0]],'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'sourceRGBUnchanged':bool(np.array_equal(rgb,source)) if source is not None else None}
 if source is not None:Image.fromarray((np.any(rgb!=source,axis=2)*255).astype('uint8')).save(b/f'{n}_RGBRepairMask.png')
for n,box in boxes.items():
 original=np.array(ref.crop(box));rgb=original.copy();im=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').resize((box[2]-box[0],box[3]-box[1]),Image.Resampling.LANCZOS);a=np.array(im.getchannel('A'));raw_alphas[n]=im.getchannel('A')
 if n=='panel_source_r13':
  donor=Image.open(out/'panel_r10.png').convert('RGB').crop((170,210,730,790)).resize(im.size,Image.Resampling.LANCZOS)
  mask=Image.new('L',im.size);ImageDraw.Draw(mask).rectangle((55,136,855,956),fill=255);f=np.array(mask.filter(ImageFilter.GaussianBlur(3)))/255.
  rgb=(rgb*(1-f[:,:,None])+np.array(donor)*f[:,:,None]).astype('uint8');a=np.maximum(a,np.array(mask))
 a=np.minimum(a,255-np.array(title.crop(box)))
 save(n,rgb,a,original)
# Preserve visible original scenery. Previously Scenario-reconstructed scenery supplies only
# hidden UI/hero areas; generously dilated masks prevent lettering or frame residue.
m=Image.new('L',ref.size);d=ImageDraw.Draw(m);d.rectangle((0,0,941,143),fill=255);d.rectangle((0,0,180,239),fill=255)
m=np.maximum(np.array(m),np.array(title))
for n,box in boxes.items():
 layer=Image.new('L',ref.size);layer.paste(raw_alphas[n],box[:2]);m=np.maximum(m,np.array(layer))
m=cv2.dilate(m,np.ones((23,23),np.uint8));m=np.array(Image.fromarray(m).filter(ImageFilter.GaussianBlur(5)));m[m>245]=255
old=Image.open(out/'environment_r10.png').convert('RGB').resize(ref.size,Image.Resampling.LANCZOS)
f=m[:,:,None]/255.;env=(np.array(ref)*(1-f)+np.array(old)*f).astype('uint8');save('environment_source_r13',env,np.full(m.shape,255,np.uint8));Image.fromarray(m).save(b/'SceneryReconstructionMask.png')
# Original fixed goal-heading plaque; keep English/no-result text live.
entries=json.loads(Path('ProjectDocs/AI/FailStyleAlignment/SourceExtractionR11/Sources/UiRepairMap.json').read_text());entry=next(e for e in entries if e['name']=='heading');box=entry['sourceBox'];rgb=np.array(ref.crop(box));a=np.array(Image.open(out/'heading_r11.png').getchannel('A'));save('heading_source_ru_r13',rgb,a,rgb.copy());audit['heading_source_ru_r13']['sourceBox']=box
(b/'AssetProvenance.json').write_text(json.dumps({'referenceSHA256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'assets':audit,'notes':'Two Scenario alpha extractions, original RGB retained on Ivan and outer carving. Attached title removed from both alpha layers. Reuse existing approved empty panel texture only for occupied interior. Scenery visible original outside dilated UI masks, prior Scenario environment beneath.'},indent=2))
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(255,0,180))]:
 sheet=Image.new('RGB',(950,620),color);d=ImageDraw.Draw(sheet)
 for i,n in enumerate(['ivan_source_r13','panel_source_r13','heading_source_ru_r13']):
  im=Image.open(out/f'{n}.png');im.thumbnail((310,560));x=i*316+(316-im.width)//2;sheet.paste(im,(x,30),im);d.text((i*316+3,8),n,fill=(128,128,128))
 sheet.save(q/f'Alpha{label}.png')
print(json.dumps(audit,indent=2))
