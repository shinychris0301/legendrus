from pathlib import Path
from PIL import Image, ImageChops, ImageDraw
import hashlib,json

root=Path.cwd();base=Path(__file__).resolve().parent;qa=base.parent/'QA';qa.mkdir(exist_ok=True)
out=root/'Assets/LegendsOfRus/Resources/ProductionUI/RewardReceivedAligned';out.mkdir(exist_ok=True)
records=[]
for kind in ['panel','chest']:
    source=base/'Scenario/panel_repair/booster_medallion_trial01.png' if kind=='panel' else base/'chest_source.png'
    mask=base/'Scenario'/('panel_repair_alpha' if kind=='panel' else 'chest')/'booster_medallion_trial01.png'
    rgb=Image.open(source).convert('RGB');alpha=Image.open(mask).convert('RGBA').getchannel('A')
    assert rgb.size==alpha.size
    im=rgb.convert('RGBA');im.putalpha(alpha)
    bounds=(0,0,*im.size)
    if kind=='panel':
        x0,y0,x1,y1=alpha.getbbox();bounds=(x0-4,y0-4,x1+4,y1+4);im=im.crop(bounds)
    dest=out/(kind+'.png');im.save(dest)
    assert ImageChops.difference(im.convert('RGB'),rgb.crop(bounds)).getbbox() is None
    if kind=='panel':assert im.getchannel('A').crop((100,150,im.width-100,im.height-100)).getextrema()==(255,255)
    preview=im.copy();preview.thumbnail((400,540))
    sheet=Image.new('RGB',(1200,580));d=ImageDraw.Draw(sheet)
    for i,(label,color) in enumerate([('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(220,0,150))]):
        sheet.paste(color,(i*400,0,(i+1)*400,580));sheet.paste(preview,(i*400+(400-preview.width)//2,30),preview);d.text((i*400+10,8),label,fill=(128,128,128))
    sheet.save(qa/(kind+'_alpha.png'))
    records.append({'asset':str(dest.relative_to(root)),'rgbSource':str(source.relative_to(root)),'alphaSource':str(mask.relative_to(root)),'sourcePixelBounds':bounds,'size':im.size,'originalRGBPreserved':True,'sha256':hashlib.sha256(dest.read_bytes()).hexdigest()})
(qa/'AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
