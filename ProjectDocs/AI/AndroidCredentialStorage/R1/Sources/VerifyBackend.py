from pathlib import Path
import json, os, subprocess, sys, re, time
root=Path(__file__).resolve().parents[5]
base=Path(__file__).resolve().parent.parent
name=sys.argv[1]
env=os.environ.copy();env['PYTHONDONTWRITEBYTECODE']='1'
start=time.time()
run=subprocess.run([sys.executable,'-m','unittest','discover','-s','tests','-v'],cwd=root/'Backend/LegendsOfRusService',env=env,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
(base/'QA'/(name+'.log')).write_text(run.stdout)
match=re.search(r'Ran (\d+) tests',run.stdout)
count=int(match.group(1)) if match else 0
record={'command':'python3 -m unittest discover -s tests -v','cwd':'Backend/LegendsOfRusService','exitCode':run.returncode,'tests':count,'passed':count if run.returncode==0 else None,'durationSeconds':time.time()-start,'liveRuStoreTest':False}
(base/'QA'/(name+'.json')).write_text(json.dumps(record,indent=2))
print(json.dumps(record,indent=2))
if run.returncode:print(run.stdout)
if run.returncode or not count:sys.exit(1)
