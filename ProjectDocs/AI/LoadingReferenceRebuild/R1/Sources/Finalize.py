from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,shutil
b=Path(__file__).resolve().parent;qa=b.parent/'QA';out=b/'Final';out.mkdir(exist_ok=True);ref=Image.open(b/'OriginalReference.png').convert('RGB');d=json.loads((b/'Assessment.json').read_text());bounds=d['bounds'];records=[]
def load(n):
 im=Image.open(b/'Inputs'/(n+'.png')).convert('RGBA');alpha=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').resize(im.size,Image.Resampling.LANCZOS).getchannel('A');return im,np.array(alpha)
def save(n,im,op):
 p=out/(n+'.png');im.save(p);records.append({'file':p.name,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':op})
def remove_small(a,minarea=100):
 count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(a>32),8);support=np.zeros_like(a)
 for i in range(1,count):
  if stats[i,cv2.CC_STAT_AREA]>minarea:support[labels==i]=1
 return a*cv2.dilate(support,np.ones((3,3),np.uint8))
logo,a=load('logo');m=Image.new('L',logo.size);ImageDraw.Draw(m).polygon([(230,0),(335,0),(375,40),(565,75),(565,398),(0,398),(0,75),(192,40)],fill=255);a=np.minimum(a,np.array(m));a=remove_small(a);logo.putalpha(Image.fromarray(a));save('source_logo',logo,'Original RGB/Scenario alpha; upper frame fragments excluded by contour')
tip,ta=load('tip');tip.putalpha(Image.fromarray(ta));save('source_tip_ru',tip,'Original complete parchment panel and Russian lettering; Scenario alpha; original RGB untouched')
# English plate uses one broad feathered paper repair, rather than separate mismatched title/body patches.
rgb=np.array(tip)[:,:,:3];mask=np.zeros(ta.shape,np.uint8);mask[84:140,93:300]=255;mask[175:288,35:362]=255
# Limit inpainting to dark lettering and its immediate shadow.
dark=rgb.mean(2)<143;mask=np.uint8((mask>0)&dark)*255;mask=cv2.dilate(mask,np.ones((5,5),np.uint8));mask[83:142,90:302]=255;clean=cv2.inpaint(rgb,mask,7,cv2.INPAINT_TELEA);en=Image.fromarray(clean).convert('RGBA');en.putalpha(Image.fromarray(ta));save('source_tip_blank',en,'Same source tip; bounded inpaint removes lettering for live English; original alpha');Image.fromarray(mask).save(b/'TipEnglishRepairMask.png')
for n in ['ivan','firebird']:
 im,a=load(n);x,y,_,_=bounds[n];cut=Image.new('L',im.size);cut.paste(Image.fromarray(ta),(bounds['tip'][0]-x,bounds['tip'][1]-y));a=np.uint8(np.rint(a.astype(float)*(255-np.array(cut))/255))
 # The separate tip panel covers its entire occupied region; no panel remnants on character layers.
 xx1=max(0,bounds['tip'][0]-x);yy1=max(0,bounds['tip'][1]-y);xx2=min(im.width,bounds['tip'][2]-x);yy2=min(im.height,bounds['tip'][3]-y)
 # Exclude only panel alpha above; retain feathers that extend beyond its contour.
 if n=='firebird':a[max(0,1428-y):,:max(0,716-x)]=0
 a=remove_small(a,150);im.putalpha(Image.fromarray(a));save('source_'+n,im,'Original RGB with Scenario alpha, overlaid tip alpha excluded, detached UI fragments removed')
# Progress track: separate source border, navy remaining-work surface, source sapphire fill.
bar,ba=load('progress');rgb=np.array(bar)[:,:,:3];interior=Image.new('L',bar.size);ImageDraw.Draw(interior).polygon([(84,24),(648,24),(670,48),(648,71),(84,71),(63,48)],fill=255);mask=np.array(interior)
# Donor is the visible unfilled navy portion; repeat its source texture inside the track.
donor=np.array(ref.crop((651,1509,743,1547)));yy,xx=np.indices(mask.shape);rgb[mask>0]=donor[yy[mask>0]%donor.shape[0],xx[mask>0]%donor.shape[1]]
bar=Image.fromarray(rgb).convert('RGBA');bar.putalpha(Image.fromarray(ba));save('source_progress_track',bar,'Original Scenario-isolated gold track, inside blue fill/fixed75% replaced by source navy donor texture')
fill=ref.crop((190,1508,400,1548)).convert('RGBA');save('source_progress_fill',fill,'Clean original sapphire fill segment without numbers; native measured clipping')
# Loading word plus side dividers: source RGB and bounded gold-support alpha.
box=(229,1433,717,1484);status=ref.crop(box).convert('RGBA');rgb=np.array(status)[:,:,:3];a=np.uint8((rgb[:,:,0]>rgb[:,:,2]*1.28)&(rgb[:,:,1]>rgb[:,:,2]*1.15)&(rgb[:,:,0]>85))*255;a=cv2.morphologyEx(a,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8));a=cv2.dilate(a,np.ones((3,3),np.uint8));allowed=np.zeros_like(a);allowed[3:51,120:375]=255;allowed[19:42,:120]=255;allowed[19:42,375:]=255;a=np.minimum(a,allowed);a=remove_small(a,12);status.putalpha(Image.fromarray(a));save('source_loading_ru',status,'Original Russian loading lettering and side flourishes; local gold-support alpha; original RGB')
# Margin ornaments: Scenario sheet returned opaque scenery rectangles. Restrict to narrow source UI contours.
full=np.array(ref.convert('RGBA'));fa=np.zeros(full.shape[:2],np.uint8);sheet=Image.open(b/'Scenario/ornaments/booster_medallion_trial01.png').convert('RGBA').resize((1000,500),Image.Resampling.LANCZOS).getchannel('A')
polys={'ornamentTL':[(0,0),(221,0),(221,38),(145,44),(114,73),(97,117),(69,152),(36,174),(28,220),(0,220)],'ornamentTR':[(0,0),(194,0),(194,221),(164,221),(155,166),(119,145),(102,92),(79,66),(40,42),(0,33)],'ornamentBL':[(0,0),(31,0),(35,50),(77,89),(105,128),(139,165),(199,177),(272,177),(272,203),(0,203)],'ornamentBR':[(195,0),(228,0),(228,234),(0,234),(0,211),(73,204),(112,186),(145,148),(172,108),(193,51)],'ornamentBottom':[(0,72),(58,72),(81,50),(130,51),(147,26),(174,17),(200,0),(225,20),(254,33),(285,57),(334,57),(374,73),(443,73),(443,99),(0,99)]}
for n,xy in d['ornamentPlacements'].items():
 box=bounds[n];rgb=np.array(ref.crop(box));h,w=rgb.shape[:2];m=Image.new('L',(w,h));ImageDraw.Draw(m).polygon(polys[n],fill=255);a=np.uint8((rgb[:,:,0]>rgb[:,:,2]*1.20)&(rgb[:,:,1]>rgb[:,:,2]*1.07)&(rgb[:,:,0]>39))*255;a=cv2.dilate(a,np.ones((3,3),np.uint8))
 circles={'ornamentTL':(45,43,31),'ornamentTR':(148,43,31),'ornamentBL':(44,153,32),'ornamentBR':(185,184,32),'ornamentBottom':(201,53,25)}
 cx,cy,r=circles[n];cv2.circle(a,(cx,cy),r,255,-1);a=np.minimum(a,np.array(m));raw=np.array(sheet.crop((xy[0],xy[1],xy[0]+w,xy[1]+h)));a=np.minimum(a,raw);fa[box[1]:box[3],box[0]:box[2]]=np.maximum(fa[box[1]:box[3],box[0]:box[2]],a)
