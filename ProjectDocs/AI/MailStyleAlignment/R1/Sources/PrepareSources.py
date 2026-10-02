from pathlib import Path
from PIL import Image,ImageDraw
import cv2,numpy as np,json,hashlib
s=Path(__file__).resolve().parent
assets=[('title_ru','Inbox',(198,105,740,266)),('sender_0','Inbox',(85,357,211,483)),('sender_1','Inbox',(85,493,211,619)),('sender_2','Inbox',(85,629,211,753)),('sender_3','Inbox',(85,765,211,889)),('sender_4','Inbox',(85,899,211,1024)),('gem','Letter',(401,1153,525,1267)),('coin','Letter',(214,1150,332,1264)),('hammer','Letter',(594,1148,719,1271)),('potion','Inbox',(692,538,742,609)),('book','Inbox',(685,938,741,1007)),('arrow','Inbox',(835,401,875,450)),('claim_frame','Letter',(165,1330,775,1501)),('compose','Inbox',(153,1504,278,1616)),('envelope','Inbox',(674,1503,793,1617)),('trash','Inbox',(797,1487,928,1617))]
out=[]
for n,ref,box in assets:
 original=np.array(Image.open(s/(ref+'.png')).convert('RGB').crop(box));rgb=original.copy();h,w=rgb.shape[:2];mask=np.zeros((h,w),'uint8')
 if n=='claim_frame':
  mask[65:123,169:457]=255;rgb=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
 Image.fromarray(original).save(s/'Crops'/(n+'.png'));Image.fromarray(rgb).save(s/'Inputs'/(n+'_native.png'));Image.fromarray(rgb).resize((w*3,h*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(mask).save(s/'RepairMasks'/(n+'.png'));out.append({'name':n,'reference':ref,'box':box,'sourceSha256':hashlib.sha256((s/(ref+'.png')).read_bytes()).hexdigest()})
(s/'Assessment.json').write_text(json.dumps({'assets':out,'approach':'Scenario alpha isolation; original RGB outside declared label repairs. Reuse clean scenery/parchment and approved header.'},indent=2))
print('Prepared',len(out))
