from pathlib import Path
import json,os,subprocess,resource,time,hashlib,shutil,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
unity='/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity'
config='/tmp/lor-android-credential-build-r5/config'
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
def run(name,args,env):
 out=base/'QA'/name;out.mkdir(exist_ok=False)
 cmd=[unity,'-batchmode','-nographics','-projectPath',str(root),'-logFile',str(out/'unity.log')]+args
 start=time.time()
 print('Running',name,flush=True)
 with (out/'launcher.log').open('w') as log:code=subprocess.call(cmd,env=env,stdout=log,stderr=subprocess.STDOUT,preexec_fn=limits)
 record={'command':cmd,'exitCode':code,'durationSeconds':time.time()-start}
 if (out/'results.xml').exists():record['result']=ET.parse(out/'results.xml').getroot().attrib
 (out/'summary.json').write_text(json.dumps(record,indent=2));print(json.dumps(record),flush=True)
 if code:raise SystemExit(code)
 return record
record=run('FinalPolicyAndCampaignTests',['-runTests','-testPlatform','EditMode','-testFilter','LegendsOfRus.Tests.FunctionalGameFlow.CredentialProbeProtocolTests;LegendsOfRus.Tests.AndroidBuild;LegendsOfRus.Tests.Campaign.CampaignCoordinatorTests','-testResults',str(base/'QA/FinalPolicyAndCampaignTests/results.xml')],{k:v for k,v in os.environ.items() if k != 'XDG_CONFIG_HOME'})
assert record['result']['result']=='Passed' and record['result']['skipped']=='0'
policy=json.loads((root/'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json').read_text());apk=root/policy['developmentOutputRelativePath']
if apk.exists():
 archive=apk.parent/'BeforeCredentialStorageR5StartupFix';archive.mkdir(exist_ok=False)
 previous=archive/apk.name;size=apk.stat().st_size;digest=hashlib.sha256(apk.read_bytes()).hexdigest();apk.rename(previous)
 (base/'QA/PreviousArtifact.json').write_text(json.dumps({'path':str(previous.relative_to(root)),'bytes':size,'sha256':digest},indent=2))
assert shutil.disk_usage(root).free>5*1024**3,'Less than 5GiB free before build'
env=os.environ.copy();env.pop('XDG_CONFIG_HOME',None)
# Keep the existing default development-signing identity. No credentials are read by this script.
run('FinalBuild',['-executeMethod','LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk','-quit'],env)
assert apk.is_file()
(base/'QA/Artifact.json').write_text(json.dumps({'path':str(apk.relative_to(root)),'bytes':apk.stat().st_size,'sha256':hashlib.sha256(apk.read_bytes()).hexdigest(),'installed':False,'deviceTested':False},indent=2))
print('APK ready:',str(apk.relative_to(root)),flush=True)
