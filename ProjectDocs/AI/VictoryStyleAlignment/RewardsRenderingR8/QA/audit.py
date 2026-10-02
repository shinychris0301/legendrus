from pathlib import Path
import json,hashlib,re
root=Path.cwd();review=root/'ProjectDocs/AI/VictoryStyleAlignment/RewardsRenderingR8';base=Path('/tmp/lor-victory-r8')
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(p,obj):p.write_text(json.dumps(obj,indent=2)+'\n')
baseline=json.loads((base/'BaselineProductionHashes.json').read_text())
changed=[x['path'] for x in baseline if (root/x['path']).is_file() and digest(root/x['path'])!=x['sha256']]
missing=[x['path'] for x in baseline if not (root/x['path']).is_file()]
new=['Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryPanelFit.cs'+ext for ext in ['', '.meta']]
oldids=set(re.findall(r'^--- !u!\d+ &(\d+)',(base/'baseline/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab').read_text(),re.M))
newids=set(re.findall(r'^--- !u!\d+ &(\d+)',(root/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab').read_text(),re.M))
old_evidence=json.loads((root/'ProjectDocs/AI/VictoryStyleAlignment/FirebirdAndButtonR7/UnityVerification/EvidenceHashes.json').read_text())
evidence_changed=[x['path'] for x in old_evidence if not (root/x['path']).is_file() or digest(root/x['path'])!=x['sha256']]
whitespace=[p+':'+str(i) for p in changed+new if p.endswith('.cs') for i,line in enumerate((root/p).read_text().splitlines(),1) if line.rstrip()!=line]
audit={'baseline_files':len(baseline),'unchanged_baseline_files':len(baseline)-len(changed)-len(missing),'changed_existing_files':changed,'missing_files':missing,'new_files':new,'existing_metas':sum(x['path'].endswith('.meta') for x in baseline),'changed_existing_metas':[p for p in changed if p.endswith('.meta')],'old_victory_object_ids':len(oldids),'new_victory_object_ids':len(newids),'removed_prior_object_ids':sorted(oldids-newids),'old_R7_evidence_files':len(old_evidence),'old_R7_evidence_changed':evidence_changed,'changed_CSharp_trailing_whitespace':whitespace,'scope':'R7 manifest of Scripts/Tests, production resources/config/prefabs, Packages and ProjectSettings; compared with preceding local state, not historical git HEAD.'}
write(review/'QA/SourceAudit.json',audit)
write(review/'FinalProductionHashes.json',[{'path':p,'sha256':digest(root/p)} for p in sorted([x['path'] for x in baseline]+new)])
print(json.dumps(audit,indent=2))
assert not missing and not evidence_changed and not audit['changed_existing_metas'] and not audit['removed_prior_object_ids'] and not whitespace
