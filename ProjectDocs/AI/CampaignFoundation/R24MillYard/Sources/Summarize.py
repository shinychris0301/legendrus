from pathlib import Path
import json,xml.etree.ElementTree as ET
base=Path(__file__).resolve().parent.parent
suites=[];names=[]
for n in ['Active','Campaign','Compatibility','Checkpoint']:
 p=base/'QA/Verify01'/n/'results.xml';root=ET.parse(p).getroot();a=root.attrib
 names.extend(t.attrib['fullname'] for t in root.iter('test-case'))
 assert a['result']=='Passed' and int(a['failed'])==0 and int(a['skipped'])==0
 suites.append(dict(name=n,passed=int(a['passed']),failed=int(a['failed']),skipped=int(a['skipped']),durationSeconds=float(a['duration']),results=str(p.relative_to(base))))
assert len(names)==len(set(names))==sum(x['passed'] for x in suites)
data=dict(distinctTestCases=len(set(names)),unity='6000.5.9f1',suites=suites,totalPassed=sum(x['passed'] for x in suites),failed=0,skipped=0,storage='Isolated test stores, not live user saves',rendering='Prior authorized process-only Vulkan, descriptor4096; project settings unchanged',notRun=['Android build/device','Full project test suite','Human rounds31–120 difficulty acceptance','Visual/pointer hit-area audit'])
(base/'QA/Verification.json').write_text(json.dumps(data,indent=2)+'\n');print(json.dumps(data,indent=2))
