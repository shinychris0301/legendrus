from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent.parent;a=b/'Assets';s=b/'Sources'
def extract(name):
    original=Image.open(s/'Inputs'/f'{name}.png').convert('RGBA')
    mask=Image.open(s/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA');mask.load()
    alpha=mask.getchannel('A').resize(original.size,Image.Resampling.LANCZOS)
    original.putalpha(alpha)
    return original
title=extract('title');alpha=title.getchannel('A');d=ImageDraw.Draw(alpha)
# Source plaque face is intentionally opaque; the extractor mistook dark texture for background.
d.polygon([(64,79),(570,79),(596,112),(582,166),(73,172),(42,136)],fill=255)
d.rectangle((0,0,225,29),fill=0) # unrelated resource-bar ends above the title
title.putalpha(alpha);title.save(a/'chapter_title.png')
nav=extract('navigation');alpha=nav.getchannel('A');d=ImageDraw.Draw(alpha)
for cx in [102,285,471,658,846]:
    d.ellipse((cx-73,26,cx+73,174),fill=255)
    d.rectangle((cx-58,174,cx+58,207),fill=255)
d.rectangle((365,0,569,6),fill=0) # map-button ornament from the row above
nav.putalpha(alpha)
for name,l,r in [('home',0,192),('map',192,377),('tasks',377,565),('guild',565,751),('shop',751,941)]:nav.crop((l,0,r,236)).save(a/f'nav_{name}.png')
for name in ['back','map_button']:
    im=extract(name);alpha=im.getchannel('A');d=ImageDraw.Draw(alpha)
    if name=='back':d.rectangle((39,33,168,65),fill=255)
    else:d.rectangle((67,36,425,93),fill=255)
    im.putalpha(alpha);im.save(a/(name+'_source.png'))
card=extract('card');alpha=card.getchannel('A');ar=np.asarray(alpha).copy()
# Preserve all source border/corner ornament RGB. Open an inner window for the separate painting/surface.
ar[36:222,39:719]=0;ar[19:240,60:702]=0
# The panel alpha still contains scenery/lacquer strips around this window.
# Isolate original metal and jewels within the frame band, retaining a narrow bevel.
rgb=np.array(card.convert('RGB'));hsv=cv2.cvtColor(rgb,cv2.COLOR_RGB2HSV)
metal=((hsv[:,:,0]>=9)&(hsv[:,:,0]<=42)&(hsv[:,:,1]>62)&(hsv[:,:,2]>48))
jewel=((hsv[:,:,0]>=88)&(hsv[:,:,0]<=130)&(hsv[:,:,1]>80)&(hsv[:,:,2]>68))
seed=((metal|jewel)&(ar>30)).astype(np.uint8)*255
count,labels,stats,_=cv2.connectedComponentsWithStats(seed,8)
clean=np.zeros(seed.shape,np.uint8)
for i in range(1,count):
    if stats[i,cv2.CC_STAT_AREA]>=5:clean[labels==i]=255
support=cv2.dilate(clean,np.ones((3,3),np.uint8))
support=cv2.GaussianBlur(support,(3,3),.35)
final_alpha=np.minimum(ar,support)
# Keep the intentionally dark metal channel between the parallel gold rails.
# It belongs to the frame, unlike the adjacent painted scenery/lacquer strips.
for x0,y0,x1,y1 in [(65,4,700,18),(65,240,700,251),(7,58,23,201),(736,58,750,201)]:
    final_alpha[y0:y1,x0:x1]=ar[y0:y1,x0:x1]
final_alpha[80:188,24:40]=0
final_alpha[19:24,80:353]=0
final_alpha[19:24,405:682]=0
card.putalpha(Image.fromarray(final_alpha));card.save(a/'card_frame.png')
(s/'FrameSeparation.json').write_text(json.dumps({'method':'Scenario panel alpha plus bounded source metal/jewel mask and one-pixel bevel support','removedAlphaPixels':int(((ar>0)&(final_alpha==0)).sum()),'visibleRgbUnchanged':True,'centerTransparent':bool((final_alpha[50:210,70:690]==0).all()),'limitation':'Very dark fine ornament edges use approximate alpha; no RGB regeneration.'},indent=2))
src=Image.open(s/'ChapterSelectReference.png').convert('RGBA')
# Empty navy face constructed exclusively from an unlettered patch of this card's own lacquer.
# Patch reflection makes a continuous texture without a flat inpaint rectangle or baked labels.
tile=src.crop((790,435,812,480)).convert('RGB')
strip=Image.new('RGB',(44,45));strip.paste(tile,(0,0));strip.paste(tile.transpose(Image.Transpose.FLIP_LEFT_RIGHT),(22,0))
surface=Image.new('RGB',(758,258))
for x in range(0,758,44):
    for y in range(0,258,45):surface.paste(strip if y==0 else strip.transpose(Image.Transpose.FLIP_TOP_BOTTOM),(x,y))
surface=surface.filter(ImageFilter.GaussianBlur(.8));surface.save(a/'card_surface.png')
# Original star/status plaque, with only the example digits removed for live progress.
plate=src.crop((571,476,754,533));raw=np.array(plate.convert('RGB'));mask=np.zeros(raw.shape[:2],np.uint8);mask[10:44,23:112]=255
raw=cv2.inpaint(raw,mask,5,cv2.INPAINT_TELEA);plate=Image.fromarray(raw).convert('RGBA');al=Image.new('L',plate.size);ImageDraw.Draw(al).polygon([(12,1),(167,1),(182,12),(182,44),(169,56),(12,56),(0,44),(0,13)],fill=255);plate.putalpha(al);plate.save(a/'progress_plate.png')
# Reference lock, isolated from existing chapter/character source art rather than replacing it with a CSS icon.
lock=src.crop((417,967,526,1086));rgb=np.array(lock.convert('RGB'));hsv=cv2.cvtColor(rgb,cv2.COLOR_RGB2HSV)
mask=((hsv[:,:,0]<42)&(hsv[:,:,1]>55)&(hsv[:,:,2]>48)).astype(np.uint8)*255
contours,_=cv2.findContours(mask,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);largest=max(contours,key=cv2.contourArea)
filled=np.zeros(mask.shape,np.uint8);cv2.drawContours(filled,[largest],-1,255,cv2.FILLED)
filled=cv2.morphologyEx(filled,cv2.MORPH_CLOSE,np.ones((3,3),np.uint8))
lock.putalpha(Image.fromarray(filled));lock.save(a/'lock_source.png')
env=s/'ScenarioEnvironment/booster_medallion_trial01.png'
if env.exists():
    im=Image.open(env);im.load();clean=im.convert('RGB').resize(src.size,Image.Resampling.LANCZOS)
    exclusions=Image.new('L',src.size);d=ImageDraw.Draw(exclusions)
    for box in [(0,0,941,145),(0,0,183,242),(145,95,805,311),(83,304,861,1323),(0,1327,214,1432),(214,1310,721,1450),(0,1427,941,1672)]:d.rectangle(box,fill=255)
    exclusions=exclusions.filter(ImageFilter.MaxFilter(21)).filter(ImageFilter.GaussianBlur(6))
    Image.composite(clean,src.convert('RGB'),exclusions).save(a/'reference_environment.png')
    exclusions.save(s/'EnvironmentUiExclusionMask.png')
for color,label in [('#000000','Black'),('#ffffff','White'),('#ff00bb','Magenta')]:
    sheet=Image.new('RGB',(1000,1100),color)
    for name,x,y,w,h in [('chapter_title.png',120,0,644,205),('card_frame.png',80,215,758,258),('nav_home.png',0,495,192,236),('nav_map.png',192,495,185,236),('nav_tasks.png',377,495,188,236),('nav_guild.png',565,495,186,236),('nav_shop.png',751,495,190,236),('back_source.png',0,780,208,91),('map_button_source.png',220,755,495,128),('lock_source.png',30,930,109,119),('progress_plate.png',240,950,183,57)]:
        im=Image.open(a/name).convert('RGBA');im.thumbnail((w,h));sheet.paste(im,(x,y),im)
    sheet.save(b/f'QA/ReferenceAlpha_{label}.png')
(s/'ReferenceRepairs.json').write_text(json.dumps({'source':str(s/'ChapterSelectReference.png'),'sourceSha256':hashlib.sha256((s/'ChapterSelectReference.png').read_bytes()).hexdigest(),'operations':['Scenario masks on unchanged original RGB for title, navigation, Back, map button and frame','restore intentional opaque title/nav/button faces','remove unrelated neighboring UI fragments','open frame inner window for separate approved paintings','reconstruct empty navy card face from its own unlettered texture donor','remove sample progress numbers locally; retain original star/plaque','source-color lock segmentation'], 'exactness':'Visible original sprites retain source RGB; repaired empty surfaces, regenerated environment and live chapter text are not pixel-identical to reference.'},indent=2))

if (s/'Scenario/continue/booster_medallion_trial01.png').exists():
    im=extract('continue');al=im.getchannel('A');ImageDraw.Draw(al).rectangle((38,22,220,53),fill=255);keep=Image.new('L',im.size);ImageDraw.Draw(keep).polygon([(0,31),(20,13),(47,4),(125,0),(147,0),(214,4),(244,16),(258,34),(242,53),(219,61),(142,61),(131,68),(119,62),(41,62),(18,52)],fill=255)
    al=Image.fromarray(np.minimum(np.array(al),np.array(keep)));im.putalpha(al);im.save(a/'continue_source.png')
