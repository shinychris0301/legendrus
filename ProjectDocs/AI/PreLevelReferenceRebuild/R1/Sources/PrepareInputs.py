from pathlib import Path
from PIL import Image
import hashlib,json,shutil
b=Path(__file__).resolve().parent;r=Path.cwd();src=Path('/home/k2/Downloads/match3final/Pre-level.png');shutil.copyfile(src,b/'OriginalReference.png');im=Image.open(src).convert('RGBA')
boxes={'goals':[208,382,737,735],'moves':[365,675,579,881],'boosters':[120,819,823,1139],'rewards':[122,1133,820,1469],'mace':[154,926,324,1105],'firebird':[317,926,488,1105],'shield':[478,926,650,1105],'potion':[635,926,793,1105],'chest':[232,1210,450,1418]}
for n,box in boxes.items():im.crop(box).save(b/'Inputs'/f'{n}.png')
(b/'ExtractionPlan.json').write_text(json.dumps({'source':str(src),'sha256':hashlib.sha256(src.read_bytes()).hexdigest(),'boxes':boxes,'reuse':'Home shared header; Chapter Map banner/play/back/footer extracted controls','live':'All mission values, inventory quantities, rewards, English and body headings remain native','reconstruction':'Only scenery hidden by interface requires generation. Original RGB retained under Scenario alpha for foreground. Panel contents and sample numerals removed locally with bounded masks.'},indent=2))
p=b/'BeforeAssetsHashes.json'
if not p.exists():p.write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in (r/'Assets').rglob('*') if p.is_file()}))
shutil.copyfile(r/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/PreLevel.prefab',b/'PreLevel_before.prefab.txt')
