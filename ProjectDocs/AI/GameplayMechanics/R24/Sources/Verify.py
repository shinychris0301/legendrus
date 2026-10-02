"""Run mechanics checks with captures and an optional NVIDIA GL workaround.
No persistent backend/project settings, driver installation or shell profile is changed.
"""
from pathlib import Path
import argparse,json,os,shutil,subprocess,time,resource,xml.etree.ElementTree as ET
parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('run_id')
parser.add_argument('suite',choices=['HaloFinal','FinalArt','Storage','Recording','Rounds','Mechanics','Seal','Spark','Remaining','Impact','Audio','Shared','Art','Energy','Spectacle','Materials','Animation','Interaction','EditMode','All'])
parser.add_argument('--vulkan-verification',action='store_true',help='Use Vulkan in the verification child process only; requires explicit project-owner authorization.')
parser.add_argument('--nvidia-execmem-workaround',action='store_true',help='Set __GL_WRITE_TEXT_SECTION=0 for the Unity child process only.')
parser.add_argument('--file-limit',type=int,help='Optional soft open-file limit for verification child only.')
args=parser.parse_args()
def child_limits():
 if args.file_limit:resource.setrlimit(resource.RLIMIT_NOFILE,(args.file_limit,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
if not args.run_id.replace('-','').replace('_','').isalnum():parser.error('Use an alphanumeric run ID with optional hyphens/underscores.')
out=Path('/tmp/lor-gameplay-mechanics-r24')/args.run_id;out.mkdir(parents=True,exist_ok=False)
config=str(out/'isolated-config');env=os.environ.copy()
env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config,LOR_JOURNEY_CAPTURE_DIR=str(out/'Journey'))
if args.nvidia_execmem_workaround:env['__GL_WRITE_TEXT_SECTION']='0'
suites={'HaloFinal':'LegendsOfRus.Tests.CampaignPlayMode.MechanicsSequenceArtTests;LegendsOfRus.Tests.CampaignPlayMode.MechanicsCreationFinishTests;LegendsOfRus.Tests.PlayMode.CoreLoopVfxBoosterFeedbackPlayModeTests','FinalArt':'LegendsOfRus.Tests.CampaignPlayMode.MechanicsSequenceArtTests;LegendsOfRus.Tests.CampaignPlayMode.MechanicsCreationFinishTests;LegendsOfRus.Tests.CampaignPlayMode.MechanicsImpactReadabilityTests;LegendsOfRus.Tests.CampaignPlayMode.MechanicsMaterialFeedbackTests','Art':'LegendsOfRus.Tests.CampaignPlayMode.MechanicsSequenceArtTests;LegendsOfRus.Tests.CampaignPlayMode.MechanicsCreationFinishTests','Energy':'MechanicsEnergyTests','Spectacle':'MechanicsSpectacleTests','EditMode':'LegendsOfRus.Tests.MechanicsTimelineTests;LegendsOfRus.Tests.MechanicsInteractionRulesTests;LegendsOfRus.Tests.MechanicsPrototypeTests;LegendsOfRus.Tests.SparkMechanicsTests;LegendsOfRus.Tests.BoardModelSpecialTests;LegendsOfRus.Tests.BoardSpecialCombinationBalanceTests','Materials':'MechanicsMaterialFeedbackTests','Animation':'MechanicsCausalAnimationTests','Interaction':'MechanicsInteractionPlayModeTests','Shared':'LegendsOfRus.Tests.PlayMode.AudioHapticsFidelityVerificationPlayModeTests','Audio':'MechanicsAudioTests','Impact':'MechanicsImpactReadabilityTests','Storage':'MechanicsPlaytestStorageTests','Recording':'MechanicsPlaytestRecordingTests','Rounds':'MechanicsRoundPlaytestTests','Mechanics':'MechanicsSandboxPlayModeTests','Seal':'RunicSealPlayModeTests','Spark':'SparkSandboxPlayModeTests'}
selected=list(suites) if args.suite=='All' else ['Recording','Rounds','Mechanics','Spark'] if args.suite=='Remaining' else [args.suite]
# Fresh processes reduce cross-suite state; OpenGL can still crash in an isolated class.
# Every class retains its existing assertions and screenshot captures.
# The approved Vulkan audit also supports the originally failing combined-process case.
if args.vulkan_verification and args.suite=='All':selected=['All']
for name in selected:
 d=out/name;d.mkdir()
 stage_env=env.copy();stage_config=str(out/('isolated-config-'+name))
 stage_env.update(XDG_CONFIG_HOME=stage_config,LOR_LOADING_TEST_CONFIG_ROOT=stage_config,LOR_JOURNEY_CAPTURE_DIR=str(d/'Journey'))
 command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests','-testPlatform','EditMode' if name=='EditMode' else 'PlayMode','-testFilter',';'.join((v if v.startswith('LegendsOfRus.') else 'LegendsOfRus.Tests.CampaignPlayMode.'+v) for k,v in suites.items() if k not in ('EditMode','FinalArt','HaloFinal')) if name=='All' else (suites[name] if suites[name].startswith('LegendsOfRus.') else 'LegendsOfRus.Tests.CampaignPlayMode.'+suites[name]),'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
 if args.vulkan_verification:command.append('-force-vulkan')
 record={'command':command,'isolatedConfig':stage_config,'offscreenCaptures':True,'freshProcessPerClass':name!='All','vulkanVerificationRequested':args.vulkan_verification,'nvidiaWorkaroundRequested':args.nvidia_execmem_workaround,'driverEnvironment':{key:stage_env.get(key) for key in ['__GL_WRITE_TEXT_SECTION','__GL_SHADER_DISK_CACHE']},'fileLimit':args.file_limit,'startUnix':time.time()}
 print('Running',name,'with captures enabled; backend '+('Vulkan (authorized test only)' if args.vulkan_verification else 'project default'),flush=True)
 with (d/'launcher.log').open('w') as output:record['exitCode']=subprocess.call(command,env=stage_env,stdout=output,stderr=subprocess.STDOUT,preexec_fn=child_limits)
 record['endUnix']=time.time();(d/'command.json').write_text(json.dumps(record,indent=2));shutil.copytree(d,base/'QA'/args.run_id/name)
 if record['exitCode'] or not (d/'results.xml').exists():raise SystemExit('Verification failed: '+name)
 result=ET.parse(d/'results.xml').getroot().attrib
 print(name,{key:result.get(key) for key in ['result','total','passed','failed','skipped']},flush=True)
 if result.get('result')!='Passed' or int(result.get('total','0'))==0:raise SystemExit('Tests did not pass.')
