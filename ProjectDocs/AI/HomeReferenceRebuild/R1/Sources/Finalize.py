from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np, cv2,json,hashlib,shutil
b=Path(__file__).resolve().parent;out=b/'Final';out.mkdir(exist_ok=True);source=Image.open(b/'OriginalReference.png').convert('RGBA');bounds=json.loads((b/'Assessment.json').read_text())['bounds'];layers={};records={}
def save(n,im,box=None,repair=None):
 im.save(out/(n+'.png'));records[n]={'size':list(im.size),'sourceBounds':box,'repair':repair,'sha256':hashlib.sha256((out/(n+'.png')).read_bytes()).hexdigest()}
def maskpoly(size,pts):
 m=Image.new('L',size);ImageDraw.Draw(m).polygon(pts,fill=255);return m
def blank_caption(im,n):
 a=np.array(im);mask=np.zeros(a.shape[:2],np.uint8)
 if n=='play':
  # Reconstruct only the English blank face from original unlettered emerald texture.
  donor=im.crop((85,96,117,165)).resize(im.size,Image.Resampling.BICUBIC).filter(ImageFilter.GaussianBlur(2))
  mask=Image.new('L',im.size);ImageDraw.Draw(mask).rectangle((116,86,474,178),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(7))
  repaired=Image.composite(donor,im,mask);repaired.putalpha(im.getchannel('A'));a=np.array(repaired)
 else:
  # Preserve the original thin gold caption rails and corner bevels.
  mask=maskpoly(im.size,[(30,178),(im.width-31,178),(im.width-20,187),(im.width-20,197),(im.width-30,207),(30,207),(21,197),(21,187)])
  rng=np.random.default_rng(41);noise=rng.normal(0,1,(im.height,im.width,1));patch=np.clip(np.array([8,23,31])[None,None,:]+noise,0,255).astype('uint8')
  aa=np.array(mask);a[aa>0,:3]=patch[aa>0]
 return Image.fromarray(a)
def inpaint(im,rects):
 a=np.array(im);mask=np.zeros(a.shape[:2],np.uint8)
 for x,y,r,t in rects:mask[y:t,x:r]=255
 a[:,:,:3]=cv2.inpaint(a[:,:,:3],mask,4,cv2.INPAINT_TELEA);return Image.fromarray(a)
for n,box in bounds.items():
 im=source.crop(box);a=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS);im.putalpha(a);layers[n]=im
# Keep largest connected panel component; remove neighboring card tips under Start.
p=layers['play'];a=np.array(p.getchannel('A'));a[229:,:]=0
num,labels,stats,_=cv2.connectedComponentsWithStats((a>12).astype('uint8'));idx=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);a[labels!=idx]=0;p.putalpha(Image.fromarray(a));layers['play']=p
# Restore opaque panel interiors only; preserve actual exterior Scenario silhouette.
for n,rect in [('logo',(105,78,425,310)),('play',(83,71,500,188)),('map',(27,52,177,197)),('shop',(25,52,177,197)),('events',(25,52,177,197)),('characters',(25,52,177,197)),('footer',(25,40,900,180)),('header',(173,32,905,121))]:
 im=layers[n];a=im.getchannel('A');ImageDraw.Draw(a).rectangle(rect,fill=255);im.putalpha(a)
# Original character minus the separately extracted overlapping controls, with their same native positions.
hero=layers['hero'];ha=np.array(hero.getchannel('A'));hy=bounds['hero'][1]
for n in ['play','map','shop','events','characters']:
 x,y,r,t=bounds[n];a=np.array(layers[n].getchannel('A'));a=cv2.dilate(a,np.ones((3,3),np.uint8));sub=ha[y-hy:t-hy,x:r];ha[y-hy:t-hy,x:r]=np.minimum(sub,255-a)
# Restore the independent lower tail mask, excluded by the broader subject isolation.
# Supplemental Scenario attempt selected the card instead of feathers: reject its alpha.
# Bounded manual silhouette restores original tail pixels, with warm-pixel edge support at the fine tips.
x,y,r,t=bounds['tail'];rgb=np.array(source.crop(bounds['tail']))[:,:,:3]
poly=maskpoly((r-x,t-y),[(0,0),(206,0),(205,121),(198,172),(189,216),(174,251),(159,288),(145,316),(133,310),(0,237)])
ta=np.array(poly.filter(ImageFilter.GaussianBlur(.55)))
warm=((rgb[:,:,0].astype(float)>rgb[:,:,2]*1.35)&(rgb[:,:,0]>65)).astype('uint8')*255
warm=cv2.morphologyEx(warm,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8))
ta[215:]=np.minimum(ta[215:],warm[215:]);ha[y-hy:t-hy,x:r]=np.maximum(ha[y-hy:t-hy,x:r],ta)
n='characters';x,y,r,t=bounds[n];a=cv2.dilate(np.array(layers[n].getchannel('A')),np.ones((3,3),np.uint8));ha[y-hy:t-hy,x:r]=np.minimum(ha[y-hy:t-hy,x:r],255-a)
# Remove disconnected residual card fragments under the panel row.
ha[982:]=0;hero.putalpha(Image.fromarray(ha));save('source_hero',hero,bounds['hero'],'Alpha-only removal of overlapping native controls; source RGB unchanged')
for n in ['logo','avatar']:save('source_'+n,layers[n],bounds[n])
for n in ['play','map','shop','events','characters']:
 im=layers[n];save('source_'+n+'_ru',im,bounds[n])
 rect=(126,88,460,171) if n=='play' else (17,175,191,210)
 save('source_'+n+'_blank',blank_caption(im,n),bounds[n],'Bounded static lettering inpaint for English only')
