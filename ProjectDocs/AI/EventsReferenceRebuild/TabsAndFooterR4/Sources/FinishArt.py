from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned');q=b.parent/'QA'
g=Image.open(b/'Scenario/booster_medallion_trial01.png').convert('RGBA')
stats={};out={}
for key,box,size,rects in [('Tab',(87,24,936,246),(283,74),[(65,23,219,60),(240,-5,290,38)]),('Footer',(41,394,982,452),(941,58),[(225,7,714,38)])]:
 src=Image.open(b/(key+'Source.png')).convert('RGBA');fill=g.crop(box).resize(size,Image.Resampling.LANCZOS)
 m=Image.new('L',size);d=ImageDraw.Draw(m)
 for r in rects:d.rectangle(r,fill=255)
 m=m.filter(ImageFilter.GaussianBlur(1.6));m.save(b/(key+'RepairMask.png'))
 result=Image.composite(fill,src,m)
 if key=='Tab':
  # Accepted source-isolated normal tab supplies the same outer UI contour.
  alpha=Image.open(art/'tab_r3.png').convert('RGBA').resize(size,Image.Resampling.LANCZOS).getchannel('A')
 else:
  # Footer contains intentional opaque black/navy lacquer; clear only the outer corners below carved silhouette.
  alpha=Image.new('L',size,255);d=ImageDraw.Draw(alpha)
  d.polygon([(0,45),(12,52),(22,58),(0,58)],fill=0);d.polygon([(941,45),(929,52),(919,58),(941,58)],fill=0)
 result.putalpha(alpha)
 out[key]=result
 path=art/('tab_selected_r7.png' if key=='Tab' else 'footer_r7.png');result.save(path)
 a=np.array(result);s=np.array(src);mask=np.array(m)
 visible=(mask==0)&(a[:,:,3]>0)
 stats[key]={'retainedSourcePixels':int(visible.sum()),'mismatchedRGB':int(np.any(a[:,:,:3][visible]!=s[:,:,:3][visible],axis=1).sum()),'repairedMaskPixels':int((mask>0).sum()),'totalPixels':size[0]*size[1]}
 # Clearly labelled QA artifact, not a game screen.
 canvas=Image.new('RGB',(max(size[0],850),size[1]*3+96),(30,35,40));draw=ImageDraw.Draw(canvas)
 for j,(colour,label) in enumerate([((0,0,0),'BLACK'),((255,255,255),'WHITE'),((235,0,200),'MAGENTA')]):
  y=32+j*(size[1]+20);canvas.paste(colour,(0,y,canvas.width,y+size[1]));canvas.paste(result,(0,y),result);draw.text((5,y-16),label,fill='white')
 canvas.save(q/(key+'Alpha.png'))
(b/'ArtAudit.json').write_text(json.dumps(stats,indent=2))
print(json.dumps(stats,indent=2))
