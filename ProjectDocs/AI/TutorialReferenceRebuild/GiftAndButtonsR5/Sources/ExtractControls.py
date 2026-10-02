from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parents[1];root=Path('Assets/LegendsOfRus/Resources/ProductionUI');dst=root/'TutorialOriginal';ref=Image.open(b/'Sources/Reference.png').convert('RGB');assets=[];records=[]
def poly(size,points):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in points],fill=255);return m.resize(size,Image.Resampling.LANCZOS)
def save(name,rgb,a,source):
 im=rgb.convert('RGBA');im.putalpha(a);im.save(dst/(name+'.png'));assets.append((name,im));records.append({'name':name,'source':source,'sha256':hashlib.sha256((dst/(name+'.png')).read_bytes()).hexdigest()})
# The original gift art is retained inside its circular medallion; only outside alpha changes.
gift=Image.open(root/'HomeReferenceAligned/source_gift.png').convert('RGBA');m=Image.new('L',(gift.width*4,gift.height*4));d=ImageDraw.Draw(m);d.ellipse((4*4,9*4,170*4,176*4),fill=255);d.polygon([(80*4,16*4),(84*4,8*4),(89*4,16*4)],fill=255);m=m.resize(gift.size,Image.Resampling.LANCZOS);a=Image.fromarray(np.minimum(np.array(m),np.array(gift.getchannel('A'))));save('gift_medallion_r5',gift,a,'HomeReferenceAligned/source_gift.png (RGB unchanged)')
boxes={'done':(202,1415,738,1557),'first':(248,1551,700,1634)}
points={
'done':[(9,65),(26,55),(23,47),(25,40),(29,49),(32,44),(32,37),(36,41),(41,35),(41,25),(59,12),(62,6),(69,9),(83,9),(89,5),(85,12),(245,12),(259,12),(266,8),(273,13),(447,12),(443,7),(452,9),(467,9),(473,5),(476,13),(492,25),(492,37),(499,43),(500,38),(505,43),(510,40),(509,48),(514,55),(528,64),(514,69),(508,78),(510,90),(506,98),(500,99),(502,92),(499,85),(493,90),(492,110),(475,121),(471,128),(462,124),(446,124),(441,126),(442,121),(285,121),(280,126),(275,127),(266,138),(256,130),(247,127),(246,123),(89,123),(90,127),(83,125),(65,126),(59,128),(56,120),(42,109),(42,92),(36,86),(31,89),(29,98),(24,94),(25,86),(28,82),(27,77),(20,75)],
'first':[(8,41),(20,33),(20,27),(23,25),(23,30),(27,25),(26,18),(35,14),(35,6),(44,9),(61,7),(57,14),(216,14),(221,8),(227,14),(393,14),(388,7),(407,9),(414,6),(416,15),(429,18),(428,28),(432,30),(434,26),(435,34),(447,40),(433,47),(431,57),(417,63),(411,78),(406,74),(390,73),(394,67),(59,67),(62,73),(46,72),(35,78),(33,69),(24,61),(25,51),(19,48)]}
for key,box in boxes.items():
 rgb=ref.crop(box);a=poly(rgb.size,points[key])
 # Preserve both complete gold rails, including their dark bevel and highlight rows.
 solid=Image.new('L',rgb.size);ImageDraw.Draw(solid).rectangle((82,10,448,126) if key=='done' else (54,8,397,73),fill=255);a=Image.fromarray(np.maximum(np.array(a),np.array(solid)))
 if key=='done':
  ar=np.array(rgb).astype(float);gold=((ar[:,:,0]>60)&(ar[:,:,0]>ar[:,:,1]*1.05)&(ar[:,:,1]>ar[:,:,2]*1.15)).astype('uint8')*255
  gold=cv2.dilate(gold,np.ones((3,3),np.uint8));aa=np.array(a);yy,xx=np.indices(aa.shape);tips=((xx<43)|(xx>492))&((yy<53)|(yy>78));aa[tips]=np.minimum(aa[tips],gold[tips]);a=Image.fromarray(aa)
 save(key+'_ru_r5',rgb,a,str(box)+' original RGB and lettering')
 # Localize only the text interior, using surrounding original surface via inpainting.
 rect=(116,36,414,101) if key=='done' else (72,23,374,63)
 ar=np.array(rgb);mask=np.zeros(ar.shape[:2],np.uint8);x0,y0,x1,y1=rect
 mask[y0:y1,x0:x1]=255
 # Fill the bounded lettering face from neighboring source colors, preserving rails.
 localized=cv2.inpaint(ar,mask,20,cv2.INPAINT_TELEA)
 Image.fromarray(mask).save(b/'Sources'/(key+'_LetterMask.png'));save(key+'_r5',Image.fromarray(localized),a,str(box)+' source pixels except recorded lettering mask')
for label,color in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(1120,540),color)
 for index,(name,im) in enumerate(assets):
  pos=[(20,15),(215,10),(215,166),(215,325),(215,425)][index];sheet.paste(im,pos,im)
 sheet.save(b/'QA'/('Alpha'+label+'.png'))
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'assets':records,'scenarioJobs':0,'generatedArt':False,'method':'Source RGB, supersampled silhouette alpha; localized bounded inpaint from neighboring source pixels; original shared sources untouched'},indent=2)+'\n')
print('Prepared five new sprites')
