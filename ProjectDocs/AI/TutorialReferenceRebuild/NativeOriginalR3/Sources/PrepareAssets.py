from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parents[1];s=b/'Sources';dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal');dst.mkdir(exist_ok=True)
ref=Image.open(s/'Reference.png').convert('RGB');data=json.loads((s/'Boxes.json').read_text());boxes=data['source'];places=data['sheet']
pa=Image.open(s/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').resize((788,1121),Image.Resampling.LANCZOS)
ca=Image.open(s/'Scenario/controls/booster_medallion_trial01.png').convert('RGBA').resize((1400,880),Image.Resampling.LANCZOS)
# Return the exact original RGB under Scenario alpha, retaining deliberately opaque faces.
def alpha(im,interior):
 a=np.array(im.getchannel('A'));mask=(a>96).astype('uint8');n,labels,stats,_=cv2.connectedComponentsWithStats(mask)
 if n>1:
  largest=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);connected=(labels==largest).astype('uint8');cnt,_=cv2.findContours(connected,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);solid=np.zeros_like(a);cv2.drawContours(solid,cnt,-1,255,-1);a=np.maximum(a,solid)
  support=cv2.dilate(connected,np.ones((5,5),np.uint8));a[support==0]=0
 x0,y0,x1,y1=interior;a[y0:y1,x0:x1]=255
 return Image.fromarray(a)
def tiled(tile,size):
 w,h=tile.size;q=Image.new('RGB',(w*2,h*2));q.paste(tile,(0,0));q.paste(ImageOps.mirror(tile),(w,0));q.paste(ImageOps.flip(tile),(0,h));q.paste(ImageOps.flip(ImageOps.mirror(tile)),(w,h));out=Image.new('RGB',size)
 for y in range(0,size[1],h*2):
  for x in range(0,size[0],w*2):out.paste(q,(x,y))
 return out
navy=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/reward_panel.png').convert('RGB');navy=navy.crop((navy.width//2-24,navy.height//2-24,navy.width//2+24,navy.height//2+24))
green=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/LoginAligned/login.png').convert('RGB');green=green.crop((110,40,170,70))
record=[];assets=[]
for unused in ('section','moves','card','reward'):
 p=dst/(unused+'.png')
 if p.exists():p.unlink()
def save(name,rgb,a,mask=None):
 im=rgb.convert('RGBA');im.putalpha(a);im.save(dst/(name+'.png'));assets.append((name,im))
 if mask:mask.save(s/(name+'_RepairMask.png'))
 record.append({'name':name,'sha256':hashlib.sha256((dst/(name+'.png')).read_bytes()).hexdigest(),'alpha':'Scenario source isolation, connected contour and intentionally opaque interior','repairMask':name+'_RepairMask.png' if mask else None})
def repair(rgb,rects,tile):
 m=Image.new('L',rgb.size);d=ImageDraw.Draw(m)
 for r in rects:d.rectangle(r,fill=255)
 arr=np.array(m);blend=np.minimum(cv2.distanceTransform((arr>0).astype('uint8'),cv2.DIST_L2,3)/4,1)[...,None];base=np.array(rgb);surface=np.array(tiled(tile,rgb.size));out=Image.fromarray(np.uint8(np.round(base*(1-blend)+surface*blend)))
 return out,m
