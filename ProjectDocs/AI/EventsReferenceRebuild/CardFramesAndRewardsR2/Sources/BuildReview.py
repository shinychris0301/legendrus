from pathlib import Path
import shutil,json,hashlib,xml.etree.ElementTree as E
b=Path(__file__).resolve().parent.parent
run=Path('/tmp/lor-events-frames-r2/Verify03')
for mode in ['Import','EditMode','PlayMode']:
 dest=b/'QA/Verify03'/mode;dest.mkdir(parents=True,exist_ok=True)
 for name in ['command.json','unity.log','results.xml']:
  p=run/mode/name
  if p.exists():shutil.copyfile(p,dest/name)
for p in (run/'Events').glob('*'):
 dest=b/'UnityCaptures';dest.mkdir(exist_ok=True);shutil.copyfile(p,dest/p.name)
old=Path('ProjectDocs/AI/EventsReferenceRebuild/R1/UnityCaptures/Events_941x1672_RU.png')
shutil.copyfile(old,b/'QA/PreviousEventsR1.png')
shutil.copyfile('Assets/LegendsOfRus/Resources/DesignScreens/Events.png',b/'Sources/OriginalReference.png')
choices=[('UnityCaptures/Events_941x1672_RU.png','Implemented — Russian'),('UnityCaptures/Events_941x1672_EN.png','Implemented — English'),('UnityCaptures/Events_576x1280_safe_RU.png','Narrow phone — Russian'),('UnityCaptures/Events_576x1280_safe_EN.png','Narrow phone — English'),('UnityCaptures/Events_1080x2400_safe_RU.png','Tall phone — safe area'),('UnityCaptures/Events_MaxBalances_RU.png','Maximum balances'),('UnityCaptures/Events_ActiveEmpty_RU.png','Active — empty state'),('UnityCaptures/Events_CompletedEmpty_EN.png','Completed — empty state'),('Sources/OriginalReference.png','Original reference'),('QA/PreviousEventsR1.png','Previous R1 — historical Unity capture')]
options=''.join(f'<option value="{p}">{label}</option>' for p,label in choices)
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Events — Card Frames and Rewards R2</title><style>body{background:#0a101a;color:#eeddb5;font:16px/1.5 system-ui;margin:24px auto;max-width:960px;padding:0 16px}h1{font-size:26px}a{color:#9bcded}select{max-width:100%;padding:10px;background:#14263a;color:#f6e4b6;border:1px solid #90713f;font:inherit}.page{display:block;width:min(100%,471px);height:auto;margin:20px auto;border:1px solid #78603b}.note{color:#c3c9d0}button{font:inherit;padding:8px;background:#213146;color:#ffe8b6;border:1px solid #89704a;cursor:pointer}</style><h1>Events — Card Frames and Rewards R2</h1><p>Actual Unity page with clean frame corners, fitted scenery windows and larger individually placed reward icons. Original title and shared Home header retained. Existing Scenario extractions reused; no new requests or credits.</p><p class="note">Events remain upcoming; rewards are labelled examples. The Firebird preview works. Schedules, participation and event rewards are not implemented.</p><label>View <select id="view">OPTIONS</select></label> <button id="original">Compare with original</button><img id="page" class="page" src="UnityCaptures/Events_941x1672_RU.png" alt="Selected portrait Events page"><p id="caption" class="note">Actual Unity Play Mode capture; isolated test save. Reference values are not copied into the save.</p><p><a href="ImplementationReport.md">Implementation and verification</a> · <a href="QA/ReferenceFidelityReview.md">Remaining reference differences</a> · <a href="QA/FrameAndRewardComparison.png">Original / before / after details</a> · <a href="QA/RewardAlpha.png">Reward alpha</a> · <a href="QA/FrameAlpha.png">Frame alpha over black / white / magenta</a></p><script>const picker=document.querySelector('#view'),page=document.querySelector('#page'),caption=document.querySelector('#caption');let previous=picker.value;function show(){page.src=picker.value;caption.textContent=picker.value.includes('OriginalReference')?'Original reference — comparison only.':picker.value.includes('PreviousEvents')?'Historical R1 Unity capture — before this rebuild.':'Actual Unity Play Mode capture; isolated save and simulated screen/safe area.';}picker.onchange=show;document.querySelector('#original').onclick=()=>{if(picker.value.includes('OriginalReference'))picker.value=previous;else{previous=picker.value;picker.value='Sources/OriginalReference.png';}show();};</script></html>'''.replace('OPTIONS',options)
(b/'Events_CardFramesAndRewards_R2_Review.html').write_text(html)
before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()}
record={'modified':[p for p in before.keys()&after.keys() if before[p]!=after[p]],'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{}}
for m in ['EditMode','PlayMode']:
 r=E.parse(run/m/'results.xml').getroot();record['checks'][m]={k:r.get(k) for k in ['result','passed','failed','skipped','total']}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps(record,indent=2))
