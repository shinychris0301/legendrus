"""Install inspected clean Scenario scenery without pixel edits or GUID changes."""
from pathlib import Path
import shutil,hashlib,json
from PIL import Image
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/SceneryR4';p=o/'Sources/ScenarioScene/booster_medallion_trial01.png'
im=Image.open(p).convert('RGBA');assert im.size==(1024,1024);assert im.getchannel('A').getextrema()==(255,255)
shutil.copy2(p,o/'Staging/environment.png');shutil.copy2(p,r/'Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/environment.png')
(o/'QA/ExtractionAudit.json').write_text(json.dumps([{'name':'environment','size':im.size,'role':'Complete clean environment reconstructed by Scenario from scene reference; no source UI or actor pixels pasted into runtime plate','sha256':hashlib.sha256(p.read_bytes()).hexdigest()}],indent=2)+'\n')
print('Installed one opaque environment; existing meta retained.')
