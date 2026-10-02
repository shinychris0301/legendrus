from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');out.mkdir(parents=True,exist_ok=True)
records=[]
for f in sorted((b/'Inputs').glob('*.png')):
 n=f.stem;rgb=Image.open(f).convert('RGB');raw=Image.open(b/'Scenario'/n/'booster_medallion_trial01.png').convert('RGBA');raw=raw.resize(rgb.size,Image.Resampling.LANCZOS);alpha=raw.getchannel('A');w,h=rgb.size;mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
 if n.endswith('_frame'):
  inset=22;d.rectangle((inset,inset,w-inset,h-inset),fill=255)
  if n=='card_frame':d.rectangle((48,12,w-48,38),fill=255)
  mask=mask.filter(ImageFilter.GaussianBlur(2))
  # Remove residual donor engraving from the cleared interior, retaining the source border.
  a=np.array(rgb).astype(float);base=np.median(a[h//3:h*2//3,w//3:w*2//3].reshape(-1,3),0);rng=np.random.default_rng(36);grain=rng.normal(0,1.2,(h,w,1));fill=Image.fromarray(np.uint8(np.clip(base+grain,0,255)));rgb=Image.composite(fill,rgb,mask)
  alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(mask)))
 if n=='title':
  d.rounded_rectangle((76,46,w-73,h-23),radius=14,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(2));fill=Image.new('RGB',(w,h),(9,19,24));rgb=Image.composite(fill,rgb,mask)
  contour=Image.new('L',(w,h));ImageDraw.Draw(contour).polygon([(7,72),(30,59),(52,58),(74,27),(192,28),(200,20),(253,20),(287,0),(316,0),(352,22),(401,25),(513,27),(542,58),(580,66),(584,98),(553,103),(527,127),(335,130),(310,144),(275,144),(255,132),(72,133),(45,106),(8,101)],fill=255)
  alpha=contour.filter(ImageFilter.GaussianBlur(.55))
 elif n in ['upgrade','select'] or n.startswith('portrait_') and n!='portrait_frame':
  # Largest foreground component only: remove detached neighboring frame fragments.
  a=np.array(alpha);count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(a>30),8);keep=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);support=cv2.dilate(np.uint8(labels==keep),np.ones((3,3),np.uint8));alpha=Image.fromarray(a*support)
 rgb=rgb.convert('RGBA');rgb.putalpha(alpha);rgb.save(out/f'{n}.png');mask.save(b/'RepairMasks'/f'{n}_final.png');records.append({'asset':str(out/f'{n}.png'),'sha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'sourceAlpha':'Scenario Pixa background removal; no generated RGB','repairMask':f'RepairMasks/{n}_final.png'})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'revision':1,'sourceAssessment':'Sources/Assessment.json','assets':records,'reuse':['approved Home Ivan/Firebird','IndividualRanking clean background and embedded fonts','Shop native saved-state header','Home footer icons','existing booster sprites'],'limits':'Main Ivan pose is reused, source preview portraits are small. Title exterior alpha bounded locally; obscured panel interiors rebuilt locally. Hero ownership/stats/upgrades unavailable.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 fs=sorted(out.glob('*.png'));sheet=Image.new('RGB',(1000,((len(fs)+3)//4)*275),color);d=ImageDraw.Draw(sheet)
 for i,f in enumerate(fs):
  a=Image.open(f);a.thumbnail((240,245));x=i%4*250;y=i//4*275;sheet.paste(a,(x,y+25),a);d.text((x+3,y+4),f.stem,fill=(130,130,130))
 sheet.save(b.parent/'QA'/f'Alpha_{name}.jpg')
