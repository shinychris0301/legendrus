from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned');ref=Image.open(b/'OriginalReference.png').convert('RGB');audit={}
def save(n,im,original=None,notes=''):
 p=out/(n+'.png');im.save(p);audit[n]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'size':im.size,'notes':notes}
 if original is not None:
  changed=np.any(np.array(im.convert('RGB'))!=np.array(original.convert('RGB')),axis=2);Image.fromarray((changed*255).astype('uint8')).save(b/(n+'_RGBRepairMask.png'));audit[n]['changedRGBPixels']=int(changed.sum());audit[n]['alphaUnchanged']=bool(np.array_equal(np.array(im.getchannel('A')),np.array(original.getchannel('A'))))
# Restore only visible original scenery; existing generated environment fills hidden pixels.
m=np.zeros((1672,941),np.uint8)
for n,pos in [('header_ru',(0,0)),('banner_ru',(156,194)),('foreground_ru',(29,392)),('footer',(5,1037)),('scene_rail_left',(0,200)),('scene_rail_right',(866,200))]:
 layer=Image.new('L',ref.size);layer.paste(Image.open(out/(n+'.png')).getchannel('A'),pos);m=np.maximum(m,np.array(layer))
m=cv2.dilate(m,np.ones((19,19),np.uint8));m=np.array(Image.fromarray(m).filter(ImageFilter.GaussianBlur(4)));m[m>245]=255
crop=[0,150,941,1091];a=m[150:1091,:];f=a[:,:,None]/255.;old=np.array(Image.open(out/'environment.png').convert('RGB').resize((941,941),Image.Resampling.LANCZOS));original=np.array(ref.crop(crop));rgb=(original*(1-f)+old*f).astype('uint8');save('environment_source_r8',Image.fromarray(rgb),notes='Original visible scene; earlier Scenario reconstruction only beneath expanded UI/actor masks');Image.fromarray(a).save(b/'SceneryReconstructionMask.png');audit['environment_source_r8']['unchangedSourcePixels']=int(np.all(rgb==original,axis=2).sum())
# Fixed next-chapter lettering: isolate gold, no wood patch behind it.
box=[302,1393,641,1428];rgb=np.array(ref.crop(box));r,g,bb=[rgb[:,:,i].astype(float) for i in range(3)];alpha=(np.clip(np.minimum.reduce([(r-bb-8)/20,(g-bb-3)/13,(r-26)/30]),0,1)*255).astype('uint8');alpha[(r<80)|(g<58)]=0
count,labels,stats,_=cv2.connectedComponentsWithStats((alpha>0).astype('uint8'),8)
keep=[i for i in range(1,count) if stats[i,cv2.CC_STAT_HEIGHT]>=15 and stats[i,cv2.CC_STAT_AREA]>=35]
alpha[~np.isin(labels,keep)]=0
im=Image.fromarray(np.dstack([rgb,alpha]));save('next_heading_ru_r8',im,im.copy(),'Original RGB and extracted gold alpha; upcoming status stays live');audit['next_heading_ru_r8']['sourceBox']=box
# Source frame/alpha plus repaired localized lettering faces from one atlas job.
p=b/'Scenario/faces/booster_medallion_trial01.png'
if p.exists():
 atlas=Image.open(p).convert('RGB').resize((1024,1024),Image.Resampling.LANCZOS);entries=json.loads((b/'FaceAtlasMap.json').read_text());boxes={'banner':[(83,48,548,157)],'next':[(101,20,297,78)],'map':[(83,21,314,76)],'foreground':[(331,639,548,688)],'teaser':[(70,23,490,178)]}
 for n,entry in entries.items():
  src=Image.open(out/(n+'_ru.png')).convert('RGBA');donor=src.convert('RGB');piece=atlas.crop(entry['atlas']);donor.paste(piece,entry['crop'][:2]);mask=Image.new('L',src.size);d=ImageDraw.Draw(mask)
  for box in boxes[n]:d.rectangle(box,fill=255)
  mask=mask.filter(ImageFilter.GaussianBlur(2));f=np.array(mask)[:,:,None]/255.;rgb=(np.array(src.convert('RGB'))*(1-f)+np.array(donor)*f).astype('uint8');im=Image.fromarray(rgb).convert('RGBA');im.putalpha(src.getchannel('A'));save(n+'_blank_r8',im,src,'One Scenario face repair, bounded feathered composite; original alpha and outside-mask RGB preserved');mask.save(b/(n+'_FaceMask.png'))
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2))
for label,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(235,0,160))]:
 sheet=Image.new('RGB',(1050,1100),col);x=y=10;rh=0
 for n in audit:
  if n.startswith('environment'):continue
  im=Image.open(out/(n+'.png'));im.thumbnail((700,540))
  if x+im.width>1050:x=10;y+=rh+20;rh=0
  sheet.paste(im,(x,y),im);x+=im.width+20;rh=max(rh,im.height)
 sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
print('Prepared',len(audit),'assets')
