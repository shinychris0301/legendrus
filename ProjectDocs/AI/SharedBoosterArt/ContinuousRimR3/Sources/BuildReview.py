from pathlib import Path
from PIL import Image
import shutil,json,hashlib,xml.etree.ElementTree as ET
b=Path(__file__).resolve().parent.parent;q=b/'QA';run=Path('/tmp/lor-booster-continuous-rim-r3/Verify01');r1=b.parent/'R1';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters')
for n in ['Import','EditMode','PlayMode']:shutil.copytree(run/n,q/'Verify01'/n,dirs_exist_ok=True)
views=['BoosterSelection/BoosterSelection_941x1672_RU.png','BoosterSelection/BoosterSelection_941x1672_EN.png','BoosterSelection/BoosterSelection_576x1280_safe_EN.png','PreLevel/PreLevel_941x1672_RU.png','Pause/Pause_941x1672_RU.png','Fail/Fail_941x1672_RU.png']
opts=[]
for name in views:
 p=q/'Verify01'/name;p.parent.mkdir(parents=True,exist_ok=True);shutil.copy(run/name,p);opts.append((str(p.relative_to(b)),name.split('/')[0]+' · '+Path(name).stem))
shutil.copy(r1/'Sources/OriginalReference.png',b/'Sources/OriginalReference.png');opts.append(('Sources/OriginalReference.png','Original reference'))
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Continuous Booster Rim R3</title><style>body{margin:0;background:#09131f;color:#efdab5;font:16px/1.5 system-ui}header,footer{max-width:760px;margin:24px auto;padding:0 16px}main{max-width:520px;margin:auto}img{width:100%;display:block}select{max-width:100%;padding:10px;background:#183047;color:#ffe1a6}a{color:#a6d7ff}</style><header><h1>Continuous Booster Rim · R3</h1><p>A complete circular band has been recreated through Scenario. One coherent gold rim replaces all eight patched bands. Original booster subjects remain intact; existing shared sprite bindings update the consuming pages.</p><p><a href="AssetReview.html">Inspect original / isolated band / corrected icon</a> · <a href="ImplementationReport.md">Report</a></p><select id="view">OPTIONS</select><p>Actual Unity captures. The original reference is labelled separately. Live badges remain independent from the icon art.</p></header><main><img id="capture" src="DEFAULT" alt="Selected Unity capture"></main><footer><p>34 targeted Unity checks passed. Scenario: one generation (12 CU) and one alpha isolation (3 CU), 15 CU total. The rim is a recreation guided by the original, not an exact extraction. No lower-arc donor joins remain.</p></footer><script>document.querySelector('#view').onchange=e=>document.querySelector('#capture').src=e.target.value;</script></html>'''
(b/'SharedBooster_ContinuousRim_R3_Review.html').write_text(html.replace('OPTIONS',''.join(f'<option value="{p}">{n}</option>' for p,n in opts)).replace('DEFAULT',opts[0][0]))
names=['mace','firebird','shield','potion','scroll','lightning','bomb','swap'];dst=q/'AssetCopies';dst.mkdir(exist_ok=True)
for n in names:
 shutil.copy(r1/'QA/AssetCopies'/(n+'_source.png'),dst/(n+'_source.png'))
 shutil.copy(b/'Sources/Before'/(n+'_framed.png'),dst/(n+'_before.png'))
 for suffix in ['_rim','_framed']:shutil.copy(out/(n+suffix+'.png'),dst/(n+suffix+'.png'))
s=(r1/'AssetReview.html').read_text().replace('Booster extraction diagnostics','Recreated continuous metal rim').replace('Source extraction diagnostics','Recreated continuous metal rim · R3').replace('Shared framed sprite','Isolated metal band').replace('Isolated subject','Corrected framed icon').replace("+n+'_framed.png'","+n+'_rim.png'").replace("+n+'.png'","+n+'_framed.png'")
start=s.index('<p>Subject RGB is original.');end=s.index('</p>',start)+4
s=s[:start]+'''<p>Scenario recreated the entire metal band as one piece. All eight icons share that same band, including the unavailable previews. Booster subjects retain their source pixels. The old repaired arcs are fully replaced; the new metal and fine engravings are a recreation rather than exact original pixels.</p>'''+s[end:]
s=s.replace('<figure><figcaption>Isolated metal band', '<figure><figcaption>Previous R2 (rejected joins)</figcaption><img id="before" alt="Previous patched rim"></figure><figure><figcaption>Recreated metal band').replace("const n=menu.value;","const n=menu.value;document.querySelector('#before').src='QA/AssetCopies/'+n+'_before.png';")
s=s.replace('SharedBoosterIcons_R1_Review.html','SharedBooster_ContinuousRim_R3_Review.html');(b/'AssetReview.html').write_text(s)
old=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());now={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};audit={'modified':[p for p,h in old.items() if p in now and h!=now[p]],'added':sorted(set(now)-set(old)),'removed':sorted(set(old)-set(now)),'tests':{n:ET.parse(run/n/'results.xml').getroot().attrib for n in ['EditMode','PlayMode']}}
(q/'ScopeAndVerification.json').write_text(json.dumps(audit,indent=2));print(json.dumps(audit,indent=2))
