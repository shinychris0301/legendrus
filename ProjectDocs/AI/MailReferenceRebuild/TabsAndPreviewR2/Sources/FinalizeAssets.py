from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;res=Path('Assets/LegendsOfRus/Resources/ProductionUI');out=res/'MailAligned';sheet=Image.open(b/'Sheet.png').convert('RGBA');result=Image.open(b/'Scenario/tabs_preview/booster_medallion_trial01.png').convert('RGBA');bounds=json.loads((b/'Bounds.json').read_text());src=Image.open(b/'Inbox.png').convert('RGB');written={}
def polygon(size,pts):
 m=Image.new('L',size);ImageDraw.Draw(m).polygon(pts,fill=255);return m

def save(name,im):
 im.save(out/(name+'.png'));written[name]={'size':im.size,'sha256':hashlib.sha256((out/(name+'.png')).read_bytes()).hexdigest()};return im
for name in ['tabs_joined_r2','preview_portrait_r2']:
 box=bounds[name]['sheet'];rgb=sheet.crop(box);a=result.crop(box).getchannel('A');ar=np.array(a)
 # Remove disconnected specks. Source alpha is refined within documented support only.
 n,labels,stats,_=cv2.connectedComponentsWithStats((ar>20).astype('uint8'),8)
 allowed=np.zeros_like(ar)
 for i in range(1,n):
  if stats[i,cv2.CC_STAT_AREA]>1000:allowed[labels==i]=255
 ar=np.minimum(ar,cv2.dilate(allowed,np.ones((3,3),np.uint8)))
 if name=='preview_portrait_r2':
  # Keep the opaque portrait/plaque faces intact; Scenario sometimes holes dark pixels.
  inner=polygon(rgb.size,[(195,27),(290,70),(363,151),(378,259),(346,326),(380,356),(378,434),(354,458),(39,457),(19,435),(20,355),(57,327),(24,274),(25,183),(80,84)])
  ar=np.maximum(ar,np.array(inner))
  # Exterior parchment cannot be present outside the gold contour. Use traced support.
  support=polygon(rgb.size,[(192,7),(210,18),(226,29),(281,49),(330,82),(363,132),(381,190),(393,200),(382,218),(381,270),(362,306),(347,325),(367,329),(381,347),(394,354),(394,433),(384,443),(364,463),(225,466),(206,476),(190,467),(37,467),(20,455),(5,439),(5,358),(17,349),(29,332),(47,329),(27,297),(10,248),(12,222),(0,205),(12,190),(26,133),(52,88),(105,48),(168,26)])
  support=Image.new('L',rgb.size);d=ImageDraw.Draw(support)
  d.ellipse((14,28,385,384),fill=255)
  d.polygon([(170,29),(192,7),(211,21),(229,32)],fill=255)
  d.polygon([(0,205),(16,189),(32,206),(16,222)],fill=255)
  d.polygon([(370,203),(392,201),(382,218),(368,223)],fill=255)
  d.polygon([(28,333),(47,326),(346,326),(365,333),(381,350),(393,358),(393,430),(380,443),(361,462),(218,464),(205,474),(190,465),(37,465),(22,453),(7,437),(7,360),(20,347)],fill=255)
  ar=np.minimum(ar,np.array(support))
 else:
  # Reestablish intentionally opaque full-width panel interior; trim is allowed around it.
  opaque=polygon(rgb.size,[(110,27),(1690,27),(1690,150),(1746,174),(16,174),(96,150),(96,49)])
  color=np.array(rgb)[:,:,:3].astype(float);r,g,bl=color[:,:,0],color[:,:,1],color[:,:,2]
  gold=((r>bl*1.55)&(g>bl*1.15)&(r>68)&(g>40)).astype('uint8')*255
  gold=cv2.GaussianBlur(cv2.dilate(gold,np.ones((3,3),np.uint8)),(3,3),.55)
  # The exterior cutout is limited to metallic scrollwork, not rectangular scenery.
  support=np.maximum(np.array(opaque),gold)
  ar=np.minimum(ar,support);ar[np.array(opaque)>0]=255
  ar[:10]=0
  # Traced exterior excludes autumn foliage beyond the two scrolls.
  left=polygon((145,178),[(120,10),(104,26),(90,40),(78,44),(77,58),(69,72),(58,60),(51,69),(47,81),(31,88),(20,99),(19,123),(27,140),(12,148),(0,148),(0,177),(145,177),(145,10)])
  ar[:,:145]=np.minimum(ar[:,:145],np.array(left));ar[:,-145:]=np.minimum(ar[:,-145:],np.array(left.transpose(Image.Transpose.FLIP_LEFT_RIGHT)))
  n,labels,stats,_=cv2.connectedComponentsWithStats((ar>32).astype('uint8'),8)
  largest=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);keep=(labels==largest).astype('uint8')*255
  ar=np.minimum(ar,cv2.dilate(keep,np.ones((3,3),np.uint8)))
 if name=='tabs_joined_r2':
  c=np.array(rgb);region=c[38:154,98:613,:3];blueOnly=(region[:,:,2]>region[:,:,0]*1.3)&(region[:,:,2]>region[:,:,1]*1.05)
  adjusted=region.copy();adjusted[:,:,1]=(region[:,:,1]*.56).astype('uint8');adjusted[:,:,2]=(region[:,:,2]*.45).astype('uint8');region[blueOnly]=adjusted[blueOnly];c[38:154,98:613,:3]=region;rgb=Image.fromarray(c)
 rgb.putalpha(Image.fromarray(ar));save(name,rgb)
