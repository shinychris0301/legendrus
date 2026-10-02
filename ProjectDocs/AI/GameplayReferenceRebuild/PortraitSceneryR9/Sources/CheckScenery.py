from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent;qa=b/'QA'
old=Path('ProjectDocs/AI/GameplayReferenceRebuild/SurfaceBoundariesR8/QA/Gameplay/Gameplay_941x1672_RU.png')
new=qa/'Gameplay/Gameplay_941x1672_RU.png'
for label,path in [('Before',old),('After',new)]:
 im=Image.open(path)
 im.crop((0,165,355,407)).resize((710,484)).save(qa/f'Portrait{label}.png')
 # Match the scale and framing of the user's supplied close-up, for honest review.
 im.crop((0,165,355,407)).resize((195,133)).save(qa/f'Portrait{label}_Small.png')
art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
before=np.array(Image.open(art/'GameplaySurfaceBoundariesR8/environment.png').convert('RGB'))
after=np.array(Image.open(art/'GameplayPortraitSceneryR9/environment.png').convert('RGB'))
mask=np.array(Image.open(b/'Sources/EnvironmentRepairMask.png'))
assert np.array_equal(before[mask==0],after[mask==0])
native=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_Normal.png'))
control=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_HudHidden_Control.png'))
assert np.array_equal(native[246:282,24:258],control[246:282,24:258])
report={'outsideMaskUnchanged':True,'unchangedEnvironmentPixels':int(np.all(before==after,axis=2).sum()),
 'nativeHudFreeControlMatchedPixels':8424,
 'qualification':'Automated checks establish bounded scope and absence of a live overlay, not visual acceptance. Inspect native close-ups at the user-provided scale; R8 softened the edge without removing the dark patch.'}
(qa/'SceneryChecks.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
