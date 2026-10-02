from pathlib import Path
from PIL import Image,ImageOps,ImageDraw
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/ErrorDialogStyleAlignment/ParchmentToneR2');s=b/'Sources';original=Image.open('ProjectDocs/AI/ErrorDialogStyleAlignment/R1/Sources/OriginalReference.png').convert('RGB');before=Image.open(s/'PanelBefore.png').convert('RGBA');arr=np.array(before);h,w=arr.shape[:2]
# Original empty parchment, free of lettering, icons and frame ornamentation.
donor=np.array(original.crop((606,605,748,811))).astype('float32');low=cv2.GaussianBlur(donor,(0,0),10);grain=donor-low;grain-=grain.mean(axis=(0,1));base=np.median(donor.reshape(-1,3),axis=0)
# Mirrored source texture keeps seams continuous. Low-frequency illumination comes from one face, not separate patches.
tile=np.concatenate([grain,grain[:,::-1]],axis=1);tile=np.concatenate([tile,tile[::-1]],axis=0);texture=np.tile(tile,(int(np.ceil(h/tile.shape[0])),int(np.ceil(w/tile.shape[1])),1))[:h,:w]
y,x=np.mgrid[:h,:w]
# Rounded inner-paper mask protects the frame's projecting corner flourishes.
shape=Image.new('L',(w,h));ImageDraw.Draw(shape).rounded_rectangle((38,183,657,817),radius=90,fill=255)
distance=cv2.distanceTransform(np.array(shape),cv2.DIST_L2,5);mask=np.clip(distance/35,0,1);mask=mask*mask*(3-2*mask)
# One gentle lighting field across the entire interior.
illumination=1.5*np.exp(-(((x-348)/280)**2+((y-476)/350)**2));paper=np.clip(base+texture*.75+illumination[:,:,None],0,255)
rgb=arr[:,:,:3].astype('float32');after=arr.copy();after[:,:,:3]=np.rint(rgb*(1-mask[:,:,None])+paper*mask[:,:,None]).astype('uint8');out=Image.fromarray(after);out.save('Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/panel.png');Image.fromarray(np.rint(mask*255).astype('uint8')).save(s/'RepairMask.png')
assert np.array_equal(after[:,:,3],arr[:,:,3]);assert np.array_equal(after[mask==0],arr[mask==0]);assert np.array_equal(after[:183],arr[:183]);stats=[]
for label,r in [('before',arr),('after',after)]:
 a=r[240:755,100:595,:3].astype(float);lum=a.mean(axis=2);blur=cv2.GaussianBlur(lum,(0,0),14);stats.append({'version':label,'interiorLowFrequencyStdDev':float(blur.std()),'interiorMeanRGB':a.mean(axis=(0,1)).tolist()})
(s/'RepairRecord.json').write_text(json.dumps({'method':'Single continuous parchment tone from original clean donor; source high-frequency texture normalized; smooth edge transition','donorBounds':[606,605,748,811],'baseRGB':base.tolist(),'alphaUnchanged':True,'outsideMaskUnchanged':True,'headerUnchanged':True,'beforeSha256':hashlib.sha256((s/'PanelBefore.png').read_bytes()).hexdigest(),'afterSha256':hashlib.sha256(Path('Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/panel.png').read_bytes()).hexdigest(),'metrics':stats,'newScenarioCalls':0,'costCU':0},indent=2))
for label,im in [('Before',before),('After',out)]:
 sheet=Image.new('RGB',(w*3,h))
 for i,c in enumerate(['black','white','#ef00bd']):bg=Image.new('RGBA',(w,h),c);bg.alpha_composite(im);sheet.paste(bg.convert('RGB'),(w*i,0))
 sheet.thumbnail((1500,1000));sheet.save(b/'QA'/f'{label}_AlphaReview.png')
print(json.dumps(stats))
