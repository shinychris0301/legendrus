from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json
b=Path(__file__).resolve().parent;s=Image.open(b/'OriginalReference.png').convert('RGB')
# Five distinct original flags (the red flag is reused by two guild members).
boxes={'blue':(258,502,313,569),'red':(261,671,313,735),'green':(261,835,313,899),'purple':(261,1000,314,1064),'winter':(261,1326,313,1391)}
sheet=Image.new('RGB',(len(boxes)*280,320),'white');placements={}
for i,(name,box) in enumerate(boxes.items()):
 im=s.crop(box).resize(((box[2]-box[0])*4,(box[3]-box[1])*4),Image.Resampling.LANCZOS);x=i*280+28;y=24;sheet.paste(im,(x,y));placements[name]={'source':box,'sheet':[x,y,x+im.width,y+im.height]}
sheet.save(b/'Inputs/flags.png');(b/'FlagBounds.json').write_text(json.dumps(placements,indent=2))
# Source tab strip; restore empty faces from clean source donors, retain gold rails/jewel.
box=(69,337,854,440);tabs=s.crop(box)
donor=s.crop((496,519,545,582));navy=np.array(donor);navy=np.concatenate([navy,navy[:,::-1]],1);navy=np.concatenate([navy,navy[::-1]],0)
def surface(w,h):return Image.fromarray(np.tile(navy,(h//navy.shape[0]+1,w//navy.shape[1]+1,1))[:h,:w])
for x1,y1,x2,y2 in [(105,365,307,406),(365,363,559,407),(628,363,818,407)]:
 if x1==105:clean=s.crop((114,357,154,367)).resize((x2-x1,y2-y1),Image.Resampling.LANCZOS)
 else:clean=surface(x2-x1,y2-y1)
 tabs.paste(clean,(x1-box[0],y1-box[1]))
# Remove the illustrative request badge using the same tab's mirrored left corner.
patch=s.crop((332,337,388,384)).transpose(Image.Transpose.FLIP_LEFT_RIGHT);tabs.paste(patch,(549-box[0],0))
tabs.save(b/'Inputs/tabs.png')
# Empty outer enclosure retaining original rails, curled corners and center sapphires.
fbox=(43,415,903,1504);frame=s.crop(fbox);w,h=frame.size
frame.paste(surface(w-94,h-105),(47,38))
# Clear all footer controls/count while retaining bottom rail and corner scrolls.
frame.paste(surface(755,63),(50,1012))
# The right inner sapphire rail is decorative; no fake scroll interaction is added.
frame.paste(s.crop((834,514,863,1414)),(834-fbox[0],514-fbox[1]))
frame.save(b/'Inputs/frame.png')
(b/'PanelBounds.json').write_text(json.dumps({'tabs':box,'frame':fbox,'donor':[496,519,545,582],'note':'Hidden faces, badge-covered tab corner and rails reconstructed from source pixels; no generation of RGB'},indent=2))
