from pathlib import Path
import json,hashlib,ast,re,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
before=json.loads((base/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings','Backend'] for p in (root/d).rglob('*') if p.is_file() and '__pycache__' not in p.parts}
record={'baselineFiles':len(before),'changed':sorted(k for k in before.keys()&after.keys() if before[k]!=after[k]),'added':sorted(after.keys()-before.keys()),'deleted':sorted(before.keys()-after.keys()),'unchanged':sum(before[k]==after[k] for k in before.keys()&after.keys())}
assert len(record['changed'])==7 and len(record['added'])==6 and not record['deleted'],record
assert all(k.startswith('Assets/LegendsOfRus/Scripts/Runtime/') for k in record['changed'])
for p in (base/'Sources').glob('*.py'):ast.parse(p.read_text())
for k in record['changed']+record['added']:
 p=root/k
 if p.suffix=='.cs':assert all(line.rstrip()==line for line in p.read_text().splitlines()),k
 q=base/'Sources/After'/k;q.parent.mkdir(parents=True,exist_ok=True);q.write_bytes(p.read_bytes())
newguids={re.search(r'^guid: (\w+)',(root/k).read_text(),re.M).group(1):k for k in record['added'] if k.endswith('.meta')}
for guid,k in newguids.items():
 found=[str(p.relative_to(root)) for p in (root/'Assets').rglob('*.meta') if ('guid: '+guid) in p.read_text(errors='ignore')]
 assert found==[k],found
record['newGuidsUnique']=True
record['packageSettingsBackendChanged']=False
(base/'QA/Scope.json').write_text(json.dumps(record,indent=2))
(base/'Sources/AfterHashes.json').write_text(json.dumps({k:after[k] for k in record['changed']+record['added']},indent=2))
names=json.loads((base/'Sources/FinalPhasePlan.json').read_text())+['NativeJourneys','NormalAfterJourneys','NormalRegression']
runs=[];distinct={};passed=skipped=failed=0
for name in names:
 s=json.loads((base/'QA'/name/'summary.json').read_text());r=s['result'];assert s['exitCode']==0 and int(r['failed'])==0
 passed+=int(r['passed']);skipped+=int(r['skipped']);failed+=int(r['failed'])
 runs.append({'name':name,'passed':int(r['passed']),'failed':int(r['failed']),'skipped':int(r['skipped']),'durationSeconds':s['durationSeconds'],'nonPassing':s['nonPassing']})
 for t in ET.parse(base/'QA'/name/'results.xml').findall('.//test-case'):distinct[t.get('fullname')]=t.get('result')
summary={'runs':runs,'passedExecutions':passed,'failedExecutions':failed,'skippedExecutions':skipped,'distinctPassedTestNames':sum(v=='Passed' for v in distinct.values()),'note':'Eight profile-phase executions share one test name. Intermediate fixture correction runs excluded.'}
(base/'QA/RunInventory.json').write_text(json.dumps(summary,indent=2))
print(json.dumps({'scope':record,'final':{k:v for k,v in summary.items() if k!='runs'}},indent=2))
