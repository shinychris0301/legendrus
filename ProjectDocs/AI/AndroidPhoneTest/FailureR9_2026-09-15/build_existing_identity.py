from pathlib import Path
import os,subprocess,json,time
r=Path.cwd();o=Path('/tmp/lor-fail-r9-android/run2/Build');o.mkdir(parents=True,exist_ok=False);env=os.environ.copy();env.pop('XDG_CONFIG_HOME',None)
cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-nographics','-projectPath',str(r),'-executeMethod','LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand.BuildDevelopmentApk','-quit','-logFile',str(o/'unity.log')]
rec={'command':cmd,'environmentOverrides':{'XDG_CONFIG_HOME':'unset; use existing default local debug identity'},'startUnix':time.time()}
with (o/'launcher.log').open('w') as f:rec['exitCode']=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
rec['endUnix']=time.time();(o/'command.json').write_text(json.dumps(rec,indent=2)+'\n');print('Build',rec['exitCode'],flush=True);raise SystemExit(rec['exitCode'])
