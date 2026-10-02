"""Require the exact projection/reader MonoScripts; preserve existing serialized membership."""
from collections import Counter
from pathlib import Path
import json
import re

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
before=json.loads((ROOT/'ProjectDocs/AI/CoreOutcomeReadability/R6StockAndroidRefresh/QA/PackedAssets.json').read_text())['assets']
record=json.loads((QA/'PackedAssets.json').read_text())
assert record['result']=='Succeeded' and record['profileRevision']==13 and record['dialogueRevision']==2
assert record['gameplayRevision']==24 and record['victoryRevision']==12 and record['stockLayouts']==4
assert record['storyScenes']==58 and record['storyMilestones']==57
after=record['assets']
scripts=['Assets/LegendsOfRus/Scripts/Runtime/Services/StoryRecap.cs',
         'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionStoryRecapView.cs']
key=lambda row:(row['path'],row['guid'],row['type'])
assert Counter(map(key,before))==Counter(key(row) for row in after if row['path'] not in scripts)
added=[]
for script in scripts:
    rows=[row for row in after if row['path']==script]
    assert len(rows)==1 and rows[0]['type']=='MonoScript'
    guid=re.search(r'^guid: (\w+)',(ROOT/(script+'.meta')).read_text(),re.M).group(1)
    assert rows[0]['guid']==guid
    added.extend(rows)
assert len(after)-len(before)==2
pages={}
for name in ['Profile','Dialogue','Gameplay','Victory']:
    path='Assets/LegendsOfRus/Resources/ProductionUI/Pages/'+name+'.prefab'
    old=[r for r in before if r['path']==path];new=[r for r in after if r['path']==path]
    assert Counter(map(key,old))==Counter(map(key,new))
    pages[name]={'beforeObjects':len(old),'afterObjects':len(new),
                 'packedObjectByteDelta':sum(r['bytes'] for r in new)-sum(r['bytes'] for r in old)}
result={'beforeRows':len(before),'afterRows':len(after),'addedRows':2,
        'removedPaths':[],'addedPaths':scripts,'pageDeltas':pages,
        'newRecapMonoScripts':added,'allExistingPathGuidTypeMembershipUnchanged':True,
        'successfulUnityBuildReportVerified':True,'profileRevision':13,'dialogueRevision':2,
        'gameplayRevision':24,'victoryRevision':12,'stockLayouts':4,
        'packedObjectBytesAreNotCompressedApkOrDeviceMemory':True}
(QA/'PackedComparison.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
