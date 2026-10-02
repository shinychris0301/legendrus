from pathlib import Path
from PIL import Image
import json,hashlib,shutil
r=Path.cwd();b=Path(__file__).resolve().parent
src=Path('/home/k2/Downloads/match3final/chater map.png');shutil.copyfile(src,b/'OriginalReference.png');im=Image.open(src).convert('RGBA')
boxes={'banner':[77,116,875,389],'play':[232,1460,713,1663],'back':[8,1472,182,1628],'node':[369,1314,496,1431],'filled_stars':[373,1417,490,1464],'empty_stars':[436,665,540,704],'footer':[0,1590,941,1672]}
for n,box in boxes.items():im.crop(box).save(b/'Inputs'/f'{n}.png')
(b/'ExtractionPlan.json').write_text(json.dumps({'source':str(src),'sha256':hashlib.sha256(src.read_bytes()).hexdigest(),'boxes':boxes,'intent':'Original banner and Play lettering retained for Russian; native numbers and saved stars; isolate source UI alpha without regenerating controls. Footer may require bounded removal of adjacent controls. Clean scene/scroll path follows controls pass.'},indent=2))
base=b/'BeforeAssetsHashes.json'
if not base.exists():base.write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest()for p in (r/'Assets').rglob('*')if p.is_file()}))
shutil.copyfile(r/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab',b/'ChapterMap_before.prefab.txt')
