from pathlib import Path
from PIL import Image
import numpy as np,json,hashlib,sys
b=Path(__file__).resolve().parents[1];out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');sheet=Image.open(b/'Sources/Sheet.png').convert('RGB');record={}
for item in json.loads((b/'Sources/Sheet.json').read_text()):
 key=item['name'];name='letter_title_ru_r5' if key=='letter_title' else key+'_r5'
 source=np.array(sheet.crop(item['sheetBox']));asset=np.array(Image.open(out/(name+'.png')).convert('RGBA'));visible=asset[:,:,3]>0
 assert np.array_equal(source[visible],asset[:,:,:3][visible]),name+' RGB changed'
 record[name]={'originalVisibleRgbUnchanged':True,'visiblePixels':int(visible.sum())}
run=Path('/tmp/lor-mail-letter-r5')/sys.argv[1];previous=b.parent/'ParchmentBlendR4/UnityCaptures';record['inboxComparisons']={}
for name in ('Mail_941x1672_RU.png','Mail_941x1672_EN.png','Mail_576x1280_safe_RU.png','Mail_576x1280_safe_EN.png','Mail_System_RU.png','Mail_EmptySent_RU.png'):
 a=np.array(Image.open(previous/name));c=np.array(Image.open(run/'Mail'/name));changed=np.any(a!=c,axis=2);record['inboxComparisons'][name]={'changedPixels':int(changed.sum())}
record['parchmentUnchanged']=all(hashlib.sha256((out/(n+'_surface_r4.png')).read_bytes()).hexdigest()==json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())[str(out/(n+'_surface_r4.png'))] for n in ('letter','preview'));assert record['parchmentUnchanged']
(b/'QA/IntegrityChecks.json').write_text(json.dumps(record,indent=2));print(json.dumps(record,indent=2))
