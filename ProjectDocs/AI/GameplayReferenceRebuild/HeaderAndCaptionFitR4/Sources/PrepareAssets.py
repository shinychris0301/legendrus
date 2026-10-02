from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
root=Path.cwd();b=Path(__file__).resolve().parent
src=Image.open(root/'ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA')
# Reuse accepted Scenario-isolated, value-cleared faces. Restore only source rails and joints.
a=Image.new('RGBA',(709,186))
for n,folder,x in [('moves','GameplayReferenceAligned',0),('goals','GameplayReferenceAligned',143),('score','GameplayRefinedR2',494)]:
 a.alpha_composite(Image.open(root/f'Assets/LegendsOfRus/Resources/ProductionUI/{folder}/{n}.png').convert('RGBA'),(x,0))
strips=[(230,58,920,66),(365,66,384,224),(714,66,738,224),(230,224,738,234)]
for box in strips:a.paste(src.crop(box),(box[0]-230,box[1]-57))
# Clip the outer edge to the actual metal silhouette, excluding squared extraction padding.
mask=Image.new('L',a.size);d=ImageDraw.Draw(mask)
d.polygon([(0,9),(7,2),(681,2),(692,0),(700,9),(708,19),(708,164),(700,178),(687,181),(0,177)],fill=255)
al=np.minimum(np.asarray(a.getchannel('A')),np.asarray(mask));a.putalpha(Image.fromarray(al))
out=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayHeaderR4';out.mkdir(exist_ok=True);a.save(out/'header.png')
for bg,n in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
 p=Image.new('RGBA',a.size,bg+(255,));p.alpha_composite(a);p.convert('RGB').save(b/f'HeaderAlpha{n}.png')
(b/'AssetProvenance.json').write_text(json.dumps({'newScenarioCalls':0,'method':'Existing Scenario-extracted blank faces joined with original source rail and seam pixels; bounded outer-alpha cleanup. No generated artwork or text baking.','sourceRailAndJointBoxes':strips,'sourceSHA256':hashlib.sha256((root/'ProjectDocs/AI/GameplayReferenceRebuild/R1/Sources/OriginalReference.png').read_bytes()).hexdigest()},indent=2))
