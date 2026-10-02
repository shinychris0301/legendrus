from pathlib import Path
from PIL import Image
import numpy as np, json
b=Path(__file__).resolve().parent.parent;qa=b/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI')
prior=Path('ProjectDocs/AI/GameplayReferenceRebuild/PortraitBandR7/QA/Gameplay/Gameplay_941x1672_RU.png')
current=qa/'Gameplay/Gameplay_941x1672_RU.png'
for suffix,path in [('Before',prior),('After',current)]:
 im=Image.open(path)
 im.crop((0,185,340,365)).resize((680,360)).save(qa/f'Portrait{suffix}.png')
 im.crop((86,1334,282,1626)).resize((392,584)).save(qa/f'Booster{suffix}.png')
old=np.array(Image.open(art/'GameplayRefinedR2/environment.png').convert('RGB'))
new=np.array(Image.open(art/'GameplaySurfaceBoundariesR8/environment.png').convert('RGB'))
mask=np.array(Image.open(b/'Sources/EnvironmentRepairMask.png'))
assert np.array_equal(old[mask==0],new[mask==0])
original=np.array(Image.open('ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/Inputs/card.png').convert('RGBA'))
card=np.array(Image.open(art/'GameplaySurfaceBoundariesR8/card.png'))
restore=np.array(Image.open(b/'Sources/RestoredCardOrnamentMask.png'))>0
assert np.array_equal(card[:,:,:3][restore],original[:,:,:3][restore])
oldcard=np.array(Image.open(art/'GameplayOrnamentsR5/card.png'))
gold=(original[:,:,0].astype(float)>95)&(original[:,:,1]>48)&(original[:,:,0].astype(float)>original[:,:,2]*1.65)
restoredGold=restore&gold&np.any(oldcard[:,:,:3]!=original[:,:,:3],axis=2)
normal=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_Normal.png'))
control=np.array(Image.open(qa/'Gameplay/Gameplay_PortraitBand_HudHidden_Control.png'))
rect=(slice(246,282),slice(24,258))
assert np.array_equal(normal[rect],control[rect])
report={'unchangedEnvironmentOutsideSupport':True,'unchangedEnvironmentPixels':int(np.all(old==new,axis=2).sum()),
 'restoredTopCarvingRGBMatchesOriginal':True,'recoveredGoldPixels':int(restoredGold.sum()),
 'nativeHudFreeControlMatchedPixels':234*36,
 'qualification':'Control comparison excludes a live UI strip, not a baked scenery defect. Scenery continuity and remaining joins were inspected visually in native captures; it is not a pixel-exact reconstruction of hidden original scenery.'}
(qa/'SurfaceChecks.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
