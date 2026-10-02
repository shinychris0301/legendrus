"""Source title RGB with Scenario silhouette, bounded alpha cleanup and opaque plaque face."""
from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json,hashlib
b=Path(__file__).resolve().parent;out=b.parent/'Extracted';out.mkdir(exist_ok=True);qa=b.parent/'QA';qa.mkdir(exist_ok=True)
src=Image.open(b/'title_original.png').convert('RGB');scale=2;size=(src.width*scale,src.height*scale);rgb=src.resize(size,Image.Resampling.LANCZOS)
a=np.array(Image.open(b/'Scenario/title/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(size,Image.Resampling.LANCZOS))
def shape(points):
 im=Image.new('L',(size[0]*2,size[1]*2));ImageDraw.Draw(im).polygon([(x*scale*2,y*scale*2) for x,y in points],fill=255);return np.array(im.resize(size,Image.Resampling.LANCZOS))
allowed=shape([(0,85),(35,70),(55,42),(230,42),(230,0),(510,0),(510,42),(704,42),(742,73),(742,154),(0,154)])
face=shape([(62,50),(681,50),(708,89),(706,117),(678,138),(45,138),(16,119),(24,89)])
a=np.minimum(np.maximum(a,face),allowed);a[155*scale:]=0
# Recover the tiny original crest tip above the initial staging crop, without repainting.
expandedBox=[113,103,856,276]
src=Image.open(json.loads((b/'Source.json').read_text())['source']).convert('RGB').crop(expandedBox)
size=(src.width*scale,src.height*scale);rgb=src.resize(size,Image.Resampling.LANCZOS)
a=np.pad(a,((4*scale,0),(0,0)))
cap=shape([(357,1),(350,6),(365,6)])
pixels=np.array(rgb).astype(float);gold=(pixels[:,:,0]>85)&(pixels[:,:,0]>pixels[:,:,2]*1.45)&(pixels[:,:,1]>45)
a[:4*scale]=np.maximum(a[:4*scale],np.where(gold,cap,0)[:4*scale])
final=rgb.convert('RGBA');final.putalpha(Image.fromarray(a));final.save(out/'title_original_ru.png')
native=src.convert('RGBA');native.putalpha(Image.fromarray(a).resize(src.size,Image.Resampling.LANCZOS));native.save(out/'title_original_ru_native.png')
assert np.array_equal(np.array(final)[:,:,:3],np.array(rgb));assert np.array_equal(np.array(native)[:,:,:3],np.array(src))
# Full lettering body remains opaque so source shading and lacquer show without holes.
assert a[73*scale:131*scale,65*scale:675*scale].min()==255
sheet=Image.new('RGB',(size[0],size[1]*3))
for i,color in enumerate([(0,0,0),(255,255,255),(200,0,150)]):
 tile=Image.new('RGBA',size,color+(255,));tile.alpha_composite(final);sheet.paste(tile.convert('RGB'),(0,i*size[1]))
sheet.save(qa/'TransparencyInspection.png')
record={'source':json.loads((b/'Source.json').read_text()),'output':'Extracted/title_original_ru.png','expandedOriginalBox':expandedBox,'originalRGBPreserved':True,'nativeOriginalRGBIdentical':True,'runtimeRGBEqualsLanczos2xOriginal':True,'originalRussianLetteringRetained':True,'opaqueLetteringBodyVerified':True,'cleanup':'Scenario alpha bounded to title contour; remove neighboring header/profile/timer; restore intentionally opaque plaque face; recover original crest tip in four-pixel source extension','repaintedRGBPixels':0,'billing':json.loads((b/'Scenario/title/Billing.json').read_text()),'sha256':hashlib.sha256((out/'title_original_ru.png').read_bytes()).hexdigest()}
(b.parent/'AssetProvenance.json').write_text(json.dumps(record,indent=2));print(record['billing'])
