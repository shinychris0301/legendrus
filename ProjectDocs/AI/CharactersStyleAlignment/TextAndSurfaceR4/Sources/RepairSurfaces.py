from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned');src=Image.open('ProjectDocs/AI/CharactersStyleAlignment/R1/Sources/OriginalReference.png').convert('RGB');records=[]
def texture(bounds,size):
 donor=np.array(src.crop(bounds)).astype(float);base=np.median(donor.reshape(-1,3),axis=0);grain=np.clip(donor-cv2.GaussianBlur(donor,(0,0),4),-2.5,2.5);grain=np.concatenate([grain,grain[:,::-1]],1);grain=np.concatenate([grain,grain[::-1]],0);w,h=size;grain=np.tile(grain,((h+grain.shape[0]-1)//grain.shape[0],(w+grain.shape[1]-1)//grain.shape[1],1))[:h,:w];return Image.fromarray(np.uint8(np.clip(base+grain*.7,0,255))).convert('RGBA')
for old,new,bounds,face in [
 ('title','title_r4',(253,162,273,210),(76,44,515,122)),
 ('upgrade','upgrade_r4',(478,777,488,803),(25,16,203,52)),
 ('select','select_r4',(707,777,722,803),(22,16,198,52)),
 ('detail_frame_r2','detail_frame_r4',(891,466,909,590),(19,21,467,513))]:
 im=Image.open(b/'Before'/f'{old}.png').convert('RGBA');before=np.array(im);mask=Image.new('L',im.size);ImageDraw.Draw(mask).rounded_rectangle(face,radius=12 if old in ['title','detail_frame_r2'] else 6,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(1.5));clean=Image.composite(texture(bounds,im.size),im,mask);clean.putalpha(im.getchannel('A'))
 # Clear attached dark scenery outside the title's plaque, while retaining the original gold/sapphire silhouette and opaque face.
 if old=='title':
  a=np.array(im);rgb=a[:,:,:3].astype(float);gold=(rgb[:,:,0]>rgb[:,:,2]*1.2)&(rgb[:,:,1]>rgb[:,:,2]*1.05)&(rgb[:,:,0]>55);blue=(rgb[:,:,2]>rgb[:,:,0]*1.25)&(rgb[:,:,2]>45)
  support=np.uint8((gold|blue)&(a[:,:,3]>25))*255;support=cv2.dilate(support,np.ones((3,3),np.uint8));support=Image.fromarray(support).filter(ImageFilter.GaussianBlur(.45));opaque=Image.new('L',im.size);ImageDraw.Draw(opaque).rounded_rectangle((72,39,522,125),radius=24,fill=255)
  alpha=np.minimum(a[:,:,3],np.maximum(np.array(support),np.array(opaque)));count,labels,stats,_=cv2.connectedComponentsWithStats(np.uint8(alpha>30),8);keep=1+np.argmax(stats[1:,cv2.CC_STAT_AREA]);support=cv2.dilate(np.uint8(labels==keep),np.ones((3,3),np.uint8));alpha=alpha*support;clean.putalpha(Image.fromarray(alpha))
 clean.save(out/f'{new}.png');mask.save(b/f'{new}_repair_mask.png');after=np.array(clean)
 records.append({'asset':str(out/f'{new}.png'),'before':str(b/'Before'/f'{old}.png'),'rgbRepairMask':str(b/f'{new}_repair_mask.png'),'sourceTextureBounds':bounds,'originalRgbOutsideMaskPreserved':bool(np.array_equal(before[:,:,:3][np.array(mask)==0],after[:,:,:3][np.array(mask)==0])),'alphaPreserved':bool(np.array_equal(before[:,:,3],after[:,:,3])),'sha256':hashlib.sha256((out/f'{new}.png').read_bytes()).hexdigest()})
(b.parent/'AssetProvenance.json').write_text(json.dumps({'method':'Local repair of existing Scenario-derived sprites; no generation, no new API calls','costCU':0,'records':records},indent=2))
for color,name in [((0,0,0),'black'),((255,255,255),'white'),((180,0,160),'magenta')]:
 sheet=Image.new('RGB',(700,920),color)
 for i,record in enumerate(records):
  im=Image.open(record['asset']);im.thumbnail((650,205));sheet.paste(im,(20,20+i*225),im)
 sheet.save(b.parent/'QA'/f'Alpha_{name}.png')
