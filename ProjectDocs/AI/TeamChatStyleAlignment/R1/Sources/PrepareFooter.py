from pathlib import Path
from PIL import Image
import numpy as np,cv2,json
s=Path(__file__).resolve().parent;ref=Image.open(s/'OriginalReference.png').convert('RGB');a=json.loads((s/'Assessment.json').read_text());a['rejectedInitialFooterCrops']='gift/help/sticker isolated too tightly and clipped original round rims; retained as evidence, not runtime. Replace with complete source button assemblies and live labels.'
for n,box,rect in [('footer_gift',(85,1492,305,1644),(111,1593,286,1628)),('footer_help',(361,1484,602,1645),(380,1593,580,1628)),('footer_sticker',(638,1493,898,1645),(674,1593,867,1628))]:
 im=ref.crop(box);im.save(s/'Crops'/(n+'.png'));rgb=np.array(im);m=np.zeros(rgb.shape[:2],'uint8');x1,y1,x2,y2=rect;m[y1-box[1]:y2-box[1],x1-box[0]:x2-box[0]]=255;donor=np.array(ref.crop((350,295,390,305)));tile=np.tile(donor,(rgb.shape[0]//10+1,rgb.shape[1]//40+1,1))[:rgb.shape[0],:rgb.shape[1]];
 if n=='footer_help':
  m[:]=0;points=np.array([(390,1594),(566,1594),(575,1610),(564,1626),(394,1626),(384,1610)],np.int32)-np.array(box[:2]);cv2.fillPoly(m,[points],255)
 rgb[m>0]=tile[m>0];im=Image.fromarray(rgb);im.save(s/'Inputs'/(n+'_native.png'));im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(m).save(s/'RepairMasks'/(n+'.png'));a['assets']=[x for x in a['assets'] if x['name']!=n];a['assets'].append({'name':n,'box':box,'repairRegions':[rect],'operation':'Complete original footer button isolated, original label face repaired from clean source navy donor for live translation. Help badge3 retained as fixed example art.'})
a['assets']=[x for x in a['assets'] if x['name'] not in ['gift','help','sticker']];(s/'Assessment.json').write_text(json.dumps(a,indent=2))
