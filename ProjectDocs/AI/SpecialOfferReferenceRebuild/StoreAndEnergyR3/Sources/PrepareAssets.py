from pathlib import Path
from PIL import Image,ImageDraw,ImageChops
import numpy as np,json,hashlib,uuid
b=Path(__file__).resolve().parent;root=Path('Assets/LegendsOfRus/Resources/ProductionUI');out=root/'SpecialOfferAligned'
src=Path('ProjectDocs/AI/WorldMapReferenceRebuild/NavigationAndRegionsR2/Sources/Inputs/navigation.png')
source=Image.open(src).convert('RGBA');bounds=(751,147,921,219)
art=source.crop(bounds);mask=Image.open(root/'WorldMapReferenceAligned/nav_shop.png').convert('RGBA').crop((0,147,170,219)).getchannel('A')
# Isolate only the lower plaque, excluding the shop illustration and its vertical frame.
outline=Image.new('L',art.size);ImageDraw.Draw(outline).polygon([(0,33),(5,29),(8,21),(8,17),(13,13),(13,9),(17,6),(77,6),(82,2),(87,6),(149,6),(155,11),(158,17),(158,25),(164,29),(166,33),(160,38),(156,38),(156,47),(151,58),(97,58),(83,71),(68,58),(17,58),(9,51),(8,42),(4,40)],fill=255)
mask=ImageChops.multiply(mask,outline);art.putalpha(mask)
p=b/'UnusedStoreExtraction.png';art.save(p)
audit={'source':str(src),'sourceSHA256':hashlib.sha256(src.read_bytes()).hexdigest(),'sourceBounds':bounds,'size':art.size,'alpha':'Existing approved World Map Scenario extraction plus local plaque contour; excludes the medallion above','sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'rgbMismatchPixels':int(np.any(np.array(art)[:,:,:3]!=np.array(source.crop(bounds))[:,:,:3],axis=2).sum()),'newScenarioCalls':0,'newCU':0}
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(416,178),color);im=art.resize((340,144),Image.Resampling.NEAREST);sheet.paste(im,(20,20),im);sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
print(audit)
