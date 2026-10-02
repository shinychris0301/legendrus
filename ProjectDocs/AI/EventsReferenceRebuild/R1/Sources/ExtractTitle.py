"""Restore original lettering RGB using the previously accepted Scenario title alpha."""
from pathlib import Path
from PIL import Image
import hashlib,json,numpy as np
root=Path.cwd();base=Path(__file__).resolve().parent.parent
source=Path('Assets/LegendsOfRus/Resources/DesignScreens/Events.png')
old=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/title_r3.png')
box=(205,95,754,302)
rgb=Image.open(source).convert('RGB').crop(box)
mask=base/'Sources/ScenarioTitle/booster_medallion_trial01.png'
alpha=Image.open(mask).convert('RGBA').getchannel('A').resize(rgb.size,Image.Resampling.LANCZOS)
assert rgb.size==alpha.size
out=rgb.convert('RGBA');out.putalpha(alpha)
p=old.with_name('title_original_r4.png');out.save(p)
blank=out.copy();blank.paste(Image.open(old).convert('RGB'),(9,23));blank.putalpha(alpha);blank.save(old.with_name('title_blank_r4.png'))
a=np.array(out);original=np.array(rgb);visible=a[:,:,3]>0
assert np.array_equal(a[:,:,:3][visible],original[visible])
(base/'QA/TitleProvenance.json').write_text(json.dumps({'source':str(source),'sourceSHA256':hashlib.sha256(source.read_bytes()).hexdigest(),'sourceBox':box,'alpha':str(mask),'alphaSHA256':hashlib.sha256(mask.read_bytes()).hexdigest(),'scenarioJob':str(base/'Sources/ScenarioTitle/Job.json'),'output':str(p),'visibleSourceRGBPixels':int(visible.sum()),'RGBMismatches':0,'newScenarioRequests':1,'cuCost':3},indent=2)+'\n')
plate=Image.new('RGB',(549*3,207))
for i,c in enumerate(['black','white','#ff00ff']):
 bg=Image.new('RGBA',out.size,c);bg.alpha_composite(out);plate.paste(bg.convert('RGB'),(549*i,0))
plate.save(base/'QA/TitleAlpha.png')
