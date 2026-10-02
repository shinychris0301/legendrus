from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;q=b.parent/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned');art.mkdir(exist_ok=True)
boxes=json.loads((b/'SourceBoxes.json').read_text());g=Image.open(b/'Scenario/surfaces/booster_medallion_trial01.png').convert('RGBA');api=Image.open(b/'Scenario/isolate/booster_medallion_trial01.png').convert('RGBA')
audit={}
def save(name,im,source=None):
 im.save(art/(name+'.png'))
 if source is not None:
  a=np.array(im);s=np.array(source);mask=a[:,:,3]>0;audit[name]={'visiblePixels':int(mask.sum()),'RGBMismatches':int(np.any(a[:,:,:3][mask]!=s[:,:,:3][mask],axis=1).sum())}
def gray_alpha(im):
 a=np.array(im)[:,:,:3].astype(float);grey=((a.max(2)-a.min(2))<23)&(a.mean(2)>20)
 # Only gray connected to exterior is removable; intentional panel faces remain opaque.
 n,lab=cv2.connectedComponents(grey.astype('uint8'));ids=np.unique(np.r_[lab[0,:],lab[-1,:],lab[:,0],lab[:,-1]]);ids=ids[ids!=0]
 alpha=(~np.isin(lab,ids)).astype('uint8')*255
 return Image.fromarray(alpha)
def isolate_icon(im,key):
 a=np.array(im.convert('RGB'));h,w=a.shape[:2];m=np.full((h,w),cv2.GC_PR_BGD,np.uint8)
 m[4:h-4,4:w-4]=cv2.GC_PR_FGD;m[:4]=m[-3:]=0;m[:,:3]=m[:,-3:]=0
 if key in ['gem_blue','gem_red','feather','potion','medal']:
  # Original quantities occupy lower-right corners. Exclude them from the subject mask.
  m[int(h*.83):,int(w*.82):]=0
 if key=='gem_blue':m[(a[:,:,0]>a[:,:,2]*.9)&(a[:,:,1]>a[:,:,2]*.85)]=0
 cv2.grabCut(a,m,None,np.zeros((1,65)),np.zeros((1,65)),5,cv2.GC_INIT_WITH_MASK)
 alpha=np.isin(m,[1,3]).astype('uint8')*255
 n,labels,stats,_=cv2.connectedComponentsWithStats(alpha)
 if n>1:alpha=(labels==(1+np.argmax(stats[1:,4]))).astype('uint8')*255
 # Fill enclosed shadow holes while preserving external gaps.
 cs,_=cv2.findContours(alpha,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);cv2.drawContours(alpha,cs,-1,255,-1)
 return Image.fromarray(alpha).filter(ImageFilter.GaussianBlur(.3))
for key,item in boxes['surfaces'].items():
 source=Image.open(b/(key+'_source.png')).convert('RGBA');fill=g.crop(item['atlasBox']).resize(source.size)
 if key.startswith('blank_'):
  alpha=gray_alpha(fill) if key!='blank_activate' else isolate_icon(fill,'activate')
  fill.putalpha(alpha);save(key,fill)
 else:
  # Keep all exposed original border RGB; replace the occupied interior with repaired lacquer.
  m=Image.new('L',source.size);d=ImageDraw.Draw(m);w,h=source.size
  d.rectangle((15,15,w-16,h-16),fill=255)
  if key!="objectives":d.rectangle((70,7,w-70,42),fill=255)
  m=m.filter(ImageFilter.GaussianBlur(1.5))
  im=Image.composite(fill,source,m);alpha=Image.new('L',source.size);ImageDraw.Draw(alpha).rounded_rectangle((1,1,w-2,h-2),radius=10,fill=255);im.putalpha(alpha)
  save(key,im);m.save(b/(key+'_repair_mask.png'))
for key,item in boxes['isolated'].items():
 source=Image.open(b/(key+'_source.png')).convert('RGBA')
 if key in ['title','play','activate']:
  alpha=Image.open(art/('blank_'+key+'.png')).getchannel('A')
  # Template isolation follows the same source layout; remove the two neighbouring header strips.
  if key=='title':
   aa=np.array(alpha);aa[:16,:230]=0;aa[:15,400:]=0;alpha=Image.fromarray(aa)
 else:alpha=isolate_icon(source,key)
 outlines={
 'hourglass':[(3,0),(29,0),(32,5),(27,12),(22,19),(25,27),(30,35),(29,42),(4,43),(1,38),(6,28),(13,20),(8,13),(2,6)],
 'potion':[(46,3),(59,3),(66,8),(64,18),(58,24),(60,35),(70,47),(74,63),(67,79),(54,90),(31,93),(16,88),(6,79),(2,62),(5,46),(14,35),(29,29),(38,25),(41,15)],
 'medal':[(45,2),(34,8),(26,7),(29,21),(13,10),(18,29),(6,20),(8,42),(4,46),(17,66),(33,71),(27,84),(34,92),(59,94),(64,84),(60,72),(79,61),(87,50),(87,18),(78,4),(78,27),(65,30),(61,17),(50,22),(54,10)],
 'premium_chest':[(15,33),(21,22),(30,17),(41,10),(70,8),(95,12),(118,20),(132,30),(144,54),(144,94),(129,103),(89,114),(54,104),(16,94)]
 }
 if key in outlines:
  alpha=Image.new('L',source.size);ImageDraw.Draw(alpha).polygon(outlines[key],fill=255)
  alpha=alpha.filter(ImageFilter.GaussianBlur(.35))
 source.putalpha(alpha)
 if key=='title':
  repair=Image.new('L',source.size);ImageDraw.Draw(repair).rectangle((64,316,286,352),fill=255)
  repair=repair.filter(ImageFilter.GaussianBlur(1));original=source.copy()
  source=Image.composite(Image.open(art/'blank_title.png'),source,repair);source.putalpha(alpha)
  repair.save(b/'title_feather_repair_mask.png')
 save(key,source,Image.open(b/(key+'_source.png')).convert('RGBA'))
