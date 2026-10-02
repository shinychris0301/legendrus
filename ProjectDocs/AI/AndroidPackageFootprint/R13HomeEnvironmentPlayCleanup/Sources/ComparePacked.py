"""Verify only the two approved candidate paths leave packed membership."""
from pathlib import Path
from collections import Counter
import json
BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
before=json.loads((BASE.parent/'R11HomeHeroArchive/QA/PackedAssets.json').read_text())['assets'];after=json.loads((QA/'PackedAssets.json').read_text())['assets']
candidates={'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/'+n+'.png' for n in ('home_environment_generated','home_play_button')}
retired={'f83c954371cabff0f9a3fad969811846','bbb3a20b4740b869c94d2023f0da4929','7b33517046ee539d4a86ecfd63012ea5','072000aa439cc61468feba1c5d67b108','50ba28375a9a4edb1be246997041faea','4585318c279622051a6d988eb18e05e3'}
old_paths={r['path'] for r in before if r['path']};new_paths={r['path'] for r in after if r['path']}
assert old_paths-new_paths==candidates;assert not new_paths-old_paths;assert not any(r['guid'] in retired for r in after)
key=lambda r:(r['path'],r['guid'],r['type'])
assert Counter(key(r) for r in before if r['path'] not in candidates)==Counter(key(r) for r in after)
rows=[r for r in before if r['path'] in candidates]
record={'beforeRows':len(before),'afterRows':len(after),'removedPaths':sorted(candidates),'addedPaths':[],'removedRows':rows,'removedPackedObjectBytes':sum(r['bytes'] for r in rows),'otherObjectMembershipUnchanged':True,'sixRetiredGuidsExcluded':True,'compressedSavingsEvidence':'ArtifactComparison.json'}
(QA/'PackedComparison.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
