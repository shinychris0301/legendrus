from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib,shutil
b=Path(__file__).resolve().parent;out=b/'Final';out.mkdir(exist_ok=True);qa=b.parent/'QA';ref=Image.open(b/'OriginalReference.png').convert('RGB');assessment=json.loads((b/'Assessment.json').read_text());records=[]
def read(n):
 im=Image.open(b/'Inputs'/(n+'.png')).convert('RGBA');a=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').resize(im.size,Image.Resampling.LANCZOS).getchannel('A');return im,np.array(a)
def save(n,im,operation):
 p=out/(n+'.png');im.save(p);records.append({'file':p.name,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':operation})
logo,a=read('logo');logo.putalpha(Image.fromarray(a));save('source_logo',logo,'Original RGB with Scenario alpha; opaque carved plaque retained')
button,a=read('start');rgb=np.array(button)[:,:,:3];mask=np.zeros(a.shape,np.uint8);gold=(rgb[:,:,0]>rgb[:,:,2]*1.25)&(rgb[:,:,1]>rgb[:,:,2]*1.13)&(rgb[:,:,0]>75);mask[78:130,80:548]=np.uint8(gold[78:130,80:548])*255;mask=cv2.dilate(mask,np.ones((9,9),np.uint8));fixed=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA);button=Image.fromarray(fixed).convert('RGBA');button.putalpha(Image.fromarray(a));Image.fromarray(mask).save(b/'StartLetteringRepairMask.png');save('source_start_blank',button,'Original border RGB/Scenario alpha; bounded gold-letter mask inpaint only to support live localized text')
# Local cleanup rejects the scenery rectangles retained by the grouped corner isolation.
# Keep only metallic/sapphire UI within bounded corner contours; original RGB is unchanged.
full=np.array(ref.convert('RGBA'));fa=np.zeros(full.shape[:2],np.uint8)
corners=assessment['cornerBounds'];poly=[[(0,0),(134,0),(134,24),(93,27),(82,48),(67,79),(38,88),(24,113),(24,136),(0,136)],[(0,0),(134,0),(134,136),(110,136),(110,111),(94,84),(70,76),(53,48),(37,26),(0,24)],[(0,0),(24,0),(28,40),(38,54),(63,68),(77,91),(96,107),(134,110),(134,136),(0,136)],[(110,0),(134,0),(134,136),(0,136),(0,110),(37,109),(56,88),(77,73),(99,57),(109,34)]]
cornerAlpha=Image.open(b/'Scenario/frame_corners/booster_medallion_trial01.png').convert('RGBA').resize((328,332),Image.Resampling.LANCZOS).getchannel('A')
for i,box in enumerate(corners):
 rgb=np.array(ref.crop(box));h,w=rgb.shape[:2];m=Image.new('L',(w,h));ImageDraw.Draw(m).polygon(poly[i],fill=255);gold=(rgb[:,:,0]>rgb[:,:,2]*1.22)&(rgb[:,:,1]>rgb[:,:,2]*1.10)&(rgb[:,:,0]>44);a=np.uint8(gold)*255;a=cv2.dilate(a,np.ones((3,3),np.uint8));a=cv2.GaussianBlur(a,(3,3),.4)
 # Restore the opaque sapphire medallion and its dark engraved rim.
 cx,cy=[(37,38),(98,39),(38,97),(99,97)][i];cv2.circle(a,(cx,cy),25,255,-1)
 a=np.minimum(a,np.array(m));raw=np.array(cornerAlpha.crop((i%2*164,i//2*166,i%2*164+w,i//2*166+h)));a=np.minimum(a,raw)
 fa[box[1]:box[3],box[0]:box[2]]=a;m.save(b/f'FrameCornerContour_{i}.png')
# Clean straight metal rails; their dark 3D strokes are retained by a one-pixel support dilation.
for box in [(134,0,807,24),(134,1651,807,1672),(0,136,23,1536),(918,136,941,1536)]:
 x1,y1,x2,y2=box;rgb=full[y1:y2,x1:x2,:3];m=np.uint8((rgb[:,:,0]>rgb[:,:,2]*1.2)&(rgb[:,:,1]>rgb[:,:,2]*1.08)&(rgb[:,:,0]>35))*255;m=cv2.dilate(m,np.ones((3,3),np.uint8));fa[y1:y2,x1:x2]=np.maximum(fa[y1:y2,x1:x2],m)
full[:,:,3]=fa;frame=Image.fromarray(full);save('source_outer_frame',frame,'Original RGB; Scenario corner alpha limited by local metal/sapphire/contour masks; narrow source rails; no scenery regions')
hero,a=read('hero');rgb=np.array(hero)[:,:,:3];x,y,_,_=assessment['bounds']['hero'];hs=hero.size
# Remove the source Start artwork from the joint subject, where the separate native button sits.
buttonMask=Image.new('L',hs);buttonMask.paste(Image.fromarray(np.array(Image.open(b/'Scenario/start/booster_medallion_trial01.png').convert('RGBA').resize(button.size,Image.Resampling.LANCZOS).getchannel('A'))),(159-x,1356-y));bm=np.array(buttonMask);a=np.uint8(np.rint(a.astype(float)*(255-bm)/255));buttonMask.save(b/'HeroButtonExclusion.png')
# Border decoration is supplied independently. Its complete removal includes adjoining non-subject edge fragments.
fm=fa[y:y+hs[1],x:x+hs[0]];a=np.uint8(np.rint(a.astype(float)*(255-fm)/255));a[-5:]=0
# Remove baked version numerals; preserve the flanking original gold decorative marks.
vm=np.zeros(a.shape,np.uint8);vx1,vy1,vx2,vy2=420-x,1570-y,520-x,1602-y
sub=rgb[vy1:vy2,vx1:vx2];gold=(sub[:,:,0]>sub[:,:,2]*1.3)&(sub[:,:,1]>sub[:,:,2]*1.13)&(sub[:,:,0]>60);vm[vy1:vy2,vx1:vx2]=np.uint8(gold)*255;vm=cv2.dilate(vm,np.ones((3,3),np.uint8));rgb=cv2.inpaint(rgb,vm,3,cv2.INPAINT_TELEA);Image.fromarray(vm).save(b/'HeroVersionRepairMask.png')
# Drop detached frame specks, retaining the connected Ivan/Firebird silhouette.
n,lab,stats,_=cv2.connectedComponentsWithStats(np.uint8(a>25),8);support=np.zeros_like(a)
for k in range(1,n):
 if stats[k,cv2.CC_STAT_AREA]>80:support[lab==k]=1
support=cv2.dilate(support,np.ones((3,3),np.uint8));a*=support
hero=Image.fromarray(rgb).convert('RGBA');hero.putalpha(Image.fromarray(a));save('source_ivan_firebird',hero,'Original joint subject RGB/Scenario alpha; source button/frame alpha removed; bounded baked version lettering inpaint')
p=b/'Scenario/backgrounds/booster_medallion_trial01.png';shutil.copyfile(p,out/'clean_environment.png');records.append({'file':'clean_environment.png','sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':'Unmodified Scenario reconstruction of hidden environment only'})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'assets':records,'assessment':'Sources/Assessment.json','limitations':'Scenery reconstructed; opaque button lettering and version areas locally repaired; source font identity unverified. Joint hero is cut where original native Start occluded it. Corner Scenario result retained scenery and was corrected locally.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sheet=Image.new('RGB',(1300,900),color)
 for n,box in [('source_logo',(0,0,650,490)),('source_start_blank',(0,510,650,210)),('source_ivan_firebird',(660,0,430,800)),('source_outer_frame',(1090,0,210,375))]:
  im=Image.open(out/(n+'.png'));im.thumbnail(box[2:],Image.Resampling.LANCZOS);sheet.paste(im,box[:2],im)
 sheet.save(qa/f'Alpha_{name}.png')
costs=[{'job':p.parent.name,**json.loads(p.read_text())} for p in (b/'Scenario').glob('*/Billing.json')];(qa/'ScenarioAudit.json').write_text(json.dumps({'jobs':costs,'totalCU':sum(v['cuCost'] for v in costs),'acceptedJobs':len(costs),'duplicateSubmissions':0,'localCornerCleanupRequired':True},indent=2))
print('Staged five independent assets; no production assets changed yet.')
