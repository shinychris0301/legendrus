from pathlib import Path
import json,os,subprocess,time
root=Path.cwd();base=Path('/tmp/lor-fail-r9-android/run1');base.mkdir(exist_ok=False)
overrides={'XDG_CONFIG_HOME':str(base/'config'),'LOR_LOADING_TEST_CONFIG_ROOT':str(base/'config')};env=os.environ.copy();env.update(overrides)
for name,args in [('GuardTests',['-runTests','-testPlatform','EditMode','-assemblyNames','LegendsOfRus.AndroidBuild.Editor.Tests','-testResults',str(base/'GuardTests/results.xml')]),('Build',['-executeMethod','LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk','-quit'])]:
 folder=base/name;folder.mkdir();cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-nographics','-projectPath',str(root)]+args+['-logFile',str(folder/'unity.log')];rec={'command':cmd,'environmentOverrides':overrides,'startUnix':time.time()}
 with (folder/'launcher.log').open('w') as f:rec['exitCode']=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
 rec['endUnix']=time.time();(folder/'command.json').write_text(json.dumps(rec,indent=2)+'\n');print(name,rec['exitCode'],flush=True)
 if rec['exitCode']:raise SystemExit(rec['exitCode'])
