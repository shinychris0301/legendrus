"""Reconcile the guarded build, preserved R5, native claim code and unchanged project inputs."""
from pathlib import Path
import hashlib
import json
import re
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
read = lambda name: json.loads((QA / name).read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

policy = read('Policy/summary.json')
cases = ET.parse(QA / 'Policy/results.xml').getroot().findall('.//test-case')
assert policy['exitCode'] == 0 and len(cases) == len({c.get('fullname') for c in cases}) == 160
assert all(c.get('result') == 'Passed' for c in cases)
assert read('Build/summary.json')['exitCode'] == 0
scope = read('Scope.json')
assert scope['baselineFiles'] == scope['finalFiles'] == scope['unchanged'] == 7839
assert not scope['changed'] and not scope['added'] and not scope['deleted']
assert all(sha(ROOT / p) == h for p, h in read('BeforeHashes.json').items())
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/DailyRewardRefreshBuildAudit.cs'
assert not helper.exists() and not Path(str(helper) + '.meta').exists()
artifact = read('Artifact.json'); original = read('PreviousArtifact.json')
for row in (artifact, original):
    path = ROOT / row['path']
    assert path.stat().st_size == row['bytes'] and sha(path) == row['sha256']
assert original['bytes'] == 766013502 and original['sha256'] == '91c38f1a6ae63482d8d9e713dcb8f0f891b9b0a67e8743b30de390ab906dc8a9'
apk = ROOT / artifact['path']
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
assert Path(str(ROOT / original['path']) + '.sha256.inherited').read_text().strip() == original['sha256'] + '  ' + apk.name
inspection = read('ApkVerification.json')
assert all(inspection[k] == artifact[k] for k in ('bytes', 'sha256'))
assert all(inspection[k] is True for k in ('signatureVerified', 'sameCertificateAsPriorLocalApk', 'zipCrcPassed',
    'zip16KiBAlignmentPassed', 'editorAndTestHelpersAbsentFromPlayerMetadata'))
assert inspection['package'] == 'com.example.legendsofrus.v117debug' and inspection['debuggable'] is True
assert len(inspection['elfLibraries']) == 6 and all(inspection['presetSelectors'].values())
comparison = read('ArtifactComparison.json')
assert not comparison['addedEntries'] and not comparison['removedEntries'] and comparison['permissionsUnchanged']
assert comparison['identicalPayloadCount'] == 400 and comparison['unityPayloadUnchanged']
assert {r['path'] for r in comparison['changedPayloads']} == {'assets/bin/Data/boot.config',
    'assets/bin/Data/unity_app_guid', 'lib/arm64-v8a/libil2cpp.so'}
assert all(r['exactAccounting'] and r['unindexedBytes'] == 0 for r in (comparison['old'], comparison['new']))
packed = read('PackedAssets.json'); membership = read('PackedComparison.json')
assert packed['result'] == 'Succeeded' and packed['unityVersion'] == '6000.5.9f1'
assert membership['beforeRows'] == membership['afterRows'] == 29387
assert membership['allExistingPathGuidTypeMembershipUnchanged'] is True
assert len(membership['pageDeltas']) == 7 and all(r['packedObjectByteDelta'] == 0 for r in membership['pageDeltas'].values())
assert len(read('CredentialDex.json')['compiledCredentialClasses']) == 9
payload = read('PayloadReview.json')
assert payload['changedPayloadCount'] == 3 and payload['allFourCredentialJavaSourceMetadataPinsUnchanged'] is True
assert read('NativeEngineComparison.json')['binaryIdentical'] is True
native = read('NativeClaimVerification.json')
assert native['compiledOverflowRejectionPackaged'] is True and native['physicalExecutionVerified'] is False
before, after = native['before'], native['after']
assert before['generatedOverflowCatch'] is False and after['generatedOverflowCatch'] is True
assert after['packagedMethodBytesMatchCompiledOutput'] is True and after['packagedLibraryMatchesGradleInput'] is True
assert after['nativeMethodSha256'] != before['nativeMethodSha256']
assert after['packagedLibrarySha256'] == comparison['new']['entries']['lib/arm64-v8a/libil2cpp.so']['sha256']
cpp = (BASE / 'Sources/AfterGeneratedClaim.cpp').read_text()
catch = re.search(r'CATCH_[0-9a-f]+:\s*\{(.*?)\n\t\}', cpp, re.S | re.I)
assert catch and '(bool)0' in catch.group(1) and 'SaveService_TrySave' not in catch.group(1)
assert 'return L_49;' in cpp  # Current SDK-generated shared return for the rejected candidate.
with zipfile.ZipFile(apk) as archive:
    metadata = archive.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
    assert all(name.encode() not in metadata for name in ('DailyRewardRefreshBuildAudit',
        'FixedPresetsRefreshBuildAudit', 'DailyRewardAlignmentPlayModeTests'))
previous_runtime = json.loads((ROOT / 'ProjectDocs/AI/DailyRewardRecovery/R1/QA/FinalValidation.json').read_text())
assert previous_runtime['unityTotalDistinctPassed'] == 21
result = {'result': 'Passed', 'artifactSha256': artifact['sha256'], 'artifactBytes': artifact['bytes'],
    'freshUnityEditDistinctPassed': 160, 'projectInputsUnchanged': 7839,
    'originalR5ApkAndSidecarPreserved': True, 'dailyRewardOverflowRejectionPackaged': True,
    'nativeClaimLibrarySha256': after['packagedLibrarySha256'],
    'nativeClaimMethodBytesMatchBuildOutput': True, 'existingAll29387PackedRowsUnchanged': True,
    'sevenNativePagesPackedSizesUnchanged': True, 'identicalZipPayloads': 400, 'reviewedChangedZipPayloads': 3,
    'priorServiceUnityDistinctPassesHistorical': 21, 'newPlayModeRun': False,
    'normalPersonalJournalRead': False, 'physicalGameplayTested': False,
    'productionReleaseBuilt': False, 'gitCommitOrPushPerformed': False}
(QA / 'BuildValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
