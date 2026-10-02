"""Package actual Unity evidence and a standalone portrait review. Run from project root."""
from pathlib import Path
import hashlib,html,json,shutil,sys
import xml.etree.ElementTree as ET

b=Path(__file__).resolve().parents[1]
run=Path('/tmp/lor-friends-tabs-flags-r2')/sys.argv[1]
captures=b/'UnityCaptures';captures.mkdir(exist_ok=True)
for p in (run/'FriendsList').glob('*'):shutil.copy2(p,captures/p.name)
shutil.copytree(run/'TeamChat',b/'QA/RegressionTeamChat',dirs_exist_ok=True)
shutil.copytree(run/'TeamHub',b/'QA/RegressionTeamHub',dirs_exist_ok=True)
for old in run.parent.glob('Verify*'):
    for mode in ('Import','EditMode','PlayMode'):
        if (old/mode).exists():shutil.copytree(old/mode,b/'QA'/old.name/mode,dirs_exist_ok=True)
previous=Path('ProjectDocs/AI/FriendsListReferenceRebuild/R1/UnityCaptures/FriendsList_941x1672_RU.png')
shutil.copy2(previous,b/'Sources/PreviousFriendsList.png')
choices=[('UnityCaptures/FriendsList_941x1672_RU.png','Current — Russian, 941 × 1672')]
choices += [('UnityCaptures/'+p.name,p.stem.replace('FriendsList_','')) for p in sorted(captures.glob('*.png')) if p.name!='FriendsList_941x1672_RU.png']
choices += [('Sources/OriginalReference.png','Original Friends reference'),('Sources/RequestsReference.png','Original Requests reference'),('Sources/PreviousFriendsList.png','Before — Reference Rebuild R1')]
options=''.join(f'<option value="{p}">{html.escape(label)}</option>' for p,label in choices)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Friends List — Joined Tabs, Guild Flags and Frame R2</title><style>body{background:#0a101a;color:#eeddb5;font:16px/1.5 system-ui;margin:24px auto;max-width:960px;padding:0 16px}h1{font-size:26px}a{color:#9bcded}select{max-width:100%;padding:10px;background:#14263a;color:#f6e4b6;border:1px solid #90713f;font:inherit}.page{display:block;width:min(100%,471px);height:auto;margin:20px auto;border:1px solid #78603b}.note{color:#c3c9d0}button{font:inherit;padding:8px;background:#213146;color:#ffe8b6;border:1px solid #89704a;cursor:pointer}</style><h1>Friends List — Joined Tabs, Guild Flags and Frame R2</h1><p>Original joined tab rails and central sapphire, five source guild flags and source-derived outer list curls/trim. Live localized tab selection, search, sorting, shared header and original-lettered action/footer controls are preserved.</p><p class="note">Actual Unity captures below. Friends, requests, guilds and presence are examples. Online friend actions are not connected and show an explanation. Search and sorting work locally. English uses live labels. Shared scenery, reconstructed frames and live fonts differ from the original.</p><label>View <select id="view">OPTIONS</select></label> <button id="original">Compare with original</button><img id="page" class="page" src="UnityCaptures/FriendsList_941x1672_RU.png" alt="Friends List portrait page"><p id="caption" class="note">Actual Unity Play Mode capture, isolated save and simulated screen/safe area.</p><p><a href="ImplementationReport.md">Implementation and checks</a> · <a href="QA/ReferenceFidelityReview.md">Remaining differences</a> · <a href="QA/AssetAlpha.png">Alpha inspection</a></p><script>const picker=document.querySelector('#view'),page=document.querySelector('#page'),caption=document.querySelector('#caption');let previous=picker.value;function show(){page.src=picker.value;caption.textContent=picker.value.includes('RequestsReference')?'Original Requests reference.':picker.value.includes('OriginalReference')?'Original Friends List reference.':picker.value.includes('PreviousFriendsList')?'Historical Reference Rebuild R1 native capture.':'Actual Unity Play Mode capture, isolated save and simulated screen/safe area.';}picker.onchange=show;document.querySelector('#original').onclick=()=>{if(picker.value.includes('OriginalReference'))picker.value=previous;else{previous=picker.value;picker.value='Sources/OriginalReference.png';}show();};</script></html>'''.replace('OPTIONS',options)
(b/'FriendsList_TabsFlagsAndFrame_R2_Review.html').write_text(page)
before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
after={str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()}
record={'modified':sorted(p for p in before.keys()&after.keys() if before[p]!=after[p]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'checks':{},'finalRun':str(run)}
for mode in ('EditMode','PlayMode'):
    r=ET.parse(run/mode/'results.xml').getroot();record['checks'][mode]={k:r.get(k) for k in ('result','passed','failed','skipped','total')}
(b/'QA/ScopeAndVerification.json').write_text(json.dumps(record,indent=2)+'\n')
changed=set(record['modified'])|set(record['deleted'])
preserved={
    'existingPNGAndMetas':not any(p.endswith(('.png','.meta')) for p in changed),
    'otherPagePrefabs':not any('/Pages/' in p and p.endswith('.prefab') and not p.endswith('/FriendsList.prefab') for p in changed),
    'fonts':not any(p.endswith(('.ttf','.otf')) for p in changed),
    'priorEventDetailChanges':not any('EventDetail' in p for p in changed),
}
assert all(preserved.values()),preserved
(b/'QA/PreservationChecks.json').write_text(json.dumps(preserved,indent=2)+'\n')
print(json.dumps(record,indent=2))
