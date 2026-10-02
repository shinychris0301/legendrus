from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned');src=Image.open(b/'OriginalReference.png').convert('RGBA');audit={}
def save(n,im,note):
 p=out/(n+'.png');im.save(p);audit[n]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'size':im.size,'method':note}
original=src.crop((32,104,917,1449));panel=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/TutorialAligned/panel.png').convert('RGBA');alpha=panel.getchannel('A')
repair=Image.open(b/'Scenario/panel/booster_medallion_trial01.png').convert('RGBA').resize(panel.size,Image.Resampling.LANCZOS)
# Keep original crest, complete header and repaired close-occluded right corner from the earlier approved extraction.
base=panel.copy();base.paste(original.crop((42,85,760,274)),(42,85));base.paste(original.crop((760,180,790,260)),(760,180));base.putalpha(alpha)
m=Image.new('L',base.size);d=ImageDraw.Draw(m);d.rectangle((42,278,840,1276),fill=255);d.rectangle((140,1250,744,1320),fill=255);m=m.filter(ImageFilter.GaussianBlur(2));m.save(b/'PanelBodyRepairMask.png')
ru=Image.composite(repair,base,m);ru.putalpha(alpha);save('panel_ru_r2',ru,'Original Russian title and outer carving, earlier Scenario alpha/corner repair; bounded new Scenario village/platform/lacquer interior. No commercial figures baked in.')
m2=Image.new('L',base.size);d=ImageDraw.Draw(m2);d.rectangle((244,88,645,166),fill=255);d.rectangle((112,168,785,256),fill=255);m2=m2.filter(ImageFilter.GaussianBlur(1));m2.save(b/'PanelEnglishRepairMask.png');en=Image.composite(repair,ru,m2);en.putalpha(alpha);save('panel_blank_r2',en,'Same original frame with bounded blank navy title for live English; Russian variant unchanged.')
# Use original RGB with the returned Scenario alpha. Restore the final few ribbon-tip pixels below the request crop.
hero=src.crop((71,485,850,931));raw=Image.open(b/'Scenario/hero/booster_medallion_trial01.png').convert('RGBA');a=Image.new('L',hero.size);a.paste(raw.getchannel('A').resize((779,438),Image.Resampling.LANCZOS),(0,0));aa=np.array(a);rgb=np.asarray(hero).astype(float);r,g,bl=rgb[:,:,:3].transpose(2,0,1)
extra=(r>40)&(r>g*1.7)&(r>bl*1.5);bounds=np.zeros(aa.shape,bool);bounds[438:,680:735]=True;aa[bounds&extra]=255
# Remove isolated subpixel residue below the ribbon while retaining the connected subject.
n,labels,stats,_=cv2.connectedComponentsWithStats((aa>0).astype('uint8'),8)
for i in range(1,n):
 if stats[i,cv2.CC_STAT_AREA]<35:aa[labels==i]=0
hero.putalpha(Image.fromarray(aa));save('hero_source_r2',hero,'Fresh Scenario source-alpha extraction; unchanged original RGB, locally restored short source ribbon tip. No chest regeneration.')
# Separate faint source golden sparks/rays outside the object; exclude attached discount/savings UI.
glow=src.crop((71,485,850,931));ga=(np.clip((r-105)/140,0,1)*np.clip((g-65)/115,0,1)*np.clip((r-bl-50)/100,0,1)*255).astype('uint8');exclusion=cv2.dilate(aa,np.ones((3,3),'uint8'));ga[exclusion>10]=0;ga[:140,:180]=0;ga[:125,575:]=0;ga[425:]=0;glow.putalpha(Image.fromarray(ga));save('hero_glow_r2',glow,'Original warm highlight pixels outside the subject, masked to remove discount/savings/timer UI; transparent decorative glow.')
# Reuse original slim reward-card perimeter; clear the sample icon/count/caption with matching approved navy material.
frame=src.crop((78,1003,204,1190));navy=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/ShopAligned/empty_panel.png').crop((50,40,650,350)).resize(frame.size,Image.Resampling.LANCZOS);fm=Image.new('L',frame.size);ImageDraw.Draw(fm).rectangle((7,8,118,179),fill=255);frame=Image.composite(navy,frame,fm);mask=Image.new('L',frame.size);ImageDraw.Draw(mask).rounded_rectangle((1,1,124,185),radius=6,fill=255);frame.putalpha(mask);save('reward_frame_r2',frame,'Original slim gold card perimeter, opaque approved blank navy interior for live preview category and unspecified amount.')
(b/'AssetProvenance.json').write_text(json.dumps(audit,indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 im=Image.new('RGB',(1200,1450),color)
 for n,xy,size in [('panel_ru_r2',(0,0),(600,1000)),('panel_blank_r2',(600,0),(600,1000)),('hero_source_r2',(10,950),(760,440)),('hero_glow_r2',(790,980),(400,260)),('reward_frame_r2',(810,1250),(126,187))]:
  a=Image.open(out/(n+'.png'));a.thumbnail(size);im.paste(a,xy,a)
 im.save(b.parent/'QA'/('Alpha'+label+'.png'))
print('Prepared',len(audit),'assets')
