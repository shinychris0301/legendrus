from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;dest=Path('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned');sheet=Image.open(b/'Sheet.png').convert('RGB');alphaSheet=Image.open(b/'Scenario/tabs/booster_medallion_trial01.png').convert('RGBA').getchannel('A');ref=Image.open(b.parent.parent/'R1/Sources/Reference.png').convert('RGB');art=[];records=[]
for entry in json.loads((b/'Sheet.json').read_text()):
 i=entry['index'];rgb=sheet.crop(entry['sheetBox']);w,h=rgb.size;a=np.array(alphaSheet.crop(entry['sheetBox']));a[20:h-12,20:w-20]=255
 # Keep the original opaque panel and its top finial; clear outer sheet/neighbor margins.
 support=Image.new('L',(w,h));d=ImageDraw.Draw(support)
 if i==0:points=[(15,8),(107,8),(116,0),(124,8),(217,8),(230,22),(230,56),(218,71),(15,71),(0,56),(0,23)]
 else:points=[(12,0),(w/2-13,0),(w/2,13),(w/2,h/2-13),(w/2-13,h/2),(12,h/2),(0,h/2-12),(0,12)]
 d.polygon([(x*2,y*2) for x,y in points],fill=255);support=support.filter(ImageFilter.GaussianBlur(.5));a=np.minimum(a,np.array(support));alpha=Image.fromarray(a)
 face=Image.new('L',(w,h));ImageDraw.Draw(face).rectangle((32,30,w-32,h-23),fill=255);face=face.filter(ImageFilter.GaussianBlur(1))
 original=np.array(rgb).astype(float);gold=(original[:,:,0]>85)&(original[:,:,1]>55)&(original[:,:,0]>original[:,:,2]*1.25)
 for active in (False,True):
  state='active' if active else 'normal';actualState=i==0
  changed=original.copy()
  if active!=actualState:
   if active:
    lum=original.mean(2);changed[:,:,0]=lum*.20;changed[:,:,1]=lum*.95+19;changed[:,:,2]=lum*1.65+32
   else:
    lum=original.mean(2);changed[:,:,0]=lum*.32;changed[:,:,1]=lum*.46;changed[:,:,2]=lum*.55
   changed=np.clip(changed,0,255).astype('uint8');paint=np.array(face).astype(float)/255;paint[gold]=0
   changed=(changed*paint[:,:,None]+original*(1-paint[:,:,None])).round().astype('uint8')
  else:changed=original.astype('uint8')
  for lang in ('ru','en'):
   result=Image.fromarray(changed)
   if lang=='en':
    donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/'+('tab_active' if active else 'tab')+'.png').convert('RGB');donor=donor.crop((int(donor.width*.13),int(donor.height*.25),int(donor.width*.87),int(donor.height*.75))).resize((w,h),Image.Resampling.LANCZOS)
    result=Image.composite(donor,result,face)
   result=result.convert('RGBA');result.putalpha(alpha);name=f'tab_{i}_{state}_{lang}_r3';result.save(dest/(name+'.png'));art.append((name,result))
   if lang=='ru':records.append({'name':name,'sourceBox':entry['sourceBox'],'originalState':active==actualState,'goldStrokeRGBDifferences':int(np.any(np.array(result)[:,:,:3]!=original,axis=2)[gold].sum())})
 alpha.save(b/f'Tab{i}Alpha.png');face.save(b/f'Tab{i}FaceMask.png')
# Exact Russian source RGB and lettering under the already approved Scenario-derived alpha.
old=Image.open(dest/'details.png').convert('RGBA');original=ref.crop((611,1402,901,1484)).resize(old.size,Image.Resampling.LANCZOS);ru=original.convert('RGBA');ru.putalpha(old.getchannel('A'));ru.save(dest/'details_ru_r3.png');art.append(('details_ru_r3',ru))
# Repair the English face without the previous broad text-removal stripe.
donor=np.array(ref.crop((644,1422,660,1443)));donor=np.concatenate([donor,donor[:,::-1]],1);donor=np.concatenate([donor,donor[::-1]],0);donor=Image.fromarray(np.tile(donor,(3,10,1))[:82,:290]).resize(old.size,Image.Resampling.LANCZOS)
mask=Image.new('L',old.size);ImageDraw.Draw(mask).rectangle((90,54,774,192),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(4));en=Image.composite(donor,original,mask).convert('RGBA');en.putalpha(old.getchannel('A'));en.save(dest/'details_blank_r3.png');art.append(('details_blank_r3',en));mask.save(b/'DetailsBlankRepairMask.png')
for label,color in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(230,0,150))]:
 canvas=Image.new('RGB',(1050,2080),color)
 for j,(name,im) in enumerate(art):
  xy=((j%2)*510+15,(j//2)*185+15) if j<16 else (80,1510+(j-16)*265)
  canvas.paste(im,xy,im)
 canvas.save(b.parent/'QA'/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps({'scenarioCalls':1,'scenarioCU':3,'tabStates':records,'detailsRussian':'Original source RGB uniformly upsampled3x under unchanged accepted details.png alpha','detailsEnglish':'Bounded native lettering-footprint donor repair; existing source edging retained','newRGBGeneration':False},indent=2))
