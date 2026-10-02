from pathlib import Path
from PIL import Image,ImageChops,ImageDraw,ImageFilter
import json,hashlib
root=Path.cwd();base=Path(__file__).resolve().parent;qa=base.parent/'QA';qa.mkdir(exist_ok=True)
out=root/'Assets/LegendsOfRus/Resources/ProductionUI/OutOfEnergyAligned';out.mkdir(exist_ok=True)
records=[]
def scenario(kind):return base/'Scenario'/kind/'booster_medallion_trial01.png'
def store(key,rgb,alpha,source,operation):
    im=rgb.convert('RGBA');im.putalpha(alpha)
    assert ImageChops.difference(im.convert('RGB'),rgb.convert('RGB')).getbbox() is None
    dest=out/(key+'.png');im.save(dest)
    records.append({'asset':str(dest.relative_to(root)),'rgbSource':str(source.relative_to(root)),'size':im.size,'alphaOperation':operation,'rgbPreserved':True,'sha256':hashlib.sha256(dest.read_bytes()).hexdigest()})
    preview=im.copy();preview.thumbnail((360,500));sheet=Image.new('RGB',(1080,540));d=ImageDraw.Draw(sheet)
    for i,(label,color) in enumerate([('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]):
        sheet.paste(color,(i*360,0,(i+1)*360,540));sheet.paste(preview,(i*360+(360-preview.width)//2,30),preview);d.text((i*360+10,8),label,fill=(128,128,128))
    sheet.save(qa/(key+'_alpha.png'))
rgb=Image.open(scenario('panel')).convert('RGB');alpha=Image.open(scenario('panel_alpha')).convert('RGBA').getchannel('A');x0,y0,x1,y1=alpha.getbbox();box=(x0-3,y0-3,x1+3,y1+3)
assert alpha.crop((150,180,850,1150)).getextrema()==(255,255)
store('panel',rgb.crop(box),alpha.crop(box),scenario('panel'),'Scenario alpha; transparent exterior trimmed with 3px padding; opaque navy retained')
rgb=Image.open(scenario('controls')).convert('RGB');alpha=Image.open(scenario('controls_alpha')).convert('RGBA').getchannel('A')
# Scenario retained source-colored navy rectangles. Keep the gold outer ornament
# and intentional opaque center, removing only the outside background pixels.
box=(277,73,738,397);r=rgb.crop(box);a=alpha.crop(box)
gold=Image.new('L',r.size);gold.putdata([255 if rr>65 and rr>bb*1.5 and gg>bb*1.12 and rr>gg*1.05 else 0 for rr,gg,bb in r.getdata()])
gold=gold.filter(ImageFilter.MaxFilter(3));center=Image.new('L',r.size);ImageDraw.Draw(center).ellipse((95,39,358,291),fill=255)
mask=ImageChops.lighter(gold,center).filter(ImageFilter.GaussianBlur(.35));a=ImageChops.multiply(a,mask)
store('medallion',r,a,scenario('controls'),'Scenario alpha intersected with local gold silhouette and opaque central medallion ellipse; source RGB unchanged')
for key,box,points in [
 ('green',(96,601,389,723),[(116,605),(362,605),(384,625),(384,697),(363,719),(116,719),(98,700),(98,625)]),
 ('blue',(612,601,907,710),[(636,605),(882,605),(903,625),(903,685),(883,706),(636,706),(616,686),(616,625)])]:
    mask=Image.new('L',rgb.size);ImageDraw.Draw(mask).polygon(points,fill=255);mask=mask.filter(ImageFilter.GaussianBlur(.4));store(key,rgb.crop(box),ImageChops.multiply(alpha,mask).crop(box),scenario('controls'),'Scenario alpha plus beveled contour mask; removed neighboring row-rail fragments')
info=json.loads((base/'SourceBounds.json').read_text());rgb=Image.open(base/'icons_source.png').convert('RGB');alpha=Image.open(scenario('icons')).convert('RGBA').getchannel('A')
for key,origin in info['iconsPositions'].items():
    x,y=origin;bounds=info['iconsBounds'][key];w,h=bounds[2]-bounds[0],bounds[3]-bounds[1];r=rgb.crop((x,y,x+w,y+h));a=alpha.crop((x,y,x+w,y+h))
    mask=Image.new('L',(w,h));d=ImageDraw.Draw(mask)
    if key=='close':d.ellipse((2,4,76,79),fill=255)
    elif key=='friends':d.ellipse((7,7,94,95),fill=255)
    elif key=='ad':
        d.polygon([(16,15),(80,0),(84,13),(21,29),(82,28),(82,82),(17,82)],fill=255)
    else:
        # Hourglass outer rails enclose intentionally dark glass; no reference scenery.
        d.polygon([(6,0),(52,0),(52,8),(44,10),(43,20),(32,31),(43,43),(44,49),(53,52),(53,60),(3,60),(3,52),(12,48),(13,42),(23,30),(14,20),(13,10),(6,8)],fill=255)
    a=ImageChops.multiply(a,mask.filter(ImageFilter.GaussianBlur(.35)));store(key,r,a,base/'icons_source.png','Scenario alpha intersected with local icon contour; original reference RGB retained')
(qa/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
