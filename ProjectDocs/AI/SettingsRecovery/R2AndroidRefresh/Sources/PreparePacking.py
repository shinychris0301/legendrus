"""Reversibly relocate three active generated packing caches to obtain a fresh asset inventory."""
from pathlib import Path
import json
import shutil
import subprocess

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
names = subprocess.check_output(['ps', '-eo', 'comm='], text=True).splitlines()
assert not any(n.strip() in ('Unity', 'Unity Main Thre') for n in names)
target = Path('/tmp/lor-settings-android-refresh-r2/BeforeFreshPacking')
target.mkdir(parents=True, exist_ok=False)
paths = ['Library/PlayerDataCache/Android', 'Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build/outputs/apk/debug', 'Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/assets']
record = []
for index, relative in enumerate(paths):
    source = ROOT / relative
    assert source.is_dir()
    files=[p for p in source.rglob('*') if p.is_file()]
    assert not any(p.suffix.lower() in ('.keystore','.jks','.pem','.key','.p12') for p in files)
    if index>=3:assert len(files)==6
    destination = target / str(index)
    shutil.move(str(source), destination)
    record.append({'source': relative, 'preservedAt': str(destination)})
    (QA / 'FreshPackingCachePreservation.json').write_text(json.dumps(record, indent=2) + '\n')
assert shutil.disk_usage(ROOT).free > 8 * 1024**3
print('Three generated packing caches preserved under /tmp; source, signing and IL2CPP caches retained.')
