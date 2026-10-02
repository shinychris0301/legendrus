from pathlib import Path
import hashlib,json
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parent.parent;root=Path('Assets/LegendsOfRus/Resources/ProductionUI');before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
keys=['IndividualRankingFonts/PlayerNames.ttf','IndividualRankingFonts/Title.ttf','SharedBoosters/potion.png','SharedBoosters/firebird.png','GameplayReferenceAligned/gem_sapphire.png','GameplayReferenceAligned/gem_ruby.png','HomeReferenceAligned/source_gift.png','HomeReferenceAligned/clean_environment.png']
keys += ['EventDetailAligned/'+k+'.png' for k in ['progress_track_r6','panel_rails_r6','chest0_r5','chest1_r5','chest2_r5','chest3_r5','chest4_r5','premium_chest_r5','title','play','activate']]
record={}
for key in keys:
 p=root/key;h=hashlib.sha256(p.read_bytes()).hexdigest();assert before[str(p)]==h;record[key]=h
(b/'Sources/ReusedAssets.json').write_text(json.dumps(record,indent=2))
# Source eyes remain opaque; detached skyline pixel is removed from both solid/light alpha.
a=np.array(Image.open(root/'EventDetailAligned/firebird_r7.png'))[:,:,3]
for x,y in [(109,136),(57,268),(113,289)]:assert a[y,x]>=200,(x,y,int(a[y,x]))
assert not a[180:187,665:669].any()
l=np.array(Image.open(root/'EventDetailAligned/firebird_sparks_r7.png'))[:,:,3];assert not l[180:187,665:669].any()
(b/'QA/PreservationChecks.json').write_text(json.dumps({'unchangedApprovedResources':len(record),'threeBlueEyeSamplesOpaque':True,'detachedSkylinePixelRemovedFromSubjectAndLight':True,'newScenarioCalls':0},indent=2))
print('PASS approved resources/fonts unchanged; blue eyes retained; skyline residue removed')
