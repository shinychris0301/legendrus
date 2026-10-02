from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/GameplayReferenceAligned');report={}
original=Image.open(b/'OriginalReference.png')
for name,box in json.loads((b/'ExtractionPlan.json').read_text()).items():
 if not name.startswith('gem_'):continue
 a=Image.open(dst/(name+'.png')).convert('RGBA');rgb=Image.open(b/'Inputs'/(name+'.png')).convert('RGB');expected=original.crop(box).resize(a.size,Image.Resampling.LANCZOS).convert('RGB')
 same=bool(np.array_equal(np.array(a)[:,:,:3],np.array(rgb)));repro=bool(np.array_equal(np.array(rgb),np.array(expected)));corners=[a.getpixel(p)[3] for p in [(0,0),(a.width-1,0),(0,a.height-1),(a.width-1,a.height-1)]]
 assert same and repro and not any(corners),name
 report[name]={'dimensions':a.size,'sourceRGBUnchanged':same,'inputReproducesFromOriginal':repro,'transparentCorners':corners,'alphaBounds':a.getchannel('A').getbbox(),'sha256':hashlib.sha256((dst/(name+'.png')).read_bytes()).hexdigest()}
a=Image.open(dst/'board.png').convert('RGBA');assert a.getpixel((a.width//2,a.height//2))[3]==0
sheet=Image.new('RGB',(a.width*3,a.height));d=ImageDraw.Draw(sheet)
for i,bg in enumerate(['black','white','#ef19c1']):
 d.rectangle((i*a.width,0,(i+1)*a.width-1,a.height),fill=bg);sheet.paste(a,(i*a.width,0),a)
sheet.save(b.parent/'QA/BoardAlpha.jpg')
(b.parent/'QA/GemSourceIntegrity.json').write_text(json.dumps(report,indent=2));print('PASS: six source RGB / reproducible crop / transparent-corner checks; board center transparent.')
