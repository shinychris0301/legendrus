from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,json,hashlib
root=Path.cwd();b=root/'ProjectDocs/AI/TutorialStyleAlignment/NativeDesignR3';s=b/'Sources'
source=Image.open(root/'ProjectDocs/AI/TutorialStyleAlignment/DesignR2/Tutorial_Help_Design_R2.png').convert('RGB')
w,h=source.size;rgb=np.array(source);mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
d.rectangle((81,268,943,1667),fill=255);d.rectangle((155,163,883,280),fill=255)
# Reuse the original approved navy surface; reflected tiling avoids stretching its texture.
original=Image.open(root/'ProjectDocs/AI/TutorialStyleAlignment/R1/Sources/OriginalReference.png')
tile=original.crop((84,1210,160,1320));tw,th=tile.size;tile2=Image.new('RGB',(tw*2,th*2));tile2.paste(tile,(0,0));tile2.paste(ImageOps.mirror(tile),(tw,0));tile2.paste(ImageOps.flip(tile),(0,th));tile2.paste(ImageOps.flip(ImageOps.mirror(tile)),(tw,th))
patch=Image.new('RGB',(w,h))
for y in range(0,h,th*2):
 for x in range(0,w,tw*2):patch.paste(tile2,(x,y))
a=np.array(mask);rgb[a>0]=np.array(patch)[a>0]
# The Close is a separate interactive sprite. Mirror the unoccupied shoulder under it.
rgb[80:282,844:1024]=rgb[80:282,0:180][:,::-1];a[80:282,844:1024]=255
Image.fromarray(rgb).save(s/'Inputs/panel_r3.png');Image.fromarray(a).save(s/'RepairMask.png')
p=s/'Inputs/panel_r3.png'
(s/'Assessment.json').write_text(json.dumps({'assets':[{'name':'panel_r3','inputSha256':hashlib.sha256(p.read_bytes()).hexdigest()}],'source':'../../DesignR2/Tutorial_Help_Design_R2.png','operation':'Retain generated concept perimeter; replace occupied interior using approved original navy texture; mirror shoulder beneath separate Close control. Scenario alpha only.'},indent=2))
print('Prepared blank frame for isolation')
