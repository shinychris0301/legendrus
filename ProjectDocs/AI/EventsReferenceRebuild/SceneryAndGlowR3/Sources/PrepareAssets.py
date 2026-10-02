"""Source-aligned clean scenery repairs and independent original Firebird emission.
Visible source scenery is preserved outside recorded repair masks. Hidden content is Scenario reconstruction.
"""
from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;base=b.parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned')
generated=Image.open(b/'Scenario/cards/booster_medallion_trial01.png').convert('RGB');assert generated.size==(1024,768)
records=[];preview=Image.new('RGB',(800,700),'#081423');alphaPlate=Image.new('RGB',(388*3,320*2))
for i,n in enumerate(['firebird','yaga','merchant','knight']):
 orig=np.array(Image.open(b/(n+'_source.png')).convert('RGB'));h,w=orig.shape[:2];source=np.array(Image.open(out/(n+'_r3.png')).convert('RGBA'));assert source.shape[:2]==(h,w)
 core=source[:,:,3]/255.;rgb=orig.astype(float);repair=(core>.005).astype(np.uint8)
 if n=='firebird':
  # Fire trails and small sparks have strong warm chroma, unlike violet sky / blue buildings.
  warm=np.clip((rgb[:,:,0]-np.maximum(rgb[:,:,2]*1.8,rgb[:,:,1]*.76)-26)/100,0,1)
  hot=np.clip((rgb[:,:,0]-190)/55,0,1)*np.clip((rgb[:,:,1]-125)/90,0,1)*np.clip((225-rgb[:,:,2])/85,0,1)
  emission=np.maximum(warm,hot)*np.clip((rgb[:,:,0]-110)/100,0,1)
  yy,xx=np.mgrid[:h,:w];keep=(xx+yy>36)&(xx+(h-1-yy)>38)&(xx>2)&(xx<w-3)&(yy>2)&(yy<h-3)
  emission*=keep
  # Exclude architecture and broad sunset light from the independent effect layer.
  effect=emission.copy();effect[145:245,:185]=0;effect[165:270,:60]=0;effect[100:285,342:]=0
  detail=np.clip((rgb[:,:,0]-cv2.GaussianBlur(rgb[:,:,0],(0,0),3)-2)/18,0,1)
  sparks=effect*(1-core)*detail
  sparkRGB=np.dstack([orig,np.round(sparks*255).astype(np.uint8)])
  Image.fromarray(sparkRGB).save(out/'firebird_sparks_r6.png')
  halo=cv2.GaussianBlur(np.maximum(emission,core*.55).astype(np.float32),(0,0),6.5)*.36
  halo*=np.minimum(np.minimum(xx/10,(w-1-xx)/10),np.minimum(yy/10,(h-1-yy)/10)).clip(0,1)
  haloRGB=np.zeros_like(source);haloRGB[:,:,:3]=(255,143,23);haloRGB[:,:,3]=np.round(halo*255).astype(np.uint8)
  Image.fromarray(haloRGB).save(out/'firebird_halo_r6.png')
  repair=np.maximum(repair,(emission>.04).astype(np.uint8))
  for row,im in enumerate([Image.fromarray(sparkRGB),Image.fromarray(haloRGB)]):
   for c,color in enumerate(['black','white','#ff00ff']):
    plate=Image.new('RGBA',(w,h),color);plate.alpha_composite(im);alphaPlate.paste(plate.convert('RGB'),(c*w,row*h))
  for name,operation in [('firebird_sparks_r6','Original source RGB, warm-emission alpha outside existing subject; corner UI excluded'),('firebird_halo_r6','Soft amber halo derived from source emission/core alpha; explicitly reconstructed light')]:
   records.append({'asset':name,'operation':operation,'source':str(b/'firebird_source.png')})
 # Cover source UI curl footprints at image corners as well as all foreground silhouettes.
 repair[:22,:26]=1;repair[h-28:,:28]=1
 mask=cv2.dilate(repair,np.ones((13,13),np.uint8)).astype(float)
 mask=cv2.GaussianBlur(mask,(0,0),3.5);mask=np.clip(mask*1.6,0,1)
 # Low mask tails are truncated so genuinely unmodified visible source pixels can be audited.
 mask[mask<.015]=0;mask[repair>0]=1
 tile=generated.crop(((i%2)*512,(i//2)*384,(i%2+1)*512,(i//2+1)*384)).resize((w,h),Image.Resampling.LANCZOS);new=np.array(tile)
 clean=np.round(orig*(1-mask[:,:,None])+new*mask[:,:,None]).astype(np.uint8)
 fade=np.tile(np.clip((w-1-np.arange(w))/28,0,1),(h,1));rgba=Image.fromarray(np.dstack([clean,np.round(fade*255).astype(np.uint8)]))
 dest=out/('scene_'+n+'_r6.png');rgba.save(dest);Image.fromarray(np.round(mask*255).astype(np.uint8)).save(base/'QA'/(n+'_repair_mask.png'))
 mismatch=np.any(clean!=orig,axis=2)&(mask==0);assert not mismatch.any()
 records.append({'asset':'scene_'+n+'_r6','source':str(b/(n+'_source.png')),'operation':'Source scenery with generated pixels only under foreground/emission/corner repair masks; right28px blend','preservedSourcePixels':int((mask==0).sum()),'changedOutsideRepairMask':int(mismatch.sum()),'repairCoverage':float((mask>0).mean()),'sha256':hashlib.sha256(dest.read_bytes()).hexdigest()})
 # Diagnostic compositing, not a finished game page.
 plate=Image.new('RGBA',(w,h),'#081423');plate.alpha_composite(rgba)
 if n=='firebird':plate.alpha_composite(Image.fromarray(haloRGB))
 plate.alpha_composite(Image.fromarray(source))
 if n=='firebird':plate.alpha_composite(Image.fromarray(sparkRGB))
 preview.paste(plate.convert('RGB'),((i%2)*400,(i//2)*350))
 previewPic=plate.convert('RGB');previewPic.save(base/'QA'/(n+'_layer_preview.png'))
preview.save(base/'QA/LayerComposites.png');alphaPlate.save(base/'QA/FirebirdAlpha.png')
(base/'QA/AssetProvenance.json').write_text(json.dumps({'assets':records,'newScenarioJobs':2,'costCU':24,'headingJobUsage':'Already submitted before user requested shared Home background; output retained as evidence only, never installed','runtimeEnvironment':'ProductionUI/HomeReferenceAligned/clean_environment, white tint, centered cover'},indent=2)+'\n')
print('Prepared four source-preserving clean plates and two emission layers; Home background reused.')
