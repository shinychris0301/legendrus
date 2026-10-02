from pathlib import Path
from PIL import Image
import json
b=Path(__file__).resolve().parents[1];src=Image.open(b/'Sources/OriginalReference.png').convert('RGB')
items={'energy_medallion':(695,804,803,917),'message_swords':(634,602,679,641),'message_strength':(694,1066,738,1110)}
for key,box in items.items():
 im=src.crop(box);im.resize((im.width*6,im.height*6),Image.Resampling.LANCZOS).save(b/'Sources/Inputs'/f'{key}.png')
(b/'Sources/Assessment.json').write_text(json.dumps({'isolations':items,'reuse':'NavigationAligned/energy gold lightning; existing portraits/chest/frames/header/footer unchanged','whyNew':'Inspected existing energy and swords assets: green energy action plaque/gold bolt and shield-backed swords differ from the source blue medallion and plain tiny swords. No matching strength illustration found.','parchment':'Reuse original alpha/edge and existing repaired color surface; add original clean donor grain only inside the old text-removal mask.','newRGBGeneration':False},indent=2))
