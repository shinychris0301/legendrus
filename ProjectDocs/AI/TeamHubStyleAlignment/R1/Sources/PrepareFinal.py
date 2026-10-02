from pathlib import Path
from PIL import Image,ImageDraw
import cv2,numpy as np,json,hashlib,shutil
b=Path(__file__).resolve().parent;out=b/'Final';out.mkdir(exist_ok=True);qa=b.parent/'QA';qa.mkdir(exist_ok=True);records=json.loads((b/'Assessment.json').read_text())['assets'];ref=Image.open(b/'OriginalReference.png').convert('RGB');audit=[]
def shape(size,kind,coords):
 im=Image.new('L',(size[0]*3,size[1]*3));d=ImageDraw.Draw(im)
 if kind=='ellipse':d.ellipse(tuple(x*3 for x in coords),fill=255)
 else:d.polygon([(x*3,y*3) for x,y in coords],fill=255)
 return np.array(im.resize(size,Image.Resampling.LANCZOS))
for r in records:
 n=r['name'];original=Image.open(b/'Crops'/(n+'.png')).convert('RGB');w,h=original.size;rgb=np.array(original);repair=np.zeros((h,w),np.uint8);cleanup=[]
 if 'atlas' in r:
  raw=Image.open(b/'Scenario'/r['atlas']/'booster_medallion_trial01.png').convert('RGBA');a=np.array(raw.crop(r['atlasBox']).getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
 else:a=np.array(Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
 if n.startswith('portrait_'):
  # Only the occluded lower cap is reconstructed; original face and upper ornament stay untouched.
  mapping={x['name']:x['box'] for x in json.loads((b/'PortraitRepairMap.json').read_text())};generated=Image.open(b/'ScenarioPortraitCaps/booster_medallion_trial01.png').convert('RGB');cap=np.array(generated.crop(mapping[n]).resize((w,h),Image.Resampling.LANCZOS))
  stop={'portrait_ivan':85,'portrait_olga':86,'portrait_svyatoslav':78,'portrait_velimir':78,'portrait_lada':73}[n]
  yy,xx=np.mgrid[:h,:w];blend=np.clip((yy-(stop-5))/5.,0,1);repair[blend>0]=255;rgb=np.rint(rgb*(1-blend[:,:,None])+cap*blend[:,:,None]).astype('uint8')
  originalAlpha=shape((w,h),'ellipse',(2,3,w-3,h-1));fg=((cap.max(2).astype(float)-cap.min(2)>22)&(cap.max(2)>55)).astype('uint8');cs,_=cv2.findContours(fg,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);ga=np.zeros((h,w),'uint8');cv2.fillConvexPoly(ga,cv2.convexHull(max(cs,key=cv2.contourArea)),255);edgeBlend=np.clip((yy-(stop-14))/9.,0,1);a=np.rint(originalAlpha*(1-edgeBlend)+ga*edgeBlend).astype('uint8');cleanup.append('Scenario generated only the inaccessible lower collar/beard/rim cap; composite below recorded boundary. Original face and upper RGB retained. Generated full portraits remain evidence only.')
 elif n=='title_ru':
  allowed=shape((w,h),'poly',[(0,58),(170,58),(185,23),(230,5),(295,0),(340,18),(391,56),(585,56),(585,146),(326,146),(298,171),(279,171),(248,146),(0,146)])
  face=shape((w,h),'poly',[(92,61),(499,61),(510,98),(491,140),(83,140),(77,99)]);a=np.minimum(np.maximum(a,face),allowed);cleanup.append('Remove header progress/banner remnants; preserve opaque plaque face')
 elif n=='banner':
  a[-4:]=0;cleanup.append('Remove bottom task-panel rail; banner bottom is hidden by reference task panel')
 elif n.endswith('_frame'):
  donor=ref.crop((515,1190,630,1231)) if n!='donate_frame' else ref.crop((515,1190,630,1231));tile=Image.new('RGB',(w,h))
  for y in range(0,h,donor.height):
   for x in range(0,w,donor.width):tile.paste(donor,(x,y))
  
  if n=='donate_frame':
   arr=np.array(tile).astype(float);arr[:,:,0]*=.6;arr[:,:,1]*=1.5;arr[:,:,2]*=.65;tile=Image.fromarray(np.clip(arr,0,255).astype('uint8'))
  repair[11:h-11,11:w-11]=255;rgb[repair>0]=np.array(tile)[repair>0];face=shape((w,h),'poly',[(8,8),(w-8,8),(w-8,h-8),(8,h-8)]);a=np.maximum(a,face);a[:3]=0;a[-3:]=0;a[:,:3]=0;a[:,-3:]=0;cleanup.append('Expanded blank-face repair removes remaining source labels/icon tips; opaque face retained')
 else:
  a=np.array(Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
  if n=='chat':
   cs,_=cv2.findContours((a>100).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);filled=np.zeros_like(a);cv2.drawContours(filled,[max(cs,key=cv2.contourArea)],-1,255,-1);a=np.maximum(a,filled);cleanup.append('Restore intentionally opaque speech-bubble face inside original outer rim')
  if n=='task_banner':a[98:]=0;cleanup.append('Remove neighboring task card rail below banner')
  cleanup.append('Individual Scenario alpha isolation supersedes rejected grouped extraction and local contour drafts; retain original RGB')

 rgba=Image.fromarray(rgb).convert('RGBA');rgba.putalpha(Image.fromarray(a));rgba=rgba.resize((w*3,h*3),Image.Resampling.LANCZOS);rgba.save(out/(n+'.png'));Image.fromarray(repair).save(b/'RepairMasks'/(n+'.png'))
 assert np.array_equal(rgb[repair==0],np.array(original)[repair==0])
 audit.append({'name':n,'sourceBox':r['box'],'sourceRGBOutsideRepairMaskIdentical':True,'repairedPixels':int((repair>0).sum()),'cleanup':cleanup,'outputSHA256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()})
 if n=='title_ru':
  blank=rgb.copy();mask=np.zeros((h,w),np.uint8);mask[67:135,126:457]=255;donor=np.array(ref.crop((515,1190,630,1231)));tile=np.tile(donor,(h//donor.shape[0]+1,w//donor.shape[1]+1,1))[:h,:w];blank[mask>0]=tile[mask>0];im=Image.fromarray(blank).convert('RGBA');im.putalpha(Image.fromarray(a));im.resize((w*3,h*3),Image.Resampling.LANCZOS).save(out/'title_blank.png');Image.fromarray(mask).save(b/'RepairMasks/title_blank.png');audit.append({'name':'title_blank','sourceBox':r['box'],'repairedPixels':int((mask>0).sum()),'cleanup':['Original title glyph footprint replaced with clean source lacquer for live English title'],'outputSHA256':hashlib.sha256((out/'title_blank.png').read_bytes()).hexdigest()})
shutil.copyfile(b/'Scenario/background/booster_medallion_trial01.png',out/'background.png');audit.append({'name':'background','operation':'Scenario generated clean scenery; hidden environment cannot be extracted','outputSHA256':hashlib.sha256((out/'background.png').read_bytes()).hexdigest()})
for bg,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(210,0,145))]:
 sheet=Image.new('RGB',(1000,((len(records)+1+3)//4)*210),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(sorted(x for x in out.glob('*.png') if x.stem!='background')):
  im=Image.open(p);im.thumbnail((235,175));x=(i%4)*250+(250-im.width)//2;y=(i//4)*210;tile=Image.new('RGBA',im.size,color+(255,));tile.alpha_composite(im);sheet.paste(tile.convert('RGB'),(x,y));d.text(((i%4)*250+5,y+181),p.stem,fill='white' if bg!='White' else 'black')
 sheet.save(qa/('AssetsOn'+bg+'.png'))
(b.parent/'AssetProvenance.json').write_text(json.dumps(audit,indent=2))
print('Prepared',len(audit),'assets')
