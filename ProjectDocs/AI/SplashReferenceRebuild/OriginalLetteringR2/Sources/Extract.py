from pathlib import Path
from PIL import Image
import json,hashlib,numpy as np
b=Path(__file__).resolve().parent.parent;prev=b.parent/'R1/Sources';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/SplashReferenceAligned');source=prev/'Inputs/start.png';mask=prev/'Scenario/start/booster_medallion_trial01.png'
im=Image.open(source).convert('RGBA');alpha=Image.open(mask).convert('RGBA').resize(im.size,Image.Resampling.LANCZOS).getchannel('A');im.putalpha(alpha);p=art/'source_start_lettered_ru.png';im.save(p);im.save(b/'Sources/source_start_lettered_ru.png')
assert np.array_equal(np.array(im)[:,:,:3],np.array(Image.open(source).convert('RGB')))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 bg=Image.new('RGB',im.size,color);bg.paste(im,(0,0),im);bg.save(b/f'QA/Start_{name}.png')
(b/'AssetProvenance.json').write_text(json.dumps({'original':'/home/k2/Downloads/match3final/1.splash.png','sourceBounds':[159,1356,788,1554],'sourceInputSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'reusedScenarioMaskSha256':hashlib.sha256(mask.read_bytes()).hexdigest(),'output':str(p),'outputSha256':hashlib.sha256(p.read_bytes()).hexdigest(),'originalRGBExact':True,'newScenarioCalls':0,'newCostCU':0,'method':'Reuse prior Scenario alpha. Preserve complete original panel surface and Russian lettering. No text removal/inpainting/recoloring/resizing.'},indent=2))
