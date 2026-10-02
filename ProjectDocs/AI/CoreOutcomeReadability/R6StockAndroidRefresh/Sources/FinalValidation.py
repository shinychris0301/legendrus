"""Reconcile guarded build, package inclusion, archive and unchanged local inputs; no device work."""
from pathlib import Path
import ast, datetime, hashlib, json, re
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
def read(name):return json.loads((QA/name).read_text())
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
for path in BASE.rglob('*.json'):json.loads(path.read_text())
for path in (BASE/'Sources').glob('*.py'):ast.parse(path.read_text(),filename=str(path))
policy=read('Policy/summary.json');cases=ET.parse(QA/'Policy/results.xml').findall('.//test-case')
assert policy['exitCode']==0 and not policy['nonPassing'] and len(cases)==130
assert len({c.get('fullname') for c in cases})==130 and all(c.get('result')=='Passed' for c in cases)
assert all(int(policy['result'][k])==0 for k in ('failed','skipped','inconclusive'))
assert read('Build/summary.json')['exitCode']==0
packed=read('PackedAssets.json')
assert packed['result']=='Succeeded' and packed['gameplayRevision']==24 and packed['stockLayouts']==4
assert packed['unityVersion']=='6000.5.9f1'
before=read('BeforeHashes.json')
after={str(p.relative_to(ROOT)):sha(p) for directory in ('Assets','Packages','ProjectSettings','Backend')
       for p in (ROOT/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts and
       p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
assert before==after and len(before)==7825
scope=read('Scope.json')
assert scope['baselineFiles']==scope['finalFiles']==scope['unchanged']==7825
assert not scope['changed'] and not scope['added'] and not scope['deleted']
assert not (ROOT/'Assets/LegendsOfRus/Scripts/Editor/GameplayStockRefreshBuildAudit.cs').exists()
assert not list((BASE/'Sources').rglob('__pycache__'))
inputs=read('InstalledStockInputs.json')
for path,pin in inputs['installedR5Inputs'].items():
    assert before[path]==after[path]==pin['sha256']
    if 'revision' in pin:assert f"  Revision: {pin['revision']}\n" in (ROOT/path).read_text()
assert inputs['precedingUnityChecks']==36 and inputs['precedingChecksAreHistorical'] and not inputs['newRuntimeCaptures']
artifact=read('Artifact.json');apk=ROOT/artifact['path']
assert apk.stat().st_size==artifact['bytes'] and sha(apk)==artifact['sha256']
assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
prior=read('PreviousArtifact.json');old=ROOT/prior['path']
assert old.stat().st_size==prior['bytes']==765772146
assert sha(old)==prior['sha256']=='924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff'
assert old.with_suffix('.apk.sha256.inherited').read_text().split()[0]==prior['sha256']
verification=read('ApkVerification.json')
assert verification['sha256']==artifact['sha256'] and verification['bytes']==artifact['bytes']
assert verification['signatureVerified'] and verification['sameCertificateAsPriorLocalApk']
assert verification['zipCrcPassed'] and verification['zip16KiBAlignmentPassed']
assert len(verification['elfLibraries'])==6 and all(verification['runtimeMetadataMarkers'].values())
assert all(min(l['loadSegmentAlignments'])>=16384 for l in verification['elfLibraries'])
comparison=read('ArtifactComparison.json')
assert comparison['old']['sha256']==prior['sha256'] and comparison['new']['sha256']==artifact['sha256']
assert not comparison['addedEntries'] and not comparison['removedEntries'] and comparison['permissionsUnchanged']
assert comparison['new']['unindexedBytes']==0 and comparison['old']['exactAccounting'] and comparison['new']['exactAccounting']
assert len(comparison['new']['entries'])==len(comparison['old']['entries'])==403
review=read('PayloadReview.json')
assert review['changedPayloadCount']==5 and review['unchangedPayloadCount']==398
assert review['bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged'] and review['allFourCredentialJavaSourceMetadataPinsUnchanged']
native=read('NativeEngineComparison.json')
assert native['binaryIdentical'] and native['dynamicSymbolDefinitionsUnchanged']
assert not native['addedBindings'] and not native['removedBindings']
membership=read('PackedComparison.json')
assert membership['beforeRows']==29363 and membership['afterRows']==29384 and membership['addedRows']==21
assert membership['allOtherPathGuidTypeMembershipUnchanged'] and not membership['removedPaths']
assert membership['pageDeltas']['Gameplay']['beforeObjects']==429 and membership['pageDeltas']['Gameplay']['afterObjects']==449
assert membership['newStockMonoScript']['type']=='MonoScript'
dex=read('CredentialDex.json')
assert dex['jniEntryDefinitionsVerified'] and dex['operationFieldDefinitionsVerified'] and len(dex['compiledCredentialClasses'])==9
for path,digest in read('PinnedPluginInventory.json').items():assert sha(ROOT/path)==digest
assert not artifact['installed'] and not artifact['deviceTested']
cleanup=read('GeneratedFileCleanup.json')
assert all(row['path'] in ('Assets/AddressableAssetsData/Android/addressables_content_state.bin',
                         'Assets/AddressableAssetsData/Android/addressables_content_state.bin.meta',
                         'Assets/AddressableAssetsData/link.xml','Assets/AddressableAssetsData/link.xml.meta') for row in cleanup)
inventory=json.loads((ROOT/'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json').read_text())
old_recordings={f['path']:f['sha256'] for f in inventory['rawFiles'] if Path(f['path']).is_absolute()}
folder=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
current_recordings={str(p):sha(p) for p in folder.rglob('*.json')}
assert old_recordings==current_recordings
output=QA/'FinalValidation.json';links=[]
report=BASE/'TestResults.md'
for target in re.findall(r'\]\(([^)]+)\)',report.read_text()):
    if target.startswith(('http:','https:','#')):continue
    path=(report.parent/target.split('#')[0]).resolve()
    assert path.exists() or path==output,(report,target)
    links.append(path)
result={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'result':'Passed',
        'currentPolicyChecksPassed':130,'successfulUnityBuildAndPackedReportVerified':True,
        'all7825ProjectInputsByteIdentical':True,'installedGameplayRevision':24,'boundStockLayouts':4,
        'preceding36RuntimeChecksAreHistorical':True,'newPlayModeOrCaptures':False,
        'temporaryBuildHelperAbsent':True,'taskPythonCacheAbsent':True,
        'currentApkAndSidecarVerified':True,'archivedR4ApkAndSidecarVerified':True,
        'exactGameplayObjectAdditions':20,'exactNewStockMonoScripts':1,'unrelatedPackedMembershipUnchanged':True,
        'all403ZipPathsRetained':True,'unchangedPayloadCount':398,'reviewedPayloadChanges':5,
        'nativeEngineBinaryIdentical':True,'normalManualRecordingsUnchanged':len(current_recordings),
        'newDocumentationLinksResolved':len(links),'artifactSha256':artifact['sha256'],'artifactBytes':artifact['bytes'],
        'physicalDeviceExecution':False,'hostingDeploymentOrCommit':False}
output.write_text(json.dumps(result,indent=2)+'\n')
assert all(path.exists() for path in links)
print(json.dumps(result,indent=2))
