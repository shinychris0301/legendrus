from pathlib import Path
from PIL import Image
import json,hashlib
s=Path('ProjectDocs/AI/LoginStyleAlignment/R1/Sources');im=Image.open(s/'OriginalReference.png').convert('RGB');p=s/'Assessment.json';a=json.loads(p.read_text());a['rejected']=[{'name':'frame','reason':'Scenario removed dark ornamental corners, retaining only masking debris. Replaced by individual corner isolations.'}];a['assets']=[r for r in a['assets'] if r['name']!='frame']
for name,box in {'corner_top':(0,0,310,293),'corner_left':(0,1270,145,1672),'corner_right':(811,1198,941,1672)}.items():
 out=im.crop(box);out.save(s/'Inputs'/(name+'_native.png'));out.resize((out.width*2,out.height*2),Image.Resampling.LANCZOS).save(s/'Inputs'/(name+'.png'));Image.new('L',out.size).save(s/'RepairMasks'/(name+'.png'));a['assets'].append({'name':name,'bounds':box,'repairPixels':0,'factor':2,'inputSha256':hashlib.sha256((s/'Inputs'/(name+'.png')).read_bytes()).hexdigest()})
p.write_text(json.dumps(a,indent=2))
# Reject failed runtime and final output rather than accidentally displaying it.
for d in [s/'Final',Path('Assets/LegendsOfRus/Resources/ProductionUI/LoginAligned')]:
 f=d/'frame.png'
 if f.exists():f.unlink()
