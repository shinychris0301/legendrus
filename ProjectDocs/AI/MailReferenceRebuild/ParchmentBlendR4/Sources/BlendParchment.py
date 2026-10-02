"""Blend approved parchment grain to the original inner-edge lighting; preserve alpha/frame.
Run with the project image-processing environment from the project root.
"""
from pathlib import Path
from PIL import Image
import cv2,numpy as np,json,hashlib
b=Path(__file__).resolve().parents[1];old=b.parent/'OpenedLetterR3';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned');records={}
for name,pad in [('letter',(48,37,49,42)),('preview',(27,22,31,25))]:
 im=Image.open(b/'Sources'/f'{name}_surface_r3.png').convert('RGBA');w,h=im.size;W,H=w//3,h//3
 a=np.array(im.resize((W,H),Image.Resampling.LANCZOS));rgb=a[:,:,:3].copy();mask=np.zeros((H,W),np.uint8);l,t,r,bot=pad
 # Trace the actual inner paper contour, including all four previously restored
 # corner paper patches. Keep only the carved gold and outer frame excluded.
 curve=([(110,31),(88,37),(74,47),(58,66),(46,87),(36,113)] if name=='letter' else [(69,14),(51,20),(36,32),(23,50),(15,78)])
 pts=curve+[(x,H-1-y) for x,y in reversed(curve)]+[(W-1-x,H-1-y) for x,y in curve]+[(W-1-x,y) for x,y in reversed(curve)]
 cv2.fillPoly(mask,[np.array(pts,dtype=np.int32)],255)
 donor=np.array(Image.open(old/'Sources/Letter.png').convert('RGB').crop((704,956,832,1052))).astype(np.float32)
 donor=donor-cv2.GaussianBlur(donor,(0,0),12)+np.median(donor.reshape(-1,3),axis=0)
 tile=np.concatenate([donor,donor[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0)
 texture=np.clip(np.tile(tile,(H//tile.shape[0]+1,W//tile.shape[1]+1,1))[:H,:W],0,255).astype(np.uint8)
 x,y,bw,bh=cv2.boundingRect(mask)
 res=cv2.seamlessClone(texture.copy(),rgb.copy(),mask.copy(),(x+bw//2,y+bh//2),cv2.NORMAL_CLONE)
 # Preserve the original high-resolution frame and all alpha, change only the paper.
 native_delta=res.astype(np.int16)-rgb.astype(np.int16)
 weight=np.clip(cv2.distanceTransform(mask,cv2.DIST_L2,5)/5.0,0,1)
 weight=weight*weight*(3-2*weight)
 delta=cv2.resize(native_delta.astype(np.float32)*weight[:,:,None],(w,h),interpolation=cv2.INTER_CUBIC)
 support=cv2.resize(mask,(w,h),interpolation=cv2.INTER_NEAREST)>0
 before=np.array(im);after=before.copy();adjusted=np.clip(before[:,:,:3].astype(np.float32)+delta,0,255).round().astype(np.uint8)
 after[:,:,:3][support]=adjusted[support]
 assert np.array_equal(after[:,:,3],before[:,:,3]);assert np.array_equal(after[~support],before[~support])
 asset=f'{name}_surface_r4.png';Image.fromarray(after).save(out/asset)
 Image.fromarray(mask).save(b/'Sources'/f'{name}_BlendMask.png')
 result=Image.fromarray(after)
 canvas=Image.new('RGB',(1080,360))
 for i,p in enumerate([im,result]):
  c=p.crop((0,0,540,540)).resize((540,360));canvas.paste(c,(540*i,0),c)
 canvas.save(b/'QA'/f'{name}_CornerBeforeAfter.png')
 result.thumbnail((700,950));result.save(b/'QA'/asset)
 records[name]={'asset':str(out/asset),'method':'Poisson blend of approved clean donor grain into original inner-edge lighting, including the paper inside all four restored corner contours. Correction fades over 5 native pixels. Original high-resolution carving outside mask and all alpha preserved. No generated RGB.','source':str(b/'Sources'/f'{name}_surface_r3.png'),'sourceSha256':hashlib.sha256((b/'Sources'/f'{name}_surface_r3.png').read_bytes()).hexdigest(),'sha256':hashlib.sha256((out/asset).read_bytes()).hexdigest(),'alphaUnchanged':True,'outsideMaskUnchanged':True}
(b/'Sources/Provenance.json').write_text(json.dumps(records,indent=2))
# Diagnostic only: transparent assets over black, white and saturated magenta.
canvas=Image.new('RGB',(1200,850))
for row,name in enumerate(('preview','letter')):
 im=Image.open(out/f'{name}_surface_r4.png').convert('RGBA');im.thumbnail((380,410))
 for col,color in enumerate(((0,0,0),(255,255,255),(255,0,150))):
  canvas.paste(color,(col*400,row*425,(col+1)*400,(row+1)*425));canvas.paste(im,(col*400+(400-im.width)//2,row*425),im)
canvas.save(b/'QA/AlphaBackgrounds.png')
print('Two parchment surfaces blended. Frame outside masks and all alpha unchanged.')
