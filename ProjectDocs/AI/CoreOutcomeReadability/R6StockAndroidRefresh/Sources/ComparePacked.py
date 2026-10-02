"""Require the exact twenty Gameplay objects and one new stock MonoScript; retain other membership."""
from collections import Counter
from pathlib import Path
import json, re

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
before=json.loads((ROOT/'ProjectDocs/AI/CoreOutcomeReadability/R4CaptionAndroidRefresh/QA/PackedAssets.json').read_text())['assets']
record=json.loads((QA/'PackedAssets.json').read_text())
assert record['result']=='Succeeded' and record['gameplayRevision']==24 and record['stockLayouts']==4
after=record['assets']
page='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab'
script='Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayStockLayout.cs'
key=lambda row:(row['path'],row['guid'],row['type'])
assert Counter(key(row) for row in before if row['path']!=page)==Counter(key(row) for row in after if row['path'] not in (page,script))
old=[row for row in before if row['path']==page];new=[row for row in after if row['path']==page]
assert {row['guid'] for row in old}=={row['guid'] for row in new}
delta=Counter(row['type'] for row in new);delta.subtract(Counter(row['type'] for row in old))
expected={'GameObject':4,'RectTransform':4,'CanvasRenderer':4,'MonoBehaviour':8}
assert dict(delta)==expected,dict(delta)
added=[row for row in after if row['path']==script]
assert len(added)==1 and added[0]['type']=='MonoScript'
guid=re.search(r'^guid: (\w+)',(ROOT/(script+'.meta')).read_text(),re.M).group(1)
assert added[0]['guid']==guid
assert len(after)-len(before)==21
result={'beforeRows':len(before),'afterRows':len(after),'addedRows':21,
        'removedPaths':[],'addedPaths':[script],
        'pageDeltas':{'Gameplay':{'beforeObjects':len(old),'afterObjects':len(new),'addedObjectTypes':expected,
        'packedObjectByteDelta':sum(r['bytes'] for r in new)-sum(r['bytes'] for r in old)}},
        'newStockMonoScript':added[0], 'allOtherPathGuidTypeMembershipUnchanged':True,
        'successfulUnityBuildReportVerified':True,'gameplayRevision':24,'stockLayouts':4,
        'packedObjectBytesAreNotCompressedApkOrDeviceMemory':True}
(QA/'PackedComparison.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