# Recover original Firebird including left tail omitted by the initial segmentation.
src=Image.open(b/'FirebirdGuidance.png').convert('RGBA');a=np.array(src);pixa=Image.open(b/'Scenario/firebird/booster_medallion_trial01.png').convert('RGBA');alpha=np.array(pixa.getchannel('A'))
alpha[285:,120:]=0 # Countdown frame/text are independent controls, never part of the bird.
tail=Image.new('L',src.size);ImageDraw.Draw(tail).polygon([(12,342),(19,290),(43,210),(75,160),(106,116),(133,95),(127,69),(111,52),(138,27),(183,5),(226,15),(279,68),(295,120),(265,187),(225,239),(153,288),(98,332),(79,342)],fill=255)
rgb=a[:,:,:3].astype(float);warm=(rgb[:,:,0]>80)&(rgb[:,:,0]>rgb[:,:,1]*1.13)&(rgb[:,:,1]>rgb[:,:,2]*1.06)
recovery=(warm&(np.array(tail)>0)).astype('uint8')*255
recovery=np.where((rgb[:,:,0]>120)&(rgb[:,:,1]>rgb[:,:,2]*1.2),recovery,0).astype("uint8")
alpha=np.maximum(alpha,recovery)
alpha[279:288,120:]=0
alpha[337:,:]=0
# Feathered wing glow uses original warm pixels close to the isolated silhouette; no city rectangles.
near=cv2.dilate(alpha,np.ones((13,13),np.uint8))>0
sparks=np.where(warm&near,np.clip((rgb[:,:,0]-rgb[:,:,2])*1.4,0,190),0).astype('uint8');sparks[285:,120:]=0
bird=src.copy();bird.putalpha(Image.fromarray(alpha));save('firebird',bird,src)
glow=src.copy();glow.putalpha(Image.fromarray(sparks));save('firebird_sparks',glow,src)
halo=Image.new('RGBA',src.size,(255,157,32,0));halo.putalpha(Image.fromarray(sparks).filter(ImageFilter.GaussianBlur(10)).point(lambda x:int(x*.34)));save('firebird_halo',halo)
# A rail with no filled progress. Source end caps/outer metal stay separate from a navy empty track.
original=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/R1/Sources/OriginalEventDetailReference.png').convert('RGBA')
rail=original.crop((35,995,906,1027));mask=Image.new('L',rail.size,255);d=ImageDraw.Draw(mask);d.rectangle((19,7,850,24),fill=0);rail.putalpha(mask);save('progress_rail',rail)
for i,x in enumerate([130,295,462,631,805]):
 diamond=original.crop((x-21,988,x+21,1032));da=Image.new('L',diamond.size);ImageDraw.Draw(da).polygon([(21,1),(41,22),(21,43),(1,22)],fill=255);diamond.putalpha(da);save('diamond'+str(i),diamond)
card=original.crop((77,1060,182,1167));ca=Image.new('L',card.size,255);ImageDraw.Draw(ca).rectangle((6,6,98,100),fill=0);card.putalpha(ca);save('reward_card',card)
# Local source diagnostic only, not an implementation page.
for key in ['title','play','firebird','chest0','gem_blue','premium','progress','potion','medal','premium_chest','hourglass']:
 im=Image.open(art/(key+'.png')).convert('RGBA');scale=min(1,880/im.width);im=im.resize((int(im.width*scale),int(im.height*scale)))
 canvas=Image.new('RGB',(max(im.width,360),3*(im.height+24)),(35,40,45));dr=ImageDraw.Draw(canvas)
 for j,(col,label) in enumerate([((0,0,0),'BLACK'),((255,255,255),'WHITE'),((235,0,210),'MAGENTA')]):
  y=j*(im.height+24);dr.text((5,y+3),label,fill='white');canvas.paste(col,(0,y+24,canvas.width,y+24+im.height));canvas.paste(im,(0,y+24),im)
 canvas.save(q/(key+'_alpha.png'))
(b/'ArtAudit.json').write_text(json.dumps(audit,indent=2));print('Saved',len(list(art.glob('*.png'))),'sprites; source RGB retained for isolated subjects/lettering')
