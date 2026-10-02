from pathlib import Path
import json, os, resource, subprocess, sys, time
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
name, mode=sys.argv[1:3]
assert name.isalnum() and mode in ('Install','EditMode','PlayMode')
out=BASE/'QA'/name;out.mkdir(exist_ok=False)
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
config=Path('/tmp/lor-gameplay-large-stock-r5')/name/'config'
license_parent=config/'unity3d/Unity';license_parent.mkdir(parents=True)
(license_parent/'licenses').symlink_to('/home/k2/.config/unity3d/Unity/licenses',target_is_directory=True)
env={k:v for k,v in os.environ.items() if not k.startswith('LOR_')}
env.update(XDG_CONFIG_HOME=str(config),LOR_LOADING_TEST_CONFIG_ROOT=str(config),LOR_CORE_OUTCOME_READABILITY_DIR=str(out))
if mode=='PlayMode':
    for page in ('GAMEPLAY','HOME','FAIL','SPLASH','LOADING','VICTORY','CHAPTER_COMPLETE','PRELEVEL','BOOSTER_SELECTION','CAMPAIGN_UI'):
        env['LOR_'+page+'_CAPTURE_DIR']=str(out/page)
helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/InstallStockPrefab.cs'
command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(ROOT),'-logFile',str(out/'unity.log')]
if mode=='Install':
    assert not helper.exists() and not Path(str(helper)+'.meta').exists()
    helper.write_bytes((BASE/'Sources/InstallStockPrefab.cs').read_bytes())
    command+=['-executeMethod','InstallStockPrefab.Apply','-quit','-nographics']
else:
    command+=['-runTests','-testPlatform',mode,'-testFilter',sys.argv[3],'-testResults',str(out/'results.xml'),'-force-vulkan' if mode=='PlayMode' else '-nographics']
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
start=time.time();cleaned=[]
try:
    with (out/'launcher.log').open('w') as stream:
        code=subprocess.call(command,env=env,stdout=stream,stderr=subprocess.STDOUT,preexec_fn=limits)
    record={'exitCode':code,'durationSeconds':time.time()-start,'command':command,'isolatedConfig':str(config),'mode':mode}
    if (out/'results.xml').exists():
        tree=ET.parse(out/'results.xml');record['result']=tree.getroot().attrib
        record['nonPassing']=[{'name':t.get('fullname'),'result':t.get('result'),'message':t.findtext('.//message')} for t in tree.findall('.//test-case') if t.get('result')!='Passed']
finally:
    if mode=='Install':
        for p in (helper,Path(str(helper)+'.meta')):
            if p.exists():p.unlink();cleaned.append(str(p.relative_to(ROOT)))
    before=json.loads((BASE/'Sources/BeforeHashes.json').read_text())
    for p in (ROOT/'Assets').glob('InitTestScene*'):
        if str(p.relative_to(ROOT)) not in before and p.name.endswith(('.unity','.unity.meta')):
            p.unlink();cleaned.append(str(p.relative_to(ROOT)))
    (out/'TemporaryCleanup.json').write_text(json.dumps({'removed':cleaned},indent=2)+'\n')
(out/'summary.json').write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps({k:v for k,v in record.items() if k!='command'},indent=2),flush=True)
assert code==0
if mode!='Install':assert record.get('result',{}).get('result')=='Passed' and int(record['result']['passed'])>0
