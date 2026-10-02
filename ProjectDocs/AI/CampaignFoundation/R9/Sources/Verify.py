from pathlib import Path
import os,sys,json,subprocess,resource,shutil,time,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
out=Path('/tmp/lor-campaign-foundation-r9')/sys.argv[1];out.mkdir(parents=True,exist_ok=False)
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
suites={'Candidate':'LegendsOfRus.Tests.CampaignPlayMode.CampaignSectorCandidateTests','EditMode':'LegendsOfRus.Tests.Campaign','Campaign':'LegendsOfRus.Tests.CampaignPlayMode.CampaignUIIntegrationTests;LegendsOfRus.Tests.CampaignPlayMode.CampaignBoardIntegrationTests','Native':'LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'}
if len(sys.argv)>2:suites={name:suites[name] for name in sys.argv[2:]}
for name,filters in suites.items():
 d=out/name;d.mkdir();env=os.environ.copy();config=str(d/'isolated-config');env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config,LOR_JOURNEY_CAPTURE_DIR=str(d/'Journey'))
 command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests','-testPlatform','EditMode' if name=='EditMode' else 'PlayMode','-testFilter',filters,'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log'),'-force-vulkan']
 print('Running',name,flush=True);start=time.time()
 with (d/'launcher.log').open('w') as f:code=subprocess.call(command,env=env,stdout=f,stderr=subprocess.STDOUT,preexec_fn=limits)
 (d/'command.json').write_text(json.dumps({'command':command,'exitCode':code,'startUnix':start,'endUnix':time.time(),'isolatedConfig':config,'authorization':'Prior process-only Vulkan/descriptor4096 approval; project backend unchanged'},indent=2))
 shutil.copytree(d,base/'QA'/sys.argv[1]/name)
 if code or not (d/'results.xml').exists():raise SystemExit('Failed '+name)
 result=ET.parse(d/'results.xml').getroot().attrib;print(result,flush=True)
 if result.get('result')!='Passed':raise SystemExit('Tests failed '+name)
