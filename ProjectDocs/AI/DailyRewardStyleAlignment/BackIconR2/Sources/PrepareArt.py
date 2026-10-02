from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib
b=Path('ProjectDocs/AI/DailyRewardStyleAlignment/BackIconR2')
s=Image.open(b/'Sources/BackSource.png').convert('RGBA');mask=Image.open(b/'Sources/Scenario/booster_medallion_trial01.png').convert('RGBA');mask=mask.resize(s.size,Image.Resampling.LANCZOS)
s.putalpha(mask.getchannel('A'));assert np.all(np.array(s)[35:95,35:105,3]==255)
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned/back_r2.png');s.save(out)
for name,color in [('Black','black'),('White','white'),('Magenta','#ff00b0')]:
 bg=Image.new('RGBA',s.size,color);bg.alpha_composite(s);bg.resize((560,532)).save(b/f'QA/BackOn{name}.png')
(b/'Sources/Provenance.json').write_text(json.dumps({'source':'Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png','region':[10,1504,150,1637],'scenarioJob':json.loads((b/'Sources/Scenario/Job.json').read_text())['id'],'billing':json.loads((b/'Sources/Scenario/Billing.json').read_text()),'originalRGBPreserved':True,'runtime':str(out),'sha256':hashlib.sha256(out.read_bytes()).hexdigest()},indent=2)+'\n')
