"""Compare every indexed APK payload and account for archive overhead exactly."""
from pathlib import Path
import hashlib
import json
import struct
import subprocess
import zipfile

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
FOLDER = ROOT / 'Builds/Verification/v1_17_step01'
NEW = FOLDER / 'LegendsOfRus-v1.17-step01-development.apk'
OLD = FOLDER / 'BeforeFixedPresetsRefreshR5' / NEW.name


def digest(stream):
    return hashlib.file_digest(stream, 'sha256').hexdigest()


def inspect(path):
    entries = {}
    gaps = []
    previous = 0
    header_bytes = 0
    with zipfile.ZipFile(path) as archive, path.open('rb') as raw:
        assert archive.testzip() is None
        for info in sorted(archive.infolist(), key=lambda i: i.header_offset):
            assert not info.flag_bits & 8, 'Data descriptor needs separate accounting'
            assert info.filename not in entries, 'Duplicate archive entry'
            if info.header_offset > previous:
                pos = previous
                records = 0
                while pos < info.header_offset:
                    raw.seek(pos)
                    header = raw.read(30)
                    assert header[:4] == b'PK\x03\x04', 'Unrecognized inter-entry gap'
                    compressed, expanded, name_length, extra_length = struct.unpack_from('<IIHH', header, 18)
                    assert not (compressed or expanded or name_length)
                    pos += 30 + extra_length
                    records += 1
                assert pos == info.header_offset
                gaps.append({'offset': previous, 'bytes': pos - previous, 'unnamedRecords': records})
            raw.seek(info.header_offset)
            header = raw.read(30)
            assert header[:4] == b'PK\x03\x04'
            name_length, extra_length = struct.unpack_from('<HH', header, 26)
            header_bytes += 30 + name_length + extra_length
            previous = info.header_offset + 30 + name_length + extra_length + info.compress_size
            with archive.open(info) as stream:
                content_hash = digest(stream)
            entries[info.filename] = {'bytes': info.file_size, 'compressedBytes': info.compress_size,
                'sha256': content_hash, 'compressionMethod': info.compress_type}
        payload = sum(e['compressedBytes'] for e in entries.values())
        gap_bytes = sum(g['bytes'] for g in gaps)
        tail = path.stat().st_size - previous
        assert payload + header_bytes + gap_bytes + tail == path.stat().st_size
        raw.seek(0)
        sha = digest(raw)
    return {'path': str(path.relative_to(ROOT)), 'sha256': sha, 'bytes': path.stat().st_size,
        'payloadBytes': payload, 'headerBytes': header_bytes, 'unindexedBytes': gap_bytes,
        'unnamedRecords': sum(g['unnamedRecords'] for g in gaps), 'tailBytes': tail,
        'zipCrcPassed': True, 'exactAccounting': True, 'gaps': gaps, 'entries': entries}


def main():
    old = inspect(OLD)
    new = inspect(NEW)
    before = old['entries']
    after = new['entries']
    added = sorted(set(after) - set(before))
    removed = sorted(set(before) - set(after))
    changed = [{'path': name, 'before': before[name], 'after': after[name]}
               for name in sorted(set(before) & set(after))
               if before[name]['sha256'] != after[name]['sha256']]
    aapt = '/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/build-tools/36.0.0/aapt'
    permissions = [subprocess.check_output([aapt, 'dump', 'permissions', str(p)], text=True) for p in (OLD, NEW)]
    result = {'old': old, 'new': new, 'addedEntries': added, 'removedEntries': removed,
        'changedPayloads': changed, 'identicalPayloadCount': sum(before[n]['sha256'] == after[n]['sha256'] for n in set(before) & set(after)),
        'savedBytes': old['bytes'] - new['bytes'], 'savedPercent': 100 * (old['bytes'] - new['bytes']) / old['bytes'],
        'permissionsUnchanged': permissions[0] == permissions[1], 'permissions': permissions[1].splitlines(),
        'unityPayloadUnchanged': before['assets/bin/Data/data.unity3d']['sha256'] == after['assets/bin/Data/data.unity3d']['sha256']}
    (QA / 'ArtifactComparison.json').write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps({k: v for k, v in result.items() if k not in ('old', 'new')}, indent=2))
    assert not added and not removed, 'Entry inventory changed'
    assert result['permissionsUnchanged']
    assert new['unindexedBytes']==0, 'Unexpected stale packaging records'
    # UI refresh may grow the package; record size change without claiming savings.
    # Build metadata changes must be reviewed individually, not silently accepted.


if __name__ == '__main__':
    main()
