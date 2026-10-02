from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parent;art=Path('Assets/LegendsOfRus/Resources/ProductionUI');dst=art/'GameplayPortraitBandR7';dst.mkdir(exist_ok=True)
report={'newScenarioCalls':0,'assets':{}}
for name,path in [('header',art/'GameplayHeaderR6/header.png'),('portrait',art/'GameplayOrnamentsR5/portrait.png')]:
 a=np.array(Image.open(path).convert('RGBA'));before=a.copy();support=np.zeros(a.shape[:2],bool)
 if name=='header':
  # This is the unused lower tail of the reconstructed left wing, not the main HUD rail.
  support[173:,:115]=True;a[173:,:115,3]=0
 else:
  # Follow the bottom metal contour instead of the former nameplate's straight dark edge.
  # Leave both corner scrolls and the central sapphire/diamond untouched.
  for x in list(range(42,102))+list(range(135,180)):
   rgb=a[195:214,x,:3].astype(float)
   ys=np.flatnonzero((rgb[:,0]>95)&(rgb[:,1]>45)&(rgb[:,0]>rgb[:,2]*1.35))
   if not len(ys):continue
   y=int(ys[-1])+195
   support[y+1:214,x]=True;a[y+1:214,x,3]=0
 Image.fromarray(a).save(dst/f'{name}.png');Image.fromarray((support*255).astype('uint8')).save(b/f'{name}_RepairSupport.png')
 for c,n in [((0,0,0),'Black'),((255,255,255),'White'),((255,0,180),'Magenta')]:
  im=Image.new('RGBA',(a.shape[1],a.shape[0]),c+(255,));im.alpha_composite(Image.fromarray(a));im.save(b/f'{name}_Alpha{n}.png')
 report['assets'][name]={'changedRGBPixels':int(np.any(a[:,:,:3]!=before[:,:,:3],axis=2).sum()),'changedAlphaPixels':int((a[:,:,3]!=before[:,:,3]).sum()),'changesOutsideRepairSupport':int(((a[:,:,3]!=before[:,:,3])&~support).sum())}
report['headerTailVisiblePixelsAfter']=int((np.array(Image.open(dst/'header.png'))[173:,:115,3]>0).sum())
report['method']='Alpha-only removal of unused left-header tail below the portrait and bounded lower-portrait dark residue. No generation, recoloring or changes to main HUD/card/board geometry.'
(b/'AssetProvenance.json').write_text(json.dumps(report,indent=2));print(report)
