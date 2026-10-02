from pathlib import Path
import subprocess,json,time
BASE=Path(__file__).resolve().parent.parent
ADB='/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'
PACKAGE='com.example.legendsofrus.v117debug'
def command(*args):return subprocess.run([ADB,*args],capture_output=True,timeout=30)
report='no_backup/lor-credential-probe-report.json'
p=command('shell','run-as',PACKAGE,'cat',report);assert p.returncode==0
before=json.loads(p.stdout);assert before['status']=='passed'
remaining={}
for name in ['lor-backend-credentials-v1.bin','lor-backend-credentials-v1.bin.new','lor-backend-credentials-v1.bin.bak','lor-credential-development-probe']:
    remaining[name]=command('shell','run-as',PACKAGE,'test','-e','no_backup/'+name).returncode==0
assert not any(remaining.values())
(BASE/'QA/FixtureCleanup.json').write_text(json.dumps({'remainingOwnedArtifacts':remaining,'probeVerifiedAliasAbsent':True,'aliasContentExported':False},indent=2)+'\n')
assert command('shell','am','force-stop',PACKAGE).returncode==0
p=command('shell','am','start','-W','-n',PACKAGE+'/com.unity3d.player.UnityPlayerGameActivity');assert p.returncode==0
(BASE/'QA/NormalLaunch.txt').write_text(p.stdout.decode())
time.sleep(10)
p=command('shell','run-as',PACKAGE,'cat',report);assert p.returncode==0 and json.loads(p.stdout)==before
assert command('shell','run-as',PACKAGE,'test','-d','no_backup/lor-credential-development-probe').returncode!=0
pid=command('shell','pidof',PACKAGE);assert pid.returncode==0
p=command('shell','dumpsys','activity','activities')
foreground=any('topResumedActivity' in x and PACKAGE in x for x in p.stdout.decode(errors='replace').splitlines())
assert foreground,'Expected game in foreground before screenshot'
p=command('exec-out','screencap','-p');assert p.returncode==0 and p.stdout.startswith(b'\x89PNG');(BASE/'QA/NormalLaunchDevice.png').write_bytes(p.stdout)
log=command('logcat','--pid='+pid.stdout.decode().strip(),'-d','-v','brief','-s','Unity:E','AndroidRuntime:E','*:S').stdout.decode(errors='replace')
result={'processRunning':True,'launchWithoutProbeExtra':True,'redactedReportUnchanged':True,'ownershipDirectoryAbsent':True,'ownAppForeground':foreground,
    'invalidStartCallbackErrors':log.count('Start() can not take parameters'),
    'fatalExceptionObserved':'FATAL EXCEPTION' in log,
    'playAssetPackClassLookupErrorObserved':'ClassNotFoundException' in log and 'com.google.android.play.core.assetpacks.AssetPackManager' in log,
    'credentialDataExported':False}
(BASE/'QA/NormalLaunch.json').write_text(json.dumps(result,indent=2)+'\n')
assert result['invalidStartCallbackErrors']==0 and not result['fatalExceptionObserved']
print(json.dumps(result))
