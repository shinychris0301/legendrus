from pathlib import Path
import json,shutil,hashlib,sys,html
import xml.etree.ElementTree as ET
b=Path(__file__).resolve().parents[1];run=Path('/tmp/lor-characters-safeedges-r3')/sys.argv[1];out=b/'UnityCaptures';out.mkdir(exist_ok=True)
for p in (run/'Characters').glob('*'):shutil.copy2(p,out/p.name)
for old in sorted(list(run.parent.glob('Verify*'))+list(run.parent.glob('Baseline*'))):
 for mode in ('Import','EditMode','PlayMode'):
  if(old/mode).exists():shutil.copytree(old/mode,b/'QA'/old.name/mode,dirs_exist_ok=True)
beforeOut=b/'BeforeCaptures';beforeOut.mkdir(exist_ok=True)
for p in (Path('ProjectDocs/AI/CharactersReferenceRebuild/OriginalLetteringR2/UnityCaptures')).glob('*'):shutil.copy2(p,beforeOut/p.name)
choices=['Characters_1080x2400_safe_RU.png']+[p.name for p in sorted(out.glob('*.png')) if p.name!='Characters_1080x2400_safe_RU.png'];assert len(choices)>=6
options=''.join('<option value="UnityCaptures/'+n+'">Unity · '+html.escape(n[:-4])+'</option>' for n in choices)
options+='<option value="Sources/Reference.png">Supplied original reference</option><option value="BeforeCaptures/Characters_1080x2400_safe_RU.png">Before · Previous Characters / Hero Collection</option>'
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Characters / Hero Collection — Safe Edges R3</title><style>body{background:#09111e;color:#efdcb4;font:16px/1.5 system-ui;max-width:920px;margin:24px auto;padding:0 16px}h1{font-size:26px}a{color:#a1d3f8}select{background:#17273b;color:#efdcb4;padding:10px;max-width:100%}img{display:block;width:min(100%,471px);height:auto;margin:24px auto;border:1px solid #78603b}</style><h1>Characters / Hero Collection — Safe Edges R3</h1><p>Header and footer follow the safe screen edges. Taller phones show more collection cards while preserving the original art proportions and lettering.</p><p>Views below are actual Unity Play Mode captures with isolated saves and simulated phone sizes/safe areas. The shared header and exclusive Ivan highlight are preserved. Original Upgrade and Select lettering does not enable those features: both buttons remain disabled. The existing collection previews and routes are unchanged.</p><label>View <select id="view">OPTIONS</select></label><img id="page" src="UnityCaptures/Characters_1080x2400_safe_RU.png" alt="Characters / Hero Collection page"><p><a href="ImplementationReport.md">Changes and validation</a> · <a href="QA/ReferenceFidelityReview.md">Remaining reference differences</a> · <a href="QA/ScopeAndVerification.json">Verification results</a></p><script>document.querySelector('#view').onchange=e=>document.querySelector('#page').src=e.target.value;</script></html>'''.replace('OPTIONS',options)
(b/'Characters_SafeEdges_R3_Review.html').write_text(page)
before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};record={'modified':sorted(p for p in before.keys()&after.keys() if before[p]!=after[p]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{},'run':str(run),'captures':len(choices)}
for mode in ('EditMode','PlayMode'):
 r=ET.parse(run/mode/'results.xml').getroot();record['checks'][mode]={k:r.get(k) for k in ('result','passed','failed','skipped','total')}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2));print(json.dumps(record,indent=2))
