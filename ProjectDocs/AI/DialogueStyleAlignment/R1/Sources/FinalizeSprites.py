from pathlib import Path
from PIL import Image,ImageOps,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib,shutil
b=Path(__file__).resolve().parent;s=b;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DialogueAligned');out.mkdir(exist_ok=True);src=Image.open(b/'OriginalReference.png').convert('RGB');data=json.loads((b/'Assessment.json').read_text());records=[]
def field(box,size,strength=.75):
 donor=np.array(src.crop(box)).astype('float32');grain=donor-cv2.GaussianBlur(donor,(0,0),6);grain-=grain.mean(axis=(0,1));base=np.median(donor.reshape(-1,3),axis=0)
 tile=np.concatenate([grain,grain[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0);w,h=size;texture=np.tile(tile,(int(np.ceil(h/tile.shape[0])),int(np.ceil(w/tile.shape[1])),1))[:h,:w];return Image.fromarray(np.uint8(np.clip(base+strength*texture,0,255)))
for a in data['assets']:
 n=a['name'];im=src.crop(a['bounds']);w,h=im.size;repair=Image.new('L',im.size)
 job='dobrynya_retry' if n=='dobrynya' else n
 alpha=Image.open(b/'Scenario'/job/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(im.size,Image.Resampling.LANCZOS)
 if n=='panel':
  shape=Image.new('L',im.size);ImageDraw.Draw(shape).rounded_rectangle((20,155,w-20,h-21),radius=32,fill=255)
  distance=cv2.distanceTransform(np.array(shape),cv2.DIST_L2,5);m=np.clip(distance/16,0,1);m=m*m*(3-2*m);paperMask=Image.fromarray(np.uint8(np.ceil(m*255)));im=Image.composite(field((86,1162,183,1204),im.size),im,paperMask)
  # Keep the complete navy face opaque. Remove original fixed speaker lettering only.
  head=Image.new('L',im.size);ImageDraw.Draw(head).rectangle((277,68,574,123),fill=255);head=head.filter(ImageFilter.MaxFilter(9)).filter(ImageFilter.GaussianBlur(2));im=Image.composite(field((287,1098,318,1135),im.size,.4),im,head);repair=Image.fromarray(np.maximum(np.array(paperMask),np.array(head)))
  opaque=Image.new('L',im.size);d=ImageDraw.Draw(opaque);d.rectangle((25,132,w-25,h-30),fill=255);d.rectangle((245,58,591,137),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(opaque)))
  # Clip unrelated tableware outside the panel silhouette; never remove frame/crest.
  keep=Image.new('L',im.size);ImageDraw.Draw(keep).polygon([(0,96),(157,96),(190,34),(360,34),(418,0),(484,34),(650,34),(688,96),(w,96),(w,h),(0,h)],fill=255);alpha=Image.fromarray(np.minimum(np.array(alpha),np.array(keep)))
 elif n=='continue':
  shape=Image.new('L',im.size);ImageDraw.Draw(shape).rounded_rectangle((54,25,w-54,h-22),radius=18,fill=255);distance=cv2.distanceTransform(np.array(shape),cv2.DIST_L2,5);m=np.clip(distance/12,0,1);m=m*m*(3-2*m);repair=Image.fromarray(np.uint8(np.ceil(m*255)));im=Image.composite(field((310,1480,620,1494),im.size,.65),im,repair);opaque=Image.new('L',im.size);ImageDraw.Draw(opaque).rectangle((67,32,w-67,h-29),fill=255);alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(opaque)))
 else:
  # Original lower bodies are occluded by the UI/table. Fade only the hidden lower edge;
  # remove neighboring nameplate/tableware, preserving all visible portrait pixels.
  ar=np.array(alpha).astype(float)
  panel=Image.open(out/'panel.png').getchannel('A');overlay=Image.new('L',im.size);overlay.paste(panel,(45-a['bounds'][0],1022-a['bounds'][1]));ar*=1-np.array(overlay)/255.0
  y=np.arange(h)+a['bounds'][1];fade=np.clip((1080-y)/28,0,1);ar*=fade[:,None];alpha=Image.fromarray(np.uint8(ar))
 rgba=im.convert('RGBA');rgba.putalpha(alpha);rgba.save(out/f'{n}.png');repair.save(b/'RepairMasks'/f'{n}_final.png')
 original=np.array(src.crop(a['bounds']));assert np.array_equal(np.array(im)[np.array(repair)==0],original[np.array(repair)==0]);assert hashlib.sha256((b/'Inputs'/f'{n}.png').read_bytes()).hexdigest()==a['inputSha256']
 records.append({'name':n,'jobFolder':job,'bounds':a['bounds'],'finalSha256':hashlib.sha256((out/f'{n}.png').read_bytes()).hexdigest(),'originalRGBOutsideRepairUnchanged':True,'submittedInputUnchanged':True})
 sheet=Image.new('RGB',(w*3,h))
 for i,c in enumerate(['black','white','#ef00bd']):back=Image.new('RGBA',im.size,c);back.alpha_composite(rgba);sheet.paste(back.convert('RGB'),(w*i,0))
 sheet.thumbnail((1500,1000));sheet.save(b.parent/'QA'/f'{n}_AlphaReview.png')
shutil.copy2(b/'Scenario/backgrounds/booster_medallion_trial01.png',out/'hall.png')
(b.parent/'QA/AssetChecks.json').write_text(json.dumps(records,indent=2));print('Four source sprites plus clean hall finalized.')
