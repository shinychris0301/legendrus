"""Fail closed unless all APK differences are build IDs or byte-identical DEX renaming."""
from collections import Counter
import json
import re
import uuid
import zipfile
from Compare import ROOT, QA, OLD, NEW
from Build import sha

comparison = json.loads((QA / 'ArtifactComparison.json').read_text())
old = comparison['old']['entries']
new = comparison['new']['entries']
dex = re.compile(r'classes\d*\.dex')
old_dex = {n: v['sha256'] for n, v in old.items() if dex.fullmatch(n)}
new_dex = {n: v['sha256'] for n, v in new.items() if dex.fullmatch(n)}
assert Counter(old_dex.values()) == Counter(new_dex.values()), 'DEX bytes changed'
renames = [{'before': n, 'after': [k for k, h in new_dex.items() if h == digest]}
           for n, digest in old_dex.items() if new_dex.get(n) != digest]
with zipfile.ZipFile(OLD) as before, zipfile.ZipFile(NEW) as after:
    def boot(archive):
        lines = archive.read('assets/bin/Data/boot.config').decode().splitlines()
        values = {}
        for line in lines:
            key, value = line.split('=', 1)
            values.setdefault(key, []).append(value)
        return values
    a, b = boot(before), boot(after)
    changed_keys = sorted(k for k in set(a) | set(b) if Counter(a.get(k, [])) != Counter(b.get(k, [])))
    assert changed_keys == ['build-guid', 'player-connection-guid'], 'Unexpected boot configuration difference'
    assert Counter(a['player-connection-ip']) == Counter(b['player-connection-ip'])
    for archive, config in ((before, a), (after, b)):
        app_guid = archive.read('assets/bin/Data/unity_app_guid').decode().strip()
        assert uuid.UUID(app_guid).int and uuid.UUID(config['build-guid'][0]).int
        assert len(config['player-connection-guid']) == 1 and config['player-connection-guid'][0].isdigit()
allowed = {'assets/bin/Data/boot.config', 'assets/bin/Data/unity_app_guid'} | set(old_dex)
assert all(row['path'] in allowed for row in comparison['changedPayloads'])
assert comparison['new']['unindexedBytes'] == 0
assert comparison['savedBytes'] == comparison['old']['unindexedBytes']
pins = json.loads((ROOT / 'ProjectDocs/AI/AndroidCredentialStorage/R2DeviceBuild/QA/PinnedPluginInventory.json').read_text())
assert all(sha(ROOT / name) == digest for name, digest in pins.items())
result = {'allDifferencesExplained': True, 'dexPayloadMultisetIdentical': True,
    'dexFiles': len(old_dex), 'dexRenames': renames, 'bootChangedKeys': changed_keys,
    'buildAndAppGuidsValid': True, 'connectionAddressMultisetUnchanged': True,
    'connectionAddressOrderChanged': a['player-connection-ip'] != b['player-connection-ip'],
    'otherPayloadsByteIdentical': True,
    'approvedJavaPinsUnchanged': True, 'newUnindexedBytes': 0,
    'savedBytesEqualPriorUnindexedBytes': True,
    'physicalDeviceTested': False}
(QA / 'ReviewedDelta.json').write_text(json.dumps(result, indent=2) + '\n')
print(json.dumps(result, indent=2))
