"""Preserve the rejected audit candidate and rebuild with original signing context/fresh data pack."""
from pathlib import Path
import json
import shutil
from Build import ROOT, BASE, QA, LOGS, hashes, sha, require_idle, save, run

require_idle()
before = json.loads((QA / 'BeforeHashes.json').read_text())
assert hashes() == before
policy = json.loads((ROOT / 'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json').read_text())
apk = ROOT / policy['developmentOutputRelativePath']
assert apk.is_file()
initial = apk.parent / 'BeforeFixedPresetsRefreshR5' / 'InitialAuditCandidate.apk'
assert not initial.exists()
initial_hash = sha(apk)
apk.rename(initial)
save('InitialArtifact.json', {'path': str(initial.relative_to(ROOT)), 'bytes': initial.stat().st_size,
     'sha256': initial_hash, 'accepted': False,
     'reasons': ['Incremental build exposed no packed-asset inventory to audit helper',
                 'Isolated XDG configuration changed default development certificate'],
     'sourceScopeUnchanged': True, 'noDeviceOrSigningFileAccess': True})

# Reversible relocation of only generated player-data/packaging caches to tmpfs.
# No IL2CPP source cache, Gradle signing configuration, player assets or personal data is moved.
target = LOGS.parent / 'BeforeFreshPacking'
target.mkdir(exist_ok=False)
relative_paths = [
    'Library/PlayerDataCache/Android',
    'Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build/outputs/apk/debug',
    'Library/LegendsOfRusPackagingBackups/BeforeFixedPresetsRefreshR5',
]
moved = []
for index, relative in enumerate(relative_paths):
    source = ROOT / relative
    assert source.is_dir()
    destination = target / str(index)
    shutil.move(str(source), destination)
    moved.append({'source': relative, 'preservedAt': str(destination)})
save('FreshPackingCachePreservation.json', moved)
assert shutil.disk_usage(ROOT).free > 8 * 1024**3
helper = ROOT / 'Assets/LegendsOfRus/Scripts/Editor/FixedPresetsRefreshBuildAudit.cs'
assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
shutil.copy2(BASE / 'Sources' / helper.name, helper)
try:
    run('BuildVerified', ['-executeMethod', 'FixedPresetsRefreshBuildAudit.Build', '-quit'])
    assert apk.is_file()
    digest = sha(apk)
    save('Artifact.json', {'path': str(apk.relative_to(ROOT)), 'bytes': apk.stat().st_size,
                         'sha256': digest, 'installed': False, 'deviceTested': False})
    apk.with_suffix('.apk.sha256').write_text(digest + '  ' + apk.name + '\n')
    print('Rebuilt APK candidate ready for independent signature/payload/preset verification.', flush=True)
finally:
    assert helper.read_bytes() == (BASE / 'Sources' / helper.name).read_bytes()
    helper.unlink()
    if helper.with_suffix('.cs.meta').exists(): helper.with_suffix('.cs.meta').unlink()
