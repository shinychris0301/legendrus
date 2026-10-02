from pathlib import Path
import os,sys,json,subprocess,time,hashlib,shutil,xml.etree.ElementTree as ET
root=Path.cwd();base=Path('ProjectDocs/AI/AndroidDeviceAudit/R1')
assert len(sys.argv)==2 and Path(sys.argv[1]).name==sys.argv[1] and sys.argv[1] not in ('.','..')
out=Path('/tmp/lor-android-device-r1')/sys.argv[1];out.mkdir(parents=True,exist_ok=False)
unity='/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity'
config=str(out/'test-config');env=os.environ.copy();env.update({'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config})
def run(name,args,environment,overrides):
    folder=out/name;folder.mkdir();cmd=[unity,'-batchmode','-nographics','-projectPath',str(root)]+args+['-logFile',str(folder/'unity.log')]
    rec={'command':cmd,'environmentOverrides':overrides,'startUnix':time.time()}
    with (folder/'launcher.log').open('w') as log:rec['exitCode']=subprocess.call(cmd,env=environment,stdout=log,stderr=subprocess.STDOUT,start_new_session=True)
    rec['endUnix']=time.time();(folder/'command.json').write_text(json.dumps(rec,indent=2)+'\n');print(name,rec['exitCode'],flush=True)
    if rec['exitCode']:raise SystemExit(rec['exitCode'])
run('PolicyTests',['-runTests','-testPlatform','EditMode','-assemblyNames','LegendsOfRus.AndroidBuild.Editor.Tests','-testResults',str(out/'PolicyTests/results.xml')],env,{'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config})
result=ET.parse(out/'PolicyTests/results.xml').getroot().attrib
print({k:result[k] for k in ('total','passed','failed','skipped')},flush=True)
assert result['result']=='Passed' and result['skipped']=='0'
policy=json.loads((root/'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json').read_text())
apk=root/policy['developmentOutputRelativePath']
if apk.exists():
    archive=apk.parent/('BeforeAndroidDeviceAuditR1_'+sys.argv[1]);archive.mkdir(exist_ok=False);shutil.copy2(apk,archive/apk.name)
    (archive/(apk.name+'.sha256')).write_text(hashlib.sha256(apk.read_bytes()).hexdigest()+'  '+apk.name+'\n')
env=os.environ.copy();env.pop('XDG_CONFIG_HOME',None)
run('Build',['-executeMethod','LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk','-quit'],env,{'XDG_CONFIG_HOME':'unset; retain existing default debug-signing identity'})
assert apk.is_file()
target=root/'Builds/Verification/AndroidDeviceAuditR1/LegendsOfRus-UI-Audit-R1-development.apk';target.parent.mkdir(parents=True,exist_ok=False);shutil.copy2(apk,target)
hash_=hashlib.sha256(target.read_bytes()).hexdigest();target.with_suffix('.apk.sha256').write_text(hash_+'  '+target.name+'\n')
(out/'Artifact.json').write_text(json.dumps({'path':str(target.relative_to(root)),'bytes':target.stat().st_size,'sha256':hash_},indent=2)+'\n')
print('Artifact ready:',str(target.relative_to(root)),flush=True)
