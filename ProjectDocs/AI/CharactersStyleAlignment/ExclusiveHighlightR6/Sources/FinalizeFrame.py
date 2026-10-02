from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;im=Image.open(b/'Inputs/card_normal_r6.png').convert('RGBA');raw=Image.open(b/'Scenario/card_normal_r6/booster_medallion_trial01.png').convert('RGBA').resize(im.size,Image.Resampling.LANCZOS);w,h=im.size
# The frame is independent of the source portrait, lettering, stars and corner badges.
clear=Image.new('L',im.size);ImageDraw.Draw(clear).rectangle((10,9,w-10,h-14),fill=255);clear=clear.filter(ImageFilter.GaussianBlur(.4));alpha=np.minimum(np.array(raw.getchannel('A')),255-np.array(clear));im.putalpha(Image.fromarray(alpha));out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned/card_normal_r6.png');im.save(out);clear.save(b/'InteriorRemovalMask.png')
record={'asset':str(out),'source':'Inputs/card_normal_r6.png','sourceRegion':json.loads((b/'Assessment.json').read_text()),'alpha':'Scenario Pixa background-removal + bounded local removal of occupied interior','rgbOriginalPreserved':bool(np.array_equal(np.array(im)[:,:,:3],np.array(Image.open(b/'Inputs/card_normal_r6.png').convert('RGB')))),'sha256':hashlib.sha256(out.read_bytes()).hexdigest(),'cuCost':3,'generation':'none; source-alpha extraction only','activeFrameReusedUnchanged':'card_border_r2'};(b.parent/'AssetProvenance.json').write_text(json.dumps(record,indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 bg=Image.new('RGBA',im.size,color+(255,));bg.alpha_composite(im);bg.convert('RGB').save(b.parent/'QA'/f'Alpha_{name}.png')
