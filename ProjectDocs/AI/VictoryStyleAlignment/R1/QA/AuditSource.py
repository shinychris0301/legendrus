from pathlib import Path
import hashlib,json,re,difflib
root=Path(__file__).resolve().parents[5];docs=root/'ProjectDocs/AI/VictoryStyleAlignment/R1'
cache=Path('/home/k2/.cache/lor-victory-r1');before=json.loads((cache/'BeforeProductionHashes.json').read_text());hashfile=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
changed=[x['path'] for x in before if not (root/x['path']).exists() or hashfile(root/x['path'])!=x['sha256']]
expected=['Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab','Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs','Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionUiFix01Runtime.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs']
assert set(changed)==set(expected),changed
new=[str(p.relative_to(root)) for p in (root/'Assets/LegendsOfRus').rglob('*') if p.is_file() and ('/VictoryAligned/' in str(p) or p.name in ['VictoryAligned.meta','ProductionVictoryView.cs','ProductionVictoryView.cs.meta','ProductionVictoryLayout.cs','ProductionVictoryLayout.cs.meta','ProductionVictoryRuntime.cs','ProductionVictoryRuntime.cs.meta','VictoryAlignmentPlayModeTests.cs','VictoryAlignmentPlayModeTests.cs.meta','VictoryAlignmentEditModeTests.cs','VictoryAlignmentEditModeTests.cs.meta'])]
oldmeta=[x for x in before if x['path'].endswith('.meta')];assert not any(x.endswith('.meta') for x in changed)
p='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab';ids=lambda path:set(re.findall(r'^--- !u!\d+ &(-?\d+)',path.read_text(),re.M));oldids=ids(cache/'before'/p);newids=ids(root/p);assert oldids<=newids
files=sorted(set(x['path'] for x in before)|set(new));manifest=[{'path':p,'sha256':hashfile(root/p)} for p in files]
(docs/'FinalProductionHashes.json').write_text(json.dumps(manifest,indent=2)+'\n')
audit={'baselineFiles':len(before),'changedFiles':changed,'unchangedFiles':len(before)-len(changed),'existingMetas':len(oldmeta),'changedMetas':[],'oldVictoryObjectIDs':len(oldids),'newVictoryObjectIDs':len(newids),'lostVictoryObjectIDs':sorted(oldids-newids),'newProductionFiles':sorted(new),'finalProductionFiles':len(files)}
(docs/'QA/SourceAudit.json').write_text(json.dumps(audit,indent=2)+'\n')
parts=[]
for p in changed:
 if p.endswith('.cs'):parts.extend(difflib.unified_diff((cache/'before'/p).read_text().splitlines(True),(root/p).read_text().splitlines(True),fromfile='before/'+p,tofile='after/'+p))
(docs/'AppliedSourceChanges.diff').write_text(''.join(parts))
whitespace=[]
for p in sorted(set(changed+new)):
 if not p.endswith('.cs'):continue
 for n,line in enumerate((root/p).read_text().splitlines(),1):
  if line.rstrip()!=line:whitespace.append([p,n])
assert not whitespace,whitespace
(docs/'QA/WhitespaceChecks.json').write_text(json.dumps({'files':[p for p in sorted(set(changed+new)) if p.endswith('.cs')],'trailingWhitespace':whitespace},indent=2)+'\n')
print(json.dumps(audit,indent=2))
