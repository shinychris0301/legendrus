from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib,re,uuid
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned');src=Image.open(b/'Sources/OriginalReference.png').convert('RGB');records=[];previews=[]
def save(key,im,note):
 p=art/(key+'.png');im.save(p);meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]+','guid: '+uuid.uuid4().hex,(art/'visit_frame.png.meta').read_text(),count=1))
 records.append({'asset':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'operation':note});previews.append((key,im))
def matte(name,size):return Image.open(b/'Sources/Scenario'/name/'booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS)
flagdata=json.loads((b/'Sources/FlagBounds.json').read_text());sheet=Image.open(b/'Sources/Inputs/flags.png');alpha=matte('flags',sheet.size)
for key,r in flagdata.items():
 if key=='winter':r['source']=[260,1320,313,1382] # recover top finial clipped by the initial staging crop
 native=src.crop(r['source']);rgb=native.resize((native.width*4,native.height*4),Image.Resampling.LANCZOS);pix=np.array(rgb).astype('float32');w,h=rgb.size
 # Scenario kept the opaque navy crop backing. Refine its mask using the original gold outline.
 gold=((pix[:,:,0]>65)&(pix[:,:,0]>pix[:,:,1]*1.10)&(pix[:,:,0]>pix[:,:,2]*1.35)).astype('uint8')
 gold=cv2.morphologyEx(gold,cv2.MORPH_CLOSE,np.ones((9,9),'uint8'))
 cs,_=cv2.findContours(gold,cv2.RETR_EXTERNAL,cv2.CHAIN_APPROX_SIMPLE);main=max(cs,key=cv2.contourArea)
 keep=np.zeros((h,w),'uint8');cv2.fillConvexPoly(keep,cv2.convexHull(main),255);top=np.argmax(gold>0,axis=0);keep[np.arange(h)[:,None]<top[None,:]]=0;keep=cv2.GaussianBlur(keep,(3,3),.6)
 # Original face stays opaque; transparent exterior is traced from the connected metal silhouette.
 out=rgb.convert('RGBA');out.putalpha(Image.fromarray(keep));save('flag_'+key+'_r2',out,'Original RGB; Scenario retained backing, locally replaced by gold-outline silhouette; winter top finial recovered from original')
navy=np.array(src.crop((496,519,545,582)));navy=np.concatenate([navy,navy[:,::-1]],1);navy=np.concatenate([navy,navy[::-1]],0)
def material(w,h):return Image.fromarray(np.tile(navy,(h//navy.shape[0]+1,w//navy.shape[1]+1,1))[:h,:w])
raw=Image.open(b/'Sources/Inputs/tabs.png').convert('RGB');w,h=raw.size;a=np.array(matte('tabs',raw.size));support=Image.new('L',(w,h));d=ImageDraw.Draw(support);d.polygon([(12,12),(28,7),(126,7),(137,0),(150,7),(385,7),(400,0),(415,7),(639,7),(654,0),(669,7),(w-24,7),(w-3,16),(w-3,88),(w-15,96),(443,96),(405,103),(370,96),(10,96),(0,72),(0,30)],fill=255);a=np.minimum(a,np.array(support));opaque=Image.new('L',(w,h));ImageDraw.Draw(opaque).rectangle((24,14,w-22,82),fill=255);a=np.maximum(a,np.array(opaque))
# Replace the full empty faces, not only lettering-shaped rectangles. The surrounding source gold remains.
polys=[[(33,22),(173,22),(183,34),(183,66),(171,78),(33,78),(24,67),(24,33)],[(285,23),(427,23),(445,35),(445,67),(430,80),(285,80),(275,68),(275,35)],[(550,23),(751,23),(766,36),(766,67),(751,80),(550,80),(537,68),(537,35)]]
# Faces use actual global panel insets; first two extend to their right corners.
polys[0]=[(33,22),(229,22),(241,34),(241,66),(229,78),(33,78),(24,67),(24,33)]
polys[1]=[(285,23),(501,23),(515,35),(515,67),(501,80),(285,80),(275,68),(275,35)]
base=raw.copy()
for poly in polys:
 mask=Image.new('L',(w,h));ImageDraw.Draw(mask).polygon(poly,fill=255);base.paste(material(w,h),(0,0),mask)
segments=[(0,0,267,h),(267,0,533,h),(533,0,w,h)]
for selected in range(3):
 rgb=base.copy();blue=Image.open('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/tab_active.png').convert('RGB');blue=blue.crop((int(blue.width*.3),int(blue.height*.30),int(blue.width*.7),int(blue.height*.65))).resize((w,h),Image.Resampling.LANCZOS)
 mask=Image.new('L',(w,h));ImageDraw.Draw(mask).polygon(polys[selected],fill=255);rgb.paste(blue,(0,0),mask)
 for i,box in enumerate(segments):
  if selected!=0 and i!=selected:continue
  out=rgb.crop(box).convert('RGBA');out.putalpha(Image.fromarray(a).crop(box));out=out.resize((out.width*3,out.height*3),Image.Resampling.LANCZOS)
  save('tab_'+str(i)+('_active' if i==selected else '')+'_r2',out,'Connected original tab rails with repaired badge corner and source-family empty face; live localized labels')
# First tab inactive variant, absent from selected==0 pass.
out=base.crop(segments[0]).convert('RGBA');out.putalpha(Image.fromarray(a).crop(segments[0]));save('tab_0_r2',out.resize((out.width*3,out.height*3),Image.Resampling.LANCZOS),'Original tab rail; navy donor face for inactive state')
raw=Image.open(b/'Sources/Inputs/frame.png').convert('RGB');w,h=raw.size
raw.paste(material(w-94,28),(47,18)) # remove first portrait tip and row rail remnants
raw.paste(src.crop((310,1486,414,1500)).transpose(Image.Transpose.FLIP_TOP_BOTTOM).resize((w-94,18),Image.Resampling.LANCZOS),(47,0)) # replace old tab-bottom residue with source straight rail
# Remove partial adjacent Events badge below the lower rail using source rail continuation.
raw.paste(raw.crop((280,h-13,330,h)).resize((70,13)),(463,h-13))
a=np.array(matte('frame',raw.size));allowed=Image.new('L',(w,h));d=ImageDraw.Draw(allowed);d.polygon([(20,0),(w-25,0),(w-4,24),(w-4,66),(w-22,88),(w-22,h-93),(w-3,h-69),(w-2,h-21),(w-27,h-1),(25,h-1),(1,h-22),(1,h-64),(19,h-94),(19,87),(1,58),(1,22)],fill=255);a=np.minimum(a,np.array(allowed))
# Keep the full panel face and fine rails; source curls retain Scenario transparency outside it.
opaque=Image.new('L',(w,h));ImageDraw.Draw(opaque).rectangle((28,17,w-29,h-13),fill=255);a=np.maximum(a,np.array(opaque));out=raw.convert('RGBA');out.putalpha(Image.fromarray(a));save('outer_frame_r2',out.resize((w*2,h*2),Image.Resampling.LANCZOS),'Original outer rails/curls and sapphire trim; source-donor empty face; Scenario alpha plus bounded restoration; neighbor tip/badge removed')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
sheet=Image.new('RGB',(1050,len(previews)*180),(25,28,32));d=ImageDraw.Draw(sheet)
for i,(key,im) in enumerate(previews):
 d.text((5,i*180+2),key,fill='white')
 for j,color in enumerate(('black','white','#ec00b5')):
  tile=Image.new('RGBA',(345,154),color);small=im.copy();small.thumbnail((337,148));tile.alpha_composite(small,((345-small.width)//2,(154-small.height)//2));sheet.paste(tile.convert('RGB'),(j*350,i*180+22))
sheet.save(b/'QA/AssetAlpha.png')
