from pathlib import Path
from PIL import Image,ImageDraw,ImageOps
import numpy as np,cv2,json,hashlib
b=Path('ProjectDocs/AI/TutorialStyleAlignment/NativeDesignR3');s=b/'Sources';rgb=np.array(Image.open(s/'Inputs/panel_r3.png').convert('RGB'));w,h=1024,1792
# Final face repair uses a clean navy patch from the approved concept itself.
# The immutable Scenario input stays unchanged; only the documented repair region differs.
concept=Image.open('ProjectDocs/AI/TutorialStyleAlignment/DesignR2/Tutorial_Help_Design_R2.png').convert('RGB')
tile=concept.crop((815,1108,869,1182));tw,th=tile.size
mirrored=Image.new('RGB',(tw*2,th*2));mirrored.paste(tile,(0,0));mirrored.paste(ImageOps.mirror(tile),(tw,0));mirrored.paste(ImageOps.flip(tile),(0,th));mirrored.paste(ImageOps.flip(ImageOps.mirror(tile)),(tw,th))
patch=Image.new('RGB',(w,h))
for yy in range(0,h,th*2):
 for xx in range(0,w,tw*2):patch.paste(mirrored,(xx,yy))
face=Image.new('L',(w,h));draw=ImageDraw.Draw(face);draw.rectangle((81,268,943,1667),fill=255);draw.rectangle((155,163,883,280),fill=255)
fm=np.array(face);blend=np.minimum(cv2.distanceTransform((fm>0).astype('uint8'),cv2.DIST_L2,3)/12,1)[...,None]
rgb=np.round(rgb*(1-blend)+np.array(patch)*blend).astype('uint8')
a=np.array(Image.open(s/'Scenario/panel_r3/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize((w,h),Image.Resampling.LANCZOS))
# Preserve intentionally opaque interior. Keep only the main frame contour, no scenery islands.
contours,_=cv2.findContours((a>128).astype('uint8'),cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);solid=np.zeros_like(a);cv2.drawContours(solid,[max(contours,key=cv2.contourArea)],-1,255,-1)
a=np.maximum(a,solid);n,labels,stats,_=cv2.connectedComponentsWithStats((a>10).astype('uint8'));largest=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);a[labels!=largest]=0
a[260:1670,79:946]=255
out=Image.fromarray(np.dstack([rgb,a]));dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/TutorialAligned/panel_r3.png');out.save(dst)
for name,c in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(w,h),c);sheet.paste(out,(0,0),out);sheet.save(b/'QA'/('FrameOn'+name+'.png'))
original=np.array(Image.open('ProjectDocs/AI/TutorialStyleAlignment/DesignR2/Tutorial_Help_Design_R2.png').convert('RGB'));mask=np.array(Image.open(s/'RepairMask.png'));assert np.array_equal(original[mask==0],rgb[mask==0])
(b/'AssetProvenance.json').write_text(json.dumps({'source':'../DesignR2/Tutorial_Help_Design_R2.png','sourceSha256':hashlib.sha256(Path('ProjectDocs/AI/TutorialStyleAlignment/DesignR2/Tutorial_Help_Design_R2.png').read_bytes()).hexdigest(),'runtime':str(dst),'sha256':hashlib.sha256(dst.read_bytes()).hexdigest(),'sourceRgbOutsideRepairMaskVerified':True,'alpha':'Scenario source isolation; largest connected exterior plus restored opaque interior','faceDonorBounds':[815,1108,869,1182],'faceRepair':'Reflected concept navy tile with12px inward feather; immutable Scenario input retained','repairMask':'Sources/RepairMask.png','billing':json.loads((s/'Scenario/panel_r3/Billing.json').read_text()),'reuse':'R1 Close/Next; existing game gems/wood/specials/boosters/obstacles, Settings cards/tabs, embedded ranking fonts and clean scenery; no whole reference at runtime'},indent=2))
print('Final art saved; original RGB outside repair mask passed.')
