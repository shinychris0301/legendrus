from pathlib import Path
from PIL import Image,ImageDraw
import json,hashlib
b=Path(__file__).resolve().parent.parent;root=b.parents[3];art=root/'Assets/LegendsOfRus/Resources/ProductionUI/MechanicsBoardArtR10';out=b/'Art';out.mkdir(exist_ok=True)
names=['bomb','line','spark','color','crate','crate_damaged','seal','seal_damaged','ice','ice_damaged','chain','chain_damaged'];records=[]
for group in range(3):
 sheet=Image.new('RGB',(768,4*276),(20,25,34));draw=ImageDraw.Draw(sheet)
 for row,name in enumerate(names[group*4:group*4+4]):
  p=art/(name+'.png');im=Image.open(p).convert('RGBA');alpha=im.getchannel('A');bbox=alpha.point(lambda a:255 if a>32 else 0).getbbox();hist=alpha.histogram();w,h=im.size
  center=alpha.crop((int(w*.4),int(h*.4),int(w*.6),int(h*.6)));records.append({'name':name,'size':[w,h],'alphaBoundsAbove32':bbox,'transparentPixels':hist[0],'partialAlphaPixels':sum(hist[1:255]),'centerAlphaExtrema':center.getextrema(),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
  im.thumbnail((256,256),Image.Resampling.LANCZOS)
  for col,color in enumerate(['black','white','#ff00bb']):
   tile=Image.new('RGBA',(256,256),color);tile.alpha_composite(im,((256-im.width)//2,(256-im.height)//2));sheet.paste(tile.convert('RGB'),(col*256,row*276+20))
  draw.text((5,row*276+3),name,fill='white')
 sheet.save(out/f'AlphaGroup{group+1}.png')
(b/'QA/AlphaAudit.json').write_text(json.dumps({'purpose':'Diagnostic composites only; generated production pixels untouched','sprites':records},indent=2))
print('Checked',len(records),'sprites; all have transparency:',all(r['transparentPixels']>0 for r in records))
