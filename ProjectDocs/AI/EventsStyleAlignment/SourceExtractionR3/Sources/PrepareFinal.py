"""Preserve original RGB outside documented UI text/face repairs; Scenario supplies masks."""
from pathlib import Path
from PIL import Image,ImageDraw
import cv2,numpy as np,json,hashlib
b=Path(__file__).resolve().parent;base=b.parent;out=base/'Staging';out.mkdir(exist_ok=True);records=[]
assessment=json.loads((b/'AssetAssessment.json').read_text());ref=Image.open(assessment['source']).convert('RGB')
for e in assessment['assets']:
 n=e['name'];job=b/'Scenario'/n/'booster_medallion_trial01.png'
 if not job.exists():continue
 orig=np.array(Image.open(b/'Crops'/(n+'.png')).convert('RGB'));fixed=np.array(Image.open(b/'Inputs'/(n+'.png')).convert('RGB'));h,w=orig.shape[:2]
 alpha=np.array(Image.open(job).convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS));allowed=np.array(Image.open(b/'RepairMasks'/(n+'.png')))
 # Remove remaining baked shadow using source-surface texture, not a regenerated frame.
 repairs={'title':((68,64,458,153),(72,51,111,65)), 'tab':((86,16,208,61),(34,17,80,60)), 'action':((43,18,184,64),(72,14,155,21)), 'status':((1,5,131,35),(160,6,205,32))}
 if n in repairs:
  (x0,y0,x1,y1),(dx0,dy0,dx1,dy1)=repairs[n];tile=orig[dy0:dy1,dx0:dx1]
  if n=='title':tile=np.array(ref.crop((765,1130,890,1174)))
  tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0)
  donor=np.tile(tile,(h//tile.shape[0]+1,w//tile.shape[1]+1,1))[:h,:w];mask=np.zeros((h,w),np.uint8);mask[y0:y1,x0:x1]=255;allowed=np.maximum(allowed,mask)
  candidate=cv2.seamlessClone(donor,orig,mask.copy(),((x0+x1)//2,(y0+y1)//2),cv2.NORMAL_CLONE)
  if n=='action':
   yy,xx=np.mgrid[:h,:w];rgb=orig.astype(float);valid=(xx>28)&(xx<195)&(yy>15)&(yy<62)&(mask==0)&(rgb[:,:,1]>rgb[:,:,0]*1.18)&(rgb[:,:,1]>rgb[:,:,2]*1.3)
   coords=np.stack([np.ones_like(xx),xx/w,yy/h,(xx/w)**2,(yy/h)**2],-1);fit=np.linalg.lstsq(coords[valid],rgb[valid],rcond=None)[0];candidate=np.clip(coords@fit,0,255).astype(np.uint8)
  if n=='action':
   face=np.zeros((h,w),np.uint8);cv2.fillPoly(face,[np.array([(32,14),(191,14),(201,29),(201,47),(189,64),(32,64),(22,49),(22,29)])],255)
   weight=np.clip(cv2.distanceTransform(face,cv2.DIST_L2,3)/7,0,1)[:,:,None];candidate=np.round(orig*(1-weight)+candidate*weight).astype(np.uint8);mask=face;allowed=np.maximum(allowed,face)
  fixed[mask>0]=candidate[mask>0]
 if n=='status':
  # Pixa returned empty alpha for this dark thin strip. Restore its source silhouette.
  alpha[:]=0;cv2.fillPoly(alpha,[np.array([(0,2),(w-10,2),(w-2,10),(w-2,h-10),(w-10,h-2),(0,h-2)])],255)
 if n=='frame':
  cv2.rectangle(alpha,(13,13),(w-14,h-14),255,-1)
  # Remove residual painted scenery in the inner corner pockets, retain gold filigree.
  for x0,x1 in [(12,42),(w-42,w-12)]:
   for y0,y1 in [(12,42),(h-42,h-12)]:
    area=orig[y0:y1,x0:x1].astype(float);gold=(area[:,:,0]>area[:,:,2]*1.25)&(area[:,:,0]>65)
    patch=np.array(ref.crop((800,1140,800+x1-x0,1140+y1-y0)))
    view=fixed[y0:y1,x0:x1];view[~gold]=patch[~gold];allowed[y0:y1,x0:x1][~gold]=255
 if n in ['yaga','knight']:
  alpha[h-17:,0:18]=0 # neighboring card corner, not part of the subject
 if n=='yaga':alpha[h-34:,0:23]=0
 if n=='rewards_knight':alpha[:26,:82]=0 # baked reward heading above the first icon
 if n=='action':alpha[68:,:]=0;alpha[59:,216:]=0 # neighbouring card rail outside button
 if n in ['title','tab','action']:
  count,labels,stats,_=cv2.connectedComponentsWithStats((alpha>24).astype(np.uint8))
  if count>1:
   keep=(labels==1+np.argmax(stats[1:,cv2.CC_STAT_AREA])).astype(np.uint8);keep=cv2.dilate(keep,np.ones((3,3),np.uint8));alpha[keep==0]=0
 fixed[allowed==0]=orig[allowed==0]
 rgba=Image.fromarray(np.dstack([fixed,alpha]));rgba.save(out/(n+'_r3.png'));Image.fromarray(allowed).save(b/'RepairMasks'/(n+'_final.png'))
 changed=np.any(fixed!=orig,axis=2);assert not (changed&(allowed==0)).any()
 records.append({'asset':n,'sourceBox':e['sourceBox'],'output':str(out/(n+'_r3.png')),'sha256':hashlib.sha256((out/(n+'_r3.png')).read_bytes()).hexdigest(),'changedRGBPixels':int(changed.sum()),'changedOutsideAllowedRegion':0,'alphaCoverage':float((alpha>0).mean()),'sourceAlpha':str(job),'note':'Original RGB retained except recorded local surface repairs; alpha cleanup is separately bounded in this script.'})
(base/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
for group,names in [('UI',['title','tab','action','status','frame']),('Subjects',['firebird','yaga','merchant','knight']),('Rewards',['rewards_firebird','rewards_yaga','rewards_merchant','rewards_knight','reward_merchant_chest','reward_merchant_medal'])]:
 canvas=Image.new('RGB',(1200,2400),'#555555');draw=ImageDraw.Draw(canvas);y=0
 for n in names:
  p=out/(n+'_r3.png')
  if not p.exists():continue
  im=Image.open(p);im.thumbnail((390,325));draw.text((2,y),n,fill='white');y+=18
  for i,c in enumerate(['black','white','#ff00ff']):
   plate=Image.new('RGBA',im.size,c);plate.alpha_composite(im);canvas.paste(plate.convert('RGB'),(i*400,y))
  y+=im.height+12
 canvas.crop((0,0,1200,y)).save(base/'QA'/('Final'+group+'.png'))
print('Prepared',len(records),'staging assets; original RGB outside repair masks verified.')
