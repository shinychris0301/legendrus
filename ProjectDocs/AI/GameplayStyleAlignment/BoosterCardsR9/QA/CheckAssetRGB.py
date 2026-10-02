"""Verify installed booster RGB against the recorded dedicated atlas regions."""
from pathlib import Path
from PIL import Image
import numpy as np
import json
root=Path(__file__).resolve().parents[5]
doc=root/'ProjectDocs/AI/GameplayStyleAlignment/BoosterCardsR9'
provenance=json.loads((doc/'GeneratedAssetProvenance.json').read_text())
checks=[]
for asset in provenance['assets']:
    source=np.asarray(Image.open(doc/asset['source']))
    installed=np.asarray(Image.open(root/asset['runtimePath']))
    x1,y1,x2,y2=asset['sourceCrop']
    equal=bool(np.array_equal(source[y1:y2,x1:x2,:3],installed[:,:,:3]))
    assert equal,asset['name']
    alpha=installed[:,:,3]
    checks.append({'name':asset['name'],'size':asset['size'],'rgbEqual':equal,
        'alphaMin':int(alpha.min()),'alphaMax':int(alpha.max()),
        'retainedPixels':int((alpha>0).sum()),'opaquePixels':int((alpha==255).sum())})
result={'exactRGBChecks':checks,
    'manualInspection':['Native source atlas and SeaPearl','3x source grids',
       'Each object on white','Final all-four native/display-size black, white and magenta proofs'],
    'notPerformedByArtTask':['Unity import/runtime verification','Physical device','Color calibration'],
    'newRuntimeFiles':[a['runtimePath'] for a in provenance['assets']]}
(doc/'QA'/'AssetChecks.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
