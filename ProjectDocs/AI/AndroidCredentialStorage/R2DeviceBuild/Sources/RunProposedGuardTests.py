from pathlib import Path
import json, subprocess, tempfile
root = Path(__file__).resolve().parents[5]
base = Path(__file__).resolve().parent.parent
mono = Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/MonoBleedingEdge')
with tempfile.TemporaryDirectory(prefix='lor-guard-host-') as temporary:
    executable = str(Path(temporary) / 'tests.exe')
    command = [str(mono/'bin/mono'), str(mono/'lib/mono/4.5/csc.exe'), '-nologo', '-out:'+executable,
        str(base/'Sources/Proposed/Assets/LegendsOfRus/Editor/AndroidBuild/Unity/ReviewedAndroidCredentialPlugins.cs'),
        str(base/'Sources/GuardExceptionHostTests.cs')]
    compiled = subprocess.run(command, capture_output=True, text=True)
    (base/'QA/GuardExceptionCompile.log').write_text(compiled.stdout + compiled.stderr)
    assert compiled.returncode == 0, 'Candidate helper compilation failed'
    tested = subprocess.run([str(mono/'bin/mono'), executable, str(root)], capture_output=True, text=True)
    (base/'QA/GuardExceptionHostTests.log').write_text(tested.stdout + tested.stderr)
    assert tested.returncode == 0, 'Candidate helper tests failed'
    count = sum(line.startswith('PASS:') for line in tested.stdout.splitlines())
patch = subprocess.run(['git', 'apply', '--check', str(base/'Sources/ProposedCredentialPluginException.patch')], cwd=root, capture_output=True, text=True)
assert patch.returncode == 0, patch.stderr
(base/'QA/ProposalVerification.json').write_text(json.dumps({'hostCasesPassed':count, 'hostFailed':0,
    'patchAppliesCleanly':True, 'appliedToProject':False, 'unityIntegrationTested':False}, indent=2)+'\n')
print(f'{count} host checks passed; patch applies cleanly (not applied).')
