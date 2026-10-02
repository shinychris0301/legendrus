"""Run the bounded Victory R8 rebuild and existing Victory/Pre-Level checks.

Launch manually with a new run directory name, from the Unity project root.
Never overwrite earlier evidence or use the user's save directory.
"""
from pathlib import Path
import json
import os
import subprocess
import sys
import time


root = Path.cwd()
base = Path('/tmp/lor-victory-r8')
if len(sys.argv) != 2 or Path(sys.argv[1]).name != sys.argv[1] or sys.argv[1] in ('.', '..'):
    raise SystemExit('Usage: python3 /tmp/lor-victory-r8/run.py UNIQUE_RUN_NAME')
if not (root / 'ProjectSettings/ProjectVersion.txt').is_file():
    raise SystemExit('Run from the Unity project root.')
out = base / sys.argv[1]
out.mkdir(parents=True, exist_ok=False)
config = str(out / 'test-config')
overrides = {
    'XDG_CONFIG_HOME': config,
    'LOR_LOADING_TEST_CONFIG_ROOT': config,
    'LOR_VICTORY_CAPTURE_DIR': str(out / 'Victory'),
    'LOR_PRELEVEL_CAPTURE_DIR': str(out / 'PreLevel'),
}
env = os.environ.copy()
env.update(overrides)
steps = [
    ('ImportVictory', ['-executeMethod', 'LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildVictoryAlignment', '-quit']),
    ('EditMode', ['-runTests', '-testPlatform', 'EditMode', '-testFilter', 'LegendsOfRus.Tests.ProductionCoreLoopUi.VictoryAlignmentEditModeTests;LegendsOfRus.Tests.ProductionCoreLoopUi.PreLevelAlignmentEditModeTests']),
    ('PlayMode', ['-runTests', '-testPlatform', 'PlayMode', '-testFilter', 'LegendsOfRus.Tests.PlayMode.VictoryAlignmentPlayModeTests;LegendsOfRus.Tests.PlayMode.PreLevelAlignmentPlayModeTests']),
]
for name, args in steps:
    folder = out / name
    folder.mkdir()
    command = ['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity', '-batchmode', '-projectPath', str(root)] + args + ['-logFile', str(folder / 'unity.log')]
    if name in ('EditMode', 'PlayMode'):
        command += ['-testResults', str(folder / 'results.xml')]
    record = {'command': command, 'environmentOverrides': overrides, 'startUnix': time.time()}
    with (folder / 'launcher.log').open('w') as log:
        record['exitCode'] = subprocess.call(command, env=env, stdout=log, stderr=subprocess.STDOUT)
    record['endUnix'] = time.time()
    (folder / 'command.json').write_text(json.dumps(record, indent=2) + '\n')
    print(name, record['exitCode'], flush=True)
    if record['exitCode']:
        raise SystemExit(record['exitCode'])
