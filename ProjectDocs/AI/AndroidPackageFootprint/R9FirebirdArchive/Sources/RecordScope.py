"""Restore generated build metadata only, and verify the exact task scope and texture moves."""
from pathlib import Path
import json
import shutil
import difflib
from Build import ROOT, QA, LOGS, GENERATED, hashes, require_idle, save, sha

require_idle()
before=json.loads((QA/'BeforeHashes.json').read_text())
prebuild=json.loads((QA/'BeforeBuildHashes.json').read_text())
cleaned=[]
for relative in GENERATED:
    path=ROOT/relative
    if path.exists() and sha(path)==prebuild.get(relative):continue
    if relative in prebuild:
        saved=LOGS/'Before'/relative
        assert saved.exists() and sha(saved)==prebuild[relative]
        shutil.copy2(saved,path);cleaned.append({'path':relative,'action':'restored pre-build generated metadata'})
    elif path.exists():path.unlink();cleaned.append({'path':relative,'action':'removed new generated metadata'})
after=hashes()
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),
    'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),
    'unchanged':sum(after.get(p)==v for p,v in before.items()),'moves':[]}
for name in ('home_firebird_generated',):
    for extension in ('.png','.png.meta'):
        old='Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/'+name+extension
        new='Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/'+name+extension
        assert before[old]==after[new]
        scope['moves'].append({'from':old,'to':new,'sha256':after[new],'bytesPreserved':True})
expected={'Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs'}
added={x['to'] for x in scope['moves']}|{'Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome.meta'}
added.update('Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/RetiredHomeFirebirdEditModeTests.cs'+s for s in ('','.meta'))
save('Scope.json',scope);save('GeneratedFileCleanup.json',cleaned)
assert set(scope['changed'])==expected,scope['changed']
assert set(scope['added'])==added,scope['added']
assert set(scope['deleted'])=={x['from'] for x in scope['moves']},scope['deleted']
changes=[]
for name in scope['changed']:
    source=QA.parent/'Sources/Before'/name
    assert source.exists() and sha(source)==before[name],name
    changes.extend(difflib.unified_diff(source.read_text().splitlines(True),(ROOT/name).read_text().splitlines(True),fromfile='a/'+name,tofile='b/'+name))
(QA/'TaskChanges.patch').write_text(''.join(changes))
print(json.dumps(scope,indent=2))
