from pathlib import Path
from PIL import Image
import hashlib,json
b=Path(__file__).resolve().parent;src=Image.open(b/'clock_api_input.png').convert('RGBA');api=Image.open(b/'ScenarioClock/booster_medallion_trial01.png').convert('RGBA')
a=api.getchannel('A').resize(src.size,Image.Resampling.LANCZOS);src.putalpha(a)
bounds=a.getbbox();sprite=src.crop(bounds)
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/clock_r2.png');sprite.save(out)
q=b.parent/'QA';q.mkdir(exist_ok=True)
qa=Image.new('RGB',(sprite.width*3,sprite.height))
for i,c in enumerate(['black','white','#ff00ff']):
 tile=Image.new('RGBA',sprite.size,c);tile.alpha_composite(sprite);qa.paste(tile.convert('RGB'),(i*sprite.width,0))
qa.save(q/'Clock_AlphaQA.png')
(b.parent/'AssetProvenance.json').write_text(json.dumps({'source':'Assets/LegendsOfRus/Resources/DesignScreens/Events.png','sourceSHA256':hashlib.sha256(Path('Assets/LegendsOfRus/Resources/DesignScreens/Events.png').read_bytes()).hexdigest(),'sourceBounds':[435,494,477,536],'apiInput':'Sources/clock_api_input.png','processing':'Original source RGB enlarged 8x using Lanczos, Scenario alpha applied without recoloring; transparent bounds trimmed. No surrounding reference scenery retained.','alphaBounds':bounds,'output':str(out),'outputSHA256':hashlib.sha256(out.read_bytes()).hexdigest(),'otherArt':'All existing sprites unchanged; individual paintings framed by native aspect-fill masks.'},indent=2)+'\n')
print('clock',sprite.size,'alpha',a.getextrema())
