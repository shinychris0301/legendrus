from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned');source=Image.open(b/'Sources/OriginalReference.png').convert('RGB');records=[];previews=[]
def save(key,im,note):
 p=art/(key+'_r3.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'message_frame.png.meta').read_text(),count=1))
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':note});previews.append((key,im))
old=Image.open(art/'message_frame.png').convert('RGBA');a=np.array(old);h,w=a.shape[:2]
mask=np.array(Image.open('ProjectDocs/AI/TeamChatStyleAlignment/R1/Sources/RepairMasks/message_frame.png').convert('L'))
weight=np.minimum(cv2.distanceTransform((mask>0).astype('uint8'),cv2.DIST_L2,5)/6,1);weight=cv2.resize(weight,(w,h),interpolation=cv2.INTER_LINEAR)
# Clean lower-right portion of the original bubble, below its first line and after its second line.
donor=np.array(source.crop((696,500,830,527))).astype('float32');grain=donor-cv2.GaussianBlur(donor,(0,0),2.5)
grain=np.concatenate([grain,grain[:,::-1]],axis=1);grain=np.concatenate([grain,grain[::-1]],axis=0)
grain=np.tile(grain,(102//grain.shape[0]+1,680//grain.shape[1]+1,1))[:102,:680];grain=cv2.resize(grain,(w,h),interpolation=cv2.INTER_CUBIC)
result=a.copy();result[:,:,:3]=np.clip(a[:,:,:3].astype('float32')+grain*weight[:,:,None]*1.25,0,255).astype('uint8');result[weight==0]=a[weight==0]
assert np.array_equal(result[:,:,3],a[:,:,3]);assert np.array_equal(result[weight==0],a[weight==0]);save('message_frame',Image.fromarray(result),'Existing RGB/alpha preserved outside original lettering-removal mask; donor high-frequency grain blended into repaired interior, existing base shading retained')
Image.fromarray((weight*255).astype('uint8')).save(b/'Sources/ParchmentRepairMask.png');source.crop((696,500,830,527)).resize((804,162)).save(b/'QA/GrainDonor.png')
for key,box in json.loads((b/'Sources/Assessment.json').read_text())['isolations'].items():
 files=list((b/'Sources/Scenario'/key).glob('*.png'))
 if not files:continue
 raw=source.crop(box);size=(raw.width*6,raw.height*6);rgb=raw.resize(size,Image.Resampling.LANCZOS);out=rgb.convert('RGBA');alpha=np.array(Image.open(files[0]).convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS))
 # Keep the primary object and its antialiased edge, removing disconnected parchment fragments.
 cs,_=cv2.findContours((alpha>80).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);main=max(cs,key=cv2.contourArea);keep=np.zeros_like(alpha);cv2.drawContours(keep,[main],-1,255,-1);alpha=np.minimum(alpha,cv2.dilate(keep,np.ones((7,7),'uint8')))
 if key=='energy_medallion':
  support=Image.new('L',size);ImageDraw.Draw(support).polygon([(x*6,y*6)for x,y in [(54,0),(72,9),(96,26),(108,58),(94,89),(66,108),(49,113),(19,96),(3,74),(0,51),(13,27),(35,11)]],fill=255);alpha=np.minimum(alpha,np.array(support))
 out.putalpha(Image.fromarray(alpha));assert out.convert('RGB').tobytes()==rgb.tobytes();save(key,out,'Original RGB with Scenario alpha; disconnected neighboring-fragment cleanup; medallion bounded by silhouette support')
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'newAssets':records,'parchmentAlphaUnchanged':True,'parchmentOutsideRepairMaskUnchanged':True,'reusedGoldLightning':'ProductionUI/NavigationAligned/energy','grainDonorSourceBox':[696,500,830,527]},indent=2))
sheet=Image.new('RGB',(1050,len(previews)*205),(25,28,32));d=ImageDraw.Draw(sheet)
for i,(key,im) in enumerate(previews):
 d.text((5,i*205+2),key,fill='white')
 for j,color in enumerate(('black','white','#ec00b5')):
  tile=Image.new('RGBA',(340,177),color);im2=im.copy();im2.thumbnail((330,168));tile.alpha_composite(im2,((340-im2.width)//2,(177-im2.height)//2));sheet.paste(tile.convert('RGB'),(j*350,i*205+22))
sheet.save(b/'QA/AssetAlpha.png')