rgb=ref.crop(boxes['panel']);a=alpha(pa,(30,34,758,1090))
paper=ref.crop((522,400,788,457))
# Keep only the panel silhouette, excluding room pixels around its outer rails.
outline=[(0,76),(10,60),(7,38),(18,18),(38,7),(376,8),(392,0),(410,8),(746,7),(772,20),(781,43),(776,65),(787,80),(775,94),(775,360),(787,378),(775,392),(775,665),(787,680),(775,697),(775,1080),(786,1101),(767,1112),(747,1108),(410,1110),(393,1120),(376,1110),(39,1110),(15,1118),(7,1101),(14,1080),(14,698),(0,680),(14,665),(14,393),(0,378),(14,360),(14,95)]
sil=Image.new('L',rgb.size);ImageDraw.Draw(sil).polygon(outline,fill=255);a=Image.fromarray(np.minimum(np.array(a),np.array(sil)))
def remove_ink(rgb,rects):
 arr=np.array(rgb);mask=np.zeros(arr.shape[:2],np.uint8)
 for x0,y0,x1,y1 in rects:
  patch=arr[y0:y1,x0:x1];ink=(patch.mean(axis=2)<125).astype(np.uint8)*255
  mask[y0:y1,x0:x1]=cv2.dilate(ink,np.ones((3,3),np.uint8))
 return Image.fromarray(cv2.inpaint(arr,mask,5,cv2.INPAINT_TELEA)),Image.fromarray(mask)

def local(rects):return [(x0-76,y0-304,x1-76,y1-304) for x0,y0,x1,y1 in rects]
# Keep the whole panel, section ribbons, internal frames and correct source Russian wording.
# Clear only illustration interiors that native approved resources replace, plus corrected labels.
navyRects=[(130,400,493,650),(127,774,257,894),(282,774,415,898),(439,774,571,898),
 (127,1078,220,1182),(245,1078,338,1182),(363,1078,460,1182),(516,1114,587,1200),
 (172,1291,777,1393),(336,1265,615,1295)]
rgb,m1=repair(rgb,local(navyRects),navy)
paperRects=[(114,909,577,960),(592,775,817,963),(351,1190,473,1242),(489,1217,614,1250)]
rgb,m2=remove_ink(rgb,local(paperRects));m=Image.fromarray(np.maximum(np.array(m1),np.array(m2)))
save('panel_ru',rgb,a,m)
englishRects=[(230,341,708,382),(231,706,714,742),(235,1001,710,1043)]
rgb,m3=repair(rgb,local(englishRects),navy)
rgb,m4=remove_ink(rgb,local([(536,470,809,627),(129,1190,348,1242),(617,1087,818,1238)]))
m=Image.fromarray(np.maximum.reduce([np.array(m),np.array(m3),np.array(m4)]));save('panel',rgb,a,m)
interiors={'title':(65,66,548,150),'section':(37,15,598,62),'reward':(47,45,622,141),'done':(71,25,450,109),'first':(48,19,390,60),'moves':(31,35,101,115),'card':(18,17,141,129)}
repairs={'title':[(102,74,510,143)],'section':[(69,22,561,58)],'reward':[(40,43,632,144),(184,12,490,43)],'done':[(94,34,429,103)],'first':[(69,19,378,61)],'moves':[(29,30,106,117)],'card':[(17,15,143,133)]}

# Bound returned masks to the actual source silhouettes; Pixa retained some crop rectangles.
polygons={
 'title':[(2,122),(24,105),(38,70),(57,52),(190,48),(230,35),(253,25),(276,7),(299,0),(323,22),(353,29),(391,47),(552,49),(578,68),(592,105),(611,122),(584,140),(573,162),(545,178),(335,174),(310,184),(290,186),(271,174),(60,177),(34,160),(23,140)],
 'section':[(2,39),(16,25),(26,17),(47,8),(306,5),(316,0),(330,7),(592,8),(610,23),(632,37),(615,52),(606,64),(34,66),(19,54)],
 'reward':[(1,93),(21, 70),(34,48),(70,32),(166,28),(191,8),(210,1),(489,1),(514,22),(597,28),(641,43),(655, 70),(673, 90),(653,111),(645,139),(618,155),(57,156),(27,139),(14,115)],
 'done':[(1,72),(26,49),(40,22),(82,10),(252,3),(276,10),(441,10),(477,30),(492,52),(520, 70),(493,93),(479,118),(441,128),(278,128),(260,140),(240,129),(67,128),(38,111),(23,92)],
 'first':[(0, 40),(20,26),(39,8),(210,6),(222,0),(240,8),(399,8),(420,25),(439, 40),(420,54),(398,70),(36,70),(17,56)],
 'moves':[(1,76),(12,64),(17,38),(34,21),(53,17),(64,3),(77,16),(100,23),(116,42),(122,64),(132,77),(121,89),(114,113),(91,130),(75,134),(65,141),(52,134),(30,125),(14,105),(10,87)],
 'card':[(7,23),(23,4),(132,4),(155,24),(155,128),(137,144),(24,145),(5,127)]}
