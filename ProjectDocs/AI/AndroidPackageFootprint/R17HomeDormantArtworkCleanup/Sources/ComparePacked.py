"""Verify only the nine approved candidate paths leave packed membership."""
from pathlib import Path
from collections import Counter
import json
BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
before=json.loads((BASE.parent/'R16HomeWorldMapFooterCleanup/QA/PackedAssets.json').read_text())['assets'];after=json.loads((QA/'PackedAssets.json').read_text())['assets']
candidates={'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/'+n+'.png' for n in ('nav_map_generated', 'nav_shop_generated', 'nav_events_generated', 'nav_characters_generated', 'mail_medallion_generated', 'reward_medallion_generated', 'quest_reward_frame', 'quest_reward_gem', 'home_map_foliage')}
retired={'51e28d72f0853db939602ec4f6546f7c','82085c57945c64ecab5231171e999c10','f83c954371cabff0f9a3fad969811846','03d94585ef770c0a9aca2184b5239550','bbb3a20b4740b869c94d2023f0da4929','7b33517046ee539d4a86ecfd63012ea5','072000aa439cc61468feba1c5d67b108','50ba28375a9a4edb1be246997041faea','4585318c279622051a6d988eb18e05e3'}
retired.update(['c799ce05d21b30886a6fa33a9c3e4c33', '4bbb7d70ab8e8c29385b536e7e8dbf2f', 'b22f4fdf95dddd63f8c32fab7a753da0', '69135cdae1222fa598c05c4ab1539397', '23aee3e96f6777c45a4cae8a64c33c4c', '717db484c9b631d909a47c6df0c87daf', 'd76c3dc6c6b59ff2ca8b7698b698ec03', '2cb9251bbe55ba200b4383179b701814', '94bad924675b43dcadb2595547a51117'])
old_paths={r['path'] for r in before if r['path']};new_paths={r['path'] for r in after if r['path']}
assert old_paths-new_paths==candidates;assert not new_paths-old_paths;assert not any(r['guid'] in retired for r in after)
key=lambda r:(r['path'],r['guid'],r['type'])
assert Counter(key(r) for r in before if r['path'] not in candidates)==Counter(key(r) for r in after)
rows=[r for r in before if r['path'] in candidates]
record={'beforeRows':len(before),'afterRows':len(after),'removedPaths':sorted(candidates),'addedPaths':[],'removedRows':rows,'removedPackedObjectBytes':sum(r['bytes'] for r in rows),'otherObjectMembershipUnchanged':True,'eighteenRetiredGuidsExcluded':True,'compressedSavingsEvidence':'ArtifactComparison.json'}
(QA/'PackedComparison.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
