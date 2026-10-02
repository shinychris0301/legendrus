from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,hashlib,json,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned')
source=Image.open(b/'Sources/OriginalReference.png').convert('RGBA');box=(180,1458,773,1608)
blank=Image.open(art/'details.png').convert('RGBA');original=source.crop(box).resize(blank.size,Image.Resampling.LANCZOS)
# Reuse the approved source-registered Scenario isolation already used by English.
# Do not threshold away the dark carving inside the gold ornamentation.
im=original.copy();im.putalpha(blank.getchannel('A'));p=art/'details_ru_r7.png';im.save(p)
meta=Path(str(p)+'.meta')
if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/'details.png.meta').read_text(),count=1))
assert np.array_equal(np.array(im)[:,:,:3],np.array(original)[:,:,:3]);assert np.array_equal(np.array(im)[:,:,3],np.array(blank)[:,:,3])
sheet=Image.new('RGB',(1200,390));d=ImageDraw.Draw(sheet)
for i,(n,asset) in enumerate([('RU original lettering',im),('EN existing blank face',blank)]):
 for j,c in enumerate(['black','white','#ff00dd']):
  panel=Image.new('RGBA',(400,170),c);thumb=asset.copy();thumb.thumbnail((385,160));panel.alpha_composite(thumb,((400-thumb.width)//2,(170-thumb.height)//2));sheet.paste(panel.convert('RGB'),(j*400,i*195))
 d.text((8,i*195+174),n,fill='white')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(dict(sourceBox=box,oldRUBox=[180,1458,761,1600],outputSize=im.size,sourceRGBPreservedAfterUniformUpsampling=True,alphaIdenticalToApprovedEnglish=True,alphaSource='ProductionUI/TeamRankingAligned/details.png',alphaSourceSHA256=hashlib.sha256((art/'details.png').read_bytes()).hexdigest(),assetSHA256=hashlib.sha256(p.read_bytes()).hexdigest(),scenarioJobs=0,notes='Existing Scenario isolation reused; no new repair/generation. Source Russian lettering preserved. Both languages use native 593x150.'),indent=2)+'\n')
