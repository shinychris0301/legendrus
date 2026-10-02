from pathlib import Path
from PIL import Image, ImageDraw
import json, numpy as np, hashlib, shutil
root=Path.cwd();docs=root/'ProjectDocs/AI/GameplayStyleAlignment/SourcePaletteR8';art=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned'
src=Path('/home/k2/Downloads/LegendsOfRus_Extracted_Elements/LegendsOfRus_SourceMatched_Elements/Assets/LegendsOfRus/Art/Gameplay_SourceExtracted/01_Gems/Canonical')
names=['ruby','sapphire','emerald','gold','amethyst','orange'];snames=['red','blue','green','gold','purple','orange']
refs=docs/'Sources/References';refs.mkdir(exist_ok=True)
for n in snames:shutil.copy2(src/f'gem_{n}.png',refs/f'gem_{n}.png')
records=[]
for row,(label,rev) in enumerate([('Source','source'),('R7','r7'),('R8','r8')]):
 if row==0:out=Image.new('RGB',(800,375),'#261e13');draw=ImageDraw.Draw(out)
 draw.text((8,row*125+50),label,fill='white')
 for col,(name,sname) in enumerate(zip(names,snames)):
  path=refs/f'gem_{sname}.png' if rev=='source' else art/f'gem_{name}_{rev}.png'
  im=Image.open(path).convert('RGBA');im=im.crop(im.getchannel('A').getbbox());im.thumbnail((80,92));out.paste(im,(90+col*115+(80-im.width)//2,row*125+15+(92-im.height)//2),im)
  ar=np.array(im);rgb=ar[:,:,:3][ar[:,:,3]>=200]/255.;luma=rgb@np.array([.2126,.7152,.0722]);hsv=np.array(im.convert('RGB').convert('HSV'));sat=hsv[:,:,1][ar[:,:,3]>=200]/255.
  records.append({'revision':rev,'gem':name,'drawnSize':list(im.size),'medianSrgbLuma':round(float(np.median(luma)),4),'p90SrgbLuma':round(float(np.percentile(luma,90)),4),'medianHsvSaturation':round(float(np.median(sat)),4),'brightPixelFractionLumaAbove08':round(float((luma>.8).mean()),4)})
out.save(docs/'QA/Source_R7_R8_BoardSize.png')
(docs/'QA/BoardSizeDiagnostics.json').write_text(json.dumps({'scope':'Diagnostic resampling to80x92 envelope; alpha>=200. sRGB luma proxy, no calibrated perceptual or health conclusion. Generated R8 is an adaptation, not pixel-exact source RGB.','rows':records},indent=2)+'\n')
for bg,label in [('#000000','black'),('#ffffff','white'),('#ed16cd','magenta')]:
 out=Image.new('RGB',(900,380),bg)
 for i,n in enumerate(names):
  im=Image.open(art/f'gem_{n}_r8.png').convert('RGBA');im.thumbnail((142,270));out.paste(im,(i*150+(150-im.width)//2,20+(270-im.height)//2),im)
  small=Image.open(art/f'gem_{n}_r8.png').convert('RGBA');small.thumbnail((80,92));out.paste(small,(i*150+(150-small.width)//2,284),small)
 out.save(docs/'QA'/f'AllSix_{label}.png')
provenance=json.loads((docs/'AlphaPreparedProvenance.json').read_text())+json.loads((docs/'QA/MaskedGemPreparation.json').read_text())
assert len(provenance)==6
(docs/'GeneratedAssetProvenance.json').write_text(json.dumps({'method':'Built-in imagegen','artReference':'Native exact-source extracts; reference only, not runtime screenshot crops.','finalAssets':provenance,'sapphireIterations':'Initial and first correction too narrow; width-only edit did not resolve it. Final independent source-guided reconstruction selected at approx0.73 visible width/height. Earlier sources archived in Sources/Variants.','referenceInputs':[{'path':str(f.relative_to(root)),'sha256':hashlib.sha256(f.read_bytes()).hexdigest()} for f in refs.glob('*.png')]},indent=2)+'\n')
print('Six runtime assets; three alpha proof sheets; source/R7/R8 board-size diagnostic and provenance updated.')
