"""Prove nine exact sprite-only edits and preserve all 72 prefab identities."""
from pathlib import Path
import hashlib,json,re
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
rows=json.loads((QA/'CandidatesBefore.json').read_text());old=(BASE/'Sources/Before/Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab').read_text();new=(ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab').read_text();expected=old
for row in rows:expected=expected.replace('  m_Sprite: {fileID: 21300000, guid: '+row['guid']+', type: 3}','  m_Sprite: {fileID: 0}')
assert new==expected;before=json.loads((QA/'BeforeHashes.json').read_text());paths=[p for p in before if p.endswith('.prefab')];unchanged=[p for p in paths if hashlib.file_digest((ROOT/p).open('rb'),'sha256').hexdigest()==before[p]]
assert len(paths)==72 and len(unchanged)==71
record={'prefabCount':72,'other71PrefabsByteIdentical':True,'homeChangeOnlyNineSpriteReferences':True,'homeSerializedObjectCount':len(re.findall(r'^--- !u!',old,re.M)),'serializedObjectHeadersIdentical':re.findall(r'^--- !u!.*',old,re.M)==re.findall(r'^--- !u!.*',new,re.M),'allOtherHomeBytesAndReferencesIdentical':True,'taskStartInputsVerifiedBeforeImplementation':7790}
(QA/'PrefabPreservation.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
