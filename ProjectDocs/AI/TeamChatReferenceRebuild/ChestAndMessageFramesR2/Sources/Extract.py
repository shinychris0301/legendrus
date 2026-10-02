from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];src=Image.open(b/'Sources/OriginalReference.png').convert('RGB');art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned');records=[];outputs=[]
def save(key,im,desc):
 p=art/(key+'_r2.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'title_ru.png.meta').read_text(),count=1))
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':desc});outputs.append((key,im))
def patch(im,box,donor):im.paste(src.crop(donor).resize((box[2]-box[0],box[3]-box[1]),Image.Resampling.LANCZOS),box[:2])
def navy(w,h):
 # Original unoccupied dark navy gap; reflected tiling avoids stretched columns.
 tile=np.array(src.crop((226,910,682,934)));a=np.concatenate([tile,tile[:,::-1]],axis=1);a=np.concatenate([a,a[::-1]],axis=0)
 return Image.fromarray(np.tile(a,(h//a.shape[0]+1,w//a.shape[1]+1,1))[:h,:w])
# Separate frame, no text, portrait or screenshot content in the opaque interior.
im=src.crop((24,276,912,1404));im.paste(navy(832,1114),(30,7));patch(im,(30,0,856,15),(240,276,431,291));patch(im,(30,1121,856,1128),(218,1397,633,1404))
# Preserve narrow source side rails while clearing the top title/crest overlap at inner edge.
im=im.convert('RGBA');mask=Image.new('L',im.size,0);ImageDraw.Draw(mask).rounded_rectangle((2,0,887,1127),radius=14,fill=255);im.putalpha(mask);save('chat_surround',im.resize((1776,2256),Image.Resampling.LANCZOS),'Original side/bottom rails; opaque navy interior reconstructed from clean source gap; top rail source donor; estimated outer contour')
im=src.crop((58,278,881,411));im.paste(navy(787,101),(18,18));patch(im,(20,0,803,20),(240,278,431,298));patch(im,(20,117,803,133),(235,395,618,411));patch(im,(808,16,823,68),(866,340,881,392));im=im.convert('RGBA');mask=Image.new('L',im.size,0);ImageDraw.Draw(mask).rounded_rectangle((0,0,822,132),radius=18,fill=255);im.putalpha(mask);save('chest_panel',im.resize((1646,266),Image.Resampling.LANCZOS),'Source enclosure rails with bounded replacement under crest/chest; source navy donor interior; approximate rounded exterior')
im=src.crop((182,935,878,1015));im.paste(navy(659,65),(24,7));im=im.convert('RGBA');mask=Image.new('L',im.size,0);ImageDraw.Draw(mask).rounded_rectangle((0,0,695,79),radius=9,fill=255);im.putalpha(mask);save('system_panel',im.resize((1392,160),Image.Resampling.LANCZOS),'Original system-row rail with original text removed using source navy donor; intentionally opaque')
a=json.loads((b/'Sources/Assessment.json').read_text())
for key,box in a['isolations'].items():
 folder=b/'Sources/Scenario'/key;files=list(folder.glob('*.png'))
 if not files:continue
 result=Image.open(files[0]).convert('RGBA');raw=src.crop(box);size=(raw.width*4,raw.height*4);rgb=raw.resize(size,Image.Resampling.LANCZOS);alpha=result.getchannel('A').resize(size,Image.Resampling.LANCZOS)
 # Clip unrelated neighboring rails, keeping opaque interiors of heraldry and info face.
 support=Image.new('L',size,0);d=ImageDraw.Draw(support)
 if key=='guild_crest':pts=[(1,41),(16,25),(46,18),(65,0),(89,18),(125,25),(136,44),(129,111),(97,139),(67,158),(27,135),(7,108)]
 elif key=='system_crest':pts=[(8,31),(27,18),(53,2),(74,4),(106,23),(121,49),(112,76),(83,95),(61,102),(30,86),(3,63)]
 else:pts=None
 if pts:d.polygon([(x*4,y*4)for x,y in pts],fill=255)
 else:d.ellipse((2*4,2*4,(raw.width-2)*4,(raw.height-2)*4),fill=255)
 aa=np.minimum(np.array(alpha),np.array(support));out=rgb.convert('RGBA');out.putalpha(Image.fromarray(aa));assert out.convert('RGB').tobytes()==rgb.tobytes();save(key,out,'Original RGB with Scenario alpha and bounded support removing neighboring rails')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,indent=2))
sheet=Image.new('RGB',(1080,len(outputs)*200),(25,28,34));d=ImageDraw.Draw(sheet)
for i,(key,im) in enumerate(outputs):
 d.text((6,i*200+2),key,fill='white')
 for j,color in enumerate(('black','white','#eb00b0')):
  tile=Image.new('RGBA',(352,172),color);v=im.copy();v.thumbnail((340,166));tile.alpha_composite(v,((352-v.width)//2,(172-v.height)//2));sheet.paste(tile.convert('RGB'),(j*360,i*200+22))
sheet.save(b/'QA/AssetAlpha.png')
