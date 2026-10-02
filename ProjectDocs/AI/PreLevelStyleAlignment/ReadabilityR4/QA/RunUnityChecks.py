from pathlib import Path
import subprocess,os,json,time,sys
root=Path.cwd();base=Path('/home/k2/.cache/lor-prelevel-r4');out=base/sys.argv[1]
env=os.environ.copy();config=str(base/'test-config');env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config,LOR_PRELEVEL_CAPTURE_DIR=str(out/'PreLevel'))
steps=[('ImportPreLevel',['-executeMethod','LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder.RebuildPreLevelAlignment','-quit']),('EditMode',['-runTests','-testPlatform','EditMode','-testFilter','LegendsOfRus.Tests.ProductionCoreLoopUi.PreLevelAlignmentEditModeTests']),('PlayMode',['-runTests','-testPlatform','PlayMode','-testFilter','LegendsOfRus.Tests.PlayMode.PreLevelAlignmentPlayModeTests'])]
for name,args in steps:
 folder=out/name;folder.mkdir(parents=True,exist_ok=True)
 cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root)]+args+['-logFile',str(folder/'unity.log')]
 if name in ['EditMode','PlayMode']:cmd+=['-testResults',str(folder/'results.xml')]
 record={'command':cmd,'environmentOverrides':{k:env[k] for k in ['XDG_CONFIG_HOME','LOR_LOADING_TEST_CONFIG_ROOT','LOR_PRELEVEL_CAPTURE_DIR']},'startUnix':time.time()}
 with (folder/'launcher.log').open('w') as f:record['exitCode']=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
 record['endUnix']=time.time();(folder/'command.json').write_text(json.dumps(record,indent=2)+'\n');print(name,record['exitCode'],flush=True)
 if record['exitCode']:sys.exit(record['exitCode'])
