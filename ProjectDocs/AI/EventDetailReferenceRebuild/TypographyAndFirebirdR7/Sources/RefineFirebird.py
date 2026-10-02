"""Refine accepted Scenario/source alpha only; preserve original RGB and source geometry."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json
b=Path(__file__).resolve().parent;qa=b.parent/'QA';art=Path('Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned')
src=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/EdgesAndOrnamentsR3/Sources/bird_source.png').convert('RGBA');rgb=np.array(src)[:,:,:3]
old=np.array(Image.open(art/'firebird_r3.png').getchannel('A'));mask=(old>=128).astype('uint8')*255
# Close pinholes within solid feathers, preserving the large gaps between feather ribbons.
inv=255-mask;n,labels,stats,_=cv2.connectedComponentsWithStats(inv)
for i in range(1,n):
 if stats[i,cv2.CC_STAT_AREA]<=12:mask[labels==i]=255
# Re-evaluate only the narrow source edge. Existing eroded interiors stay foreground,
# so blue feather eyes and red/black shaded portions cannot be punched out.
seeds=np.full(mask.shape,cv2.GC_BGD,np.uint8)
seeds[cv2.dilate(mask,np.ones((5,5),np.uint8))>0]=cv2.GC_PR_BGD
seeds[mask>0]=cv2.GC_PR_FGD
seeds[cv2.erode(mask,np.ones((3,3),np.uint8))>0]=cv2.GC_FGD
cv2.grabCut(rgb.copy(),seeds,None,np.zeros((1,65)),np.zeros((1,65)),4,cv2.GC_INIT_WITH_MASK)
a=np.isin(seeds,[1,3]).astype('uint8')*255
# Detached low-area segmentation dust is not part of the feather silhouette.
# Recognizable source starbursts remain in the separate existing emission layer.
n,labels,stats,_=cv2.connectedComponentsWithStats(a)
for i in range(1,n):
 if stats[i,cv2.CC_STAT_AREA]<22:a[labels==i]=0
# Smooth sub-pixel stair steps without changing the large silhouette or filling feather gaps.
a=Image.fromarray(a).filter(ImageFilter.GaussianBlur(.45))
arr=np.array(a);plaque=np.array(Image.open(art/'countdown_r3.png').getchannel('A'))
ex=np.zeros(arr.shape,np.uint8);ex[279:342,110:600]=plaque
arr[ex>0]=0;arr[-4:]=0;arr[177:191,662:673]=0 # Detached city window, not a feather.
bird=src.copy();bird.putalpha(Image.fromarray(arr));bird.save(art/'firebird_r7.png')
# Preserve source starbursts while removing light from detached silhouette dust.
prior=np.array(Image.open(art/'firebird_sparks_r3.png').getchannel('A'))
near=cv2.dilate((arr>32).astype('uint8'),np.ones((9,9),np.uint8))>0
stars=Image.new('L',src.size);d=ImageDraw.Draw(stars)
for x,y,r in [(37,294,26),(224,264,30),(174,140,12)]:d.ellipse((x-r,y-r,x+r,y+r),fill=255)
keep=near|(np.array(stars)>0);sparks=np.where(keep,prior,0).astype('uint8');sparks[ex>0]=0;sparks[-4:]=0;sparks[177:191,662:673]=0
light=src.copy();light.putalpha(Image.fromarray(sparks));light.save(art/'firebird_sparks_r7.png')
halo=Image.new('RGBA',src.size,(255,156,32,0));halo.putalpha(Image.fromarray(sparks).filter(ImageFilter.GaussianBlur(7)).point(lambda v:int(v*.32)));halo.save(art/'firebird_halo_r7.png')
for key in ['firebird_r7','firebird_sparks_r7']:
 im=Image.open(art/(key+'.png'));out=Image.new('RGB',(im.width,3*(im.height+22)));d=ImageDraw.Draw(out)
 for i,col in enumerate(['black','white','#ed00d2']):
  y=i*(im.height+22);d.text((4,y+3),col,fill='white');d.rectangle((0,y+22,im.width,y+22+im.height),fill=col);out.paste(im,(0,y+22),im)
 out.save(qa/(key+'_alpha.png'))
# Record provenance, not a claim of perfect alpha or original font identity.
audit={}
for key in ['firebird_r7','firebird_sparks_r7']:
 im=np.array(Image.open(art/(key+'.png')));v=im[:,:,3]>0;diff=int(np.any(im[:,:,:3][v]!=rgb[v],axis=1).sum());assert diff==0
 audit[key]={'visiblePixels':int(v.sum()),'sourceRGBMismatches':diff}
(qa/'ArtAudit.json').write_text(json.dumps(audit,indent=2))
