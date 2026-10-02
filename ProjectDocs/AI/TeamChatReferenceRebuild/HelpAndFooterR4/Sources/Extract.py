from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned');source=Image.open(b/'Sources/OriginalReference.png').convert('RGB');records=[];previews=[]
def save(key,im,note):
 p=art/(key+'_r4.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'send_frame.png.meta').read_text(),count=1))
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':note});previews.append((key,im))
for key,box in json.loads((b/'Sources/ExtractionBounds.json').read_text()).items():
 raw=source.crop(box);size=(raw.width*4,raw.height*4);rgb=raw.resize(size,Image.Resampling.LANCZOS);out=rgb.convert('RGBA');alpha=Image.open(b/'Sources/Scenario'/key/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS)
 out.putalpha(alpha)
 assert out.convert('RGB').tobytes()==rgb.tobytes()
 if key=='help':
  save('help_ru',out,'Original Russian button RGB with Scenario alpha; lettering, green face and gold border retained together')
  # English requires a live label: reconstruct only the hidden lettering area.
  a=np.array(out);h,w=a.shape[:2];original=a.copy();x0,x1,y0,y1=24*4,143*4,13*4,49*4
  field=a[:,:,:3].copy();left=np.mean(a[y0:y1,15*4:23*4,:3],axis=1);right=np.mean(a[y0:y1,145*4:150*4,:3],axis=1)
  t=np.linspace(0,1,x1-x0)[None,:,None];field[y0:y1,x0:x1]=left[:,None,:]*(1-t)+right[:,None,:]*t
  mask=np.zeros((h,w),'uint8');mask[y0:y1,x0:x1]=255;weight=np.minimum(cv2.distanceTransform((mask>0).astype('uint8'),cv2.DIST_L2,5)/8,1)
  a[:,:,:3]=(a[:,:,:3]*(1-weight[:,:,None])+field*weight[:,:,None]).astype('uint8')
  assert np.array_equal(a[:,:,3],original[:,:,3]);assert np.array_equal(a[mask==0],original[mask==0]);Image.fromarray(mask).save(b/'Sources/EnglishFaceRepairMask.png')
  save('help_blank',Image.fromarray(a),'Same source border and Scenario alpha; lettering interior repaired from side-face colors for live localized English')
 else:
  # Scenario retains the dark backing between scrolls. Remove that backing, not the gold/blue artwork.
  a=np.array(out);light=np.max(a[:,:,:3],axis=2).astype('float32');keep=np.clip((light-16)/32,0,1);keep=keep*keep*(3-2*keep)
  a[:,:,3]=(a[:,:,3]*keep).astype('uint8');out=Image.fromarray(a)
  save('footer_sapphire',out,'Source RGB with Scenario alpha; neighboring plaque excluded by staged silhouette; dark backing removed with luminance alpha refinement')
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'newAssets':records,'sourceSha256':hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest()},indent=2)+'\n')
sheet=Image.new('RGB',(1080,len(previews)*185),(25,28,32));d=ImageDraw.Draw(sheet)
for i,(key,im) in enumerate(previews):
 d.text((5,i*185+2),key,fill='white')
 for j,color in enumerate(('black','white','#ec00b5')):
  tile=Image.new('RGBA',(355,157),color);im2=im.copy();im2.thumbnail((350,148));tile.alpha_composite(im2,((355-im2.width)//2,(157-im2.height)//2));sheet.paste(tile.convert('RGB'),(j*360,i*185+22))
sheet.save(b/'QA/AssetAlpha.png')
