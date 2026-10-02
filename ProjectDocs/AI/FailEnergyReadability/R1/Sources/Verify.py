from pathlib import Path
import os, sys, json, subprocess, resource, time, re
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[5]
base = Path(__file__).resolve().parent.parent
name, platform = sys.argv[1:3]
out = base / 'QA' / name
out.mkdir(parents=True, exist_ok=False)
config = str(Path('/tmp/lor-fail-energy-r1') / name / 'config')
# Let Unity validate its existing machine entitlement. Never inspect/copy license contents.
# Only this Editor licensing directory is shared; all product save/preferences paths stay isolated.
license_parent = Path(config) / 'unity3d/Unity'
license_parent.mkdir(parents=True, exist_ok=True)
(license_parent/'licenses').symlink_to('/home/k2/.config/unity3d/Unity/licenses', target_is_directory=True)
env = os.environ.copy()
env.update(XDG_CONFIG_HOME=config, LOR_LOADING_TEST_CONFIG_ROOT=config)
# Sequence capture is optional and expensive; page captures enable their layout checks.
keys = set()
for source in (root / 'Assets/LegendsOfRus/Tests').rglob('*.cs'):
    keys.update(re.findall(r'"(LOR_[A-Z_]+_CAPTURE_DIR)"', source.read_text()))
for key in sorted(keys - {'LOR_JOURNEY_CAPTURE_DIR', 'LOR_VFX_CAPTURE_DIR'}):
    folder = out / 'Captures' / key
    folder.mkdir(parents=True, exist_ok=True)
    env[key] = str(folder)
for key in ['LOR_JOURNEY_CAPTURE_DIR', 'LOR_VFX_CAPTURE_DIR', 'LOR_PERSISTENCE_PHASE', 'LOR_LANGUAGE_RESTART_PHASE']:
    env.pop(key, None)
overrides = json.loads(Path(sys.argv[4]).read_text()) if len(sys.argv)>4 else {}
allowed = {'XDG_CONFIG_HOME', 'LOR_LOADING_TEST_CONFIG_ROOT', 'LOR_PERSISTENCE_PHASE',
           'LOR_PERSISTENCE_CHECKPOINT', 'LOR_LANGUAGE_RESTART_PHASE', 'LOR_JOURNEY_CAPTURE_DIR'}
assert set(overrides) <= allowed
env.update(overrides)
config = env['XDG_CONFIG_HOME']
assert config == env['LOR_LOADING_TEST_CONFIG_ROOT'] and config.startswith('/tmp/lor-fail-energy-r1/')
command = ['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity', '-batchmode',
           '-projectPath', str(root), '-runTests', '-testPlatform', platform,
           '-testFilter', sys.argv[3] if len(sys.argv)>3 else 'LegendsOfRus.',
           '-testResults', str(out/'results.xml'), '-logFile', str(out/'unity.log'), '-force-vulkan']
def limits():
    resource.setrlimit(resource.RLIMIT_NOFILE, (4096, resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
start = time.time()
print('Running', name, platform, flush=True)
with (out/'launcher.log').open('w') as stream:
    code = subprocess.call(command, env=env, stdout=stream, stderr=subprocess.STDOUT, preexec_fn=limits)
record = {'command': command, 'exitCode': code, 'durationSeconds': time.time()-start,
          'isolatedConfig': config, 'processOnlyVulkan': True, 'phaseOverrides': overrides}
if (out/'results.xml').exists():
    tree = ET.parse(out/'results.xml')
    record['result'] = tree.getroot().attrib
    record['nonPassing'] = [{'name': t.get('fullname'), 'result': t.get('result'),
                            'message': t.findtext('.//message')} for t in tree.findall('.//test-case') if t.get('result') != 'Passed']
(out/'summary.json').write_text(json.dumps(record, indent=2))
print(json.dumps({k:v for k,v in record.items() if k not in ['nonPassing','command']}, indent=2), flush=True)

# A zero-case filter is not verification, even when Unity returns success.
result = record.get("result", {})
if code != 0 or int(result.get("total", "0")) == 0 or result.get("result") != "Passed":
    raise SystemExit(1)
