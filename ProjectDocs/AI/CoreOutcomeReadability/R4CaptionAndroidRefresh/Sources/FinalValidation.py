"""Cross-check this refresh's package, build, preservation and documentation evidence."""
from pathlib import Path
import ast
import datetime
import hashlib
import json
import re
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'

def read(path):
    return json.loads(path.read_text())

def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()

json_paths = [path for path in BASE.rglob('*.json') if path != QA / 'FinalValidation.json']
for path in json_paths:
    read(path)
for path in BASE.rglob('*.xml'):
    ET.parse(path)
sources = list((BASE / 'Sources').glob('*.py'))
for path in sources:
    ast.parse(path.read_text(), filename=str(path))
policy = read(QA / 'Policy/summary.json')
assert policy['exitCode'] == 0 and not policy['nonPassing']
assert all(int(policy['result'][key]) == 0 for key in ('failed', 'skipped', 'inconclusive'))
cases = ET.parse(QA / 'Policy/results.xml').findall('.//test-case')
assert len(cases) == 130 and len({case.get('fullname') for case in cases}) == 130
assert all(case.get('result') == 'Passed' for case in cases)
assert read(QA / 'Build/summary.json')['exitCode'] == 0
assert read(QA / 'PackedAssets.json')['result'] == 'Succeeded'
scope = read(QA / 'Scope.json')
assert scope['baselineFiles'] == scope['finalFiles'] == scope['unchanged'] == 7802
assert not scope['changed'] and not scope['added'] and not scope['deleted']
before = read(QA / 'BeforeHashes.json')
after = {str(path.relative_to(ROOT)): sha(path)
         for folder in ('Assets', 'Packages', 'ProjectSettings')
         for path in (ROOT / folder).rglob('*') if path.is_file()
         and path.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key')
         and not path.name.startswith('.env')}
assert before == after
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/CoreCaptionRefreshBuildAudit.cs'
assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
assert not list((BASE / 'Sources').rglob('__pycache__'))
caption_inputs = read(QA / 'InstalledCaptionInputs.json')
for path, pin in caption_inputs['approvedR3Inputs'].items():
    assert pin['sha256'] == before[path] == after[path]
    if 'revision' in pin:
        assert '  Revision: ' + str(pin['revision']) + '\n' in (ROOT / path).read_text()
assert caption_inputs['precedingUnityChecks'] == 44 and caption_inputs['precedingChecksAreHistorical']

artifact = read(QA / 'Artifact.json')
apk = ROOT / artifact['path']
assert apk.stat().st_size == artifact['bytes'] == 765772146
assert sha(apk) == artifact['sha256'] == '924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff'
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
prior = read(QA / 'PreviousArtifact.json')
assert (ROOT / prior['path']).stat().st_size == prior['bytes'] == 765773174
assert sha(ROOT / prior['path']) == prior['sha256'] == 'c065e5ab23f9404cb960660e4fd460ae57f15c4310a181022a65da094890fdb5'
prior_sidecar = (ROOT / prior['path']).with_suffix('.apk.sha256.inherited')
assert prior_sidecar.read_text().split()[0] == prior['sha256']
verification = read(QA / 'ApkVerification.json')
assert verification['sha256'] == artifact['sha256'] and verification['bytes'] == artifact['bytes']
assert verification['signatureVerified'] and verification['sameCertificateAsPriorLocalApk']
assert verification['zipCrcPassed'] and verification['zip16KiBAlignmentPassed']
assert len(verification['elfLibraries']) == 6 and all(verification['runtimeMetadataMarkers'].values())
assert all(min(library['loadSegmentAlignments']) >= 16384 for library in verification['elfLibraries'])
comparison = read(QA / 'ArtifactComparison.json')
assert comparison['old']['sha256'] == prior['sha256']
assert comparison['new']['sha256'] == artifact['sha256']
assert not comparison['addedEntries'] and not comparison['removedEntries']
assert comparison['permissionsUnchanged'] and comparison['new']['unindexedBytes'] == 0
assert comparison['old']['exactAccounting'] and comparison['new']['exactAccounting']
assert len(comparison['new']['entries']) == len(comparison['old']['entries']) == 403
review = read(QA / 'PayloadReview.json')
assert review['changedPayloadCount'] == 5 and review['unchangedPayloadCount'] == 398
assert review['bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged']
assert review['allFourCredentialJavaSourceMetadataPinsUnchanged']
native = read(QA / 'NativeEngineComparison.json')
assert native['binaryIdentical'] and native['dynamicSymbolDefinitionsUnchanged']
assert not native['addedBindings'] and not native['removedBindings']
packed = read(QA / 'PackedComparison.json')
assert packed['beforeRows'] == 29347 and packed['afterRows'] == 29363 and packed['addedRows'] == 16
assert packed['allOtherPathGuidTypeMembershipUnchanged']
assert not packed['addedPaths'] and not packed['removedPaths']
dex = read(QA / 'CredentialDex.json')
assert dex['jniEntryDefinitionsVerified'] and dex['operationFieldDefinitionsVerified']
assert len(dex['compiledCredentialClasses']) == 9
for path, digest in read(QA / 'PinnedPluginInventory.json').items():
    assert sha(ROOT / path) == digest