portrait=Image.open(out/'preview_portrait_r2.png');save('preview_nameplate_r2',portrait.crop((0,322,404,478)))
# Shared clean face material, separate from original continuous trim.
navy=Image.open(res/'TeamRankingAligned/table_panel_r4.png').convert('RGBA');navy=navy.crop((navy.width//3,navy.height//3,navy.width*2//3,navy.height*2//3))
blue=Image.open(res/'IndividualRankingAligned/tab_active.png').convert('RGBA').crop((90,48,210,148))
for name,donor in [('tab_face_r2',navy),('tab_face_active_r2',blue)]:
 im=donor.resize((488,104),Image.Resampling.LANCZOS);im.putalpha(polygon(im.size,[(20,0),(468,0),(487,19),(487,85),(468,103),(20,103),(0,85),(0,19)]));save(name,im)
# Ink ornaments are flat dark brown on parchment; extract visible stroke contrast, excluding paper.
for name,box in [('preview_divider_r2',(278,1315,414,1334)),('preview_rule_r2',(291,1098,843,1106))]:
 im=src.crop(box).convert('RGBA');a=np.array(im);lum=a[:,:,:3].astype(float).mean(2)
 alpha=np.clip((143-lum)*4.2,0,255).astype('uint8')
 alpha=cv2.medianBlur(alpha,3) if name=='preview_divider_r2' else alpha
 im.putalpha(Image.fromarray(alpha));im=im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS);save(name,im)
# Existing preview paper retained except a residual source reward-slot edge on its bottom rail.
paper=Image.open(out/'preview_frame.png').convert('RGBA');repair=Image.new('L',paper.size);ImageDraw.Draw(repair).rectangle((730,1247,1070,1279),fill=255);repair=repair.filter(ImageFilter.GaussianBlur(3))
donor=paper.crop((1140,1240,1500,1286));patch=paper.copy();patch.paste(donor,(720,1240));clean=Image.composite(patch,paper,repair);clean.putalpha(paper.getchannel('A'));save('preview_paper_r2',clean);repair.save(b/'PaperRepairMask.png')
# Diagnostic alpha compositions, not production previews.
thumbs=[]
for name in written:
 im=Image.open(out/(name+'.png'));im.thumbnail((700,255));thumbs.append((name,im))
height=sum(im.height+34 for _,im in thumbs);check=Image.new('RGB',(2160,height),(28,35,45));draw=ImageDraw.Draw(check);y=0
for name,im in thumbs:
 for i,c in enumerate(['black','white','#ef00bb']):
  draw.text((i*720+8,y+4),name,fill='white');bg=Image.new('RGBA',(704,im.height),c);bg.alpha_composite(im,(0,0));check.paste(bg.convert('RGB'),(i*720+8,y+25))
 y+=im.height+34
check.save(b.parent/'QA/AlphaBackgrounds.png')
(b/'AssetProvenance.json').write_text(json.dumps({'operation':'Source RGB under Scenario alpha, bounded support repairs; no generated RGB','source':{'file':'Inbox.png','sha256':hashlib.sha256((b/'Inbox.png').read_bytes()).hexdigest()},'bounds':bounds,'artifacts':written,'finalRefinements':['Scenario retained staging rectangles; local traced silhouettes remove exterior parchment/scenery without another request.','Disconnected exterior foliage removed; first-tab blue outline neutralized for inactive state.','Selected texture: approved IndividualRankingAligned/tab_active.png crop(90,48,210,148).','Existing parchment residual reward edge repaired under PaperRepairMask.png using neighboring rail; alpha unchanged.'],'limitations':'Obscured tab badge rails mirrored; captions hidden surfaces use existing lacquer. Portrait contour traced conservatively. Font identity unverified. Upsampling adds no source detail.'},indent=2))
print('Saved',len(written),'new sprites')
