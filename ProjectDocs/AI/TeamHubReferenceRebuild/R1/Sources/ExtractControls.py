"""Keep source nav/card borders and Russian lettering; reuse approved blank lacquer."""
from pathlib import Path
import hashlib,json,re,uuid
from PIL import Image,ImageDraw
import numpy as np

b=Path(__file__).resolve().parents[1]
art=Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamHubAligned')
ref=Image.open(b/'Sources/OriginalReference.png').convert('RGBA')
donor=Image.open(art/'nav_frame.png').convert('RGB').crop((330,80,570,200))
records=[];assets=[]
def polygon(w,h,points):
    a=Image.new('L',(w*4,h*4));ImageDraw.Draw(a).polygon([(x*4,y*4) for x,y in points],fill=255)
    return a.resize((w,h),Image.Resampling.LANCZOS)
def save(key,im,box,repair=None):
    path=art/(key+'.png');im.save(path);meta=Path(str(path)+'.meta')
    if not meta.exists():
        s=(art/'power.png.meta').read_text();s=re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,s,count=1);meta.write_text(s)
    records.append({'asset':str(path),'bounds':box,'repair':repair,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()});assets.append((key,im))
boxes=[(33,894,248,1028),(252,894,458,1028),(465,894,659,1028),(667,894,908,1028)]
for i,box in enumerate(boxes):
    im=ref.crop(box);w,h=im.size
    mask=polygon(w,h,[(19,3),(w-24,3),(w-7,16),(w-3,35),(w-3,h-27),(w-20,h-7),(w-40,h-2),(23,h-2),(3,h-20),(2,27)])
    im.putalpha(mask);assert np.array_equal(np.asarray(im)[:,:,:3],np.asarray(ref.crop(box))[:,:,:3])
    save('nav_'+str(i)+'_ru',im,box)
    # Keep each original icon and frame. Only the label footprint is replaced for English.
    blank=im.copy();patch=donor.resize((w-33,39),Image.Resampling.LANCZOS)
    if i==3:
        a=np.asarray(patch).astype(float);a[:,:,0]*=.6;a[:,:,1]*=1.5;a[:,:,2]*=.65;patch=Image.fromarray(np.clip(a,0,255).astype('uint8'))
    blank.paste(patch,(17,84));blank.putalpha(mask)
    save('nav_'+str(i)+'_blank',blank,box,{'bounds':[17,84,w-16,123],'method':'approved blank lacquer; original icon and surrounding border unchanged'})

box=(412,425,906,539);im=ref.crop(box);w,h=im.size
mask=polygon(w,h,[(20,2),(w-4,2),(w-2,h-3),(19,h-3),(3,h-18),(3,19)])
im.paste(donor.resize((w-32,h-28),Image.Resampling.LANCZOS),(16,14))
# The adjacent chest occludes the lower-right rail. Continue the visible central rail,
# rather than shipping its gold lid as part of this independent panel.
rail=ref.crop((530,525,630,539))
for x in range(310,w,rail.width):im.paste(rail.crop((0,0,min(rail.width,w-x),14)),(x,100))
# Rebuild the occluded right cap from the visible left cap, removing a scenery sliver.
im.paste(im.crop((0,0,16,h)).transpose(Image.Transpose.FLIP_LEFT_RIGHT),(w-16,0))
im.putalpha(mask)
save('power_frame_r2',im,box,{'bounds':[16,14,w-16,h-14],'method':'approved opaque lacquer replaces fixed power/icon footprints; lower-right occluded rail continued from source x530–630 y525–539; right cap mirrored from visible left cap'})

box=(369,589,640,625);im=ref.crop(box);w,h=im.size
mask=polygon(w,h,[(20,1),(w-19,1),(w-2,14),(w-2,23),(w-18,h-2),(22,h-2),(2,20),(2,15)])
# Make the channel transparent for native progress geometry and live text.
ImageDraw.Draw(mask).rectangle((19,7,w-20,h-8),fill=0)
a=np.array(im);rgb=a[:,:,:3].astype(np.int16);blue=(rgb[:,:,2]>rgb[:,:,0]+15)&(rgb[:,:,2]>rgb[:,:,1]+5)
blue &= np.array(mask)>0
a[blue,:3]=[7,20,28]
im=Image.fromarray(a);im.putalpha(mask);save('progress_rail_r2',im,box,{'bounds':[19,7,w-20,h-8],'method':'transparent channel; baked blue fill fragments cleared to navy; native fill and live count'})

sheet=Image.new('RGB',(930,len(assets)*165))
draw=ImageDraw.Draw(sheet)
for row,(name,im) in enumerate(assets):
    for col,color in enumerate(('black','white','#ff00dd')):
        tile=Image.new('RGBA',(310,145),color);show=im.copy();show.thumbnail((300,138));tile.alpha_composite(show,((310-show.width)//2,(145-show.height)//2));sheet.paste(tile.convert('RGB'),(col*310,row*165))
    draw.text((5,row*165+146),name,fill='white')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps({'sourceSHA256':hashlib.sha256((b/'Sources/OriginalReference.png').read_bytes()).hexdigest(),'scenarioJobs':0,'creditsUsed':0,'assets':records,'limitations':'Hand-traced exterior alpha and reconstructed hidden English/power surfaces are estimates.'},indent=2)+'\n')
