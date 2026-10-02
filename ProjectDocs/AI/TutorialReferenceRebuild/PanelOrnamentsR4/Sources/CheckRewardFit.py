from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parents[1];root=Path('Assets/LegendsOfRus/Resources/ProductionUI');protected=np.array(Image.open(b/'Sources/ProtectedOrnaments.png'))>0
records=[]
for name,path,rect in [('gift','HomeReferenceAligned/source_gift.png',(250,1304,130,86)),('coin','MailAligned/coin.png',(432,1302,110,80)),('gem','GameplayReferenceAligned/gem_sapphire.png',(610,1297,91,91))]:
 im=Image.open(root/path).convert('RGBA');x,y,w,h=rect;scale=min(w/im.width,h/im.height);sz=round(im.width*scale),round(im.height*scale);a=im.getchannel('A').resize(sz,Image.Resampling.LANCZOS);mask=Image.new('L',(788,1121));mask.paste(a,(round(x+(w-sz[0])/2)-76,round(y+(h-sz[1])/2)-304));overlap=int(((np.array(mask)>16)&protected).sum());records.append({'asset':name,'overlapWithProtectedFramePixels':overlap});assert overlap==0,records[-1]
(b/'QA/RewardFitChecks.json').write_text(json.dumps({'check':'Conservative offline alpha footprints using native preserveAspect geometry, not a substitute for Unity capture','results':records},indent=2)+'\n');print(records)
