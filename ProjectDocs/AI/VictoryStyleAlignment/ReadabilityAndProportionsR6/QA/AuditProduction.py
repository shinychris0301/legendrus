from pathlib import Path
import json,hashlib,re
root=Path.cwd(); review=root/'ProjectDocs/AI/VictoryStyleAlignment/ReadabilityAndProportionsR6'
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def write(p,obj):p.write_text(json.dumps(obj,indent=2)+'\n')
baseline=json.loads(Path('/tmp/lor-victory-r6/BaselineProductionHashes.json').read_text())
changed=[x['path'] for x in baseline if (root/x['path']).is_file() and digest(root/x['path'])!=x['sha256']]
missing=[x['path'] for x in baseline if not (root/x['path']).is_file()]
new=[]
oldids=set(re.findall(r'^--- !u!\d+ &(\d+)',(Path('/tmp/lor-victory-r6/baseline/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab')).read_text(),re.M))
newids=set(re.findall(r'^--- !u!\d+ &(\d+)',(root/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab').read_text(),re.M))
old_evidence=json.loads((root/'ProjectDocs/AI/VictoryStyleAlignment/RewardsAndActionsR5/UnityVerification/EvidenceHashes.json').read_text())
evidence_changed=[x['path'] for x in old_evidence if not (root/x['path']).is_file() or digest(root/x['path'])!=x['sha256']]
audit={'baseline_files':len(baseline),'unchanged_baseline_files':len(baseline)-len(changed)-len(missing),'changed_existing_files':changed,'missing_files':missing,'new_files':new,'existing_metas':sum(x['path'].endswith('.meta') for x in baseline),'changed_existing_metas':[p for p in changed if p.endswith('.meta')],'old_victory_object_ids':len(oldids),'new_victory_object_ids':len(newids),'removed_prior_object_ids':sorted(oldids-newids),'old_R5_evidence_files':len(old_evidence),'old_R5_evidence_changed':evidence_changed,'scope':'R5 manifest of Scripts/Tests, production resources/config/prefabs, Packages and ProjectSettings; compared with accepted local state, not historical git HEAD.'}
write(review/'QA/SourceAudit.json',audit)
write(review/'FinalProductionHashes.json',[{'path':p,'sha256':digest(root/p)} for p in sorted([x['path'] for x in baseline]+new)])
print(json.dumps(audit,indent=2))
