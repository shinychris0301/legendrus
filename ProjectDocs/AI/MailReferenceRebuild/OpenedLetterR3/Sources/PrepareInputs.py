from pathlib import Path
from PIL import Image
import json,hashlib
b=Path(__file__).resolve().parent;im=Image.open(b/'Letter.png').convert('RGB');box=(78,340,230,494)
crop=im.crop(box).resize((608,616),Image.Resampling.LANCZOS);crop.save(b/'Sheet.png')
(b/'Bounds.json').write_text(json.dumps({'guild_emblem_r3':{'source':box,'outputSize':[608,616]},'operation':'Extract original navy circular guild medallion. Existing TeamHub/Chat/Ranking emblems inspected but have shield silhouettes, not this circular source design. No reward/icon regeneration.','sourceSha256':hashlib.sha256((b/'Letter.png').read_bytes()).hexdigest()},indent=2))
