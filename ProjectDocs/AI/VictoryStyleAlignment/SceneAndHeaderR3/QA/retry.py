from pathlib import Path
import os,subprocess,time,json
root=Path.cwd();cache=Path('/tmp/lor-victory-r3');out=cache/'countdown_retry';out.mkdir(exist_ok=True);config=str(cache/'test-config');env=os.environ.copy();env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config,LOR_VICTORY_CAPTURE_DIR=str(out/'Victory'))
cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests','-testPlatform','PlayMode','-testFilter','LegendsOfRus.Tests.PlayMode.VictoryAlignmentPlayModeTests.HeaderRefillTimeReadsExistingEnergyClockWithoutChangingTheSave','-logFile',str(out/'unity.log'),'-testResults',str(out/'results.xml')]
record={'command':cmd,'environmentOverrides':{k:env[k] for k in ['XDG_CONFIG_HOME','LOR_LOADING_TEST_CONFIG_ROOT','LOR_VICTORY_CAPTURE_DIR']},'startUnix':time.time()}
with (out/'launcher.log').open('w') as f: record['exitCode']=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
record['endUnix']=time.time();(out/'command.json').write_text(json.dumps(record,indent=2)+'\n');print(record['exitCode'],flush=True)
