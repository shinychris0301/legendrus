"""Validate this package refresh's evidence and all task-start project inputs."""
import ast
import datetime
import hashlib
import json
import re
import xml.etree.ElementTree as ET
from pathlib import Path

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
cases = ET.parse(QA / 'Policy/results.xml').findall('.//test-case')
assert len(cases) == 130 and len({case.get('fullname') for case in cases}) == 130
assert all(case.get('result') == 'Passed' for case in cases)
assert read(QA / 'Build/summary.json')['exitCode'] == 0
assert read(QA / 'PackedAssets.json')['result'] == 'Succeeded'
scope = read(QA / 'Scope.json')
assert scope['baselineFiles'] == scope['finalFiles'] == scope['unchanged'] == 7798
assert not scope['changed'] and not scope['added'] and not scope['deleted']
before = read(QA / 'BeforeHashes.json')
after = {str(path.relative_to(ROOT)): sha(path)
         for folder in ['Assets', 'Packages', 'ProjectSettings']
         for path in (ROOT / folder).rglob('*') if path.is_file()
         and path.suffix.lower() not in ('.keystore', '.jks', '.p12', '.pem', '.key')
         and not path.name.startswith('.env')}
assert before == after
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/CoreOutcomeUiRefreshBuildAudit.cs'
assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
assert not (BASE / 'Sources/__pycache__').exists()

artifact = read(QA / 'Artifact.json')
apk = ROOT / artifact['path']
assert apk.stat().st_size == artifact['bytes'] and sha(apk) == artifact['sha256']
assert apk.with_suffix('.apk.sha256').read_text().strip() == artifact['sha256'] + '  ' + apk.name
prior = read(QA / 'PreviousArtifact.json')
assert (ROOT / prior['path']).stat().st_size == prior['bytes'] == 765750206
assert sha(ROOT / prior['path']) == prior['sha256'] == 'c83940412715c9ebed87eb3e47b12bdd971cb8aae8a48735f015dd9ac9279fbe'
prior_sidecar = (ROOT / prior['path']).with_suffix('.apk.sha256.inherited')
assert prior_sidecar.read_text().split()[0] == prior['sha256']
verification = read(QA / 'ApkVerification.json')
assert verification['sha256'] == artifact['sha256'] and verification['bytes'] == artifact['bytes']
assert verification['signatureVerified'] and verification['sameCertificateAsPriorLocalApk']
assert verification['zipCrcPassed'] and verification['zip16KiBAlignmentPassed']
assert len(verification['elfLibraries']) == 6 and all(verification['runtimeMetadataMarkers'].values())
comparison = read(QA / 'ArtifactComparison.json')
assert comparison['old']['sha256'] == prior['sha256']
assert comparison['new']['sha256'] == artifact['sha256']
assert not comparison['addedEntries'] and not comparison['removedEntries']
assert comparison['permissionsUnchanged'] and comparison['new']['unindexedBytes'] == 0
assert len(comparison['new']['entries']) == len(comparison['old']['entries']) == 403
review = read(QA / 'PayloadReview.json')
assert review['changedPayloadCount'] == 6 and review['unchangedPayloadCount'] == 397
assert review['bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged']
assert review['allFourCredentialJavaSourceMetadataPinsUnchanged']
native = read(QA / 'NativeEngineComparison.json')
assert native['addedBindings'] == ['UnityEngine.CanvasRenderer::set_cullTransparentMesh_Injected']
assert not native['removedBindings'] and native['dynamicSymbolDefinitionsUnchanged']
packed = read(QA / 'PackedComparison.json')
assert packed['beforeRows'] == 29322 and packed['afterRows'] == 29347 and packed['addedRows'] == 25
assert packed['allOtherPathGuidTypeMembershipUnchanged']
dex = read(QA / 'CredentialDex.json')
assert dex['jniEntryDefinitionsVerified'] and dex['operationFieldDefinitionsVerified']
assert len(dex['compiledCredentialClasses']) == 9
assert not artifact['installed'] and not artifact['deviceTested']

links = []
resolved_links = []
report = BASE / 'TestResults.md'
ai = ROOT / 'ProjectDocs/AI'
documents = {report: report.read_text()}
documents[ai / 'CurrentTask.md'] = (ai / 'CurrentTask.md').read_text().split(
    '# 2026-10-01 — Core outcome readability and input R1 complete')[0]
documents[ai / 'SessionLog.md'] = (ai / 'SessionLog.md').read_text().rsplit(
    '# 2026-10-01 — Core outcome Android refresh R2 complete', 1)[1]
documents[ai / 'ConversationHandoff.md'] = (ai / 'ConversationHandoff.md').read_text().split(
    'Prepared 2026-09-12.')[0]
schedule = ai / 'DevelopmentSchedule.md'
documents[schedule] = '\n'.join(line for line in schedule.read_text().splitlines()
                                if 'CoreOutcomeReadability/R2AndroidRefresh/' in line)
for path, content in documents.items():
    for target in re.findall(r'\]\(([^)]+)\)', content):
        if not target.startswith(('http:', 'https:', '#')):
            resolved = (path.parent / target.split('#')[0]).resolve()
            # The report links to the result that this script creates below.
            if resolved != QA / 'FinalValidation.json':
                assert resolved.exists(), (path, target)
            resolved_links.append(resolved)
            links.append(target)
result = {
    'checkedAtUtc': datetime.datetime.now(datetime.timezone.utc).isoformat(),
    'result': 'Passed', 'jsonEvidenceFilesParsed': len(json_paths),
    'pythonSourcesParsed': len(sources), 'currentPolicyChecksPassed': 130,
    'previousR1RuntimeChecksAreHistorical': True,
    'successfulUnityBuildAndPackedReportVerified': True,
    'all7798ProjectInputsByteIdentical': True,
    'sourceArtFontGuidPrefabSettingsPackagesAndGuardsPreserved': True,
    'temporaryBuildHelperAbsent': True, 'taskPythonCacheAbsent': True,
    'currentApkAndSidecarVerified': True, 'archivedR17ApkAndSidecarVerified': True,
    'exactPackedUiObjectAdditions': 24, 'newTouchMonoScriptRows': 1,
    'unrelatedPackedMembershipUnchanged': True, 'all403ZipPathsRetained': True,
    'unchangedPayloadCount': 397, 'reviewedPayloadChanges': 6,
    'regeneratedNativeEngineAdditionalBindingVerified': True,
    'newDocumentationLinksResolved': len(links),
    'artifactSha256': artifact['sha256'], 'artifactBytes': artifact['bytes'],
    'physicalDeviceExecution': False, 'hostingDeploymentOrCommit': False,
}
(QA / 'FinalValidation.json').write_text(json.dumps(result, indent=2) + '\n')
assert all(path.exists() for path in resolved_links)
assert read(QA / 'FinalValidation.json') == result
print(json.dumps(result, indent=2))
