"""Previously authorized alpha-only cleanup of the generated wider blue jewel.
Run after PrepareArt.py; unlike that script this removes a painted checkerboard.
"""
from pathlib import Path
from collections import deque
from PIL import Image
import numpy as np,json,hashlib,shutil
root=Path(__file__).resolve().parents[5]
docs=root/'ProjectDocs/AI/GameplayStyleAlignment/CalmerReferenceR6'
source=docs/'Sources/gem_sapphire_r6_wider_raw.png'
a=np.array(Image.open(source).convert('RGBA'));rgb=a[:,:,:3].astype(int)
eligible=(rgb.max(2)-rgb.min(2)<45)&(rgb.min(2)>120)
h,w=eligible.shape;seen=np.zeros((h,w),bool);queue=deque()
for y,x in [(0,x) for x in range(w)]+[(h-1,x) for x in range(w)]+[(y,0) for y in range(h)]+[(y,w-1) for y in range(h)]:
 if eligible[y,x] and not seen[y,x]:seen[y,x]=True;queue.append((y,x))
while queue:
 y,x=queue.popleft()
 for yy,xx in ((y-1,x),(y+1,x),(y,x-1),(y,x+1)):
  if 0<=yy<h and 0<=xx<w and eligible[yy,xx] and not seen[yy,xx]:seen[yy,xx]=True;queue.append((yy,xx))
a[:,:,3]=np.where(seen,0,255)
image=Image.fromarray(a);bounds=image.getchannel('A').getbbox();final=image.crop(bounds)
assert np.array_equal(np.array(final)[:,:,:3],rgb[bounds[1]:bounds[3],bounds[0]:bounds[2]])
out=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/gem_sapphire_r6.png';final.save(out)
for color,name in [('black','black'),('white','white'),('#ed16cd','magenta')]:
 proof=Image.new('RGBA',final.size,color);proof.alpha_composite(final);proof.convert('RGB').save(docs/'QA'/('gem_sapphire_r6_'+name+'.png'))
records=json.loads((docs/'GeneratedAssetProvenance.json').read_text());entry=next(d for d in records if d['name']=='gem_sapphire_r6')
entry.update(source=json.loads((docs/'SapphireRevision.json').read_text())['output'],sourceCopy=str(source.relative_to(root)),sourceSHA256=hashlib.sha256(source.read_bytes()).hexdigest(),runtimeSHA256=hashlib.sha256(out.read_bytes()).hexdigest(),runtimeDimensions=list(final.size),transparentMarginTrim=list(bounds),alphaOnlyCleanup='Previously authorized local cleanup: flood remove edge-connected light near-neutral painted checkerboard (RGB range <45 and min >120). Retained RGB unchanged, artwork opaque; transparent margins trimmed.')
entry.pop('opaquePixelsRestored',None)
(docs/'GeneratedAssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
print('Sapphire retained RGB verified; alpha cleanup reproducible.')
