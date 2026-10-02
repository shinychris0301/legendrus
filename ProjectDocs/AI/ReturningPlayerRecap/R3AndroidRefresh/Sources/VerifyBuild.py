"""Verify build/package/source preservation before the current offline preflight trusts this APK."""
from pathlib import Path
import ast
import datetime
import hashlib
import json
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream,'sha256').hexdigest()


def read(name):
    return json.loads((QA/name).read_text())


def validate():
    for path in BASE.rglob('*.json'):
        json.loads(path.read_text())
    for path in (BASE/'Sources').glob('*.py'):
        ast.parse(path.read_text(),filename=str(path))
    policy=read('Policy/summary.json')
    cases=ET.parse(QA/'Policy/results.xml').findall('.//test-case')
    assert policy['exitCode']==0 and not policy['nonPassing'] and len(cases)==130
    assert len({c.get('fullname') for c in cases})==130 and all(c.get('result')=='Passed' for c in cases)
    assert all(int(policy['result'][key])==0 for key in ['failed','skipped','inconclusive'])
    assert read('Build/summary.json')['exitCode']==0
    packed=read('PackedAssets.json')
    assert packed['result']=='Succeeded' and packed['unityVersion']=='6000.5.9f1'
    assert [packed[key] for key in ['profileRevision','dialogueRevision','gameplayRevision','victoryRevision','stockLayouts','storyScenes','storyMilestones']]==[13,2,24,12,4,58,57]
    before=read('BeforeHashes.json')
    after={str(p.relative_to(ROOT)):sha(p) for directory in ['Assets','Packages','ProjectSettings','Backend']
           for p in (ROOT/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts and
           p.suffix.lower() not in ['.keystore','.jks','.p12','.pem','.key'] and not p.name.startswith('.env')}
    assert before==after and len(before)==7833
    scope=read('Scope.json')
    assert scope['baselineFiles']==scope['finalFiles']==scope['unchanged']==7833
    assert not scope['changed'] and not scope['added'] and not scope['deleted']
    helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/StoryRecapRefreshBuildAudit.cs'
    assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
    assert not list(BASE.rglob('__pycache__'))
    inputs=read('InstalledRecapInputs.json')
    for path,pin in inputs['installedR2Inputs'].items():
        assert before[path]==after[path]==pin
    assert inputs['precedingUnityChecks']==49 and inputs['precedingChecksAreHistorical'] and not inputs['newRuntimeCaptures']
    artifact=read('Artifact.json');apk=ROOT/artifact['path']
    assert apk.stat().st_size==artifact['bytes']==766017314 and sha(apk)==artifact['sha256']
    assert artifact['sha256']=='2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e'
    assert apk.with_suffix('.apk.sha256').read_text().strip()==artifact['sha256']+'  '+apk.name
    prior=read('PreviousArtifact.json');old=ROOT/prior['path']
    assert old.stat().st_size==prior['bytes']==765764694
    assert sha(old)==prior['sha256']=='42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5'
    assert old.with_suffix('.apk.sha256.inherited').read_text().strip()==prior['sha256']+'  '+old.name
    verification=read('ApkVerification.json')
    assert verification['sha256']==artifact['sha256'] and verification['bytes']==artifact['bytes']
    assert verification['signatureVerified'] and verification['sameCertificateAsPriorLocalApk']
    assert verification['zipCrcPassed'] and verification['zip16KiBAlignmentPassed']
    assert len(verification['elfLibraries'])==6 and all(verification['runtimeMetadataMarkers'].values())
    assert all(verification['recapRuntimeTypes'].values()) and verification['editorAndTestHelpersAbsentFromPlayerMetadata']
    assert all(min(row['loadSegmentAlignments'])>=16384 for row in verification['elfLibraries'])
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
    assert membership['beforeRows']==29384 and membership['afterRows']==29386 and membership['addedRows']==2
    assert membership['allExistingPathGuidTypeMembershipUnchanged'] and not membership['removedPaths']
    assert len(membership['newRecapMonoScripts'])==2
    assert all(row['beforeObjects']==row['afterObjects'] and row['packedObjectByteDelta']==0 for row in membership['pageDeltas'].values())
    dex=read('CredentialDex.json')
    assert dex['jniEntryDefinitionsVerified'] and dex['operationFieldDefinitionsVerified'] and len(dex['compiledCredentialClasses'])==9
    for path,digest in read('PinnedPluginInventory.json').items():
        assert sha(ROOT/path)==digest
    assert not artifact['installed'] and not artifact['deviceTested']
    allowed=['Assets/AddressableAssetsData/Android/addressables_content_state.bin',
             'Assets/AddressableAssetsData/Android/addressables_content_state.bin.meta',
             'Assets/AddressableAssetsData/link.xml','Assets/AddressableAssetsData/link.xml.meta']
    assert all(row['path'] in allowed for row in read('GeneratedFileCleanup.json'))
    inventory=json.loads((ROOT/'ProjectDocs/AI/ChapterOneHumanAcceptance/R1/QA/EvidenceInventory.json').read_text())
    recordings={f['path']:f['sha256'] for f in inventory['rawFiles'] if Path(f['path']).is_absolute()}
    folder=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests')
    assert recordings=={str(p):sha(p) for p in folder.rglob('*.json')} and len(recordings)==5
    result={'checkedAtUtc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'result':'Passed',
            'currentPolicyChecksPassed':130,'successfulUnityBuildAndPackedReportVerified':True,
            'all7833ProjectInputsByteIdentical':True,'profileRevision':13,'dialogueRevision':2,
            'gameplayRevision':24,'victoryRevision':12,'preceding49UnityChecksAreHistorical':True,
            'newPlayModeOrCaptures':False,'temporaryBuildHelperAbsent':True,'taskPythonCacheAbsent':True,
            'currentApkAndSidecarVerified':True,'archivedR6ApkAndSidecarVerified':True,
            'exactNewRecapMonoScripts':2,'allExistingPackedMembershipUnchanged':True,
            'all403ZipPathsRetained':True,'unchangedPayloadCount':398,'reviewedPayloadChanges':5,
            'nativeEngineBinaryIdentical':True,'normalManualRecordingsUnchanged':5,
            'artifactSha256':artifact['sha256'],'artifactBytes':artifact['bytes'],
            'physicalDeviceExecution':False,'hostingDeploymentOrCommit':False}
    (QA/'BuildValidation.json').write_text(json.dumps(result,indent=2)+'\n')
    return result


if __name__=='__main__':
    print(json.dumps(validate(),indent=2))
