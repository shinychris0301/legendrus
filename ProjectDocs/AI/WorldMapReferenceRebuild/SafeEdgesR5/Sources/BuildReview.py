from pathlib import Path
import json,shutil,hashlib,sys,html
import xml.etree.ElementTree as ET
b=Path(__file__).resolve().parents[1];run=Path('/tmp/lor-worldmap-safeedges-r5')/sys.argv[1];out=b/'UnityCaptures';out.mkdir(exist_ok=True)
for p in (run/'WorldMap').glob('*'):shutil.copy2(p,out/p.name)
for old in run.parent.glob('Verify*'):
 if not old.is_dir():continue
 for mode in ('Import','EditMode','PlayMode'):
  if (old/mode).exists():shutil.copytree(old/mode,b/'QA'/old.name/mode,dirs_exist_ok=True)
shutil.copytree(run/'Home',b/'QA/HomeRegression',dirs_exist_ok=True)
choices=['WorldMap_1080x2400_safearea_RU.png','WorldMap_941x1672_RU.png','WorldMap_941x1672_EN.png','WorldMap_576x1280_safearea_RU.png','WorldMap_1080x2400_safearea_EN.png','WorldMap_Resize_50.png','WorldMap_Resize_100.png']
assert all((out/n).exists() for n in choices)
options=''.join('<option value="UnityCaptures/'+n+'">'+html.escape(n[:-4])+'</option>' for n in choices)
options += '<option value="../../NavigationResponsiveAudit/R1/Captures/WorldMap_1080x2400_safearea_RU.png">Before R5 — tall Russian</option><option value="../../NavigationResponsiveAudit/R1/Captures/WorldMap_941x1672_RU.png">Before R5 — reference size</option>'
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>World Map — Safe Edges R5</title><style>body{background:#09111e;color:#efdcb4;font:16px/1.5 system-ui;max-width:920px;margin:24px auto;padding:0 16px}h1{font-size:26px}a{color:#a1d3f8}select{background:#17273b;color:#efdcb4;padding:10px;max-width:100%}img{display:block;width:min(100%,471px);height:auto;margin:24px auto;border:1px solid #78603b}</style><h1>World Map — Safe Edges R5</h1><p>Shared header and title follow the safe top; Back, chapter action and navigation follow the safe bottom. The masked chapter viewport expands between them, preserving artwork size, scroll progress and later-chapters-above ordering.</p><p>Actual Unity Play Mode captures with isolated saves and simulated phone sizes/safe areas. Choose Before R5 to compare. This is a capture viewer, not an interactive build.</p><label>View <select id="view">OPTIONS</select></label><img id="page" src="UnityCaptures/WorldMap_1080x2400_safearea_RU.png" alt="Implemented World Map"><p><a href="ImplementationReport.md">Changes and validation</a> · <a href="QA/ScopeAndVerification.json">Verification results</a></p><script>document.querySelector('#view').onchange=e=>document.querySelector('#page').src=e.target.value;</script></html>'''.replace('OPTIONS',options)
(b/'WorldMap_SafeEdges_R5_Review.html').write_text(page)
before=json.loads((b/'QA/BeforeAssetsHashes.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};record={'modified':sorted(p for p in before.keys()&after.keys() if before[p]!=after[p]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{},'run':str(run)}
for mode in ('EditMode','PlayMode'):
 r=ET.parse(run/mode/'results.xml').getroot();record['checks'][mode]={k:r.get(k) for k in ('result','passed','failed','skipped','total')}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2))
print(json.dumps(record,indent=2))