assert not artifact['installed'] and not artifact['deviceTested']
cleanup = read(QA / 'GeneratedFileCleanup.json')
assert {row['path'] for row in cleanup} == {'Assets/AddressableAssetsData/link.xml', 'Assets/AddressableAssetsData/link.xml.meta'}
assert all(row['action'] == 'removed new generated metadata' for row in cleanup)

links = []
output = QA / 'FinalValidation.json'
report = BASE / 'TestResults.md'
ai = ROOT / 'ProjectDocs/AI'
documents = {report: report.read_text()}
documents[ai / 'CurrentTask.md'] = (ai / 'CurrentTask.md').read_text().split(
    '# 2026-10-01 — Core caption readability R3 complete')[0]
documents[ai / 'SessionLog.md'] = (ai / 'SessionLog.md').read_text().rsplit(
    '# 2026-10-01 — Core caption Android refresh R4 complete', 1)[1]
documents[ai / 'ConversationHandoff.md'] = (ai / 'ConversationHandoff.md').read_text().split(
    'Prepared 2026-09-12.')[0]
schedule = ai / 'DevelopmentSchedule.md'
documents[schedule] = '\n'.join(line for line in schedule.read_text().splitlines()
                                if 'CoreOutcomeReadability/R4CaptionAndroidRefresh/' in line)
for path, content in documents.items():
    assert all(line == line.rstrip() for line in content.splitlines())
    for target in re.findall(r'\]\(([^)]+)\)', content):
        if not target.startswith(('http:', 'https:', '#')):
            resolved = (path.parent / target.split('#')[0]).resolve()
            assert resolved.exists() or resolved == output, (path, target)
            links.append(resolved)
for path in (BASE / 'Sources').glob('*'):
    if path.suffix in ('.py', '.cs'):
        assert all(line == line.rstrip() for line in path.read_text().splitlines()), path
result = {
    'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'result': 'Passed', 'jsonEvidenceFilesParsed': len(json_paths),
    'pythonSourcesParsed': len(sources), 'currentPolicyChecksPassed': 130,
    'previousR3RuntimeChecksAreHistorical': True,
    'successfulUnityBuildAndPackedReportVerified': True,
    'all7802ProjectInputsByteIdentical': True,
    'sourceArtFontGuidPrefabSettingsPackagesAndGuardsPreserved': True,
    'approvedCaptionInputHashesAndRevisionsVerified': True,
    'temporaryBuildHelperAbsent': True, 'taskPythonCacheAbsent': True,
    'currentApkAndSidecarVerified': True, 'archivedR2ApkAndSidecarVerified': True,
    'exactPackedCaptionFaceObjectAdditions': 16, 'unrelatedPackedMembershipUnchanged': True,
    'all403ZipPathsRetained': True, 'unchangedPayloadCount': 398, 'reviewedPayloadChanges': 5,
    'nativeEngineBinaryIdentical': True, 'newDocumentationLinksResolved': len(links),
    'sourceAndNewDocumentationWhitespaceChecked': True,
    'artifactSha256': artifact['sha256'], 'artifactBytes': artifact['bytes'],
    'physicalDeviceExecution': False, 'hostingDeploymentOrCommit': False
}
output.write_text(json.dumps(result, indent=2) + '\n')
assert all(path.exists() for path in links) and read(output) == result
print(json.dumps(result, indent=2))
