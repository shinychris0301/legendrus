from pathlib import Path
import json,os,subprocess
root=Path.cwd();out=Path('/tmp/lor-home-reward-r4/Verify01');config=str(out/'test-config');over={'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config,'LOR_HOME_CAPTURE_DIR':str(out/'Home'),'LOR_SPLASH_CAPTURE_DIR':str(out/'Splash'),'LOR_LOADING_CAPTURE_DIR':str(out/'Loading')};env=os.environ.copy();env.update(over)
for phase,args in [('EditMode',['-testPlatform','EditMode','-testFilter','LegendsOfRus.Tests.ProductionCoreLoopUi.HomeAlignmentDeliveryEditModeTests']),('PlayMode',['-testPlatform','PlayMode','-testFilter','LegendsOfRus.Tests.PlayMode.HomeAlignmentPlayModeTests'])]:
 d=out/phase;d.mkdir(parents=True,exist_ok=True);cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests',*args,'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
 with (d/'launcher.log').open('w') as f:code=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
 (d/'Command.json').write_text(json.dumps({'command':cmd,'environmentOverrides':over,'exitCode':code},indent=2));print(phase,code,flush=True)
