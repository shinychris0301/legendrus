"""Check exact source scope, preserved methods and the unchanged APK before documentation."""
from pathlib import Path
import difflib,hashlib,json,re,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
    with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/folder).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
owned=['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs']
changed=sorted(n for n in before if before[n]!=after.get(n));assert changed==sorted(owned) and set(after)==set(before) and len(after)==7839
old=(BASE/'Sources/Before'/owned[0]).read_text();new=(ROOT/owned[0]).read_text()
def methods(s):
    pattern=re.compile(r'^        (?:(?:public|private|internal|protected|static)\s+)*(?:[A-Za-z0-9_<>,]+\s+)+([A-Za-z_]\w*)\s*\([^;{}]*?\)\s*(?:=>[^\n;]+;|\{)',re.M)
    found={}
    for m in pattern.finditer(s):
        key=m.group(1);text=m.group(0)
        if text.rstrip().endswith('{'):
            depth=1;end=m.end()
            while depth:
                if s[end]=='{':depth+=1
                elif s[end]=='}':depth-=1
                end+=1
            text=s[m.start():end]
        found.setdefault(key,[]).append(text)
    return found
a,b=methods(old),methods(new);assert set(a)==set(b)
allowed={'MigrateSave','NormalizeTutorialProgress','NormalizeStoryProgress','NormalizeChapterCompletionProgress','ChapterCompletionPrerequisitesMet'}
actual={n for n in a if a[n]!=b[n]};assert actual==allowed,actual
assert all(a[n]==b[n] for n in a if n not in allowed)
prior_fixture=(BASE/'Sources/Before'/owned[1]).read_text();fixture=(ROOT/owned[1]).read_text()
assert fixture.split('        static System.Reflection.FieldInfo StartupRewriteField')[0].rstrip()==prior_fixture.removesuffix('    }\n}\n').rstrip()
migrate=b['MigrateSave'][0];assert 'Save = migration.Data' not in migrate and 'Persist();' not in migrate
assert 'if (!canPersist || persistenceInProgress) return;' in migrate and 'TryCommitBalance(candidate)' in migrate and 'if (ReferenceEquals(Save, candidate)) saveRewriteRequired = false;' in migrate and 'persistenceInProgress = false;' in migrate
for name in ('NormalizeTutorialProgress','NormalizeStoryProgress','NormalizeChapterCompletionProgress','ChapterCompletionPrerequisitesMet'):assert 'Save.' not in b[name][0] and 'PlayerSaveData save' in b[name][0]
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes'] and Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
previous=ROOT/'ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh';native=json.loads((previous/'QA/AfterNative.json').read_text());cpp=(previous/'Sources/AfterGameState_MigrateSave.cpp').read_text()
assert hashlib.sha256(cpp.removesuffix('\n').encode()).hexdigest()==native['methods']['GameState_MigrateSave']['generatedMethodSha256']
assert 'GameState_Persist_' in cpp and cpp.index('__this->___Save =')<cpp.index('GameState_Persist_',cpp.index('__this->___Save ='))
assert native['methods']['GameState_MigrateSave']['packagedMethodBytesMatchCompiledOutput']
tests=read('TestReconciliation.json');assert tests['result']=='Passed'
patch=[]
for relative in owned:
    patch.extend(difflib.unified_diff((BASE/'Sources/Before'/relative).read_text().splitlines(True),(ROOT/relative).read_text().splitlines(True),fromfile='a/'+relative,tofile='b/'+relative))
(BASE/'Sources/TaskChanges.patch').write_text(''.join(patch));subprocess.run(['git','apply','--reverse','--check',str(BASE/'Sources/TaskChanges.patch')],cwd=ROOT,check=True,capture_output=True)
language_files=['Assets/LegendsOfRus/Scripts/Runtime/Services/LanguagePreference.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsView.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionLanguageSelectView.cs']
assert all(sha(ROOT/n)==before[n] for n in language_files)
session=(ROOT/language_files[1]).read_text();selection=session[session.index('public static void SelectLanguage'):session.index('public static void SignInGuest')]
assert selection.index('SetLanguage(code)')<selection.index('LanguagePreference.Save(languageCode)')
preference=(ROOT/language_files[0]).read_text();assert preference.index('PlayerPrefs.SetString')<preference.index('PlayerPrefs.Save()')
audit={'scope':'source ordering and healthy cross-process behavior; no rejected-language-write reproduction','filesUnchanged':language_files,'sessionSelectionBeforePreferenceWrite':True,'playerPrefsUpdateBeforeFlush':True,'healthyFreshUnityRestartVerified':True,'futureWork':'Acknowledge language saving before session adoption and provide existing native error feedback; retain independent preference/profile ownership.','normalPersonalStorageRead':False}
(QA/'LanguageAudit.json').write_text(json.dumps(audit,indent=2)+'\n')
result={'result':'Passed','changedProductionInputs':changed,'projectInputCount':7839,'projectInputsUnchanged':7837,'inputsAddedOrDeleted':False,'productionOwnerFilesChanged':1,'fixtureFilesChanged':1,'onlyFiveExistingProductionMethodsChanged':sorted(actual),'otherExistingProductionMethodNamesPreserved':len(a)-5,'existingFixtureContentPreserved':True,'unityPlayDistinctPassed':tests['playPassed'],'unityEditDistinctPassed':tests['editPassed'],'unityTotalDistinctPassed':tests['totalDistinctPassed'],'successfulFinalUnityLaunchers':7,'languageHealthyRestartPhaseExecutionsPassed':2,'languageFailureHandlingPending':True,'apkAndSidecarUnchanged':True,'startupFixNotYetPackaged':True,'approvedArtSettingsPackagesBackendGuardsAndPreviousEditsPreserved':True,'normalPersonalJournalRead':False,'gitCommitOrPushPerformed':False,'physicalGameplayTested':False,'human201To210AcceptancePending':True}
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
