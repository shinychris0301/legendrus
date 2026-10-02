from pathlib import Path
import json,shutil,hashlib,sys,html
import xml.etree.ElementTree as ET
b=Path(__file__).resolve().parents[1];run=Path('/tmp/lor-friends-footer-r6')/sys.argv[1];out=b/'UnityCaptures';out.mkdir(exist_ok=True)
for p in (run/'FriendsList').glob('*'):shutil.copy2(p,out/p.name)
for old in run.parent.glob('Verify*'):
 if not old.is_dir():continue
 for mode in ('Import','EditMode','PlayMode'):
  if (old/mode).exists():shutil.copytree(old/mode,b/'QA'/old.name/mode,dirs_exist_ok=True)
shutil.copytree(run/'WorldMap',b/'QA/WorldMapRegression',dirs_exist_ok=True)
choices=['FriendsList_1080x2400_safe_RU.png','FriendsList_941x1672_RU.png','FriendsList_941x1672_EN.png','FriendsList_576x1280_safe_RU.png','FriendsList_1080x2400_safe_EN.png','FriendsList_Requests_RU.png']
assert all((out/n).exists() for n in choices)
options=''.join('<option value="UnityCaptures/'+n+'">'+html.escape(n[:-4])+'</option>' for n in choices)
options += '<option value="../FrameAndFooterR5/UnityCaptures/FriendsList_1080x2400_safe_RU.png">Before R6 — tall Russian</option><option value="../FrameAndFooterR5/UnityCaptures/FriendsList_941x1672_RU.png">Before R6 — reference size</option><option value="QA/WorldMapRegression/WorldMap_1080x2400_safearea_RU.png">World Map — verified footer</option>'
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Friends List — World Map Footer R6</title><style>body{background:#09111e;color:#efdcb4;font:16px/1.5 system-ui;max-width:920px;margin:24px auto;padding:0 16px}h1{font-size:26px}a{color:#a1d3f8}select{background:#17273b;color:#efdcb4;padding:10px;max-width:100%}img{display:block;width:min(100%,471px);height:auto;margin:24px auto;border:1px solid #78603b}</style><h1>Friends List — World Map Footer R6</h1><p>Footer metalwork now comes directly from World Map. Shop retains its complete original face; Map, Events, Friends and Characters use approved center emblems inside the World Map rims. Only Friends uses the blue active frame. Captions are live and localized, and all five navigation routes remain functional.</p><p>Actual Unity Play Mode captures with isolated saves and simulated phone sizes/safe areas. Choose Before R6 to compare. This is a capture viewer, not an interactive build.</p><label>View <select id="view">OPTIONS</select></label><img id="page" src="UnityCaptures/FriendsList_1080x2400_safe_RU.png" alt="Implemented Friends List"><p><a href="ImplementationReport.md">Changes and validation</a> · <a href="QA/ScopeAndVerification.json">Verification results</a></p><script>document.querySelector('#view').onchange=e=>document.querySelector('#page').src=e.target.value;</script></html>'''.replace('OPTIONS',options)
(b/'FriendsList_WorldMapFooter_R6_Review.html').write_text(page)
before=json.loads((b/'Sources/AssetsBefore.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};record={'modified':sorted(p for p in before.keys()&after.keys() if before[p]!=after[p]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{},'run':str(run)}
for mode in ('EditMode','PlayMode'):
 r=ET.parse(run/mode/'results.xml').getroot();record['checks'][mode]={k:r.get(k) for k in ('result','passed','failed','skipped','total')}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2))
print(json.dumps(record,indent=2))
