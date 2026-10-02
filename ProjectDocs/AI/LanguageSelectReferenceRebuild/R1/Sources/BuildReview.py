from pathlib import Path
import json,shutil,hashlib,sys,html
import xml.etree.ElementTree as ET
b=Path(__file__).resolve().parents[1];run=Path('/tmp/lor-language-rebuild-r1')/sys.argv[1];out=b/'UnityCaptures';out.mkdir(exist_ok=True)
for p in (run/'LanguageSelect').glob('*'):shutil.copy2(p,out/p.name)
for name in ('Settings_Language_RU','Settings_Language_safe_EN'):
 for ext in ('.png','.json'):shutil.copy2(run/'Settings'/(name+ext),out/(name+ext))
for old in sorted(list(run.parent.glob('Verify*'))+list(run.parent.glob('Baseline*'))):
 if not old.is_dir():continue
 for mode in ('Import','EditMode','PlayMode'):
  if(old/mode).exists():shutil.copytree(old/mode,b/'QA'/old.name/mode,dirs_exist_ok=True)
beforeOut=b/'BeforeCaptures';beforeOut.mkdir(exist_ok=True)
for p in (run.parent/'Baseline01/LanguageSelect').glob('*'):shutil.copy2(p,beforeOut/p.name)
choices=['LanguageSelect_941x1672_RU.png']+[p.name for p in sorted(out.glob('*.png')) if p.name!='LanguageSelect_941x1672_RU.png'];assert len(choices)>=6
options=''.join('<option value="UnityCaptures/'+n+'">Unity · '+html.escape(n[:-4])+'</option>' for n in choices)
options+='<option value="Sources/Reference.png">Original language-modal reference (Settings backdrop)</option><option value="BeforeCaptures/LanguageSelect_941x1672_RU.png">Before · Previous Language Select</option>'
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Language Select — Reference Rebuild R1</title><style>body{background:#09111e;color:#efdcb4;font:16px/1.5 system-ui;max-width:920px;margin:24px auto;padding:0 16px}h1{font-size:26px}a{color:#a1d3f8}select{background:#17273b;color:#efdcb4;padding:10px;max-width:100%}img{display:block;width:min(100%,471px);height:auto;margin:24px auto;border:1px solid #78603b}</style><h1>Language Select — Reference Rebuild R1</h1><p>The selector now reuses the current approved Splash backdrop and Settings modal, with consistent native button press feedback.</p><p>Russian and English selection uses the existing saved language preference. The backdrop is non-interactive, and Close returns to the originating page. The original reference shows Settings behind the modal; this separate startup route intentionally uses Splash. Unity views are actual Play Mode captures with isolated saves and simulated phone dimensions/safe areas; reference and previous-version views are labelled separately.</p><label>View <select id="view">OPTIONS</select></label><img id="page" src="UnityCaptures/LanguageSelect_941x1672_RU.png" alt="Language Select page"><p><a href="ImplementationReport.md">Changes and validation</a> · <a href="QA/ReferenceFidelityReview.md">Remaining reference differences</a> · <a href="QA/ScopeAndVerification.json">Verification results</a></p><script>document.querySelector('#view').onchange=e=>document.querySelector('#page').src=e.target.value;</script></html>'''.replace('OPTIONS',options)
(b/'LanguageSelect_ReferenceRebuild_R1_Review.html').write_text(page)
before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()};record={'modified':sorted(p for p in before.keys()&after.keys() if before[p]!=after[p]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{},'run':str(run),'captures':len(choices)}
for mode in ('EditMode','PlayMode'):
 r=ET.parse(run/mode/'results.xml').getroot();record['checks'][mode]={k:r.get(k) for k in ('result','passed','failed','skipped','total')}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2));print(json.dumps(record,indent=2))
