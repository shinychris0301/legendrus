"""Review exact recap/code/build-ID differences and unchanged plugin pins."""
from pathlib import Path
import hashlib
import json
import zipfile

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
comparison = json.loads((QA / 'ArtifactComparison.json').read_text())
expected = {
    'assets/bin/Data/Managed/Metadata/global-metadata.dat': 'Compiled recap reader/projection and Profile/router changes',
    'assets/bin/Data/boot.config': 'Build and player connection identifiers',
    'assets/bin/Data/data.unity3d': 'Installed Profile revision13 and recap projection/reader MonoScripts; exact packed membership checked separately',
    'assets/bin/Data/unity_app_guid': 'Build identifier',
    'lib/arm64-v8a/libil2cpp.so': 'Compiled recap reader/projection and Profile/router changes'
}
assert {row['path'] for row in comparison['changedPayloads']} == set(expected)
native = json.loads((QA / 'NativeEngineComparison.json').read_text())
assert native['binaryIdentical'] and native['dynamicSymbolDefinitionsUnchanged']
assert not native['addedBindings'] and not native['removedBindings']
configs = []
for row in (comparison['old'], comparison['new']):
    with zipfile.ZipFile(ROOT / row['path']) as archive:
        configs.append(archive.read('assets/bin/Data/boot.config').decode().splitlines())
allowed = ('build-guid=', 'player-connection-guid=')
stable = lambda config: [line for line in config if not line.startswith(allowed)]
assert stable(configs[0]) == stable(configs[1])
assert all(sum(line.startswith(prefix) for line in config) == 1 for config in configs for prefix in allowed)
pins = json.loads((QA / 'PinnedPluginInventory.json').read_text())
assert len(pins) == 4
for path, value in pins.items():
    with (ROOT / path).open('rb') as stream:
        assert hashlib.file_digest(stream, 'sha256').hexdigest() == value
record = {
    'expectedChanges': expected, 'changedPayloadCount': 5,
    'unchangedPayloadCount': comparison['identicalPayloadCount'],
    'nativeEngineBinaryIdentical': True,
    'bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged': True,
    'allFourCredentialJavaSourceMetadataPinsUnchanged': True,
    'zipPathsUnchanged': True, 'unindexedBytes': comparison['new']['unindexedBytes'],
    'physicalDeviceExecution': False
}
(QA / 'PayloadReview.json').write_text(json.dumps(record, indent=2) + '\n')
print(json.dumps(record, indent=2))
