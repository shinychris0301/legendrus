from pathlib import Path
import hashlib,json,re,difflib
root=Path(__file__).resolve().parents[5];docs=root/'ProjectDocs/AI/VictoryStyleAlignment/RewardsAndDepthR2';cache=Path('/home/k2/.cache/lor-victory-r2');sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
before=json.loads((cache/'BeforeProductionHashes.json').read_text());changed=[x['path'] for x in before if sha(root/x['path'])!=x['sha256']]
expected=['Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab','Assets/LegendsOfRus/Scripts/Editor/Step20ProductionUiFix01SerializedPrefabBuilder.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryLayout.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryRuntime.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryView.cs','Assets/LegendsOfRus/Tests/PlayMode/VictoryAlignmentPlayModeTests.cs','Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/VictoryAlignmentEditModeTests.cs'];assert set(changed)==set(expected),changed
new=['Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/rewards_panel_r2.png','Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/rewards_panel_r2.png.meta','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryStarDepth.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionVictoryStarDepth.cs.meta']
p='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Victory.prefab';ids=lambda p:set(re.findall(r'^--- !u!\d+ &(-?\d+)',p.read_text(),re.M));oldids=ids(cache/'before'/p);newids=ids(root/p);assert oldids<=newids
assert not any(p.endswith('.meta') for p in changed)
manifest=[{'path':p,'sha256':sha(root/p)} for p in sorted(set(x['path'] for x in before)|set(new))];(docs/'FinalProductionHashes.json').write_text(json.dumps(manifest,indent=2)+'\n')
audit={'baselineFiles':len(before),'changedFiles':changed,'unchangedFiles':len(before)-len(changed),'existingMetas':sum(x['path'].endswith('.meta') for x in before),'changedMetas':[],'oldVictoryObjectIDs':len(oldids),'newVictoryObjectIDs':len(newids),'lostVictoryObjectIDs':[],'newProductionFiles':new,'finalProductionFiles':len(manifest)}
(docs/'QA/SourceAudit.json').write_text(json.dumps(audit,indent=2)+'\n')
patch=[];bad=[]
for p in changed+new:
 if not p.endswith('.cs'):continue
 lines=(root/p).read_text().splitlines(True)
 for n,line in enumerate(lines,1):
  if line.rstrip('\n').rstrip()!=line.rstrip('\n'):bad.append([p,n])
 old=(cache/'before'/p).read_text().splitlines(True) if (cache/'before'/p).exists() else []
 patch.extend(difflib.unified_diff(old,lines,fromfile='before/'+p,tofile='after/'+p))
assert not bad,bad
(docs/'AppliedSourceChanges.diff').write_text(''.join(patch));(docs/'QA/WhitespaceChecks.json').write_text(json.dumps({'trailingWhitespace':bad,'checkedFiles':[p for p in changed+new if p.endswith('.cs')]},indent=2)+'\n')
print(json.dumps(audit,indent=2))
