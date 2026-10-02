from pathlib import Path
from PIL import Image
import hashlib,json,shutil
b=Path(__file__).resolve().parent;ref=Path('/home/k2/Downloads/match3final/2.loading.png');shutil.copy2(ref,b/'OriginalReference.png');im=Image.open(ref).convert('RGB')
boxes={'logo':[185,15,750,413],'ivan':[23,383,432,1214],'firebird':[481,619,918,1451],'tip':[276,1068,670,1429],'progress':[105,1480,835,1572]}
for n,box in boxes.items():im.crop(box).save(b/'Inputs'/(n+'.png'))
# Separate margin ornament islands to prevent the opaque scene becoming the isolated subject.
boxes['ornamentTL']=[0,0,221,220];boxes['ornamentTR']=[747,0,941,221];boxes['ornamentBL']=[0,1469,272,1672];boxes['ornamentBR']=[713,1438,941,1672];boxes['ornamentBottom']=[270,1573,713,1672]
plate=Image.new('RGB',(720,500),(255,0,255));places={'ornamentTL':[0,0],'ornamentTR':[260,0],'ornamentBL':[0,250],'ornamentBR':[280,250],'ornamentBottom':[275,130]}
# Bottom crest overlaps upper right on this proposed sheet; use its own row instead.
plate=Image.new('RGB',(1000,500),(255,0,255));places={'ornamentTL':[0,0],'ornamentTR':[260,0],'ornamentBL':[0,250],'ornamentBR':[280,250],'ornamentBottom':[530,0]}
for n,xy in places.items():plate.paste(im.crop(boxes[n]),xy)
plate.save(b/'Inputs/ornaments.png')
(b/'Assessment.json').write_text(json.dumps({'source':str(ref),'sha256':hashlib.sha256(ref.read_bytes()).hexdigest(),'bounds':boxes,'ornamentPlacements':places,'userRequest':'Extract panels together with original static Russian text. Percentage remains measured; English retains translation.','navigation':'Existing measured Home preparation, no fake time or baked percentage.'},indent=2))
root=Path.cwd();snapshot={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in Path(d).rglob('*') if p.is_file()};(b.parent/'QA/Before.json').write_text(json.dumps(snapshot,indent=2))
