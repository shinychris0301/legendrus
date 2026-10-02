from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];old=Path('ProjectDocs/AI/FriendsListStyleAlignment/R1');art=Path('Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned');source=Image.open(b/'Sources/OriginalReference.png').convert('RGB');records=[];previews=[]
def save(name,im,note):
 p=art/(name+'.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'visit_frame.png.meta').read_text(),count=1))
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':note});previews.append((name,im))
for r in json.loads((old/'Sources/Assessment.json').read_text())['assets']:
 key=r['name']
 if key not in ('energy_frame','visit_frame') and not key.startswith('nav_'):continue
 accepted=Image.open(art/(key+'.png')).convert('RGBA');rgb=source.crop(r['box']).resize(accepted.size,Image.Resampling.LANCZOS);out=rgb.convert('RGBA');out.putalpha(accepted.getchannel('A'))
 assert out.getchannel('A').tobytes()==accepted.getchannel('A').tobytes();assert out.convert('RGB').tobytes()==rgb.tobytes()
 save(key.replace('_frame','')+'_ru_rebuild',out,'Original source RGB and Russian lettering restored under unchanged accepted Scenario-derived alpha')
raw=Image.open(b/'Sources/Inputs/row.png').convert('RGB');size=(raw.width*2,raw.height*2);out=raw.resize(size,Image.Resampling.LANCZOS).convert('RGBA');a=Image.open(b/'Sources/Scenario/row/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS)
# Intentionally opaque navy panel face; only the exterior is transparent.
mask=Image.new('L',size);ImageDraw.Draw(mask).polygon([(0,24),(24,0),(size[0]-25,0),(size[0]-1,24),(size[0]-1,size[1]-25),(size[0]-25,size[1]-1),(24,size[1]-1),(0,size[1]-25)],fill=255);a=Image.fromarray(np.maximum(np.array(a),np.array(mask)));out.putalpha(a);save('row_rebuild',out,'Source rails and empty navy donor reconstruction; Scenario alpha with local silhouette restoration of thin rails and intentionally opaque interior; hidden rail continuation is approximate')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
sheet=Image.new('RGB',(1050,len(previews)*170),(25,28,32));d=ImageDraw.Draw(sheet)
for i,(name,im) in enumerate(previews):
 d.text((5,i*170+2),name,fill='white')
 for j,color in enumerate(('black','white','#ec00b5')):
  tile=Image.new('RGBA',(345,145),color);small=im.copy();small.thumbnail((337,139));tile.alpha_composite(small,((345-small.width)//2,(145-small.height)//2));sheet.paste(tile.convert('RGB'),(j*350,i*170+22))
sheet.save(b/'QA/AssetAlpha.png')
