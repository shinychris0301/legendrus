from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,json,hashlib
base=Path('ProjectDocs/AI/SpecialOfferStyleAlignment/R1');out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned');out.mkdir(exist_ok=True)
source=Path('Assets/LegendsOfRus/Resources/ProductionUI/PreLevelAligned/chest.png')
frame=Path('ProjectDocs/AI/ShopStyleAlignment/R1/Sources/Scenario/card/booster_medallion_trial01.png');im=ImageOps.fit(Image.open(frame).convert('RGBA'),(862,982));m=Image.new('L',(3448,3928));pts=[(25,4),(405,4),(427,24),(427,466),(406,488),(25,488),(3,466),(3,27)];ImageDraw.Draw(m).polygon([(x*8,y*8) for x,y in pts],fill=255);im.putalpha(m.resize(im.size,Image.Resampling.LANCZOS));im.save(out/'card.png')
manifest={'newScenarioCalls':0,'newScenarioCU':0,'chest':{'source':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'change':'Reuse original approved isolated high-resolution PreLevel chest directly, with unchanged RGB/alpha/GUID. Shop chest crop was rejected for a clipped bottom edge.'},'card':{'source':str(frame),'sha256':hashlib.sha256(frame.read_bytes()).hexdigest(),'change':'Reused original1024x1152 Scenario empty-card generation at862x982 with same traced opaque-face silhouette; avoids enlarging Shop431x491 resource.'}}
(base/'Sources/ArtProvenance.json').write_text(json.dumps(manifest,indent=2)+'\n')
for bg in ['black','white','#ff00aa']:
 qa=Image.new('RGB',(1000,650),bg)
 for i,name in enumerate(['chest','card']):
  im=Image.open(source if name=='chest' else out/(name+'.png'));im.thumbnail((480,620));qa.paste(im,(i*500,10),im)
 qa.save(base/'QA'/('Alpha_'+bg.replace('#','')+'.jpg'))
