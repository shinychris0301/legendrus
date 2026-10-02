from pathlib import Path
import subprocess,os,json,time,sys,xml.etree.ElementTree as ET,shutil
root=Path.cwd();base=root/'ProjectDocs/AI/CampaignFoundation/R1';run_id=sys.argv[1] if len(sys.argv)>1 else 'Verify01';out=Path('/tmp/lor-campaign-foundation-r1')/run_id;out.mkdir(parents=True,exist_ok=False)
env=os.environ.copy();config=str(out/'isolated-config');env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config,LOR_JOURNEY_CAPTURE_DIR=str(out/'Journey'))
for name,args in [('EditMode',['-testPlatform','EditMode','-assemblyNames','LegendsOfRus.ContentPipelineCore.Tests']),('PlayMode',['-testPlatform','PlayMode','-testFilter','LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'])]:
 if len(sys.argv)>2 and name!=sys.argv[2]: continue
 d=out/name;d.mkdir();command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests']+args+['-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
 rec={'command':command,'isolatedConfig':config,'startUnix':time.time()};print('Running',name,flush=True)
 with (d/'launcher.log').open('w') as f:rec['exitCode']=subprocess.call(command,env=env,stdout=f,stderr=subprocess.STDOUT)
 rec['endUnix']=time.time();(d/'command.json').write_text(json.dumps(rec,indent=2));target=base/'QA'/run_id/name;shutil.copytree(d,target)
 if rec['exitCode'] or not (d/'results.xml').exists():raise SystemExit('Verification failed: '+name)
 r=ET.parse(d/'results.xml').getroot().attrib;print(name,{k:r.get(k) for k in ['result','total','passed','failed','skipped']},flush=True)
 if r.get('result')!='Passed' or int(r.get('total','0'))==0:raise SystemExit('Tests not passed: '+name)