for x1,y1,x2,y2 in [(221,0,747,28),(0,220,28,1469),(913,221,941,1438),(272,1648,713,1672)]:
 rgb=full[y1:y2,x1:x2,:3];a=np.uint8((rgb[:,:,0]>rgb[:,:,2]*1.2)&(rgb[:,:,1]>rgb[:,:,2]*1.07)&(rgb[:,:,0]>35))*255;a=cv2.dilate(a,np.ones((3,3),np.uint8));fa[y1:y2,x1:x2]=np.maximum(fa[y1:y2,x1:x2],a)
full[:,:,3]=fa;save('source_outer_frame',Image.fromarray(full),'Original RGB; Scenario ornament alpha plus bounded local contour/metal support; opaque scenery excluded')
p=b/'Scenario/backgrounds/booster_medallion_trial01.png';shutil.copy2(p,out/'clean_environment.png');records.append({'file':'clean_environment.png','sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':'Unmodified Scenario reconstruction of hidden village only'})
for col,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sh=Image.new('RGB',(1400,1150),col)
 for n,box in [('source_logo',(0,0,565,400)),('source_tip_ru',(580,0,394,361)),('source_tip_blank',(990,0,394,361)),('source_ivan',(0,410,350,710)),('source_firebird',(370,410,350,710)),('source_progress_track',(720,420,670,95)),('source_loading_ru',(750,530,488,51)),('source_outer_frame',(800,595,300,540))]:
  im=Image.open(out/(n+'.png'));im.thumbnail(box[2:]);sh.paste(im,box[:2],im)
 sh.save(qa/f'Alpha_{name}.png')
costs=[{'job':p.parent.name,**json.loads(p.read_text())} for p in (b/'Scenario').glob('*/Billing.json')];(qa/'ScenarioAudit.json').write_text(json.dumps({'jobs':costs,'totalCU':sum(v['cuCost'] for v in costs),'acceptedJobs':len(costs),'duplicateRequests':0},indent=2));(b.parent/'AssetProvenance.json').write_text(json.dumps({'assets':records,'source':d,'limitations':'Hidden scenery reconstructed; English panel lettering locally repaired; frame alpha locally bounded; native progress uses repeated source donor texture.'},indent=2));print('Prepared',len(records),'sprites. Cost',sum(v['cuCost'] for v in costs))
