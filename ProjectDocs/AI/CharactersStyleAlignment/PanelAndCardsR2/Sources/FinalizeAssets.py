from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');records=[]
for f in sorted((b/'Inputs').glob('*.png')):
 n=f.stem
 if n.endswith('_retry'):continue
 rgb=Image.open(f).convert('RGBA');folder=b/'Scenario'/n
 if n=='star_r2' and not (folder/'booster_medallion_trial01.png').exists():folder=b/'Scenario/star_r2_retry'
 raw=Image.open(folder/'booster_medallion_trial01.png').convert('RGBA').resize(rgb.size,Image.Resampling.LANCZOS);a=np.array(raw.getchannel('A'))
 # Retain the largest subject; preserve intentionally opaque medal and plaque faces.
 count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(a>35),8);keep=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);a=a*cv2.dilate(np.uint8(labels==keep),np.ones((3,3),np.uint8));alpha=Image.fromarray(a)
 finalMask=Image.new('L',rgb.size)
 if n.startswith('ability_'):
  assessment=json.loads((b/'Assessment.json').read_text());region=next(x['bounds'] for x in assessment['assets'] if x['name']==n);rgb=Image.open(assessment['source']).convert('RGBA').crop(tuple(region))
  index=int(n.split('_')[1]);cx=[78,77,72,70][index];cy=74
  ImageDraw.Draw(finalMask).ellipse((cx-9,cy-10,cx+9,cy+10),fill=255);finalMask=finalMask.filter(ImageFilter.GaussianBlur(.5));rgb=Image.composite(Image.new('RGBA',rgb.size,(15,21,22,255)),rgb,finalMask)
  face=Image.new('L',rgb.size);d=ImageDraw.Draw(face);d.ellipse((7,7,81,81),fill=255);d.ellipse((cx-10,cy-10,cx+10,cy+10),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(face)))
  # Bound the number medallion to its actual source circle instead of a shared oversized cap.
  contour=Image.new('L',rgb.size);d=ImageDraw.Draw(contour);d.ellipse((1,1,87,86),fill=255);d.ellipse((cx-15,cy-15,cx+15,cy+15),fill=255);alpha=Image.fromarray(np.minimum(np.array(alpha),np.array(contour.filter(ImageFilter.GaussianBlur(.5)))))
 if n=='rarity_r2':
  ImageDraw.Draw(finalMask).rectangle((52,11,279,36),fill=255);finalMask=finalMask.filter(ImageFilter.GaussianBlur(1));w,h=rgb.size;yy,xx=np.mgrid[:h,:w];shade=1+.13*np.cos(xx/34)+.09*np.sin(yy/3);arr=np.uint8(np.clip(shade[:,:,None]*np.array([8,45,73]),0,255));rgb=Image.composite(Image.fromarray(arr).convert('RGBA'),rgb,finalMask)
  alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(finalMask)))
 if n.startswith('badge_'):
  polygons={'badge_gem_r2':[(27,7),(39,12),(46,23),(43,37),(26,47),(15,40),(8,28),(12,14)],'badge_shield_r2':[(14,7),(27,11),(35,22),(31,34),(15,45),(2,34),(0,21),(4,11)],'badge_sword_r2':[(40,9),(42,20),(35,34),(19,47),(8,43),(8,33),(13,17),(26,11)],'badge_skull_r2':[(23,3),(37,8),(44,22),(39,36),(27,43),(13,38),(7,24),(11,10)]}
  contour=Image.new('L',rgb.size);ImageDraw.Draw(contour).polygon(polygons[n],fill=255);alpha=contour.filter(ImageFilter.GaussianBlur(.45))
 finalMask.save(b/'RepairMasks'/f'{n}_final.png')
 rgb.putalpha(alpha);rgb.save(out/f'{n}.png');records.append({'asset':str(out/f'{n}.png'),'source':'Scenario alpha + immutable input RGB','jobFolder':str(folder),'sha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest()})
# Dedicated frame variants keep existing R1 PNGs and GUIDs untouched.
frame=Image.open(out/'card_frame.png').convert('RGBA');w,h=frame.size;cut=Image.new('L',frame.size);d=ImageDraw.Draw(cut);d.rounded_rectangle((17,17,w-17,h-17),radius=22,fill=255);d.rectangle((47,10,w-47,38),fill=255);a=np.array(frame.getchannel('A'));a=np.minimum(a,255-np.array(cut.filter(ImageFilter.GaussianBlur(.5))));frame.putalpha(Image.fromarray(a));frame.save(out/'card_border_r2.png');cut.save(b/'RepairMasks/card_border_r2.png')
# Soft selected gold perimeter, separate from the clickable native card.
alpha=frame.getchannel('A');glow=alpha.filter(ImageFilter.MaxFilter(3)).filter(ImageFilter.GaussianBlur(2)).point(lambda x:int(x*.35));selection=Image.new('RGBA',frame.size,(255,186,47,0));selection.putalpha(glow);selection.save(out/'card_selection_r2.png')
source=Image.open(b.parent.parent/'R1/Sources/OriginalReference.png').convert('RGB');panel=source.crop((443,309,929,843)).convert('RGBA');w,h=panel.size;donor=np.array(source.crop((891,466,909,590))).astype(float);base=np.median(donor.reshape(-1,3),axis=0);grain=donor-cv2.GaussianBlur(donor,(0,0),3);tile=np.concatenate([grain,grain[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);tile=np.tile(tile,((h+tile.shape[0]-1)//tile.shape[0],(w+tile.shape[1]-1)//tile.shape[1],1))[:h,:w];field=Image.fromarray(np.uint8(np.clip(base+tile*.85,0,255))).convert('RGBA');mask=Image.new('L',(w,h));ImageDraw.Draw(mask).rounded_rectangle((20,22,w-20,h-22),radius=12,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(2));panel=Image.composite(field,panel,mask);panel.putalpha(Image.open(out/'detail_frame.png').getchannel('A'));panel.save(out/'detail_frame_r2.png');mask.save(b/'RepairMasks/detail_frame_r2.png')
fade=np.zeros((128,2,4),dtype=np.uint8);fade[:,:,:3]=[9,15,18];fade[:,:,3]=np.uint8(np.clip(np.linspace(0,1,128)**.65*255,0,255))[:,None];Image.fromarray(fade).save(out/'portrait_fade_r2.png')
track=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/progress_frame.png').convert('RGBA');track.crop((39,249,2134,459)).save(out/'progress_track_r2.png')
for n in ['card_border_r2','card_selection_r2','detail_frame_r2','portrait_fade_r2','progress_track_r2']:
 records.append({'asset':str(out/f'{n}.png'),'source':'Local derivative of R1/source frame, or explicit alpha utility; no generation','sha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest()})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'revision':2,'sourceAssessment':'Sources/Assessment.json','assets':records,'reusedUnchanged':['R1 portraits and title/control assets','Profile stat icons','ChapterComplete gem','Home empty progress frame','Gameplay clean environment inside collection cards only','current Characters main hero/header/footer'],'limits':'Source concept rarity/stars/ability illustrations are previews; no real hero stats or progression.'},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 fs=sorted(out.glob('*_r2.png'));sheet=Image.new('RGB',(1000,((len(fs)+3)//4)*275),color);d=ImageDraw.Draw(sheet)
 for i,f in enumerate(fs):
  a=Image.open(f);a.thumbnail((240,245));x=i%4*250;y=i//4*275;sheet.paste(a,(x,y+25),a);d.text((x+3,y+4),f.stem,fill=(130,130,130))
 sheet.save(b.parent/'QA'/f'Alpha_{name}.jpg')
