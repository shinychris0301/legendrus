"""Compare asset membership against R7; packed sizes are not compressed APK savings."""
from collections import Counter
from pathlib import Path
import json

BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
before=json.loads((BASE.parent/'R7ProgressCleanup/QA/PackedAssets.json').read_text())['assets']
after=json.loads((QA/'PackedAssets.json').read_text())['assets']
candidate='Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_firebird_generated.png'
retired={'072000aa439cc61468feba1c5d67b108','50ba28375a9a4edb1be246997041faea','4585318c279622051a6d988eb18e05e3'}
before_paths={r['path'] for r in before if r['path']}
after_paths={r['path'] for r in after if r['path']}
removed=sorted(before_paths-after_paths);added=sorted(after_paths-before_paths)
assert removed==[candidate],removed
assert not added,added
assert not any(r['guid'] in retired for r in after)
removed_rows=[r for r in before if r['path']==candidate]
# Membership includes duplicate subobjects; ignore size here to expose unexpected object changes.
key=lambda r:(r['path'],r['guid'],r['type'])
old=Counter(key(r) for r in before if r['path']!=candidate)
new=Counter(key(r) for r in after)
assert old==new, 'Additional packed-object membership changed'
result={'beforeRows':len(before),'afterRows':len(after),'removedPaths':removed,'addedPaths':added,
        'removedRows':removed_rows,'removedPackedObjectBytes':sum(r['bytes'] for r in removed_rows),
        'otherObjectMembershipUnchanged':True,'threeRetiredGuidsExcluded':True,
        'compressedSavingsEvidence':'ArtifactComparison.json'}
(QA/'PackedComparison.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
