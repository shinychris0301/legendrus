from pathlib import Path
from PIL import Image,ImageFilter
import numpy as np,cv2,json,hashlib,shutil
r=Path.cwd();b=Path(__file__).resolve().parent.parent
src=r/'ProjectDocs/AI/WorldMapReferenceRebuild/NavigationAndRegionsR2/Assets'
dst=r/'Assets/LegendsOfRus/Resources/ProductionUI/WorldMapReferenceAligned'
baseline=b/'Sources/BeforeAssetsHashes.json'
if not baseline.exists():baseline.write_text(json.dumps({str(p.relative_to(r)):hashlib.sha256(p.read_bytes()).hexdigest() for p in (r/'Assets').rglob('*')if p.is_file()}))
dst.mkdir(exist_ok=True)
names=['reference_environment','header_joined','chapter_title','card_frame','card_surface','lock_source','progress_plate','continue_source','map_button_source','back_source','footer_backing']+['nav_'+n for n in ['home','map','tasks','guild','shop']]
for n in names:shutil.copyfile(src/(n+'.png'),dst/(n+'.png'))
# Border glow derives exclusively from the frame alpha. The card interior stays empty.
frame=Image.open(src/'card_frame.png').convert('RGBA');alpha=frame.getchannel('A')
glow=Image.new('RGBA',frame.size,(255,190,55,0));glow.putalpha(alpha.filter(ImageFilter.GaussianBlur(3)));glow.save(dst/'card_border_glow.png')
repairs={'chapter_title':(80,96,563,165),'back_source':(43,37,171,66),'map_button_source':(140,43,360,96),'continue_source':(40,20,223,56)}
for n in ['home','map','tasks','guild','shop']:
    im=Image.open(src/f'nav_{n}.png');repairs['nav_'+n]=(34,169,im.width-27,195)
for n,box in repairs.items():
    im=Image.open(src/(n+'.png')).convert('RGBA');rgb=np.array(im.convert('RGB'));mask=np.zeros(rgb.shape[:2],np.uint8);x0,y0,x1,y1=box;mask[y0:y1,x0:x1]=255
    repaired=Image.fromarray(cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)).convert('RGBA');repaired.putalpha(im.getchannel('A'));repaired.save(dst/(n+'_blank.png'))
for i in range(1,21):
    original=Image.open(r/f'ProjectDocs/AI/WorldMapReferenceRebuild/DesignR1/Assets/region_{i:02}.png').convert('RGBA')
    for suffix,height in [('',199),('_current',239)]:
        width=round(original.height*445/height);left=(original.width-width)//2
        im=original.crop((left,0,left+width,original.height))
        x=np.arange(im.width)/max(1,im.width-1);mask=np.clip((1-x)/.40,0,1)*255
        im.putalpha(Image.fromarray(np.repeat(mask[np.newaxis,:],im.height,axis=0).astype(np.uint8)))
        im.save(dst/f'region_{i:02}{suffix}.png')

chapters=json.loads((r/'ProjectDocs/AI/WorldMapReferenceRebuild/DesignR1/Sources/Chapters.json').read_text())
(dst/'chapters.json').write_text(json.dumps({'chapters':chapters},ensure_ascii=False,indent=2))
(b/'Sources/AssetManifest.json').write_text(json.dumps({'reuse':'Approved R2 source extraction and R1 Scenario paintings; no new API calls','operations':['separate alpha-only border glow','local English blank face repairs','center cover-crop for normal/current aspect ratios with alpha taper; original visible RGB retained'],'files':{p.name:hashlib.sha256(p.read_bytes()).hexdigest()for p in dst.glob('*')if p.is_file()and p.suffix!='.meta'}},indent=2))
print('Prepared native World Map resources')
