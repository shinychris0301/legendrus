from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,uuid,shutil,hashlib
b=Path(__file__).resolve().parent;assets=b.parent/'Assets';src=Image.open(b/'OriginalReference.png').convert('RGBA')
a=Image.open(assets/'rewards.png').convert('RGBA');m=Image.open(b/'Scenario/rewards_frame/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(a.size,Image.Resampling.LANCZOS);ImageDraw.Draw(m).rectangle((22,24,a.width-22,a.height-26),fill=255);d=ImageDraw.Draw(m);d.rectangle((0,320,a.width,336),fill=0);d.polygon([(321,319),(349,319),(337,328)],fill=255);a.putalpha(m);a.save(assets/'rewards.png')
# Complete source silhouettes and panel edges after Scenario alpha processing.
import runpy
runpy.run_path(str(b/'RepairEdges.py'))
# Original visible landscape is preserved. Only the missing/covered interface areas use generated scenery.
gen=Image.open(b/'Scenario/environment/booster_medallion_trial01.png').convert('RGBA').resize(src.size,Image.Resampling.LANCZOS)
mask=Image.new('L',src.size);d=ImageDraw.Draw(mask)
for box in [(0,0,941,145),(0,0,180,238),(72,113,880,393),(201,376,744,741),(356,669,585,885),(114,812,829,1144),(116,1125,827,1471),(0,1430,941,1672)]:d.rectangle(box,fill=255)
mask=mask.filter(ImageFilter.MaxFilter(13)).filter(ImageFilter.GaussianBlur(5));Image.composite(gen,src,mask).convert('RGB').save(assets/'environment.png');mask.save(b/'EnvironmentRepairMask.png')
dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/PreLevelReferenceAligned');dst.mkdir(parents=True,exist_ok=True)
if not dst.with_suffix('.meta').exists():dst.with_suffix('.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n')
for p in assets.glob('*.png'):
 shutil.copyfile(p,dst/p.name)
 meta=dst/(p.name+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n')
# Three-background QA contact sheet from final alpha assets.
names=[p.stem for p in assets.glob('*.png') if p.stem!='environment'];out=Image.new('RGB',(1200,1200));draw=ImageDraw.Draw(out)
for col,bg in enumerate(['#000000','#ffffff','#ef1abd']):
 y=0
 for n in names:
  a=Image.open(assets/(n+'.png')).convert('RGBA');a.thumbnail((376,105));out.paste(bg,(col*400,y,col*400+400,y+125));out.paste(a,(col*400+10,y+16),a);draw.text((col*400+10,y),n,fill='#888888');y+=125
out.save(b.parent/'QA/AlphaFinal.jpg')
(b/'AssetProvenance.json').write_text(json.dumps({'original':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'workflow':'Scenario alpha plus original RGB; bounded opaque-surface and numeral repair; hidden scenery reconstruction only','jobs':11,'cuCost':sum(json.loads(p.read_text()).get('cuCost',0) for p in (b/'Scenario').glob('*/Billing.json')),'sourceSceneryPixelsUnchanged':int((np.array(mask)==0).sum()),'assets':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in dst.glob('*.png')},'limitations':['Panel content faces reconstructed from a clean original navy patch','Complete booster silhouettes retraced from original pixels; reward top closure reconstructed from original rail','Hidden scenery is inferred, not recovered','Booster example badges covered by larger live count badges']},indent=2))
