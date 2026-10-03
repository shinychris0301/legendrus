"""Reconcile fresh build, archive, source scope and packaged fixed presets; no device access."""
from pathlib import Path
import hashlib
import json
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
QA = Path(__file__).resolve().parent.parent / 'QA'
PRESETS = ['stock-999', 'stock-1000', 'stock-9999', 'stock-max', 'story-early', 'story-finale']
read = lambda name: json.loads((QA / name).read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

policy = read('Policy/summary.json')
cases = ET.parse(QA / 'Policy/results.xml').getroot().findall('.//test-case')
assert policy['exitCode'] == 0 and len(cases) == 153
assert len({case.attrib['fullname'] for case in cases}) == 153
assert all(case.attrib['result'] == 'Passed' for case in cases)
guards = read('EntryGuards/summary.json')
assert guards['passed'] == 120 and guards['failed'] == 0
assert len(guards['checks']) == 120 and all(c['passed'] for c in guards['checks'])
assert read('BuildVerified/summary.json')['exitCode'] == 0
assert read('Build/summary.json')['exitCode'] != 0  # Initial audit attempt remains visible.

artifact = read('Artifact.json')
for row in [artifact, read('PreviousArtifact.json'), read('InitialArtifact.json')]:
    path = ROOT / row['path']
    assert path.stat().st_size == row['bytes'] and sha(path) == row['sha256']
apk = ROOT / artifact['path']
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
old = read('PreviousArtifact.json')
assert old['bytes'] == 766017314 and old['sha256'] == '2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e'
assert Path(str(ROOT / old['path']) + '.sha256.inherited').read_text().strip() == old['sha256'] + '  ' + apk.name
assert read('InitialArtifact.json')['accepted'] is False

verification = read('ApkVerification.json')
assert all(verification[key] == artifact[key] for key in ('bytes', 'sha256'))
assert all(verification[key] is True for key in ('signatureVerified', 'sameCertificateAsPriorLocalApk',
    'zipCrcPassed', 'zip16KiBAlignmentPassed', 'editorAndTestHelpersAbsentFromPlayerMetadata'))
assert verification['presetRuntimeTypes']['DevelopmentGameplayFixtures'] is True
assert all(verification['presetSelectors'].get(key) is True for key in PRESETS)
assert verification['physicalDeviceTested'] is False
packed = read('PackedAssets.json')
assert packed['result'] == 'Succeeded' and packed['unityVersion'] == '6000.5.9f1'
assert [packed[k] for k in ('profileRevision', 'dialogueRevision', 'gameplayRevision', 'victoryRevision',
    'stockLayouts', 'storyScenes', 'storyMilestones')] == [13, 2, 24, 12, 4, 58, 57]
assert [r['selector'] for r in packed['fixedPresets']] == PRESETS
assert [r['completedRounds'] for r in packed['fixedPresets']] == [0, 0, 0, 0, 8, 500]
assert [r['acknowledgedScenes'] for r in packed['fixedPresets']] == [0, 0, 0, 0, 3, 58]
assert all(r['energy'] == 4 for r in packed['fixedPresets'])
assert [r['stocks'] for r in packed['fixedPresets'][:4]] == [[n] * 4 for n in (999, 1000, 9999, 2147483647)]
membership = read('PackedComparison.json')
assert membership['beforeRows'] == 29386 and membership['afterRows'] == 29387
assert membership['allExistingPathGuidTypeMembershipUnchanged'] is True
assert all(r['packedObjectByteDelta'] == 0 for r in membership['pageDeltas'].values())
scope = read('Scope.json')
assert scope['baselineFiles'] == scope['finalFiles'] == scope['unchanged'] == 7839
assert not scope['added'] and not scope['changed'] and not scope['deleted']
before = read('BeforeHashes.json')
assert len(before) == 7839 and all(sha(ROOT / p) == digest for p, digest in before.items())
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/FixedPresetsRefreshBuildAudit.cs'
assert not helper.exists() and not Path(str(helper) + '.meta').exists()
comparison = read('ArtifactComparison.json')
assert not comparison['addedEntries'] and not comparison['removedEntries']
assert comparison['permissionsUnchanged'] is True and comparison['identicalPayloadCount'] == 398
assert all(r['exactAccounting'] and r['unindexedBytes'] == 0 for r in (comparison['old'], comparison['new']))
payload = read('PayloadReview.json')
assert payload['changedPayloadCount'] == 5 and payload['allFourCredentialJavaSourceMetadataPinsUnchanged'] is True
assert read('NativeEngineComparison.json')['binaryIdentical'] is True
assert len(read('CredentialDex.json')['compiledCredentialClasses']) == 9
result = {'result': 'Passed', 'artifactSha256': artifact['sha256'], 'artifactBytes': artifact['bytes'],
    'unityPolicyContentPresetDistinctPassed': 153, 'compiledSyntheticGuardChecksPassed': 120,
    'projectInputsUnchanged': 7839, 'fixedPresetsPackaged': PRESETS,
    'originalApkAndInheritedSidecarPreserved': True, 'initialRejectedCandidatePreserved': True,
    'samePublicSigningCertificateAsOriginal': True, 'existingPackedMembershipPreserved': True,
    'physicalGameplayTested': False, 'normalPersonalSaveRead': False,
    'productionReleaseBuilt': False, 'newPlayModeRun': False}
(QA / 'BuildValidation.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
