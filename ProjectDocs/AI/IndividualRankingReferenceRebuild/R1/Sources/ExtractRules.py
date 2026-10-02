"""Original lettering and button face; reuse the accepted isolated button alpha."""
from pathlib import Path
import hashlib,json,re,uuid
import numpy as np
from PIL import Image

base=Path(__file__).resolve().parents[1]
art=Path('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned')
source=Image.open(base/'Sources/OriginalReference.png').convert('RGB')
box=(247,1548,697,1629)
rgb=source.crop(box).convert('RGBA')
mask=Image.open(art/'rules.png').getchannel('A').resize(rgb.size,Image.Resampling.LANCZOS)
rgb.putalpha(mask)
path=art/'rules_original_ru.png'
rgb.save(path)
meta=Path(str(path)+'.meta')
if not meta.exists():
    text=(art/'rules.png.meta').read_text()
    text=re.sub(r'guid: [a-f0-9]{32}', 'guid: '+uuid.uuid4().hex,text,count=1)
    meta.write_text(text)
assert np.array_equal(np.asarray(rgb)[:,:,:3],np.asarray(source.crop(box)))
sheet=Image.new('RGB',(450,81*3))
for i,color in enumerate(('black','white','#ff00dd')):
    bg=Image.new('RGBA',rgb.size,color);bg.alpha_composite(rgb);sheet.paste(bg.convert('RGB'),(0,i*81))
sheet.save(base/'QA/RulesAlpha.png')
(base/'Sources/AssetProvenance.json').write_text(json.dumps({
    'source':'/home/k2/Downloads/match3final/individual ranking.png',
    'sourceSHA256':hashlib.sha256((base/'Sources/OriginalReference.png').read_bytes()).hexdigest(),
    'bounds':box,'asset':str(path),'rgb':'unchanged original pixels',
    'alpha':'accepted rules sprite alpha, downsampled to original resolution',
    'scenarioJobs':0,'creditsUsed':0,
    'limitations':'Inherited fine exterior alpha remains an estimate; no layered original available.'
},indent=2)+'\n')
