from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent.parent;qa=b/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
for label,path in [('Before',Path('ProjectDocs/AI/GameplayReferenceRebuild/PortraitSceneryR9/QA/Gameplay/Gameplay_941x1672_RU.png')),('After',qa/'Gameplay/Gameplay_941x1672_RU.png')]:
 Image.open(path).crop((0,200,941,355)).save(qa/f'Portrait{label}.png')
before=np.array(Image.open(art/'GameplayPortraitSceneryR9/environment.png').convert('RGB'))
after=np.array(Image.open(art/'GameplayHeaderSceneryR10/environment.png').convert('RGB'))
source=np.array(Image.open('ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGB'))
mask=np.array(Image.open(b/'Sources/EnvironmentRepairMask.png'))
hidden=np.array(Image.open(b/'Sources/SourcePlaqueExclusion.png'))
visible=(mask==255)&(hidden==0)
assert np.array_equal(before[mask==0],after[mask==0])
assert np.array_equal(after[visible],source[visible])
native=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_Normal.png'))
control=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_HudHidden_Control.png'))
# Exposed strip beneath the complete header, excluding the live level plaque.
region=np.zeros(native.shape[:2],bool);region[248:310,230:941]=True;region[233:283,395:615]=False
assert np.array_equal(native[region],control[region])
report={'outsideMaskUnchanged':True,'sourceSkyPixelsMatchingExactly':int(visible.sum()),
 'nativeHeaderStripControlMatchedPixels':int(region.sum()),
 'qualification':'Control comparison rules out UI darkening in exposed sky; visual inspection is separate. Source-pixel comparison confirms visible sky restoration, not recovery of hidden scenery.'}
(qa/'SceneryChecks.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
