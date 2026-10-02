from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,uuid,re,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');raw=Image.open(b/'Sheet.png').convert('RGBA');mask=Image.open(b/'Scenario/controls/booster_medallion_trial01.png').getchannel('A');bounds=json.loads((b/'Bounds.json').read_text());outputs={}
for name in ['row_rebuild','claim_all_ru_rebuild']:
 im=raw.crop(bounds[name]);a=mask.crop(bounds[name]);w,h=im.size
 if name=='row_rebuild':
  # Source portrait tips overlap these rails. Continue neighboring source metal.
  im.paste(im.crop((250,0,330,24)),(156,0));im.paste(im.crop((250,h-24,330,h)),(156,h-24));d=ImageDraw.Draw(a);d.rectangle((15,15,w-16,h-16),fill=255)
 else:
  allowed=Image.new('L',(w*2,h*2));d=ImageDraw.Draw(allowed)
  pts=[(0,76),(11,63),(23,29),(42,24),(65,21),(171,22),(180,15),(188,9),(195,0),(203,8),(213,17),(226,25),(338,24),(358,28),(369,37),(376,63),(391,76),(377,88),(367,106),(353,119),(227,119),(215,123),(197,136),(180,121),(61,120),(31,116),(19,107),(12,92)]
  d.polygon([(x*4,y*4) for x,y in pts],fill=255);allowed=allowed.resize((w,h),Image.Resampling.LANCZOS);a=Image.fromarray(np.minimum(np.array(a),np.array(allowed)));d=ImageDraw.Draw(a);d.polygon([(84,96),(700,96),(738,150),(695,218),(87,218),(44,150)],fill=255)
 im.putalpha(a);outputs[name]=im
# Russian Claim face: exact original crop RGB under previously approved mask.
old=Path('ProjectDocs/AI/MailStyleAlignment/R1/Sources');original=Image.open(old/'Crops/claim_frame.png').convert('RGBA');original=original.resize(Image.open(out/'claim_frame.png').size,Image.Resampling.LANCZOS);original.putalpha(Image.open(out/'claim_frame.png').getchannel('A'));outputs['claim_ru_rebuild']=original
blank=outputs['claim_all_ru_rebuild'].copy();rgb=np.array(blank.convert('RGB'));repair=Image.new('L',blank.size);ImageDraw.Draw(repair).polygon([(88,91),(690,91),(738,150),(690,218),(88,218),(48,150)],fill=255)
from PIL import ImageFilter
repair=repair.filter(ImageFilter.GaussianBlur(4));donor=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/source_play_blank.png').convert('RGB').crop((120,80,460,179)).resize(blank.size,Image.Resampling.LANCZOS);d=np.array(donor).astype(float);d[:,:,1]*=1.18;donor=Image.fromarray(np.clip(d,0,255).astype('uint8'));blank2=Image.composite(donor,blank.convert('RGB'),repair).convert('RGBA');blank2.putalpha(blank.getchannel('A'));outputs['claim_all_blank_rebuild']=blank2;repair.save(b/'FooterCaptionRepair.png')
records=[]
for name,im in outputs.items():
 p=out/(name+'.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():
  text=(out/'title_ru.png.meta').read_text();text=re.sub('guid: [0-9a-f]+','guid: '+uuid.uuid4().hex,text,count=1);text=re.sub('spriteID: [0-9a-f]+','spriteID: '+uuid.uuid4().hex,text);meta.write_text(text)
 records.append({'name':name,'path':str(p),'size':im.size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
(b/'AssetProvenance.json').write_text(json.dumps({'assets':records,'processing':'Original RGB restored under Scenario alpha; row face uses accepted navy donor; source portrait intersections cleared with neighboring rail strips; footer allowed contour removes neighboring rails/backdrop, opaque green face restored. Russian Claim reuses accepted mask unchanged. English green face uses a bounded feathered approved Home Play donor after inpainting candidates were rejected. Hidden pixels and fine alpha remain approximate.'},indent=2))
sheet=Image.new('RGB',(1200,900));d=ImageDraw.Draw(sheet)
for row,bg in enumerate(['black','white','#e018a0']):
 d.rectangle((0,row*300,1200,(row+1)*300),fill=bg)
 for col,(n,im) in enumerate(outputs.items()):
  thumb=im.copy();thumb.thumbnail((290,255));sheet.paste(thumb,(col*300+(300-thumb.width)//2,row*300+20),thumb)
sheet.save(b.parent/'QA/AlphaBackgrounds.png')
