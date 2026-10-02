"""Extract original button RGB using the already approved matching silhouette mask.
No generation, recoloring, upscaling or replacement lettering is used.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageChops
from collections import deque
import json,hashlib,uuid,re
import numpy as np
root=Path.cwd();base=Path(__file__).resolve().parent.parent
art=root/'Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned'
source=root/'Assets/LegendsOfRus/Resources/DesignScreens/Events.png'
src=Image.open(source).convert('RGBA');alpha=Image.open(art/'action_r3.png').convert('RGBA').getchannel('A')
# Bound the donor mask to the button itself; exclude the neighbouring card curl
# and the old donor's opaque lower rectangular fringe.
contour=[(13,0),(205,0),(221,8),(221,22),(225,35),(222,43),(219,57),(208,68),(197,72),(27,72),(10,67),(4,57),(5,45),(0,39),(4,29),(5,13)]
mask=Image.new('L',(900,320));ImageDraw.Draw(mask).polygon([(x*4,y*4) for x,y in contour],fill=255)
alpha=ImageChops.darker(alpha,mask.resize((225,80),Image.Resampling.LANCZOS))
# Remove disconnected specks left by the neighbouring card ornament.
a=np.array(alpha);seen=np.zeros(a.shape,bool);groups=[]
for y,x in zip(*np.where(a>4)):
    if seen[y,x]:continue
    todo=deque([(y,x)]);seen[y,x]=True;group=[]
    while todo:
        yy,xx=todo.popleft();group.append((yy,xx))
        for dy,dx in ((-1,0),(1,0),(0,-1),(0,1)):
            ny,nx=yy+dy,xx+dx
            if 0<=ny<80 and 0<=nx<225 and not seen[ny,nx] and a[ny,nx]>4:seen[ny,nx]=True;todo.append((ny,nx))
    groups.append(group)
keep=np.zeros(a.shape,bool)
for yy,xx in max(groups,key=len):keep[yy,xx]=True
# Retain adjacent low-alpha antialias pixels, clear remote islands.
for y,x in zip(*np.where((a>0)&(a<=4))):
    if keep[max(0,y-1):y+2,max(0,x-1):x+2].any():keep[y,x]=True
alpha=Image.fromarray(np.where(keep,a,0).astype('uint8'))
(base/'QA').mkdir(exist_ok=True)
records=[];previews=[]
for i,y in enumerate([632,972,1250,1528]):
    box=(674,y,899,y+80);cut=src.crop(box);cut.putalpha(alpha)
    name=f'action_lettered_{i}_r9';p=art/(name+'.png');cut.save(p)
    meta=Path(str(p)+'.meta')
    if not meta.exists():
        m=(art/'action_r3.png.meta').read_text();m=re.sub(r'^guid: \w+', 'guid: '+uuid.uuid4().hex,m,flags=re.M);meta.write_text(m)
    a=np.array(cut);original=np.array(src.crop(box));visible=a[:,:,3]>0
    records.append({'asset':str(p.relative_to(root)),'box':box,'originalVisibleRGBMismatches':int(np.any(a[:,:,:3][visible]!=original[:,:,:3][visible],axis=1).sum()),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'alphaDonor':'action_r3.png'})
    previews.append(cut)
# Source lettering extracted by its gold/chrome contrast inside tightly bounded
# text-only rectangles. The existing normal/selected frames remain unchanged.
for i,box in enumerate([(107,331,260,361),(426,331,516,361),(659,331,864,361)]):
    letters=src.crop(box);arr=np.array(letters).astype(float)
    contrast=np.minimum(arr[:,:,0]-arr[:,:,2]*1.3,arr[:,:,1]-arr[:,:,2]*1.05)
    letterAlpha=np.clip((contrast-12)/24,0,1)*255
    letters.putalpha(Image.fromarray(letterAlpha.astype('uint8')))
    for selected in (False,True):
        plate=Image.open(art/('tab_selected_r7.png' if selected else 'tab_r3.png')).convert('RGBA')
        # Frame sizes differ by two source pixels; lettering stays native resolution.
        x=(plate.width-letters.width)//2;y=25
        plate.alpha_composite(letters,(x,y))
        name=f'tab_lettered_{i}_{"selected" if selected else "normal"}_r9';p=art/(name+'.png');plate.save(p)
        meta=Path(str(p)+'.meta')
        if not meta.exists():
            m=(art/'tab_selected_r7.png.meta').read_text();m=re.sub(r'^guid: \w+', 'guid: '+uuid.uuid4().hex,m,flags=re.M);meta.write_text(m)
        records.append({'asset':str(p.relative_to(root)),'letterSourceBox':box,'method':'Source gold lettering over approved tab state; no generated text','sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
canvas=Image.new('RGB',(960,410),(25,29,35));d=ImageDraw.Draw(canvas)
for row,(col,label) in enumerate([('black','BLACK'),('white','WHITE'),('#eb00c8','MAGENTA')]):
    y=30+row*130;d.text((10,y-20),label,fill='white');d.rectangle((0,y,960,y+100),fill=col)
    for i,im in enumerate(previews):canvas.paste(im,(8+i*238,y+10),im)
canvas.save(base/'QA/ButtonAlpha.png')
(base/'Sources/Provenance.json').write_text(json.dumps({'source':str(source.relative_to(root)),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'method':'Unmodified source crop RGB plus existing approved action silhouette; transparent exterior, opaque green face and original Cyrillic. No generative edit.','assets':records},indent=2)+'\n')
