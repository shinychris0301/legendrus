from pathlib import Path
import json, shutil, hashlib
from PIL import Image, ImageDraw
root = Path.cwd()
b = Path(__file__).resolve().parent.parent
home = root / 'Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned'
previous = b.parent / 'DesignR1'
assets = b / 'Assets'
records = []
def copy(src, name):
    shutil.copyfile(src, assets / name)
    records.append({'asset': name, 'source': str(src.relative_to(root)), 'sha256': hashlib.sha256(src.read_bytes()).hexdigest(), 'operation': 'unchanged copy'})
for name in ['source_avatar','source_gold','source_gems','source_energy','source_plus','source_settings',
             'source_map_ru','source_shop_ru','source_events_ru','source_characters_ru','source_footer_carving','clean_environment']:
    copy(home / (name+'.png'), name+'.png')
for name in ['title.png','back.png','lock.png','Body.ttf','Numbers.ttf']:
    copy(previous / 'Assets' / name, name)
copy(root/'Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned/footer_backing_r8.png','footer_backing.png')
# The original Home strip begins at x174. Extend its existing opaque lacquer and
# rails behind the separate portrait, without altering its visible right-hand artwork.
header = Image.open(home/'source_header.png').convert('RGBA')
joined = header.copy()
donor = header.crop((174,0,248,140)).transpose(Image.Transpose.FLIP_LEFT_RIGHT)
joined.alpha_composite(donor,(100,0))

joined.save(assets/'header_joined.png')
records.append({'asset':'header_joined.png','source':str((home/'source_header.png').relative_to(root)),
                'operation':'existing 74-pixel lacquer/rail donor mirrored behind avatar; original x174+ pixels unchanged'})
assert joined.crop((174,0,941,140)).tobytes()==header.crop((174,0,941,140)).tobytes()
number = b/'Sources/ScenarioMedallion/booster_medallion_trial01.png'
if number.exists():
    im=Image.open(number);im.load()
    copy(number,'number_medallion.png')
# Accepted paintings are referenced from R1, never regenerated or modified.
records += [{'asset':f'../DesignR1/Assets/region_{i:02}.png','sha256':hashlib.sha256((previous/f'Assets/region_{i:02}.png').read_bytes()).hexdigest(),'operation':'unchanged accepted painting'} for i in range(1,21)]
(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,ensure_ascii=False,indent=2))
for color,label in [('#000000','Black'),('#ffffff','White'),('#ff00bb','Magenta')]:
    sheet=Image.new('RGB',(1000,940),color)
    for name,box in [('header_joined.png',(0,0,941,140)),('source_avatar.png',(0,0,174,235)),('title.png',(200,160,561,184)),('source_map_ru.png',(20,380,208,236)),('source_shop_ru.png',(245,380,205,236)),('source_events_ru.png',(470,380,201,236)),('source_characters_ru.png',(695,380,207,236)),('back.png',(30,690,110,110)),('lock.png',(195,690,80,100)),('number_medallion.png',(360,665,200,200))]:
        p=assets/name
        if not p.exists():continue
        im=Image.open(p).convert('RGBA');im.thumbnail((box[2],box[3]),Image.Resampling.LANCZOS)
        sheet.paste(im,(box[0],box[1]),im)
    sheet.save(b/f'QA/Alpha_{label}.png')
