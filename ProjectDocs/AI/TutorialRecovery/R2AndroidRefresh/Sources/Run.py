"""Focused existing Unity review checks, using isolated product storage and fresh evidence."""
from pathlib import Path
import json
import os
import resource
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
name, mode = sys.argv[1:3]
assert name.isalnum() and mode in ('EditMode', 'PlayMode')
assert not any(n.strip() in ('Unity', 'Unity Main Thre') for n in
               subprocess.check_output(['ps', '-eo', 'comm='], text=True).splitlines())
out = BASE / 'QA' / name
out.mkdir(exist_ok=False)
local = Path('/tmp/lor-tutorial-android-refresh-r2') / name
local.mkdir(parents=True, exist_ok=False)
config = (Path('/tmp/lor-continuation-r1') / ('EnergyR1' + name) / 'config'
          if 'ContinuationTransactionPlayModeTests' in sys.argv[3] else local / 'config')
licensing = config / 'unity3d/Unity'
licensing.mkdir(parents=True)
(licensing / 'licenses').symlink_to('/home/k2/.config/unity3d/Unity/licenses', target_is_directory=True)
env = {key: value for key, value in os.environ.items() if not key.startswith('LOR_')}
env.update(XDG_CONFIG_HOME=str(config), LOR_LOADING_TEST_CONFIG_ROOT=str(config))
if len(sys.argv)>4:
    options=json.loads(Path(sys.argv[4]).read_text());assert set(options)<= {"LOR_GAMEPLAY_QA"};env.update(options)
if mode == 'PlayMode':
    keys = set()
    for source in (ROOT / 'Assets/LegendsOfRus/Tests').rglob('*.cs'):
        import re
        keys.update(re.findall(r'"(LOR_[A-Z_]+_CAPTURE_DIR)"', source.read_text()))
    for key in keys - {'LOR_VFX_CAPTURE_DIR'}:
        folder = out / 'Captures' / key
        folder.mkdir(parents=True)
        env[key] = str(folder)
command = ['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity', '-batchmode',
           '-projectPath', str(ROOT), '-runTests', '-testPlatform', mode,
           '-testFilter', sys.argv[3], '-testResults', str(out / 'results.xml'),
           '-logFile', str(local / 'unity.log'), '-force-vulkan' if mode == 'PlayMode' else '-nographics']


def limits():
    resource.setrlimit(resource.RLIMIT_NOFILE, (4096, resource.getrlimit(resource.RLIMIT_NOFILE)[1]))


start = time.time()
try:
    with (local / 'launcher.log').open('w') as stream:
        code = subprocess.call(command, env=env, stdout=stream, stderr=subprocess.STDOUT, preexec_fn=limits, start_new_session=True)
finally:
    baseline = json.loads((BASE / 'QA/BeforeHashes.json').read_text())
    removed = []
    for path in (ROOT / 'Assets').glob('InitTestScene*'):
        if str(path.relative_to(ROOT)) not in baseline and path.name.endswith(('.unity', '.unity.meta')):
            removed.append(str(path.relative_to(ROOT)))
            path.unlink()
    (out / 'TemporaryFixtureCleanup.json').write_text(json.dumps({'removed': removed}, indent=2) + '\n')
record = {'exitCode': code, 'durationSeconds': time.time() - start, 'command': command,
          'isolatedConfig': str(config), 'mode': mode, 'rawLogs': str(local),
          'processOnlyVulkan': mode == 'PlayMode'}
if (out / 'results.xml').exists():
    tree = ET.parse(out / 'results.xml')
    record['result'] = tree.getroot().attrib
    record['nonPassing'] = [{'name': case.get('fullname'), 'result': case.get('result'),
                            'message': case.findtext('.//message')}
                           for case in tree.findall('.//test-case') if case.get('result') != 'Passed']
(out / 'summary.json').write_text(json.dumps(record, indent=2) + '\n')
print(json.dumps({key: value for key, value in record.items() if key != 'command'}, indent=2), flush=True)
assert code == 0 and record.get('result', {}).get('result') == 'Passed'
