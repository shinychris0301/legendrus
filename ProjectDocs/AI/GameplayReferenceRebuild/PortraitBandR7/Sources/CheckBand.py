from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent;qa=b/'QA';qa.mkdir(exist_ok=True)
old=Image.open('ProjectDocs/AI/GameplayReferenceRebuild/HeaderShadowR6/QA/Gameplay/Gameplay_941x1672_RU.png').convert('RGB')
new=Image.open('/tmp/lor-gameplay-r7/Verify02/Gameplay/Gameplay_941x1672_RU.png').convert('RGB')
env=Image.open('/tmp/lor-gameplay-r7/Verify02/Gameplay/Gameplay_PortraitBand_HeaderHidden_Control.png').convert('RGB')
box=(146,233,223,241)
def delta(a,b):return np.max(np.abs(np.asarray(a.crop(box)).astype(int)-np.asarray(b.crop(box)).astype(int)),axis=2)
dold=delta(old,env);dnew=delta(new,env)
report={'nativeExposedScenerySampleBox':box,'beforePixelsDifferentFromEnvironmentBeyond1':int((dold>1).sum()),'afterPixelsDifferentFromEnvironmentBeyond1':int((dnew>1).sum()),'maxAfterChannelDifference':int(dnew.max())}
(qa/'BandPixelCheck.json').write_text(json.dumps(report,indent=2));print(report)
for im,n in [(old,'Before'),(new,'After')]:im.crop((0,170,280,267)).save(qa/f'Portrait{n}.png')
assert report['beforePixelsDifferentFromEnvironmentBeyond1']>0
assert report['afterPixelsDifferentFromEnvironmentBeyond1']==0
