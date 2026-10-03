"""Correlate the generated daily claim catch and compiled method bytes with the packaged APK."""
from pathlib import Path
import hashlib
import json
import re
import struct
import subprocess
import sys
import zipfile

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
stage = sys.argv[1]
assert stage in ('Before', 'After')
cpp = ROOT / 'Library/Bee/artifacts/Android/il2cppOutput/cpp/LegendsOfRus.Runtime.cpp'
source = cpp.read_text()
pattern = r'^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool (GameState_ClaimDailyReward_m[0-9A-F]+) \([^\n;]*DailyRewardGrant_[^\n;]*\)\s*\n\{'
matches = list(re.finditer(pattern, source, re.M))
assert len(matches) == 1
match = matches[0]
opening = source.index('{', match.start()); depth = 1; end = opening + 1
while depth:
    if source[end] == '{': depth += 1
    elif source[end] == '}': depth -= 1
    end += 1
body = source[match.start():end]
symbol = match.group(1)
out = BASE / 'Sources' / (stage + 'GeneratedClaim.cpp')
assert not out.exists()
out.write_text(body + '\n')
local = ROOT / 'Library/Bee/artifacts/Android/d8kzr/libil2cpp.so'
listing = subprocess.check_output(['readelf', '--wide', '--syms', str(local)], text=True)
rows = [line.split() for line in listing.splitlines() if line.split() and line.split()[-1] == symbol]
assert len(rows) == 1 and rows[0][3] == 'FUNC'
address, size = int(rows[0][1], 16), int(rows[0][2])

def method_bytes(stream):
    stream.seek(0); header = stream.read(64)
    assert header[:6] == b'\x7fELF\x02\x01'
    phoff = struct.unpack_from('<Q', header, 32)[0]
    stride, count = struct.unpack_from('<HH', header, 54)
    for index in range(count):
        stream.seek(phoff + index * stride)
        segment = stream.read(stride)
        kind = struct.unpack_from('<I', segment)[0]
        offset, virtual, _, filesz = struct.unpack_from('<QQQQ', segment, 8)
        if kind == 1 and virtual <= address and address + size <= virtual + filesz:
            stream.seek(offset + address - virtual)
            data = stream.read(size)
            assert len(data) == size
            return data
    raise AssertionError('Method is not in a file-backed ELF load segment.')

with local.open('rb') as stream:
    compiled = method_bytes(stream)
apk = ROOT / 'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
with zipfile.ZipFile(apk) as archive:
    packaged = archive.read('lib/arm64-v8a/libil2cpp.so')
import io
assert method_bytes(io.BytesIO(packaged)) == compiled
gradle = ROOT / 'Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/jniLibs/arm64-v8a/libil2cpp.so'
with gradle.open('rb') as stream:
    assert hashlib.file_digest(stream, 'sha256').hexdigest() == hashlib.sha256(packaged).hexdigest()
has_catch = 'catch(Il2CppExceptionWrapper&' in body and 'OverflowException_' in body
record = {'stage': stage, 'generatedMethodSymbol': symbol,
    'generatedMethodSha256': hashlib.sha256(body.encode()).hexdigest(),
    'generatedOverflowCatch': has_catch, 'nativeMethodBytes': size,
    'nativeMethodSha256': hashlib.sha256(compiled).hexdigest(),
    'packagedMethodBytesMatchCompiledOutput': True,
    'packagedLibraryMatchesGradleInput': True,
    'packagedLibrarySha256': hashlib.sha256(packaged).hexdigest(),
    'compiledCatchIsNotAndroidExecution': True}
assert has_catch == (stage == 'After')
(QA / (stage + 'NativeClaim.json')).write_text(json.dumps(record, indent=2) + '\n')
if stage == 'After':
    before = json.loads((QA / 'BeforeNativeClaim.json').read_text())
    assert record['nativeMethodSha256'] != before['nativeMethodSha256']
    assert record['generatedMethodSha256'] != before['generatedMethodSha256']
    (QA / 'NativeClaimVerification.json').write_text(json.dumps({'before': before, 'after': record,
        'compiledOverflowRejectionPackaged': True, 'physicalExecutionVerified': False}, indent=2) + '\n')
print(stage + ' generated/native claim correlated with packaged method bytes.')
