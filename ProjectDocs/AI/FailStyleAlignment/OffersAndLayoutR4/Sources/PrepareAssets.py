"""Prepare independent Scenario sprites. Authorized local alpha cleanup only; preserve RGB.
No pixels are taken from a reference screenshot. Generated sources remain unmodified.
"""
from pathlib import Path
import json,hashlib
import numpy as np
from PIL import Image
root=Path.cwd()
base=root/'ProjectDocs/AI/FailStyleAlignment/OffersAndLayoutR4'
art=root/'Assets/LegendsOfRus/Resources/ProductionUI/FailAligned'
qa=base/'QA/Alpha';qa.mkdir(parents=True,exist_ok=True)
config=json.loads((base/'Sources/ScenarioRequests.json').read_text())
sources={a['name']:base/'Sources'/a['name']/'Scenario'/a['name']/(a['name']+'.png') for a in config['assets']}

records=[]
for name,source in sources.items():
 if not source.exists():print(name,'pending');continue
 destination=art/(name+'.png')
 if destination.exists():print(name,'already prepared');continue
 im=Image.open(source)
 record={'name':name,'source':str(source.relative_to(root)),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'sourceSize':list(im.size)}
 if name.startswith('environment'):
  destination.write_bytes(source.read_bytes());record['operation']='byte-for-byte copy'
 else:
  assert im.mode=='RGBA'
  a=np.array(im);old=a[:,:,3].copy()
  a[:,:,3][old<=4]=0;a[:,:,3][old>=235]=255
  out=Image.fromarray(a,'RGBA');bbox=out.getchannel('A').getbbox();assert bbox
  # Trim only transparent padding from genuinely isolated generated art.
  out=out.crop(bbox);out.save(destination)
  record.update({'operation':'alpha<=4 to0; alpha>=235 to255; trim transparent padding; RGB retained','trimBox':list(bbox),'changedAlphaPixels':int((old!=a[:,:,3]).sum()),'preparedSize':list(out.size)})
  for bg,color in [('Black',(0,0,0,255)),('White',(255,255,255,255)),('Magenta',(255,0,255,255))]:
   diagnostic=Image.new('RGBA',out.size,color);diagnostic.alpha_composite(out);diagnostic.convert('RGB').save(qa/(name+'_'+bg+'.png'))
 record['sha256']=hashlib.sha256(destination.read_bytes()).hexdigest();records.append(record)
 print(name,'prepared',Image.open(destination).size)
ledger=base/'QA/AssetPreparation.json'
previous=json.loads(ledger.read_text()) if ledger.exists() else []
ledger.write_text(json.dumps(previous+records,indent=2)+'\n')
