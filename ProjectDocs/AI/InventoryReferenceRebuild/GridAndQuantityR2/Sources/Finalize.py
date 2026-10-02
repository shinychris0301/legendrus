from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent;out=Path('Assets/LegendsOfRus/Resources/ProductionUI/InventoryAligned');qa=b.parent/'QA'
sheet=Image.open(b/'Sheet.png').convert('RGB');matte=Image.open(b/'Scenario/frames/booster_medallion_trial01.png').convert('RGBA').getchannel('A');assert sheet.size==matte.size
items=json.loads((b/'Sheet.json').read_text());art=[];records=[]
for entry in items:
 name=entry['name'];rgb=sheet.crop(entry['sheetBox']);alpha=matte.crop(entry['sheetBox']);mask=Image.new('L',rgb.size)
 if name=='grid_frame_r2':
  # The tiny between-card donor retained gold fragments. Replace that rejected hidden face
  # with approved empty navy material; keep the submitted sheet as immutable API evidence.
  donor=np.array(Image.open('Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/tab.png').convert('RGB').crop((75,45,585,158)))
  donor=np.concatenate([donor,donor[:,::-1]],1);donor=np.concatenate([donor,donor[::-1]],0)
  texture=Image.fromarray(np.tile(donor,(rgb.height//donor.shape[0]+1,rgb.width//donor.shape[1]+1,1))[:rgb.height,:rgb.width])
  ImageDraw.Draw(mask).rectangle((33,27,902,728),fill=255);rgb=Image.composite(texture,rgb,mask)
  a=np.array(alpha);a[22:733,29:907]=255;a[750:]=0;alpha=Image.fromarray(a)
 else:
  # Remove neighboring medallion/corner scraps outside the quantity plaque silhouette.
  points=[(27,0),(221,0),(230,7),(232,18),(242,32),(233,40),(231,57),(222,66),(210,71),(28,71),(16,67),(9,57),(10,44),(0,37),(9,30),(12,15),(19,8)]
  support=Image.new('L',rgb.size);ImageDraw.Draw(support).polygon([(x*3,y*3) for x,y in points],fill=255);support=support.filter(ImageFilter.GaussianBlur(.7))
  a=np.array(alpha);a[21:201,60:675]=255;alpha=Image.fromarray(np.minimum(a,np.array(support)))
 rgba=rgb.convert('RGBA');rgba.putalpha(alpha);rgba.save(out/(name+'.png'));mask.save(b/(name+'_FinalRepairMask.png'));alpha.save(b/(name+'_FinalAlpha.png'))
 art.append((name,rgba));prepared=np.array(sheet.crop(entry['sheetBox']));actual=np.array(rgb);outside=np.array(mask)==0
 records.append({**entry,'runtimePath':str(out/(name+'.png')),'rgbChangesOutsideFinalRepairMask':int(np.any(prepared!=actual,axis=2)[outside].sum()),'opaquePixels':int((np.array(alpha)==255).sum()),'transparentPixels':int((np.array(alpha)==0).sum())})
for label,bg in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(230,0,150))]:
 canvas=Image.new('RGB',(1000,1040),bg)
 for (name,im),xy in zip(art,[(30,20),(125,800)]):canvas.paste(im,xy,im)
 canvas.save(qa/('AssetsOn'+label+'.png'))
(b/'AssetProvenance.json').write_text(json.dumps({'scenarioCalls':1,'scenarioCU':3,'newRGBGeneration':False,'source':'../R1/Sources/Reference.png','hiddenGridFace':'Approved IndividualRanking tab interior; first narrow-gap donor rejected due repeated gold residue','assets':records},indent=2))
