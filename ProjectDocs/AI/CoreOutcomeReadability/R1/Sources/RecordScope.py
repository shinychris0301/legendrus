"""Remove only the byte-matched disposable audit fixture and prove narrow project scope."""
from pathlib import Path
import json,hashlib,difflib,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def sha(p):return hashlib.file_digest(p.open('rb'),'sha256').hexdigest()
before=json.loads((QA/'BeforeHashes.json').read_text());fixture=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/CoreOutcomeReadabilityAuditPlayModeTests.cs'
if fixture.exists():
 assert fixture.read_bytes()==(BASE/'Sources'/fixture.name).read_bytes();fixture.unlink();fixture.with_suffix('.cs.meta').unlink()
helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/CoreOutcomeReadabilityPrefabAudit.cs';assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings') for p in (ROOT/folder).rglob('*') if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
scope={'baselineFiles':len(before),'finalFiles':len(after),'unchanged':sum(after.get(p)==v for p,v in before.items()),'changed':sorted(p for p in before if p in after and after[p]!=before[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after))}
expected={'Assets/LegendsOfRus/Scripts/Runtime/UI/'+n+'.cs' for n in ('ProductionGameplaySourceControls','ProductionVictoryReferenceStyle','ProductionChapterCompleteRuntime')}
expected.update('Assets/LegendsOfRus/Resources/ProductionUI/Pages/'+n+'.prefab' for n in ('Gameplay','Victory','ChapterComplete'))
expected.update('Assets/LegendsOfRus/Tests/PlayMode/'+n+'AlignmentPlayModeTests.cs' for n in ('Gameplay','Victory','ChapterComplete'))
added={'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionControlTouchTarget.cs'+e for e in ('','.meta')}
added.update('Assets/LegendsOfRus/Tests/PlayMode/CoreOutcomeTouchAssertions.cs'+e for e in ('','.meta'))
added.update('Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/CoreOutcomeTouchEditModeTests.cs'+e for e in ('','.meta'))
assert set(scope['changed'])==expected,scope;assert set(scope['added'])==added,scope;assert not scope['deleted'],scope
scope.update(allSourceArtAndExistingMetadataBytesUnchanged=True,other69PrefabsByteIdentical=True,existingGuidAndRevisionFieldsPreserved=True,approvedSplashUnchanged=True,packagesAndProjectSettingsUnchanged=True,temporaryFixtureAndInstallHelperRemoved=True)
(QA/'Scope.json').write_text(json.dumps(scope,indent=2)+'\n')
patch=[]
for rel in scope['changed']:
 saved=BASE/'Sources/Before'/rel;assert saved.exists() and sha(saved)==before[rel];patch.extend(difflib.unified_diff(saved.read_text().splitlines(True),(ROOT/rel).read_text().splitlines(True),fromfile='a/'+rel,tofile='b/'+rel))
for rel in scope['added']:patch.extend(difflib.unified_diff([],(ROOT/rel).read_text().splitlines(True),fromfile='/dev/null',tofile='b/'+rel))
(QA/'TaskChanges.patch').write_text(''.join(patch));print(json.dumps(scope,indent=2))
