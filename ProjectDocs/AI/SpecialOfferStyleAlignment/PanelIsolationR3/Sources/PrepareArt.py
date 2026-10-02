"""Keep panel RGB and replace the old polygon mask with fresh Scenario alpha."""
from pathlib import Path
from PIL import Image
import numpy as np, json, hashlib
b=Path('ProjectDocs/AI/SpecialOfferStyleAlignment/PanelIsolationR3')
s=Image.open(b/'Sources/PanelBefore.png').convert('RGBA')
m=Image.open(b/'Sources/Scenario/booster_medallion_trial01.png').convert('RGBA')
assert s.size==m.size
before=np.array(s);alpha=np.array(m)[:,:,3]
assert np.all(alpha[90:-90,90:-90]==255),'Opaque panel face must remain intact'
s.putalpha(m.getchannel('A'))
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned/card.png');s.save(out)
assert np.array_equal(np.array(s)[:,:,:3],before[:,:,:3])
for name,color in [('Black','black'),('White','white'),('Magenta','#ff00b0')]:
 bg=Image.new('RGBA',s.size,color);bg.alpha_composite(s);bg.save(b/f'QA/PanelOn{name}.png')
manifest={'runtime':str(out),'sha256':hashlib.sha256(out.read_bytes()).hexdigest(),'source':'Sources/PanelBefore.png','scenarioJob':json.loads((b/'Sources/Scenario/Job.json').read_text())['id'],'model':'model_pixa-background-removal','billing':json.loads((b/'Sources/Scenario/Billing.json').read_text()),'originalRGBExact':True,'opaqueFacePreserved':True,'dimensions':s.size,'changedAlphaPixels':int(np.count_nonzero(alpha!=before[:,:,3])),'preserveExistingMeta':True}
(b/'Sources/RuntimeProvenance.json').write_text(json.dumps(manifest,indent=2)+'\n');print(json.dumps(manifest,indent=2))
