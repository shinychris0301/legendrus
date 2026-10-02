from pathlib import Path
from PIL import Image
import json
b=Path(__file__).resolve().parents[1];src=Image.open(b/'Sources/OriginalReference.png').convert('RGB')
items={'guild_crest':(88,250,224,408),'system_crest':(57,923,180,1025),'chest_info':(823,278,875,327)}
for name,box in items.items():
 im=src.crop(box);im.resize((im.width*4,im.height*4),Image.Resampling.LANCZOS).save(b/'Sources/Inputs'/f'{name}.png')
(b/'Sources/Assessment.json').write_text(json.dumps({'source':'OriginalReference.png','isolations':items,'operation':'Scenario alpha isolation only; original RGB retained','framePlan':'Source rails and opaque navy interiors reconstructed from clean source donors. No screenshot contents in runtime surfaces.','preserve':'Shared header, existing portraits/chest/parchment, source-lettered footer, local-only behavior'},indent=2))
