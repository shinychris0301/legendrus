from pathlib import Path
import os, subprocess, resource, json, sys
base=Path(__file__).resolve().parent.parent
root=Path(__file__).resolve().parents[5]
name=sys.argv[1] if len(sys.argv)>1 else 'Build'
out=base/'QA'/name;out.mkdir(parents=True,exist_ok=False)
config='/tmp/lor-prelevel-pause-r1/'+name+'/config'
lic=Path(config)/'unity3d/Unity';lic.mkdir(parents=True);(lic/'licenses').symlink_to('/home/k2/.config/unity3d/Unity/licenses',target_is_directory=True)
env=os.environ.copy();env.update(XDG_CONFIG_HOME=config,LOR_LOADING_TEST_CONFIG_ROOT=config)
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-quit','-projectPath',str(root),'-executeMethod',sys.argv[2],'-logFile',str(out/'unity.log'),'-force-vulkan']
with (out/'launcher.log').open('w') as log:code=subprocess.call(cmd,env=env,stdout=log,stderr=subprocess.STDOUT,preexec_fn=limits)
(out/'result.json').write_text(json.dumps({'exitCode':code,'command':cmd,'processOnlyVulkan':True},indent=2))
print('Build exit:',code);raise SystemExit(code)
