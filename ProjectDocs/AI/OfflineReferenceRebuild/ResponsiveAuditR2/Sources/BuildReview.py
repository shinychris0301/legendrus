from pathlib import Path
import hashlib, json, shutil, html
import xml.etree.ElementTree as ET
root=Path.cwd()
base=root/'ProjectDocs/AI/OfflineReferenceRebuild/ResponsiveAuditR2'
run=Path('/tmp/lor-offline-responsive-r2/Verify01')
(base/'UnityCaptures').mkdir(exist_ok=True)
(base/'QA').mkdir(exist_ok=True)
for p in (run/'Offline').iterdir():
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
views=[('Offline_941x1672_RU','Russian · original lettering'),('Offline_576x1280_safe_EN','English · narrow safe area'),('Offline_1080x2400_safe_RU','Russian · tall safe area'),('Offline_Retry_941x1672_RU','Russian · retry remains offline'),('NetworkDetected_941x1672_RU','Russian · device network detected'),('NetworkDetected_576x1280_safe_EN','English · network is not server confirmation'),('Offline_RelocalizedReturn_ru','English · tall · return Home'),('Offline_RelocalizedReturn_en','Russian · tall · return Home'),('Offline_NativeConnected_ru','Russian · native retry with network detected'),('Offline_HeaderTarget_2','Energy target · narrow phone'),('Offline_HeaderTarget_4','Settings route · narrow phone')]
options=''.join(f'<option value="{key}">{html.escape(label)}</option>' for key,label in views)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Offline — Responsive Audit R2</title><style>*{box-sizing:border-box}body{margin:0;background:#080e17;color:#e3d5b4;font:16px/1.5 system-ui}main{max-width:680px;margin:auto;padding:20px}h1{font:28px Georgia;color:#dabb79}a{color:#b5d1eb}select{width:100%;padding:12px;background:#122333;color:#eee;border:1px solid #7f693f;margin:8px 0 18px}img{display:block;width:100%;height:auto}.image-window{max-width:100%;overflow:auto}.native img{width:auto;max-width:none}button{padding:10px;margin:0 0 12px;background:#19304a;color:#eee;border:1px solid #7f693f}figure{margin:0 auto;max-width:470px}figcaption{font-size:13px;padding:12px 0;color:#c2bcae}</style><main><h1>Offline · Responsive Audit R2</h1><p>Actual Unity Play Mode captures. Resource buttons have larger touch areas; Retry and Home hit rectangles are separate. The artwork is preserved. Native retry, device-network states, Russian/English labels, safe edges and navigation are checked. Device connectivity does not confirm game-server availability.</p><p><a href="ImplementationReport.md">Report</a> · <a href="QA/ReferenceFidelityReview.md">Visual findings</a> · <a href="QA/ScopeAndVerification.json">Verification</a></p><label for="view">Capture</label><select id="view">OPTIONS</select><button id="zoom" type="button">View at native pixels</button><figure><div class="image-window" id="window"><img id="page" alt="Offline page captured in Unity"></div><figcaption id="caption"></figcaption></figure><p>This viewer displays screenshots, not an interactive game. Safe areas are simulated; Android device testing was not performed. Existing tall-screen space and reference typography differences remain.</p></main><script>const view=document.getElementById('view'),pic=document.getElementById('page');function show(){pic.src='UnityCaptures/'+view.value+'.png';document.getElementById('caption').textContent=view.selectedOptions[0].textContent+' — actual Unity capture';}view.addEventListener('change',show);document.getElementById('zoom').onclick=()=>{const on=document.getElementById('window').classList.toggle('native');document.getElementById('zoom').textContent=on?'Fit to page':'View at native pixels';};show();</script></html>'''
(base/'Offline_ResponsiveAudit_R2_Review.html').write_text(page.replace('OPTIONS',options))
print(json.dumps(scope,indent=2))
