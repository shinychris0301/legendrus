from pathlib import Path
from PIL import Image,ImageDraw
import hashlib,json
b=Path(__file__).resolve().parent
root=Path.cwd()
files={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in (root/'Assets').rglob('*') if p.is_file()}
p=b/'BeforeAssetsHashes.json'
if not p.exists():p.write_text(json.dumps(files,indent=2))
s=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/Events.png').convert('RGBA')
tab=s.crop((40,306,323,380));footer=s.crop((0,1614,941,1672))
tab.save(b/'TabSource.png');footer.save(b/'FooterSource.png')
# Separate isolated UI surfaces; the footer is intentionally opaque dark carved lacquer.
a=Image.new('RGB',(1024,512),(70,70,70))
a.paste(tab.resize((849,222)),(87,24));a.paste(footer,(41,394));a.save(b/'SurfaceGuidance.png')
