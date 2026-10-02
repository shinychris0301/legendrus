from pathlib import Path
import hashlib,json,re
root=Path.cwd();b=Path('/tmp/lor-fail-r1');r=root/'ProjectDocs/AI/FailStyleAlignment/R1'
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(p,o):p.write_text(json.dumps(o,indent=2)+'\n')
baseline=json.loads((b/'BaselineProductionHashes.json').read_text());paths={e['path'] for e in baseline}
changed=[e['path'] for e in baseline if (root/e['path']).is_file() and digest(root/e['path'])!=e['sha256']];missing=[e['path'] for e in baseline if not (root/e['path']).is_file()]
new=[]
for directory in ['Assets/LegendsOfRus/Scripts','Assets/LegendsOfRus/Tests','Assets/LegendsOfRus/Resources/ProductionUI/FailAligned']:
 for p in (root/directory).rglob('*'):
  if p.is_file() and str(p.relative_to(root)) not in paths:new.append(str(p.relative_to(root)))
new.append('Assets/LegendsOfRus/Resources/ProductionUI/FailAligned.meta');new=sorted(set(new))
oldids=set(re.findall(r'^--- !u!\d+ &(\d+)',(b/'baseline/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab').read_text(),re.M));ids=set(re.findall(r'^--- !u!\d+ &(\d+)',(root/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Fail.prefab').read_text(),re.M))
old=json.loads((root/'ProjectDocs/AI/VictoryStyleAlignment/RewardsRenderingR8/UnityVerification/EvidenceHashes.json').read_text());oldchanged=[e['path'] for e in old if digest(root/e['path'])!=e['sha256']]
whitespace=[p+':'+str(i) for p in changed+new if p.endswith('.cs') for i,line in enumerate((root/p).read_text().splitlines(),1) if line.rstrip()!=line]
audit={'baseline_files':len(baseline),'unchanged_baseline_files':len(baseline)-len(changed)-len(missing),'changed_existing_files':changed,'missing_files':missing,'new_files':new,'existing_metas':sum(e['path'].endswith('.meta') for e in baseline),'changed_existing_metas':[p for p in changed if p.endswith('.meta')],'old_fail_object_ids':len(oldids),'new_fail_object_ids':len(ids),'removed_prior_object_ids':sorted(oldids-ids),'old_R8_evidence_files':len(old),'old_R8_evidence_changed':oldchanged,'changed_CSharp_trailing_whitespace':whitespace,'scope':'Verified prior local R8 manifest; unrelated dirty Gameplay/PreLevel and all prior page states retained. New paths enumerated in Scripts/Tests and Failure artwork.'}
write(r/'QA/SourceAudit.json',audit);write(r/'FinalProductionHashes.json',[{'path':p,'sha256':digest(root/p)} for p in sorted(paths|set(new))]);print(json.dumps(audit,indent=2));assert not missing and not oldchanged and not audit['changed_existing_metas'] and not audit['removed_prior_object_ids'] and not whitespace
