from pathlib import Path
from PIL import Image
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;root=Path.cwd();art=root/'Assets/LegendsOfRus/Resources/ProductionUI'
a=np.array(Image.open(art/'GameplayOrnamentsR5/header.png').convert('RGBA'));before=a.copy();rgb=a[:,:,:3].astype(float)
# Only outer gutters: exclude all lacquer/parchment/wood faces and interior dividers.
band=np.zeros(a.shape[:2],bool)
band[:7,:]=True;band[168:,:115]=True;band[170:,115:259]=True;band[163:,259:610]=True;band[176:,610:]=True
band[:,:3]=True;band[:,814:]=True
# Retain original metal RGB and alpha. Eliminate dark matte between the metal and scene,
# with a narrow antialiased transition in shaded bronze instead of a rectangular black drop.
metal=(rgb[:,:,0]>95)&(rgb[:,:,1]>45)&(rgb[:,:,0]>rgb[:,:,2]*1.4)
distance=cv2.distanceTransform((~metal).astype('uint8'),cv2.DIST_L2,cv2.DIST_MASK_PRECISE)
strength=np.clip((rgb[:,:,0]-45)/50,0,1)*np.clip((rgb[:,:,1]-22)/27,0,1)
strength=np.maximum(strength,np.where(distance<=1,0.45,0))
strength[metal]=1
alpha=np.rint(a[:,:,3]*strength).astype('uint8');a[:,:,3]=np.where(band,alpha,a[:,:,3])
dst=art/'GameplayHeaderR6';dst.mkdir(exist_ok=True);Image.fromarray(a).save(dst/'header.png')
Image.fromarray((band*255).astype('uint8')).save(b/'RepairSupport.png')
for c,n in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
 im=Image.new('RGBA',(a.shape[1],a.shape[0]),c+(255,));im.alpha_composite(Image.fromarray(a));im.save(b/f'HeaderAlpha{n}.png')
report={'method':'Bounded alpha-only removal of baked exterior dark matte; opaque faces, divider recesses and all RGB preserved. Retain lit gold and one-pixel contact shading.','newScenarioCalls':0,'changedRGBPixels':int(np.any(a[:,:,:3]!=before[:,:,:3],axis=2).sum()),'changedAlphaPixels':int((a[:,:,3]!=before[:,:,3]).sum()),'changesOutsideRepairSupport':int(((a[:,:,3]!=before[:,:,3])&~band).sum()),'litGoldAlphaChanges':int(((a[:,:,3]!=before[:,:,3])&metal).sum())}
(b/'AssetProvenance.json').write_text(json.dumps(report,indent=2));print(report)
# Inspection-only composite of old/new artwork over the same unmodified environment crop.
env=Image.open(art/'GameplayRefinedR2/environment.png').convert('RGBA')
for im,n in [(Image.fromarray(before),'Before'),(Image.fromarray(a),'After')]:
 base=env.copy();base.alpha_composite(im,(115,57));base.crop((230,200,940,250)).resize((1065,75)).save(b/f'Edge{n}.png')
