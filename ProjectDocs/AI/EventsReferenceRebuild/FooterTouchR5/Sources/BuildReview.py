from pathlib import Path
import hashlib, json, shutil, html
import xml.etree.ElementTree as ET
root=Path.cwd()
base=root/'ProjectDocs/AI/EventsReferenceRebuild/FooterTouchR5'
run=Path('/tmp/lor-events-footer-r5/Verify01')
(base/'UnityCaptures').mkdir(exist_ok=True)
(base/'QA').mkdir(exist_ok=True)
for p in (run/'Events').iterdir():
    if p.suffix in ('.png','.json'): shutil.copy2(p,base/'UnityCaptures'/p.name)
results={}
for mode in ('Import','EditMode','PlayMode'):
    dest=base/'QA/Verify01'/mode;dest.mkdir(parents=True,exist_ok=True)
    for name in ('results.xml','command.json','unity.log','launcher.log'):
        if (run/mode/name).exists(): shutil.copy2(run/mode/name,dest/name)
    if mode=='Import': continue
    a=ET.parse(dest/'results.xml').getroot().attrib
    results[mode]={k:a[k] for k in ('total','passed','failed','skipped')}
before=json.loads((base/'Sources/AssetsBefore.json').read_text())
after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()}
scope={'modified':[p for p in before if p in after and before[p]!=after[p]],'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items()),'tests':results,'nativeCaptureCount':len(list((base/'UnityCaptures').glob('*.png')))}
(base/'QA/ScopeAndVerification.json').write_text(json.dumps(scope,indent=2)+'\n')
views=[('Events_941x1672_RU','Upcoming · Russian · reference size'),('Events_941x1672_EN','Upcoming · English · reference size'),('Events_1080x2400_safe_RU','Upcoming · Russian · tall safe area'),('Events_1080x2400_safe_EN','Upcoming · English · tall safe area'),('Events_576x1280_safe_EN','Upcoming · English · narrow phone'),('Events_TallCategory_0_ru','Active empty state · Russian · narrow phone'),('Events_TallCategory_2_en','Completed empty state · English · narrow phone'),('Events_TallReturn_ru','Return from Event Detail · Russian'),('Events_TallReturn_en','Return from Event Detail · English'),('Events_MaxBalances_RU','Maximum balances · Russian')]
options=''.join(f'<option value="{key}">{html.escape(label)}</option>' for key,label in views)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Events — Footer Touch R5</title><style>*{box-sizing:border-box}body{margin:0;background:#080e17;color:#e3d5b4;font:16px/1.5 system-ui}main{max-width:680px;margin:auto;padding:20px}h1{font:28px Georgia;color:#dabb79}a{color:#b5d1eb}select{width:100%;padding:12px;background:#122333;color:#eee;border:1px solid #7f693f;margin:8px 0 18px}img{display:block;width:100%;height:auto}figure{margin:0 auto;max-width:470px}figcaption{font-size:13px;padding:12px 0;color:#c2bcae}</style><main><h1>Events · Footer Touch R5</h1><p>Actual Unity Play Mode captures. Back now accepts taps in the clear margin around its original icon. Approved artwork and visible layout are unchanged; header, footer, categories and return navigation are checked.</p><p><a href="ImplementationReport.md">Report</a> · <a href="QA/ReferenceFidelityReview.md">Visual findings</a> · <a href="QA/ScopeAndVerification.json">Verification</a></p><label for="view">Capture</label><select id="view">OPTIONS</select><figure><img id="page" alt="Events page captured in Unity"><figcaption id="caption"></figcaption></figure><p>This viewer displays screenshots, not an interactive game. Safe areas are simulated; Android device testing was not performed. Existing spacing below the four event cards and reference typography differences remain.</p></main><script>const view=document.getElementById('view'),pic=document.getElementById('page');function show(){pic.src='UnityCaptures/'+view.value+'.png';document.getElementById('caption').textContent=view.selectedOptions[0].textContent+' — actual Unity capture';}view.addEventListener('change',show);show();</script></html>'''
(base/'Events_FooterTouch_R5_Review.html').write_text(page.replace('OPTIONS',options))
print(json.dumps(scope,indent=2))
