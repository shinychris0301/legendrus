"""Trace isolated original rails and replace only the opaque footer's repaired wood interior."""
from pathlib import Path
import hashlib,json
import numpy as np
from PIL import Image,ImageDraw,ImageFilter,ImageOps
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/SceneFrameR3';out=o/'Staging';out.mkdir(exist_ok=True)
p=json.loads((o/'Sources/Provenance.json').read_text());source=Image.open(r/p['source']).convert('RGB');audit=[]
def save(n,im,role):
 im.save(out/(n+'.png'));audit.append({'name':n,'size':im.size,'role':role,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
# Source-coordinate contours along the inside of the carved rails. The existing header overlays the join.
left=[(75,0),(75,35),(57,60),(39,85),(27,110),(19,135),(16,160),(22,185),(17,207),(16,275),(16,400),(16,550),(17,675),(17,780),(23,840)]
right=[(0,0),(0,35),(20,60),(41,85),(53,110),(58,140),(58,162),(55,184),(59,207),(59,275),(59,400),(59,550),(59,675),(59,780),(55,840)]
for n,points in [('left',left),('right',right)]:
 box=p[n+'RailBox'];im=source.crop(box).convert('RGBA');w,h=im.size
 m=Image.new('L',(w*4,h*4));poly=([(0,0)]+points+[(0,h)]) if n=='left' else ([(w,0)]+points+[(w,h)])
 ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in poly],fill=255);im.putalpha(m.resize((w,h),Image.Resampling.LANCZOS));save('scene_rail_'+n,im,'original source RGB with locally traced antialiased contour; scenery excluded')
wood=o/'Sources/ScenarioWood/booster_medallion_trial01.png'
if wood.exists():
 footer=Image.open(Path('/tmp/lor-chaptercomplete-r3/before/Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/footer.png')).convert('RGBA')
 generated=Image.open(wood).convert('RGB');target=ImageOps.fit(generated,(835,565),method=Image.Resampling.LANCZOS)
 # Match average source material color; retain generated natural grain without periodic tiling.
 sample=np.asarray(source.crop(p['woodReferenceBox']),dtype=float);a=np.asarray(target,dtype=float);ratio=sample.mean(axis=(0,1))/np.maximum(1,a.mean(axis=(0,1)));a=np.clip(a*ratio,0,255).astype('uint8');target=Image.fromarray(a)
 mask=Image.new('L',(835,565),255);mask=ImageOps.expand(mask,border=8,fill=0).filter(ImageFilter.GaussianBlur(4)).crop((8,8,843,573))
 footer.paste(target,(48,24),mask);save('footer',footer,'existing frame/alpha preserved; Scenario clean grain only in previous interior repair, source mean-color matched and edge blended')
 (o/'QA/WoodColorMatch.json').write_text(json.dumps({'generatedMeanRgb':np.asarray(generated).mean(axis=(0,1)).tolist(),'sourceSampleMeanRgb':sample.mean(axis=(0,1)).tolist(),'appliedChannelMultipliers':ratio.tolist()},indent=2)+'\n')
for name,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 sheet=Image.new('RGB',(1200,900),col)
 for n,xy in [('scene_rail_left',(10,20)),('scene_rail_right',(105,20)),('footer',(220,20))]:
  file=out/(n+'.png')
  if file.exists():
   im=Image.open(file).convert('RGBA');sheet.paste(im,xy,im)
 sheet.save(o/'QA'/('AssetsOn'+name+'.png'))
(o/'QA/ExtractionAudit.json').write_text(json.dumps(audit,indent=2)+'\n');print('Prepared',len(audit),'assets')
