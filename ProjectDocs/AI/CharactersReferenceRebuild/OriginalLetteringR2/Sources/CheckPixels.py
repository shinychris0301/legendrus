from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parents[1];r=Image.open(b/'Sources/Reference.png').convert('RGB');root=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');records=[]
for a in json.loads((b/'Sources/AssetProvenance.json').read_text())['assets']:
 x,y,w,h=a['sourceBounds'];actual=np.array(Image.open(root/(a['name']+'.png')).convert('RGBA'));source=np.array(r.crop((x,y,x+w,y+h)));checks={}
 if a.get('sourceRGB'):
  assert np.array_equal(actual[:,:,:3],source),a['name'];checks['originalRGBExact']=True
 if a.get('sourceTextPatch'):
  xx,yy,ww,hh=a['sourceTextPatch'];assert np.array_equal(actual[yy:yy+hh,xx:xx+ww,:3],source[yy:yy+hh,xx:xx+ww]);checks['originalLetteringFaceExact']=True
 if a.get('alphaFrom') in ['title_r4','upgrade_r4','select_r4']:
  alpha=np.array(Image.open(root/(a['alphaFrom']+'.png')).convert('RGBA'))[:,:,3];assert np.array_equal(actual[:,:,3],alpha);checks['acceptedAlphaPreserved']=True
 if 'alternate' in a['name']:
  src=np.array(Image.open(root/(a['name'].replace('alternate','source')+'.png')).convert('RGBA'));assert np.array_equal(actual[:,:,3],src[:,:,3]);gold=(source[:,:,0].astype(float)>source[:,:,2]*1.18)&(source[:,:,1].astype(float)>source[:,:,2]*1.05);assert np.array_equal(actual[:,:,:3][gold],source[gold]);checks['warmLetteringAndFrameUnchanged']=True
 records.append({'asset':a['name'],'checks':checks})
(b/'QA/SourcePixelChecks.json').write_text(json.dumps(records,indent=2)+'\n');print('PASS source RGB, text faces, accepted alpha and alternate warm lettering')
