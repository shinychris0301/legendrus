from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent.parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned')
src=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/R1/Sources/OriginalEventDetailReference.png').convert('RGBA')
frame=np.array(Image.open(art/'panel_rails_r6.png'));original=np.array(src.crop((8,730,933,1504)));visible=frame[:,:,3]>0
mismatches=int(np.any(frame[:,:,:3][visible]!=original[:,:,:3][visible],axis=1).sum());assert mismatches==0
track=np.array(Image.open(art/'progress_track_r6.png'))
assert np.all(track[18,20:860,3]==255), 'Empty channel must be continuous and opaque'
assert np.all(track[7,20:860,3]>0), 'Top edge must be continuous'
assert np.all(track[29,20:860,3]>0), 'Bottom edge must be continuous'
for x0,y0,x1,y1 in [(80,842,880,872),(75,1187,410,1213),(495,1187,893,1213)]:
 # Exclude the two-pixel antialias fringe adjacent to the rails; inspect the text-bearing interior.
 assert not frame[y0-730+2:y1-730-1,x0-8+2:x1-8-1,3].any(), 'Frame must not contain original heading text'
before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());reuse={}
paths=[Path('Assets/LegendsOfRus/Resources/ProductionUI')/p for p in ['SharedBoosters/potion.png','SharedBoosters/firebird.png','GameplayReferenceAligned/gem_sapphire.png','GameplayReferenceAligned/gem_ruby.png','HomeReferenceAligned/source_gift.png','HomeReferenceAligned/clean_environment.png']]
paths += [art/(k+'_r5.png') for k in ['chest0','chest1','chest2','chest3','chest4','premium_chest']]
for p in paths:
 h=hashlib.sha256(p.read_bytes()).hexdigest();assert before[str(p)]==h;reuse[str(p)]=h
(b/'Sources/ReusedAssets.json').write_text(json.dumps(reuse,indent=2))
(b/'QA/AssetChecks.json').write_text(json.dumps({'frameVisiblePixels':int(visible.sum()),'frameSourceRGBMismatches':mismatches,'continuousEmptyChannel':True,'continuousUpperAndLowerRail':True,'noOriginalHeadingTextInFrame':True,'unchangedApprovedResourcesAndChests':len(reuse)},indent=2))
print('PASS: frame source RGB, continuous channel/rails, excluded lettering, unchanged approved resources')
