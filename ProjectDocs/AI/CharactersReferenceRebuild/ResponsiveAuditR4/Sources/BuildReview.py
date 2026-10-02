from pathlib import Path
import hashlib, json, shutil, html, sys
import xml.etree.ElementTree as ET
root=Path.cwd()
base=root/'ProjectDocs/AI/CharactersReferenceRebuild/ResponsiveAuditR4'
runName=sys.argv[1] if len(sys.argv)>1 else 'Verify01'
if Path(runName).name!=runName or runName in ('.','..'): raise SystemExit('Expected a run directory name')
run=Path('/tmp/lor-characters-responsive-r4')/runName
for mode in ('EditMode','PlayMode'):
    result=ET.parse(run/mode/'results.xml').getroot().attrib
    if result.get('result')!='Passed' or int(result.get('failed',1)) or int(result.get('skipped',1)): raise SystemExit('Cannot publish incomplete or failing Unity evidence')
(base/'UnityCaptures').mkdir(exist_ok=True)
(base/'QA').mkdir(exist_ok=True)
for p in (run/'Characters').iterdir():
    if p.suffix in ('.png','.json'): shutil.copy2(p,base/'UnityCaptures'/p.name)
results={}
for mode in ('Import','EditMode','PlayMode'):
    dest=base/'QA'/runName/mode;dest.mkdir(parents=True,exist_ok=True)
    for name in ('results.xml','command.json','unity.log','launcher.log'):
        if (run/mode/name).exists(): shutil.copy2(run/mode/name,dest/name)
    if mode=='Import': continue
    a=ET.parse(dest/'results.xml').getroot().attrib
    results[mode]={k:a[k] for k in ('total','passed','failed','skipped')}
before=json.loads((base/'Sources/AssetsBefore.json').read_text())
after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()}
scope={'modified':[p for p in before if p in after and before[p]!=after[p]],'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items()),'tests':results,'nativeCaptureCount':len(list((base/'UnityCaptures').glob('*.png')))}
(base/'QA/ScopeAndVerification.json').write_text(json.dumps(scope,indent=2)+'\n')
views=[('Characters_941x1672_RU','Russian · reference size'),('Characters_941x1672_EN','English · reference size'),('Characters_576x1280_safe_EN','English · narrow safe area'),('Characters_1080x2400_safe_RU','Russian · tall safe area'),('Characters_941x1412_short_RU','Russian · short screen'),('Characters_LockedScrolled_RU','Future cards · scrolled to bottom'),('Characters_Rare_RU','Rare filter · Russian'),('Characters_Firebird_RU','Firebird preview · Russian'),('Characters_NativePreview_en','Rare filter · native card tap'),('Characters_NativeInfoTarget_en','Info touch target · English'),('Characters_RelocalizedInfo_ru','Info changed to English · tall'),('Characters_Info_1080x2400_safe_RU','Info · Russian · tall'),('Characters_NativeRoute_2','Energy shortcut · native edge tap')]
options=''.join(f'<option value="{key}">{html.escape(label)}</option>' for key,label in views)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Characters — Responsive Audit R4</title><style>*{box-sizing:border-box}body{margin:0;background:#080e17;color:#e3d5b4;font:16px/1.5 system-ui}main{max-width:680px;margin:auto;padding:20px}h1{font:28px Georgia;color:#dabb79}a{color:#b5d1eb}select{width:100%;padding:12px;background:#122333;color:#eee;border:1px solid #7f693f;margin:8px 0 18px}img{display:block;width:100%;height:auto}.image-window{max-width:100%;overflow:auto}.native img{width:auto;max-width:none}button{padding:10px;margin:0 0 12px;background:#19304a;color:#eee;border:1px solid #7f693f}figure{margin:0 auto;max-width:470px}figcaption{font-size:13px;padding:12px 0;color:#c2bcae}</style><main><h1>Characters · Responsive Audit R4</h1><p>Actual Unity Play Mode captures. Expanded the resource, rarity-tab and collection-info touch targets while preserving their artwork. Reserved space above the footer keeps Info separate from collection scrolling. Native navigation, filtering, preview selection and modal Close are checked; Ivan remains the current hero.</p><p><a href="ImplementationReport.md">Report</a> · <a href="QA/ReferenceFidelityReview.md">Visual findings</a> · <a href="QA/ScopeAndVerification.json">Verification</a></p><label for="view">Capture</label><select id="view">OPTIONS</select><button id="zoom" type="button">View at native pixels</button><figure><div class="image-window" id="window"><img id="page" alt="Characters page captured in Unity"></div><figcaption id="caption"></figcaption></figure><p>This viewer displays screenshots, not an interactive game. Safe areas are simulated; Android device testing was not performed. Existing scenery, surface and live typography differences remain. Hero ownership and upgrades are still unavailable.</p></main><script>const view=document.getElementById('view'),pic=document.getElementById('page');function show(){pic.src='UnityCaptures/'+view.value+'.png';document.getElementById('caption').textContent=view.selectedOptions[0].textContent+' — actual Unity capture';}view.addEventListener('change',show);document.getElementById('zoom').onclick=()=>{const on=document.getElementById('window').classList.toggle('native');document.getElementById('zoom').textContent=on?'Fit to page':'View at native pixels';};show();</script></html>'''
(base/'Characters_ResponsiveAudit_R4_Review.html').write_text(page.replace('OPTIONS',options))
print(json.dumps(scope,indent=2))
