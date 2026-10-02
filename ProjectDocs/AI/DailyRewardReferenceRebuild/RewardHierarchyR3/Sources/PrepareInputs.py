from pathlib import Path
from PIL import Image
import json,hashlib,shutil
b=Path(__file__).resolve().parent
src=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png').convert('RGB')
boxes={'chest':(528,832,842,1082),'milestone1':(224,1180,324,1270),'milestone2':(402,1178,509,1273),'milestone3':(585,1178,691,1273),'milestone4':(738,1164,890,1282),'plaque':(546,754,826,835),'badge':(60,1183,161,1305)}
atlas=Image.new('RGB',(800,650),'#202020');positions={'chest':(20,20),'milestone1':(370,30),'milestone2':(510,30),'milestone3':(650,30),'milestone4':(365,180),'plaque':(20,335),'badge':(400,335)}
for name,box in boxes.items():atlas.paste(src.crop(box),positions[name])
atlas.save(b/'Inputs/subjects.png')
repair=Image.new('RGB',(1024,1024),'black');repair.paste(src.crop((476,746,906,1101)),(30,30));repair.paste(src.crop(boxes['plaque']),(520,30));repair.paste(src.crop(boxes['badge']),(560,200));repair.save(b/'Inputs/panel.png')
(b/'Boxes.json').write_text(json.dumps({'source':boxes,'atlas':positions},indent=2))
if not (b/'BeforeAssetsHashes.json').exists():
 (b/'BeforeAssetsHashes.json').write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()},indent=2))
 shutil.copy2('Assets/LegendsOfRus/Resources/ProductionUI/Pages/DailyReward.prefab',b/'DailyReward_before.prefab.txt')
