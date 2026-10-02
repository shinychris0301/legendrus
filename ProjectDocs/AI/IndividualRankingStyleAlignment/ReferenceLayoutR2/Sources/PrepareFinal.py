from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;qa=b.parent/'QA';out=b/'Final';out.mkdir(exist_ok=True);(b/'FinalMasks').mkdir(exist_ok=True)
ref=Image.open(b/'OriginalReference.png').convert('RGB');assessment=json.loads((b/'AssetAssessment.json').read_text());records=[]
centers=[(202,651,40,39),(202,723,40,39),(202,792,40,38),(202,854,40,36),(203,918,40,36),(202,978,40,36),(202,1040,40,36),(202,1100,40,35),(202,1159,40,36),(202,1217,40,34),(201,1298,43,42),(201,1369,40,36),(201,1426,40,35),(201,1505,40,37)]
def shape(size,ellipse=None,poly=None,rounded=None):
 m=Image.new('L',(size[0]*4,size[1]*4));d=ImageDraw.Draw(m)
 if ellipse:d.ellipse(tuple(round(x*4) for x in ellipse),fill=255)
 if poly:d.polygon([(round(x*4),round(y*4)) for x,y in poly],fill=255)
 if rounded:d.rounded_rectangle(tuple(round(x*4) for x in rounded[:4]),radius=rounded[4]*4,fill=255)
 return m.resize(size,Image.Resampling.LANCZOS)
def api_alpha(r,size):
 name=r['name'];single=b/'Scenario'/name/'booster_medallion_trial01.png'
 if name=='chest_bronze':single=b/'Scenario/chest_bronze_corrected/booster_medallion_trial01.png'
 if single.exists():return Image.open(single).convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS),'individual Scenario alpha'
 if 'atlas' in r:
  im=Image.open(b/'Scenario'/(r['atlas']+'_atlas')/'booster_medallion_trial01.png').convert('RGBA')
  expected=(768,256) if r['atlas']=='row_rewards' else (1024,1024);im=im.resize(expected,Image.Resampling.LANCZOS)
  return im.crop(r['atlasBox']).getchannel('A').resize(size,Image.Resampling.LANCZOS),'atlas Scenario alpha'
 return Image.new('L',size,255),'opaque source panel (no isolation required)'
