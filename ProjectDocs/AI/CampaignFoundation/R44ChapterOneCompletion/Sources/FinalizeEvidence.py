from pathlib import Path
import json, difflib
import xml.etree.ElementTree as ET

base=Path(__file__).resolve().parent.parent
root=base.parents[3]
latest={};runs=[]
for name in ['Preflight','CompletionWitnesses','Collection','Integration02','EditMode']:
    p=base/'QA'/name
    summary=json.loads((p/'summary.json').read_text())
    tree=ET.parse(p/'results.xml')
    runs.append(dict(name=name,result=summary['result'],exitCode=summary['exitCode'],durationSeconds=summary['durationSeconds']))
    for case in tree.findall('.//test-case'):
        latest[case.get('fullname')]=dict(result=case.get('result'),run=name,message=case.findtext('.//message'))
counts={state:sum(v['result']==state for v in latest.values()) for state in ['Passed','Failed','Skipped','Inconclusive']}
record=dict(runs=runs,latestDistinctCounts=counts,
    incompleteRuns=['Integration: wrapper exited 143 for unknown reason; owned Unity process stopped without result XML. Replaced by Collection and Integration02.'],
    limitations=['Focused runtime coverage, not a fresh full-project Play Mode sweep.',
    'Automated solver replays are not human difficulty acceptance.',
    'No Android build or device test in this task.'])
(base/'QA/Verification.json').write_text(json.dumps(record,indent=2)+'\n')
patch=[]
for before in sorted((base/'Sources').glob('*.before.*')):
    name=before.name.replace('.before','')
    if before.suffix=='.json':after=root/'Assets/LegendsOfRus/Resources/CampaignActive'/name
    else:
        matches=list((root/'Assets/LegendsOfRus').rglob(name));assert len(matches)==1,(name,matches)
        after=matches[0]
    relative=str(after.relative_to(root))
    patch.extend(difflib.unified_diff(before.read_text().splitlines(True),after.read_text().splitlines(True),fromfile='a/'+relative,tofile='b/'+relative))
(base/'Sources/Changes.patch').write_text(''.join(patch))
print(json.dumps(record,indent=2))