def ornament_alpha(rgb,limit):
 ar=np.array(rgb).astype(float);r,g,blue=ar[:,:,0],ar[:,:,1],ar[:,:,2]
 gold=((r>65)&(r>g*1.08)&(g>blue*1.15)&(np.array(limit)>0)).astype(np.uint8)*255
 gold=cv2.morphologyEx(gold,cv2.MORPH_CLOSE,np.ones((5,5),np.uint8))
 contours,_=cv2.findContours(gold,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE)
 mask=np.zeros_like(gold)
 for c in contours:
  if cv2.contourArea(c)>10:cv2.drawContours(mask,[c],-1,255,-1)
 mask=cv2.dilate(mask,np.ones((3,3),np.uint8));mask=cv2.GaussianBlur(mask,(3,3),.5)
 return Image.fromarray(np.minimum(mask,np.array(limit)))
def silhouette(name,size):
 m=Image.new('L',(size[0]*4,size[1]*4));ImageDraw.Draw(m).polygon([(x*4,y*4) for x,y in polygons[name]],fill=255)
 return m.resize(size,Image.Resampling.LANCZOS)

for name,pos in places.items():
 if name not in ("title","done","first"):continue
 rgb=ref.crop(boxes[name]);w,h=rgb.size;a=alpha(ca.crop((pos[0],pos[1],pos[0]+w,pos[1]+h)),interiors[name]);a=Image.fromarray(np.minimum(np.array(a),np.array(silhouette(name,(w,h)))))
 a=ornament_alpha(rgb,a)
 if name=='first':
  keep=Image.new('L',rgb.size);ImageDraw.Draw(keep).polygon([(29,27),(44,11),(397,11),(414,28),(414,52),(397,66),(44,66),(29,51)],fill=255);a=Image.fromarray(np.maximum(np.array(a),np.array(keep)))
 if name in ('title','done','first'):save(name+'_ru',rgb,a)
 rgb,m=repair(rgb,repairs[name],green if name=='done' else navy);save(name,rgb,a,m)
# This is an independent clean reconstruction, never the original UI screenshot.
env=Image.open(s/'Scenario/environment/booster_medallion_trial01.png').convert('RGB');env.save(dst/'environment.png')
record.append({'name':'environment','sha256':hashlib.sha256((dst/'environment.png').read_bytes()).hexdigest(),'method':'One Scenario clean scenery reconstruction; hidden room is not exact source recovery'})
for label,c in [('Black','black'),('White','white'),('Magenta','magenta')]:
 contact=Image.new('RGB',(1500,1900),c);x=y=10;row=0
 for name,im in assets:
  scale=min(1,650/im.width,1100/im.height);im=im.resize((round(im.width*scale),round(im.height*scale)),Image.Resampling.LANCZOS)
  if x+im.width>1500:x=10;y+=row+16;row=0
  contact.paste(im,(x,y),im);x+=im.width+16;row=max(row,im.height)
 contact.save(b/'QA'/('Alpha'+label+'.png'))
(s/'AssetProvenance.json').write_text(json.dumps({'source':str(s/'Reference.png'),'sourceSha256':hashlib.sha256((s/'Reference.png').read_bytes()).hexdigest(),'assets':record,'note':'Original RGB retained outside masks; title/buttons RU retain original lettering. Bounded hidden-face donor repairs are approximate. No gems or boosters extracted or generated.'},indent=2))
print('Prepared',len(record),'source-derived resources')
