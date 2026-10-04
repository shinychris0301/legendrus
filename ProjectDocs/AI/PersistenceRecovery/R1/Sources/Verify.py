"""Reconcile Unity evidence, narrow source ownership and unchanged packaged APK."""
from pathlib import Path
import ast,difflib,hashlib,json,subprocess,xml.etree.ElementTree as ET,zipfile
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
before=read('BeforeHashes.json');after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings','Backend') for p in (ROOT/folder).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
expected=sorted(['Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs'])
changed=sorted(n for n in before if after.get(n)!=before[n]);assert set(before)==set(after) and len(before)==7839 and changed==expected,(changed,expected)
patch=''
for n in changed:
 old=BASE/'Sources/Before'/n;assert sha(old)==before[n]
 patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),(ROOT/n).read_text().splitlines(True),fromfile='a/'+n,tofile='b/'+n))
(BASE/'Sources/TaskChanges.patch').write_text(patch)
subprocess.run(['git','apply','--reverse','--check','-'],input=patch,text=True,cwd=ROOT,check=True,capture_output=True)
def original(n):return (BASE/'Sources/Before'/n).read_text(),(ROOT/n).read_text()
def region(s,start,end):return s[s.index(start):s.index(end,s.index(start))]
def strip(s,start,end):return s.replace(region(s,start,end),'',1)
a,b=original('Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs')
b=b.replace('        bool persistenceInProgress;\n','',1)
assert a[:a.index('        public void Persist()')]==b[:b.index('        public void Persist()')]
assert a[a.rfind('    }'):]==b[b.rfind('    }'):]
a,b=original('Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs')
marker='        [TestCase(false,false)] [TestCase(false,true)] [TestCase(true,false)] [TestCase(true,true)]';assert b.count(marker)==1
assert b[:b.index(marker)].rstrip()+'\n'+b[b.rfind('    }'):]==a[:a.rfind('    }')].rstrip()+'\n'+a[a.rfind('    }'):]
assert read('Reproduce/summary.json')['result']['failed']=='6'
counts={};names=set();modes={'PlayMode':set(),'EditMode':set()}
for group in read('FinalGroups.json'):
 s=read(group+'/summary.json');cases=ET.parse(QA/group/'results.xml').findall('.//test-case');groupnames={c.get('fullname') for c in cases}
 assert s['exitCode']==0 and s['result']['failed']==s['result']['skipped']=='0'
 assert len(cases)==len(groupnames) and all(c.get('result')=='Passed' for c in cases)
 assert not names.intersection(groupnames);names.update(groupnames);modes[s['mode']].update(groupnames);counts[group]=len(cases)
 assert s['isolatedConfig'].startswith('/tmp/lor-continuation-r1/PersistenceR1Retained/' if group=='Retained' else '/tmp/lor-persistence-recovery-r1/')
for group,num in [('Focus',6),('BoundariesCorrected',13)]:
 s=read(group+'/summary.json');focused={c.get('fullname') for c in ET.parse(QA/group/'results.xml').findall('.//test-case')};assert len(focused)==num and focused<=names and s['exitCode']==0
artifact=read('ArtifactBefore.json');apk=ROOT/artifact['path'];assert sha(apk)==artifact['sha256'] and apk.stat().st_size==artifact['bytes'] and Path(str(apk)+'.sha256').read_text()==artifact['sidecar']
with zipfile.ZipFile(apk) as z:assert b'TryPersist\0' not in z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
for p in (BASE/'Sources').glob('*.py'):ast.parse(p.read_text(),filename=str(p))
subprocess.run(['git','diff','--check'],cwd=ROOT,check=True,capture_output=True)
assert not list((ROOT/'Assets').glob('InitTestScene*.unity'))
result={'result':'Passed','freshUnityGroupCounts':counts,'unityPlayDistinctPassed':len(modes['PlayMode']),'unityEditDistinctPassed':len(modes['EditMode']),'unityTotalDistinctPassed':len(names),'failed':0,'skipped':0,'preFixFailures':6,'focusedChecksIncludedOnce':True,'ordinaryPersistenceHonorsSessionWriteLock':True,'legacyChangedReentryRejected':True,'healthySaveAndProgressObjectIdentityPreserved':True,'resetHistoryReplacementPolicyUnchanged':True,'healthyLegacyAndBackupStartupRewritePassed':True,'startupRewriteExhaustionPreservesJournalAndLocksOrdinaryRetry':True,'changedProductionInputs':changed,'projectInputsUnchanged':len(before)-len(changed),'addedProductionInputs':[],'deletedProductionInputs':[],'gameStateOutsideGeneralPersistenceAndPrivateBusyFlagBytePreserved':True,'existingStartupMigrationAndResetMethodsBytePreserved':True,'existingSettingsPrefabBuilderRevisionAndBindingsPreserved':True,'existingFixtureChecksBytePreserved':True,'allExistingMetasArtPrefabsBuildersSettingsPackagesBackendAndBuildGuardsPreserved':True,'taskPatchReverseCheckPassed':True,'currentApkBytes':artifact['bytes'],'currentApkSha256':artifact['sha256'],'apkAndSidecarUnchanged':True,'persistenceFixNotYetPackaged':True,'human201To210AcceptanceStillPending':True,'userPreviouslyConfirmedStillUnrecorded':True,'freshStateRecoveryIsSameProcessUnityEvidence':True,'normalPersonalJournalRead':False,'physicalGameplayTested':False,'newAndroidBuild':False,'phoneOrHostingAction':False,'gitCommitOrPushPerformed':False}
(QA/'TestReconciliation.json').write_text(json.dumps({'result':'Passed','groups':counts,'playPassed':len(modes['PlayMode']),'editPassed':len(modes['EditMode']),'totalDistinctPassed':len(names),'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False},indent=2)+'\n')
(QA/'SourceValidation.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result,indent=2))
