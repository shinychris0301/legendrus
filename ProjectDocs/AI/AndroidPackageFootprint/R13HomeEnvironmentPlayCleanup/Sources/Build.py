"""Fresh launcher packaging through the existing guarded Unity command; no device work."""
from pathlib import Path
import hashlib
import json
import os
import resource
import shutil
import subprocess
import time
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
UNITY = Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity')
LOGS = Path('/tmp/lor-home-environment-play-cleanup-r13/build')
GENERATED = [
    'Assets/AddressableAssetsData/Android/addressables_content_state.bin',
    'Assets/AddressableAssetsData/Android/addressables_content_state.bin.meta',
    'Assets/AddressableAssetsData/link.xml',
    'Assets/AddressableAssetsData/link.xml.meta',
]


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def hashes():
    # Never inspect signing files, private keys or player storage.
    return {str(p.relative_to(ROOT)): sha(p)
            for folder in ('Assets', 'Packages', 'ProjectSettings')
            for p in (ROOT / folder).rglob('*')
            if p.is_file() and p.suffix.lower() not in
            ('.keystore', '.jks', '.p12', '.pem', '.key')
            and not p.name.startswith('.env')}


def save(name, value):
    (QA / name).write_text(json.dumps(value, indent=2) + '\n')


def require_idle():
    names = subprocess.check_output(['ps', '-eo', 'comm='], text=True).splitlines()
    assert not any(n.strip() in ('Unity', 'Unity Main Thre') for n in names), 'Unity is open'


def limits():
    resource.setrlimit(resource.RLIMIT_NOFILE, (4096, resource.getrlimit(resource.RLIMIT_NOFILE)[1]))


def run(name, args):
    out = QA / name
    out.mkdir(exist_ok=False)
    cmd = [str(UNITY), '-batchmode', '-nographics', '-projectPath', str(ROOT),
           '-logFile', str(LOGS / (name + '.unity.log'))] + args
    env = {k: v for k, v in os.environ.items()
           if k != 'XDG_CONFIG_HOME' and not k.startswith('LOR_')}
    start = time.time()
    print('Running ' + name, flush=True)
    with (LOGS / (name + '.launcher.log')).open('w') as log:
        code = subprocess.call(cmd, env=env, stdout=log, stderr=subprocess.STDOUT, preexec_fn=limits)
    record = {'command': cmd, 'exitCode': code, 'durationSeconds': time.time() - start}
    if (out / 'results.xml').exists():
        record['result'] = ET.parse(out / 'results.xml').getroot().attrib
    (out / 'summary.json').write_text(json.dumps(record, indent=2) + '\n')
    print(json.dumps(record), flush=True)
    assert code == 0, name + ' failed; inspect local logs'
    return record


def main():
    require_idle()
    QA.mkdir(parents=True, exist_ok=True)
    LOGS.mkdir(exist_ok=False)
    assert UNITY.is_file()
    assert shutil.disk_usage(ROOT).free > 8 * 1024**3
    before = hashes()
    save('BeforeBuildHashes.json', before)
    with (QA / 'WorkingTreeBefore.txt').open('w') as out:
        subprocess.run(['git', 'status', '--short'], cwd=ROOT, stdout=out, check=True)
    for relative in GENERATED:
        source = ROOT / relative
        if source.exists():
            target = LOGS / 'Before' / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)

    for test_run in ('EditAccepted','PlayFinal'):
        result = json.loads((QA/test_run/'summary.json').read_text())['result']
        assert result['result']=='Passed' and result['skipped']=='0'
    require_idle()
    policy = json.loads((ROOT / 'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json').read_text())
    apk = ROOT / policy['developmentOutputRelativePath']
    prior = json.loads((BASE.parent / 'R11HomeHeroArchive/QA/Artifact.json').read_text())
    digest = sha(apk)
    assert digest == prior['sha256'], 'Artifact differs from the audited baseline'
    archive = apk.parent / 'BeforeHomeEnvironmentPlayCleanupR13'
    archive.mkdir(exist_ok=False)
    old = archive / apk.name
    apk.rename(old)
    save('PreviousArtifact.json', {'path': str(old.relative_to(ROOT)), 'bytes': old.stat().st_size, 'sha256': digest})
    sidecar = apk.with_suffix('.apk.sha256')
    if sidecar.exists():
        # Preserve inherited sidecars as historical data, not proof of the prior APK hash.
        sidecar.rename(archive / (sidecar.name + '.inherited'))

    # Isolate only package-task incremental state and final launcher outputs.
    # Do not clean player assets, IL2CPP, Gradle project/configuration or signing state.
    launcher = ROOT / 'Library/Bee/Android/Prj/IL2CPP/Gradle/launcher/build'
    isolation = ROOT / 'Library/LegendsOfRusPackagingBackups/BeforeHomeEnvironmentPlayCleanupR13'
    isolation.mkdir(parents=True, exist_ok=False)
    moved = []
    for relative in ('outputs/apk/debug', 'intermediates/incremental/packageDebug',
                     'intermediates/apk_ide_redirect_file/debug'):
        source = launcher / relative
        assert source.is_dir(), 'Expected packaging directory missing: ' + relative
        target = isolation / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        inventory = [{'path': str(p.relative_to(source)), 'bytes': p.stat().st_size, 'sha256': sha(p)}
                     for p in source.rglob('*') if p.is_file()]
        source.rename(target)
        moved.append({'source': str(source.relative_to(ROOT)), 'backup': str(target.relative_to(ROOT)), 'files': inventory})
    save('PackagingIsolation.json', moved)
    run('Build', ['-executeMethod', 'HomeEnvironmentPlayCleanupBuildAudit.Build', '-quit'])
    assert apk.is_file()
    artifact_hash = sha(apk)
    save('Artifact.json', {'path': str(apk.relative_to(ROOT)), 'bytes': apk.stat().st_size,
         'sha256': artifact_hash, 'installed': False, 'deviceTested': False})
    sidecar.write_text(artifact_hash + '  ' + apk.name + '\n')
    print('Guarded APK built; payload/signature/scope verification pending.', flush=True)


if __name__ == '__main__':
    require_idle()
    helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/HomeEnvironmentPlayCleanupBuildAudit.cs'
    assert not helper.exists() and not helper.with_suffix('.cs.meta').exists()
    shutil.copy2(BASE/'Sources'/helper.name,helper)
    try:
        main()
    finally:
        assert helper.read_bytes()==(BASE/'Sources'/helper.name).read_bytes()
        helper.unlink()
        if helper.with_suffix('.cs.meta').exists():helper.with_suffix('.cs.meta').unlink()
