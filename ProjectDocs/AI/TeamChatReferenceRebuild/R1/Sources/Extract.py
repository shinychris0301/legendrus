from pathlib import Path
from PIL import Image,ImageDraw
import hashlib,json,re,uuid
b=Path(__file__).resolve().parents[1];root=Path.cwd();art=root/'Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned'
src=Image.open(b/'Sources/OriginalReference.png').convert('RGB')
items=[('send','send_frame',(679,1406,898,1486)),('footer_gift','footer_gift',(85,1492,305,1644)),('footer_help','footer_help',(361,1484,602,1645)),('footer_sticker','footer_sticker',(638,1493,898,1645))]
records=[];sheet=Image.new('RGB',(1000,4*220),(20,25,32));draw=ImageDraw.Draw(sheet)
for n,(key,maskkey,box) in enumerate(items):
 mask=Image.open(art/(maskkey+'.png')).convert('RGBA');rgb=src.crop(box).resize(mask.size,Image.Resampling.LANCZOS);out=rgb.convert('RGBA');out.putalpha(mask.getchannel('A'));dest=art/(key+'_ru_rebuild.png');out.save(dest)
 meta=Path(str(dest)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/(maskkey+'.png.meta')).read_text(),count=1))
 assert out.convert('RGB').tobytes()==rgb.tobytes()
 records.append({'asset':str(dest.relative_to(root)),'sourceBox':box,'alphaFrom':str((art/(maskkey+'.png')).relative_to(root)),'sha256':hashlib.sha256(dest.read_bytes()).hexdigest(),'sourceRGBPreservedAfterResizing':True})
 for j,color in enumerate(('black','white','#e800b7')):
  tile=Image.new('RGBA',(320,180),color);preview=out.copy();preview.thumbnail((310,172));tile.alpha_composite(preview,((320-preview.width)//2,(180-preview.height)//2));sheet.paste(tile.convert('RGB'),(j*333,n*220+25))
 draw.text((5,n*220+5),key,fill='white')
sheet.save(b/'QA/AssetAlpha.png');(b/'Sources/AssetProvenance.json').write_text(json.dumps({'sourceSha256':hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),'newScenarioJobs':0,'costCU':0,'upstream':'TeamChatStyleAlignment/R1 Scenario alpha isolation; accepted masks reused unchanged','assets':records},indent=2))
