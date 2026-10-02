"""Original RU fixed labels + artwork. Reuse Scenario R11 masks; no API call.
RGB is byte-identical to the source crop. Only alpha is extracted/adjusted.
"""
from pathlib import Path
import hashlib,json
from PIL import Image,ImageDraw
import numpy as np
B=Path(__file__).resolve().parents[1];ROOT=B.parents[3]
SOURCE=ROOT/'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Fail.png'
ART=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/FailAligned'
OUT=B/'Staging';OUT.mkdir(exist_ok=True)
source=Image.open(SOURCE).convert('RGB')
entries=[('title_ru_r12',(123,523,818,768),'title_r11'),('retry_ru_r12',(155,1023,786,1137),'retry_r11'),
('name_mace_ru_r12',(85,1520,276,1575),'nameplate_r11'),('name_firebird_ru_r12',(266,1520,470,1575),'nameplate_r11'),
('name_shield_ru_r12',(463,1520,673,1575),'nameplate_r11'),('name_potion_ru_r12',(667,1520,863,1575),'nameplate_r11')]
records=[]
for name,box,base in entries:
    rgb=source.crop(box);alpha=Image.open(ART/(base+'.png')).getchannel('A').resize(rgb.size,Image.Resampling.LANCZOS)
    if name.startswith('name_'):
        # Keep R11's Scenario-derived panel mask, scaled only to the native variant
        # width. All original glyphs lie in the unchanged opaque central face.
        inside=Image.new('L',rgb.size);d=ImageDraw.Draw(inside);w,h=rgb.size
        d.polygon([(15,9),(w-26,9),(w-12,21),(w-12,33),(w-26,43),(15,43),(8,33),(8,21)],fill=255)
        alpha=Image.fromarray(np.maximum(np.array(alpha),np.array(inside)))
    # The two middle plaques have different widths/end caps from the first one.
    # Correct the reused mask to their actual gold outline; keep all source RGB.
    outlines={
      'name_firebird_ru_r12':[(17,3),(94,3),(99,0),(102,3),(182,3),(184,8),(193,16),(193,23),(197,25),(193,28),(193,35),(184,43),(182,47),(17,47),(15,41),(6,34),(5,14),(15,7)],
      'name_shield_ru_r12':[(22,3),(97,3),(100,0),(106,3),(188,3),(190,8),(202,15),(202,21),(206,25),(202,28),(202,35),(190,43),(188,47),(22,47),(20,42),(10,35),(10,29),(3,25),(10,21),(10,14),(20,7)]}
    if name in outlines:
        mask=Image.new('L',(rgb.width*4,rgb.height*4));ImageDraw.Draw(mask).polygon([(x*4,y*4) for x,y in outlines[name]],fill=255)
        alpha=mask.resize(rgb.size,Image.Resampling.LANCZOS)
    out=rgb.convert('RGBA');out.putalpha(alpha);out.save(OUT/(name+'.png'))
    assert np.array_equal(np.array(out)[:,:,:3],np.array(rgb))
    records.append({'name':name,'sourceBox':box,'size':rgb.size,'maskSource':str((ART/(base+'.png')).relative_to(ROOT)),
        'sourceRgbByteIdentical':True,'changedRgbPixels':0,'sha256':hashlib.sha256((OUT/(name+'.png')).read_bytes()).hexdigest()})
(B/'QA/ExtractionAudit.json').write_text(json.dumps({'source':str(SOURCE.relative_to(ROOT)),'sourceSha256':hashlib.sha256(SOURCE.read_bytes()).hexdigest(),'newScenarioRequests':0,'newCreativeUnits':0,'assets':records},indent=2)+'\n')
for color,label in [('black','Black'),('white','White'),('#ff00ff','Magenta')]:
    canvas=Image.new('RGB',(760,780),color);d=ImageDraw.Draw(canvas);y=4
    for entry in records:
        im=Image.open(OUT/(entry['name']+'.png'));d.text((4,y),entry['name'],fill='#888888');y+=16
        canvas.paste(im,(4,y),im);y+=im.height+10
    canvas.crop((0,0,760,y)).save(B/'QA'/('AlphaOn'+label+'.png'))
print('Six sprites prepared; all source RGB unchanged; no new API requests.')