# Header is a real opaque lacquer panel; dynamic names/numbers are removed before native Text is placed.
h=layers['header'];h=inpaint(h,[(178,35,310,102),(405,57,475,84),(597,57,646,83),(748,55,802,84),(191,112,285,125)])
a=np.array(h.getchannel('A'));a[:,:174]=0;a[129:]=0;h.putalpha(Image.fromarray(a))
ha=np.array(h);ha[114:123,193:320,:3]=(15,19,25);h=Image.fromarray(ha)
# Native resource panels and settings own their own graphic (not invisible hotspots on a screenshot).
for n,box in {'gold':[344,39,524,100],'gems':[529,39,694,100],'energy':[702,39,851,100],'settings':[853,23,941,118]}.items():
 save('source_'+n,h.crop(box),box,'Saved values removed locally' if n!='settings' else None)
 a=np.array(h.getchannel('A'));a[box[1]:box[3],box[0]:box[2]]=0;h.putalpha(Image.fromarray(a))
save('source_header',h,bounds['header'],'Removed dynamic labels, progress fill and native control regions')
# Original plus icon through a circular alpha, reused by all three resource controls.
box=[477,47,521,94];p=source.crop(box);a=Image.new('L',p.size);ImageDraw.Draw(a).ellipse((1,1,42,44),fill=255);p.putalpha(a);save('source_plus',p,box)
# Footer split into independent native mail, quest and gift surfaces; full original carved backing remains behind.
f=layers['footer'];f=inpaint(f,[(385,115,456,147),(640,121,703,164)])
# Empty quest track uses its original unfilled dark wood donor; no invented 15/30 progress.
a=np.array(f);donor=np.array(source.crop((475,1568,550,1590)));trackmask=np.zeros(a.shape[:2],np.uint8);cv2.fillPoly(trackmask,[np.array([(248,108),(574,108),(588,126),(574,139),(248,139),(237,124)])],255)
dark=np.array(Image.fromarray(donor[:,:,:3]).resize((a.shape[1],a.shape[0])));a[trackmask>0,:3]=dark[trackmask>0]
f=Image.fromarray(a)
for n,box in {'mail':[0,5,184,182],'quest_ru':[184,0,758,190],'gift':[758,5,941,182]}.items():
 im=f.crop(box)
 if n=='gift':
  # Badge is an independent state-driven child. Remove original badge footprint from gift alpha.
  aa=np.array(im);mm=np.zeros(aa.shape[:2],np.uint8);cv2.circle(mm,(142,24),27,255,-1);donor=aa[:,::-1].copy();aa[mm>0]=donor[mm>0];im=Image.fromarray(aa)
 save('source_'+n,im,[box[0],box[1]+1454,box[2],box[3]+1454], 'Footer dynamic progress/reward cleared; badge separate')
 if n=='quest_ru':save('source_quest_blank',inpaint(im,[(61,40,418,82)]),None,'English-only caption removal')
 # Separate islands remain in the carved base.
 a=np.array(f.getchannel('A'));a[box[1]:box[3],box[0]:box[2]]=0;f.putalpha(Image.fromarray(a))
save('source_footer_carving',f,bounds['footer'])
box=[875,1455,928,1510];im=source.crop(box);a=Image.new('L',im.size);ImageDraw.Draw(a).ellipse((2,2,51,53),fill=255);im.putalpha(a);save('source_daily_badge',im,box)
shutil.copy2(b/'Scenario/backgrounds/booster_medallion_trial01.png',out/'clean_environment.png');records['clean_environment']={'method':'Scenario reconstructs hidden village only','sha256':hashlib.sha256((out/'clean_environment.png').read_bytes()).hexdigest()}
(b/'FinalManifest.json').write_text(json.dumps(records,indent=2))
# One-page offline assembly for checking alpha registration, never a runtime dependency.
canvas=Image.open(out/'clean_environment.png').convert('RGBA').resize(source.size)
for n,box in [('source_hero',bounds['hero']),('source_header',bounds['header']),('source_logo',bounds['logo']),('source_avatar',bounds['avatar'])]+[(f'source_{n}_ru',bounds[n])for n in ['play','map','shop','events','characters']]+[(n,r['sourceBounds'])for n,r in records.items() if n in ['source_gold','source_gems','source_energy','source_settings','source_mail','source_quest_ru','source_gift','source_footer_carving','source_daily_badge']]:
 canvas.alpha_composite(Image.open(out/(n+'.png')),(box[0],box[1]))
canvas.save(b.parent/'QA/Assembly_OFFLINE_NOT_UNITY.png')
# Transparency contact sheets, split into readable sizes.
names=[p for p in out.glob('*.png') if p.name!='clean_environment.png']
for bg,name in [((0,0,0),'black'),((255,255,255),'white'),((255,0,255),'magenta')]:
 sheet=Image.new('RGB',(1500,((len(names)+4)//5)*310),bg);d=ImageDraw.Draw(sheet)
 for i,p in enumerate(names):
  im=Image.open(p).convert('RGBA');im.thumbnail((290,276));x=(i%5)*300;y=(i//5)*310;sheet.paste(im,(x,y+20),im);d.text((x,y),p.stem,fill='gray')
 sheet.save(b.parent/'QA'/('Alpha_'+name+'.jpg'))
