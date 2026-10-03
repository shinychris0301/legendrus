"""Require exact fixed-factory MonoScript addition with existing packed membership preserved."""
from collections import Counter
from pathlib import Path
import json,re
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
before=json.loads((ROOT/'ProjectDocs/AI/ReturningPlayerRecap/R3AndroidRefresh/QA/PackedAssets.json').read_text())['assets']
record=json.loads((QA/'PackedAssets.json').read_text());after=record['assets']
assert record['result']=='Succeeded'
assert [record[k] for k in ['profileRevision','dialogueRevision','gameplayRevision','victoryRevision','stockLayouts','storyScenes','storyMilestones']]==[13,2,24,12,4,58,57]
script='Assets/LegendsOfRus/Scripts/Runtime/Services/DevelopmentGameplayFixtures.cs'
key=lambda r:(r['path'],r['guid'],r['type'])
assert Counter(map(key,before))==Counter(key(r) for r in after if r['path']!=script)
added=[r for r in after if r['path']==script]
assert len(added)==1 and added[0]['type']=='MonoScript'
guid=re.search(r'^guid: (\w+)',(ROOT/(script+'.meta')).read_text(),re.M).group(1)
assert added[0]['guid']==guid and len(after)==len(before)+1
pages={}
for name in ['Profile','Dialogue','Gameplay','Victory','ChapterComplete']:
 path='Assets/LegendsOfRus/Resources/ProductionUI/Pages/'+name+'.prefab'
 old=[r for r in before if r['path']==path];new=[r for r in after if r['path']==path]
 assert Counter(map(key,old))==Counter(map(key,new))
 assert sum(r['bytes'] for r in new)==sum(r['bytes'] for r in old)
 pages[name]={'beforeObjects':len(old),'afterObjects':len(new),'packedObjectByteDelta':0}
fixtures=record['fixedPresets'];assert [f['selector'] for f in fixtures]==['stock-999','stock-1000','stock-9999','stock-max','story-early','story-finale']
result={'beforeRows':len(before),'afterRows':len(after),'addedRows':1,'newFactoryMonoScript':added,'allExistingPathGuidTypeMembershipUnchanged':True,'pageDeltas':pages,'sixPureEditorCandidatesChecked':fixtures,'candidateCheckIsNotAndroidActivation':True}
(QA/'PackedComparison.json').write_text(json.dumps(result,indent=2)+'\n');print('Exact one factory MonoScript added; existing membership/page sizes retained.')
