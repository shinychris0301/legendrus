"""Install only the precise new settings through owning APIs; no art reimport/rebuild."""
from pathlib import Path
import json,os,shutil,subprocess,time
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';LOCAL=Path('/tmp/lor-core-outcome-readability-r1/Install');LOCAL.mkdir(exist_ok=False)
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/CoreOutcomeReadabilityPrefabAudit.cs';assert not helper.exists();shutil.copy2(BASE/'Sources'/helper.name,helper)
command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-nographics','-projectPath',str(ROOT),'-executeMethod','CoreOutcomeReadabilityPrefabAudit.Apply','-quit','-logFile',str(LOCAL/'unity.log')]
start=time.time()
try:
 with (LOCAL/'launcher.log').open('w') as out:code=subprocess.call(command,stdout=out,stderr=subprocess.STDOUT)
 (QA/'InstallSummary.json').write_text(json.dumps({'exitCode':code,'command':command,'seconds':time.time()-start},indent=2)+'\n');assert code==0 and (QA/'PrefabInstallation.json').exists()
 print('Three active prefabs updated through precise owning helpers.',flush=True)
finally:
 assert helper.read_bytes()==(BASE/'Sources'/helper.name).read_bytes();helper.unlink()
 if helper.with_suffix('.cs.meta').exists():helper.with_suffix('.cs.meta').unlink()