portrait_index=0
for r in assessment['assets']:
 name=r['name']
 if name=='footer':continue # irrecoverable composite; reuse established independent footer ornament instead
 box=[628,446,762,541] if name=='chest_bronze' else r['sourceBox'];original=ref.crop(box);rgb=np.array(original if name=='chest_bronze' else Image.open(b/'Inputs'/(name+'.png')).convert('RGB'));h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8) if name=='chest_bronze' else np.array(Image.open(b/'RepairMasks'/(name+'.png')));alpha,method=api_alpha(r,(w,h));cleanup=[]
 if r['group']=='portraits':
  cx,cy,rx,ry=centers[portrait_index];portrait_index+=1;cx-=box[0];cy-=box[1]
  # API retained adjacent rails/heads. A bounded source-frame contour removes only exterior pixels.
  alpha=shape((w,h),ellipse=(cx-rx,cy-ry,cx+rx,cy+ry));
  if name in ['portrait_solovey','portrait_vodyanoi','portrait_zmei']:
   stop={'portrait_solovey':1384,'portrait_vodyanoi':1447,'portrait_zmei':1240}[name]-box[1];a=np.array(alpha);a[stop:]=0;alpha=Image.fromarray(a)
  cleanup.append('Source portrait contour replaces contaminated exterior alpha; opaque portrait interior retained')
 if name in ['medal_gold','medal_silver','medal_bronze','rank_plain','rank_player']:
  rgb=np.array(original);h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8)
  # Remove only numeral strokes rather than blurring a rectangular face.
  if name.startswith('medal'):x0,y0,x1,y1=30,18,w-26,h-16
  else:x0,y0,x1,y1=13,12,w-11,h-10
  # Lettering shadows remain after bright-stroke removal. Reconstruct only the inner disc.
  rx=(w*.245 if name.startswith('medal') else w*.315);ry=(h*.285 if name.startswith('medal') else h*.31)
  cx=w*.50;cy=h*.52
  mask=np.array(shape((w,h),ellipse=(cx-rx,cy-ry,cx+rx,cy+ry)))
  yy,xx=np.mgrid[:h,:w];t=np.clip((yy-(cy-ry))/(2*ry),0,1)
  base={'medal_gold':(68,46,14),'medal_silver':(38,45,48),'medal_bronze':(75,34,14),'rank_plain':(37,31,20),'rank_player':(8,43,65)}[name]
  texture=np.array(original).mean(2);texture=cv2.GaussianBlur(texture,(0,0),2);texture=(texture-texture.mean())*.06
  face=np.stack([np.clip(c*(1.1-.3*t)+texture,0,255) for c in base],2)
  blend=mask.astype(float)[:,:,None]/255;rgb=np.rint(rgb*(1-blend)+face*blend).astype(np.uint8)
  if name.startswith('medal'):
   core=shape((w,h),ellipse=(18,15,w-15,h-6));p=np.array(original).astype(float);metal=(p[:,:,0]>75)&(p[:,:,1]>55)&((p[:,:,0]>p[:,:,2]*1.2) if name!='medal_silver' else ((p.max(2)-p.min(2))<65));metal[:2]=False;metal[-9:]=False;metal[:3,:10]=False;metal[:3,-10:]=False
   a=np.maximum(np.array(core),metal.astype(np.uint8)*255);alpha=Image.fromarray(a);cleanup.append('Preserve opaque medal face and source metallic laurel silhouette; remove rail fragments')
  else:alpha=shape((w,h),ellipse=(2,3,w-2,h-2));cleanup.append('Opaque circular source badge; numeral replaced by live text')
 if name=='title':
  allowed=shape((w,h),poly=[(0,89),(53,57),(64,44),(276,44),(300,23),(345,0),(393,0),(433,24),(445,43),(691,43),(741,74),(742,148),(708,165),(17,165),(0,135)])
  face=shape((w,h),poly=[(62,61),(681,61),(706,92),(690,134),(37,134),(22,111)])
  a=np.minimum(np.maximum(np.array(alpha),np.array(face)),np.array(allowed));a[155:]=0;alpha=Image.fromarray(a);cleanup.append('Restore opaque plaque face; exclude adjacent profile and timer ornaments')
 if name in ['tab','tab_active','rules']:
  face=shape((w,h),rounded=(12,12,w-12,h-12,8));limit=shape((w,h),rounded=(2,2,w-2,h-2,10));alpha=Image.fromarray(np.minimum(np.maximum(np.array(alpha),np.array(face)),np.array(limit)));cleanup.append('Keep opaque button face; trim exterior panel fragments')
 if name in ['reward_frame','reward_panel','table_frame','row','row_player']:
  # Opaque panel faces have all source text/portraits/icons removed under a recorded repair mask.
  alpha=shape((w,h),rounded=(1,1,w-1,h-1,6 if name.startswith('row') else 12))
  if not name.startswith('row'):
   face=shape((w,h),rounded=(12,12,w-12,h-12,4));p=rgb.astype(float);metal=(p[:,:,0]>p[:,:,1]*1.08)&(p[:,:,1]>p[:,:,2]*1.12)&(p[:,:,0]>48);a=np.maximum(np.array(face),metal.astype(np.uint8)*255);alpha=Image.fromarray(np.minimum(a,np.array(alpha)));cleanup.append('Remove scenery and stray blue icon remnants outside opaque face; retain warm metallic border')
  if name in ['reward_panel','table_frame']:
   tile=np.array(ref.crop((420,904,514,935)));tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);face=np.tile(tile,(h//tile.shape[0]+1,w//tile.shape[1]+1,1))[:h,:w];rgb[mask>0]=face[mask>0]
  if name.startswith('row'):
   original_rgb=np.array(original);mask[:]=255
   # Source top/bottom strips repeated from a clear centre section remove overhanging portraits/rewards.
   donor=ref.crop((420,904,514,935)) if name=='row' else ref.crop((696,1282,738,1320))
   tile=np.array(donor);tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);rgb=np.tile(tile,(h//tile.shape[0]+1,w//tile.shape[1]+1,1))[:h,:w]
   edge=7 if name=='row' else 10
   for sy in [0,h-edge]:
    strip=original_rgb[sy:sy+edge,375:410];rgb[sy:sy+edge]=np.tile(strip,(1,w//35+1,1))[:,:w]
   if name=='row':
    rail=np.array(ref.crop((410,889,445,893)));rgb[:4]=np.tile(rail,(1,w//35+1,1))[:,:w];rgb[-4:]=rgb[:4]
   rgb[:,:10]=original_rgb[:,:10];rgb[:,-10:]=original_rgb[:,-10:];mask[:,:10]=0;mask[:,-10:]=0
   cleanup.append('Empty face and interrupted horizontal rails reconstructed from clean source donor strips')
 if name=='back':alpha=shape((w,h),ellipse=(7,8,w-5,h-4));cleanup.append('Circular Back control separated from carved footer')
 if name=='clock':alpha=shape((w,h),ellipse=(5,7,w-4,h-3));cleanup.append('Exclude timer badge rail')
 if name=='power':
  # Tiny isolated gold silhouette; the atlas model kept its panel rectangle.
  p=rgb.astype(float);a=((p[:,:,0]>90)&(p[:,:,1]>55)&(p[:,:,0]>p[:,:,2]*1.45)).astype(np.uint8)*255;alpha=Image.fromarray(a).filter(ImageFilter.MaxFilter(3));cleanup.append('Gold silhouette cleanup inside source icon bounds')
 if name=='gem':
  box=[228,474,255,506];original=ref.crop(box);rgb=np.array(original);h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8);alpha=shape((w,h),poly=[(6,3),(20,3),(26,11),(14,30),(1,11)]);cleanup.append('Corrected clipped gem source bounds; source diamond contour retained')
 if name=='card':alpha=shape((w,h),poly=[(8,1),(33,4),(27,33),(0,29)]);cleanup.append('Card silhouette excludes source surface')
 if name.startswith('chest_') or name.startswith('row_chest_'):
  a=np.array(alpha);n,labels,stats,_=cv2.connectedComponentsWithStats((a>90).astype(np.uint8),8);label=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);core=(labels==label).astype(np.uint8);core=cv2.dilate(core,np.ones((3,3),np.uint8));a[core==0]=0;alpha=Image.fromarray(a);cleanup.append('Remove disconnected neighboring label fragments after Scenario isolation')
 rgba=Image.fromarray(rgb).convert('RGBA');rgba.putalpha(alpha)
 # High-quality interpolation only; this does not claim additional source detail.
 scale=3 if max(w,h)<300 else 2 if max(w,h)<1000 else 1;rgba=rgba.resize((w*scale,h*scale),Image.Resampling.LANCZOS)
 rgba.save(out/(name+'.png'));Image.fromarray(mask).save(b/'FinalMasks'/(name+'.png'))
 changed=np.any(np.array(original)!=rgb,axis=2);assert not np.any(changed&(mask==0)),name
 records.append({'name':name,'sourceBox':box,'alphaMethod':method,'localCleanup':cleanup,'repairMask':'FinalMasks/'+name+'.png','repairedPixels':int((mask>0).sum()),'sourceRGBOutsideRepairMask':'verified identical','outputSize':list(rgba.size),'scale':scale,'sha256':hashlib.sha256((out/(name+'.png')).read_bytes()).hexdigest()})
background=b/'Scenario/background/booster_medallion_trial01.png';Image.open(background).convert('RGBA').save(out/'background.png');records.append({'name':'background','classification':'Scenario generated clean scenery only','sha256':hashlib.sha256((out/'background.png').read_bytes()).hexdigest()})
(b/'FinalAssets.json').write_text(json.dumps(records,indent=2)+'\n')
# QA sheets are evidence only, never runtime assets.
for color,label in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
 assets=[p for p in sorted(out.glob('*.png')) if p.stem!='background'];sheet=Image.new('RGB',(1100,((len(assets)+4)//5)*180),color);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(assets):
  im=Image.open(p).convert('RGBA');im.thumbnail((206,146),Image.Resampling.LANCZOS);x=i%5*220+(220-im.width)//2;y=i//5*180+20;sheet.paste(im,(x,y),im);d.text((i%5*220+5,i//5*180+3),p.stem,fill=(128,128,128))
 sheet.save(qa/('AssetsOn'+label+'.jpg'),quality=92)
print('Prepared',len(records),'final assets; source RGB repair-mask checks passed.')
