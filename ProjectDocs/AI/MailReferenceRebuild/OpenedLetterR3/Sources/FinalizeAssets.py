from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');src=Image.open(b/'Letter.png').convert('RGB');records={}
def save(name,im,description):
 p=out/(name+'.png');im.save(p);records[name]={'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'method':description}
im=Image.open(b/'Sheet.png').convert('RGBA');alpha=Image.open(b/'Scenario/guild_emblem/booster_medallion_trial01.png').convert('RGBA').getchannel('A');assert alpha.size==im.size;im.putalpha(alpha);save('guild_emblem_r3',im,'Original enlarged RGB with unmodified Scenario alpha; no recoloring or character generation.')
for name,box in [('letter_divider_r3',(101,559,840,591)),('letter_rule_r3',(101,1068,840,1080))]:
 im=src.crop(box).convert('RGBA');a=np.array(im);lum=a[:,:,:3].astype(float).mean(2);al=np.clip((143-lum)*4.2,0,255).astype('uint8')
 support=Image.new('L',im.size);d=ImageDraw.Draw(support);cy=im.height//2
 d.rectangle((0,cy-3,im.width,cy+3),fill=255)
 if name=='letter_divider_r3':d.polygon([(353,16),(369,0),(385,16),(369,31)],fill=255)
 im.putalpha(Image.fromarray(np.minimum(al,np.array(support))));im=im.resize((im.width*2,im.height*2),Image.Resampling.LANCZOS);save(name,im,{'sourceBounds':box,'method':'Original ink/metal strokes isolated by luminance inside bounded line/diamond support; no opaque paper crop.'})
thumbs=[]
for name in records:
 im=Image.open(out/(name+'.png'));im.thumbnail((620,310));thumbs.append((name,im))
h=sum(im.height+34 for _,im in thumbs);check=Image.new('RGB',(1920,h),(25,33,45));d=ImageDraw.Draw(check);y=0
for name,im in thumbs:
 for n,c in enumerate(['black','white','#ed00c6']):
  d.text((n*640+8,y+4),name,fill='white');bg=Image.new('RGBA',(624,im.height),c);bg.alpha_composite(im);check.paste(bg.convert('RGB'),(n*640+8,y+25))
 y+=im.height+34
check.save(b.parent/'QA/AlphaBackgrounds.png');(b/'AssetProvenance.json').write_text(json.dumps({'source':'Letter.png','sourceSha256':hashlib.sha256((b/'Letter.png').read_bytes()).hexdigest(),'assets':records,'reusedUnchanged':['MailAligned/preview_divider_r2.png','MailAligned/letter_frame.png','MailAligned/claim_ru_rebuild.png','MailAligned/claim_frame.png','Approved gameplay sapphire and shared booster artwork'],'limitations':'Source is raster artwork. Enlargement adds no detail; alpha may differ at fine contours. No exact-font claim.'},indent=2));print('Saved',len(records),'source sprites')
